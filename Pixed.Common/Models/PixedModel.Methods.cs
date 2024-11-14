using Pixed.Core.Models;
using System;
using System.IO;

namespace Pixed.Common.Models;
public static class PixedModelMethods
{
    public static void Undo(this PixedModel model)
    {
        model.HistoryIndex--;
        if (model.History.Count == 1 || model.HistoryIndex < 0)
        {
            return;
        }

        model.HistoryIndex = Math.Clamp(model.HistoryIndex, 0, model.History.Count - 1);

        byte[] data = model.History[model.HistoryIndex];
        MemoryStream stream = new(data);
        model.Deserialize(stream);
        Subjects.ProjectModified.OnNext(model);
        Subjects.FrameChanged.OnNext(model.CurrentFrame);
        Subjects.LayerChanged.OnNext(model.CurrentFrame.CurrentLayer);
        stream.Dispose();
    }

    public static void Redo(this PixedModel model)
    {
        model.HistoryIndex++;
        if (model.History.Count == 0 || model.HistoryIndex >= model.History.Count)
        {
            return;
        }

        if (model.HistoryIndex < 0)
        {
            model.HistoryIndex = 0;
        }

        byte[] data = model.History[model.HistoryIndex];
        MemoryStream stream = new(data);
        model.Deserialize(stream);
        Subjects.ProjectModified.OnNext(model);
        Subjects.FrameChanged.OnNext(model.CurrentFrame);
        Subjects.LayerChanged.OnNext(model.CurrentFrame.CurrentLayer);
        stream.Dispose();
    }

    public static void Process(this PixedModel model, bool allFrames, bool allLayers, Action<Frame, Layer> action, ApplicationData applicationData, bool executeSubjects = true)
    {
        Frame[] frames = allFrames ? [.. model.Frames] : [applicationData.CurrentFrame];

        foreach (Frame frame in frames)
        {
            Layer[] layers = allLayers ? [.. frame.Layers] : [frame.CurrentLayer];

            foreach (Layer layer in layers)
            {
                action.Invoke(frame, layer);
            }

            if (executeSubjects)
            {
                Subjects.FrameModified.OnNext(frame);
            }
        }
    }
}
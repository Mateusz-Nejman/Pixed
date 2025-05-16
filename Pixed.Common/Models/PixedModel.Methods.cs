using Pixed.Common.Services;
using Pixed.Core.Models;
using System;
using System.Threading.Tasks;

namespace Pixed.Common.Models;
public static class PixedModelMethods
{
    public static async Task Undo(this PixedModel model, IHistoryService historyService)
    {
        var historyCount = historyService.GetHistoryCount(model);
        var index = historyService.GetHistoryIndex(model);
        index--;
        if (historyCount == 1 || index < 0)
        {
            return;
        }

        index = Math.Clamp(index, 0, historyCount - 1);
        historyService.SetHistoryIndex(model, index);
        var id = historyService.GetHistoryId(model, index);

        var stream = await historyService.GetHistoryItem(model, id);
        model.Deserialize(stream);
        model.ResetRecursive();
        Subjects.ProjectModified.OnNext(model);
        Subjects.FrameChanged.OnNext(model.CurrentFrame);
        Subjects.LayerChanged.OnNext(model.CurrentFrame.CurrentLayer);
        stream.Dispose();
    }

    public static async Task Redo(this PixedModel model, IHistoryService historyService)
    {
        var historyCount = historyService.GetHistoryCount(model);
        var index = historyService.GetHistoryIndex(model);
        index++;
        if (historyCount == 0 || index >= historyCount)
        {
            return;
        }

        if (index < 0)
        {
            index = 0;
        }

        historyService.SetHistoryIndex(model, index);
        var id = historyService.GetHistoryId(model, index);

        var stream = await historyService.GetHistoryItem(model, id);

        model.Deserialize(stream);
        model.ResetRecursive();
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
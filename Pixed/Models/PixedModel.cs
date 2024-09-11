using Pixed.Services.History;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Pixed.Models
{
    internal class PixedModel
    {
        private readonly ObservableCollection<Frame> _frames;
        private ObservableCollection<HistoryEntry> _history;
        private int _historyIndex = -1;

        public ObservableCollection<Frame> Frames => _frames;
        public int Width => Frames[0].Width;
        public int Height => Frames[0].Height;

        public PixedModel()
        {
            _frames = [];
            _history = [];
        }

        public List<int> GetAllColors()
        {
            return _frames.SelectMany(f => f.Layers).Select(l => l.GetPixels()).SelectMany(p => p).Where(p => p != UniColor.Transparent).Distinct().Order().ToList();
        }

        public static PixedModel FromFrames(ObservableCollection<Frame> frames)
        {
            PixedModel model = new PixedModel();

            foreach (var frame in frames)
            {
                model.Frames.Add(frame);
            }

            return model;
        }

        public void AddHistory(HistoryEntry entry)
        {
            _history.Add(entry);
            _historyIndex = _history.Count - 1;
        }

        public void Undo()
        {
            if (_history.Count == 0 || _historyIndex < 0)
            {
                return;
            }

            HistoryEntry currentEntry = _history[_historyIndex];
            Frame? frame = Frames.FirstOrDefault(f => f.Id == currentEntry.FrameId, null);

            if (frame == null)
            {
                _history.RemoveAt(_historyIndex);
                _historyIndex--;
                Undo();
                return;
            }

            Layer? layer = frame.Layers.FirstOrDefault(l => l.Id == currentEntry.LayerId, null);

            if ((layer == null))
            {
                _history.RemoveAt(_historyIndex);
                _historyIndex--;
                Undo();
                return;
            }

            for (int a = 0; a < currentEntry.PixelX.Length; a++)
            {
                int pixelX = currentEntry.PixelX[a];
                int pixelY = currentEntry.PixelY[a];
                int oldcolor = currentEntry.OldColor[a];
                int newColor = currentEntry.NewColor[a];

                if (layer.GetPixel(pixelX, pixelY) == newColor)
                {
                    layer.SetPixel(pixelX, pixelY, oldcolor);
                }
            }

            _historyIndex--;
            Subjects.RefreshCanvas.OnNext(true);
        }

        public void Redo()
        {
            if (_history.Count == 0 || _historyIndex >= _history.Count)
            {
                return;
            }

            if (_historyIndex < 0)
            {
                _historyIndex = 0;
            }

            HistoryEntry currentEntry = _history[_historyIndex];
            Frame? frame = Frames.FirstOrDefault(f => f.Id == currentEntry.FrameId, null);

            if (frame == null)
            {
                _history.RemoveAt(_historyIndex);
                Redo();
                return;
            }

            Layer? layer = frame.Layers.FirstOrDefault(l => l.Id == currentEntry.LayerId, null);

            if ((layer == null))
            {
                _history.RemoveAt(_historyIndex);
                Redo();
                return;
            }

            for (int a = 0; a < currentEntry.PixelX.Length; a++)
            {
                int pixelX = currentEntry.PixelX[a];
                int pixelY = currentEntry.PixelY[a];
                int oldcolor = currentEntry.OldColor[a];
                int newColor = currentEntry.NewColor[a];

                if (layer.GetPixel(pixelX, pixelY) == oldcolor)
                {
                    layer.SetPixel(pixelX, pixelY, newColor);
                }
            }

            _historyIndex++;
            Subjects.RefreshCanvas.OnNext(true);
        }
    }
}

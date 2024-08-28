using Pixed.Models;
using Pixed.Selection;
using Pixed.Utils;
using System.Drawing;

namespace Pixed.Tools.Selection
{
    internal class LassoSelect : AbstractDragSelect
    {
        private int _prevX;
        private int _prevY;
        private List<Point> _points = [];
        public override void OnDragSelectStart(int x, int y, Frame frame, ref Bitmap overlay)
        {
            _points.Clear();
            _points.Add(new Point(x, y));

            _startX = x;
            _startY = y;

            _prevX = x;
            _prevY = y;
        }

        public override void OnDragSelect(int x, int y, Frame frame, ref Bitmap overlay)
        {
            AddPixel(x, y, frame);
            List<Pixel> pixels = new List<Pixel>();

            foreach (var point in GetLassoPixels())
            {
                pixels.Add(new Pixel(point.X, point.Y, frame.GetPixel(point.X, point.Y)));
            }
            BaseSelection selection = new ShapeSelection(pixels);
            SetSelection(selection, ref overlay);
        }

        public override void OnDragSelectEnd(int x, int y, Frame frame, ref Bitmap overlay)
        {
            AddPixel(x, y, frame);
            BaseSelection selection = new LassoSelection(GetLassoPixels(), frame);
            SetSelection(selection, ref overlay);
        }

        private List<Point> GetLassoPixels()
        {
            var line = MathUtils.GetBresenhamLine(_prevX, _prevY, _startX, _startY);
            return _points.Concat(line).ToList();
        }

        private void AddPixel(int x, int y, Frame frame)
        {
            x = Math.Clamp(x, 0, frame.Width - 1);
            y = Math.Clamp(y, 0, frame.Height - 1);

            var interpolated = MathUtils.GetBresenhamLine(x, y, _prevX, _prevY);
            _points = _points.Concat(interpolated).ToList();

            _prevX = x;
            _prevY = y;
        }

        private void SetSelection(BaseSelection selection, ref Bitmap overlay)
        {
            _selection = selection;
            overlay.Clear();
            Subjects.SelectionCreated.OnNext(selection);
            DrawSelectionOnOverlay(ref overlay);
        }
    }
}

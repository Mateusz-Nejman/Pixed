namespace Pixed.Selection
{
    internal class RectangularSelection : BaseSelection
    {
        private int _x1;
        private int _y1;
        private int _x2;
        private int _y2;

        public RectangularSelection(int x1, int y1, int x2, int y2) : base()
        {
            SetOrderedRectangleCoordinates(x1, y1, x2, y2);

            for(int x = _x1; x <= _x2; x++)
            {
                for(int y = _y1; y <= _y2; y++)
                {
                    Pixels.Add(new Models.Pixel(x, y));
                }
            }
        }

        private void SetOrderedRectangleCoordinates(int x1, int y1,int x2,int y2)
        {
            _x1 = Math.Min(x1, x2);
            _y1 = Math.Min(y1, y2);
            _x2 = Math.Max(x2, x1);
            _y2 = Math.Max(y2, y1);
        }
    }
}

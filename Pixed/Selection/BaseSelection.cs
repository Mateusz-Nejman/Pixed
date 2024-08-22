using Pixed.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Selection
{
    internal class BaseSelection
    {
        public List<Pixel> Pixels { get; }

        public BaseSelection()
        {
            Pixels = [];
            Reset();
        }

        public void Reset()
        {
            Pixels.Clear();
        }

        public void Move(int xDiff, int yDiff)
        {
            for(int i = 0;  i < Pixels.Count; i++)
            {
                var pixel = Pixels[i];
                pixel.X += xDiff;
                pixel.Y += yDiff;
                Pixels[i] = pixel;
            }
        }

        public void FillSelectionFromFrame(Frame frame)
        {
            for (int i = 0; i < Pixels.Count; i++)
            {
                var pixel = Pixels[i];

                if (!frame.PointInside(pixel.X, pixel.Y))
                {
                    continue;
                }

                var color = frame.GetPixel(Pixels[i].X, Pixels[i].Y);
                pixel.Color = Color.FromArgb(128, Color.FromArgb(color)).ToArgb();
            }
        }
    }
}

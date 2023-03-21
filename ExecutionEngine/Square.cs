using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionEngine
{
    internal class Square
    {
        internal double CenterX { get; }
        internal double CenterY { get; }
        internal double Width { get; }
        internal double Height { get; }

        internal Square(double centerX, double centerY, double width, double height)
        {
            CenterX = centerX;
            CenterY = centerY;
            Width = width;
            Height = height;
        }

        // Calculate the area of the square
        internal double Area()
        {
            return Width * Height;
        }

        // Calculate the intersection of two squares
        internal double Intersection(Square s)
        {
            double x1 = CenterX - Width / 2; // Calculate the left edge of the square
            double x2 = s.CenterX - s.Width / 2; // Calculate the left edge of the square
            double y1 = CenterY - Height / 2; // Calculate the top edge of the square
            double y2 = s.CenterY - s.Height / 2; // Calculate the top edge of the square

            double xOverlap = Math.Max(0, Math.Min(x1 + Width, x2 + s.Width) - Math.Max(x1, x2));
            double yOverlap = Math.Max(0, Math.Min(y1 + Height, y2 + s.Height) - Math.Max(y1, y2));

            return xOverlap * yOverlap;
        }

        // Calculate the IoU of two squares
        internal double IoU(Square s)
        {
            double intersection = Intersection(s);
            double union = Area() + s.Area() - intersection;

            return intersection / union;
        }
    }
}

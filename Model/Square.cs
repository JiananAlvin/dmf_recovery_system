using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Test")]

namespace Model;

internal class Square
{
    internal double Ltx { get; }
    internal double Lty { get; }
    internal double Width { get; }
    internal double Height { get; }

    internal Square(double ltx, double lty, double width, double height)
    {
        Ltx = ltx;
        Lty = lty;
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
        double xOverlap = Math.Max(0, Math.Min(Ltx + Width, s.Ltx + s.Width) - Math.Max(Ltx, s.Ltx));
        double yOverlap = Math.Max(0, Math.Min(Lty + Height, s.Lty + s.Height) - Math.Max(Lty, s.Lty));

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

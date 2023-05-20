using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UnitTest")]
namespace ExecutionEngine
{
    public class Triangle
    {
        private Point VertexA;
        private Point VertexB;
        private Point VertexC;

        public Triangle(Point a, Point b, Point c)
        {
            this.VertexA = a;
            this.VertexB = b;
            this.VertexC = c;
        }

        public bool IsPointInTriangle(Point p)
        {
            Vector2 PA = new Vector2(this.VertexA.X - p.X, this.VertexA.Y - p.Y);
            Vector2 PB = new Vector2(this.VertexB.X - p.X, this.VertexB.Y - p.Y);
            Vector2 PC = new Vector2(this.VertexC.X - p.X, this.VertexC.Y - p.Y);

            // Calculate the cross products
            float t1 = PA.X * PB.Y - PA.Y * PB.X;  // PA x PB
            float t2 = PB.X * PC.Y - PB.Y * PC.X;  // PB x PC
            float t3 = PC.X * PA.Y - PC.Y * PA.X;  // PC x PA
            return t1 * t2 >= 0 && t1 * t3 >= 0;
        }
    }
}

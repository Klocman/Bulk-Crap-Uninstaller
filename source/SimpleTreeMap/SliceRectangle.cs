using System.Drawing;

namespace SimpleTreeMap
{
    public class SliceRectangle<T>
    {
        public int Height { get; set; }

        public Slice<T> Slice { get; set; }

        public int Width { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
        public Rectangle PaintRect { get; set; }

        public bool Contains(Point p)
        {
            return p.X >= X && p.X <= X + Width && p.Y >= Y && p.Y <= Y + Height;
        }
    }
}
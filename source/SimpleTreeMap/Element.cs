using System.Drawing;

namespace SimpleTreeMap
{
    public class Element<T>
    {
        public T Object { get; set; }

        public double Value { get; set; }

        public Color Color { get; set; }
        public string Text { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SimpleTreeMap
{
    /// <summary>
    /// Based on https://pascallaurin42.blogspot.com/2013/12/implementing-treemap-in-c.html
    /// </summary>
    public class Class1
    {
        public void DrawTreemap<T>(IEnumerable<SliceRectangle<T>> rectangles, int width, int height)
        {
            var font = new Font("Arial", 8);

            var bmp = new Bitmap(width, height);
            var gfx = Graphics.FromImage(bmp);

            gfx.FillRectangle(Brushes.Blue, new RectangleF(0, 0, width, height));

            foreach (var r in rectangles)
            {
                gfx.DrawRectangle(Pens.Black,
                    new Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1));

                gfx.DrawString(r.Slice.Elements.First().Object.ToString(), font,
                    Brushes.White, r.X, r.Y);
            }

            var form = new Form { AutoSize = true };
            form.Controls.Add(new PictureBox { Width = width, Height = height, Image = bmp });
            form.ShowDialog();
        }

        private SliceResult<T> GetElementsForSlice<T>(ICollection<Element<T>> elements, double sliceWidth)
        {
            var elementsInSlice = new List<Element<T>>();
            var remainingElements = new List<Element<T>>();
            var current = 0d;
            var total = elements.Sum(x => x.Value);

            foreach (var element in elements)
            {
                if (current > sliceWidth)
                    remainingElements.Add(element);
                else
                {
                    elementsInSlice.Add(element);
                    current += element.Value / total;
                }
            }

            return new SliceResult<T>
            {
                Elements = elementsInSlice,
                ElementsSize = current,
                RemainingElements = remainingElements
            };
        }

        public IEnumerable<SliceRectangle<T>> GetRectangles<T>(Slice<T> slice, int width, int height)
        {
            var area = new SliceRectangle<T>
            { Slice = slice, Width = width, Height = height };

            foreach (var rect in GetRectangles(area))
            {
                // Make sure no rectangle go outside the original area
                if (rect.X + rect.Width > area.Width) rect.Width = area.Width - rect.X;
                if (rect.Y + rect.Height > area.Height) rect.Height = area.Height - rect.Y;

                yield return rect;
            }
        }

        private IEnumerable<SliceRectangle<T>> GetRectangles<T>(SliceRectangle<T> sliceRectangle)
        {
            var isHorizontalSplit = sliceRectangle.Width >= sliceRectangle.Height;
            var currentPos = 0;
            foreach (var subSlice in sliceRectangle.Slice.SubSlices)
            {
                var subRect = new SliceRectangle<T> { Slice = subSlice };
                int rectSize;

                if (isHorizontalSplit)
                {
                    rectSize = (int)Math.Round(sliceRectangle.Width * subSlice.Size);
                    subRect.X = sliceRectangle.X + currentPos;
                    subRect.Y = sliceRectangle.Y;
                    subRect.Width = rectSize;
                    subRect.Height = sliceRectangle.Height;
                }
                else
                {
                    rectSize = (int)Math.Round(sliceRectangle.Height * subSlice.Size);
                    subRect.X = sliceRectangle.X;
                    subRect.Y = sliceRectangle.Y + currentPos;
                    subRect.Width = sliceRectangle.Width;
                    subRect.Height = rectSize;
                }

                currentPos += rectSize;

                if (subSlice.Elements.Count > 1)
                {
                    foreach (var sr in GetRectangles(subRect))
                        yield return sr;
                }
                else if (subSlice.Elements.Count == 1)
                    yield return subRect;
            }
        }

        public Slice<T> GetSlice<T>(ICollection<Element<T>> elements, double totalSize,
            double sliceWidth)
        {
            if (!elements.Any()) return null;
            if (elements.Count == 1)
                return new Slice<T>
                { Elements = elements, Size = totalSize };

            var sliceResult = GetElementsForSlice(elements, sliceWidth);

            return new Slice<T>
            {
                Elements = elements,
                Size = totalSize,
                SubSlices = new[]
                {
                    GetSlice(sliceResult.Elements, sliceResult.ElementsSize, sliceWidth),
                    GetSlice(sliceResult.RemainingElements, 1 - sliceResult.ElementsSize,
                        sliceWidth)
                }
            };
        }

        private void Main()
        {
            const int width = 400;
            const int height = 300;
            const double minSliceRatio = 0.35;

            var elements = new[] { 24, 45, 32, 87, 34, 58, 10, 4, 5, 9, 52, 34 }
                .Select(x => new Element<string> { Object = x.ToString(), Value = x })
                .OrderByDescending(x => x.Value)
                .ToList();

            var slice = GetSlice(elements, 1, minSliceRatio);

            var rectangles = GetRectangles(slice, width, height)
                .ToList();

            DrawTreemap(rectangles, width, height);
        }
    }

    public class Element<T>
    {
        public T Object { get; set; }

        public double Value { get; set; }
    }

    public class Slice<T>
    {
        public ICollection<Element<T>> Elements { get; set; }

        public double Size { get; set; }

        public ICollection<Slice<T>> SubSlices { get; set; }
    }

    public class SliceResult<T>
    {
        public ICollection<Element<T>> Elements { get; set; }

        public double ElementsSize { get; set; }

        public ICollection<Element<T>> RemainingElements { get; set; }
    }

    public class SliceRectangle<T>
    {
        public int Height { get; set; }

        public Slice<T> Slice { get; set; }

        public int Width { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}
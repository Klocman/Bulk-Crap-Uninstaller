using System.Collections.Generic;

namespace SimpleTreeMap
{
    public class Slice<T>
    {
        public ICollection<Element<T>> Elements { get; set; }

        public double Size { get; set; }

        public ICollection<Slice<T>> SubSlices { get; set; }
    }
}
using System.Collections.Generic;

namespace SimpleTreeMap
{
    public class SliceResult<T>
    {
        public ICollection<Element<T>> Elements { get; set; }

        public double ElementsSize { get; set; }

        public ICollection<Element<T>> RemainingElements { get; set; }
    }
}
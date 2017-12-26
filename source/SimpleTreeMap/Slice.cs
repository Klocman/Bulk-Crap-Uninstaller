using System.Collections.Generic;
using System.Linq;

namespace SimpleTreeMap
{
    public class Slice<T>
    {
        public ICollection<Element<T>> Elements { get; set; }

        public double Size { get; set; }

        public ICollection<Slice<T>> SubSlices { get; set; }
        
        public string ToElementNames()
        {
            return string.Join("\n", Elements.Select(x => x.Text).ToArray());
        }
    }
}
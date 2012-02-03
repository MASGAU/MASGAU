using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU.Comparers
{
    public abstract class ReversableComparer<T>: IComparer<T>
    {
        protected Boolean reverse;
        protected ReversableComparer(Boolean reverse) {
            this.reverse = reverse;
        }

        public abstract int Compare(T one, T two);

        protected int ReversableCompare(IComparable one, IComparable two) {
            if(reverse)
                return two.CompareTo(one);
            else
                return one.CompareTo(two);
        }
    }
}

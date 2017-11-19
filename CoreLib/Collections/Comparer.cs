using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Collections
{
    public class Comparer<T> : IComparer<T>
    {
        private Func<T, T, int> _comparer;
        public Comparer(Func<T, T, int> comparer)
        {
            this._comparer = comparer;
        }
        public static IComparer<T> Create(Func<T, T, int> comparer)
        {
            return new Comparer<T>(comparer);
        }
        public int Compare(T x, T y)
        {
            return _comparer(x, y);
        }
    }
}

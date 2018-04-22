using System.Collections.Generic;

namespace Kotas.Utils.Common.Comparers
{
    public class IntArrayComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] i1, int[] i2)
        {
            if (ReferenceEquals(i1, i2))
            {
                return true;
            }
            if (i1 == null || i2 == null)
            {
                return false;
            }
            if (i1.Length != i2.Length)
            {
                return false;
            }

            for (int i = 0; i < i1.Length; ++i)
            {
                if (i1[i] != i2[i]) return false;
            }

            return true;
        }

        public int GetHashCode(int[] obj)
        {
            if (obj == null)
                return 0;
            var hc = obj.Length;
            foreach (int t in obj)
            {
                hc = unchecked(hc * 314159 + t);
            }
            return hc;
        }
    }
}

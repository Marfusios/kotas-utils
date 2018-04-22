using System.Collections.Generic;

namespace Kotas.Utils.Common.Comparers
{
    public class StringArrayComparer : IEqualityComparer<string[]>
    {
        public bool Equals(string[] i1, string[] i2)
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

        public int GetHashCode(string[] obj)
        {
            var hashcode = 0;
            if (obj == null)
                return hashcode;

            foreach (var value in obj)
            {
                if (value != null)
                    hashcode += value.GetHashCode();
            }
            return hashcode;
        }
    }
}

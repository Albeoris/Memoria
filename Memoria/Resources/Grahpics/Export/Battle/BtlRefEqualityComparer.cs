using System.Collections.Generic;

namespace Memoria
{
    internal sealed class BtlRefEqualityComparer : IEqualityComparer<BTL_REF>
    {
        public static readonly BtlRefEqualityComparer Instance = new BtlRefEqualityComparer();

        public bool Equals(BTL_REF x, BTL_REF y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.prog_no == y.prog_no && x.power == y.power && x.attr == y.attr && x.rate == y.rate;
        }

        public int GetHashCode(BTL_REF obj)
        {
            unchecked
            {
                var hashCode = obj.prog_no.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.power.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.attr.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.rate.GetHashCode();
                return hashCode;
            }
        }
    }
}
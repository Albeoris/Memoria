using System;
using System.Collections.Generic;

namespace Memoria.Assets
{
    internal sealed class BtlRefEqualityComparer : IEqualityComparer<BTL_REF>
    {
        public static readonly BtlRefEqualityComparer Instance = new BtlRefEqualityComparer();

        public Boolean Equals(BTL_REF x, BTL_REF y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.ScriptId == y.ScriptId && x.Power == y.Power && x.Elements == y.Elements && x.Rate == y.Rate;
        }

        public Int32 GetHashCode(BTL_REF obj)
        {
            unchecked
            {
                var hashCode = obj.ScriptId.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Power.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Elements.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Rate.GetHashCode();
                return hashCode;
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace Memoria.Assets
{
    internal sealed class CmdInfoEqualityComparer : IEqualityComparer<BattleCommandInfo>
    {
        public static readonly CmdInfoEqualityComparer Instance = new CmdInfoEqualityComparer();

        public Boolean Equals(BattleCommandInfo x, BattleCommandInfo y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Target == y.Target && x.DefaultAlly == y.DefaultAlly && x.DisplayStats == y.DisplayStats && x.VfxIndex == y.VfxIndex && x.ForDead == y.ForDead && x.DefaultCamera == y.DefaultCamera && x.DefaultOnDead == y.DefaultOnDead;
        }

        public Int32 GetHashCode(BattleCommandInfo obj)
        {
            unchecked
            {
                var hashCode = obj.Target.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.DefaultAlly.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.DisplayStats.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.VfxIndex.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.ForDead.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.DefaultCamera.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.DefaultOnDead.GetHashCode();
                return hashCode;
            }
        }
    }
}
using System;
using System.Collections.Generic;

namespace Memoria.Assets
{
    internal sealed class CmdInfoEqualityComparer : IEqualityComparer<CMD_INFO>
    {
        public static readonly CmdInfoEqualityComparer Instance = new CmdInfoEqualityComparer();

        public Boolean Equals(CMD_INFO x, CMD_INFO y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.cursor == y.cursor && x.def_cur == y.def_cur && x.sub_win == y.sub_win && x.vfx_no == y.vfx_no && x.sfx_no == y.sfx_no && x.dead == y.dead && x.def_cam == y.def_cam && x.def_dead == y.def_dead;
        }

        public Int32 GetHashCode(CMD_INFO obj)
        {
            unchecked
            {
                var hashCode = obj.cursor.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.def_cur.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.sub_win.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.vfx_no.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.sfx_no.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.dead.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.def_cam.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.def_dead.GetHashCode();
                return hashCode;
            }
        }
    }
}
using System;
using System.Collections.Generic;

namespace Memoria
{
    internal sealed class ActionComparer : IEqualityComparer<KeyValuePair<String, AA_DATA>>
    {
        public static readonly ActionComparer Instance = new ActionComparer();

        public static Int32 CalcDiff(KeyValuePair<String, AA_DATA> item, IEnumerable<KeyValuePair<String, AA_DATA>> other)
        {
            Int32 total = 0;
            String xn = item.Key;
            AA_DATA x = item.Value;
            foreach (KeyValuePair<String, AA_DATA> data in other)
            {
                AA_DATA y = data.Value;
                if (x == y)
                    continue;

                if (x.AddNo != y.AddNo)
                    total++;
                if (x.Category != y.Category)
                    total++;
                if (x.MP != y.MP)
                    total++;
                if (x.Type != y.Type)
                    total++;
                if (x.Vfx2 != y.Vfx2)
                    total++;
                if (x.Info.cursor != y.Info.cursor)
                    total++;
                if (x.Info.cursor != y.Info.cursor)
                    total++;
                if (x.Info.def_cur != y.Info.def_cur)
                    total++;
                if (x.Info.sub_win != y.Info.sub_win)
                    total++;
                if (x.Info.vfx_no != y.Info.vfx_no)
                    total++;
                if (x.Info.sfx_no != y.Info.sfx_no)
                    total++;
                if (x.Info.dead != y.Info.dead)
                    total++;
                if (x.Info.def_cam != y.Info.def_cam)
                    total++;
                if (x.Info.def_dead != y.Info.def_dead)
                    total++;
                if (x.Ref.attr != y.Ref.attr)
                    total++;
                if (x.Ref.prog_no != y.Ref.prog_no)
                    total++;
                if (x.Ref.power != y.Ref.power)
                    total++;
                if (x.Ref.rate != y.Ref.rate)
                    total++;
            }
            return total;
        }

        public Boolean Equals(KeyValuePair<String, AA_DATA> x, KeyValuePair<String, AA_DATA> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return Equals(x.Key, y.Key) &&
                CmdInfoEqualityComparer.Instance.Equals(x.Value.Info, y.Value.Info) && BtlRefEqualityComparer.Instance.Equals(x.Value.Ref, y.Value.Ref) && x.Value.Category == y.Value.Category && x.Value.AddNo == y.Value.AddNo && x.Value.MP == y.Value.MP && x.Value.Type == y.Value.Type && x.Value.Vfx2 == y.Value.Vfx2;
        }

        public Int32 GetHashCode(KeyValuePair<String, AA_DATA> obj)
        {
            unchecked
            {
                var hashCode = obj.Key.GetHashCode();
                hashCode = (hashCode * 397) ^ (obj.Value.Info == null ? 0 : CmdInfoEqualityComparer.Instance.GetHashCode(obj.Value.Info));
                hashCode = (hashCode * 397) ^ (obj.Value.Ref == null ? 0 : BtlRefEqualityComparer.Instance.GetHashCode(obj.Value.Ref));
                hashCode = (hashCode * 397) ^ obj.Value.Category.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Value.AddNo.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Value.MP.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Value.Type.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Value.Vfx2.GetHashCode();
                return hashCode;
            }
        }
    }
}
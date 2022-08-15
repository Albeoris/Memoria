using System;
using System.Collections.Generic;

namespace Memoria.Assets
{
    internal sealed class ActionComparer : IEqualityComparer<KeyValuePair<Dictionary<String, String>, AA_DATA>>
    {
        public static readonly ActionComparer Instance = new ActionComparer();

        public static Int32 CalcDiff(KeyValuePair<Dictionary<String, String>, AA_DATA> item, IEnumerable<KeyValuePair<Dictionary<String, String>, AA_DATA>> other)
        {
            Int32 total = 0;
            AA_DATA x = item.Value;
            foreach (KeyValuePair<Dictionary<String, String>, AA_DATA> data in other)
            {
                AA_DATA y = data.Value;
                if (x == y)
                    continue;

                if (x.AddStatusNo != y.AddStatusNo)
                    total++;
                if (x.Category != y.Category)
                    total++;
                if (x.MP != y.MP)
                    total++;
                if (x.Type != y.Type)
                    total++;
                if (x.Vfx2 != y.Vfx2)
                    total++;
                if (x.Info.Target != y.Info.Target)
                    total++;
                if (x.Info.Target != y.Info.Target)
                    total++;
                if (x.Info.DefaultAlly != y.Info.DefaultAlly)
                    total++;
                if (x.Info.DisplayStats != y.Info.DisplayStats)
                    total++;
                if (x.Info.VfxIndex != y.Info.VfxIndex)
                    total++;
                if (x.Info.ForDead != y.Info.ForDead)
                    total++;
                if (x.Info.DefaultCamera != y.Info.DefaultCamera)
                    total++;
                if (x.Info.DefaultOnDead != y.Info.DefaultOnDead)
                    total++;
                if (x.Ref.Elements != y.Ref.Elements)
                    total++;
                if (x.Ref.ScriptId != y.Ref.ScriptId)
                    total++;
                if (x.Ref.Power != y.Ref.Power)
                    total++;
                if (x.Ref.Rate != y.Ref.Rate)
                    total++;
            }
            return total;
        }

        public Boolean Equals(KeyValuePair<Dictionary<String, String>, AA_DATA> x, KeyValuePair<Dictionary<String, String>, AA_DATA> y)
        {
            return Equals(x.Key["US"], y.Key["US"]) &&
                CmdInfoEqualityComparer.Instance.Equals(x.Value.Info, y.Value.Info) && BtlRefEqualityComparer.Instance.Equals(x.Value.Ref, y.Value.Ref) && x.Value.Category == y.Value.Category && x.Value.AddStatusNo == y.Value.AddStatusNo && x.Value.MP == y.Value.MP && x.Value.Type == y.Value.Type && x.Value.Vfx2 == y.Value.Vfx2;
        }

        public Int32 GetHashCode(KeyValuePair<Dictionary<String, String>, AA_DATA> obj)
        {
            unchecked
            {
                var hashCode = obj.Key["US"].GetHashCode();
                hashCode = (hashCode * 397) ^ (obj.Value.Info == null ? 0 : CmdInfoEqualityComparer.Instance.GetHashCode(obj.Value.Info));
                hashCode = (hashCode * 397) ^ (obj.Value.Ref == null ? 0 : BtlRefEqualityComparer.Instance.GetHashCode(obj.Value.Ref));
                hashCode = (hashCode * 397) ^ obj.Value.Category.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Value.AddStatusNo.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Value.MP.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Value.Type.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Value.Vfx2.GetHashCode();
                return hashCode;
            }
        }
    }
}
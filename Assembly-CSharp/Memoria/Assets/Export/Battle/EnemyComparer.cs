using Memoria.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Memoria.Assets
{
    internal class EnemyComparer : IEqualityComparer<KeyValuePair<Dictionary<String, String>, SB2_MON_PARM>>
    {
        public static readonly EnemyComparer Instance = new EnemyComparer();

        public static Int32 CalcDiff(KeyValuePair<Dictionary<String, String>, SB2_MON_PARM> item, IEnumerable<KeyValuePair<Dictionary<String, String>, SB2_MON_PARM>> other)
        {
            Int32 total = 0;
            SB2_MON_PARM baseValue = item.Value;
            foreach (KeyValuePair<Dictionary<String, String>, SB2_MON_PARM> data in other)
            {
                SB2_MON_PARM enemy = data.Value;
                if (baseValue == enemy)
                    continue;

                if (baseValue.ResistStatus != enemy.ResistStatus)
                    total++;
                if (baseValue.AutoStatus != enemy.AutoStatus)
                    total++;
                if (baseValue.InitialStatus != enemy.InitialStatus)
                    total++;
                if (baseValue.MaxHP != enemy.MaxHP)
                    total++;
                if (baseValue.MaxMP != enemy.MaxMP)
                    total++;
                if (baseValue.WinGil != enemy.WinGil)
                    total++;
                if (baseValue.WinExp != enemy.WinExp)
                    total++;

                if (!baseValue.WinItems.SequenceEqual(enemy.WinItems))
                    total++;

                if (!baseValue.StealItems.SequenceEqual(enemy.StealItems))
                    total++;

                if (baseValue.Radius != enemy.Radius)
                    total++;
                if (baseValue.Geo != enemy.Geo)
                    total++;

                if (!baseValue.Mot.SequenceEqual(enemy.Mot))
                    total++;

                if (!baseValue.Mesh.SequenceEqual(enemy.Mesh))
                    total++;

                if (baseValue.Flags != enemy.Flags)
                    total++;
                if (baseValue.AP != enemy.AP)
                    total++;

                if (baseValue.Element.Speed != enemy.Element.Speed)
                    total++;
                if (baseValue.Element.Strength != enemy.Element.Strength)
                    total++;
                if (baseValue.Element.Magic != enemy.Element.Magic)
                    total++;
                if (baseValue.Element.Spirit != enemy.Element.Spirit)
                    total++;
                if (baseValue.Element.pad != enemy.Element.pad)
                    total++;
                if (baseValue.Element.trans != enemy.Element.trans)
                    total++;
                if (baseValue.Element.cur_capa != enemy.Element.cur_capa)
                    total++;
                if (baseValue.Element.max_capa != enemy.Element.max_capa)
                    total++;

                if (baseValue.GuardElement != enemy.GuardElement || baseValue.AbsorbElement != enemy.AbsorbElement || baseValue.HalfElement != enemy.HalfElement || baseValue.WeakElement != enemy.WeakElement)
                    total++;

                if (baseValue.Level != enemy.Level)
                    total++;
                if (baseValue.Category != enemy.Category)
                    total++;
                if (baseValue.HitRate != enemy.HitRate)
                    total++;
                if (baseValue.PhysicalDefence != enemy.PhysicalDefence)
                    total++;
                if (baseValue.PhysicalEvade != enemy.PhysicalEvade)
                    total++;
                if (baseValue.MagicalDefence != enemy.MagicalDefence)
                    total++;
                if (baseValue.MagicalEvade != enemy.MagicalEvade)
                    total++;
                if (baseValue.BlueMagic != enemy.BlueMagic)
                    total++;

                if (!baseValue.Bone.SequenceEqual(enemy.Bone))
                    total++;

                if (baseValue.DieSfx != enemy.DieSfx)
                    total++;
                if (baseValue.Konran != enemy.Konran)
                    total++;
                if (baseValue.MesCnt != enemy.MesCnt)
                    total++;

                if (!baseValue.IconBone.SequenceEqual(enemy.IconBone))
                    total++;

                if (!baseValue.IconY.SequenceEqual(enemy.IconY))
                    total++;

                if (!baseValue.IconZ.SequenceEqual(enemy.IconZ))
                    total++;

                if (baseValue.StartSfx != enemy.StartSfx)
                    total++;
                if (baseValue.ShadowX != enemy.ShadowX)
                    total++;
                if (baseValue.ShadowZ != enemy.ShadowZ)
                    total++;
                if (baseValue.ShadowBone != enemy.ShadowBone)
                    total++;
                if (baseValue.WinCard != enemy.WinCard)
                    total++;
                if (baseValue.ShadowOfsX != enemy.ShadowOfsX)
                    total++;
                if (baseValue.ShadowOfsZ != enemy.ShadowOfsZ)
                    total++;
                if (baseValue.ShadowBone2 != enemy.ShadowBone2)
                    total++;
                if (baseValue.Pad0 != enemy.Pad0)
                    total++;
                if (baseValue.Pad1 != enemy.Pad1)
                    total++;
                if (baseValue.Pad2 != enemy.Pad2)
                    total++;
            }
            return total;
        }

        public Boolean Equals(KeyValuePair<Dictionary<String, String>, SB2_MON_PARM> x, KeyValuePair<Dictionary<String, String>, SB2_MON_PARM> y)
        {
            if (x.Key["US"] != y.Key["US"])
                return false;

            return CalcDiff(x, new[] { y }) == 0;
        }

        public Int32 GetHashCode(KeyValuePair<Dictionary<String, String>, SB2_MON_PARM> obj)
        {
            Int64 hashCode = obj.Key["US"].GetHashCode();
            SB2_MON_PARM enemy = obj.Value;

            hashCode = (hashCode * 397) ^ (UInt32)enemy.ResistStatus;
            hashCode = (hashCode * 397) ^ (UInt32)enemy.AutoStatus;
            hashCode = (hashCode * 397) ^ (UInt32)enemy.InitialStatus;

            hashCode = (hashCode * 397) ^ enemy.MaxHP;
            hashCode = (hashCode * 397) ^ enemy.MaxMP;
            hashCode = (hashCode * 397) ^ enemy.WinGil;
            hashCode = (hashCode * 397) ^ enemy.WinExp;

            GetHashCode(ref hashCode, enemy.WinItems);
            GetHashCode(ref hashCode, enemy.StealItems);

            hashCode = (hashCode * 397) ^ enemy.Radius;
            hashCode = (hashCode * 397) ^ enemy.Geo;

            GetHashCode(ref hashCode, enemy.Mot);
            GetHashCode(ref hashCode, enemy.Mesh);

            hashCode = (hashCode * 397) ^ enemy.Flags;
            hashCode = (hashCode * 397) ^ enemy.AP;

            hashCode = (hashCode * 397) ^ enemy.Element.Speed;
            hashCode = (hashCode * 397) ^ enemy.Element.Strength;
            hashCode = (hashCode * 397) ^ enemy.Element.Magic;
            hashCode = (hashCode * 397) ^ enemy.Element.Spirit;
            hashCode = (hashCode * 397) ^ enemy.Element.pad;
            hashCode = (hashCode * 397) ^ enemy.Element.trans;
            hashCode = (hashCode * 397) ^ enemy.Element.cur_capa;
            hashCode = (hashCode * 397) ^ enemy.Element.max_capa;

            GetHashCode(ref hashCode, new Byte[] { enemy.GuardElement, enemy.AbsorbElement, enemy.HalfElement, enemy.WeakElement });

            hashCode = (hashCode * 397) ^ enemy.Level;
            hashCode = (hashCode * 397) ^ enemy.Category;
            hashCode = (hashCode * 397) ^ enemy.HitRate;
            hashCode = (hashCode * 397) ^ enemy.PhysicalDefence;
            hashCode = (hashCode * 397) ^ enemy.PhysicalEvade;
            hashCode = (hashCode * 397) ^ enemy.MagicalDefence;
            hashCode = (hashCode * 397) ^ enemy.MagicalEvade;
            hashCode = (hashCode * 397) ^ enemy.BlueMagic;

            GetHashCode(ref hashCode, enemy.Bone);

            hashCode = (hashCode * 397) ^ enemy.DieSfx;
            hashCode = (hashCode * 397) ^ enemy.Konran;
            hashCode = (hashCode * 397) ^ enemy.MesCnt;

            GetHashCode(ref hashCode, enemy.IconBone);
            GetHashCode(ref hashCode, enemy.IconY);
            GetHashCode(ref hashCode, enemy.IconZ);

            hashCode = (hashCode * 397) ^ enemy.StartSfx;
            hashCode = (hashCode * 397) ^ enemy.ShadowX;
            hashCode = (hashCode * 397) ^ enemy.ShadowZ;
            hashCode = (hashCode * 397) ^ enemy.ShadowBone;
            hashCode = (hashCode * 397) ^ (Int32)enemy.WinCard;
            hashCode = (hashCode * 397) ^ enemy.ShadowOfsX;
            hashCode = (hashCode * 397) ^ enemy.ShadowOfsZ;
            hashCode = (hashCode * 397) ^ enemy.ShadowBone2;
            hashCode = (hashCode * 397) ^ enemy.Pad0;
            hashCode = (hashCode * 397) ^ enemy.Pad1;
            hashCode = (hashCode * 397) ^ enemy.Pad2;

            return unchecked((Int32)hashCode);
        }

        private void GetHashCode(ref Int64 hashCode, Byte[] array)
        {
            foreach (Byte item in array)
                hashCode = (hashCode * 397) ^ item;
        }

        private void GetHashCode(ref Int64 hashCode, SByte[] array)
        {
            foreach (SByte item in array)
                hashCode = (hashCode * 397) ^ item;
        }

        private void GetHashCode(ref Int64 hashCode, UInt16[] array)
        {
            foreach (UInt16 item in array)
                hashCode = (hashCode * 397) ^ item;
        }

        private void GetHashCode(ref Int64 hashCode, Int32[] array)
        {
            foreach (Int32 item in array)
                hashCode = (hashCode * 397) ^ item;
        }

        private void GetHashCode(ref Int64 hashCode, RegularItem[] array)
        {
            foreach (Int32 item in array)
                hashCode = (hashCode * 397) ^ item;
        }

        public static Boolean EqualElements(SB2_ELEMENT x, SB2_ELEMENT y)
        {
            return x.Speed == y.Speed
                   && x.Strength == y.Strength
                   && x.Magic == y.Magic
                   && x.Spirit == y.Spirit
                   && x.pad == y.pad
                   && x.trans == y.trans
                   && x.cur_capa == y.cur_capa
                   && x.max_capa == y.max_capa;
        }
    }
}

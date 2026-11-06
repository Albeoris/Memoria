using System;
using System.IO;
using System.Collections.Generic;
using Memoria.Prime.Text;

namespace Memoria.Data
{
    // Currently, both EnemyCategory (byte flags, up to 8 categories) and EnemyCategorySet (hash set, any amount of categories) co-exist
    // See EnemyCategory.cs for the similar type specifications
    [Flags]
    public enum EnemyCategory : byte
    {
        Other = 0,
        Humanoid = 1,
        Beast = 2,
        Devil = 4,
        Dragon = 8,
        Undead = 16,
        Stone = 32,
        Soul = 64,
        Flight = 128
    }

    public enum EnemyCategoryToken : int
    {
        Humanoid = 0,
        Beast = 1,
        Devil = 2,
        Dragon = 3,
        Undead = 4,
        Stone = 5,
        Soul = 6,
        Flight = 7
    }

    public class EnemyCategorySet : HashSet<EnemyCategoryToken>
    {
        public EnemyCategorySet() : base() { }

        public EnemyCategorySet(HashSet<EnemyCategoryToken> hash) : base(hash) { }

        public EnemyCategorySet(EnemyCategory category) : base()
        {
            if ((category & EnemyCategory.Humanoid) != 0)   Add(EnemyCategoryToken.Humanoid);
            if ((category & EnemyCategory.Beast) != 0)      Add(EnemyCategoryToken.Beast);
            if ((category & EnemyCategory.Devil) != 0)      Add(EnemyCategoryToken.Devil);
            if ((category & EnemyCategory.Dragon) != 0)     Add(EnemyCategoryToken.Dragon);
            if ((category & EnemyCategory.Undead) != 0)     Add(EnemyCategoryToken.Undead);
            if ((category & EnemyCategory.Stone) != 0)      Add(EnemyCategoryToken.Stone);
            if ((category & EnemyCategory.Soul) != 0)       Add(EnemyCategoryToken.Soul);
            if ((category & EnemyCategory.Flight) != 0)     Add(EnemyCategoryToken.Flight);
        }

        public EnemyCategory ToEnemyCategory()
        {
            EnemyCategory category = 0;
            if (Contains(EnemyCategoryToken.Humanoid))  category |= EnemyCategory.Humanoid;
            if (Contains(EnemyCategoryToken.Beast))     category |= EnemyCategory.Beast;
            if (Contains(EnemyCategoryToken.Devil))     category |= EnemyCategory.Devil;
            if (Contains(EnemyCategoryToken.Dragon))    category |= EnemyCategory.Dragon;
            if (Contains(EnemyCategoryToken.Undead))    category |= EnemyCategory.Undead;
            if (Contains(EnemyCategoryToken.Stone))     category |= EnemyCategory.Stone;
            if (Contains(EnemyCategoryToken.Soul))      category |= EnemyCategory.Soul;
            if (Contains(EnemyCategoryToken.Flight))    category |= EnemyCategory.Flight;
            return category;
        }

        // These UInt32 conversions are currently used by NCalc to allow the use of up to 32 categories instead of 8 using the legacy operations
        // They may be removed in the future
        public void SetFromUInt32(UInt32 category)
        {
            for (Int32 i = 0; i < 32; i++)
            {
                if ((category & (1u << i)) != 0)
                    Add((EnemyCategoryToken)i);
                else
                    Remove((EnemyCategoryToken)i);
            }
        }

        public UInt32 ToUInt32()
        {
            UInt32 val = 0;
            for (Int32 i = 0; i < 32; i++)
                if (Contains((EnemyCategoryToken)i))
                    val |= 1u << i;
            return val;
        }

        public static Int32 CountCommon(params EnemyCategorySet[] elems)
        {
            if (elems.Length == 0)
                return 0;
            HashSet<EnemyCategoryToken> hash = new HashSet<EnemyCategoryToken>(elems[0]);
            for (Int32 i = 1; i < elems.Length; i++)
                hash.IntersectWith(elems[i]);
            return hash.Count;
        }

        public static EnemyCategorySet Combine(params EnemyCategorySet[] categories)
        {
            HashSet<EnemyCategoryToken> hash = new HashSet<EnemyCategoryToken>();
            foreach (EnemyCategorySet cat in categories)
                hash.UnionWith(cat);
            return new EnemyCategorySet(hash);
        }

        public static EnemyCategorySet Combine(EnemyCategorySet category, params EnemyCategoryToken[] tokens)
        {
            return [.. category, .. tokens];
        }

        public static EnemyCategorySet Disjoin(EnemyCategorySet elem, params EnemyCategorySet[] remove)
        {
            EnemyCategorySet disjoined = new EnemyCategorySet(elem);
            foreach (EnemyCategorySet elemset in remove)
                disjoined.RemoveMultiple(elemset);
            return disjoined;
        }

        public static EnemyCategorySet Disjoin(EnemyCategorySet elem, params EnemyCategoryToken[] remove)
        {
            EnemyCategorySet disjoined = new EnemyCategorySet(elem);
            foreach (EnemyCategoryToken elemset in remove)
                disjoined.Remove(elemset);
            return disjoined;
        }

        public void AddMultiple(params EnemyCategoryToken[] tokens)
        {
            foreach (EnemyCategoryToken tok in tokens)
                Add(tok);
        }

        public void AddMultiple(EnemyCategorySet other)
        {
            foreach (EnemyCategoryToken tok in other)
                Add(tok);
        }

        public void RemoveMultiple(params EnemyCategoryToken[] tokens)
        {
            foreach (EnemyCategoryToken tok in tokens)
                Remove(tok);
        }

        public void RemoveMultiple(EnemyCategorySet other)
        {
            foreach (EnemyCategoryToken tok in other)
                Remove(tok);
        }

        public static EnemyCategorySet ReadBinary(BinaryReader reader)
        {
            EnemyCategorySet res = new EnemyCategorySet();
            UInt32 count = reader.ReadUInt32();
            for (UInt32 i = 0; i < count; i++)
                res.Add((EnemyCategoryToken)reader.ReadInt32());
            return res;
        }

        // Used by the system of "Memoria.PatchableFieldAttribute" for BattlePatch.txt for example
        public static Boolean TryParse(String str, out EnemyCategorySet res)
        {
            res = new EnemyCategorySet();
            foreach (String tokstr in str.Split([',']))
            {
                if (tokstr.Trim().TryEnumParse(out EnemyCategoryToken tok))
                    res.Add(tok);
                else
                    return false;
            }
            return true;
        }
    }
}

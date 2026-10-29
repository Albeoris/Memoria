using System;
using System.IO;
using System.Collections.Generic;
using Memoria.Prime.Text;

namespace Memoria.Data
{
    // Currently, both EffectElement (byte flags, up to 8 elements) and ElementSet (hash set, any amount of elements) co-exist

    // Legacy elements EffectElement:
    // - Check if the Fire is part of the element:  (elem & EffectElement.Fire) != 0
    // - Combine Fire with an existing element:     elem | EffectElement.Fire
    [Flags]
    public enum EffectElement : byte
    {
        None = 0,
        Fire = 1,
        Cold = 2,
        Thunder = 4,
        Earth = 8,
        Aqua = 16,
        Wind = 32,
        Holy = 64,
        Darkness = 128
    }

    // Extended elements ElementSet:
    // - Check if the Fire is part of the element:  elem.Contains(ElementToken.Fire)
    // - Combine Fire with an existing element:     Combine(elem, ElementToken.Fire)
    public enum ElementToken : int
    {
        Fire = 0,
        Cold = 1,
        Thunder = 2,
        Earth = 3,
        Aqua = 4,
        Wind = 5,
        Holy = 6,
        Darkness = 7
    }

    public class ElementSet : HashSet<ElementToken>
    {
        public ElementSet() : base() { }

        public ElementSet(HashSet<ElementToken> hash) : base(hash) { }

        public ElementSet(EffectElement elem) : base()
        {
            if ((elem & EffectElement.Fire) != 0)       Add(ElementToken.Fire);
            if ((elem & EffectElement.Cold) != 0)       Add(ElementToken.Cold);
            if ((elem & EffectElement.Thunder) != 0)    Add(ElementToken.Thunder);
            if ((elem & EffectElement.Earth) != 0)      Add(ElementToken.Earth);
            if ((elem & EffectElement.Aqua) != 0)       Add(ElementToken.Aqua);
            if ((elem & EffectElement.Wind) != 0)       Add(ElementToken.Wind);
            if ((elem & EffectElement.Holy) != 0)       Add(ElementToken.Holy);
            if ((elem & EffectElement.Darkness) != 0)   Add(ElementToken.Darkness);
        }

        public EffectElement ToEffectElement()
        {
            EffectElement elem = 0;
            if (Contains(ElementToken.Fire))        elem |= EffectElement.Fire;
            if (Contains(ElementToken.Cold))        elem |= EffectElement.Cold;
            if (Contains(ElementToken.Thunder))     elem |= EffectElement.Thunder;
            if (Contains(ElementToken.Earth))       elem |= EffectElement.Earth;
            if (Contains(ElementToken.Aqua))        elem |= EffectElement.Aqua;
            if (Contains(ElementToken.Wind))        elem |= EffectElement.Wind;
            if (Contains(ElementToken.Holy))        elem |= EffectElement.Holy;
            if (Contains(ElementToken.Darkness))    elem |= EffectElement.Darkness;
            return elem;
        }

        // These UInt32 conversions are currently used by NCalc to allow the use of up to 32 elements instead of 8 using the legacy operations
        // They may be removed in the future
        public void SetFromUInt32(UInt32 elem)
        {
            for (Int32 i = 0; i < 32; i++)
            {
                if ((elem & (1u << i)) != 0)
                    Add((ElementToken)i);
                else
                    Remove((ElementToken)i);
            }
        }

        public UInt32 ToUInt32()
        {
            UInt32 val = 0;
            for (Int32 i = 0; i < 32; i++)
                if (Contains((ElementToken)i))
                    val |= 1u << i;
            return val;
        }

        public static Int32 CountCommon(params ElementSet[] elems)
        {
            if (elems.Length == 0)
                return 0;
            HashSet<ElementToken> hash = new HashSet<ElementToken>(elems[0]);
            for (Int32 i = 1; i < elems.Length; i++)
                hash.IntersectWith(elems[i]);
            return hash.Count;
        }

        public static ElementSet Combine(params ElementSet[] elems)
        {
            ElementSet comb = new ElementSet();
            foreach (ElementSet elem in elems)
                comb.UnionWith(elem);
            return comb;
        }

        public static ElementSet Combine(ElementSet elem, params ElementToken[] tokens)
        {
            return [.. elem, .. tokens];
        }

        public static ElementSet Disjoin(ElementSet elem, params ElementSet[] remove)
        {
            ElementSet disjoined = new ElementSet(elem);
            foreach (ElementSet elemset in remove)
                disjoined.RemoveMultiple(elemset);
            return disjoined;
        }

        public static ElementSet Disjoin(ElementSet elem, params ElementToken[] remove)
        {
            ElementSet disjoined = new ElementSet(elem);
            foreach (ElementToken elemset in remove)
                disjoined.Remove(elemset);
            return disjoined;
        }

        public void AddMultiple(params ElementToken[] tokens)
        {
            foreach (ElementToken tok in tokens)
                Add(tok);
        }

        public void AddMultiple(ElementSet other)
        {
            foreach (ElementToken tok in other)
                Add(tok);
        }

        public void RemoveMultiple(params ElementToken[] tokens)
        {
            foreach (ElementToken tok in tokens)
                Remove(tok);
        }

        public void RemoveMultiple(ElementSet other)
        {
            foreach (ElementToken tok in other)
                Remove(tok);
        }

        public static ElementSet ReadBinary(BinaryReader reader)
        {
            ElementSet res = new ElementSet();
            UInt32 count = reader.ReadUInt32();
            for (UInt32 i = 0; i < count; i++)
                res.Add((ElementToken)reader.ReadInt32());
            return res;
        }

        // Used by the system of "Memoria.PatchableFieldAttribute" for BattlePatch.txt for example
        public static Boolean TryParse(String str, out ElementSet res)
        {
            res = new ElementSet();
            foreach (String tokstr in str.Split([',']))
            {
                if (tokstr.Trim().TryEnumParse(out ElementToken tok))
                    res.Add(tok);
                else
                    return false;
            }
            return true;
        }
    }
}

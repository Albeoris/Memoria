using System;

namespace FF8.JSM
{
    public readonly struct Int26
    {
        public readonly UInt32 Raw;

        public Int26(Int32 raw)
        {
            Raw = (UInt32)raw;
        }
        
        public Int26(Int32 raw, Jsm.Expression.VariableSource source, Jsm.Expression.VariableType type)
        {
            Raw = (UInt32) ((raw & 0x3FFFFFF) | ((Int32) source << 26) | ((Int32) type << 29));
        }
        
        public Int32 Value => (Int32)(Raw & 0x3FFFFFF);
        public Jsm.Expression.VariableSource Source => (Jsm.Expression.VariableSource) ((Raw >> 26) & 7);
        public Jsm.Expression.VariableType Type => (Jsm.Expression.VariableType) (Raw >> 29);

        public override String ToString() => $"{Type} {Source} = {Value}";

        public override Boolean Equals(Object obj)
        {
            if (obj is Int26 other)
                return Raw == other.Raw;
            return false;
        }

        public Boolean Equals(Int26 other) => Raw == other.Raw;
        public override Int32 GetHashCode() => (Int32)Raw;
        public static Boolean operator ==(Int26 left, Int26 right) => left.Raw == right.Raw;
        public static Boolean operator !=(Int26 left, Int26 right) => left.Raw != right.Raw;
    }
}
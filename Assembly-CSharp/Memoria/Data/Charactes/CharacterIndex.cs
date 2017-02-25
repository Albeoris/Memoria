using System;

namespace Memoria.Data
{
    public struct CharacterIndex
    {
        private readonly Byte _index;

        public static readonly CharacterIndex Zidane = 0;
        public static readonly CharacterIndex Vivi = 1;
        public static readonly CharacterIndex Garnet = 2;
        public static readonly CharacterIndex Steiner = 3;
        public static readonly CharacterIndex Freya = 4;
        public static readonly CharacterIndex Quina = 5;
        public static readonly CharacterIndex Cinna = 5;
        public static readonly CharacterIndex Eiko = 6;
        public static readonly CharacterIndex Marcus = 6;
        public static readonly CharacterIndex Amarant = 7;
        public static readonly CharacterIndex Blank = 7;
        public static readonly CharacterIndex Beatrix = 8;

        private CharacterIndex(Byte index)
        {
            _index = index;
        }

        public static implicit operator Byte(CharacterIndex value)
        {
            return value._index;
        }

        public static implicit operator CharacterIndex(Byte value)
        {
            return new CharacterIndex(value);
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is CharacterIndex)
                return _index == ((CharacterIndex)obj)._index;
            return false;
        }

        public override Int32 GetHashCode()
        {
            return _index.GetHashCode();
        }

        public override String ToString()
        {
            return _index.ToString();
        }
    }
}
using System;

namespace Memoria.Data
{
    public struct CharacterId
    {
        private readonly Int32 _id;

        public const Int32 CharacterCount = 12;

        public static readonly CharacterId Zidane = 0;
        public static readonly CharacterId Vivi = 1;
        public static readonly CharacterId Garnet = 2;
        public static readonly CharacterId Steiner = 3;
        public static readonly CharacterId Freya = 4;
        public static readonly CharacterId Quina = 5;
        public static readonly CharacterId Eiko = 6;
        public static readonly CharacterId Amarant = 7;
        public static readonly CharacterId Cinna = 8;
        public static readonly CharacterId Marcus = 9;
        public static readonly CharacterId Blank = 10;
        public static readonly CharacterId Beatrix = 11;

        private CharacterId(Int32 id)
        {
            _id = id;
        }

        public static implicit operator Int32(CharacterId value)
        {
            return value._id;
        }

        public static implicit operator CharacterId(Int32 value)
        {
            return new CharacterId(value);
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is CharacterId)
                return _id == ((CharacterId)obj)._id;
            return false;
        }

        public override Int32 GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override String ToString()
        {
            return _id.ToString();
        }
    }
}
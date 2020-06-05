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

        public CharacterIndex ToCharacterIndex()
        {
            return _id switch
            {
                0 => CharacterIndex.Zidane,
                1 => CharacterIndex.Vivi,
                2 => CharacterIndex.Garnet,
                3 => CharacterIndex.Steiner,
                4 => CharacterIndex.Freya,
                5 => CharacterIndex.Quina,
                6 => CharacterIndex.Eiko,
                7 => CharacterIndex.Amarant,
                8 => CharacterIndex.Cinna,
                9 => CharacterIndex.Marcus,
                10 => CharacterIndex.Blank,
                11 => CharacterIndex.Beatrix,
                _ => throw new NotSupportedException(ToString())
            };
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
                return _id == ((CharacterId) obj)._id;
            return false;
        }

        public override Int32 GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override String ToString()
        {
            return _id switch
            {
                0 => nameof(Zidane),
                1 => nameof(Vivi),
                2 => nameof(Garnet),
                3 => nameof(Steiner),
                4 => nameof(Freya),
                5 => nameof(Quina),
                6 => nameof(Eiko),
                7 => nameof(Amarant),
                8 => nameof(Cinna),
                9 => nameof(Marcus),
                10 => nameof(Blank),
                11 => nameof(Beatrix),
                _ => _id.ToString()
            };
        }
    }
}
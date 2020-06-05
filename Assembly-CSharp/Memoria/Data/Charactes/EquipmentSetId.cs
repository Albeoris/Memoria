using System;

namespace Memoria.Data
{
    public struct EquipmentSetId
    {
        private readonly Int32 _id;

        public const Int32 EquipmentSetCount = 16;

        public static readonly EquipmentSetId Zidane = new EquipmentSetId(0);
        public static readonly EquipmentSetId Vivi = new EquipmentSetId(1);
        public static readonly EquipmentSetId Garnet = new EquipmentSetId(2);
        public static readonly EquipmentSetId Steiner = new EquipmentSetId(3);
        public static readonly EquipmentSetId Freya = new EquipmentSetId(4);
        public static readonly EquipmentSetId Quina = new EquipmentSetId(5);
        public static readonly EquipmentSetId Eiko = new EquipmentSetId(6);
        public static readonly EquipmentSetId Amarant = new EquipmentSetId(7);
        public static readonly EquipmentSetId Cinna = new EquipmentSetId(8);
        public static readonly EquipmentSetId Marcus = new EquipmentSetId(9);
        public static readonly EquipmentSetId Blank = new EquipmentSetId(10);
        public static readonly EquipmentSetId Beatrix = new EquipmentSetId(11);
        public static readonly EquipmentSetId Marcus2 = new EquipmentSetId(12);
        public static readonly EquipmentSetId Beatrix2 = new EquipmentSetId(13);
        public static readonly EquipmentSetId Blank2 = new EquipmentSetId(14);
        public static readonly EquipmentSetId Empty = new EquipmentSetId(15);

        public EquipmentSetId(Int32 id)
        {
            _id = id;
        }
        
        public static EquipmentSetId[] GetAll()
        {
            EquipmentSetId[] result = new EquipmentSetId[EquipmentSetCount];
            for (Byte i = 0; i < result.Length; i++)
                result[i] = new EquipmentSetId(i);
            return result;
        }

        public CharacterId ToCharacterId()
        {
            return _id switch
            {
                0 => CharacterId.Zidane,
                1 => CharacterId.Vivi,
                2 => CharacterId.Garnet,
                3 => CharacterId.Steiner,
                4 => CharacterId.Freya,
                5 => CharacterId.Quina,
                6 => CharacterId.Eiko,
                7 => CharacterId.Amarant,
                8 => CharacterId.Cinna,
                9 => CharacterId.Marcus,
                10 => CharacterId.Blank,
                11 => CharacterId.Beatrix,
                12 => CharacterId.Marcus,
                13 => CharacterId.Beatrix,
                14 => CharacterId.Blank,
                _ => throw new NotSupportedException(ToString())
            };
        }

        public static implicit operator Int32(EquipmentSetId value)
        {
            return value._id;
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is EquipmentSetId)
                return _id == ((EquipmentSetId) obj)._id;
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
                12 => nameof(Marcus2),
                13 => nameof(Beatrix2),
                14 => nameof(Blank2),
                15 => nameof(Empty),
                _ => _id.ToString()
            };
        }
    }
}
using System;

namespace Memoria.Data
{
    public struct CharacterPresetId
    {
        private readonly Byte _presetId;

        public static readonly CharacterPresetId Zidane = 0;
        public static readonly CharacterPresetId Vivi = 1;
        public static readonly CharacterPresetId Garnet = 2;
        public static readonly CharacterPresetId Steiner = 3;
        public static readonly CharacterPresetId Freya = 4;
        public static readonly CharacterPresetId Quina = 5;
        public static readonly CharacterPresetId Eiko = 6;
        public static readonly CharacterPresetId Amarant = 7;
        public static readonly CharacterPresetId Cinna1 = 8;
        public static readonly CharacterPresetId Cinna2 = 9;
        public static readonly CharacterPresetId Marcus1 = 10;
        public static readonly CharacterPresetId Marcus2 = 11;
        public static readonly CharacterPresetId Blank1 = 12;
        public static readonly CharacterPresetId Blank2 = 13;
        public static readonly CharacterPresetId Beatrix1 = 14;
        public static readonly CharacterPresetId Beatrix2 = 15;
        public static readonly CharacterPresetId StageZidane = 16;
        public static readonly CharacterPresetId StageCinna = 17;
        public static readonly CharacterPresetId StageMarcus = 18;
        public static readonly CharacterPresetId StageBlank = 19;

        public static CharacterPresetId[] GetWellKnownPresetIds()
        {
            CharacterPresetId[] result = new CharacterPresetId[16];
            for (Byte i = 0; i < result.Length; i++)
                result[i] = i;
            return result;
        }

        private CharacterPresetId(Byte presetId)
        {
            _presetId = presetId;
        }

        public static implicit operator Byte(CharacterPresetId value)
        {
            return value._presetId;
        }

        public static implicit operator CharacterPresetId(Byte value)
        {
            return new CharacterPresetId(value);
        }

        public CharacterId ToCharacterId()
        {
            return _presetId switch
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
                9 => CharacterId.Cinna,
                10 => CharacterId.Marcus,
                11 => CharacterId.Marcus,
                12 => CharacterId.Blank,
                13 => CharacterId.Blank,
                14 => CharacterId.Beatrix,
                15 => CharacterId.Beatrix,
                16 => CharacterId.Zidane,
                17 => CharacterId.Cinna,
                18 => CharacterId.Marcus,
                19 => CharacterId.Blank,
                _ => throw new NotSupportedException(this.ToString())
            };
        }

        public EquipmentSetId ToEquipmentSetId()
        {
            return _presetId switch
            {
                0 => EquipmentSetId.Zidane,
                1 => EquipmentSetId.Vivi,
                2 => EquipmentSetId.Garnet,
                3 => EquipmentSetId.Steiner,
                4 => EquipmentSetId.Freya,
                5 => EquipmentSetId.Quina,
                6 => EquipmentSetId.Eiko,
                7 => EquipmentSetId.Amarant,
                8 => EquipmentSetId.Cinna,
                9 => EquipmentSetId.Cinna,
                10 => EquipmentSetId.Marcus,
                11 => EquipmentSetId.Marcus2,
                12 => EquipmentSetId.Blank,
                13 => EquipmentSetId.Blank2,
                14 => EquipmentSetId.Beatrix,
                15 => EquipmentSetId.Beatrix2,
                16 => EquipmentSetId.Zidane,
                17 => EquipmentSetId.Cinna,
                18 => EquipmentSetId.Marcus,
                19 => EquipmentSetId.Blank,
                _ => throw new NotSupportedException(this.ToString())
            };
        }

        public CharacterIndex ToCharacterIndex()
        {
            return ToCharacterId().ToCharacterIndex();
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is CharacterPresetId)
                return _presetId == ((CharacterPresetId)obj)._presetId;
            return false;
        }

        public override Int32 GetHashCode()
        {
            return _presetId.GetHashCode();
        }

        public override String ToString()
        {
            return _presetId switch
            {
                0 => nameof(Zidane),
                1 => nameof(Vivi),
                2 => nameof(Garnet),
                3 => nameof(Steiner),
                4 => nameof(Freya),
                5 => nameof(Quina),
                6 => nameof(Eiko),
                7 => nameof(Amarant),
                8 => nameof(Cinna1),
                9 => nameof(Cinna2),
                10 => nameof(Marcus1),
                11 => nameof(Marcus2),
                12 => nameof(Blank1),
                13 => nameof(Blank2),
                14 => nameof(Beatrix1),
                15 => nameof(Beatrix2),
                16 => nameof(StageZidane),
                17 => nameof(StageCinna),
                18 => nameof(StageMarcus),
                19 => nameof(StageBlank),
                _ => _presetId.ToString()
            };
        }
    }
}
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
            switch (_presetId)
            {
                case 0:
                    return nameof(Zidane);
                case 1:
                    return nameof(Vivi);
                case 2:
                    return nameof(Garnet);
                case 3:
                    return nameof(Steiner);
                case 4:
                    return nameof(Freya);
                case 5:
                    return nameof(Quina);
                case 6:
                    return nameof(Eiko);
                case 7:
                    return nameof(Amarant);
                case 8:
                    return nameof(Cinna1);
                case 9:
                    return nameof(Cinna2);
                case 10:
                    return nameof(Marcus1);
                case 11:
                    return nameof(Marcus2);
                case 12:
                    return nameof(Blank1);
                case 13:
                    return nameof(Blank2);
                case 14:
                    return nameof(Beatrix1);
                case 15:
                    return nameof(Beatrix2);

                case 16:
                    return nameof(StageZidane);
                case 17:
                    return nameof(StageCinna);
                case 18:
                    return nameof(StageMarcus);
                case 19:
                    return nameof(StageBlank);
                default:
                    return _presetId.ToString();
            }
        }
    }
}
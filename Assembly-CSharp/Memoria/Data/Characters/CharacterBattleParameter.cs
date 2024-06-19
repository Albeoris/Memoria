using System;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class CharacterBattleParameter : ICsvEntry
    {
        public CharacterSerialNumber Id;
        public String AvatarSprite;
        public String ModelId;
        public String TranceModelId;
        public Byte[] TranceGlowingColor = new Byte[3];
        public String[] AnimationId = new String[34];
        public SpecialEffect AttackSequence;
        public Byte WeaponBone;
        public Byte[] ShadowData = new Byte[5];
        public Byte[] StatusBone = new Byte[6];
        public SByte[] StatusOffsetY = new SByte[6];
        public SByte[] StatusOffsetZ = new SByte[6];
        public Int32[] WeaponSound = new Int32[2];
        public Single[] WeaponOffsetPos = new Single[] { 0, 0, 0 };
        public Single[] WeaponOffsetRot = new Single[] { 0, 0, 0 };

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 rawIndex = 0;
            Id = (CharacterSerialNumber)CsvParser.Int32(raw[rawIndex++]);
            AvatarSprite = CsvParser.String(raw[rawIndex++]);
            ModelId = CsvParser.String(raw[rawIndex++]);
            TranceModelId = CsvParser.String(raw[rawIndex++]);
            TranceGlowingColor = CsvParser.ByteArray(raw[rawIndex++]);
            if (TranceGlowingColor.Length < 3)
                Array.Resize(ref TranceGlowingColor, 3);
            for (Int32 i = 0; i < 34; i++)
                AnimationId[i] = CsvParser.String(raw[rawIndex++]);
            AttackSequence = (SpecialEffect)CsvParser.Int32(raw[rawIndex++]);
            WeaponBone = CsvParser.Byte(raw[rawIndex++]);
            ShadowData = CsvParser.ByteArray(raw[rawIndex++]);
            if (ShadowData.Length < 5)
                Array.Resize(ref ShadowData, 5);
            StatusBone = CsvParser.ByteArray(raw[rawIndex++]);
            if (StatusBone.Length < 6)
                Array.Resize(ref StatusBone, 6);
            StatusOffsetY = CsvParser.SByteArray(raw[rawIndex++]);
            if (StatusOffsetY.Length < 6)
                Array.Resize(ref StatusOffsetY, 6);
            StatusOffsetZ = CsvParser.SByteArray(raw[rawIndex++]);
            if (StatusOffsetZ.Length < 6)
                Array.Resize(ref StatusOffsetZ, 6);

            if (metadata.HasOption($"Include{nameof(WeaponSound)}"))
                WeaponSound = CsvParser.Int32Array(raw[rawIndex++]);
            else if (FF9Snd.ff9battleSoundWeaponSndEffect02.TryGetValue(Id, out Int32[] sounds))
                WeaponSound = sounds;

            if (metadata.HasOption($"Include{nameof(WeaponOffsetPos)}"))
            {
                string[] WeaponOffsetPosExtract = CsvParser.String(raw[rawIndex++]).Split(','); ;
                if (WeaponOffsetPosExtract.Length == 3)
                {
                    for (Int32 i = 0; i < WeaponOffsetPosExtract.Length; i++)
                        WeaponOffsetPos[i] = Convert.ToSingle(WeaponOffsetPosExtract[i]);
                }
            }

            if (metadata.HasOption($"Include{nameof(WeaponOffsetRot)}"))
            {
                string[] WeaponOffsetRotExtract = CsvParser.String(raw[rawIndex++]).Split(','); ;
                if (WeaponOffsetRotExtract.Length == 3)
                {
                    for (Int32 i = 0; i < WeaponOffsetRotExtract.Length; i++)
                        WeaponOffsetRot[i] = Convert.ToSingle(WeaponOffsetRotExtract[i]);
                }
            }
        }

        public void WriteEntry(CsvWriter writer, CsvMetaData metadata)
        {
            writer.Int32((Int32)Id);
            writer.String(AvatarSprite);
            writer.String(ModelId);
            writer.String(TranceModelId);
            for (Int32 i = 0; i < 34; i++)
                writer.String(AnimationId[i]);
            writer.Int32((Int32)AttackSequence);
            writer.Byte(WeaponBone);
            writer.ByteArray(ShadowData);
            writer.ByteArray(StatusBone);
            writer.SByteArray(StatusOffsetY);
            writer.SByteArray(StatusOffsetZ);

            if (metadata.HasOption($"Include{nameof(CharacterBattleParameter.WeaponSound)}"))
                writer.Int32Array(WeaponSound);
        }
    }
}

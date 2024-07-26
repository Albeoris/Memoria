using Memoria.Prime.CSV;
using System;
using System.Collections.Generic;

namespace Memoria.Data
{
    public sealed class CharacterBattleParameter : ICsvEntry
    {
        public CharacterSerialNumber Id;
        public String AvatarSprite;
        public String ModelId;
        public String TranceModelId;
        public Int32[] TranceGlowingColor = new Int32[3];
        public String[] AnimationId = new String[34];
        public SpecialEffect AttackSequence;
        public Byte WeaponBone;
        public Byte[] ShadowData = new Byte[5];
        public Byte[] StatusBone = new Byte[6];
        public SByte[] StatusOffsetY = new SByte[6];
        public SByte[] StatusOffsetZ = new SByte[6];
        public Int32[] WeaponSound = new Int32[0];
        public Single[] WeaponOffset = new Single[6];
        public Boolean TranceParameters = false;
        public String[] TranceAnimationId = new String[34];
        public SpecialEffect TranceAttackSequence;
        public Byte TranceWeaponBone;
        public Byte[] TranceShadowData = new Byte[5];
        public Byte[] TranceStatusBone = new Byte[6];
        public SByte[] TranceStatusOffsetY = new SByte[6];
        public SByte[] TranceStatusOffsetZ = new SByte[6];
        public Int32[] TranceWeaponSound = new Int32[0];
        public Single[] TranceWeaponOffset = new Single[6];

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 rawIndex = 0;
            Id = (CharacterSerialNumber)CsvParser.Int32(raw[rawIndex++]);
            AvatarSprite = CsvParser.String(raw[rawIndex++]);
            ModelId = CsvParser.String(raw[rawIndex++]);
            TranceModelId = CsvParser.String(raw[rawIndex++]);
            TranceGlowingColor = CsvParser.Int32Array(raw[rawIndex++]);
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

            if (metadata.HasOption($"Include{nameof(WeaponOffset)}"))
            {
                WeaponOffset = CsvParser.SingleArray(raw[rawIndex++]);
                if (WeaponOffset.Length < 6)
                    Array.Resize(ref WeaponOffset, 6);
            }
            TranceParameters = metadata.HasOption($"Include{nameof(TranceParameters)}");
            if (TranceParameters)
            {
                for (Int32 i = 0; i < 34; i++)
                    TranceAnimationId[i] = CsvParser.String(raw[rawIndex++]);
                TranceAttackSequence = (SpecialEffect)CsvParser.Int32(raw[rawIndex++]);
                TranceWeaponBone = CsvParser.Byte(raw[rawIndex++]);
                TranceShadowData = CsvParser.ByteArray(raw[rawIndex++]);
                if (TranceShadowData.Length < 5)
                    Array.Resize(ref TranceShadowData, 5);
                TranceStatusBone = CsvParser.ByteArray(raw[rawIndex++]);
                if (TranceStatusBone.Length < 6)
                    Array.Resize(ref TranceStatusBone, 6);
                TranceStatusOffsetY = CsvParser.SByteArray(raw[rawIndex++]);
                if (TranceStatusOffsetY.Length < 6)
                    Array.Resize(ref TranceStatusOffsetY, 6);
                TranceStatusOffsetZ = CsvParser.SByteArray(raw[rawIndex++]);
                if (TranceStatusOffsetZ.Length < 6)
                    Array.Resize(ref TranceStatusOffsetZ, 6);
                TranceWeaponSound = CsvParser.Int32Array(raw[rawIndex++]);
                TranceWeaponOffset = CsvParser.SingleArray(raw[rawIndex++]);
                if (TranceWeaponOffset.Length < 6)
                    Array.Resize(ref TranceWeaponOffset, 6);
                TranceParameters = true;
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
            
            if (metadata.HasOption($"Include{nameof(WeaponSound)}"))
                writer.Int32Array(WeaponSound);
            if (metadata.HasOption($"Include{nameof(WeaponOffset)}"))
                writer.SingleArray(WeaponOffset);
            if (metadata.HasOption($"Include{nameof(TranceParameters)}"))
            {
                for (Int32 i = 0; i < 34; i++)
                    writer.String(TranceAnimationId[i]);
                writer.Int32((Int32)TranceAttackSequence);
                writer.Byte(TranceWeaponBone);
                writer.ByteArray(TranceShadowData);
                writer.ByteArray(TranceStatusBone);
                writer.SByteArray(TranceStatusOffsetY);
                writer.SByteArray(TranceStatusOffsetZ);
                writer.Int32Array(TranceWeaponSound);
                writer.SingleArray(TranceWeaponOffset);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using Memoria.Prime;
using Memoria.Prime.CSV;
using FF9;

namespace Memoria.Data
{
    public sealed class CharacterBattleParameter : ICsvEntry
    {
        public CharacterSerialNumber Id;
        public CharacterBattleParameter Data;

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
        public Int32[] WeaponSound = [];
        public Single[] WeaponSize = [1f, 1f, 1f];
        public Single[] WeaponOffsetPos = new Single[3];
        public Single[] WeaponOffsetRot = new Single[3];
        public Boolean TranceParameters = false;
        public String[] TranceAnimationId = new String[34];
        public SpecialEffect TranceAttackSequence;
        public Byte TranceWeaponBone;
        public Byte[] TranceShadowData = new Byte[5];
        public Byte[] TranceStatusBone = new Byte[6];
        public SByte[] TranceStatusOffsetY = new SByte[6];
        public SByte[] TranceStatusOffsetZ = new SByte[6];
        public Int32[] TranceWeaponSound = [];
        public Single[] TranceWeaponSize = [1f, 1f, 1f];
        public Single[] TranceWeaponOffsetPos = new Single[3];
        public Single[] TranceWeaponOffsetRot = new Single[3];

        public static CharacterBattleParameter GetExisting(CharacterSerialNumber id)
        {
            if (btl_mot.BattleParameterList.TryGetValue(id, out CharacterBattleParameter result))
                return result;
            throw new NotSupportedException($"The option AppendMode must be used to patch existing entries but the entry {id} doesn't exist");
        }

        public Vector3 GetWeaponRotationFixed(UInt16 weaponModel, Boolean trance = false)
        {
            Single[] rot = trance ? TranceWeaponOffsetRot : WeaponOffsetRot;
            if (weaponModel == 515) // Fix #739
                return rot.ToVector3(false) + new Vector3(270f, 0f, 0f);
            return rot.ToVector3(false);
        }

        public void ParseDataEntry(String[] raw, CsvMetaData metadata, ref Int32 index)
        {
            if (metadata.HasField("AvatarSprite")) AvatarSprite = CsvParser.String(raw[index++]);
            if (metadata.HasField("ModelId")) ModelId = CsvParser.String(raw[index++]);
            if (metadata.HasField("TranceModelId")) TranceModelId = CsvParser.String(raw[index++]);
            if (metadata.HasField("TranceGlowingColor")) TranceGlowingColor = CsvParser.Int32Array(raw[index++]);
            if (TranceGlowingColor.Length < 3)
                Array.Resize(ref TranceGlowingColor, 3);
            if (metadata.HasField("AnimationIds"))
                for (Int32 i = 0; i < 34; i++)
                    AnimationId[i] = CsvParser.String(raw[index++]);
            FlagDuplicateAnimations(AnimationId);
            if (metadata.HasField("AttackSequence")) AttackSequence = (SpecialEffect)CsvParser.Int32(raw[index++]);
            if (metadata.HasField("WeaponBone")) WeaponBone = CsvParser.Byte(raw[index++]);
            if (metadata.HasField("ShadowData")) ShadowData = CsvParser.ByteArray(raw[index++]);
            if (ShadowData.Length < 5)
                Array.Resize(ref ShadowData, 5);
            if (metadata.HasField("StatusBone")) StatusBone = CsvParser.ByteArray(raw[index++]);
            if (StatusBone.Length < 6)
                Array.Resize(ref StatusBone, 6);
            if (metadata.HasField("StatusOffsetY")) StatusOffsetY = CsvParser.SByteArray(raw[index++]);
            if (StatusOffsetY.Length < 6)
                Array.Resize(ref StatusOffsetY, 6);
            if (metadata.HasField("StatusOffsetZ")) StatusOffsetZ = CsvParser.SByteArray(raw[index++]);
            if (StatusOffsetZ.Length < 6)
                Array.Resize(ref StatusOffsetZ, 6);

            if (metadata.HasField("WeaponSound"))
            {
                if (metadata.HasOption($"Include{nameof(WeaponSound)}"))
                    WeaponSound = CsvParser.Int32Array(raw[index++]);
                else if (FF9Snd.ff9battleSoundWeaponSndEffect02.TryGetValue(Id, out Int32[] sounds))
                    WeaponSound = sounds;
            }

            if (metadata.HasOption($"IncludeWeaponOffsets") && metadata.HasField("WeaponOffsets"))
            {
                WeaponSize = CsvParser.SingleArray(raw[index++]);
                WeaponOffsetPos = CsvParser.SingleArray(raw[index++]);
                WeaponOffsetRot = CsvParser.SingleArray(raw[index++]);
            }

            if (metadata.HasField("TranceParameters"))
            {
                TranceParameters = metadata.HasOption($"Include{nameof(TranceParameters)}");
                if (TranceParameters)
                {
                    for (Int32 i = 0; i < 34; i++)
                        TranceAnimationId[i] = CsvParser.String(raw[index++]);
                    FlagDuplicateAnimations(TranceAnimationId);
                    TranceAttackSequence = (SpecialEffect)CsvParser.Int32(raw[index++]);
                    TranceWeaponBone = CsvParser.Byte(raw[index++]);
                    TranceShadowData = CsvParser.ByteArray(raw[index++]);
                    if (TranceShadowData.Length < 5)
                        Array.Resize(ref TranceShadowData, 5);
                    TranceStatusBone = CsvParser.ByteArray(raw[index++]);
                    if (TranceStatusBone.Length < 6)
                        Array.Resize(ref TranceStatusBone, 6);
                    TranceStatusOffsetY = CsvParser.SByteArray(raw[index++]);
                    if (TranceStatusOffsetY.Length < 6)
                        Array.Resize(ref TranceStatusOffsetY, 6);
                    TranceStatusOffsetZ = CsvParser.SByteArray(raw[index++]);
                    if (TranceStatusOffsetZ.Length < 6)
                        Array.Resize(ref TranceStatusOffsetZ, 6);
                    TranceWeaponSound = CsvParser.Int32Array(raw[index++]);
                    if (metadata.HasOption($"IncludeWeaponOffsets"))
                    {
                        TranceWeaponSize = CsvParser.SingleArray(raw[index++]);
                        TranceWeaponOffsetPos = CsvParser.SingleArray(raw[index++]);
                        TranceWeaponOffsetRot = CsvParser.SingleArray(raw[index++]);
                    }
                }
            }
        }

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;
            Id = (CharacterSerialNumber)CsvParser.Int32(raw[index++]);
            Data = metadata.IsAppendMode ? GetExisting(Id) : this;
            Data.ParseDataEntry(raw, metadata, ref index);
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
            if (metadata.HasOption($"IncludeWeaponOffsets"))
            {
                writer.SingleArray(WeaponSize);
                writer.SingleArray(WeaponOffsetPos);
                writer.SingleArray(WeaponOffsetRot);
            }
            if (metadata.HasOption($"Include{nameof(TranceParameters)}"))
            {
                if (TranceParameters)
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
                    if (metadata.HasOption($"IncludeWeaponOffsets"))
                    {
                        writer.SingleArray(TranceWeaponSize);
                        writer.SingleArray(TranceWeaponOffsetPos);
                        writer.SingleArray(TranceWeaponOffsetRot);
                    }
                }
                else
                {
                    for (Int32 i = 0; i < 34; i++)
                        writer.String(AnimationId[i]);
                    writer.Int32((Int32)AttackSequence);
                    writer.Byte(WeaponBone);
                    writer.ByteArray(ShadowData);
                    writer.ByteArray(StatusBone);
                    writer.SByteArray(StatusOffsetY);
                    writer.SByteArray(StatusOffsetZ);
                    writer.Int32Array(WeaponSound);
                    if (metadata.HasOption($"IncludeWeaponOffsets"))
                    {
                        writer.SingleArray(WeaponSize);
                        writer.SingleArray(WeaponOffsetPos);
                        writer.SingleArray(WeaponOffsetRot);
                    }
                }
            }
        }

        private static void FlagDuplicateAnimations(String[] animList)
        {
            HashSet<String> animSet = new HashSet<String>();
            for (Int32 i = 0; i < animList.Length; i++)
                if (!animSet.Add(animList[i]))
                    animList[i] += $" ({i} Duplicate)";
        }
    }
}

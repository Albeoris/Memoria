﻿using System;
using UnityEngine;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public sealed class BattleStatusDataEntry : ICsvEntry
    {
        public String Comment;
        public BattleStatusId Id;

        public Byte Priority;

        public Byte OprCnt;
        public UInt16 ContiCnt;

        public BattleStatus ClearOnApply;
        public BattleStatus ImmunityProvided;

        public Int32 SPSEffect = -1;
        public Int32 SPSAttach = 0;
        public Vector3 SPSExtraPos = default;
        public Int32 SHPEffect = -1;
        public Int32 SHPAttach = 0;
        public Vector3 SHPExtraPos = default;

        public Int32 ColorKind = -1;
        public Int32 ColorPriority = 0;
        public Int32[] ColorBase = [0, 0, 0];

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;
            Comment = CsvParser.String(raw[index++]);
            Id = (BattleStatusId)CsvParser.Int32(raw[index++]);

            Priority = CsvParser.Byte(raw[index++]);
            OprCnt = CsvParser.Byte(raw[index++]);
            ContiCnt = CsvParser.UInt16(raw[index++]);
            ClearOnApply = BattleStatusEntry.ParseBattleStatus(raw[index++], metadata);
            ImmunityProvided = BattleStatusEntry.ParseBattleStatus(raw[index++], metadata);

            if (metadata.HasOption($"IncludeVisuals"))
            {
                Single[] arrayf;
                SPSEffect = CsvParser.Int32(raw[index++]);
                SPSAttach = CsvParser.Int32(raw[index++]);
                arrayf = CsvParser.SingleArray(raw[index++]);
                if (arrayf.Length == 1)
                    SPSExtraPos = new Vector3(arrayf[0], 0f, 0f);
                else if (arrayf.Length == 2)
                    SPSExtraPos = new Vector3(arrayf[0], 0f, arrayf[1]);
                else if (arrayf.Length >= 3)
                    SPSExtraPos = new Vector3(arrayf[0], arrayf[1], arrayf[2]);
                else
                    SPSExtraPos = default;
                SHPEffect = CsvParser.Int32(raw[index++]);
                SHPAttach = CsvParser.Int32(raw[index++]);
                arrayf = CsvParser.SingleArray(raw[index++]);
                if (arrayf.Length == 1)
                    SHPExtraPos = new Vector3(arrayf[0], 0f, 0f);
                else if (arrayf.Length == 2)
                    SHPExtraPos = new Vector3(arrayf[0], 0f, arrayf[1]);
                else if (arrayf.Length >= 3)
                    SHPExtraPos = new Vector3(arrayf[0], arrayf[1], arrayf[2]);
                ColorKind = CsvParser.Int32(raw[index++]);
                ColorPriority = CsvParser.Int32(raw[index++]);
                ColorBase = CsvParser.Int32Array(raw[index++]);
                if (ColorBase.Length < 3)
                    Array.Resize(ref ColorBase, 3);
            }
            else
            {
                // Setup default datas for older versions of the CSV
                switch (Id)
                {
                    case BattleStatusId.Venom:
                        SPSEffect = 1;
                        SPSAttach = 0;
                        break;
                    case BattleStatusId.Silence:
                        SHPEffect = 2;
                        SHPAttach = 2;
                        SHPExtraPos = new Vector3(-92f, 0f, 0f);
                        break;
                    case BattleStatusId.Blind:
                        SPSEffect = 6;
                        SPSAttach = 3;
                        break;
                    case BattleStatusId.Trouble:
                        SHPEffect = 3;
                        SHPAttach = 4;
                        SHPExtraPos = new Vector3(92f, 0f, 0f);
                        break;
                    case BattleStatusId.Zombie:
                        ColorKind = 0;
                        ColorPriority = 100;
                        ColorBase = [-48, -72, -88];
                        break;
                    case BattleStatusId.Berserk:
                        SPSEffect = 7;
                        SPSAttach = 4;
                        ColorKind = 0;
                        ColorPriority = 95;
                        ColorBase = [16, -40, -40];
                        break;
                    case BattleStatusId.Trance:
                        ColorKind = 2;
                        ColorPriority = 10;
                        break;
                    case BattleStatusId.Poison:
                        SPSEffect = 0;
                        SPSAttach = 0;
                        break;
                    case BattleStatusId.Sleep:
                        SPSEffect = 2;
                        SPSAttach = 0;
                        break;
                    case BattleStatusId.Haste:
                        SHPEffect = 1;
                        SHPAttach = 0;
                        SHPExtraPos = new Vector3(-148f, 0f, 0f);
                        break;
                    case BattleStatusId.Slow:
                        SHPEffect = 0;
                        SHPAttach = 0;
                        SHPExtraPos = new Vector3(212f, 0f, 0f);
                        break;
                    case BattleStatusId.Shell:
                        ColorKind = 1;
                        ColorPriority = 50;
                        ColorBase = [-64, 24, 72];
                        break;
                    case BattleStatusId.Protect:
                        ColorKind = 1;
                        ColorPriority = 50;
                        ColorBase = [40, 40, -80];
                        break;
                    case BattleStatusId.Heat:
                        SPSEffect = 3;
                        SPSAttach = 1;
                        ColorKind = 0;
                        ColorPriority = 90;
                        ColorBase = [80, -16, -72];
                        break;
                    case BattleStatusId.Freeze:
                        SPSEffect = 4;
                        SPSAttach = 1;
                        ColorKind = 0;
                        ColorPriority = 85;
                        ColorBase = [-48, 0, 96];
                        break;
                    case BattleStatusId.Reflect:
                        SPSEffect = 5;
                        SPSAttach = 1;
                        break;
                }
            }
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            sw.String(Comment);
            sw.Int32((Int32)Id);

            sw.Byte(Priority);
            sw.Byte(OprCnt);
            sw.UInt16(ContiCnt);

            BattleStatusEntry.WriteBattleStatus(sw, metadata, ClearOnApply);
            BattleStatusEntry.WriteBattleStatus(sw, metadata, ImmunityProvided);

            if (metadata.HasOption($"IncludeVisuals"))
            {
                sw.Int32(SPSEffect);
                sw.Int32(SPSAttach);
                sw.SingleArray([SPSExtraPos.x, SPSExtraPos.y, SPSExtraPos.z]);
                sw.Int32(SHPEffect);
                sw.Int32(SHPAttach);
                sw.SingleArray([SHPExtraPos.x, SHPExtraPos.y, SHPExtraPos.z]);
                sw.Int32(ColorKind);
                sw.Int32(ColorPriority);
                sw.Int32Array(ColorBase);
            }
        }
    }
}

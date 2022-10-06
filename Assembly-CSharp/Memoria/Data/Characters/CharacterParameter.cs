using System;
using Memoria.Prime.CSV;
using NCalc;

namespace Memoria.Data
{
    public sealed class CharacterParameter : ICsvEntry
    {
        public CharacterId Id;
        public Byte DefaultRow;
        public Byte DefaultWinPose;
        public Byte DefaultCategory;
        public CharacterPresetId DefaultMenuType;
        public EquipmentSetId DefaultEquipmentSet;
        public String SerialNumberFormula;
        public String NameKeyword;

        public CharacterSerialNumber GetSerialNumber()
        {
            Expression e = new Expression(SerialNumberFormula);
            PLAYER player = FF9StateSystem.Common.FF9.GetPlayer(Id);
            NCalcUtility.InitializeExpressionPlayer(ref e, player);
            e.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
            e.EvaluateParameter += NCalcUtility.commonNCalcParameters;
            Int64 val = NCalcUtility.ConvertNCalcResult(e.Evaluate(), -1);
            if (val >= 0)
                return (CharacterSerialNumber)val;
            return CharacterSerialNumber.ZIDANE_DAGGER;
        }

        public void ParseEntry(String[] raw)
        {
            Id = (CharacterId)CsvParser.Byte(raw[0]);
            DefaultRow = CsvParser.Byte(raw[1]);
            DefaultWinPose = CsvParser.Byte(raw[2]);
            DefaultCategory = CsvParser.Byte(raw[3]);
            DefaultMenuType = (CharacterPresetId)CsvParser.Byte(raw[4]);
            DefaultEquipmentSet = (EquipmentSetId)CsvParser.Byte(raw[5]);
            SerialNumberFormula = CsvParser.String(raw[6]);
            NameKeyword = CsvParser.String(raw[7]);
        }

        public void WriteEntry(CsvWriter writer)
        {
            writer.Byte((Byte)Id);
            writer.Byte(DefaultRow);
            writer.Byte(DefaultWinPose);
            writer.Byte(DefaultCategory);
            writer.Byte((Byte)DefaultMenuType);
            writer.Byte((Byte)DefaultEquipmentSet);
            writer.String(SerialNumberFormula);
            writer.String(NameKeyword);
        }
    }
}

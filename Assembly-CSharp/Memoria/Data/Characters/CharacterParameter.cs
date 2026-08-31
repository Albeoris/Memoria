using Memoria.Prime.CSV;
using NCalc;
using System;

namespace Memoria.Data
{
    public sealed class CharacterParameter : ICsvEntry
    {
        public CharacterId Id;
        public CharacterParameter Data;

        public Byte DefaultRow;
        public Byte DefaultWinPose;
        public Byte DefaultCategory;
        public CharacterPresetId DefaultMenuType;
        public EquipmentSetId DefaultEquipmentSet;
        public String SerialNumberFormula;
        public String NameKeyword;

        public static CharacterParameter GetExisting(CharacterId id)
        {
            if (ff9play.CharacterParameterList.TryGetValue(id, out CharacterParameter result))
                return result;
            throw new NotSupportedException($"The option AppendMode must be used to patch existing entries but the entry {id} doesn't exist");
        }

        public void ParseDataEntry(String[] raw, CsvMetaData metadata, ref Int32 index)
        {
            if (metadata.HasField("DefaultRow")) DefaultRow = CsvParser.Byte(raw[index++]);
            if (metadata.HasField("DefaultWinPose")) DefaultWinPose = CsvParser.Byte(raw[index++]);
            if (metadata.HasField("DefaultCategory")) DefaultCategory = CsvParser.Byte(raw[index++]);
            if (metadata.HasField("DefaultMenuType")) DefaultMenuType = (CharacterPresetId)CsvParser.Byte(raw[index++]);
            if (metadata.HasField("DefaultEquipmentSet")) DefaultEquipmentSet = (EquipmentSetId)CsvParser.Byte(raw[index++]);
            if (metadata.HasField("SerialNumberFormula")) SerialNumberFormula = CsvParser.String(raw[index++]);
            if (metadata.HasField("NameKeyword")) NameKeyword = CsvParser.String(raw[index++]);
        }

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;
            Id = (CharacterId)CsvParser.Byte(raw[index++]);
            Data = metadata.IsAppendMode ? GetExisting(Id) : this;
            Data.ParseDataEntry(raw, metadata, ref index);
        }

        public void WriteEntry(CsvWriter writer, CsvMetaData metadata)
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
    }
}

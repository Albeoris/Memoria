using Memoria.Prime.CSV;
using System;

namespace Memoria.Data
{
    public class BattleActionEntry : ICsvEntry
    {
        public String Comment;
        public BattleAbilityId Id;

        public AA_DATA ActionData;

        public void ParseEntry(String[] raw, CsvMetaData metadata)
        {
            Int32 index = 0;

            Comment = CsvParser.String(raw[index++]);
            Id = (BattleAbilityId)CsvParser.Int32(raw[index++]);

            TargetDisplay menuWindow = CsvParser.EnumValue<TargetDisplay>(raw[index++]);
            TargetType targets = CsvParser.EnumValue<TargetType>(raw[index++]);
            Boolean defaultAlly = CsvParser.Boolean(raw[index++]);
            Boolean forDead = CsvParser.Boolean(raw[index++]);
            Boolean defaultOnDead = CsvParser.Boolean(raw[index++]);
            Boolean defaultCamera = CsvParser.Boolean(raw[index++]);
            Int16 animation1 = CsvParser.Int16(raw[index++]);
            UInt16 animation2 = CsvParser.UInt16(raw[index++]);
            Int32 scriptId = CsvParser.Int32(raw[index++]);
            Int32 power = CsvParser.Int32(raw[index++]);
            Byte elements = CsvParser.Byte(raw[index++]);
            Int32 rate = CsvParser.Int32(raw[index++]);
            Byte category = CsvParser.Byte(raw[index++]);
            BattleStatusIndex statusIndex = (BattleStatusIndex)CsvParser.Int32(raw[index++]);
            Int32 mp = CsvParser.Int32(raw[index++]);
            Byte type = CsvParser.Byte(raw[index++]);

            BattleCommandInfo cmd = new BattleCommandInfo(targets, defaultAlly, menuWindow, animation1, forDead, defaultCamera, defaultOnDead);
            BTL_REF btl = new BTL_REF(scriptId, power, elements, rate);
            ActionData = new AA_DATA(cmd, btl, category, statusIndex, mp, type, animation2);

            if (metadata.HasOption($"Include{nameof(AA_DATA.CastingTitleType)}"))
                ActionData.CastingTitleType = CsvParser.Byte(raw[index++]);
        }

        public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
        {
            sw.String(Comment);
            sw.Int32((Int32)Id);

            BattleCommandInfo cmdInfo = ActionData.Info;
            BTL_REF btlRef = ActionData.Ref;
            sw.EnumValue(cmdInfo.DisplayStats);
            sw.EnumValue(cmdInfo.Target);
            sw.Boolean(cmdInfo.DefaultAlly);
            sw.Boolean(cmdInfo.ForDead);
            sw.Boolean(cmdInfo.DefaultOnDead);
            sw.Boolean(cmdInfo.DefaultCamera);
            sw.Int16(cmdInfo.VfxIndex);
            sw.UInt16(ActionData.Vfx2);
            sw.Int32(btlRef.ScriptId);
            sw.Int32(btlRef.Power);
            sw.Byte(btlRef.Elements);
            sw.Int32(btlRef.Rate);
            sw.Byte(ActionData.Category);
            sw.Int32((Int32)ActionData.AddStatusNo);
            sw.Int32(ActionData.MP);
            sw.Byte(ActionData.Type);
            if (metadata.HasOption($"Include{nameof(AA_DATA.CastingTitleType)}"))
                sw.Byte(ActionData.CastingTitleType);
        }
    }
}

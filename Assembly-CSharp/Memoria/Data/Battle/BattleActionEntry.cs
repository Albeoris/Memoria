using System;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public class BattleActionEntry : ICsvEntry
    {
        public String Comment;
        public Int32 Id;

        public AA_DATA ActionData;

        public void ParseEntry(String[] raw)
        {
            Comment = CsvParser.String(raw[0]);
            Id = CsvParser.Int32(raw[1]);

            TargetDisplay menuWindow = CsvParser.EnumValue<TargetDisplay>(raw[2]);
            TargetType targets = CsvParser.EnumValue<TargetType>(raw[3]);
            Boolean defaultAlly = CsvParser.Boolean(raw[4]);
            Boolean forDead = CsvParser.Boolean(raw[5]);
            Boolean defaultOnDead = CsvParser.Boolean(raw[6]);
            Boolean defaultCamera = CsvParser.Boolean(raw[7]);
            Int16 animation1 = CsvParser.Int16(raw[8]);
            UInt16 animation2 = CsvParser.UInt16(raw[9]);
            Byte scriptId = CsvParser.Byte(raw[10]);
            Byte power = CsvParser.Byte(raw[11]);
            Byte elements = CsvParser.Byte(raw[12]);
            Byte rate = CsvParser.ByteOrMinusOne(raw[13]);
            Byte category = CsvParser.Byte(raw[14]);
            Byte statusIndex = CsvParser.Byte(raw[15]);
            Byte mp = CsvParser.Byte(raw[16]);
            Byte type = CsvParser.Byte(raw[17]);

            BattleCommandInfo cmd = new BattleCommandInfo(targets, defaultAlly, menuWindow, animation1, forDead, defaultCamera, defaultOnDead);
            BTL_REF btl = new BTL_REF(scriptId, power, elements, rate);
            ActionData = new AA_DATA(cmd, btl, category, statusIndex, mp, type, animation2);
        }

        public void WriteEntry(CsvWriter sw)
        {
            sw.String(Comment);
            sw.Int32(Id);

            BattleCommandInfo cmdInfo = ActionData.Info;
            BTL_REF btlRef = ActionData.Ref;
            sw.EnumValue(cmdInfo.DisplayStats);
            sw.EnumValue(cmdInfo.Target); // target
            sw.Boolean(cmdInfo.DefaultAlly);
            sw.Boolean(cmdInfo.ForDead);
            sw.Boolean(cmdInfo.DefaultOnDead);
            sw.Boolean(cmdInfo.DefaultCamera);
            sw.Int16(cmdInfo.VfxIndex);
            sw.UInt16(ActionData.Vfx2);
            sw.Byte(btlRef.ScriptId); // scriptId
            sw.Byte(btlRef.Power);
            sw.Byte(btlRef.Elements);
            sw.ByteOrMinusOne(btlRef.Rate);
            sw.Byte(ActionData.Category);
            sw.Byte(ActionData.AddStatusNo);
            sw.Byte(ActionData.MP);
            sw.Byte(ActionData.Type);
        }
    }
}
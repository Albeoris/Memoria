using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Assets.Sources.Scripts.UI.Common;
using UnityEngine;

namespace Memoria
{
    public static class BattleSceneExporter
    {
        private static HashSet<SB2_MON_PARM> Monsters = new HashSet<SB2_MON_PARM>(new MonsterComparer());
        private static HashSet<KeyValuePair<String,AA_DATA>> Actions = new HashSet<KeyValuePair<String, AA_DATA>>(ActionComparer.Instance);

        public static void ExportSafe()
        {
            try
            {
                if (!Configuration.Export.Battle)
                {
                    Log.Message("[BattleSceneExporter] Pass through {Configuration.Export.Battle = 0}.");
                    return;
                }

                Boolean old = AssetManager.UseBundles;
                AssetManager.UseBundles = true;

                //File.WriteAllText(@"D:\BattleMapList.txt", AssetManager.Load<TextAsset>("EmbeddedAsset/Manifest/BattleMap/BattleMapList.txt", false).text);
                
                foreach (String scene in CreateSceneList())
                    ExportSceneSafe(scene);

                String relativePath = "BattleMap/Actions/";
                String outputDirectory = Path.Combine(Configuration.Export.Path, relativePath);
                    SerializeAllActions(outputDirectory);

                AssetManager.UseBundles = old;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "[BattleSceneExporter] Failed to export field resources.");
            }
        }

        private static void ExportSceneSafe(String battleSceneName)
        {
            try
            {   
                String relativePath = "BattleMap/BattleScene/EVT_BATTLE_" + battleSceneName + '/';
                String outputDirectory = Path.Combine(Configuration.Export.Path, relativePath);
                String outputPath = outputDirectory + "Main.json";
                if (File.Exists(outputPath))
                {
                    //Log.Warning($"[BattleSceneExporter] Export was skipped bacause a file already exists: [{outputPath}].");
                    //return;
                }

                Log.Message("[BattleSceneExporter] Exporting [{0}]...", battleSceneName);

                Int32 battleZoneId = FF9BattleDB.SceneData["BSC_" + battleSceneName];
                String textEmbadedPath = EmbadedTextResources.GetCurrentPath("/Battle/" + battleZoneId + ".mes");
                String[] text = EmbadedSentenseLoader.LoadSentense(textEmbadedPath);

                BTL_SCENE btlScene = new BTL_SCENE();
                btlScene.ReadBattleScene(battleSceneName);

                String directoryPath = Path.GetDirectoryName(outputPath);
                if (directoryPath != null)
                    Directory.CreateDirectory(directoryPath);

                SerializeScene(btlScene, outputDirectory, text);
                CompileScene(outputDirectory);

                Log.Message("[BattleSceneExporter] Exporting completed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[BattleSceneExporter] Failed to export map [{0}].", battleSceneName);
            }
        }

        private static IEnumerable<String> CreateSceneList()
        {
            foreach (String str in FF9BattleDB.SceneData.Keys)
                yield return str.Substring(4);
        }

        private static void SerializeScene(BTL_SCENE btlScene, String outputDirectory, String[] text)
        {
            Int32 currentText = 0;
            Int32 currentMessage = btlScene.header.TypCount + btlScene.header.AtkCount;

            String outputPath = outputDirectory + "Main.json";
            using (JsonWriter writer = new JsonWriter(outputPath))
                SerializeMain(btlScene, writer);

            outputPath = outputDirectory + "Patterns.json";
            using (JsonWriter writer = new JsonWriter(outputPath))
                SerializePatterns(btlScene, writer);

            outputPath = outputDirectory + "Enemies.json";
            using (JsonWriter writer = new JsonWriter(outputPath))
                SerializeEnemies(btlScene, writer, text, ref currentText, ref currentMessage);

            outputPath = outputDirectory + "Actions.json";
            using (JsonWriter writer = new JsonWriter(outputPath))
                SerializeActions(btlScene, writer, text, ref currentText);
        }

        private static void SerializeMain(BTL_SCENE btlScene, JsonWriter writer)
        {
            writer.BeginObject();
            writer.WriteByte("version", btlScene.header.Ver);
            writer.WriteFlags("flags", (BattleSceneFlags)btlScene.header.Flags);
            writer.EndObject();
        }

        private static void SerializePatterns(BTL_SCENE btlScene, JsonWriter writer)
        {
            writer.BeginComplexArray();
            foreach (SB2_PATTERN pattern in btlScene.PatAddr)
            {
                writer.BeginObject();
                writer.WriteByte("rate", pattern.Rate);
                writer.WriteByte("enemyCount", pattern.MonCount);
                writer.WriteByte("camera", pattern.Camera);
                writer.WriteByte("pad0", pattern.Pad0);
                writer.WriteUInt32("ap", pattern.AP);
                writer.BeginComplexArray("spawn");
                foreach (SB2_PUT point in pattern.Put)
                {
                    writer.BeginObject();
                    writer.WriteByte("typeNo", point.TypeNo);
                    writer.WriteFlags("flags", (BattleSceneSpawnFlags)point.Flags);
                    writer.WriteByte("pease", point.Pease);
                    writer.WriteByte("pad", point.Pad);
                    writer.WriteInt16("x", point.Xpos);
                    writer.WriteInt16("y", point.Ypos);
                    writer.WriteInt16("z", point.Zpos);
                    writer.WriteInt16("rot", point.Rot);
                    writer.EndObject();
                }
                writer.EndComplexArray();
                writer.EndObject();
            }
            writer.EndComplexArray();
        }

        private static void SerializeEnemies(BTL_SCENE btlScene, JsonWriter writer, String[] text, ref Int32 currentText, ref Int32 currentMessage)
        {
            writer.BeginComplexArray();
            foreach (SB2_MON_PARM enemy in btlScene.MonAddr)
            {
                writer.BeginObject();
                writer.WriteString("name", text[currentText++]);
                writer.WriteUInt32("invalidStatus", enemy.Status[0]);
                writer.WriteUInt32("permanentStatus", enemy.Status[1]);
                writer.WriteUInt32("currentStatus", enemy.Status[2]);
                writer.WriteUInt16("maxHp", enemy.MaxHP);
                writer.WriteUInt16("maxMp", enemy.MaxMP);
                writer.WriteUInt16("winGil", enemy.WinGil);
                writer.WriteUInt16("winExp", enemy.WinExp);

                writer.BeginArray("winItems"); // 4
                foreach (Byte item in enemy.WinItems)
                    writer.WriteByteValue(item);
                writer.EndArray();

                writer.BeginArray("stealItems"); // 4
                foreach (Byte item in enemy.StealItems)
                    writer.WriteByteValue(item);
                writer.EndArray();

                writer.WriteUInt16("radius", enemy.Radius);
                writer.WriteUInt16("geo", enemy.Geo);

                writer.BeginArray("mot"); // 6
                foreach (UInt16 item in enemy.Mot)
                    writer.WriteUInt16Value(item);
                writer.EndArray();

                writer.BeginArray("mesh"); // 2
                foreach (UInt16 item in enemy.Mesh)
                    writer.WriteUInt16Value(item);
                writer.EndArray();

                writer.WriteUInt16("flags", enemy.Flags);
                writer.WriteUInt16("ap", enemy.AP);

                writer.BeginObject("element");
                writer.WriteByte("dex", enemy.Element.dex);
                writer.WriteByte("str", enemy.Element.str);
                writer.WriteByte("mgc", enemy.Element.mgc);
                writer.WriteByte("wpr", enemy.Element.wpr);
                writer.WriteByte("pad", enemy.Element.pad);
                writer.WriteByte("trans", enemy.Element.trans);
                writer.WriteByte("curCapa", enemy.Element.cur_capa);
                writer.WriteByte("maxCapa", enemy.Element.max_capa);
                writer.EndObject();

                writer.BeginArray("attributes"); // 4
                foreach (Byte item in enemy.Attr)
                    writer.WriteByteValue(item);
                writer.EndArray();

                writer.WriteByte("level", enemy.Level);
                writer.WriteByte("category", enemy.Category);
                writer.WriteByte("hitRate", enemy.HitRate);
                writer.WriteByte("pdp", enemy.P_DP);
                writer.WriteByte("pav", enemy.P_AV);
                writer.WriteByte("mdp", enemy.M_DP);
                writer.WriteByte("mav", enemy.M_AV);
                writer.WriteByte("blue", enemy.Blue);

                writer.BeginArray("bones"); // 4
                foreach (Byte item in enemy.Bone)
                    writer.WriteByteValue(item);
                writer.EndArray();

                writer.WriteUInt16("dieSfx", enemy.DieSfx);
                writer.WriteByte("Konran", enemy.Konran);
                writer.WriteByte("MesCnt", enemy.MesCnt);

                writer.BeginArray("iconBones"); // 6
                foreach (Byte item in enemy.IconBone)
                    writer.WriteByteValue(item);
                writer.EndArray();

                writer.BeginArray("iconY"); // 6
                foreach (SByte item in enemy.IconY)
                    writer.WriteSByteValue(item);
                writer.EndArray();

                writer.BeginArray("iconZ"); // 6
                foreach (SByte item in enemy.IconZ)
                    writer.WriteSByteValue(item);
                writer.EndArray();

                writer.WriteUInt16("startSfx", enemy.StartSfx);
                writer.WriteUInt16("shadowX", enemy.ShadowX);
                writer.WriteUInt16("shadowZ", enemy.ShadowZ);
                writer.WriteByte("shadowBone", enemy.ShadowBone);
                writer.WriteByte("card", enemy.Card);
                writer.WriteInt16("shadowOfsX", enemy.ShadowOfsX);
                writer.WriteInt16("shadowOfsZ", enemy.ShadowOfsZ);
                writer.WriteByte("shadowBone2", enemy.ShadowBone2);
                writer.WriteByte("pad0", enemy.Pad0);
                writer.WriteUInt16("pad1", enemy.Pad1);
                writer.WriteUInt16("pad2", enemy.Pad2);

                writer.BeginComplexArray("messages");
                for (int i = 0; i < enemy.MesCnt; i++)
                    writer.WriteStringValue(text[currentMessage++]);
                writer.EndComplexArray();
                writer.EndObject();
            }
            writer.EndComplexArray();
        }

        private static void SerializeActions(BTL_SCENE btlScene, JsonWriter writer, String[] text, ref Int32 currentText)
        {
            writer.BeginComplexArray();
            foreach (AA_DATA action in btlScene.atk)
            {
                String name = text[currentText++];

                Actions.Add(new KeyValuePair<string, AA_DATA>(name, action));

                writer.BeginObject();
                writer.WriteString("name", name);
                writer.WriteEnum("targets", (TargetType)action.Info.cursor);
                writer.WriteBoolean("defaultAlly", action.Info.def_cur == 1);
                writer.WriteBoolean("defaultCamera", action.Info.def_cam != 0);
                writer.WriteInt16("animationId1", action.Info.vfx_no);
                writer.WriteUInt16("animationId2", action.Vfx2);
                writer.WriteByte("scriptId", action.Ref.prog_no);
                writer.WriteByte("power", action.Ref.power);
                writer.WriteFlags("elements", (EffectElement)action.Ref.attr);
                writer.WriteByte("rate", action.Ref.rate);
                writer.WriteByte("category", action.Category);
                writer.WriteByte("addNo", action.AddNo);
                writer.WriteByte("mp", action.MP);
                writer.WriteByte("type", action.Type);
                writer.WriteUInt16("name", UInt16.Parse(action.Name));
                writer.EndObject();
            }
            writer.EndComplexArray();
        }

        private static void SerializeAllActions(String outputDirectory)
        {
            UInt16 id = 0;
            using (CsvWriter csv = new CsvWriter(outputDirectory + "Actions.csv"))
            {
                csv.WriteLine("# This file contains NPC actions.");
                csv.WriteLine("# ----------------------------------------------------------------------------------------------------------------------------------");
                csv.WriteLine("# Id;cursor;def_cur;vfx_no;Vfx2;def_cam;prog_no;power;attr;rate;Category;AddNo;MP;Type");
                csv.WriteLine("# UInt16;UInt8;UInt8;Int16;UInt16;UInt8;UInt8;UInt8;UInt8;UInt8;UInt8;UInt8;UInt8;UInt8");
                csv.WriteLine("# ----------------------------------------------------------------------------------------------------------------------------------");

                foreach (var group in Actions.GroupBy(p => p.Key))
                {
                    KeyValuePair<string, AA_DATA>[] items = group.OrderBy(v => ActionComparer.CalcDiff(v, group)).ToArray();
                    KeyValuePair<string, AA_DATA> baseItem = items.First();
                    csv.WriteEntry(new ActionEntry(id, baseItem.Value), baseItem.Key);

                    String name = group.Key;
                    String outputPath = outputDirectory + FF9TextTool.RemoveOpCode(name) + ".json";

                    using (JsonWriter writer = new JsonWriter(outputPath))
                    {
                        writer.BeginComplexArray();
                        foreach (KeyValuePair<String, AA_DATA> pair in items)
                        {
                            AA_DATA action = pair.Value;
                            writer.BeginObject();
                            writer.WriteUInt16("id", id);
                            writer.WriteString("name", name);
                            if (baseItem.Value.Info.cursor != action.Info.cursor)
                                writer.WriteEnum("targets", (TargetType)action.Info.cursor);
                            if (baseItem.Value.Info.def_cur != action.Info.def_cur)
                                writer.WriteBoolean("defaultAlly", action.Info.def_cur == 1);
                            if (baseItem.Value.Info.vfx_no != action.Info.vfx_no)
                                writer.WriteInt16("animationId1", action.Info.vfx_no);
                            if (baseItem.Value.Vfx2 != action.Vfx2)
                                writer.WriteUInt16("animationId2", action.Vfx2);

                            if (baseItem.Value.Ref.prog_no != action.Ref.prog_no)
                                writer.WriteByte("scriptId", action.Ref.prog_no);
                            if (baseItem.Value.Ref.power != action.Ref.power)
                                writer.WriteByte("power", action.Ref.power);
                            if (baseItem.Value.Ref.attr != action.Ref.attr)
                                writer.WriteFlags("elements", (EffectElement)action.Ref.attr);
                            if (baseItem.Value.Ref.rate != action.Ref.rate)
                                writer.WriteByte("rate", action.Ref.rate);

                            if (baseItem.Value.Category != action.Category)
                                writer.WriteByte("category", action.Category);
                            if (baseItem.Value.AddNo != action.AddNo)
                                writer.WriteByte("addNo", action.AddNo);
                            if (baseItem.Value.MP != action.MP)
                                writer.WriteByte("mp", action.MP);
                            if (baseItem.Value.Type != action.Type)
                                writer.WriteByte("type", action.Type);
                            writer.EndObject();
                        }
                        writer.EndComplexArray();
                    }
                    id++;
                }
            }
        }

        private class ActionEntry : ICsvEntry
        {
            private AA_DATA aaData;
            private ushort _index;

            public ActionEntry(ushort index, AA_DATA baseItemValue)
            {
                _index = index;
                aaData = baseItemValue;
            }

            public static AA_DATA Create()
            {
                //name = "0"
                //def_dead = 0
                //deaf = 0
                //sfx_no = 4095
                //sub_win = 0
                return null;
            }

            public void ParseEntry(string[] raw)
            {
                throw new NotImplementedException();
            }

            public void WriteEntry(CsvWriter sw)
            {
                sw.UInt16(_index);
                var cmdInfo = aaData.Info;
                var btlRef = aaData.Ref;
                sw.Byte(cmdInfo.cursor);
                sw.Byte(cmdInfo.def_cur);
                sw.Byte(cmdInfo.def_cam);
                sw.Int16(cmdInfo.vfx_no);
                sw.UInt16(aaData.Vfx2);
                sw.Byte(btlRef.prog_no);
                sw.Byte(btlRef.power);
                sw.Byte(btlRef.attr);
                sw.Byte(btlRef.rate);
                sw.Byte(aaData.Category);
                sw.Byte(aaData.AddNo);
                sw.Byte(aaData.MP);
                sw.Byte(aaData.Type);
            }
        }

        private static void CompileScene(String outputDirectory)
        {
        }
    }

    internal class MonsterComparer : IEqualityComparer<SB2_MON_PARM>
    {
        public bool Equals(SB2_MON_PARM x, SB2_MON_PARM y)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode(SB2_MON_PARM obj)
        {
            throw new NotImplementedException();
        }
    }

    [Flags]
    public enum BattleSceneSpawnFlags : byte
    {
        Target = 1,
        Multi = 2
    }

    [Flags]
    public enum BattleSceneFlags : ushort
    {
        Special = 1,
        BackAtk = 2,
        NoGameOver = 4,
        ExpZero = 8,
        NoWinPose = 16,
        NoRunAway = 32,
        NoNearAtk = 64,
        NoMagical = 128,
        ReverseAtk = 256,
        FixedCam1 = 512,
        FixedCam2 = 1024,
        AfterEvent = 2048,
        MesEvent = 4096,
        FieldBgm = 8192
    }

    public sealed class JsonWriter : IDisposable
    {
        private readonly StreamWriter _streamWriter;
        private readonly Stack<Boolean> _isArray = new Stack<Boolean>();
        private readonly Stack<Boolean> _isArrayFirst = new Stack<Boolean>();
        private Int32 _level;
        private Boolean _hasTag = false;

        public JsonWriter(String outputPath)
        {
            _streamWriter = File.CreateText(outputPath);
            _isArray.Push(false);
        }

        public void Dispose()
        {
            _streamWriter.Dispose();
        }

        public void BeginObject()
        {
            AtValueBegin();
            _isArray.Push(false);
            PadLeft();
            _streamWriter.WriteLine('{');
            _level++;
        }

        private void AtValueBegin()
        {
            if (!_isArray.Peek())
                return;

            if (_isArrayFirst.Peek())
            {
                _isArrayFirst.Pop();
                _isArrayFirst.Push(false);
            }
            else
            {
                _streamWriter.Write(", ");
            }
        }

        public void BeginObject(String tag)
        {
            AtValueBegin();
            _isArray.Push(false);
            WriteTag(tag);
            _streamWriter.WriteLine('{');
            _level++;
        }

        public void EndObject()
        {
            if (_isArray.Pop())
                _isArrayFirst.Pop();
            _level--;
            PadLeft();
            _streamWriter.WriteLine('}');
        }

        public void BeginArray(String tag)
        {
            AtValueBegin();
            _isArray.Push(true);
            _isArrayFirst.Push(true);
            WriteTag(tag);
            _streamWriter.Write('[');
        }

        public void EndArray()
        {
            if (_isArray.Pop())
                _isArrayFirst.Pop();
            _streamWriter.WriteLine(']');
        }

        public void BeginComplexArray(String tag)
        {
            AtValueBegin();
            _isArray.Push(true);
            _isArrayFirst.Push(true);
            WriteTag(tag);
            _streamWriter.WriteLine('[');
            _level++;
        }

        public void BeginComplexArray()
        {
            AtValueBegin();
            _isArray.Push(true);
            _isArrayFirst.Push(true);
            PadLeft();
            _streamWriter.WriteLine('[');
            _level++;
        }

        public void EndComplexArray()
        {
            if (_isArray.Pop())
                _isArrayFirst.Pop();
            _level--;
            PadLeft();
            _streamWriter.WriteLine(']');
        }

        public void WriteBoolean(String tag, Boolean value)
        {
            WriteTag(tag);
            WriteBooleanValue(value);
            WriteLine();
        }

        private void WriteBooleanValue(bool value)
        {
            AtValueBegin();
            _streamWriter.Write(value.ToString(CultureInfo.InvariantCulture));
            AtValueEnd();
        }

        public void WriteByte(String tag, Byte value)
        {
            WriteTag(tag);
            WriteByteValue(value);
            WriteLine();
        }

        public void WriteByteValue(Byte value)
        {
            AtValueBegin();
            _streamWriter.Write(value.ToString(CultureInfo.InvariantCulture));
            AtValueEnd();
        }

        public void WriteSByteValue(SByte value)
        {
            AtValueBegin();
            _streamWriter.Write(value.ToString(CultureInfo.InvariantCulture));
            AtValueEnd();
        }

        public void WriteInt16(String tag, Int16 value)
        {
            WriteTag(tag);
            WriteInt16Value(value);
            WriteLine();
        }

        private void WriteInt16Value(short value)
        {
            AtValueBegin();
            _streamWriter.Write(value.ToString(CultureInfo.InvariantCulture));
            AtValueEnd();
        }

        public void WriteUInt16(String tag, UInt16 value)
        {
            WriteTag(tag);
            WriteUInt16Value(value);
            WriteLine();
        }

        public void WriteUInt16Value(UInt16 value)
        {
            AtValueBegin();
            _streamWriter.Write(value.ToString(CultureInfo.InvariantCulture));
            AtValueEnd();
        }

        public void WriteUInt32(String tag, UInt32 value)
        {
            WriteTag(tag);
            WriteUInt32Value(value);
            WriteLine();
        }

        public void WriteString(String tag, String value)
        {
            WriteTag(tag);
            WriteStringValue(value);
            WriteLine();
        }

    private void WriteUInt32Value(uint value)
        {
            AtValueBegin();
            _streamWriter.Write(value.ToString(CultureInfo.InvariantCulture));
            AtValueEnd();
        }

        public void WriteEnum<T>(String tag, T value) where T : struct
        {
            WriteTag(tag);
            WirteEnumValue(value);
            WriteLine();
        }

        private void WirteEnumValue<T>(T value) where T : struct
        {
            AtValueBegin();
            UInt64 integer = EnumCache<T>.ToUInt64(value);

            _streamWriter.Write('"');
            _streamWriter.Write(JsonEncoder.Encode(value.ToString()));
            _streamWriter.Write('(');
            _streamWriter.Write(integer.ToString(CultureInfo.InvariantCulture));
            _streamWriter.Write(')');
            _streamWriter.Write('"');
            AtValueEnd();
        }

        public void WriteFlags<T>(String tag, T value) where T : struct
        {
            WriteTag(tag);
            BeginArrayValue();
            UInt64 flags = EnumCache<T>.ToUInt64(value);
            Boolean first = true;
            for (int index = 0; index < EnumFlags<T>.Integers.Length; index++)
            {
                ulong flg = EnumFlags<T>.Integers[index];

                if ((flags & flg) == flg)
                {
                    flags &= ~flg;

                    if (first)
                        first = false;
                    else
                        _streamWriter.Write(", ");

                    _streamWriter.Write('"');
                    _streamWriter.Write(JsonEncoder.Encode(EnumFlags<T>.Names[index]));
                    _streamWriter.Write('(');
                    _streamWriter.Write(flg.ToString(CultureInfo.InvariantCulture));
                    _streamWriter.Write(')');
                    _streamWriter.Write('"');
                }
            }

            // Unexpected flags
            if (flags != 0)
            {
                if (!first)
                    _streamWriter.Write(", ");

                _streamWriter.Write('(');
                _streamWriter.Write(flags.ToString(CultureInfo.InvariantCulture));
                _streamWriter.Write(')');
            }

            EndArrayValue();
            _streamWriter.WriteLine();
        }

        private void WriteTag(String tag)
        {
            _hasTag = true;
            PadLeft();
            _streamWriter.Write('"');
            _streamWriter.Write(tag);
            _streamWriter.Write("\": ");
        }

        public void WriteStringValue(String value)
        {
            AtValueBegin();
            _streamWriter.Write('"');
            _streamWriter.Write(JsonEncoder.Encode(value));
            _streamWriter.Write('"');
            AtValueEnd();
        }

        private void AtValueEnd()
        {
            _hasTag = false;
        }

        private void BeginArrayValue()
        {
            _streamWriter.Write('[');
        }

        private void EndArrayValue()
        {
            _streamWriter.Write(']');
        }

        private void PadLeft()
        {
            for (int i = 0; i < _level; i++)
                _streamWriter.Write('\t');
        }

        private void WriteLine()
        {
            _streamWriter.WriteLine();
        }
    }
}
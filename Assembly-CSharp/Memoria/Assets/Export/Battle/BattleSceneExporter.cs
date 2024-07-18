using Assets.Sources.Scripts.UI.Common;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.CSV;
using Memoria.Prime.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Memoria.Assets
{
    public static class BattleSceneExporter
    {
        private sealed class Context
        {
            public HashSet<String> Locations = new HashSet<String>();
        }

        private static Dictionary<KeyValuePair<Dictionary<String, String>, SB2_MON_PARM>, Context> Enemies = new Dictionary<KeyValuePair<Dictionary<String, String>, SB2_MON_PARM>, Context>(EnemyComparer.Instance);
        private static Dictionary<KeyValuePair<Dictionary<String, String>, AA_DATA>, Context> Actions = new Dictionary<KeyValuePair<Dictionary<String, String>, AA_DATA>, Context>(ActionComparer.Instance);

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

                foreach (KeyValuePair<String, Int32> scene in CreateSceneList())
                {
                    FF9StateSystem.Battle.battleMapIndex = scene.Value;
                    ExportSceneSafe(scene.Key);
                }

                SerializeAllEnemies();
                SerializeAllActions();

                AssetManager.UseBundles = old;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "[BattleSceneExporter] Failed to export field resources.");
            }
        }

        private static void ExportSceneSafe(String battleSceneName)
        {
            Boolean exportMap = true;
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

                List<Dictionary<String, String>> text;
                try
                {
                    text = ReadSceneText(battleSceneName);
                }
                catch
                {
                    Log.Error("[BattleSceneExporter] Map not found [{0}].", battleSceneName);
                    return;
                }

                BTL_SCENE btlScene = new BTL_SCENE();
                btlScene.ReadBattleScene(battleSceneName);

                btlseq.btlseqinstance btlSeq = new btlseq.btlseqinstance();
                btlseq.ReadBattleSequence(battleSceneName, ref btlSeq);

                String directoryPath = Path.GetDirectoryName(outputPath);
                if (directoryPath != null)
                    Directory.CreateDirectory(directoryPath);

                SerializeScene(btlScene, btlSeq, outputDirectory, text);
                CompileScene(outputDirectory);

                exportMap = false;

                EmbadedTextResources.CurrentSymbol = "US";
                String[] usBattleText = FF9TextTool.GetBattleText(FF9BattleDB.SceneData["BSC_" + battleSceneName]);
                EmbadedTextResources.CurrentSymbol = null;
                Dictionary<String, BattleActionThread.TextGetter> langBattleText = new Dictionary<String, BattleActionThread.TextGetter>();
                foreach (String symbol in Configuration.Export.Languages)
                    langBattleText[symbol] = index => text[index][symbol];

                for (Int32 i = 0; i < btlScene.header.AtkCount; i++)
                {
                    if (btlSeq.seq_work_set.SeqData[i] != 0)
                    {
                        List<BattleActionThread> atkSeq = BattleActionThread.LoadFromBtlSeq(btlScene, btlSeq, index => usBattleText[index], i, langBattleText);
                        String seqStr = BattleActionThread.GetSequenceStringCode(atkSeq);
                        if (!String.IsNullOrEmpty(seqStr))
                        {
                            String seqPath = outputDirectory + i.ToString("D2") + " " + FF9TextTool.RemoveOpCode(usBattleText[btlScene.header.TypCount + i]) + UnifiedBattleSequencer.EXTENSION_SEQ;
                            File.WriteAllText(seqPath, seqStr);
                        }
                    }
                }

                Log.Message("[BattleSceneExporter] Exporting completed successfully.");
            }
            catch (Exception ex)
            {
                if (exportMap)
                    Log.Error(ex, "[BattleSceneExporter] Failed to export map [{0}].", battleSceneName);
                else
                    Log.Error(ex, "[BattleSceneExporter] Failed to export attack sequences [{0}].", battleSceneName);
            }
        }

        private static List<Dictionary<String, String>> ReadSceneText(String battleSceneName)
        {
            Int32 battleZoneId = FF9BattleDB.SceneData["BSC_" + battleSceneName];

            List<Dictionary<String, String>> result = new List<Dictionary<String, String>>();
            foreach (String symbol in Configuration.Export.Languages)
            {
                EmbadedTextResources.CurrentSymbol = symbol;
                ModTextResources.Export.CurrentSymbol = symbol;

                String textEmbadedPath = EmbadedTextResources.GetCurrentPath("/Battle/" + battleZoneId + ".mes");
                String[] text = EmbadedSentenseLoader.LoadSentense(textEmbadedPath);
                for (Int32 i = 0; i < text.Length; i++)
                {
                    while (result.Count < i + 1)
                        result.Add(new Dictionary<String, String>(Configuration.Export.Languages.Length));

                    result[i].Add(symbol, text[i]);
                }
            }

            return result;
        }

        private static IEnumerable<KeyValuePair<String, Int32>> CreateSceneList()
        {
            foreach (KeyValuePair<String, Int32> str in FF9BattleDB.SceneData)
                yield return new KeyValuePair<String, Int32>(str.Key.Substring(4), str.Value);
        }

        private static void SerializeScene(BTL_SCENE btlScene, btlseq.btlseqinstance btlSeq, String outputDirectory, List<Dictionary<String, String>> text)
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
                SerializeEnemies(outputDirectory, btlScene, writer, text, ref currentText, ref currentMessage);

            outputPath = outputDirectory + "Actions.json";
            using (JsonWriter writer = new JsonWriter(outputPath))
                SerializeActions(outputDirectory, btlScene, writer, text, ref currentText);

            outputPath = outputDirectory + "Scripts.json";
            using (JsonWriter writer = new JsonWriter(outputPath))
                SerializeScripts(outputDirectory, btlScene, btlSeq, writer, text, ref currentText);
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
                writer.WriteByte("enemyCount", pattern.MonsterCount);
                writer.WriteByte("camera", pattern.Camera);
                writer.WriteByte("pad0", pattern.Pad0);
                writer.WriteUInt32("ap", pattern.AP);
                writer.BeginComplexArray("spawn");
                foreach (SB2_PUT point in pattern.Monster)
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

        private static void SerializeEnemies(String outputDirectory, BTL_SCENE btlScene, JsonWriter writer, List<Dictionary<String, String>> localizableText, ref Int32 currentText, ref Int32 currentMessage)
        {
            writer.BeginComplexArray();
            foreach (SB2_MON_PARM enemy in btlScene.MonAddr)
            {
                Dictionary<String, String> names = localizableText[currentText++];

                Context context;
                var key = new KeyValuePair<Dictionary<String, String>, SB2_MON_PARM>(names, enemy);
                if (!Enemies.TryGetValue(key, out context))
                {
                    context = new Context();
                    Enemies.Add(key, context);
                }
                context.Locations.Add(outputDirectory);

                writer.BeginObject();
                writer.WriteString("name", names["US"]);
                writer.WriteUInt32("invalidStatus", (UInt32)enemy.ResistStatus);
                writer.WriteUInt32("permanentStatus", (UInt32)enemy.AutoStatus);
                writer.WriteUInt32("currentStatus", (UInt32)enemy.InitialStatus);
                writer.WriteUInt16("maxHp", (UInt16)enemy.MaxHP);
                writer.WriteUInt16("maxMp", (UInt16)enemy.MaxMP);
                writer.WriteUInt16("winGil", (UInt16)enemy.WinGil);
                writer.WriteUInt16("winExp", (UInt16)enemy.WinExp);

                writer.BeginArray("winItems"); // 4
                foreach (Int32 item in enemy.WinItems)
                    writer.WriteItem(item);
                writer.EndArray();

                writer.BeginArray("stealItems"); // 4
                foreach (Int32 item in enemy.StealItems)
                    writer.WriteItem(item);
                writer.EndArray();

                writer.WriteUInt16("radius", enemy.Radius);
                writer.WriteInt16("geo", enemy.Geo);

                writer.BeginObject("animation");
                for (Int32 i = 0; i < enemy.Mot.Length; i++) // 6
                    writer.WriteUInt16("a" + (i + 1).ToString(CultureInfo.InvariantCulture), enemy.Mot[i]);
                writer.EndObject();

                writer.WriteUInt16OrMinusOne("currentMesh", enemy.Mesh[0]);
                writer.WriteUInt16OrMinusOne("banishMesh", enemy.Mesh[1]);

                writer.WriteUInt16("flags", enemy.Flags);
                writer.WriteUInt16("ap", (UInt16)enemy.AP);

                writer.BeginObject("element");
                writer.WriteByte("dex", enemy.Element.Speed);
                writer.WriteByte("str", enemy.Element.Strength);
                writer.WriteByte("mgc", enemy.Element.Magic);
                writer.WriteByte("wpr", enemy.Element.Spirit);
                writer.EndObject();

                writer.BeginObject("attributes");
                writer.WriteByte("invalid", enemy.GuardElement);
                writer.WriteByte("absorb", enemy.AbsorbElement);
                writer.WriteByte("half", enemy.HalfElement);
                writer.WriteByte("weak", enemy.WeakElement);
                writer.EndObject();

                writer.WriteByte("level", enemy.Level);
                writer.WriteByte("category", enemy.Category);
                writer.WriteByte("hitRate", enemy.HitRate);
                writer.WriteInt32("pdp", enemy.PhysicalDefence);
                writer.WriteInt32("pav", enemy.PhysicalEvade);
                writer.WriteInt32("mdp", enemy.MagicalDefence);
                writer.WriteInt32("mav", enemy.MagicalEvade);
                writer.WriteInt32("blue", enemy.BlueMagic);

                writer.BeginArray("bones"); // 4
                foreach (Byte item in enemy.Bone)
                    writer.WriteByteValue(item);
                writer.EndArray();

                writer.WriteUInt16OrMinusOne("dieSfx", enemy.DieSfx);
                writer.WriteByte("defaultAttack", enemy.Konran);

                writer.BeginObject("iconBones"); // 6
                for (Int32 i = 0; i < 6; i++)
                    writer.WriteString("b" + (i + 1).ToString(CultureInfo.InvariantCulture), String.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2})", enemy.IconBone[i], enemy.IconY[i], enemy.IconZ[i]));
                writer.EndObject();

                writer.WriteUInt16OrMinusOne("startSfx", enemy.StartSfx);
                writer.WriteUInt16("shadowX", enemy.ShadowX);
                writer.WriteUInt16("shadowZ", enemy.ShadowZ);
                writer.WriteByte("shadowBone", enemy.ShadowBone);
                writer.WriteInt32("card", (Int32)enemy.WinCard);
                writer.WriteByte("shadowBone2", enemy.ShadowBone2);

                writer.BeginComplexArray("messages");
                for (Int32 i = 0; i < enemy.MesCnt; i++)
                    writer.WriteStringValue(localizableText[currentMessage++]["US"]);
                writer.EndComplexArray();
                writer.EndObject();
            }
            writer.EndComplexArray();
        }

        private static void SerializeActions(String outputDirectory, BTL_SCENE btlScene, JsonWriter writer, List<Dictionary<String, String>> localizableText, ref Int32 currentText)
        {
            writer.BeginComplexArray();
            foreach (AA_DATA action in btlScene.atk)
            {
                Dictionary<String, String> names = localizableText[currentText++];

                Context context;
                var key = new KeyValuePair<Dictionary<String, String>, AA_DATA>(names, action);
                if (!Actions.TryGetValue(key, out context))
                {
                    context = new Context();
                    Actions.Add(key, context);
                }
                context.Locations.Add(outputDirectory);

                writer.BeginObject();
                writer.WriteString("name", names["US"]);
                writer.WriteEnum("targets", action.Info.Target);
                writer.WriteBoolean("defaultAlly", action.Info.DefaultAlly);
                writer.WriteBoolean("defaultCamera", action.Info.DefaultCamera);
                writer.WriteInt16("animationId1", action.Info.VfxIndex);
                writer.WriteUInt16("animationId2", action.Vfx2);
                writer.WriteInt32("scriptId", action.Ref.ScriptId);
                writer.WriteInt32("power", action.Ref.Power);
                writer.WriteFlags("elements", (EffectElement)action.Ref.Elements);
                writer.WriteInt32("rate", action.Ref.Rate);
                writer.WriteByte("category", action.Category);
                writer.WriteEnum("addNo", action.AddStatusNo);
                writer.WriteInt32("mp", action.MP);
                writer.WriteByte("type", action.Type);
                writer.WriteUInt16("name", UInt16.Parse(action.Name));
                writer.EndObject();
            }
            writer.EndComplexArray();
        }

        private static void SerializeScripts(String outputDirectory, BTL_SCENE btlScene, btlseq.btlseqinstance btlSeq, JsonWriter writer, List<Dictionary<String, String>> localizableText, ref Int32 currentText)
        {
            btlSeq.seq_work_set.CameraNo = (byte)0;

            String outputPath = outputDirectory + "Scripts.cs";
            using (StreamWriter output = File.CreateText(outputPath))
            {
                for (Int32 index = 0; index < btlSeq.seq_work_set.AnmOfsList.Length; index++)
                {
                    SEQ_WORK seqWork = new SEQ_WORK();
                    seqWork.Flags = new SeqFlag();
                    seqWork.CmdPtr = new CMD_DATA();
                    seqWork.OldPtr = 0;
                    seqWork.IncCnt = (short)0;
                    seqWork.DecCnt = (short)0;
                    seqWork.AnmCnt = (short)0;
                    seqWork.SfxTime = (byte)0;
                    seqWork.TurnTime = (byte)0;
                    seqWork.SVfxTime = (byte)0;
                    seqWork.FadeTotal = (byte)0;
                    seqWork.CurPtr = btlSeq.seq_work_set.SeqData[index];
                    seqWork.AnmIDOfs = btlSeq.seq_work_set.AnmOfsList[index];

                    StringBuilder sb = new StringBuilder();

                    using (btlSeq.sequenceReader = new BinaryReader(new MemoryStream(btlSeq.data)))
                    {
                        for (Int32 code = 1; code != 0; code = btlseq1.gSeqProg[btlSeq.wSeqCode].Exec(btlSeq, seqWork, sb))
                        {
                            btlSeq.sequenceReader.BaseStream.Seek(seqWork.CurPtr + 4, SeekOrigin.Begin);
                            btlSeq.wSeqCode = btlSeq.sequenceReader.ReadByte();
                            if (btlSeq.wSeqCode > btlseq.gSeqProg.Length)
                                btlSeq.wSeqCode = 0;
                            if (seqWork.CurPtr != seqWork.OldPtr && btlseq.gSeqProg[btlSeq.wSeqCode].Init != null)
                                btlseq1.gSeqProg[btlSeq.wSeqCode].Init(btlSeq, seqWork, sb);
                            seqWork.OldPtr = seqWork.CurPtr;
                        }
                    }

                    output.WriteLine(index);
                    output.WriteLine(sb.ToString());
                    output.WriteLine();
                }
            }

            writer.BeginObject();

            writer.WriteInt32("camOffset", btlSeq.camOffset);
            writer.WriteInt32("projOffset", FF9StateSystem.Battle.battleMapIndex == 21 ? 100 : 0);

            writer.BeginArray("data");
            foreach (Byte value in btlSeq.data)
                writer.WriteByteValue(value);
            writer.EndArray();

            writer.BeginArray("SeqData");
            foreach (Int32 value in btlSeq.seq_work_set.SeqData)
                writer.WriteInt32Value(value);
            writer.EndArray();

            writer.BeginArray("AnmAddrList");
            foreach (Int32 value in btlSeq.seq_work_set.AnmAddrList)
                writer.WriteInt32Value(value);
            writer.EndArray();

            writer.BeginArray("AnmOfsList");
            foreach (Byte value in btlSeq.seq_work_set.AnmOfsList)
                writer.WriteByteValue(value);
            writer.EndArray();

            writer.BeginComplexArray("sequenceProperty");
            foreach (SequenceProperty item in btlSeq.sequenceProperty)
            {
                writer.BeginObject();
                writer.WriteInt32("Montype", item.Montype);

                writer.BeginArray("PlayableSequence");
                foreach (Int32 value in item.PlayableSequence)
                    writer.WriteInt32Value(value);
                writer.EndArray();

                writer.EndObject();
            }
            writer.EndComplexArray();

            writer.EndObject();
        }

        private static void SerializeAllEnemies()
        {
            String relativePath = "BattleMap/Enemies/";
            String outputDirectory = Path.Combine(Configuration.Export.Path, relativePath);
            Directory.CreateDirectory(outputDirectory);

            UInt16 id = 0;
            CsvMetaData csvOptions = new CsvMetaData();
            using (CsvWriter csv = new CsvWriter(outputDirectory + "Enemies.csv"))
            {
                Dictionary<String, List<TxtEntry>> localizationCsv = new Dictionary<String, List<TxtEntry>>(Configuration.Export.Languages.Length);
                foreach (String symbol in Configuration.Export.Languages)
                {
                    localizationCsv.Add(symbol, new List<TxtEntry>());
                }

                csv.WriteMetaData(csvOptions);
                csv.WriteLine("# This file contains NPCs.");
                csv.WriteLine("# ----------------------------------------------------------------------------------------------------------------------------------");
                csv.WriteLine("# Id;invalidStatus;permanentStatus;currentStatus;maxHp;maxMp;winGil;winExp;winItem1;winItem2;winItem3;winItem4;stealItem1;stealItem2;stealItem3;stealItem4;radius;geo;anim1;anim2;anim3;anim4;anim5;anim6;currentMesh;banishMesh;flags;ap;dex;str;mgc;wpr;invalid;absorb;half;weak;level;category;hitRate;pdp;pav;mdp;mav;blue;camBone1;camBone2;camBone3;targetBone;dieSfx;defaultAttack;iconBone1;iconBone2;iconBone3;iconBone4;iconBone5;iconBone6;startSfx;shadowX;shadowZ;shadowBone;card;shadowBone2;");
                csv.WriteLine("# UInt16;UInt32;UInt32;UInt32;UInt16;UInt16;UInt16;UInt16;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;UInt16;UInt16;UInt16;UInt16;UInt16;UInt16;UInt16;UInt16;UInt16;UInt16;UInt16;UInt16;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;UInt16;Byte;(Byte, SByte, SByte);(Byte, SByte, SByte);(Byte, SByte, SByte);(Byte, SByte, SByte);(Byte, SByte, SByte);(Byte, SByte, SByte);UInt16;UInt16;UInt16;Byte;Byte;Byte;");
                csv.WriteLine("# ----------------------------------------------------------------------------------------------------------------------------------");

                foreach (KeyValuePair<Dictionary<String, String>, SB2_MON_PARM> key in Enemies.Where(a => a.Value.Locations.Count < 2).Select(a => a.Key).ToArray())
                    Enemies.Remove(key);

                foreach (var group in Enemies.Keys.OrderBy(v => v.Value.Level).ThenBy(v => v.Value.MaxHP).ThenBy(v => v.Value.WinExp).GroupBy(p => p.Key["US"]))
                {
                    KeyValuePair<Dictionary<String, String>, SB2_MON_PARM>[] items = group.OrderBy(v => EnemyComparer.CalcDiff(v, group)).ToArray();
                    KeyValuePair<Dictionary<String, String>, SB2_MON_PARM> baseItem = items.First();
                    csv.WriteEntry(new EnemyEntry(id, baseItem.Value), csvOptions, RemovePrefix(baseItem.Key["US"]));

                    foreach (KeyValuePair<String, String> pair in baseItem.Key)
                        localizationCsv[pair.Key].Add(new TxtEntry { Index = id, Prefix = "$battleEnemy", Value = pair.Value });

                    SB2_MON_PARM baseValue = baseItem.Value;
                    Dictionary<String, String> names = group.First().Key;
                    String outputPath = outputDirectory + FF9TextTool.RemoveOpCode(names["US"]) + ".json";

                    using (JsonWriter writer = new JsonWriter(outputPath))
                    {
                        writer.BeginComplexArray();
                        foreach (KeyValuePair<Dictionary<String, String>, SB2_MON_PARM> pair in items)
                        {
                            SB2_MON_PARM enemy = pair.Value;
                            writer.BeginObject();
                            writer.WriteUInt16("id", id);
                            writer.WriteString("name", RemovePrefix(names["US"]));

                            if (baseValue.ResistStatus != enemy.ResistStatus)
                                writer.WriteUInt32("invalidStatus", (UInt32)enemy.ResistStatus);
                            if (baseValue.AutoStatus != enemy.AutoStatus)
                                writer.WriteUInt32("permanentStatus", (UInt32)enemy.AutoStatus);
                            if (baseValue.InitialStatus != enemy.InitialStatus)
                                writer.WriteUInt32("currentStatus", (UInt32)enemy.InitialStatus);
                            if (baseValue.MaxHP != enemy.MaxHP)
                                writer.WriteUInt16("maxHp", (UInt16)enemy.MaxHP);
                            if (baseValue.MaxMP != enemy.MaxMP)
                                writer.WriteUInt16("maxMp", (UInt16)enemy.MaxMP);
                            if (baseValue.WinGil != enemy.WinGil)
                                writer.WriteUInt16("winGil", (UInt16)enemy.WinGil);
                            if (baseValue.WinExp != enemy.WinExp)
                                writer.WriteUInt16("winExp", (UInt16)enemy.WinExp);

                            if (!baseValue.WinItems.SequenceEqual(enemy.WinItems))
                            {
                                writer.BeginObject("winItems");
                                for (Int32 i = 0; i < enemy.WinItems.Length; i++) // 4
                                    if (baseValue.WinItems[i] != enemy.WinItems[i])
                                        writer.WriteItem("i" + (i + 1).ToString(CultureInfo.InvariantCulture), (Int32)enemy.WinItems[i]);
                                writer.EndObject();
                            }

                            if (!baseValue.StealItems.SequenceEqual(enemy.StealItems))
                            {
                                writer.BeginObject("stealItems");
                                for (Int32 i = 0; i < enemy.StealItems.Length; i++) // 4
                                    if (baseValue.StealItems[i] != enemy.StealItems[i])
                                        writer.WriteItem("i" + (i + 1).ToString(CultureInfo.InvariantCulture), (Int32)enemy.StealItems[i]);
                                writer.EndObject();
                            }

                            if (baseValue.Radius != enemy.Radius)
                                writer.WriteUInt16("radius", enemy.Radius);
                            if (baseValue.Geo != enemy.Geo)
                                writer.WriteInt16("geo", enemy.Geo);

                            if (!baseValue.Mot.SequenceEqual(enemy.Mot))
                            {
                                writer.BeginObject("animation");
                                for (Int32 i = 0; i < enemy.Mot.Length; i++) // 6
                                    if (baseValue.Mot[i] != enemy.Mot[i])
                                        writer.WriteUInt16("a" + (i + 1).ToString(CultureInfo.InvariantCulture), enemy.Mot[i]);
                                writer.EndObject();
                            }

                            if (baseValue.Mesh[0] != enemy.Mesh[0])
                                writer.WriteUInt16OrMinusOne("currentMesh", enemy.Mesh[0]);
                            if (baseValue.Mesh[1] != enemy.Mesh[1])
                                writer.WriteUInt16OrMinusOne("banishMesh", enemy.Mesh[1]);

                            if (baseValue.Flags != enemy.Flags)
                                writer.WriteUInt16("flags", enemy.Flags);
                            if (baseValue.AP != enemy.AP)
                                writer.WriteUInt16("ap", (UInt16)enemy.AP);

                            if (!EnemyComparer.EqualElements(baseValue.Element, enemy.Element))
                            {
                                writer.BeginObject("element");
                                if (baseValue.Element.Speed != enemy.Element.Speed)
                                    writer.WriteByte("dex", enemy.Element.Speed);
                                if (baseValue.Element.Strength != enemy.Element.Strength)
                                    writer.WriteByte("str", enemy.Element.Strength);
                                if (baseValue.Element.Magic != enemy.Element.Magic)
                                    writer.WriteByte("mgc", enemy.Element.Magic);
                                if (baseValue.Element.Spirit != enemy.Element.Spirit)
                                    writer.WriteByte("wpr", enemy.Element.Spirit);
                                writer.EndObject();
                            }

                            if (baseValue.GuardElement != enemy.GuardElement || baseValue.AbsorbElement != enemy.AbsorbElement || baseValue.HalfElement != enemy.HalfElement || baseValue.WeakElement != enemy.WeakElement)
                            {
                                writer.BeginObject("attributes");
                                if (baseValue.GuardElement != enemy.GuardElement)
                                    writer.WriteByte("invalid", enemy.GuardElement);
                                if (baseValue.AbsorbElement != enemy.AbsorbElement)
                                    writer.WriteByte("absorb", enemy.AbsorbElement);
                                if (baseValue.HalfElement != enemy.HalfElement)
                                    writer.WriteByte("half", enemy.HalfElement);
                                if (baseValue.WeakElement != enemy.WeakElement)
                                    writer.WriteByte("weak", enemy.WeakElement);
                                writer.EndObject();
                            }

                            if (baseValue.Level != enemy.Level)
                                writer.WriteByte("level", enemy.Level);
                            if (baseValue.Category != enemy.Category)
                                writer.WriteByte("category", enemy.Category);
                            if (baseValue.HitRate != enemy.HitRate)
                                writer.WriteByte("hitRate", enemy.HitRate);
                            if (baseValue.PhysicalDefence != enemy.PhysicalDefence)
                                writer.WriteInt32("pdp", enemy.PhysicalDefence);
                            if (baseValue.PhysicalEvade != enemy.PhysicalEvade)
                                writer.WriteInt32("pav", enemy.PhysicalEvade);
                            if (baseValue.MagicalDefence != enemy.MagicalDefence)
                                writer.WriteInt32("mdp", enemy.MagicalDefence);
                            if (baseValue.MagicalEvade != enemy.MagicalEvade)
                                writer.WriteInt32("mav", enemy.MagicalEvade);
                            if (baseValue.BlueMagic != enemy.BlueMagic)
                                writer.WriteInt32("blue", enemy.BlueMagic);

                            if (!baseValue.Bone.SequenceEqual(enemy.Bone))
                            {
                                writer.BeginObject("bones");
                                if (baseValue.Bone[0] != enemy.Bone[0])
                                    writer.WriteByte("cam1", baseValue.Bone[0]);
                                if (baseValue.Bone[1] != enemy.Bone[1])
                                    writer.WriteByte("cam2", baseValue.Bone[1]);
                                if (baseValue.Bone[2] != enemy.Bone[2])
                                    writer.WriteByte("cam3", baseValue.Bone[2]);
                                if (baseValue.Bone[3] != enemy.Bone[3])
                                    writer.WriteByte("target", baseValue.Bone[3]);
                                writer.EndObject();
                            }

                            if (baseValue.DieSfx != enemy.DieSfx)
                                writer.WriteUInt16OrMinusOne("dieSfx", enemy.DieSfx);
                            if (baseValue.Konran != enemy.Konran)
                                writer.WriteByte("defaultAttack", enemy.Konran);

                            if (!baseValue.IconBone.SequenceEqual(enemy.IconBone) || !baseValue.IconY.SequenceEqual(enemy.IconY) || !baseValue.IconZ.SequenceEqual(enemy.IconZ))
                            {
                                writer.BeginObject("iconBones"); // 6
                                for (Int32 i = 0; i < 6; i++)
                                {
                                    if (baseValue.IconBone[i] != enemy.IconBone[i] || baseValue.IconY[i] != enemy.IconY[i] || baseValue.IconZ[i] != enemy.IconZ[i])
                                        writer.WriteString("b" + (i + 1).ToString(CultureInfo.InvariantCulture), String.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2})", enemy.IconBone[i], enemy.IconY[i], enemy.IconZ[i]));
                                }
                                writer.EndObject();
                            }

                            if (baseValue.StartSfx != enemy.StartSfx)
                                writer.WriteUInt16OrMinusOne("startSfx", enemy.StartSfx);
                            if (baseValue.ShadowX != enemy.ShadowX)
                                writer.WriteUInt16("shadowX", enemy.ShadowX);
                            if (baseValue.ShadowZ != enemy.ShadowZ)
                                writer.WriteUInt16("shadowZ", enemy.ShadowZ);
                            if (baseValue.ShadowBone != enemy.ShadowBone)
                                writer.WriteByte("shadowBone", enemy.ShadowBone);
                            if (baseValue.WinCard != enemy.WinCard)
                                writer.WriteInt32("card", (Int32)enemy.WinCard);
                            if (baseValue.ShadowBone2 != enemy.ShadowBone2)
                                writer.WriteByte("shadowBone2", enemy.ShadowBone2);

                            writer.EndObject();
                        }
                        writer.EndComplexArray();
                    }
                    id++;
                }

                foreach (KeyValuePair<String, List<TxtEntry>> pair in localizationCsv)
                {
                    TextResourcePath outputPath = TextResourcePath.ForExport(outputDirectory + $"Enemies.{pair.Key}");
                    outputPath.WriteAll(pair.Value);
                }
            }
        }

        private static void SerializeAllActions()
        {
            String relativePath = "BattleMap/Actions/";
            String outputDirectory = Path.Combine(Configuration.Export.Path, relativePath);
            Directory.CreateDirectory(outputDirectory);

            UInt16 id = 0;
            CsvMetaData csvOptions = new CsvMetaData();
            using (CsvWriter csv = new CsvWriter(outputDirectory + "Actions.csv"))
            {
                Dictionary<String, List<TxtEntry>> localizationCsv = new Dictionary<String, List<TxtEntry>>(Configuration.Export.Languages.Length);
                foreach (String symbol in Configuration.Export.Languages)
                {
                    localizationCsv.Add(symbol, new List<TxtEntry>());
                }

                csv.WriteMetaData(csvOptions);
                csv.WriteLine("# This file contains NPC actions.");
                csv.WriteLine("# ----------------------------------------------------------------------------------------------------------------------------------");
                csv.WriteLine("# Id;cursor;def_cur;vfx_no;Vfx2;def_cam;prog_no;power;attr;rate;Category;AddNo;MP;Type");
                csv.WriteLine("# UInt16;UInt8;UInt8;Int16;UInt16;UInt8;UInt8;UInt8;UInt8;UInt8;UInt8;UInt8;UInt8;UInt8");
                csv.WriteLine("# ----------------------------------------------------------------------------------------------------------------------------------");

                foreach (KeyValuePair<Dictionary<String, String>, AA_DATA> key in Actions.Where(a => a.Value.Locations.Count < 2).Select(a => a.Key).ToArray())
                    Actions.Remove(key);

                foreach (var group in Actions.Keys.OrderByDescending(v => v.Value.Ref.Elements)
                    .ThenBy(v => v.Value.Ref.ScriptId == 64)
                    .ThenBy(v => v.Value.Ref.ScriptId < 9)
                    .ThenByDescending(v => v.Value.MP > 0)
                    .ThenBy(v => v.Value.Type)
                    .ThenBy(v => v.Value.AddStatusNo)
                    .ThenByDescending(v => v.Value.Ref.ScriptId)
                    .ThenBy(v => v.Value.Ref.Power)
                    .ThenBy(v => v.Value.Info.Target)
                    .ThenBy(v => v.Value.Info.DefaultAlly)
                    .ThenBy(v => v.Value.Category)
                    .ThenBy(v => v.Key["US"])
                    .ThenBy(v => v.Value.MP)
                    .GroupBy(p => p.Key["US"]))
                {
                    KeyValuePair<Dictionary<String, String>, AA_DATA>[] items = group.OrderBy(v => ActionComparer.CalcDiff(v, group)).ToArray();
                    KeyValuePair<Dictionary<String, String>, AA_DATA> baseItem = items.First();
                    csv.WriteEntry(new ActionEntry(id, baseItem.Value), csvOptions, RemovePrefix(baseItem.Key["US"]));

                    foreach (KeyValuePair<String, String> pair in baseItem.Key)
                        localizationCsv[pair.Key].Add(new TxtEntry { Index = id, Prefix = "$battleAction", Value = pair.Value });

                    Dictionary<String, String> names = group.First().Key;
                    String outputPath = outputDirectory + FF9TextTool.RemoveOpCode(names["US"]) + ".json";

                    using (JsonWriter writer = new JsonWriter(outputPath))
                    {
                        writer.BeginComplexArray();
                        foreach (KeyValuePair<Dictionary<String, String>, AA_DATA> pair in items)
                        {
                            AA_DATA action = pair.Value;
                            writer.BeginObject();
                            writer.WriteUInt16("id", id);
                            writer.WriteString("name", RemovePrefix(names["US"]));
                            if (baseItem.Value.Info.Target != action.Info.Target)
                                writer.WriteEnum("targets", action.Info.Target);
                            if (baseItem.Value.Info.DefaultAlly != action.Info.DefaultAlly)
                                writer.WriteBoolean("defaultAlly", action.Info.DefaultAlly);
                            if (baseItem.Value.Info.VfxIndex != action.Info.VfxIndex)
                                writer.WriteInt16("animationId1", action.Info.VfxIndex);
                            if (baseItem.Value.Vfx2 != action.Vfx2)
                                writer.WriteUInt16("animationId2", action.Vfx2);

                            if (baseItem.Value.Ref.ScriptId != action.Ref.ScriptId)
                                writer.WriteInt32("scriptId", action.Ref.ScriptId);
                            if (baseItem.Value.Ref.Power != action.Ref.Power)
                                writer.WriteInt32("power", action.Ref.Power);
                            if (baseItem.Value.Ref.Elements != action.Ref.Elements)
                                writer.WriteFlags("elements", (EffectElement)action.Ref.Elements);
                            if (baseItem.Value.Ref.Rate != action.Ref.Rate)
                                writer.WriteInt32("rate", action.Ref.Rate);

                            if (baseItem.Value.Category != action.Category)
                                writer.WriteByte("category", action.Category);
                            if (baseItem.Value.AddStatusNo != action.AddStatusNo)
                                writer.WriteEnum("addNo", action.AddStatusNo);
                            if (baseItem.Value.MP != action.MP)
                                writer.WriteInt32("mp", action.MP);
                            if (baseItem.Value.Type != action.Type)
                                writer.WriteByte("type", action.Type);
                            writer.EndObject();
                        }
                        writer.EndComplexArray();
                    }
                    id++;
                }

                foreach (KeyValuePair<String, List<TxtEntry>> pair in localizationCsv)
                {
                    TextResourcePath outputPath = TextResourcePath.ForExport(outputDirectory + $"Actions.{pair.Key}.tmp"); // Don't remove .tmp or "key" will be truncated
                    outputPath.WriteAll(pair.Value);
                }
            }
        }

        private static String RemovePrefix(String value)
        {
            Int32 index = value.IndexOf(']');
            if (index > 0 && index + 1 < value.Length)
                return value.Substring(index + 1);
            return value;
        }

        private class ActionEntry : ICsvEntry
        {
            private AA_DATA _aaData;
            private UInt16 _index;

            public ActionEntry(UInt16 index, AA_DATA baseItemValue)
            {
                _index = index;
                _aaData = baseItemValue;
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

            public void ParseEntry(String[] raw, CsvMetaData metadata)
            {
                throw new NotImplementedException();
            }

            public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
            {
                sw.UInt16(_index);
                var cmdInfo = _aaData.Info;
                var btlRef = _aaData.Ref;
                sw.EnumValue(cmdInfo.Target);
                sw.Boolean(cmdInfo.DefaultAlly);
                sw.Boolean(cmdInfo.DefaultCamera);
                sw.Int16(cmdInfo.VfxIndex);
                sw.UInt16(_aaData.Vfx2);
                sw.Int32(btlRef.ScriptId);
                sw.Int32(btlRef.Power);
                sw.Byte(btlRef.Elements);
                sw.Int32(btlRef.Rate);
                sw.Byte(_aaData.Category);
                sw.EnumValue(_aaData.AddStatusNo);
                sw.Int32(_aaData.MP);
                sw.Byte(_aaData.Type);
            }
        }

        private sealed class LocalizationEntry : ICsvEntry
        {
            public UInt16 Id;
            public String Text;

            public void ParseEntry(String[] raw, CsvMetaData metadata)
            {
                Id = CsvParser.UInt16(raw[0]);
                Text = raw[1];
            }

            public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
            {
                sw.UInt16(Id);
                sw.String(Text);
            }
        }

        private class EnemyEntry : ICsvEntry
        {
            private SB2_MON_PARM _enData;
            private UInt16 _index;

            public EnemyEntry(UInt16 index, SB2_MON_PARM baseItemValue)
            {
                _index = index;
                _enData = baseItemValue;
            }

            public static SB2_MON_PARM Create()
            {
                //name = "0"
                return null;
            }

            public void ParseEntry(String[] raw, CsvMetaData metadata)
            {
                throw new NotImplementedException();
            }

            public void WriteEntry(CsvWriter sw, CsvMetaData metadata)
            {
                sw.UInt16(_index);

                sw.UInt32((UInt32)_enData.ResistStatus);
                sw.UInt32((UInt32)_enData.AutoStatus);
                sw.UInt32((UInt32)_enData.InitialStatus);
                sw.UInt16((UInt16)_enData.MaxHP);
                sw.UInt16((UInt16)_enData.MaxMP);
                sw.UInt16((UInt16)_enData.WinGil);
                sw.UInt16((UInt16)_enData.WinExp);

                // 4
                foreach (Int32 item in _enData.WinItems)
                    sw.Item(item);

                // 4
                foreach (Int32 item in _enData.StealItems)
                    sw.Item(item);

                sw.UInt16(_enData.Radius);
                sw.Int16(_enData.Geo);

                // 6
                foreach (UInt16 anim in _enData.Mot)
                    sw.UInt16(anim);

                // 2
                foreach (UInt16 mesh in _enData.Mesh)
                    sw.UInt16(mesh);

                sw.UInt16(_enData.Flags);
                sw.UInt16((UInt16)_enData.AP);

                sw.Byte(_enData.Element.Speed);
                sw.Byte(_enData.Element.Strength);
                sw.Byte(_enData.Element.Magic);
                sw.Byte(_enData.Element.Spirit);

                sw.Byte(_enData.GuardElement);
                sw.Byte(_enData.AbsorbElement);
                sw.Byte(_enData.HalfElement);
                sw.Byte(_enData.WeakElement);

                sw.Byte(_enData.Level);
                sw.Byte(_enData.Category);
                sw.Byte(_enData.HitRate);
                sw.Int32(_enData.PhysicalDefence);
                sw.Int32(_enData.PhysicalEvade);
                sw.Int32(_enData.MagicalDefence);
                sw.Int32(_enData.MagicalEvade);
                sw.Int32(_enData.BlueMagic);

                // 4
                foreach (Byte bone in _enData.Bone)
                    sw.Byte(bone);

                sw.UInt16OrMinusOne(_enData.DieSfx);
                sw.Byte(_enData.Konran);

                // 6
                for (Int32 i = 0; i < 6; i++)
                    sw.String(String.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2})", _enData.IconBone[i], _enData.IconY[i], _enData.IconZ[i]));

                sw.UInt16OrMinusOne(_enData.StartSfx);
                sw.UInt16(_enData.ShadowX);
                sw.UInt16(_enData.ShadowZ);
                sw.Byte(_enData.ShadowBone);
                sw.Int32((Int32)_enData.WinCard);
                sw.Byte(_enData.ShadowBone2);
            }
        }

        private static void CompileScene(String outputDirectory)
        {
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

        private void WriteBooleanValue(Boolean value)
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

        public void WriteByteOrMinusOne(String tag, Byte value)
        {
            WriteTag(tag);
            WriteByteValueOrMinusOne(value);
            WriteLine();
        }

        public void WriteByteValueOrMinusOne(Byte value)
        {
            AtValueBegin();
            if (value == Byte.MaxValue)
                _streamWriter.Write("-1");
            else
                _streamWriter.Write(value.ToString(CultureInfo.InvariantCulture));
            AtValueEnd();
        }

        public void WriteItem(String tag, Int32 value)
        {
            WriteTag(tag);
            WriteItem(value);
            WriteLine();
        }

        public void WriteItem(Int32 value)
        {
            AtValueBegin();
            if (value == Byte.MaxValue)
                _streamWriter.Write("-1");
            else
                _streamWriter.Write(value.ToString(CultureInfo.InvariantCulture));
            AtValueEnd();
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

        private void WriteInt16Value(Int16 value)
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

        public void WriteUInt16OrMinusOne(String tag, UInt16 value)
        {
            WriteTag(tag);
            WriteUInt16ValueOrMinusOne(value);
            WriteLine();
        }

        public void WriteUInt16Value(UInt16 value)
        {
            AtValueBegin();
            _streamWriter.Write(value.ToString(CultureInfo.InvariantCulture));
            AtValueEnd();
        }

        public void WriteUInt16ValueOrMinusOne(UInt16 value)
        {
            AtValueBegin();
            if (value == UInt16.MaxValue)
                _streamWriter.Write("-1");
            else
                _streamWriter.Write(value.ToString(CultureInfo.InvariantCulture));
            AtValueEnd();
        }

        public void WriteInt32(String tag, Int32 value)
        {
            WriteTag(tag);
            WriteInt32Value(value);
            WriteLine();
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

        public void WriteInt32Value(Int32 value)
        {
            AtValueBegin();
            _streamWriter.Write(value.ToString(CultureInfo.InvariantCulture));
            AtValueEnd();
        }

        private void WriteUInt32Value(UInt32 value)
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
            for (Int32 index = 0; index < EnumFlags<T>.Integers.Length; index++)
            {
                UInt64 flg = EnumFlags<T>.Integers[index];

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
            for (Int32 i = 0; i < _level; i++)
                _streamWriter.Write('\t');
        }

        private void WriteLine()
        {
            _streamWriter.WriteLine();
        }
    }

    public class btlseq1
    {
        public static SequenceProgram[] gSeqProg = new SequenceProgram[34]
        {
            new SequenceProgram(null, _SeqExecEnd),
            new SequenceProgram(SeqInitWait, SeqExecWait),
            new SequenceProgram(null, _SeqExecCalc),
            new SequenceProgram(SeqInitMoveToTarget, SeqExecMoveToTarget),
            new SequenceProgram(SeqInitMoveToTurn, SeqExecMoveToTarget),
            new SequenceProgram(null, _SeqExecAnim),
            new SequenceProgram(null, _SeqExecSVfx),
            new SequenceProgram(null, _SeqExecWaitAnim),
            new SequenceProgram(null, _SeqExecVfx),
            new SequenceProgram(null, _SeqExecWaitLoadVfx),
            new SequenceProgram(null, _SeqExecStartVfx),
            new SequenceProgram(null, _SeqExecWaitVfx),
            new SequenceProgram(SeqInitScale, SeqExecScale),
            new SequenceProgram(null, _SeqExecMeshHide),
            new SequenceProgram(null, _SeqExecMessage),
            new SequenceProgram(null, _SeqExecMeshShow),
            new SequenceProgram(null, _SeqExecSetCamera),
            new SequenceProgram(null, _SeqExecDefaultIdle),
            new SequenceProgram(null, _SeqExecRunCamera),
            new SequenceProgram(SeqInitMoveToPoint, SeqExecMoveToPoint),
            new SequenceProgram(null, _SeqExecTurn),
            new SequenceProgram(null, _SeqExecTexAnimPlay),
            new SequenceProgram(null, _SeqExecTexAnimOnce),
            new SequenceProgram(null, _SeqExecTexAnimStop),
            new SequenceProgram(null, _SeqExecFastEnd),
            new SequenceProgram(null, _SeqExecSfx),
            new SequenceProgram(null, _SeqExecVfx),
            new SequenceProgram(SeqInitMoveToOffset, SeqExecMoveToPoint),
            new SequenceProgram(null, _SeqExecTargetBone),
            new SequenceProgram(null, _SeqExecFadeOut),
            new SequenceProgram(SeqInitMoveToTarget, SeqExecMoveToTarget),
            new SequenceProgram(null, _SeqExecShadow),
            new SequenceProgram(null, _SeqExecRunCamera),
            new SequenceProgram(null, _SeqExecMessage)
        };

        private static Int32 _SeqExecEnd(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            pMe.AppendLine($"SeqExecEnd();");
            pSeqWork.CmdPtr = null;
            return 0;
        }

        public static void SeqInitMoveToTarget(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            WK_MOVE wMove = new WK_MOVE();
            wMove.Next = (ushort)4;
            pSeqWork.IncCnt = (short)1;
            wMove.Frames = (short)btlSeq.sequenceReader.ReadByte();
            short num1 = btlSeq.sequenceReader.ReadInt16();
            pSeqWork.Work = btlseq.SequenceConverter.WkMoveToWork(wMove);
            pMe.AppendLine($"SeqInitMoveToTarget{btlSeq.wSeqCode}(frames: {wMove.Frames}, dst: {num1});");
        }

        public static Int32 SeqExecMoveToTarget(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            WK_MOVE wkMove2 = btlseq.SequenceConverter.WorkToWkMove(pSeqWork.Work);
            pSeqWork.CurPtr += wkMove2.Next;
            return 1;
        }

        public static void SeqInitMoveToTurn(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            WK_MOVE wMove = new WK_MOVE();
            wMove.Next = 2;
            pSeqWork.IncCnt = 1;
            wMove.Frames = btlSeq.sequenceReader.ReadByte();
            pSeqWork.Work = btlseq.SequenceConverter.WkMoveToWork(wMove);
            pMe.AppendLine($"SeqInitMoveToTurn(frames: {wMove.Frames});");
        }

        public static void SeqInitScale(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            WK_SCALE wScale = new WK_SCALE();
            wScale.Org = btlSeq.sequenceReader.ReadInt16();
            wScale.Frames = btlSeq.sequenceReader.ReadByte();
            pSeqWork.IncCnt = 1;
            pSeqWork.Work = btlseq.SequenceConverter.WkScaleToWork(wScale);
            pMe.AppendLine($"SeqInitScale(frames: {wScale.Frames}, num1: {wScale.Org});");
        }

        public static Int32 SeqExecScale(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            WK_SCALE wkScale = btlseq.SequenceConverter.WorkToWkScale(pSeqWork.Work);
            pSeqWork.CurPtr += 4;
            return 1;
        }

        public static void SeqInitMoveToPoint(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            WK_MOVE wkMove = btlseq.SequenceConverter.WorkToWkMove(pSeqWork.Work);
            wkMove.Next = 8;
            pSeqWork.IncCnt = 1;
            wkMove.Frames = btlSeq.sequenceReader.ReadByte();
            pSeqWork.Work = btlseq.SequenceConverter.WkMoveToWork(wkMove);
            wkMove.Dst[0] = btlSeq.sequenceReader.ReadInt16();
            wkMove.Dst[1] = btlSeq.sequenceReader.ReadInt16();
            wkMove.Dst[2] = btlSeq.sequenceReader.ReadInt16();
            pMe.AppendLine($"SeqInitMoveToPoint(framse: {wkMove.Frames}, x: {wkMove.Dst[0]}, y: {wkMove.Dst[1]}, z: {wkMove.Dst[2]});");
        }

        public static Int32 SeqExecMoveToPoint(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            WK_MOVE wkMove2 = btlseq.SequenceConverter.WorkToWkMove(pSeqWork.Work);
            pSeqWork.CurPtr += wkMove2.Next;
            return 1;
        }

        public static void SeqInitMoveToOffset(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            WK_MOVE wMove = new WK_MOVE();
            wMove.Next = 8;
            pSeqWork.IncCnt = 1;
            wMove.Frames = btlSeq.sequenceReader.ReadByte();
            pSeqWork.Work = btlseq.SequenceConverter.WkMoveToWork(wMove);
            var x = btlSeq.sequenceReader.ReadInt16();
            var y = btlSeq.sequenceReader.ReadInt16();
            var z = btlSeq.sequenceReader.ReadInt16();
            pMe.AppendLine($"SeqInitMoveToOffset(frames: {wMove.Frames}, x: {x}, y: {y}, z: {z});");
        }

        public static void SeqInitWait(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            pSeqWork.DecCnt = btlSeq.sequenceReader.ReadByte();
            pMe.AppendLine($"SeqInitWait(decCnt: {pSeqWork.DecCnt});");
        }

        public static Int32 SeqExecWait(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            pSeqWork.CurPtr += 2;
            return 1;
        }

        public static Int32 _SeqExecCalc(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            pMe.AppendLine($"SeqExecCalc();");
            ++pSeqWork.CurPtr;
            return 1;
        }

        public static Int32 _SeqExecAnim(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            Byte num = btlSeq.sequenceReader.ReadByte();
            pMe.AppendLine($"SeqExecAnim(num: {num});");

            pSeqWork.AnmCnt = 0;
            pSeqWork.CurPtr += 2;
            return 1;
        }

        public static Int32 _SeqExecSVfx(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            pSeqWork.SVfxNum = btlSeq.sequenceReader.ReadUInt16();
            pSeqWork.SVfxParam = btlSeq.sequenceReader.ReadByte();
            pSeqWork.SVfxTime = (Byte)(btlSeq.sequenceReader.ReadByte() + 1U);
            pSeqWork.CurPtr += 5;

            pMe.AppendLine($"SeqExecSVfx(svfxNum: {pSeqWork.SVfxNum}, svfxParam: {pSeqWork.SVfxParam}, svfxTime: {pSeqWork.SVfxTime});");
            return 1;
        }

        public static Int32 _SeqExecWaitAnim(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            ++pSeqWork.CurPtr;
            pMe.AppendLine($"SeqExecWaitAnim();");
            return 1;
        }

        public static Int32 _SeqExecVfx(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            Int16[] numArray = new Int16[4];
            UInt16 num = btlSeq.sequenceReader.ReadUInt16();
            numArray[0] = btlSeq.sequenceReader.ReadInt16();
            numArray[1] = btlSeq.sequenceReader.ReadInt16();
            numArray[2] = btlSeq.sequenceReader.ReadInt16();
            numArray[3] = btlSeq.wSeqCode != 26 ? (Int16)0 : (Int16)1;
            pSeqWork.CurPtr += 9;
            pMe.AppendLine($"SeqExecVfx(fxNo: {num}, a0: {numArray[0]}, a1: {numArray[1]}, a2: {numArray[2]}, a3: {numArray[3]})");
            return 1;
        }

        public static Int32 _SeqExecWaitLoadVfx(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            pMe.AppendLine($"SeqExecWaitLoadVfx();");
            ++pSeqWork.CurPtr;
            return 1;
        }

        public static Int32 _SeqExecStartVfx(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            pMe.AppendLine($"SeqExecStartVfx();");
            ++pSeqWork.CurPtr;
            return 1;
        }

        public static Int32 _SeqExecWaitVfx(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            pMe.AppendLine($"SeqExecWaitVfx();");
            ++pSeqWork.CurPtr;
            return 1;
        }

        public static Int32 _SeqExecMeshHide(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            UInt16 mesh = btlSeq.sequenceReader.ReadUInt16();
            pSeqWork.CurPtr += 3;
            pMe.AppendLine($"SeqExecMeshHide(mesh: {mesh});");
            return 1;
        }

        public static Int32 _SeqExecMessage(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            btlseq.BattleLog("SeqExecMessage");
            UInt16 num1 = btlSeq.sequenceReader.ReadByte();
            if ((num1 & 128) != 0)
            {
                pMe.AppendLine($"SeqExecMessage_Command();");
            }
            else if (btlSeq.wSeqCode == 33)
            {
                pMe.AppendLine($"SeqExecMessage_Title(message: {num1});");
            }
            else
            {
                pMe.AppendLine($"SeqExecMessage_Message(message: {num1});");
            }

            pSeqWork.CurPtr += 2;
            return 1;
        }

        public static Int32 _SeqExecMeshShow(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            UInt16 mesh = btlSeq.sequenceReader.ReadUInt16();
            pMe.AppendLine($"SeqExecMeshShow(mesh: {mesh});");
            pSeqWork.CurPtr += 3;
            return 1;
        }

        public static Int32 _SeqExecSetCamera(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            btlSeq.seq_work_set.CameraNo = btlSeq.sequenceReader.ReadByte();
            pMe.AppendLine($"SeqExecSetCamera(cameraNo: {btlSeq.seq_work_set.CameraNo});");
            pSeqWork.CurPtr += 2;
            return 1;
        }

        public static Int32 _SeqExecDefaultIdle(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            Byte defIdle = btlSeq.sequenceReader.ReadByte();
            pMe.AppendLine($"SeqExecDefaultIdle(defIdle: {defIdle});");
            pSeqWork.CurPtr += 2;
            return 1;
        }

        public static Int32 _SeqExecRunCamera(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            Byte cameraNo = btlSeq.sequenceReader.ReadByte();
            pMe.AppendLine($"SeqExecRunCamera{btlSeq.wSeqCode}(cameraNo: {cameraNo});");
            pSeqWork.CurPtr += 2;
            return 1;
        }

        public static Int32 _SeqExecTurn(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            Int16 num1 = btlSeq.sequenceReader.ReadInt16();
            Int16 num2 = (Int16)(btlSeq.sequenceReader.ReadInt16() / 4096.0 * 360.0);
            pSeqWork.TurnTime = btlSeq.sequenceReader.ReadByte();

            pMe.AppendLine($"SeqExecTurn(num1: {num1}, num2: {num2}, turnTime: {pSeqWork.TurnTime});");
            pSeqWork.CurPtr += 6;
            return 1;
        }

        public static Int32 _SeqExecTexAnimPlay(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            pMe.AppendLine($"SeqExecTexAnimPlay(animationNo: {btlSeq.sequenceReader.ReadByte()});");
            pSeqWork.CurPtr += 2;
            return 1;
        }

        public static Int32 _SeqExecTexAnimOnce(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            pMe.AppendLine($"SeqExecTexAnimOnce(animationNo: {btlSeq.sequenceReader.ReadByte()});");
            pSeqWork.CurPtr += 2;
            return 1;
        }

        public static Int32 _SeqExecTexAnimStop(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            pMe.AppendLine($"SeqExecTexAnimStop(animationNo: {btlSeq.sequenceReader.ReadByte()});");
            pSeqWork.CurPtr += 2;
            return 1;
        }

        public static Int32 _SeqExecFastEnd(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            pMe.AppendLine($"SeqExecFastEnd();");
            pSeqWork.CmdPtr = null;
            return 0;
        }

        public static Int32 _SeqExecSfx(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            pSeqWork.SfxNum = btlSeq.sequenceReader.ReadUInt16();
            pSeqWork.SfxTime = (Byte)(btlSeq.sequenceReader.ReadByte() + 1U);
            btlSeq.sequenceReader.Read();
            pSeqWork.SfxVol = btlSeq.sequenceReader.ReadByte();

            pMe.AppendLine($"SeqExecSfx(sfxNum: {pSeqWork.SfxNum}, sfxTime: {pSeqWork.SfxTime}, sfxVol: {pSeqWork.SfxVol});");
            pSeqWork.CurPtr += 6;
            return 1;
        }

        public static Int32 _SeqExecTargetBone(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            Byte tarBone = btlSeq.sequenceReader.ReadByte();
            pMe.AppendLine($"SeqExecTargetBone(tarBone: {tarBone});");
            pSeqWork.CurPtr += 2;
            return 1;
        }

        public static Int32 _SeqExecFadeOut(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            pSeqWork.FadeTotal = pSeqWork.FadeStep = btlSeq.sequenceReader.ReadByte();
            pMe.AppendLine($"SeqExecFadeOut(fadeTotal: {pSeqWork.FadeTotal});");
            pSeqWork.CurPtr += 2;
            return 1;
        }

        public static Int32 _SeqExecShadow(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe)
        {
            Byte shadow = btlSeq.sequenceReader.ReadByte();
            pMe.AppendLine($"SeqExecShadow(shadow: {shadow});");
            pSeqWork.CurPtr += 2;
            return 1;
        }

        public class SequenceProgram
        {
            public InitEvent Init;
            public ExecEvent Exec;

            public SequenceProgram(InitEvent init, ExecEvent exec)
            {
                this.Init = init;
                this.Exec = exec;
            }

            public delegate void InitEvent(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe);

            public delegate Int32 ExecEvent(btlseq.btlseqinstance btlSeq, SEQ_WORK pSeqWork, StringBuilder pMe);
        }
    }
}

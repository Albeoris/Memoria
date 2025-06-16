using Memoria.Prime;
using NCalc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Global.Sound.SaXAudio
{
    public class AudioEffectManager
    {
        private const String FILENAME = "AudioEffects.txt";

        private static Dictionary<Int32, EffectPreset> fieldIDPresets = new Dictionary<Int32, EffectPreset>();
        private static Dictionary<Int32, EffectPreset> battleIDPresets = new Dictionary<Int32, EffectPreset>();
        private static Dictionary<Int32, EffectPreset> battleBgIDPresets = new Dictionary<Int32, EffectPreset>();
        private static List<EffectPreset> conditionalPresets = new List<EffectPreset>();

        public static Boolean Initialized = false;

        public static Boolean IsSaXAudio { get; private set; } = ISdLibAPIProxy.Instance is SdLibAPIWithSaXAudio;

        public static void ApplyFieldEffects(Int32 fieldID)
        {
            if (!IsSaXAudio) return;
            if (!Initialized) Init();

            Log.Message($"[ApplyFieldEffects] {fieldID}");
            ResetEffects();
            if (ApplyConditionalEffects()) return;

            if (fieldIDPresets.TryGetValue(fieldID, out EffectPreset preset))
            {
                ApplyPreset(ref preset);
            }
        }

        public static void ApplyBattleEffects(Int32 battleID)
        {
            if (!IsSaXAudio) return;
            if (!Initialized) Init();

            Log.Message($"[ApplyBattleEffects] {battleID}");
            ResetEffects();
            if (ApplyConditionalEffects()) return;

            if (battleIDPresets.TryGetValue(battleID, out EffectPreset preset))
            {
                ApplyPreset(ref preset);
            }
        }

        public static void ApplyBattleBgEffects(Int32 battleBgID)
        {
            if (!IsSaXAudio) return;
            if (!Initialized) Init();

            Log.Message($"[ApplyBattleBgEffects] {battleBgID}");

            if (ApplyConditionalEffects()) return;

            if (battleBgIDPresets.TryGetValue(battleBgID, out EffectPreset preset))
            {
                ApplyPreset(ref preset);
            }
        }

        private static Boolean ApplyConditionalEffects()
        {
            for (Int32 i = 0; i < conditionalPresets.Count; i++)
            {
                EffectPreset preset = conditionalPresets[i];
                Expression c = new Expression(preset.Condition);
                c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                c.EvaluateParameter += NCalcUtility.worldNCalcParameters;

                try
                {
                    if (NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                    {
                        ApplyPreset(ref preset);
                        return true;
                    }
                }
                catch (Exception err)
                {
                    Log.Error($"[AudioEffectManager] Couldn't evaluate condition: '{preset.Condition.Trim()}' in preset '{preset.Name}'");
                    Log.Error(err);
                    continue;
                }
            }
            return false;
        }

        public static void ResetEffects()
        {
            SdLibAPIWithSaXAudio saXAudio = ISdLibAPIProxy.Instance as SdLibAPIWithSaXAudio;
            SaXAudio.RemoveReverb(saXAudio.BusMusic, 0, true);
            SaXAudio.RemoveEq(saXAudio.BusMusic, 0, true);
            SaXAudio.RemoveEcho(saXAudio.BusMusic, 0, true);
            SaXAudio.SetVolume(saXAudio.BusMusic, 1f, 0, true);

            SaXAudio.RemoveReverb(saXAudio.BusAmbient, 0, true);
            SaXAudio.RemoveEq(saXAudio.BusAmbient, 0, true);
            SaXAudio.RemoveEcho(saXAudio.BusAmbient, 0, true);
            SaXAudio.SetVolume(saXAudio.BusAmbient, 1f, 0, true);

            SaXAudio.RemoveReverb(saXAudio.BusSoundEffect, 0, true);
            SaXAudio.RemoveEq(saXAudio.BusSoundEffect, 0, true);
            SaXAudio.RemoveEcho(saXAudio.BusSoundEffect, 0, true);
            SaXAudio.SetVolume(saXAudio.BusSoundEffect, 1f, 0, true);

            SaXAudio.RemoveReverb(saXAudio.BusVoice, 0, true);
            SaXAudio.RemoveEq(saXAudio.BusVoice, 0, true);
            SaXAudio.RemoveEcho(saXAudio.BusVoice, 0, true);
            SaXAudio.SetVolume(saXAudio.BusVoice, 1f, 0, true);
        }

        private static void Init()
        {
            Initialized = true;

            fieldIDPresets.Clear();
            battleIDPresets.Clear();
            battleBgIDPresets.Clear();
            conditionalPresets.Clear();

            foreach (AssetManager.AssetFolder folder in AssetManager.FolderLowToHigh)
            {
                if (folder.TryFindAssetInModOnDisc(FILENAME, out String fullPath))
                {
                    var presets = LoadPresets($"{Path.GetDirectoryName(fullPath)}\\");
                    foreach (var preset in presets.Values)
                    {
                        Boolean listed = false;
                        foreach (Int32 fieldID in preset.FieldIDs)
                        {
                            fieldIDPresets[fieldID] = preset;
                            fieldIDPresets[fieldID].RemoveIDs();
                            listed = true;
                        }
                        foreach (Int32 battleID in preset.BattleIDs)
                        {
                            battleIDPresets[battleID] = preset;
                            battleIDPresets[battleID].RemoveIDs();
                            listed = true;
                        }
                        foreach (Int32 battleBgID in preset.BattleBgIDs)
                        {
                            battleBgIDPresets[battleBgID] = preset;
                            battleBgIDPresets[battleBgID].RemoveIDs();
                            listed = true;
                        }
                        if (!listed && !String.IsNullOrEmpty(preset.Condition))
                        {
                            conditionalPresets.Add(preset);
                            conditionalPresets.Last().RemoveIDs();
                        }
                    }
                }
            }

            FileSystemWatcher watcher = new FileSystemWatcher("./", $"*{FILENAME}");
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += (sender, e) =>
            {
                if (e.ChangeType != WatcherChangeTypes.Changed) return;
                Log.Message($"[AudioEffectManager] File changed {e.FullPath}");
                Init();
            };
            watcher.EnableRaisingEvents = true;
        }

        private static void ApplyPreset(ref EffectPreset preset)
        {
            if (!String.IsNullOrEmpty(preset.Condition))
            {
                Expression c = new Expression(preset.Condition);
                c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
                c.EvaluateParameter += NCalcUtility.worldNCalcParameters;

                try
                {
                    if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                        return;
                }
                catch (Exception e)
                {
                    Log.Error($"[AudioEffectManager] Couldn't evaluate condition: '{preset.Condition.Trim()}' in preset '{preset.Name}'");
                    Log.Error(e);
                    return;
                }
            }
            SdLibAPIWithSaXAudio saXAudio = ISdLibAPIProxy.Instance as SdLibAPIWithSaXAudio;

            Boolean reverb = (preset.Effects & EffectPreset.Effect.Reverb) != 0;
            Boolean eq = (preset.Effects & EffectPreset.Effect.Eq) != 0;
            Boolean echo = (preset.Effects & EffectPreset.Effect.Echo) != 0;
            Boolean volume = (preset.Effects & EffectPreset.Effect.Volume) != 0;

            if ((preset.Layers & EffectPreset.Layer.Music) != 0)
            {
                if (reverb) SaXAudio.SetReverb(saXAudio.BusMusic, preset.Reverb, 0, true);
                if (eq) SaXAudio.SetEq(saXAudio.BusMusic, preset.Eq, 0, true);
                if (echo) SaXAudio.SetEcho(saXAudio.BusMusic, preset.Echo, 0, true);
                if (volume) SaXAudio.SetVolume(saXAudio.BusMusic, preset.Volume, 0, true);
            }
            if ((preset.Layers & EffectPreset.Layer.Ambient) != 0)
            {
                if (reverb) SaXAudio.SetReverb(saXAudio.BusAmbient, preset.Reverb, 0, true);
                if (eq) SaXAudio.SetEq(saXAudio.BusAmbient, preset.Eq, 0, true);
                if (echo) SaXAudio.SetEcho(saXAudio.BusAmbient, preset.Echo, 0, true);
                if (volume) SaXAudio.SetVolume(saXAudio.BusAmbient, preset.Volume, 0, true);
            }
            if ((preset.Layers & EffectPreset.Layer.SoundEffect) != 0)
            {
                if (reverb) SaXAudio.SetReverb(saXAudio.BusSoundEffect, preset.Reverb, 0, true);
                if (eq) SaXAudio.SetEq(saXAudio.BusSoundEffect, preset.Eq, 0, true);
                if (echo) SaXAudio.SetEcho(saXAudio.BusSoundEffect, preset.Echo, 0, true);
                if (volume) SaXAudio.SetVolume(saXAudio.BusSoundEffect, preset.Volume, 0, true);
            }
            if ((preset.Layers & EffectPreset.Layer.Voice) != 0)
            {
                if (reverb) SaXAudio.SetReverb(saXAudio.BusVoice, preset.Reverb, 0, true);
                if (eq) SaXAudio.SetEq(saXAudio.BusVoice, preset.Eq, 0, true);
                if (echo) SaXAudio.SetEcho(saXAudio.BusVoice, preset.Echo, 0, true);
                if (volume) SaXAudio.SetVolume(saXAudio.BusVoice, preset.Volume, 0, true);
            }

            Log.Message($"[AudioEffectManager] Applied '{preset.Name}' effect preset");
        }

        public struct EffectPreset
        {
            public enum Effect : Byte
            {
                None = 0,
                Reverb = 1,
                Eq = 1 << 1,
                Echo = 1 << 2,
                Volume = 1 << 3,
                All = Reverb | Eq | Echo | Volume
            }

            public enum Layer : Byte
            {
                None = 0,
                Music = 1,
                Ambient = 1 << 1,
                SoundEffect = 1 << 2,
                Voice = 1 << 3,
                All = Music | Ambient | SoundEffect | Voice
            }

            public String Name = "";
            public Effect Effects = Effect.None;
            public SaXAudio.ReverbParameters Reverb = new SaXAudio.ReverbParameters();
            public SaXAudio.EqParameters Eq = new SaXAudio.EqParameters();
            public SaXAudio.EchoParameters Echo = new SaXAudio.EchoParameters();
            public Single Volume = 1f;

            public Layer Layers = Layer.None;
            public HashSet<Int32> FieldIDs = new HashSet<Int32>();
            public HashSet<Int32> BattleIDs = new HashSet<Int32>();
            public HashSet<Int32> BattleBgIDs = new HashSet<Int32>();

            public String Condition = "";

            public EffectPreset() { }
            public EffectPreset(String str)
            {
                String[] tokens = str.Split(';');
                Int32 i = 0;
                Name = tokens[i++];
                Effects = (Effect)Byte.Parse(tokens[i++]);

                FieldInfo[] fields = typeof(SaXAudio.ReverbParameters).GetFields();
                object reverb = Reverb;
                foreach (FieldInfo field in fields)
                {
                    field.SetValue(reverb, Convert.ChangeType(tokens[i++], field.FieldType, CultureInfo.InvariantCulture));
                }
                Reverb = (SaXAudio.ReverbParameters)reverb;

                fields = typeof(SaXAudio.EqParameters).GetFields();
                object eq = Eq;
                foreach (FieldInfo field in fields)
                {
                    field.SetValue(eq, Convert.ChangeType(tokens[i++], field.FieldType, CultureInfo.InvariantCulture));
                }
                Eq = (SaXAudio.EqParameters)eq;

                fields = typeof(SaXAudio.EchoParameters).GetFields();
                object echo = Echo;
                foreach (FieldInfo field in fields)
                {
                    field.SetValue(echo, Convert.ChangeType(tokens[i++], field.FieldType, CultureInfo.InvariantCulture));
                }
                Echo = (SaXAudio.EchoParameters)echo;

                Volume = Single.Parse(tokens[i++]);

                if (i == tokens.Length) return;

                Layers = (Layer)Byte.Parse(tokens[i++]);

                String[] ids = tokens[i++].Split('|');
                if (ids.Length > 0)
                {
                    FieldIDs.Clear();
                    foreach (String id in ids)
                    {
                        if (id.Length > 0)
                            FieldIDs.Add(Int32.Parse(id));
                    }
                }

                ids = tokens[i++].Split('|');
                if (ids.Length > 0)
                {
                    BattleIDs.Clear();
                    foreach (String id in ids)
                    {
                        if (id.Length > 0)
                            BattleIDs.Add(Int32.Parse(id));
                    }
                }

                ids = tokens[i++].Split('|');
                if (ids.Length > 0)
                {
                    BattleBgIDs.Clear();
                    foreach (String id in ids)
                    {
                        if (id.Length > 0)
                            BattleBgIDs.Add(Int32.Parse(id));
                    }
                }

                Condition = tokens[i++].Replace('\x0A', ';');
            }

            public override readonly String ToString()
            {
                StringBuilder builder = new StringBuilder();
                const Char separator = ';';
                builder.Append(Name);
                builder.Append(separator);
                builder.Append((Byte)Effects);

                FieldInfo[] fields = typeof(SaXAudio.ReverbParameters).GetFields();
                foreach (FieldInfo field in fields)
                {
                    builder.Append(separator);
                    builder.Append((String)Convert.ChangeType(field.GetValue(Reverb), typeof(String), CultureInfo.InvariantCulture));
                }

                fields = typeof(SaXAudio.EqParameters).GetFields();
                foreach (FieldInfo field in fields)
                {
                    builder.Append(separator);
                    builder.Append((String)Convert.ChangeType(field.GetValue(Eq), typeof(String), CultureInfo.InvariantCulture));
                }

                fields = typeof(SaXAudio.EchoParameters).GetFields();
                foreach (FieldInfo field in fields)
                {
                    builder.Append(separator);
                    builder.Append((String)Convert.ChangeType(field.GetValue(Echo), typeof(String), CultureInfo.InvariantCulture));
                }

                builder.Append(separator);
                builder.Append(Volume);

                builder.Append(separator);
                builder.Append((Byte)Layers);

                builder.Append(separator);
                builder.Append(String.Join("|", FieldIDs.Select(x => x.ToString()).ToArray()));

                builder.Append(separator);
                builder.Append(String.Join("|", BattleIDs.Select(x => x.ToString()).ToArray()));

                builder.Append(separator);
                builder.Append(String.Join("|", BattleBgIDs.Select(x => x.ToString()).ToArray()));

                builder.Append(separator);
                builder.Append(Condition.Trim().Replace(';', '\x0A'));

                return builder.ToString();
            }

            public void RemoveIDs()
            {
                FieldIDs = null;
                BattleIDs = null;
                BattleBgIDs = null;
            }
        }

        public static SortedDictionary<String, EffectPreset> LoadPresets(String modLocation, Boolean backup = false)
        {
            SortedDictionary<String, EffectPreset> presets = new SortedDictionary<String, EffectPreset>();
            String path = Path.Combine(modLocation, $"{FILENAME}{(backup ? ".bak" : "")}");
            if (File.Exists(path))
            {
                try
                {
                    String mod = Path.GetDirectoryName(modLocation);
                    Log.Message($"[AudioEffectManager] Load {mod} presets");
                    String[] lines = File.ReadAllLines(path);
                    foreach (String line in lines)
                    {
                        String decryptedLine = Decrypt(line, mod);
                        if (decryptedLine.Length == 0 || decryptedLine.StartsWith("#"))
                            continue;
                        EffectPreset preset = new EffectPreset(decryptedLine);
                        presets.Add(preset.Name, preset);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            return presets;
        }

        public static void SavePresets(SortedDictionary<String, EffectPreset> presets, String modLocation, Boolean backup = false)
        {
            try
            {
                String mod = Path.GetDirectoryName(modLocation);
                Log.Message($"[AudioEffectManager] Save {mod} presets");
                List<String> lines = new List<String>();
                foreach (EffectPreset effectPreset in presets.Values)
                {
                    lines.Add(Encrypt(effectPreset.ToString(), mod));
                }
                String path = Path.Combine(modLocation, $"{FILENAME}{(backup ? ".bak" : "")}");
                File.WriteAllLines(path, lines.ToArray());
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static string Encrypt(string plainText, string password)
        {
            using Aes aes = Aes.Create();
            var key = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("AlexandriaCastle"), 10000);
            aes.Key = key.GetBytes(32);
            aes.IV = key.GetBytes(16);
            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                cs.Write(plainBytes, 0, plainBytes.Length);
                cs.FlushFinalBlock();
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string cipherText, string password)
        {
            using Aes aes = Aes.Create();
            var key = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes("AlexandriaCastle"), 10000);
            aes.Key = key.GetBytes(32);
            aes.IV = key.GetBytes(16);

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cs);

            return reader.ReadToEnd();
        }
    }

}

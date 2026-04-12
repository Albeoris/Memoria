using Memoria.Prime;
using NCalc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Global.Sound.SaXAudio
{
    public class AudioEffectManager
    {
        private const String FILENAME = "StreamingAssets/Data/Voices/AudioEffects.csv";

        private static Dictionary<Int32, EffectPreset> fieldIDPresets = new Dictionary<Int32, EffectPreset>();
        private static Dictionary<Int32, EffectPreset> battleIDPresets = new Dictionary<Int32, EffectPreset>();
        private static Dictionary<Int32, EffectPreset> battleBgIDPresets = new Dictionary<Int32, EffectPreset>();
        private static Dictionary<String, EffectPreset> resourceIDPresets = new Dictionary<String, EffectPreset>();
        private static List<EffectPreset> conditionalPresets = new List<EffectPreset>();
        private static Dictionary<String, EffectPreset> unlistedPresets = new Dictionary<String, EffectPreset>();
        private static EffectPreset currentPreset = null;

        private static Boolean initialized = false;
        private static FileSystemWatcher watcher = null;

        public static Boolean IsSaXAudio { get; private set; } = ISdLibAPIProxy.Instance is SdLibAPIWithSaXAudio;

        public static void Initialize()
        {
            new Thread(() =>
            {
                lock (battleBgIDPresets)
                {
                    if (!initialized) Init();
                }
            }).Start();
        }

        public static void ApplyFieldEffects(Int32 fieldID)
        {
            if (!IsSaXAudio) return;

            lock (battleBgIDPresets)
            {
                if (!initialized) Init();
            }

            if (fieldIDPresets.TryGetValue(fieldID, out EffectPreset preset) && ApplyPreset(preset))
            {
                currentPreset = preset;
                Log.Message($"[AudioEffectManager] Applied preset '{preset.Name}' to field {fieldID}");
                return;
            }
            ResetEffects();
        }

        public static void ApplyBattleEffects(Int32 battleID, Int32 battleBgID)
        {
            if (!IsSaXAudio) return;

            lock (battleBgIDPresets)
            {
                if (!initialized) Init();
            }

            if (battleIDPresets.TryGetValue(battleID, out EffectPreset preset) && ApplyPreset(preset))
            {
                currentPreset = preset;
                Log.Message($"[AudioEffectManager] Applied preset '{preset.Name}' to battle {battleID}");
                return;
            }

            if (battleBgIDPresets.TryGetValue(battleBgID, out preset) && ApplyPreset(preset))
            {
                currentPreset = preset;
                Log.Message($"[AudioEffectManager] Applied preset '{preset.Name}' to battle background {battleBgID}");
                return;
            }
            ResetEffects();
        }

        public static EffectPreset GetPreset(SoundProfile profile, Int32 bus)
        {
            if (!IsSaXAudio || !initialized)
                return null;

            EffectPreset preset = null;

            // 1. resourceIDs gets priority
            if (resourceIDPresets.TryGetValue(profile.ResourceID, out preset))
                if (String.IsNullOrEmpty(preset.Condition) || EvaluatePresetCondition(profile, preset.CompiledCondition, preset.Name))
                    return preset;

            // 2. then conditional
            for (Int32 i = 0; i < conditionalPresets.Count; i++)
            {
                preset = conditionalPresets[i];

                Boolean isBusValid = preset.Layers == EffectPreset.Layer.None || preset.IsBusInLayers(bus);
                if (isBusValid && EvaluatePresetCondition(profile, preset.CompiledCondition, preset.Name))
                    return preset;
            }

            // 3. then filter current preset
            preset = currentPreset;
            if (preset != null && preset.IsBusInLayers(bus))
            {
                if (!String.IsNullOrEmpty(preset.Condition) && !EvaluatePresetCondition(profile, preset.CompiledCondition, preset.Name))
                    return new EffectPreset(); // empty preset because we want to exclude that particular sound
            }

            return null;
        }

        public static EffectPreset GetUnlistedPreset(String presetName)
        {
            if (!IsSaXAudio || !initialized || !unlistedPresets.ContainsKey(presetName)) return null;
            return unlistedPresets[presetName];
        }

        public static EffectPreset FindPreset(String presetName)
        {
            if (!IsSaXAudio || !initialized) return null;

            if (unlistedPresets.ContainsKey(presetName))
                return unlistedPresets[presetName];

            foreach (Int32 key in fieldIDPresets.Keys)
            {
                if (fieldIDPresets[key].Name == presetName)
                    return fieldIDPresets[key];
            }

            foreach (Int32 key in battleBgIDPresets.Keys)
            {
                if (battleBgIDPresets[key].Name == presetName)
                    return battleBgIDPresets[key];
            }

            foreach (Int32 key in battleIDPresets.Keys)
            {
                if (battleIDPresets[key].Name == presetName)
                    return battleIDPresets[key];
            }

            foreach (String key in resourceIDPresets.Keys)
            {
                if (resourceIDPresets[key].Name == presetName)
                    return resourceIDPresets[key];
            }

            for (Int32 i = 0; i < conditionalPresets.Count; i++)
            {
                if (conditionalPresets[i].Name == presetName)
                    return conditionalPresets[i];
            }

            return null;
        }

        public static Boolean EvaluatePresetCondition(SoundProfile profile, Expression c, String presetName)
        {
            if (c == null) return false;

            c.Parameters["SoundIndex"] = profile.SoundIndex;
            c.Parameters["ResourceID"] = profile.ResourceID;
            c.Parameters["SoundProfileType"] = profile.SoundProfileType;
            c.Parameters["SoundProfileType_Default"] = SoundProfileType.Default;
            c.Parameters["SoundProfileType_Music"] = SoundProfileType.Music;
            c.Parameters["SoundProfileType_SoundEffect"] = (Byte)SoundProfileType.SoundEffect;
            c.Parameters["SoundProfileType_MovieAudio"] = SoundProfileType.MovieAudio;
            c.Parameters["SoundProfileType_Song"] = SoundProfileType.Song;
            c.Parameters["SoundProfileType_Sfx"] = SoundProfileType.Sfx;
            c.Parameters["SoundProfileType_Voice"] = SoundProfileType.Voice;

            try
            {
                if (NCalcUtility.EvaluateNCalcCondition(c.Evaluate()))
                    return true;
            }
            catch (Exception e)
            {
                Log.Error($"[AudioEffectManager] Couldn't evaluate condition in preset '{presetName}'");
                Log.Error(e);
            }

            return false;
        }

        public static void ApplyPresetOnSound(EffectPreset preset, Int32 soundID, String soundName, Single fade = 0)
        {
            if (!IsSaXAudio) return;

            lock (battleBgIDPresets)
            {
                if (!initialized) Init();
            }

            if (preset.Effects == EffectPreset.Effect.None) return;

            Boolean reverb = (preset.Effects & EffectPreset.Effect.Reverb) != 0;
            Boolean eq = (preset.Effects & EffectPreset.Effect.Eq) != 0;
            Boolean echo = (preset.Effects & EffectPreset.Effect.Echo) != 0;
            Boolean volume = (preset.Effects & EffectPreset.Effect.Volume) != 0;

            if (reverb) SaXAudio.SetReverb(soundID, preset.Reverb, fade);
            else SaXAudio.RemoveReverb(soundID, fade);
            if (eq) SaXAudio.SetEq(soundID, preset.Eq, fade);
            else SaXAudio.RemoveEq(soundID, fade);
            if (echo) SaXAudio.SetEcho(soundID, preset.Echo, fade);
            else SaXAudio.RemoveEcho(soundID, fade);
            // Problem of applying volume directly onto the sound, it's multiplicative and there's no way to restore the original value
            // This shouldn't be an issue unless ApplyPresetOnSound is called multiple times on the same sound
            if (volume) SaXAudio.SetVolume(soundID, SaXAudio.GetVolume(soundID) * preset.Volume, fade);

            // Can cause a bit too much spam
            //Log.Message($"[AudioEffectManager] Applied preset '{preset.Name}' on sound '{soundName}'");
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
            fieldIDPresets.Clear();
            battleIDPresets.Clear();
            battleBgIDPresets.Clear();
            conditionalPresets.Clear();
            unlistedPresets.Clear();

            foreach (AssetManager.AssetFolder folder in AssetManager.FolderLowToHigh)
            {
                if (folder.TryFindAssetInModOnDisc(FILENAME, out String fullPath))
                {
                    var presets = LoadPresets(folder.FolderPath);
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
                        foreach (String resourceID in preset.ResourceIDs)
                        {
                            resourceIDPresets[resourceID] = preset;
                            resourceIDPresets[resourceID].RemoveIDs();
                            listed = true;
                        }
                        if (!listed)
                        {
                            if (!String.IsNullOrEmpty(preset.Condition))
                                conditionalPresets.Add(preset);
                            else
                                unlistedPresets[preset.Name] = preset;
                        }
                    }
                }
            }

            if (watcher == null)
            {
                watcher = new FileSystemWatcher("./", Path.GetFileName(FILENAME));
                watcher.IncludeSubdirectories = true;
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Changed += (sender, e) =>
                {
                    if (e.ChangeType != WatcherChangeTypes.Changed) return;
                    initialized = false;
                    Log.Message($"'{e.FullPath}' changed");
                };
                watcher.EnableRaisingEvents = true;
            }
            initialized = true;
        }

        private static Boolean ApplyPreset(EffectPreset preset)
        {
            SdLibAPIWithSaXAudio saXAudio = ISdLibAPIProxy.Instance as SdLibAPIWithSaXAudio;

            if ((preset.Layers & EffectPreset.Layer.Music) != 0) ApplyPresetOnBus(preset, saXAudio.BusMusic);
            if ((preset.Layers & EffectPreset.Layer.Ambient) != 0) ApplyPresetOnBus(preset, saXAudio.BusAmbient);
            if ((preset.Layers & EffectPreset.Layer.SoundEffect) != 0) ApplyPresetOnBus(preset, saXAudio.BusSoundEffect);
            if ((preset.Layers & EffectPreset.Layer.Voice) != 0) ApplyPresetOnBus(preset, saXAudio.BusVoice);

            return true;
        }

        private static void ApplyPresetOnBus(EffectPreset preset, Int32 bus)
        {
            Boolean reverb = (preset.Effects & EffectPreset.Effect.Reverb) != 0;
            Boolean eq = (preset.Effects & EffectPreset.Effect.Eq) != 0;
            Boolean echo = (preset.Effects & EffectPreset.Effect.Echo) != 0;
            Boolean volume = (preset.Effects & EffectPreset.Effect.Volume) != 0;

            if (reverb) SaXAudio.SetReverb(bus, preset.Reverb, 0, true);
            else SaXAudio.RemoveReverb(bus, 0, true);
            if (eq) SaXAudio.SetEq(bus, preset.Eq, 0, true);
            else SaXAudio.RemoveEq(bus, 0, true);
            if (echo) SaXAudio.SetEcho(bus, preset.Echo, 0, true);
            else SaXAudio.RemoveEcho(bus, 0, true);
            if (volume) SaXAudio.SetVolume(bus, preset.Volume, 0, true);
            else SaXAudio.SetVolume(bus, 1f, 0, true);
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
                    String[] lines = File.ReadAllLines(path);
                    foreach (String line in lines)
                    {
                        if (line.Length == 0 || line.StartsWith("#"))
                            continue;

                        EffectPreset preset = new EffectPreset();
                        preset.ParseEntry(line.Split(';'), null);

                        if (!string.IsNullOrEmpty(preset.Name))
                        {
                            presets[preset.Name] = preset;
                        }
                    }
                    Log.Message($"[AudioEffectManager] Loaded {mod} presets from CSV");
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
                List<String> lines = new List<String>();

                lines.Add("# ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                lines.Add("# Name;Effects;Reverb.WetDryMix;Reverb.ReflectionsDelay;Reverb.ReverbDelay;Reverb.RearDelay;Reverb.SideDelay;Reverb.PositionLeft;Reverb.PositionRight;Reverb.PositionMatrixLeft;Reverb.PositionMatrixRight;Reverb.EarlyDiffusion;Reverb.LateDiffusion;Reverb.LowEQGain;Reverb.LowEQCutoff;Reverb.HighEQGain;Reverb.HighEQCutoff;Reverb.RoomFilterFreq;Reverb.RoomFilterMain;Reverb.RoomFilterHF;Reverb.ReflectionsGain;Reverb.ReverbGain;Reverb.DecayTime;Reverb.Density;Reverb.RoomSize;Reverb.DisableLateField;Eq.FrequencyCenter0;Eq.Gain0;Eq.Bandwidth0;Eq.FrequencyCenter1;Eq.Gain1;Eq.Bandwidth1;Eq.FrequencyCenter2;Eq.Gain2;Eq.Bandwidth2;Eq.FrequencyCenter3;Eq.Gain3;Eq.Bandwidth3;Echo.WetDryMix;Echo.Feedback;Echo.Delay;Volume;Layers;FieldIDs;BattleIDs;BattleBgIDs;ResourceIDs;NCalcCondition");
                lines.Add("# String;Byte;Single;UInt32;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Byte;Single;Single;Single;Single;Single;Single;Single;Single;Boolean;Single;Single;Single;Single;Single;Single;Single;Single;Single;Single;Single;Single;Single;Single;Single;Single;Byte;String;String;String;String;String");
                lines.Add("# ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");

                foreach (EffectPreset effectPreset in presets.Values)
                    lines.Add(effectPreset.ToString());

                String path = Path.Combine(modLocation, $"{FILENAME}{(backup ? ".bak" : "")}");
                File.WriteAllLines(path, lines.ToArray());
                Log.Message($"[AudioEffectManager] Saved {mod} presets to CSV");
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}

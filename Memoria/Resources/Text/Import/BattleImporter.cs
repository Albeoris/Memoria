using System;
using System.Collections.Generic;
using System.IO;

namespace Memoria
{
    public sealed class BattleImporter : TextImporter
    {
        private String TypeName => nameof(BattleImporter);

        private volatile Boolean _initialized;
        private readonly Dictionary<Int32, String[]> _cache = new Dictionary<int, string[]>();

        public Task InitializationTask { get; private set; }

        public void InitializeAsync()
        {
            InitializationTask = Task.Run(Initialize);
        }

        private void Initialize()
        {
            Dictionary<String, String> dic;
            if (!TryLoadReplacements(out dic))
                return;

            Log.Message($"[{TypeName}] Loading...");

            foreach (KeyValuePair<String, Int32> pair in FF9BattleDB.SceneData)
            {
                Int32 index = pair.Value;
                if (index == 220 || index == 238) // Junk?
                    continue;

                String path = EmbadedTextResources.GetCurrentPath("/Battle/" + index + ".mes");
                String[] text = EmbadedSentenseLoader.LoadSentense(path);
                if (text != null)
                {
                    for (int i = 0; i < text.Length; i++)
                    {
                        String key = BattleFormatter.GetKey(text[i]);
                        String value;
                        if (dic.TryGetValue(key, out value))
                            text[i] = value;
                    }
                }

                _cache[index] = text;
            }

            _initialized = true;
        }

        private Boolean TryLoadReplacements(out Dictionary<String, String> dic)
        {
            String importPath = ModTextResources.Import.Battle;
            if (!File.Exists(importPath))
            {
                Log.Warning($"[{TypeName}] Import was skipped bacause a file does not exist: [{importPath}].");
                dic = null;
                return false;
            }

            Log.Message($"[{TypeName}] Loading from [{importPath}]...");

            TxtEntry[] entries = TxtReader.ReadStrings(importPath);

            BattleFormatter.Parse(entries, out dic);

            Log.Message($"[{TypeName}] Loading completed successfully.");
            return true;
        }

        protected override Boolean LoadInternal()
        {
            Int32 battleZoneId = FF9TextToolAccessor.GetBattleZoneId();
            String path = EmbadedTextResources.GetCurrentPath("/Battle/" + battleZoneId + ".mes");
            String[] text = EmbadedSentenseLoader.LoadSentense(path);
            if (text != null)
                FF9TextToolAccessor.SetBattleText(text);
            return true;
        }

        protected override Boolean LoadExternal()
        {
            try
            {
                if (!_initialized)
                    return false;

                Int32 battleZoneId = FF9TextToolAccessor.GetBattleZoneId();

                String[] result;
                if (!_cache.TryGetValue(battleZoneId, out result))
                    return false;

                if (result != null)
                    FF9TextToolAccessor.SetBattleText(result);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"[{TypeName}] Failed to import resource.");
                return false;
            }
        }
    }
}
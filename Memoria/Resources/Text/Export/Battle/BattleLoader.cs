using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Memoria
{
    public sealed class BattleLoader : SingleFileExporter
    {
        protected override string TypeName => nameof(BattleLoader);
        protected override string ExportPath => ModTextResources.Export.Battle;

        protected override TxtEntry[] PrepareEntries()
        {
            Dictionary<String, String> dic = new Dictionary<String, String>(1024);

            foreach (KeyValuePair<String, Int32> pair in FF9BattleDB.SceneData)
            {
                Int32 index = pair.Value;
                if (index == 220 || index == 238) // Junk?
                    continue;

                String path = EmbadedTextResources.GetCurrentPath("/Battle/" + index + ".mes");
                String[] text = EmbadedSentenseLoader.LoadSentense(path);
                if (text == null)
                    continue;

                foreach (String line in text)
                {
                    if (String.IsNullOrEmpty(line))
                        continue;

                    String key = BattleFormatter.GetKey(line);
                    if (!dic.ContainsKey(key))
                        dic.Add(key, BattleFormatter.GetValue(line));
                }
            }

            return dic.Select(p => new TxtEntry {Prefix = p.Key, Value = p.Value}).ToArray();
        }
    }
}
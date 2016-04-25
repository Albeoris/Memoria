using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Sources.Scripts.UI.Common;

namespace Memoria
{
    public sealed class LocationNameLoader : IExporter
    {
        public void Export()
        {
            try
            {
                Log.Message($"[{nameof(LocationNameLoader)}] Exporting...");
                Int32 skipped = 0;

                String text = EmbadedSentenseLoader.LoadText(EmbadedTextResources.LocationNames);

                Dictionary<Int32, String> locationNames = new Dictionary<Int32, String>();
                String[] array = text.Split('\r');
                for (int i = 0; i < array.Length; i++)
                {
                    String str = array[i];
                    str = str.Replace("\n", String.Empty);
                    if (!String.IsNullOrEmpty(str))
                    {
                        String key = str.Split(':')[0];
                        String value = str.Split(':')[1];
                        locationNames[int.Parse(key)] = FF9TextTool.RemoveOpCode(value);
                    }
                }

                Dictionary<String, LinkedList<TxtEntry>> entries = LocationNameFormatter.Build(locationNames);

                String directory = ModTextResources.Export.LocationNamesDirectory;
                Directory.CreateDirectory(directory);

                foreach (KeyValuePair<String, LinkedList<TxtEntry>> pair in entries)
                {
                    String outputPath = Path.Combine(directory, "Names of " + pair.Key + ".strings");
                    if (File.Exists(outputPath))
                    {
                        skipped++;
                        continue;
                    }

                    TxtWriter.WriteStrings(outputPath, pair.Value.ToArray());
                }

                if (skipped > 0)
                    Log.Warning($"[{nameof(LocationNameLoader)}] Exporting completed but [{skipped}/{entries.Count}] files has been skipped because already exists.");
                else
                    Log.Message($"[{nameof(LocationNameLoader)}] Exporting completed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"[{nameof(LocationNameLoader)}] Failed to export resource.");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Sources.Scripts.UI.Common;

namespace Memoria
{
    public sealed class LocationNameImporter : TextImporter
    {
        public String TypeName => nameof(LocationNameImporter);
        public String ImportDirectory => ModTextResources.Import.LocationNamesDirectory;

        public void Export()
        {
            try
            {
                Log.Message($"[{nameof(LocationNameExporter)}] Exporting...");
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

                Char[] invalidChars = Path.GetInvalidFileNameChars();
                foreach (KeyValuePair<String, LinkedList<TxtEntry>> pair in entries)
                {
                    String outputPath = Path.Combine(directory, "Names of " + pair.Key.ReplaceChars("_", invalidChars) + ".strings");
                    if (File.Exists(outputPath))
                    {
                        skipped++;
                        continue;
                    }

                    TxtWriter.WriteStrings(outputPath, pair.Value.ToArray());
                }

                if (skipped > 0)
                    Log.Warning($"[{nameof(LocationNameExporter)}] Exporting completed but [{skipped}/{entries.Count}] files has been skipped because already exists.");
                else
                    Log.Message($"[{nameof(LocationNameExporter)}] Exporting completed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"[{nameof(LocationNameExporter)}] Failed to export resource.");
            }
        }

        protected override bool LoadExternal()
        {
            try
            {
                String importDirectory = ImportDirectory;
                if (!Directory.Exists(importDirectory))
                {
                    Log.Warning($"[{TypeName}] Import was skipped bacause a directory does not exist: [{importDirectory}].");
                    return false;
                }

                Log.Message($"[{TypeName}] Importing from [{importDirectory}]...");

                Dictionary<Int32, String> locationNames = FF9TextToolAccessor.GetLocationName();
                foreach (String filePath in Directory.GetFiles(importDirectory, "Names of *", SearchOption.TopDirectoryOnly))
                {
                    TxtEntry[] entries = TxtReader.ReadStrings(filePath);
                    LocationNameFormatter.Fill(filePath, entries, locationNames);
                }

                Log.Message($"[{TypeName}] Importing completed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"[{TypeName}] Failed to import resource from.");
                return false;
            }
        }

        protected override bool LoadInternal()
        {
            String text = EmbadedSentenseLoader.LoadText(EmbadedTextResources.LocationNames);

            Dictionary<Int32, String> locationNames = FF9TextToolAccessor.GetLocationName();
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
            return true;
        }
    }
}
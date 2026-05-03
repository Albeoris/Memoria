using Assets.Sources.Scripts.UI.Common;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.IO;

namespace Memoria.Assets
{
    public sealed class LocationNameImporter : TextImporter
    {
        public String TypeName => nameof(LocationNameImporter);
        public String ImportDirectory => ModTextResources.Import.LocationNamesDirectory;

        protected override Boolean LoadExternal()
        {
            try
            {
                String importDirectory = ImportDirectory;
                if (!Directory.Exists(importDirectory))
                {
                    Log.Warning($"[{TypeName}] Import was skipped because a directory does not exist: [{importDirectory}].");
                    return false;
                }

                Log.Message($"[{TypeName}] Importing from [{importDirectory}]...");

                Dictionary<Int32, String> locationNames = FF9TextTool.DisplayBatch.locationName;
                locationNames.Clear();
                foreach (String filePath in Directory.GetFiles(importDirectory, "Names of *", SearchOption.TopDirectoryOnly))
                {
                    TextResourcePath existingFile = TextResourcePath.ForImportExistingFile(filePath);
                    TxtEntry[] entries = existingFile.ReadAll();
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

        protected override Boolean LoadInternal()
        {
            String text = EmbadedSentenseLoader.LoadText(EmbadedTextResources.LocationNames);

            Dictionary<Int32, String> locationNames = FF9TextTool.DisplayBatch.locationName;
            locationNames.Clear();
            String[] entries = text.Split('\n');
            for (Int32 i = 0; i < entries.Length; i++)
            {
                String line = entries[i].Replace("\r", String.Empty);
                if (!String.IsNullOrEmpty(line))
                {
                    String[] keyValue = line.Split(':');
                    if (keyValue.Length >= 2)
                        locationNames[Int32.Parse(keyValue[0])] = FF9TextTool.RemoveOpCode(keyValue[1]);
                }
            }
            return true;
        }
    }
}

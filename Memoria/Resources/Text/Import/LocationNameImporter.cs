using System;
using System.Collections.Generic;
using System.IO;
using Assets.Sources.Scripts.UI.Common;

namespace Memoria
{
    public sealed class LocationNameImporter : TextImporter
    {
        public String TypeName => nameof(LocationNameImporter);
        public String ImportDirectory => ModTextResources.Import.LocationNamesDirectory;

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
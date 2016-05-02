using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Memoria
{
    public sealed class FieldExporter : IExporter
    {
        public void Export()
        {
            try
            {
                String directory = ModTextResources.Export.FieldsDirectory;
                if (Directory.Exists(directory))
                {
                    Log.Warning($"[{nameof(FieldExporter)}] Some fields refer to each other. They should be exported together.");
                    Log.Warning($"[{nameof(FieldExporter)}] Export was skipped bacause the directory already exists: [{directory}].");
                    return;
                }

                Log.Message($"[{nameof(FieldExporter)}] Exporting...");
                FieldFormatter formatter = new FieldFormatter();

                foreach (KeyValuePair<Int32, String> pair in FF9DBAll.EventDB)
                {
                    Int32 fieldZoneId = pair.Key;

                    String path = EmbadedTextResources.GetCurrentPath("/Field/" + GetFieldTextFileName(fieldZoneId) + ".mes");
                    String text = EmbadedSentenseLoader.LoadText(path);
                    if (text == null)
                        continue;

                    String name = fieldZoneId.ToString("D4", CultureInfo.InvariantCulture) + '_' + pair.Value;
                    text = TextOpCodeModifier.Modify(text);
                    String[] lines = FF9TextToolInterceptor.ExtractSentense(text);

                    TxtEntry[] commands = formatter.Build(name, lines);

                    Directory.CreateDirectory(directory);
                    String outputPath = Path.Combine(directory, name + ".strings");
                    TxtWriter.WriteStrings(outputPath, commands);
                }

                ExportTags(directory, formatter);

                Log.Message($"[{nameof(FieldExporter)}] Exporting completed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"[{nameof(FieldExporter)}] Failed to export resource.");
            }
        }

        private static void ExportTags(string directory, FieldFormatter formatter)
        {
            if (directory == null)
                return;

            ExportCustomTags(formatter);
            ExportFieldTags(directory, formatter);
        }

        private static void ExportCustomTags(FieldFormatter formatter)
        {
            TxtEntry[] generalNames = formatter.Names.General;
            if (generalNames != null)
                TxtWriter.WriteStrings(ModTextResources.Export.FieldTags, generalNames);
        }

        private static void ExportFieldTags(string directory, FieldFormatter formatter)
        {
            Dictionary<String, TxtEntry[]> fieldNames = formatter.Names.Fields;
            if (fieldNames == null)
                return;

            foreach (KeyValuePair<String, TxtEntry[]> pair in fieldNames)
            {
                String fieldPath = Path.Combine(directory, pair.Key + "_Tags.strings");
                TxtWriter.WriteStrings(fieldPath, pair.Value);
            }
        }

        private static string GetFieldTextFileName(int fieldZoneId)
        {
            string str = fieldZoneId.ToString();
            if (FF9StateSystem.MobilePlatform && fieldZoneId == 71)
                str += "m";
            return str;
        }
    }
}
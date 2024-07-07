using Assets.Sources.Scripts.UI.Common;
using Memoria.Prime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ExtensionMethodsIEnumerable = Memoria.Scenes.ExtensionMethodsIEnumerable;

namespace Memoria.Assets
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

                KeyValuePair<Int32, String> chocoboForest = new KeyValuePair<Int32, String>(945, "MES_CHOCO");
                foreach (KeyValuePair<Int32, String> pair in ExtensionMethodsIEnumerable.Append(FF9DBAll.EventDB, chocoboForest))
                {
                    Int32 fieldZoneId = pair.Key;

                    String path = EmbadedTextResources.GetCurrentPath("/Field/" + GetFieldTextFileName(fieldZoneId) + ".mes");
                    String text = EmbadedSentenseLoader.LoadText(path);
                    if (text == null)
                        continue;

                    String name = fieldZoneId.ToString("D4", CultureInfo.InvariantCulture) + '_' + pair.Value;
                    text = TextOpCodeModifier.Modify(text);
                    String[] lines = FF9TextTool.ExtractSentense(text);

                    TxtEntry[] commands = formatter.Build(name, lines);

                    Directory.CreateDirectory(directory);
                    TextResourcePath outputPath = TextResourcePath.ForExport(Path.Combine(directory, name));
                    outputPath.WriteAll(commands);
                }

                ExportTags(directory, formatter);

                Log.Message($"[{nameof(FieldExporter)}] Exporting completed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"[{nameof(FieldExporter)}] Failed to export resource.");
            }
        }

        private static void ExportTags(String directory, FieldFormatter formatter)
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
                ModTextResources.Export.FieldTags.WriteAll(generalNames);
        }

        private static void ExportFieldTags(String directory, FieldFormatter formatter)
        {
            Dictionary<String, TxtEntry[]> fieldNames = formatter.Names.Fields;
            if (fieldNames == null)
                return;

            foreach (KeyValuePair<String, TxtEntry[]> pair in fieldNames)
            {
                TextResourcePath fieldPath = TextResourcePath.ForExport(Path.Combine(directory, pair.Key + "_Tags"));
                fieldPath.WriteAll(pair.Value);
            }
        }

        private static String GetFieldTextFileName(Int32 fieldZoneId)
        {
            String str = fieldZoneId.ToString();
            if (FF9StateSystem.MobilePlatform && fieldZoneId == 71)
                str += "m";
            return str;
        }
    }
}

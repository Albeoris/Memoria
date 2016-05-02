using System;
using System.Collections.Generic;
using System.IO;

namespace Memoria
{
    public static class TextResourceExporter
    {
        public static void ExportSafe()
        {
            try
            {
                if (!Configuration.Export.Text)
                {
                    Log.Message("[TextResourceExporter] Pass through {Configuration.Export.Text = 0}.");
                    return;
                }

                foreach (String symbol in Configuration.Export.Languages)
                {
                    EmbadedTextResources.CurrentSymbol = symbol;
                    ModTextResources.Export.CurrentSymbol = symbol;
                    foreach (IExporter exporter in EnumerateExporters())
                        exporter.Export();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to export text resources.");
            }
            finally
            {
                EmbadedTextResources.CurrentSymbol = null;
                ModTextResources.Export.CurrentSymbol = null;
            }
        }

        private static IEnumerable<IExporter> EnumerateExporters()
        {
            foreach (EtcTextResource value in Enum.GetValues(typeof(EtcTextResource)))
                yield return new EtcExporter(value);

            yield return new CommandExporter();
            yield return new AbilityExporter();
            yield return new SkillExporter();
            yield return new ItemExporter();
            yield return new KeyItemExporter();
            yield return new BattleExporter();
            yield return new LocationNameExporter();
            yield return new FieldExporter();
        }
    }

    public interface IExporter
    {
        void Export();
    }

    public abstract class SingleFileExporter : IExporter
    {
        protected abstract String TypeName { get; }
        protected abstract String ExportPath { get; }

        protected abstract TxtEntry[] PrepareEntries();

        public void Export()
        {
            try
            {
                String exportPath = ExportPath;
                if (File.Exists(exportPath))
                {
                    Log.Warning($"[{TypeName}] Export was skipped bacause a file already exists: [{exportPath}].");
                    return;
                }

                Log.Message($"[{TypeName}] Exporting...");

                TxtEntry[] abilities = PrepareEntries();

                FileCommander.PrepareFileDirectory(exportPath);
                TxtWriter.WriteStrings(exportPath, abilities);

                Log.Message($"[{TypeName}] Exporting completed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"[{TypeName}] Failed to export resource.");
            }
        }
    }
}
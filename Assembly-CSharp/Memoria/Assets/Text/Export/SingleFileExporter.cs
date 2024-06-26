using System;
using System.IO;
using Memoria.Prime;

namespace Memoria.Assets
{
    public abstract class SingleFileExporter : IExporter
    {
        protected abstract String TypeName { get; }
        protected abstract TextResourcePath ExportPath { get; }

        protected abstract TxtEntry[] PrepareEntries();

        public void Export()
        {
            try
            {
                TextResourcePath exportPath = ExportPath;
                if (File.Exists(exportPath.Value))
                {
                    Log.Warning($"[{TypeName}] Export was skipped bacause a file already exists: [{exportPath}].");
                    return;
                }

                Log.Message($"[{TypeName}] Exporting...");

                TxtEntry[] entries = PrepareEntries();

                FileCommander.PrepareFileDirectory(exportPath.Value);
                exportPath.WriteAll(entries);

                Log.Message($"[{TypeName}] Exporting completed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"[{TypeName}] Failed to export resource.");
            }
        }
    }
}

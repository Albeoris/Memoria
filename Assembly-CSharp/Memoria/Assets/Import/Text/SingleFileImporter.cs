using System;
using System.IO;
using Memoria.Prime;

namespace Memoria.Assets
{
    public abstract class SingleFileImporter : TextImporter
    {
        protected abstract String TypeName { get; }
        protected abstract String ImportPath { get; }
        protected abstract void ProcessEntries(TxtEntry[] entreis);

        protected override Boolean LoadExternal()
        {
            try
            {
                String importPath = ImportPath;
                if (!File.Exists(importPath))
                {
                    Log.Warning($"[{TypeName}] Import was skipped bacause a file does not exist: [{importPath}].");
                    return false;
                }

                Log.Message($"[{TypeName}] Importing from [{importPath}]...");

                TxtEntry[] entries = TxtReader.ReadStrings(importPath);

                ProcessEntries(entries);

                Log.Message($"[{TypeName}] Importing completed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"[{TypeName}] Failed to import resource from.");
                return false;
            }
        }
    }
}
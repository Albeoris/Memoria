using Memoria.Prime;
using System;
using System.IO;

namespace Memoria.Assets
{
    public abstract class SingleFileImporter : TextImporter
    {
        protected abstract String TypeName { get; }
        protected abstract TextResourceReference ImportPath { get; }
        protected abstract void ProcessEntries(TxtEntry[] entreis);

        protected override Boolean LoadExternal()
        {
            try
            {
                TextResourceReference importPath = ImportPath;
                if (!importPath.IsExists(out TextResourcePath existingFile))
                {
                    Log.Warning($"[{TypeName}] Import was skipped because a file does not exist: [{importPath}].");
                    return false;
                }

                Log.Message($"[{TypeName}] Importing from [{importPath}]...");

                TxtEntry[] entries = existingFile.ReadAll();

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

using System;
using System.IO;
using Memoria.Prime;
using UnityEngine;

namespace Memoria.Assets
{
    public sealed class CreditsImporter : TextImporter
    {
        public String TypeName => nameof(CreditsImporter);

        private readonly String _internalPath;
        private readonly String _externalPath;

        public String Result { get; private set; }

        public CreditsImporter(String internalPath, String externalPath)
        {
            _internalPath = internalPath;
            _externalPath = externalPath;
        }

        public static String TryLoadCredits(String internalPath, String externalPath)
        {
            CreditsImporter importer = new CreditsImporter(internalPath, externalPath);
            if (Configuration.Import.Text)
            {
                if (importer.LoadExternal() || importer.LoadInternal())
                    return importer.Result;
            }
            else
            {
                if (importer.LoadInternal())
                    return importer.Result;
            }
            return null;
        }

        protected override Boolean LoadExternal()
        {
            try
            {
                String externalPath = _externalPath;
                if (!File.Exists(_externalPath))
                {
                    externalPath = ModTextResources.Import.Credits;
                    if (!File.Exists(externalPath))
                    {
                        Log.Warning($"[{TypeName}] Import was skipped bacause a file does not exist: [{_externalPath}].");
                        return false;
                    }
                }

                Log.Message($"[{TypeName}] Importing from [{externalPath}]...");

                Result = File.ReadAllText(externalPath);

                Log.Message($"[{TypeName}] Importing completed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"[{TypeName}] Failed to import resource.");
                return false;
            }
        }

        protected override Boolean LoadInternal()
        {
            TextAsset textAsset = AssetManager.Load<TextAsset>(_internalPath, false);
            if (textAsset != null)
            {
                Result = textAsset.text;
                return true;
            }
            return false;
        }
    }
}
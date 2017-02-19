using System;
using System.IO;
using System.Text;
using Memoria.Prime;
using UnityEngine;

namespace Memoria.Assets
{
    public sealed class CreditsExporter : IExporter
    {
        private const String TypeName = nameof(CreditsExporter);

        private readonly TextAsset _amazonCredits;
        private readonly TextAsset _mobileCredits;
        private readonly TextAsset _estoreCredits;
        private readonly TextAsset _steamCredits;

        public CreditsExporter()
        {
            try
            {
                _amazonCredits = AssetManager.Load<TextAsset>("EmbeddedAsset/Manifest/Text/StaffCredits_Amazon.txt", false);
                _mobileCredits = AssetManager.Load<TextAsset>("EmbeddedAsset/Manifest/Text/StaffCredits_Mobile.txt", false);
                _estoreCredits = AssetManager.Load<TextAsset>("EmbeddedAsset/Manifest/Text/StaffCredits_EStore.txt", false);
                _steamCredits = AssetManager.Load<TextAsset>("EmbeddedAsset/Manifest/Text/StaffCredits_Steam.txt", false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"[{TypeName}] Failed to load resources.");
            }
        }

        public void Export()
        {
            try
            {
                Directory.CreateDirectory(ModTextResources.Export.ManifestDirectory);

                Export(_amazonCredits, ModTextResources.Export.CreditsAmazon);
                Export(_mobileCredits, ModTextResources.Export.CreditsMobile);
                Export(_estoreCredits, ModTextResources.Export.CreditsEStore);
                Export(_steamCredits, ModTextResources.Export.CreditsSteam);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"[{TypeName}] Failed to export resources.");
            }
        }

        private static void Export(TextAsset asset, String exportPath)
        {
            try
            {
                if (File.Exists(exportPath))
                {
                    Log.Warning($"[{TypeName}] Export was skipped bacause a file already exists: [{exportPath}].");
                    return;
                }

                Log.Message($"[{TypeName}] Exporting to {exportPath}...");

                if (asset != null)
                {
                    File.WriteAllText(exportPath, asset.text, Encoding.Unicode);
                    Log.Message($"[{TypeName}] Exporting completed successfully.");
                }
                else
                {
                    Log.Warning($"[{TypeName}] Nothing to export.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"[{TypeName}] Failed to export resource.");
            }
        }
    }
}
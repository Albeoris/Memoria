using Memoria.Prime;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Memoria.Assets
{
    public sealed class CreditsExporter : IExporter
    {
        private const String TypeName = nameof(CreditsExporter);

        private readonly String _amazonCredits;
        private readonly String _mobileCredits;
        private readonly String _estoreCredits;
        private readonly String _steamCredits;

        public CreditsExporter()
        {
            try
            {
                _amazonCredits = AssetManager.LoadString("EmbeddedAsset/Manifest/Text/StaffCredits_Amazon.txt");
                _mobileCredits = AssetManager.LoadString("EmbeddedAsset/Manifest/Text/StaffCredits_Mobile.txt");
                _estoreCredits = AssetManager.LoadString("EmbeddedAsset/Manifest/Text/StaffCredits_EStore.txt");
                _steamCredits = AssetManager.LoadString("EmbeddedAsset/Manifest/Text/StaffCredits_Steam.txt");
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

        private static void Export(String asset, String exportPath)
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
                    File.WriteAllText(exportPath, asset, Encoding.Unicode);
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

using Memoria.Prime;
using System;
using UnityEngine;

namespace Memoria.Assets
{
    public static class ResourceExporter
    {
        public static void ExportSafe()
        {
            try
            {
                if (!Configuration.Export.Enabled)
                {
                    Log.Message("[ResourceExporter] Pass through {Configuration.Export.Enabled = 0}.");
                    return;
                }

                TextResourceExporter.ExportSafe();
                GraphicResourceExporter.ExportSafe();
                FieldSceneExporter.ExportSafe();
                BattleSceneExporter.ExportSafe();
                Log.Message("[ResourceExporter] Application will now quit. Please disable Configuration.Export.Enabled and restart the game.");
                UIManager.Input.ConfirmQuit();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ResourceExporter] Failed to export resources.");
            }
        }
    }
}

using System;
using Memoria.Prime;

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
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ResourceExporter] Failed to export resources.");
            }
        }
    }
}
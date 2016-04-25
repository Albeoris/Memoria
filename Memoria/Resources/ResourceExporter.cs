using System;

namespace Memoria
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
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ResourceExporter] Failed to export resources.");
            }
        }
    }
}
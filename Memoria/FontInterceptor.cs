using System;
using UnityEngine;

namespace Memoria
{
    // Don't call from Patcher!
    public static class FontInterceptor
    {
        private static readonly Font DefaultFont = InitializeFont();

        // Keep signature
        public static Font LoadFont(Font originalFont)
        {
            Log.Message("[FontInterceptor] Changing font [Original: {0}]", originalFont);
            try
            {
                return DefaultFont ?? originalFont;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[FontInterceptor] Failed to intercept font.");
                return originalFont;
            }
        }

        private static Font InitializeFont()
        {
            OnApplicationStartSafe();

            Log.Message("[FontInterceptor] Dynamic font initialization.");
            try
            {
                if (!Configuration.Font.Enabled)
                {
                    Log.Message("[FontInterceptor] Pass through {Configuration.Font.Enabled = 0}.");
                    return null;
                }

                if (Configuration.Font.Names.IsNullOrEmpty())
                {
                    Log.Error("[FontInterceptor] An invalid font configuration [Configuration.Font.Names is empty].");
                    return null;
                }

                Log.Message("[FontInterceptor] Initialize new font: [Names: {{{0}}}, Size: {1}]", String.Join(", ", Configuration.Font.Names), Configuration.Font.Size);
                return Font.CreateDynamicFontFromOSFont(Configuration.Font.Names, Configuration.Font.Size);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[FontInterceptor] Failed to create a dynamic font.");
                return null;
            }
        }

        private static void OnApplicationStartSafe()
        {
            try
            {
                ResourceExporter.ExportSafe();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
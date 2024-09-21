using System;
using System.Linq;
using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Vanilla : Settings_Vanilla
    {
        public SettingsGrid_Vanilla()
        {
            CreateCheckbox("IsX64", "x64", Lang.Settings.Xsixfour_Tooltip, 0, "IsX64Enabled");
            CreateCheckbox("IsDebugMode", Lang.Settings.Debuggable, Lang.Settings.Debuggable_Tooltip, 4);
            CreateCheckbox("CheckUpdates", Lang.Settings.CheckUpdates, Lang.Settings.CheckUpdates_Tooltip);

            String OSversion = $"{Environment.OSVersion}";
            if (OSversion.Contains("Windows") && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WINELOADER")))
                CreateCheckbox("SteamOverlayFix", Lang.SteamOverlay.OptionLabel, Lang.Settings.SteamOverlayFix_Tooltip);

            try
            {
                LoadSettings();
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }
    }
}

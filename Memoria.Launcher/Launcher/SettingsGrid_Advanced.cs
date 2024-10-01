using System;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Advanced : Settings
    {
        public SettingsGrid_Advanced()
        {
            CreateCheckbox("AntiAliasing", Lang.Settings.AntiAliasing, Lang.Settings.AntiAliasing_Tooltip);

            LoadSettings();
        }
    }
}

using System;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Advanced : Settings
    {
        public SettingsGrid_Advanced()
        {
            //CreateCheckbox("AntiAliasing", Lang.Settings.AntiAliasing, Lang.Settings.AntiAliasing_Tooltip);

            CreateCheckbox("AudioBackend", Lang.Settings.AudioBackend, Lang.Settings.AudioBackend_Tooltip);
            CreateCheckbox("WorldSmoothTexture", Lang.Settings.WorldSmoothTexture, Lang.Settings.WorldSmoothTexture_Tooltip);
            CreateCheckbox("BattleSmoothTexture", Lang.Settings.BattleSmoothTexture, Lang.Settings.BattleSmoothTexture_Tooltip);
            CreateCheckbox("ElementsSmoothTexture", Lang.Settings.ElementsSmoothTexture, Lang.Settings.ElementsSmoothTexture_Tooltip);
            LoadSettings();
        }
    }
}

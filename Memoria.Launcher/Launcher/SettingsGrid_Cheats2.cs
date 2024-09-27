using System;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Cheats2 : Settings
    {
        public SettingsGrid_Cheats2()
        {

            CreateTextbloc(Lang.Settings.Other, true);
            CreateCheckbox("EasyTetraMaster", Lang.Settings.EasyTetraMaster, Lang.Settings.EasyTetraMaster_Tooltip);
            CreateCheckbox("ExcaliburIINoTimeLimit", Lang.Settings.ExcaliburIINoTimeLimit, Lang.Settings.ExcaliburIINoTimeLimit_Tooltip);

            LoadSettings();
        }
    }
}

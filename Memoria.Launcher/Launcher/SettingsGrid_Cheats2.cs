using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Cheats2 : UiGrid
    {
        public SettingsGrid_Cheats2()
        {
            DataContext = (MainWindow)Application.Current.MainWindow;

            CreateHeading(Lang.Settings.Other);
            CreateCheckbox("EasyTetraMaster", Lang.Settings.EasyTetraMaster, Lang.Settings.EasyTetraMaster_Tooltip);
            CreateCheckbox("ExcaliburIINoTimeLimit", Lang.Settings.ExcaliburIINoTimeLimit, Lang.Settings.ExcaliburIINoTimeLimit_Tooltip);
            CreateCheckbox("EasyJumpRopeMinigame", Lang.Settings.EasyJumpRopeMinigame, Lang.Settings.EasyJumpRopeMinigame_Tooltip);
            CreateCheckbox("HippaulRacingViviSpeed", Lang.Settings.HippaulRacingViviSpeed, Lang.Settings.HippaulRacingViviSpeed_Tooltip);

        }
    }
}

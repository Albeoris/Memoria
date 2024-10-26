using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Cheats2 : UiGrid
    {
        public SettingsGrid_Cheats2()
        {
            DataContext = (MainWindow)Application.Current.MainWindow;

            CreateHeading("Settings.Minigames");
            CreateCheckbox("EasyTetraMaster", "Settings.EasyTetraMaster", "Settings.EasyTetraMaster_Tooltip");
            CreateCheckbox("ExcaliburIINoTimeLimit", "Settings.ExcaliburIINoTimeLimit", "Settings.ExcaliburIINoTimeLimit_Tooltip");
            CreateCheckbox("EasyJumpRopeMinigame", "Settings.EasyJumpRopeMinigame", "Settings.EasyJumpRopeMinigame_Tooltip");
            CreateCheckbox("HippaulRacingViviSpeed", "Settings.HippaulRacingViviSpeed", "Settings.HippaulRacingViviSpeed_Tooltip");
            CreateCheckbox("SwordplayAssistance", "Settings.SwordplayAssistance", "Settings.SwordplayAssistance_Tooltip");
            CreateCheckbox("FrogCatchingIncrement", "Settings.FrogCatchingIncrement", "Settings.FrogCatchingIncrement_Tooltip");

        }
    }
}

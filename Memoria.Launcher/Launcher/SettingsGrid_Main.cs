using System;
using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Main : UiGrid
    {
        public SettingsGrid_Main()
        {
            DataContext = (MainWindow)Application.Current.MainWindow;

            CreateHeading("Settings.QoL");

            CreateCheckbox("SkipIntros", "Settings.SkipIntrosToMainMenu", "Settings.SkipIntrosToMainMenu_Tooltip");
            CreateCheckbox("BattleSwirlFrames", "Settings.SkipBattleSwirl", "Settings.SkipBattleSwirl_Tooltip");

            String[] comboboxchoices = [
                "Settings.UIColumnsChoice0", // default 8 - 6
                "Settings.UIColumnsChoice1", // 3 columns
                "Settings.UIColumnsChoice2"  // 4 columns
            ];
            CreateCombobox("UIColumnsChoice", comboboxchoices, 50, "Settings.UIColumnsChoice", "Settings.UIColumnsChoice_Tooltip");



            CreateHeading("Settings.Battle");

            comboboxchoices = [
                "Settings.BattleInterfaceType0",
                "Settings.BattleInterfaceType1",
                "Settings.BattleInterfaceType2"
            ];
            CreateCombobox("BattleInterface", comboboxchoices, 50, "Settings.BattleInterface", "Settings.BattleInterface_Tooltip");

            comboboxchoices = [
                "Settings.SpeedChoiceType0",
                "Settings.SpeedChoiceType1",
                "Settings.SpeedChoiceType2",
                "Settings.SpeedChoiceType3"
            ];
            CreateCombobox("ATBModeChoice", comboboxchoices, 50, "Settings.SpeedChoice", "Settings.SpeedChoice_Tooltip");

            CreateSlider("BattleTPSDividedBy15", "BattleTPS", 15, 75, 3, "{0}x", 50, "Settings.BattleTPS", "Settings.BattleTPS_Tooltip");



            CreateHeading("Settings.Volume");

            CreateSlider("SoundVolume", "SoundVolume", 0, 100, 5, "{0}", 50, "Settings.SoundVolume");
            CreateSlider("MusicVolume", "MusicVolume", 0, 100, 5, "{0}", 50, "Settings.MusicVolume");
            CreateSlider("MovieVolume", "MovieVolume", 0, 100, 5, "{0}", 50, "Settings.MovieVolume");
        }
    }
}

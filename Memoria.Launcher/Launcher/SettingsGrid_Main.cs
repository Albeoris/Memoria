using System;
using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Main : UiGrid
    {
        public SettingsGrid_Main()
        {
            DataContext = (MainWindow)Application.Current.MainWindow;

            CreateHeading(Lang.Settings.QoL);

            CreateCheckbox("SkipIntros", Lang.Settings.SkipIntrosToMainMenu, Lang.Settings.SkipIntrosToMainMenu_Tooltip);
            CreateCheckbox("BattleSwirlFrames", Lang.Settings.SkipBattleSwirl, Lang.Settings.SkipBattleSwirl_Tooltip);

            String[] comboboxchoices = new String[]{
                Lang.Settings.UIColumnsChoice0, // default 8 - 6
                Lang.Settings.UIColumnsChoice1, // 3 columns
                Lang.Settings.UIColumnsChoice2  // 4 columns
            };
            CreateCombobox("UIColumnsChoice", comboboxchoices, 50, Lang.Settings.UIColumnsChoice, Lang.Settings.UIColumnsChoice_Tooltip);



            CreateHeading(Lang.Settings.Battle);

            comboboxchoices = new String[]{
                Lang.Settings.BattleInterfaceType0,
                Lang.Settings.BattleInterfaceType1,
                Lang.Settings.BattleInterfaceType2
            };
            CreateCombobox("BattleInterface", comboboxchoices, 50, Lang.Settings.BattleInterface, Lang.Settings.BattleInterface_Tooltip);

            comboboxchoices = new String[]{
                Lang.Settings.SpeedChoiceType0,
                Lang.Settings.SpeedChoiceType1,
                Lang.Settings.SpeedChoiceType2,
                Lang.Settings.SpeedChoiceType3
            };
            CreateCombobox("ATBModeChoice", comboboxchoices, 50, Lang.Settings.SpeedChoice, Lang.Settings.SpeedChoice_Tooltip);

            CreateSlider("BattleTPSDividedBy15", "BattleTPS", 15, 75, 3, "{0}x", 50, Lang.Settings.BattleTPS, Lang.Settings.BattleTPS_Tooltip);



            CreateHeading(Lang.Settings.Volume);

            CreateSlider("SoundVolume", "SoundVolume", 0, 100, 5, "{0}", 50, Lang.Settings.SoundVolume);
            CreateSlider("MusicVolume", "MusicVolume", 0, 100, 5, "{0}", 50, Lang.Settings.MusicVolume);
            CreateSlider("MovieVolume", "MovieVolume", 0, 100, 5, "{0}", 50, Lang.Settings.MovieVolume);
        }
    }
}

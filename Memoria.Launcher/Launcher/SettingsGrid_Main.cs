using System;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Main : Settings
    {
        public SettingsGrid_Main()
        {

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


            IniFile.SanitizeMemoriaIni();
            LoadSettings();
        }

        public void ComeBackToLauncherReloadSettings()
        {
            LoadSettings();
        }
    }
}

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

            CreateTextbloc(Lang.Settings.UIColumnsChoice, Lang.Settings.UIColumnsChoice_Tooltip);
            String[] comboboxchoices = new String[]{
                Lang.Settings.UIColumnsChoice0, // default 8 - 6
                Lang.Settings.UIColumnsChoice1, // 3 columns
                Lang.Settings.UIColumnsChoice2 // 4 columns
            };
            CreateCombobox("UIColumnsChoice", comboboxchoices);








            CreateHeading(Lang.Settings.Battle);

            CreateTextbloc(Lang.Settings.BattleInterface, Lang.Settings.BattleInterface_Tooltip);
            comboboxchoices = new String[]{
                Lang.Settings.BattleInterfaceType0,
                Lang.Settings.BattleInterfaceType1,
                Lang.Settings.BattleInterfaceType2
            };
            CreateCombobox("BattleInterface", comboboxchoices);

            CreateTextbloc(Lang.Settings.SpeedChoice, Lang.Settings.SpeedChoice_Tooltip);
            comboboxchoices = new String[]{
                Lang.Settings.SpeedChoiceType0,
                Lang.Settings.SpeedChoiceType1,
                Lang.Settings.SpeedChoiceType2,
                Lang.Settings.SpeedChoiceType3
            };
            CreateCombobox("ATBModeChoice", comboboxchoices);

            /*
            CreateTextbloc(Lang.Settings.AccessBattleMenu, false, Lang.Settings.AccessBattleMenu_Tooltip);
            String[] accessmenuchoices =
            {
                Lang.Settings.AccessBattleMenuType0,
                Lang.Settings.AccessBattleMenuType1,
                Lang.Settings.AccessBattleMenuType2,
                Lang.Settings.AccessBattleMenuType3
            };
            CreateCombobox("AccessBattleMenu", accessmenuchoices, 4, Lang.Settings.AccessBattleMenu_Tooltip);
            */



            //CreateTextbloc(Lang.Settings.BattleTPS, Lang.Settings.BattleTPS_Tooltip);
            CreateSlider("BattleTPSDividedBy10", "BattleTPS", 15, 75, 1, "{0}x", 50, Lang.Settings.BattleTPS, Lang.Settings.BattleTPS_Tooltip);

            LoadSettings();
        }

        public void ComeBackToLauncherReloadSettings()
        {
            LoadSettings();
        }
    }
}

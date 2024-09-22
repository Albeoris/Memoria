using System;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Main : Settings
    {
        public SettingsGrid_Main()
        {
            SetRows(2);

            CreateTextbloc(Lang.Settings.Other, true);

            CreateTextbloc(Lang.Settings.BattleInterface, false, Lang.Settings.BattleInterface_Tooltip);
            String[] comboboxchoices = new String[]{
                Lang.Settings.BattleInterfaceType0,
                Lang.Settings.BattleInterfaceType1,
                Lang.Settings.BattleInterfaceType2
            };
            CreateCombobox("BattleInterface", comboboxchoices);

            CreateTextbloc(Lang.Settings.UIColumnsChoice, false, Lang.Settings.UIColumnsChoice_Tooltip);
            comboboxchoices = new String[]{
                Lang.Settings.UIColumnsChoice0, // default 8 - 6
                Lang.Settings.UIColumnsChoice1, // 3 columns
                Lang.Settings.UIColumnsChoice2 // 4 columns
            };
            CreateCombobox("UIColumnsChoice", comboboxchoices);

            CreateCheckbox("SkipIntros", Lang.Settings.SkipIntrosToMainMenu, Lang.Settings.SkipIntrosToMainMenu_Tooltip);
            CreateCheckbox("BattleSwirlFrames", Lang.Settings.SkipBattleSwirl, Lang.Settings.SkipBattleSwirl_Tooltip);
            CreateCheckbox("HideCards", Lang.Settings.HideSteamBubbles, Lang.Settings.HideSteamBubbles_Tooltip);

            CreateTextbloc(Lang.Settings.SpeedChoice, false, Lang.Settings.SpeedChoice_Tooltip);
            comboboxchoices = new String[]{
                Lang.Settings.SpeedChoiceType0,
                Lang.Settings.SpeedChoiceType1,
                Lang.Settings.SpeedChoiceType2,
                //Lang.Settings.SpeedChoiceType3,
                //Lang.Settings.SpeedChoiceType4,
                Lang.Settings.SpeedChoiceType5
            };
            CreateCombobox("Speed", comboboxchoices);

            CreateTextbloc(Lang.Settings.TripleTriad, false, Lang.Settings.TripleTriad_Tooltip);
            comboboxchoices = new String[]{
                Lang.Settings.TripleTriadType0,
                Lang.Settings.TripleTriadType1,
                Lang.Settings.TripleTriadType2
            };
            CreateCombobox("TripleTriad", comboboxchoices);

            IniFile.SanitizeMemoriaIni();

            LoadSettings();
        }

        public void ComeBackToLauncherReloadSettings()
        {
            LoadSettings();
        }
    }
}

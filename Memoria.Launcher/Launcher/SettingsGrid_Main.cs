using System;
using System.Drawing.Text;
using System.Windows.Controls;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Main : Settings
    {
        public SettingsGrid_Main()
        {
            SetRows(2);
            
            CreateCheckbox("WidescreenSupport", Lang.Settings.Widescreen, Lang.Settings.Widescreen_Tooltip);
            CreateCheckbox("AntiAliasing", Lang.Settings.AntiAliasing, Lang.Settings.AntiAliasing_Tooltip);

            CreateTextbloc(Lang.Settings.FPSDropboxChoice, false, Lang.Settings.SharedFPS_Tooltip);
            String[] comboboxchoices = new String[]{
                Lang.Settings.FPSDropboxChoice0, // default 30 20 15
                Lang.Settings.FPSDropboxChoice1, // 30
                Lang.Settings.FPSDropboxChoice2, // 60
                Lang.Settings.FPSDropboxChoice3, // 90
                Lang.Settings.FPSDropboxChoice4  // 120
            };
            CreateCombobox("FPSDropboxChoice", comboboxchoices);

            CreateTextbloc(Lang.Settings.CameraStabilizer, false, Lang.Settings.CameraStabilizer_Tooltip);

            CreateSlider("CameraStabilizer", "CameraStabilizer", 0, 99, 1);

            CreateTextbloc(Lang.Settings.BattleInterface, false, Lang.Settings.BattleInterface_Tooltip);
            comboboxchoices = new String[]{
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

            CreateCheckbox("UsePsxFont", Lang.Settings.UsePsxFont, Lang.Settings.UsePsxFont_Tooltip);

            CreateTextbloc(Lang.Settings.FontChoice, false, Lang.Settings.FontChoice_Tooltip);
            FontCollection installedFonts = new InstalledFontCollection();
            String[] fontNames = new String[installedFonts.Families.Length + 2];
            fontNames[0] = "Final Fantasy IX PC";
            fontNames[1] = "Final Fantasy IX PSX";
            for (Int32 fontindex = 0; fontindex < installedFonts.Families.Length; ++fontindex)
                fontNames[fontindex + 2] = installedFonts.Families[fontindex].Name;
            CreateCombobox("FontChoice", fontNames, 2, "", true);

            IniFile.SanitizeMemoriaIni();

            LoadSettings();
        }

        private ComboBox FontChoiceBox;

        public void ComeBackToLauncherReloadSettings()
        {
            LoadSettings();
        }
    }
}

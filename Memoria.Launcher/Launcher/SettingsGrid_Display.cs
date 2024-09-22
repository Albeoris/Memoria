using System;
using System.Drawing.Text;
using System.Windows.Controls;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Display : Settings
    {
        public SettingsGrid_Display()
        {
            SetRows(2);


            CreateTextbloc(Lang.Settings.FPSDropboxChoice, false, Lang.Settings.SharedFPS_Tooltip);
            String[] comboboxchoices = new String[]{
                Lang.Settings.FPSDropboxChoice0, // default 30 20 15
                Lang.Settings.FPSDropboxChoice1, // 30
                Lang.Settings.FPSDropboxChoice2, // 60
                Lang.Settings.FPSDropboxChoice3, // 90
                Lang.Settings.FPSDropboxChoice4  // 120
            };
            CreateCombobox("FPSDropboxChoice", comboboxchoices);

            CreateCheckbox("WidescreenSupport", Lang.Settings.Widescreen, Lang.Settings.Widescreen_Tooltip);
            CreateCheckbox("AntiAliasing", Lang.Settings.AntiAliasing, Lang.Settings.AntiAliasing_Tooltip, 4);

            CreateTextbloc(Lang.Settings.CameraStabilizer, false, Lang.Settings.CameraStabilizer_Tooltip);
            CreateSlider("CameraStabilizer", "CameraStabilizer", 0, 99, 1);

            CreateCheckbox("UsePsxFont", Lang.Settings.UsePsxFont, Lang.Settings.UsePsxFont_Tooltip);

            CreateTextbloc(Lang.Settings.FontChoice, false, Lang.Settings.FontChoice_Tooltip);
            FontCollection installedFonts = new InstalledFontCollection();
            String[] fontNames = new String[installedFonts.Families.Length + 2];
            fontNames[0] = "Final Fantasy IX PC";
            fontNames[1] = "Final Fantasy IX PSX";
            for (Int32 fontindex = 0; fontindex < installedFonts.Families.Length; ++fontindex)
                fontNames[fontindex + 2] = installedFonts.Families[fontindex].Name;
            CreateCombobox("FontChoice", fontNames, 2, "", true);

            LoadSettings();
        }
    }
}

using System;
using System.Drawing.Text;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Display : Settings
    {
        public SettingsGrid_Display()
        {
            CreateTextbloc(Lang.Settings.FPSDropboxChoice, Lang.Settings.SharedFPS_Tooltip);
            String[] comboboxchoices = new String[]{
                Lang.Settings.FPSDropboxChoice0, // default 30 20 15
                Lang.Settings.FPSDropboxChoice1, // 30
                Lang.Settings.FPSDropboxChoice2, // 60
                Lang.Settings.FPSDropboxChoice3, // 90
                Lang.Settings.FPSDropboxChoice4  // 120
            };
            CreateCombobox("FPSDropboxChoice", comboboxchoices);

            CreateCheckbox("WidescreenSupport", Lang.Settings.Widescreen, Lang.Settings.Widescreen_Tooltip);

            //CreateTextbloc(Lang.Settings.CameraStabilizer, Lang.Settings.CameraStabilizer_Tooltip);
            CreateSlider("CameraStabilizer", "CameraStabilizer", 0, 97, 1, "", 50, Lang.Settings.CameraStabilizer, Lang.Settings.CameraStabilizer_Tooltip);

            CreateTextbloc(Lang.Settings.FieldShader, Lang.Settings.FieldShader_Tooltip, "shader_comparison2.jpg");
            comboboxchoices = new String[]{
                Lang.Settings.ShaderDropboxChoice0,
                Lang.Settings.ShaderDropboxChoice1,
                Lang.Settings.ShaderDropboxChoice2,
                Lang.Settings.ShaderDropboxChoice3,
                Lang.Settings.ShaderDropboxChoice4,
                Lang.Settings.ShaderDropboxChoice5
            };
            CreateCombobox("ShaderFieldChoice", comboboxchoices);

            CreateTextbloc(Lang.Settings.BattleShader, Lang.Settings.BattleShader_Tooltip, "shader_comparison2.jpg");
            comboboxchoices = new String[]{
                Lang.Settings.ShaderDropboxChoice0,
                Lang.Settings.ShaderDropboxChoice1,
                Lang.Settings.ShaderDropboxChoice2,
                Lang.Settings.ShaderDropboxChoice3,
                Lang.Settings.ShaderDropboxChoice4,
                Lang.Settings.ShaderDropboxChoice5
            };
            CreateCombobox("ShaderBattleChoice", comboboxchoices);

            CreateCheckbox("UsePsxFont", Lang.Settings.UsePsxFont, Lang.Settings.UsePsxFont_Tooltip, 0, "", "alexandriaPreview.png");

            //CreateTextbloc(Lang.Settings.FontChoice, false, Lang.Settings.FontChoice_Tooltip);
            FontCollection installedFonts = new InstalledFontCollection();
            String[] fontNames = new String[installedFonts.Families.Length + 2];
            fontNames[0] = "Final Fantasy IX PC";
            fontNames[1] = "Final Fantasy IX PSX";
            for (Int32 fontindex = 0; fontindex < installedFonts.Families.Length; ++fontindex)
                fontNames[fontindex + 2] = installedFonts.Families[fontindex].Name;
            CreateCombobox("FontChoice", fontNames, 25, Lang.Settings.FontChoice, Lang.Settings.FontChoice_Tooltip, "", true);

            LoadSettings();
        }
    }
}

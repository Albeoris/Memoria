using System;
using System.Collections.Generic;
using System.Linq;
using Application = System.Windows.Application;
using Memoria.Launcher.Utils;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Display : UiGrid
    {
        public SettingsGrid_Display()
        {
            DataContext = (MainWindow)Application.Current.MainWindow;
            String[] comboboxchoices = [
                "Launcher.Auto",
                "Settings.FPSDropboxChoice0", // default 30 20 15
                "Settings.FPSDropboxChoice1", // 30
                "Settings.FPSDropboxChoice2", // 60
                "Settings.FPSDropboxChoice3", // 90
                "Settings.FPSDropboxChoice4"  // 120
            ];
            CreateCombobox("FPSDropboxChoice", ComboBoxOptions.Localized(comboboxchoices), 50, "Settings.FPSDropboxChoice", "Settings.SharedFPS_Tooltip");

            CreateCheckbox("WidescreenSupport", "Settings.Widescreen", "Settings.Widescreen_Tooltip");

            //CreateTextbloc(Lang.Settings.CameraStabilizer, Lang.Settings.CameraStabilizer_Tooltip);
            CreateSlider("CameraStabilizer", "CameraStabilizer", 0, 97, 1, "", 50, "Settings.CameraStabilizer", "Settings.CameraStabilizer_Tooltip");

            comboboxchoices = [
                "Settings.ShaderDropboxChoice0",
                "Settings.ShaderDropboxChoice1",
                "Settings.ShaderDropboxChoice2",
                "Settings.ShaderDropboxChoice3",
                "Settings.ShaderDropboxChoice4",
                "Settings.ShaderDropboxChoice5"
            ];
            CreateCombobox("ShaderFieldChoice", ComboBoxOptions.Localized(comboboxchoices), 50, "Settings.FieldShader", "Settings.FieldShader_Tooltip", "shader_comparison2.jpg");

            comboboxchoices = [
                "Settings.ShaderDropboxChoice0",
                "Settings.ShaderDropboxChoice1",
                "Settings.ShaderDropboxChoice2",
                "Settings.ShaderDropboxChoice3",
                "Settings.ShaderDropboxChoice4",
                "Settings.ShaderDropboxChoice5"
            ];
            CreateCombobox("ShaderBattleChoice", ComboBoxOptions.Localized(comboboxchoices), 50, "Settings.BattleShader", "Settings.BattleShader_Tooltip", "shader_comparison2.jpg");

            CreateCheckbox("UsePsxFont", "Settings.UsePsxFont", "Settings.UsePsxFont_Tooltip", 0, "", "alexandriaPreview.png");

            IEnumerable<String> fontNames = new[] { "Final Fantasy IX PC", "Final Fantasy IX PSX" }
                .Concat(SystemFontService.Current.InstalledFontNames)
                .Distinct(StringComparer.OrdinalIgnoreCase);
            CreateCombobox("FontChoice", ComboBoxOptions.Literal(fontNames), 45, "Settings.FontChoice", "Settings.FontChoice_Tooltip", "", ComboBoxSelectionMode.Value);

        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using Application = System.Windows.Application;

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Display : UiGrid
    {
        public SettingsGrid_Display()
        {
            DataContext = (MainWindow)Application.Current.MainWindow;
            String[] comboboxchoices = [
                "Settings.FPSDropboxChoice0", // default 30 20 15
                "Settings.FPSDropboxChoice1", // 30
                "Settings.FPSDropboxChoice2", // 60
                "Settings.FPSDropboxChoice3", // 90
                "Settings.FPSDropboxChoice4"  // 120
            ];
            CreateCombobox("FPSDropboxChoice", comboboxchoices, 50, "Settings.FPSDropboxChoice", "Settings.SharedFPS_Tooltip");

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
            CreateCombobox("ShaderFieldChoice", comboboxchoices, 50, "Settings.FieldShader", "Settings.FieldShader_Tooltip", "shader_comparison2.jpg");

            comboboxchoices = [
                "Settings.ShaderDropboxChoice0",
                "Settings.ShaderDropboxChoice1",
                "Settings.ShaderDropboxChoice2",
                "Settings.ShaderDropboxChoice3",
                "Settings.ShaderDropboxChoice4",
                "Settings.ShaderDropboxChoice5"
            ];
            CreateCombobox("ShaderBattleChoice", comboboxchoices, 50, "Settings.BattleShader", "Settings.BattleShader_Tooltip", "shader_comparison2.jpg");

            CreateCheckbox("UsePsxFont", "Settings.UsePsxFont", "Settings.UsePsxFont_Tooltip", 0, "", "alexandriaPreview.png");

            List<String> fontNames = ["Final Fantasy IX PC", "Final Fantasy IX PSX"];
            if (File.Exists("FontList"))
            {
                try
                {
                    String[] fonts = File.ReadAllLines("FontList");
                    foreach (String s in fonts)
                    {
                        if (String.IsNullOrWhiteSpace(s)) continue;
                        fontNames.Add(s);
                    }
                }
                catch { }
            }
            CreateCombobox("FontChoice", fontNames, 25, "Settings.FontChoice", "Settings.FontChoice_Tooltip", "", true);

        }
    }
}

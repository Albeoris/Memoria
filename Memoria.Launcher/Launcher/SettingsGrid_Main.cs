using System;
using System.Drawing.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Binding = System.Windows.Data.Binding;
using HorizontalAlignment = System.Windows.HorizontalAlignment;


// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming
// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable ArrangeStaticMemberQualifier
#pragma warning disable 649
#pragma warning disable 169

namespace Memoria.Launcher
{
    public sealed class SettingsGrid_Main : Settings
    {
        public SettingsGrid_Main()
        {
            SetRows(2);
            SetCols(8);

            Width = 260;
            Margin = new Thickness(0);

            DataContext = this;

            Thickness rowMargin = new(0, 3, 0, 3);


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

            Row++;

            TextBlock CameraStabilizerIndex = AddUiElement(UiTextBlockFactory.Create(""), Row, 0, 1, 8);
            CameraStabilizerIndex.SetBinding(TextBlock.TextProperty, new Binding(nameof(CameraStabilizer)) { Mode = BindingMode.TwoWay });
            CameraStabilizerIndex.Foreground = Brushes.White;
            CameraStabilizerIndex.Margin = rowMargin;
            Slider CameraStabilizerSlider = AddUiElement(UiSliderFactory.Create(0), Row, 1, 1, 8);
            CameraStabilizerSlider.SetBinding(Slider.ValueProperty, new Binding(nameof(CameraStabilizer)) { Mode = BindingMode.TwoWay });
            CameraStabilizerSlider.TickFrequency = 1;
            CameraStabilizerSlider.TickPlacement = TickPlacement.BottomRight;
            CameraStabilizerSlider.Height = 20;
            CameraStabilizerSlider.IsSnapToTickEnabled = true;
            CameraStabilizerSlider.Minimum = 0;
            CameraStabilizerSlider.Maximum = 99;
            CameraStabilizerSlider.Margin = new(3, 3, 3, 3);


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
            FontChoiceBox = AddUiElement(UiComboBoxFactory.Create(), Row, 2, 1, 7);
            FontChoiceBox.Height = 20;
            FontChoiceBox.FontSize = 10;
            FontChoiceBox.Margin = rowMargin;

            FontCollection installedFonts = new InstalledFontCollection();
            String[] fontNames = new String[installedFonts.Families.Length + 2];
            fontNames[0] = "Final Fantasy IX PC";
            fontNames[1] = "Final Fantasy IX PSX";
            for (Int32 fontindex = 0; fontindex < installedFonts.Families.Length; ++fontindex)
                fontNames[fontindex + 2] = installedFonts.Families[fontindex].Name;
            FontChoiceBox.ItemsSource = fontNames;
            FontChoiceBox.SetBinding(Selector.SelectedItemProperty, new Binding(nameof(FontChoice)) { Mode = BindingMode.TwoWay });

            IniFile.SanitizeMemoriaIni();

            LoadSettings();
            FontChoiceBox.SelectedItem = _fontChoice;
        }

        private ComboBox FontChoiceBox;

        public void ComeBackToLauncherReloadSettings()
        {
            LoadSettings();
        }
    }
}

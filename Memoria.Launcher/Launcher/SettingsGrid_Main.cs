using System;
using System.ComponentModel;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Application = System.Windows.Application;
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
            SetRows(22);
            SetCols(9);

            Width = 260;
            VerticalAlignment = VerticalAlignment.Bottom;
            HorizontalAlignment = HorizontalAlignment.Right;
            Margin = new Thickness(0);

            DataContext = this;

            Thickness rowMargin = new(8, 2, 3, 2);


            CreateCheckbox("WidescreenSupport", Lang.Settings.Widescreen, Lang.Settings.Widescreen_Tooltip);
            CreateCheckbox("AntiAliasing", Lang.Settings.AntiAliasing, Lang.Settings.AntiAliasing_Tooltip);

            Row++;

            TextBlock FPSDropboxText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.FPSDropboxChoice), Row, 0, 1, 4);
            FPSDropboxText.Foreground = Brushes.White;
            FPSDropboxText.Margin = rowMargin;
            FPSDropboxText.ToolTip = Lang.Settings.SharedFPS_Tooltip;
            ComboBox FPSDropbox = AddUiElement(UiComboBoxFactory.Create(), Row, 4, 1, 5);
            FPSDropbox.ItemsSource = new String[]{
                Lang.Settings.FPSDropboxChoice0, // default 30 20 15
                Lang.Settings.FPSDropboxChoice1, // 30
                Lang.Settings.FPSDropboxChoice2, // 60
                Lang.Settings.FPSDropboxChoice3, // 90
                Lang.Settings.FPSDropboxChoice4  // 120
            };
            FPSDropbox.SetBinding(Selector.SelectedIndexProperty, new Binding(nameof(FPSDropboxChoice)) { Mode = BindingMode.TwoWay });
            FPSDropbox.Height = 20;
            FPSDropbox.FontSize = 10;
            FPSDropbox.Margin = rowMargin;

            Row++;

            TextBlock CameraStabilizerText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.CameraStabilizer), Row, 0, 1, 8);
            CameraStabilizerText.Foreground = Brushes.White;
            CameraStabilizerText.Margin = rowMargin;
            CameraStabilizerText.ToolTip = Lang.Settings.CameraStabilizer_Tooltip;

            Row++;

            TextBlock CameraStabilizerIndex = AddUiElement(UiTextBlockFactory.Create(""), Row, 0, 1, 1);
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

            Row++;

            TextBlock battleInterfaceText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.BattleInterface), Row, 0, 1, 4);
            battleInterfaceText.Foreground = Brushes.White;
            battleInterfaceText.Margin = rowMargin;
            battleInterfaceText.ToolTip = Lang.Settings.BattleInterface_Tooltip;
            ComboBox battleInterfaceBox = AddUiElement(UiComboBoxFactory.Create(), Row, 4, 1, 5);
            battleInterfaceBox.ItemsSource = new String[]{
                Lang.Settings.BattleInterfaceType0,
                Lang.Settings.BattleInterfaceType1,
                Lang.Settings.BattleInterfaceType2
            };
            battleInterfaceBox.SetBinding(Selector.SelectedIndexProperty, new Binding(nameof(BattleInterface)) { Mode = BindingMode.TwoWay });
            battleInterfaceBox.Height = 20;
            battleInterfaceBox.FontSize = 10;
            battleInterfaceBox.Margin = rowMargin;

            Row++;

            TextBlock UIColumnsChoiceText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.UIColumnsChoice), Row, 0, 1, 4);
            UIColumnsChoiceText.Foreground = Brushes.White;
            UIColumnsChoiceText.Margin = rowMargin;
            UIColumnsChoiceText.ToolTip = Lang.Settings.UIColumnsChoice_Tooltip;
            ComboBox UIColumnsChoiceBox = AddUiElement(UiComboBoxFactory.Create(), Row, 4, 1, 5);
            UIColumnsChoiceBox.ItemsSource = new String[]{
                Lang.Settings.UIColumnsChoice0, // default 8 - 6
                Lang.Settings.UIColumnsChoice1, // 3 columns
                Lang.Settings.UIColumnsChoice2 // 4 columns
            };
            UIColumnsChoiceBox.SetBinding(Selector.SelectedIndexProperty, new Binding(nameof(UIColumnsChoice)) { Mode = BindingMode.TwoWay });
            UIColumnsChoiceBox.Height = 20;
            UIColumnsChoiceBox.FontSize = 10;
            UIColumnsChoiceBox.Margin = rowMargin;

            CreateCheckbox("SkipIntros", Lang.Settings.SkipIntrosToMainMenu, Lang.Settings.SkipIntrosToMainMenu_Tooltip);
            CreateCheckbox("BattleSwirlFrames", Lang.Settings.SkipBattleSwirl, Lang.Settings.SkipBattleSwirl_Tooltip);
            CreateCheckbox("HideCards", Lang.Settings.HideSteamBubbles, Lang.Settings.HideSteamBubbles_Tooltip);

            Row++;

            TextBlock speedChoiceText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.SpeedChoice), Row, 0, 1, 4);
            speedChoiceText.Foreground = Brushes.White;
            speedChoiceText.Margin = rowMargin;
            speedChoiceText.ToolTip = Lang.Settings.SpeedChoice_Tooltip;
            ComboBox speedChoiceBox = AddUiElement(UiComboBoxFactory.Create(), Row, 4, 1, 5);
            speedChoiceBox.ItemsSource = new String[]{
                Lang.Settings.SpeedChoiceType0,
                Lang.Settings.SpeedChoiceType1,
                Lang.Settings.SpeedChoiceType2,
                //Lang.Settings.SpeedChoiceType3,
                //Lang.Settings.SpeedChoiceType4,
                Lang.Settings.SpeedChoiceType5
            };
            speedChoiceBox.SetBinding(Selector.SelectedIndexProperty, new Binding(nameof(Speed)) { Mode = BindingMode.TwoWay });
            speedChoiceBox.Height = 20;
            speedChoiceBox.FontSize = 10;
            speedChoiceBox.Margin = rowMargin;

            Row++;

            TextBlock tripleTriadText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.TripleTriad), Row, 0, 1, 4);
            tripleTriadText.Foreground = Brushes.White;
            tripleTriadText.Margin = rowMargin;
            tripleTriadText.ToolTip = Lang.Settings.TripleTriad_Tooltip;
            ComboBox tripleTriadBox = AddUiElement(UiComboBoxFactory.Create(), Row, 4, 1, 5);
            tripleTriadBox.ItemsSource = new String[]{
                Lang.Settings.TripleTriadType0,
                Lang.Settings.TripleTriadType1,
                Lang.Settings.TripleTriadType2
            };
            tripleTriadBox.SetBinding(Selector.SelectedIndexProperty, new Binding(nameof(TripleTriad)) { Mode = BindingMode.TwoWay });
            tripleTriadBox.Height = 20;
            tripleTriadBox.FontSize = 10;
            tripleTriadBox.Margin = rowMargin;

            CreateCheckbox("UsePsxFont", Lang.Settings.UsePsxFont, Lang.Settings.UsePsxFont_Tooltip);

            Row++;

            TextBlock fontChoiceText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.FontChoice), Row, 0, 1, 2);
            fontChoiceText.Foreground = Brushes.White;
            fontChoiceText.Margin = rowMargin;
            fontChoiceText.ToolTip = Lang.Settings.FontChoice_Tooltip;
            FontChoiceBox = AddUiElement(UiComboBoxFactory.Create(), Row, 2, 1, 7);
            //_fontChoiceBox.IsEnabled = false;
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

using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using System.Drawing.Text;
using Ini;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using System.Text.RegularExpressions;


// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming
// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable ArrangeStaticMemberQualifier
#pragma warning disable 649
#pragma warning disable 169

namespace Memoria.Launcher
{
    public sealed class MemoriaIniControl : UiGrid, INotifyPropertyChanged
    {
        public MemoriaIniControl()
        {
            SetRows(22);
            SetCols(9);
            
            Width = 260;
            VerticalAlignment = VerticalAlignment.Bottom;
            HorizontalAlignment = HorizontalAlignment.Right;
            Margin = new Thickness(0);

            DataContext = this;

            Thickness rowMargin = new(8, 2, 3, 2);

            /*UiTextBlock optionsText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.IniOptions), 0, 0, 2, 8);
            optionsText.Padding = new Thickness(0, 4, 0, 0);
            optionsText.Foreground = Brushes.White;
            optionsText.FontSize = 14;
            optionsText.FontWeight = FontWeights.Bold;
            optionsText.Margin = rowMargin;*/
            Int32 row = 0;

            if (Directory.Exists("MoguriSoundtrack"))
            {
                UiCheckBox isUsingOrchestralMusic = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.UseOrchestralMusic, null), row, 0, 1, 9);
                isUsingOrchestralMusic.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(OrchestralMusic)) { Mode = BindingMode.TwoWay });
                isUsingOrchestralMusic.Foreground = Brushes.White;
                isUsingOrchestralMusic.Margin = rowMargin;
                isUsingOrchestralMusic.ToolTip = Lang.Settings.UseOrchestralMusic_Tooltip;

                row++;
            }

            if (Directory.Exists("MoguriVideo"))
            {
                UiCheckBox isUsing30fpsVideo = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Use30FpsVideo, null), row, 0, 1, 9);
                isUsing30fpsVideo.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(HighFpsVideo)) { Mode = BindingMode.TwoWay });
                isUsing30fpsVideo.Foreground = Brushes.White;
                isUsing30fpsVideo.Margin = rowMargin;
                isUsing30fpsVideo.ToolTip = Lang.Settings.Use30FpsVideo_Tooltip;

                row++;
            }
            
            UiCheckBox isWidescreenSupport = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Widescreen, null), row, 0, 1, 9);
            isWidescreenSupport.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(WidescreenSupport)) { Mode = BindingMode.TwoWay });
            isWidescreenSupport.Foreground = Brushes.White;
            isWidescreenSupport.Margin = rowMargin;
            isWidescreenSupport.ToolTip = Lang.Settings.Widescreen_Tooltip;

            row++;

            UiCheckBox isAntiAliasing = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.AntiAliasing, null), row, 0, 1, 9);
            isAntiAliasing.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(AntiAliasing)) { Mode = BindingMode.TwoWay });
            isAntiAliasing.Foreground = Brushes.White;
            isAntiAliasing.Margin = rowMargin;
            isAntiAliasing.ToolTip = Lang.Settings.AntiAliasing_Tooltip;

            row++;

            UiTextBlock sharedFpsText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.SharedFPS), row, 0, 1, 9);
            sharedFpsText.Foreground = Brushes.White;
            sharedFpsText.Margin = rowMargin;
            sharedFpsText.ToolTip = Lang.Settings.SharedFPS_Tooltip;

            row++;

            /*UiTextBlock sharedFpsIndex = AddUiElement(UiTextBlockFactory.Create(""), row, 0, 1, 1);
            sharedFpsIndex.SetBinding(TextBlock.TextProperty, new Binding(nameof(SharedFPS)) { Mode = BindingMode.TwoWay });
            sharedFpsIndex.Foreground = Brushes.White;
            sharedFpsIndex.Margin = rowMargin;
            Slider sharedFps = AddUiElement(UiSliderFactory.Create(0), row, 1, 1, 7);
            sharedFps.SetBinding(Slider.ValueProperty, new Binding(nameof(SharedFPS)) { Mode = BindingMode.TwoWay });
            sharedFps.TickFrequency = 5;
            sharedFps.IsSnapToTickEnabled = true;
            sharedFps.Minimum = 15;
            sharedFps.Maximum = 120;
            sharedFps.Margin = new Thickness(0, 0, 3, 0);*/

            UiTextBlock battleFPSText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.BattleFPS), row, 0, 1, 3);
            battleFPSText.Foreground = Brushes.White;
            battleFPSText.Margin = rowMargin;
            battleFPSText.ToolTip = Lang.Settings.BattleFPS_Tooltip;

            UiTextBlock fieldFPSText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.FieldFPS), row, 3, 1, 3);
            fieldFPSText.Foreground = Brushes.White;
            fieldFPSText.Margin = rowMargin;
            fieldFPSText.ToolTip = Lang.Settings.FieldFPS_Tooltip;

            UiTextBlock worldFPSText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.WorldFPS), row, 6, 1, 3);
            worldFPSText.Foreground = Brushes.White;
            worldFPSText.Margin = rowMargin;
            worldFPSText.ToolTip = Lang.Settings.WorldFPS_Tooltip;

            row++;

            DoubleCollection tickMarks = new DoubleCollection();
            tickMarks.Add(20);
            tickMarks.Add(30);
            tickMarks.Add(60);
            tickMarks.Add(120);

            UiTextBlock battleFPSIndex = AddUiElement(UiTextBlockFactory.Create(""), row, 0, 1, 1);
            battleFPSIndex.SetBinding(TextBlock.TextProperty, new Binding(nameof(BattleFPS)) { Mode = BindingMode.TwoWay });
            battleFPSIndex.Foreground = Brushes.White;
            battleFPSIndex.Margin = new(8, 0, 0, 2);
            //battleFPSIndex.TextAlignment = TextAlignment.Right;
            Slider battleFPSSlider = AddUiElement(UiSliderFactory.Create(60), row, 1, 1, 2);
            battleFPSSlider.SetBinding(Slider.ValueProperty, new Binding(nameof(BattleFPS)) { Mode = BindingMode.TwoWay });
            battleFPSSlider.TickFrequency = 10;
            battleFPSSlider.TickPlacement = TickPlacement.BottomRight;
            battleFPSSlider.Ticks = tickMarks;
            battleFPSSlider.Height = 20;
            battleFPSSlider.IsSnapToTickEnabled = true;
            battleFPSSlider.Minimum = 20;
            battleFPSSlider.Maximum = 120;
            battleFPSSlider.Margin = new(3, 0, 3, 0);

            //row++;

            UiTextBlock fieldFPSIndex = AddUiElement(UiTextBlockFactory.Create(""), row, 3, 1, 1);
            fieldFPSIndex.SetBinding(TextBlock.TextProperty, new Binding(nameof(FieldFPS)) { Mode = BindingMode.TwoWay });
            fieldFPSIndex.Foreground = Brushes.White;
            fieldFPSIndex.Margin = new(8, 0, 0, 2);
            //fieldFPSIndex.TextAlignment = TextAlignment.Right;
            Slider fieldFPSSlider = AddUiElement(UiSliderFactory.Create(60), row, 4, 1, 2);
            fieldFPSSlider.SetBinding(Slider.ValueProperty, new Binding(nameof(FieldFPS)) { Mode = BindingMode.TwoWay });
            fieldFPSSlider.TickFrequency = 10;
            fieldFPSSlider.TickPlacement = TickPlacement.BottomRight;
            fieldFPSSlider.Ticks = tickMarks;
            fieldFPSSlider.Height = 20;
            fieldFPSSlider.IsSnapToTickEnabled = true;
            fieldFPSSlider.Minimum = 20;
            fieldFPSSlider.Maximum = 120;
            fieldFPSSlider.Margin = new(3, 0, 3, 0);

            //row++;

            UiTextBlock worldFPSIndex = AddUiElement(UiTextBlockFactory.Create(""), row, 6, 1, 1);
            worldFPSIndex.SetBinding(TextBlock.TextProperty, new Binding(nameof(WorldFPS)) { Mode = BindingMode.TwoWay });
            worldFPSIndex.Foreground = Brushes.White;
            worldFPSIndex.Margin = new(8, 0, 0, 2);
            //worldFPSIndex.TextAlignment = TextAlignment.Right;
            Slider worldFPSSlider = AddUiElement(UiSliderFactory.Create(60), row, 7, 1, 2);
            worldFPSSlider.SetBinding(Slider.ValueProperty, new Binding(nameof(WorldFPS)) { Mode = BindingMode.TwoWay });
            worldFPSSlider.TickFrequency = 10;
            worldFPSSlider.TickPlacement = TickPlacement.BottomRight;
            worldFPSSlider.Ticks = tickMarks;
            worldFPSSlider.Height = 20;
            worldFPSSlider.IsSnapToTickEnabled = true;
            worldFPSSlider.Minimum = 20;
            worldFPSSlider.Maximum = 120;
            worldFPSSlider.Margin = new(3, 0, 3, 0);

            row++;

            UiTextBlock CameraStabilizerText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.CameraStabilizer), row, 0, 1, 8);
            CameraStabilizerText.Foreground = Brushes.White;
            CameraStabilizerText.Margin = rowMargin;
            CameraStabilizerText.ToolTip = Lang.Settings.CameraStabilizer_Tooltip;

            row++;

            UiTextBlock CameraStabilizerIndex = AddUiElement(UiTextBlockFactory.Create(""), row, 0, 1, 1);
            CameraStabilizerIndex.SetBinding(TextBlock.TextProperty, new Binding(nameof(CameraStabilizer)) { Mode = BindingMode.TwoWay });
            CameraStabilizerIndex.Foreground = Brushes.White;
            CameraStabilizerIndex.Margin = rowMargin;
            Slider CameraStabilizerSlider = AddUiElement(UiSliderFactory.Create(0), row, 1, 1, 8);
            CameraStabilizerSlider.SetBinding(Slider.ValueProperty, new Binding(nameof(CameraStabilizer)) { Mode = BindingMode.TwoWay });
            CameraStabilizerSlider.TickFrequency = 1;
            CameraStabilizerSlider.TickPlacement = TickPlacement.BottomRight;
            CameraStabilizerSlider.Height = 20;
            CameraStabilizerSlider.IsSnapToTickEnabled = true;
            CameraStabilizerSlider.Minimum = 0;
            CameraStabilizerSlider.Maximum = 99;
            CameraStabilizerSlider.Margin = new(3, 3, 3, 3);

            row++;

            UiTextBlock battleInterfaceText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.BattleInterface), row, 0, 1, 4);
            battleInterfaceText.Foreground = Brushes.White;
            battleInterfaceText.Margin = rowMargin;
            battleInterfaceText.ToolTip = Lang.Settings.BattleInterface_Tooltip;
            UiComboBox battleInterfaceBox = AddUiElement(UiComboBoxFactory.Create(), row, 4, 1, 5);
            battleInterfaceBox.ItemsSource = new String[]{
                Lang.Settings.BattleInterfaceType0,
                Lang.Settings.BattleInterfaceType1,
                Lang.Settings.BattleInterfaceType2
            };
            battleInterfaceBox.SetBinding(Selector.SelectedIndexProperty, new Binding(nameof(BattleInterface)) { Mode = BindingMode.TwoWay });
            battleInterfaceBox.Height = 20;
            battleInterfaceBox.FontSize = 10;
            battleInterfaceBox.Margin = rowMargin;

            row++;

            UiTextBlock UIColumnsChoiceText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.UIColumnsChoice), row, 0, 1, 4);
            UIColumnsChoiceText.Foreground = Brushes.White;
            UIColumnsChoiceText.Margin = rowMargin;
            UIColumnsChoiceText.ToolTip = Lang.Settings.UIColumnsChoice_Tooltip;
            UiComboBox UIColumnsChoiceBox = AddUiElement(UiComboBoxFactory.Create(), row, 4, 1, 5);
            UIColumnsChoiceBox.ItemsSource = new String[]{
                Lang.Settings.UIColumnsChoice0, // default 8 - 6
                Lang.Settings.UIColumnsChoice1, // 3 columns
                Lang.Settings.UIColumnsChoice2 // 4 columns
            };
            UIColumnsChoiceBox.SetBinding(Selector.SelectedIndexProperty, new Binding(nameof(UIColumnsChoice)) { Mode = BindingMode.TwoWay });
            UIColumnsChoiceBox.Height = 20;
            UIColumnsChoiceBox.FontSize = 10;
            UIColumnsChoiceBox.Margin = rowMargin;

            row++;

            UiCheckBox isSkipIntros = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.SkipIntrosToMainMenu, null), row, 0, 1, 9);
            isSkipIntros.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(SkipIntros)) { Mode = BindingMode.TwoWay });
            isSkipIntros.Foreground = Brushes.White;
            isSkipIntros.Margin = rowMargin;
            isSkipIntros.ToolTip = Lang.Settings.SkipIntrosToMainMenu_Tooltip;

            row++;

            UiCheckBox isSkipBattleSwirl = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.SkipBattleSwirl, null), row, 0, 1, 9);
            isSkipBattleSwirl.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(BattleSwirlFrames)) { Mode = BindingMode.TwoWay });
            isSkipBattleSwirl.Foreground = Brushes.White;
            isSkipBattleSwirl.Margin = rowMargin;
            isSkipBattleSwirl.ToolTip = Lang.Settings.SkipBattleSwirl_Tooltip;

            row++;

            /*
            UiTextBlock battleSwirlFramesText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.SkipBattleLoading), row, 0, 1, 8);
            battleSwirlFramesText.Foreground = Brushes.White;
            battleSwirlFramesText.Margin = rowMargin;
            row++;
            UiTextBlock battleSwirlFramesTextindex = AddUiElement(UiTextBlockFactory.Create(""), row, 0, 1, 2);
            battleSwirlFramesTextindex.SetBinding(TextBlock.TextProperty, new Binding(nameof(BattleSwirlFrames)) { Mode = BindingMode.TwoWay });
            battleSwirlFramesTextindex.Foreground = Brushes.White;
            battleSwirlFramesTextindex.Margin = rowMargin;
            Slider battleSwirlFrames = AddUiElement(UiSliderFactory.Create(0), row++, 1, 1, 6);
            battleSwirlFrames.SetBinding(Slider.ValueProperty, new Binding(nameof(BattleSwirlFrames)) { Mode = BindingMode.TwoWay });
            battleSwirlFrames.TickFrequency = 5;
            */

            /*
            //soundVolumeSlider.TickPlacement = TickPlacement.BottomRight;
            battleSwirlFrames.IsSnapToTickEnabled = true;
            battleSwirlFrames.Minimum = 0;
            battleSwirlFrames.Maximum = 120;
            battleSwirlFrames.Margin = rowMargin;
            */

            UiCheckBox isHideCards = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.HideSteamBubbles, null), row, 0, 1, 9);
            isHideCards.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(HideCards)) { Mode = BindingMode.TwoWay });
            isHideCards.Foreground = Brushes.White;
            isHideCards.Margin = rowMargin;
            isHideCards.ToolTip = Lang.Settings.HideSteamBubbles_Tooltip;

            row++;

            UiTextBlock speedChoiceText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.SpeedChoice), row, 0, 1, 4);
            speedChoiceText.Foreground = Brushes.White;
            speedChoiceText.Margin = rowMargin;
            speedChoiceText.ToolTip = Lang.Settings.SpeedChoice_Tooltip;
            UiComboBox speedChoiceBox = AddUiElement(UiComboBoxFactory.Create(), row, 4, 1, 5);
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

            row++;

            UiTextBlock tripleTriadText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.TripleTriad), row, 0, 1, 4);
            tripleTriadText.Foreground = Brushes.White;
            tripleTriadText.Margin = rowMargin;
            tripleTriadText.ToolTip = Lang.Settings.TripleTriad_Tooltip;
            UiComboBox tripleTriadBox = AddUiElement(UiComboBoxFactory.Create(), row, 4, 1, 5);
            tripleTriadBox.ItemsSource = new String[]{
                Lang.Settings.TripleTriadType0,
                Lang.Settings.TripleTriadType1,
                Lang.Settings.TripleTriadType2
            };
            tripleTriadBox.SetBinding(Selector.SelectedIndexProperty, new Binding(nameof(TripleTriad)) { Mode = BindingMode.TwoWay });
            tripleTriadBox.Height = 20;
            tripleTriadBox.FontSize = 10;
            tripleTriadBox.Margin = rowMargin;

            row++;

            /*UiTextBlock volumeText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.Volume), row++, 0, 1, 8);
            volumeText.Foreground = Brushes.White;
            volumeText.Margin = rowMargin;
            volumeText.TextAlignment = TextAlignment.Center;*/
            /*
            UiTextBlock soundVolumeText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.SoundVolume), row, 0, 1, 3);
            soundVolumeText.Foreground = Brushes.White;
            soundVolumeText.Margin = rowMargin;
            soundVolumeText.ToolTip = Lang.Settings.SoundVolume_Tooltip;
            UiTextBlock soundVolumeTextIndex = AddUiElement(UiTextBlockFactory.Create(""), row, 3, 1, 1);
            soundVolumeTextIndex.SetBinding(TextBlock.TextProperty, new Binding(nameof(SoundVolume)) { Mode = BindingMode.TwoWay });
            soundVolumeTextIndex.Foreground = Brushes.White;
            soundVolumeTextIndex.Margin = rowMargin;
            soundVolumeTextIndex.TextAlignment = TextAlignment.Right;
            Slider soundVolumeSlider = AddUiElement(UiSliderFactory.Create(0), row, 4, 1, 5);
            soundVolumeSlider.SetBinding(Slider.ValueProperty, new Binding(nameof(SoundVolume)) { Mode = BindingMode.TwoWay });
            soundVolumeSlider.TickFrequency = 5;
            soundVolumeSlider.IsSnapToTickEnabled = true;
            soundVolumeSlider.TickPlacement = TickPlacement.BottomRight;
            soundVolumeSlider.Height = 20;
            soundVolumeSlider.Minimum = 0;
            soundVolumeSlider.Maximum = 100;
            soundVolumeSlider.Margin = new(3, 3, 3, 3);

            row++;

            UiTextBlock musicVolumeText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.MusicVolume), row, 0, 1, 3);
            musicVolumeText.Foreground = Brushes.White;
            musicVolumeText.Margin = rowMargin;
            musicVolumeText.ToolTip = Lang.Settings.MusicVolume_Tooltip;
            UiTextBlock musicVolumeTextIndex = AddUiElement(UiTextBlockFactory.Create(""), row, 3, 1, 1);
            musicVolumeTextIndex.SetBinding(TextBlock.TextProperty, new Binding(nameof(MusicVolume)) { Mode = BindingMode.TwoWay });
            musicVolumeTextIndex.Foreground = Brushes.White;
            musicVolumeTextIndex.Margin = rowMargin;
            musicVolumeTextIndex.TextAlignment = TextAlignment.Right;
            Slider musicVolumeSlider = AddUiElement(UiSliderFactory.Create(0), row, 4, 1, 4);
            musicVolumeSlider.SetBinding(Slider.ValueProperty, new Binding(nameof(MusicVolume)) { Mode = BindingMode.TwoWay });
            musicVolumeSlider.TickFrequency = 5;
            musicVolumeSlider.IsSnapToTickEnabled = true;
            musicVolumeSlider.TickPlacement = TickPlacement.BottomRight;
            musicVolumeSlider.Height = 20;
            musicVolumeSlider.Minimum = 0;
            musicVolumeSlider.Maximum = 100;
            musicVolumeSlider.Margin = new(3, 3, 3, 3);

            row++;

            UiTextBlock movieVolumeText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.MovieVolume), row, 0, 1, 3);
            movieVolumeText.Foreground = Brushes.White;
            movieVolumeText.Margin = rowMargin;
            movieVolumeText.ToolTip = Lang.Settings.MovieVolume_Tooltip;
            UiTextBlock movieVolumeTextIndex = AddUiElement(UiTextBlockFactory.Create(""), row, 3, 1, 1);
            movieVolumeTextIndex.SetBinding(TextBlock.TextProperty, new Binding(nameof(MovieVolume)) { Mode = BindingMode.TwoWay });
            movieVolumeTextIndex.Foreground = Brushes.White;
            movieVolumeTextIndex.Margin = rowMargin;
            movieVolumeTextIndex.TextAlignment = TextAlignment.Right;
            Slider movieVolumeSlider = AddUiElement(UiSliderFactory.Create(0), row, 4, 1, 4);
            movieVolumeSlider.SetBinding(Slider.ValueProperty, new Binding(nameof(MovieVolume)) { Mode = BindingMode.TwoWay });
            movieVolumeSlider.TickFrequency = 5;
            movieVolumeSlider.IsSnapToTickEnabled = true;
            movieVolumeSlider.TickPlacement = TickPlacement.BottomRight;
            movieVolumeSlider.Height = 20;
            movieVolumeSlider.Minimum = 0;
            movieVolumeSlider.Maximum = 100;
            movieVolumeSlider.Margin = new(3, 3, 3, 3);

            row++;
            */

            UiCheckBox usePsxFont = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.UsePsxFont, null), row, 0, 1, 9);
            usePsxFont.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(UsePsxFont)) { Mode = BindingMode.TwoWay });
            usePsxFont.Foreground = Brushes.White;
            usePsxFont.Margin = rowMargin;
            usePsxFont.ToolTip = Lang.Settings.UsePsxFont_Tooltip;

            row++;

            UiTextBlock fontChoiceText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.FontChoice), row, 0, 1, 2);
            fontChoiceText.Foreground = Brushes.White;
            fontChoiceText.Margin = rowMargin;
            fontChoiceText.ToolTip = Lang.Settings.FontChoice_Tooltip;
            _fontChoiceBox = AddUiElement(UiComboBoxFactory.Create(), row, 2, 1, 7);
            //_fontChoiceBox.IsEnabled = false;
            _fontChoiceBox.Height = 20;
            _fontChoiceBox.FontSize = 10;
            _fontChoiceBox.Margin = rowMargin;

            FontCollection installedFonts = new InstalledFontCollection();
            String[] fontNames = new String[installedFonts.Families.Length + 2];
            fontNames[0] = _fontDefaultPC;
            fontNames[1] = _fontDefaultPSX;
            for (Int32 fontindex = 0; fontindex < installedFonts.Families.Length; ++fontindex)
                fontNames[fontindex+2] = installedFonts.Families[fontindex].Name;
            _fontChoiceBox.ItemsSource = fontNames;
            _fontChoiceBox.SetBinding(Selector.SelectedItemProperty, new Binding(nameof(FontChoice)) { Mode = BindingMode.TwoWay });

            LoadSettings();
        }

        public Int16 WidescreenSupport
        {
            get { return _iswidescreensupport; }
            set
            {
                if (_iswidescreensupport != value)
                {
                    _iswidescreensupport = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int16 SharedFPS
        {
            get { return _sharedfps; }
            set
            {
                if (_sharedfps != value)
                {
                    _sharedfps = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int16 BattleFPS
        {
            get { return _battlefps; }
            set
            {
                if (_battlefps != value)
                {
                    _battlefps = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 FieldFPS
        {
            get { return _fieldfps; }
            set
            {
                if (_fieldfps != value)
                {
                    _fieldfps = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 WorldFPS
        {
            get { return _worldfps; }
            set
            {
                if (_worldfps != value)
                {
                    _worldfps = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 CameraStabilizer
        {
            get { return _camerastabilizer; }
            set
            {
                if (_camerastabilizer != value)
                {
                    _camerastabilizer = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 BattleInterface
        {
            get { return _battleInterface; }
            set
            {
                if (_battleInterface != value)
                {
                    _battleInterface = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 UIColumnsChoice
        {
            get { return _uicolumnschoice; }
            set
            {
                if (_uicolumnschoice != value)
                {
                    _uicolumnschoice = value;
                    OnPropertyChanged();
                }
            }
        }
        public Rect BattleInterfaceMenu
		{
            get
			{
                if (WidescreenSupport == 0)
                {
                    switch (BattleInterface)
                    {

                        case 0:
                        default:
                            return new Rect(-400, -362, 630, 236);
                        case 1:
                            return new Rect(-400, -382, 530, 220);
                        case 2:
                            return new Rect(-400, -360, 650, 280);
                    }
                }
                else
                {
                    switch (BattleInterface)
                    {
                        case 0:
                        default:
                            return new Rect(-400, -362, 630, 236);
                        case 1:
                            return new Rect(-500, -382, 530, 220);
                        case 2:
                            return new Rect(-500, -360, 650, 280);
                    }
                }
			}
        }
        public Rect BattleInterfaceDetail
        {
            get
            {
                if (WidescreenSupport == 0)
                {
                    switch (BattleInterface)
                    {
                        case 0:
                        default:
                            return new Rect(345, -380, 796, 230);
                        case 1:
                            return new Rect(345, -422, 672, 208);
                        case 2:
                            return new Rect(345, -422, 672, 208);
                    }
                }
                else
                {
                    switch (BattleInterface)
                    {
                        case 0:
                        default:
                            return new Rect(345, -380, 796, 230);
                        case 1:
                            return new Rect(500, -422, 672, 208);
                        case 2:
                            return new Rect(500, -422, 672, 208);
                    }
                }
            }
        }
        public Int16 SkipIntros
        {
            get { return _isskipintros; }
            set
            {   
                if (_isskipintros == 0)
                {
                    _isskipintros = 3;
                    OnPropertyChanged();
                }
                else if (_isskipintros != value)
                {
                    _isskipintros = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 BattleSwirlFrames
        {
            get { return _battleswirlframes; }
            set
            {
                if (_battleswirlframes != value)
                {
                    _battleswirlframes = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 AntiAliasing
        {
            get { return _antialiasing; }
            set
            {
                if (_antialiasing != value)
                {
                    _antialiasing = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 OrchestralMusic
        {
            get { return _isusingorchestralmusic; }
            set
            {
                if (_isusingorchestralmusic != value)
                {
                    _isusingorchestralmusic = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int16 HighFpsVideo
        {
            get { return _isusin30fpsvideo; }
            set
            {
                if (_isusin30fpsvideo != value)
                {
                    _isusin30fpsvideo = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 HideCards
        {
            get { return _ishidecards; }
            set
            {
                if (_ishidecards != value)
                {
                    _ishidecards = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 Speed
        {
            get { return _speed; }
            set
            {
                if (_speed != value)
                {
                    _speed = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 TripleTriad
        {
            get { return _tripleTriad; }
            set
            {
                if (_tripleTriad != value)
                {
                    _tripleTriad = value;
                    OnPropertyChanged();
                }
            }
        }
        /*public Int16 BattleSwirlFrames
        {
            get { return _battleswirlframes; }
            set
            {
                if (_battleswirlframes != value)
                {
                    _battleswirlframes = value;
                    OnPropertyChanged();
                }
            }
        }*/
        public Int16 SoundVolume
        {
            get { return _soundvolume; }
            set
            {
                if (_soundvolume != value)
                {
                    _soundvolume = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 MusicVolume
        {
            get { return _musicvolume; }
            set
            {
                if (_musicvolume != value)
                {
                    _musicvolume = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 MovieVolume
        {
            get { return _movievolume; }
            set
            {
                if (_movievolume != value)
                {
                    _movievolume = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 UsePsxFont
        {
            get { return _usepsxfont; }
            set
            {
                if (_usepsxfont != value)
                {
                    _usepsxfont = value;
                    OnPropertyChanged();
                }
            }
        }
        public String FontChoice
        {
            get { return _fontChoice; }
            set
            {
                if (_fontChoice != value)
                {
                    _fontChoice = value;
                    OnPropertyChanged();
                    OnPropertyChanged("UsePsxFont");
                }
            }
        }
        public Int16 ScaledBattleUI
        {
            get { return _scaledbattleui; }
            set
            {
                if (_scaledbattleui != value)
                {
                    _scaledbattleui = value;
                    OnPropertyChanged();
                }
            }
        }
        public double ScaleUIFactor
        {
            get { return _scaledbattleuiscale; }
            set
            {
                if (_scaledbattleuiscale != value)
                {
                    _scaledbattleuiscale = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsOptionPresentInIni(String category, String option)
        {
            if (File.Exists(_iniPath))
            {
                IniFile iniFile = new IniFile(_iniPath);
                String value = iniFile.ReadValue(category, option);
                if (!String.IsNullOrEmpty(value))
                {
                    return true;
                }
            }
            return false;
        }
        private Int16 _iswidescreensupport, _battleInterface, _uicolumnschoice, _isskipintros, _isusingorchestralmusic, _isusin30fpsvideo, _ishidecards, _speed, _tripleTriad, _battleswirlframes, _antialiasing, _soundvolume, _musicvolume, _movievolume, _usepsxfont, _scaledbattleui, _sharedfps, _battlefps, _fieldfps, _worldfps, _camerastabilizer;
        private double _scaledbattleuiscale;
        private String _fontChoice;
        private UiComboBox _fontChoiceBox;
        private readonly String _fontDefaultPC = "Final Fantasy IX PC";
        private readonly String _fontDefaultPSX = "Final Fantasy IX PSX";
        private readonly String _iniPath = AppDomain.CurrentDomain.BaseDirectory + @"Memoria.ini";

        public void ComeBackToLauncherFromModManager()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                if (!File.Exists(_iniPath))
                {
                    Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("Memoria.ini");
                    StreamReader reader = new StreamReader(input);
                    string text = reader.ReadToEnd();
                    File.WriteAllText(_iniPath, text);
                }

                SanitizeMemoriaIni();

                IniFile iniFile = new IniFile(_iniPath);

                String modstring = iniFile.ReadValue("Mod", "FolderNames");

                if (String.IsNullOrEmpty(modstring))
                {
                    _isusingorchestralmusic = 0;
                    _isusin30fpsvideo = 1;
                    OnPropertyChanged(nameof(OrchestralMusic));
                    OnPropertyChanged(nameof(HighFpsVideo));
                }

                string[] mods = modstring.Split(',');
                char[] charsToTrim = { ' ', '\"' };
                _isusingorchestralmusic = 0;
                _isusin30fpsvideo = 0;
                foreach (string mod in mods)
                {
                    string cleanmodString = mod.Trim(charsToTrim);
                    if (cleanmodString == "MoguriSoundtrack")
                    {
                        _isusingorchestralmusic = 1;
                        OnPropertyChanged(nameof(OrchestralMusic));
                    }
                    if (cleanmodString == "MoguriVideo")
                    {
                        _isusin30fpsvideo = 1;
                        OnPropertyChanged(nameof(HighFpsVideo));
                    }
                        
                }

                String value = iniFile.ReadValue("Graphics", nameof(WidescreenSupport));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 1";
                    OnPropertyChanged(nameof(WidescreenSupport));
                }
                if (!Int16.TryParse(value, out _iswidescreensupport))
                    _iswidescreensupport = 1;

                value = iniFile.ReadValue("Graphics", "FieldFPS");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 30";
                    OnPropertyChanged(nameof(SharedFPS));
                }
                if (!Int16.TryParse(value, out _sharedfps))
                    _sharedfps = 30;

                value = iniFile.ReadValue("Graphics", "BattleFPS");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 30";
                    OnPropertyChanged(nameof(BattleFPS));
                }
                if (!Int16.TryParse(value, out _battlefps))
                    _battlefps = 30;

                value = iniFile.ReadValue("Graphics", "FieldFPS");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 30";
                    OnPropertyChanged(nameof(FieldFPS));
                }
                if (!Int16.TryParse(value, out _fieldfps))
                    _fieldfps = 30;

                value = iniFile.ReadValue("Graphics", "WorldFPS");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 20";
                    OnPropertyChanged(nameof(WorldFPS));
                }
                if (!Int16.TryParse(value, out _worldfps))
                    _worldfps = 20;

                value = iniFile.ReadValue("Graphics", "CameraStabilizer");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 85";
                    OnPropertyChanged(nameof(CameraStabilizer));
                }
                if (!Int16.TryParse(value, out _camerastabilizer))
                    _camerastabilizer = 30;

                String valueMenuPos = iniFile.ReadValue("Interface", "BattleMenuPosX");
                String valuePSXMenu = iniFile.ReadValue("Interface", "PSXBattleMenu");
                Int32 menuPosX = -400;
                Int32 psxMenu = 0;
                if (!String.IsNullOrEmpty(valueMenuPos))
                    if (!Int32.TryParse(valueMenuPos, out menuPosX))
                        menuPosX = -400;
                if (!String.IsNullOrEmpty(valuePSXMenu))
                    if (!Int32.TryParse(valuePSXMenu, out psxMenu))
                        psxMenu = 0;
                if (psxMenu > 0)
                    _battleInterface = 2;
                else if (menuPosX != -400)
                    _battleInterface = 1;
                else
                    _battleInterface = 0;

                value = iniFile.ReadValue("Graphics", nameof(SkipIntros));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(SkipIntros));
                }
                if (!Int16.TryParse(value, out _isskipintros))
                    _isskipintros = 0;

                value = iniFile.ReadValue("Interface", "MenuItemRowCount");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 8";
                    OnPropertyChanged(nameof(UIColumnsChoice));
                }
                String newvalue = ((Int16.Parse(value) / 4) - 2).ToString();
                if (!Int16.TryParse(newvalue, out _uicolumnschoice))
                    _uicolumnschoice = 0;

                value = iniFile.ReadValue("Graphics", nameof(BattleSwirlFrames));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 1";
                    OnPropertyChanged(nameof(BattleSwirlFrames));
                }
                newvalue = (Int16.Parse(value) == 0) ? "1" : "0";
                if (!Int16.TryParse(newvalue, out _battleswirlframes))
                    _battleswirlframes = 0;

                value = iniFile.ReadValue("Graphics", nameof(AntiAliasing));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 8";
                    OnPropertyChanged(nameof(AntiAliasing));
                }
                if (!Int16.TryParse(value, out _antialiasing))
                    _antialiasing = 1;

                value = iniFile.ReadValue("Icons", nameof(HideCards));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(HideCards));
                }
                if (!Int16.TryParse(value, out _ishidecards))
                    _ishidecards = 0;

                value = iniFile.ReadValue("Battle", nameof(Speed));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(Speed));
                }
                if (!Int16.TryParse(value, out _speed))
                    _speed = 0;
                else if (_speed > 3)
                    _speed = 3;

                value = iniFile.ReadValue("TetraMaster", nameof(TripleTriad));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(TripleTriad));
                }
                if (!Int16.TryParse(value, out _tripleTriad))
                    _tripleTriad = 0;

                /*value = iniFile.ReadValue("Graphics", nameof(BattleSwirlFrames));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 115";
                    OnPropertyChanged(nameof(BattleSwirlFrames));
                }
                if (!Int16.TryParse(value, out _battleswirlframes))
                    _battleswirlframes = 115;
                */
                value = iniFile.ReadValue("Audio", nameof(SoundVolume));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 100";
                    OnPropertyChanged(nameof(SoundVolume));
                }
                if (!Int16.TryParse(value, out _soundvolume))
                    _soundvolume = 100;

                value = iniFile.ReadValue("Audio", nameof(MusicVolume));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 100";
                    OnPropertyChanged(nameof(MusicVolume));
                }
                if (!Int16.TryParse(value, out _musicvolume))
                    _musicvolume = 100;

                value = iniFile.ReadValue("Audio", nameof(MovieVolume));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 100";
                    OnPropertyChanged(nameof(MovieVolume));
                }
                if (!Int16.TryParse(value, out _movievolume))
                    _movievolume = 100;

                
                value = iniFile.ReadValue("Font", "Enabled");
                Int16 enabledFont = 0;
                if (String.IsNullOrEmpty(value) || !Int16.TryParse(value, out enabledFont) || enabledFont == 0)
                {
                    _fontChoice = _fontDefaultPC;
                    _usepsxfont = 0;
                }
                else
                {
                    value = iniFile.ReadValue("Font", "Names");
                    if (String.IsNullOrEmpty(value) || value.Length < 2)
                    {
                        _fontChoice = _fontDefaultPC;
                        _usepsxfont = 0;
                    }
                    else
                    {
                        String[] fontList = value.Trim('"').Split(new[] { "\", \"" }, StringSplitOptions.None);
                        _fontChoice = fontList[0];
                        if (_fontChoice.CompareTo("Alexandria") == 0 || _fontChoice.CompareTo("Garnet") == 0)
                        {
                            _fontChoice = _fontDefaultPSX;
                            _usepsxfont = 1;
                        }
                        else
                        {
                            _usepsxfont = 0;
                        }
                    }
                }
                _fontChoiceBox.SelectedItem = _fontChoice;

                Refresh(nameof(WidescreenSupport));
                Refresh(nameof(SharedFPS));
                Refresh(nameof(BattleFPS));
                Refresh(nameof(FieldFPS));
                Refresh(nameof(WorldFPS));
                Refresh(nameof(CameraStabilizer));
                Refresh(nameof(BattleInterface));
                Refresh(nameof(UIColumnsChoice));
                Refresh(nameof(SkipIntros));
                Refresh(nameof(OrchestralMusic));
                Refresh(nameof(HighFpsVideo));
                Refresh(nameof(HideCards));
                Refresh(nameof(Speed));
                Refresh(nameof(TripleTriad));
                Refresh(nameof(BattleSwirlFrames));
                Refresh(nameof(AntiAliasing));
                Refresh(nameof(SoundVolume));
                Refresh(nameof(MusicVolume));
                Refresh(nameof(MovieVolume));
                Refresh(nameof(UsePsxFont));
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private async void Refresh([CallerMemberName] String propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }
        private string UpdateModList()
        {
            //List<string> modList = new List<string>();
            IniFile iniFile = new IniFile(_iniPath);
            String str = iniFile.ReadValue("Mod", "FolderNames");
            if (String.IsNullOrEmpty(str))
                str = "";
            else
            {
                str = str.Replace("\"", "").Replace("MoguriSoundtrack", "").Replace("MoguriVideo", "");
            }
            
            if (Directory.Exists("MoguriSoundtrack") && OrchestralMusic == 1)
                str = str + ",MoguriSoundtrack";
            if (Directory.Exists("MoguriVideo") && HighFpsVideo == 1)
                str = str + ",MoguriVideo";

            String[] modList = Regex.Split(str, ",");
            String modList2 = null;

            for (Int32 i = 0; i < modList.Length; i++)
            {
                modList[i] = modList[i].Trim(' ');
                if (!String.IsNullOrEmpty(modList[i]))
                {
                    if (String.IsNullOrEmpty(modList2))
                        modList2 = "\"" + modList[i] + "\"";
                    else
                    {
                        modList2 = modList2 + ", \"" + modList[i] + "\"";
                    }

                }
            }
            return modList2;
        }

        private async void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                IniFile iniFile = new IniFile(_iniPath);
                switch (propertyName)
                {
                    case nameof(OrchestralMusic):
                    case nameof(HighFpsVideo):
                        iniFile.WriteValue("Mod", "FolderNames ", " " + UpdateModList());
                        break;
                    case nameof(WidescreenSupport):
                        iniFile.WriteValue("Graphics", propertyName + " ", " " + WidescreenSupport.ToString());
                        if (WidescreenSupport == 1)
                            iniFile.WriteValue("Graphics", "Enabled ", " 1");
                        break;
                    case nameof(SharedFPS):
                        iniFile.WriteValue("Graphics", "BattleFPS ", " " + SharedFPS);
                        iniFile.WriteValue("Graphics", "FieldFPS ", " " + SharedFPS);
                        iniFile.WriteValue("Graphics", "WorldFPS ", " " + SharedFPS);
                        iniFile.WriteValue("Graphics", "Enabled ", " 1");
                        break;
                    case nameof(BattleFPS):
                        iniFile.WriteValue("Graphics", "BattleFPS ", " " + BattleFPS);
                        if (BattleFPS != 30)
                            iniFile.WriteValue("Graphics", "Enabled ", " 1");
                        break;
                    case nameof(FieldFPS):
                        iniFile.WriteValue("Graphics", "FieldFPS ", " " + FieldFPS);
                        if (FieldFPS != 30)
                            iniFile.WriteValue("Graphics", "Enabled ", " 1");
                        break;
                    case nameof(WorldFPS):
                        iniFile.WriteValue("Graphics", "WorldFPS ", " " + WorldFPS);
                        if (WorldFPS != 20)
                            iniFile.WriteValue("Graphics", "Enabled ", " 1");
                        break;
                    case nameof(CameraStabilizer):
                        iniFile.WriteValue("Graphics", "CameraStabilizer ", " " + CameraStabilizer);
                        if (CameraStabilizer != 0)
                            iniFile.WriteValue("Graphics", "Enabled ", " 1");
                        break;
                    case nameof(BattleInterface):
                        iniFile.WriteValue("Interface", "BattleMenuPosX ", " " + (Int32)BattleInterfaceMenu.X);
                        iniFile.WriteValue("Interface", "BattleMenuPosY ", " " + (Int32)BattleInterfaceMenu.Y);
                        iniFile.WriteValue("Interface", "BattleMenuWidth ", " " + (Int32)BattleInterfaceMenu.Width);
                        iniFile.WriteValue("Interface", "BattleMenuHeight ", " " + (Int32)BattleInterfaceMenu.Height);
                        iniFile.WriteValue("Interface", "BattleDetailPosX ", " " + (Int32)BattleInterfaceDetail.X);
                        iniFile.WriteValue("Interface", "BattleDetailPosY ", " " + (Int32)BattleInterfaceDetail.Y);
                        iniFile.WriteValue("Interface", "BattleDetailWidth ", " " + (Int32)BattleInterfaceDetail.Width);
                        iniFile.WriteValue("Interface", "BattleDetailHeight ", " " + (Int32)BattleInterfaceDetail.Height);
                        iniFile.WriteValue("Interface", "BattleRowCount ", " " + (BattleInterface == 2 ? 4 : 5));
                        iniFile.WriteValue("Interface", "BattleColumnCount ", " " + (BattleInterface == 2 ? 1 : 1));
                        iniFile.WriteValue("Interface", "PSXBattleMenu ", " " + (BattleInterface == 2 ? 1 : 0));
                        break;
                    case nameof(UIColumnsChoice):
                        iniFile.WriteValue("Interface", "MenuItemRowCount ", " " + (Int32)((UIColumnsChoice + 2) * 4));
                        iniFile.WriteValue("Interface", "MenuAbilityRowCount ", " " + (Int32)((UIColumnsChoice + 2) * 3));
                        if (UIColumnsChoice == 0)
                        {
                            iniFile.WriteValue("Interface", "MenuEquipRowCount ", " 5");
                            iniFile.WriteValue("Interface", "MenuChocographRowCount ", " 5");
                        }
                        else if (UIColumnsChoice == 1)
                        {
                            iniFile.WriteValue("Interface", "MenuEquipRowCount ", " 7");
                            iniFile.WriteValue("Interface", "MenuChocographRowCount ", " 7");
                        }
                        else if (UIColumnsChoice == 2)
                        {
                            iniFile.WriteValue("Interface", "MenuEquipRowCount ", " 8");
                            iniFile.WriteValue("Interface", "MenuChocographRowCount ", " 8");
                        }
                        break;
                    case nameof(SkipIntros):
                        if (SkipIntros == 3)
                        {
                            iniFile.WriteValue("Graphics", propertyName + " ", " 3");
                            iniFile.WriteValue("Graphics", "Enabled ", " 1");
                        }
                        else if (SkipIntros == 0)
                        {
                            iniFile.WriteValue("Graphics", propertyName + " ", " 0");
                        }
                        break;
                    case nameof(HideCards):
                        iniFile.WriteValue("Icons", propertyName + " ", " " + HideCards);
                        iniFile.WriteValue("Icons", "HideBeach ", " " + HideCards); // Merged
                        iniFile.WriteValue("Icons", "HideSteam ", " " + HideCards); // Merged
                        if (HideCards == 1)
                            iniFile.WriteValue("Icons", "Enabled ", " 1");
                        break;
                    case nameof(Speed):
                        iniFile.WriteValue("Battle", propertyName + " ", " " + (Speed < 3 ? Speed : 5));
                        if (Speed != 0)
                            iniFile.WriteValue("Battle", "Enabled ", " 1");
                        break;
                    case nameof(TripleTriad):
                        iniFile.WriteValue("TetraMaster", propertyName + " ", " " + TripleTriad);
                        if (TripleTriad > 0)
                            iniFile.WriteValue("TetraMaster", "Enabled ", " 1");
                        break;
                    case nameof(BattleSwirlFrames):
                        if (BattleSwirlFrames == 1)
                        {
                            iniFile.WriteValue("Graphics", propertyName + " ", " 0");
                            iniFile.WriteValue("Graphics", "Enabled ", " 1");
                        }
                        else if (BattleSwirlFrames == 0)
                        {
                            iniFile.WriteValue("Graphics", propertyName + " ", " 70");
                        }
                        break;
                    case nameof(AntiAliasing):
                        if (AntiAliasing == 1)
                        {
                            iniFile.WriteValue("Graphics", propertyName + " ", " 8");
                            iniFile.WriteValue("Graphics", "Enabled ", " 1");
                        }
                        else if (AntiAliasing == 0)
                        {
                            iniFile.WriteValue("Graphics", propertyName + " ", " 0");
                        }
                        break;
                    /*case nameof(BattleSwirlFrames):
                        iniFile.WriteValue("Graphics", propertyName + " ", " " + BattleSwirlFrames);
                        iniFile.WriteValue("Graphics", "Enabled ", " 1");
                        break;*/
                    case nameof(SoundVolume):
                        iniFile.WriteValue("Audio", propertyName + " ", " " + SoundVolume);
                        //iniFile.WriteValue("Audio", "MovieVolume" + " ", " " + SoundVolume);
                        break;
                    case nameof(MusicVolume):
                        iniFile.WriteValue("Audio", propertyName + " ", " " + MusicVolume);
                        break;
                    case nameof(MovieVolume):
                        iniFile.WriteValue("Audio", propertyName + " ", " " + MovieVolume);
                        break;
                    case nameof(UsePsxFont):
                        if (UsePsxFont == 1)
                        {
                            iniFile.WriteValue("Font", "Enabled ", " 1");
                            iniFile.WriteValue("Font", "Names ", " \"Alexandria\", \"Garnet\"");
                            FontChoice = _fontDefaultPSX;
                        }
                        else if (UsePsxFont == 0)
                        {
                            _usepsxfont = 0;
                        }
                        break;
                    case nameof(FontChoice):
                        if (FontChoice.CompareTo(_fontDefaultPC) == 0)
                        {
                            iniFile.WriteValue("Font", "Enabled ", " 0");
                            _usepsxfont = 0;
                        }
                        else if (FontChoice.CompareTo(_fontDefaultPSX) == 0)
                        {
                            iniFile.WriteValue("Font", "Enabled ", " 1");
                            iniFile.WriteValue("Font", "Names ", " \"Alexandria\", \"Garnet\"");
                            _usepsxfont = 1;
                        }
                        else
                        {
                            iniFile.WriteValue("Font", "Enabled ", " 1");
                            iniFile.WriteValue("Font", "Names ", " \"" + FontChoice + "\", \"Times Bold\"");
                            _usepsxfont = 0;
                        }
                        break;
                    case nameof(ScaledBattleUI):
                        iniFile.WriteValue("Graphics", propertyName + " ", " " + ScaledBattleUI);
                        if (ScaledBattleUI == 1)
                            iniFile.WriteValue("Graphics", "Enabled ", " 1");
                        break;
                    case nameof(ScaleUIFactor):
                        iniFile.WriteValue("Graphics", propertyName + " ", " " + ScaleUIFactor.ToString().Replace(',', '.'));
                        break;
                }
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }
        private async void SanitizeMemoriaIni()
        {
            MakeSureSpacesAroundEqualsigns();

            try
            {
                if (File.Exists(_iniPath))
                {
                    IniFile iniFile = new IniFile(_iniPath);
                    String _checklatestadded = iniFile.ReadValue("Interface", "FadeDuration"); // check if the latest ini parameter is already there
                    if (String.IsNullOrEmpty(_checklatestadded))
                    {
                        MakeIniNotNull("Mod", "FolderNames", "");
                        MakeIniNotNull("Mod", "Priorities", "");
                        MakeIniNotNull("Mod", "UseFileList", "1");

                        MakeIniNotNull("Font", "Enabled", "1");
                        MakeIniNotNull("Font", "Names", "\"Arial\", \"Times Bold\"");
                        MakeIniNotNull("Font", "Size", "24");

                        MakeIniNotNull("Graphics", "Enabled", "0");
                        MakeIniNotNull("Graphics", "BattleFPS", "60");
                        MakeIniNotNull("Graphics", "BattleTPS", "15");
                        MakeIniNotNull("Graphics", "FieldFPS", "60");
                        MakeIniNotNull("Graphics", "FieldTPS", "30");
                        MakeIniNotNull("Graphics", "WorldFPS", "60");
                        MakeIniNotNull("Graphics", "WorldTPS", "20");
                        MakeIniNotNull("Graphics", "MenuFPS", "60");
                        MakeIniNotNull("Graphics", "MenuTPS", "60");
                        MakeIniNotNull("Graphics", "BattleSwirlFrames", "0");
                        MakeIniNotNull("Graphics", "WidescreenSupport", "1");
                        MakeIniNotNull("Graphics", "SkipIntros", "0");
                        MakeIniNotNull("Graphics", "GarnetHair", "0");
                        MakeIniNotNull("Graphics", "TileSize", "32");
                        MakeIniNotNull("Graphics", "AntiAliasing", "8");
                        MakeIniNotNull("Graphics", "CameraStabilizer", "85");

                        MakeIniNotNull("Control", "Enabled", "1");
                        MakeIniNotNull("Control", "DisableMouse", "0");
                        MakeIniNotNull("Control", "DialogProgressButtons", "\"Confirm\"");
                        MakeIniNotNull("Control", "WrapSomeMenus", "1");
                        MakeIniNotNull("Control", "BattleAutoConfirm", "1");
                        MakeIniNotNull("Control", "TurboDialog", "1");
                        MakeIniNotNull("Control", "PSXScrollingMethod", "1");
                        MakeIniNotNull("Control", "PSXMovementMethod", "1");
                        MakeIniNotNull("Control", "AlwaysCaptureGamepad", "1");

                        MakeIniNotNull("Battle", "Enabled", "0");
                        MakeIniNotNull("Battle", "SFXRework", "1");
                        MakeIniNotNull("Battle", "Speed", "0");
                        MakeIniNotNull("Battle", "NoAutoTrance", "0");
                        MakeIniNotNull("Battle", "EncounterInterval", "960");
                        MakeIniNotNull("Battle", "EncounterInitial", "-1440");
                        MakeIniNotNull("Battle", "PersistentDangerValue", "0");
                        MakeIniNotNull("Battle", "AutoPotionOverhealLimit", "-1");
                        MakeIniNotNull("Battle", "GarnetConcentrate", "0");
                        MakeIniNotNull("Battle", "SelectBestTarget", "1");
                        MakeIniNotNull("Battle", "BreakDamageLimit", "0");
                        MakeIniNotNull("Battle", "ViviAutoAttack", "0");
                        MakeIniNotNull("Battle", "CountersBetterTarget", "0");
                        MakeIniNotNull("Battle", "LockEquippedAbilities", "0");
                        MakeIniNotNull("Battle", "FloatEvadeBonus", "0");
                        MakeIniNotNull("Battle", "AccessMenus", "0");
                        MakeIniNotNull("Battle", "CustomBattleFlagsMeaning", "0");

                        MakeIniNotNull("Icons", "Enabled", "1");
                        MakeIniNotNull("Icons", "HideCursor", "0");
                        MakeIniNotNull("Icons", "HideCards", "0");
                        MakeIniNotNull("Icons", "HideExclamation", "0");
                        MakeIniNotNull("Icons", "HideQuestion", "0");
                        MakeIniNotNull("Icons", "HideBeach", "0");
                        MakeIniNotNull("Icons", "HideSteam", "0");

                        MakeIniNotNull("Cheats", "Enabled", "1");
                        MakeIniNotNull("Cheats", "Rotation", "1");
                        MakeIniNotNull("Cheats", "Perspective", "1");
                        MakeIniNotNull("Cheats", "SpeedMode", "1");
                        MakeIniNotNull("Cheats", "SpeedFactor", "3");
                        MakeIniNotNull("Cheats", "SpeedTimer", "0");
                        MakeIniNotNull("Cheats", "BattleAssistance", "0");
                        MakeIniNotNull("Cheats", "Attack9999", "0");
                        MakeIniNotNull("Cheats", "NoRandomEncounter", "1");
                        MakeIniNotNull("Cheats", "MasterSkill", "0");
                        MakeIniNotNull("Cheats", "LvMax", "0");
                        MakeIniNotNull("Cheats", "GilMax", "0");

                        MakeIniNotNull("Hacks", "Enabled", "0");
                        MakeIniNotNull("Hacks", "AllCharactersAvailable", "0");
                        MakeIniNotNull("Hacks", "RopeJumpingIncrement", "1");
                        MakeIniNotNull("Hacks", "FrogCatchingIncrement", "1");
                        MakeIniNotNull("Hacks", "HippaulRacingViviSpeed", "33");
                        MakeIniNotNull("Hacks", "StealingAlwaysWorks", "0");
                        MakeIniNotNull("Hacks", "DisableNameChoice", "0");
                        MakeIniNotNull("Hacks", "ExcaliburIINoTimeLimit", "0");

                        MakeIniNotNull("TetraMaster", "Enabled", "1");
                        MakeIniNotNull("TetraMaster", "TripleTriad", "0");
                        MakeIniNotNull("TetraMaster", "ReduceRandom", "0");
                        MakeIniNotNull("TetraMaster", "MaxCardCount", "100");
                        MakeIniNotNull("TetraMaster", "DiscardAutoButton", "1");
                        MakeIniNotNull("TetraMaster", "DiscardAssaultCards", "0");
                        MakeIniNotNull("TetraMaster", "DiscardFlexibleCards", "1");
                        MakeIniNotNull("TetraMaster", "DiscardMaxAttack", "224");
                        MakeIniNotNull("TetraMaster", "DiscardMaxPDef", "255");
                        MakeIniNotNull("TetraMaster", "DiscardMaxMDef", "255");
                        MakeIniNotNull("TetraMaster", "DiscardMaxSum", "480");
                        MakeIniNotNull("TetraMaster", "DiscardMinDeckSize", "10");
                        MakeIniNotNull("TetraMaster", "DiscardKeepSameType", "1");
                        MakeIniNotNull("TetraMaster", "DiscardKeepSameArrow", "0");
                        MakeIniNotNull("TetraMaster", "DiscardExclusions", "56, 75, 76, 77, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 98, 99, 100");

                        MakeIniNotNull("Interface", "BattleRowCount", "5");
                        MakeIniNotNull("Interface", "BattleColumnCount", "1");
                        MakeIniNotNull("Interface", "BattleMenuPosX", "-400");
                        MakeIniNotNull("Interface", "BattleMenuPosY", "-362");
                        MakeIniNotNull("Interface", "BattleMenuWidth", "630");
                        MakeIniNotNull("Interface", "BattleMenuHeight", "236");
                        MakeIniNotNull("Interface", "BattleDetailPosX", "345");
                        MakeIniNotNull("Interface", "BattleDetailPosY", "-380");
                        MakeIniNotNull("Interface", "BattleDetailWidth", "796");
                        MakeIniNotNull("Interface", "BattleDetailHeight", "230");
                        MakeIniNotNull("Interface", "PSXBattleMenu", "0");
                        MakeIniNotNull("Interface", "ScanDisplay", "1");
                        MakeIniNotNull("Interface", "BattleCommandTitleFormat", "");
                        MakeIniNotNull("Interface", "BattleDamageTextFormat", "");
                        MakeIniNotNull("Interface", "BattleRestoreTextFormat", "");
                        MakeIniNotNull("Interface", "BattleMPDamageTextFormat", "");
                        MakeIniNotNull("Interface", "BattleMPRestoreTextFormat", "");
                        MakeIniNotNull("Interface", "MenuItemRowCount", "8");
                        MakeIniNotNull("Interface", "MenuAbilityRowCount", "6");
                        MakeIniNotNull("Interface", "MenuEquipRowCount", "5");
                        MakeIniNotNull("Interface", "MenuChocographRowCount", "5");
                        MakeIniNotNull("Interface", "FadeDuration", "40");

                        MakeIniNotNull("Fixes", "Enabled", "1");
                        MakeIniNotNull("Fixes", "KeepRestTimeInBattle", "1");
                        
                        MakeIniNotNull("SaveFile", "DisableAutoSave", "0");
                        MakeIniNotNull("SaveFile", "AutoSaveOnlyAtMoogle", "0");
                        MakeIniNotNull("SaveFile", "SaveOnCloud", "0");

                        MakeIniNotNull("Speedrun", "Enabled", "0");
                        MakeIniNotNull("Speedrun", "SplitSettingsPath", "");
                        MakeIniNotNull("Speedrun", "LogGameTimePath", "");

                        MakeIniNotNull("Debug", "Enabled", "0");
                        MakeIniNotNull("Debug", "SigningEventObjects", "0");
                        MakeIniNotNull("Debug", "StartModelViewer", "0");
                        MakeIniNotNull("Debug", "StartFieldCreator", "0");
                        MakeIniNotNull("Debug", "RenderWalkmeshes", "0");

                        MakeSureSpacesAroundEqualsigns();
                    }
                }
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }

        }

        private async void MakeSureSpacesAroundEqualsigns()
        {
            try
            {
                if (File.Exists(_iniPath))
                {
                    string wholeFile = File.ReadAllText(_iniPath);
                    wholeFile = wholeFile.Replace("=", " = ");
                    wholeFile = wholeFile.Replace("  ", " ");
                    wholeFile = wholeFile.Replace("  ", " ");
                    File.WriteAllText(_iniPath, wholeFile);
                }
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }

        private async void MakeIniNotNull(String Category, String Setting, String Defaultvalue)
        {
            IniFile iniFile = new IniFile(_iniPath);
            String value = iniFile.ReadValue(Category, Setting);
            if (String.IsNullOrEmpty(value))
            {
                iniFile.WriteValue(Category, Setting + " ", " " + Defaultvalue);
            }
        }
    }
}

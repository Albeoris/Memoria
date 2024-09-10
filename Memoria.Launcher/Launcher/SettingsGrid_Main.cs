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
    public sealed class SettingsGrid_Main : UiGrid, INotifyPropertyChanged
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

            Int32 row = 0;

            CheckBox isWidescreenSupport = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Widescreen, null), row, 0, 1, 9);
            isWidescreenSupport.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(WidescreenSupport)) { Mode = BindingMode.TwoWay });
            isWidescreenSupport.Foreground = Brushes.White;
            isWidescreenSupport.Margin = rowMargin;
            isWidescreenSupport.ToolTip = Lang.Settings.Widescreen_Tooltip;

            row++;

            CheckBox isAntiAliasing = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.AntiAliasing, null), row, 0, 1, 9);
            isAntiAliasing.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(AntiAliasing)) { Mode = BindingMode.TwoWay });
            isAntiAliasing.Foreground = Brushes.White;
            isAntiAliasing.Margin = rowMargin;
            isAntiAliasing.ToolTip = Lang.Settings.AntiAliasing_Tooltip;

            row++;

            TextBlock FPSDropboxText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.FPSDropboxChoice), row, 0, 1, 4);
            FPSDropboxText.Foreground = Brushes.White;
            FPSDropboxText.Margin = rowMargin;
            FPSDropboxText.ToolTip = Lang.Settings.SharedFPS_Tooltip;
            ComboBox FPSDropbox = AddUiElement(UiComboBoxFactory.Create(), row, 4, 1, 5);
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

            row++;

            TextBlock CameraStabilizerText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.CameraStabilizer), row, 0, 1, 8);
            CameraStabilizerText.Foreground = Brushes.White;
            CameraStabilizerText.Margin = rowMargin;
            CameraStabilizerText.ToolTip = Lang.Settings.CameraStabilizer_Tooltip;

            row++;

            TextBlock CameraStabilizerIndex = AddUiElement(UiTextBlockFactory.Create(""), row, 0, 1, 1);
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

            TextBlock battleInterfaceText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.BattleInterface), row, 0, 1, 4);
            battleInterfaceText.Foreground = Brushes.White;
            battleInterfaceText.Margin = rowMargin;
            battleInterfaceText.ToolTip = Lang.Settings.BattleInterface_Tooltip;
            ComboBox battleInterfaceBox = AddUiElement(UiComboBoxFactory.Create(), row, 4, 1, 5);
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

            TextBlock UIColumnsChoiceText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.UIColumnsChoice), row, 0, 1, 4);
            UIColumnsChoiceText.Foreground = Brushes.White;
            UIColumnsChoiceText.Margin = rowMargin;
            UIColumnsChoiceText.ToolTip = Lang.Settings.UIColumnsChoice_Tooltip;
            ComboBox UIColumnsChoiceBox = AddUiElement(UiComboBoxFactory.Create(), row, 4, 1, 5);
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

            CheckBox isSkipIntros = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.SkipIntrosToMainMenu, null), row, 0, 1, 9);
            isSkipIntros.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(SkipIntros)) { Mode = BindingMode.TwoWay });
            isSkipIntros.Foreground = Brushes.White;
            isSkipIntros.Margin = rowMargin;
            isSkipIntros.ToolTip = Lang.Settings.SkipIntrosToMainMenu_Tooltip;

            row++;

            CheckBox isSkipBattleSwirl = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.SkipBattleSwirl, null), row, 0, 1, 9);
            isSkipBattleSwirl.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(BattleSwirlFrames)) { Mode = BindingMode.TwoWay });
            isSkipBattleSwirl.Foreground = Brushes.White;
            isSkipBattleSwirl.Margin = rowMargin;
            isSkipBattleSwirl.ToolTip = Lang.Settings.SkipBattleSwirl_Tooltip;

            row++;

            CheckBox isHideCards = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.HideSteamBubbles, null), row, 0, 1, 9);
            isHideCards.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(HideCards)) { Mode = BindingMode.TwoWay });
            isHideCards.Foreground = Brushes.White;
            isHideCards.Margin = rowMargin;
            isHideCards.ToolTip = Lang.Settings.HideSteamBubbles_Tooltip;

            row++;

            TextBlock speedChoiceText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.SpeedChoice), row, 0, 1, 4);
            speedChoiceText.Foreground = Brushes.White;
            speedChoiceText.Margin = rowMargin;
            speedChoiceText.ToolTip = Lang.Settings.SpeedChoice_Tooltip;
            ComboBox speedChoiceBox = AddUiElement(UiComboBoxFactory.Create(), row, 4, 1, 5);
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

            TextBlock tripleTriadText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.TripleTriad), row, 0, 1, 4);
            tripleTriadText.Foreground = Brushes.White;
            tripleTriadText.Margin = rowMargin;
            tripleTriadText.ToolTip = Lang.Settings.TripleTriad_Tooltip;
            ComboBox tripleTriadBox = AddUiElement(UiComboBoxFactory.Create(), row, 4, 1, 5);
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

            CheckBox usePsxFont = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.UsePsxFont, null), row, 0, 1, 9);
            usePsxFont.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(UsePsxFont)) { Mode = BindingMode.TwoWay });
            usePsxFont.Foreground = Brushes.White;
            usePsxFont.Margin = rowMargin;
            usePsxFont.ToolTip = Lang.Settings.UsePsxFont_Tooltip;

            row++;

            TextBlock fontChoiceText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.FontChoice), row, 0, 1, 2);
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
                fontNames[fontindex + 2] = installedFonts.Families[fontindex].Name;
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
        public Int16 FPSDropboxChoice
        {
            get { return _fpsdropboxchoice; }
            set
            {
                if (_fpsdropboxchoice != value)
                {
                    _fpsdropboxchoice = value;
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
                    return BattleInterface switch
                    {
                        1 => new Rect(-400, -382, 530, 220),
                        2 => new Rect(-400, -360, 650, 280),
                        _ => new Rect(-400, -362, 630, 236),
                    };
                }
                else
                {
                    return BattleInterface switch
                    {
                        1 => new Rect(-500, -382, 530, 220),
                        2 => new Rect(-500, -360, 650, 280),
                        _ => new Rect(-400, -362, 630, 236),
                    };
                }
            }
        }
        public Rect BattleInterfaceDetail
        {
            get
            {
                if (WidescreenSupport == 0)
                {
                    return BattleInterface switch
                    {
                        1 => new Rect(345, -422, 672, 208),
                        2 => new Rect(345, -422, 672, 208),
                        _ => new Rect(345, -380, 796, 230),
                    };
                }
                else
                {
                    return BattleInterface switch
                    {
                        1 => new Rect(500, -422, 672, 208),
                        2 => new Rect(500, -422, 672, 208),
                        _ => new Rect(345, -380, 796, 230),
                    };
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
        private Int16 _iswidescreensupport, _battleInterface, _uicolumnschoice, _fpsdropboxchoice, _isskipintros, _ishidecards, _speed, _tripleTriad, _battleswirlframes, _antialiasing, _soundvolume, _musicvolume, _movievolume, _usepsxfont, _scaledbattleui, _sharedfps, _battlefps, _fieldfps, _worldfps, _camerastabilizer;
        private double _scaledbattleuiscale;
        private String _fontChoice;
        private ComboBox _fontChoiceBox;
        private readonly String _fontDefaultPC = "Final Fantasy IX PC";
        private readonly String _fontDefaultPSX = "Final Fantasy IX PSX";
        private readonly String _iniPath = AppDomain.CurrentDomain.BaseDirectory + @"Memoria.ini";

        public void ComeBackToLauncherReloadSettings()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                

                IniFile.SanitizeMemoriaIni();

                IniFile iniFile = new(_iniPath);

                String value = iniFile.ReadValue("Graphics", nameof(WidescreenSupport));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 1";
                    OnPropertyChanged(nameof(WidescreenSupport));
                }
                if (!Int16.TryParse(value, out _iswidescreensupport))
                    _iswidescreensupport = 1;


                value = iniFile.ReadValue("Graphics", "FieldFPS");
                Boolean value1isInt = Int16.TryParse(value, out Int16 value1);
                value = iniFile.ReadValue("Graphics", "BattleFPS");
                Boolean value2isInt = Int16.TryParse(value, out Int16 value2);
                value = iniFile.ReadValue("Graphics", "WorldFPS");
                Boolean value3isInt = Int16.TryParse(value, out Int16 value3);
                if (value1isInt && value2isInt && value3isInt)
                {
                    if (value1 == 30 && value2 == 15 && value3 == 20)
                        _fpsdropboxchoice = 0;
                    else if (value1 == 30 && value2 == 30 && value3 == 30)
                        _fpsdropboxchoice = 1;
                    else if (value1 == 60 && value2 == 60 && value3 == 60)
                        _fpsdropboxchoice = 2;
                    else if (value1 == 90 && value2 == 90 && value3 == 90)
                        _fpsdropboxchoice = 3;
                    else if (value1 == 120 && value2 == 120 && value3 == 120)
                        _fpsdropboxchoice = 4;
                    else
                        _fpsdropboxchoice = -1;
                }

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
                if (String.IsNullOrEmpty(value) || !Int16.TryParse(value, out Int16 enabledFont) || enabledFont == 0)
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
                Refresh(nameof(FPSDropboxChoice));
                Refresh(nameof(SkipIntros));
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

        private async void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                IniFile iniFile = new(_iniPath);
                switch (propertyName)
                {
                    case nameof(WidescreenSupport):
                        iniFile.WriteValue("Graphics", propertyName + " ", " " + WidescreenSupport.ToString());
                        if (WidescreenSupport == 1)
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
                    case nameof(FPSDropboxChoice):
                        iniFile.WriteValue("Graphics", "Enabled ", " 1");
                        switch (FPSDropboxChoice)
                        {
                            case (0):
                                iniFile.WriteValue("Graphics", "FieldFPS ", " 30");
                                iniFile.WriteValue("Graphics", "BattleFPS ", " 15");
                                iniFile.WriteValue("Graphics", "WorldFPS ", " 20");
                                break;
                            case (1):
                                iniFile.WriteValue("Graphics", "FieldFPS ", " 30");
                                iniFile.WriteValue("Graphics", "BattleFPS ", " 30");
                                iniFile.WriteValue("Graphics", "WorldFPS ", " 30");
                                break;
                            case (2):
                                iniFile.WriteValue("Graphics", "FieldFPS ", " 60");
                                iniFile.WriteValue("Graphics", "BattleFPS ", " 60");
                                iniFile.WriteValue("Graphics", "WorldFPS ", " 60");
                                break;
                            case (3):
                                iniFile.WriteValue("Graphics", "FieldFPS ", " 90");
                                iniFile.WriteValue("Graphics", "BattleFPS ", " 90");
                                iniFile.WriteValue("Graphics", "WorldFPS ", " 90");
                                break;
                            case (4):
                                iniFile.WriteValue("Graphics", "FieldFPS ", " 120");
                                iniFile.WriteValue("Graphics", "BattleFPS ", " 120");
                                iniFile.WriteValue("Graphics", "WorldFPS ", " 120");
                                break;
                            default:
                                break;
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
    }
}

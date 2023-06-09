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
            PsxFontInstalled = true || IsOptionPresentInIni("Graphics", "UseGarnetFont");
            SBUIInstalled = false && IsOptionPresentInIni("Graphics", "ScaledBattleUI");
            Int16 numberOfRows = 12;
            if (PsxFontInstalled)
                numberOfRows += 1;
            if (SBUIInstalled)
                numberOfRows += 2;

            SetRows(numberOfRows);
            SetCols(8);
            
            Width = 200;
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;
            Margin = new Thickness(0);

            DataContext = this;

            Thickness rowMargin = new Thickness(8, 2, 3, 2);

            UiTextBlock optionsText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.IniOptions), 0, 0, 2, 8);
            optionsText.Padding = new Thickness(0, 4, 0, 0);
            optionsText.Foreground = Brushes.White;
            optionsText.FontSize = 14;
            optionsText.FontWeight = FontWeights.Bold;
            optionsText.Margin = rowMargin;
            Int32 row = 2;

            UiCheckBox isWidescreenSupport = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Widescreen, null), row++, 0, 1, 8);
            isWidescreenSupport.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(WidescreenSupport)) { Mode = BindingMode.TwoWay });
            isWidescreenSupport.Foreground = Brushes.White;
            isWidescreenSupport.Margin = rowMargin;

            UiTextBlock battleInterfaceText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.BattleInterface), row, 0, 1, 3);
            battleInterfaceText.ToolTip = Lang.Settings.BattleInterfaceTooltip;
            battleInterfaceText.Foreground = Brushes.White;
            battleInterfaceText.Margin = rowMargin;
            UiComboBox battleInterfaceBox = AddUiElement(UiComboBoxFactory.Create(), row++, 3, 1, 5);
            battleInterfaceBox.ItemsSource = new String[]{
                Lang.Settings.BattleInterfaceType0,
                Lang.Settings.BattleInterfaceType1,
                Lang.Settings.BattleInterfaceType2
            };
            battleInterfaceBox.SetBinding(Selector.SelectedIndexProperty, new Binding(nameof(BattleInterface)) { Mode = BindingMode.TwoWay });
            battleInterfaceBox.ToolTip = Lang.Settings.BattleInterfaceTooltip;
            battleInterfaceBox.Height = 22;
            battleInterfaceBox.FontSize = 10;
            battleInterfaceBox.Margin = rowMargin;

            UiCheckBox isSkipIntros = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.SkipIntrosToMainMenu, null), row++, 0, 1, 8);
            isSkipIntros.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(SkipIntros)) { Mode = BindingMode.TwoWay });
            isSkipIntros.Foreground = Brushes.White;
            isSkipIntros.Margin = rowMargin;

            /*UiTextBlock battleSwirlFramesText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.SkipBattleLoading), row++, 0, 1, 8);
            battleSwirlFramesText.Foreground = Brushes.White;
            battleSwirlFramesText.Margin = rowMargin;
            UiTextBlock battleSwirlFramesTextindex = AddUiElement(UiTextBlockFactory.Create(""), row, 0, 1, 2);
            battleSwirlFramesTextindex.SetBinding(TextBlock.TextProperty, new Binding(nameof(BattleSwirlFrames)) { Mode = BindingMode.TwoWay });
            battleSwirlFramesTextindex.Foreground = Brushes.White;
            battleSwirlFramesTextindex.Margin = rowMargin;
            Slider battleSwirlFrames = AddUiElement(UiSliderFactory.Create(0), row++, 1, 1, 6);
            battleSwirlFrames.SetBinding(Slider.ValueProperty, new Binding(nameof(BattleSwirlFrames)) { Mode = BindingMode.TwoWay });
            battleSwirlFrames.TickFrequency = 5;
            //soundVolumeSlider.TickPlacement = TickPlacement.BottomRight;
            battleSwirlFrames.IsSnapToTickEnabled = true;
            battleSwirlFrames.Minimum = 0;
            battleSwirlFrames.Maximum = 120;
            battleSwirlFrames.Margin = rowMargin;
            //soundVolumeSlider.Height = 4;*/

            UiCheckBox isHideCards = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.HideSteamBubbles, null), row++, 0, 1, 8);
            isHideCards.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(HideCards)) { Mode = BindingMode.TwoWay });
            isHideCards.ToolTip = Lang.Settings.HideSteamBubblesTooltip;
            isHideCards.Foreground = Brushes.White;
            isHideCards.Margin = rowMargin;

            UiTextBlock speedChoiceText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.SpeedChoice), row, 0, 1, 3);
            speedChoiceText.ToolTip = Lang.Settings.SpeedChoiceTooltip;
            speedChoiceText.Foreground = Brushes.White;
            speedChoiceText.Margin = rowMargin;
            UiComboBox speedChoiceBox = AddUiElement(UiComboBoxFactory.Create(), row++, 3, 1, 5);
            speedChoiceBox.ItemsSource = new String[]{
                Lang.Settings.SpeedChoiceType0,
                Lang.Settings.SpeedChoiceType1,
                Lang.Settings.SpeedChoiceType2,
                //Lang.Settings.SpeedChoiceType3,
                //Lang.Settings.SpeedChoiceType4,
                Lang.Settings.SpeedChoiceType5
            };
            speedChoiceBox.SetBinding(Selector.SelectedIndexProperty, new Binding(nameof(Speed)) { Mode = BindingMode.TwoWay });
            speedChoiceBox.ToolTip = Lang.Settings.SpeedChoiceTooltip;
            speedChoiceBox.Height = 22;
            speedChoiceBox.FontSize = 10;
            speedChoiceBox.Margin = rowMargin;

            UiTextBlock tripleTriadText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.TripleTriad), row, 0, 1, 3);
            tripleTriadText.Foreground = Brushes.White;
            tripleTriadText.Margin = rowMargin;
            UiComboBox tripleTriadBox = AddUiElement(UiComboBoxFactory.Create(), row++, 3, 1, 5);
            tripleTriadBox.ItemsSource = new String[]{
                Lang.Settings.TripleTriadType0,
                Lang.Settings.TripleTriadType1,
                Lang.Settings.TripleTriadType2
            };
            tripleTriadBox.SetBinding(Selector.SelectedIndexProperty, new Binding(nameof(TripleTriad)) { Mode = BindingMode.TwoWay });
            tripleTriadBox.Height = 22;
            tripleTriadBox.FontSize = 10;
            tripleTriadBox.Margin = rowMargin;

            UiTextBlock soundVolumeText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.SoundVolume), row++, 0, 1, 8);
            soundVolumeText.Foreground = Brushes.White;
            soundVolumeText.Margin = rowMargin;
            UiTextBlock soundVolumeTextIndex = AddUiElement(UiTextBlockFactory.Create(""), row, 0, 1, 2);
            soundVolumeTextIndex.SetBinding(TextBlock.TextProperty, new Binding(nameof(SoundVolume)) { Mode = BindingMode.TwoWay });
            soundVolumeTextIndex.Foreground = Brushes.White;
            soundVolumeTextIndex.Margin = rowMargin;
            //soundVolumeTextIndex.TextAlignment = TextAlignment.Right;
            Slider soundVolumeSlider = AddUiElement(UiSliderFactory.Create(0), row++, 2, 1, 6);
            soundVolumeSlider.SetBinding(Slider.ValueProperty, new Binding(nameof(SoundVolume)) { Mode = BindingMode.TwoWay });
            soundVolumeSlider.TickFrequency = 5;
            soundVolumeSlider.IsSnapToTickEnabled = true;
            soundVolumeSlider.Minimum = 0;
            soundVolumeSlider.Maximum = 100;
            soundVolumeSlider.Margin = rowMargin;

            UiTextBlock musicVolumeText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.MusicVolume), row++, 0, 1, 8);
            musicVolumeText.Foreground = Brushes.White;
            musicVolumeText.Margin = rowMargin;
            UiTextBlock musicVolumeTextIndex = AddUiElement(UiTextBlockFactory.Create(""), row, 0, 1, 2);
            musicVolumeTextIndex.SetBinding(TextBlock.TextProperty, new Binding(nameof(MusicVolume)) { Mode = BindingMode.TwoWay });
            musicVolumeTextIndex.Foreground = Brushes.White;
            musicVolumeTextIndex.Margin = rowMargin;
            Slider musicVolumeSlider = AddUiElement(UiSliderFactory.Create(0), row++, 2, 1, 6);
            musicVolumeSlider.SetBinding(Slider.ValueProperty, new Binding(nameof(MusicVolume)) { Mode = BindingMode.TwoWay });
            musicVolumeSlider.TickFrequency = 5;
            musicVolumeSlider.IsSnapToTickEnabled = true;
            musicVolumeSlider.Minimum = 0;
            musicVolumeSlider.Maximum = 100;
            musicVolumeSlider.Margin = rowMargin;

            if (PsxFontInstalled)
            {
                UiTextBlock fontChoiceText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.FontChoice), row, 0, 1, 2);
                fontChoiceText.Foreground = Brushes.White;
                fontChoiceText.Margin = rowMargin;
                _fontChoiceBox = AddUiElement(UiComboBoxFactory.Create(), row++, 2, 1, 6);
                FontCollection installedFonts = new InstalledFontCollection();
                String[] fontNames = new String[installedFonts.Families.Length + 2];
                fontNames[0] = _fontDefaultPC;
                fontNames[1] = _fontDefaultPSX;
                for (Int32 fontindex = 0; fontindex < installedFonts.Families.Length; ++fontindex)
                    fontNames[fontindex+2] = installedFonts.Families[fontindex].Name;
                _fontChoiceBox.ItemsSource = fontNames;
                _fontChoiceBox.SetBinding(Selector.SelectedItemProperty, new Binding(nameof(FontChoice)) { Mode = BindingMode.TwoWay });
                _fontChoiceBox.Height = 22;
                _fontChoiceBox.FontSize = 10;
                _fontChoiceBox.Margin = rowMargin;
            }

            if (SBUIInstalled)
            {
                UiCheckBox useSBUI = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.SBUIenabled, null), row++, 0, 2, 8);
                useSBUI.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(ScaledBattleUI)) { Mode = BindingMode.TwoWay });
                useSBUI.Foreground = Brushes.White;
                useSBUI.Margin = rowMargin;
                Slider sBUIScale = AddUiElement(UiSliderFactory.Create(0), row, 1, 1, 7);
                sBUIScale.SetBinding(Slider.ValueProperty, new Binding(nameof(ScaleUIFactor)) { Mode = BindingMode.TwoWay });
                sBUIScale.TickFrequency = 0.1;
                //musicVolumeSlider.TickPlacement = TickPlacement.BottomRight;
                sBUIScale.IsSnapToTickEnabled = true;
                sBUIScale.Minimum = 0.1;
                sBUIScale.Maximum = 3.0;
                sBUIScale.Margin = rowMargin;
                UiTextBlock sBUIScaleTextindex = AddUiElement(UiTextBlockFactory.Create(""), row++, 0, 2, 1);
                sBUIScaleTextindex.SetBinding(TextBlock.TextProperty, new Binding(nameof(ScaleUIFactor)) { Mode = BindingMode.TwoWay });
                sBUIScaleTextindex.Foreground = Brushes.White;
                sBUIScaleTextindex.Margin = new Thickness(8, 0, 0, 0);
            }

            LoadSettings();
        }

        public bool PsxFontInstalled = false;
        public bool SBUIInstalled = false;

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
        public Rect BattleInterfaceMenu
		{
            get
			{
                switch (BattleInterface)
				{
                    case 0:
                    default:
                        return new Rect(-400, -362, 630, 236);
                    case 1:
                        return new Rect(-602, -382, 530, 220);
                    case 2:
                        return new Rect(-552, -360, 650, 280);
                }
			}
        }
        public Rect BattleInterfaceDetail
        {
            get
            {
                switch (BattleInterface)
                {
                    case 0:
                    default:
                        return new Rect(345, -380, 796, 230);
                    case 1:
                        return new Rect(558, -422, 672, 208);
                    case 2:
                        return new Rect(558, -422, 672, 208);
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
        public Int16 UseGarnetFont
        {
            get { return _usegarnetfont; }
            set
            {
                if (_usegarnetfont != value)
                {
                    _usegarnetfont = value;
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
        private Int16 _iswidescreensupport, _battleInterface, _isskipintros, _ishidecards, _speed, _tripleTriad, _battleswirlframes, _soundvolume, _musicvolume, _usegarnetfont, _scaledbattleui;
        private double _scaledbattleuiscale;
        private String _fontChoice;
        private UiComboBox _fontChoiceBox;
        private readonly String _fontDefaultPC = "Final Fantasy IX PC";
        private readonly String _fontDefaultPSX = "Final Fantasy IX PSX";
        private readonly String _iniPath = AppDomain.CurrentDomain.BaseDirectory + @"Memoria.ini";

        private void LoadSettings()
        {
            try
            {
                if (!File.Exists(_iniPath))
                {
                    Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("Ini.Memoria.ini");
                    Stream output = File.Create(_iniPath);
                    input.CopyTo(output, 8192);
                }

                IniFile iniFile = new IniFile(_iniPath);

                String value = iniFile.ReadValue("Graphics", nameof(WidescreenSupport));
                if (String.IsNullOrEmpty(value))
                {
                    value = "1";
                    OnPropertyChanged(nameof(WidescreenSupport));
                }
                if (!Int16.TryParse(value, out _iswidescreensupport))
                    _iswidescreensupport = 1;

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
                    value = "0";
                    OnPropertyChanged(nameof(SkipIntros));
                }
                if (!Int16.TryParse(value, out _isskipintros))
                    _isskipintros = 0;

                value = iniFile.ReadValue("Icons", nameof(HideCards));
                if (String.IsNullOrEmpty(value))
                {
                    value = "0";
                    OnPropertyChanged(nameof(HideCards));
                }
                if (!Int16.TryParse(value, out _ishidecards))
                    _ishidecards = 0;

                value = iniFile.ReadValue("Battle", nameof(Speed));
                if (String.IsNullOrEmpty(value))
                {
                    value = "0";
                    OnPropertyChanged(nameof(Speed));
                }
                if (!Int16.TryParse(value, out _speed))
                    _speed = 0;
                else if (_speed > 3)
                    _speed = 3;

                value = iniFile.ReadValue("TetraMaster", nameof(TripleTriad));
                if (String.IsNullOrEmpty(value))
                {
                    value = "0";
                    OnPropertyChanged(nameof(TripleTriad));
                }
                if (!Int16.TryParse(value, out _tripleTriad))
                    _tripleTriad = 0;

                value = iniFile.ReadValue("Graphics", nameof(BattleSwirlFrames));
                if (String.IsNullOrEmpty(value))
                {
                    value = "115";
                    OnPropertyChanged(nameof(BattleSwirlFrames));
                }
                if (!Int16.TryParse(value, out _battleswirlframes))
                    _battleswirlframes = 115;

                value = iniFile.ReadValue("Audio", nameof(SoundVolume));
                if (String.IsNullOrEmpty(value))
                {
                    value = "100";
                    OnPropertyChanged(nameof(SoundVolume));
                }
                if (!Int16.TryParse(value, out _soundvolume))
                    _soundvolume = 100;

                value = iniFile.ReadValue("Audio", nameof(MusicVolume));
                if (String.IsNullOrEmpty(value))
                {
                    value = "100";
                    OnPropertyChanged(nameof(MusicVolume));
                }
                if (!Int16.TryParse(value, out _musicvolume))
                    _musicvolume = 100;

                Refresh(nameof(WidescreenSupport));
                Refresh(nameof(BattleInterface));
                Refresh(nameof(SkipIntros));
                Refresh(nameof(HideCards));
                Refresh(nameof(Speed));
                Refresh(nameof(TripleTriad));
                Refresh(nameof(BattleSwirlFrames));
                Refresh(nameof(SoundVolume));
                Refresh(nameof(MusicVolume));

                if (PsxFontInstalled) {
                    value = iniFile.ReadValue("Font", "Enabled");
                    Int16 enabledFont = 0;
                    if (String.IsNullOrEmpty(value) || !Int16.TryParse(value, out enabledFont) || enabledFont == 0)
                    {
                        _fontChoice = _fontDefaultPC;
                    }
                    else
                    {
                        value = iniFile.ReadValue("Font", "Names");
                        if (String.IsNullOrEmpty(value) || value.Length < 2)
                        {
                            _fontChoice = _fontDefaultPC;
                        }
                        else
                        {
                            String[] fontList = value.Trim('"').Split(new[] { "\", \"" }, StringSplitOptions.None);
                            _fontChoice = fontList[0];
                            if (_fontChoice.CompareTo("Alexandria") == 0 || _fontChoice.CompareTo("Garnet") == 0)
                                _fontChoice = _fontDefaultPSX;
                        }
                    }
                    _fontChoiceBox.SelectedItem = _fontChoice;
                }
                if (SBUIInstalled)
                {
                    value = iniFile.ReadValue("Graphics", nameof(ScaledBattleUI));
                    if (String.IsNullOrEmpty(value))
                    {
                        value = "1";
                        OnPropertyChanged(nameof(ScaledBattleUI));
                    }
                    if (!Int16.TryParse(value, out _scaledbattleui))
                        _scaledbattleui = 1;
                    OnPropertyChanged(nameof(ScaledBattleUI));

                    value = iniFile.ReadValue("Graphics", nameof(ScaleUIFactor));
                    if (String.IsNullOrEmpty(value))
                    {
                        value = "0.6";
                        OnPropertyChanged(nameof(ScaleUIFactor));
                    }
                    if (!double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _scaledbattleuiscale))
                        _scaledbattleuiscale = 0.6;
                    OnPropertyChanged(nameof(ScaleUIFactor));
                }
            }
            catch (Exception ex){ UiHelper.ShowError(Application.Current.MainWindow, ex); }
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

                IniFile iniFile = new IniFile(_iniPath);
                switch (propertyName)
                {
                    case nameof(WidescreenSupport):
                        iniFile.WriteValue("Graphics", propertyName, " " + WidescreenSupport.ToString());
                        if (WidescreenSupport == 1)
                            iniFile.WriteValue("Graphics", "Enabled", " 1");
                        break;
                    case nameof(BattleInterface):
                        iniFile.WriteValue("Interface", "BattleMenuPosX", " " + (Int32)BattleInterfaceMenu.X);
                        iniFile.WriteValue("Interface", "BattleMenuPosY", " " + (Int32)BattleInterfaceMenu.Y);
                        iniFile.WriteValue("Interface", "BattleMenuWidth", " " + (Int32)BattleInterfaceMenu.Width);
                        iniFile.WriteValue("Interface", "BattleMenuHeight", " " + (Int32)BattleInterfaceMenu.Height);
                        iniFile.WriteValue("Interface", "BattleDetailPosX", " " + (Int32)BattleInterfaceDetail.X);
                        iniFile.WriteValue("Interface", "BattleDetailPosY", " " + (Int32)BattleInterfaceDetail.Y);
                        iniFile.WriteValue("Interface", "BattleDetailWidth", " " + (Int32)BattleInterfaceDetail.Width);
                        iniFile.WriteValue("Interface", "BattleDetailHeight", " " + (Int32)BattleInterfaceDetail.Height);
                        iniFile.WriteValue("Interface", "BattleRowCount", " " + (BattleInterface == 2 ? 4 : 5));
                        iniFile.WriteValue("Interface", "BattleColumnCount", " " + (BattleInterface == 2 ? 1 : 1));
                        iniFile.WriteValue("Interface", "PSXBattleMenu", " " + (BattleInterface == 2 ? 1 : 0));
                        break;
                    case nameof(SkipIntros):
                        if (SkipIntros == 3)
                        {
                            iniFile.WriteValue("Graphics", propertyName, " 3");
                            iniFile.WriteValue("Graphics", "Enabled ", " 1");
                        }
                        else if (SkipIntros == 0)
                        {
                            iniFile.WriteValue("Graphics", propertyName, " 0");
                        }
                        break;
                    case nameof(HideCards):
                        iniFile.WriteValue("Icons", propertyName, " " + HideCards);
                        iniFile.WriteValue("Icons", "HideBeach", " " + HideCards); // Merged
                        iniFile.WriteValue("Icons", "HideSteam", " " + HideCards); // Merged
                        if (HideCards == 1)
                            iniFile.WriteValue("Icons", "Enabled ", " 1");
                        break;
                    case nameof(Speed):
                        iniFile.WriteValue("Battle", propertyName, " " + (Speed < 3 ? Speed : 5));
                        if (Speed != 0)
                            iniFile.WriteValue("Battle", "Enabled ", " 1");
                        break;
                    case nameof(TripleTriad):
                        iniFile.WriteValue("TetraMaster", propertyName, " " + TripleTriad);
                        if (TripleTriad > 0)
                            iniFile.WriteValue("TetraMaster", "Enabled", " 1");
                        break;
                    case nameof(BattleSwirlFrames):
                        iniFile.WriteValue("Graphics", propertyName, " " + BattleSwirlFrames);
                        break;
                    case nameof(SoundVolume):
                        iniFile.WriteValue("Audio", propertyName, " " + SoundVolume);
                        break;
                    case nameof(MusicVolume):
                        iniFile.WriteValue("Audio", propertyName, " " + MusicVolume);
                        break;
                    case nameof(UseGarnetFont):
                        iniFile.WriteValue("Graphics", propertyName, " " + UseGarnetFont);
                        if (UseGarnetFont == 1)
                            iniFile.WriteValue("Graphics", "Enabled ", " 1");
                        break;
                    case nameof(FontChoice):
                        if (FontChoice.CompareTo(_fontDefaultPC) == 0)
                            iniFile.WriteValue("Font", "Enabled ", " 0");
                        else if (FontChoice.CompareTo(_fontDefaultPSX) == 0)
                        {
                            iniFile.WriteValue("Font", "Enabled ", " 1");
                            iniFile.WriteValue("Font", "Names", " \"Alexandria\", \"Garnet\"");
                        }
                        else
                        {
                            iniFile.WriteValue("Font", "Enabled ", " 1");
                            iniFile.WriteValue("Font", "Names", " \"" + FontChoice + "\", \"Times Bold\"");
                        }
                        break;
                    case nameof(ScaledBattleUI):
                        iniFile.WriteValue("Graphics", propertyName, " " + ScaledBattleUI);
                        if (ScaledBattleUI == 1)
                            iniFile.WriteValue("Graphics", "Enabled ", " 1");
                        break;
                    case nameof(ScaleUIFactor):
                        iniFile.WriteValue("Graphics", propertyName, " " + ScaleUIFactor.ToString().Replace(',', '.'));
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
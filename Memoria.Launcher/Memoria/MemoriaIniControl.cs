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
            SBUIInstalled = IsOptionPresentInIni("Graphics", "ScaledBattleUI");
            short baseNumberOfRows = 23;
            short numberOfRows = baseNumberOfRows;
            if (PsxFontInstalled)
                numberOfRows = (short)(numberOfRows + 2);
            if (SBUIInstalled)
                numberOfRows = (short)(numberOfRows + 4);

            SetRows((short)(numberOfRows + 1));
            SetCols(8);
            

            Width = 200;
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;
            Margin = new Thickness(0);

            DataContext = this;

            Thickness rowMargin = new Thickness(8, 0, 3, 0);

            

            UiTextBlock optionsText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.IniOptions), row: 0, col: 0, rowSpan: 3, colSpan: 8);
            optionsText.Padding = new Thickness(0, 4, 0, 0);
            optionsText.Foreground = Brushes.White;
            optionsText.FontSize = 14;
            optionsText.FontWeight = FontWeights.Bold;
            optionsText.Margin = rowMargin;


            UiCheckBox isWidescreenSupport = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Widescreen, null), 3, 0, 2, 8);
            isWidescreenSupport.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(WidescreenSupport)) { Mode = BindingMode.TwoWay });
            isWidescreenSupport.Foreground = Brushes.White;
            isWidescreenSupport.Margin = rowMargin;


            UiCheckBox isSkipIntros = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.SkipIntrosToMainMenu, null), 5, 0, 2, 8);
            isSkipIntros.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(SkipIntros)) { Mode = BindingMode.TwoWay });
            isSkipIntros.Foreground = Brushes.White;
            isSkipIntros.Margin = rowMargin;

            /*UiCheckBox battleSwirlFrames = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.SkipBattleLoading, null), 7, 0, 2, 8);
            battleSwirlFrames.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(BattleSwirlFrames)) { Mode = BindingMode.TwoWay });
            battleSwirlFrames.Foreground = Brushes.White;
            battleSwirlFrames.Margin = rowMargin;*/

            

            UiCheckBox isHideCards = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.HideCardsBubbles, null), 7, 0, 2, 8);
            isHideCards.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(HideCards)) { Mode = BindingMode.TwoWay });
            isHideCards.Foreground = Brushes.White;
            isHideCards.Margin = rowMargin;

            UiCheckBox isTurnBased = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.TurnBasedBattles, null), 9, 0, 2, 8);
            isTurnBased.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(Speed)) { Mode = BindingMode.TwoWay });
            isTurnBased.Foreground = Brushes.White;
            isTurnBased.Margin = rowMargin;

            UiTextBlock battleSwirlFramesText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.SkipBattleLoading), row: 11, col: 0, rowSpan: 2, colSpan: 8);
            battleSwirlFramesText.Foreground = Brushes.White;
            battleSwirlFramesText.Margin = rowMargin;
            UiTextBlock battleSwirlFramesTextindex = AddUiElement(UiTextBlockFactory.Create(""), row: 13, col: 0, rowSpan: 2, colSpan: 2);
            battleSwirlFramesTextindex.SetBinding(TextBlock.TextProperty, new Binding(nameof(BattleSwirlFrames)) { Mode = BindingMode.TwoWay });
            battleSwirlFramesTextindex.Foreground = Brushes.White;
            battleSwirlFramesTextindex.Margin = rowMargin;
            Slider battleSwirlFrames = AddUiElement(UiSliderFactory.Create(0), row: 13, col: 1, rowSpan: 1, colSpan: 7);
            battleSwirlFrames.SetBinding(Slider.ValueProperty, new Binding(nameof(BattleSwirlFrames)) { Mode = BindingMode.TwoWay });
            battleSwirlFrames.TickFrequency = 5;
            //soundVolumeSlider.TickPlacement = TickPlacement.BottomRight;
            battleSwirlFrames.IsSnapToTickEnabled = true;
            battleSwirlFrames.Minimum = 0;
            battleSwirlFrames.Maximum = 120;
            battleSwirlFrames.Margin = rowMargin;
            //soundVolumeSlider.Height = 4;

            UiTextBlock soundVolumeText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.SoundVolume), row: 15, col: 0, rowSpan: 2, colSpan: 8);
            soundVolumeText.Foreground = Brushes.White;
            soundVolumeText.Margin = rowMargin;
            UiTextBlock soundVolumeTextIndex = AddUiElement(UiTextBlockFactory.Create(""), row: 17, col: 0, rowSpan: 2, colSpan: 2);
            soundVolumeTextIndex.SetBinding(TextBlock.TextProperty, new Binding(nameof(SoundVolume)) { Mode = BindingMode.TwoWay });
            soundVolumeTextIndex.Foreground = Brushes.White;
            soundVolumeTextIndex.Margin = rowMargin;
            //soundVolumeTextIndex.TextAlignment = TextAlignment.Right;
            Slider soundVolumeSlider = AddUiElement(UiSliderFactory.Create(0), row: 17, col: 1, rowSpan: 1, colSpan: 7);
            soundVolumeSlider.SetBinding(Slider.ValueProperty, new Binding(nameof(SoundVolume)) { Mode = BindingMode.TwoWay });
            soundVolumeSlider.TickFrequency = 5;
            soundVolumeSlider.IsSnapToTickEnabled = true;
            soundVolumeSlider.Minimum = 0;
            soundVolumeSlider.Maximum = 100;
            soundVolumeSlider.Margin = rowMargin;

            UiTextBlock musicVolumeText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.MusicVolume), row: 19, col: 0, rowSpan: 2, colSpan: 8);
            musicVolumeText.Foreground = Brushes.White;
            musicVolumeText.Margin = rowMargin;
            UiTextBlock musicVolumeTextIndex = AddUiElement(UiTextBlockFactory.Create(""), row: 21, col: 0, rowSpan: 2, colSpan: 2);
            musicVolumeTextIndex.SetBinding(TextBlock.TextProperty, new Binding(nameof(MusicVolume)) { Mode = BindingMode.TwoWay });
            musicVolumeTextIndex.Foreground = Brushes.White;
            musicVolumeTextIndex.Margin = rowMargin;
            Slider musicVolumeSlider = AddUiElement(UiSliderFactory.Create(0), row: 21, col: 1, rowSpan: 1, colSpan: 7);
            musicVolumeSlider.SetBinding(Slider.ValueProperty, new Binding(nameof(MusicVolume)) { Mode = BindingMode.TwoWay });
            musicVolumeSlider.TickFrequency = 5;
            musicVolumeSlider.IsSnapToTickEnabled = true;
            musicVolumeSlider.Minimum = 0;
            musicVolumeSlider.Maximum = 100;
            musicVolumeSlider.Margin = rowMargin;



            if (PsxFontInstalled)
            {
                /*UiCheckBox useGarnetFont = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.UsePsxFont, null), (short)(baseNumberOfRows), 0, 2, 8);
                useGarnetFont.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(UseGarnetFont)) { Mode = BindingMode.TwoWay });*/
                UiTextBlock fontChoiceText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.FontChoice), row: baseNumberOfRows, col: 0, rowSpan: 2, colSpan: 3);
                fontChoiceText.Foreground = Brushes.White;
                fontChoiceText.Margin = rowMargin;
                _fontChoiceBox = AddUiElement(UiComboBoxFactory.Create(), (short)(baseNumberOfRows), 2, 2, 6);
                FontCollection installedFonts = new InstalledFontCollection();
                String[] fontNames = new String[installedFonts.Families.Length + 2];
                fontNames[0] = _fontDefaultPC;
                fontNames[1] = _fontDefaultPSX;
                for (Int32 fontindex = 0; fontindex < installedFonts.Families.Length; ++fontindex)
                    fontNames[fontindex+2] = installedFonts.Families[fontindex].Name;
                _fontChoiceBox.ItemsSource = fontNames;
                _fontChoiceBox.SetBinding(Selector.SelectedItemProperty, new Binding(nameof(FontChoice)) { Mode = BindingMode.TwoWay });
                //fontChoice.Foreground = Brushes.White;
                _fontChoiceBox.Margin = rowMargin;

                baseNumberOfRows = (short)(baseNumberOfRows + 2);
            }

            if (SBUIInstalled)
            {
                UiCheckBox useSBUI = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.SBUIenabled, null), (short)(baseNumberOfRows), 0, 2, 8);
                useSBUI.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(ScaledBattleUI)) { Mode = BindingMode.TwoWay });
                useSBUI.Foreground = Brushes.White;
                useSBUI.Margin = rowMargin;

                Slider sBUIScale = AddUiElement(UiSliderFactory.Create(0), row: (short)(baseNumberOfRows + 2), col: 1, rowSpan: 1, colSpan: 7);
                sBUIScale.SetBinding(Slider.ValueProperty, new Binding(nameof(ScaleUIFactor)) { Mode = BindingMode.TwoWay });
                sBUIScale.TickFrequency = (double)0.1;
                //musicVolumeSlider.TickPlacement = TickPlacement.BottomRight;
                sBUIScale.IsSnapToTickEnabled = true;
                sBUIScale.Minimum = (double)0.1;
                sBUIScale.Maximum = (double)3.0;
                sBUIScale.Margin = rowMargin;

                UiTextBlock sBUIScaleTextindex = AddUiElement(UiTextBlockFactory.Create(""), row: (short)(baseNumberOfRows + 2), col: 0, rowSpan: 2, colSpan: 1);
                sBUIScaleTextindex.SetBinding(TextBlock.TextProperty, new Binding(nameof(ScaleUIFactor)) { Mode = BindingMode.TwoWay });
                sBUIScaleTextindex.Foreground = Brushes.White;
                sBUIScaleTextindex.Margin = new Thickness(8, 0, 0, 0);

                //baseNumberOfRows = (short)(baseNumberOfRows + 4);

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
            get { return _isturnbased; }
            set
            {
                if (_isturnbased != value)
                {
                    _isturnbased = value;
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
        private Int16 _iswidescreensupport, _isskipintros, _ishidecards, _isturnbased, _battleswirlframes, _soundvolume, _musicvolume, _usegarnetfont, _scaledbattleui;
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
                if (!Int16.TryParse(value, out _isturnbased))
                    _isturnbased = 0;
                value = iniFile.ReadValue("Graphics", nameof(BattleSwirlFrames));
                if (String.IsNullOrEmpty(value))
                {
                    value = "20";
                    OnPropertyChanged(nameof(BattleSwirlFrames));
                }
                if (!Int16.TryParse(value, out _battleswirlframes))
                    _battleswirlframes = 20;
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
                Refresh(nameof(SkipIntros));
                Refresh(nameof(HideCards));
                Refresh(nameof(Speed));
                Refresh(nameof(BattleSwirlFrames));
                Refresh(nameof(SoundVolume));
                Refresh(nameof(MusicVolume));

                if (PsxFontInstalled) {
                    /*value = iniFile.ReadValue("Graphics", nameof(UseGarnetFont));
                    if (String.IsNullOrEmpty(value))
                    {
                        value = "1";
                        OnPropertyChanged(nameof(UseGarnetFont));
                    }
                    if (!Int16.TryParse(value, out _usegarnetfont))
                        _usegarnetfont = 1;
                    OnPropertyChanged(nameof(UseGarnetFont));*/
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
                        iniFile.WriteValue("Graphics", propertyName, " " + (WidescreenSupport).ToString());
                        if (WidescreenSupport == 1)
                        {
                            iniFile.WriteValue("Graphics", "Enabled", " 1");
                        }
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
                        iniFile.WriteValue("Icons", propertyName, " " + (HideCards).ToString());
                        if (HideCards == 1)
                        {
                            iniFile.WriteValue("Icons", "Enabled ", " 1");
                        }
                        break;
                    case nameof(Speed):
                        if (Speed == 1)
                        {
                            iniFile.WriteValue("Battle", propertyName, " 2");
                            iniFile.WriteValue("Battle", "Enabled ", " 1");
                        }
                        else if (Speed == 0)
                        {
                            iniFile.WriteValue("Battle", propertyName, " 0");
                        }
                        break;
                    case nameof(BattleSwirlFrames):
                        iniFile.WriteValue("Graphics", propertyName, " " + (BattleSwirlFrames).ToString());
                        break;
                    case nameof(SoundVolume):
                        iniFile.WriteValue("Audio", propertyName, " " + (SoundVolume).ToString());
                        break;
                    case nameof(MusicVolume):
                        iniFile.WriteValue("Audio", propertyName, " " + (MusicVolume).ToString());
                        break;
                    case nameof(UseGarnetFont):
                        if (UseGarnetFont == 1)
                        {
                            iniFile.WriteValue("Graphics", "Enabled ", " 1");
                        }
                        iniFile.WriteValue("Graphics", propertyName, " " + (UseGarnetFont).ToString());
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
                        if (ScaledBattleUI == 1)
                        {
                            iniFile.WriteValue("Graphics", "Enabled ", " 1");
                        }
                        iniFile.WriteValue("Graphics", propertyName, " " + (ScaledBattleUI).ToString());
                        break;
                    case nameof(ScaleUIFactor):
                        iniFile.WriteValue("Graphics", propertyName, " " + (ScaleUIFactor).ToString().Replace(',', '.'));
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
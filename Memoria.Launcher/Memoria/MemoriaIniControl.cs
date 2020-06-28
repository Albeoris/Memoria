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


            PsxFontInstalled = IsOptionPresentInIni("Graphics", "UseGarnetFont");
            SBUIInstalled = IsOptionPresentInIni("Graphics", "ScaledBattleUI");
            short baseNumberOfRows = 20;
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

            UiCheckBox isBattleSwirlFramesZero = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.SkipBattleLoading, null), 7, 0, 2, 8);
            isBattleSwirlFramesZero.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(BattleSwirlFrames)) { Mode = BindingMode.TwoWay });
            isBattleSwirlFramesZero.Foreground = Brushes.White;
            isBattleSwirlFramesZero.Margin = rowMargin;

            UiCheckBox isHideCards = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.HideCardsBubbles, null), 9, 0, 2, 8);
            isHideCards.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(HideCards)) { Mode = BindingMode.TwoWay });
            isHideCards.Foreground = Brushes.White;
            isHideCards.Margin = rowMargin;

            UiCheckBox isTurnBased = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.TurnBasedBattles, null), 11, 0, 2, 8);
            isTurnBased.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(Speed)) { Mode = BindingMode.TwoWay });
            isTurnBased.Foreground = Brushes.White;
            isTurnBased.Margin = rowMargin;

            UiTextBlock soundVolumeText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.SoundVolume), row: 13, col: 0, rowSpan: 2, colSpan: 8);
            soundVolumeText.Foreground = Brushes.White;
            soundVolumeText.Margin = rowMargin;

            Slider soundVolumeSlider = AddUiElement(UiSliderFactory.Create(0), row: 15, col: 0, rowSpan: 1, colSpan: 8);
            soundVolumeSlider.SetBinding(Slider.ValueProperty, new Binding(nameof(SoundVolume)) { Mode = BindingMode.TwoWay });
            soundVolumeSlider.TickFrequency = 5;
            //soundVolumeSlider.TickPlacement = TickPlacement.BottomRight;
            soundVolumeSlider.IsSnapToTickEnabled = true;
            soundVolumeSlider.Minimum = 0;
            soundVolumeSlider.Maximum = 100;
            soundVolumeSlider.Margin = rowMargin;
            //soundVolumeSlider.Height = 4;

            UiTextBlock musicVolumeText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.MusicVolume), row: 16, col: 0, rowSpan: 2, colSpan: 8);
            musicVolumeText.Foreground = Brushes.White;
            musicVolumeText.Margin = rowMargin;

            Slider musicVolumeSlider = AddUiElement(UiSliderFactory.Create(0), row: 18, col: 0, rowSpan: 1, colSpan: 8);
            musicVolumeSlider.SetBinding(Slider.ValueProperty, new Binding(nameof(MusicVolume)) { Mode = BindingMode.TwoWay });
            musicVolumeSlider.TickFrequency = 5;
            //musicVolumeSlider.TickPlacement = TickPlacement.BottomRight;
            musicVolumeSlider.IsSnapToTickEnabled = true;
            musicVolumeSlider.Minimum = 0;
            musicVolumeSlider.Maximum = 100;
            musicVolumeSlider.Margin = rowMargin;



            if (PsxFontInstalled)
            {
                UiCheckBox useGarnetFont = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.UsePsxFont, null), (short)(baseNumberOfRows), 0, 2, 8);
                useGarnetFont.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(UseGarnetFont)) { Mode = BindingMode.TwoWay });
                useGarnetFont.Foreground = Brushes.White;
                useGarnetFont.Margin = rowMargin;

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
        public Int16 BattleSwirlFrames
        {
            get { return _isbattleswirlframeszero; }
            set
            {
                if (_isbattleswirlframeszero != value)
                {
                    _isbattleswirlframeszero = value;
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


        private Int16 _iswidescreensupport, _isskipintros, _isbattleswirlframeszero, _ishidecards, _isturnbased, _soundvolume, _musicvolume, _usegarnetfont, _scaledbattleui;
        private double _scaledbattleuiscale;
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
                    value = "1";
                if (!Int16.TryParse(value, out _iswidescreensupport))
                    _iswidescreensupport = 1;

                value = iniFile.ReadValue("Graphics", nameof(SkipIntros));
                if (String.IsNullOrEmpty(value))
                    value = "0";
                if (!Int16.TryParse(value, out _isskipintros))
                    _isskipintros = 0;

                value = iniFile.ReadValue("Graphics", nameof(BattleSwirlFrames));
                if (String.IsNullOrEmpty(value))
                    value = "13";
                if (!Int16.TryParse(value, out _isbattleswirlframeszero))
                    _isbattleswirlframeszero = 13;

                value = iniFile.ReadValue("Icons", nameof(HideCards));
                if (String.IsNullOrEmpty(value))
                    value = "0";
                if (!Int16.TryParse(value, out _ishidecards))
                    _ishidecards = 0;

                value = iniFile.ReadValue("Battle", nameof(Speed));
                if (String.IsNullOrEmpty(value))
                    value = "0";
                if (!Int16.TryParse(value, out _isturnbased))
                    _isturnbased = 0;

                value = iniFile.ReadValue("Audio", nameof(SoundVolume));
                if (String.IsNullOrEmpty(value))
                    value = "100";
                if (!Int16.TryParse(value, out _soundvolume))
                    _soundvolume = 100;

                value = iniFile.ReadValue("Audio", nameof(MusicVolume));
                if (String.IsNullOrEmpty(value))
                    value = "100";
                if (!Int16.TryParse(value, out _musicvolume))
                    _musicvolume = 100;

                


                OnPropertyChanged(nameof(WidescreenSupport));
                OnPropertyChanged(nameof(SkipIntros));
                OnPropertyChanged(nameof(BattleSwirlFrames));
                OnPropertyChanged(nameof(HideCards));
                OnPropertyChanged(nameof(Speed));
                OnPropertyChanged(nameof(SoundVolume));
                OnPropertyChanged(nameof(MusicVolume));



                if (PsxFontInstalled) {
                    value = iniFile.ReadValue("Graphics", nameof(UseGarnetFont));
                    if (String.IsNullOrEmpty(value))
                        value = "1";
                    if (!Int16.TryParse(value, out _usegarnetfont))
                        _usegarnetfont = 1;
                    OnPropertyChanged(nameof(UseGarnetFont));
                }
                if (SBUIInstalled)
                {
                    value = iniFile.ReadValue("Graphics", nameof(ScaledBattleUI));
                    if (String.IsNullOrEmpty(value))
                        value = "1";
                    if (!Int16.TryParse(value, out _scaledbattleui))
                        _scaledbattleui = 1;
                    OnPropertyChanged(nameof(ScaledBattleUI));

                    value = iniFile.ReadValue("Graphics", nameof(ScaleUIFactor));
                    if (String.IsNullOrEmpty(value))
                        value = "0.6";
                    if (!double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _scaledbattleuiscale))
                        _scaledbattleuiscale = 0.6;
                    OnPropertyChanged(nameof(ScaleUIFactor));
                }
            }
            catch (Exception ex){ UiHelper.ShowError(Application.Current.MainWindow, ex); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
                    case nameof(BattleSwirlFrames):
                        if (BattleSwirlFrames == 1)
                        {
                            iniFile.WriteValue("Graphics", propertyName, " 0");
                            iniFile.WriteValue("Graphics", "Enabled ", " 1");
                        }
                        else if (BattleSwirlFrames == 13)
                        {
                            iniFile.WriteValue("Graphics", propertyName, "13");
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
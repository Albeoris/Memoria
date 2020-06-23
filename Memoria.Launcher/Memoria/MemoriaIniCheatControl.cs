﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
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
    public sealed class MemoriaIniCheatControl : UiGrid, INotifyPropertyChanged
    {
        public MemoriaIniCheatControl()
        {
            SetRows(24);
            SetCols(8);
            

            Width = 200;
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;
            Margin = new Thickness(0);

            DataContext = this;

            Thickness rowMargin = new Thickness(8, 0, 3, 0);


            UiTextBlock cheatOptionsText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.IniCheats), row: 0, col: 0, rowSpan: 3, colSpan: 8);
            cheatOptionsText.Padding = new Thickness(0, 4, 0, 0);
            cheatOptionsText.Foreground = Brushes.White;
            cheatOptionsText.FontSize = 14;
            cheatOptionsText.FontWeight = FontWeights.Bold;
            cheatOptionsText.Margin = rowMargin;

            UiCheckBox stealingAlwaysWorks = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.MaxStealRate, null), 3, 0, 2, 8);
            stealingAlwaysWorks.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(StealingAlwaysWorks)) { Mode = BindingMode.TwoWay });
            stealingAlwaysWorks.Foreground = Brushes.White;
            stealingAlwaysWorks.Margin = rowMargin;


            UiCheckBox garnetConcentrate = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.DisableCantConcentrate, null), 5, 0, 2, 8);
            garnetConcentrate.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(GarnetConcentrate)) { Mode = BindingMode.TwoWay });
            garnetConcentrate.Foreground = Brushes.White;
            garnetConcentrate.Margin = rowMargin;

            UiCheckBox speedMode = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.SpeedMode, null), 7, 0, 2, 8);
            speedMode.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(SpeedMode)) { Mode = BindingMode.TwoWay });
            speedMode.Foreground = Brushes.White;
            speedMode.Margin = rowMargin;

            UiTextBlock speedFactorText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.SpeedFactor), row: 9, col: 0, rowSpan: 2, colSpan: 2);
            speedFactorText.Foreground = Brushes.White;
            speedFactorText.Margin = rowMargin;

            UiTextBlock speedFactorTextindex = AddUiElement(UiTextBlockFactory.Create(""), row: 9, col: 2, rowSpan: 2, colSpan: 1);
            speedFactorTextindex.SetBinding(TextBlock.TextProperty, new Binding(nameof(SpeedFactor)) { Mode = BindingMode.TwoWay });
            speedFactorTextindex.Foreground = Brushes.White;

            Slider speedFactor = AddUiElement(UiSliderFactory.Create(0), row: 9, col: 3, rowSpan: 1, colSpan: 5);
            speedFactor.SetBinding(Slider.ValueProperty, new Binding(nameof(SpeedFactor)) { Mode = BindingMode.TwoWay });
            speedFactor.TickFrequency = 1;
            speedFactor.IsSnapToTickEnabled = true;
            speedFactor.Minimum = 2;
            speedFactor.Maximum = 12;
            speedFactor.Margin = new Thickness(0, 0, 3, 0);

            UiCheckBox battleAssistance = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.PermanentTranse, null), 11, 0, 2, 8);
            battleAssistance.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(BattleAssistance)) { Mode = BindingMode.TwoWay });
            battleAssistance.Foreground = Brushes.White;
            battleAssistance.Margin = rowMargin;

            UiCheckBox attack9999 = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.MaxDamage, null), 13, 0, 2, 8);
            attack9999.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(Attack9999)) { Mode = BindingMode.TwoWay });
            attack9999.Foreground = Brushes.White;
            attack9999.Margin = rowMargin;

            UiCheckBox noRandomEncounter = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.NoRandomBattles, null), 15, 0, 2, 8);
            noRandomEncounter.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(NoRandomEncounter)) { Mode = BindingMode.TwoWay });
            noRandomEncounter.Foreground = Brushes.White;
            noRandomEncounter.Margin = rowMargin;

            UiCheckBox masterSkill = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.PermanentCheats, null), 17, 0, 2, 8);
            masterSkill.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(MasterSkill)) { Mode = BindingMode.TwoWay });
            masterSkill.Foreground = Brushes.White;
            masterSkill.Margin = rowMargin;

            LoadSettings();
        }

        public Int16 StealingAlwaysWorks
        {
            get { return _stealingalwaysworks; }
            set
            {
                if (_stealingalwaysworks != value)
                {
                    _stealingalwaysworks = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 GarnetConcentrate
        {
            get { return _garnetconcentrate; }
            set
            {
                if (_garnetconcentrate != value)
                {
                    _garnetconcentrate = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 SpeedMode
        {
            get { return _speedmode; }
            set
            {
                if (_speedmode != value)
                {
                    _speedmode = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 SpeedFactor
        {
            get { return _speedfactor; }
            set
            {
                if (_speedfactor != value)
                {
                    _speedfactor = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 BattleAssistance
        {
            get { return _battleassistance; }
            set
            {
                if (_battleassistance != value)
                {
                    _battleassistance = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 Attack9999
        {
            get { return _attack9999; }
            set
            {
                if (_attack9999 != value)
                {
                    _attack9999 = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 NoRandomEncounter
        {
            get { return _norandomencounter; }
            set
            {
                if (_norandomencounter != value)
                {
                    _norandomencounter = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 MasterSkill
        {
            get { return _masterskill; }
            set
            {
                if (_masterskill != value)
                {
                    _masterskill = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int16 _stealingalwaysworks, _garnetconcentrate, _speedmode, _speedfactor, _battleassistance, _attack9999, _norandomencounter, _masterskill;

        private readonly String _iniPath = AppDomain.CurrentDomain.BaseDirectory + "\\Memoria.ini";

        private void LoadSettings()
        {

            try
            {
                IniFile iniFile = new IniFile(_iniPath);
                
                String value = iniFile.ReadValue("Hacks", nameof(StealingAlwaysWorks));
                if (String.IsNullOrEmpty(value))
                    value = "0";
                if (!Int16.TryParse(value, out _stealingalwaysworks))
                    _stealingalwaysworks = 0;

                value = iniFile.ReadValue("Battle", nameof(GarnetConcentrate));
                if (String.IsNullOrEmpty(value))
                    value = "0";
                if (!Int16.TryParse(value, out _garnetconcentrate))
                    _garnetconcentrate = 0;

                value = iniFile.ReadValue("Cheats", nameof(SpeedMode));
                if (String.IsNullOrEmpty(value))
                    value = "0";
                if (!Int16.TryParse(value, out _speedmode))
                    _speedmode = 0;

                value = iniFile.ReadValue("Cheats", nameof(SpeedFactor));
                if (String.IsNullOrEmpty(value))
                    value = "2";
                if (!Int16.TryParse(value, out _speedfactor))
                    _speedfactor = 2;

                value = iniFile.ReadValue("Cheats", nameof(BattleAssistance));
                if (String.IsNullOrEmpty(value))
                    value = "0";
                if (!Int16.TryParse(value, out _battleassistance))
                    _battleassistance = 0;

                value = iniFile.ReadValue("Cheats", nameof(Attack9999));
                if (String.IsNullOrEmpty(value))
                    value = "0";
                if (!Int16.TryParse(value, out _attack9999))
                    _attack9999 = 0;

                value = iniFile.ReadValue("Cheats", nameof(NoRandomEncounter));
                if (String.IsNullOrEmpty(value))
                    value = "0";
                if (!Int16.TryParse(value, out _norandomencounter))
                    _norandomencounter = 0;

                value = iniFile.ReadValue("Cheats", nameof(MasterSkill));
                if (String.IsNullOrEmpty(value))
                    value = "0";
                if (!Int16.TryParse(value, out _masterskill))
                    _masterskill = 0;


                OnPropertyChanged(nameof(StealingAlwaysWorks));
                OnPropertyChanged(nameof(GarnetConcentrate));
                OnPropertyChanged(nameof(SpeedMode));
                OnPropertyChanged(nameof(SpeedFactor));
                OnPropertyChanged(nameof(BattleAssistance));
                OnPropertyChanged(nameof(Attack9999));
                OnPropertyChanged(nameof(NoRandomEncounter));
                OnPropertyChanged(nameof(MasterSkill));
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
                    case nameof(StealingAlwaysWorks):
                        
                        
                        
                        iniFile.WriteValue("Hacks", propertyName, " " + (StealingAlwaysWorks).ToString());
                        if (StealingAlwaysWorks == 0)
                        {
                            iniFile.WriteValue("Hacks", propertyName, " 0");
                        }
                        else if (StealingAlwaysWorks == 1)
                        {
                            iniFile.WriteValue("Hacks", "Enabled", " 1");
                            iniFile.WriteValue("Hacks", propertyName, " 2");
                        }
                        break;
                    case nameof(GarnetConcentrate):
                        iniFile.WriteValue("Battle", propertyName, " " + (GarnetConcentrate).ToString());
                        if (GarnetConcentrate == 1)
                        {
                            iniFile.WriteValue("Battle", "Enabled", " 1");
                        }
                        break;
                    case nameof(SpeedMode):
                        iniFile.WriteValue("Cheats", propertyName, " " + (SpeedMode).ToString());
                        if (SpeedMode == 1)
                        {
                            iniFile.WriteValue("Cheats", "Enabled", " 1");
                        }
                        break;
                    case nameof(SpeedFactor):
                        if (SpeedFactor < 13)
                        {
                            iniFile.WriteValue("Cheats", propertyName, " " + (SpeedFactor).ToString());
                        }
                        break;
                    case nameof(BattleAssistance):
                        iniFile.WriteValue("Cheats", propertyName, " " + (BattleAssistance).ToString());
                        if (BattleAssistance == 1)
                        {
                            iniFile.WriteValue("Cheats", "Enabled", " 1");
                        }
                        break;
                    case nameof(Attack9999):
                        iniFile.WriteValue("Cheats", propertyName, " " + (Attack9999).ToString());
                        if (Attack9999 == 1)
                        {
                            iniFile.WriteValue("Cheats", "Enabled", " 1");
                        }
                        break;
                    case nameof(NoRandomEncounter):
                        iniFile.WriteValue("Cheats", propertyName, " " + (NoRandomEncounter).ToString());
                        if (NoRandomEncounter == 1)
                        {
                            iniFile.WriteValue("Cheats", "Enabled", " 1");
                        }
                        break;
                    case nameof(MasterSkill):
                        iniFile.WriteValue("Cheats", propertyName, " " + (MasterSkill).ToString());
                        iniFile.WriteValue("Cheats", "LvMax", " " + (MasterSkill).ToString());
                        iniFile.WriteValue("Cheats", "GilMax", " " + (MasterSkill).ToString());
                        if (MasterSkill == 1)
                        {
                            iniFile.WriteValue("Cheats", "Enabled", " 1");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }
        private IEnumerable<UInt16> EnumerateSpeedFactor()
        {
            for (ushort i = 2; i < 16; i++)
                yield return i;
        }

    }
}
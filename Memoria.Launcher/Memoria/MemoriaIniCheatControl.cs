using Ini;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public sealed class MemoriaIniCheatControl : UiGrid, INotifyPropertyChanged
    {
        public MemoriaIniCheatControl()
        {
            SetRows(17);
            SetCols(8);

            Width = 260;
            VerticalAlignment = VerticalAlignment.Bottom;
            HorizontalAlignment = HorizontalAlignment.Center;
            Margin = new Thickness(0);

            DataContext = this;

            Thickness rowMargin = new Thickness(8, 2, 3, 2);

            Int32 row = 0;

            UiTextBlock cheatOptionsText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.IniCheats), row, 0, 2, 8);
            cheatOptionsText.Padding = new Thickness(0, 4, 0, 2);
            cheatOptionsText.Foreground = Brushes.White;
            cheatOptionsText.FontSize = 14;
            cheatOptionsText.FontWeight = FontWeights.Bold;
            cheatOptionsText.Margin = rowMargin;

            row++;
            row++;
            /*
            AddUiElement(UiTextBlockFactory.Create("──────────────────────────────────────"), row, 0, 1, 8).Foreground = Brushes.White;
            */

            UiCheckBox stealingAlwaysWorks = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.MaxStealRate, null), row, 0, 1, 8);
            stealingAlwaysWorks.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(StealingAlwaysWorks)) { Mode = BindingMode.TwoWay });
            stealingAlwaysWorks.Foreground = Brushes.White;
            stealingAlwaysWorks.Margin = rowMargin;
            stealingAlwaysWorks.ToolTip = Lang.Settings.MaxStealRate_Tooltip;

            row++;

            UiCheckBox noAutoTrance = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.NoAutoTrance, null), row, 0, 1, 8);
            noAutoTrance.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(NoAutoTrance)) { Mode = BindingMode.TwoWay });
            noAutoTrance.Foreground = Brushes.White;
            noAutoTrance.Margin = rowMargin;
            noAutoTrance.ToolTip = Lang.Settings.NoAutoTrance_Tooltip;

            row++;

            UiCheckBox garnetConcentrate = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.DisableCantConcentrate, null), row, 0, 1, 8);
            garnetConcentrate.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(GarnetConcentrate)) { Mode = BindingMode.TwoWay });
            garnetConcentrate.Foreground = Brushes.White;
            garnetConcentrate.Margin = rowMargin;
            garnetConcentrate.ToolTip = Lang.Settings.DisableCantConcentrate_Tooltip;

            row++;

            UiCheckBox breakDamageLimit = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.BreakDamageLimit, null), row, 0, 1, 8);
            breakDamageLimit.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(BreakDamageLimit)) { Mode = BindingMode.TwoWay });
            breakDamageLimit.Foreground = Brushes.White;
            breakDamageLimit.Margin = rowMargin;
            breakDamageLimit.ToolTip = Lang.Settings.BreakDamageLimit_Tooltip;

            row++;

            UiTextBlock accessBattleMenuText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.AccessBattleMenu), row, 0, 2, 4);
            accessBattleMenuText.Foreground = Brushes.White;
            accessBattleMenuText.Margin = rowMargin;
            accessBattleMenuText.TextWrapping = TextWrapping.WrapWithOverflow;
            accessBattleMenuText.ToolTip = Lang.Settings.AccessBattleMenu_Tooltip;
            UiComboBox accessBattleMenuBox = AddUiElement(UiComboBoxFactory.Create(), row, 4, 2, 4);
            accessBattleMenuBox.ItemsSource = new String[]{
                Lang.Settings.AccessBattleMenuType0,
                Lang.Settings.AccessBattleMenuType1,
                Lang.Settings.AccessBattleMenuType2,
                Lang.Settings.AccessBattleMenuType3
            };
            accessBattleMenuBox.SetBinding(Selector.SelectedIndexProperty, new Binding(nameof(AccessBattleMenu)) { Mode = BindingMode.TwoWay });
            accessBattleMenuBox.Height = 20;
            accessBattleMenuBox.FontSize = 10;
            accessBattleMenuBox.Margin = rowMargin;

            row++;
            row++;

            UiCheckBox speedMode = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.SpeedMode, null), row, 0, 1, 8);
            speedMode.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(SpeedMode)) { Mode = BindingMode.TwoWay });
            speedMode.Foreground = Brushes.White;
            speedMode.Margin = rowMargin;
            speedMode.ToolTip = Lang.Settings.SpeedMode_Tooltip;

            row++;

            //UiTextBlock speedFactorText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.SpeedFactor), row, 0, 1, 3);
            //speedFactorText.Foreground = Brushes.White;
            //speedFactorText.Margin = rowMargin;
            //speedFactorText.ToolTip = Lang.Settings.SpeedFactor_Tooltip;
            UiTextBlock speedFactorTextindex = AddUiElement(UiTextBlockFactory.Create(""), row, 0, 1, 1);
            speedFactorTextindex.SetBinding(TextBlock.TextProperty, new Binding(nameof(SpeedFactor)) { Mode = BindingMode.TwoWay, StringFormat = "{0}x" });
            speedFactorTextindex.Foreground = Brushes.White;
            speedFactorTextindex.Margin = rowMargin;
            Slider speedFactor = AddUiElement(UiSliderFactory.Create(0), row, 1, 1, 7);
            speedFactor.SetBinding(Slider.ValueProperty, new Binding(nameof(SpeedFactor)) { Mode = BindingMode.TwoWay });
            speedFactor.TickFrequency = 1;
            speedFactor.IsSnapToTickEnabled = true;
            speedFactor.TickPlacement = TickPlacement.BottomRight;
            speedFactor.Height = 20;
            speedFactor.Minimum = 2;
            speedFactor.Maximum = 12;
            speedFactor.Margin = new Thickness(0, 0, 3, 0);

            row++;

            UiTextBlock BattleTPSText = AddUiElement(UiTextBlockFactory.Create(Lang.Settings.BattleTPS), row, 0, 1, 8);
            BattleTPSText.Foreground = Brushes.White;
            BattleTPSText.Margin = rowMargin;
            BattleTPSText.ToolTip = Lang.Settings.BattleTPS_Tooltip;

            row++;

            UiTextBlock BattleTPSindex = AddUiElement(UiTextBlockFactory.Create(""), row, 0, 1, 1);
            BattleTPSindex.SetBinding(TextBlock.TextProperty, new Binding(nameof(BattleTPSDividedBy10)) { Mode = BindingMode.TwoWay, StringFormat = "{0}x" });
            BattleTPSindex.Foreground = Brushes.White;
            BattleTPSindex.Margin = rowMargin;
            Slider BattleTPSFactor = AddUiElement(UiSliderFactory.Create(0), row, 1, 1, 7);
            BattleTPSFactor.SetBinding(Slider.ValueProperty, new Binding(nameof(BattleTPS)) { Mode = BindingMode.TwoWay });
            BattleTPSFactor.TickFrequency = 1;
            BattleTPSFactor.IsSnapToTickEnabled = true;
            BattleTPSFactor.TickPlacement = TickPlacement.BottomRight;
            BattleTPSFactor.Height = 20;
            BattleTPSFactor.Minimum = 15;
            BattleTPSFactor.Maximum = 150;
            BattleTPSFactor.Margin = new Thickness(0, 0, 3, 0);

            row++;

            UiCheckBox battleAssistance = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.BattleAssistance, null), row, 0, 1, 8);
            battleAssistance.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(BattleAssistance)) { Mode = BindingMode.TwoWay });
            battleAssistance.Foreground = Brushes.White;
            battleAssistance.Margin = rowMargin;
            battleAssistance.ToolTip = Lang.Settings.BattleAssistance_Tooltip;

            row++;

            /*UiCheckBox attack9999 = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.MaxDamage, null), row++, 0, 1, 8);
            attack9999.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(Attack9999)) { Mode = BindingMode.TwoWay });
            attack9999.Foreground = Brushes.White;
            attack9999.Margin = rowMargin;*/

            UiCheckBox noRandomEncounter = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.NoRandomBattles, null), row, 0, 1, 8);
            noRandomEncounter.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(NoRandomEncounter)) { Mode = BindingMode.TwoWay });
            noRandomEncounter.Foreground = Brushes.White;
            noRandomEncounter.Margin = rowMargin;
            noRandomEncounter.ToolTip = Lang.Settings.NoRandomBattles_Tooltip;

            row++;

            UiCheckBox masterSkill = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.PermanentCheats, null), row, 0, 1, 8);
            masterSkill.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(MasterSkill)) { Mode = BindingMode.TwoWay });
            masterSkill.Foreground = Brushes.White;
            masterSkill.Margin = rowMargin;
            masterSkill.ToolTip = Lang.Settings.PermanentCheats_Tooltip;

            row++;

            UiCheckBox maxTetraMasterCards = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.MaxCardCount, null), row, 0, 1, 8);
            maxTetraMasterCards.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(MaxCardCount)) { Mode = BindingMode.TwoWay });
            maxTetraMasterCards.Foreground = Brushes.White;
            maxTetraMasterCards.Margin = rowMargin;
            maxTetraMasterCards.ToolTip = Lang.Settings.MaxCardCount_Tooltip;


            /*AddUiElement(UiTextBlockFactory.Create("──────────────────────────────────────"), row++, 0, 1, 8).Foreground = Brushes.White;*/



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
        public Int16 NoAutoTrance
        {
            get { return _noautotrance; }
            set
            {
                if (_noautotrance != value)
                {
                    _noautotrance = value;
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
        public Int16 BreakDamageLimit
        {
            get { return _breakDamageLimit; }
            set
            {
                if (_breakDamageLimit != value)
                {
                    _breakDamageLimit = value;
                    OnPropertyChanged();
                }
            }
        }
        public Int16 AccessBattleMenu
        {
            get { return _accessBattleMenu; }
            set
            {
                if (_accessBattleMenu != value)
                {
                    _accessBattleMenu = value;
                    OnPropertyChanged();
                }
            }
        }
        public String AvailableBattleMenus => AccessBattleMenu < 3 ? " \"Equip\", \"SupportingAbility\"" : "";
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
        public Int16 BattleTPS
        {
            get { return _battletpsfactor; }
            set
            {
                if (_battletpsfactor != value)
                {
                    _battletpsfactor = value;
                    BattleTPSDividedBy10 = ((Decimal)value / 15);
                    OnPropertyChanged();
                }
            }
        }
        public Decimal BattleTPSDividedBy10
        {
            get { return ((Decimal)BattleTPS / 15); }
            set
            {
                OnPropertyChanged();
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
        public Int16 MaxCardCount
        {
            get { return _maxcardcount; }
            set
            {
                if (_maxcardcount != value)
                {
                    _maxcardcount = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int16 _stealingalwaysworks, _noautotrance, _garnetconcentrate, _breakDamageLimit, _accessBattleMenu, _speedmode, _speedfactor, _battletpsfactor, _battleassistance, _attack9999, _norandomencounter, _masterskill, _maxcardcount;

        private readonly String _iniPath = AppDomain.CurrentDomain.BaseDirectory + "\\Memoria.ini";

        private void LoadSettings()
        {

            try
            {
                IniFile iniFile = new IniFile(_iniPath);

                String value = iniFile.ReadValue("Hacks", nameof(StealingAlwaysWorks));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(StealingAlwaysWorks));
                }
                if (!Int16.TryParse(value, out _stealingalwaysworks))
                    _stealingalwaysworks = 0;


                value = iniFile.ReadValue("Battle", nameof(NoAutoTrance));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(NoAutoTrance));
                }
                if (!Int16.TryParse(value, out _noautotrance))
                    _noautotrance = 0;

                value = iniFile.ReadValue("Battle", nameof(GarnetConcentrate));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(GarnetConcentrate));
                }
                if (!Int16.TryParse(value, out _garnetconcentrate))
                    _garnetconcentrate = 0;

                value = iniFile.ReadValue("Battle", nameof(BreakDamageLimit));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(BreakDamageLimit));
                }
                if (!Int16.TryParse(value, out _breakDamageLimit))
                    _breakDamageLimit = 0;

                value = iniFile.ReadValue("Battle", "AccessMenus");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(AccessBattleMenu));
                }
                if (!Int16.TryParse(value, out _accessBattleMenu))
                    _accessBattleMenu = 0;

                value = iniFile.ReadValue("Cheats", nameof(SpeedMode));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(SpeedMode));
                }
                if (!Int16.TryParse(value, out _speedmode))
                    _speedmode = 0;

                value = iniFile.ReadValue("Cheats", nameof(SpeedFactor));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 2";
                    OnPropertyChanged(nameof(SpeedFactor));
                }
                if (!Int16.TryParse(value, out _speedfactor))
                    _speedfactor = 2;

                value = iniFile.ReadValue("Graphics", " " + nameof(BattleTPS));
                if (String.IsNullOrEmpty(value))
                {
                    _battletpsfactor = 15;
                    value = " 15";
                    OnPropertyChanged(nameof(BattleTPS));
                }
                if (!Int16.TryParse(value, out _battletpsfactor))
                    _battletpsfactor = 15;
                Boolean valueexists = decimal.TryParse(value, out Decimal decvalue);
                BattleTPSDividedBy10 = valueexists ? decvalue / 15 : (decimal)1.5;

                value = iniFile.ReadValue("Cheats", nameof(BattleAssistance));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(BattleAssistance));
                }
                if (!Int16.TryParse(value, out _battleassistance))
                    _battleassistance = 0;

                value = iniFile.ReadValue("Cheats", nameof(Attack9999));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(Attack9999));
                }
                if (!Int16.TryParse(value, out _attack9999))
                    _attack9999 = 0;

                value = iniFile.ReadValue("Cheats", nameof(NoRandomEncounter));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(NoRandomEncounter));
                }
                if (!Int16.TryParse(value, out _norandomencounter))
                    _norandomencounter = 0;

                value = iniFile.ReadValue("Cheats", nameof(MasterSkill));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(MasterSkill));
                }
                if (!Int16.TryParse(value, out _masterskill))
                    _masterskill = 0;

                /*value = null;
                foreach (String prop in new String[] { "BattleFPS", "FieldFPS", "WorldFPS" })
                {
                    value = iniFile.ReadValue("Graphics", prop);
                    if (!String.IsNullOrEmpty(value))
                        break;
                }*/

                value = iniFile.ReadValue("TetraMaster", nameof(MaxCardCount));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 100";
                    OnPropertyChanged(nameof(MaxCardCount));
                }
                if (!Int16.TryParse(value, out _maxcardcount))
                    _maxcardcount = 1;

                Refresh(nameof(StealingAlwaysWorks));
                Refresh(nameof(NoAutoTrance));
                Refresh(nameof(GarnetConcentrate));
                Refresh(nameof(BreakDamageLimit));
                Refresh(nameof(AccessBattleMenu));
                Refresh(nameof(SpeedMode));
                Refresh(nameof(SpeedFactor));
                Refresh(nameof(BattleTPS));
                Refresh(nameof(BattleAssistance));
                Refresh(nameof(Attack9999));
                Refresh(nameof(NoRandomEncounter));
                Refresh(nameof(MasterSkill));
                Refresh(nameof(MaxCardCount));
            }
            catch (Exception ex) { UiHelper.ShowError(Application.Current.MainWindow, ex); }
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
                    case nameof(StealingAlwaysWorks):
                        iniFile.WriteValue("Hacks", propertyName + " ", " " + StealingAlwaysWorks);
                        if (StealingAlwaysWorks == 0)
                        {
                            iniFile.WriteValue("Hacks", propertyName + " ", " 0");
                        }
                        else if (StealingAlwaysWorks == 1)
                        {
                            iniFile.WriteValue("Hacks", "Enabled ", " 1");
                            iniFile.WriteValue("Hacks", propertyName + " ", " 2");
                        }
                        break;
                    case nameof(NoAutoTrance):
                        iniFile.WriteValue("Battle", propertyName + " ", " " + NoAutoTrance);
                        if (NoAutoTrance == 1)
                            iniFile.WriteValue("Battle", "Enabled ", " 1");
                        break;
                    case nameof(GarnetConcentrate):
                        iniFile.WriteValue("Battle", propertyName + " ", " " + GarnetConcentrate);
                        if (GarnetConcentrate == 1)
                            iniFile.WriteValue("Battle", "Enabled ", " 1");
                        break;
                    case nameof(BreakDamageLimit):
                        iniFile.WriteValue("Battle", propertyName + " ", " " + BreakDamageLimit);
                        if (BreakDamageLimit == 1)
                            iniFile.WriteValue("Battle", "Enabled ", " 1");
                        break;
                    case nameof(AccessBattleMenu):
                        iniFile.WriteValue("Battle", "AccessMenus ", " " + AccessBattleMenu);
                        iniFile.WriteValue("Battle", "AvailableMenus ", AvailableBattleMenus);
                        if (AccessBattleMenu > 0)
                            iniFile.WriteValue("Battle", "Enabled ", " 1");
                        break;
                    case nameof(SpeedMode):
                        iniFile.WriteValue("Cheats", propertyName + " ", " " + SpeedMode);
                        if (SpeedMode == 1)
                            iniFile.WriteValue("Cheats", "Enabled ", " 1");
                        break;
                    case nameof(SpeedFactor):
                        if (SpeedFactor < 13)
                            iniFile.WriteValue("Cheats", propertyName + " ", " " + SpeedFactor);
                        break;
                    case nameof(BattleTPS):
                        iniFile.WriteValue("Graphics", propertyName + " ", " " + BattleTPS);
                        if (BattleTPS != 15)
                            iniFile.WriteValue("Cheats", "Enabled ", " 1");
                        break;
                    case nameof(BattleAssistance):
                        iniFile.WriteValue("Cheats", propertyName + " ", " " + BattleAssistance);
                        iniFile.WriteValue("Cheats", "Attack9999 ", " " + BattleAssistance); // Merged
                        if (BattleAssistance == 1)
                            iniFile.WriteValue("Cheats", "Enabled ", " 1");
                        break;
                    case nameof(Attack9999):
                        iniFile.WriteValue("Cheats", propertyName + " ", " " + Attack9999);
                        if (Attack9999 == 1)
                            iniFile.WriteValue("Cheats", "Enabled ", " 1");
                        break;
                    case nameof(NoRandomEncounter):
                        iniFile.WriteValue("Cheats", propertyName + " ", " " + NoRandomEncounter);
                        if (NoRandomEncounter == 1)
                            iniFile.WriteValue("Cheats", "Enabled ", " 1");
                        break;
                    case nameof(MasterSkill):
                        iniFile.WriteValue("Cheats", propertyName + " ", " " + MasterSkill);
                        iniFile.WriteValue("Cheats", "LvMax ", " " + MasterSkill);
                        iniFile.WriteValue("Cheats", "GilMax ", " " + MasterSkill);
                        if (MasterSkill == 1)
                            iniFile.WriteValue("Cheats", "Enabled ", " 1");
                        break;
                    case nameof(MaxCardCount):
                        if (MaxCardCount == 1)
                        {
                            iniFile.WriteValue("TetraMaster", propertyName + " ", " 10000");
                            iniFile.WriteValue("TetraMaster", "Enabled ", " 1");
                        }
                        else if (MaxCardCount == 0)
                        {
                            iniFile.WriteValue("TetraMaster", propertyName + " ", " 100");
                        }
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

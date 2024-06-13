using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Ini;
using Microsoft.Win32;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using ComboBox = System.Windows.Controls.ComboBox;
using Control = System.Windows.Controls.Control;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming
// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable ArrangeStaticMemberQualifier
#pragma warning disable 649
#pragma warning disable 169

namespace Memoria.Launcher
{
    public sealed class ShaderSettingControl : UiGrid, INotifyPropertyChanged
    {
        public ShaderSettingControl()
        {
            SetRows(20);

            Width = 260;
            VerticalAlignment = VerticalAlignment.Bottom;
            HorizontalAlignment = HorizontalAlignment.Left;
            Margin = new Thickness(5);
            DataContext = this;

            Thickness rowMargin = new Thickness(0, 2, 0, 2);

            UiCheckBox EnableCustomShader =
                AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.EnableCustomShader, null), 3, 0, 3, 8);
            EnableCustomShader.SetBinding(ToggleButton.IsCheckedProperty,
                new Binding(nameof(CustomShader)) { Mode = BindingMode.TwoWay });
            EnableCustomShader.Foreground = Brushes.White;
            EnableCustomShader.Margin = rowMargin;
            EnableCustomShader.ToolTip = Lang.Settings.EnableCustomShader_Tooltip;

            UiCheckBox EnableToonShading =
                AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.EnableToonShading, null), 5, 0, 3, 8);
            EnableToonShading.SetBinding(ToggleButton.IsCheckedProperty,
                new Binding(nameof(ToonShading)) { Mode = BindingMode.TwoWay });
            EnableToonShading.Foreground = Brushes.White;
            EnableToonShading.ToolTip = Lang.Settings.EnableCustomShader_Tooltip;

            UiCheckBox EnableRealismShading =
                AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.EnableRealismShading, null), 7, 0, 3, 8);
            EnableRealismShading.SetBinding(ToggleButton.IsCheckedProperty,
                new Binding(nameof(RealismShading)) { Mode = BindingMode.TwoWay });
            EnableRealismShading.Foreground = Brushes.White;
            EnableRealismShading.ToolTip = Lang.Settings.EnableCustomShader_Tooltip;

            UiCheckBox EnableOutlineForFieldCharacter =
                AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.EnableOutlineForFieldCharacter, null), 9, 0, 3, 8);
            EnableOutlineForFieldCharacter.SetBinding(ToggleButton.IsCheckedProperty,
                new Binding(nameof(OutlineForFieldCharacter)) { Mode = BindingMode.TwoWay });
            EnableOutlineForFieldCharacter.Foreground = Brushes.White;
            EnableOutlineForFieldCharacter.ToolTip = Lang.Settings.EnableCustomShader_Tooltip;

            UiCheckBox EnableOutlineForBattleCharacter =
                AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.EnableOutlineForBattleCharacter, null), 11, 0, 3,
                    8);
            EnableOutlineForBattleCharacter.SetBinding(ToggleButton.IsCheckedProperty,
                new Binding(nameof(OutlineForBattleCharacter)) { Mode = BindingMode.TwoWay });
            EnableOutlineForBattleCharacter.Foreground = Brushes.White;
            EnableOutlineForBattleCharacter.ToolTip = Lang.Settings.EnableCustomShader_Tooltip;

            foreach (FrameworkElement child in Children)
            {
                //if (!ReferenceEquals(child, backround))
                child.Margin = new Thickness(child.Margin.Left + 8, child.Margin.Top, child.Margin.Right + 8,
                    child.Margin.Bottom);

                TextBlock textblock = child as TextBlock;
                if (textblock != null)
                {
                    textblock.Foreground = Brushes.WhiteSmoke;
                    textblock.FontWeight = FontWeight.FromOpenTypeWeight(500);
                    continue;
                }

                Control control = child as Control;
                if (control != null && !(control is ComboBox))
                    control.Foreground = Brushes.WhiteSmoke;
            }

            try
            {
                LoadSettings();
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
            //LoadSettings("");
        }

        #region Properties

        private Int16 _customshader, _toonShading, _realismShading, _outlineForField, _outlineForBattle;

        public Int16 CustomShader
        {
            get { return _customshader; }
            set
            {
                if (_customshader != value)
                {
                    _customshader = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int16 ToonShading
        {
            get { return _toonShading; }
            set
            {
                if (_toonShading != value)
                {
                    if (value == 1)
                    {
                        RealismShading = 0;
                    }
                    _toonShading = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int16 RealismShading
        {
            get { return _realismShading; }
            set
            {
                if (_realismShading != value)
                {
                    if (value == 1)
                    {
                        ToonShading = 0;
                    }
                    _realismShading = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int16 OutlineForFieldCharacter
        {
            get { return _outlineForField; }
            set
            {
                if (_outlineForField != value)
                {
                    _outlineForField = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int16 OutlineForBattleCharacter
        {
            get { return _outlineForBattle; }
            set
            {
                if (_outlineForBattle != value)
                {
                    _outlineForBattle = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        
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
                        MakeIniNotNull("Graphics", "CustomShader", "0");
                        MakeIniNotNull("Graphics", "ToonShading", "0");
                        MakeIniNotNull("Graphics", "RealismShading", "0");
                        MakeIniNotNull("Graphics", "OutlineForFieldCharacter", "0");
                        MakeIniNotNull("Graphics", "OutlineForBattleCharacter", "0");

                        MakeIniNotNull("Control", "Enabled", "1");
                        MakeIniNotNull("Control", "DisableMouse", "0");
                        MakeIniNotNull("Control", "DialogProgressButtons", "\"Confirm\"");
                        MakeIniNotNull("Control", "WrapSomeMenus", "1");
                        MakeIniNotNull("Control", "BattleAutoConfirm", "1");
                        MakeIniNotNull("Control", "TurboDialog", "1");
                        MakeIniNotNull("Control", "PSXScrollingMethod", "1");
                        MakeIniNotNull("Control", "PSXMovementMethod", "1");
                        MakeIniNotNull("Control", "AlwaysCaptureGamepad", "0");

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

        private async void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                IniFile iniFile = new IniFile(_iniPath);
                switch (propertyName)
                {
                    case nameof(CustomShader):
                        iniFile.WriteValue("Graphics", "CustomShader ", " " + CustomShader.ToString());
                        break;
                    case nameof(ToonShading):
                        iniFile.WriteValue("Graphics", "ToonShading ", " " + ToonShading.ToString());
                        break;
                    case nameof(RealismShading):
                        iniFile.WriteValue("Graphics", "RealismShading ", " " + RealismShading.ToString());
                        break;
                    case nameof(OutlineForFieldCharacter):
                        iniFile.WriteValue("Graphics", "OutlineForFieldCharacter ",
                            " " + OutlineForFieldCharacter.ToString());
                        break;
                    case nameof(OutlineForBattleCharacter):
                        iniFile.WriteValue("Graphics", "OutlineForBattleCharacter ",
                            " " + OutlineForBattleCharacter.ToString());
                        break;
                }
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }

        #endregion

        private readonly String _iniPath = AppDomain.CurrentDomain.BaseDirectory + @"Memoria.ini";
        private void LoadSettings()
        {
            try
            {
                SanitizeMemoriaIni();
                IniFile iniFile = new IniFile(_iniPath);
                string value = iniFile.ReadValue("Graphics", "CustomShader");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                }
                if (!Int16.TryParse(value, out _customshader))
                    _customshader = 0;
                OnPropertyChanged(nameof(CustomShader));
                
                value = iniFile.ReadValue("Graphics", "ToonShading");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                }
                if (!Int16.TryParse(value, out _toonShading))
                    _toonShading = 0;
                OnPropertyChanged(nameof(ToonShading));

                value = iniFile.ReadValue("Graphics", "RealismShading");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                }
                if (!Int16.TryParse(value, out _realismShading))
                    _realismShading = 0;
                OnPropertyChanged(nameof(RealismShading));

                value = iniFile.ReadValue("Graphics", "OutlineForFieldCharacter");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                }
                if (!Int16.TryParse(value, out _outlineForField))
                    _outlineForField = 0;
                OnPropertyChanged(nameof(OutlineForFieldCharacter));

                value = iniFile.ReadValue("Graphics", "OutlineForBattleCharacter");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                }
                if (!Int16.TryParse(value, out _outlineForBattle))
                    _outlineForBattle = 0;
                OnPropertyChanged(nameof(OutlineForBattleCharacter));
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }
    }
}

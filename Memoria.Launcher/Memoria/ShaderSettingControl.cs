using System;
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
using ComboBox = System.Windows.Controls.ComboBox;
using Control = System.Windows.Controls.Control;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

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
            SetRows(7);
            SetCols(8);

            Width = 260;
            VerticalAlignment = VerticalAlignment.Bottom;
            HorizontalAlignment = HorizontalAlignment.Left;
            Margin = new Thickness(0);
            DataContext = this;

            Thickness rowMargin = new Thickness(0, 2, 0, 2);

            Int32 row = 0;

            UiCheckBox EnableCustomShader = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Shader_Enable, null), row, 0, 1, 8);
            EnableCustomShader.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(this.EnableCustomShader)) { Mode = BindingMode.TwoWay });
            EnableCustomShader.Foreground = Brushes.White;
            EnableCustomShader.FontWeight = FontWeights.Bold;
            EnableCustomShader.Margin = rowMargin;
            EnableCustomShader.FontSize = 14;

            row++;

            UiTextBlock separateLineField = AddUiElement(UiTextBlockFactory.Create("╙ " + Lang.Settings.Shader_Field_chars), row: row, col: 0, rowSpan: 1, colSpan: 8);
            separateLineField.Margin = rowMargin;

            row++;

            UiCheckBox EnableRealismShadingForField = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Shader_Realism, null), row, 0, 1, 4);
            EnableRealismShadingForField.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(Shader_Field_Realism)) { Mode = BindingMode.TwoWay });
            EnableRealismShadingForField.Foreground = Brushes.White;

            UiCheckBox EnableToonShadingForField = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Shader_Toon, null), row, 3, 1, 4);
            EnableToonShadingForField.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(Shader_Field_Toon)) { Mode = BindingMode.TwoWay });
            EnableToonShadingForField.Foreground = Brushes.White;

            row++;

            UiCheckBox EnableOutlineForFieldCharacter = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Shader_Outlines, null), row, 0, 1, 8);
            EnableOutlineForFieldCharacter.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(Shader_Field_Outlines)) { Mode = BindingMode.TwoWay });
            EnableOutlineForFieldCharacter.Foreground = Brushes.White;

            row++;

            UiTextBlock separateLineBattle = AddUiElement(UiTextBlockFactory.Create("╙ " + Lang.Settings.Shader_Battle_chars), row: row, col: 0, rowSpan: 1, colSpan: 8);
            separateLineBattle.Margin = rowMargin;

            row++;

            UiCheckBox EnableRealismShadingForBattle = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Shader_Realism, null), row, 0, 1, 4);
            EnableRealismShadingForBattle.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(Shader_Battle_Realism)) { Mode = BindingMode.TwoWay });
            EnableRealismShadingForBattle.Foreground = Brushes.White;

            UiCheckBox EnableToonShadingForBattle = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Shader_Toon, null), row, 3, 1, 4);
            EnableToonShadingForBattle.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(Shader_Battle_Toon)) { Mode = BindingMode.TwoWay });
            EnableToonShadingForBattle.Foreground = Brushes.White;

            row++;

            UiCheckBox EnableOutlineForBattleCharacter = AddUiElement(UiCheckBoxFactory.Create(Lang.Settings.Shader_Outlines, null), row, 0, 1, 8);
            EnableOutlineForBattleCharacter.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(Shader_Battle_Outlines)) { Mode = BindingMode.TwoWay });
            EnableOutlineForBattleCharacter.Foreground = Brushes.White;

            foreach (FrameworkElement child in Children)
            {
                //if (!ReferenceEquals(child, backround))
                //child.Margin = new Thickness(child.Margin.Left + 8, child.Margin.Top, child.Margin.Right + 8, child.Margin.Bottom);

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

        private Int16 _enableCustomShader, _toonShadingForField, _toonShadingForBattle, _realismShadingForField, _realismShadingForBattle, _outlineForField, _outlineForBattle;

        public Int16 EnableCustomShader
        {
            get { return _enableCustomShader; }
            set
            {
                if (_enableCustomShader != value)
                {
                    _enableCustomShader = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int16 Shader_Field_Realism
        {
            get { return _realismShadingForField; }
            set
            {
                if (_realismShadingForField != value)
                {
                    if (value == 1)
                    {
                        Shader_Field_Toon = 0;
                    }
                    _realismShadingForField = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int16 Shader_Field_Toon
        {
            get { return _toonShadingForField; }
            set
            {
                if (_toonShadingForField != value)
                {
                    if (value == 1)
                    {
                        Shader_Field_Realism = 0;
                    }
                    _toonShadingForField = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int16 Shader_Field_Outlines
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
        
        public Int16 Shader_Battle_Realism
        {
            get { return _realismShadingForBattle; }
            set
            {
                if (_realismShadingForBattle != value)
                {
                    if (value == 1)
                    {
                        Shader_Battle_Toon = 0;
                    }
                    _realismShadingForBattle = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public Int16 Shader_Battle_Toon
        {
            get { return _toonShadingForBattle; }
            set
            {
                if (_toonShadingForBattle != value)
                {
                    if (value == 1)
                    {
                        Shader_Battle_Realism = 0;
                    }
                    _toonShadingForBattle = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int16 Shader_Battle_Outlines
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
        
        private async void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                IniFile iniFile = new IniFile(_iniPath);
                switch (propertyName)
                {
                    case nameof(EnableCustomShader):
                        iniFile.WriteValue("Shaders", "Enabled ", " " + EnableCustomShader.ToString());
                        break;
                    case nameof(Shader_Field_Realism):
                        iniFile.WriteValue("Shaders", "Shader_Field_Realism ", " " + Shader_Field_Realism.ToString());
                        break;
                    case nameof(Shader_Field_Toon):
                        iniFile.WriteValue("Shaders", "Shader_Field_Toon ", " " + Shader_Field_Toon.ToString());
                        break;
                    case nameof(Shader_Field_Outlines):
                        iniFile.WriteValue("Shaders", "Shader_Field_Outlines ", " " + Shader_Field_Outlines.ToString());
                        break;
                    case nameof(Shader_Battle_Realism):
                        iniFile.WriteValue("Shaders", "Shader_Battle_Realism ", " " + Shader_Battle_Realism.ToString());
                        break;
                    case nameof(Shader_Battle_Toon):
                        iniFile.WriteValue("Shaders", "Shader_Battle_Toon ", " " + Shader_Battle_Toon.ToString());
                        break;
                    case nameof(Shader_Battle_Outlines):
                        iniFile.WriteValue("Shaders", "Shader_Battle_Outlines ", " " + Shader_Battle_Outlines.ToString());
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
                IniFile iniFile = new IniFile(_iniPath);
                string value = iniFile.ReadValue("Shaders", "Enabled");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                }
                if (!Int16.TryParse(value, out _enableCustomShader))
                    _enableCustomShader = 0;
                OnPropertyChanged(nameof(EnableCustomShader));

                value = iniFile.ReadValue("Shaders", "Shader_Field_Realism");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                }
                if (!Int16.TryParse(value, out _realismShadingForField))
                    _realismShadingForField = 0;
                OnPropertyChanged(nameof(Shader_Field_Realism));

                value = iniFile.ReadValue("Shaders", "Shader_Field_Toon");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                }
                if (!Int16.TryParse(value, out _toonShadingForField))
                    _toonShadingForField = 0;
                OnPropertyChanged(nameof(Shader_Field_Toon));
                
                value = iniFile.ReadValue("Shaders", "Shader_Field_Outlines");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                }
                if (!Int16.TryParse(value, out _outlineForField))
                    _outlineForField = 0;
                OnPropertyChanged(nameof(Shader_Field_Outlines));
                
                value = iniFile.ReadValue("Shaders", "Shader_Battle_Realism");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                }
                if (!Int16.TryParse(value, out _realismShadingForBattle))
                    _realismShadingForBattle = 0;
                OnPropertyChanged(nameof(Shader_Battle_Realism));

                value = iniFile.ReadValue("Shaders", "Shader_Battle_Toon");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                }
                if (!Int16.TryParse(value, out _toonShadingForBattle))
                    _toonShadingForBattle = 0;
                OnPropertyChanged(nameof(Shader_Battle_Toon));

                value = iniFile.ReadValue("Shaders", "Shader_Battle_Outlines");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                }
                if (!Int16.TryParse(value, out _outlineForBattle))
                    _outlineForBattle = 0;
                OnPropertyChanged(nameof(Shader_Battle_Outlines));
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }
    }
}

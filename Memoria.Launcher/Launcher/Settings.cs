using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Application = System.Windows.Application;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming
// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable ArrangeStaticMemberQualifier
#pragma warning disable 649
#pragma warning disable 169

namespace Memoria.Launcher
{
    public class Settings : UiGrid, INotifyPropertyChanged
    {
        #region Properties




        private Int16 _enableCustomShader;
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
        private Int16 _realismShadingForField;
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
        private Int16 _toonShadingForField;
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
        private Int16 _outlineForField;
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
        private Int16 _realismShadingForBattle;
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

        private Int16 _toonShadingForBattle;
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
        private Int16 _outlineForBattle;
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
        public void LoadSettings()
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

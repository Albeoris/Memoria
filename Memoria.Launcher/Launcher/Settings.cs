using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Application = System.Windows.Application;
using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;


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

        private String _iniPath = @"./Memoria.ini";

        private Int16 _iswidescreensupport;
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
        private Int16 _camerastabilizer;
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
        private Int16 _battleInterface;
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
        private Int16 _fpsdropboxchoice;
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
        private Int16 _uicolumnschoice;
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
        private Int16 _isskipintros;
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
        private Int16 _battleswirlframes;
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
        private Int16 _antialiasing;
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
        private Int16 _ishidecards;
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
        private Int16 _speed;
        public Int16 Speed
        {
            get { return _speed; }
            set
            {
                if (_speed != value)
                {
                    if (_speed == 3 || _speed == 4)
                        _speed = 5;
                    else
                        _speed = value;
                    OnPropertyChanged();
                }
            }
        }
        private Int16 _tripleTriad;
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
        private Int16 _usepsxfont;
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
        protected String _fontChoice;
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
        protected String _fontDefaultPC = "Final Fantasy IX PC";
        protected String _fontDefaultPSX = "Final Fantasy IX PSX";

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




        private Int16 _stealingalwaysworks;
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
        private Int16 _noautotrance;
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
        private Int16 _garnetconcentrate;
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
        private Int16 _breakDamageLimit;
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
        private Int16 _accessBattleMenu;
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
        private Int16 _speedmode;
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
        private Int16 _speedfactor;
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
        private Int16 _battletpsfactor;
        public Int16 BattleTPS
        {
            get { return _battletpsfactor; }
            set
            {
                if (_battletpsfactor != value)
                {
                    _battletpsfactor = value;
                    BattleTPSDividedBy10 = (float)Math.Round((double)value / 15, 2);
                    OnPropertyChanged();
                }
            }
        }
        public Single BattleTPSDividedBy10
        {
            get { return (float)Math.Round((double)BattleTPS / 15, 2); }
            set
            {
                OnPropertyChanged();
            }
        }
        private Int16 _battleassistance;
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
        private Int16 _norandomencounter;
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
        private Int16 _masterskill;
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
        private Int16 _maxcardcount;
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
        private Int16 _reducerandom;
        public Int16 ReduceRandom
        {
            get { return _reducerandom; }
            set
            {
                if (_reducerandom != value)
                {
                    _reducerandom = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Simple settings list
        public readonly Object[][] SettingsList =
        {
            // variable, variable_ini, category_ini, //[0 -> ?], [1 -> ?]
            
            ["WidescreenSupport", "WidescreenSupport", "Graphics"],
            ["CameraStabilizer", "CameraStabilizer", "Graphics"],
            ["SkipIntros", "SkipIntros", "Graphics"],
            ["HideCards", "HideCards", "Icons"],
            ["HideCards", "HideBeach", "Icons"],
            ["HideCards", "HideSteam", "Icons"],
            ["Speed", "Speed", "Battle"],
            ["TripleTriad", "TripleTriad", "TetraMaster"],
            ["AntiAliasing", "AntiAliasing", "Graphics"],
            ["Shader_Field_Realism", "Shader_Field_Realism", "Shaders"],
            ["Shader_Field_Toon", "Shader_Field_Toon", "Shaders"],
            ["Shader_Field_Outlines", "Shader_Field_Outlines", "Shaders"],
            ["Shader_Battle_Realism", "Shader_Battle_Realism", "Shaders"],
            ["Shader_Battle_Toon", "Shader_Battle_Toon", "Shaders"],
            ["Shader_Battle_Outlines", "Shader_Battle_Outlines", "Shaders"],
            ["NoAutoTrance", "NoAutoTrance", "Battle"],
            ["GarnetConcentrate", "GarnetConcentrate", "Battle"],
            ["BreakDamageLimit", "BreakDamageLimit", "Battle"],
            ["SpeedMode", "SpeedMode", "Cheats"],
            ["SpeedFactor", "SpeedFactor", "Cheats"],
            ["BattleTPS", "BattleTPS", "Graphics"],
            ["BattleAssistance", "BattleAssistance", "Cheats"],
            ["BattleAssistance", "Attack9999", "Cheats"],
            ["NoRandomEncounter", "NoRandomEncounter", "Cheats"],
            ["MasterSkill", "MasterSkill", "Cheats"],
            ["MasterSkill", "LvMax", "Cheats"],
            ["MasterSkill", "GilMax", "Cheats"],
            ["ReduceRandom", "ReduceRandom", "TetraMaster"],

        };
        #endregion

        #region Write ini
        public event PropertyChangedEventHandler PropertyChanged;
        private async void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                IniFile iniFile = new IniFile(_iniPath);

                foreach (Object[] item in SettingsList) {
                    if (item[0] is String property && property == propertyName && item[1] is String name_ini && item[2] is String category)
                    {
                        Object varValue = this.GetType().GetProperty(property)?.GetValue(this);
                        //MessageBox.Show($"{varValue}", "debug", MessageBoxButtons.OK);
                        if (varValue != null)
                        {
                            iniFile.WriteValue(category, name_ini + " ", " " + varValue);
                            if (varValue.ToString() != "0")
                                iniFile.WriteValue(category, "Enabled ", " 1");
                        }
                    }
                }
                
                switch (propertyName)
                {
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
                    case nameof(AccessBattleMenu):
                        iniFile.WriteValue("Battle", "AccessMenus ", " " + AccessBattleMenu);
                        iniFile.WriteValue("Battle", "AvailableMenus ", AvailableBattleMenus);
                        if (AccessBattleMenu > 0)
                            iniFile.WriteValue("Battle", "Enabled ", " 1");
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
                    case nameof(ReduceRandom):
                        if (ReduceRandom != 0)
                            iniFile.WriteValue("TetraMaster", "Enabled ", " 1");
                        iniFile.WriteValue("TetraMaster", propertyName + " ", " " + ReduceRandom);
                        break;
                }
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }

        #endregion

        #region LoadSettings

        public readonly Object[][] SettingsList2 =
        {
            // variable, variable_ini, category_ini
            ["WidescreenSupport", "WidescreenSupport", "Graphics"],
        };

        public void LoadSettings()
        {
            try
            {
                IniFile iniFile = new(_iniPath);

                /*foreach (Object[] item in SettingsList2)
                {
                    if (item[0] is String name && item[1] is String name_ini && item[2] is String category)
                    {
                        
                    }
                }*/



                String value = iniFile.ReadValue("Graphics", nameof(WidescreenSupport));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 1";
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
                }
                if (!Int16.TryParse(value, out _camerastabilizer))
                    _camerastabilizer = 85;

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

                value = iniFile.ReadValue("Shaders", "Shader_Field_Realism");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                }
                if (!Int16.TryParse(value, out _realismShadingForField))
                    _realismShadingForField = 0;

                value = iniFile.ReadValue("Shaders", "Shader_Field_Toon");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                }
                if (!Int16.TryParse(value, out _toonShadingForField))
                    _toonShadingForField = 0;

                value = iniFile.ReadValue("Shaders", "Shader_Field_Outlines");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                }
                if (!Int16.TryParse(value, out _outlineForField))
                    _outlineForField = 0;

                value = iniFile.ReadValue("Shaders", "Shader_Battle_Realism");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                }
                if (!Int16.TryParse(value, out _realismShadingForBattle))
                    _realismShadingForBattle = 0;

                value = iniFile.ReadValue("Shaders", "Shader_Battle_Toon");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                }
                if (!Int16.TryParse(value, out _toonShadingForBattle))
                    _toonShadingForBattle = 0;

                value = iniFile.ReadValue("Shaders", "Shader_Battle_Outlines");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                }
                if (!Int16.TryParse(value, out _outlineForBattle))
                    _outlineForBattle = 0;


                value = iniFile.ReadValue("Hacks", nameof(StealingAlwaysWorks));
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
                }
                if (!Int16.TryParse(value, out _speedfactor))
                    _speedfactor = 2;
                Refresh(nameof(SpeedFactor));

                value = iniFile.ReadValue("Graphics", " " + nameof(BattleTPS));
                if (String.IsNullOrEmpty(value))
                {
                    _battletpsfactor = 15;
                    value = " 15";
                    OnPropertyChanged(nameof(BattleTPS));
                }
                if (!Int16.TryParse(value, out _battletpsfactor))
                    _battletpsfactor = 15;
                Boolean valueexists = Single.TryParse(value, out Single decvalue);
                BattleTPSDividedBy10 = valueexists ? decvalue / 15f : 1.5f;

                value = iniFile.ReadValue("Cheats", nameof(BattleAssistance));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(BattleAssistance));
                }
                if (!Int16.TryParse(value, out _battleassistance))
                    _battleassistance = 0;

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

                value = iniFile.ReadValue("TetraMaster", nameof(MaxCardCount));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 100";
                    OnPropertyChanged(nameof(MaxCardCount));
                }
                Int16 cardCount = 100;
                if (!Int16.TryParse(value, out cardCount))
                    _maxcardcount = 0;
                else
                    _maxcardcount = cardCount < 9999 ? (Int16)0 : (Int16)1;


                value = iniFile.ReadValue("TetraMaster", nameof(ReduceRandom));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(ReduceRandom));
                }
                if (!Int16.TryParse(value, out _reducerandom))
                    _reducerandom = 0;




                foreach (Object[] item in SettingsList)
                {
                    if (item[0] is String property)
                    {
                        Refresh(property);
                    }
                }

                Refresh(nameof(BattleInterface));
                Refresh(nameof(UIColumnsChoice));
                Refresh(nameof(FPSDropboxChoice));
                Refresh(nameof(BattleSwirlFrames));
                Refresh(nameof(UsePsxFont));
                Refresh(nameof(StealingAlwaysWorks));
                Refresh(nameof(AccessBattleMenu));
                Refresh(nameof(MaxCardCount));
                
            }
            catch (Exception ex)
            {
                UiHelper.ShowError(Application.Current.MainWindow, ex);
            }
        }
        #endregion

        #region Refresh
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

        #endregion

    }
}

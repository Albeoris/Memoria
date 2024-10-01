using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Application = System.Windows.Application;
using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Memoria.Launcher
{
    public class Settings : UiGrid, INotifyPropertyChanged
    {
        public Settings()
        {
            SetCols(8);
            Width = 293;
            Margin = new Thickness(0);
            DataContext = this;
        }
        #region Prop checkbox

        private readonly String _iniPath = @"./Memoria.ini";

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
        private Int16 _isskipintros;
        public Int16 SkipIntros
        {
            get { return _isskipintros; }
            set
            {
                if (_isskipintros != value)
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
        private Int16 _worldmapboost;
        public Int16 WorldmapBoost
        {
            get { return _worldmapboost; }
            set
            {
                if (_worldmapboost != value)
                {
                    _worldmapboost = value;
                    OnPropertyChanged();
                }
            }
        }
        private Int16 _worldmapshiptilt;
        public Int16 WorldmapShipTilt
        {
            get { return _worldmapshiptilt; }
            set
            {
                if (_worldmapshiptilt != value)
                {
                    _worldmapshiptilt = value;
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
        private Int16 _viviautoattack;
        public Int16 ViviAutoAttack
        {
            get { return _viviautoattack; }
            set
            {
                if (_viviautoattack != value)
                {
                    _viviautoattack = value;
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
        private Int16 _accessbattlemenutoggle;
        public Int16 AccessBattleMenuToggle
        {
            get { return _accessbattlemenutoggle; }
            set
            {
                if (_accessbattlemenutoggle != value)
                {
                    _accessbattlemenutoggle = value;
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
        private Int16 _attack9999;
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
        private Int16 _easytetramaster;
        public Int16 EasyTetraMaster
        {
            get { return _easytetramaster; }
            set
            {
                if (_easytetramaster != value)
                {
                    _easytetramaster = value;
                    OnPropertyChanged();
                }
            }
        }
        private Int16 _excaliburiinotimelimit;
        public Int16 ExcaliburIINoTimeLimit
        {
            get { return _excaliburiinotimelimit; }
            set
            {
                if (_excaliburiinotimelimit != value)
                {
                    _excaliburiinotimelimit = value;
                    OnPropertyChanged();
                }
            }
        }
        private Int16 _easyjumpropeminigame;
        public Int16 EasyJumpRopeMinigame
        {
            get { return _easyjumpropeminigame; }
            set
            {
                if (_easyjumpropeminigame != value)
                {
                    _easyjumpropeminigame = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Prop sliders
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
        private Int16 _worldmapfov;
        public Int16 WorldmapFOV
        {
            get { return _worldmapfov; }
            set
            {
                if (_worldmapfov != value)
                {
                    _worldmapfov = value;
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
                    if (_speedfactor == 0)
                        _speedmode = 0;
                    else
                        _speedmode = 1;
                    _speedfactor = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Prop other

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
        private Int16 _shaderfieldchoice;
        public Int16 ShaderFieldChoice
        {
            get { return _shaderfieldchoice; }
            set
            {
                if (_shaderfieldchoice != value)
                {
                    _shaderfieldchoice = value;
                    OnPropertyChanged();
                }
            }
        }
        private Int16 _shaderbattlechoice;
        public Int16 ShaderBattleChoice
        {
            get { return _shaderbattlechoice; }
            set
            {
                if (_shaderbattlechoice != value)
                {
                    _shaderbattlechoice = value;
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
        private Int16 _atbmodechoice;
        public Int16 ATBModeChoice
        {
            get { return _atbmodechoice; }
            set
            {
                if (_atbmodechoice != value)
                {
                    _atbmodechoice = value;
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

        private Int16 _worldmapmistpreset;
        public Int16 WorldmapMistPreset
        {
            get { return _worldmapmistpreset; }
            set
            {
                if (_worldmapmistpreset != value)
                {
                    _worldmapmistpreset = value;
                    OnPropertyChanged();
                }
            }
        }
        private Int16 _worldmapdistancepreset;
        public Int16 WorldmapDistancePreset
        {
            get { return _worldmapdistancepreset; }
            set
            {
                if (_worldmapdistancepreset != value)
                {
                    _worldmapdistancepreset = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Simple settings list
        public readonly Object[][] SettingsList =
        {
            // Variable, _variable, variable_ini, category_ini, [0 -> ?], [1 -> ?], default value
            
            // Checkboxes
            ["WidescreenSupport", "_iswidescreensupport", "WidescreenSupport", "Graphics", 0, 1],
            ["AntiAliasing", "_antialiasing", "AntiAliasing", "Graphics", 0, 1],
            ["SkipIntros", "_isskipintros", "SkipIntros", "Graphics", 0, 3],
            ["BattleSwirlFrames", "_battleswirlframes", "BattleSwirlFrames", "Graphics", 70, 0],
            ["MaxCardCount", "_maxcardcount", "MaxCardCount", "TetraMaster", 100, 1000],
            ["HideCards", "_ishidecards", "HideCards", "Icons", 0, 1],
            ["HideCards", "_ishidecards", "HideBeach", "Icons", 0, 1],
            ["HideCards", "_ishidecards", "HideSteam", "Icons", 0, 1],
            ["WorldmapBoost", "_worldmapboost", "FieldOfViewSpeedBoost", "Worldmap", 0, 100],
            ["WorldmapShipTilt", "_worldmapshiptilt", "CameraTiltShip", "Worldmap", 0, 100],

            ["StealingAlwaysWorks", "_stealingalwaysworks", "StealingAlwaysWorks", "Hacks", 0, 2],
            ["NoAutoTrance", "_noautotrance", "NoAutoTrance", "Battle", 0, 1],
            ["ViviAutoAttack", "_viviautoattack", "ViviAutoAttack", "Battle", 0, 1],
            ["BreakDamageLimit", "_breakDamageLimit", "BreakDamageLimit", "Battle", 0, 1],
            ["AccessBattleMenuToggle", "_accessbattlemenutoggle", "AccessMenus", "Battle", 0, 3],
            ["GarnetConcentrate", "_garnetconcentrate", "GarnetConcentrate", "Battle", 0, 1],

            //["SpeedMode", "_speedmode", "SpeedMode", "Cheats", 0, 1],
            ["BattleAssistance", "_battleassistance", "BattleAssistance", "Cheats", 0, 1],
            ["Attack9999", "_attack9999", "Attack9999", "Cheats", 0, 1],
            ["NoRandomEncounter", "_norandomencounter", "NoRandomEncounter", "Cheats", 0, 1],
            ["MasterSkill", "_masterskill", "MasterSkill", "Cheats", 0, 1],
            ["MasterSkill", "_masterskill", "LvMax", "Cheats", 0, 1],
            ["MasterSkill", "_masterskill", "GilMax", "Cheats", 0, 1],

            ["EasyTetraMaster", "_easytetramaster", "EasyWin", "TetraMaster", 0, 1],
            ["ExcaliburIINoTimeLimit", "_excaliburiinotimelimit", "ExcaliburIINoTimeLimit", "Hacks", 0, 1],
            ["EasyJumpRopeMinigame", "_easyjumpropeminigame", "RopeJumpingIncrement", "Hacks", 1, 1000],

            // Sliders
            ["CameraStabilizer", "_camerastabilizer", "CameraStabilizer", "Graphics", 0, 1],
            ["BattleTPS", "_battletpsfactor", "BattleTPS", "Graphics", 0, 1],
            ["WorldmapFOV", "_worldmapfov", "FieldOfView", "Worldmap", 0, 1],
            //["SpeedFactor", "_speedfactor", "SpeedFactor", "Cheats", 0, 1],
        };
        #endregion

        #region Write ini
        public event PropertyChangedEventHandler PropertyChanged;
        private async void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                IniFile iniFile = new IniFile(IniFile.IniPath);

                foreach (Object[] item in SettingsList) {
                    if (item[0] is String property && property == propertyName && item[2] is String name_ini && item[3] is String category && item[4] is Int32 valueZero && item[5] is Int32 valueOne) //  
                    {
                        Object propValue = this.GetType().GetProperty(property)?.GetValue(this);
                        //MessageBox.Show($"{propValue}", "debug", MessageBoxButtons.OK);
                        if (propValue != null && Int16.TryParse(propValue.ToString(), out Int16 varValue))
                        {
                            if (varValue == 0)
                            {
                                iniFile.WriteValue(category, name_ini + " ", " " + valueZero);
                            }
                            else if (varValue == 1)
                            {
                                iniFile.WriteValue(category, name_ini + " ", " " + valueOne);
                                iniFile.WriteValue(category, "Enabled ", " 1");
                            }
                            else
                            {
                                iniFile.WriteValue(category, name_ini + " ", " " + varValue);
                                iniFile.WriteValue(category, "Enabled ", " 1");
                            }
                        }
                    }
                }
                Int32 var0 = 0; Int32 var1 = 0; Int32 var2 = 0; Int32 var3 = 0; Int32 var4 = 0; Int32 var5 = 0;
                switch (propertyName)
                {
                    case nameof(SpeedFactor):
                        var0 = SpeedFactor;
                        iniFile.WriteValue("Cheats", "SpeedFactor ", " " + var0);
                        if (var0 < 2)
                            iniFile.WriteValue("Cheats", "SpeedMode ", " 0");
                        else
                        {
                            iniFile.WriteValue("Cheats", "SpeedMode ", " 1");
                            iniFile.WriteValue("Cheats", "Enabled ", " 1");
                        }
                        break;
                    case nameof(FPSDropboxChoice):
                        var0 = FPSDropboxChoice;
                        if (var0 >= 0)
                        {
                            iniFile.WriteValue("Graphics", "Enabled ", " 1");
                            if (var0 == 0) { var1 = 30; var2 = 15; var3 = 20; }
                            if (var0 == 1) { var1 = 30; var2 = 30; var3 = 30; }
                            if (var0 == 2) { var1 = 60; var2 = 60; var3 = 60; }
                            if (var0 == 3) { var1 = 90; var2 = 90; var3 = 90; }
                            if (var0 == 4) { var1 = 120; var2 = 120; var3 = 120; }
                            iniFile.WriteValue("Graphics", "FieldFPS ", " " + var1);
                            iniFile.WriteValue("Graphics", "BattleFPS ", " " + var2);
                            iniFile.WriteValue("Graphics", "WorldFPS ", " " + var3);
                        }
                        break;
                    case nameof(ShaderFieldChoice):
                        var0 = ShaderFieldChoice;
                        if (var0 >= 0)
                        {
                            iniFile.WriteValue("Shaders", "Enabled ", " 1");
                            if (var0 == 1) { var2 = 1; var3 = 1; }
                            if (var0 == 2) { var2 = 1; }
                            if (var0 == 3) { var1 = 1; var3 = 1; }
                            if (var0 == 4) { var1 = 1; }
                            if (var0 == 5) { var3 = 1; }
                            iniFile.WriteValue("Shaders", "Shader_Field_Realism ", " " + var1);
                            iniFile.WriteValue("Shaders", "Shader_Field_Toon ", " " + var2);
                            iniFile.WriteValue("Shaders", "Shader_Field_Outlines ", " " + var3);
                        }
                        break;
                    case nameof(ShaderBattleChoice):
                        var0 = ShaderBattleChoice;
                        if (var0 >= 0)
                        {
                            iniFile.WriteValue("Shaders", "Enabled ", " 1");
                            if (var0 == 1) { var2 = 1; var3 = 1; }
                            if (var0 == 2) { var2 = 1; }
                            if (var0 == 3) { var1 = 1; var3 = 1; }
                            if (var0 == 4) { var1 = 1; }
                            if (var0 == 5) { var3 = 1; }
                            iniFile.WriteValue("Shaders", "Shader_Battle_Realism ", " " + var1);
                            iniFile.WriteValue("Shaders", "Shader_Battle_Toon ", " " + var2);
                            iniFile.WriteValue("Shaders", "Shader_Battle_Outlines ", " " + var3);
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
                    case nameof(UIColumnsChoice):
                        var0 = UIColumnsChoice;
                        if (var0 >= 0)
                        {
                            iniFile.WriteValue("Interface", "Enabled ", " 1");
                            if (var0 == 0) { var1 = 8; var2 = 6; var3 = 5; }
                            if (var0 == 1) { var1 = 12; var2 = 9; var3 = 7; }
                            if (var0 == 2) { var1 = 16; var2 = 12; var3 = 8; }
                            iniFile.WriteValue("Interface", "MenuItemRowCount ", " " + var1);
                            iniFile.WriteValue("Interface", "MenuAbilityRowCount ", " " + var2);
                            iniFile.WriteValue("Interface", "MenuEquipRowCount ", " " + var3);
                            iniFile.WriteValue("Interface", "MenuChocographRowCount ", " " + var3);
                        }
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
                    case nameof(ATBModeChoice):
                        if (ATBModeChoice != 0)
                            iniFile.WriteValue("Battle", "Enabled ", " 1");
                        if (ATBModeChoice == 3)
                            iniFile.WriteValue("Battle", "Speed ", " 5");
                        else
                            iniFile.WriteValue("Battle", "Speed ", " " + ATBModeChoice);
                        break;
                    case nameof(TripleTriad):
                        var0 = TripleTriad;
                        if (var0 >= 0)
                        {
                            iniFile.WriteValue("TetraMaster", "Enabled ", " 1");
                            if (var0 == 0) { var1 = 0; var2 = 0; }
                            if (var0 == 1) { var1 = 0; var2 = 1; }
                            if (var0 == 2) { var1 = 1; var2 = 0; }
                            if (var0 == 3) { var1 = 2; var2 = 0; }
                            iniFile.WriteValue("TetraMaster", "TripleTriad ", " " + var1);
                            iniFile.WriteValue("TetraMaster", "ReduceRandom ", " " + var2);
                        }
                        break;
                    case nameof(WorldmapMistPreset):
                        var0 = WorldmapMistPreset;
                        if (var0 >= 0)
                        {
                            iniFile.WriteValue("Worldmap", "Enabled ", " 1");
                            if (var0 == 0) { var1 = 100; var2 = 55; var3 = 27; var4 = 80;  var5 = 7; }
                            if (var0 == 1) { var1 = 250; var2 = 10; var3 = 30; var4 = 200; var5 = 7; }
                            if (var0 == 2) { var1 = 400; var2 = 10; var3 = 50; var4 = 300; var5 = 4; }
                            if (var0 == 3) { var1 = 450; var2 = 0; var3 = 250; var4 = 330; var5 = 0; }
                            iniFile.WriteValue("Worldmap", "MistViewDistance ", " " + var1);
                            iniFile.WriteValue("Worldmap", "MistStartDistance_base ", " " + var2);
                            iniFile.WriteValue("Worldmap", "MistStartDistance ", " " + var3);
                            iniFile.WriteValue("Worldmap", "MistEndDistance ", " " + var4);
                            iniFile.WriteValue("Worldmap", "MistDensity ", " " + var5);
                        }
                        break;
                    case nameof(WorldmapDistancePreset):
                        var0 = WorldmapDistancePreset;
                        if (var0 >= 0)
                        {
                            iniFile.WriteValue("Worldmap", "Enabled ", " 1");
                            if (var0 == 0) { var1 = 100; var2 = 86; var3 = 142; }
                            if (var0 == 1) { var1 = 200; var2 = 150; var3 = 250; }
                            if (var0 == 2) { var1 = 300; var2 = 250; var3 = 350; }
                            if (var0 == 3) { var1 = 450; var2 = 400; var3 = 490; }
                            iniFile.WriteValue("Worldmap", "NoMistViewDistance ", " " + var1);
                            iniFile.WriteValue("Worldmap", "FogStartDistance ", " " + var2);
                            iniFile.WriteValue("Worldmap", "FogEndDistance ", " " + var3);
                        }
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
                IniFile iniFile = new(IniFile.IniPath);

                /*foreach (Object[] item in SettingsList2)
                {
                    if (item[0] is String name && item[1] is String name_ini && item[2] is String category)
                    {
                        
                    }
                }*/

                String value;
                Boolean value1isInt;
                Boolean value2isInt;
                Boolean value3isInt;
                Boolean value4isInt;
                Boolean value5isInt;
                Int16 value1;
                Int16 value2;
                Int16 value3;
                Int16 value4;
                Int16 value5;

                value = iniFile.ReadValue("Graphics", nameof(WidescreenSupport));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 1";
                }
                if (!Int16.TryParse(value, out _iswidescreensupport))
                    _iswidescreensupport = 1;

                value = iniFile.ReadValue("Graphics", "FieldFPS");
                value1isInt = Int16.TryParse(value, out value1);
                value = iniFile.ReadValue("Graphics", "BattleFPS");
                value2isInt = Int16.TryParse(value, out value2);
                value = iniFile.ReadValue("Graphics", "WorldFPS");
                value3isInt = Int16.TryParse(value, out value3);
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
                value1isInt = Int16.TryParse(value, out value1);
                if (value1 == 0)
                    _isskipintros = 0;
                else
                    _isskipintros = 1;

                value = iniFile.ReadValue("Interface", "MenuItemRowCount");
                value1isInt = Int16.TryParse(value, out value1);
                value = iniFile.ReadValue("Interface", "MenuAbilityRowCount");
                value2isInt = Int16.TryParse(value, out value2);
                value = iniFile.ReadValue("Interface", "MenuEquipRowCount");
                value3isInt = Int16.TryParse(value, out value3);
                value = iniFile.ReadValue("Interface", "MenuChocographRowCount");
                value4isInt = Int16.TryParse(value, out value4);
                if (value1 == 8 && value2 == 6 && value3 == 5 && value4 == 5)
                    _uicolumnschoice = 0;
                else if (value1 == 12 && value2 == 9 && value3 == 7 && value4 == 7)
                    _uicolumnschoice = 1;
                else if (value1 == 16 && value2 == 12 && value3 == 8 && value4 == 8)
                    _uicolumnschoice = 2;
                else
                    _uicolumnschoice = -1;

                value = iniFile.ReadValue("Graphics", nameof(BattleSwirlFrames));
                if (String.IsNullOrEmpty(value))
                {
                    value = " 1";
                    OnPropertyChanged(nameof(BattleSwirlFrames));
                }
                String newvalue = (Int16.Parse(value) == 0) ? "1" : "0";
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

                value = iniFile.ReadValue("Battle", "Speed");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged("ATBModeChoice");
                }
                if (!Int16.TryParse(value, out _atbmodechoice))
                    _atbmodechoice = 0;
                else if (_atbmodechoice > 3)
                    _atbmodechoice = 3;

                value = iniFile.ReadValue("TetraMaster", "TripleTriad");
                value1isInt = Int16.TryParse(value, out value1);
                value = iniFile.ReadValue("TetraMaster", "ReduceRandom");
                value2isInt = Int16.TryParse(value, out value2);
                if (value1 == 0 && value2 == 0)
                    _tripleTriad = 0;
                else if (value1 == 0 && value2 > 0)
                    _tripleTriad = 1;
                else if (value1 == 1)
                    _tripleTriad = 2;
                else if (value1 == 2)
                    _tripleTriad = 3;
                else
                    _tripleTriad = -1;

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
                value1isInt = Int16.TryParse(value, out value1);
                value = iniFile.ReadValue("Shaders", "Shader_Field_Toon");
                value2isInt = Int16.TryParse(value, out value2);
                value = iniFile.ReadValue("Shaders", "Shader_Field_Outlines");
                value3isInt = Int16.TryParse(value, out value3);
                if (value1 == 0 && value2 == 0 && value3 == 0)
                    _shaderfieldchoice = 0;
                else if (value1 == 0 && value2 == 1 && value3 == 1)
                    _shaderfieldchoice = 1;
                else if (value1 == 0 && value2 == 1 && value3 == 0)
                    _shaderfieldchoice = 2;
                else if (value1 == 1 && value2 == 0 && value3 == 1)
                    _shaderfieldchoice = 3;
                else if (value1 == 1 && value2 == 0 && value3 == 0)
                    _shaderfieldchoice = 4;
                else if (value1 == 0 && value2 == 0 && value3 == 1)
                    _shaderfieldchoice = 5;
                else
                    _shaderfieldchoice = -1;

                value = iniFile.ReadValue("Shaders", "Shader_Battle_Realism");
                value1isInt = Int16.TryParse(value, out value1);
                value = iniFile.ReadValue("Shaders", "Shader_Battle_Toon");
                value2isInt = Int16.TryParse(value, out value2);
                value = iniFile.ReadValue("Shaders", "Shader_Battle_Outlines");
                value3isInt = Int16.TryParse(value, out value3);
                if (value1 == 0 && value2 == 0 && value3 == 0)
                    _shaderbattlechoice = 0;
                else if (value1 == 0 && value2 == 1 && value3 == 1)
                    _shaderbattlechoice = 1;
                else if (value1 == 0 && value2 == 1 && value3 == 0)
                    _shaderbattlechoice = 2;
                else if (value1 == 1 && value2 == 0 && value3 == 1)
                    _shaderbattlechoice = 3;
                else if (value1 == 1 && value2 == 0 && value3 == 0)
                    _shaderbattlechoice = 4;
                else if (value1 == 0 && value2 == 0 && value3 == 1)
                    _shaderbattlechoice = 5;
                else
                    _shaderbattlechoice = -1;

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

                value = iniFile.ReadValue("TetraMaster", "MaxCardCount");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 100";
                    OnPropertyChanged(nameof(MaxCardCount));
                }
                value1isInt = Int16.TryParse(value, out value1);
                if (value1 == 100)
                    _maxcardcount = 0;
                else
                    _maxcardcount = 1;

                value = iniFile.ReadValue("TetraMaster", "EasyWin");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(EasyTetraMaster));
                }
                if (!Int16.TryParse(value, out _easytetramaster))
                    _easytetramaster = 0;


                value = iniFile.ReadValue("Worldmap", "MistViewDistance");
                value1isInt = Int16.TryParse(value, out value1);
                value = iniFile.ReadValue("Worldmap", "MistStartDistance_base");
                value2isInt = Int16.TryParse(value, out value2);
                value = iniFile.ReadValue("Worldmap", "MistStartDistance");
                value3isInt = Int16.TryParse(value, out value3);
                value = iniFile.ReadValue("Worldmap", "MistEndDistance");
                value4isInt = Int16.TryParse(value, out value4);
                value = iniFile.ReadValue("Worldmap", "MistDensity");
                value5isInt = Int16.TryParse(value, out value5);
                if (value1 == 100 && value2 == 55 && value3 == 27 && value4 == 80 && value5 == 7)
                    _worldmapmistpreset = 0;
                else if (value1 == 250 && value2 == 10 && value3 == 30 && value4 == 200 && value5 == 7)
                    _worldmapmistpreset = 1;
                else if (value1 == 400 && value2 == 10 && value3 == 50 && value4 == 300 && value5 == 4)
                    _worldmapmistpreset = 2;
                else if (value1 == 450 && value2 == 0 && value3 == 250 && value4 == 330 && value5 == 0)
                    _worldmapmistpreset = 3;
                else
                    _worldmapmistpreset = -1;

                value = iniFile.ReadValue("Worldmap", "NoMistViewDistance");
                value1isInt = Int16.TryParse(value, out value1);
                value = iniFile.ReadValue("Worldmap", "FogStartDistance");
                value2isInt = Int16.TryParse(value, out value2);
                value = iniFile.ReadValue("Worldmap", "FogEndDistance");
                value3isInt = Int16.TryParse(value, out value3);
                if (value1 == 100 && value2 == 86 && value3 == 142)
                    _worldmapdistancepreset = 0;
                else if (value1 == 200 && value2 == 150 && value3 == 250)
                    _worldmapdistancepreset = 1;
                else if (value1 == 300 && value2 == 250 && value3 == 350)
                    _worldmapdistancepreset = 2;
                else if (value1 == 450 && value2 == 400 && value3 == 490)
                    _worldmapdistancepreset = 3;
                else
                    _worldmapdistancepreset = -1;

                value = iniFile.ReadValue("Worldmap", "FieldOfView");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(WorldmapFOV));
                }
                if (!Int16.TryParse(value, out _worldmapfov))
                    _worldmapfov = 0;

                value = iniFile.ReadValue("Worldmap", "FieldOfViewSpeedBoost");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 100";
                    OnPropertyChanged(nameof(WorldmapBoost));
                }
                value1isInt = Int16.TryParse(value, out value1);
                if (value1 == 0)
                    _worldmapboost = 0;
                else
                    _worldmapboost = 1;

                value = iniFile.ReadValue("Worldmap", "CameraTiltShip");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 100";
                    OnPropertyChanged(nameof(WorldmapShipTilt));
                }
                value1isInt = Int16.TryParse(value, out value1);
                if (value1 == 0)
                    _worldmapshiptilt = 0;
                else
                    _worldmapshiptilt = 1;

                value = iniFile.ReadValue("Battle", "ViviAutoAttack");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(ViviAutoAttack));
                }
                if (!Int16.TryParse(value, out _viviautoattack))
                    _viviautoattack = 0;

                value = iniFile.ReadValue("Hacks", "ExcaliburIINoTimeLimit");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(ExcaliburIINoTimeLimit));
                }
                if (!Int16.TryParse(value, out _excaliburiinotimelimit))
                    _excaliburiinotimelimit = 0;

                value = iniFile.ReadValue("Battle", "AccessMenus");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 0";
                    OnPropertyChanged(nameof(AccessBattleMenuToggle));
                }
                value1isInt = Int16.TryParse(value, out value1);
                if (value1 == 0)
                    _accessbattlemenutoggle = 0;
                else
                    _accessbattlemenutoggle = 1;

                value = iniFile.ReadValue("Hacks", "RopeJumpingIncrement");
                if (String.IsNullOrEmpty(value))
                {
                    value = " 1";
                    OnPropertyChanged(nameof(EasyJumpRopeMinigame));
                }
                value1isInt = Int16.TryParse(value, out value1);
                if (value1 == 1)
                    _easyjumpropeminigame = 0;
                else
                    _easyjumpropeminigame = 1;

                foreach (Object[] item in SettingsList)
                {
                    if (item[0] is String property)
                    {
                        Refresh(property);
                    }
                }
                Refresh(nameof(ShaderFieldChoice));
                Refresh(nameof(ShaderBattleChoice));
                Refresh(nameof(BattleInterface));
                Refresh(nameof(UIColumnsChoice));
                Refresh(nameof(FPSDropboxChoice));
                Refresh(nameof(UsePsxFont));
                Refresh(nameof(WorldmapMistPreset));
                Refresh(nameof(WorldmapDistancePreset));
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

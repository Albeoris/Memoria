using System;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace Memoria.Launcher
{
    public sealed partial class Lang
    {
        public static String LangName = "en";

        #region Lazy

        private static readonly Lazy<Lang> Instance = new Lazy<Lang>(Initialize, true);

        private static Lang Initialize()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                XmlElement def = XmlHelper.LoadEmbadedDocument(assembly, $"Languages.en.xml");
                XmlElement cur = null;

                IniFile iniFile = new IniFile(SettingsGrid_Vanilla.IniPath);
                String forcedLang = iniFile.ReadValue("Memoria", "LauncherLanguage");

                String[] fileNames = String.IsNullOrEmpty(forcedLang) ?
                    new String[] { CultureInfo.CurrentCulture.Name, CultureInfo.CurrentCulture.TwoLetterISOLanguageName, CultureInfo.CurrentCulture.ThreeLetterISOLanguageName } :
                    new String[] { forcedLang, CultureInfo.CurrentCulture.Name, CultureInfo.CurrentCulture.TwoLetterISOLanguageName, CultureInfo.CurrentCulture.ThreeLetterISOLanguageName, };
                foreach (String name in fileNames)
                {
                    cur = XmlHelper.LoadEmbadedDocument(assembly, $"Languages.{name}.xml");
                    if (cur != null)
                    {
                        LangName = name;
                        break;
                    }
                }
                //File.AppendAllText("MBROutput2.txt", name + \n");
                return new Lang(def, cur ?? def);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to initialize Lang Data.");
                Console.WriteLine(ex);
                Console.ReadLine();
                Environment.Exit(1);
            }
            return null;
        }

        #endregion

        #region Instance

        private readonly XmlElement _default;
        private readonly XmlElement _current;

        private Lang(XmlElement def, XmlElement cur)
        {
            _default = def ?? throw new ArgumentNullException(nameof(def));
            _current = cur ?? throw new ArgumentNullException(nameof(cur));
        }

        public string GetString(string arg, params string[] path)
        {
            if (arg == null) throw new ArgumentNullException(nameof(arg));
            if (arg == String.Empty) throw new ArgumentException(nameof(arg));

            try
            {
                string str = FindString(_current, arg, path);
                if (str != null)
                    return str;
                Console.WriteLine("String is not found in current dictionary: {0}.{1}", string.Join(".", path), arg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("String is not found in current dictionary: {0}.{1}", string.Join(".", path), arg);
                Console.WriteLine(ex);
            }

            try
            {
                string str = FindString(_default, arg, path);
                if (str != null)
                    return str;
                Console.WriteLine("String is not found in current dictionary: {0}.{1}", string.Join(".", path), arg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("String is not found in default dictionary: {0}.{1}", string.Join(".", path), arg);
                Console.WriteLine(ex);
            }

            return "#StringNotFound#";
        }

        private string FindString(XmlElement root, string arg, params string[] path)
        {
            XmlElement result = root;
            foreach (string s in path)
            {
                result = result[s];
                if (result == null)
                    break;
            }

            return result?.FindString(arg);
        }

        #endregion
    }

    public sealed partial class Lang
    {
        public static class Measurement
        {
            private static string GetMeasurement(string name)
            {
                return Instance.Value.GetString(name, "Measurement");
            }

            public static readonly string Elapsed = GetMeasurement("Elapsed");
            public static readonly string Remaining = GetMeasurement("Remaining");
            public static readonly string SecondAbbr = GetMeasurement("SecondAbbr");
            public static readonly string ByteAbbr = GetMeasurement("ByteAbbr");
            public static readonly string KByteAbbr = GetMeasurement("KByteAbbr");
            public static readonly string MByteAbbr = GetMeasurement("MByteAbbr");
            public static readonly string GByteAbbr = GetMeasurement("GByteAbbr");
            public static readonly string TByteAbbr = GetMeasurement("TByteAbbr");
            public static readonly string PByteAbbr = GetMeasurement("PByteAbbr");
            public static readonly string EByteAbbr = GetMeasurement("EByteAbbr");
        }

        public static class Settings
        {
            private static String GetSettings(string name)
            {
                return Instance.Value.GetString(name, nameof(Settings));
            }

            public static readonly string LauncherWindowTitle = GetSettings(nameof(LauncherWindowTitle));
            public static readonly string MemoriaEngine = GetSettings(nameof(MemoriaEngine));

            public static readonly string ActiveMonitor = GetSettings(nameof(ActiveMonitor));
            public static readonly string PrimaryMonitor = GetSettings(nameof(PrimaryMonitor));
            public static readonly string Resolution = GetSettings(nameof(Resolution));
            public static readonly string WindowMode = GetSettings(nameof(WindowMode));
            public static readonly string Window = GetSettings(nameof(Window));
            public static readonly string ExclusiveFullscreen = GetSettings(nameof(ExclusiveFullscreen));
            public static readonly string BorderlessFullscreen = GetSettings(nameof(BorderlessFullscreen));
            public static readonly string Debuggable = GetSettings(nameof(Debuggable));
            public static readonly string CheckUpdates = GetSettings(nameof(CheckUpdates));
            public static readonly string IniOptions = GetSettings(nameof(IniOptions));
            public static readonly string Widescreen = GetSettings(nameof(Widescreen));
            public static readonly string BattleInterface = GetSettings(nameof(BattleInterface));
            public static readonly string BattleInterfaceType0 = GetSettings(nameof(BattleInterfaceType0));
            public static readonly string BattleInterfaceType1 = GetSettings(nameof(BattleInterfaceType1));
            public static readonly string BattleInterfaceType2 = GetSettings(nameof(BattleInterfaceType2));
            public static readonly string SkipIntrosToMainMenu = GetSettings(nameof(SkipIntrosToMainMenu));
            public static readonly string SkipBattleSwirl = GetSettings(nameof(SkipBattleSwirl));
            public static readonly string AntiAliasing = GetSettings(nameof(AntiAliasing));
            public static readonly string SkipBattleLoading = GetSettings(nameof(SkipBattleLoading));
            public static readonly string HideCardsBubbles = GetSettings(nameof(HideCardsBubbles));
            public static readonly string HideSteamBubbles = GetSettings(nameof(HideSteamBubbles));
            public static readonly string TurnBasedBattles = GetSettings(nameof(TurnBasedBattles));
            public static readonly string SpeedChoice = GetSettings(nameof(SpeedChoice));
            public static readonly string SpeedChoiceType0 = GetSettings(nameof(SpeedChoiceType0));
            public static readonly string SpeedChoiceType1 = GetSettings(nameof(SpeedChoiceType1));
            public static readonly string SpeedChoiceType2 = GetSettings(nameof(SpeedChoiceType2));
            public static readonly string SpeedChoiceType3 = GetSettings(nameof(SpeedChoiceType3));
            public static readonly string TripleTriad = GetSettings(nameof(TripleTriad));
            public static readonly string TripleTriadType0 = GetSettings(nameof(TripleTriadType0));
            public static readonly string TripleTriadType0_ReduceRandom = GetSettings(nameof(TripleTriadType0_ReduceRandom));
            public static readonly string TripleTriadType1 = GetSettings(nameof(TripleTriadType1));
            public static readonly string TripleTriadType2 = GetSettings(nameof(TripleTriadType2));
            public static readonly string Volume = GetSettings(nameof(Volume));
            public static readonly string SoundVolume = GetSettings(nameof(SoundVolume));
            public static readonly string MusicVolume = GetSettings(nameof(MusicVolume));
            public static readonly string MovieVolume = GetSettings(nameof(MovieVolume));
            public static readonly string IniCheats = GetSettings(nameof(IniCheats));
            public static readonly string MaxStealRate = GetSettings(nameof(MaxStealRate));
            public static readonly string DisableCantConcentrate = GetSettings(nameof(DisableCantConcentrate));
            public static readonly string BreakDamageLimit = GetSettings(nameof(BreakDamageLimit));
            public static readonly string AccessBattleMenu = GetSettings(nameof(AccessBattleMenu));
            public static readonly string AccessBattleMenuType0 = GetSettings(nameof(AccessBattleMenuType0));
            public static readonly string AccessBattleMenuType1 = GetSettings(nameof(AccessBattleMenuType1));
            public static readonly string AccessBattleMenuType2 = GetSettings(nameof(AccessBattleMenuType2));
            public static readonly string AccessBattleMenuType3 = GetSettings(nameof(AccessBattleMenuType3));
            public static readonly string SpeedMode = GetSettings(nameof(SpeedMode));
            public static readonly string SpeedFactor = GetSettings(nameof(SpeedFactor));
            public static readonly string PermanentTranse = GetSettings(nameof(PermanentTranse));
            public static readonly string MaxDamage = GetSettings(nameof(MaxDamage));
            public static readonly string NoRandomBattles = GetSettings(nameof(NoRandomBattles));
            public static readonly string PermanentCheats = GetSettings(nameof(PermanentCheats));
            public static readonly string UsePsxFont = GetSettings(nameof(UsePsxFont));
            public static readonly string FontChoice = GetSettings(nameof(FontChoice));
            public static readonly string SBUIenabled = GetSettings(nameof(SBUIenabled));
            public static readonly string BattleTPS = GetSettings(nameof(BattleTPS));
            public static readonly string SharedFPS = GetSettings(nameof(SharedFPS));
            public static readonly string BattleFPS = GetSettings(nameof(BattleFPS));
            public static readonly string FieldFPS = GetSettings(nameof(FieldFPS));
            public static readonly string WorldFPS = GetSettings(nameof(WorldFPS));
            public static readonly string CameraStabilizer = GetSettings(nameof(CameraStabilizer));
            public static readonly string UseOrchestralMusic = GetSettings(nameof(UseOrchestralMusic));
            public static readonly string Use30FpsVideo = GetSettings(nameof(Use30FpsVideo));
            public static readonly string MaxCardCount = GetSettings(nameof(MaxCardCount));
            public static readonly string UIColumnsChoice = GetSettings(nameof(UIColumnsChoice));
            public static readonly string UIColumnsChoice0 = GetSettings(nameof(UIColumnsChoice0));
            public static readonly string UIColumnsChoice1 = GetSettings(nameof(UIColumnsChoice1));
            public static readonly string UIColumnsChoice2 = GetSettings(nameof(UIColumnsChoice2));
            public static readonly string NoAutoTrance = GetSettings(nameof(NoAutoTrance));
            public static readonly string tetraMasterReduceRandomBox0 = GetSettings(nameof(tetraMasterReduceRandomBox0));
            public static readonly string tetraMasterReduceRandomBox1 = GetSettings(nameof(tetraMasterReduceRandomBox1));
            public static readonly string tetraMasterReduceRandomBox2 = GetSettings(nameof(tetraMasterReduceRandomBox2));

            // Settings.ini Tooltips
            public static readonly string ActiveMonitor_Tooltip = GetSettings(nameof(ActiveMonitor_Tooltip));
            public static readonly string WindowMode_Tooltip = GetSettings(nameof(WindowMode_Tooltip));
            public static readonly string Resolution_Tooltip = GetSettings(nameof(Resolution_Tooltip));
            public static readonly string Xsixfour_Tooltip = GetSettings(nameof(Xsixfour_Tooltip));
            public static readonly string Debuggable_Tooltip = GetSettings(nameof(Debuggable_Tooltip));
            public static readonly string CheckUpdates_Tooltip = GetSettings(nameof(CheckUpdates_Tooltip));
            public static readonly string SteamOverlayFix_Tooltip = GetSettings(nameof(SteamOverlayFix_Tooltip));

            // Memoria.ini Tooltips
            public static readonly string UseOrchestralMusic_Tooltip = GetSettings(nameof(UseOrchestralMusic_Tooltip));
            public static readonly string Use30FpsVideo_Tooltip = GetSettings(nameof(Use30FpsVideo_Tooltip));
            public static readonly string Widescreen_Tooltip = GetSettings(nameof(Widescreen_Tooltip));
            public static readonly string AntiAliasing_Tooltip = GetSettings(nameof(AntiAliasing_Tooltip));
            public static readonly string BattleTPS_Tooltip = GetSettings(nameof(BattleTPS_Tooltip));
            public static readonly string SharedFPS_Tooltip = GetSettings(nameof(SharedFPS_Tooltip));
            public static readonly string BattleFPS_Tooltip = GetSettings(nameof(BattleFPS_Tooltip));
            public static readonly string FieldFPS_Tooltip = GetSettings(nameof(FieldFPS_Tooltip));
            public static readonly string WorldFPS_Tooltip = GetSettings(nameof(WorldFPS_Tooltip));
            public static readonly string CameraStabilizer_Tooltip = GetSettings(nameof(CameraStabilizer_Tooltip));
            public static readonly string BattleInterface_Tooltip = GetSettings(nameof(BattleInterface_Tooltip));
            public static readonly string SkipIntrosToMainMenu_Tooltip = GetSettings(nameof(SkipIntrosToMainMenu_Tooltip));
            public static readonly string SkipBattleSwirl_Tooltip = GetSettings(nameof(SkipBattleSwirl_Tooltip));
            public static readonly string HideSteamBubbles_Tooltip = GetSettings(nameof(HideSteamBubbles_Tooltip));
            public static readonly string SpeedChoice_Tooltip = GetSettings(nameof(SpeedChoice_Tooltip));
            public static readonly string TripleTriad_Tooltip = GetSettings(nameof(TripleTriad_Tooltip));
            public static readonly string SoundVolume_Tooltip = GetSettings(nameof(SoundVolume_Tooltip));
            public static readonly string MusicVolume_Tooltip = GetSettings(nameof(MusicVolume_Tooltip));
            public static readonly string MovieVolume_Tooltip = GetSettings(nameof(MovieVolume_Tooltip));
            public static readonly string UsePsxFont_Tooltip = GetSettings(nameof(UsePsxFont_Tooltip));
            public static readonly string FontChoice_Tooltip = GetSettings(nameof(FontChoice_Tooltip));
            public static readonly string UIColumnsChoice_Tooltip = GetSettings(nameof(UIColumnsChoice_Tooltip));

            // Memoria.ini Cheats tooltips
            public static readonly string MaxStealRate_Tooltip = GetSettings(nameof(MaxStealRate_Tooltip));
            public static readonly string NoAutoTrance_Tooltip = GetSettings(nameof(NoAutoTrance_Tooltip));
            public static readonly string DisableCantConcentrate_Tooltip = GetSettings(nameof(DisableCantConcentrate_Tooltip));
            public static readonly string BreakDamageLimit_Tooltip = GetSettings(nameof(BreakDamageLimit_Tooltip));
            public static readonly string AccessBattleMenu_Tooltip = GetSettings(nameof(AccessBattleMenu_Tooltip));
            public static readonly string SpeedMode_Tooltip = GetSettings(nameof(SpeedMode_Tooltip));
            public static readonly string BattleAssistance_Tooltip = GetSettings(nameof(BattleAssistance_Tooltip));
            public static readonly string NoRandomBattles_Tooltip = GetSettings(nameof(NoRandomBattles_Tooltip));
            public static readonly string PermanentCheats_Tooltip = GetSettings(nameof(PermanentCheats_Tooltip));
            public static readonly string MaxCardCount_Tooltip = GetSettings(nameof(MaxCardCount_Tooltip));

            public static readonly string FPSDropboxChoice = GetSettings(nameof(FPSDropboxChoice));
            public static readonly string FPSDropboxChoice0 = GetSettings(nameof(FPSDropboxChoice0));
            public static readonly string FPSDropboxChoice1 = GetSettings(nameof(FPSDropboxChoice1));
            public static readonly string FPSDropboxChoice2 = GetSettings(nameof(FPSDropboxChoice2));
            public static readonly string FPSDropboxChoice3 = GetSettings(nameof(FPSDropboxChoice3));
            public static readonly string FPSDropboxChoice4 = GetSettings(nameof(FPSDropboxChoice4));

            public static readonly string Shader_Enable = GetSettings(nameof(Shader_Enable));
            public static readonly string Shader_Field_chars = GetSettings(nameof(Shader_Field_chars));
            public static readonly string Shader_Battle_chars = GetSettings(nameof(Shader_Battle_chars));
            public static readonly string Shader_Realism = GetSettings(nameof(Shader_Realism));
            public static readonly string Shader_Toon = GetSettings(nameof(Shader_Toon));
            public static readonly string Shader_Outlines = GetSettings(nameof(Shader_Outlines));
            public static readonly string Custom = GetSettings(nameof(Custom));

            public static readonly string Other = GetSettings(nameof(Other));
            public static readonly string Display = GetSettings(nameof(Display));
            public static readonly string Cards = GetSettings(nameof(Cards));
            public static readonly string Advanced = GetSettings(nameof(Advanced));
            public static readonly string FieldShader = GetSettings(nameof(FieldShader));
            public static readonly string BattleShader = GetSettings(nameof(BattleShader));
            public static readonly string FieldShader_Tooltip = GetSettings(nameof(FieldShader_Tooltip));
            public static readonly string BattleShader_Tooltip = GetSettings(nameof(BattleShader_Tooltip));
            public static readonly string ShaderDropboxChoice0 = GetSettings(nameof(ShaderDropboxChoice0));
            public static readonly string ShaderDropboxChoice1 = GetSettings(nameof(ShaderDropboxChoice1));
            public static readonly string ShaderDropboxChoice2 = GetSettings(nameof(ShaderDropboxChoice2));
            public static readonly string ShaderDropboxChoice3 = GetSettings(nameof(ShaderDropboxChoice3));
            public static readonly string ShaderDropboxChoice4 = GetSettings(nameof(ShaderDropboxChoice4));
            public static readonly string ShaderDropboxChoice5 = GetSettings(nameof(ShaderDropboxChoice5));
            public static readonly string menuSettings = GetSettings(nameof(menuSettings));
            public static readonly string menuAdvanced = GetSettings(nameof(menuAdvanced));
            public static readonly string menuCheats = GetSettings(nameof(menuCheats));
            public static readonly string menuPresets = GetSettings(nameof(menuPresets));
            public static readonly string Worldmap = GetSettings(nameof(Worldmap));
            public static readonly string QoL = GetSettings(nameof(QoL));

            public static readonly string Battle = GetSettings(nameof(Battle));
            public static readonly string VanillaCheats = GetSettings(nameof(VanillaCheats));
            public static readonly string EasyTetraMaster = GetSettings(nameof(EasyTetraMaster));
            public static readonly string EasyTetraMaster_Tooltip = GetSettings(nameof(EasyTetraMaster_Tooltip));
            public static readonly string CardReduceRandom = GetSettings(nameof(CardReduceRandom));
            public static readonly string CardReduceRandom_Tooltip = GetSettings(nameof(CardReduceRandom_Tooltip));
            public static readonly string WorldmapMistPreset = GetSettings(nameof(WorldmapMistPreset));
            public static readonly string WorldmapMistPreset_Tooltip = GetSettings(nameof(WorldmapMistPreset_Tooltip));
            public static readonly string WorldmapMistPresetChoice0 = GetSettings(nameof(WorldmapMistPresetChoice0));
            public static readonly string WorldmapMistPresetChoice1 = GetSettings(nameof(WorldmapMistPresetChoice1));
            public static readonly string WorldmapMistPresetChoice2 = GetSettings(nameof(WorldmapMistPresetChoice2));
            public static readonly string WorldmapMistPresetChoice3 = GetSettings(nameof(WorldmapMistPresetChoice3));
            public static readonly string WorldmapDistancePreset = GetSettings(nameof(WorldmapDistancePreset));
            public static readonly string WorldmapDistancePreset_Tooltip = GetSettings(nameof(WorldmapDistancePreset_Tooltip));
            public static readonly string WorldmapDistancePresetChoice0 = GetSettings(nameof(WorldmapDistancePresetChoice0));
            public static readonly string WorldmapDistancePresetChoice1 = GetSettings(nameof(WorldmapDistancePresetChoice1));
            public static readonly string WorldmapDistancePresetChoice2 = GetSettings(nameof(WorldmapDistancePresetChoice2));
            public static readonly string WorldmapDistancePresetChoice3 = GetSettings(nameof(WorldmapDistancePresetChoice3));
            public static readonly string WorldmapFOV = GetSettings(nameof(WorldmapFOV));
            public static readonly string WorldmapFOV_Tooltip = GetSettings(nameof(WorldmapFOV_Tooltip));
            public static readonly string WorldmapBoost = GetSettings(nameof(WorldmapBoost));
            public static readonly string WorldmapBoost_Tooltip = GetSettings(nameof(WorldmapBoost_Tooltip));
            public static readonly string WorldmapShipTilt = GetSettings(nameof(WorldmapShipTilt));
            public static readonly string WorldmapShipTilt_Tooltip = GetSettings(nameof(WorldmapShipTilt_Tooltip));

            public static readonly string ExcaliburIINoTimeLimit = GetSettings(nameof(ExcaliburIINoTimeLimit));
            public static readonly string ExcaliburIINoTimeLimit_Tooltip = GetSettings(nameof(ExcaliburIINoTimeLimit_Tooltip));
            public static readonly string ViviAutoAttack = GetSettings(nameof(ViviAutoAttack));
            public static readonly string ViviAutoAttack_Tooltip = GetSettings(nameof(ViviAutoAttack_Tooltip));

            public static readonly string AccessBattleMenuToggle = GetSettings(nameof(AccessBattleMenuToggle));
            public static readonly string AccessBattleMenuToggle_Tooltip = GetSettings(nameof(AccessBattleMenuToggle_Tooltip));
            public static readonly string EasyJumpRopeMinigame = GetSettings(nameof(EasyJumpRopeMinigame));
            public static readonly string EasyJumpRopeMinigame_Tooltip = GetSettings(nameof(EasyJumpRopeMinigame_Tooltip));
            public static readonly string Attack9999 = GetSettings(nameof(Attack9999));
            public static readonly string Attack9999_Tooltip = GetSettings(nameof(Attack9999_Tooltip));
            public static readonly string WMCameraHeight = GetSettings(nameof(WMCameraHeight));
            public static readonly string WMCameraHeight_Tooltip = GetSettings(nameof(WMCameraHeight_Tooltip));
            public static readonly string WorldmapTPS = GetSettings(nameof(WorldmapTPS));
            public static readonly string WorldmapTPS_Tooltip = GetSettings(nameof(WorldmapTPS_Tooltip));
            public static readonly string HippaulRacingViviSpeed = GetSettings(nameof(HippaulRacingViviSpeed));
            public static readonly string HippaulRacingViviSpeed_Tooltip = GetSettings(nameof(HippaulRacingViviSpeed_Tooltip));
            public static readonly string AudioBackend = GetSettings(nameof(AudioBackend));
            public static readonly string AudioBackend_Tooltip = GetSettings(nameof(AudioBackend_Tooltip));
            public static readonly string WorldSmoothTexture = GetSettings(nameof(WorldSmoothTexture));
            public static readonly string WorldSmoothTexture_Tooltip = GetSettings(nameof(WorldSmoothTexture_Tooltip));
            public static readonly string BattleSmoothTexture = GetSettings(nameof(BattleSmoothTexture));
            public static readonly string BattleSmoothTexture_Tooltip = GetSettings(nameof(BattleSmoothTexture_Tooltip));
            public static readonly string ElementsSmoothTexture = GetSettings(nameof(ElementsSmoothTexture));
            public static readonly string ElementsSmoothTexture_Tooltip = GetSettings(nameof(ElementsSmoothTexture_Tooltip));
        }

        public static class SteamOverlay
        {
            private static String GetSteamOverlay(string name)
            {
                return Instance.Value.GetString(name, nameof(SteamOverlay));
            }

            public static readonly String OptionLabel = GetSteamOverlay(nameof(OptionLabel));
            public static readonly String Caption = GetSteamOverlay(nameof(Caption));
            public static readonly String FixAreYouSure = GetSteamOverlay(nameof(FixAreYouSure));
            public static readonly String RollbackAreYouSure = GetSteamOverlay(nameof(RollbackAreYouSure));
        }

        public static class Launcher
        {
            private static string GetLauncher(string name)
            {
                return Instance.Value.GetString(name, "Launcher");
            }

            public static readonly string Launch = GetLauncher("Launch");
            public static readonly string Launching = GetLauncher("Launching");
            public static readonly string ModManager = GetLauncher("ModManager");
            public static readonly string QuestionTitle = GetLauncher("QuestionTitle");
            public static readonly string ErrorTitle = GetLauncher("ErrorTitle");
            public static readonly string NewVersionIsAvailable = GetLauncher("NewVersionIsAvailable");
            public static readonly string AdvSettings = GetLauncher("AdvSettings");
            public static readonly string AdvSettingsTitle = GetLauncher("AdvSettingsTitle");
            public static readonly string Return = GetLauncher("Return");
            public static readonly string ModelViewer = GetLauncher("ModelViewer");
            public static readonly string ModUpdateAvailable = GetLauncher("ModUpdateAvailable");
            public static readonly string ModConflict = GetLauncher("ModConflict");
        }

        public static class ModEditor
        {
            private static String GetModEditor(string name)
            {
                return Instance.Value.GetString(name, nameof(ModEditor));
            }

            public static readonly String WindowTitle = GetModEditor(nameof(WindowTitle));
            public static readonly String ModInfos = GetModEditor(nameof(ModInfos));
            public static readonly String TabMyMods = GetModEditor(nameof(TabMyMods));
            public static readonly String TabCatalog = GetModEditor(nameof(TabCatalog));
            public static readonly String Mod = GetModEditor(nameof(Mod));
            public static readonly String Name = GetModEditor(nameof(Name));
            public static readonly String Website = GetModEditor(nameof(Website));
            public static readonly String Author = GetModEditor(nameof(Author));
            public static readonly String Release = GetModEditor(nameof(Release));
            public static readonly String ReleaseOriginal = GetModEditor(nameof(ReleaseOriginal));
            public static readonly String Category = GetModEditor(nameof(Category));
            public static readonly String Version = GetModEditor(nameof(Version));
            public static readonly String Description = GetModEditor(nameof(Description));
            public static readonly String ReleaseNotes = GetModEditor(nameof(ReleaseNotes));
            public static readonly String SubModPanel = GetModEditor(nameof(SubModPanel));
            public static readonly String Priority = GetModEditor(nameof(Priority));
            public static readonly String Installed = GetModEditor(nameof(Installed));
            public static readonly String Active = GetModEditor(nameof(Active));
            public static readonly String Progress = GetModEditor(nameof(Progress));
            public static readonly String Speed = GetModEditor(nameof(Speed));
            public static readonly String TimeLeft = GetModEditor(nameof(TimeLeft));
            public static readonly String ActiveIncompatibleMods = GetModEditor(nameof(ActiveIncompatibleMods));
            public static readonly String IncompatibleWithMemoria = GetModEditor(nameof(IncompatibleWithMemoria));
            public static readonly String ApplyModPresetText = GetModEditor(nameof(ApplyModPresetText));
            public static readonly String ApplyModPresetCaption = GetModEditor(nameof(ApplyModPresetCaption));
            public static readonly String UpdateTooltip = GetModEditor(nameof(UpdateTooltip));
            public static readonly String TooltipMoveUp = GetModEditor(nameof(TooltipMoveUp));
            public static readonly String TooltipMoveDown = GetModEditor(nameof(TooltipMoveDown));
            public static readonly String TooltipCheckCompatibility = GetModEditor(nameof(TooltipCheckCompatibility));
            public static readonly String TooltipActivateAll = GetModEditor(nameof(TooltipActivateAll));
            public static readonly String TooltipDeactivateAll = GetModEditor(nameof(TooltipDeactivateAll));
            public static readonly String TooltipUninstall = GetModEditor(nameof(TooltipUninstall));
            public static readonly String TooltipDownload = GetModEditor(nameof(TooltipDownload));
            public static readonly String TooltipCancel = GetModEditor(nameof(TooltipCancel));
        }

        public static class Error
        {
            private static string GetError(string name, string dockableName)
            {
                return Instance.Value.GetString(name, "Error", dockableName);
            }

            public static class File
            {
                private static string GetErrorFile(string name)
                {
                    return GetError(name, "File");
                }

                public static readonly string UnknownFormat = GetErrorFile("Unknown file format.");
                public static readonly string EndOfStream = GetErrorFile("Unexpected end of stream.");
            }
        }
    }
}

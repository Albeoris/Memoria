using System;
using System.Globalization;
using System.Reflection;
using System.Xml;
using Ini;

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

                IniFile iniFile = new IniFile(GameSettingsControl.IniPath);
                String forcedLang = iniFile.ReadValue("Memoria", "LauncherLanguage");

                String[] fileNames = String.IsNullOrEmpty(forcedLang) ?
                    new String[] { CultureInfo.CurrentCulture.Name, CultureInfo.CurrentCulture.TwoLetterISOLanguageName, CultureInfo.CurrentCulture.ThreeLetterISOLanguageName } :
                    new String[] { forcedLang, CultureInfo.CurrentCulture.Name, CultureInfo.CurrentCulture.TwoLetterISOLanguageName, CultureInfo.CurrentCulture.ThreeLetterISOLanguageName,  };
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

            public static readonly string ActiveMonitor = GetSettings(nameof(ActiveMonitor));
            public static readonly string PrimaryMonitor = GetSettings(nameof(PrimaryMonitor));
            public static readonly string Resolution = GetSettings(nameof(Resolution));
            public static readonly string WindowMode = GetSettings(nameof(WindowMode));
            public static readonly string Window = GetSettings(nameof(Window));
            public static readonly string ExclusiveFullscreen = GetSettings(nameof(ExclusiveFullscreen));
            public static readonly string BorderlessFullscreen = GetSettings(nameof(BorderlessFullscreen));
            public static readonly string AudioSamplingFrequency = GetSettings(nameof(AudioSamplingFrequency));
            public static readonly string AudioSamplingFrequencyFormat = GetSettings(nameof(AudioSamplingFrequencyFormat));
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
            public static readonly string SpeedChoiceType4 = GetSettings(nameof(SpeedChoiceType4));
            public static readonly string SpeedChoiceType5 = GetSettings(nameof(SpeedChoiceType5));
            public static readonly string TripleTriad = GetSettings(nameof(TripleTriad));
            public static readonly string TripleTriadType0 = GetSettings(nameof(TripleTriadType0));
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
            public static readonly string BattleAssistance = GetSettings(nameof(BattleAssistance));
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
            public static readonly string EnableCustomShader = GetSettings(nameof(EnableCustomShader));
            public static readonly string EnableRealismShadingForField = GetSettings(nameof(EnableRealismShadingForField));
            public static readonly string EnableRealismShadingForBattle = GetSettings(nameof(EnableRealismShadingForBattle));
            public static readonly string EnableToonShadingForField = GetSettings(nameof(EnableToonShadingForField));
            public static readonly string EnableToonShadingForBattle = GetSettings(nameof(EnableToonShadingForBattle));
            public static readonly string EnableOutlineForFieldCharacter = GetSettings(nameof(EnableOutlineForFieldCharacter));
            public static readonly string EnableOutlineForBattleCharacter = GetSettings(nameof(EnableOutlineForBattleCharacter));
            public static readonly string EnableSSAO = GetSettings(nameof(EnableSSAO));
            public static readonly string NoAutoTrance = GetSettings(nameof(NoAutoTrance));

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
            public static readonly string EnableCustomShader_Tooltip = GetSettings(nameof(EnableCustomShader_Tooltip));

            // Memoria.ini Cheats tooltips
            public static readonly string MaxStealRate_Tooltip = GetSettings(nameof(MaxStealRate_Tooltip));
            public static readonly string NoAutoTrance_Tooltip = GetSettings(nameof(NoAutoTrance_Tooltip));
            public static readonly string DisableCantConcentrate_Tooltip = GetSettings(nameof(DisableCantConcentrate_Tooltip));
            public static readonly string BreakDamageLimit_Tooltip = GetSettings(nameof(BreakDamageLimit_Tooltip));
            public static readonly string AccessBattleMenu_Tooltip = GetSettings(nameof(AccessBattleMenu_Tooltip));
            public static readonly string SpeedMode_Tooltip = GetSettings(nameof(SpeedMode_Tooltip));
            public static readonly string SpeedFactor_Tooltip = GetSettings(nameof(SpeedFactor_Tooltip));
            public static readonly string BattleAssistance_Tooltip = GetSettings(nameof(BattleAssistance_Tooltip));
            public static readonly string NoRandomBattles_Tooltip = GetSettings(nameof(NoRandomBattles_Tooltip));
            public static readonly string PermanentCheats_Tooltip = GetSettings(nameof(PermanentCheats_Tooltip));
            public static readonly string MaxCardCount_Tooltip = GetSettings(nameof(MaxCardCount_Tooltip));



        }


        public static class SdLib
        {
            private static String GetSdLib(string name)
            {
                return Instance.Value.GetString(name, nameof(SdLib));
            }

            public static readonly string Caption = GetSdLib(nameof(Caption));
            public static readonly string AreYouSure = GetSdLib(nameof(AreYouSure));
            public static readonly String SuccessBoth = GetSdLib(nameof(SuccessBoth));
            public static readonly String SuccessX64 = GetSdLib(nameof(SuccessX64));
            public static readonly String SuccessX86 = GetSdLib(nameof(SuccessX86));
            public static readonly String Fail = GetSdLib(nameof(Fail));
            public static readonly String CannotRead = GetSdLib(nameof(CannotRead));
            public static readonly String CannotWrite = GetSdLib(nameof(CannotWrite));
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

        public static class Button
        {
            private static string GetButton(string name)
            {
                return Instance.Value.GetString(name, "Button");
            }

            public static readonly string OK = GetButton("OK");
            public static readonly string Cancel = GetButton("Cancel");
            public static readonly string Continue = GetButton("Continue");
            public static readonly string Clone = GetButton("Clone");
            public static readonly string Remove = GetButton("Remove");
            public static readonly string Rollback = GetButton("Rollback");
            public static readonly string Inject = GetButton("Inject");
            public static readonly string SaveAs = GetButton("SaveAs");
            public static readonly string Exit = GetButton("Exit");
            public static readonly string Exiting = GetButton("Exiting");
            public static readonly string Launch = GetButton("Launch");
            public static readonly string Launching = GetButton("Launching");
            public static readonly string ModManager = GetButton("ModManager");
        }

        public static class InfoProvider
        {
            private static string GetInfoProvider(string name, string providerName)
            {
                return Instance.Value.GetString(name, "InfoProvider", providerName);
            }

            public static class ApplicationConfig
            {
                private static string GetApplicationConfig(string name)
                {
                    return GetInfoProvider(name, "ApplicationConfig");
                }

                public static readonly string Title = GetApplicationConfig("Title");
                public static readonly string Description = GetApplicationConfig("Description");
                public static readonly string NewTitle = GetApplicationConfig("NewTitle");
                public static readonly string NewDescription = GetApplicationConfig("NewDescription");
                public static readonly string FileTitle = GetApplicationConfig("FileTitle");
                public static readonly string FileDescription = GetApplicationConfig("FileDescription");
            }

            public static class GameLocation
            {
                private static string GetGameLocation(string name)
                {
                    return GetInfoProvider(name, "GameLocation");
                }

                public static readonly string Title = GetGameLocation("Title");
                public static readonly string Description = GetGameLocation("Description");
                public static readonly string ConfigurationTitle = GetGameLocation("ConfigurationTitle");
                public static readonly string ConfigurationDescription = GetGameLocation("ConfigurationDescription");
                public static readonly string SteamRegistryTitle = GetGameLocation("SteamRegistryTitle");
                public static readonly string SteamRegistryDescription = GetGameLocation("SteamRegistryDescription");
                public static readonly string UserTitle = GetGameLocation("UserTitle");
                public static readonly string UserDescription = GetGameLocation("UserDescription");
            }

            public static class WorkingLocation
            {

                private static string GetWorkingLocation(string name)
                {
                    return GetInfoProvider(name, "WorkingLocation");
                }

                public static readonly string Title = GetWorkingLocation("Title");
                public static readonly string Description = GetWorkingLocation("Description");
                public static readonly string ConfigurationTitle = GetWorkingLocation("ConfigurationTitle");
                public static readonly string ConfigurationDescription = GetWorkingLocation("ConfigurationDescription");
                public static readonly string UserTitle = GetWorkingLocation("UserTitle");
                public static readonly string UserDescription = GetWorkingLocation("UserDescription");
            }

            public static class TextEncoding
            {
                private static string GetTextEncoding(string name)
                {
                    return GetInfoProvider(name, "TextEncoding");
                }

                public static readonly string Title = GetTextEncoding("Title");
                public static readonly string Description = GetTextEncoding("Description");
                public static readonly string NewTitle = GetTextEncoding("NewTitle");
                public static readonly string NewDescription = GetTextEncoding("NewDescription");
                public static readonly string WorkingLocationTitle = GetTextEncoding("WorkingLocationTitle");
                public static readonly string WorkingLocationDescription = GetTextEncoding("WorkingLocationDescription");
                public static readonly string UserTitle = GetTextEncoding("UserTitle");
                public static readonly string UserDescription = GetTextEncoding("UserDescription");
            }

            public static class AudioSettings
            {
                private static string GetAudioSettings(string name)
                {
                    return GetInfoProvider(name, "AudioSettings");
                }

                public static readonly string Title = GetAudioSettings("Title");
                public static readonly string Description = GetAudioSettings("Description");
                public static readonly string NewTitle = GetAudioSettings("NewTitle");
                public static readonly string NewDescription = GetAudioSettings("NewDescription");
            }
        }

        public static class Dockable
        {
            private static string GetDockable(string name, string dockableName)
            {
                return Instance.Value.GetString(name, "Dockable", dockableName);
            }

            private static string GetDockable(string name, string dockableName, string childName)
            {
                return Instance.Value.GetString(name, "Dockable", dockableName, childName);
            }

            public static class DataSources
            {
                private static string GetDockableInfoProviders(string name)
                {
                    return GetDockable(name, "DataSources");
                }

                public static readonly string Header = GetDockableInfoProviders("Header");
            }

            public static class GameFileCommander
            {
                private static string GetDockableInfoProviders(string name)
                {
                    return GetDockable(name, "GameFileCommander");
                }

                public static readonly string Header = GetDockableInfoProviders("Header");
                public static readonly string Unpack = GetDockableInfoProviders("Unpack");
                public static readonly string Pack = GetDockableInfoProviders("Pack");
                public static readonly string ArchivesNode = GetDockableInfoProviders("ArchivesNode");
            }

            public static class GameFilePreview
            {
                private static string GetDockableInfoProviders(string name)
                {
                    return GetDockable(name, "GameFilePreview");
                }

                private static string GetDockableInfoProviders(string name, string childName)
                {
                    return GetDockable(name, "GameFilePreview", childName);
                }

                public static readonly string Header = GetDockableInfoProviders("Header");

                public static class Ykd
                {
                    private static string GetDockableInfoProvidersYkd(string name)
                    {
                        return GetDockableInfoProviders(name, "Ykd");
                    }

                    public static readonly string ResourceRemovingTitle = GetDockableInfoProvidersYkd("ResourceRemovingTitle");
                    public static readonly string ConfirmResourceRemoving = GetDockableInfoProvidersYkd("ConfirmResourceRemoving");
                }
            }
        }

        public static class EncodingEditor
        {
            private static string GetEncodingEditor(string name, string childName)
            {
                return Instance.Value.GetString(name, nameof(EncodingEditor), childName);
            }
            
            public static class List
            {
                private static string GetEncodingEditorList(string name)
                {
                    return GetEncodingEditor(name, nameof(List));
                }

                public static readonly string Name = GetEncodingEditorList(nameof(Name));
                public static readonly string Pattern = GetEncodingEditorList(nameof(Pattern));
                public static readonly string Encoding = GetEncodingEditorList(nameof(Encoding));
                public static readonly string Font = GetEncodingEditorList(nameof(Font));
                public static readonly string Edit = GetEncodingEditorList(nameof(Edit));
            }

            public static class EncodingEditDialog
            {
                private static string GetEncodingEditorEncodingEditDialog(string name)
                {
                    return GetEncodingEditor(name, nameof(EncodingEditDialog));
                }

                public static readonly string Title = GetEncodingEditorEncodingEditDialog(nameof(Title));
            }

            public static class EncodingSelectDialog
            {
                private static string GetEncodingEditorEncodingSelectDialog(string name)
                {
                    return GetEncodingEditor(name, nameof(EncodingSelectDialog));
                }

                public static readonly string Title = GetEncodingEditorEncodingSelectDialog(nameof(Title));
                public static readonly string Watermark = GetEncodingEditorEncodingSelectDialog(nameof(Watermark));
                public static readonly string ConfirmMessageFormat = GetEncodingEditorEncodingSelectDialog(nameof(ConfirmMessageFormat));
                public static readonly string ErrorMessageFormat = GetEncodingEditorEncodingSelectDialog(nameof(ErrorMessageFormat));
            }

            public static class Main
            {
                private static string GetEncodingEditorMain(string name)
                {
                    return GetEncodingEditor(name, "Main");
                }

                public static readonly string Before = GetEncodingEditorMain("Before");
                public static readonly string Width = GetEncodingEditorMain("Width");
                public static readonly string After = GetEncodingEditorMain("After");
                public static readonly string FromText = GetEncodingEditorMain("FromText");
                public static readonly string ToText = GetEncodingEditorMain("ToText");
            }

            public static class Extra
            {
                private static string GetEncodingEditorExtra(string name)
                {
                    return GetEncodingEditor(name, "Extra");
                }

                public static readonly string Row = GetEncodingEditorExtra("Row");
                public static readonly string Column = GetEncodingEditorExtra("Column");
                public static readonly string FromText = GetEncodingEditorExtra("FromText");
                public static readonly string ToText = GetEncodingEditorExtra("ToText");
            }
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
            public static readonly String PreviewImageMissing = GetModEditor(nameof(PreviewImageMissing));
            public static readonly String TooltipMoveUp = GetModEditor(nameof(TooltipMoveUp));
            public static readonly String TooltipMoveDown = GetModEditor(nameof(TooltipMoveDown));
            public static readonly String TooltipCheckCompatibility = GetModEditor(nameof(TooltipCheckCompatibility));
            public static readonly String TooltipActivateAll = GetModEditor(nameof(TooltipActivateAll));
            public static readonly String TooltipDeactivateAll = GetModEditor(nameof(TooltipDeactivateAll));
            public static readonly String TooltipUninstall = GetModEditor(nameof(TooltipUninstall));
            public static readonly String TooltipDownload = GetModEditor(nameof(TooltipDownload));
            public static readonly String TooltipCancel = GetModEditor(nameof(TooltipCancel));
        }

        public static class Dialogue
        {
            private static string GetDialogue(string name, string attrName)
            {
                return Instance.Value.GetString(name, "Dialogue", attrName);
            }

            public static class SaveAs
            {
                private static string GetDialogueSaveAs(string name)
                {
                    return GetDialogue(name, "SaveAs");
                }

                public static readonly string Title = GetDialogueSaveAs("Title");
            }
        }

        public static class Message
        {
            private static string GetMessage(string name, string attrName)
            {
                return Instance.Value.GetString(name, "Message", attrName);
            }

            public static class Done
            {
                private static string GetMessageDone(string name)
                {
                    return GetMessage(name, "Done");
                }

                public static readonly string Title = GetMessageDone("Title");
                public static readonly string ExtractionCompleteFormat = GetMessageDone("ExtractionCompleteFormat");
                public static readonly string InjectionCompleteFormat = GetMessageDone("InjectionCompleteFormat");
            }

            public static class Question
            {
                private static string GetMessageQuestion(string name)
                {
                    return GetMessage(name, nameof(Question));
                }

                public static readonly string Title = GetMessageQuestion(nameof(Title));
                public static readonly string AreYouSureTitle = GetMessageQuestion(nameof(AreYouSureTitle));
                public static readonly string NewVersionIsAvailable = GetMessageQuestion(nameof(NewVersionIsAvailable));
            }

            public static class Error
            {
                private static string GetMessageError(string name)
                {
                    return GetMessage(name, "Error");
                }

                public static readonly string Title = GetMessageError("Title");
            }
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

                public static readonly string UnknownFormat = GetErrorFile("UnknownFormat");
                public static readonly string EndOfStream = GetErrorFile("Unexpected end of stream.");
            }

            public static class Process
            {
                private static string GetErrorProcess(string name)
                {
                    return GetError(name, "Process");
                }

                public static readonly string CannotGetExecutablePath = GetErrorProcess("CannotGetExecutablePath");
            }

            public static class Text
            {
                private static string GetErrorText(string name)
                {
                    return GetError(name, "Text");
                }

                public static readonly string TooLongTagNameFormat = GetErrorText("TooLongTagNameFormat");
            }
        }
    }
}

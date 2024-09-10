using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using Application = System.Windows.Application;
using System.Reflection;

namespace Memoria.Launcher
{
    public class IniFile
    {
        public String path;

        public IniFile(String INIPath)
        {
            this.path = INIPath;
        }

        [DllImport("kernel32")]
        private static extern Int64 WritePrivateProfileString(String section, String key, String val, String filePath);

        [DllImport("kernel32")]
        private static extern Int32 GetPrivateProfileString(String section, String key, String def, StringBuilder retVal, Int32 size, String filePath);

        public void WriteValue(String Section, String Key, String Value)
        {
            IniFile.WritePrivateProfileString(Section, Key, Value, this.path);
        }

        public String ReadValue(String Section, String Key)
        {
            _retBuffer.Clear();
            IniFile.GetPrivateProfileString(Section, Key, "", _retBuffer, _bufferSize, this.path);
            return _retBuffer.ToString();
        }

        private const Int32 _bufferSize = 10000;
        private StringBuilder _retBuffer = new StringBuilder(_bufferSize);

        private static readonly String _iniPath = AppDomain.CurrentDomain.BaseDirectory + @"Memoria.ini";

        public static async void SanitizeMemoriaIni()
        {
            if (!File.Exists(_iniPath))
            {
                Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("Memoria.ini");
                StreamReader reader = new(input);
                string text = reader.ReadToEnd();
                File.WriteAllText(_iniPath, text);
            }
            else {
                MakeSureSpacesAroundEqualsigns();
                try
                {
                    RemoveDuplicateKeys(_iniPath);
                    IniFile iniFile = new(_iniPath);
                    String _checklatestadded = iniFile.ReadValue("Interface", "SynthIngredientStockDisplayed"); // check if the latest ini parameter is already there
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
                        MakeIniNotNull("Graphics", "FieldSmoothTexture", "1");
                        MakeIniNotNull("Graphics", "WorldSmoothTexture", "1");
                        MakeIniNotNull("Graphics", "BattleSmoothTexture", "1");
                        MakeIniNotNull("Graphics", "ElementsSmoothTexture", "1");
                        MakeIniNotNull("Graphics", "SFXSmoothTexture", "-1");
                        MakeIniNotNull("Graphics", "UISmoothTexture", "-1");

                        MakeIniNotNull("Control", "Enabled", "1");
                        MakeIniNotNull("Control", "DisableMouse", "0");
                        MakeIniNotNull("Control", "DialogProgressButtons", "\"Confirm\"");
                        MakeIniNotNull("Control", "WrapSomeMenus", "1");
                        MakeIniNotNull("Control", "BattleAutoConfirm", "1");
                        MakeIniNotNull("Control", "TurboDialog", "1");
                        MakeIniNotNull("Control", "PSXScrollingMethod", "1");
                        MakeIniNotNull("Control", "PSXMovementMethod", "1");
                        MakeIniNotNull("Control", "AlwaysCaptureGamepad", "1");

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
                        MakeIniNotNull("Interface", "MinimapPreset", "1");
                        MakeIniNotNull("Interface", "MinimapOffsetX", "0");
                        MakeIniNotNull("Interface", "MinimapOffsetY", "0");
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
                        MakeIniNotNull("Interface", "SynthIngredientStockDisplayed", "1");
                        MakeIniNotNull("Interface", "DisplayPSXDiscChanges", "1");

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

                        MakeIniNotNull("Shaders", "Enabled", "0");
                        MakeIniNotNull("Shaders", "Shader_Field_Realism", "0");
                        MakeIniNotNull("Shaders", "Shader_Field_Toon", "0");
                        MakeIniNotNull("Shaders", "Shader_Field_Outlines", "0");
                        MakeIniNotNull("Shaders", "Shader_Battle_Realism", "0");
                        MakeIniNotNull("Shaders", "Shader_Battle_Toon", "0");
                        MakeIniNotNull("Shaders", "Shader_Battle_Outlines", "0");

                        MakeSureSpacesAroundEqualsigns();
                    }
                }
                catch (Exception ex)
                {
                    UiHelper.ShowError(Application.Current.MainWindow, ex);
                }
            }
        }

        private static async void MakeSureSpacesAroundEqualsigns()
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

        private static async void MakeIniNotNull(String Category, String Setting, String Defaultvalue)
        {
            IniFile iniFile = new(_iniPath);
            String value = iniFile.ReadValue(Category, Setting);
            if (String.IsNullOrEmpty(value))
            {
                iniFile.WriteValue(Category, Setting + " ", " " + Defaultvalue);
            }
        }

        public static void RemoveDuplicateKeys(string iniPath)
        {
            string wholeFile = File.ReadAllText(iniPath);
            string cleanedContent = RemoveDuplicateKeysFromContent(wholeFile);
            File.WriteAllText(iniPath, cleanedContent);
        }

        private static string RemoveDuplicateKeysFromContent(string content)
        {
            var sections = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            string[] lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string currentSection = "";

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = line.Trim('[', ']');
                    if (!sections.ContainsKey(currentSection))
                    {
                        sections[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    }
                }
                else if (!line.Contains(";") && line.Contains("=") && !line.StartsWith("="))
                {
                    var keyValue = line.Split(['='], 2);
                    sections[currentSection][keyValue[0].Trim()] = keyValue[1].Trim();
                }
                else
                {
                    sections[currentSection][line] = "zzz";
                }
            }

            return GenerateContentFromSections(sections);
        }

        private static string GenerateContentFromSections(Dictionary<string, Dictionary<string, string>> sections)
        {
            var result = new List<string>();

            foreach (var section in sections)
            {
                result.Add($"[{section.Key}]");
                foreach (var keyValue in section.Value)
                {
                    if (keyValue.Value != "zzz")
                    {
                        result.Add($"{keyValue.Key} = {keyValue.Value}");
                    }
                    else
                    {
                        result.Add($"{keyValue.Key}");
                    }
                }
                result.Add(""); // Add a blank line after each section for readability
            }
            return string.Join(Environment.NewLine, result);
        }
    }
}

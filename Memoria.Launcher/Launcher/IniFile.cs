using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using Application = System.Windows.Application;
using System.Reflection;
using System.Linq;

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


        public static void SanitizeMemoriaIni()
        {
            Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("Memoria.ini");
            string text;
            using (StreamReader reader = new(input))
            {
                text = reader.ReadToEnd();
            }

            if (!File.Exists(_iniPath))
            {
                File.WriteAllText(_iniPath, text);
                return;
            }

            File.WriteAllLines(_iniPath, MergeIniFiles(text.Replace("\r", "").Split('\n'), File.ReadAllLines(_iniPath)));
        }

        private static String[] MergeIniFiles(String[] newIni, String[] previousIni)
        {
            List<String> mergedIni = new List<String>(previousIni.Where(line => !line.StartsWith("\t; "))); // In order to have a persisting custom comment, the user must use a slightly different format than "	; " (eg. "	;; ")
            for (Int32 i = 0; i < mergedIni.Count; i++)
            {
                // Hotfix: replace incorrect default formulas by the correct ones
                if (String.Compare(mergedIni[i], "StatusDurationFormula = ContiCnt * (IsNegativeStatus ? 8 * (60 - TargetSpirit) : 8 * TargetSpirit)") == 0
                    || String.Compare(mergedIni[i], "StatusTickFormula = OprCnt * (IsNegativeStatus ? 4 * (60 - TargetSpirit) : 4 * TargetSpirit)") == 0)
                {
                    mergedIni.RemoveAt(i--);
                }
            }
            String currentSection = "";
            Int32 sectionFirstLine = 0;
            Int32 sectionLastLine = 0;
            foreach (String line in newIni)
            {
                String trimmedLine = line.Trim();
                if (trimmedLine.Length == 0)
                    continue;
                if (trimmedLine.StartsWith("["))
                {
                    currentSection = trimmedLine.Substring(1, trimmedLine.IndexOf("]") - 1);
                    sectionFirstLine = mergedIni.FindIndex(s => s.Trim().StartsWith("[" + currentSection + "]"));
                    Boolean hasSection = sectionFirstLine >= 0;
                    if (hasSection)
                    {
                        sectionFirstLine++;
                        sectionLastLine = sectionFirstLine;
                        while (sectionLastLine < mergedIni.Count)
                        {
                            if (mergedIni[sectionLastLine].Trim().StartsWith("["))
                                break;
                            sectionLastLine++;
                        }
                        while (sectionLastLine > 0 && mergedIni[sectionLastLine - 1].Trim().Length == 0)
                            sectionLastLine--;
                    }
                    else
                    {
                        mergedIni.Add("[" + currentSection + "]");
                        sectionFirstLine = mergedIni.Count;
                        sectionLastLine = sectionFirstLine;
                    }
                }
                else if (trimmedLine.StartsWith(";"))
                {
                    if (!mergedIni.Exists(s => s.Trim() == trimmedLine))
                    {
                        mergedIni.Insert(sectionFirstLine, line);
                        sectionFirstLine++;
                        sectionLastLine++;
                    }
                }
                else
                {
                    String fieldName = trimmedLine.Substring(0, trimmedLine.IndexOfAny(new Char[] { ' ', '\t', '=' }));
                    Boolean fieldKnown = false;
                    for (Int32 i = sectionFirstLine; i < sectionLastLine; i++)
                    {
                        if (mergedIni[i].Trim().StartsWith(fieldName))
                        {
                            fieldKnown = true;
                            break;
                        }
                    }
                    if (!fieldKnown)
                    {
                        mergedIni.Insert(sectionLastLine, line);
                        sectionFirstLine++;
                        sectionLastLine++;
                    }
                }
            }
            return mergedIni.ToArray();
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

    public class IniReader
    {
        public IniFile IniFile;

        public struct Key
        {
            public String Section;
            public String Name;

            public Key(String section, String name)
            {
                Section = section;
                Name = name;
            }
        }

        public Dictionary<Key, String> Options = new Dictionary<Key, String>();

        public IniReader(String path)
        {
            IniFile = new IniFile(path);
            if (!File.Exists(path)) return;
            using (Stream input = File.OpenRead(IniFile.path))
            using (StreamReader sr = new StreamReader(input))
            {
                String section = null;
                while (!sr.EndOfStream)
                {
                    String line = sr.ReadLine().Trim();
                    if (String.IsNullOrEmpty(line))
                        continue;

                    String key = null;
                    String value = null;
                    for (int i = 0; i < line.Length; i++)
                    {
                        Char ch = line[i];
                        if (ch == ';' || ch == '#')
                            break;

                        if (ch == '[')
                        {
                            section = line.Substring(i + 1, line.IndexOf(']', i) - i - 1);
                            break;
                        }

                        if (ch == '=')
                        {
                            key = line.Substring(0, i).TrimEnd();
                            value = line.Substring(i + 1).Trim();
                            break;
                        }
                    }

                    if (String.IsNullOrEmpty(section) || String.IsNullOrEmpty(key))
                        continue;
                    Options[new Key(section, key)] = value;
                }
            }
        }
    }
}

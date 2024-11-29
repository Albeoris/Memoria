using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Memoria
{
    public class IniFile
    {
        public const String MemoriaIniPath = @"./Memoria.ini";
        public const String SettingsIniPath = @"./Settings.ini";

        private String _path;
        private static IniFile _settings;
        private static IniFile _memoria;

        public static Boolean PreventWrite = false;

        public static IniFile SettingsIni
        {
            get
            {
                if (_settings == null)
                    _settings = new IniFile(IniFile.SettingsIniPath);
                return _settings;
            }
        }
        public static IniFile MemoriaIni
        {
            get
            {
                if (_memoria == null)
                {
                    SanitizeMemoriaIni();
                    _memoria = new IniFile(IniFile.MemoriaIniPath);
                }
                return _memoria;
            }
        }

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

        public IniFile(String path)
        {
            _path = path;
            if (!File.Exists(path)) return;
            using (Stream input = File.OpenRead(path))
            {
                Init(input);
            }
        }

        public IniFile(Stream input)
        {
            Init(input);
        }

        public String GetSetting(String section, String key, String _default = "")
        {
            Key k = new Key(section, key);
            return Options.ContainsKey(k) ? Options[k] : _default;
        }

        public void SetSetting(String Section, String Key, String Value)
        {
            if (PreventWrite) return;
            Key k = new Key(Section, Key);
            Options[k] = Value;
        }

        public void Save()
        {
            if (PreventWrite) return;
            if (_path == null) throw new NullReferenceException("This IniFile was created without a path, values cannot be written.");
            if (!File.Exists(_path)) return;
            WriteAllSettings(_path);
        }

        public void Reload()
        {
            if (_path == null) throw new NullReferenceException("This IniFile was created without a path, it cannot be reloaded.");
            using (Stream input = File.OpenRead(_path))
            {
                Init(input);
            }
        }

        private static void SanitizeMemoriaIni()
        {
            Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("Memoria.ini");
            string text;
            using (StreamReader reader = new(input))
            {
                text = reader.ReadToEnd();
            }

            if (!File.Exists(MemoriaIniPath))
            {
                File.WriteAllText(MemoriaIniPath, text);
                return;
            }

            File.WriteAllLines(MemoriaIniPath, MergeIniFiles(text.Replace("\r", "").Split('\n'), File.ReadAllLines(MemoriaIniPath)));
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
                // Make sure spaces are present around =
                if (!mergedIni[i].Trim().StartsWith(";"))
                {
                    var split = mergedIni[i].Split('=');
                    for (Int32 j = 0; j < split.Length; j++)
                    {
                        split[j] = split[j].Trim();
                    }
                    mergedIni[i] = String.Join(" = ", split);
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
                        if (mergedIni.Count > 0 && mergedIni.Last().Length > 0)
                            mergedIni.Add("");

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
                        if (mergedIni[i].Trim().StartsWith(fieldName + " ="))
                        {
                            fieldKnown = true;
                            break;
                        }
                    }
                    if (!fieldKnown)
                    {
                        mergedIni.Insert(sectionLastLine, line);
                        sectionLastLine++;
                    }
                }
            }
            return mergedIni.ToArray();
        }

        public void WriteAllSettings(String path, String[] ignoreSections = null, String[] ignoreOptions = null)
        {
            List<String> lines = new List<string>(File.ReadAllLines(path));

            foreach (var option in Options)
            {
                if (ignoreOptions != null && ignoreOptions.Contains($"{option.Key.Section}.{option.Key.Name}"))
                    continue;
                if (ignoreSections != null && ignoreSections.Contains(option.Key.Section))
                    continue;

                Boolean sectionFound = false;
                Boolean optionFound = false;
                for (Int32 i = 0; i < lines.Count; i++)
                {
                    string trimmed = lines[i].Replace(" =", "=").Trim();
                    if (!sectionFound)
                    {
                        if (trimmed == $"[{option.Key.Section}]")
                            sectionFound = true;
                        continue;
                    }

                    if (trimmed.StartsWith("["))
                    {
                        if (sectionFound)
                        {
                            lines.Insert(i, $"{option.Key.Name} = {option.Value}");
                            optionFound = true;
                        }
                        break;
                    }

                    if (trimmed.StartsWith($"{option.Key.Name}="))
                    {
                        lines[i] = $"{option.Key.Name} = {option.Value}";
                        optionFound = true;
                        break;
                    }
                }

                if (!optionFound)
                {
                    if (!sectionFound)
                        lines.Add($"\n[{option.Key.Section}]");
                    lines.Add($"{option.Key.Name} = {option.Value}");
                }
            }

            File.WriteAllLines(path, lines.ToArray());
        }

        private void Init(Stream input)
        {
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

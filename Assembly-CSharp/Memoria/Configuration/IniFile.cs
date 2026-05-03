using System;
using System.Collections.Generic;
using System.IO;

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
                    _memoria = new IniFile(IniFile.MemoriaIniPath);
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

        public void Reload()
        {
            if (_path == null) throw new NullReferenceException("This IniFile was created without a path, it cannot be reloaded.");
            using (Stream input = File.OpenRead(_path))
            {
                Init(input);
            }
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Ini
{
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

using System;
using System.Linq;
using System.Collections.Generic;

namespace Memoria.Prime.CSV
{
    public class CsvMetaData
    {
        public Version Version
        {
            get => _version;
            set => _version = value;
        }

        public Boolean IsAppendMode => _appendMode;

        public Boolean HasOption(String optionName)
        {
            return _optionsOn.Contains(optionName);
        }

        public void AddOption(String optionName)
        {
            _optionsOn.Add(optionName);
        }

        public Boolean HasField(String fieldName)
        {
            return !_appendMode || _appendList.Contains(fieldName);
        }

        public Boolean ParseLine(String line)
        {
            String[] code = line.Split(INSTRUCTION_SEP, 2, StringSplitOptions.None);
            if (code.Length == 0)
                return false;
            if (code.Length == 1)
            {
                String optionName = code[0];
                Boolean add = true;
                if (code[0].StartsWith("-"))
                {
                    add = false;
                    optionName = code[0].Substring(1);
                }
                else if (code[0].StartsWith("+"))
                {
                    optionName = code[0].Substring(1);
                }
                if (String.Equals(optionName, "AppendMode"))
                    _appendMode = add;
                else if (add)
                    _optionsOn.Add(optionName);
                else
                    _optionsOn.Remove(optionName);
                return true;
            }
            if (String.Equals(code[0], "VERSION"))
            {
                _version = new Version(code[1]);
                return true;
            }
            if (String.Equals(code[0], "AppendMode"))
            {
                _appendList.Clear();
                _appendMode = true;
                foreach (String token in code[1].Split(';'))
                {
                    String trimmed = token.Trim();
                    if (trimmed.Length > 0)
                        _appendList.Add(trimmed);
                }
                return true;
            }
            return false;
        }

        public String[] GenerateLines()
        {
            List<String> lines = new List<String>();
            if (_version != new Version())
                lines.Add("VERSION:" + _version.ToString());
            foreach (String option in _optionsOn)
                lines.Add(option);
            if (_appendMode)
                lines.Add("AppendMode:" + String.Join(";", _appendList.ToArray()));
            return lines.ToArray();
        }

        // Maybe we might want to add more flexibility in the future, like "Int32 GetParameterInt(String paramName)" or things like that
        private static readonly String[] INSTRUCTION_SEP = new[] { ":" };
        private HashSet<String> _optionsOn = new HashSet<String>();
        private HashSet<String> _appendList = new HashSet<String>();
        private Boolean _appendMode = false;
        private Version _version = new Version();
    }
}

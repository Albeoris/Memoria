using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Memoria.Prime.Ini
{
    public sealed class IniReader
    {
        private readonly String _filePath;

        private Dictionary<String, IniSection> _sections;
        private Dictionary<String, IniValue> _values;
        private IniValue _currnetValue;
        private StringBuilder _sb;
        private String _line;
        private Boolean _section;
        private Int32 _index;

        public IniReader(String filePath)
        {
            _filePath = filePath;
        }

        public void Read(Ini output)
        {
            try
            {
                _sections = output.GetSections().ToDictionary(s => s.Name);
                if (_sections.Count == 0)
                    return;

                using (Stream input = File.OpenRead(_filePath))
                using (StreamReader sr = new StreamReader(input))
                {
                    _sb = new StringBuilder();
                    while (!sr.EndOfStream)
                    {
                        _line = sr.ReadLine();
                        if (String.IsNullOrEmpty(_line))
                            continue;

                        ParseLine();
                    }
                }
            }
            finally
            {
                _sections = null;
                _values = null;
                _sb = null;
            }
        }

        private void ParseLine()
        {
            _section = false;
            _index = 0;
            _sb.Length = 0;
            if (!TryBeginParseLine())
                return;

            if (_section)
                ReadSection();
            else
                ReadPair();
        }

        private void ReadPair()
        {
            if (!ReadKey())
                return;

            Boolean escape = false;
            while (++_index < _line.Length)
            {
                Char ch = _line[_index];
                if (escape)
                {
                    if (ch != ';')
                        break;

                    _sb.Append(';');
                    escape = false;
                }

                if (ch == ';')
                    escape = true;
                else
                    _sb.Append(ch);
            }

            _currnetValue.ParseValue(_sb.ToString().Trim());
        }

        private Boolean ReadKey()
        {
            while (++_index < _line.Length)
            {
                Char ch = _line[_index];
                if (ch == '=')
                {
                    if (!_values.TryGetValue(_sb.ToString().TrimEnd(), out _currnetValue))
                        return false;

                    _sb.Length = 0;
                    return true;
                }

                _sb.Append(ch);
            }

            return false;
        }

        private Boolean TryBeginParseLine()
        {
            for (; _index < _line.Length; _index++)
            {
                Char ch = _line[_index];
                if (ch == ';' || ch == '#')
                    return false;

                if (ch == '[')
                {
                    _section = true;
                    return true;
                }

                if (Char.IsLetterOrDigit(ch))
                {
                    if (_values == null)
                        return false;

                    _sb.Append(ch);
                    return true;
                }
            }

            return false;
        }

        private void ReadSection()
        {
            while (++_index < _line.Length)
            {
                Char ch = _line[_index];
                if (ch == ']')
                {
                    IniSection section;
                    if (_sections.TryGetValue(_sb.ToString(), out section))
                    {
                        try
                        {
                            _values = section.GetValuesInternal().ToDictionary(v => v.Name);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Failed to load data for the section [{0}]", section.Name);
                            _values = new Dictionary<String, IniValue>();
                        }
                    }
                    break;
                }
                _sb.Append(ch);
            }
        }
    }
}
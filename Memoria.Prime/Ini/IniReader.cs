using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Memoria.Prime.Text;

namespace Memoria.Prime.Ini
{
    internal abstract class SectionBinding
    {
        public readonly String Name;

        protected SectionBinding(String name)
        {
            Name = name;
        }

        public abstract IniSection CreateSection();
        public abstract void UpdateSection(IniSection value);
    }

    internal sealed class SectionBinding<T> : SectionBinding where T : IniSection
    {
        private Func<T> _ctor;
        private Action<T> _setter;

        public SectionBinding(String name, Func<T> ctor, Action<T> setter)
            :base(name)
        {
            _ctor = ctor;
            _setter = setter;
        }

        public override IniSection CreateSection()
        {
            return _ctor();
        }

        public override void UpdateSection(IniSection value)
        {
            _setter((T)value);
        }
    }

    public sealed class IniReader
    {
        private readonly String _filePath;

        private Ini _output;
        private Dictionary<String, SectionBinding> _sections;
        private Dictionary<String, IniValue> _values;
        private SectionBinding _currentSectionBinding;
        private IniSection _currentSection;
        private Dictionary<String, IniSection> _sectionsRead;
        private IniValue _currnetValue;
        private IniValue<Boolean> _enabledValue;
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
                _sectionsRead.Clear();
                _output = output;
                _sections = output.GetBindings().ToDictionary(s => s.Name);
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
                // Read configuration files inside the different mod sections
                IniSection modSection;
                if (_sectionsRead.TryGetValue("Mod", out modSection))
                    if (modSection.Enabled)
                    {
                        String[] ModFolders = null;
                        foreach (IniValue v in modSection.GetValues())
                            if (String.Compare(v.Name, "FolderNames") == 0)
                            {
                                IniArray<String> vs = v as IniArray<String>;
                                if (vs != null)
                                {
                                    // Consider that only the main configuration file can have a "[Mod] FolderNames" field (others are ignored as it is)
                                    ModFolders = new String[vs.Value.Length];
                                    vs.Value.CopyTo(ModFolders, 0);
                                    break;
                                }
                            }
                        if (ModFolders != null)
                            for (Int32 i = ModFolders.Length-1; i >= 0; --i) // Read in reverse order so that first folder's changes overwrite the others
                            {
                                String subConfigPath = ModFolders[i] + Path.DirectorySeparatorChar + _filePath;
                                if (File.Exists(subConfigPath))
                                {
                                    using (Stream input = File.OpenRead(subConfigPath))
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
                            }
                    }
            }
            finally
            {
                FlushSection();
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

            if (_enabledValue == null && _currnetValue.Name == nameof(IniSection.Enabled))
                _enabledValue = (IniValue<Boolean>)_currnetValue;
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
                    FlushSection();

                    String sectionName = _sb.ToString().TrimEnd("Section", StringComparison.InvariantCulture);
                    if (_sections.TryGetValue(sectionName, out _currentSectionBinding))
                    {
                        try
                        {
                            if (!_sectionsRead.TryGetValue(_currentSectionBinding.Name, out _currentSection))
                                _currentSection = _currentSectionBinding.CreateSection();
                            _values = _currentSection.GetValues().ToDictionary(v => v.Name);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Failed to load data for the section [{0}]", _currentSection.Name);
                            _values = new Dictionary<String, IniValue>();
                        }
                    }

                    _enabledValue = null;
                    break;
                }
                _sb.Append(ch);
            }
        }

        private void FlushSection()
        {
            if (_currentSection != null && _currentSectionBinding != null)
            {
                if (!_currentSection.Enabled.Value)
                    _currentSection.Reset();

                _currentSectionBinding.UpdateSection(_currentSection);
                _sectionsRead[_currentSection.Name] = _currentSection;
            }
        }
    }
}
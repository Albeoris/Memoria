using System;
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

		public Boolean HasOption(String optionName)
		{
			return _optionsOn.Contains(optionName);
		}

		public void AddOption(String optionName)
		{
			_optionsOn.Add(optionName);
		}

		public Boolean ParseLine(String line)
		{
			String[] code = line.Split(INSTRUCTION_SEP, 2, StringSplitOptions.None);
			if (code.Length == 0)
				return false;
			if (code.Length == 1)
			{
				if (code[0].StartsWith("-"))
					_optionsOn.Remove(code[0].Substring(1));
				else if (code[0].StartsWith("+"))
					_optionsOn.Add(code[0].Substring(1));
				else
					_optionsOn.Add(code[0]);
				return true;
			}
			if (String.Compare(code[0], "VERSION") == 0)
			{
				_version = new Version(code[1]);
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
			return lines.ToArray();
		}

		// Maybe we might want to add more flexibility in the future, like "Int32 GetParameterInt(String paramName)" or things like that
		private static readonly String[] INSTRUCTION_SEP = new[] { ":" };
		private HashSet<String> _optionsOn = new HashSet<String>();
		private Version _version = new Version();
	}
}

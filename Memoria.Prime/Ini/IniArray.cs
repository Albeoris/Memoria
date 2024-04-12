using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Memoria.Prime.Ini
{
	public sealed class IniArray<T> : IniValue
	{
		private readonly IniTryParseValue<T> _parser;
		private readonly IniFormatValue<T> _formatter;

		public T[] Value;

		public IniArray(String name, IniTryParseValue<T> parser, IniFormatValue<T> formatter)
			: base(name)
		{
			_parser = parser;
			_formatter = formatter;
		}

		public override void ParseValue(String rawString)
		{
			if (string.IsNullOrEmpty(rawString))
			{
				Value = new T[0];
				return;
			}

			if (rawString.Length < 2)
			{
				Log.Warning("Failed to parse a value [{0}] of the type [{1}] from the string [{2}].", Name, typeof(T[]).Name, rawString);
				return;
			}

			MatchCollection argMatches = new Regex(typeof(T) == typeof(String) ? @"""([^""]*)""\s*(,|$)" : @"([^,]*)\s*(,|$)").Matches(rawString);
			List<T> result = new List<T>();
			for (Int32 i = 0; i < argMatches.Count; i++)
			{
				T value;
				String token = argMatches[i].Groups[1].Value;
				if (typeof(T) != typeof(String))
					token = token.Trim();
				if (token.Length == 0)
					continue;
				if (!_parser(token, out value))
					return;

				result.Add(value);
			}

			Value = result.ToArray();
		}

		public override void WriteValue(StreamWriter sw)
		{
			if (Value.IsNullOrEmpty())
				return;

			String enclose = typeof(T) == typeof(String) ? "\"" : string.Empty;
			sw.Write(enclose);
			sw.Write(string.Join($"{enclose}, {enclose}", Value.Select(v => _formatter(v)).ToArray()));
			sw.Write(enclose);
		}

		public static implicit operator T[](IniArray<T> handler)
		{
			return handler.Value;
		}
	}
}

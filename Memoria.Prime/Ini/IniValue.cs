using Memoria.Prime.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Memoria.Prime.Ini
{
	public abstract class IniValue
	{
		public readonly String Name;

		protected IniValue(String name)
		{
			Name = name;
		}

		public abstract void ParseValue(String rawString);
		public abstract void WriteValue(StreamWriter sw);

		public void WriteLine(StreamWriter sw)
		{
			sw.Write(Name);
			sw.Write(" = ");
			WriteValue(sw);
			sw.WriteLine();
		}

		public static IniValue<String> String(String name)
		{
			return new IniValue<String>(name, TryParseString, FormatString);
		}

		public static IniValue<String> Path(String name)
		{
			return new IniValue<String>(name, TryParsePath, FormatPath);
		}

		public static IniValue<Boolean> Boolean(String name)
		{
			return new IniValue<Boolean>(name, TryParseBoolean, FormatBoolean);
		}

		public static IniValue<Int32> Int32(String name)
		{
			return new IniValue<Int32>(name, TryParseInt32, FormatInt32);
		}

		public static IniArray<Int32> Int32Array(String name)
		{
			return new IniArray<Int32>(name, TryParseInt32, FormatInt32);
		}

		public static IniSet<Int32> Int32Set(String name)
		{
			return new IniSet<Int32>(name, TryParseInt32, FormatInt32);
		}

		public static IniArray<String> StringArray(String name)
		{
			return new IniArray<String>(name, TryParseString, FormatString);
		}

		public static IniArray<T> Array<T>(String name, IniTryParseValue<T> parser, IniFormatValue<T> formatter)
		{
			return new IniArray<T>(name, parser, formatter);
		}

		private static Boolean TryParseString(String rawstring, out String value)
		{
			value = rawstring;
			return true;
		}

		private static String FormatString(String value)
		{
			return value;
		}

		private static readonly KeyValuePair<String, TextReplacement>[] PathReplacement = new Dictionary<String, TextReplacement>
		{
			{"%DataPath%", Application.dataPath},
			{"%PersistentDataPath%", Application.persistentDataPath},
			{"%TemporaryCachePath%", Application.temporaryCachePath},
			{"%StreamingAssets%", GetStreamingAssetsPath()}
		}.ToArray();

		private static String GetStreamingAssetsPath()
		{
			return Application.platform == RuntimePlatform.WindowsPlayer
				? "./StreamingAssets"
				: Application.streamingAssetsPath;
		}

		internal static Boolean TryParsePath(String rawstring, out String value)
		{
			if (!System.String.IsNullOrEmpty(rawstring))
			{
				rawstring = rawstring.ReplaceAll(PathReplacement);
			}

			value = rawstring;
			return true;
		}

		private static String FormatPath(String value)
		{
			return value;
		}

		private static Boolean TryParseInt32(String rawstring, out Int32 value)
		{
			return System.Int32.TryParse(rawstring, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
		}

		private static String FormatInt32(Int32 value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		private static Boolean TryParseBoolean(String rawstring, out Boolean value)
		{
			value = false;
			if (rawstring.Length != 1)
				return false;

			switch (rawstring[0])
			{
				case '1':
					value = true;
					return true;
				case '0':
					return true;
				default:
					return false;
			}
		}

		private static String FormatBoolean(Boolean value)
		{
			return value ? "1" : "0";
		}
	}

	public sealed class IniValue<T> : IniValue
	{
		private readonly IniTryParseValue<T> _parser;
		private readonly IniFormatValue<T> _formatter;

		public T Value;

		public IniValue(String name, IniTryParseValue<T> parser, IniFormatValue<T> formatter)
			: base(name)
		{
			_parser = parser;
			_formatter = formatter;
		}

		public override void ParseValue(String rawString)
		{
			T value;
			if (_parser(rawString, out value))
				Value = value;
			else
				Log.Warning("Failed to parse a value [{0}] of the type [{1}] from the string [{2}].", Name, typeof(T).Name, rawString);
		}

		public override void WriteValue(StreamWriter sw)
		{
			sw.Write(_formatter(Value));
		}

		public static implicit operator T(IniValue<T> handler)
		{
			return handler.Value;
		}
	}
}

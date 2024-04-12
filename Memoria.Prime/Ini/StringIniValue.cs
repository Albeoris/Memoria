using System;
using System.IO;

namespace Memoria.Prime.Ini
{
	public sealed class StringIniValue : IniValue
	{
		public String Value;

		public StringIniValue(String name)
			: base(name)
		{
		}

		public override void ParseValue(String rawString)
		{
			Value = rawString;
		}

		public override void WriteValue(StreamWriter sw)
		{
			sw.Write(Value);
		}
	}
}

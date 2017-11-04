using System;
using System.IO;
using System.Linq;

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

            String[] list = rawString.Trim('"').Split(new[] {"\", \""}, StringSplitOptions.None);
            T[] result = new T[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                T value;
                if (!_parser(list[i], out value))
                    return;

                result[i] = value;
            }

            Value = result;
        }

        public override void WriteValue(StreamWriter sw)
        {
            if (Value.IsNullOrEmpty())
                return;

            sw.Write('"');
            sw.Write(string.Join("\", \"", Value.Select(v => _formatter(v)).ToArray()));
            sw.Write('"');
        }

        public static implicit operator T[](IniArray<T> handler)
        {
            return handler.Value;
        }
    }
}
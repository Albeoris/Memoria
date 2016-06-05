using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Memoria
{
    public sealed class CsvWriter : IDisposable
    {
        private readonly StreamWriter _sw;
        private readonly StringBuilder _sb = new StringBuilder();

        public CsvWriter(String filePath)
        {
            _sw = File.CreateText(filePath);
        }

        public void Dispose()
        {
            _sw.Dispose();
        }

        public void WriteLine(String line)
        {
            _sw.WriteLine(line);
        }

        public void WriteEntry(ICsvEntry entry, String comment)
        {
            entry.WriteEntry(this);
            if (!System.String.IsNullOrEmpty(comment))
                String("# " + comment);

            if (_sb.Length > 0)
            {
                _sw.WriteLine(_sb.ToString());
                _sb.Length = 0;
            }
        }

        public void Byte(Byte value)
        {
            String(value.ToString(CultureInfo.InvariantCulture));
        }

        public void UInt16(UInt16 value)
        {
            String(value.ToString(CultureInfo.InvariantCulture));
        }

        public void ByteArray(Byte[] array)
        {
            String(System.String.Join(", ", array.Select(v => v.ToString(CultureInfo.InvariantCulture)).ToArray()));
        }

        public void String(String value)
        {
            if (_sb.Length > 0)
                _sb.Append(';');

            if (value.IndexOf(';') > -1)
            {
                _sb.Append('"');
                _sb.Append(value);
                _sb.Append('"');
            }
            else
            {
                _sb.Append(value);
            }
        }
    }
}
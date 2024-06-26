using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Memoria.Prime.CSV
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

        public void WriteMetaData(CsvMetaData metadata)
        {
            String[] mdLines = metadata.GenerateLines();
            foreach (String line in mdLines)
                _sw.WriteLine("#!" + line);
        }

        public void WriteLine(String line)
        {
            _sw.WriteLine(line);
        }

        public void WriteEntry(ICsvEntry entry, CsvMetaData metadata, String comment)
        {
            entry.WriteEntry(this, metadata);
            if (!System.String.IsNullOrEmpty(comment))
                String("# " + comment);

            if (_sb.Length > 0)
            {
                _sw.WriteLine(_sb.ToString());
                _sb.Length = 0;
            }
        }

        public void Boolean(Boolean value)
        {
            String((value ? 1 : 0).ToString(CultureInfo.InvariantCulture));
        }

        public void SByte(SByte value)
        {
            String(value.ToString(CultureInfo.InvariantCulture));
        }

        public void Byte(Byte value)
        {
            String(value.ToString(CultureInfo.InvariantCulture));
        }

        public void ByteOrMinusOne(Byte value)
        {
            if (value == System.Byte.MaxValue)
                String("-1");
            else
                String(value.ToString(CultureInfo.InvariantCulture));
        }

        public void Item(Int32 value)
        {
            if (value == System.Byte.MaxValue)
                String("-1");
            else
                String(value.ToString(CultureInfo.InvariantCulture));
        }

        public void AnyAbility(Int32 value)
        {
            Int32 poolNum = value / 256;
            Int32 idInPool = value % 256;
            if (idInPool < 192)
                String("AA:" + (poolNum * 192 + idInPool).ToString(CultureInfo.InvariantCulture));
            else
                String("SA:" + (poolNum * 64 + (idInPool - 192)).ToString(CultureInfo.InvariantCulture));
        }

        public void Int16(Int16 value)
        {
            String(value.ToString(CultureInfo.InvariantCulture));
        }

        public void Int32(Int32 value)
        {
            String(value.ToString(CultureInfo.InvariantCulture));
        }

        public void UInt16(UInt16 value)
        {
            String(value.ToString(CultureInfo.InvariantCulture));
        }

        public void UInt16OrMinusOne(UInt16 value)
        {
            if (value == System.UInt16.MaxValue)
                String("-1");
            else
                String(value.ToString(CultureInfo.InvariantCulture));
        }

        public void UInt32(UInt32 value)
        {
            String(value.ToString(CultureInfo.InvariantCulture));
        }

        public void UInt64(UInt64 value)
        {
            String(value.ToString(CultureInfo.InvariantCulture));
        }

        public void Single(Single value)
        {
            String(value.ToString(CultureInfo.InvariantCulture));
        }

        public void ItemArray(Int32[] array)
        {
            String(System.String.Join(", ", array.Select(v => v == System.Byte.MaxValue ? "-1" : v.ToString(CultureInfo.InvariantCulture)).ToArray()));
        }

        public void AnyAbilityArray(Int32[] array)
        {
            Func<Int32, String> toStringFunc = (Int32 value) =>
            {
                Int32 poolNum = value / 256;
                Int32 idInPool = value % 256;
                if (idInPool < 192)
                    return "AA:" + (poolNum * 192 + idInPool).ToString(CultureInfo.InvariantCulture);
                else
                    return "SA:" + (poolNum * 64 + (idInPool - 192)).ToString(CultureInfo.InvariantCulture);
            };
            String(System.String.Join(", ", array.Select(toStringFunc).ToArray()));
        }

        public void ByteArray(Byte[] array)
        {
            String(System.String.Join(", ", array.Select(v => v.ToString(CultureInfo.InvariantCulture)).ToArray()));
        }

        public void SByteArray(SByte[] array)
        {
            String(System.String.Join(", ", array.Select(v => v.ToString(CultureInfo.InvariantCulture)).ToArray()));
        }

        public void Int32Array(Int32[] array)
        {
            String(System.String.Join(", ", array.Select(v => v.ToString(CultureInfo.InvariantCulture)).ToArray()));
        }

        public void SingleArray(Single[] array)
        {
            String(System.String.Join(", ", array.Select(v => v.ToString(CultureInfo.InvariantCulture)).ToArray()));
        }

        public void EnumValue<T>(T value) where T : struct
        {
            UInt64 integer = EnumCache<T>.ToUInt64(value);

            if (_sb.Length > 0)
                _sb.Append(';');

            _sb.Append(value);
            _sb.Append('(');
            _sb.Append(integer.ToString(CultureInfo.InvariantCulture));
            _sb.Append(')');
        }

        public void String(String value)
        {
            if (value == null)
                value = "<null>";
            else if (value == System.String.Empty)
                value = "\"\"";

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

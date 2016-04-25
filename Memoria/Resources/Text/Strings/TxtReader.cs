using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Memoria
{
    public sealed class TxtReader
    {
        private readonly Stream _input;
        private readonly ITxtFormatter _formatter;

        public TxtReader(Stream input, ITxtFormatter formatter)
        {
            _input = input;
            _formatter = formatter;
        }

        public TxtEntry[] Read(out String name)
        {
            using (StreamReader sr = new StreamReader(_input, Encoding.UTF8, true, 4096))
            {
                name = sr.ReadLine();
                if (_formatter is StringsFormatter) // TEMP
                    name = name.Substring(2, name.Length - 4);

                string countStr = sr.ReadLine();
                if (_formatter is StringsFormatter) // TEMP
                    countStr = countStr.Substring(2, countStr.Length - 4);
                int count = int.Parse(countStr, CultureInfo.InvariantCulture);
                TxtEntry[] result = new TxtEntry[count];

                int offset = 0;
                for (int i = 0; i < count && !sr.EndOfStream; i++)
                {
                    TxtEntry entry = _formatter.Read(sr);
                    if (entry == null)
                    {
                        offset++;
                        continue;
                    }

                    if (String.IsNullOrEmpty(entry.Prefix))
                    {
                        Log.Warning("Неверная запись [Line: {0}, Value: {1}] в файле: {2}", i, entry, name);
                        offset++;
                        continue;
                    }

                    result[i - offset] = entry;
                }

                return result;
            }
        }
    }
}
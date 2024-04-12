using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Memoria.Assets
{
	public sealed class TxtWriter
	{
		private readonly Stream _output;
		private readonly ITxtFormatter _formatter;

		public TxtWriter(Stream output, ITxtFormatter formatter)
		{
			_output = output;
			_formatter = formatter;
		}

		public void Write(String name, IList<TxtEntry> entries)
		{
			using (StreamWriter sw = new StreamWriter(_output, Encoding.UTF8, 4096))
			{
				if (_formatter is StringsFormatter) // TEMP
				{
					sw.WriteLine("/*");
					sw.WriteLine(name);
					sw.WriteLine(entries.Count.ToString("D4", CultureInfo.InvariantCulture));
					sw.WriteLine("*/");
				}
				else
				{
					sw.WriteLine(name);
					sw.WriteLine(entries.Count.ToString("D4", CultureInfo.InvariantCulture));
				}

				for (Int32 i = 0; i < entries.Count; i++)
				{
					TxtEntry entry = entries[i];
					entry.TryUpdateIndex(i);
					_formatter.Write(sw, entry);
				}
			}
		}

		public static void WriteStrings(String outputPath, IList<TxtEntry> entries)
		{
			using (FileStream output = File.Create(outputPath))
				new TxtWriter(output, StringsFormatter.Instance).Write(outputPath, entries);
		}
	}
}

using Memoria.Prime;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Memoria.Assets
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
			name = null;

			using (StreamReader sr = new StreamReader(_input, Encoding.UTF8, true, 4096))
			{
				String countStr = null;

				if (_formatter is StringsFormatter) // TEMP
				{
					Boolean comment = false;
					for (int i = 0; i < 2 || comment;)
					{
						String value = sr.ReadLine();
						if (value.StartsWith("/*"))
						{
							comment = true;
							value = value.Substring(2);
						}
						if (value.EndsWith("*/"))
						{
							comment = false;
							value = value.Substring(0, value.Length - 2);
						}
						value = value.Trim();

						if (!String.IsNullOrEmpty(value))
						{
							if (i == 0)
								name = value;
							else
								countStr = value;

							i++;
						}
					}
				}
				else
				{
					name = sr.ReadLine();
					countStr = sr.ReadLine();
				}

				Int32 count = Int32.Parse(countStr, CultureInfo.InvariantCulture);
				TxtEntry[] result = new TxtEntry[count];

				Int32 offset = 0;
				for (Int32 i = 0; i < count && !sr.EndOfStream; i++)
				{
					TxtEntry entry;
					try
					{
						entry = _formatter.Read(sr);
					}
					catch (Exception ex)
					{
						if (i > 0)
						{
							TxtEntry previous = result[i - 1];
							throw new Exception($"Cannot read {i} entry from {name}.\r\nPrevious: [Index: {previous.Index}, Pefix: {previous.Prefix}, Value: {previous.Value}]", ex);
						}
						else
						{
							throw new Exception($"Cannot read {i} entry from {name}.", ex);
						}
					}
					if (entry == null)
					{
						offset++;
						continue;
					}

					if (String.IsNullOrEmpty(entry.Prefix))
					{
						Log.Warning("Invalid record [Line: {0}, Value: {1}] in the file: {2}", i, entry, name);
						offset++;
						continue;
					}

					result[i - offset] = entry;
				}

				return result;
			}
		}

		public static TxtEntry[] ReadStrings(String inputPath)
		{
			String name;
			using (FileStream output = File.OpenRead(inputPath))
				return new TxtReader(output, StringsFormatter.Instance).Read(out name);
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ByteReader
{
	public ByteReader(Byte[] bytes)
	{
		this.mBuffer = bytes;
	}

	public ByteReader(TextAsset asset)
	{
		this.mBuffer = asset.bytes;
	}

	public static ByteReader Open(String path)
	{
		FileStream fileStream = File.OpenRead(path);
		if (fileStream != null)
		{
			fileStream.Seek(0L, SeekOrigin.End);
			Byte[] array = new Byte[fileStream.Position];
			fileStream.Seek(0L, SeekOrigin.Begin);
			fileStream.Read(array, 0, (Int32)array.Length);
			fileStream.Close();
			return new ByteReader(array);
		}
		return (ByteReader)null;
	}

	public Boolean canRead
	{
		get
		{
			return this.mBuffer != null && this.mOffset < (Int32)this.mBuffer.Length;
		}
	}

	private static String ReadLine(Byte[] buffer, Int32 start, Int32 count)
	{
		return Encoding.UTF8.GetString(buffer, start, count);
	}

	public String ReadLine()
	{
		return this.ReadLine(true);
	}

	public String ReadLine(Boolean skipEmptyLines)
	{
		Int32 length = this.mBuffer.Length;
		if (skipEmptyLines)
		{
			while (this.mOffset < length && (Int32)this.mBuffer[this.mOffset] < 32)
				++this.mOffset;
		}
		Int32 mOffset = this.mOffset;
		if (mOffset < length)
		{
			while (mOffset < length)
			{
				switch (this.mBuffer[mOffset++])
				{
					case 10:
					case 13:
						goto label_7;
					default:
						continue;
				}
			}
			++mOffset;
		label_7:
			String str = ByteReader.ReadLine(this.mBuffer, this.mOffset, mOffset - this.mOffset - 1);
			this.mOffset = mOffset;
			return str;
		}
		this.mOffset = length;
		return (String)null;
	}

	public Dictionary<String, String> ReadDictionary()
	{
		Dictionary<String, String> dictionary = new Dictionary<String, String>();
		Char[] separator = new Char[]
		{
			'='
		};
		while (this.canRead)
		{
			String text = this.ReadLine();
			if (text == null)
			{
				break;
			}
			if (!text.StartsWith("//"))
			{
				String[] array = text.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
				if ((Int32)array.Length == 2)
				{
					String key = array[0].Trim();
					String value = array[1].Trim().Replace("\\n", "\n");
					dictionary[key] = value;
				}
			}
		}
		return dictionary;
	}

	public BetterList<String> ReadCSV()
	{
		ByteReader.mTemp.Clear();
		String text = String.Empty;
		Boolean flag = false;
		Int32 num = 0;
		while (this.canRead)
		{
			if (flag)
			{
				String text2 = this.ReadLine(false);
				if (text2 == null)
				{
					return null;
				}
				text2 = text2.Replace("\\n", "\n");
				text = text + "\n" + text2;
			}
			else
			{
				text = this.ReadLine(true);
				if (text == null)
				{
					return null;
				}
				text = text.Replace("\\n", "\n");
				num = 0;
			}
			Int32 i = num;
			Int32 length = text.Length;
			while (i < length)
			{
				Char c = text[i];
				if (c == ',')
				{
					if (!flag)
					{
						ByteReader.mTemp.Add(text.Substring(num, i - num));
						num = i + 1;
					}
				}
				else if (c == '"')
				{
					if (flag)
					{
						if (i + 1 >= length)
						{
							ByteReader.mTemp.Add(text.Substring(num, i - num).Replace("\"\"", "\""));
							return ByteReader.mTemp;
						}
						if (text[i + 1] != '"')
						{
							ByteReader.mTemp.Add(text.Substring(num, i - num).Replace("\"\"", "\""));
							flag = false;
							if (text[i + 1] == ',')
							{
								i++;
								num = i + 1;
							}
						}
						else
						{
							i++;
						}
					}
					else
					{
						num = i + 1;
						flag = true;
					}
				}
				i++;
			}
			if (num < text.Length)
			{
				if (flag)
				{
					continue;
				}
				ByteReader.mTemp.Add(text.Substring(num, text.Length - num));
			}
			return ByteReader.mTemp;
		}
		return null;
	}

	private Byte[] mBuffer;

	private Int32 mOffset;

	private static BetterList<String> mTemp = new BetterList<String>();
}

using System;
using System.Collections.Generic;
using System.IO;

public class WMLogger
{
	public WMLogger(String filePath)
	{
		this.filePath = filePath;
	}

	public void WriteToFile()
	{
		using (StreamWriter streamWriter = new StreamWriter(this.filePath))
		{
			foreach (String value in this.messages)
			{
				streamWriter.WriteLine(value);
			}
		}
	}

	public void Log(String message)
	{
		this.messages.Add(message);
	}

	private readonly List<String> messages = new List<String>();

	private readonly String filePath;
}

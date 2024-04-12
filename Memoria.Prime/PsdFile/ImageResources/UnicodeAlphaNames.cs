/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2014 Tao Yue
//

//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace Memoria.Prime.PsdFile
{
	/// <summary>
	/// The names of the alpha channels.
	/// </summary>
	public class UnicodeAlphaNames : ImageResource
	{
		public override ResourceID ID
		{
			get { return ResourceID.UnicodeAlphaNames; }
		}

		private List<String> _channelNames = new List<String>();
		public List<String> ChannelNames
		{
			get { return _channelNames; }
		}

		public UnicodeAlphaNames()
		  : base(String.Empty)
		{
		}

		public UnicodeAlphaNames(PsdBinaryReader reader, String name, Int32 resourceDataLength)
		  : base(name)
		{
			var endPosition = reader.BaseStream.Position + resourceDataLength;

			while (reader.BaseStream.Position < endPosition)
			{
				var channelName = reader.ReadUnicodeString();

				// Photoshop writes out a null terminator for Unicode alpha names.
				// There is no null terminator on other Unicode strings in PSD files.
				if (channelName.EndsWith("\0"))
				{
					channelName = channelName.Substring(0, channelName.Length - 1);
				}
				ChannelNames.Add(channelName);
			}
		}

		protected override void WriteData(PsdBinaryWriter writer)
		{
			foreach (var channelName in ChannelNames)
			{
				// We must add a null terminator because Photoshop always strips the
				// last character of a Unicode alpha name, even if it is not null.
				writer.WriteUnicodeString(channelName + "\0");
			}
		}
	}
}

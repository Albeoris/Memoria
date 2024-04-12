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
using System.Diagnostics;

namespace Memoria.Prime.PsdFile
{
	[DebuggerDisplay("Layer Info: { _key }")]
	public class RawLayerInfo : LayerInfo
	{
		private String _signature;
		public override String Signature
		{
			get { return _signature; }
		}

		private String _key;
		public override String Key
		{
			get { return _key; }
		}

		public Byte[] Data { get; private set; }

		public RawLayerInfo(String key, String signature = "8BIM")
		{
			_signature = signature;
			_key = key;
		}

		public RawLayerInfo(PsdBinaryReader reader, String signature, String key,
		  Int64 dataLength)
		{
			_signature = signature;
			_key = key;

			Util.CheckByteArrayLength(dataLength);
			Data = reader.ReadBytes((Int32)dataLength);
		}

		protected override void WriteData(PsdBinaryWriter writer)
		{
			writer.Write(Data);
		}
	}
}

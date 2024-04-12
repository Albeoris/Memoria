/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2016 Tao Yue
//

//
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace Memoria.Prime.PsdFile
{
	public static class LayerInfoFactory
	{
		/// <summary>
		/// Loads the next LayerInfo record.
		/// </summary>
		/// <param name="reader">The file reader</param>
		/// <param name="psdFile">The PSD file.</param>
		/// <param name="globalLayerInfo">True if the LayerInfo record is being
		///   loaded from the end of the Layer and Mask Information section;
		///   false if it is being loaded from the end of a Layer record.</param>
		public static LayerInfo Load(PsdBinaryReader reader, PsdFile psdFile,
		  Boolean globalLayerInfo)
		{
			Util.DebugMessage(reader.BaseStream, "Load, Begin, LayerInfo");

			// Some keys use a signature of 8B64, but the identity of these keys
			// is undocumented.  We will therefore accept either signature.
			var signature = reader.ReadAsciiChars(4);
			if ((signature != "8BIM") && (signature != "8B64"))
			{
				throw new PsdInvalidException(
				  $"{nameof(LayerInfo)} signature invalid, must be 8BIM or 8B64.");
			}

			var key = reader.ReadAsciiChars(4);
			var hasLongLength = LayerInfoUtil.HasLongLength(key, psdFile.IsLargeDocument);
			var length = hasLongLength
			  ? reader.ReadInt64()
			  : reader.ReadInt32();
			var startPosition = reader.BaseStream.Position;

			LayerInfo result;
			switch (key)
			{
				case "Layr":
				case "Lr16":
				case "Lr32":
					result = new InfoLayers(reader, psdFile, key, length);
					break;
				case "lsct":
				case "lsdk":
					result = new LayerSectionInfo(reader, key, (Int32)length);
					break;
				case "luni":
					result = new LayerUnicodeName(reader);
					break;
				default:
					result = new RawLayerInfo(reader, signature, key, length);
					break;
			}

			// May have additional padding applied.
			var endPosition = startPosition + length;
			if (reader.BaseStream.Position < endPosition)
				reader.BaseStream.Position = endPosition;

			// Documentation states that the length is even-padded.  Actually:
			//   1. Most keys have 4-padded lengths.
			//   2. However, some keys (LMsk) have even-padded lengths.
			//   3. Other keys (Txt2, Lr16, Lr32) have unpadded lengths.
			//
			// Photoshop writes data that is always 4-padded, even when the stated
			// length is not a multiple of 4.  The length mismatch seems to occur
			// only on global layer info.  We do not read extra padding in other
			// cases because third-party programs are likely to follow the spec.

			if (globalLayerInfo)
			{
				reader.ReadPadding(startPosition, 4);
			}

			Util.DebugMessage(reader.BaseStream, "Load, End, LayerInfo, {0}, {1}",
			  result.Signature, result.Key);
			return result;
		}
	}

	internal static class LayerInfoUtil
	{
		internal static Boolean HasLongLength(String key, Boolean isLargeDocument)
		{
			if (!isLargeDocument)
			{
				return false;
			}

			switch (key)
			{
				case "LMsk":
				case "Lr16":
				case "Lr32":
				case "Layr":
				case "Mt16":
				case "Mt32":
				case "Mtrn":
				case "Alph":
				case "FMsk":
				case "lnk2":
				case "FEid":
				case "FXid":
				case "PxSD":
					return true;

				default:
					return false;
			}
		}
	}

	public abstract class LayerInfo
	{
		public abstract String Signature { get; }

		public abstract String Key { get; }

		protected abstract void WriteData(PsdBinaryWriter writer);

		public void Save(PsdBinaryWriter writer, Boolean globalLayerInfo,
		  Boolean isLargeDocument)
		{
			Util.DebugMessage(writer.BaseStream, "Save, Begin, LayerInfo, {0}, {1}",
			  Signature, Key);

			writer.WriteAsciiChars(Signature);
			writer.WriteAsciiChars(Key);

			var startPosition = writer.BaseStream.Position;
			using (var lengthWriter = new PsdBlockLengthWriter(writer,
			  LayerInfoUtil.HasLongLength(Key, isLargeDocument)))
			{
				// Depending on the key, the length may be unpadded, 2-padded, or
				// 4-padded.  Thus, it is up to each implementation of WriteData to
				// pad the length correctly.
				WriteData(writer);
			}

			// Data for global layer info is always padded to a multiple of 4,
			// even if this causes the stated length to be incorrect.
			if (globalLayerInfo)
			{
				writer.WritePadding(startPosition, 4);
			}

			Util.DebugMessage(writer.BaseStream, "Save, End, LayerInfo, {0}, {1}",
			  Signature, Key);
		}
	}
}

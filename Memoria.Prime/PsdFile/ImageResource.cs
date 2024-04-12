/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2015 Tao Yue
//
// Portions of this file are provided under the BSD 3-clause License:
//   Copyright (c) 2006, Jonas Beckeman
//

//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Memoria.Prime.PsdFile
{
	public enum ResourceID
	{
		Undefined = 0,
		MacPrintInfo = 1001,
		ResolutionInfo = 1005,
		AlphaChannelNames = 1006,
		DisplayInfo = 1007,
		Caption = 1008,
		BorderInfo = 1009,
		BackgroundColor = 1010,
		PrintFlags = 1011,
		MultichannelHalftoneInfo = 1012,
		ColorHalftoneInfo = 1013,
		DuotoneHalftoneInfo = 1014,
		MultichannelTransferFunctions = 1015,
		ColorTransferFunctions = 1016,
		DuotoneTransferFunctions = 1017,
		DuotoneImageInfo = 1018,
		BlackWhiteRange = 1019,
		EpsOptions = 1021,
		QuickMaskInfo = 1022,
		LayerStateInfo = 1024,
		WorkingPathUnsaved = 1025,
		LayersGroupInfo = 1026,
		IptcNaa = 1028,
		RawFormatImageMode = 1029,
		JpegQuality = 1030,
		GridGuidesInfo = 1032,
		ThumbnailBgr = 1033,
		CopyrightInfo = 1034,
		Url = 1035,
		ThumbnailRgb = 1036,
		GlobalAngle = 1037,
		ColorSamplersObsolete = 1038,
		IccProfile = 1039,
		Watermark = 1040,
		IccUntagged = 1041,
		EffectsVisible = 1042,
		SpotHalftone = 1043,
		DocumentSpecific = 1044,
		UnicodeAlphaNames = 1045,
		IndexedColorTableCount = 1046,
		TransparentIndex = 1047,
		GlobalAltitude = 1049,
		Slices = 1050,
		WorkflowUrl = 1051,
		JumpToXpep = 1052,
		AlphaIdentifiers = 1053,
		UrlList = 1054,
		VersionInfo = 1057,
		ExifData1 = 1058,
		ExifData3 = 1059,
		XmpMetadata = 1060,
		CaptionDigest = 1061,
		PrintScale = 1062,
		PixelAspectRatio = 1064,
		LayerComps = 1065,
		AlternateDuotoneColors = 1066,
		AlternateSpotColors = 1067,
		LayerSelectionIDs = 1069,
		HdrToningInfo = 1070,
		PrintInfo = 1071,
		LayerGroupsEnabled = 1072,
		ColorSamplers = 1073,
		MeasurementScale = 1074,
		TimelineInfo = 1075,
		SheetDisclosure = 1076,
		FloatDisplayInfo = 1077,
		OnionSkins = 1078,
		CountInfo = 1080,
		PrintSettingsInfo = 1082,
		PrintStyle = 1083,
		MacNsPrintInfo = 1084,
		WinDevMode = 1085,
		AutoSaveFilePath = 1086,
		AutoSaveFormat = 1087,
		PathInfo = 2000,  // 2000-2999: Path Information
		ClippingPathName = 2999,
		LightroomWorkflow = 8000,
		PrintFlagsInfo = 10000
	}

	/// <summary>
	/// Abstract class for Image Resources
	/// </summary>
	public abstract class ImageResource
	{
		private String _signature;
		public String Signature
		{
			get { return _signature; }
			set
			{
				if (value.Length != 4)
				{
					throw new ArgumentException(
					  $"{nameof(Signature)} must be 4 characters in length.");
				}
				_signature = value;
			}
		}

		public String Name { get; set; }

		public abstract ResourceID ID { get; }

		protected ImageResource(String name)
		{
			Signature = "8BIM";
			Name = name;
		}

		/// <summary>
		/// Write out the image resource block: header and data.
		/// </summary>
		public void Save(PsdBinaryWriter writer)
		{
			Util.DebugMessage(writer.BaseStream, "Save, Begin, ImageResource");

			writer.WriteAsciiChars(Signature);
			writer.Write((UInt16)ID);
			writer.WritePascalString(Name, 2);

			// Length is unpadded, but data is even-padded
			var startPosition = writer.BaseStream.Position;
			using (new PsdBlockLengthWriter(writer))
			{
				WriteData(writer);
			}
			writer.WritePadding(startPosition, 2);

			Util.DebugMessage(writer.BaseStream, "Save, End, ImageResource, {0}", ID);
		}

		/// <summary>
		/// Write the data for this image resource.
		/// </summary>
		protected abstract void WriteData(PsdBinaryWriter writer);

		public override String ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "{0} {1}", ID, Name);
		}
	}

	/// <summary>
	/// Creates the appropriate subclass of ImageResource.
	/// </summary>
	public static class ImageResourceFactory
	{
		public static ImageResource CreateImageResource(PsdBinaryReader reader)
		{
			Util.DebugMessage(reader.BaseStream, "Load, Begin, ImageResource");

			var signature = reader.ReadAsciiChars(4);
			var resourceIdInt = reader.ReadUInt16();
			var name = reader.ReadPascalString(2);
			var dataLength = (Int32)reader.ReadUInt32();

			var dataPaddedLength = Util.RoundUp(dataLength, 2);
			var endPosition = reader.BaseStream.Position + dataPaddedLength;

			ImageResource resource = null;
			var resourceId = (ResourceID)resourceIdInt;
			switch (resourceId)
			{
				case ResourceID.ResolutionInfo:
					resource = new ResolutionInfo(reader, name);
					break;
				case ResourceID.AlphaChannelNames:
					resource = new AlphaChannelNames(reader, name, dataLength);
					break;
				case ResourceID.UnicodeAlphaNames:
					resource = new UnicodeAlphaNames(reader, name, dataLength);
					break;
				case ResourceID.VersionInfo:
					resource = new VersionInfo(reader, name);
					break;
				default:
					resource = new RawImageResource(reader, signature, resourceId, name, dataLength);
					break;
			}

			Util.DebugMessage(reader.BaseStream, "Load, End, ImageResource, {0}",
			  resourceId);

			// Reposition the reader if we do not consume the full resource block.
			// This takes care of the even-padding, and also preserves forward-
			// compatibility in case a resource block is later extended with
			// additional properties.
			if (reader.BaseStream.Position < endPosition)
				reader.BaseStream.Position = endPosition;

			// However, overruns are definitely an error.
			if (reader.BaseStream.Position > endPosition)
				throw new PsdInvalidException("Corruption detected in resource.");

			return resource;
		}
	}

	public class ImageResources : List<ImageResource>
	{
		public ImageResources() : base()
		{
		}

		public ImageResource Get(ResourceID id)
		{
			return Find(x => x.ID == id);
		}

		public void Set(ImageResource resource)
		{
			Predicate<ImageResource> matchId = delegate (ImageResource res)
			{
				return res.ID == resource.ID;
			};
			var itemIdx = this.FindIndex(matchId);
			var lastItemIdx = this.FindLastIndex(matchId);

			if (itemIdx == -1)
			{
				Add(resource);
			}
			else if (itemIdx != lastItemIdx)
			{
				RemoveAll(matchId);
				Insert(itemIdx, resource);
			}
			else
			{
				this[itemIdx] = resource;
			}
		}
	}
}

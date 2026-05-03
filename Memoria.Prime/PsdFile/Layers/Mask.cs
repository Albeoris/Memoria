/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2014 Tao Yue
//
// Portions of this file are provided under the BSD 3-clause License:
//   Copyright (c) 2006, Jonas Beckeman
//

//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Specialized;

namespace Memoria.Prime.PsdFile
{
    public class Mask
    {
        /// <summary>
        /// The layer to which this mask belongs
        /// </summary>
        public Layer Layer { get; private set; }

        /// <summary>
        /// The rectangle enclosing the mask.
        /// </summary>
        public Rectangle Rect { get; set; }

        private Byte _backgroundColor;
        public Byte BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                if ((value != 0) && (value != 255))
                    throw new PsdInvalidException("Mask background must be fully-opaque or fully-transparent.");
                _backgroundColor = value;
            }
        }

        private static Int32 s_positionVsLayerBit = BitVector32.CreateMask();
        private static Int32 s_disabledBit = BitVector32.CreateMask(s_positionVsLayerBit);
        private static Int32 s_invertOnBlendBit = BitVector32.CreateMask(s_disabledBit);

        private BitVector32 _flags;
        public BitVector32 Flags => _flags;

        /// <summary>
        /// If true, the position of the mask is relative to the layer.
        /// </summary>
        public Boolean PositionVsLayer
        {
            get { return _flags[s_positionVsLayerBit]; }
            set { _flags[s_positionVsLayerBit] = value; }
        }

        public Boolean Disabled
        {
            get { return _flags[s_disabledBit]; }
            set { _flags[s_disabledBit] = value; }
        }

        /// <summary>
        /// if true, invert the mask when blending.
        /// </summary>
        public Boolean InvertOnBlend
        {
            get { return _flags[s_invertOnBlendBit]; }
            set { _flags[s_invertOnBlendBit] = value; }
        }

        /// <summary>
        /// Mask image data.
        /// </summary>
        public Byte[] ImageData { get; set; }

        public Mask(Layer layer)
        {
            Layer = layer;
            _flags = new BitVector32();
        }

        public Mask(Layer layer, Rectangle rect, Byte color, BitVector32 flags)
        {
            Layer = layer;
            Rect = rect;
            BackgroundColor = color;
            _flags = flags;
        }
    }

    /// <summary>
    /// Mask info for a layer.  Contains both the layer and user masks.
    /// </summary>
    public class MaskInfo
    {
        public Mask LayerMask { get; set; }

        public Mask UserMask { get; set; }

        /// <summary>
        /// Construct MaskInfo with null masks.
        /// </summary>
        public MaskInfo()
        {
        }

        public MaskInfo(PsdBinaryReader reader, Layer layer)
        {
            Util.DebugMessage(reader.BaseStream, "Load, Begin, MaskInfo");

            var maskLength = reader.ReadUInt32();
            if (maskLength <= 0)
                return;

            var startPosition = reader.BaseStream.Position;
            var endPosition = startPosition + maskLength;

            // Read layer mask
            var rectangle = reader.ReadRectangle();
            var backgroundColor = reader.ReadByte();
            var flagsByte = reader.ReadByte();
            LayerMask = new Mask(layer, rectangle, backgroundColor, new BitVector32(flagsByte));

            // User mask is supplied separately when there is also a vector mask.
            if (maskLength == 36)
            {
                var userFlagsByte = reader.ReadByte();
                var userBackgroundColor = reader.ReadByte();
                var userRectangle = reader.ReadRectangle();
                UserMask = new Mask(layer, userRectangle, userBackgroundColor,
                  new BitVector32(userFlagsByte));
            }

            // 20-byte mask data will end with padding.
            reader.BaseStream.Position = endPosition;

            Util.DebugMessage(reader.BaseStream, "Load, End, MaskInfo");
        }

        ///////////////////////////////////////////////////////////////////////////

        public void Save(PsdBinaryWriter writer)
        {
            Util.DebugMessage(writer.BaseStream, "Save, Begin, MaskInfo");

            if (LayerMask == null)
            {
                writer.Write((UInt32)0);
                return;
            }

            using (new PsdBlockLengthWriter(writer))
            {
                writer.Write(LayerMask.Rect);
                writer.Write(LayerMask.BackgroundColor);
                writer.Write((Byte)LayerMask.Flags.Data);

                if (UserMask == null)
                {
                    // Pad by 2 bytes to make the block length 20
                    writer.Write((UInt16)0);
                }
                else
                {
                    writer.Write((Byte)UserMask.Flags.Data);
                    writer.Write(UserMask.BackgroundColor);
                    writer.Write(UserMask.Rect);
                }
            }

            Util.DebugMessage(writer.BaseStream, "Save, End, MaskInfo");
        }

    }
}

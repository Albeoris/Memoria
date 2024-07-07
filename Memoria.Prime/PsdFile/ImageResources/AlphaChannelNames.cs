/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is provided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2013 Tao Yue
//

//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace Memoria.Prime.PsdFile
{
    /// <summary>
    /// The names of the alpha channels
    /// </summary>
    public class AlphaChannelNames : ImageResource
    {
        public override ResourceID ID
        {
            get { return ResourceID.AlphaChannelNames; }
        }

        private List<String> _channelNames = new List<String>();
        public List<String> ChannelNames
        {
            get { return _channelNames; }
        }

        public AlphaChannelNames() : base(String.Empty)
        {
        }

        public AlphaChannelNames(PsdBinaryReader reader, String name, Int32 resourceDataLength)
          : base(name)
        {
            var endPosition = reader.BaseStream.Position + resourceDataLength;

            // Alpha channel names are Pascal strings, with no padding in-between.
            while (reader.BaseStream.Position < endPosition)
            {
                var channelName = reader.ReadPascalString(1);
                ChannelNames.Add(channelName);
            }
        }

        protected override void WriteData(PsdBinaryWriter writer)
        {
            foreach (var channelName in ChannelNames)
            {
                writer.WritePascalString(channelName, 1);
            }
        }
    }
}

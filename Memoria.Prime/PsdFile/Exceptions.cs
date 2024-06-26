/////////////////////////////////////////////////////////////////////////////////
//
// Photoshop PSD FileType Plugin for Paint.NET
// http://psdplugin.codeplex.com/
//
// This software is perovided under the MIT License:
//   Copyright (c) 2006-2007 Frank Blumenberg
//   Copyright (c) 2010-2012 Tao Yue
//

//
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace Memoria.Prime.PsdFile
{
    [Serializable]
    public class PsdInvalidException : Exception
    {
        public PsdInvalidException()
        {
        }

        public PsdInvalidException(String message)
          : base(message)
        {
        }
    }

    [Serializable]
    public class RleException : Exception
    {
        public RleException() { }

        public RleException(String message) : base(message) { }
    }
}

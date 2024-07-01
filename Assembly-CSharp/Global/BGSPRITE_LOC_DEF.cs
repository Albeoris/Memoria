using System;
using System.IO;
using UnityEngine;

/// <summary>Class for individual tiles in an overlay</summary>
public class BGSPRITE_LOC_DEF
{
    public void ReadData_BGSPRITE_DEF(BinaryReader reader)
    {
        UInt32 input = reader.ReadUInt32();
        Byte b = 0;
        this.clutY = (UInt16)BitUtil.ReadBits(input, ref b, 9);
        this.clutX = (UInt16)BitUtil.ReadBits(input, ref b, 6);
        this.texY = (Byte)BitUtil.ReadBits(input, ref b, 1);
        this.texX = (Byte)BitUtil.ReadBits(input, ref b, 4);
        this.res = (Byte)BitUtil.ReadBits(input, ref b, 2);
        this.alpha = (Byte)BitUtil.ReadBits(input, ref b, 2);
        this.v = (Byte)BitUtil.ReadBits(input, ref b, 8);
        input = reader.ReadUInt32();
        b = 0;
        this.u = (Byte)BitUtil.ReadBits(input, ref b, 8);
        this.h = (UInt16)BitUtil.ReadBits(input, ref b, 10);
        this.w = (UInt16)BitUtil.ReadBits(input, ref b, 10);
        this.trans = (Byte)BitUtil.ReadBits(input, ref b, 1);
        this.pad = (Byte)BitUtil.ReadBits(input, ref b, 3);
    }

    public void ReadData_BGSPRITELOC_DEF(BinaryReader reader)
    {
        this.startOffset = reader.BaseStream.Position;
        UInt32 input = reader.ReadUInt32();
        this.oriData = input;
        Byte b = 0;
        this.depth = (Int32)((UInt16)BitUtil.ReadBits(input, ref b, 12));
        this.offY = (UInt16)BitUtil.ReadBits(input, ref b, 10);
        this.offX = (UInt16)BitUtil.ReadBits(input, ref b, 10);
    }

    public UInt16 clutX;
    public UInt16 clutY;

    public Byte texX;
    public Byte texY;
    public Byte res;
    public Byte alpha;
    public Byte u;
    public Byte v;

    public UInt16 w;
    public UInt16 h;

    public Byte trans;
    public Byte pad;

    public Int32 depth;

    public UInt16 offX;
    public UInt16 offY;
    public UInt16 atlasX;
    public UInt16 atlasY;

    public Int64 startOffset;

    public UInt32 oriData;

    public Transform transform;

    public Vector3 cacheLocalPos;
}

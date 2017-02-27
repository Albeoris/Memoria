using System;
using System.Collections.Generic;
using System.IO;

public class BGANIM_DEF
{
	public BGANIM_DEF()
	{
		this.frameList = new List<BGANIMFRAME_DEF>();
	}

	public void ReadData(BinaryReader reader)
	{
		UInt32 input = reader.ReadUInt32();
		Byte b = 0;
		this.flags = (Byte)BitUtil.ReadBits(input, ref b, 8);
		this.frameCount = (Int32)BitUtil.ReadBits(input, ref b, 24);
		input = reader.ReadUInt32();
		b = 0;
		this.camNdx = (Byte)BitUtil.ReadBits(input, ref b, 8);
		this.curFrame = (Int32)BitUtil.ReadBits(input, ref b, 24);
		this.frameRate = reader.ReadInt16();
		this.counter = reader.ReadUInt16();
		this.offset = reader.ReadUInt32();
        this.CalculateActualFrameCount();
    }

    public void CalculateActualFrameCount()
    {
        Int32 num = 256;
        Int32 num2 = (this.frameCount - 1) * (Int32)this.frameRate;
        Int32 num3 = (this.frameCount - 1) * num;
        this.actualFrameCount = this.frameCount;
        Int32 num4 = num2 - num3;
        if (num4 >= num)
        {
            this.actualFrameCount = this.frameCount - num4 / num;
        }
    }

    public Byte flags;

	public Int32 frameCount;

	public Byte camNdx;

	public Int32 curFrame;

	public Int16 frameRate;

	public UInt16 counter;

	public UInt32 offset;

    public Int32 actualFrameCount;

    public List<BGANIMFRAME_DEF> frameList;
}

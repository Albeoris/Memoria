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
		Byte bitIndex = 0;
		this.flags = (ANIM_FLAG)BitUtil.ReadBits(input, ref bitIndex, 8);
		this.frameCount = (Int32)BitUtil.ReadBits(input, ref bitIndex, 24);
		input = reader.ReadUInt32();
		bitIndex = 0;
		this.camNdx = (Byte)BitUtil.ReadBits(input, ref bitIndex, 8);
		this.curFrame = (Int32)BitUtil.ReadBits(input, ref bitIndex, 24);
		this.frameRate = reader.ReadInt16();
		this.counter = reader.ReadUInt16();
		this.offset = reader.ReadUInt32();
		this.curFrame = 0;
		this.frameRate = 256;
		this.counter = 0;
		this.flags = BGANIM_DEF.ANIM_FLAG.SingleFrame;
        this.CalculateActualFrameCount();
    }

    public void CalculateActualFrameCount()
    {
        const Int32 defaultRate = 256;
        Int32 completeCount = (this.frameCount - 1) * this.frameRate;
        Int32 completeDefault = (this.frameCount - 1) * defaultRate;
        this.actualFrameCount = this.frameCount;
        Int32 excess = completeCount - completeDefault;
		if (Math.Abs(excess) >= defaultRate)
			this.actualFrameCount = this.frameCount - excess / defaultRate;
	}

    public ANIM_FLAG flags;

	public Byte camNdx;

	public Int32 frameCount;
	public Int32 curFrame;
	public Int16 frameRate;
	public UInt16 counter;
	public Int32 actualFrameCount;

	public UInt32 offset;

	public List<BGANIMFRAME_DEF> frameList;

	[Flags]
	public enum ANIM_FLAG
	{
		SingleFrame = 1,
		Animate = 2,
		HasNotFinished = 4,
		Loop = 16,
		Palindrome = 32,

		StartPlay = Animate | HasNotFinished,
		ContinuePlay = HasNotFinished | Loop,
		Modifiables = Loop | Palindrome
	}
}

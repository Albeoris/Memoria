using System;

public class FF9PLAY_SKILL
{
	public FF9PLAY_SKILL()
	{
		this.Base = new Byte[4];
		this.defParam = new Int32[5];
	}

	public UInt32 cur_hp;

	public UInt32 cur_mp;

	public UInt32 max_hp;

	public UInt32 max_mp;

	public Byte[] Base;

	public Int32[] defParam;
}

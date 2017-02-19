using System;

public class STAT_INFO
{
	public STAT_INFO()
	{
		this.cnt = new STAT_CNT();
	}

	public UInt32 invalid;

	public UInt32 permanent;

	public UInt32 cur;

	public STAT_CNT cnt;
}

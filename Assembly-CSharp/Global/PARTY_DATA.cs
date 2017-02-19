using System;

public class PARTY_DATA
{
	public PARTY_DATA()
	{
		this.member = new PLAYER[4];
	}

	public PLAYER[] member;

	public UInt16 escape_no;

	public UInt16 summon_flag;

	public UInt32 gil;

	public UInt32 pad;
}

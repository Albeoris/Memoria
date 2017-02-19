using System;

public class SB2_PATTERN
{
	public SB2_PATTERN()
	{
		this.Put = new SB2_PUT[4];
	}

	public const Int32 Size = 56;

	public Byte Rate;

	public Byte MonCount;

	public Byte Camera;

	public Byte Pad0;

	public UInt32 AP;

	public SB2_PUT[] Put;
}

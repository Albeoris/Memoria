using System;

public class SB2_PATTERN
{
	public SB2_PATTERN()
	{
		this.Monster = new SB2_PUT[4];
	}

	public const Int32 Size = 56;

	[Memoria.PatchableFieldAttribute]
	public Byte Rate;

	//[Memoria.PatchableFieldAttribute]
	public Byte MonsterCount;

	[Memoria.PatchableFieldAttribute]
	public Byte Camera;

	public Byte Pad0;

	[Memoria.PatchableFieldAttribute]
	public UInt32 AP;

    //[Memoria.PatchableFieldAttribute]
    public SB2_PUT[] Monster;
}

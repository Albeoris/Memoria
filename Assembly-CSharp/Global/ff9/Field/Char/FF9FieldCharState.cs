using System;

public class FF9FieldCharState
{
	public FF9FieldCharState()
	{
		this._padbyte = new Byte[2];
		this.clr = new FF9FieldCharColor[2];
	}

	public UInt32 attr;

	public SByte floor;

	public SByte arate;

	public Byte[] _padbyte;

	public FF9FieldCharColor[] clr;

	public FF9FieldCharMirror mirror;
}

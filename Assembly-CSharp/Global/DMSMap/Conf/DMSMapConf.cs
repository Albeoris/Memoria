using System;

public struct DMSMapConf
{
	public UInt16 attr;

	public UInt16 version;

	public UInt16 bgNo;

	public Byte lightCount;

	public Byte lightUse;

	public Byte charCount;

	public Byte charUse;

	public Byte evtCount;

	public Byte evtUse;

	public DMSMapLight[] DMSMapLight;

	public DMSMapChar[] DMSMapChar;
}

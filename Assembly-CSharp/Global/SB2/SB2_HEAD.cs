using System;

public class SB2_HEAD
{
	public const UInt16 SB2_FLG_SPECIAL = 1;

	public const UInt16 SB2_FLG_BACKATK = 2;

	public const UInt16 SB2_FLG_NOGAMEOVER = 4;

	public const UInt16 SB2_FLG_EXPZERO = 8;

	public const UInt16 SB2_FLG_NOWINPOSE = 16;

	public const UInt16 SB2_FLG_NORUNAWAY = 32;

	public const UInt16 SB2_FLG_NONEARATK = 64;

	public const UInt16 SB2_FLG_NOMAGICAL = 128;

	public const UInt16 SB2_FLG_REVERSEATK = 256;

	public const UInt16 SB2_FLG_FIXEDCAM1 = 512;

	public const UInt16 SB2_FLG_FIXEDCAM2 = 1024;

	public const UInt16 SB2_FLG_AFTEREVENT = 2048;

	public const UInt16 SB2_FLG_MESEVENT = 4096;

	public const UInt16 SB2_FLG_FIELDBGM = 8192;

	public const Int32 Size = 8;

	public Byte Ver;

	public Byte PatCount;

	public Byte TypCount;

	public Byte AtkCount;

	public UInt16 Flags;

	public Int16 Pad1;
}

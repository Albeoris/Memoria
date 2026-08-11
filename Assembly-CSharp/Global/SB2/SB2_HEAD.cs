using System;

public class SB2_HEAD
{
    public const UInt16 SB2_FLG_SPECIAL = 0x1;
    public const UInt16 SB2_FLG_BACKATK = 0x2;
    public const UInt16 SB2_FLG_NOGAMEOVER = 0x4;
    public const UInt16 SB2_FLG_EXPZERO = 0x8;
    public const UInt16 SB2_FLG_NOWINPOSE = 0x10;
    public const UInt16 SB2_FLG_NORUNAWAY = 0x20;
    public const UInt16 SB2_FLG_NONEARATK = 0x40;
    public const UInt16 SB2_FLG_NOMAGICAL = 0x80;
    public const UInt16 SB2_FLG_REVERSEATK = 0x100;
    public const UInt16 SB2_FLG_FIXEDCAM1 = 0x200;
    public const UInt16 SB2_FLG_FIXEDCAM2 = 0x400;
    public const UInt16 SB2_FLG_AFTEREVENT = 0x800;
    public const UInt16 SB2_FLG_MESEVENT = 0x1000;
    public const UInt16 SB2_FLG_FIELDBGM = 0x2000;

    public Byte Ver;
    public Byte PatCount;
    public Byte TypCount;
    public Byte AtkCount;
    public UInt16 Flags;
    public Int16 Pad1;
    public UInt16 BattleMapIndex;
}

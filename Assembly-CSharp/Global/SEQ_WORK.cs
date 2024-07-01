using System;

public class SEQ_WORK
{
    public SeqFlag Flags;

    public CMD_DATA CmdPtr;

    public Int32 CurPtr;

    public Int32 OldPtr;

    public Int16 IncCnt;

    public Int16 DecCnt;

    public Int16 AnmCnt;

    public Byte AnmIDOfs;

    public Byte SfxTime;

    public UInt16 SfxNum;

    public Byte SfxAttr;

    public Byte SfxVol;

    public Int16 TurnOrg;

    public Int16 TurnRot;

    public Byte TurnCnt;

    public Byte TurnTime;

    public UInt16 SVfxNum;

    public Byte SVfxParam;

    public Byte SVfxTime;

    public Byte FadeTotal;

    public Byte FadeStep;

    public Byte[] Work = new Byte[16];
}

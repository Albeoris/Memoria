using System;
using UnityEngine;

public class BTL2D_ENT
{
    public BTL2D_ENT()
    {
        this.Work = new BTL2D_ENT.UnionNumSym();
    }

    public BTL_DATA BtlPtr;

    public UInt32 Stat;

    public Transform trans;

    public UInt16 Cnt;

    public Byte Delay;

    public SByte Yofs;

    public Byte Type;

    public Byte NoClip;

    public String CustomMessage;

    public String CustomColor;

    public HUDMessage.MessageStyle CustomStyle;

    public BTL2D_ENT.UnionNumSym Work;

    public class UnionNumSym
    {
        public UnionNumSym()
        {
            this.Num = new BTL2D_ENT_NUM();
        }

        public BTL2D_ENT_NUM Num;
    }
}

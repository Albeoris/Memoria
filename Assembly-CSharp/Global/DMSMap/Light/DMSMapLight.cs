using System;
using System.IO;

public struct DMSMapLight
{
    public void Read(BinaryReader reader)
    {
        this.type = reader.ReadSByte();
        this.no = reader.ReadSByte();
        this.shadowI = reader.ReadSByte();
        this.shadowR = reader.ReadSByte();
        this.clr = new SByte[4];
        this.floor = new SByte[4];
        for (Int32 i = 0; i < 4; i++)
        {
            this.clr[i] = reader.ReadSByte();
        }
        for (Int32 j = 0; j < 4; j++)
        {
            this.floor[j] = reader.ReadSByte();
        }
    }

    public override String ToString()
    {
        String text = String.Empty;
        text = text + "type : " + this.type;
        text = text + "\nno : " + this.no;
        text = text + "\nshadowI : " + this.shadowI;
        text = text + "\nshadowR : " + this.shadowR;
        for (Int32 i = 0; i < 4; i++)
        {
            String text2 = text;
            text = String.Concat(new Object[]
            {
                text2,
                "\nclr[",
                i,
                "] : ",
                this.clr[i]
            });
        }
        for (Int32 j = 0; j < 4; j++)
        {
            String text2 = text;
            text = String.Concat(new Object[]
            {
                text2,
                "\nfloor[",
                j,
                "] : ",
                this.floor[j]
            });
        }
        return text;
    }

    public SByte type;

    public SByte no;

    public SByte shadowI;

    public SByte shadowR;

    public SByte[] clr;

    public SByte[] floor;
}

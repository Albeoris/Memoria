using System;
using System.IO;

public class DMSMapLight
{
    public void Read(BinaryReader reader)
    {
        this.type = reader.ReadSByte();
        this.no = reader.ReadSByte();
        this.shadowI = (Int16)(reader.ReadSByte() << 3);
        this.shadowR = reader.ReadSByte();
        this.clr = new Int16[4];
        this.floor = new SByte[4];
        for (Int32 i = 0; i < 4; i++)
            this.clr[i] = (Int16)(reader.ReadSByte() << 3);
        for (Int32 i = 0; i < 4; i++)
            this.floor[i] = reader.ReadSByte();
    }

    public override String ToString()
    {
        String text = "type : " + this.type;
        text += "\nno : " + this.no;
        text += "\nshadowI : " + this.shadowI;
        text += "\nshadowR : " + this.shadowR;
        for (Int32 i = 0; i < 4; i++)
            text += $"\nclr[{i}] : {this.clr[i]}";
        for (Int32 i = 0; i < 4; i++)
            text += $"\nfloor[{i}] : {this.floor[i]}";
        return text;
    }

    public SByte type;
    public SByte no;

    public Int16 shadowI;
    public SByte shadowR;
    public Int16[] clr;
    public SByte[] floor;
}

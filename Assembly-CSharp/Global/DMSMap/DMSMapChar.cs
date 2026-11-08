using System;
using System.IO;

public class DMSMapChar
{
    public void Read(BinaryReader reader)
    {
        this.geoNo = reader.ReadUInt16();
        this.shadowI = (Int16)(reader.ReadSByte() << 3);
        this.shadowR = reader.ReadSByte();
        this.clr = new Int16[3];
        for (Int32 i = 0; i < 3; i++)
            this.clr[i] = (Int16)(reader.ReadSByte() << 3);
        this.shadowZ = reader.ReadSByte();
    }

    public override String ToString()
    {
        String text = "geoNo : " + this.geoNo;
        text += "\nshadowI : " + this.shadowI;
        text += "\nshadowR : " + this.shadowR;
        for (Int32 i = 0; i < 3; i++)
            text += $"\nclr[{i}] : {this.clr[i]}";
        return text;
    }

    public UInt16 geoNo;

    public Int16 shadowI;
    public SByte shadowR;
    public Int16[] clr;
    public SByte shadowZ;
}

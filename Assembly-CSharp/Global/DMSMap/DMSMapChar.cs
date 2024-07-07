using System;
using System.IO;

public struct DMSMapChar
{
    public void Read(BinaryReader reader)
    {
        this.geoNo = reader.ReadUInt16();
        this.shadowI = reader.ReadSByte();
        this.shadowR = reader.ReadSByte();
        this.clr = new SByte[3];
        for (Int32 i = 0; i < 3; i++)
        {
            this.clr[i] = reader.ReadSByte();
        }
        this.shadowZ = reader.ReadSByte();
    }

    public override String ToString()
    {
        String text = String.Empty;
        text = text + "geoNo : " + this.geoNo;
        text = text + "\nshadowI : " + this.shadowI;
        text = text + "\nshadowR : " + this.shadowR;
        for (Int32 i = 0; i < 3; i++)
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
        return text;
    }

    public UInt16 geoNo;

    public SByte shadowI;

    public SByte shadowR;

    public SByte[] clr;

    public SByte shadowZ;
}

using System;
using System.IO;

public class BGLIGHTCLR_DEF
{
    public BGLIGHTCLR_DEF()
    {
        this.clr = new Byte[3];
    }

    public void ReadData(BinaryReader reader)
    {
        this.clr[0] = reader.ReadByte();
        this.clr[1] = reader.ReadByte();
        this.clr[2] = reader.ReadByte();
        this.camNdx = reader.ReadByte();
    }

    public Byte[] clr;

    public Byte camNdx;
}

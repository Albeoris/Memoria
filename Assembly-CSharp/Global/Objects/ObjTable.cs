using System;
using System.IO;

public class ObjTable
{
    public void ReadData(BinaryReader br)
    {
        this.ofs = br.ReadUInt16();
        this.size = br.ReadUInt16();
        this.varn = br.ReadByte();
        this.flags = br.ReadByte();
        this.pad1 = br.ReadByte();
        this.pad2 = br.ReadByte();
    }

    public UInt16 ofs;

    public UInt16 size;

    public Byte varn;

    public Byte flags;

    public Byte pad1;

    public Byte pad2;
}

using System;
using System.IO;
using Memoria.Data;

public class ObjTable
{
    public void ReadData(BinaryReader br, Int32 index, Int32 entryCount, Boolean vanillaFormat)
    {
        if (vanillaFormat)
        {
            memoria_id = index;
            if (index + 1 >= entryCount)
                player_link = CharacterId.Beatrix;
            else if (index + 9 >= entryCount)
                player_link = (CharacterId)(index + 9 - entryCount);
            else
                player_link = CharacterId.NONE;
            append_mode = 0;
        }
        else
        {
            memoria_id = br.ReadInt32();
            Int32 linknum = br.ReadInt32();
            player_link = linknum >= 0 ? (CharacterId)linknum : CharacterId.NONE;
            append_mode = br.ReadByte();
        }
        ofs = br.ReadUInt16();
        size = br.ReadUInt16();
        varn = br.ReadByte();
        flags = br.ReadByte();
        pad = br.ReadUInt16();
    }

    public Boolean IsVanillaEntry => memoria_id >= 0 && memoria_id < 1000;

    public Int32 memoria_id;
    public Byte append_mode;
    public UInt16 ofs;
    public UInt16 size;
    public Byte varn;
    public Byte flags;
    public UInt16 pad;
    public CharacterId player_link;

    public Int32 mod_index;
}

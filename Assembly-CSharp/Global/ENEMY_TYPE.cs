using Memoria.Data;
using System;

public class ENEMY_TYPE
{
    public ENEMY_TYPE()
    {
        this.max = new POINTS();
        this.mot = new String[6];
        this.cam_bone = new Byte[3];
        this.icon_bone = new Byte[6];
        this.icon_y = new SByte[6];
        this.icon_z = new SByte[6];
    }

    public String name;
    public Byte category;
    public Byte level;
    public POINTS max;
    public String[] mot;
    public UInt16 radius;
    public Byte p_atk_no;
    public Int32 blue_magic_no;

    public Int32 mes;

    public Byte[] cam_bone;
    public UInt32 cam_addr;

    public UInt16 die_snd_no;

    public Byte[] icon_bone;
    public SByte[] icon_y;
    public SByte[] icon_z;
}

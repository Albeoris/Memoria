using System;

public struct MBG_DEF
{
    public MBG_DEF(String name, Byte isRGB24, Byte type)
    {
        this.name = name;
        this.isRGB24 = isRGB24;
        this.type = type;
    }

    public const Byte MBG_NORMAL = 0;

    public const Byte MBG_WITH_DATA = 1;

    public String name;

    public Byte isRGB24;

    public Byte type;
}

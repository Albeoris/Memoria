using System;

public class EncountData
{
    public EncountData()
    {
        this.scene = new UInt16[4];
    }

    public UInt16[] scene;

    public Byte pattern;

    public Byte pad;
}

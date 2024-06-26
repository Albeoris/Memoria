using System;

public class FF9StateWorldMap
{
    public FF9StateWorldMap()
    {
        this.attr = 0u;
        this.nextMode = 0;
        this.nextMapNo = 0;
    }

    public UInt32 attr;

    public Byte[] evtPtr;

    public Byte nextMode;

    public Int16 nextMapNo;
}

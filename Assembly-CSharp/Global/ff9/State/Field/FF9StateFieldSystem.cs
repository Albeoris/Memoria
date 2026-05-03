using System;

public class FF9StateFieldSystem
{
    public FF9StateFieldSystem()
    {
        this.loc = new FF9StateFieldLocation();
    }

    public void ff9ResetStateFieldSystem()
    {
        this.attr = 0u;
        this.prevMapNo = -1;
    }

    public UInt32 attr;

    public Int32 playerID;

    private Int16 prevMapNo;

    public FF9StateFieldLocation loc;
}

using System;

public class FF9StateFieldLocation
{
    public FF9StateFieldLocation()
    {
        this.map = new FF9StateFieldMap();
    }

    public void ff9ResetStateFieldLocation()
    {
        this.attr = 0;
        this.nextLocNo = FF9StateSystem.Common.FF9.fldLocNo;
    }

    public Int32 attr;

    private Int16 nextLocNo;

    public FF9StateFieldMap map;
}

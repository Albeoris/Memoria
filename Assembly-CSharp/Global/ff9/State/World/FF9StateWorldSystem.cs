using System;

public class FF9StateWorldSystem
{
    public FF9StateWorldSystem()
    {
        this.attr = 0u;
        this.map = new FF9StateWorldMap();
    }

    public UInt32 attr;

    public FF9StateWorldMap map;
}

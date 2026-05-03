using System;
using System.Collections.Generic;

public class BTL2D_WORK
{
    public BTL2D_WORK()
    {
        this.Entry = new List<BTL2D_ENT>(btl2d.BTL2D_INITIAL_COUNT);
        for (Int32 i = 0; i < btl2d.BTL2D_INITIAL_COUNT; i++)
            this.Entry.Add(new BTL2D_ENT());
    }

    public List<BTL2D_ENT> Entry;
    public UInt16 Timer;
    public Byte OldDisappear;
}

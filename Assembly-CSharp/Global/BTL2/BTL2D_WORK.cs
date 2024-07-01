using System;

public class BTL2D_WORK
{
    public BTL2D_WORK()
    {
        this.Entry = new BTL2D_ENT[16];
        for (Int32 i = 0; i < (Int32)this.Entry.Length; i++)
        {
            this.Entry[i] = new BTL2D_ENT();
        }
        this.Libra = new BTL2D_LIBRA();
        this.Peep = new BTL2D_PEEP();
        this.Reserve = new Byte[3];
    }

    public BTL2D_ENT[] Entry;

    public Int16 NewID;

    public UInt16 Timer;

    public BTL2D_LIBRA Libra;

    public BTL2D_PEEP Peep;

    public Byte OldDisappear;

    public Byte[] Reserve;
}

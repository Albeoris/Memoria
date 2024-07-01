using System;

public class SeqFlag
{
    public SeqFlag()
    {
        this.WaitLoadVfx = false;
        this.DoneLoadVfx = false;
        this.FinishIdle = false;
        this.EventMode = false;
        this.SeqMode = false;
    }

    public Boolean WaitLoadVfx;

    public Boolean DoneLoadVfx;

    public Boolean FinishIdle;

    public Boolean EventMode;

    public Boolean SeqMode;

    public Int32 Reserve;
}

using System;

public struct VIB_PARMS_DEF
{
    public UInt16 statusFlags;

    public Int16 frameStart;

    public Int16 frameStop;

    public Int16 frameRate;

    public Int32 frameNdx;

    public VIB_DEF? vibPtr;

    public VIB_TRACK_DEF[] tracks;

    public Byte[][][] frameData;
}

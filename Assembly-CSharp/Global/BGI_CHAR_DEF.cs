using System;
using UnityEngine;

public class BGI_CHAR_DEF
{
    public BGI_CHAR_DEF()
    {
        this.xEdgeNdx = new UInt16[2];
        this.zEdgeNdx = new UInt16[2];
        this.xIntercept = new Int16[2];
        this.zIntercept = new Int16[2];
        this.charPos = Vector3.zero;
        this.lastPos = Vector3.zero;
        this.charRot = Vector3.zero;
        this.charNdx = -1;
    }

    public UInt16 charFlags;

    public Int16 heading;

    public Int16 activeFloor;

    public Int16 activeTri;

    public Int16 lastFloor;

    public Int16 lastTri;

    public UInt16[] xEdgeNdx;

    public UInt16[] zEdgeNdx;

    public Int16[] xIntercept;

    public Int16[] zIntercept;

    public Single charRadius;

    public Single charRadiusSquared;

    public Vector3 charPos;

    public Vector3 lastPos;

    public Vector3 charRot;

    public Int32 charNdx;

    public Single talkRad;

    public Single collRad;

    public Single speed;

    public FieldMapActor actor;
}

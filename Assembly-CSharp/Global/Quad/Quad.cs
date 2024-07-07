using System;

public class Quad : Obj
{
    public Quad()
    {
        this.q = new QuadPos[Quad.numOfQuadPoses];
        for (Int32 i = 0; i < Quad.numOfQuadPoses; i++)
        {
            this.q[i] = new QuadPos();
        }
    }

    public Quad(Int32 sid, Int32 uid) : base(sid, uid, EventEngine.sizeOfQuad, 16)
    {
        this.q = new QuadPos[Quad.numOfQuadPoses];
        for (Int32 i = 0; i < Quad.numOfQuadPoses; i++)
        {
            this.q[i] = new QuadPos();
        }
        base.cid = 3;
        base.flags = 1;
    }

    public void copy(Quad quad)
    {
        this.n = quad.n;
        for (Int32 i = 0; i < Quad.numOfQuadPoses; i++)
        {
            this.q[i].X = quad.q[i].X;
            this.q[i].Y = quad.q[i].Y;
            this.q[i].Z = quad.q[i].Z;
        }
    }

    public Int32 n;

    public QuadPos[] q;

    private static Int32 numOfQuadPoses = 8;
}

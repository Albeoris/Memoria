using System;
using UnityEngine;

public class QuadPos
{
    public Int16 X
    {
        get
        {
            return this.x;
        }
        set
        {
            this.x = value;
            this.v.Set((Single)this.x, (Single)this.y, (Single)this.z);
        }
    }

    public Int16 Y
    {
        get
        {
            return this.y;
        }
        set
        {
            this.y = value;
            this.v.Set((Single)this.x, (Single)this.y, (Single)this.z);
        }
    }

    public Int16 Z
    {
        get
        {
            return this.z;
        }
        set
        {
            this.z = value;
            this.v.Set((Single)this.x, (Single)this.y, (Single)this.z);
        }
    }

    public Vector3 Vector3Val
    {
        get
        {
            return this.v;
        }
    }

    private Int16 x;

    private Int16 y;

    private Int16 z;

    private Vector3 v;
}

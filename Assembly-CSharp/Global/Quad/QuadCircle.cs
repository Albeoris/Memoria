using System;
using UnityEngine;

public class QuadCircle
{
    public QuadCircle(Vector3 center, Single radiant)
    {
        this.center = center;
        this.radiant = radiant;
        this.useOriginalArea = false;
    }

    public QuadCircle(Vector3 center, Single radiant, Boolean useOriginalArea)
    {
        this.center = center;
        this.radiant = radiant;
        this.useOriginalArea = useOriginalArea;
    }

    public Vector3 Center
    {
        get
        {
            return this.center;
        }
    }

    public Single Radiant
    {
        get
        {
            return this.radiant;
        }
    }

    public Boolean UseOriginalArea
    {
        get
        {
            return this.useOriginalArea;
        }
    }

    public Boolean IsCollisionEnter(Single x, Single z)
    {
        Vector3 a = this.center;
        Vector3 b = new Vector3(x, 0f, z);
        Single num = Vector3.Distance(a, b);
        return num < this.radiant;
    }

    private Vector3 center;

    private Single radiant;

    private Boolean useOriginalArea;
}

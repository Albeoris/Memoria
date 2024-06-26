using System;
using UnityEngine;

public struct WMTriangle
{
    public WMTriangle(Vector3 point0, Vector3 point1, Vector3 point2)
    {
        this.point0 = point0;
        this.point1 = point1;
        this.point2 = point2;
    }

    public Vector3 point0;

    public Vector3 point1;

    public Vector3 point2;
}

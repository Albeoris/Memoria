using System;
using UnityEngine;

public static class WMMath
{
    public static Single Angle2D(Vector2 start, Vector2 end, Boolean returnPositiveAngle = true)
    {
        Single y = end.y - start.y;
        Single x = end.x - start.x;
        Single num = Mathf.Atan2(y, x);
        Single num2 = num * 57.29578f;
        if (num2 < 0f && returnPositiveAngle)
        {
            return num2 + 360f;
        }
        return num2;
    }

    public static Vector2 VectorFromAngle2D(Single angleDegree)
    {
        Single f = angleDegree * 0.0174532924f;
        Single x = Mathf.Cos(f);
        Single y = Mathf.Sin(f);
        return new Vector2(x, y);
    }

    public static Vector3 RotateVectorXZ(Vector3 vector, Single angle)
    {
        Single f = angle * 0.0174532924f;
        return new Vector3
        {
            x = vector.x * Mathf.Cos(f) - vector.z * Mathf.Sin(f),
            z = vector.x * Mathf.Sin(f) + vector.z * Mathf.Cos(f)
        };
    }
}

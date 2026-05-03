using System;
using UnityEngine;

public static class WMPhysics
{
    public static Boolean Raycast(Ray ray, WMMesh mesh, out WMRaycastHit hit)
    {
        Int32[] triangles = mesh.Triangles;
        Vector4[] tangents = mesh.Tangents;
        Vector3[] vertices = mesh.Vertices;
        Transform transform = mesh.Transform;
        hit = default(WMRaycastHit);
        for (Int32 i = 0; i < (Int32)triangles.Length / 3; i++)
        {
            Int32 num = (Int32)tangents[triangles[i * 3]].x;
            if (num != 4078 || WMPhysics.IgnoreExceptions)
            {
                if (num != 4088 || WMPhysics.IgnoreExceptions)
                {
                    if (num != 2040 || WMPhysics.IgnoreExceptions)
                    {
                        Single num2 = Vector3.Dot(Vector3.up, mesh.TriangleNormals[i]);
                        if (num2 <= 0.1f)
                        {
                            if (!WMPhysics.IgnoreExceptions)
                            {
                                goto IL_15E;
                            }
                        }
                        Vector3 vector = vertices[triangles[i * 3]];
                        Vector3 vector2 = vertices[triangles[i * 3 + 1]];
                        Vector3 vector3 = vertices[triangles[i * 3 + 2]];
                        vector = transform.TransformPoint(vector);
                        vector2 = transform.TransformPoint(vector2);
                        vector3 = transform.TransformPoint(vector3);
                        WMTriangle t = new WMTriangle(vector, vector2, vector3);
                        if (WMPhysics.intersect3D_RayTriangle(ray, t, out hit.point) == 1)
                        {
                            hit.triangleIndex = i;
                            return true;
                        }
                    }
                }
            }
        IL_15E:;
        }
        return false;
    }

    public static Boolean RaycastOnSpecifiedTriangle(Ray ray, WMMesh mesh, Int32 triangleIndex, out WMRaycastHit hit)
    {
        Vector3[] vertices = mesh.Vertices;
        Int32[] triangles = mesh.Triangles;
        Transform transform = mesh.Transform;
        hit = default(WMRaycastHit);
        Vector3 vector = vertices[triangles[triangleIndex * 3]];
        Vector3 vector2 = vertices[triangles[triangleIndex * 3 + 1]];
        Vector3 vector3 = vertices[triangles[triangleIndex * 3 + 2]];
        vector = transform.TransformPoint(vector);
        vector2 = transform.TransformPoint(vector2);
        vector3 = transform.TransformPoint(vector3);
        WMTriangle t = new WMTriangle(vector, vector2, vector3);
        if (WMPhysics.intersect3D_RayTriangle(ray, t, out hit.point) != 0)
        {
            hit.triangleIndex = triangleIndex;
            return true;
        }
        return false;
    }

    private static Int32 intersect3D_RayTriangle(Ray R, WMTriangle T, out Vector3 I)
    {
        I = Vector3.zero;
        Vector3 vector = T.point1 - T.point0;
        Vector3 vector2 = T.point2 - T.point0;
        Vector3 lhs = Vector3.Cross(vector, vector2);
        if (lhs == Vector3.zero)
        {
            return -1;
        }
        Vector3 direction = R.direction;
        Vector3 rhs = R.origin - T.point0;
        Single num = -Vector3.Dot(lhs, rhs);
        Single num2 = Vector3.Dot(lhs, direction);
        if (Mathf.Abs(num2) < 1E-08f)
        {
            if (num == 0f)
            {
                return 2;
            }
            return 0;
        }
        else
        {
            Single num3 = num / num2;
            if ((Double)num3 < 0.0)
            {
                return 0;
            }
            I = R.origin + num3 * direction;
            Single num4 = Vector3.Dot(vector, vector);
            Single num5 = Vector3.Dot(vector, vector2);
            Single num6 = Vector3.Dot(vector2, vector2);
            Vector3 lhs2 = I - T.point0;
            Single num7 = Vector3.Dot(lhs2, vector);
            Single num8 = Vector3.Dot(lhs2, vector2);
            Single num9 = num5 * num5 - num4 * num6;
            Single num10 = (num5 * num8 - num6 * num7) / num9;
            if ((Double)num10 < 0.0 || (Double)num10 > 1.0)
            {
                return 0;
            }
            Single num11 = (num5 * num7 - num4 * num8) / num9;
            if ((Double)num11 < 0.0 || (Double)(num10 + num11) > 1.0)
            {
                return 0;
            }
            return 1;
        }
    }

    private const Single SMALL_NUM = 1E-08f;

    public static Boolean IgnoreExceptions;

    public static Boolean CastRayFromSky;

    public static Boolean UseInfiniteRaycast;
}

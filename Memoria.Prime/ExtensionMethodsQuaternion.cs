using System;
using UnityEngine;

namespace Memoria.Prime
{
    public enum EulerRotationOrder
    {
        EULER_XYZ = 0,
        EULER_XZY,
        EULER_YZX,
        EULER_YXZ,
        EULER_ZXY,
        EULER_ZYX,
        //SPHERIC_XYZ
    }

    public static class ExtensionMethodsQuaternion
    {
        public static Single Norm2(this Quaternion q)
        {
            return q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;
        }

        public static Single Norm(this Quaternion q)
        {
            return Mathf.Sqrt(q.Norm2());
        }

        public static void SetSpecificCoordinate(this ref Quaternion q, Int32 coordType, Single coord)
        {
            if (coordType < 0 || coordType > 3)
                return;
            if (q.w < 0f)
            {
                q.x = -q.x;
                q.y = -q.y;
                q.z = -q.z;
                q.w = -q.w;
            }
            coord = Mathf.Clamp(coord, -1f, 1f);
            q[coordType] = coord;
            Single targetW2 = 1f - q.x * q.x - q.y * q.y - q.z * q.z;
            if (targetW2 >= 0f)
            {
                q.w = (Single)Math.Sqrt(targetW2);
            }
            else
            {
                q.w = 0f;
                if (coordType != 0)
                {
                    Single targetX2 = 1f - q.y * q.y - q.z * q.z;
                    if (targetX2 > 0f)
                        q.x = Math.Sign(q.x) * (Single)Math.Sqrt(targetX2);
                    else
                        q.x = 0f;
                }
                if (coordType != 1)
                {
                    Single targetY2 = 1f - q.x * q.x - q.z * q.z;
                    if (targetY2 > 0f)
                        q.y = Math.Sign(q.y) * (Single)Math.Sqrt(targetY2);
                    else
                        q.y = 0f;
                }
                if (coordType != 2)
                {
                    Single targetZ2 = 1f - q.x * q.x - q.y * q.y;
                    if (targetZ2 > 0f)
                        q.z = Math.Sign(q.z) * (Single)Math.Sqrt(targetZ2);
                    else
                        q.z = 0f;
                }
            }
        }

        public static void SetupFromEulerAngles(this ref Quaternion q, Vector3 eulerAngles, EulerRotationOrder order = EulerRotationOrder.EULER_ZXY)
        {
            const Double HalfDegToRad = Math.PI / 360.0;
            Double cx = Math.Cos(eulerAngles.x * HalfDegToRad);
            Double sx = Math.Sin(eulerAngles.x * HalfDegToRad);
            Double cy = Math.Cos(eulerAngles.y * HalfDegToRad);
            Double sy = Math.Sin(eulerAngles.y * HalfDegToRad);
            Double cz = Math.Cos(eulerAngles.z * HalfDegToRad);
            Double sz = Math.Sin(eulerAngles.z * HalfDegToRad);

            // Could be optimized...
            Quaternion qx = new Quaternion((Single)sx, 0, 0, (Single)cx);
            Quaternion qy = new Quaternion(0, (Single)sy, 0, (Single)cy);
            Quaternion qz = new Quaternion(0, 0, (Single)sz, (Single)cz);
            Quaternion[] op;
            switch (order)
            {
                case EulerRotationOrder.EULER_XYZ: op = [qx, qy, qz]; break;
                case EulerRotationOrder.EULER_XZY: op = [qx, qz, qy]; break;
                case EulerRotationOrder.EULER_YZX: op = [qy, qz, qx]; break;
                case EulerRotationOrder.EULER_YXZ: op = [qy, qx, qz]; break;
                default:
                case EulerRotationOrder.EULER_ZXY: op = [qz, qx, qy]; break;
                case EulerRotationOrder.EULER_ZYX: op = [qz, qy, qx]; break;
            }
            q = op[2] * op[1] * op[0];
        }

        public static Quaternion ToQuaternion(this Matrix4x4 matrix, Boolean normalize = true)
        {
            Single f = 0.5f * Mathf.Sqrt(1 + matrix.m00 - matrix.m11 - matrix.m22);
            if (Math.Abs(f) < Mathf.Epsilon)
                return Quaternion.identity;
            Quaternion q = new Quaternion();
            Single wf = matrix.m21 - matrix.m12;
            if (Math.Sign(f) == -Math.Sign(wf)) // Choose a quaternion with w >= 0
                f = -f;
            q.x = f;
            f = 1f / (4f * f);
            q.y = f * (matrix.m01 + matrix.m10);
            q.z = f * (matrix.m02 + matrix.m20);
            q.w = f * wf;
            if (normalize)
            {
                Single norm = q.Norm();
                if (norm != 1f)
                {
                    q.x /= norm;
                    q.y /= norm;
                    q.z /= norm;
                    q.w /= norm;
                }
            }
            return q;
        }
    }
}

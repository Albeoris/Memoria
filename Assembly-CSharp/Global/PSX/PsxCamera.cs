using System;
using UnityEngine;
using Object = System.Object;

public class PsxCamera
{
    public static void DebugLogPsxCam(Int16[,] psxRotation, Int32[] psxTranslation)
    {
        String text = String.Empty;
        String text2 = text;
        text = String.Concat(new Object[]
        {
            text2,
            "R0\t",
            psxRotation[0, 0],
            "\t",
            psxRotation[0, 1],
            "\t",
            psxRotation[0, 2],
            "\n"
        });
        text2 = text;
        text = String.Concat(new Object[]
        {
            text2,
            "R1\t",
            psxRotation[1, 0],
            "\t",
            psxRotation[1, 1],
            "\t",
            psxRotation[1, 2],
            "\n"
        });
        text2 = text;
        text = String.Concat(new Object[]
        {
            text2,
            "R2\t",
            psxRotation[2, 0],
            "\t",
            psxRotation[2, 1],
            "\t",
            psxRotation[2, 2],
            "\n"
        });
        text2 = text;
        text = String.Concat(new Object[]
        {
            text2,
            "T\t",
            psxTranslation[0],
            "\t",
            psxTranslation[1],
            "\t",
            psxTranslation[2],
            "\n"
        });
        global::Debug.Log(text);
    }

    public static void UnityMatrix2PsxMatrix(Matrix4x4 unityWorldToCameraMatrix, Int16[,] psxRotation, Int32[] psxTranslation)
    {
        Matrix4x4 matrix4x = unityWorldToCameraMatrix;
        psxRotation[0, 0] = PSX.FloatToFp12(matrix4x.m00);
        psxRotation[0, 1] = PSX.FloatToFp12(-matrix4x.m01);
        psxRotation[0, 2] = PSX.FloatToFp12(matrix4x.m02);
        psxRotation[1, 0] = PSX.FloatToFp12(-matrix4x.m10);
        psxRotation[1, 1] = PSX.FloatToFp12(matrix4x.m11);
        psxRotation[1, 2] = PSX.FloatToFp12(-matrix4x.m12);
        psxRotation[2, 0] = PSX.FloatToFp12(-matrix4x.m20);
        psxRotation[2, 1] = PSX.FloatToFp12(matrix4x.m21);
        psxRotation[2, 2] = PSX.FloatToFp12(-matrix4x.m22);
        psxTranslation[0] = (Int32)matrix4x.m03;
        psxTranslation[1] = -(Int32)matrix4x.m13;
        psxTranslation[2] = -(Int32)matrix4x.m23;
    }

    private Vector3 RotationMatrix2EulerAngle(Matrix4x4 r)
    {
        return new Vector3
        {
            x = Mathf.Atan2(r.m21, r.m22),
            y = Mathf.Atan2(-r.m20, Mathf.Sqrt(r.m21 * r.m21 + r.m22 * r.m22)),
            z = Mathf.Atan2(r.m10, r.m00)
        };
    }

    public static Matrix4x4 PsxMatrix2UnityMatrix(Int16[,] rotation)
    {
        Matrix4x4 identity = Matrix4x4.identity;
        identity.m00 = PSX.Fp12ToFloat(rotation[0, 0]);
        identity.m01 = -PSX.Fp12ToFloat(rotation[0, 1]);
        identity.m02 = PSX.Fp12ToFloat(rotation[0, 2]);
        identity.m10 = -PSX.Fp12ToFloat(rotation[1, 0]);
        identity.m11 = PSX.Fp12ToFloat(rotation[1, 1]);
        identity.m12 = -PSX.Fp12ToFloat(rotation[1, 2]);
        identity.m20 = -PSX.Fp12ToFloat(rotation[2, 0]);
        identity.m21 = PSX.Fp12ToFloat(rotation[2, 1]);
        identity.m22 = -PSX.Fp12ToFloat(rotation[2, 2]);
        return identity;
    }

    public static Matrix4x4 PsxMatrix2UnityMatrix(Single[] pmat, Single zoffset)
    {
        Single num = 1f;
        Matrix4x4 identity = Matrix4x4.identity;
        identity.m00 = pmat[0] / 4096f * num;
        identity.m01 = pmat[1] / -4096f * num;
        identity.m02 = pmat[2] / 4096f * num;
        identity.m10 = pmat[3] / -4096f * num;
        identity.m11 = pmat[4] / 4096f * num;
        identity.m12 = pmat[5] / -4096f * num;
        identity.m20 = pmat[6] / -4096f * num;
        identity.m21 = pmat[7] / 4096f * num;
        identity.m22 = pmat[8] / -4096f * num;
        identity.m03 = pmat[9];
        identity.m13 = -pmat[10];
        identity.m23 = -(pmat[11] + zoffset);
        return identity;
    }

    public static Matrix4x4 PerspectiveOffCenter(Single left, Single right, Single bottom, Single top, Single near, Single far)
    {
        Single value = 2f * near / (right - left);
        Single value2 = 2f * near / (top - bottom);
        Single value3 = (right + left) / (right - left);
        Single value4 = (top + bottom) / (top - bottom);
        Single value5 = -(far + near) / (far - near);
        Single value6 = -(2f * far * near) / (far - near);
        Single value7 = -1f;
        Matrix4x4 result = default(Matrix4x4);
        result[0, 0] = value;
        result[0, 1] = 0f;
        result[0, 2] = value3;
        result[0, 3] = 0f;
        result[1, 0] = 0f;
        result[1, 1] = value2;
        result[1, 2] = value4;
        result[1, 3] = 0f;
        result[2, 0] = 0f;
        result[2, 1] = 0f;
        result[2, 2] = value5;
        result[2, 3] = value6;
        result[3, 0] = 0f;
        result[3, 1] = 0f;
        result[3, 2] = value7;
        result[3, 3] = 0f;
        return result;
    }

    public static void SetOrthoZ(ref Matrix4x4 m, Single near, Single far)
    {
        Single value = -(far + near) / (far - near);
        Single value2 = -(2f * far * near) / (far - near);
        Single value3 = -1f;
        m[2, 0] = 0f;
        m[2, 1] = 0f;
        m[2, 2] = value;
        m[2, 3] = value2;
        m[3, 0] = 0f;
        m[3, 1] = 0f;
        m[3, 2] = value3;
        m[3, 3] = 0f;
    }

    private Matrix4x4 PsxRotTrans2UnityModelView(Matrix4x4 psxRot, Vector3 psxTrans)
    {
        Matrix4x4 lhs = Matrix4x4.TRS(psxTrans, Quaternion.identity, Vector3.one);
        return lhs * psxRot;
    }

    public static Matrix4x4 PsxProj2UnityProj(Single zNear, Single zFar)
    {
        Single bottom = FieldMap.PsxScreenHeightNative / 2.2f;
        Single top = FieldMap.PsxScreenHeightNative - bottom;

        return PsxCamera.PerspectiveOffCenter(-FieldMap.HalfScreenWidth, FieldMap.HalfScreenWidth, -bottom, top, zNear, zFar);
    }

    public const Int32 DefaultClipDistance = 16383;

    public Int16[,] PsxCameraRotation = new Int16[3, 3];

    public Int32[] PsxCameraTranslation = new Int32[3];

    public Int32 PsxGeomScreen;

    public Int32 ClipDistance = 16383;
}

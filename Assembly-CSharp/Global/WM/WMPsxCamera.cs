using System;
using UnityEngine;
using Object = System.Object;

public class WMPsxCamera
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

	public Matrix4x4 PsxMatrix2UnityMatrix(Int16[,] rotation)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity.m00 = PSX.Fp12ToFloat(rotation[0, 0]);
		identity.m01 = PSX.Fp12ToFloat(rotation[0, 1]);
		identity.m02 = PSX.Fp12ToFloat(rotation[0, 2]);
		identity.m10 = PSX.Fp12ToFloat(rotation[1, 0]);
		identity.m11 = PSX.Fp12ToFloat(rotation[1, 1]);
		identity.m12 = PSX.Fp12ToFloat(rotation[1, 2]);
		identity.m20 = PSX.Fp12ToFloat(rotation[2, 0]);
		identity.m21 = PSX.Fp12ToFloat(rotation[2, 1]);
		identity.m22 = PSX.Fp12ToFloat(rotation[2, 2]);
		return identity;
	}

	public Matrix4x4 PerspectiveOffCenter(Single left, Single right, Single bottom, Single top, Single near, Single far)
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

	private Matrix4x4 PsxRotTrans2UnityModelView(Matrix4x4 psxRot, Vector3 psxTrans)
	{
		Matrix4x4 lhs = Matrix4x4.TRS(psxTrans, Quaternion.identity, Vector3.one);
		return lhs * psxRot;
	}

	private Matrix4x4 PsxProj2UnityProj(Single psxGeomScreen, Single clipDistance)
	{
		Single halfSceneWidth = FieldMap.HalfFieldWidth;
		Single halfSceneHeight = FieldMap.HalfFieldHeight;
		Single far = psxGeomScreen + clipDistance;
		return this.PerspectiveOffCenter(-halfSceneWidth, halfSceneWidth, -halfSceneHeight, halfSceneHeight, psxGeomScreen, far);
	}

	public void ConvertCameraPsx2UnityOld(Camera unityCam)
	{
		Vector3 psxTrans = new Vector3((Single)this.PsxCameraTranslation[0], (Single)this.PsxCameraTranslation[1], (Single)this.PsxCameraTranslation[2]);
		Matrix4x4 psxRot = this.PsxMatrix2UnityMatrix(this.PsxCameraRotation);
		Matrix4x4 matrix4x = this.PsxRotTrans2UnityModelView(psxRot, psxTrans);
		matrix4x *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, -1f, 1f));
		unityCam.worldToCameraMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f, -1f, -1f)) * matrix4x;
		Int16[,] psxRotation = new Int16[3, 3];
		Int32[] psxTranslation = new Int32[3];
		WMPsxCamera.UnityMatrix2PsxMatrix(unityCam.worldToCameraMatrix, psxRotation, psxTranslation);
		unityCam.nearClipPlane = (Single)this.PsxGeomScreen;
		unityCam.farClipPlane = (Single)(this.PsxGeomScreen + this.ClipDistance);
		unityCam.projectionMatrix = this.PsxProj2UnityProj((Single)this.PsxGeomScreen, (Single)this.ClipDistance);
	}

	public Matrix4x4 GetPsxProj2UnityProj()
	{
		return this.PsxProj2UnityProj((Single)this.PsxGeomScreen, (Single)this.ClipDistance);
	}

	public Quaternion QuaternionFromMatrix(Matrix4x4 m)
	{
		return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
	}

	public void ConvertCameraUnity2Psx(Camera unityCam)
	{
		WMPsxCamera.UnityMatrix2PsxMatrix(unityCam.worldToCameraMatrix, this.PsxCameraRotation, this.PsxCameraTranslation);
		this.PsxGeomScreen = (Int32)unityCam.nearClipPlane;
		this.ClipDistance = 32766;
		unityCam.projectionMatrix = this.PsxProj2UnityProj((Single)this.PsxGeomScreen, (Single)this.ClipDistance);
	}

	public const Int32 DefaultClipDistance = 32766;

	public Int16[,] PsxCameraRotation = new Int16[3, 3];

	public Int32[] PsxCameraTranslation = new Int32[3];

	public Int32 PsxGeomScreen;

	public Int32 ClipDistance = 16383;
}

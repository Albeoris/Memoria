using System;
using System.Collections.Generic;
using UnityEngine;
using FF9;

namespace Memoria
{
	static class SmoothFrameUpdater_Battle
	{
		// Max (squared) distance per frame to be considered as a smooth movement for battle units
		private const Single ActorSmoothMovementMaxSqr = 600f * 600f;
		// Max degree turn per frame to be considered as a smooth movement for field actors
		private const Single ActorSmoothTurnMaxDeg = 30f;
		// Max scaling factor per frame to be considered as a smooth movement for field actors
		private const Single ActorSmoothScaleMaxChange = 1.3f;
		private const Single ActorSmoothScaleMinChange = 1f / 1.3f;
		// Max (squared) distance per frame to be considered as a smooth movement for the camera
		private const Single CameraSmoothMovementMaxSqr = 450f * 450f;
		// Max degree turn per frame to be considered as a smooth movement for the camera
		private const Single CameraSmoothTurnMaxDeg = 15f;

		// Disable smooth effects for the duration of a couple of main loop ticks
		public static Int32 Skip
		{
			get => _skipCount;
			set
			{
				_skipCount = value;
				if (_skipCount > 0)
				{
					_cameraRegistered = false;
					for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
					{
						if (next.bi.slave == 0 && next.gameObject != null && next.gameObject.activeInHierarchy)
						{
							next._smoothUpdateRegistered = false;
							next._smoothUpdatePlayingAnim = false;
						}
					}
				}
			}
		}

		public static void RegisterState()
		{
			for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
			{
				if (next.bi.slave == 0 && next.gameObject != null && next.gameObject.activeInHierarchy)
				{
					if (next._smoothUpdateRegistered)
					{
						next._smoothUpdatePosPrevious = next._smoothUpdatePosActual;
						next._smoothUpdateRotPrevious = next._smoothUpdateRotActual;
						next._smoothUpdateScalePrevious = next._smoothUpdateScaleActual;
					}
					else
					{
						next._smoothUpdatePosPrevious = next.gameObject.transform.position;
						next._smoothUpdateRotPrevious = next.gameObject.transform.rotation;
						next._smoothUpdateScalePrevious = next.gameObject.transform.localScale;
					}
					next._smoothUpdatePosActual = next.gameObject.transform.position;
					next._smoothUpdateRotActual = next.gameObject.transform.rotation;
					next._smoothUpdateScaleActual = next.gameObject.transform.localScale;
					next._smoothUpdateRegistered = true;
				}
			}
			Camera camera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
			if (camera != null)
			{
				if (_cameraRegistered)
				{
					_cameraW2CMatrixPrevious = _cameraW2CMatrixActual;
					_cameraProjMatrixPrevious = _cameraProjMatrixActual;
				}
				else
				{
					_cameraW2CMatrixPrevious = camera.worldToCameraMatrix;
					_cameraProjMatrixPrevious = camera.projectionMatrix;
				}
				_cameraW2CMatrixActual = camera.worldToCameraMatrix;
				_cameraProjMatrixActual = camera.projectionMatrix;
				_cameraRegistered = true;
			}
		}

		public static void Apply(Single smoothFactor)
		{
			if (_skipCount > 0)
				return;
			Single unclampedFactor = 1f + smoothFactor;
			for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
			{
				if (next.bi.slave == 0 && next.gameObject != null && next.gameObject.activeInHierarchy)
				{
					Vector3 frameMove = next._smoothUpdatePosActual - next._smoothUpdatePosPrevious;
					if (frameMove.sqrMagnitude > 0f && frameMove.sqrMagnitude < ActorSmoothMovementMaxSqr)
						next.gameObject.transform.position = next._smoothUpdatePosActual + smoothFactor * frameMove;
					if (Quaternion.Angle(next._smoothUpdateRotPrevious, next._smoothUpdateRotActual) < ActorSmoothTurnMaxDeg)
						next.gameObject.transform.rotation = Quaternion.LerpUnclamped(next._smoothUpdateRotPrevious, next._smoothUpdateRotActual, unclampedFactor);
					Single minScaleFactor, maxScaleFactor;
					if (next._smoothUpdateScalePrevious.x != 0f)
					{
						minScaleFactor = next._smoothUpdateScaleActual.x / next._smoothUpdateScalePrevious.x;
						maxScaleFactor = minScaleFactor;
					}
					else
					{
						minScaleFactor = ActorSmoothScaleMinChange;
						maxScaleFactor = ActorSmoothScaleMaxChange;
					}
					if (next._smoothUpdateScalePrevious.y != 0f)
					{
						Single ratio = next._smoothUpdateScaleActual.y / next._smoothUpdateScalePrevious.y;
						minScaleFactor = Mathf.Min(minScaleFactor, ratio);
						maxScaleFactor = Mathf.Max(maxScaleFactor, ratio);
					}
					else
					{
						minScaleFactor = ActorSmoothScaleMinChange;
						maxScaleFactor = ActorSmoothScaleMaxChange;
					}
					if (next._smoothUpdateScalePrevious.z != 0f)
					{
						Single ratio = next._smoothUpdateScaleActual.z / next._smoothUpdateScalePrevious.z;
						minScaleFactor = Mathf.Min(minScaleFactor, ratio);
						maxScaleFactor = Mathf.Max(maxScaleFactor, ratio);
					}
					else
					{
						minScaleFactor = ActorSmoothScaleMinChange;
						maxScaleFactor = ActorSmoothScaleMaxChange;
					}
					if (minScaleFactor > ActorSmoothScaleMinChange && maxScaleFactor < ActorSmoothScaleMaxChange)
						next.gameObject.transform.localScale = unclampedFactor * next._smoothUpdateScaleActual - smoothFactor * next._smoothUpdateScalePrevious;
					if (next._smoothUpdatePlayingAnim)
					{
						GameObject go = next.gameObject;
						AnimationState anim = go.GetComponent<Animation>()[next.currentAnimationName];
						Single animTime = Mathf.LerpUnclamped(next._smoothUpdateAnimTimePrevious, next._smoothUpdateAnimTimeActual, unclampedFactor);
						animTime = Mathf.Max(0f, Mathf.Min(anim.length, animTime));
						anim.time = animTime;
						go.GetComponent<Animation>().Sample();
					}
				}
			}
			Camera camera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
			if (_cameraRegistered && camera != null)
			{
				Vector3 cameraMove = MatrixGetTranslation(_cameraW2CMatrixActual) - MatrixGetTranslation(_cameraW2CMatrixPrevious);
				if (cameraMove.sqrMagnitude >= CameraSmoothMovementMaxSqr)
					return;
				//Single cameraAngle = Quaternion.Angle(MatrixGetRotation(_cameraW2CMatrixActual), MatrixGetRotation(_cameraW2CMatrixPrevious));
				//if (cameraAngle >= CameraSmoothTurnMaxDeg)
				//	return;
				camera.worldToCameraMatrix = MatrixLerpUnclamped(_cameraW2CMatrixPrevious, _cameraW2CMatrixActual, unclampedFactor);
				camera.projectionMatrix = MatrixLerpUnclamped(_cameraProjMatrixPrevious, _cameraProjMatrixActual, unclampedFactor);
			}
		}

		public static void ResetState()
		{
			if (_skipCount > 0)
				_skipCount--;
			for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
			{
				if (next.bi.slave == 0 && next.gameObject != null && next.gameObject.activeInHierarchy)
				{
					if (next._smoothUpdateRegistered)
					{
						next.gameObject.transform.position = next._smoothUpdatePosActual;
						next.gameObject.transform.rotation = next._smoothUpdateRotActual;
						next.gameObject.transform.localScale = next._smoothUpdateScaleActual;
					}
					if (next._smoothUpdatePlayingAnim)
						next.gameObject.GetComponent<Animation>()[next.currentAnimationName].time = next._smoothUpdateAnimTimeActual;
				}
			}
			Camera camera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
			if (_cameraRegistered && camera != null)
			{
				camera.worldToCameraMatrix = _cameraW2CMatrixActual;
				camera.projectionMatrix = _cameraProjMatrixActual;
			}
		}

		private static Vector3 MatrixGetTranslation(Matrix4x4 mat)
		{
			// Assume mat.m33 == 1
			return new Vector3(mat.m03, mat.m13, mat.m23);
		}

		private static Quaternion MatrixGetRotation(Matrix4x4 mat)
		{
			// TODO: try to get it continuous
			Quaternion q = new Quaternion();
			q.w = Mathf.Sqrt(Mathf.Max(0, 1f + mat[0, 0] + mat[1, 1] + mat[2, 2])) / 2f;
			q.x = Mathf.Sqrt(Mathf.Max(0, 1f + mat[0, 0] - mat[1, 1] - mat[2, 2])) / 2f;
			q.y = Mathf.Sqrt(Mathf.Max(0, 1f - mat[0, 0] + mat[1, 1] - mat[2, 2])) / 2f;
			q.z = Mathf.Sqrt(Mathf.Max(0, 1f - mat[0, 0] - mat[1, 1] + mat[2, 2])) / 2f;
			q.x *= Mathf.Sign(q.x * (mat[2, 1] - mat[1, 2]));
			q.y *= Mathf.Sign(q.y * (mat[0, 2] - mat[2, 0]));
			q.z *= Mathf.Sign(q.z * (mat[1, 0] - mat[0, 1]));
			return q;
		}

		private static Matrix4x4 MatrixLerpUnclamped(Matrix4x4 from, Matrix4x4 to, Single t)
		{
			Matrix4x4 lerped = new Matrix4x4();
			for (int i = 0; i < 16; i++)
				lerped[i] = Mathf.LerpUnclamped(from[i], to[i], t);
			return lerped;
		}

		private static Int32 _skipCount = 0;
		private static Boolean _cameraRegistered = false;
		private static Matrix4x4 _cameraW2CMatrixPrevious;
		private static Matrix4x4 _cameraW2CMatrixActual;
		private static Matrix4x4 _cameraProjMatrixPrevious;
		private static Matrix4x4 _cameraProjMatrixActual;
	}
}

partial class BTL_DATA
{
	public Boolean _smoothUpdateRegistered = false;
	public Vector3 _smoothUpdatePosPrevious;
	public Vector3 _smoothUpdatePosActual;
	public Quaternion _smoothUpdateRotPrevious;
	public Quaternion _smoothUpdateRotActual;
	public Vector3 _smoothUpdateScalePrevious;
	public Vector3 _smoothUpdateScaleActual;
	public Boolean _smoothUpdatePlayingAnim = false;
	public Single _smoothUpdateAnimTimePrevious;
	public Single _smoothUpdateAnimTimeActual;
}

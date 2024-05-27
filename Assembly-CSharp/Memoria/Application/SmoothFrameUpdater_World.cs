using Memoria.Prime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria
{
	static class SmoothFrameUpdater_World
	{
		// Max (squared) distance per frame to be considered as a smooth movement for wmActors
		private static readonly Single ActorSmoothMovementMaxSqr = ff9.S(800) * ff9.S(800);

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
					for (ObjList objList = ff9.GetActiveObjList(); objList != null; objList = objList.next)
					{
						Obj obj = objList.obj;
						if (obj != null && obj.cid == 4)
						{
							WMActor wmActor = (obj as Actor)?.wmActor;
							if (wmActor != null)
							{
								wmActor._smoothUpdateRegistered = false;
								wmActor._smoothUpdatePlayingAnim = false;
							}
						}
					}
				}
			}
		}

		public static void RegisterState()
		{
			for (ObjList objList = ff9.GetActiveObjList(); objList != null; objList = objList.next)
			{
				Obj obj = objList.obj;
				if (obj.cid == 4)
				{
					WMActor wmActor = (obj as Actor)?.wmActor;
					if (wmActor != null)
					{
						if (wmActor._smoothUpdateRegistered)
							wmActor._smoothUpdatePosPrevious = wmActor._smoothUpdatePosActual + ff9.world.BlockShift;
						else
							wmActor._smoothUpdatePosPrevious = wmActor.transform.position;
						wmActor._smoothUpdatePosActual = wmActor.transform.position;
						wmActor._smoothUpdateRegistered = true;
					}
				}
			}
			if (ff9.world.MainCamera != null)
			{
				if (_cameraRegistered)
				{
					_cameraFieldOfViewPrevious = _cameraFieldOfViewActual;
					_cameraPosPrevious = _cameraPosActual + ff9.world.BlockShift;
					_cameraRotPrevious = _cameraRotActual;
				}
				else
				{
					_cameraFieldOfViewPrevious = ff9.world.MainCamera.fieldOfView;
					_cameraPosPrevious = ff9.world.MainCamera.transform.position;
					_cameraRotPrevious = ff9.world.MainCamera.transform.rotation;
				}
				_cameraFieldOfViewActual = ff9.world.MainCamera.fieldOfView;
				_cameraPosActual = ff9.world.MainCamera.transform.position;
				_cameraRotActual = ff9.world.MainCamera.transform.rotation;
				_cameraRegistered = true;
			}
			Apply(0f);
		}

		public static void Apply(Single smoothFactor)
		{
			if (_skipCount > 0)
				return;
			SFXData.LoadLoop();
			for (ObjList objList = ff9.GetActiveObjList(); objList != null; objList = objList.next)
			{
				Obj obj = objList.obj;
				if (obj.cid == 4)
				{
					WMActor wmActor = (obj as Actor)?.wmActor;
					if (wmActor != null && wmActor._smoothUpdateRegistered && ff9.objIsVisible(obj))
					{
						wmActor.transform.position = Vector3.Lerp(wmActor._smoothUpdatePosPrevious, wmActor._smoothUpdatePosActual, smoothFactor);
						if (wmActor._smoothUpdatePlayingAnim)
						{
							GameObject go = wmActor.originalActor.go;
							String animName = FF9DBAll.AnimationDB.GetValue(wmActor.originalActor.anim);
							AnimationState anim = go.GetComponent<Animation>()[animName];
							if (anim != null)
							{
								Single animTime;
								if (wmActor._smoothUpdateAnimTimePrevious > wmActor._smoothUpdateAnimTimeActual)
									animTime = Mathf.Lerp(wmActor._smoothUpdateAnimTimePrevious, anim.length, smoothFactor);
								else
									animTime = Mathf.Lerp(wmActor._smoothUpdateAnimTimePrevious, wmActor._smoothUpdateAnimTimeActual, smoothFactor);

								/*if(wmActor.name == "obj14_WM")
									Log.Message($"[DEBUG] {wmActor.name} prev: {wmActor._smoothUpdateAnimTimePrevious} actual:{wmActor._smoothUpdateAnimTimeActual} length: {anim.length} t:{smoothFactor} animTime: {animTime}");*/

								anim.time = Mathf.Clamp(anim.length, 0f, animTime);
								go.GetComponent<Animation>().Sample();
							}
						}
					}
				}
			}
			if (_cameraRegistered && ff9.world.MainCamera != null)
			{
				ff9.world.MainCamera.fieldOfView = Mathf.LerpUnclamped(_cameraFieldOfViewPrevious, _cameraFieldOfViewActual, smoothFactor);
				ff9.world.MainCamera.transform.position = Vector3.LerpUnclamped(_cameraPosPrevious, _cameraPosActual, smoothFactor);
				ff9.world.MainCamera.transform.rotation = Quaternion.LerpUnclamped(_cameraRotPrevious, _cameraRotActual, smoothFactor);
			}
		}

		public static void ResetState()
		{
			if (_skipCount > 0)
				_skipCount--;
			for (ObjList objList = ff9.GetActiveObjList(); objList != null; objList = objList.next)
			{
				Obj obj = objList.obj;
				if (obj.cid == 4)
				{
					WMActor wmActor = (obj as Actor)?.wmActor;
					if (wmActor != null)
					{
						if (wmActor._smoothUpdateRegistered && ff9.objIsVisible(obj))
							wmActor.transform.position = wmActor._smoothUpdatePosActual;
						if (wmActor._smoothUpdatePlayingAnim)
						{
							AnimationState anim = wmActor.originalActor.go.GetComponent<Animation>()[FF9DBAll.AnimationDB.GetValue(wmActor.originalActor.anim)];
							if (anim != null)
								anim.time = wmActor._smoothUpdateAnimTimeActual;
						}
					}
				}
			}
			if (_cameraRegistered && ff9.world.MainCamera != null)
			{
				ff9.world.MainCamera.fieldOfView = _cameraFieldOfViewActual;
				ff9.world.MainCamera.transform.position = _cameraPosActual;
				ff9.world.MainCamera.transform.rotation = _cameraRotActual;
			}
		}

		private static Int32 _skipCount = 0;
		private static Boolean _cameraRegistered = false;
		private static Single _cameraFieldOfViewPrevious;
		private static Single _cameraFieldOfViewActual;
		private static Vector3 _cameraPosPrevious;
		private static Vector3 _cameraPosActual;
		private static Quaternion _cameraRotPrevious;
		private static Quaternion _cameraRotActual;
	}
}

partial class WMActor
{
	public Boolean _smoothUpdateRegistered = false;
	public Vector3 _smoothUpdatePosPrevious;
	public Vector3 _smoothUpdatePosActual;
	public Boolean _smoothUpdatePlayingAnim = false;
	public Single _smoothUpdateAnimTimePrevious;
	public Single _smoothUpdateAnimTimeActual;
}

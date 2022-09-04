using System;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria
{
	static class SmoothFrameUpdater_Field
	{
		// TODO: add a smooth effect for EBG overlays (FieldMap.scene.overlayList[].transform.localPosition etc)
		// TODO: add a smooth effect for field SPS (FieldSPSSystem._spsList[].pos etc)

		// Max (squared) distance per frame to be considered as a smooth movement for field actors
		private static readonly Single ActorSmoothMovementMaxSqr = 100 * 100;
		// Max degree turn per frame to be considered as a smooth movement for field actors
		private static readonly Single ActorSmoothTurnMaxDeg = 20;
		// Max (squared) distance per frame to be considered as a smooth movement for the camera
		private static readonly Single CameraSmoothMovementMaxSqr = 450f * 450f;

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
					EventEngine eEngine = PersistenSingleton<EventEngine>.Instance;
					for (ObjList objList = eEngine?.GetActiveObjList(); objList != null; objList = objList.next)
					{
						if (objList.obj != null && objList.obj.cid == 4)
						{
							FieldMapActorController actor = (objList.obj as Actor)?.fieldMapActorController;
							if (actor != null)
							{
								actor._smoothUpdateRegistered = false;
								actor._smoothUpdatePlayingAnim = false;
							}
						}
					}
				}
			}
		}

		public static void RegisterState()
		{
			FieldMap fieldmap = PersistenSingleton<EventEngine>.Instance.fieldmap;
			EventEngine eEngine = PersistenSingleton<EventEngine>.Instance;
			for (ObjList objList = eEngine?.GetActiveObjList(); objList != null; objList = objList.next)
			{
				if (objList.obj != null && eEngine.objIsVisible(objList.obj) && objList.obj.cid == 4)
				{
					FieldMapActorController actor = (objList.obj as Actor)?.fieldMapActorController;
					if (actor?.originalActor.go != null)
					{
						GameObject go = actor.originalActor.go;
						if (actor._smoothUpdateRegistered)
						{
							actor._smoothUpdatePosPrevious = actor._smoothUpdatePosActual;
							actor._smoothUpdateRotPrevious = actor._smoothUpdateRotActual;
						}
						else
						{
							actor._smoothUpdatePosPrevious = go.transform.position;
							actor._smoothUpdateRotPrevious = go.transform.rotation;
						}
						actor._smoothUpdatePosActual = go.transform.position;
						actor._smoothUpdateRotActual = go.transform.rotation;
						actor._smoothUpdateRegistered = true;
					}
				}
			}
			Camera mainCamera = fieldmap?.GetMainCamera();
			if (mainCamera != null)
			{
				if (_cameraRegistered)
					_cameraPosPrevious = _cameraPosActual;
				else
					_cameraPosPrevious = mainCamera.transform.position;
				_cameraPosActual = mainCamera.transform.position;
				_cameraRegistered = true;
			}
		}

		public static void Apply(Single smoothFactor)
		{
			if (_skipCount > 0)
				return;
			FieldMap fieldmap = PersistenSingleton<EventEngine>.Instance.fieldmap;
			EventEngine eEngine = PersistenSingleton<EventEngine>.Instance;
			Single unclampedFactor = 1f + smoothFactor;
			for (ObjList objList = eEngine?.GetActiveObjList(); objList != null; objList = objList.next)
			{
				if (objList.obj != null && eEngine.objIsVisible(objList.obj) && objList.obj.cid == 4)
				{
					FieldMapActorController actor = (objList.obj as Actor)?.fieldMapActorController;
					if (actor?.originalActor.go != null && actor._smoothUpdateRegistered)
					{
						GameObject go = actor.originalActor.go;
						Vector3 frameMove = actor._smoothUpdatePosActual - actor._smoothUpdatePosPrevious;
						if (frameMove.sqrMagnitude > 0f && frameMove.sqrMagnitude < ActorSmoothMovementMaxSqr)
							go.transform.position = actor._smoothUpdatePosActual + smoothFactor * frameMove;
						if (Quaternion.Angle(actor._smoothUpdateRotPrevious, actor._smoothUpdateRotActual) < ActorSmoothTurnMaxDeg)
							go.transform.rotation = Quaternion.LerpUnclamped(actor._smoothUpdateRotPrevious, actor._smoothUpdateRotActual, unclampedFactor);
						if (!actor._smoothUpdatePlayingAnim)
							continue;
						String animName = FF9DBAll.AnimationDB.GetValue(actor.originalActor.anim);
						AnimationState anim = go.GetComponent<Animation>()[animName];
						Single animTime = Mathf.LerpUnclamped(actor._smoothUpdateAnimTimePrevious, actor._smoothUpdateAnimTimeActual, unclampedFactor);
						animTime = Mathf.Max(0f, Mathf.Min(anim.length, animTime));
						anim.time = animTime;
						go.GetComponent<Animation>().Sample();
					}
				}
			}
			if (_cameraRegistered)
			{
				Camera mainCamera = fieldmap?.GetMainCamera();
				if (mainCamera != null && (_cameraPosActual - _cameraPosPrevious).sqrMagnitude < CameraSmoothMovementMaxSqr)
					mainCamera.transform.position = Vector3.LerpUnclamped(_cameraPosPrevious, _cameraPosActual, unclampedFactor);
			}
		}

		public static void ResetState()
		{
			if (_skipCount > 0)
				_skipCount--;
			FieldMap fieldmap = PersistenSingleton<EventEngine>.Instance.fieldmap;
			EventEngine eEngine = PersistenSingleton<EventEngine>.Instance;
			for (ObjList objList = eEngine?.GetActiveObjList(); objList != null; objList = objList.next)
			{
				if (objList.obj != null && objList.obj.cid == 4)
				{
					FieldMapActorController actor = (objList.obj as Actor)?.fieldMapActorController;
					if (actor?.originalActor.go != null && actor._smoothUpdateRegistered)
					{
						GameObject go = actor.originalActor.go;
						if (actor._smoothUpdateRegistered)
						{
							go.transform.position = actor._smoothUpdatePosActual;
							go.transform.rotation = actor._smoothUpdateRotActual;
						}
						if (actor._smoothUpdatePlayingAnim)
							go.GetComponent<Animation>()[FF9DBAll.AnimationDB.GetValue(actor.originalActor.anim)].time = actor._smoothUpdateAnimTimeActual;
					}
				}
			}
			if (_cameraRegistered)
			{
				Camera mainCamera = fieldmap?.GetMainCamera();
				if (mainCamera != null)
					mainCamera.transform.position = _cameraPosActual;
			}
		}

		private static Int32 _skipCount = 0;
		private static Boolean _cameraRegistered = false;
		private static Vector3 _cameraPosPrevious;
		private static Vector3 _cameraPosActual;
	}
}

partial class FieldMapActorController
{
	public Boolean _smoothUpdateRegistered = false;
	public Vector3 _smoothUpdatePosPrevious;
	public Vector3 _smoothUpdatePosActual;
	public Quaternion _smoothUpdateRotPrevious;
	public Quaternion _smoothUpdateRotActual;
	public Boolean _smoothUpdatePlayingAnim = false;
	public Single _smoothUpdateAnimTimePrevious;
	public Single _smoothUpdateAnimTimeActual;
}

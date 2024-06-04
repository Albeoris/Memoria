using Memoria.Prime;
using System;
using UnityEngine;

namespace Memoria
{
	static class SmoothFrameUpdater_Field
	{
		// TODO: add a smooth effect for field SPS (FieldSPSSystem._spsList[].pos etc)

		// Max (squared) distance per frame to be considered as a smooth movement for field actors
		private const Single ActorSmoothMovementMaxSqr = 400f * 400f; // Iifa tree leaf spiral moves at ~350
		// Max degree turn per frame to be considered as a smooth movement for field actors
		// private const Single ActorSmoothTurnMaxDeg = 45f;
		// Max (squared) distance per frame to be considered as a smooth movement for EBG overlays
		//private const Single OverlaySmoothMovementMaxSqr = 20f * 20f;
		// Max (squared) distance per frame to be considered as a smooth movement for the camera
		private const Single CameraSmoothMovementMaxSqr = 450f * 450f;

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
							}
						}
					}
					foreach (FieldSPS fieldSPS in EventEngine.Instance.fieldSps.SpsList)
					{
						if (fieldSPS == null || !fieldSPS.enabled)
							continue;
						fieldSPS._smoothUpdateRegistered = false;
					}
				}
			}
		}

		public static void RegisterState()
		{
			FieldMap fieldmap = PersistenSingleton<EventEngine>.Instance.fieldmap;
			EventEngine eEngine = PersistenSingleton<EventEngine>.Instance;
			// Actors
			for (ObjList objList = eEngine?.GetActiveObjList(); objList != null; objList = objList.next)
			{
				if (objList.obj != null && eEngine.objIsVisible(objList.obj) && objList.obj.cid == 4)
				{
					FieldMapActorController actor = (objList.obj as Actor)?.fieldMapActorController;
					if (actor?.originalActor.go != null)
					{
						GameObject go = actor.originalActor.go;
						String curAnim = FF9DBAll.AnimationDB.GetValue(actor.originalActor.anim);
						Animation anim = actor.gameObject.GetComponent<Animation>();
						AnimationState animState = anim[curAnim];

						if (actor._smoothUpdateRegistered)
						{
							actor._smoothUpdatePosPrevious = actor._smoothUpdatePosActual;
							actor._smoothUpdateShadowPrevious = actor._smoothUpdateShadowActual;
							actor._smoothUpdateRotPrevious = actor._smoothUpdateRotActual;

							actor._smoothUpdateAnimNamePrevious = actor._smoothUpdateAnimNameActual;
							actor._smoothUpdateAnimTimePrevious = actor._smoothUpdateAnimTimeActual;
						}
						else
						{
							actor._smoothUpdatePosPrevious = go.transform.position;
							actor._smoothUpdateShadowPrevious = (objList.obj as Actor).fieldMapActor.shadowTran.position;
							actor._smoothUpdateRotPrevious = go.transform.rotation;

							actor._smoothUpdateAnimNamePrevious = curAnim;
							actor._smoothUpdateAnimTimePrevious = animState.time;
						}
						actor._smoothUpdatePosActual = go.transform.position;
						actor._smoothUpdateShadowActual = (objList.obj as Actor).fieldMapActor.shadowTran.position;
						actor._smoothUpdateRotActual = go.transform.rotation;

						actor._smoothUpdateAnimNameActual = curAnim;
						actor._smoothUpdateAnimTimeActual = animState.time;

						if (actor._smoothUpdateRegistered && actor._smoothUpdateAnimNamePrevious == actor._smoothUpdateAnimNameActual)
						{
							Single speed = actor._smoothUpdateAnimTimeActual - actor._smoothUpdateAnimTimePrevious;
							Int32 direction = actor.originalActor.animFrame - actor.originalActor.lastAnimFrame;
							Boolean hasLooped =
								(direction < 0 && actor._smoothUpdateAnimTimeActual + speed < 0f) ||
								(direction > 0 && actor._smoothUpdateAnimTimeActual + speed > animState.length);
							if (!hasLooped)
								actor._smoothUpdateAnimSpeed = speed;
						}
						else
						{
							actor._smoothUpdateAnimTimePrevious = actor._smoothUpdateAnimTimeActual;
							actor._smoothUpdateAnimSpeed = 0f;
						}

						actor._smoothUpdateRegistered = true;
					}
				}
			}
			// SPS
			foreach (FieldSPS fieldSPS in EventEngine.Instance.fieldSps.SpsList)
			{
				if (fieldSPS == null || !fieldSPS.enabled)
					continue;

				if (fieldSPS._smoothUpdateRegistered)
				{
					fieldSPS._smoothUpdatePosPrevious = fieldSPS._smoothUpdatePosActual;
					fieldSPS._smoothUpdateRotPrevious = fieldSPS._smoothUpdateRotActual;
					fieldSPS._smoothUpdateScalePrevious = fieldSPS._smoothUpdateScaleActual;
				}
				else
				{
					fieldSPS._smoothUpdatePosPrevious = fieldSPS.pos;
					fieldSPS._smoothUpdateRotPrevious = Quaternion.Euler(fieldSPS.rot.x, fieldSPS.rot.y, fieldSPS.rot.z);
					fieldSPS._smoothUpdateScalePrevious = fieldSPS.scale;
				}

				fieldSPS._smoothUpdatePosActual = fieldSPS.pos;
				fieldSPS._smoothUpdateRotActual = Quaternion.Euler(fieldSPS.rot.x, fieldSPS.rot.y, fieldSPS.rot.z);
				fieldSPS._smoothUpdateScaleActual = fieldSPS.scale;

				fieldSPS._smoothUpdateRegistered = true;
			}
			// Layers
			// Interfere with snouz's "Camera stabilizer"
			/*if (fieldmap?.scene?.overlayList != null)
			{
				foreach (BGOVERLAY_DEF bgLayer in fieldmap.scene.overlayList)
				{
					if (bgLayer.transform != null && !((bgLayer.flags & BGOVERLAY_DEF.OVERLAY_FLAG.Loop) != 0) && !((bgLayer.flags & BGOVERLAY_DEF.OVERLAY_FLAG.ScrollWithOffset) != 0)) // && (bgLayer.flags & BGOVERLAY_DEF.OVERLAY_FLAG.Active) != 0)
					{
						if (bgLayer._smoothUpdateRegistered)
							bgLayer._smoothUpdatePosPrevious = bgLayer._smoothUpdatePosActual;
						else
							bgLayer._smoothUpdatePosPrevious = bgLayer.transform.position;
						bgLayer._smoothUpdatePosActual = bgLayer.transform.position;
						bgLayer._smoothUpdateRegistered = true;
					}
				}
			}*/
			// Camera
			Camera mainCamera = fieldmap?.GetMainCamera();
			if (mainCamera != null)
			{
				_cameraReverseMove = _cameraRegistered && (mainCamera.transform.position - _cameraPosPrevious).sqrMagnitude < 1f;
				if (_cameraRegistered)
					_cameraPosPrevious = _cameraPosActual;
				else
					_cameraPosPrevious = mainCamera.transform.position;
				_cameraPosActual = mainCamera.transform.position;
				_cameraRegistered = true;
			}

			Apply(0f);
		}

		public static void Apply(Single smoothFactor)
		{
			if (_skipCount > 0 || smoothFactor > 1f)
				return;
			SFXData.LoadLoop();
			FieldMap fieldmap = PersistenSingleton<EventEngine>.Instance.fieldmap;
			EventEngine eEngine = PersistenSingleton<EventEngine>.Instance;
			for (ObjList objList = eEngine?.GetActiveObjList(); objList != null; objList = objList.next)
			{
				if (objList.obj != null && eEngine.objIsVisible(objList.obj) && objList.obj.cid == 4)
				{
					FieldMapActorController actor = (objList.obj as Actor)?.fieldMapActorController;
					if (actor?.originalActor.go != null && actor._smoothUpdateRegistered && actor._smoothUpdateAnimNamePrevious == actor._smoothUpdateAnimNameActual)
					{
						GameObject go = actor.originalActor.go;
						Animation anim = actor.originalActor.go.GetComponent<Animation>();
						AnimationState animState = anim[actor._smoothUpdateAnimNameActual];

						Vector3 frameMove = actor._smoothUpdatePosActual - actor._smoothUpdatePosPrevious;
						if (frameMove.sqrMagnitude > 0f && frameMove.sqrMagnitude < ActorSmoothMovementMaxSqr)
						{
							go.transform.position = Vector3.Lerp(actor._smoothUpdatePosPrevious, actor._smoothUpdatePosActual, smoothFactor);
							(objList.obj as Actor).fieldMapActor.shadowTran.position = Vector3.Lerp(actor._smoothUpdateShadowPrevious, actor._smoothUpdateShadowActual, smoothFactor);
						}
						//if (frameMove.sqrMagnitude >= ActorSmoothMovementMaxSqr) Log.Message($"[DEBUG] {Time.frameCount} {actor.name}_{actor.GetInstanceID()} {frameMove.sqrMagnitude} cur {go.transform.position} prev {actor._smoothUpdatePosPrevious} {actor._smoothUpdatePosActual} t {smoothFactor}");

						//if (Quaternion.Angle(actor._smoothUpdateRotPrevious, actor._smoothUpdateRotActual) < ActorSmoothTurnMaxDeg)
						go.transform.rotation = Quaternion.Lerp(actor._smoothUpdateRotPrevious, actor._smoothUpdateRotActual, smoothFactor);

						if (anim != null)
						{
							animState.time = Mathf.Lerp(actor._smoothUpdateAnimTimePrevious, actor._smoothUpdateAnimTimePrevious + actor._smoothUpdateAnimSpeed, smoothFactor);
							if (animState.time > animState.length)
								animState.time -= animState.length;
							else if (animState.time < 0f)
								animState.time += animState.length;
							anim.Sample();
							//if(actor.name == "obj15") Log.Message($"[DEBUG] {Time.frameCount} {actor.name} {animState.name} mag {frameMove.sqrMagnitude} ang {Quaternion.Angle(actor._smoothUpdateRotPrevious, actor._smoothUpdateRotActual)} cur {go.transform.position} prev {actor._smoothUpdatePosPrevious} {actor._smoothUpdatePosActual} t {smoothFactor}");
						}
						// if (actor.isPlayer) Log.Message($"[DEBUG] {Time.frameCount} {frameMove.sqrMagnitude} anim {animState.name} curTime {animState.time} prev {actor._smoothUpdateAnimTimePrevious} actual {actor._smoothUpdateAnimTimeActual} speed {actor._smoothUpdateAnimSpeed} length {animState.length} rot {go.transform.GetChildByName("bone000").rotation.eulerAngles.y} t {smoothFactor}");
					}
				}
			}
			foreach (FF9FieldCharState charState in FF9StateSystem.Field.FF9Field.loc.map.charStateArray.Values)
				fldchar.updateMirrorPosAndAnim(charState.mirror);
			foreach (FieldSPS fieldSPS in EventEngine.Instance.fieldSps.SpsList)
			{
				if (fieldSPS == null || !fieldSPS.enabled)
					continue;

				fieldSPS.pos = Vector3.Lerp(fieldSPS._smoothUpdatePosPrevious, fieldSPS._smoothUpdatePosActual, smoothFactor);
				fieldSPS.rot = Quaternion.Lerp(fieldSPS._smoothUpdateRotPrevious, fieldSPS._smoothUpdateRotActual, smoothFactor).eulerAngles;
				fieldSPS.scale = (Int32)Mathf.Lerp(fieldSPS._smoothUpdateScalePrevious, fieldSPS._smoothUpdateScaleActual, smoothFactor);
			}
			/*if (fieldmap?.scene?.overlayList != null)
			{
				foreach (BGOVERLAY_DEF bgLayer in fieldmap.scene.overlayList)
				{
					if (bgLayer.transform != null && bgLayer._smoothUpdateRegistered) // && (bgLayer.flags & BGOVERLAY_DEF.OVERLAY_FLAG.Active) != 0
					{
						//Vector3 frameMove = bgLayer._smoothUpdatePosActual - bgLayer._smoothUpdatePosPrevious;
						//if (frameMove.sqrMagnitude > 0f && frameMove.sqrMagnitude < OverlaySmoothMovementMaxSqr)
						bgLayer.transform.position = Vector3.Lerp(bgLayer._smoothUpdatePosPrevious, bgLayer._smoothUpdatePosActual, smoothFactor);
						Log.Message($"[DEBUG] {Time.frameCount} {bgLayer.transform.name} mag {(bgLayer._smoothUpdatePosActual-bgLayer._smoothUpdatePosPrevious).magnitude} cur {bgLayer.transform.position} prev {bgLayer._smoothUpdatePosPrevious} {bgLayer._smoothUpdatePosActual} t {smoothFactor}");
					}
				}
			}*/
			if (_cameraRegistered && !_cameraReverseMove)
			{
				Camera mainCamera = fieldmap?.GetMainCamera();
				if (mainCamera != null && (_cameraPosActual - _cameraPosPrevious).sqrMagnitude < CameraSmoothMovementMaxSqr)
					mainCamera.transform.position = Vector3.Lerp(_cameraPosPrevious, _cameraPosActual, smoothFactor);
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
							AnimationState anim = go.GetComponent<Animation>()[actor._smoothUpdateAnimNameActual];
							if (anim != null)
								anim.time = actor._smoothUpdateAnimTimeActual;
						}
					}
				}
			}
			/*if (fieldmap?.scene?.overlayList != null)
				foreach (BGOVERLAY_DEF bgLayer in fieldmap.scene.overlayList)
					if (bgLayer.transform != null && bgLayer._smoothUpdateRegistered) // && (bgLayer.flags & BGOVERLAY_DEF.OVERLAY_FLAG.Active) != 0
						bgLayer.transform.position = bgLayer._smoothUpdatePosActual;*/
			if (_cameraRegistered)
			{
				Camera mainCamera = fieldmap?.GetMainCamera();
				if (mainCamera != null)
					mainCamera.transform.position = _cameraPosActual;
			}
		}


		private static Int32 _skipCount = 0;
		private static Boolean _cameraRegistered = false;
		private static Boolean _cameraReverseMove = false; // Used to lower camera movement flickering effect in some situations
		private static Vector3 _cameraPosPrevious;
		private static Vector3 _cameraPosActual;
	}
}

partial class FieldMapActorController
{
	public Boolean _smoothUpdateRegistered = false;
	public Vector3 _smoothUpdatePosPrevious;
	public Vector3 _smoothUpdatePosActual;
	public Vector3 _smoothUpdateShadowPrevious;
	public Vector3 _smoothUpdateShadowActual;
	public Quaternion _smoothUpdateRotPrevious;
	public Quaternion _smoothUpdateRotActual;
	public String _smoothUpdateAnimNamePrevious;
	public String _smoothUpdateAnimNameActual;
	public Single _smoothUpdateAnimTimePrevious;
	public Single _smoothUpdateAnimTimeActual;
	public Single _smoothUpdateAnimSpeed;
}
partial class FieldSPS
{
	public Boolean _smoothUpdateRegistered = false;
	public Vector3 _smoothUpdatePosPrevious;
	public Vector3 _smoothUpdatePosActual;
	public Quaternion _smoothUpdateRotPrevious;
	public Quaternion _smoothUpdateRotActual;
	public Int32 _smoothUpdateScalePrevious;
	public Int32 _smoothUpdateScaleActual;
}
/*partial class BGOVERLAY_DEF
{
	public Boolean _smoothUpdateRegistered = false;
	public Vector3 _smoothUpdatePosPrevious;
	public Vector3 _smoothUpdatePosActual;
}*/

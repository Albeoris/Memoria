using Memoria.Prime;
using System;
using UnityEngine;

namespace Memoria
{
    static class SmoothFrameUpdater_Field
    {
        // TODO: add a smooth effect for field SPS (FieldSPSSystem._spsList[].pos etc)
        // [SamsamTS] SPS interpolation doesn't work great for things like rain drops in Brumecia
        // SPSs have low frame rate to begin with, they'll look low frame rate regardless

        // Max (squared) distance per frame to be considered as a smooth movement for field actors
        private const Single ActorSmoothMovementMaxSqr = 400f * 400f; // Iifa tree leaf spiral moves at ~350
                                                                      // Max degree turn per frame to be considered as a smooth movement for field actors
        private const Single ActorSmoothTurnMaxDeg = 45f;
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
                }
            }
        }

        public static Boolean Enabled => Configuration.Graphics.FieldFPS < 0 || Configuration.Graphics.FieldTPS < Configuration.Graphics.FieldFPS;

        public static void RegisterState()
        {
            if (!Enabled) return;
            FieldMap fieldmap = PersistenSingleton<EventEngine>.Instance.fieldmap;
            EventEngine eEngine = PersistenSingleton<EventEngine>.Instance;
            // Actors
            for (ObjList objList = eEngine?.GetActiveObjList(); objList != null; objList = objList.next)
            {
                FieldMapActorController actor = (objList.obj as Actor)?.fieldMapActorController;
                if (actor?.originalActor?.go == null || !eEngine.objIsVisible(objList.obj) || objList.obj.cid != 4)
                    continue;

                GameObject go = actor.originalActor.go;
                String curAnim = FF9DBAll.AnimationDB.GetValue(actor.originalActor.anim);
                Animation anim = actor.gameObject.GetComponent<Animation>();
                if (anim == null)
                    continue;

                AnimationState animState = anim[curAnim];
                Transform shadow = (objList.obj as Actor).fieldMapActor?.shadowTran;

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
                    actor._smoothUpdateRotPrevious = go.transform.rotation;

                    if (shadow != null)
                        actor._smoothUpdateShadowPrevious = shadow.position;

                    actor._smoothUpdateAnimNamePrevious = curAnim;
                    actor._smoothUpdateAnimTimePrevious = animState.time;
                }
                actor._smoothUpdatePosActual = go.transform.position;
                actor._smoothUpdateRotActual = go.transform.rotation;

                if (shadow != null)
                    actor._smoothUpdateShadowActual = shadow.position;

                actor._smoothUpdateAnimNameActual = curAnim;
                actor._smoothUpdateAnimTimeActual = animState.time;

                if (actor._smoothUpdateRegistered && actor._smoothUpdateAnimNamePrevious == actor._smoothUpdateAnimNameActual)
                {
                    Single speed = animState.time - actor._smoothUpdateAnimTimePrevious;
                    if (Mathf.Abs(speed) < animState.length / 2f)
                        actor._smoothUpdateAnimSpeed = speed;
                }
                else
                {
                    actor._smoothUpdateAnimTimePrevious = actor._smoothUpdateAnimTimeActual;
                    actor._smoothUpdateAnimSpeed = 0f;
                }

                actor._smoothUpdateRegistered = true;
            }
            // Layers
            if (fieldmap?.scene?.overlayList != null)
            {
                foreach (BGOVERLAY_DEF bgLayer in fieldmap.scene.overlayList)
                {
                    if (bgLayer.transform != null && ((bgLayer.flags & (BGOVERLAY_DEF.OVERLAY_FLAG.Loop | BGOVERLAY_DEF.OVERLAY_FLAG.ScrollWithOffset)) == 0 || FF9StateSystem.Common.FF9.fldMapNo == 2953))
                    {
                        if (bgLayer._smoothUpdateRegistered)
                            bgLayer._smoothUpdatePosPrevious = bgLayer._smoothUpdatePosActual;
                        else
                            bgLayer._smoothUpdatePosPrevious = bgLayer.transform.position;
                        bgLayer._smoothUpdatePosActual = bgLayer.transform.position;
                        bgLayer._smoothUpdateRegistered = true;
                    }
                }
            }
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
            if (!Enabled || _skipCount > 0 || smoothFactor > 1f)
                return;
            SFXData.LoadLoop();
            FieldMap fieldmap = PersistenSingleton<EventEngine>.Instance.fieldmap;
            EventEngine eEngine = PersistenSingleton<EventEngine>.Instance;
            for (ObjList objList = eEngine?.GetActiveObjList(); objList != null; objList = objList.next)
            {
                FieldMapActorController actor = (objList.obj as Actor)?.fieldMapActorController;
                if (actor?.originalActor?.go == null || !eEngine.objIsVisible(objList.obj) || objList.obj.cid != 4 ||
                    !actor._smoothUpdateRegistered || actor._smoothUpdateAnimNamePrevious != actor._smoothUpdateAnimNameActual)
                    continue;

                GameObject go = actor.originalActor.go;
                Animation anim = go.GetComponent<Animation>();
                if (anim == null)
                    continue;

                AnimationState animState = anim[actor._smoothUpdateAnimNameActual];

                Vector3 frameMove = actor._smoothUpdatePosActual - actor._smoothUpdatePosPrevious;
                if (frameMove.sqrMagnitude > 0f && frameMove.sqrMagnitude < ActorSmoothMovementMaxSqr)
                {
                    go.transform.position = Vector3.Lerp(actor._smoothUpdatePosPrevious, actor._smoothUpdatePosActual, smoothFactor);
                    (objList.obj as Actor).fieldMapActor.shadowTran.position = Vector3.Lerp(actor._smoothUpdateShadowPrevious, actor._smoothUpdateShadowActual, smoothFactor);
                }
                //if (frameMove.sqrMagnitude >= ActorSmoothMovementMaxSqr) Log.Message($"[DEBUG] {Time.frameCount} {actor.name}_{actor.GetInstanceID()} {frameMove.sqrMagnitude} cur {go.transform.position} prev {actor._smoothUpdatePosPrevious} {actor._smoothUpdatePosActual} t {smoothFactor}");

                if (Quaternion.Angle(actor._smoothUpdateRotPrevious, actor._smoothUpdateRotActual) < ActorSmoothTurnMaxDeg)
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
                // if (actor.isPlayer) Log.Message($"[DEBUG] {Time.frameCount} {actor.name} framerate {animState?.clip?.frameRate} actualName {actor._smoothUpdateAnimNameActual} speed {actor._smoothUpdateAnimSpeed} {animState.enabled} animTime {animState.time} animLength {animState.length} t {smoothFactor} prev {actor._smoothUpdateAnimTimePrevious} actual {actor._smoothUpdateAnimTimePrevious + actor._smoothUpdateAnimSpeed}");
            }
            foreach (FF9FieldCharState charState in FF9StateSystem.Field.FF9Field.loc.map.charStateArray.Values)
                fldchar.updateMirrorPosAndAnim(charState.mirror);
            if (fieldmap?.scene?.overlayList != null)
            {
                foreach (BGOVERLAY_DEF bgLayer in fieldmap.scene.overlayList)
                {
                    if (bgLayer.transform != null && bgLayer._smoothUpdateRegistered && ((bgLayer.flags & (BGOVERLAY_DEF.OVERLAY_FLAG.Loop | BGOVERLAY_DEF.OVERLAY_FLAG.ScrollWithOffset)) == 0 || FF9StateSystem.Common.FF9.fldMapNo == 2953))
                    {
                        //Vector3 frameMove = bgLayer._smoothUpdatePosActual - bgLayer._smoothUpdatePosPrevious;
                        //if (frameMove.sqrMagnitude > 0f && frameMove.sqrMagnitude < OverlaySmoothMovementMaxSqr)
                        bgLayer.transform.position = Vector3.Lerp(bgLayer._smoothUpdatePosPrevious, bgLayer._smoothUpdatePosActual, smoothFactor);
                        //Log.Message($"[DEBUG] {Time.frameCount} {bgLayer.transform.name} mag {(bgLayer._smoothUpdatePosActual-bgLayer._smoothUpdatePosPrevious).magnitude} cur {bgLayer.transform.position} prev {bgLayer._smoothUpdatePosPrevious} {bgLayer._smoothUpdatePosActual} t {smoothFactor}");
                    }
                }
            }
            if (_cameraRegistered && !_cameraReverseMove)
            {
                Camera mainCamera = fieldmap?.GetMainCamera();
                if (mainCamera != null && (_cameraPosActual - _cameraPosPrevious).sqrMagnitude < CameraSmoothMovementMaxSqr)
                    mainCamera.transform.position = Vector3.Lerp(_cameraPosPrevious, _cameraPosActual, smoothFactor);
            }
        }

        public static void ResetState()
        {
            if (!Enabled) return;
            if (_skipCount > 0)
                _skipCount--;
            FieldMap fieldmap = PersistenSingleton<EventEngine>.Instance.fieldmap;
            EventEngine eEngine = PersistenSingleton<EventEngine>.Instance;
            for (ObjList objList = eEngine?.GetActiveObjList(); objList != null; objList = objList.next)
            {
                FieldMapActorController actor = (objList.obj as Actor)?.fieldMapActorController;
                GameObject go = actor?.originalActor?.go;
                if (go == null || !actor._smoothUpdateRegistered)
                    continue;

                go.transform.position = actor._smoothUpdatePosActual;
                go.transform.rotation = actor._smoothUpdateRotActual;

                Transform shadow = (objList.obj as Actor).fieldMapActor?.shadowTran;
                if (shadow != null)
                    shadow.position = actor._smoothUpdateShadowActual;

                AnimationState anim = go.GetComponent<Animation>()[actor._smoothUpdateAnimNameActual];
                if (anim != null)
                    anim.time = actor._smoothUpdateAnimTimeActual;
            }
            if (fieldmap?.scene?.overlayList != null)
                foreach (BGOVERLAY_DEF bgLayer in fieldmap.scene.overlayList)
                    if (bgLayer.transform != null && bgLayer._smoothUpdateRegistered && ((bgLayer.flags & (BGOVERLAY_DEF.OVERLAY_FLAG.Loop | BGOVERLAY_DEF.OVERLAY_FLAG.ScrollWithOffset)) == 0 || FF9StateSystem.Common.FF9.fldMapNo == 2953))
                        bgLayer.transform.position = bgLayer._smoothUpdatePosActual;
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
    public Quaternion _smoothUpdateRotPrevious;
    public Quaternion _smoothUpdateRotActual;
    public Vector3 _smoothUpdateShadowPrevious;
    public Vector3 _smoothUpdateShadowActual;
    public String _smoothUpdateAnimNamePrevious;
    public String _smoothUpdateAnimNameActual;
    public Single _smoothUpdateAnimTimePrevious;
    public Single _smoothUpdateAnimTimeActual;
    public Single _smoothUpdateAnimSpeed;
}
partial class BGOVERLAY_DEF
{
    public Boolean _smoothUpdateRegistered = false;
    public Vector3 _smoothUpdatePosPrevious;
    public Vector3 _smoothUpdatePosActual;
}

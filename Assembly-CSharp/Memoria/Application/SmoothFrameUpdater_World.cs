using System;
using UnityEngine;

namespace Memoria
{
    static class SmoothFrameUpdater_World
    {
        private const Single ActorSmoothMovementMaxSqr = 100f;

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
                            }
                        }
                    }
                    if (WMWorld.Instance?.Shadows != null)
                    {
                        foreach (WMShadow shadow in WMWorld.Instance.Shadows)
                        {
                            if (shadow == null)
                                continue;
                            shadow._smoothUpdateRegistered = false;
                        }
                    }
                }
            }
        }

        public static Boolean Enabled => Configuration.Graphics.WorldFPS < 0 || Configuration.Graphics.WorldTPS < Configuration.Graphics.WorldFPS;

        public static void RegisterState()
        {
            if (!Enabled) return;
            for (ObjList objList = ff9.GetActiveObjList(); objList != null; objList = objList.next)
            {
                WMActor actor = (objList.obj as Actor)?.wmActor;
                if (actor == null || objList.obj.cid != 4 || actor.Animation == null)
                    continue;

                String curAnim = FF9DBAll.AnimationDB.GetValue(actor.originalActor.anim);
                Animation anim = actor.Animation;
                AnimationState animState = anim[curAnim];

                if (actor._smoothUpdateRegistered)
                {
                    actor._smoothUpdatePosPrevious = actor._smoothUpdatePosActual + ff9.world.BlockShift;
                    actor._smoothUpdateRotPrevious = actor._smoothUpdateRotActual;

                    actor._smoothUpdateAnimNamePrevious = actor._smoothUpdateAnimNameActual;
                    actor._smoothUpdateAnimTimePrevious = actor._smoothUpdateAnimTimeActual;
                }
                else
                {
                    actor._smoothUpdatePosPrevious = actor.transform.position;
                    actor._smoothUpdateRotPrevious = actor.transform.rotation;

                    actor._smoothUpdateAnimNamePrevious = curAnim;
                    actor._smoothUpdateAnimTimePrevious = animState.time;
                }
                actor._smoothUpdatePosActual = actor.transform.position;
                actor._smoothUpdateRotActual = actor.transform.rotation;

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
            foreach (WMShadow shadow in WMWorld.Instance.Shadows)
            {
                if (shadow == null || !shadow.enabled)
                    continue;
                if (shadow._smoothUpdateRegistered)
                    shadow._smoothUpdatePosPrevious = shadow._smoothUpdatePosActual + ff9.world.BlockShift;
                else
                    shadow._smoothUpdatePosPrevious = shadow.transform.position;
                shadow._smoothUpdatePosActual = shadow.transform.position;
                shadow._smoothUpdateRegistered = true;
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
            if (!Enabled || _skipCount > 0)
                return;
            SFXData.LoadLoop();
            for (ObjList objList = ff9.GetActiveObjList(); objList != null; objList = objList.next)
            {
                WMActor actor = (objList.obj as Actor)?.wmActor;
                if (actor == null || objList.obj.cid != 4 || actor.Animation == null || !actor._smoothUpdateRegistered || actor._smoothUpdateAnimNamePrevious != actor._smoothUpdateAnimNameActual)
                    continue;

                Animation anim = actor.Animation;
                AnimationState animState = anim[actor._smoothUpdateAnimNameActual];

                Vector3 frameMove = actor._smoothUpdatePosActual - actor._smoothUpdatePosPrevious;
                if (frameMove.sqrMagnitude > 0f && frameMove.sqrMagnitude < ActorSmoothMovementMaxSqr)
                    actor.transform.position = Vector3.Lerp(actor._smoothUpdatePosPrevious, actor._smoothUpdatePosActual, smoothFactor);

                //if (Quaternion.Angle(actor._smoothUpdateRotPrevious, actor._smoothUpdateRotActual) < ActorSmoothTurnMaxDeg)
                actor.transform.rotation = Quaternion.Lerp(actor._smoothUpdateRotPrevious, actor._smoothUpdateRotActual, smoothFactor);

                if (anim != null)
                {
                    animState.time = Mathf.Lerp(actor._smoothUpdateAnimTimePrevious, actor._smoothUpdateAnimTimePrevious + actor._smoothUpdateAnimSpeed, smoothFactor);
                    if (animState.time > animState.length)
                        animState.time -= animState.length;
                    else if (animState.time < 0f)
                        animState.time += animState.length;
                    anim.Sample();
                }
                //if (actor.name == "obj12_WM") Log.Message($"[DEBUG] {Time.frameCount} {actor.name} framerate {animState?.clip?.frameRate} actualName {actor._smoothUpdateAnimNameActual} speed {actor._smoothUpdateAnimSpeed} {animState.enabled} animTime {animState.time} animLength {animState.length} t {smoothFactor} prev {actor._smoothUpdateAnimTimePrevious} actual {actor._smoothUpdateAnimTimePrevious + actor._smoothUpdateAnimSpeed}");
            }
            if (WMWorld.Instance.Shadows != null)
            {
                foreach (WMShadow shadow in WMWorld.Instance.Shadows)
                {
                    if (shadow == null || !shadow._smoothUpdateRegistered)
                        continue;
                    shadow.transform.position = Vector3.Lerp(shadow._smoothUpdatePosPrevious, shadow._smoothUpdatePosActual, smoothFactor);
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
            if (!Enabled) return;
            if (_skipCount > 0)
                _skipCount--;
            for (ObjList objList = ff9.GetActiveObjList(); objList != null; objList = objList.next)
            {
                Obj obj = objList.obj;
                if (obj.cid == 4)
                {
                    WMActor wmActor = (obj as Actor)?.wmActor;
                    if (wmActor == null || !wmActor._smoothUpdateRegistered)
                        continue;
                    wmActor.transform.position = wmActor._smoothUpdatePosActual;
                    wmActor.transform.rotation = wmActor._smoothUpdateRotActual;

                    AnimationState animState = wmActor.originalActor.go.gameObject.GetComponent<Animation>()[wmActor._smoothUpdateAnimNameActual];
                    if (animState != null)
                    {
                        animState.time = wmActor._smoothUpdateAnimTimeActual;

                        if (animState.time > animState.length)
                            animState.time -= animState.length;
                        else if (animState.time < 0f)
                            animState.time += animState.length;
                    }
                }
            }
            if (WMWorld.Instance?.Shadows != null)
            {
                foreach (WMShadow shadow in WMWorld.Instance.Shadows)
                {
                    if (shadow == null || !shadow._smoothUpdateRegistered)
                        continue;
                    shadow.transform.position = shadow._smoothUpdatePosActual;
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
    public Quaternion _smoothUpdateRotPrevious;
    public Quaternion _smoothUpdateRotActual;
    public String _smoothUpdateAnimNamePrevious;
    public String _smoothUpdateAnimNameActual;
    public Single _smoothUpdateAnimTimePrevious;
    public Single _smoothUpdateAnimTimeActual;
    public Single _smoothUpdateAnimSpeed;
}
partial class WMShadow
{
    public Boolean _smoothUpdateRegistered = false;
    public Vector3 _smoothUpdatePosPrevious;
    public Vector3 _smoothUpdatePosActual;
}

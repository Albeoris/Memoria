using FF9;
using System;
using UnityEngine;

namespace Memoria
{
    static class SmoothFrameUpdater_Battle
    {
        // Max (squared) distance per frame to be considered as a smooth movement for battle units
        private const Single ActorSmoothMovementMaxSqr = 2000f * 2000f;
        // Max degree turn per frame to be considered as a smooth movement for field actors
        private const Single ActorSmoothTurnMaxDeg = 45f;
        // Max (squared) distance per frame to be considered as a smooth movement for the camera
        private const Single CameraSmoothMovementMaxSqr = 1000f * 1000f;
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
                        next._smoothUpdateRegistered = false;
                }
            }
        }

        public static Boolean Enabled => Configuration.Graphics.BattleFPS < 0 || Configuration.Graphics.BattleTPS < Configuration.Graphics.BattleFPS;

        public static void OnBattleMapChange()
        {
            _bgInitialised = false;
            _bg = null;
            _cameraRegistered = false;
        }

        public static void RegisterState()
        {
            if (!Enabled) return;
            for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
            {
                if (next.bi.slave != 0 || next.gameObject == null || !next.gameObject.activeInHierarchy)
                    continue;

                String curAnim = next.currentAnimationName;
                Animation anim = next.gameObject.GetComponent<Animation>();
                AnimationState animState = anim?[curAnim];
                if (animState == null)
                    continue;
                next._smoothUpdateBoneDelta = Vector3.zero;

                foreach (AnimationState state in anim)
                {
                    if (state != null && state.enabled)
                    {
                        curAnim = state.name;
                        animState = state;
                        break;
                    }
                }

                if (next._smoothUpdateRegistered && anim[next._smoothUpdateAnimNamePrevious] != null)
                {
                    next._smoothUpdatePosPrevious = next._smoothUpdatePosActual;
                    next._smoothUpdateRotPrevious = next._smoothUpdateRotActual;
                    next._smoothUpdateScalePrevious = next._smoothUpdateScaleActual;

                    next._smoothUpdateAnimNamePrevious = next._smoothUpdateAnimNameActual;
                    next._smoothUpdateAnimTimePrevious = next._smoothUpdateAnimTimeActual;

                    if (next._smoothUpdateAnimNamePrevious == curAnim)
                    {
                        if (next._smoothUpdateAnimNameNext == null)
                        {
                            Single speed = animState.time - next._smoothUpdateAnimTimePrevious;
                            if (Mathf.Abs(speed) < animState.length / 2f)
                                next._smoothUpdateAnimSpeed = speed;
                        }
                        else
                        {
                            next._smoothUpdateAnimSpeed = animState.time - next._smoothUpdateAnimTimePrevious;
                            anim.Play(next._smoothUpdateAnimNameNext);
                            next._smoothUpdateAnimNameNext = null;
                        }
                    }
                    else if (!anim.IsPlaying(next._smoothUpdateAnimNamePrevious))
                    {
                        Vector3 nextBonePos = next.gameObject.transform.GetChildByName("bone000").localToWorldMatrix.GetColumn(3);
                        Single time = animState.time;
                        anim.Play(next._smoothUpdateAnimNamePrevious);
                        animState.time = time; // Reset to 0 by previous line which we don't want
                        AnimationState prevState = anim[next._smoothUpdateAnimNamePrevious];
                        prevState.time = next._smoothUpdateAnimTimePrevious + next._smoothUpdateAnimSpeed;
                        anim.Sample();
                        Vector3 curBonePos = next.gameObject.transform.GetChildByName("bone000").localToWorldMatrix.GetColumn(3);

                        next._smoothUpdateBoneDelta = nextBonePos - curBonePos;
                        next._smoothUpdateAnimNameNext = next.currentAnimationName;
                    }
                }
                else
                {
                    next._smoothUpdatePosPrevious = next.gameObject.transform.position;
                    next._smoothUpdateRotPrevious = next.gameObject.transform.rotation;
                    next._smoothUpdateScalePrevious = next.gameObject.transform.localScale;

                    next._smoothUpdateAnimNamePrevious = curAnim;
                    next._smoothUpdateAnimTimePrevious = animState.time;
                    next._smoothUpdateAnimSpeed = 0f;
                }
                next._smoothUpdatePosActual = next.gameObject.transform.position;
                next._smoothUpdateRotActual = next.gameObject.transform.rotation;
                next._smoothUpdateScaleActual = next.gameObject.transform.localScale;

                next._smoothUpdateAnimNameActual = curAnim;
                next._smoothUpdateAnimTimeActual = animState.time;

                next._smoothUpdateRegistered = true;
                geo.geoScaleUpdate(next, true);
            }
            // Sky
            if (_bg == null && FF9StateSystem.Battle.FF9Battle.map.btlBGPtr != null && !_bgInitialised)
            {
                if (battlebg.nf_BbgSkyRotation != 0 && battlebg.nf_BbgNumber != 7) // Fix #672: the sky against Nova Dragon has a sky rotation + a "vertical" texture animation
                {
                    foreach (Transform transform in FF9StateSystem.Battle.FF9Battle.map.btlBGPtr.gameObject.transform)
                    {
                        if (battlebg.getBbgAttr(transform.name) == battlebg.BBG_ATTR_SKY)
                        {
                            _bg = transform;
                            _bgRotPrevious = _bg.localRotation;
                            _bgRotActual = _bg.localRotation;
                            break;
                        }
                    }
                }
                _bgInitialised = true;
            }
            if (_bg != null)
            {
                _bgRotPrevious = _bgRotActual;
                _bgRotActual = _bg.localRotation;
            }
            // TODO: Smoothen some animated background objects (battlebg.getBbgObjAnimation) - NOTE: thunders in the Nova Dragon battle should most likely not be smoothened anyway (but things like the Crystal against Trance Kuja should)
            // Camera
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
            Apply(0f);
        }

        public static void Apply(Single smoothFactor)
        {
            if (!Enabled || _skipCount > 0)
                return;
            SFXData.LoadLoop();
            for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
            {
                if (next.gameObject == null || !next._smoothUpdateRegistered)
                    continue;

                //var pos = next.gameObject.transform.position;
                Vector3 frameMove = next._smoothUpdatePosActual + next._smoothUpdateBoneDelta - next._smoothUpdatePosPrevious;
                if (frameMove.sqrMagnitude > 0f && frameMove.sqrMagnitude < ActorSmoothMovementMaxSqr)
                    next.gameObject.transform.position = Vector3.Lerp(next._smoothUpdatePosPrevious, next._smoothUpdatePosActual + next._smoothUpdateBoneDelta, smoothFactor);

                //var mag = (next.gameObject.transform.position - pos).magnitude;
                //var ang = Vector3.Angle(pos, next.gameObject.transform.position);
                //if (next.btl_id == 1) Log.Message($"[DEBUG {Time.frameCount} magnitude {mag} boneDelta {next._smoothUpdateBoneDelta} bone {next.gameObject.transform.GetChildByName("bone000").localToWorldMatrix.GetColumn(3)} pos {next.gameObject.transform.position} prev {next._smoothUpdatePosPrevious} actual {next._smoothUpdatePosActual + next._smoothUpdateBoneDelta} t {smoothFactor}");

                if (Quaternion.Angle(next._smoothUpdateRotPrevious, next._smoothUpdateRotActual) < ActorSmoothTurnMaxDeg)
                    next.gameObject.transform.rotation = Quaternion.Lerp(next._smoothUpdateRotPrevious, next._smoothUpdateRotActual, smoothFactor);

                next.gameObject.transform.localScale = Vector3.Lerp(next._smoothUpdateScalePrevious, next._smoothUpdateScaleActual, smoothFactor);

                Animation anim = next.gameObject.GetComponent<Animation>();
                AnimationState animState = anim[next._smoothUpdateAnimNamePrevious];
                if (animState != null && next.bi.stop_anim == 0)
                {
                    animState.time = Mathf.Lerp(next._smoothUpdateAnimTimePrevious, next._smoothUpdateAnimTimePrevious + next._smoothUpdateAnimSpeed, smoothFactor);

                    if (animState.time > animState.length)
                        animState.time -= animState.length;
                    else if (animState.time < 0f)
                        animState.time += animState.length;

                    anim.Sample();
                    // if (next.btl_id == 1) Log.Message($"[DEBUG {Time.frameCount} curName {next.currentAnimationName} actualName {next._smoothUpdateAnimNameActual} prevName {next._smoothUpdateAnimNamePrevious} nextName {next._smoothUpdateAnimNameNext} speed {next._smoothUpdateAnimSpeed} {animState.enabled} animTime {animState.time} animLength {animState.length} t {smoothFactor} prev {next._smoothUpdateAnimTimePrevious} actual {next._smoothUpdateAnimTimeActual}");
                }
            }
            HonoluluBattleMain.UpdateAttachModel();
            // Sky
            if (_bg != null)
            {
                _bg.localRotation = Quaternion.Lerp(_bgRotPrevious, _bgRotActual, smoothFactor);
                // Log.Message($"[DEBUG {Time.frameCount} cur {_bg.localRotation.eulerAngles} prev {_bgRotPrevious.eulerAngles} actual {_bgRotActual.eulerAngles} t {smoothFactor}");
            }
            // Vertical texture animation of the background
            if (battlebg.nf_BbgNumber != 7) // Fix #672: the sky against Nova Dragon has a sky rotation + a "vertical" texture animation
                battlebg.geoBGTexAnimSmoothen(smoothFactor);
            // SPS
            if (FF9StateSystem.Battle.FF9Battle.btl_phase != FF9StateBattleSystem.PHASE_ENTER && FF9StateSystem.Battle.FF9Battle.btl_phase != FF9StateBattleSystem.PHASE_INIT_SYSTEM)
            {
                btl2d.StatusUpdateVisuals(smoothFactor);
                HonoluluBattleMain.battleSPS.GenerateSPS();
            }
            // Camera
            Camera camera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
            if (_cameraRegistered && camera != null)
            {
                Vector3 cameraMove = MatrixGetTranslation(_cameraW2CMatrixActual) - MatrixGetTranslation(_cameraW2CMatrixPrevious);
                if (cameraMove.sqrMagnitude >= CameraSmoothMovementMaxSqr)
                    return;
                //Single cameraAngle = Quaternion.Angle(MatrixGetRotation(_cameraW2CMatrixActual), MatrixGetRotation(_cameraW2CMatrixPrevious));
                //if (cameraAngle >= CameraSmoothTurnMaxDeg)
                //	return;
                camera.worldToCameraMatrix = MatrixLerpUnclamped(_cameraW2CMatrixPrevious, _cameraW2CMatrixActual, smoothFactor);
                camera.projectionMatrix = MatrixLerpUnclamped(_cameraProjMatrixPrevious, _cameraProjMatrixActual, smoothFactor);
            }
        }

        public static void ResetState()
        {
            if (!Enabled) return;
            if (_skipCount > 0)
                _skipCount--;
            for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
            {
                if (next.gameObject == null || !next._smoothUpdateRegistered)
                    continue;

                next.gameObject.transform.position = next._smoothUpdatePosActual;
                next.gameObject.transform.rotation = next._smoothUpdateRotActual;
                next.gameObject.transform.localScale = next._smoothUpdateScaleActual;

                AnimationState animState = next.gameObject.GetComponent<Animation>()[next._smoothUpdateAnimNameActual];
                if (animState != null)
                {
                    animState.time = next._smoothUpdateAnimTimeActual;

                    if (animState.time > animState.length)
                        animState.time -= animState.length;
                    else if (animState.time < 0f)
                        animState.time += animState.length;
                }
            }
            if (_bg != null)
            {
                _bg.localRotation = _bgRotActual;
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

        private static Boolean _bgInitialised;
        private static Transform _bg;
        private static Quaternion _bgRotPrevious;
        private static Quaternion _bgRotActual;
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
    public String _smoothUpdateAnimNamePrevious;
    public String _smoothUpdateAnimNameActual;
    public String _smoothUpdateAnimNameNext;
    public Single _smoothUpdateAnimTimePrevious;
    public Single _smoothUpdateAnimTimeActual;
    public Single _smoothUpdateAnimSpeed;
    public Vector3 _smoothUpdateBoneDelta;
    public bool _hasMeshSmoothed;
}

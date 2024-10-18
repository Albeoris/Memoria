using Assets.Scripts.Common;
using FF9;
using Memoria;
using Memoria.Prime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Memoria.Configuration;
using Object = System.Object;

public partial class FieldMapActorController : HonoBehavior
{
    public Boolean IsActive()
    {
        return this.isActive;
    }

    public Int32 MoveFrame
    {
        get => this.moveFrame;
        set => this.moveFrame = value;
    }

    public void SetActive(Boolean active)
    {
        this.isActive = active;
        if (this.isPlayer)
            PersistenSingleton<HonoInputManager>.Instance.SetVirtualAnalogEnable(active);
    }

    public Boolean SetPosition(Vector3 pos, Boolean updateLastPos, Boolean needCheckTri = true)
    {
        if (FF9StateSystem.Common.FF9.fldMapNo == 70) // Opening-For FMV
            return true;
        if ((this.charFlags & 1) == 0 || !needCheckTri)
        {
            this.curPos = pos;
            if (updateLastPos)
                this.lastPos = this.curPos;
            this.SyncPosToTransform();
            return true;
        }
        Int32 triIdxAtPos = this.GetTriIdxAtPos(pos);
        if (triIdxAtPos != -1)
        {
            WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[triIdxAtPos];
            Vector3 barycentricCoef = Math3D.CalculateBarycentricRatioXZ(pos, walkMeshTriangle.originalVertices[0], walkMeshTriangle.originalVertices[1], walkMeshTriangle.originalVertices[2]);
            Vector3 posOnTriangle = walkMeshTriangle.originalVertices[0] * barycentricCoef.x + walkMeshTriangle.originalVertices[1] * barycentricCoef.y + walkMeshTriangle.originalVertices[2] * barycentricCoef.z;
            this.curPos = posOnTriangle;
            if (updateLastPos)
                this.lastPos = this.curPos;
            this.activeFloor = walkMeshTriangle.floorIdx;
            this.activeTri = walkMeshTriangle.triIdx;
            this.lastFloor = this.activeFloor;
            this.lastTri = this.activeTri;
            this.SyncPosToTransform();
            return true;
        }
        this.SetDefaultCharPos();
        return false;
    }

    public void SetRotation(Vector3 rot)
    {
        this.actor.transform.localRotation = Quaternion.Euler(rot);
    }

    public void SetDefaultCharPos()
    {
        WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[0];
        this.curPos = walkMeshTriangle.originalCenter;
        this.lastPos = this.curPos;
        this.activeFloor = walkMeshTriangle.floorIdx;
        this.activeTri = walkMeshTriangle.triIdx;
        this.lastFloor = this.activeFloor;
        this.lastTri = this.activeTri;
        this.SyncPosToTransform();
    }

    public void SetMoveTarget(Vector3 targetPos)
    {
        this.moveTarget = targetPos;
        this.hasTarget = true;
    }

    public void ClearMoveTarget()
    {
        this.moveTarget = Vector3.zero;
        this.hasTarget = false;
    }

    public void SyncPosToTransform()
    {
        Vector3 localPosition = this.curPos;
        this.actor.transform.localPosition = localPosition;
    }

    public override void HonoAwake()
    {
        this.walkMesh = (WalkMesh)null;
        this.actor = (FieldMapActor)null;
        this.activeTri = -1;
        this.activeFloor = -1;
        this.lastTri = -1;
        this.lastFloor = -1;
        this.speed = 30f;
        this.radius = 96f;
        this.isPlayer = false;
        this.charFlags = 1;
        this.curPos = new Vector3(25f, 0f, -1100f);
        this.lastPos = new Vector3(25f, 0f, -1100f);
        this.moveVec = Vector3.zero;
        this.forces = new List<Vector3>();
        this.forcesFlag = new List<Boolean>();
        this.forcesOrigin = new List<Vector3>();
        this.forcesType = new List<Boolean>();
        this.visited = new List<Int32>();
        this.isMoving = false;
        this.isRunning = false;
        this.cTime = 0f;
        this.model = base.transform;
        this.moveTarget = Vector3.zero;
        this.hasTarget = false;
        this.movePaths = new List<Vector3>();
        this.debugPaths = new List<Vector3>();
        this.debugSmoothPaths = new List<Vector3>();
        this.totalPathCount = 0;
        this.totalPathLengthSq = 0f;
        this.cumulativeTime = 0f;
        this.amplitude = Screen.height * 0.025f;
        this.hasTalkBalloon = false;
        this.talkBalloonRect = default(Rect);
        this.warpRects = new List<Rect>();
        this.isActive = true;
        GameObject obj = GameObject.Find("FieldMap Root/VirtualJoystick");
        UnityEngine.Object.Destroy(obj);
        this.animation = this.model.GetComponent<Animation>();
        this.foundTris = new List<Int32>();
        this.adjacentActiveTris = new List<Int32>();
        this.analogControlEnabled = Configuration.AnalogControl.Enabled;
        this.stickThreshold = Configuration.AnalogControl.StickThreshold / 100.0f;
        this.minimumSpeed = Mathf.Min(Configuration.AnalogControl.MinimumSpeed, 30.0f);
    }

    public override void HonoStart()
    {
        this.LoadResources();
    }

    private IEnumerator DelayedDialog(Vector3 tPos)
    {
        yield return new WaitForEndOfFrame();
        yield break;
    }

    public override void HonoUpdate()
    {
        if (!FF9StateSystem.Field.isDebug)
            this.PlayAnimationViaEventScript();
        if (!this.isActive)
        {
            this.SyncPosToTransform();
            if (this.isPlayer && this.GetLadderFlag() != 0)
                PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, null);
            this.ClearMoveTarget();
            if (PersistenSingleton<UIManager>.Instance.IsMenuControlEnable || PersistenSingleton<UIManager>.Instance.IsWarningDialogEnable)
                return;
        }
        else if (!EventInput.IsMovementControl && this.isPlayer)
        {
            PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, null);
            return;
        }
        this.cumulativeTime += Time.deltaTime;
        if (this.isPlayer)
        {
            if (!FF9StateSystem.Field.isDebug)
            {
                Int32 ladderFlag = this.GetLadderFlag();
                if (PersistenSingleton<EventEngine>.Instance.GetUserControl() || ladderFlag != 0)
                {
                    if (EventInput.IsMovementControl && VirtualAnalog.GetTap())
                        this.CheckPosInProjectedWalkMesh();
                    if (ladderFlag != 0)
                        PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, null);
                }
            }
            else if (EventInput.IsMovementControl && VirtualAnalog.GetTap())
            {
                this.CheckPosInProjectedWalkMesh();
            }
        }

        this.isRunning = false;
        Single nearRadiusSq = this.radius * this.radius * 9f * 9f;
        Boolean dashInhibited = PersistenSingleton<EventEngine>.Instance.GetDashInh() == 1;
        if (this.isPlayer && ((UIManager.Input.GetKey(Control.Cancel) ^ FF9StateSystem.Settings.cfg.move == 1UL) || !analogControlEnabled && VirtualAnalog.GetMagnitudeRatio() > 0.95f || this.totalPathLengthSq > nearRadiusSq) && !dashInhibited)
        {
            // Running movement for the PC (60/tick) is done by moving twice at the walking speed (30/tick)
            this.isRunning = true;
            this.UpdateMovement(true);
            // The PC's last position was already copied above; copying it again misleads the engine into thinking the distance travelled is ~twice shorter than it actually was (especially with respect to the encounter rate)
            this.UpdateMovement(!Configuration.Battle.PSXEncounterMethod);
        }
        else
        {
            this.UpdateMovement(true);
        }
        if (this.isPlayer && PersistenSingleton<EventEngine>.Instance.GetUserControl())
            this.originalActor.speed = (Byte)(this.isRunning ? 60 : 30);

        if (FF9StateSystem.Common.FF9.fldMapNo == 1204 && this.originalActor.sid == 11)
        {
            // A. Castle/Underground (platform at bottom), Zidane
            GameObject marker = GameObject.Find("marker1");
            if (marker == null)
            {
                marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                marker.name = "marker1";
                marker.transform.position = new Vector3(-3353f, 0f, 1914f);
                marker.transform.localScale = new Vector3(100f, 1f, 100f);
                this.DebugSetMarker(marker);
            }
            marker = GameObject.Find("marker2");
            if (marker == null)
            {
                marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                marker.name = "marker2";
                marker.transform.position = new Vector3(-2862f, 0f, 2368f);
                marker.transform.localScale = new Vector3(100f, 1f, 100f);
                this.DebugSetMarker(marker);
            }
            marker = GameObject.Find("marker3");
            if (marker == null)
            {
                marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                marker.name = "marker3";
                marker.transform.position = new Vector3(-2001f, 0f, 3080f);
                marker.transform.localScale = new Vector3(100f, 1f, 100f);
                this.DebugSetMarker(marker);
            }
            marker = GameObject.Find("marker4");
            if (marker == null)
            {
                marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                marker.name = "marker4";
                marker.transform.position = new Vector3(-1572f, 0f, 3215f);
                marker.transform.localScale = new Vector3(100f, 1f, 100f);
                this.DebugSetMarker(marker);
            }
            marker = GameObject.Find("marker5");
            if (marker == null)
            {
                marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                marker.name = "marker5";
                marker.transform.position = new Vector3(-1054f, 0f, 3481f);
                marker.transform.localScale = new Vector3(100f, 1f, 100f);
                this.DebugSetMarker(marker);
            }
        }
    }

    public override void HonoLateUpdate()
    {
        this.SyncPosToTransform();
        this.originalActor.pos[0] = this.curPos.x;
        this.originalActor.pos[1] = this.curPos.y;
        this.originalActor.pos[2] = this.curPos.z;
        Vector3 lastMove = this.lastPos - this.curPos;
        Vector3 moveFromStillPos = this.curPos - this.stillPos;
        if (lastMove.sqrMagnitude == 0f || (this.stillCount == 0 && moveFromStillPos.sqrMagnitude < 10f))
        {
            this.stillCount++;
            if (this.stillCount > 30)
            {
                this.movePaths.Clear();
                this.ClearMoveTarget();
                this.totalPathCount = 0;
            }
        }
        else
        {
            this.stillCount = 0;
            this.stillPos = this.curPos;
        }
    }

    public override void HonoOnGUI()
    {
        if (!this.isPlayer)
            return;
        if (!this.isActive)
            return;
        if (PersistenSingleton<SceneDirector>.Instance.IsFading)
            return;
        Camera mainCamera = this.fieldMap.GetMainCamera();
        BGCAM_DEF currentBgCamera = this.fieldMap.GetCurrentBgCamera();
        if (this.totalPathCount > 0)
        {
            Vector3 pathDest = this.movePaths.Count > 0 ? this.movePaths[0] : this.moveTarget;
            Vector3 position = PSX.CalculateGTE_RTPT(pathDest + new Vector3(0f, this.amplitude, 0f), Matrix4x4.identity, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), this.fieldMap.GetProjectionOffset());
            position = mainCamera.WorldToScreenPoint(position);
            position.y = Screen.height - position.y;
            Rect position2 = new Rect(position.x - this.targetMarkSize / 2f, position.y - this.targetMarkSize + Mathf.Sin(this.cumulativeTime * 5f) * this.amplitude, this.targetMarkSize, this.targetMarkSize);
            GUI.DrawTexture(position2, this.targetMark);
        }
    }

    public override void HonoOnStartFastForwardMode()
    {
        base.HonoDefaultStartFastForwardMode();
    }

    public override void HonoOnStopFastForwardMode()
    {
        base.HonoDefaultStopFastForwardMode();
    }

    public void UpdateMovement(Boolean copyLastPos = true)
    {
        this.cTime = 0f;
        this.isMoving = false;
        if (this.isPlayer)
        {
            if (PersistenSingleton<EventEngine>.Instance.GetUserControl()
                || FF9StateSystem.Common.FF9.fldMapNo == 1751 // Iifa Tree/Inner Roots (entrance after elevator)
                || FF9StateSystem.Common.FF9.fldMapNo == 404 // Dali underground Entrance
                || (FF9StateSystem.Common.FF9.fldMapNo == 205 && this.originalActor.sid == 16) //Prima Vista/ Hallway(front of Steiner's cell), Steiner
                || (FF9StateSystem.Common.FF9.fldMapNo == 2150 && this.originalActor.sid == 13) //L. Castle/Royal Chamber, Zidane
                || (FF9StateSystem.Common.FF9.fldMapNo == 900 && this.originalActor.sid == 13)) //Treno/Pub, Dagger
            {
                if (!PersistenSingleton<EventEngine>.Instance.GetUserControl() || copyLastPos)
                    this.CopyLastPos();
            }
            this.MovePC();
            if (FF9StateSystem.Common.FF9.fldMapNo == 916 && this.originalActor.uid == 10 && this.curPos.z > -15900f && this.curPos.z < -15700f && this.curPos.x < 130f)
                this.curPos.x = 130f; // Treno/Dock, Dagger
            if (FF9StateSystem.Common.FF9.fldMapNo == 1005 && this.originalActor.uid == 7)
            {
                // Cleyra/Tree Trunk (room filled with sand), Zidane
                if (this.curPos.z > -1000f && this.curPos.z < -920f && this.curPos.x < -1060f)
                    this.curPos.x = -1060f;
                else if (this.curPos.x > -1400f && this.curPos.x < -1350f && this.curPos.z < -610f)
                    this.curPos.z = -610f;
            }
        }
        else
        {
            if ((FF9StateSystem.Common.FF9.fldMapNo == 2050 && this.originalActor.sid == 5) // Alexandria/Main Street, Mistodon
                || (FF9StateSystem.Common.FF9.fldMapNo == 350 && this.originalActor.sid == 11) // Dali/Village Road, Dali_GirlA
                || (FF9StateSystem.Common.FF9.fldMapNo == 1315 && this.originalActor.sid == 12)) // Lindblum/Town Walls, Lindblum_Soldier
            {
                if (!PersistenSingleton<EventEngine>.Instance.GetUserControl() || copyLastPos)
                    this.CopyLastPos();
            }
            this.MoveNPC();
        }
        Int32 movementFlags = 0;
        if ((this.charFlags & 1) != 0)
            movementFlags = this.ServiceChar();
        this.isMoving = (movementFlags & 1) != 0;
        if (FF9StateSystem.Field.isDebug && VirtualAnalog.GetAnalogValue().magnitude > 0.1f)
            this.isMoving = true;
        if ((this.charFlags & 1) != 0)
            this.UpdateActiveTri();
        if (FF9StateSystem.Field.isDebug)
        {
            if (this.animation.IsPlaying(FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.run)) || this.animation.IsPlaying(FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.walk)) || this.animation.IsPlaying(FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.idle)))
            {
                if (this.isMoving || this.totalPathCount > 0)
                {
                    if (this.isRunning)
                    {
                        String runAnim = FF9DBAll.AnimationDB.GetValue(this.originalActor.run);
                        if (!this.animation.IsPlaying(runAnim) && this.animation.GetClip(runAnim) != null)
                            this.animation.Play(runAnim);
                    }
                    else
                    {
                        String walkAnim = FF9DBAll.AnimationDB.GetValue(this.originalActor.walk);
                        if (!this.animation.IsPlaying(walkAnim) && this.animation.GetClip(walkAnim) != null)
                            this.animation.Play(FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.walk));
                    }
                }
                else
                {
                    String idleAnim = FF9DBAll.AnimationDB.GetValue(this.originalActor.idle);
                    if (!this.animation.IsPlaying(idleAnim) && this.animation.GetClip(idleAnim) != null)
                        this.animation.Play(idleAnim);
                }
            }
            else if (!this.animation.isPlaying)
            {
                String idleAnim = FF9DBAll.AnimationDB.GetValue(this.originalActor.idle);
                if (this.animation.GetClip(idleAnim) != null)
                    this.animation.Play(idleAnim);
            }
        }
        this.lastFloor = this.activeFloor;
        this.lastTri = this.activeTri;
        this.SyncPosToTransform();
    }

    private void PlayAnimationViaEventScript()
    {
        String curAnim = FF9DBAll.AnimationDB.GetValue(this.originalActor.anim);
        if (!this.animation.IsPlaying(curAnim))
        {
            AnimationClip clip = this.animation.GetClip(curAnim);
            if (clip != null)
            {
                if (FF9StateSystem.Common.FF9.fldMapNo == 3010 && this.originalActor.sid == 8)
                    return; // Ending/TH, Sword
                this.animation.clip = clip;
                this.animation.Play(curAnim);
                this.animation[curAnim].speed = 0f;
                this.animation[curAnim].time = (Single)this.originalActor.animFrame / (Single)this.originalActor.frameN * this.animation[curAnim].length;
                this.animation.Sample();
                if (this.originalActor.frameN == 1 && base.IsVisibled())
                    this.animation.Stop();
            }
        }
        else
        {
            Single time = (Single)this.originalActor.animFrame / (Single)this.originalActor.frameN * this.animation[curAnim].length;
            this.animation[curAnim].speed = 0f;
            this.animation[curAnim].time = time;
            this.animation.Sample();
        }
    }

    private void LateUpdate()
    {
        if (this.actor != null && this.actor.actor != null)
        {
            if ((this.actor.actor.geo_struct_flags & geo.GEO_FLAGS_LOOK) != 0)
            {
                this.UpdateNeck();
            }
            else if (FF9StateSystem.Common.FF9.fldMapNo == 576 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 3140 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 100 && this.originalActor.model == 270)
            {
                // Lindblum/Festival, Cid Human (Royal Action Figures)
                Transform childByName = base.transform.GetChildByName("bone" + this.actor.actor.neckBoneIndex.ToString("D3"));
                this.ApplyNeckRotation(childByName, 0f, -149f);
            }
        }
        if (this.originalActor.parent != null && ((FF9StateSystem.Common.FF9.fldMapNo == 2950 && this.originalActor.sid == 25) || (FF9StateSystem.Common.FF9.fldMapNo == 2951 && this.originalActor.sid == 29) || (FF9StateSystem.Common.FF9.fldMapNo == 2952 && this.originalActor.sid == 23) || (FF9StateSystem.Common.FF9.fldMapNo == 2954 && this.originalActor.sid == 26) || (FF9StateSystem.Common.FF9.fldMapNo == 2955 && this.originalActor.sid == 28)))
        {
            // Chocobo's Forest or Chocobo's Lagoon or Chocobo's Air Garden or Chocobo's Paradise (both), Zidane
            this.PretendChocoboOffset();
        }
        this.CheckOffsetPosModel();
    }

    private void PretendChocoboOffset()
    {
        this.SetPosition(this.originalActor.parent.fieldMapActorController.transform.position, true, false);
        Transform chocoOrigin = this.originalActor.parent.go.transform.FindChild("bone000");
        Transform riderOrigin = this.originalActor.go.transform.FindChild("bone000");
        riderOrigin.position = chocoOrigin.position + new Vector3(0f, 105f, 0f);
    }

    private void CheckOffsetPosModel()
    {
        if (this.originalActor.model == 200) // Air Cab
            this.originalActor.go.transform.FindChild("bone000").Translate(Vector3.forward * 650f);
        if (this.originalActor.model == 294) // Gargan Car
            this.originalActor.go.transform.FindChild("bone000").Translate(Vector3.back * 240f);
        if (this.originalActor.model == 306) // Gargant
            this.originalActor.go.transform.FindChild("bone000").Translate(Vector3.back * 190f);
        if (this.originalActor.model == 22 && FF9StateSystem.Common.FF9.fldMapNo == 116)
        {
            // Ladder, Alexandria/Rooftop
            this.originalActor.go.transform.FindChild("bone000").Translate(Vector3.up * 40f);
            this.originalActor.go.transform.FindChild("bone000").Translate(Vector3.forward * 25f);
        }
        if (this.originalActor.model == 488)
        {
            // Canal Boat
            if (this.originalActor.go.transform.localScale.y <= -1f)
                this.originalActor.go.transform.FindChild("bone000").Translate(Vector3.left * 220f);
            else if (this.originalActor.go.transform.localScale.y >= 1f)
                this.originalActor.go.transform.FindChild("bone000").Translate(Vector3.right * 130f);
        }
        if (FF9StateSystem.Common.FF9.fldMapNo == 1606 && (this.originalActor.model == 235 || this.originalActor.model == 236 || this.originalActor.model == 237))
        {
            // Mdn. Sari/Resting Room, Stew Pot or Cooked Fish or Stew Plate
            this.originalActor.go.transform.FindChild("bone000").Translate(Vector3.forward * -20f);
        }
        if (this.originalActor.anim == this.originalActor.turnr || this.originalActor.anim == this.originalActor.turnl)
        {
            Single t = (Single)this.originalActor.animFrame / (Single)(this.originalActor.frameN - 1);
            Vector3 b = Vector3.Lerp(Vector3.zero, this.originalActor.offsetTurn, t);
            Transform transform = this.originalActor.go.transform.FindChild("bone000");
            transform.position += b;
        }
    }

    private void UpdateNeck()
    {
        Vector3 a = new Vector3(this.actor.actor.xl, this.actor.actor.yl, this.actor.actor.zl);
        Transform childByName = base.transform.GetChildByName("bone" + this.actor.actor.neckBoneIndex.ToString("D3"));
        if (childByName == null)
            return;
        Vector3 a2 = a - childByName.position;
        a2 *= -1f;
        Single y = Vector2.Distance(Vector2.zero, new Vector2(a2.x, a2.z));
        Single num = Mathf.Atan2(a2.x, a2.z) * 57.29578f;
        Single num2 = Mathf.Atan2(y, -a2.y) * 57.29578f;
        Single num3 = base.transform.eulerAngles.y;
        if ((this.actor.actor.flags & 128) != 0)
            num3 = this.actor.actor.trot;
        Single rotY = num - num3;
        Single num4 = num2 - 90f;
        if (this.actor.actor.anim == 2692)
        {
            // Vivi, On_Bed_Snore
            childByName.rotation = Quaternion.AngleAxis(num4, Vector3.up) * childByName.rotation;
            childByName.localRotation *= Quaternion.AngleAxis(-90f, Vector3.left);
        }
        else if (this.actor.actor.anim == 1324)
        {
            // Blank, Tired
            childByName.rotation = Quaternion.AngleAxis(-40f, Vector3.down) * childByName.rotation;
            childByName.localRotation *= Quaternion.AngleAxis(-65f, Vector3.right);
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 1610 && this.originalActor.sid == 19 && (this.actor.actor.anim == 6126 || this.actor.actor.anim == 6130))
        {
            // Mdn. Sari/Cove, Zidane, Listen_1 or Listen_2
            childByName.rotation = Quaternion.AngleAxis(80f, Vector3.down) * childByName.rotation;
            childByName.localRotation *= Quaternion.AngleAxis(-10f, Vector3.right);
        }
        else if (this.actor.actor.anim == 3802)
        {
            // Eiko, Look_Up_2
            Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
            Int32 scCounterSvr = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
            Int32 mapIndexSvr = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR);
            if (fldMapNo == 1601 && this.originalActor.uid == 23 && scCounterSvr == 6600 && mapIndexSvr == 5)
            {
                // Mdn. Sari/Open Area, first time there
                this.ApplyNeckRotation(childByName, 23f, rotY);
            }
            else
            {
                this.ApplyNeckRotation(childByName, num4, rotY);
            }
        }
        else if (this.actor.actor.anim == 4630 || this.actor.actor.anim == 12938 || this.actor.actor.anim == 4636)
        {
            // Dagger, Hiza_1 or Hiza_Yes or Hiza_2
            childByName.rotation = Quaternion.Euler(-20f, 0f, 0f) * childByName.rotation;
        }
        else
        {
            this.ApplyNeckRotation(childByName, num4, rotY);
        }
    }

    private void ApplyNeckRotation(Transform neckBone, Single rotX, Single rotY)
    {
        neckBone.rotation = Quaternion.AngleAxis(rotY, Vector3.down) * neckBone.rotation;
        neckBone.localRotation *= Quaternion.AngleAxis(rotX, Vector3.right);
    }

    public Boolean isPlayingSomeAnimations()
    {
        return this.animation.isPlaying;
    }

    public void CopyLastPos()
    {
        this.lastPos = this.curPos;
    }

    public void ResetPos()
    {
        this.curPos = this.lastPos;
        this.activeFloor = this.lastFloor;
        this.activeTri = this.lastTri;
    }

    private void MovePC()
    {
        if (!FF9StateSystem.Field.isDebug && (!EventInput.IsMovementControl || this.fieldMap.isBattleBackupPos))
            return;
        if (!FF9StateSystem.Field.isDebug && this.originalActor.state != EventEngine.stateRunning)
            return;

        // This flag was set to false, causing the game to register digital movement unless on android
        Boolean analogMove = false;
        Vector2 analogVector = Vector2.zero;
        if (FF9StateSystem.MobilePlatform && VirtualAnalog.HasInput())
        {
            analogVector = VirtualAnalog.GetAnalogValue();
            analogMove = true;
        }
        else if (analogControlEnabled)
        {
            analogVector = PersistenSingleton<HonoInputManager>.Instance.GetAxis();
            if (analogVector.sqrMagnitude > 1f)
                analogVector.Normalize();
            analogMove = true;
        }

        if (analogMove)
            analogMove = analogVector.magnitude > stickThreshold;
        else
            analogVector = PersistenSingleton<HonoInputManager>.Instance.GetAxis();

        Boolean movingUp = UIManager.Input.GetKey(Control.Up) || analogVector.y >= 0.1f;
        Boolean movingDown = UIManager.Input.GetKey(Control.Down) || analogVector.y <= -0.1f;
        Boolean movingLeft = UIManager.Input.GetKey(Control.Left) || analogVector.x <= -0.1f;
        Boolean movingRight = UIManager.Input.GetKey(Control.Right) || analogVector.x >= 0.1f;

        // In RuntimePlatform.WindowsPlayer mode:
        //  PersistenSingleton<HonoInputManager>.Instance.GetAxis -> UnityEngine's GetAxis and UnityXInput's GetXAxis
        //  UIManager.Input.GetKey --------------------------------> UnityEngine's GetAxis and UnityXInput's GetXAxis and HonoInputManager's CheckPersistentDirectionInput
        //  UnityXInput.Input.GetXAxis ----------------------------> XInputManager's ThumbSticks and XInputManager's DPad
        //   UnityEngine.Input.GetAxis ------------------------------------------------------------> ??? On configured PS4 controllers, only capture the DPad (UnityEngine.dll)
        //   PersistenSingleton<UnityXInput.XInputManager>.Instance.CurrentState.ThumbSticks.Left -> Seems OK (XInputDotNetPure.dll)
        //   PersistenSingleton<UnityXInput.XInputManager>.Instance.CurrentState.DPad -------------> Seems OK (XInputDotNetPure.dll)
        //   PersistenSingleton<HonoInputManager>.Instance.CheckPersistentDirectionInput ----------> Keyboard arrow keys and WASD keys (user32.dll)
        // The Steam overlay should be fixed and a good controller configuration should be setup from the player's end
        HonoInputManager honoInput = PersistenSingleton<HonoInputManager>.Instance;
        Boolean isStickMovement = !honoInput.CheckPersistentDirectionInput(Control.Up)
                               && !honoInput.CheckPersistentDirectionInput(Control.Down)
                               && !honoInput.CheckPersistentDirectionInput(Control.Left)
                               && !honoInput.CheckPersistentDirectionInput(Control.Right)
                               && PersistenSingleton<UnityXInput.XInputManager>.Instance.CurrentState.DPad.Down != XInputDotNetPure.ButtonState.Pressed
                               && PersistenSingleton<UnityXInput.XInputManager>.Instance.CurrentState.DPad.Up != XInputDotNetPure.ButtonState.Pressed
                               && PersistenSingleton<UnityXInput.XInputManager>.Instance.CurrentState.DPad.Left != XInputDotNetPure.ButtonState.Pressed
                               && PersistenSingleton<UnityXInput.XInputManager>.Instance.CurrentState.DPad.Right != XInputDotNetPure.ButtonState.Pressed;

        if (!FF9StateSystem.Field.isDebug)
        {
            movingUp &= PersistenSingleton<EventEngine>.Instance.GetUserControl();
            movingDown &= PersistenSingleton<EventEngine>.Instance.GetUserControl();
            movingLeft &= PersistenSingleton<EventEngine>.Instance.GetUserControl();
            movingRight &= PersistenSingleton<EventEngine>.Instance.GetUserControl();
        }
        if (movingUp || movingDown || movingLeft || movingRight)
            this.ClearMoveTargetAndPath();
        if (this.isPlayer)
            FieldMapActorController.ccSMoveKey = movingUp || movingDown || movingLeft || movingRight || this.hasTarget;
        Single sqrNearPos = this.radius * this.radius * 0.95f * 0.95f;
        if (this.movePaths.Count > 0 && !this.hasTarget)
        {
            if (PersistenSingleton<EventEngine>.Instance.GetUserControl())
            {
                this.hasTarget = true;
                this.stillCount = 0;
                Int32 index = this.movePaths.Count - 1;
                this.moveTarget = this.movePaths[index];
                this.movePaths.RemoveAt(index);
            }
            else
            {
                this.movePaths.Clear();
                this.ClearMoveTarget();
                this.totalPathCount = 0;
            }
        }
        if (this.hasTarget)
        {
            Vector3 targetDir = this.moveTarget - this.curPos;
            targetDir.y = 0f;
            Single sqrMagnitude = targetDir.sqrMagnitude;
            if (this.movePaths.Count == 0)
                sqrNearPos = this.radius * this.radius;
            if (sqrMagnitude <= sqrNearPos)
            {
                this.ClearMoveTarget();
                this.moveVec = Vector3.zero;
                if (this.movePaths.Count == 0)
                {
                    this.totalPathCount = 0;
                    this.totalPathLengthSq = 0f;
                }
                this.MovePC();
                return;
            }
            this.moveVec = targetDir.normalized;
        }
        else
        {
            this.moveVec = Vector3.zero;
        }
        if (movingUp || movingDown || movingLeft || movingRight)
        {
            this.moveVec = Vector3.zero;
            if (analogMove)
            {
                this.moveVec = new Vector3(analogVector.x, 0f, analogVector.y);
                this.moveVec *= this.moveVec.sqrMagnitude;
            }
            else
            {
                if (this.moveVec.x < 0f || movingLeft)
                    this.moveVec.x = this.moveVec.x - this.speed;
                if (this.moveVec.x > 0f || movingRight)
                    this.moveVec.x = this.moveVec.x + this.speed;
                if (this.moveVec.z > 0f || movingUp)
                    this.moveVec.z = this.moveVec.z + this.speed;
                if (this.moveVec.z < 0f || movingDown)
                    this.moveVec.z = this.moveVec.z - this.speed;
            }
            // Removing this statement allows for variable movement speed.
            if (!analogControlEnabled)
                this.moveVec.Normalize();
            Single y = FF9StateSystem.Field.twist.y;
            if (analogControlEnabled)
            {
                if (isStickMovement && Configuration.AnalogControl.UseAbsoluteOrientationStick)
                    y = FF9StateSystem.Field.twist.x;
                else if (!isStickMovement && Configuration.AnalogControl.UseAbsoluteOrientationKeys)
                    y = FF9StateSystem.Field.twist.x;
            }
            Quaternion rotation = Quaternion.Euler(0f, y, 0f);
            this.moveVec = rotation * this.moveVec;
        }

        Vector3 actualMoveVec = this.moveVec * this.speed;

        // This block sets correct state and speed based on stick magnitude.
        if (analogControlEnabled)
        {
            Single currentSpeed = Mathf.Lerp(this.minimumSpeed, this.speed, moveVec.magnitude);
            Boolean dashInhibited = PersistenSingleton<EventEngine>.Instance.GetDashInh() == 1;
            Single trigger = wasRunning ? 0.5f : 0.6f;
            this.isRunning = (moveVec.magnitude > trigger && (UIManager.Input.GetKey(Control.Cancel) ^ FF9StateSystem.Settings.cfg.move == 1UL) && !dashInhibited);
            wasRunning = isRunning;
            actualMoveVec = this.moveVec.normalized * currentSpeed;
            // Inihibit movement if below threshold
            if (analogVector.magnitude <= stickThreshold)
                actualMoveVec = Vector3.zero;
        }

        if (Configuration.Control.PSXMovementMethod)
            actualMoveVec *= this.fieldMap.walkMesh.GetTriangleSlopeFactor(this.activeTri);

        this.curPos += actualMoveVec;
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        // Prevent rotation if below threshold
        if (movingUp || movingDown || movingLeft || movingRight || this.hasTarget && actualMoveVec != Vector3.zero)
        {
            Single moveAngle = Mathf.Atan2(-this.moveVec.x, -this.moveVec.z) * 57.29578f;
            if (Mathf.Abs(moveAngle - this.actor.actor.rotAngle[1]) > 180f)
            {
                if (moveAngle > this.actor.actor.rotAngle[1])
                    moveAngle -= 360f;
                else
                    moveAngle += 360f;
            }
            Single actorRot = Mathf.Lerp(this.actor.actor.rotAngle[1], moveAngle, 0.4f);
            while (actorRot > 180f)
                actorRot -= 360f;
            while (actorRot < -180f)
                actorRot += 360f;
            this.actor.actor.rotAngle[1] = actorRot;
            PosObj posObj = this.walkMesh.Collision(this, 0, out Single collDist);
            if (posObj != null)
            {
                instance.sLockFree = (Int64)(((posObj.flags & 16) != 0) ? 0L : 1L);
                if (instance.sLockFree == 0L)
                    instance.sLockTimer = 0L;
            }
            if (posObj != null && collDist <= 0f && instance.sLockTimer >= 0L)
            {
                this.originalActor.coll = posObj;
                this.originalActor.colldist = collDist;
                Vector3 collObjPos = new Vector3(posObj.pos[0], posObj.pos[1], posObj.pos[2]);
                Int32 fixedPointAngle = PersistenSingleton<EventEngine>.Instance.eBin.CollisionAngle(this.actor.actor, posObj, this.actor.actor.rotAngle[1]);
                Single degree = EventEngineUtils.ConvertFixedPointAngleToDegree((Int16)fixedPointAngle);
                if (degree >= -90f && degree <= 90f)
                {
                    Single safeDist = this.radius + 4 * posObj.collRad;
                    Single completeDist = (this.curPos - collObjPos).magnitude;
                    Vector3 normalized = (this.curPos - collObjPos).normalized;
                    Vector3 moveToSafe = (safeDist - completeDist) * normalized;
                    this.curPos += moveToSafe;
                    posObj = this.walkMesh.Collision(this, 0, out collDist);
                    if (posObj != null && collDist < 0f)
                    {
                        this.curPos -= moveToSafe;
                        this.curPos -= actualMoveVec;
                    }
                    instance.SCollTimer = 2;
                }
            }
            this.UpdateActiveTri();
        }
        this.CheckCollFallback();
    }

    private void CheckCollFallback()
    {
        if (FF9StateSystem.Common.FF9.fldMapNo == 2207 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 9840)
        {
            // Palace/Hall, when Zidane is summoned "inside the room past the stairs"
            return;
        }
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        if (instance.SCollTimer != 0)
        {
            if (instance.sLockTimer >= 25L)
                instance.sLockTimer = -25L;
            else
                instance.sLockTimer += instance.sLockFree;
        }
        else if (instance.sLockTimer >= 0L)
        {
            instance.sLockTimer = 0L;
        }
        else
        {
            instance.sLockTimer++;
        }
    }

    private void DebugDrawXOnSceneWindow(Vector3 position)
    {
        BGCAM_DEF currentBgCamera = this.fieldMap.GetCurrentBgCamera();
        Boolean flag = false;
        position.y = this.walkMesh.GetTriangleHeightAtPos(position, out flag);
        Int32 num = 20;
        global::Debug.DrawLine(position + new Vector3((Single)(-(Single)num), 0f, (Single)(-(Single)num)), position + new Vector3((Single)num, 0f, (Single)num), Color.red, 0f, true);
        global::Debug.DrawLine(position + new Vector3((Single)num, 0f, (Single)(-(Single)num)), position + new Vector3((Single)(-(Single)num), 0f, (Single)num), Color.red, 0f, true);
    }

    private void DebugDrawXOnSceneWindow2(Vector3 xzPos)
    {
        Vector3 zero = Vector3.zero;
        Boolean posOnTriangle = this.GetPosOnTriangle(xzPos, ref zero);
        if (posOnTriangle)
        {
            Int32 num = 20;
            global::Debug.DrawLine(zero + new Vector3((Single)(-(Single)num), 0f, (Single)(-(Single)num)), zero + new Vector3((Single)num, 0f, (Single)num), Color.red, 0f, true);
            global::Debug.DrawLine(zero + new Vector3((Single)num, 0f, (Single)(-(Single)num)), zero + new Vector3((Single)(-(Single)num), 0f, (Single)num), Color.red, 0f, true);
        }
    }

    private void DebugSetMarker(GameObject marker)
    {
        Vector3 zero = Vector3.zero;
        Boolean posOnTriangle = this.GetPosOnTriangle(marker.transform.position, ref zero);
        if (posOnTriangle)
            marker.transform.position = zero;
    }

    private Boolean GetPosOnTriangle(Vector3 pos, ref Vector3 output)
    {
        Int32 activeTriIdxAtPos = this.GetActiveTriIdxAtPos(pos);
        global::Debug.Log(String.Concat(new Object[]
        {
            "GetPosOnTriangle: pos = ",
            pos,
            ", triIdx = ",
            activeTriIdxAtPos
        }));
        if (activeTriIdxAtPos != -1)
        {
            WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[activeTriIdxAtPos];
            Vector3 barycentricCoef = Math3D.CalculateBarycentricRatioXZ(pos, walkMeshTriangle.originalVertices[0], walkMeshTriangle.originalVertices[1], walkMeshTriangle.originalVertices[2]);
            Vector3 posOnTriangle = walkMeshTriangle.originalVertices[0] * barycentricCoef.x + walkMeshTriangle.originalVertices[1] * barycentricCoef.y + walkMeshTriangle.originalVertices[2] * barycentricCoef.z;
            posOnTriangle.y += (Single)(this.fieldMap.bgi.floorList[walkMeshTriangle.floorIdx].curPos.coord[1] - this.fieldMap.bgi.floorList[walkMeshTriangle.floorIdx].orgPos.coord[1]);
            output = pos;
            output.y = posOnTriangle.y;
            return true;
        }
        return false;
    }

    private void MoveNPC()
    {
        Single sqrNearPos = this.radius * this.radius * 0.95f * 0.95f;
        if (this.movePaths.Count > 0 && !this.hasTarget)
        {
            this.hasTarget = true;
            Int32 index = this.movePaths.Count - 1;
            this.moveTarget = this.movePaths[index];
            this.movePaths.RemoveAt(index);
        }
        if (this.hasTarget)
        {
            Vector3 targetDir = this.moveTarget - this.curPos;
            targetDir.y = 0f;
            Single sqrMagnitude = targetDir.sqrMagnitude;
            if (sqrMagnitude <= sqrNearPos)
            {
                this.ClearMoveTarget();
                this.moveVec = Vector3.zero;
                if (this.movePaths.Count == 0)
                {
                    this.totalPathCount = 0;
                    this.totalPathLengthSq = 0f;
                }
                return;
            }
            this.moveVec = targetDir.normalized;
            this.actor.actor.rotAngle[1] = Mathf.Atan2(-this.moveVec.x, -this.moveVec.z) * 57.29578f;
        }
        else
        {
            this.moveVec = Vector3.zero;
        }

        Vector3 actualMoveVec = this.moveVec * this.speed;
        //if (Configuration.Control.PSXMovementMethod)
        //	actualMoveVec *= this.fieldMap.walkMesh.GetTriangleSlopeFactor(this.activeTri);
        this.curPos += actualMoveVec;
    }

    public void SetTwist(Int32 twistA, Int32 twistD)
    {
    }

    private void UpdateActiveTri()
    {
        if ((this.charFlags & 1) == 0)
            return;
        if ((FF9StateSystem.Common.FF9.fldMapNo == 2504 && this.originalActor.sid == 14) || (FF9StateSystem.Common.FF9.fldMapNo == 105 && this.originalActor.sid == 4) || (FF9StateSystem.Common.FF9.fldMapNo == 2605 && this.originalActor.sid == 11))
        {
            // I. Castle/Small Room, Chest_TerranA (containing a Fork)
            // or Alexandria/Alley, Dante
            // or Terra/Treetop, Chest_TerranA (containing a Remedy)
            return;
        }
        Int32 activeTriIdxAtPos = this.GetActiveTriIdxAtPos(this.curPos);
        if (activeTriIdxAtPos != -1)
        {
            WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[activeTriIdxAtPos];
            Vector3 barycentricCoef = Math3D.CalculateBarycentricRatioXZ(this.curPos, walkMeshTriangle.originalVertices[0], walkMeshTriangle.originalVertices[1], walkMeshTriangle.originalVertices[2]);
            Vector3 posOnTriangle = walkMeshTriangle.originalVertices[0] * barycentricCoef.x + walkMeshTriangle.originalVertices[1] * barycentricCoef.y + walkMeshTriangle.originalVertices[2] * barycentricCoef.z;
            posOnTriangle.y += (Single)(this.fieldMap.bgi.floorList[walkMeshTriangle.floorIdx].curPos.coord[1] - this.fieldMap.bgi.floorList[walkMeshTriangle.floorIdx].orgPos.coord[1]);
            this.curPos = posOnTriangle;
            this.activeFloor = walkMeshTriangle.floorIdx;
            this.activeTri = walkMeshTriangle.triIdx;
        }
        else
        {
            this.ResetPos();
        }
    }

    public Int32 ServiceChar()
    {
        Int32 movementFlags = 2;
        BGI_DEF bgi = this.fieldMap.bgi;
        Vector3 cPos = this.curPos;
        this.ShowDebugRadius();
        if (Mathf.Approximately((this.curPos - this.lastPos).magnitude, 0f))
            return movementFlags;
        if (this.activeFloor != -1 && this.activeTri != -1)
        {
            Int32 triIndex = this.activeTri;
            BGI_TRI_DEF bgi_TRI_DEF = bgi.triList[triIndex];
            if (this.walkMesh.BGI_traverseTriangles(this, ref bgi_TRI_DEF, this.lastPos, cPos, 0, 0f, 0f))
            {
                this.activeTri = triIndex;
                this.activeFloor = this.activeTri >= 0 ? (Int32)bgi.triList[this.activeTri].floorNdx : -1;
                this.curPos = this.lastPos;
                movementFlags |= 1;
                if (this.lastTri >= 0 && !this.walkMesh.BGI_pointInPoly(bgi.floorList[this.lastFloor], bgi.triList[this.lastTri], this.lastPos))
                {
                    this.lastFloor = this.activeFloor;
                    this.lastTri = this.activeTri;
                }
            }
            Int32 hitEdge = 0;
            if (!this.IsRadiusValid(ref cPos, this.activeTri, ref hitEdge))
            {
                if (!this.hasTarget)
                {
                }
                if (!this.ServiceForces(ref cPos, this.activeTri, ref hitEdge))
                {
                    this.activeTri = triIndex;
                    this.activeFloor = this.activeTri >= 0 ? (Int32)bgi.triList[this.activeTri].floorNdx : -1;
                    this.curPos = this.lastPos;
                    movementFlags |= 1;
                    if (this.lastTri >= 0 && this.lastFloor >= 0 && !this.walkMesh.BGI_pointInPoly(bgi.floorList[this.lastFloor], bgi.triList[this.lastTri], this.lastPos))
                    {
                        this.lastFloor = this.activeFloor;
                        this.lastTri = this.activeTri;
                    }
                    return movementFlags;
                }
                movementFlags |= 9;
            }
            if (cPos.x != this.curPos.x || cPos.z != this.curPos.z)
            {
                this.curPos.x = cPos.x;
                this.curPos.z = cPos.z;
                movementFlags |= 16;
            }
            this.lastTri = triIndex;
            this.lastFloor = this.lastTri >= 0 ? (Int32)bgi.triList[this.lastTri].floorNdx : -1;
            return movementFlags;
        }
        if (this.lastFloor != -1 || this.lastTri != -1)
        {
            this.ResetPos();
            return movementFlags;
        }
        this.lastPos = this.curPos;
        return movementFlags | 1;
    }

    private void ShowDebugRadius()
    {
        for (Int32 i = 0; i < 10; i++)
        {
            Vector3 vector1 = new Vector3(i, 0f, 9 - i);
            Vector3 vector2 = new Vector3(-i, 0f, 9 - i);
            Vector3 vector3 = new Vector3(i, 0f, i - 9);
            Vector3 vector4 = new Vector3(-i, 0f, i - 9);
            global::Debug.DrawLine(this.curPos, this.curPos + vector1.normalized * this.radius, Color.yellow, 0.5f, true);
            global::Debug.DrawLine(this.curPos, this.curPos + vector2.normalized * this.radius, Color.yellow, 0.5f, true);
            global::Debug.DrawLine(this.curPos, this.curPos + vector3.normalized * this.radius, Color.yellow, 0.5f, true);
            global::Debug.DrawLine(this.curPos, this.curPos + vector4.normalized * this.radius, Color.yellow, 0.5f, true);
        }
    }

    private Boolean IsRadiusValid(ref Vector3 cPos, Int32 triNdx, ref Int32 hitEdge)
    {
        if (triNdx == -1)
            return false;
        if (this.walkMesh.tris[triNdx].triFlags == 0)
            triNdx = this.lastTri;
        if (triNdx < 0)
            return false;
        this.forces.Clear();
        this.forcesFlag.Clear();
        this.forcesOrigin.Clear();
        this.forcesType.Clear();
        this.visited.Clear();
        this.visited.Add(triNdx);
        return this.RadiusValid(ref cPos, triNdx, ref hitEdge);
    }

    private Vector3 CalculateOriginalVertices(Int32 triNdx, Int32 ndx)
    {
        WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[triNdx];
        Vector3 result = walkMeshTriangle.originalVertices[ndx];
        if (FF9StateSystem.Common.FF9.fldMapNo == 2952)
        {
            // Chocobo's Air Garden
            BGI_FLOOR_DEF floor = this.fieldMap.bgi.floorList[walkMeshTriangle.floorIdx];
            for (Int32 i = 0; i < 3; i++)
                result[i] += floor.curPos.coord[i] - floor.orgPos.coord[i];
        }
        return result;
    }

    private Boolean RadiusValid(ref Vector3 cPos, Int32 triNdx, ref Int32 hitEdge)
    {
        WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[triNdx];
        Vector3 vertexA = this.CalculateOriginalVertices(triNdx, 2);
        Vector3 vertexB;
        Vector3 pointOnEdge;
        Single[] pointWithinTriangle = new Single[3];
        Boolean isPerp;
        Boolean isInvalid = false;
        for (Int32 i = 0; i < 3; i++)
        {
            vertexB = this.CalculateOriginalVertices(triNdx, i);
            Single sqrDistToEdge = Math3D.SqrDistanceToLine(cPos, vertexA, vertexB, out pointOnEdge, out isPerp);
            Single sqrRadius = this.radius * this.radius;
            if (sqrDistToEdge <= sqrRadius)
            {
                BGI_TRI_DEF neighboorTri = this.fieldMap.walkMesh.BGI_findAccessibleTriangle(this, walkMeshTriangle, (UInt32)i);
                if (neighboorTri != null)
                {
                    if (!this.visited.Contains(walkMeshTriangle.neighborIdx[i]))
                    {
                        this.visited.Add(walkMeshTriangle.neighborIdx[i]);
                        if (!this.RadiusValid(ref cPos, walkMeshTriangle.neighborIdx[i], ref hitEdge))
                        {
                            isInvalid = true;
                            hitEdge = i;
                        }
                    }
                }
                else if (sqrDistToEdge == 0f)
                {
                    isInvalid = true;
                }
                else
                {
                    Single distToEdge = Mathf.Sqrt(sqrDistToEdge);
                    this.walkMesh.BGI_computeNewPoint(pointOnEdge[0], cPos[0], this.radius, distToEdge, out pointWithinTriangle[0]);
                    this.walkMesh.BGI_computeNewPoint(pointOnEdge[2], cPos[2], this.radius, distToEdge, out pointWithinTriangle[2]);
                    Vector3 lhs = vertexA - vertexB;
                    Vector3 rhs = cPos - vertexA;
                    Vector3 rhs2 = walkMeshTriangle.originalCenter - vertexA;
                    lhs.y = 0f;
                    rhs.y = 0f;
                    rhs2.y = 0f;
                    Vector3 vector3 = Vector3.Cross(lhs, rhs);
                    Vector3 vector4 = Vector3.Cross(lhs, rhs2);
                    if ((Mathf.Sign(vector3.y) <= 0f || Mathf.Sign(vector4.y) <= 0f) && (Mathf.Sign(vector3.y) >= 0f || Mathf.Sign(vector4.y) >= 0f))
                    {
                        if (this.lastTri != -1)
                        {
                            WalkMeshTriangle lastWalkTri = this.walkMesh.tris[this.lastTri];
                            if (!Math3D.PointInsideTriangleTestXZ(cPos, lastWalkTri.originalVertices[0], lastWalkTri.originalVertices[1], lastWalkTri.originalVertices[2]))
                            {
                                pointWithinTriangle[0] *= -1f;
                                pointWithinTriangle[2] *= -1f;
                            }
                        }
                        else
                        {
                            pointWithinTriangle[0] *= -1f;
                            pointWithinTriangle[2] *= -1f;
                        }
                    }
                    if (this.forces.Count + 1 < 16)
                    {
                        pointWithinTriangle[0] = pointOnEdge[0] - cPos[0] + pointWithinTriangle[0];
                        pointWithinTriangle[2] = pointOnEdge[2] - cPos[2] + pointWithinTriangle[2];
                        this.forces.Add(new Vector3(pointWithinTriangle[0], 0f, pointWithinTriangle[2]));
                        this.forcesFlag.Add(pointOnEdge == vertexA || pointOnEdge == vertexB);
                        this.forcesOrigin.Add(pointOnEdge);
                        this.forcesType.Add(isPerp);
                    }
                    walkMeshTriangle.triFlags |= 128;
                    this.fieldMap.bgi.triList[triNdx].triFlags |= 128;
                    this.visited.Add(walkMeshTriangle.triIdx);
                    hitEdge = i;
                    isInvalid = true;
                }
            }
            vertexA = vertexB;
        }
        return !isInvalid;
    }

    private void RecalculateForce()
    {
        List<Vector3> list = new List<Vector3>();
        Int32 i = this.forces.Count;
        for (i = this.forces.Count - 1; i >= 0; i--)
        {
            if (this.forcesFlag[i] && !list.Contains(this.forcesOrigin[i]))
            {
                list.Add(this.forcesOrigin[i]);
                this.forces.RemoveAt(i);
                this.forcesFlag.RemoveAt(i);
                this.forcesOrigin.RemoveAt(i);
                this.forcesType.RemoveAt(i);
            }
        }
    }

    private Boolean ServiceForces(ref Vector3 cPos, Int32 triNdx, ref Int32 edge)
    {
        if (base.name == this.fieldMap.debugObjName)
        {
            for (Int32 i = 0; i < this.forces.Count; i++)
            {
                Vector3 position = base.transform.position;
                Vector3 vector = this.forces[i];
                Vector3 b = vector.normalized * 100f * vector.magnitude;
                Vector3 b2 = new Vector3(0f, 10f, 0f);
                global::Debug.DrawLine(position + b2, position + b + b2, Color.red, 2f, true);
                global::Debug.DrawLine(position + b2, position + Vector3.up * 50f + b2, Color.blue, 2f, true);
            }
        }
        Vector3 oldCPos = cPos;
        Vector3 forceBarycenter = Vector3.zero;
        if (this.forces.Count == 0)
            return true;
        if (this.forces.Count == 1)
        {
            cPos.x += this.forces[0].x;
            cPos.z += this.forces[0].z;
            this.curPos.x = cPos.x;
            this.curPos.z = cPos.z;
        }
        else
        {
            Boolean hasForcePoint = false;
            Int32 forcePointCount = 0;
            for (Int32 i = 0; i < this.forces.Count; i++)
            {
                if (this.forcesType[i])
                {
                    Boolean alreadyUsed = false;
                    for (Int32 j = 0; j < i; j++)
                        if (this.forces[j].x == this.forces[i].x && this.forces[j].z == this.forces[i].z)
                            alreadyUsed = true;
                    if (!alreadyUsed)
                    {
                        hasForcePoint = true;
                        forceBarycenter.x += this.forces[i].x;
                        forceBarycenter.z += this.forces[i].z;
                        forcePointCount++;
                    }
                }
            }
            if (hasForcePoint)
            {
                if (forcePointCount > 1)
                {
                    forceBarycenter.x /= forcePointCount;
                    forceBarycenter.z /= forcePointCount;
                }
            }
            else
            {
                forcePointCount = 0;
                for (Int32 i = 0; i < this.forces.Count; i++)
                {
                    Boolean alreadyUsed = false;
                    for (Int32 j = 0; j < i; j++)
                        if (this.forces[j].x == this.forces[i].x && this.forces[j].z == this.forces[i].z)
                            alreadyUsed = true;
                    if (!alreadyUsed)
                    {
                        forcePointCount++;
                        forceBarycenter.x += this.forces[i].x;
                        forceBarycenter.z += this.forces[i].z;
                    }
                }
                if (forcePointCount > 0)
                {
                    forceBarycenter.x /= (Single)forcePointCount;
                    forceBarycenter.z /= (Single)forcePointCount;
                }
            }
            Single rejectionFactor = 1.05f;
            if (FF9StateSystem.Common.FF9.fldMapNo == 1752 && (triNdx == 77 || triNdx == 78 || triNdx == 79 || triNdx == 80))
            {
                // Iifa Tree/Inner Roots (2nd area)
                rejectionFactor = triNdx == 80 ? 0.6f : 0.4f;
            }
            else if (FF9StateSystem.Common.FF9.fldMapNo == 406 && (triNdx == 103 || triNdx == 111 || triNdx == 113))
            {
                // Dali/Underground (room under the well)
                rejectionFactor = 0.4f;
            }
            cPos.x += forceBarycenter.x * rejectionFactor;
            cPos.z += forceBarycenter.z * rejectionFactor;
            this.curPos.x = cPos.x;
            this.curPos.z = cPos.z;
        }
        BGI_TRI_DEF activeTriangle = this.fieldMap.bgi.triList[triNdx];
        return !this.walkMesh.BGI_traverseTriangles(this, ref activeTriangle, oldCPos, cPos, 0, 0f, 0f);
    }

    private Int32 FindCrossingEdgeNeighbor(Int32 triIdx, Vector3 posA, Vector3 posB)
    {
        WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[triIdx];
        for (Int32 i = 0; i < 3; i++)
        {
            Int32 v1;
            Int32 v2;
            WalkMeshTriangle.GetVertexIdxForEdge(i, out v1, out v2);
            if (Math3D.FastLineSegmentIntersectionXZ(posA, posB, walkMeshTriangle.originalVertices[v1], walkMeshTriangle.originalVertices[v2]))
            {
                if (walkMeshTriangle.neighborIdx[i] != -1)
                {
                    BGI_TRI_DEF neighboorTriangle = this.fieldMap.bgi.triList[walkMeshTriangle.neighborIdx[i]];
                    Byte activeFlags = (Byte)(BGI.BGI_TRI_BITS_GET(neighboorTriangle.triFlags) & this.fieldMap.bgi.attributeMask);
                    if ((activeFlags & 0xC0) == 0)
                        return i;
                }
            }
        }
        return -1;
    }

    private Int32 GetTriIdxAtPos(Vector3 pos)
    {
        if (this.walkMesh == null)
            return -1;
        Int32 result = -1;
        Single resultDist = Single.MaxValue;
        if (FF9StateSystem.Common.FF9.fldMapNo == 70) // Opening-For FMV
            return -1;
        if (this.walkMesh == null)
            return -1;
        for (Int32 i = 0; i < this.walkMesh.tris.Count; i++)
        {
            WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[i];
            Vector3 vA = walkMeshTriangle.originalVertices[0];
            Vector3 vB = walkMeshTriangle.originalVertices[1];
            Vector3 vC = walkMeshTriangle.originalVertices[2];
            if (Math3D.PointInsideTriangleTestXZ(pos, vA, vB, vC))
            {
                Single dist = Mathf.Abs(pos.y - walkMeshTriangle.originalCenter.y);
                if (dist < resultDist)
                {
                    result = i;
                    resultDist = dist;
                }
            }
        }
        return result;
    }

    private Int32 GetTopTriIdxAtPos(Vector3 pos)
    {
        if (this.walkMesh == null)
            return -1;
        Int32 result = -1;
        Single resultHeight = Single.MinValue;
        for (Int32 i = 0; i < this.walkMesh.tris.Count; i++)
        {
            WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[i];
            Vector3 vA = walkMeshTriangle.originalVertices[0];
            Vector3 vB = walkMeshTriangle.originalVertices[1];
            Vector3 vC = walkMeshTriangle.originalVertices[2];
            if (Math3D.PointInsideTriangleTestXZ(pos, vA, vB, vC) && walkMeshTriangle.originalCenter.y > resultHeight)
            {
                result = i;
                resultHeight = walkMeshTriangle.originalCenter.y;
            }
        }
        return result;
    }

    public Int32 GetActiveTriIdxAtPos(Vector3 pos)
    {
        if (this.walkMesh == null)
            return -1;
        Int32 result = -1;
        Single resultDist = Single.MaxValue;
        this.foundTris.Clear();
        this.adjacentActiveTris.Clear();
        for (Int32 i = 0; i < this.walkMesh.tris.Count; i++)
        {
            WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[i];
            if ((walkMeshTriangle.triFlags & 1) != 0)
            {
                Vector3 vA = walkMeshTriangle.originalVertices[0];
                Vector3 vB = walkMeshTriangle.originalVertices[1];
                Vector3 vC = walkMeshTriangle.originalVertices[2];
                if (Math3D.PointInsideTriangleTestXZ(pos, vA, vB, vC))
                    this.foundTris.Add(i);
            }
        }
        if (this.foundTris.Count == 1)
        {
            if ((FF9StateSystem.Common.FF9.fldMapNo == 2712 && this.originalActor.sid == 18) || (FF9StateSystem.Common.FF9.fldMapNo == 2504 && this.originalActor.sid == 20) || (FF9StateSystem.Common.FF9.fldMapNo == 1056 && this.originalActor.sid == 18) || (FF9StateSystem.Common.FF9.fldMapNo == 2955 && this.originalActor.sid == 3) || (FF9StateSystem.Common.FF9.fldMapNo == 1807 && this.originalActor.sid == 27) || (FF9StateSystem.Common.FF9.fldMapNo == 1106 && (this.originalActor.sid == 18 || this.originalActor.sid == 22)))
            {
                // Pand./Elevator (tallest room), Zidane
                // or I. Castle/Small Room, Zidane
                // or Cleyra/Inn (w/ sandstorm), Zidane
                // or Chocobo's Paradise, Chocobo_Gold
                // or A. Castle/Hallway (room in which Pluto Knights are given orders), Dagger
                // or Cleyra/Inn (w/out sandstorm), Zidane or Freya
                if (this.activeFloor == -1)
                {
                    result = this.foundTris[0];
                }
                else
                {
                    Single heightDiff = Mathf.Abs(pos.y - this.walkMesh.tris[this.foundTris[0]].originalCenter.y);
                    if (heightDiff < 432f)
                        result = this.foundTris[0];
                    else
                        result = -1;
                }
            }
            else
            {
                result = this.foundTris[0];
            }
        }
        else if (this.foundTris.Count >= 2)
        {
            this.FindAdjacentTrianglesRecursively(0, this.activeTri);
            for (Int32 k = 0; k < this.foundTris.Count; k++)
                if (this.foundTris[k] == this.activeTri)
                    return this.foundTris[k];
            if (this.activeTri != -1)
            {
                for (Int32 l = 0; l < this.foundTris.Count; l++)
                    for (Int32 m = 0; m < this.adjacentActiveTris.Count; m++)
                        if (this.foundTris[l] == this.adjacentActiveTris[m])
                            result = this.foundTris[l];
            }
            else
            {
                for (Int32 n = 0; n < this.foundTris.Count; n++)
                {
                    Single distToCenter = Mathf.Abs(pos.y - this.walkMesh.tris[this.foundTris[n]].originalCenter.y);
                    if (distToCenter < resultDist)
                    {
                        result = this.foundTris[n];
                        resultDist = distToCenter;
                    }
                }
            }
        }
        return result;
    }

    private void FindAdjacentTrianglesRecursively(Int32 recursiveCount, Int32 currentTriIdx)
    {
        if (recursiveCount >= 3 || currentTriIdx == -1)
            return;
        WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[currentTriIdx];
        for (Int32 i = 0; i < walkMeshTriangle.neighborIdx.Length; i++)
        {
            Int32 neighboorIdx = walkMeshTriangle.neighborIdx[i];
            if (neighboorIdx != -1)
            {
                if (!this.adjacentActiveTris.Contains(neighboorIdx))
                    this.adjacentActiveTris.Add(neighboorIdx);
                this.FindAdjacentTrianglesRecursively(recursiveCount + 1, neighboorIdx);
            }
        }
    }

    private Vector3 ForceForVector(WalkMeshTriangle targetTriangle)
    {
        Single sqrRadius = this.radius * this.radius;
        Vector3 vector = targetTriangle.originalCenter;
        List<Vector3> list = new List<Vector3>();
        Queue<WalkMeshTriangle> queue = new Queue<WalkMeshTriangle>();
        List<WalkMeshTriangle> processedTri = new List<WalkMeshTriangle>();
        queue.Enqueue(targetTriangle);
        processedTri.Add(targetTriangle);
        while (queue.Count > 0)
        {
            WalkMeshTriangle walkMeshTriangle = queue.Dequeue();
            for (Int32 i = 0; i < 3; i++)
            {
                Int32 v1;
                Int32 v2;
                WalkMeshTriangle.GetVertexIdxForEdge(i, out v1, out v2);
                if (walkMeshTriangle.neighborIdx[i] == -1 || this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]].triFlags == 0)
                {
                    Single sqrDistToEdge = Math3D.SqrDistanceToLine(vector, walkMeshTriangle.originalVertices[v1], walkMeshTriangle.originalVertices[v2]);
                    if (sqrRadius > sqrDistToEdge)
                    {
                        Vector3 a = Math3D.ClosestPointToLine(vector, walkMeshTriangle.originalVertices[v1], walkMeshTriangle.originalVertices[v2]);
                        Vector3 item = -a / Mathf.Sqrt(sqrDistToEdge) * (Mathf.Sqrt(sqrRadius) - Mathf.Sqrt(sqrDistToEdge));
                        if (!list.Contains(item))
                            list.Add(item);
                    }
                }
                else
                {
                    WalkMeshTriangle walkMeshTriangle2 = this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]];
                    if (!processedTri.Contains(this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]]) && (sqrRadius > Math3D.SqrDistanceToLine(vector, walkMeshTriangle2.originalVertices[0], walkMeshTriangle2.originalVertices[1]) || sqrRadius > Math3D.SqrDistanceToLine(vector, walkMeshTriangle2.originalVertices[1], walkMeshTriangle2.originalVertices[2]) || sqrRadius > Math3D.SqrDistanceToLine(vector, walkMeshTriangle2.originalVertices[0], walkMeshTriangle2.originalVertices[2])))
                    {
                        queue.Enqueue(this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]]);
                        processedTri.Add(this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]]);
                    }
                }
            }
            for (Int32 j = 0; j < list.Count; j++)
            {
                Single num5 = Mathf.Round(list[j].x * 10000f);
                Single num6 = Mathf.Round(list[j].y * 10000f);
                Single num7 = Mathf.Round(list[j].z * 10000f);
                if (num5 != 0f || num6 != 0f || num7 != 0f)
                    vector += list[j] * 0.5f;
            }
            list.Clear();
        }
        return vector;
    }

    private Vector3 ForceForVector(WalkMeshTriangle targetTriangle, Vector3 walkableVector)
    {
        Single sqrRadius = this.radius * this.radius;
        List<Vector3> list = new List<Vector3>();
        Queue<WalkMeshTriangle> queue = new Queue<WalkMeshTriangle>();
        List<WalkMeshTriangle> processedTri = new List<WalkMeshTriangle>();
        queue.Enqueue(targetTriangle);
        processedTri.Add(targetTriangle);
        while (queue.Count > 0)
        {
            WalkMeshTriangle walkMeshTriangle = queue.Dequeue();
            for (Int32 i = 0; i < 3; i++)
            {
                Int32 v1;
                Int32 v2;
                WalkMeshTriangle.GetVertexIdxForEdge(i, out v1, out v2);
                if (walkMeshTriangle.neighborIdx[i] == -1 || this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]].triFlags == 0)
                {
                    Single sqrDistToEdge = Math3D.SqrDistanceToLine(walkableVector, walkMeshTriangle.originalVertices[v1], walkMeshTriangle.originalVertices[v2]);
                    if (sqrRadius > sqrDistToEdge)
                    {
                        Vector3 a = Math3D.ClosestPointToLine(walkableVector, walkMeshTriangle.originalVertices[v1], walkMeshTriangle.originalVertices[v2]);
                        Vector3 item = -a / Mathf.Sqrt(sqrDistToEdge) * (Mathf.Sqrt(sqrRadius) - Mathf.Sqrt(sqrDistToEdge));
                        if (!list.Contains(item))
                            list.Add(item);
                    }
                }
                else
                {
                    WalkMeshTriangle walkMeshTriangle2 = this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]];
                    if (!processedTri.Contains(this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]]) && (sqrRadius > Math3D.SqrDistanceToLine(walkableVector, walkMeshTriangle2.originalVertices[0], walkMeshTriangle2.originalVertices[1]) || sqrRadius > Math3D.SqrDistanceToLine(walkableVector, walkMeshTriangle2.originalVertices[1], walkMeshTriangle2.originalVertices[2]) || sqrRadius > Math3D.SqrDistanceToLine(walkableVector, walkMeshTriangle2.originalVertices[0], walkMeshTriangle2.originalVertices[2])))
                    {
                        queue.Enqueue(this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]]);
                        processedTri.Add(this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]]);
                    }
                }
            }
            for (Int32 j = 0; j < list.Count; j++)
            {
                Single num5 = Mathf.Round(list[j].x * 10000f);
                Single num6 = Mathf.Round(list[j].y * 10000f);
                Single num7 = Mathf.Round(list[j].z * 10000f);
                if (num5 != 0f || num6 != 0f || num7 != 0f)
                    walkableVector += list[j];
            }
            list.Clear();
        }
        return walkableVector;
    }

    private Boolean IsWalkableRadius(Vector3 start, Vector3 end, List<Int32> trisIdx)
    {
        Boolean flag = true;
        Single sqrSmallRadius = this.radius * this.radius * 0.95f * 0.95f;
        List<Vector3> vertexList = new List<Vector3>();
        List<Vector3> edgeList = new List<Vector3>();
        for (Int32 i = trisIdx.Count - 1; i >= 0; i--)
        {
            if (trisIdx[i] == -1)
            {
                trisIdx.RemoveAt(i);
            }
            else
            {
                if (this.walkMesh.tris[trisIdx[i]].neighborIdx[0] != -1 && !trisIdx.Contains(this.walkMesh.tris[trisIdx[i]].neighborIdx[0]))
                    trisIdx.Add(this.walkMesh.tris[trisIdx[i]].neighborIdx[0]);
                if (this.walkMesh.tris[trisIdx[i]].neighborIdx[1] != -1 && !trisIdx.Contains(this.walkMesh.tris[trisIdx[i]].neighborIdx[1]))
                    trisIdx.Add(this.walkMesh.tris[trisIdx[i]].neighborIdx[1]);
                if (this.walkMesh.tris[trisIdx[i]].neighborIdx[2] != -1 && !trisIdx.Contains(this.walkMesh.tris[trisIdx[i]].neighborIdx[2]))
                    trisIdx.Add(this.walkMesh.tris[trisIdx[i]].neighborIdx[2]);
            }
        }
        for (Int32 i = 0; i < trisIdx.Count; i++)
        {
            Queue<WalkMeshTriangle> queue = new Queue<WalkMeshTriangle>();
            List<WalkMeshTriangle> processedTri = new List<WalkMeshTriangle>();
            queue.Enqueue(this.walkMesh.tris[trisIdx[i]]);
            processedTri.Add(this.walkMesh.tris[trisIdx[i]]);
            while (queue.Count > 0)
            {
                WalkMeshTriangle walkMeshTriangle = queue.Dequeue();
                for (Int32 j = 0; j < 3; j++)
                {
                    Int32 v1;
                    Int32 v2;
                    WalkMeshTriangle.GetVertexIdxForEdge(j, out v1, out v2);
                    if (walkMeshTriangle.neighborIdx[j] != -1)
                    {
                        if (!processedTri.Contains(this.walkMesh.tris[walkMeshTriangle.neighborIdx[j]]) && (Math3D.SqrDistanceToLine(this.walkMesh.tris[walkMeshTriangle.neighborIdx[j]].originalVertices[0], start, end) < sqrSmallRadius || Math3D.SqrDistanceToLine(this.walkMesh.tris[walkMeshTriangle.neighborIdx[j]].originalVertices[1], start, end) < sqrSmallRadius || Math3D.SqrDistanceToLine(this.walkMesh.tris[walkMeshTriangle.neighborIdx[j]].originalVertices[2], start, end) < sqrSmallRadius))
                        {
                            queue.Enqueue(this.walkMesh.tris[walkMeshTriangle.neighborIdx[j]]);
                            processedTri.Add(this.walkMesh.tris[walkMeshTriangle.neighborIdx[j]]);
                            break;
                        }
                    }
                    else
                    {
                        edgeList.Add(walkMeshTriangle.originalVertices[v1]);
                        edgeList.Add(walkMeshTriangle.originalVertices[v2]);
                        if (!vertexList.Contains(walkMeshTriangle.originalVertices[v1]))
                            vertexList.Add(walkMeshTriangle.originalVertices[v1]);
                        if (!vertexList.Contains(walkMeshTriangle.originalVertices[v2]))
                            vertexList.Add(walkMeshTriangle.originalVertices[v2]);
                    }
                }
            }
        }
        for (Int32 i = 0; i < vertexList.Count; i++)
            flag &= Math3D.SqrDistanceToLine(vertexList[i], start, end) > sqrSmallRadius;
        for (Int32 i = 0; i < edgeList.Count; i += 2)
            flag &= !Math3D.FastLineSegmentIntersectionXZ(edgeList[i], edgeList[i + 1], start, end);
        return flag;
    }

    private Boolean IsWalkableDepth0(WalkMeshTriangle t1, WalkMeshTriangle t2)
    {
        Boolean flag = true;
        List<Vector3> outerEdgev1 = new List<Vector3>();
        List<Vector3> outerEdgev2 = new List<Vector3>();
        for (Int32 i = 0; i < 3; i++)
        {
            Int32 v1;
            Int32 v2;
            WalkMeshTriangle.GetVertexIdxForEdge(i, out v1, out v2);
            if (t1.neighborIdx[i] == -1)
            {
                outerEdgev1.Add(t1.originalVertices[v1]);
                outerEdgev2.Add(t1.originalVertices[v2]);
            }
            if (t2.neighborIdx[i] == -1)
            {
                outerEdgev1.Add(t2.originalVertices[v1]);
                outerEdgev2.Add(t2.originalVertices[v2]);
            }
        }
        for (Int32 i = 0; i < outerEdgev1.Count; i++)
            flag &= !Math3D.FastLineSegmentIntersectionXZ(t1.originalCenter, t2.originalCenter, outerEdgev1[i], outerEdgev2[i]);
        return flag;
    }

    private Boolean IsWalkableDepth0(WalkMeshTriangle t1, WalkMeshTriangle t2, Vector3 p1, Vector3 p2)
    {
        Boolean flag = true;
        List<Vector3> outerEdgev1 = new List<Vector3>();
        List<Vector3> outerEdgev2 = new List<Vector3>();
        for (Int32 i = 0; i < 3; i++)
        {
            Int32 v1;
            Int32 v2;
            WalkMeshTriangle.GetVertexIdxForEdge(i, out v1, out v2);
            if (t1.neighborIdx[i] == -1)
            {
                outerEdgev1.Add(t1.originalVertices[v1]);
                outerEdgev2.Add(t1.originalVertices[v2]);
            }
            if (t2.neighborIdx[i] == -1)
            {
                outerEdgev1.Add(t2.originalVertices[v1]);
                outerEdgev2.Add(t2.originalVertices[v2]);
            }
        }
        for (Int32 i = 0; i < outerEdgev1.Count; i++)
            flag &= !Math3D.FastLineSegmentIntersectionXZ(p1, p2, outerEdgev1[i], outerEdgev2[i]);
        return flag;
    }

    private Boolean IsTriangleVerticesAllEdges(WalkMeshTriangle tri)
    {
        List<Vector3> outerEdge = new List<Vector3>();
        List<Vector3> innerEdge = new List<Vector3>();
        for (Int32 i = 0; i < 3; i++)
        {
            Int32 v1;
            Int32 v2;
            WalkMeshTriangle.GetVertexIdxForEdge(i, out v1, out v2);
            if (tri.neighborIdx[i] == -1)
            {
                outerEdge.Add(tri.originalVertices[v1]);
                outerEdge.Add(tri.originalVertices[v2]);
            }
            else
            {
                innerEdge.Add(tri.originalVertices[v1]);
                innerEdge.Add(tri.originalVertices[v2]);
            }
        }
        if (innerEdge.Count == 4)
        {
            Single length1 = (innerEdge[0] - innerEdge[1]).magnitude;
            Single length2 = (innerEdge[2] - innerEdge[3]).magnitude;
            if (length1 * 2f > length2 && length2 * 2f > length1 && length1 > this.radius * 4f && length2 > this.radius * 4f)
                return false;
        }
        for (Int32 i = 0; i < 3; i++)
        {
            if (tri.neighborIdx[i] != -1)
            {
                WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[tri.neighborIdx[i]];
                for (Int32 j = 0; j < 3; j++)
                {
                    if (walkMeshTriangle.neighborIdx[j] == -1)
                    {
                        Int32 v1;
                        Int32 v2;
                        WalkMeshTriangle.GetVertexIdxForEdge(j, out v1, out v2);
                        outerEdge.Add(walkMeshTriangle.originalVertices[v1]);
                        outerEdge.Add(walkMeshTriangle.originalVertices[v2]);
                    }
                }
            }
        }
        return outerEdge.Contains(tri.originalVertices[0]) && outerEdge.Contains(tri.originalVertices[1]) && outerEdge.Contains(tri.originalVertices[2]);
    }

    private Boolean IsTriangleVerticesAllEdgesDepth2(WalkMeshTriangle tri)
    {
        List<Vector3> outerEdge = new List<Vector3>();
        for (Int32 i = 0; i < 3; i++)
        {
            if (tri.neighborIdx[i] == -1)
            {
                Int32 v1;
                Int32 v2;
                WalkMeshTriangle.GetVertexIdxForEdge(i, out v1, out v2);
                outerEdge.Add(tri.originalVertices[v1]);
                outerEdge.Add(tri.originalVertices[v2]);
            }
        }
        List<WalkMeshTriangle> neighboorTri = new List<WalkMeshTriangle>();
        for (Int32 i = 0; i < 3; i++)
        {
            if (tri.neighborIdx[i] != -1)
            {
                neighboorTri.Add(this.walkMesh.tris[tri.neighborIdx[i]]);
                WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[tri.neighborIdx[i]];
                for (Int32 j = 0; j < 3; j++)
                    if (walkMeshTriangle.neighborIdx[j] != -1)
                        neighboorTri.Add(this.walkMesh.tris[walkMeshTriangle.neighborIdx[j]]);
            }
        }
        for (Int32 i = 0; i < neighboorTri.Count; i++)
        {
            if (tri.neighborIdx[i] != -1)
            {
                WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[tri.neighborIdx[i]];
                for (Int32 j = 0; j < 3; j++)
                {
                    if (walkMeshTriangle.neighborIdx[j] == -1)
                    {
                        Int32 v1;
                        Int32 v2;
                        WalkMeshTriangle.GetVertexIdxForEdge(j, out v1, out v2);
                        outerEdge.Add(walkMeshTriangle.originalVertices[v1]);
                        outerEdge.Add(walkMeshTriangle.originalVertices[v2]);
                    }
                }
            }
        }
        return outerEdge.Contains(tri.originalVertices[0]) && outerEdge.Contains(tri.originalVertices[1]) && outerEdge.Contains(tri.originalVertices[2]);
    }

    private void CheckPosInProjectedWalkMesh()
    {
        if (Configuration.Control.DisableMouseInFields)
            return;
        if ((this.charFlags & 1) == 0)
            return;
        Boolean issueMoveByMouse = false;
        Boolean debugTeleport = false;
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            issueMoveByMouse = true;
        if (!issueMoveByMouse && !debugTeleport)
            return;
        PSXCameraAspect psxcameraAspect = UnityEngine.Object.FindObjectOfType<PSXCameraAspect>();
        Vector3 localMousePosRelative = psxcameraAspect.GetLocalMousePosRelative();
        if (this.walkMesh == null)
            return;
        Int32 triangleIndexAtScreenPos = this.walkMesh.GetTriangleIndexAtScreenPos(localMousePosRelative);
        DebugUtil.DebugDrawMarker(localMousePosRelative, 5f, Color.red);
        if (triangleIndexAtScreenPos == -1)
            return;
        WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[triangleIndexAtScreenPos];
        Vector3 vert1 = walkMeshTriangle.transformedVertices[0];
        Vector3 vert2 = walkMeshTriangle.transformedVertices[1];
        Vector3 vert3 = walkMeshTriangle.transformedVertices[2];
        BGCAM_DEF currentBgCamera = this.fieldMap.GetCurrentBgCamera();
        Vector3 mouseBarycentricCoef = Math3D.CalculateBarycentricRatio(localMousePosRelative, vert1, vert2, vert3);
        Vector3 centerBarycentricCoef = Math3D.CalculateBarycentric(walkMeshTriangle.originalCenter, walkMeshTriangle.originalVertices[0], walkMeshTriangle.originalVertices[1], walkMeshTriangle.originalVertices[2]);
        Vector3 centerScreenBaryCoef = Math3D.CalculateBarycentric(PSX.CalculateGTE_RTPT(walkMeshTriangle.originalCenter, Matrix4x4.identity, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), this.fieldMap.offset), walkMeshTriangle.transformedVertices[0], walkMeshTriangle.transformedVertices[1], walkMeshTriangle.transformedVertices[2]);
        Single num = centerBarycentricCoef.x / centerScreenBaryCoef.x + centerBarycentricCoef.y / centerScreenBaryCoef.y + centerBarycentricCoef.z / centerScreenBaryCoef.z;
        Vector3 vector5 = new Vector3(centerBarycentricCoef.x / centerScreenBaryCoef.x / num, centerBarycentricCoef.y / centerScreenBaryCoef.y / num, centerBarycentricCoef.z / centerScreenBaryCoef.z / num);
        Single num2 = mouseBarycentricCoef.x * vector5.x + mouseBarycentricCoef.y * vector5.y + mouseBarycentricCoef.z * vector5.z;
        mouseBarycentricCoef.x = mouseBarycentricCoef.x * vector5.x / num2;
        mouseBarycentricCoef.y = mouseBarycentricCoef.y * vector5.y / num2;
        mouseBarycentricCoef.z = mouseBarycentricCoef.z * vector5.z / num2;
        Vector3 mousePosOnTriangle = walkMeshTriangle.originalVertices[0] * mouseBarycentricCoef.x + walkMeshTriangle.originalVertices[1] * mouseBarycentricCoef.y + walkMeshTriangle.originalVertices[2] * mouseBarycentricCoef.z;
        Vector3 mouseScreenPosOnTriangle = walkMeshTriangle.transformedVertices[0] * mouseBarycentricCoef.x + walkMeshTriangle.transformedVertices[1] * mouseBarycentricCoef.y + walkMeshTriangle.transformedVertices[2] * mouseBarycentricCoef.z;
        DebugUtil.DebugDrawMarker(mouseScreenPosOnTriangle, 5f, Color.green);
        mousePosOnTriangle = this.ForceForVector(this.walkMesh.tris[triangleIndexAtScreenPos], mousePosOnTriangle);
        if (this.CheckCollideNPC(this.walkMesh.tris[triangleIndexAtScreenPos], mousePosOnTriangle))
            return;
        if (issueMoveByMouse)
        {
            if (debugTeleport)
            {
                this.SetPosition(mousePosOnTriangle, true, true);
                this.movePaths.Clear();
                this.ClearMoveTarget();
            }
            else
            {
                WalkMeshTriangle pathTriangle1 = this.walkMesh.FindPathReversed(walkMeshTriangle, this.walkMesh.tris[this.activeTri], this.radius);
                List<Int32> pathIdxList1 = new List<Int32>();
                List<Vector3> pathPos1 = new List<Vector3>();
                Single pathLength1 = 0f;
                if (pathTriangle1 != null)
                {
                    while (pathTriangle1 != null && pathTriangle1.next != null)
                    {
                        pathIdxList1.Add(pathTriangle1.triIdx);
                        pathTriangle1 = pathTriangle1.next;
                    }
                    if (pathTriangle1 != null)
                        pathIdxList1.Add(pathTriangle1.triIdx);
                    pathPos1 = this.SmoothPathsByForce(pathIdxList1, this.curPos, mousePosOnTriangle);
                    for (Int32 i = 1; i < pathPos1.Count; i++)
                        pathLength1 += Vector3.Distance(pathPos1[i - 1], pathPos1[i]);
                }
                WalkMeshTriangle pathTriangle2 = this.walkMesh.FindPathReversed(this.walkMesh.tris[this.activeTri], walkMeshTriangle, this.radius);
                List<Int32> pathIdxList2 = new List<Int32>();
                List<Vector3> pathPos2 = new List<Vector3>();
                Single pathLength2 = 0f;
                if (pathTriangle2 != null)
                {
                    while (pathTriangle2 != null && pathTriangle2.next != null)
                    {
                        pathIdxList2.Add(pathTriangle2.triIdx);
                        pathTriangle2 = pathTriangle2.next;
                    }
                    if (pathTriangle2 != null)
                        pathIdxList2.Add(pathTriangle2.triIdx);
                    pathPos2 = this.SmoothPathsByForce(pathIdxList2, mousePosOnTriangle, this.curPos);
                    pathPos2.Reverse();
                    for (Int32 i = 1; i < pathPos2.Count; i++)
                        pathLength2 += Vector3.Distance(pathPos2[i - 1], pathPos2[i]);
                }
                if (pathTriangle1 != null || pathTriangle2 != null)
                {
                    if (pathLength2 < pathLength1)
                    {
                        pathPos1 = pathPos2;
                        pathIdxList1 = pathIdxList2;
                    }
                    this.ClearMoveTarget();
                    this.movePaths.Clear();
                    this.movePaths.AddRange(pathPos1);
                    this.debugSmoothPaths.Clear();
                    this.debugSmoothPaths.AddRange(pathPos1);
                    this.debugPaths.Clear();
                    for (Int32 i = 0; i < pathIdxList1.Count; i++)
                        this.debugPaths.Add(this.walkMesh.tris[pathIdxList1[i]].originalCenter);
                    this.totalPathCount = pathIdxList1.Count;
                    this.totalPathLengthSq = 0f;
                    if (pathPos1.Count == 1)
                    {
                        this.totalPathLengthSq += (this.curPos - pathPos1[0]).sqrMagnitude;
                    }
                    else
                    {
                        for (Int32 l = 0; l < pathPos1.Count - 1; l++)
                            this.totalPathLengthSq += (pathPos1[l] - pathPos1[l + 1]).sqrMagnitude;
                    }
                }
            }
        }
    }

    private Boolean CheckCollideNPC(WalkMeshTriangle targetTriangle, Vector3 walkableVector)
    {
        FieldMapActorController[] allActorController = UnityEngine.Object.FindObjectsOfType<FieldMapActorController>();
        for (Int32 i = 0; i < allActorController.Length; i++)
        {
            if (allActorController[i] != this)
            {
                Vector3 position = allActorController[i].transform.position;
                Single sqrMagnitude = (walkableVector - position).sqrMagnitude;
                Single sqrDoubleRadius = this.radius * 2f;
                sqrDoubleRadius *= sqrDoubleRadius;
                if (sqrMagnitude < sqrDoubleRadius)
                    return true;
            }
        }
        return false;
    }

    private List<Vector3> SmoothPathsByForce(List<Int32> pathsIdx, Vector3 start, Vector3 end)
    {
        if (pathsIdx.Count == 0)
            return null;
        List<Vector3> stepPosition = new List<Vector3>();
        List<List<Int32>> list2 = new List<List<Int32>>();
        stepPosition.Add(start);
        list2.Add(new List<Int32>());
        list2[0].Add(pathsIdx[0]);
        Vector3 vector = start;
        Vector3 vector2 = vector;
        Int32 num = 0;
        Int32 num2 = 0;
        Int32 i = num2 + 1;
        if (pathsIdx.Count < 1)
        {
            list2[list2.Count - 1].Add(this.GetTriIdxAtPos(start));
            list2[list2.Count - 1].Add(this.GetTriIdxAtPos(end));
            stepPosition.Add(end);
            return stepPosition;
        }
        List<Vector3> list3 = new List<Vector3>();
        for (Int32 j = 0; j < pathsIdx.Count; j++)
        {
            WalkMeshTriangle targetTriangle = this.walkMesh.tris[pathsIdx[j]];
            Vector3 item = this.ForceForVector(targetTriangle);
            list3.Add(item);
        }
        if (this.IsTriangleVerticesAllEdges(this.walkMesh.tris[pathsIdx[num2]]))
        {
            WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[pathsIdx[num2]];
            List<Vector3> list4 = new List<Vector3>();
            if (stepPosition.Count > 1)
            {
                for (Int32 k = 0; k < 3; k++)
                {
                    Int32 v1;
                    Int32 v2;
                    WalkMeshTriangle.GetVertexIdxForEdge(k, out v1, out v2);
                    if (walkMeshTriangle.neighborIdx[k] != -1)
                    {
                        if (Math3D.FastLineSegmentIntersectionXZ(walkMeshTriangle.originalVertices[v1], walkMeshTriangle.originalVertices[v2], this.ForceForVector(walkMeshTriangle), stepPosition[stepPosition.Count - 1]))
                        {
                            Vector3 item2 = (walkMeshTriangle.originalVertices[v1] + walkMeshTriangle.originalVertices[v2]) / 2f;
                            list4.Add(item2);
                            break;
                        }
                    }
                }
            }
            if (num2 + 1 <= pathsIdx.Count - 1)
            {
                for (Int32 l = 0; l < 3; l++)
                {
                    Int32 v1;
                    Int32 v2;
                    WalkMeshTriangle.GetVertexIdxForEdge(l, out v1, out v2);
                    if (walkMeshTriangle.neighborIdx[l] != -1)
                    {
                        if (Math3D.FastLineSegmentIntersectionXZ(walkMeshTriangle.originalVertices[v1], walkMeshTriangle.originalVertices[v2], this.ForceForVector(walkMeshTriangle), this.ForceForVector(this.walkMesh.tris[pathsIdx[num2 + 1]])))
                        {
                            Vector3 item3 = (walkMeshTriangle.originalVertices[v1] + walkMeshTriangle.originalVertices[v2]) / 2f;
                            list4.Add(item3);
                            break;
                        }
                    }
                }
            }
            for (Int32 m = 0; m < list4.Count; m++)
            {
                if (!stepPosition.Contains(list4[m]))
                {
                    list2[list2.Count - 1].Add(this.GetTriIdxAtPos(list4[m]));
                    stepPosition.Add(list4[m]);
                    list2.Add(new List<Int32>());
                }
            }
            vector = stepPosition[stepPosition.Count - 1];
            vector2 = vector;
            num = num2;
        }
        Int32 num6 = 0;
        while (i < pathsIdx.Count)
        {
            num6++;
            if (num6 >= pathsIdx.Count * 2)
            {
                return stepPosition;
            }
            if (this.IsTriangleVerticesAllEdges(this.walkMesh.tris[pathsIdx[i]]))
            {
                if (i - num2 > 2 && !stepPosition.Contains(vector2))
                {
                    list2[list2.Count - 1].Add(this.GetTriIdxAtPos(vector2));
                    stepPosition.Add(vector2);
                    list2.Add(new List<Int32>());
                }
                WalkMeshTriangle walkMeshTriangle2 = this.walkMesh.tris[pathsIdx[i]];
                List<Vector3> list5 = new List<Vector3>();
                List<Single> list6 = new List<Single>();
                if (stepPosition.Count > 0)
                {
                    for (Int32 n = 0; n < 3; n++)
                    {
                        Int32 v1;
                        Int32 v2;
                        WalkMeshTriangle.GetVertexIdxForEdge(n, out v1, out v2);
                        if (walkMeshTriangle2.neighborIdx[n] != -1)
                        {
                            if (Math3D.FastLineSegmentIntersectionXZ(walkMeshTriangle2.originalVertices[v1], walkMeshTriangle2.originalVertices[v2], this.ForceForVector(walkMeshTriangle2), stepPosition[stepPosition.Count - 1]))
                            {
                                Vector3 item4 = (walkMeshTriangle2.originalVertices[v1] + walkMeshTriangle2.originalVertices[v2]) / 2f;
                                list5.Add(item4);
                                list6.Add((walkMeshTriangle2.originalVertices[v1] - walkMeshTriangle2.originalVertices[v2]).magnitude);
                                break;
                            }
                        }
                    }
                }
                if (i + 1 <= pathsIdx.Count - 1)
                {
                    for (Int32 num9 = 0; num9 < 3; num9++)
                    {
                        Int32 v1;
                        Int32 v2;
                        WalkMeshTriangle.GetVertexIdxForEdge(num9, out v1, out v2);
                        if (walkMeshTriangle2.neighborIdx[num9] != -1)
                        {
                            if (Math3D.FastLineSegmentIntersectionXZ(walkMeshTriangle2.originalVertices[v1], walkMeshTriangle2.originalVertices[v2], this.ForceForVector(walkMeshTriangle2), this.ForceForVector(this.walkMesh.tris[pathsIdx[i + 1]])))
                            {
                                Vector3 vector3 = (walkMeshTriangle2.originalVertices[v1] + walkMeshTriangle2.originalVertices[v2]) / 2f;
                                if (list5.Count > 0)
                                {
                                    list6.Add((walkMeshTriangle2.originalVertices[v1] - walkMeshTriangle2.originalVertices[v2]).magnitude);
                                    if (list6[0] > list6[1] * 2f)
                                    {
                                        list5[0] = vector3;
                                    }
                                    else if (list6[1] <= list6[0] * 2f)
                                    {
                                        list5.Add(vector3);
                                    }
                                }
                                else
                                {
                                    for (Int32 num10 = 0; num10 < 3; num10++)
                                    {
                                        Int32 num11;
                                        Int32 num12;
                                        WalkMeshTriangle.GetVertexIdxForEdge(num10, out num11, out num12);
                                        if (num10 != num9)
                                        {
                                            if (walkMeshTriangle2.neighborIdx[num9] != -1)
                                            {
                                                if (Math3D.FastLineSegmentIntersectionXZ(walkMeshTriangle2.originalVertices[num11], walkMeshTriangle2.originalVertices[num12], this.ForceForVector(walkMeshTriangle2), this.ForceForVector(this.walkMesh.tris[pathsIdx[i - 1]])))
                                                {
                                                    list5.Add((walkMeshTriangle2.originalVertices[num11] + walkMeshTriangle2.originalVertices[num12]) / 2f);
                                                }
                                            }
                                        }
                                    }
                                    list5.Add(vector3);
                                }
                                break;
                            }
                        }
                    }
                }
                for (Int32 num13 = 0; num13 < list5.Count; num13++)
                {
                    if (!stepPosition.Contains(list5[num13]))
                    {
                        list2[list2.Count - 1].Add(this.GetTriIdxAtPos(list5[num13]));
                        stepPosition.Add(list5[num13]);
                        list2.Add(new List<Int32>());
                    }
                }
                num2 = i;
                vector = stepPosition[stepPosition.Count - 1];
                i = num2 + 1;
                vector2 = vector;
                num = num2;
            }
            else
            {
                WalkMeshTriangle walkMeshTriangle3 = this.walkMesh.tris[pathsIdx[i]];
                Vector3 vector4 = list3[i];
                if (i == pathsIdx.Count - 1)
                {
                    vector4 = end;
                }
                Boolean flag = true;
                for (Int32 num14 = num2; num14 <= i; num14++)
                {
                    WalkMeshTriangle walkMeshTriangle4 = this.walkMesh.tris[pathsIdx[num14]];
                    for (Int32 num15 = 0; num15 < 3; num15++)
                    {
                        Int32 num16;
                        Int32 num17;
                        WalkMeshTriangle.GetVertexIdxForEdge(num15, out num16, out num17);
                        if (walkMeshTriangle4.neighborIdx[num15] == -1)
                        {
                            if (Math3D.FastLineSegmentIntersectionXZ(vector, vector4, walkMeshTriangle4.originalVertices[num16], walkMeshTriangle4.originalVertices[num17]))
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        break;
                    }
                    if (!flag)
                    {
                        break;
                    }
                }
                if (flag)
                {
                    list2[list2.Count - 1].Add(this.GetTriIdxAtPos(vector4));
                    vector2 = vector4;
                    num = i;
                    i++;
                }
                else if (num2 == i - 1)
                {
                    if (!stepPosition.Contains(vector))
                    {
                        list2[list2.Count - 1].Add(this.GetTriIdxAtPos(vector));
                        stepPosition.Add(vector);
                        list2.Add(new List<Int32>());
                    }
                    if (!stepPosition.Contains(list3[i]))
                    {
                        list2[list2.Count - 1].Add(this.GetTriIdxAtPos(list3[i]));
                        stepPosition.Add(list3[i]);
                        list2.Add(new List<Int32>());
                    }
                    num2 = i;
                    vector = list3[i];
                    i = num2 + 1;
                    vector2 = vector;
                    num = num2;
                }
                else
                {
                    num2 = num;
                    vector = vector2;
                    if (!stepPosition.Contains(vector))
                    {
                        list2[list2.Count - 1].Add(this.GetTriIdxAtPos(vector));
                        stepPosition.Add(vector);
                        list2.Add(new List<Int32>());
                    }
                    i = num2 + 1;
                }
            }
        }
        if (stepPosition.Count > 1)
        {
            if (list3.IndexOf(stepPosition[1]) >= 0)
            {
                WalkMeshTriangle t = this.walkMesh.tris[pathsIdx[list3.IndexOf(stepPosition[1])]];
                if (!this.IsWalkableDepth0(this.walkMesh.tris[pathsIdx[0]], t, start, stepPosition[1]) && !stepPosition.Contains(vector))
                {
                    stepPosition.Insert(1, list3[0]);
                    list2.Insert(1, list2[0]);
                }
            }
            if (list3.IndexOf(stepPosition[stepPosition.Count - 1]) >= 0)
            {
                WalkMeshTriangle t2 = this.walkMesh.tris[pathsIdx[list3.IndexOf(stepPosition[stepPosition.Count - 1])]];
                if (!this.IsWalkableDepth0(this.walkMesh.tris[pathsIdx[pathsIdx.Count - 1]], t2, end, stepPosition[stepPosition.Count - 1]) && !stepPosition.Contains(vector))
                {
                    stepPosition.Add(list3[list3.Count - 1]);
                    list2.Add(list2[list2.Count - 1]);
                }
            }
        }
        stepPosition.Add(end);
        list2.Add(new List<Int32>());
        list2[list2.Count - 1].Add(pathsIdx[pathsIdx.Count - 1]);
        for (Int32 num18 = 0; num18 < stepPosition.Count; num18++)
        {
            if (this.GetTriIdxAtPos(stepPosition[num18]) >= 0)
            {
                stepPosition[num18] = this.ForceForVector(this.walkMesh.tris[this.GetTriIdxAtPos(stepPosition[num18])], stepPosition[num18]);
            }
        }
        Int32 num19 = 0;
        Int32 num20 = 2;
        for (Int32 num21 = 1; num21 < stepPosition.Count - 1; num21++)
        {
            if (num20 - num19 > 4)
            {
                num21 = num20;
                num19 = num20;
                num20 = num19 + 2;
            }
            else
            {
                Vector3 from = stepPosition[num21 - 1] - stepPosition[num21];
                Vector3 to = stepPosition[num21 + 1] - stepPosition[num21];
                Single num22 = Vector3.Angle(from, to);
                if (num22 < 105f)
                {
                    List<Int32> list7 = new List<Int32>();
                    list7.AddRange(list2[num21 - 1]);
                    list7.AddRange(list2[num21]);
                    for (Int32 num23 = list7.Count - 1; num23 >= 0; num23--)
                    {
                        if (list7[num23] == -1)
                        {
                            list7.RemoveAt(num23);
                        }
                    }
                    Vector3 vector5 = (stepPosition[num21 - 1] + stepPosition[num21]) / 2f;
                    Vector3 a = (stepPosition[num21 + 1] + stepPosition[num21]) / 2f;
                    vector5 = (a + stepPosition[num21 - 1]) / 2f;
                    a = (a + stepPosition[num21 + 1]) / 2f;
                }
                else
                {
                    num21 = num20 - 1;
                    num19 = num20 - 1;
                    num20 = num19 + 2;
                }
            }
        }
        for (Int32 num24 = 1; num24 < stepPosition.Count; num24++)
        {
            Single num25 = Vector3.Distance(stepPosition[num24 - 1], stepPosition[num24]);
            if (num25 < (Single)this.originalActor.speed * 0.95f)
            {
                stepPosition.RemoveAt(num24);
                num24--;
            }
        }
        for (Int32 num26 = 0; num26 < stepPosition.Count; num26++)
        {
            if (this.GetTriIdxAtPos(stepPosition[num26]) >= 0)
            {
                stepPosition[num26] = this.ForceForVector(this.walkMesh.tris[this.GetTriIdxAtPos(stepPosition[num26])], stepPosition[num26]);
            }
        }
        stepPosition.Reverse();
        return stepPosition;
    }

    public Int32 GetLadderFlag()
    {
        FF9Char ff9Char = FF9StateSystem.Common.FF9.charArray[(Int32)this.originalActor.uid];
        return (Int32)(ff9Char.attr & 4u);
    }

    public void ClearMoveTargetAndPath()
    {
        this.ClearMoveTarget();
        this.totalPathCount = 0;
        this.totalPathLengthSq = 0f;
    }

    public void LoadResources()
    {
        if (this.isPlayer)
        {
            if (this.questionMark == null)
            {
                this.questionMark = AssetManager.Load<Texture2D>("CommonAsset/EventIcons/balloon_question", false);
                this.exclamationMark = AssetManager.Load<Texture2D>("CommonAsset/EventIcons/balloon_exclamation", false);
                this.targetMark = AssetManager.Load<Texture2D>("CommonAsset/EventIcons/cursor_hand_here", false);
                this.warpMark = AssetManager.Load<Texture2D>("CommonAsset/EventIcons/cursor_warp", false);
            }
            this.targetMarkSize = Screen.height * 0.1f;
            this.npc = GameObject.Find("NPC mog")?.transform;
            this.aspect = UnityEngine.Object.FindObjectOfType<PSXCameraAspect>();
        }
    }

    public FieldMap fieldMap;

    public WalkMesh walkMesh;

    public FieldMapActor actor;

    public PSXCameraAspect aspect;

    public Int32 activeTri;

    public Int32 activeFloor;

    public Int32 lastTri;

    public Int32 lastFloor;

    public Single speed;

    public Single radius;

    public Boolean isPlayer;

    public UInt16 charFlags;

    public Vector3 curPos;

    public Vector3 lastPos;

    public Vector3 curPosBeforeAttach;

    public Vector3 localScaleBeforeAttach;

    public Vector3 moveVec;

    public Int32 stillCount;

    public Vector3 stillPos;

    public Vector3 moveTarget;

    public Boolean hasTarget;

    public List<Vector3> movePaths;

    public List<Vector3> debugPaths;

    public List<Vector3> debugSmoothPaths;

    public Int32 totalPathCount;

    public Single totalPathLengthSq;

    private List<Vector3> forces;

    private List<Boolean> forcesFlag;

    private List<Vector3> forcesOrigin;

    private List<Boolean> forcesType;

    private List<Int32> visited;

    private Boolean isMoving;

    private Boolean isRunning;

    private Boolean wasRunning = false;

    private Single cTime;

    private bool analogControlEnabled;

    private float stickThreshold;

    private float minimumSpeed;

    private Transform model;

    public Actor originalActor;

    public static Boolean ccSMoveKey;

    public Texture2D questionMark;

    public Texture2D exclamationMark;

    public Texture2D targetMark;

    public Texture2D warpMark;

    private Single targetMarkSize;

    private Single cumulativeTime;

    private Single amplitude;

    private Transform npc;

    private Boolean hasTalkBalloon;

    private Rect talkBalloonRect;

    private List<Rect> warpRects;

    private Animation animation;

    private Boolean isActive;

    private List<Int32> foundTris;

    private List<Int32> adjacentActiveTris;

    public static Int32 checkedSID = 11;

    private Int32 moveFrame;
}

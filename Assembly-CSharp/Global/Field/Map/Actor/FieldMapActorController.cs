using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common;
using FF9;
using UnityEngine;
using Memoria;
using Object = System.Object;

public class FieldMapActorController : HonoBehavior
{
	public Boolean IsActive()
	{
		return this.isActive;
	}

	public Int32 MoveFrame
	{
		get
		{
			return this.moveFrame;
		}
		set
		{
			this.moveFrame = value;
		}
	}

	public void SetActive(Boolean active)
	{
		this.isActive = active;
		if (this.isPlayer)
		{
			PersistenSingleton<HonoInputManager>.Instance.SetVirtualAnalogEnable(active);
		}
	}

	public Boolean SetPosition(Vector3 pos, Boolean updateLastPos, Boolean needCheckTri = true)
	{
		if (FF9StateSystem.Common.FF9.fldMapNo == 70)
		{
			return true;
		}
		if ((this.charFlags & 1) == 0 || !needCheckTri)
		{
			this.curPos = pos;
			if (updateLastPos)
			{
				this.lastPos = this.curPos;
			}
			this.SyncPosToTransform();
			return true;
		}
		Int32 triIdxAtPos = this.GetTriIdxAtPos(pos);
		if (triIdxAtPos != -1)
		{
			WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[triIdxAtPos];
			Vector3 vector = Math3D.CalculateBarycentricRatioXZ(pos, walkMeshTriangle.originalVertices[0], walkMeshTriangle.originalVertices[1], walkMeshTriangle.originalVertices[2]);
			Vector3 vector2 = walkMeshTriangle.originalVertices[0] * vector.x + walkMeshTriangle.originalVertices[1] * vector.y + walkMeshTriangle.originalVertices[2] * vector.z;
			this.curPos = vector2;
			if (updateLastPos)
			{
				this.lastPos = this.curPos;
			}
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
        this.stickThreshold = Configuration.AnalogControl.StickThreshold/100.0f;
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
		{
			this.PlayAnimationViaEventScript();
		}
		if (!this.isActive)
		{
			this.SyncPosToTransform();
			if (this.isPlayer)
			{
				Int32 ladderFlag = this.GetLadderFlag();
				if (ladderFlag != 0)
				{
					PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, (Action)null);
				}
			}
			this.ClearMoveTarget();
			if (PersistenSingleton<UIManager>.Instance.IsMenuControlEnable || PersistenSingleton<UIManager>.Instance.IsWarningDialogEnable)
			{
				return;
			}
		}
		else if (!EventInput.IsMovementControl && this.isPlayer)
		{
			PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(false, (Action)null);
			return;
		}
		this.cumulativeTime += Time.deltaTime;
		if (this.isPlayer)
		{
			if (!FF9StateSystem.Field.isDebug)
			{
				Int32 ladderFlag2 = this.GetLadderFlag();
				if (PersistenSingleton<EventEngine>.Instance.GetUserControl() || ladderFlag2 != 0)
				{
					if (EventInput.IsMovementControl && VirtualAnalog.GetTap())
					{
						this.CheckPosInProjectedWalkMesh();
					}
					if (ladderFlag2 != 0)
					{
						PersistenSingleton<UIManager>.Instance.SetPlayerControlEnable(true, (Action)null);
					}
				}
			}
			else if (EventInput.IsMovementControl && VirtualAnalog.GetTap())
			{
				this.CheckPosInProjectedWalkMesh();
			}
		}
		this.isRunning = false;
		Single num = this.radius * this.radius * 9f * 9f;
		Boolean flag = PersistenSingleton<EventEngine>.Instance.GetDashInh() == 1;

	    if (this.isPlayer && ((UIManager.Input.GetKey(Control.Cancel) ^ FF9StateSystem.Settings.cfg.move == 1UL) || !analogControlEnabled && VirtualAnalog.GetMagnitudeRatio() > 0.95f || this.totalPathLengthSq > num) && !flag)
	    {
	        this.isRunning = true;
	        Int32 num2 = 2;
	        for (Int32 i = 0; i < num2; i++)
	        {
	            this.UpdateMovement();
	        }
	    }
	    else
	    {
	        this.UpdateMovement();
	    }
	    if (this.isPlayer && PersistenSingleton<EventEngine>.Instance.GetUserControl())
		{
			if (this.isRunning)
			{
				this.originalActor.speed = 60;
			}
			else
			{
				this.originalActor.speed = 30;
			}
		}
		if (FF9StateSystem.Common.FF9.fldMapNo == 1204 && this.originalActor.sid == 11)
		{
			GameObject gameObject = GameObject.Find("marker1");
			if (gameObject == (UnityEngine.Object)null)
			{
				gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				gameObject.name = "marker1";
				gameObject.transform.position = new Vector3(-3353f, 0f, 1914f);
				gameObject.transform.localScale = new Vector3(100f, 1f, 100f);
				this.DebugSetMarker(gameObject);
			}
			GameObject gameObject2 = GameObject.Find("marker2");
			if (gameObject2 == (UnityEngine.Object)null)
			{
				gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				gameObject2.name = "marker2";
				gameObject2.transform.position = new Vector3(-2862f, 0f, 2368f);
				gameObject2.transform.localScale = new Vector3(100f, 1f, 100f);
				this.DebugSetMarker(gameObject2);
			}
			GameObject gameObject3 = GameObject.Find("marker3");
			if (gameObject3 == (UnityEngine.Object)null)
			{
				gameObject3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				gameObject3.name = "marker3";
				gameObject3.transform.position = new Vector3(-2001f, 0f, 3080f);
				gameObject3.transform.localScale = new Vector3(100f, 1f, 100f);
				this.DebugSetMarker(gameObject3);
			}
			GameObject gameObject4 = GameObject.Find("marker4");
			if (gameObject4 == (UnityEngine.Object)null)
			{
				gameObject4 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				gameObject4.name = "marker4";
				gameObject4.transform.position = new Vector3(-1572f, 0f, 3215f);
				gameObject4.transform.localScale = new Vector3(100f, 1f, 100f);
				this.DebugSetMarker(gameObject4);
			}
			GameObject gameObject5 = GameObject.Find("marker5");
			if (gameObject5 == (UnityEngine.Object)null)
			{
				gameObject5 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				gameObject5.name = "marker5";
				gameObject5.transform.position = new Vector3(-1054f, 0f, 3481f);
				gameObject5.transform.localScale = new Vector3(100f, 1f, 100f);
				this.DebugSetMarker(gameObject5);
			}
		}
	}

	public override void HonoLateUpdate()
	{
		this.SyncPosToTransform();
		this.originalActor.pos[0] = this.curPos.x;
		this.originalActor.pos[1] = this.curPos.y;
		this.originalActor.pos[2] = this.curPos.z;
		Vector3 vector = this.lastPos - this.curPos;
		Vector3 vector2 = this.curPos - this.stillPos;
		if (vector.sqrMagnitude == 0f || (this.stillCount == 0 && vector2.sqrMagnitude < 10f))
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
		{
			return;
		}
		if (!this.isActive)
		{
			return;
		}
		if (PersistenSingleton<SceneDirector>.Instance.IsFading)
		{
			return;
		}
		Camera mainCamera = this.fieldMap.GetMainCamera();
		BGCAM_DEF currentBgCamera = this.fieldMap.GetCurrentBgCamera();
		if (this.totalPathCount > 0)
		{
			Vector3 a;
			if (this.movePaths.Count > 0)
			{
				a = this.movePaths[0];
			}
			else
			{
				a = this.moveTarget;
			}
			Vector3 position = PSX.CalculateGTE_RTPT(a + new Vector3(0f, this.amplitude, 0f), Matrix4x4.identity, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), this.fieldMap.GetProjectionOffset());
			position = mainCamera.WorldToScreenPoint(position);
			position.y = (Single)Screen.height - position.y;
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

    public void UpdateMovement()
    {
        String name = base.name;
        this.cTime = 0f;
        this.isMoving = false;
        if (this.isPlayer)
        {
            if (PersistenSingleton<EventEngine>.Instance.GetUserControl() || FF9StateSystem.Common.FF9.fldMapNo == 1751 || FF9StateSystem.Common.FF9.fldMapNo == 404 || (FF9StateSystem.Common.FF9.fldMapNo == 205 && this.originalActor.sid == 16) || (FF9StateSystem.Common.FF9.fldMapNo == 2150 && this.originalActor.sid == 13) || (FF9StateSystem.Common.FF9.fldMapNo == 900 && this.originalActor.sid == 13))
            {
                this.CopyLastPos();
            }
            this.MovePC();
            if (FF9StateSystem.Common.FF9.fldMapNo == 916 && this.originalActor.uid == 10 && this.curPos.z > -15900f && this.curPos.z < -15700f && this.curPos.x < 130f)
            {
                this.curPos.x = 130f;
            }
            if (FF9StateSystem.Common.FF9.fldMapNo == 1005 && this.originalActor.uid == 7)
            {
                if (this.curPos.z > -1000f && this.curPos.z < -920f && this.curPos.x < -1060f)
                {
                    this.curPos.x = -1060f;
                }
                else if (this.curPos.x > -1400f && this.curPos.x < -1350f && this.curPos.z < -610f)
                {
                    this.curPos.z = -610f;
                }
            }
        }
        else
        {
            if ((FF9StateSystem.Common.FF9.fldMapNo == 2050 && this.originalActor.sid == 5) || (FF9StateSystem.Common.FF9.fldMapNo == 350 && this.originalActor.sid == 11) || (FF9StateSystem.Common.FF9.fldMapNo == 1315 && this.originalActor.sid == 12))
            {
                this.CopyLastPos();
            }
            this.MoveNPC();
        }
        Int32 num = 0;
        Vector3 vector = this.curPos;
        Vector3 vector2 = this.lastPos;
        Vector3 vector3 = this.curPos - this.lastPos;
        if ((this.charFlags & 1) != 0)
        {
            num = this.ServiceChar();
        }
        this.isMoving = ((num & 1) != 0);
        if (FF9StateSystem.Field.isDebug && VirtualAnalog.GetAnalogValue().magnitude > 0.1f)
        {
            this.isMoving = true;
        }
        if ((this.charFlags & 1) != 0)
        {
            this.UpdateActiveTri();
        }
        if (FF9StateSystem.Field.isDebug)
        {
            if (this.animation.IsPlaying(FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.run)) || this.animation.IsPlaying(FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.walk)) || this.animation.IsPlaying(FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.idle)))
            {
                if (this.isMoving || this.totalPathCount > 0)
                {
                    if (this.isRunning)
                    {
                        if (!this.animation.IsPlaying(FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.run)) && this.animation.GetClip(FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.run)) != (UnityEngine.Object)null)
                        {
                            this.animation.Play(FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.run));
                        }
                    }
                    else if (!this.animation.IsPlaying(FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.walk)) && this.animation.GetClip(FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.walk)) != (UnityEngine.Object)null)
                    {
                        this.animation.Play(FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.walk));
                    }
                }
                else if (!this.animation.IsPlaying(FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.idle)))
                {
                    String name2 = FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.idle);
                    if (this.animation.GetClip(name2) != (UnityEngine.Object)null)
                    {
                        this.animation.Play(FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.idle));
                    }
                }
            }
            else if (!this.animation.isPlaying)
            {
                String name3 = FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.idle);
                if (this.animation.GetClip(name3) != (UnityEngine.Object)null)
                {
                    this.animation.Play(FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.idle));
                }
            }
        }
        this.lastFloor = this.activeFloor;
        this.lastTri = this.activeTri;
        this.SyncPosToTransform();
    }

    private void PlayAnimationViaEventScript()
	{
		if (this.originalActor.sid == 17 && this.animation.isPlaying)
		{
			foreach (Object obj in this.animation)
			{
				AnimationState animationState = (AnimationState)obj;
				if (this.animation.IsPlaying(animationState.name))
				{
					break;
				}
			}
		}
		String name = FF9DBAll.AnimationDB.GetValue((Int32)this.originalActor.anim);
		AnimationClip clip = this.animation.GetClip(name);
		if (!this.animation.IsPlaying(name))
		{
			if (clip != (UnityEngine.Object)null)
			{
				if (FF9StateSystem.Common.FF9.fldMapNo == 3010 && this.originalActor.sid == 8)
				{
					return;
				}
				this.animation.clip = clip;
				this.animation.Play(name);
				this.animation[name].speed = 0f;
				Single time = (Single)this.originalActor.animFrame / (Single)this.originalActor.frameN * this.animation[name].length;
				this.animation[name].time = time;
				this.animation.Sample();
				if (this.originalActor.frameN == 1 && base.IsVisibled())
				{
					this.animation.Stop();
				}
			}
		}
		else
		{
			this.animation[name].speed = 0f;
			Single time2 = (Single)this.originalActor.animFrame / (Single)this.originalActor.frameN * this.animation[name].length;
			this.animation[name].time = time2;
			this.animation.Sample();
		}
	}

	private void LateUpdate()
	{
		if (this.actor != (UnityEngine.Object)null && this.actor.actor != null)
		{
			if ((this.actor.actor.geo_struct_flags & geo.GEO_FLAGS_LOOK) != 0)
			{
				this.UpdateNeck();
			}
			else if (FF9StateSystem.Common.FF9.fldMapNo == 576 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 3140 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR) == 100 && this.originalActor.model == 270)
			{
				Transform childByName = base.transform.GetChildByName("bone" + this.actor.actor.neckBoneIndex.ToString("D3"));
				this.ApplyNeckRotation(childByName, 0f, -149f);
			}
		}
		if (this.originalActor.parent != null && ((FF9StateSystem.Common.FF9.fldMapNo == 2950 && this.originalActor.sid == 25) || (FF9StateSystem.Common.FF9.fldMapNo == 2951 && this.originalActor.sid == 29) || (FF9StateSystem.Common.FF9.fldMapNo == 2952 && this.originalActor.sid == 23) || (FF9StateSystem.Common.FF9.fldMapNo == 2954 && this.originalActor.sid == 26) || (FF9StateSystem.Common.FF9.fldMapNo == 2955 && this.originalActor.sid == 28)))
		{
			this.PretendChocoboOffset();
		}
		this.CheckOffsetPosModel();
	}

	private void PretendChocoboOffset()
	{
		this.SetPosition(this.originalActor.parent.fieldMapActorController.transform.position, true, false);
		Transform transform = this.originalActor.parent.go.transform.FindChild("bone000");
		Transform transform2 = this.originalActor.go.transform.FindChild("bone000");
		transform2.position = transform.position + new Vector3(0f, 105f, 0f);
	}

	private void CheckOffsetPosModel()
	{
		if (this.originalActor.model == 200)
		{
			this.originalActor.go.transform.FindChild("bone000").Translate(Vector3.forward * 650f);
		}
		if (this.originalActor.model == 294)
		{
			this.originalActor.go.transform.FindChild("bone000").Translate(Vector3.back * 240f);
		}
		if (this.originalActor.model == 306)
		{
			this.originalActor.go.transform.FindChild("bone000").Translate(Vector3.back * 190f);
		}
		if (this.originalActor.model == 22 && FF9StateSystem.Common.FF9.fldMapNo == 116)
		{
			this.originalActor.go.transform.FindChild("bone000").Translate(Vector3.up * 40f);
			this.originalActor.go.transform.FindChild("bone000").Translate(Vector3.forward * 25f);
		}
		if (this.originalActor.model == 488)
		{
			if (this.originalActor.go.transform.localScale.y <= -1f)
			{
				this.originalActor.go.transform.FindChild("bone000").Translate(Vector3.left * 220f);
			}
			else if (this.originalActor.go.transform.localScale.y >= 1f)
			{
				this.originalActor.go.transform.FindChild("bone000").Translate(Vector3.right * 130f);
			}
		}
		if (FF9StateSystem.Common.FF9.fldMapNo == 1606 && (this.originalActor.model == 235 || this.originalActor.model == 236 || this.originalActor.model == 237))
		{
			this.originalActor.go.transform.FindChild("bone000").Translate(Vector3.forward * -20f);
		}
		if (this.originalActor.anim == this.originalActor.turnr || this.originalActor.anim == this.originalActor.turnl)
		{
			Single t = (Single)this.originalActor.animFrame / (Single)(this.originalActor.frameN - 1);
			Transform transform = this.originalActor.go.transform.FindChild("bone000");
			Vector3 b = Vector3.Lerp(Vector3.zero, this.originalActor.offsetTurn, t);
			transform.position += b;
		}
	}

	private void UpdateNeck()
	{
		Vector3 a = new Vector3(this.actor.actor.xl, this.actor.actor.yl, this.actor.actor.zl);
		Transform childByName = base.transform.GetChildByName("bone" + this.actor.actor.neckBoneIndex.ToString("D3"));
		if (childByName == (UnityEngine.Object)null)
		{
			return;
		}
		Vector3 a2 = a - childByName.position;
		a2 *= -1f;
		Single y = Vector2.Distance(Vector2.zero, new Vector2(a2.x, a2.z));
		Single num = Mathf.Atan2(a2.x, a2.z) * 57.29578f;
		Single num2 = Mathf.Atan2(y, -a2.y) * 57.29578f;
		Single num3 = base.transform.eulerAngles.y;
		if ((this.actor.actor.flags & 128) != 0)
		{
			num3 = this.actor.actor.trot;
		}
		Single rotY = num - num3;
		Single num4 = num2 - 90f;
		if (this.actor.actor.anim == 2692)
		{
			childByName.rotation = Quaternion.AngleAxis(num4, Vector3.up) * childByName.rotation;
			childByName.localRotation *= Quaternion.AngleAxis(-90f, Vector3.left);
		}
		else if (this.actor.actor.anim == 1324)
		{
			childByName.rotation = Quaternion.AngleAxis(-40f, Vector3.down) * childByName.rotation;
			childByName.localRotation *= Quaternion.AngleAxis(-65f, Vector3.right);
		}
		else if (FF9StateSystem.Common.FF9.fldMapNo == 1610 && this.originalActor.sid == 19 && (this.actor.actor.anim == 6126 || this.actor.actor.anim == 6130))
		{
			childByName.rotation = Quaternion.AngleAxis(80f, Vector3.down) * childByName.rotation;
			childByName.localRotation *= Quaternion.AngleAxis(-10f, Vector3.right);
		}
		else if (this.actor.actor.anim == 3802)
		{
			Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
			Int32 varManually = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
			Int32 varManually2 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR);
			if (fldMapNo == 1601 && this.originalActor.uid == 23 && varManually == 6600 && varManually2 == 5)
			{
				this.ApplyNeckRotation(childByName, 23f, rotY);
			}
			else
			{
				this.ApplyNeckRotation(childByName, num4, rotY);
			}
		}
		// TODO Check Native: #147
		else if (this.actor.actor.anim == 4630 || this.actor.actor.anim == 12938 || this.actor.actor.anim == 4636)
		{
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
		{
			return;
		}
		if (!FF9StateSystem.Field.isDebug && this.originalActor.state != EventEngine.stateRunning)
		{
			return;
		}

        //This flag was set to false, causing the game to register digital movement unless on android.
        Boolean flag = false;
		Vector2 vector = Vector2.zero;
		if (FF9StateSystem.MobilePlatform && VirtualAnalog.HasInput())
		{
		    vector = VirtualAnalog.GetAnalogValue();
		    flag = true;
        }
		else if (analogControlEnabled)
	    {
	        vector = PersistenSingleton<HonoInputManager>.Instance.GetAxis();
	        flag = true;
	    }

	    if (flag)
	    {
	        flag = (Mathf.Abs(vector.x) >= 0.1f || Mathf.Abs(vector.y) >= 0.1f);
	    }
	    else
	    {
	        vector = PersistenSingleton<HonoInputManager>.Instance.GetAxis();
        }

	    Boolean flag2 = UIManager.Input.GetKey(Control.Up) || vector.y > 0.1f;
        Boolean flag3 = UIManager.Input.GetKey(Control.Down) || vector.y < -0.1f;
        Boolean flag4 = UIManager.Input.GetKey(Control.Left) || vector.x < -0.1f;
        Boolean flag5 = UIManager.Input.GetKey(Control.Right) || vector.x > 0.1f;
        if (!FF9StateSystem.Field.isDebug)
		{
			flag2 &= PersistenSingleton<EventEngine>.Instance.GetUserControl();
			flag3 &= PersistenSingleton<EventEngine>.Instance.GetUserControl();
			flag4 &= PersistenSingleton<EventEngine>.Instance.GetUserControl();
			flag5 &= PersistenSingleton<EventEngine>.Instance.GetUserControl();
		}
		if (flag2 || flag3 || flag4 || flag5)
		{
			this.ClearMoveTargetAndPath();
		}
		if (this.isPlayer)
		{
			FieldMapActorController.ccSMoveKey = (flag2 || flag3 || flag4 || flag5 || this.hasTarget);
		}
		Single num = this.radius * this.radius * 0.95f * 0.95f;
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
			Vector3 vector2 = this.moveTarget - this.curPos;
			vector2.y = 0f;
			Single sqrMagnitude = vector2.sqrMagnitude;
			if (this.movePaths.Count == 0)
			{
				num = this.radius * this.radius;
			}
			if (sqrMagnitude <= num)
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
			this.moveVec = vector2.normalized;
		}
		else
		{
			this.moveVec = Vector3.zero;
		}
		if (flag2 || flag3 || flag4 || flag5)
		{
			this.moveVec = Vector3.zero;
			if (flag)
			{      
				this.moveVec = new Vector3(vector.x, 0f, vector.y);
			}
            else
            {
                if (this.moveVec.x < 0f || flag4)
                {
                    this.moveVec.x = this.moveVec.x - this.speed;
                }
                if (this.moveVec.x > 0f || flag5)
                {
                    this.moveVec.x = this.moveVec.x + this.speed;
                }
                if (this.moveVec.z > 0f || flag2)
                {
                    this.moveVec.z = this.moveVec.z + this.speed;
                }
                if (this.moveVec.z < 0f || flag3)
                {
                    this.moveVec.z = this.moveVec.z - this.speed;
                }
            }
           // Removing this statement allows for variable movement speed.
            if (!analogControlEnabled)
            {
                this.moveVec.Normalize();
            }
            Single y = FF9StateSystem.Field.twist.y;
			if (analogControlEnabled && Configuration.AnalogControl.UseAbsoluteOrientation)
			{
				y = FF9StateSystem.Field.twist.x;
			}
			Quaternion rotation = Quaternion.Euler(0f, y, 0f);
			this.moveVec = rotation * this.moveVec;
		}

        Vector3 b = this.moveVec * this.speed;

        //This block sets correct state and speed based on stick magnitude.
        if (analogControlEnabled)
        {

            float currentSpeed = 0.0f;
            Boolean dashInhibited = PersistenSingleton<EventEngine>.Instance.GetDashInh() == 1;
            if (moveVec.magnitude > 0.5 && (UIManager.Input.GetKey(Control.Cancel) ^ FF9StateSystem.Settings.cfg.move == 1UL) && !dashInhibited)
            {
                this.isRunning = true;
                currentSpeed = Mathf.Lerp(this.minimumSpeed / 2.0f, this.speed, moveVec.magnitude);
            }
            else
            {
                this.isRunning = false;
                currentSpeed = Mathf.Lerp(this.minimumSpeed, this.speed, moveVec.magnitude);
            }
            b = this.moveVec.normalized * currentSpeed;
            //Inihibit movement if below threshold
            if (this.moveVec.magnitude <= stickThreshold)
            {
                b = Vector3.zero;
            }
        }
        this.curPos += b;
		EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        //Prevent rotation if below threshold
        if (flag2 || flag3 || flag4 || flag5 || this.hasTarget && b != Vector3.zero)
		{
			Single num2 = Mathf.Atan2(-this.moveVec.x, -this.moveVec.z) * 57.29578f;
			Single num3 = Mathf.Lerp(this.actor.actor.rotAngle[1], num2, 0.4f);
			if (Mathf.Abs(num2 - this.actor.actor.rotAngle[1]) > 180f)
			{
				if (num2 > this.actor.actor.rotAngle[1])
				{
					num3 += 180f;
				}
				else
				{
					num3 -= 180f;
				}
			}
			if (num3 > 180f)
			{
				num3 -= 360f;
			}
			else if (num3 < -180f)
			{
				num3 += 360f;
			}
			this.actor.actor.rotAngle[1] = num3;
			Single num4;
			PosObj posObj = this.walkMesh.Collision(this, 0, out num4);
			if (posObj != null)
			{
				instance.sLockFree = (Int64)(((posObj.flags & 16) != 0) ? 0L : 1L);
				if (instance.sLockFree == 0L)
				{
					instance.sLockTimer = 0L;
				}
			}
			if (posObj != null && num4 <= 0f && instance.sLockTimer >= 0L)
			{
				this.originalActor.coll = posObj;
				this.originalActor.colldist = num4;
				Vector3 vector3 = new Vector3(posObj.pos[0], posObj.pos[1], posObj.pos[2]);
				Int32 fixedPointAngle = PersistenSingleton<EventEngine>.Instance.eBin.CollisionAngle(this.actor.actor, posObj, this.actor.actor.rotAngle[1]);
				Single degree = EventEngineUtils.ConvertFixedPointAngleToDegree((Int16)fixedPointAngle);
				if (degree >= -90f && degree <= 90f)
				{
					Single num6 = this.radius;
					Single num7 = (Single)(4 * posObj.collRad);
					Single num8 = num6 + num7;
					Single magnitude = (this.curPos - vector3).magnitude;
					Vector3 b2 = vector3;
					Vector3 normalized = (this.curPos - b2).normalized;
					Single d = num8 - magnitude;
					Vector3 b3 = normalized * d;
					this.curPos += b3;
					magnitude = (this.curPos - vector3).magnitude;
					posObj = this.walkMesh.Collision(this, 0, out num4);
					if (posObj != null && num4 < 0f)
					{
						this.curPos -= b3;
						this.curPos -= b;
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
		if (FF9StateSystem.Common.FF9.fldMapNo == 2207 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(220) == 9840)
		{
			return;
		}
		EventEngine instance = PersistenSingleton<EventEngine>.Instance;
		if (instance.SCollTimer != 0)
		{
			if (instance.sLockTimer >= 25L)
			{
				instance.sLockTimer = -25L;
			}
			else
			{
				instance.sLockTimer += instance.sLockFree;
			}
		}
		else if (instance.sLockTimer >= 0L)
		{
			instance.sLockTimer = 0L;
		}
		else
		{
			instance.sLockTimer += 1L;
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
		{
			marker.transform.position = zero;
		}
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
			Vector3 vector = Math3D.CalculateBarycentricRatioXZ(pos, walkMeshTriangle.originalVertices[0], walkMeshTriangle.originalVertices[1], walkMeshTriangle.originalVertices[2]);
			Vector3 vector2 = walkMeshTriangle.originalVertices[0] * vector.x + walkMeshTriangle.originalVertices[1] * vector.y + walkMeshTriangle.originalVertices[2] * vector.z;
			vector2.y += (Single)(this.fieldMap.bgi.floorList[walkMeshTriangle.floorIdx].curPos.coord[1] - this.fieldMap.bgi.floorList[walkMeshTriangle.floorIdx].orgPos.coord[1]);
			output = pos;
			output.y = vector2.y;
			return true;
		}
		return false;
	}

	private void MoveNPC()
	{
		Single num = this.radius * this.radius * 0.95f * 0.95f;
		if (this.movePaths.Count > 0 && !this.hasTarget)
		{
			this.hasTarget = true;
			Int32 index = this.movePaths.Count - 1;
			this.moveTarget = this.movePaths[index];
			this.movePaths.RemoveAt(index);
		}
		if (this.hasTarget)
		{
			Vector3 vector = this.moveTarget - this.curPos;
			vector.y = 0f;
			Single sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude <= num)
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
			this.moveVec = vector.normalized;
			this.actor.actor.rotAngle[1] = Mathf.Atan2(-this.moveVec.x, -this.moveVec.z) * 57.29578f;
		}
		else
		{
			this.moveVec = Vector3.zero;
		}
		Vector3 b = this.moveVec;
		b = this.moveVec * this.speed;
		this.curPos += b;
	}

	public void SetTwist(Int32 twistA, Int32 twistD)
	{
	}

	private void UpdateActiveTri()
	{
		if ((this.charFlags & 1) == 0)
		{
			return;
		}
		if ((FF9StateSystem.Common.FF9.fldMapNo == 2504 && this.originalActor.sid == 14) || (FF9StateSystem.Common.FF9.fldMapNo == 105 && this.originalActor.sid == 4) || (FF9StateSystem.Common.FF9.fldMapNo == 2605 && this.originalActor.sid == 11))
		{
			return;
		}
		Int32 activeTriIdxAtPos = this.GetActiveTriIdxAtPos(this.curPos);
		if (activeTriIdxAtPos != -1)
		{
			WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[activeTriIdxAtPos];
			Vector3 vector = Math3D.CalculateBarycentricRatioXZ(this.curPos, walkMeshTriangle.originalVertices[0], walkMeshTriangle.originalVertices[1], walkMeshTriangle.originalVertices[2]);
			Vector3 vector2 = walkMeshTriangle.originalVertices[0] * vector.x + walkMeshTriangle.originalVertices[1] * vector.y + walkMeshTriangle.originalVertices[2] * vector.z;
			vector2.y += (Single)(this.fieldMap.bgi.floorList[walkMeshTriangle.floorIdx].curPos.coord[1] - this.fieldMap.bgi.floorList[walkMeshTriangle.floorIdx].orgPos.coord[1]);
			this.curPos = vector2;
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
		Int32 num = 2;
		BGI_DEF bgi = this.fieldMap.bgi;
		Vector3 cPos = this.curPos;
		Vector3 vector = this.lastPos;
		Vector3 vector2 = this.curPos - this.lastPos;
		this.ShowDebugRadius();
		if ((Int32)this.originalActor.sid == FieldMapActorController.checkedSID)
		{
		}
		if (Mathf.Approximately(vector2.magnitude, 0f))
		{
			return num;
		}
		if (this.activeFloor != -1 && this.activeTri != -1)
		{
			Int32 index = this.activeTri;
			BGI_TRI_DEF bgi_TRI_DEF = bgi.triList[index];
			if (this.walkMesh.BGI_traverseTriangles(this, ref bgi_TRI_DEF, vector, cPos, 0, 0f, 0f))
			{
				this.activeTri = index;
				if (this.activeTri >= 0)
				{
					this.activeFloor = (Int32)bgi.triList[this.activeTri].floorNdx;
				}
				else
				{
					this.activeFloor = -1;
				}
				this.curPos = this.lastPos;
				num |= 1;
				if (this.lastTri >= 0 && !this.walkMesh.BGI_pointInPoly(bgi.floorList[this.lastFloor], bgi.triList[this.lastTri], vector))
				{
					this.lastFloor = this.activeFloor;
					this.lastTri = this.activeTri;
				}
			}
			Int32 num2 = 0;
			if (!this.IsRadiusValid(ref cPos, this.activeTri, ref num2))
			{
				if (!this.hasTarget)
				{
				}
				if (!this.ServiceForces(ref cPos, this.activeTri, ref num2))
				{
					this.activeTri = index;
					if (this.activeTri >= 0)
					{
						this.activeFloor = (Int32)bgi.triList[this.activeTri].floorNdx;
					}
					else
					{
						this.activeFloor = -1;
					}
					this.curPos = this.lastPos;
					num |= 1;
					if (this.lastTri >= 0 && this.lastFloor >= 0 && !this.walkMesh.BGI_pointInPoly(bgi.floorList[this.lastFloor], bgi.triList[this.lastTri], vector))
					{
						this.lastFloor = this.activeFloor;
						this.lastTri = this.activeTri;
					}
					return num;
				}
				num |= 9;
			}
			if (cPos.x != this.curPos.x || cPos.z != this.curPos.z)
			{
				this.curPos.x = cPos.x;
				this.curPos.z = cPos.z;
				num |= 16;
			}
			this.lastTri = index;
			if (this.lastTri >= 0)
			{
				this.lastFloor = (Int32)bgi.triList[this.lastTri].floorNdx;
			}
			else
			{
				this.lastFloor = -1;
			}
			return num;
		}
		if (this.lastFloor != -1 || this.lastTri != -1)
		{
			this.ResetPos();
			return num;
		}
		this.lastPos = this.curPos;
		return num | 1;
	}

	private void ShowDebugRadius()
	{
		for (Int32 i = 0; i < 10; i++)
		{
			Vector3 start = this.curPos;
			Vector3 a = this.curPos;
			Vector3 vector = new Vector3((Single)i, 0f, (Single)(9 - i));
			global::Debug.DrawLine(start, a + vector.normalized * this.radius, Color.yellow, 0.5f, true);
			Vector3 start2 = this.curPos;
			Vector3 a2 = this.curPos;
			Vector3 vector2 = new Vector3((Single)(-(Single)i), 0f, (Single)(9 - i));
			global::Debug.DrawLine(start2, a2 + vector2.normalized * this.radius, Color.yellow, 0.5f, true);
			Vector3 start3 = this.curPos;
			Vector3 a3 = this.curPos;
			Vector3 vector3 = new Vector3((Single)i, 0f, (Single)(-9 + i));
			global::Debug.DrawLine(start3, a3 + vector3.normalized * this.radius, Color.yellow, 0.5f, true);
			Vector3 start4 = this.curPos;
			Vector3 a4 = this.curPos;
			Vector3 vector4 = new Vector3((Single)(-(Single)i), 0f, (Single)(-9 + i));
			global::Debug.DrawLine(start4, a4 + vector4.normalized * this.radius, Color.yellow, 0.5f, true);
		}
	}

	private Boolean IsRadiusValid(ref Vector3 cPos, Int32 triNdx, ref Int32 hitEdge)
	{
		if (triNdx == -1)
		{
			return false;
		}
		if (this.walkMesh.tris[triNdx].triFlags == 0)
		{
			triNdx = this.lastTri;
		}
		if (triNdx < 0)
		{
			return false;
		}
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
			for (Int32 i = 0; i < 3; i++)
			{
				Int32 index2;
				Int32 index = index2 = i;
				Single num = result[index2];
				result[index] = num + (Single)(this.fieldMap.bgi.floorList[walkMeshTriangle.floorIdx].curPos.coord[i] - this.fieldMap.bgi.floorList[walkMeshTriangle.floorIdx].orgPos.coord[i]);
			}
		}
		return result;
	}

	private Boolean RadiusValid(ref Vector3 cPos, Int32 triNdx, ref Int32 hitEdge)
	{
		WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[triNdx];
		Vector3 vector = this.CalculateOriginalVertices(triNdx, 2);
		Vector3 vector2 = this.CalculateOriginalVertices(triNdx, 0);
		Vector3 zero = Vector3.zero;
		Single[] array = new Single[3];
		Boolean item = false;
		Boolean flag = false;
		for (Int32 i = 0; i < 3; i++)
		{
			vector2 = this.CalculateOriginalVertices(triNdx, i);
			Single num = Math3D.SqrDistanceToLine(cPos, vector, vector2, out zero, out item);
			Single num2 = this.radius * this.radius;
			if (num <= num2)
			{
				BGI_TRI_DEF bgi_TRI_DEF = this.fieldMap.walkMesh.BGI_findAccessibleTriangle(this, walkMeshTriangle, (UInt32)i);
				if (bgi_TRI_DEF != null)
				{
					if (!this.visited.Contains(walkMeshTriangle.neighborIdx[i]))
					{
						this.visited.Add(walkMeshTriangle.neighborIdx[i]);
						if (!this.RadiusValid(ref cPos, walkMeshTriangle.neighborIdx[i], ref hitEdge))
						{
							flag = true;
							hitEdge = i;
						}
					}
				}
				else if (num == 0f)
				{
					flag = true;
				}
				else
				{
					num = Mathf.Sqrt(num);
					this.walkMesh.BGI_computeNewPoint(zero[0], cPos[0], this.radius, num, out array[0]);
					this.walkMesh.BGI_computeNewPoint(zero[2], cPos[2], this.radius, num, out array[2]);
					Vector3 lhs = vector - vector2;
					Vector3 rhs = cPos - vector;
					Vector3 rhs2 = walkMeshTriangle.originalCenter - vector;
					lhs.y = 0f;
					rhs.y = 0f;
					rhs2.y = 0f;
					Vector3 vector3 = Vector3.Cross(lhs, rhs);
					Vector3 vector4 = Vector3.Cross(lhs, rhs2);
					if ((Mathf.Sign(vector3.y) <= 0f || Mathf.Sign(vector4.y) <= 0f) && (Mathf.Sign(vector3.y) >= 0f || Mathf.Sign(vector4.y) >= 0f))
					{
						if (this.lastTri != -1)
						{
							WalkMeshTriangle walkMeshTriangle2 = this.walkMesh.tris[this.lastTri];
							if (!Math3D.PointInsideTriangleTestXZ(cPos, walkMeshTriangle2.originalVertices[0], walkMeshTriangle2.originalVertices[1], walkMeshTriangle2.originalVertices[2]))
							{
								array[0] *= -1f;
								array[2] *= -1f;
							}
						}
						else
						{
							array[0] *= -1f;
							array[2] *= -1f;
						}
					}
					if (this.forces.Count + 1 < 16)
					{
						array[0] = zero[0] - cPos[0] + array[0];
						array[2] = zero[2] - cPos[2] + array[2];
						Vector3 item2 = new Vector3(array[0], 0f, array[2]);
						this.forces.Add(item2);
						this.forcesFlag.Add(zero == vector || zero == vector2);
						this.forcesOrigin.Add(zero);
						this.forcesType.Add(item);
					}
					WalkMeshTriangle walkMeshTriangle3 = walkMeshTriangle;
					walkMeshTriangle3.triFlags = (UInt16)(walkMeshTriangle3.triFlags | 128);
					BGI_TRI_DEF bgi_TRI_DEF2 = this.fieldMap.bgi.triList[triNdx];
					bgi_TRI_DEF2.triFlags = (UInt16)(bgi_TRI_DEF2.triFlags | 128);
					this.visited.Add(walkMeshTriangle.triIdx);
					hitEdge = i;
					flag = true;
				}
			}
			vector = vector2;
		}
		return !flag;
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
		Vector3 zero = Vector3.zero;
		if (this.forces.Count == 0)
		{
			return true;
		}
		if (this.forces.Count == 1)
		{
			cPos.x += this.forces[0].x;
			cPos.z += this.forces[0].z;
			this.curPos.x = cPos.x;
			this.curPos.z = cPos.z;
		}
		else
		{
			Boolean flag = false;
			Int32 num = 0;
			for (Int32 j = 0; j < this.forces.Count; j++)
			{
				if (this.forcesType[j])
				{
					Boolean flag2 = false;
					for (Int32 k = 0; k < j; k++)
					{
						if (this.forces[k].x == this.forces[j].x && this.forces[k].z == this.forces[j].z)
						{
							flag2 = true;
						}
					}
					if (!flag2)
					{
						flag = true;
						zero.x += this.forces[j].x;
						zero.z += this.forces[j].z;
						num++;
					}
				}
			}
			if (flag)
			{
				if (num > 1)
				{
					zero.x /= (Single)num;
					zero.z /= (Single)num;
				}
			}
			else
			{
				num = 0;
				for (Int32 l = 0; l < this.forces.Count; l++)
				{
					Boolean flag2 = false;
					for (Int32 m = 0; m < l; m++)
					{
						if (this.forces[m].x == this.forces[l].x && this.forces[m].z == this.forces[l].z)
						{
							flag2 = true;
						}
					}
					if (!flag2)
					{
						num++;
						zero.x += this.forces[l].x;
						zero.z += this.forces[l].z;
					}
				}
				if (num > 0)
				{
					zero.x /= (Single)num;
					zero.z /= (Single)num;
				}
			}
			Single num2 = 1.05f;
			if (FF9StateSystem.Common.FF9.fldMapNo == 1752 && (triNdx == 77 || triNdx == 78 || triNdx == 79 || triNdx == 80))
			{
				if (triNdx == 80)
				{
					num2 = 0.6f;
				}
				else if (triNdx == 77 || triNdx == 78 || triNdx == 79)
				{
					num2 = 0.4f;
				}
			}
			else if (FF9StateSystem.Common.FF9.fldMapNo == 406 && (triNdx == 103 || triNdx == 111 || triNdx == 113))
			{
				num2 = 0.4f;
			}
			cPos.x += zero.x * num2;
			cPos.z += zero.z * num2;
			this.curPos.x = cPos.x;
			this.curPos.z = cPos.z;
		}
		BGI_TRI_DEF bgi_TRI_DEF = this.fieldMap.bgi.triList[triNdx];
		return !this.walkMesh.BGI_traverseTriangles(this, ref bgi_TRI_DEF, oldCPos, cPos, 0, 0f, 0f);
	}

	private Int32 FindCrossingEdgeNeighbor(Int32 triIdx, Vector3 posA, Vector3 posB)
	{
		WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[triIdx];
		for (Int32 i = 0; i < 3; i++)
		{
			Int32 num;
			Int32 num2;
			WalkMeshTriangle.GetVertexIdxForEdge(i, out num, out num2);
			if (Math3D.FastLineSegmentIntersectionXZ(posA, posB, walkMeshTriangle.originalVertices[num], walkMeshTriangle.originalVertices[num2]))
			{
				if (walkMeshTriangle.neighborIdx[i] != -1)
				{
					BGI_TRI_DEF bgi_TRI_DEF = this.fieldMap.bgi.triList[walkMeshTriangle.neighborIdx[i]];
					Byte b = (Byte)(BGI.BGI_TRI_BITS_GET(bgi_TRI_DEF.triFlags) & this.fieldMap.bgi.attributeMask);
					if ((b & 192) == 0)
					{
						return i;
					}
				}
			}
		}
		return -1;
	}

	private Int32 GetTriIdxAtPos(Vector3 pos)
	{
		if (this.walkMesh == null)
		{
			return -1;
		}
		Int32 result = -1;
		Single num = Single.MaxValue;
		if (FF9StateSystem.Common.FF9.fldMapNo == 70)
		{
			return -1;
		}
		if (this.walkMesh == null)
		{
			return -1;
		}
		for (Int32 i = 0; i < this.walkMesh.tris.Count; i++)
		{
			WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[i];
			Vector3 vA = walkMeshTriangle.originalVertices[0];
			Vector3 vB = walkMeshTriangle.originalVertices[1];
			Vector3 vC = walkMeshTriangle.originalVertices[2];
			if (Math3D.PointInsideTriangleTestXZ(pos, vA, vB, vC))
			{
				Single num2 = Mathf.Abs(pos.y - walkMeshTriangle.originalCenter.y);
				if (num2 < num)
				{
					result = i;
					num = num2;
				}
			}
		}
		return result;
	}

	private Int32 GetTopTriIdxAtPos(Vector3 pos)
	{
		if (this.walkMesh == null)
		{
			return -1;
		}
		Int32 result = -1;
		Single num = Single.MinValue;
		for (Int32 i = 0; i < this.walkMesh.tris.Count; i++)
		{
			WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[i];
			Vector3 vA = walkMeshTriangle.originalVertices[0];
			Vector3 vB = walkMeshTriangle.originalVertices[1];
			Vector3 vC = walkMeshTriangle.originalVertices[2];
			if (Math3D.PointInsideTriangleTestXZ(pos, vA, vB, vC) && walkMeshTriangle.originalCenter.y > num)
			{
				result = i;
				num = walkMeshTriangle.originalCenter.y;
			}
		}
		return result;
	}

	public Int32 GetActiveTriIdxAtPos(Vector3 pos)
	{
		if (this.walkMesh == null)
		{
			return -1;
		}
		Int32 result = -1;
		Single num = Single.MaxValue;
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
				Boolean flag = Math3D.PointInsideTriangleTestXZ(pos, vA, vB, vC);
				if (flag)
				{
					this.foundTris.Add(i);
				}
			}
		}
		if ((Int32)this.originalActor.sid == FieldMapActorController.checkedSID)
		{
			for (Int32 j = 0; j < this.foundTris.Count; j++)
			{
				WalkMeshTriangle walkMeshTriangle2 = this.walkMesh.tris[this.foundTris[j]];
			}
		}
		if (this.foundTris.Count == 1)
		{
			if ((FF9StateSystem.Common.FF9.fldMapNo == 2712 && this.originalActor.sid == 18) || (FF9StateSystem.Common.FF9.fldMapNo == 2504 && this.originalActor.sid == 20) || (FF9StateSystem.Common.FF9.fldMapNo == 1056 && this.originalActor.sid == 18) || (FF9StateSystem.Common.FF9.fldMapNo == 2955 && this.originalActor.sid == 3) || (FF9StateSystem.Common.FF9.fldMapNo == 1807 && this.originalActor.sid == 27) || (FF9StateSystem.Common.FF9.fldMapNo == 1106 && (this.originalActor.sid == 18 || this.originalActor.sid == 22)))
			{
				if (this.activeFloor == -1)
				{
					result = this.foundTris[0];
				}
				else
				{
					WalkMeshTriangle walkMeshTriangle3 = this.walkMesh.tris[this.foundTris[0]];
					Single num2 = Mathf.Abs(pos.y - walkMeshTriangle3.originalCenter.y);
					if (num2 < 432f)
					{
						result = this.foundTris[0];
					}
					else
					{
						result = -1;
					}
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
			{
				if (this.foundTris[k] == this.activeTri)
				{
					return this.foundTris[k];
				}
			}
			if (this.activeTri != -1)
			{
				for (Int32 l = 0; l < this.foundTris.Count; l++)
				{
					WalkMeshTriangle walkMeshTriangle4 = this.walkMesh.tris[this.activeTri];
					for (Int32 m = 0; m < this.adjacentActiveTris.Count; m++)
					{
						if (this.foundTris[l] == this.adjacentActiveTris[m])
						{
							result = this.foundTris[l];
						}
					}
				}
			}
			else
			{
				for (Int32 n = 0; n < this.foundTris.Count; n++)
				{
					WalkMeshTriangle walkMeshTriangle5 = this.walkMesh.tris[this.foundTris[n]];
					Single num3 = Mathf.Abs(pos.y - walkMeshTriangle5.originalCenter.y);
					if (num3 < num)
					{
						result = this.foundTris[n];
						num = num3;
					}
				}
			}
		}
		if ((Int32)this.originalActor.sid == FieldMapActorController.checkedSID)
		{
		}
		return result;
	}

	private void FindAdjacentTrianglesRecursively(Int32 recursiveCount, Int32 currentTriIdx)
	{
		if (recursiveCount >= 3 || currentTriIdx == -1)
		{
			return;
		}
		WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[currentTriIdx];
		for (Int32 i = 0; i < (Int32)walkMeshTriangle.neighborIdx.Length; i++)
		{
			Int32 num = walkMeshTriangle.neighborIdx[i];
			if (num != -1)
			{
				if (!this.adjacentActiveTris.Contains(num))
				{
					this.adjacentActiveTris.Add(num);
				}
				this.FindAdjacentTrianglesRecursively(recursiveCount + 1, num);
			}
		}
	}

	private Vector3 ForceForVector(WalkMeshTriangle targetTriangle)
	{
		Single num = this.radius * this.radius;
		Vector3 vector = targetTriangle.originalCenter;
		List<Vector3> list = new List<Vector3>();
		Queue<WalkMeshTriangle> queue = new Queue<WalkMeshTriangle>();
		List<WalkMeshTriangle> list2 = new List<WalkMeshTriangle>();
		queue.Enqueue(targetTriangle);
		list2.Add(targetTriangle);
		while (queue.Count > 0)
		{
			WalkMeshTriangle walkMeshTriangle = queue.Dequeue();
			for (Int32 i = 0; i < 3; i++)
			{
				Int32 num2;
				Int32 num3;
				WalkMeshTriangle.GetVertexIdxForEdge(i, out num2, out num3);
				if (walkMeshTriangle.neighborIdx[i] == -1 || this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]].triFlags == 0)
				{
					Single num4 = Math3D.SqrDistanceToLine(vector, walkMeshTriangle.originalVertices[num2], walkMeshTriangle.originalVertices[num3]);
					if (num > num4)
					{
						Vector3 a = Math3D.ClosestPointToLine(vector, walkMeshTriangle.originalVertices[num2], walkMeshTriangle.originalVertices[num3]);
						Vector3 item = -a / Mathf.Sqrt(num4) * (Mathf.Sqrt(num) - Mathf.Sqrt(num4));
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
				}
				else
				{
					WalkMeshTriangle walkMeshTriangle2 = this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]];
					if (!list2.Contains(this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]]) && (num > Math3D.SqrDistanceToLine(vector, walkMeshTriangle2.originalVertices[0], walkMeshTriangle2.originalVertices[1]) || num > Math3D.SqrDistanceToLine(vector, walkMeshTriangle2.originalVertices[1], walkMeshTriangle2.originalVertices[2]) || num > Math3D.SqrDistanceToLine(vector, walkMeshTriangle2.originalVertices[0], walkMeshTriangle2.originalVertices[2])))
					{
						queue.Enqueue(this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]]);
						list2.Add(this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]]);
					}
				}
			}
			for (Int32 j = 0; j < list.Count; j++)
			{
				Single num5 = Mathf.Round(list[j].x * 10000f);
				Single num6 = Mathf.Round(list[j].y * 10000f);
				Single num7 = Mathf.Round(list[j].z * 10000f);
				if (num5 != 0f || num6 != 0f || num7 != 0f)
				{
					vector += list[j] * 0.5f;
				}
			}
			list.Clear();
		}
		return vector;
	}

	private Vector3 ForceForVector(WalkMeshTriangle targetTriangle, Vector3 walkableVector)
	{
		Single num = this.radius * this.radius;
		List<Vector3> list = new List<Vector3>();
		Queue<WalkMeshTriangle> queue = new Queue<WalkMeshTriangle>();
		List<WalkMeshTriangle> list2 = new List<WalkMeshTriangle>();
		queue.Enqueue(targetTriangle);
		list2.Add(targetTriangle);
		while (queue.Count > 0)
		{
			WalkMeshTriangle walkMeshTriangle = queue.Dequeue();
			for (Int32 i = 0; i < 3; i++)
			{
				Int32 num2;
				Int32 num3;
				WalkMeshTriangle.GetVertexIdxForEdge(i, out num2, out num3);
				if (walkMeshTriangle.neighborIdx[i] == -1 || this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]].triFlags == 0)
				{
					Single num4 = Math3D.SqrDistanceToLine(walkableVector, walkMeshTriangle.originalVertices[num2], walkMeshTriangle.originalVertices[num3]);
					if (num > num4)
					{
						Vector3 a = Math3D.ClosestPointToLine(walkableVector, walkMeshTriangle.originalVertices[num2], walkMeshTriangle.originalVertices[num3]);
						Vector3 item = -a / Mathf.Sqrt(num4) * (Mathf.Sqrt(num) - Mathf.Sqrt(num4));
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
				}
				else
				{
					WalkMeshTriangle walkMeshTriangle2 = this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]];
					if (!list2.Contains(this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]]) && (num > Math3D.SqrDistanceToLine(walkableVector, walkMeshTriangle2.originalVertices[0], walkMeshTriangle2.originalVertices[1]) || num > Math3D.SqrDistanceToLine(walkableVector, walkMeshTriangle2.originalVertices[1], walkMeshTriangle2.originalVertices[2]) || num > Math3D.SqrDistanceToLine(walkableVector, walkMeshTriangle2.originalVertices[0], walkMeshTriangle2.originalVertices[2])))
					{
						queue.Enqueue(this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]]);
						list2.Add(this.walkMesh.tris[walkMeshTriangle.neighborIdx[i]]);
					}
				}
			}
			for (Int32 j = 0; j < list.Count; j++)
			{
				Single num5 = Mathf.Round(list[j].x * 10000f);
				Single num6 = Mathf.Round(list[j].y * 10000f);
				Single num7 = Mathf.Round(list[j].z * 10000f);
				if (num5 != 0f || num6 != 0f || num7 != 0f)
				{
					walkableVector += list[j];
				}
			}
			list.Clear();
		}
		return walkableVector;
	}

	private Boolean IsWalkableRadius(Vector3 start, Vector3 end, List<Int32> trisIdx)
	{
		Boolean flag = true;
		Single num = this.radius * this.radius * 0.95f * 0.95f;
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		for (Int32 i = trisIdx.Count - 1; i >= 0; i--)
		{
			if (trisIdx[i] == -1)
			{
				trisIdx.RemoveAt(i);
			}
			else
			{
				if (this.walkMesh.tris[trisIdx[i]].neighborIdx[0] != -1 && !trisIdx.Contains(this.walkMesh.tris[trisIdx[i]].neighborIdx[0]))
				{
					trisIdx.Add(this.walkMesh.tris[trisIdx[i]].neighborIdx[0]);
				}
				if (this.walkMesh.tris[trisIdx[i]].neighborIdx[1] != -1 && !trisIdx.Contains(this.walkMesh.tris[trisIdx[i]].neighborIdx[1]))
				{
					trisIdx.Add(this.walkMesh.tris[trisIdx[i]].neighborIdx[1]);
				}
				if (this.walkMesh.tris[trisIdx[i]].neighborIdx[2] != -1 && !trisIdx.Contains(this.walkMesh.tris[trisIdx[i]].neighborIdx[2]))
				{
					trisIdx.Add(this.walkMesh.tris[trisIdx[i]].neighborIdx[2]);
				}
			}
		}
		for (Int32 j = 0; j < trisIdx.Count; j++)
		{
			Queue<WalkMeshTriangle> queue = new Queue<WalkMeshTriangle>();
			List<WalkMeshTriangle> list3 = new List<WalkMeshTriangle>();
			queue.Enqueue(this.walkMesh.tris[trisIdx[j]]);
			list3.Add(this.walkMesh.tris[trisIdx[j]]);
			while (queue.Count > 0)
			{
				WalkMeshTriangle walkMeshTriangle = queue.Dequeue();
				for (Int32 k = 0; k < 3; k++)
				{
					Int32 num2;
					Int32 num3;
					WalkMeshTriangle.GetVertexIdxForEdge(k, out num2, out num3);
					if (walkMeshTriangle.neighborIdx[k] != -1)
					{
						if (!list3.Contains(this.walkMesh.tris[walkMeshTriangle.neighborIdx[k]]) && (Math3D.SqrDistanceToLine(this.walkMesh.tris[walkMeshTriangle.neighborIdx[k]].originalVertices[0], start, end) < num || Math3D.SqrDistanceToLine(this.walkMesh.tris[walkMeshTriangle.neighborIdx[k]].originalVertices[1], start, end) < num || Math3D.SqrDistanceToLine(this.walkMesh.tris[walkMeshTriangle.neighborIdx[k]].originalVertices[2], start, end) < num))
						{
							queue.Enqueue(this.walkMesh.tris[walkMeshTriangle.neighborIdx[k]]);
							list3.Add(this.walkMesh.tris[walkMeshTriangle.neighborIdx[k]]);
							break;
						}
					}
					else
					{
						list2.Add(walkMeshTriangle.originalVertices[num2]);
						list2.Add(walkMeshTriangle.originalVertices[num3]);
						if (!list.Contains(walkMeshTriangle.originalVertices[num2]))
						{
							list.Add(walkMeshTriangle.originalVertices[num2]);
						}
						if (!list.Contains(walkMeshTriangle.originalVertices[num3]))
						{
							list.Add(walkMeshTriangle.originalVertices[num3]);
						}
					}
				}
			}
		}
		for (Int32 l = 0; l < list.Count; l++)
		{
			flag &= (Math3D.SqrDistanceToLine(list[l], start, end) > num);
		}
		for (Int32 m = 0; m < list2.Count; m += 2)
		{
			flag &= !Math3D.FastLineSegmentIntersectionXZ(list2[m], list2[m + 1], start, end);
		}
		return flag;
	}

	private Boolean IsWalkableDepth0(WalkMeshTriangle t1, WalkMeshTriangle t2)
	{
		Boolean flag = true;
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		for (Int32 i = 0; i < 3; i++)
		{
			Int32 num;
			Int32 num2;
			WalkMeshTriangle.GetVertexIdxForEdge(i, out num, out num2);
			if (t1.neighborIdx[i] == -1)
			{
				list.Add(t1.originalVertices[num]);
				list2.Add(t1.originalVertices[num2]);
			}
			if (t2.neighborIdx[i] == -1)
			{
				list.Add(t2.originalVertices[num]);
				list2.Add(t2.originalVertices[num2]);
			}
		}
		for (Int32 j = 0; j < list.Count; j++)
		{
			flag &= !Math3D.FastLineSegmentIntersectionXZ(t1.originalCenter, t2.originalCenter, list[j], list2[j]);
		}
		return flag;
	}

	private Boolean IsWalkableDepth0(WalkMeshTriangle t1, WalkMeshTriangle t2, Vector3 p1, Vector3 p2)
	{
		Boolean flag = true;
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		for (Int32 i = 0; i < 3; i++)
		{
			Int32 num;
			Int32 num2;
			WalkMeshTriangle.GetVertexIdxForEdge(i, out num, out num2);
			if (t1.neighborIdx[i] == -1)
			{
				list.Add(t1.originalVertices[num]);
				list2.Add(t1.originalVertices[num2]);
			}
			if (t2.neighborIdx[i] == -1)
			{
				list.Add(t2.originalVertices[num]);
				list2.Add(t2.originalVertices[num2]);
			}
		}
		for (Int32 j = 0; j < list.Count; j++)
		{
			flag &= !Math3D.FastLineSegmentIntersectionXZ(p1, p2, list[j], list2[j]);
		}
		return flag;
	}

	private Boolean IsTriangleVerticesAllEdges(WalkMeshTriangle tri)
	{
		Boolean result = false;
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		for (Int32 i = 0; i < 3; i++)
		{
			Int32 num;
			Int32 num2;
			WalkMeshTriangle.GetVertexIdxForEdge(i, out num, out num2);
			if (tri.neighborIdx[i] == -1)
			{
				list.Add(tri.originalVertices[num]);
				list.Add(tri.originalVertices[num2]);
			}
			else
			{
				list2.Add(tri.originalVertices[num]);
				list2.Add(tri.originalVertices[num2]);
			}
		}
		if (list2.Count == 4)
		{
			Single magnitude = (list2[0] - list2[1]).magnitude;
			Single magnitude2 = (list2[2] - list2[3]).magnitude;
			if (magnitude * 2f > magnitude2 && magnitude2 * 2f > magnitude && magnitude > this.radius * 4f && magnitude2 > this.radius * 4f)
			{
				return result;
			}
		}
		for (Int32 j = 0; j < 3; j++)
		{
			if (tri.neighborIdx[j] != -1)
			{
				WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[tri.neighborIdx[j]];
				for (Int32 k = 0; k < 3; k++)
				{
					if (walkMeshTriangle.neighborIdx[k] == -1)
					{
						Int32 num3;
						Int32 num4;
						WalkMeshTriangle.GetVertexIdxForEdge(k, out num3, out num4);
						list.Add(walkMeshTriangle.originalVertices[num3]);
						list.Add(walkMeshTriangle.originalVertices[num4]);
					}
				}
			}
		}
		if (list.Contains(tri.originalVertices[0]) && list.Contains(tri.originalVertices[1]) && list.Contains(tri.originalVertices[2]))
		{
			result = true;
		}
		return result;
	}

	private Boolean IsTriangleVerticesAllEdgesDepth2(WalkMeshTriangle tri)
	{
		Boolean result = false;
		List<Vector3> list = new List<Vector3>();
		for (Int32 i = 0; i < 3; i++)
		{
			if (tri.neighborIdx[i] == -1)
			{
				Int32 num;
				Int32 num2;
				WalkMeshTriangle.GetVertexIdxForEdge(i, out num, out num2);
				list.Add(tri.originalVertices[num]);
				list.Add(tri.originalVertices[num2]);
			}
		}
		List<WalkMeshTriangle> list2 = new List<WalkMeshTriangle>();
		for (Int32 j = 0; j < 3; j++)
		{
			if (tri.neighborIdx[j] != -1)
			{
				list2.Add(this.walkMesh.tris[tri.neighborIdx[j]]);
				WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[tri.neighborIdx[j]];
				for (Int32 k = 0; k < 3; k++)
				{
					if (walkMeshTriangle.neighborIdx[k] != -1)
					{
						list2.Add(this.walkMesh.tris[walkMeshTriangle.neighborIdx[k]]);
					}
				}
			}
		}
		for (Int32 l = 0; l < list2.Count; l++)
		{
			if (tri.neighborIdx[l] != -1)
			{
				WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[tri.neighborIdx[l]];
				for (Int32 m = 0; m < 3; m++)
				{
					if (walkMeshTriangle.neighborIdx[m] == -1)
					{
						Int32 num3;
						Int32 num4;
						WalkMeshTriangle.GetVertexIdxForEdge(m, out num3, out num4);
						list.Add(walkMeshTriangle.originalVertices[num3]);
						list.Add(walkMeshTriangle.originalVertices[num4]);
					}
				}
			}
		}
		if (list.Contains(tri.originalVertices[0]) && list.Contains(tri.originalVertices[1]) && list.Contains(tri.originalVertices[2]))
		{
			result = true;
		}
		return result;
	}

	private void CheckPosInProjectedWalkMesh()
	{
		if ((this.charFlags & 1) == 0)
		{
			return;
		}
		Boolean flag = false;
		Boolean flag2 = false;
		BGSCENE_DEF scene = this.fieldMap.scene;
		BGI_DEF bgi = this.fieldMap.bgi;
		if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
		{
			flag = true;
		}
		if (!flag && !flag2)
		{
			return;
		}
		PSXCameraAspect psxcameraAspect = UnityEngine.Object.FindObjectOfType<PSXCameraAspect>();
		Vector3 localMousePosRelative = psxcameraAspect.GetLocalMousePosRelative();
		Vector3 vector = new Vector3(0f, 0f, (Single)scene.curZ);
		if (this.walkMesh == null)
		{
			return;
		}
		Int32 triangleIndexAtScreenPos = this.walkMesh.GetTriangleIndexAtScreenPos(localMousePosRelative);
		DebugUtil.DebugDrawMarker(localMousePosRelative, 5f, Color.red);
		if (triangleIndexAtScreenPos == -1)
		{
			return;
		}
		WalkMeshTriangle walkMeshTriangle = this.walkMesh.tris[triangleIndexAtScreenPos];
		Vector3 v = walkMeshTriangle.transformedVertices[0];
		Vector3 v2 = walkMeshTriangle.transformedVertices[1];
		Vector3 v3 = walkMeshTriangle.transformedVertices[2];
		BGCAM_DEF currentBgCamera = this.fieldMap.GetCurrentBgCamera();
		Matrix4x4 matrixRT = currentBgCamera.GetMatrixRT();
		Vector3 vector2 = Math3D.CalculateBarycentricRatio(localMousePosRelative, v, v2, v3);
		Vector3 vector3 = Math3D.CalculateBarycentric(walkMeshTriangle.originalCenter, walkMeshTriangle.originalVertices[0], walkMeshTriangle.originalVertices[1], walkMeshTriangle.originalVertices[2]);
		Vector3 vector4 = Math3D.CalculateBarycentric(PSX.CalculateGTE_RTPT(walkMeshTriangle.originalCenter, Matrix4x4.identity, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), this.fieldMap.offset), walkMeshTriangle.transformedVertices[0], walkMeshTriangle.transformedVertices[1], walkMeshTriangle.transformedVertices[2]);
		Single num = vector3.x / vector4.x + vector3.y / vector4.y + vector3.z / vector4.z;
		Vector3 vector5 = new Vector3(vector3.x / vector4.x / num, vector3.y / vector4.y / num, vector3.z / vector4.z / num);
		Single num2 = vector2.x * vector5.x + vector2.y * vector5.y + vector2.z * vector5.z;
		vector2.x = vector2.x * vector5.x / num2;
		vector2.y = vector2.y * vector5.y / num2;
		vector2.z = vector2.z * vector5.z / num2;
		Vector3 vector6 = walkMeshTriangle.originalVertices[0] * vector2.x + walkMeshTriangle.originalVertices[1] * vector2.y + walkMeshTriangle.originalVertices[2] * vector2.z;
		Vector3 pos = walkMeshTriangle.transformedVertices[0] * vector2.x + walkMeshTriangle.transformedVertices[1] * vector2.y + walkMeshTriangle.transformedVertices[2] * vector2.z;
		DebugUtil.DebugDrawMarker(pos, 5f, Color.green);
		vector6 = this.ForceForVector(this.walkMesh.tris[triangleIndexAtScreenPos], vector6);
		if (this.CheckCollideNPC(this.walkMesh.tris[triangleIndexAtScreenPos], vector6))
		{
			return;
		}
		if (flag)
		{
			if (flag2)
			{
				this.SetPosition(vector6, true, true);
				this.movePaths.Clear();
				this.ClearMoveTarget();
			}
			else
			{
				WalkMeshTriangle walkMeshTriangle2 = this.walkMesh.FindPathReversed(walkMeshTriangle, this.walkMesh.tris[this.activeTri], this.radius);
				List<Int32> list = new List<Int32>();
				List<Vector3> list2 = new List<Vector3>();
				Single num3 = 0f;
				if (walkMeshTriangle2 != null)
				{
					while (walkMeshTriangle2 != null && walkMeshTriangle2.next != null)
					{
						list.Add(walkMeshTriangle2.triIdx);
						walkMeshTriangle2 = walkMeshTriangle2.next;
					}
					if (walkMeshTriangle2 != null)
					{
						list.Add(walkMeshTriangle2.triIdx);
					}
					list2 = this.SmoothPathsByForce(list, this.curPos, vector6);
					for (Int32 i = 1; i < list2.Count; i++)
					{
						num3 += Vector3.Distance(list2[i - 1], list2[i]);
					}
				}
				WalkMeshTriangle walkMeshTriangle3 = this.walkMesh.FindPathReversed(this.walkMesh.tris[this.activeTri], walkMeshTriangle, this.radius);
				List<Int32> list3 = new List<Int32>();
				List<Vector3> list4 = new List<Vector3>();
				Single num4 = 0f;
				if (walkMeshTriangle3 != null)
				{
					while (walkMeshTriangle3 != null && walkMeshTriangle3.next != null)
					{
						list3.Add(walkMeshTriangle3.triIdx);
						walkMeshTriangle3 = walkMeshTriangle3.next;
					}
					if (walkMeshTriangle3 != null)
					{
						list3.Add(walkMeshTriangle3.triIdx);
					}
					list4 = this.SmoothPathsByForce(list3, vector6, this.curPos);
					list4.Reverse();
					for (Int32 j = 1; j < list4.Count; j++)
					{
						num4 += Vector3.Distance(list4[j - 1], list4[j]);
					}
				}
				if (walkMeshTriangle2 != null || walkMeshTriangle3 != null)
				{
					if (num4 < num3)
					{
						list2 = list4;
						list = list3;
					}
					this.ClearMoveTarget();
					this.movePaths.Clear();
					this.movePaths.AddRange(list2);
					this.debugSmoothPaths.Clear();
					this.debugSmoothPaths.AddRange(list2);
					this.debugPaths.Clear();
					for (Int32 k = 0; k < list.Count; k++)
					{
						WalkMeshTriangle walkMeshTriangle4 = this.walkMesh.tris[list[k]];
						Vector3 originalCenter = walkMeshTriangle4.originalCenter;
						this.debugPaths.Add(originalCenter);
					}
					this.totalPathCount = list.Count;
					this.totalPathLengthSq = 0f;
					if (list2.Count == 1)
					{
						this.totalPathLengthSq += (this.curPos - list2[0]).sqrMagnitude;
					}
					else
					{
						for (Int32 l = 0; l < list2.Count - 1; l++)
						{
							this.totalPathLengthSq += (list2[l] - list2[l + 1]).sqrMagnitude;
						}
					}
				}
			}
		}
	}

	private Boolean CheckCollideNPC(WalkMeshTriangle targetTriangle, Vector3 walkableVector)
	{
		FieldMapActorController[] array = UnityEngine.Object.FindObjectsOfType<FieldMapActorController>();
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			if (!(array[i] == this))
			{
				Vector3 position = array[i].transform.position;
				Single sqrMagnitude = (walkableVector - position).sqrMagnitude;
				Single num = this.radius * 2f;
				num *= num;
				if (sqrMagnitude < num)
				{
					return true;
				}
			}
		}
		return false;
	}

	private List<Vector3> SmoothPathsByForce(List<Int32> pathsIdx, Vector3 start, Vector3 end)
	{
		if (pathsIdx.Count == 0)
		{
			return null;
		}
		List<Vector3> list = new List<Vector3>();
		List<List<Int32>> list2 = new List<List<Int32>>();
		list.Add(start);
		list2.Add(new List<Int32>());
		list2[0].Add(pathsIdx[0]);
		Vector3 vector = start;
		Vector3 vector2 = vector;
		Int32 num = 0;
		Int32 num2 = 0;
		Int32 i = num2 + 1;
		Single num3 = this.radius * this.radius;
		num3 *= 0.4f;
		if (pathsIdx.Count < 1)
		{
			list2[list2.Count - 1].Add(this.GetTriIdxAtPos(start));
			list2[list2.Count - 1].Add(this.GetTriIdxAtPos(end));
			list.Add(end);
			return list;
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
			if (list.Count > 1)
			{
				for (Int32 k = 0; k < 3; k++)
				{
					Int32 num4;
					Int32 num5;
					WalkMeshTriangle.GetVertexIdxForEdge(k, out num4, out num5);
					if (walkMeshTriangle.neighborIdx[k] != -1)
					{
						if (Math3D.FastLineSegmentIntersectionXZ(walkMeshTriangle.originalVertices[num4], walkMeshTriangle.originalVertices[num5], this.ForceForVector(walkMeshTriangle), list[list.Count - 1]))
						{
							Vector3 item2 = (walkMeshTriangle.originalVertices[num4] + walkMeshTriangle.originalVertices[num5]) / 2f;
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
					Int32 num4;
					Int32 num5;
					WalkMeshTriangle.GetVertexIdxForEdge(l, out num4, out num5);
					if (walkMeshTriangle.neighborIdx[l] != -1)
					{
						if (Math3D.FastLineSegmentIntersectionXZ(walkMeshTriangle.originalVertices[num4], walkMeshTriangle.originalVertices[num5], this.ForceForVector(walkMeshTriangle), this.ForceForVector(this.walkMesh.tris[pathsIdx[num2 + 1]])))
						{
							Vector3 item3 = (walkMeshTriangle.originalVertices[num4] + walkMeshTriangle.originalVertices[num5]) / 2f;
							list4.Add(item3);
							break;
						}
					}
				}
			}
			for (Int32 m = 0; m < list4.Count; m++)
			{
				if (!list.Contains(list4[m]))
				{
					list2[list2.Count - 1].Add(this.GetTriIdxAtPos(list4[m]));
					list.Add(list4[m]);
					list2.Add(new List<Int32>());
				}
			}
			vector = list[list.Count - 1];
			vector2 = vector;
			num = num2;
		}
		Int32 num6 = 0;
		while (i < pathsIdx.Count)
		{
			num6++;
			if (num6 >= pathsIdx.Count * 2)
			{
				return list;
			}
			if (this.IsTriangleVerticesAllEdges(this.walkMesh.tris[pathsIdx[i]]))
			{
				if (i - num2 > 2 && !list.Contains(vector2))
				{
					list2[list2.Count - 1].Add(this.GetTriIdxAtPos(vector2));
					list.Add(vector2);
					list2.Add(new List<Int32>());
				}
				WalkMeshTriangle walkMeshTriangle2 = this.walkMesh.tris[pathsIdx[i]];
				List<Vector3> list5 = new List<Vector3>();
				List<Single> list6 = new List<Single>();
				if (list.Count > 0)
				{
					for (Int32 n = 0; n < 3; n++)
					{
						Int32 num7;
						Int32 num8;
						WalkMeshTriangle.GetVertexIdxForEdge(n, out num7, out num8);
						if (walkMeshTriangle2.neighborIdx[n] != -1)
						{
							if (Math3D.FastLineSegmentIntersectionXZ(walkMeshTriangle2.originalVertices[num7], walkMeshTriangle2.originalVertices[num8], this.ForceForVector(walkMeshTriangle2), list[list.Count - 1]))
							{
								Vector3 item4 = (walkMeshTriangle2.originalVertices[num7] + walkMeshTriangle2.originalVertices[num8]) / 2f;
								list5.Add(item4);
								list6.Add((walkMeshTriangle2.originalVertices[num7] - walkMeshTriangle2.originalVertices[num8]).magnitude);
								break;
							}
						}
					}
				}
				if (i + 1 <= pathsIdx.Count - 1)
				{
					for (Int32 num9 = 0; num9 < 3; num9++)
					{
						Int32 num7;
						Int32 num8;
						WalkMeshTriangle.GetVertexIdxForEdge(num9, out num7, out num8);
						if (walkMeshTriangle2.neighborIdx[num9] != -1)
						{
							if (Math3D.FastLineSegmentIntersectionXZ(walkMeshTriangle2.originalVertices[num7], walkMeshTriangle2.originalVertices[num8], this.ForceForVector(walkMeshTriangle2), this.ForceForVector(this.walkMesh.tris[pathsIdx[i + 1]])))
							{
								Vector3 vector3 = (walkMeshTriangle2.originalVertices[num7] + walkMeshTriangle2.originalVertices[num8]) / 2f;
								if (list5.Count > 0)
								{
									list6.Add((walkMeshTriangle2.originalVertices[num7] - walkMeshTriangle2.originalVertices[num8]).magnitude);
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
					if (!list.Contains(list5[num13]))
					{
						list2[list2.Count - 1].Add(this.GetTriIdxAtPos(list5[num13]));
						list.Add(list5[num13]);
						list2.Add(new List<Int32>());
					}
				}
				num2 = i;
				vector = list[list.Count - 1];
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
					if (!list.Contains(vector))
					{
						list2[list2.Count - 1].Add(this.GetTriIdxAtPos(vector));
						list.Add(vector);
						list2.Add(new List<Int32>());
					}
					if (!list.Contains(list3[i]))
					{
						list2[list2.Count - 1].Add(this.GetTriIdxAtPos(list3[i]));
						list.Add(list3[i]);
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
					if (!list.Contains(vector))
					{
						list2[list2.Count - 1].Add(this.GetTriIdxAtPos(vector));
						list.Add(vector);
						list2.Add(new List<Int32>());
					}
					i = num2 + 1;
				}
			}
		}
		if (list.Count > 1)
		{
			if (list3.IndexOf(list[1]) >= 0)
			{
				WalkMeshTriangle t = this.walkMesh.tris[pathsIdx[list3.IndexOf(list[1])]];
				if (!this.IsWalkableDepth0(this.walkMesh.tris[pathsIdx[0]], t, start, list[1]) && !list.Contains(vector))
				{
					list.Insert(1, list3[0]);
					list2.Insert(1, list2[0]);
				}
			}
			if (list3.IndexOf(list[list.Count - 1]) >= 0)
			{
				WalkMeshTriangle t2 = this.walkMesh.tris[pathsIdx[list3.IndexOf(list[list.Count - 1])]];
				if (!this.IsWalkableDepth0(this.walkMesh.tris[pathsIdx[pathsIdx.Count - 1]], t2, end, list[list.Count - 1]) && !list.Contains(vector))
				{
					list.Add(list3[list3.Count - 1]);
					list2.Add(list2[list2.Count - 1]);
				}
			}
		}
		list.Add(end);
		list2.Add(new List<Int32>());
		list2[list2.Count - 1].Add(pathsIdx[pathsIdx.Count - 1]);
		for (Int32 num18 = 0; num18 < list.Count; num18++)
		{
			if (this.GetTriIdxAtPos(list[num18]) >= 0)
			{
				list[num18] = this.ForceForVector(this.walkMesh.tris[this.GetTriIdxAtPos(list[num18])], list[num18]);
			}
		}
		Int32 num19 = 0;
		Int32 num20 = 2;
		for (Int32 num21 = 1; num21 < list.Count - 1; num21++)
		{
			if (num20 - num19 > 4)
			{
				num21 = num20;
				num19 = num20;
				num20 = num19 + 2;
			}
			else
			{
				Vector3 from = list[num21 - 1] - list[num21];
				Vector3 to = list[num21 + 1] - list[num21];
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
					Vector3 vector5 = (list[num21 - 1] + list[num21]) / 2f;
					Vector3 a = (list[num21 + 1] + list[num21]) / 2f;
					vector5 = (a + list[num21 - 1]) / 2f;
					a = (a + list[num21 + 1]) / 2f;
				}
				else
				{
					num21 = num20 - 1;
					num19 = num20 - 1;
					num20 = num19 + 2;
				}
			}
		}
		for (Int32 num24 = 1; num24 < list.Count; num24++)
		{
			Single num25 = Vector3.Distance(list[num24 - 1], list[num24]);
			if (num25 < (Single)this.originalActor.speed * 0.95f)
			{
				list.RemoveAt(num24);
				num24--;
			}
		}
		for (Int32 num26 = 0; num26 < list.Count; num26++)
		{
			if (this.GetTriIdxAtPos(list[num26]) >= 0)
			{
				list[num26] = this.ForceForVector(this.walkMesh.tris[this.GetTriIdxAtPos(list[num26])], list[num26]);
			}
		}
		list.Reverse();
		return list;
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
			if (this.questionMark == (UnityEngine.Object)null)
			{
				String[] pngInfo;
				this.questionMark = AssetManager.Load<Texture2D>("CommonAsset/EventIcons/balloon_question", out pngInfo, false);
				this.exclamationMark = AssetManager.Load<Texture2D>("CommonAsset/EventIcons/balloon_exclamation", out pngInfo, false);
				this.targetMark = AssetManager.Load<Texture2D>("CommonAsset/EventIcons/cursor_hand_here", out pngInfo, false);
				this.warpMark = AssetManager.Load<Texture2D>("CommonAsset/EventIcons/cursor_warp", out pngInfo, false);
			}
			this.targetMarkSize = (Single)Screen.height * 0.1f;
			GameObject gameObject = GameObject.Find("NPC mog");
			if (gameObject != (UnityEngine.Object)null)
			{
				this.npc = gameObject.transform;
			}
			else
			{
				this.npc = (Transform)null;
			}
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

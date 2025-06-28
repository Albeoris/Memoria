using Assets.Scripts.Common;
using FF9;
using Memoria;
using System;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable ClassNeverInstantiated.Global

public partial class EventEngine
{
    public Int32 ProcessEvents()
    {
        EventEngine.LastProcessTime = RealTime.time;
        VoicePlayer.scriptRequestedButtonPress = false;
        if (FF9StateSystem.Settings.IsNoEncounter)
        {
            // Prevent Wyerd battles
            if (FF9StateSystem.Common.FF9.fldMapNo == 303 || FF9StateSystem.Common.FF9.fldMapNo == 304)
            {
                // Ice Cavern/Icicle Field or Ice Cavern/Ice Path
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(0xCFC5, 0); // VAR_GlobBool_207
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(0xCEC5, 0); // VAR_GlobBool_206
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(0xCDC5, 0); // VAR_GlobBool_205
            }
            else if (FF9StateSystem.Common.FF9.fldMapNo == 301)
            {
                // Ice Cavern/Ice Path
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(0xEFC5, 0); // VAR_GlobBool_239
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(0xEEC5, 0); // VAR_GlobBool_238
            }
            else if (FF9StateSystem.Common.FF9.fldMapNo == 302)
            {
                // Ice Cavern/Ice Path
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(0xCFC5, 0); // VAR_GlobBool_207
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(0xCEC5, 0); // VAR_GlobBool_206
            }
            else if (FF9StateSystem.Common.FF9.fldMapNo == 2921)
            {
                // Memoria/To the Origin
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(0x28DD, 10); // VAR_GlobUInt16_40
            }
        }
        Boolean isBattle = this.gMode == 2;
        this._moveKey = false;
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (obj.cid == 4)
            {
                Actor actor = (Actor)obj;
                FieldMapActorController mapActorController = actor.fieldMapActorController;
                if (obj.uid == this._context.controlUID && obj.state == EventEngine.stateRunning)
                {
                    if (this._context.usercontrol != 0)
                    {
                        if (this.gMode == 1)
                        {
                            //if (mapActorController == null)
                            //    ;
                            this._moveKey = FieldMapActorController.ccSMoveKey;
                        }
                        else if (this.gMode == 3)
                        {
                            Single distance = 0.0f;
                            this._moveKey = ff9.w_frameEncountEnable;
                            this.CollisionRequest((PosObj)obj);
                            PosObj collisionObj = (PosObj)this.Collision(this, actor, 0, ref distance);
                            if (collisionObj != null && distance <= 0f)
                            {
                                WMActor wmActor = actor.wmActor;
                                WMActor wmCollisionActor = ((Actor)collisionObj).wmActor;
                                Vector2 from = new Vector2(wmActor.pos0 - wmActor.lastx, wmActor.pos2 - wmActor.lastz);
                                Vector2 to = new Vector2(wmCollisionActor.pos0 - wmActor.pos0, wmCollisionActor.pos2 - wmActor.pos2);
                                float angle = Vector2.Angle(from, to);
                                if (angle >= 0f && angle <= 90f)
                                    wmActor.transform.position = new Vector3(wmActor.lastx, wmActor.lasty, wmActor.lastz);
                            }
                        }
                        if (this._moveKey)
                            this.ResetIdleTimer(0);
                    }
                    else if (mapActorController != null)
                    {
                        mapActorController.CopyLastPos();
                    }
                }
                else if (mapActorController != null)
                {
                    mapActorController.CopyLastPos();
                }
                if (obj.state == EventEngine.stateRunning)
                {
                    if (!isBattle)
                        this.ProcessAnime((Actor)obj);
                    //if (this._context.usercontrol == 0)
                    //    ;
                }
            }
        }
        if (isBattle)
            this.SetupBattleState();

        this._posUsed = false;

        // TODO Check Native: #147
        Int32 result = 0;
        bool canProcessCode = true;

        if (_ff9.fldMapNo == 257) // Evil Forest/Nest
            canProcessCode = !Singleton<DialogManager>.Instance.Activate || Singleton<DialogManager>.Instance.CompletlyVisible;

        if (canProcessCode)
            result = this.eBin.ProcessCode(this._context.activeObj);

        EventHUD.CheckUIMiniGameForMobile();
        if (result == 6)
            result = 0;
        else
            this.gStopObj = null;

        Boolean hasMoved = false;
        this._aimObj = null;
        this._eyeObj = null;
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
            this.SetRenderer(objList.obj, isBattle);
        //if (this.gMode != 1)
        //    ;
        if (isBattle)
        {
            for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
            {
                Obj obj = objList.obj;
                if (obj.btlchk == 2 && this.Request(obj, 3, 5, false)) // Run the ATB function
                    obj.btlchk = 1;
            }
        }
        else
        {
            for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
            {
                Obj obj = objList.obj;
                Single deltaX = 0.0f;
                Single deltaY = 0.0f;
                Single deltaZ = 0.0f;
                if (obj.cid == 4)
                {
                    Actor actor = (Actor)obj;
                    if (this.gMode == 1)
                    {
                        if (obj.go != null)
                        {
                            FieldMapActorController actorController = obj.go.GetComponent<FieldMapActorController>();
                            if (actorController != null)
                            {
                                Int32 num = 0;
                                if (actor.cid != 4 || actor.model == UInt16.MaxValue)
                                    num = 0;
                                //else if (component.walkMesh == null)
                                //    ;
                                if ((obj.uid != this._context.controlUID || this.sLockTimer >= 0L) && (num & 1) != 0)
                                {
                                    PosObj posObj = this.fieldmap.walkMesh.Collision(actor.fieldMapActorController, 0, out Single distance);
                                    if (posObj != null)
                                    {
                                        actor.fieldMapActorController.ResetPos();
                                        if ((posObj.flags & 16) != 0)
                                            this.sLockTimer = 0L;
                                        if (obj.uid == this._context.controlUID)
                                            this._collTimer = 2;
                                    }
                                }
                            }
                        }
                        if (obj.uid == this._context.controlUID)
                        {
                            EIcon.ProcessHereIcon(actor);
                            if (this.GetUserControl())
                                this.CollisionRequest(actor);
                        }
                    }
                    else if (this.gMode == 3)
                    {
                        if ((actor.actf & EventEngine.actEye) != 0)
                        {
                            Vector3 eyePtr = ff9.w_cameraGetEyePtr();
                            eyePtr.x = actor.pos[0];
                            eyePtr.y = actor.pos[1];
                            eyePtr.z = actor.pos[2];
                            ff9.w_cameraSetEyePtr(eyePtr);
                            this._eyeObj = actor;
                        }
                        else if ((actor.actf & EventEngine.actAim) != 0)
                        {
                            Vector3 aimPtr = ff9.w_cameraGetAimPtr();
                            aimPtr.x = actor.pos[0];
                            aimPtr.y = actor.pos[1];
                            aimPtr.z = actor.pos[2];
                            ff9.w_cameraSetAimPtr(aimPtr);
                            this._aimObj = actor;
                        }
                    }
                    if (obj.go != null)
                    {
                        if (this.gMode == 3)
                        {
                            WMActor wmActor = ((Actor)obj).wmActor;
                            if (wmActor != null)
                            {
                                deltaX = 256f * (wmActor.pos0 - wmActor.lastx);
                                deltaY = 256f * (wmActor.pos1 - wmActor.lasty);
                                deltaZ = 256f * (wmActor.pos2 - wmActor.lastz);
                                hasMoved = !EventEngineUtils.nearlyEqual(wmActor.pos0, wmActor.lastx) || !EventEngineUtils.nearlyEqual(wmActor.pos2, wmActor.lastz);
                                //if ((Int32)obj.sid != 5 && (Int32)obj.sid == 11)
                                //    ;
                            }
                        }
                        else if (this.gMode == 1)
                        {
                            FieldMapActorController actorController = obj.go.GetComponent<FieldMapActorController>();
                            deltaX = actorController.curPos.x - actorController.lastPos.x;
                            deltaY = actorController.curPos.y - actorController.lastPos.y;
                            deltaZ = actorController.curPos.z - actorController.lastPos.z;
                            hasMoved = !EventEngineUtils.nearlyEqual(actorController.curPos.x, actorController.lastPos.x) || !EventEngineUtils.nearlyEqual(actorController.curPos.z, actorController.lastPos.z);
                        }
                        if (actor.follow != Byte.MaxValue && Singleton<DialogManager>.Instance.GetDialogByWindowID(actor.follow) == null)
                        {
                            actor.follow = Byte.MaxValue;
                            if (EventEngine.sLastTalker == actor)
                                EventEngine.sTalkTimer = 30;
                        }
                        if (this.gMode == 1)
                        {
                            this.ProcessTurn(actor);
                            if (actor.model != UInt16.MaxValue)
                                this.ProcessNeck(actor);
                        }
                        if (this.gMode == 3)
                            this.ProcessTurn(actor);
                        if (hasMoved)
                        {
                            Int32 animExacLowerFreeze = actor.animFlag & (EventEngine.afExec | EventEngine.afLower | EventEngine.afFreeze);
                            if (animExacLowerFreeze == 0 || animExacLowerFreeze == (EventEngine.afExec | EventEngine.afLower))
                            {
                                actor.animFlag &= (Byte)~(EventEngine.afExec | EventEngine.afLower);
                                this.SetAnim(actor, actor.speed < actor.speedth ? actor.walk : actor.run);
                            }
                        }
                        else if ((actor.animFlag & (EventEngine.afExec | EventEngine.afFreeze)) == 0 && (this._collTimer == 0 || obj.uid != this._context.controlUID))
                        {
                            this.SetAnim(actor, actor.idle);
                        }
                        if (hasMoved && obj.uid == this._context.controlUID && this._moveKey && this._context.encratio > 0)
                        {
                            Single distance = this.distance(deltaX, deltaY, deltaZ);
                            if (Configuration.Battle.PSXEncounterMethod)
                                distance = (Single)Math.Floor(distance);
                            this._encountTimer += distance;
                            if (!FF9StateSystem.Settings.IsNoEncounter && this.ProcessEncount(actor))
                                this._encountReserved = true;
                        }
                    }
                }
            }
        }
        if (this._collTimer > 0)
            --this._collTimer;
        if (EventEngine.sTalkTimer > 0 && --EventEngine.sTalkTimer != 0)
            this.ClearLookTalker();
        if (this._encountReserved && !this._posUsed)
        {
            this._encountReserved = false;
            result = 3;
        }
        if ((result == 3 || result == 7) && this.gMode == 1)
            this.BackupPosObjData();
        if (result == 7)
            this.sEventContext1.copy(this.sEventContext0);
        EMinigame.AllTreasureAchievement();
        EMinigame.AllSandyBeachAchievement();
        EMinigame.DigUpKupoAchievement();
        // In-game free camera mode: navigation
        if (this.sExternalFieldMode)
        {
            Boolean allow_camera_move = true;
            if (this.sExternalFieldFade >= 0)
            {
                if (this.sExternalFieldFade < 255 && this.sExternalFieldFade + 25 >= 255)
                {
                    this.sExternalFieldFade = 255;
                    if (this.sExternalFieldChangeField >= 0)
                    {
                        this.sExternalFieldNum = this.sExternalFieldChangeField;
                        FF9StateSystem.Common.FF9.fldMapNo = this.sExternalFieldList[this.sExternalFieldNum];
                        this.fieldmap.ChangeFieldMap(EventEngineUtils.eventIDToFBGID[this.sExternalFieldList[this.sExternalFieldNum]]);
                        if (this.sExternalFieldChangeCamera < 0)
                            this.fieldmap.SetCurrentCameraIndex(this.fieldmap.scene.cameraList.Count - 1);
                        else
                            this.fieldmap.SetCurrentCameraIndex(this.sExternalFieldChangeCamera);
                    }
                    else if (this.sExternalFieldChangeCamera >= 0)
                    {
                        this.fieldmap.SetCurrentCameraIndex(this.sExternalFieldChangeCamera);
                    }
                    else
                    {
                        this.fieldmap.ChangeFieldMap(this.sOriginalFieldName);
                        this.fieldmap.ActivateCamera();
                        foreach (GameObject field_object in this.sOriginalFieldGameObjects)
                        {
                            field_object.transform.parent = this.fieldmap.transform;
                            field_object.SetActive(true);
                        }
                        this.fieldmap.EBG_scene2DScrollRelease(1, 0);
                        FF9StateSystem.Common.FF9.fldMapNo = this.sOriginalFieldNo;
                        this.sExternalFieldList.Clear();
                        this.sOriginalFieldGameObjects.Clear();
                    }
                    BGCAM_DEF new_camera_view = this.fieldmap.scene.cameraList[this.fieldmap.camIdx];
                    this.fieldmap.EBG_scene2DScroll((Int16)((new_camera_view.vrpMinX + new_camera_view.vrpMaxX) / 2), (Int16)((new_camera_view.vrpMinY + new_camera_view.vrpMaxY) / 2), 1, 0);
                    //this.fieldmap.EBG_scene2DScroll((float)((new_camera_view.vrpMinX + new_camera_view.vrpMaxX) / 2), (float)((new_camera_view.vrpMinY + new_camera_view.vrpMaxY) / 2), 1, 0);
                }
                else if (this.sExternalFieldFade < 510 && this.sExternalFieldFade + 25 >= 510)
                {
                    this.sExternalFieldFade = -1;
                    Resources.UnloadUnusedAssets();
                    if (FF9StateSystem.Common.FF9.fldMapNo == this.sOriginalFieldNo)
                        this.sExternalFieldMode = false;
                }
                else
                {
                    this.sExternalFieldFade += 25;
                }
                if (this.sExternalFieldFade < 0)
                    SceneDirector.FadeEventSetColor(FadeMode.Sub, Color.black);
                else if (this.sExternalFieldFade < 256)
                    SceneDirector.FadeEventSetColor(FadeMode.Sub, new Color(this.sExternalFieldFade / 255f, this.sExternalFieldFade / 255f, this.sExternalFieldFade / 255f));
                else
                    SceneDirector.FadeEventSetColor(FadeMode.Sub, new Color((510 - this.sExternalFieldFade) / 255f, (510 - this.sExternalFieldFade) / 255f, (510 - this.sExternalFieldFade) / 255f));
            }
            else
            {
                if (UIManager.Input.GetKey(Control.RightBumper))
                {
                    if (this.fieldmap.camIdx + 1 < this.fieldmap.scene.cameraList.Count
                        && this.sExternalFieldList[this.sExternalFieldNum] != 61
                        && (this.sExternalFieldList[this.sExternalFieldNum] != 65 || this.fieldmap.camIdx < 2)
                        && this.sExternalFieldList[this.sExternalFieldNum] != 803
                        && this.sExternalFieldList[this.sExternalFieldNum] != 809
                        && this.sExternalFieldList[this.sExternalFieldNum] != 810
                        && this.sExternalFieldList[this.sExternalFieldNum] != 812
                        && this.sExternalFieldList[this.sExternalFieldNum] != 2553
                        && this.sExternalFieldList[this.sExternalFieldNum] != 2756)
                    {
                        this.sExternalFieldChangeField = -1;
                        this.sExternalFieldChangeCamera = this.fieldmap.camIdx + 1;
                        this.sExternalFieldFade = 0;
                    }
                    else if (this.sExternalFieldNum + 1 < this.sExternalFieldList.Count)
                    {
                        this.sExternalFieldChangeField = this.sExternalFieldNum + 1;
                        if (this.sExternalFieldList[this.sExternalFieldChangeField] == 51)
                            this.sExternalFieldChangeCamera = -1;
                        else
                            this.sExternalFieldChangeCamera = 0;
                        this.sExternalFieldFade = 0;
                    }
                    else
                    {
                        this.sExternalFieldChangeField = -1;
                        this.sExternalFieldChangeCamera = -1;
                        this.sExternalFieldFade = 0;
                    }
                    allow_camera_move = false;
                }
                else if (UIManager.Input.GetKey(Control.LeftBumper))
                {
                    if (this.fieldmap.camIdx > 0 && this.sExternalFieldList[this.sExternalFieldNum] != 51)
                    {
                        this.sExternalFieldChangeField = -1;
                        this.sExternalFieldChangeCamera = this.fieldmap.camIdx - 1;
                        this.sExternalFieldFade = 0;
                        allow_camera_move = false;
                    }
                    else if (this.sExternalFieldNum > 0)
                    {
                        this.sExternalFieldChangeField = this.sExternalFieldNum - 1;
                        if (this.sExternalFieldList[this.sExternalFieldChangeField] == 61
                            || this.sExternalFieldList[this.sExternalFieldChangeField] == 803
                            || this.sExternalFieldList[this.sExternalFieldChangeField] == 809
                            || this.sExternalFieldList[this.sExternalFieldChangeField] == 810
                            || this.sExternalFieldList[this.sExternalFieldChangeField] == 812
                            || this.sExternalFieldList[this.sExternalFieldChangeField] == 2553
                            || this.sExternalFieldList[this.sExternalFieldChangeField] == 2756)
                            this.sExternalFieldChangeCamera = 0;
                        else if (this.sExternalFieldList[this.sExternalFieldChangeField] == 65)
                            this.sExternalFieldChangeCamera = 2;
                        else
                            this.sExternalFieldChangeCamera = -1;
                        this.sExternalFieldFade = 0;
                        allow_camera_move = false;
                    }
                }
                else if (UIManager.Input.GetKey(Control.Select))
                {
                    this.sExternalFieldChangeField = -1;
                    this.sExternalFieldChangeCamera = -1;
                    this.sExternalFieldFade = 0;
                    allow_camera_move = false;
                }
                if (allow_camera_move)
                {
                    Vector2 new_camera_position = this.fieldmap.curVRP;
                    Vector2 cameraMove;
                    BGCAM_DEF bgCamera = this.fieldmap.scene.cameraList[this.fieldmap.camIdx];
                    new_camera_position[0] += FieldMap.HalfFieldWidth + bgCamera.centerOffset[0];
                    new_camera_position[1] += FieldMap.HalfFieldHeight - bgCamera.centerOffset[1];
                    if (FF9StateSystem.MobilePlatform)
                        cameraMove = VirtualAnalog.HasInput() ? VirtualAnalog.GetAnalogValue() : PersistenSingleton<HonoInputManager>.Instance.GetAxis();
                    else
                        cameraMove = PersistenSingleton<HonoInputManager>.Instance.GetAxis();
                    if (UIManager.Input.GetKey(Control.Up))
                        cameraMove.y = 1f;
                    else if (UIManager.Input.GetKey(Control.Down))
                        cameraMove.y = -1f;
                    if (UIManager.Input.GetKey(Control.Left))
                        cameraMove.x = -1f;
                    else if (UIManager.Input.GetKey(Control.Right))
                        cameraMove.x = 1f;
                    if (Mathf.Abs(cameraMove.y) > Configuration.AnalogControl.StickThreshold)
                        new_camera_position.y -= cameraMove.y * 5f;
                    if (Mathf.Abs(cameraMove.x) > Configuration.AnalogControl.StickThreshold)
                        new_camera_position.x += cameraMove.x * 5f;
                    new_camera_position.x = Mathf.Clamp(new_camera_position.x, bgCamera.vrpMinX, bgCamera.vrpMaxX);
                    new_camera_position.y = Mathf.Clamp(new_camera_position.y, bgCamera.vrpMinY, bgCamera.vrpMaxY);
                    //this.fieldmap.EBG_scene2DScroll(new_camera_position[0], new_camera_position[1], 1, 0);
                    this.fieldmap.EBG_scene2DScroll((Int16)new_camera_position[0], (Int16)new_camera_position[1], 1, 0);
                }
            }
        }
        //this.printActorsInObjList(this.E.activeObj);
        return result;
    }

    private void ClearLookTalker()
    {
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if ((Int32)obj.cid == 4)
            {
                Actor actor = (Actor)obj;
                if (((Int32)actor.actf & EventEngine.actLookTalker) != 0)
                    actor.actf = (UInt16)((UInt32)actor.actf | (UInt32)EventEngine.actLookedTalker);
            }
        }
        EventEngine.sLastTalker = (PosObj)null;
    }

    private Boolean ProcessEncount(PosObj po)
    {
        Int32 SceneNo = 0;
        if (this._context.usercontrol != 0 && (this._encountTimer > Configuration.Battle.EncounterInterval || SettingsState.IsRapidEncounter))
        {
            this._encountTimer = 0.0f;
            this._encountBase += this._context.encratio;
            if (Comn.random8() < this._encountBase >> 3)
            {
                this._encountBase = 0;
                SceneNo = this.SelectScene();
                if (SceneNo == 0) return false;
                if (SceneNo == this._lastScene)
                    SceneNo = this.SelectScene();
                this._lastScene = SceneNo;
                this.SetBattleScene(SceneNo);
                this._ff9.btlSubMapNo = -1;
                FF9StateSystem.Battle.isRandomEncounter = true;
                FF9StateSystem.Battle.isEncount = true;
            }
        }
        return SceneNo != 0;
    }

    private void SetRenderer(Obj obj, Boolean isBattle)
    {
        if (!this.isPosObj(obj))
            return;
        PosObj posObj = (PosObj)obj;
        if (!((UnityEngine.Object)posObj.go != (UnityEngine.Object)null))
            return;
        Boolean flag1 = (Int32)obj.flags != obj.tempFlag;
        Boolean flag2 = (Int32)posObj.meshflags != (Int32)posObj.tempMeshflags;
        Boolean flag3 = posObj.frontCamera != posObj.tempfrontCamera;
        Int32 meshCount = posObj.geoGetMeshCount();
        Boolean flag4 = ((Int32)obj.flags & 1) == 1;
        Boolean flag5 = posObj.frontCamera;
        Int32 num1 = 0;
        if ((UnityEngine.Object)posObj.go.GetComponent<HonoBehavior>() != (UnityEngine.Object)null)
        {
            if (flag1 || flag2 || flag3)
            {
                if (flag4 && flag5)
                {
                    for (Int32 mesh = 0; mesh < meshCount; ++mesh)
                    {
                        Boolean flag6 = (Int32)posObj.geoMeshChkFlags(mesh) == 0;
                        posObj.SetIsEnabledMeshRenderer(mesh, flag6 && flag4);
                        if (flag6 && flag4)
                            ++num1;
                    }
                    foreach (FieldMapActor componentsInChild in posObj.go.GetComponentsInChildren<FieldMapActor>())
                        componentsInChild.actor.tempFlag = -1;
                }
                else
                {
                    Actor actor = (Actor)obj;
                    foreach (Renderer componentsInChild in posObj.go.GetComponentsInChildren<Renderer>())
                    {
                        if ((Int32)FF9StateSystem.Common.FF9.fldMapNo != 52 || (Int32)actor.sid != 6 && (Int32)actor.sid != 13 || this.eBin.getVarManually(6357) == 0)
                            componentsInChild.enabled = false;
                    }
                }
                obj.tempFlag = (Int32)obj.flags;
                posObj.tempMeshflags = posObj.meshflags;
                posObj.tempfrontCamera = posObj.frontCamera;
                if (isBattle)
                {
                    Transform child1 = posObj.go.transform.FindChild("field_model");
                    if ((UnityEngine.Object)child1 != (UnityEngine.Object)null)
                    {
                        foreach (Renderer componentsInChild in child1.GetComponentsInChildren<Renderer>())
                            componentsInChild.enabled = false;
                    }
                    if (num1 == meshCount)
                    {
                        Transform child2 = posObj.go.transform.FindChild("battle_model");
                        if ((UnityEngine.Object)child2 != (UnityEngine.Object)null)
                        {
                            foreach (Renderer componentsInChild in child2.GetComponentsInChildren<Renderer>())
                                componentsInChild.enabled = true;
                        }
                    }
                }
                else
                {
                    Transform child1 = posObj.go.transform.FindChild("battle_model");
                    if ((UnityEngine.Object)child1 != (UnityEngine.Object)null)
                    {
                        foreach (Renderer componentsInChild in child1.GetComponentsInChildren<Renderer>())
                            componentsInChild.enabled = false;
                    }
                    if (num1 == meshCount)
                    {
                        Transform child2 = posObj.go.transform.FindChild("field_model");
                        if ((UnityEngine.Object)child2 != (UnityEngine.Object)null)
                        {
                            foreach (Renderer componentsInChild in child2.GetComponentsInChildren<Renderer>())
                                componentsInChild.enabled = true;
                        }
                    }
                }
            }
            if (this.gMode != 1)
                return;
            Actor actor1 = (Actor)obj;
            Dictionary<Int32, FF9Shadow> dictionary = FF9StateSystem.Field.FF9Field.loc.map.shadowArray;
            if (!dictionary.ContainsKey((Int32)actor1.uid))
                return;
            FF9Shadow ff9Shadow = dictionary[(Int32)actor1.uid];
            MeshRenderer meshRenderer = actor1.fieldMapActor.shadowMeshRenderer;
            Transform transform = actor1.fieldMapActor.shadowTran;
            if (ff9Shadow.needUpdate || flag1)
            {
                ff9Shadow.needUpdate = false;
                Boolean flag6 = flag4 && ((Int32)FF9StateSystem.Common.FF9.FF9GetCharPtr((Int32)obj.uid).attr & 16) == 0;
                meshRenderer.enabled = flag6;
                transform.localScale = (Int32)FF9StateSystem.Common.FF9.fldMapNo != 2913 || (Int32)actor1.uid != 6 ? new Vector3(ff9Shadow.xScale, 1f, ff9Shadow.zScale) : new Vector3(ff9Shadow.xScale, 1f, ff9Shadow.zScale) * actor1.fieldMapActor.transform.localScale.z;
            }
            if (!ff9Shadow.needUpdateAmp || !meshRenderer.enabled)
                return;
            ff9Shadow.needUpdateAmp = false;
            Byte num2 = (Byte)(ff9Shadow.amp * 2);
            meshRenderer.material.SetColor("_Color", (Color)new Color32(num2, num2, num2, Byte.MaxValue));
        }
        else
        {
            if (!(obj.isEnableRenderer ^ flag4))
                return;
            foreach (Renderer componentsInChild in obj.go.GetComponentsInChildren<Renderer>())
                componentsInChild.enabled = flag4;
            obj.isEnableRenderer = flag4;
        }
    }

    private void SetupBattleState()
    {
        if (FF9StateSystem.Battle.FF9Battle.btl_phase == FF9StateBattleSystem.PHASE_INIT_SYSTEM)
            return;
        Int32 sysList = this.GetSysList(3);
        for (Int32 index = 4; index < 8; ++index)
        {
            Obj obj = this._objPtrList[index];
            if (obj != null)
            {
                if ((Int32)obj.state == (Int32)EventEngine.stateRunning)
                {
                    if ((sysList & 16) == 0)
                        obj.state = EventEngine.stateSuspend;
                }
                else if ((Int32)obj.state == (Int32)EventEngine.stateSuspend && (sysList & 16) != 0)
                    obj.state = EventEngine.stateRunning;
            }
            sysList >>= 1;
        }
    }
}

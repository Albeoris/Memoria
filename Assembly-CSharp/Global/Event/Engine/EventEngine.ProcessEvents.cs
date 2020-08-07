using System;
using System.Collections.Generic;
using FF9;
using Memoria;
using UnityEngine;
using Assets.Scripts.Common;
// ReSharper disable ClassNeverInstantiated.Global

public partial class EventEngine
{
    public Int32 ProcessEvents()
    {
        if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 303 || (Int32)FF9StateSystem.Common.FF9.fldMapNo == 304)
        {
            if (FF9StateSystem.Settings.IsNoEncounter)
            {
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(53189, 0);
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(52933, 0);
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(52677, 0);
            }
        }
        else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 301)
        {
            if (FF9StateSystem.Settings.IsNoEncounter)
            {
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(61381, 0);
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(61125, 0);
            }
        }
        else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 302)
        {
            if (FF9StateSystem.Settings.IsNoEncounter)
            {
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(53189, 0);
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(52933, 0);
            }
        }
        else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2921 && FF9StateSystem.Settings.IsNoEncounter)
            PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(10461, 10);
        Boolean flag = false;
        Boolean isBattle = this.gMode == 2;
        this._moveKey = false;
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if ((Int32)obj.cid == 4)
            {
                Actor actor = (Actor)obj;
                FieldMapActorController mapActorController = actor.fieldMapActorController;
                if ((Int32)obj.uid == (Int32)this._context.controlUID && (Int32)obj.state == (Int32)EventEngine.stateRunning)
                {
                    if ((Int32)this._context.usercontrol != 0)
                    {
                        if (this.gMode == 1)
                        {
                            //if (!((UnityEngine.Object)mapActorController != (UnityEngine.Object)null))
                            //    ;
                            this._moveKey = FieldMapActorController.ccSMoveKey;
                        }
                        else if (this.gMode == 3)
                        {
                            Single distance = 0.0f;
                            this._moveKey = ff9.w_frameEncountEnable;
                            this.CollisionRequest((PosObj)obj);
                            PosObj posObj = (PosObj)this.Collision(this, actor, 0, ref distance);
                            if (posObj != null && distance <= 0f)
                            {
                                WMActor wmActor = actor.wmActor;
                                WMActor wmActor2 = ((Actor)posObj).wmActor;
                                Vector2 from = new Vector2(wmActor.pos0 - wmActor.lastx, wmActor.pos2 - wmActor.lastz);
                                Vector2 to = new Vector2(wmActor2.pos0 - wmActor.pos0, wmActor2.pos2 - wmActor.pos2);
                                float num2 = Vector2.Angle(from, to);
                                if (num2 >= 0f && num2 <= 90f)
                                {
                                    wmActor.transform.position = new Vector3(wmActor.lastx, wmActor.lasty, wmActor.lastz);
                                }
                            }
                        }
                        if (this._moveKey)
                            this.ResetIdleTimer(0);
                    }
                    else if ((UnityEngine.Object)mapActorController != (UnityEngine.Object)null)
                        mapActorController.CopyLastPos();
                }
                else if ((UnityEngine.Object)mapActorController != (UnityEngine.Object)null)
                    mapActorController.CopyLastPos();
                if ((Int32)obj.state == (Int32)EventEngine.stateRunning)
                {
                    if (!isBattle)
                        this.ProcessAnime((Actor)obj);
                    //if ((Int32)this._context.usercontrol == 0)
                    //    ;
                }
            }
        }
        if (isBattle)
            this.SetupBattleState();
        
        this._posUsed = false;
        
        // TODO Check Native: #147
        Int32 num1 = 0;
        bool canProcessCode = true;

        if (_ff9.fldMapNo == 257)
            canProcessCode = (!Singleton<DialogManager>.Instance.Activate || Singleton<DialogManager>.Instance.CompletlyVisible);
        
        if (canProcessCode)
            num1 = this.eBin.ProcessCode(this._context.activeObj);
        
        EventHUD.CheckUIMiniGameForMobile();
        if (num1 == 6)
            num1 = 0;
        else
            this.gStopObj = null;
        
        this._aimObj = null;
        this._eyeObj = (PosObj)null;
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
            this.SetRenderer(objList.obj, isBattle);
        //if (this.gMode != 1)
        //    ;
        if (isBattle)
        {
            for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
            {
                Obj p = objList.obj;
                if ((Int32)p.btlchk == 2 && this.Request(p, 3, 5, false))
                    p.btlchk = (Byte)1;
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
                if ((Int32)obj.cid == 4)
                {
                    Actor actor = (Actor)obj;
                    if (this.gMode == 1)
                    {
                        if ((UnityEngine.Object)obj.go != (UnityEngine.Object)null)
                        {
                            FieldMapActorController component = obj.go.GetComponent<FieldMapActorController>();
                            if ((UnityEngine.Object)component != (UnityEngine.Object)null)
                            {
                                Int32 num2 = 0;
                                if ((Int32)actor.cid != 4 || (Int32)actor.model == (Int32)UInt16.MaxValue)
                                    num2 = 0;
                                //else if (component.walkMesh == null)
                                //    ;
                                if (((Int32)obj.uid != (Int32)this._context.controlUID || this.sLockTimer >= 0L) && (num2 & 1) != 0)
                                {
                                    Single distance = 0.0f;
                                    PosObj posObj = this.fieldmap.walkMesh.Collision(actor.fieldMapActorController, 0, out distance);
                                    if (posObj != null)
                                    {
                                        actor.fieldMapActorController.ResetPos();
                                        if (((Int32)posObj.flags & 16) != 0)
                                            this.sLockTimer = 0L;
                                        if ((Int32)obj.uid == (Int32)this._context.controlUID)
                                            this._collTimer = 2;
                                    }
                                }
                            }
                        }
                        if ((Int32)obj.uid == (Int32)this._context.controlUID)
                        {
                            EIcon.ProcessHereIcon((PosObj)actor);
                            if (this.GetUserControl())
                                this.CollisionRequest((PosObj)actor);
                        }
                    }
                    else if (this.gMode == 3)
                    {
                        if (((Int32)actor.actf & EventEngine.actEye) != 0)
                        {
                            Vector3 eyePtr = ff9.w_cameraGetEyePtr();
                            eyePtr.x = actor.pos[0];
                            eyePtr.y = actor.pos[1];
                            eyePtr.z = actor.pos[2];
                            ff9.w_cameraSetEyePtr(eyePtr);
                            this._eyeObj = (PosObj)actor;
                        }
                        else if (((Int32)actor.actf & EventEngine.actAim) != 0)
                        {
                            Vector3 aimPtr = ff9.w_cameraGetAimPtr();
                            aimPtr.x = actor.pos[0];
                            aimPtr.y = actor.pos[1];
                            aimPtr.z = actor.pos[2];
                            ff9.w_cameraSetAimPtr(aimPtr);
                            this._aimObj = (PosObj)actor;
                        }
                    }
                    if ((UnityEngine.Object)obj.go != (UnityEngine.Object)null)
                    {
                        if (this.gMode == 3)
                        {
                            WMActor wmActor = ((Actor)obj).wmActor;
                            if ((UnityEngine.Object)wmActor != (UnityEngine.Object)null)
                            {
                                deltaX = (Single)(256.0 * ((Double)wmActor.pos0 - (Double)wmActor.lastx));
                                deltaY = (Single)(256.0 * ((Double)wmActor.pos1 - (Double)wmActor.lasty));
                                deltaZ = (Single)(256.0 * ((Double)wmActor.pos2 - (Double)wmActor.lastz));
                                flag = !EventEngineUtils.nearlyEqual(wmActor.pos0, wmActor.lastx) || !EventEngineUtils.nearlyEqual(wmActor.pos2, wmActor.lastz);
                                //if ((Int32)obj.sid != 5 && (Int32)obj.sid == 11)
                                //    ;
                            }
                        }
                        else if (this.gMode == 1)
                        {
                            FieldMapActorController component = obj.go.GetComponent<FieldMapActorController>();
                            deltaX = component.curPos.x - component.lastPos.x;
                            deltaY = component.curPos.y - component.lastPos.y;
                            deltaZ = component.curPos.z - component.lastPos.z;
                            flag = !EventEngineUtils.nearlyEqual(component.curPos.x, component.lastPos.x) || !EventEngineUtils.nearlyEqual(component.curPos.z, component.lastPos.z);
                        }
                        if ((Int32)actor.follow != (Int32)Byte.MaxValue && (UnityEngine.Object)Singleton<DialogManager>.Instance.GetDialogByWindowID((Int32)actor.follow) == (UnityEngine.Object)null)
                        {
                            actor.follow = Byte.MaxValue;
                            if (EventEngine.sLastTalker == actor)
                                EventEngine.sTalkTimer = 30;
                        }
                        if (this.gMode == 1)
                        {
                            this.ProcessTurn(actor);
                            if ((Int32)actor.model != (Int32)UInt16.MaxValue)
                                this.ProcessNeck(actor);
                        }
                        if (this.gMode == 3)
                            this.ProcessTurn(actor);
                        if (flag)
                        {
                            Int32 num2 = (Int32)actor.animFlag & (EventEngine.afExec | EventEngine.afLower | EventEngine.afFreeze);
                            if (num2 == 0 || num2 == (EventEngine.afExec | EventEngine.afLower))
                            {
                                actor.animFlag &= (Byte)~(EventEngine.afExec | EventEngine.afLower);
                                this.SetAnim(actor, (Int32)actor.speed < (Int32)actor.speedth ? (Int32)actor.walk : (Int32)actor.run);
                            }
                        }
                        else if (((Int32)actor.animFlag & (EventEngine.afExec | EventEngine.afFreeze)) == 0 && (this._collTimer == 0 || (Int32)obj.uid != (Int32)this._context.controlUID))
                            this.SetAnim(actor, (Int32)actor.idle);
                        if (flag && (Int32)obj.uid == (Int32)this._context.controlUID && this._moveKey)
                        {
                            Single distance = this.distance(deltaX, deltaY, deltaZ);
                            if (this.gMode == 3)
                            {
                                WMActor wmActor = ((Actor)obj).wmActor;
                            }
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
            num1 = 3;
        }
        if ((num1 == 3 || num1 == 7) && this.gMode == 1)
            this.BackupPosObjData();
        if (num1 == 7)
            this.sEventContext1.copy(this.sEventContext0);
        EMinigame.AllTreasureAchievement();
        EMinigame.AllSandyBeachAchievement();
        EMinigame.DigUpKupoAchievement();
        // In-game free camera mode: navigation
        if (this.sExternalFieldMode)
        {
            bool allow_camera_move = true;
            if (this.sExternalFieldFade >= 0)
            {
                if (this.sExternalFieldFade < 255 && this.sExternalFieldFade + 25 >= 255)
                {
                    this.sExternalFieldFade = 255;
                    if (this.sExternalFieldChangeField >= 0)
                    {
                        this.sExternalFieldNum = this.sExternalFieldChangeField;
                        this.fieldmap.ChangeFieldMap(EventEngineUtils.eventIDToFBGID[this.sExternalFieldList[this.sExternalFieldNum]]);
                        if (this.sExternalFieldChangeCamera < 0)
                            this.fieldmap.SetCurrentCameraIndex(this.fieldmap.scene.cameraList.Count - 1);
                        else
                            this.fieldmap.SetCurrentCameraIndex(this.sExternalFieldChangeCamera);
                        FF9StateSystem.Common.FF9.fldMapNo = this.sExternalFieldList[this.sExternalFieldNum];
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
                    this.fieldmap.EBG_scene2DScroll((short)((new_camera_view.vrpMinX + new_camera_view.vrpMaxX) / 2), (short)((new_camera_view.vrpMinY + new_camera_view.vrpMaxY) / 2), 1, 0);
                }
                else if (this.sExternalFieldFade < 510 && this.sExternalFieldFade + 25 >= 510)
                {
                    this.sExternalFieldFade = -1;
                    Resources.UnloadUnusedAssets();
                    if (FF9StateSystem.Common.FF9.fldMapNo == this.sOriginalFieldNo)
                        this.sExternalFieldMode = false;
                }
                else
                    this.sExternalFieldFade += 25;
                if (this.sExternalFieldFade < 0)
                    SceneDirector.FadeEventSetColor(FadeMode.Add, Color.black);
                else if (this.sExternalFieldFade < 256)
                    SceneDirector.FadeEventSetColor(FadeMode.Add, new Color((float)this.sExternalFieldFade / 255f, (float)this.sExternalFieldFade / 255f, (float)this.sExternalFieldFade / 255f));
                else
                    SceneDirector.FadeEventSetColor(FadeMode.Add, new Color((float)(510 - this.sExternalFieldFade) / 255f, (float)(510 - this.sExternalFieldFade) / 255f, (float)(510 - this.sExternalFieldFade) / 255f));
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
                    new_camera_position[0] += 160f + this.fieldmap.scene.cameraList[this.fieldmap.camIdx].centerOffset[0];
                    new_camera_position[1] += 112f - this.fieldmap.scene.cameraList[this.fieldmap.camIdx].centerOffset[1];
                    Vector2 vector = Vector2.zero;
                    if (FF9StateSystem.MobilePlatform)
                    {
                        if (VirtualAnalog.HasInput())
                            vector = VirtualAnalog.GetAnalogValue();
                        else
                            vector = PersistenSingleton<HonoInputManager>.Instance.GetAxis();
                    }
                    else
                    {
                        vector = PersistenSingleton<HonoInputManager>.Instance.GetAxis();
                    }
                    if (UIManager.Input.GetKey(Control.Up))
                        vector.y = 1f;
                    else if (UIManager.Input.GetKey(Control.Down))
                        vector.y = -1f;
                    if (UIManager.Input.GetKey(Control.Left))
                        vector.x = -1f;
                    else if (UIManager.Input.GetKey(Control.Right))
                        vector.x = 1f;
                    if (Mathf.Abs(vector.y) > 0.1f)
                        new_camera_position.y -= vector.y * 5f;
                    if (Mathf.Abs(vector.x) > 0.1f)
                        new_camera_position.x += vector.x * 5f;
                    if (new_camera_position.x < (float)this.fieldmap.scene.cameraList[this.fieldmap.camIdx].vrpMinX)
                        new_camera_position.x = (float)this.fieldmap.scene.cameraList[this.fieldmap.camIdx].vrpMinX;
                    else if (new_camera_position.x > (float)this.fieldmap.scene.cameraList[this.fieldmap.camIdx].vrpMaxX)
                        new_camera_position.x = (float)this.fieldmap.scene.cameraList[this.fieldmap.camIdx].vrpMaxX;
                    if (new_camera_position.y < (float)this.fieldmap.scene.cameraList[this.fieldmap.camIdx].vrpMinY)
                        new_camera_position.y = (float)this.fieldmap.scene.cameraList[this.fieldmap.camIdx].vrpMinY;
                    else if (new_camera_position.y > (float)this.fieldmap.scene.cameraList[this.fieldmap.camIdx].vrpMaxY)
                        new_camera_position.y = (float)this.fieldmap.scene.cameraList[this.fieldmap.camIdx].vrpMaxY;
                    this.fieldmap.EBG_scene2DScroll((short)new_camera_position[0], (short)new_camera_position[1], 1, 0);
                }
            }
        }
        //this.printActorsInObjList(this.E.activeObj);
        return num1;
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
        if ((Int32)this._context.usercontrol != 0 && (this._encountTimer > Configuration.Battle.EncounterInterval || SettingsState.IsRapidEncounter))
        {
            this._encountTimer = 0.0f;
            this._encountBase += (Int32)this._context.encratio;
            if (Comn.random8() < this._encountBase >> 3)
            {
                this._encountBase = 0;
                SceneNo = this.SelectScene();
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
        if ((Int32)FF9StateSystem.Battle.FF9Battle.btl_phase == 0)
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
using System.Collections.Generic;
using FF9;
using Memoria;
using UnityEngine;

public partial class EventEngine
{
    public int ProcessEvents()
    {
        if ((int)FF9StateSystem.Common.FF9.fldMapNo == 303 || (int)FF9StateSystem.Common.FF9.fldMapNo == 304)
        {
            if (FF9StateSystem.Settings.IsNoEncounter)
            {
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(53189, 0);
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(52933, 0);
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(52677, 0);
            }
        }
        else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 301)
        {
            if (FF9StateSystem.Settings.IsNoEncounter)
            {
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(61381, 0);
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(61125, 0);
            }
        }
        else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 302)
        {
            if (FF9StateSystem.Settings.IsNoEncounter)
            {
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(53189, 0);
                PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(52933, 0);
            }
        }
        else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2921 && FF9StateSystem.Settings.IsNoEncounter)
            PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(10461, 10);
        bool flag = false;
        bool isBattle = this.gMode == 2;
        this._moveKey = false;
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if ((int)obj.cid == 4)
            {
                Actor actor = (Actor)obj;
                FieldMapActorController mapActorController = actor.fieldMapActorController;
                if ((int)obj.uid == (int)this._context.controlUID && (int)obj.state == (int)EventEngine.stateRunning)
                {
                    if ((int)this._context.usercontrol != 0)
                    {
                        if (this.gMode == 1)
                        {
                            if (!((UnityEngine.Object)mapActorController != (UnityEngine.Object)null))
                                ;
                            this._moveKey = FieldMapActorController.ccSMoveKey;
                        }
                        else if (this.gMode == 3)
                        {
                            float distance = 0.0f;
                            this._moveKey = ff9.w_frameEncountEnable;
                            this.CollisionRequest((PosObj)obj);
                            PosObj posObj = (PosObj)this.Collision(this, (PosObj)actor, 0, ref distance);
                            if (posObj != null)
                            {
                                WMActor wmActor = actor.wmActor;
                                if (posObj.objParent == null || (int)posObj.objParent.sid != (int)obj.sid)
                                    wmActor.transform.position = new Vector3(wmActor.lastx, wmActor.lasty, wmActor.lastz);
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
                if ((int)obj.state == (int)EventEngine.stateRunning)
                {
                    if (!isBattle)
                        this.ProcessAnime((Actor)obj);
                    if ((int)this._context.usercontrol == 0)
                        ;
                }
            }
        }
        if (isBattle)
            this.SetupBattleState();
        this._posUsed = false;
        int num1 = this.eBin.ProcessCode(this._context.activeObj);
        EventHUD.CheckUIMiniGameForMobile();
        if (num1 == 6)
            num1 = 0;
        else
            this.gStopObj = (ObjList)null;
        this._aimObj = (PosObj)null;
        this._eyeObj = (PosObj)null;
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
            this.SetRenderer(objList.obj, isBattle);
        if (this.gMode != 1)
            ;
        if (isBattle)
        {
            for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
            {
                Obj p = objList.obj;
                if ((int)p.btlchk == 2 && this.Request(p, 3, 5, false))
                    p.btlchk = (byte)1;
            }
        }
        else
        {
            for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
            {
                Obj obj = objList.obj;
                float deltaX = 0.0f;
                float deltaY = 0.0f;
                float deltaZ = 0.0f;
                if ((int)obj.cid == 4)
                {
                    Actor actor = (Actor)obj;
                    if (this.gMode == 1)
                    {
                        if ((UnityEngine.Object)obj.go != (UnityEngine.Object)null)
                        {
                            FieldMapActorController component = obj.go.GetComponent<FieldMapActorController>();
                            if ((UnityEngine.Object)component != (UnityEngine.Object)null)
                            {
                                int num2 = 0;
                                if ((int)actor.cid != 4 || (int)actor.model == (int)ushort.MaxValue)
                                    num2 = 0;
                                else if (component.walkMesh == null)
                                    ;
                                if (((int)obj.uid != (int)this._context.controlUID || this.sLockTimer >= 0L) && (num2 & 1) != 0)
                                {
                                    float distance = 0.0f;
                                    PosObj posObj = this.fieldmap.walkMesh.Collision(actor.fieldMapActorController, 0, out distance);
                                    if (posObj != null)
                                    {
                                        actor.fieldMapActorController.ResetPos();
                                        if (((int)posObj.flags & 16) != 0)
                                            this.sLockTimer = 0L;
                                        if ((int)obj.uid == (int)this._context.controlUID)
                                            this._collTimer = 2;
                                    }
                                }
                            }
                        }
                        if ((int)obj.uid == (int)this._context.controlUID)
                        {
                            EIcon.ProcessHereIcon((PosObj)actor);
                            if (this.GetUserControl())
                                this.CollisionRequest((PosObj)actor);
                        }
                    }
                    else if (this.gMode == 3)
                    {
                        if (((int)actor.actf & EventEngine.actEye) != 0)
                        {
                            Vector3 eyePtr = ff9.w_cameraGetEyePtr();
                            eyePtr.x = actor.pos[0];
                            eyePtr.y = actor.pos[1];
                            eyePtr.z = actor.pos[2];
                            ff9.w_cameraSetEyePtr(eyePtr);
                            this._eyeObj = (PosObj)actor;
                        }
                        else if (((int)actor.actf & EventEngine.actAim) != 0)
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
                                deltaX = (float)(256.0 * ((double)wmActor.pos0 - (double)wmActor.lastx));
                                deltaY = (float)(256.0 * ((double)wmActor.pos1 - (double)wmActor.lasty));
                                deltaZ = (float)(256.0 * ((double)wmActor.pos2 - (double)wmActor.lastz));
                                flag = !EventEngineUtils.nearlyEqual(wmActor.pos0, wmActor.lastx) || !EventEngineUtils.nearlyEqual(wmActor.pos2, wmActor.lastz);
                                if ((int)obj.sid != 5 && (int)obj.sid == 11)
                                    ;
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
                        if ((int)actor.follow != (int)byte.MaxValue && (UnityEngine.Object)Singleton<DialogManager>.Instance.GetDialogByWindowID((int)actor.follow) == (UnityEngine.Object)null)
                        {
                            actor.follow = byte.MaxValue;
                            if (EventEngine.sLastTalker == actor)
                                EventEngine.sTalkTimer = 30;
                        }
                        if (this.gMode == 1)
                        {
                            this.ProcessTurn(actor);
                            if ((int)actor.model != (int)ushort.MaxValue)
                                this.ProcessNeck(actor);
                        }
                        if (this.gMode == 3)
                            this.ProcessTurn(actor);
                        if (flag)
                        {
                            int num2 = (int)actor.animFlag & (EventEngine.afExec | EventEngine.afLower | EventEngine.afFreeze);
                            if (num2 == 0 || num2 == (EventEngine.afExec | EventEngine.afLower))
                            {
                                actor.animFlag &= (byte)~(EventEngine.afExec | EventEngine.afLower);
                                this.SetAnim(actor, (int)actor.speed < (int)actor.speedth ? (int)actor.walk : (int)actor.run);
                            }
                        }
                        else if (((int)actor.animFlag & (EventEngine.afExec | EventEngine.afFreeze)) == 0 && (this._collTimer == 0 || (int)obj.uid != (int)this._context.controlUID))
                            this.SetAnim(actor, (int)actor.idle);
                        if (flag && (int)obj.uid == (int)this._context.controlUID && this._moveKey)
                        {
                            float num2 = this.distance(deltaX, deltaY, deltaZ);
                            if (this.gMode == 3)
                            {
                                WMActor wmActor = ((Actor)obj).wmActor;
                            }
                            this._encountTimer += num2;
                            if (!FF9StateSystem.Settings.IsNoEncounter && this.ProcessEncount((PosObj)actor))
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
        //this.printActorsInObjList(this.E.activeObj);
        return num1;
    }

    private void ClearLookTalker()
    {
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if ((int)obj.cid == 4)
            {
                Actor actor = (Actor)obj;
                if (((int)actor.actf & EventEngine.actLookTalker) != 0)
                    actor.actf = (ushort)((uint)actor.actf | (uint)EventEngine.actLookedTalker);
            }
        }
        EventEngine.sLastTalker = (PosObj)null;
    }

    private bool ProcessEncount(PosObj po)
    {
        int SceneNo = 0;
        if ((int)this._context.usercontrol != 0 && (double)this._encountTimer > 960.0)
        {
            this._encountTimer = 0.0f;
            this._encountBase += (int)this._context.encratio;
            if (Comn.random8() < this._encountBase >> 3)
            {
                this._encountBase = 0;
                SceneNo = this.SelectScene();
                if (SceneNo == this._lastScene)
                    SceneNo = this.SelectScene();
                this._lastScene = SceneNo;
                this.SetBattleScene(SceneNo);
                this._ff9.btlSubMapNo = (sbyte)-1;
                FF9StateSystem.Battle.isRandomEncounter = true;
            }
        }
        return SceneNo != 0;
    }

    private void SetRenderer(Obj obj, bool isBattle)
    {
        if (!this.isPosObj(obj))
            return;
        PosObj posObj = (PosObj)obj;
        if (!((UnityEngine.Object)posObj.go != (UnityEngine.Object)null))
            return;
        bool flag1 = (int)obj.flags != obj.tempFlag;
        bool flag2 = (int)posObj.meshflags != (int)posObj.tempMeshflags;
        bool flag3 = posObj.frontCamera != posObj.tempfrontCamera;
        int meshCount = posObj.geoGetMeshCount();
        bool flag4 = ((int)obj.flags & 1) == 1;
        bool flag5 = posObj.frontCamera;
        int num1 = 0;
        if ((UnityEngine.Object)posObj.go.GetComponent<HonoBehavior>() != (UnityEngine.Object)null)
        {
            if (flag1 || flag2 || flag3)
            {
                if (flag4 && flag5)
                {
                    for (int mesh = 0; mesh < meshCount; ++mesh)
                    {
                        bool flag6 = (int)posObj.geoMeshChkFlags(mesh) == 0;
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
                        if ((int)FF9StateSystem.Common.FF9.fldMapNo != 52 || (int)actor.sid != 6 && (int)actor.sid != 13 || this.eBin.getVarManually(6357) == 0)
                            componentsInChild.enabled = false;
                    }
                }
                obj.tempFlag = (int)obj.flags;
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
            Dictionary<int, FF9Shadow> dictionary = FF9StateSystem.Field.FF9Field.loc.map.shadowArray;
            if (!dictionary.ContainsKey((int)actor1.uid))
                return;
            FF9Shadow ff9Shadow = dictionary[(int)actor1.uid];
            MeshRenderer meshRenderer = actor1.fieldMapActor.shadowMeshRenderer;
            Transform transform = actor1.fieldMapActor.shadowTran;
            if (ff9Shadow.needUpdate || flag1)
            {
                ff9Shadow.needUpdate = false;
                bool flag6 = flag4 && ((int)FF9StateSystem.Common.FF9.FF9GetCharPtr((int)obj.uid).attr & 16) == 0;
                meshRenderer.enabled = flag6;
                transform.localScale = (int)FF9StateSystem.Common.FF9.fldMapNo != 2913 || (int)actor1.uid != 6 ? new Vector3(ff9Shadow.xScale, 1f, ff9Shadow.zScale) : new Vector3(ff9Shadow.xScale, 1f, ff9Shadow.zScale) * actor1.fieldMapActor.transform.localScale.z;
            }
            if (!ff9Shadow.needUpdateAmp || !meshRenderer.enabled)
                return;
            ff9Shadow.needUpdateAmp = false;
            byte num2 = (byte)(ff9Shadow.amp * 2);
            meshRenderer.material.SetColor("_Color", (Color)new Color32(num2, num2, num2, byte.MaxValue));
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
        if ((int)FF9StateSystem.Battle.FF9Battle.btl_phase == 0)
            return;
        int sysList = this.GetSysList(3);
        for (int index = 4; index < 8; ++index)
        {
            Obj obj = this._objPtrList[index];
            if (obj != null)
            {
                if ((int)obj.state == (int)EventEngine.stateRunning)
                {
                    if ((sysList & 16) == 0)
                        obj.state = EventEngine.stateSuspend;
                }
                else if ((int)obj.state == (int)EventEngine.stateSuspend && (sysList & 16) != 0)
                    obj.state = EventEngine.stateRunning;
            }
            sysList >>= 1;
        }
    }
}
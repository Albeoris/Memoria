using Memoria;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public partial class EventEngine
{
    public void updateModelsToBeAdded()
    {
        if (this.gMode != 1 && this.gMode != 3 && this._ff9Sys.prevMode != 9 || this.sEventContext1.inited == 0)
            return;
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (obj.cid == 4)
            {
                PosObj posObj = (PosObj)obj;
                if (posObj.model != UInt16.MaxValue)
                {
                    if (posObj.go == null)
                    {
                        if (this.gMode == 1)
                        {
                            posObj.go = ModelFactory.CreateModel(FF9BattleDB.GEO.GetValue(posObj.model), false, true, Configuration.Graphics.ElementsSmoothTexture);
                            GeoTexAnim.addTexAnim(posObj.go, FF9BattleDB.GEO.GetValue(posObj.model));
                            this.ReassignBasicAnimationForField((Actor)posObj);
                        }
                        else if (this.gMode == 3)
                        {
                            posObj.go = ModelFactory.CreateModel(FF9BattleDB.GEO.GetValue(posObj.model), false, true, Configuration.Graphics.WorldSmoothTexture);
                        }
                        else
                        {
                            posObj.go = ModelFactory.CreateModel(FF9BattleDB.GEO.GetValue(posObj.model), false, true, Configuration.Graphics.ElementsSmoothTexture);
                        }
                    }
                    if (posObj.go != null)
                    {
                        this.requiredAddActor = true;
                        this.toBeAddedObjUIDList.Add((Int32)posObj.uid);
                    }
                }
            }
        }
        this.sEventContext1.inited = 0;
        if (!this.requiredAddActor)
            return;
        Int32 playerUID = this.GetControlUID();
        this.requiredAddActor = false;
        using (List<Int32>.Enumerator enumerator = this.toBeAddedObjUIDList.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                Int32 current = enumerator.Current;
                Obj objUid = this.GetObjUID(current);
                Boolean isPlayer = current == playerUID;
                if (this.gMode == 1)
                {
                    if (isPlayer)
                    {
                        objUid.go.name = "Player";
                    }
                    else
                    {
                        //Debug.Log((object)("o.uid = " + (object)objUid.uid + ", o.sid = " + (object)objUid.sid));
                        objUid.go.name = "obj" + (Object)current;
                    }
                    FF9Char ff9Char = new FF9Char();
                    ff9Char.geo = objUid.go;
                    ff9Char.evt = (PosObj)objUid;
                    FF9StateSystem.Common.FF9.charArray.Add(objUid.uid, ff9Char);
                    FF9FieldCharState ff9FieldCharState = new FF9FieldCharState();
                    FF9StateSystem.Field.FF9Field.loc.map.charStateArray.Add(objUid.uid, ff9FieldCharState);
                    FF9StateSystem.Field.FF9Field.loc.map.shadowArray.Add(objUid.uid, new FF9Shadow());
                    this.fieldmap.AddFieldChar(objUid.go, ((PosObj)objUid).posField, ((PosObj)objUid).rotField, isPlayer, (Actor)objUid, true);
                    this.turnOffTriManually(objUid.sid);
                }
                else if (this.gMode == 3)
                {
                    Singleton<WMWorld>.Instance.addWMActorOnly((Actor)objUid);
                    Singleton<WMWorld>.Instance.addGameObjectToWMActor(objUid.go, ((Actor)objUid).wmActor);
                    PosObj posObj = (PosObj)objUid;
                    //if ((Int32)posObj.uid != (Int32)this._context.controlUID)
                    //    ;
                    ((Actor)objUid).wmActor.SetPosition(posObj.pos[0], posObj.pos[1], posObj.pos[2]);
                }
            }
        }
        this.requiredAddActor = false;
        this.toBeAddedObjUIDList.Clear();
    }

    private void ReassignBasicAnimationForField(Actor actor)
    {
        if ((Int32)actor.idle != 0)
            AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB.GetValue((Int32)actor.idle));
        if ((Int32)actor.walk != 0)
            AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB.GetValue((Int32)actor.walk));
        if ((Int32)actor.run != 0)
            AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB.GetValue((Int32)actor.run));
        if ((Int32)actor.turnl != 0)
            AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB.GetValue((Int32)actor.turnl));
        if ((Int32)actor.turnr == 0)
            return;
        AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB.GetValue((Int32)actor.turnr));
    }
}

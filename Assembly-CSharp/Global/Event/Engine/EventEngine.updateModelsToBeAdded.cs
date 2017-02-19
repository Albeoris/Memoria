using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public partial class EventEngine
{
    public void updateModelsToBeAdded()
    {
        if (this.gMode != 1 && this.gMode != 3 && (Int32)this._ff9Sys.prevMode != 9 || (Int32)this.sEventContext1.inited == 0)
            return;
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if ((Int32)obj.cid == 4)
            {
                PosObj posObj = (PosObj)obj;
                if ((Int32)posObj.model != (Int32)UInt16.MaxValue)
                {
                    if (!((UnityEngine.Object)posObj.go != (UnityEngine.Object)null))
                    {
                        posObj.go = ModelFactory.CreateModel(FF9BattleDB.GEO.GetValue((Int32)posObj.model), false);
                        if (this.gMode == 1)
                            GeoTexAnim.addTexAnim(posObj.go, FF9BattleDB.GEO.GetValue((Int32)posObj.model));
                        if (this.gMode == 1)
                            this.ReassignBasicAnimationForField((Actor)posObj);
                    }
                    if ((UnityEngine.Object)posObj.go != (UnityEngine.Object)null)
                    {
                        this.requiredAddActor = true;
                        this.toBeAddedObjUIDList.Add((Int32)posObj.uid);
                    }
                    if (this.gMode != 1)
                        ;
                }
            }
        }
        this.sEventContext1.inited = (Byte)0;
        if (!this.requiredAddActor)
            return;
        Int32 num = (Int32)this.GetControlUID();
        this.requiredAddActor = false;
        using (List<Int32>.Enumerator enumerator = this.toBeAddedObjUIDList.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                Int32 current = enumerator.Current;
                Obj objUid = this.GetObjUID(current);
                Boolean isPlayer = current == num;
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
                    FF9StateSystem.Common.FF9.charArray.Add((Int32)objUid.uid, ff9Char);
                    FF9FieldCharState ff9FieldCharState = new FF9FieldCharState();
                    FF9StateSystem.Field.FF9Field.loc.map.charStateArray.Add((Int32)objUid.uid, ff9FieldCharState);
                    FF9StateSystem.Field.FF9Field.loc.map.shadowArray.Add((Int32)objUid.uid, new FF9Shadow());
                    this.fieldmap.AddFieldChar(objUid.go, ((PosObj)objUid).posField, ((PosObj)objUid).rotField, isPlayer, (Actor)objUid, true);
                    this.turnOffTriManually((Int32)objUid.sid);
                }
                else if (this.gMode == 3)
                {
                    Singleton<WMWorld>.Instance.addWMActorOnly((Actor)objUid);
                    Singleton<WMWorld>.Instance.addGameObjectToWMActor(objUid.go, ((Actor)objUid).wmActor);
                    PosObj posObj = (PosObj)objUid;
                    if ((Int32)posObj.uid != (Int32)this._context.controlUID)
                        ;
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
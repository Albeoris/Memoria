using System.Collections.Generic;
using UnityEngine;

public partial class EventEngine
{
    public void updateModelsToBeAdded()
    {
        if (this.gMode != 1 && this.gMode != 3 && (int)this._ff9Sys.prevMode != 9 || (int)this.sEventContext1.inited == 0)
            return;
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if ((int)obj.cid == 4)
            {
                PosObj posObj = (PosObj)obj;
                if ((int)posObj.model != (int)ushort.MaxValue)
                {
                    if (!((UnityEngine.Object)posObj.go != (UnityEngine.Object)null))
                    {
                        posObj.go = ModelFactory.CreateModel(FF9BattleDB.GEO[(int)posObj.model], false);
                        if (this.gMode == 1)
                            GeoTexAnim.addTexAnim(posObj.go, FF9BattleDB.GEO[(int)posObj.model]);
                        if (this.gMode == 1)
                            this.ReassignBasicAnimationForField((Actor)posObj);
                    }
                    if ((UnityEngine.Object)posObj.go != (UnityEngine.Object)null)
                    {
                        this.requiredAddActor = true;
                        this.toBeAddedObjUIDList.Add((int)posObj.uid);
                    }
                    if (this.gMode != 1)
                        ;
                }
            }
        }
        this.sEventContext1.inited = (byte)0;
        if (!this.requiredAddActor)
            return;
        int num = (int)this.GetControlUID();
        this.requiredAddActor = false;
        using (List<int>.Enumerator enumerator = this.toBeAddedObjUIDList.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                int current = enumerator.Current;
                Obj objUid = this.GetObjUID(current);
                bool isPlayer = current == num;
                if (this.gMode == 1)
                {
                    if (isPlayer)
                    {
                        objUid.go.name = "Player";
                    }
                    else
                    {
                        Debug.Log((object)("o.uid = " + (object)objUid.uid + ", o.sid = " + (object)objUid.sid));
                        objUid.go.name = "obj" + (object)current;
                    }
                    FF9Char ff9Char = new FF9Char();
                    ff9Char.geo = objUid.go;
                    ff9Char.evt = (PosObj)objUid;
                    FF9StateSystem.Common.FF9.charArray.Add((int)objUid.uid, ff9Char);
                    FF9FieldCharState ff9FieldCharState = new FF9FieldCharState();
                    FF9StateSystem.Field.FF9Field.loc.map.charStateArray.Add((int)objUid.uid, ff9FieldCharState);
                    FF9StateSystem.Field.FF9Field.loc.map.shadowArray.Add((int)objUid.uid, new FF9Shadow());
                    this.fieldmap.AddFieldChar(objUid.go, ((PosObj)objUid).posField, ((PosObj)objUid).rotField, isPlayer, (Actor)objUid, true);
                    this.turnOffTriManually((int)objUid.sid);
                }
                else if (this.gMode == 3)
                {
                    Singleton<WMWorld>.Instance.addWMActorOnly((Actor)objUid);
                    Singleton<WMWorld>.Instance.addGameObjectToWMActor(objUid.go, ((Actor)objUid).wmActor);
                    PosObj posObj = (PosObj)objUid;
                    if ((int)posObj.uid != (int)this._context.controlUID)
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
        if ((int)actor.idle != 0)
            AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB[(int)actor.idle]);
        if ((int)actor.walk != 0)
            AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB[(int)actor.walk]);
        if ((int)actor.run != 0)
            AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB[(int)actor.run]);
        if ((int)actor.turnl != 0)
            AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB[(int)actor.turnl]);
        if ((int)actor.turnr == 0)
            return;
        AnimationFactory.AddAnimWithAnimatioName(actor.go, FF9DBAll.AnimationDB[(int)actor.turnr]);
    }
}
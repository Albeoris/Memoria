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
                        this.toBeAddedObjUIDList.Add(posObj.uid);
                    }
                }
            }
        }
        Boolean wasField = this.sEventContext1.inited == 1;
        this.sEventContext1.inited = 0;
        if (!this.requiredAddActor)
            return;
        Int32 playerUID = this.GetControlUID();
        this.requiredAddActor = false;
        foreach (Int32 uidRestore in this.toBeAddedObjUIDList)
        {
            Obj objUid = this.GetObjUID(uidRestore);
            Boolean isPlayer = uidRestore == playerUID;
            if (this.gMode == 1)
            {
                objUid.go.name = isPlayer ? "Player" : $"obj{uidRestore}";
                //Debug.Log((object)("o.uid = " + (object)objUid.uid + ", o.sid = " + (object)objUid.sid));
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
                ((Actor)objUid).wmActor.SetPosition(posObj.pos[0], posObj.pos[1], posObj.pos[2]);
            }
        }
        this.requiredAddActor = false;
        this.toBeAddedObjUIDList.Clear();
        if (wasField && this.gMode == 1 && this._ff9.fldMapNo == 768 && FF9StateSystem.Battle.battleMapIndex == 4)
        {
            // Burmecia/Palace, after the battle against Beatrix, fix #664
            Actor brahne = (Actor)this.FindObjByUID(11);
            Actor kuja = (Actor)this.FindObjByUID(13);
            Actor zidane = (Actor)this.FindObjByUID(15);
            Actor vivi = (Actor)this.FindObjByUID(16);
            Actor freya = (Actor)this.FindObjByUID(19);
            Actor quina = (Actor)this.FindObjByUID(20);
            Actor beatrix = (Actor)this.FindObjByUID(23);
            if (brahne != null)
            {
                this.SetActorPosition(brahne, -837, 0, -2034);
                brahne.rotAngle[1] = EventEngineUtils.ConvertFixedPointAngleToDegree(4 << 4);
            }
            if (kuja != null)
            {
                this.SetActorPosition(kuja, -196, 0, -2249);
                kuja.rotAngle[1] = EventEngineUtils.ConvertFixedPointAngleToDegree(22 << 4);
            }
            if (zidane != null)
            {
                this.SetActorPosition(zidane, -975, 0, -3657);
                zidane.rotAngle[1] = EventEngineUtils.ConvertFixedPointAngleToDegree(117 << 4);
                AnimationFactory.AddAnimWithAnimatioName(zidane.go, FF9DBAll.AnimationDB.GetValue(1383));
                zidane.idle = 1383;
                this.ExecAnim(zidane, 1383);
            }
            if (vivi != null)
            {
                this.SetActorPosition(vivi, -161, 0, -3602);
                vivi.rotAngle[1] = EventEngineUtils.ConvertFixedPointAngleToDegree(120 << 4);
                AnimationFactory.AddAnimWithAnimatioName(vivi.go, FF9DBAll.AnimationDB.GetValue(11970));
                vivi.idle = 11970;
                this.ExecAnim(vivi, 11970);
            }
            if (freya != null)
            {
                this.SetActorPosition(freya, -1800, -40, -3642);
                freya.rotAngle[1] = EventEngineUtils.ConvertFixedPointAngleToDegree(126 << 4);
                AnimationFactory.AddAnimWithAnimatioName(freya.go, FF9DBAll.AnimationDB.GetValue(8382));
                freya.idle = 8382;
                this.ExecAnim(freya, 8382);
            }
            if (quina != null)
            {
                this.SetActorPosition(quina, 860, 0, -2945);
                quina.rotAngle[1] = EventEngineUtils.ConvertFixedPointAngleToDegree(64 << 4);
                AnimationFactory.AddAnimWithAnimatioName(quina.go, FF9DBAll.AnimationDB.GetValue(4141));
                quina.idle = 4141;
                this.ExecAnim(quina, 4141);
            }
            if (beatrix != null)
            {
                this.SetActorPosition(beatrix, -1607, 0, -3138);
                beatrix.rotAngle[1] = EventEngineUtils.ConvertFixedPointAngleToDegree(2 << 4);
                AnimationFactory.AddAnimWithAnimatioName(beatrix.go, FF9DBAll.AnimationDB.GetValue(2978));
                beatrix.idle = 2978;
                this.ExecAnim(beatrix, 2978);
            }
        }
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

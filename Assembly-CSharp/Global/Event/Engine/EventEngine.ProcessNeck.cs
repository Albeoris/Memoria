using FF9;
using System;
using UnityEngine;

public partial class EventEngine
{
    private void ProcessNeck(Actor actor)
    {
        Vector3 vector3 = Vector3.zero;
        Int32 fixedPoint = 0;
        Single single = 0f;
        Single single1 = 0f;
        PosObj posObj = null;
        Int32? nullable = new Int32?(0);
        Int32? nullable1 = null;
        if ((actor.actf & EventEngine.actNeckTalk) != 0 && EventEngine.sLastTalker != null && EventEngine.sLastTalker != actor && this.dist64(actor.pos[0] - EventEngine.sLastTalker.pos[0], actor.pos[2] - EventEngine.sLastTalker.pos[2], 0f) >= (Single)EventEngine.kNeckNear2)
        {
            posObj = EventEngine.sLastTalker;
            Actor actor1 = actor;
            actor1.actf = (UInt16)(actor1.actf | (UInt16)EventEngine.actLookTalker);
        }
        if (posObj != null || (actor.actf & EventEngine.actNeckT) != 0 && (actor.animFlag & EventEngine.afLower) == 0)
        {
            if ((actor.flags & 128) != 0)
            {
                Vector3 vector31 = actor.rotAngle;
                single1 = vector31.y;
                vector31.y = actor.trot;
                actor.rotAngle = vector31;
            }
            if (posObj == null)
            {
                posObj = this.CollisionNeck(actor);
            }
            if (posObj != null)
            {
                if ((actor.actf & EventEngine.actLook) == 0 && (actor.flags & 128) == 0)
                {
                    Actor actor2 = actor;
                    actor2.actf = (UInt16)(actor2.actf | (UInt16)EventEngine.actLook);
                    this.GetDefaultLook(actor, ref vector3, 0);
                    actor.xl = vector3.x;
                    actor.yl = vector3.y;
                    actor.zl = vector3.z;
                }
                if ((actor.actf & EventEngine.actLook) != 0 && this.NeckRange(actor, ref nullable1))
                {
                    vector3.x = posObj.pos[0];
                    vector3.y = posObj.pos[1];
                    vector3.z = posObj.pos[2];
                    Single single2 = vector3.x - actor.pos[0];
                    Single single3 = vector3.z - actor.pos[2];
                    single = this.eBin.angleAsm(single2, single3);
                    Single item = single - actor.rotAngle[1];
                    if (item > 180f)
                    {
                        item = item - 360f;
                    }
                    else if (item < -180f)
                    {
                        item = item + 360f;
                    }
                    fixedPoint = EventEngineUtils.ConvertFloatAngleToFixedPoint(item);
                    if (fixedPoint > 2048)
                    {
                        fixedPoint = fixedPoint - 4096;
                    }
                    if (fixedPoint > -EventEngine.kNeckAngle && fixedPoint < EventEngine.kNeckAngle)
                    {
                        Int32 num = (Int32)actor.pos[1];
                        Int32 num1 = (Int32)vector3.y;
                        vector3.y = (Single)(3 * (((Int32)actor.pos[1] << 6) - actor.eye * actor.scaley) + ((Int32)vector3.y << 6) - posObj.eye * posObj.scaley >> 8);
                        this.LookSlow(actor, vector3);
                    }
                    else if (this.NeckTurn(actor, EventEngineUtils.ConvertFloatAngleToFixedPoint(single)))
                    {
                        this.GetDefaultLook(actor, ref vector3, (fixedPoint <= EventEngine.kNeckAngle ? (Int32)(-EventEngine.kNeckAngle) : (Int32)EventEngine.kNeckAngle));
                        this.LookSlow(actor, vector3);
                    }
                }
            }
            else if ((actor.actf & EventEngine.actLook) != 0 && this.NeckRange(actor, ref nullable) && this.NeckTurn(actor, nullable.Value))
            {
                this.NeckReturn(actor);
            }
            if ((actor.flags & 128) != 0)
            {
                Vector3 vector32 = actor.rotAngle;
                vector32.y = single1;
                if (vector32.y < -180f)
                {
                    vector32.y = vector32.y + 360f;
                }
                else if (vector32.y > 180f)
                {
                    vector32.y = vector32.y - 360f;
                }
                actor.rotAngle[1] = vector32.y;
            }
        }
        else if ((actor.actf & EventEngine.actLook) != 0 && this.NeckRange(actor, ref nullable1))
        {
            this.NeckReturn(actor);
        }
        if ((actor.actf & EventEngine.actLook) == 0)
        {
            this.geoLookReset(actor);
        }
    }

    private PosObj CollisionNeck(PosObj po)
    {
        PosObj posObj = (PosObj)null;
        Single num1 = 0.0f;
        Single num2 = po.pos[0];
        Single num3 = po.pos[1];
        Single num4 = po.pos[2];
        Int32 num5 = (Int32)((Actor)po).neckTargetID;
        Vector3 vector3 = po.rotAngle;
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (obj != po && (Int32)obj.cid == 4)
            {
                Actor actor = (Actor)obj;
                Single num6 = actor.pos[1] - num3;
                if ((Double)num6 > -EventEngine.kDefaultHeight && (Double)num6 < (Double)EventEngine.kDefaultHeight && (((Int32)actor.actf & EventEngine.actNeckM) != 0 && (num5 & (Int32)actor.neckMyID) != 0))
                {
                    Single num7 = this.disdif64(actor.pos[0] - num2, actor.pos[2] - num4, 0.0f);
                    if ((Double)num7 >= (Double)EventEngine.kNeckNear2)
                    {
                        Int32 num8 = this.eBin.CollisionAngle(po, (PosObj)actor, vector3.y);
                        Single num9 = num7 - (Single)(16 * EventEngine.kNeckRad * EventEngine.kNeckRad);
                        if (num8 < 0)
                            num8 = -num8;
                        Int32 num10 = num8 - (Int32)EventEngine.kNeckAngle;
                        if (num10 < 0)
                        {
                            Single num11 = num9 + (Single)num10;
                            if ((Double)num1 > (Double)num11)
                            {
                                num1 = num11;
                                posObj = (PosObj)actor;
                            }
                        }
                    }
                }
            }
        }
        return posObj;
    }

    private Boolean NeckRange(Actor actor, ref Int32? ap)
    {
        Boolean flag = true;
        if (((Int32)actor.actf & EventEngine.actLookedTalker) == 0)
        {
            Single floatAngle1 = this.eBin.angleAsm(actor.xl - actor.pos[0], actor.zl - actor.pos[2]);
            Single num = actor.rotAngle[1];
            Single floatAngle2 = floatAngle1 - num;
            if ((Double)floatAngle2 > 180.0)
                floatAngle2 -= 360f;
            else if ((Double)floatAngle2 < -180.0)
                floatAngle2 += 360f;
            Int32 fixedPoint = EventEngineUtils.ConvertFloatAngleToFixedPoint(floatAngle2);
            if (fixedPoint > 2048)
                fixedPoint -= 4096;
            flag = fixedPoint > -EventEngine.kNeckAngle0 && fixedPoint < (Int32)EventEngine.kNeckAngle0;
            if (!flag)
                actor.actf &= (UInt16)~EventEngine.actLook;
            if (ap.HasValue)
                ap = new Int32?(EventEngineUtils.ConvertFloatAngleToFixedPoint(floatAngle1));
        }
        return flag;
    }

    private void NeckReturn(Actor actor)
    {
        Vector3 zero = Vector3.zero;
        this.GetDefaultLook(actor, ref zero, 0);
        Single num1 = this.distance(zero.x - actor.xl, zero.y - actor.yl, zero.z - actor.zl);
        if ((Double)num1 < (Double)EventEngine.kLook)
        {
            num1 = (Single)EventEngine.kLook;
            Int32 num2 = ~(EventEngine.actLook | EventEngine.actLookTalker | EventEngine.actLookedTalker);
            actor.actf &= (UInt16)num2;
        }
        actor.xl += (Single)EventEngine.kLook * (zero.x - actor.xl) / num1;
        actor.yl += (Single)EventEngine.kLook * (zero.y - actor.yl) / num1;
        actor.zl += (Single)EventEngine.kLook * (zero.z - actor.zl) / num1;
        this.geoLookSet((PosObj)actor, actor.xl, actor.yl, actor.zl);
    }

    private void GetDefaultLook(Actor actor, ref Vector3 output, Int32 ofs)
    {
        Transform transform = actor.go.transform;
        Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
        Vector3 v = new Vector3(0.0f, (Single)((Int32)actor.eye * (Int32)actor.scaley >> 6), -500f);
        output = localToWorldMatrix.MultiplyVector(v) + transform.position;
    }

    private void LookSlow(Actor actor, Vector3 v)
    {
        Single deltaX = v.x - actor.xl;
        Single deltaY = v.y - actor.yl;
        Single deltaZ = v.z - actor.zl;
        Single num = this.distance(deltaX, deltaY, deltaZ);
        if ((Double)num < (Double)EventEngine.kLook)
            num = (Single)EventEngine.kLook;
        actor.xl += (Single)EventEngine.kLook * deltaX / num;
        actor.yl += (Single)EventEngine.kLook * deltaY / num;
        actor.zl += (Single)EventEngine.kLook * deltaZ / num;
        this.geoLookSet((PosObj)actor, actor.xl, actor.yl, actor.zl);
    }

    private void geoLookSet(PosObj po, Single posx, Single posy, Single posz)
    {
        po.geo_struct_flags |= geo.GEO_FLAGS_LOOK;
        po.geo_struct_lookat.x = posx;
        po.geo_struct_lookat.y = posy;
        po.geo_struct_lookat.z = posz;
    }

    private void geoLookReset(PosObj po)
    {
        po.geo_struct_flags = (UInt16)(po.geo_struct_flags & ~geo.GEO_FLAGS_LOOK);
    }

    private Boolean NeckTurn(Actor actor, Int32 rot)
    {
        Boolean flag = false;
        if (((Int32)actor.actf & EventEngine.actLookedTalker) == 0 && (((Int32)actor.animFlag & EventEngine.afExec) == 0 && (Int32)actor.turninst0 == 167 && (Int32)actor.level > 0))
        {
            Obj obj = (Obj)actor;
            actor.turninst1 = (Byte)(rot >> 4);
            Obj.neckTurnData[3] = actor.turninst1;
            this.Call(obj, 0, 0, false, Obj.neckTurnData);
        }
        return !flag;
    }
}

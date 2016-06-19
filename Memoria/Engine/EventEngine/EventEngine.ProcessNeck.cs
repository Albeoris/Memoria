using FF9;
using UnityEngine;

public partial class EventEngine
{
    private void ProcessNeck(Actor actor)
    {
        Vector3 vector3 = Vector3.zero;
        int fixedPoint = 0;
        float single = 0f;
        float single1 = 0f;
        PosObj posObj = null;
        int? nullable = new int?(0);
        int? nullable1 = null;
        if ((actor.actf & EventEngine.actNeckTalk) != 0 && EventEngine.sLastTalker != null && EventEngine.sLastTalker != actor && this.dist64(actor.pos[0] - EventEngine.sLastTalker.pos[0], actor.pos[2] - EventEngine.sLastTalker.pos[2], 0f) >= (float)EventEngine.kNeckNear2)
        {
            posObj = EventEngine.sLastTalker;
            Actor actor1 = actor;
            actor1.actf = (ushort)(actor1.actf | (ushort)EventEngine.actLookTalker);
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
                    actor2.actf = (ushort)(actor2.actf | (ushort)EventEngine.actLook);
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
                    float single2 = vector3.x - actor.pos[0];
                    float single3 = vector3.z - actor.pos[2];
                    single = this.eBin.angleAsm(single2, single3);
                    float item = single - actor.rotAngle[1];
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
                        int num = (int)actor.pos[1];
                        int num1 = (int)vector3.y;
                        vector3.y = (float)(3 * (((int)actor.pos[1] << 6) - actor.eye * actor.scaley) + ((int)vector3.y << 6) - posObj.eye * posObj.scaley >> 8);
                        this.LookSlow(actor, vector3);
                    }
                    else if (this.NeckTurn(actor, EventEngineUtils.ConvertFloatAngleToFixedPoint(single)))
                    {
                        this.GetDefaultLook(actor, ref vector3, (fixedPoint <= EventEngine.kNeckAngle ? (int)(-EventEngine.kNeckAngle) : (int)EventEngine.kNeckAngle));
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
        float num1 = 0.0f;
        float num2 = po.pos[0];
        float num3 = po.pos[1];
        float num4 = po.pos[2];
        int num5 = (int)((Actor)po).neckTargetID;
        Vector3 vector3 = po.rotAngle;
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (obj != po && (int)obj.cid == 4)
            {
                Actor actor = (Actor)obj;
                float num6 = actor.pos[1] - num3;
                if ((double)num6 > (double)-EventEngine.kDefaultHeight && (double)num6 < (double)EventEngine.kDefaultHeight && (((int)actor.actf & EventEngine.actNeckM) != 0 && (num5 & (int)actor.neckMyID) != 0))
                {
                    float num7 = this.disdif64(actor.pos[0] - num2, actor.pos[2] - num4, 0.0f);
                    if ((double)num7 >= (double)EventEngine.kNeckNear2)
                    {
                        int num8 = this.eBin.CollisionAngle(po, (PosObj)actor, vector3.y);
                        float num9 = num7 - (float)(16 * EventEngine.kNeckRad * EventEngine.kNeckRad);
                        if (num8 < 0)
                            num8 = -num8;
                        int num10 = num8 - (int)EventEngine.kNeckAngle;
                        if (num10 < 0)
                        {
                            float num11 = num9 + (float)num10;
                            if ((double)num1 > (double)num11)
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

    private bool NeckRange(Actor actor, ref int? ap)
    {
        bool flag = true;
        if (((int)actor.actf & EventEngine.actLookedTalker) == 0)
        {
            float floatAngle1 = this.eBin.angleAsm(actor.xl - actor.pos[0], actor.zl - actor.pos[2]);
            float num = actor.rotAngle[1];
            float floatAngle2 = floatAngle1 - num;
            if ((double)floatAngle2 > 180.0)
                floatAngle2 -= 360f;
            else if ((double)floatAngle2 < -180.0)
                floatAngle2 += 360f;
            int fixedPoint = EventEngineUtils.ConvertFloatAngleToFixedPoint(floatAngle2);
            if (fixedPoint > 2048)
                fixedPoint -= 4096;
            flag = fixedPoint > (int)-EventEngine.kNeckAngle0 && fixedPoint < (int)EventEngine.kNeckAngle0;
            if (!flag)
                actor.actf &= (ushort)~EventEngine.actLook;
            if (ap.HasValue)
                ap = new int?(EventEngineUtils.ConvertFloatAngleToFixedPoint(floatAngle1));
        }
        return flag;
    }

    private void NeckReturn(Actor actor)
    {
        Vector3 zero = Vector3.zero;
        this.GetDefaultLook(actor, ref zero, 0);
        float num1 = this.distance(zero.x - actor.xl, zero.y - actor.yl, zero.z - actor.zl);
        if ((double)num1 < (double)EventEngine.kLook)
        {
            num1 = (float)EventEngine.kLook;
            int num2 = ~(EventEngine.actLook | EventEngine.actLookTalker | EventEngine.actLookedTalker);
            actor.actf &= (ushort)num2;
        }
        actor.xl += (float)EventEngine.kLook * (zero.x - actor.xl) / num1;
        actor.yl += (float)EventEngine.kLook * (zero.y - actor.yl) / num1;
        actor.zl += (float)EventEngine.kLook * (zero.z - actor.zl) / num1;
        this.geoLookSet((PosObj)actor, actor.xl, actor.yl, actor.zl);
    }

    private void GetDefaultLook(Actor actor, ref Vector3 output, int ofs)
    {
        Transform transform = actor.go.transform;
        Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
        Vector3 v = new Vector3(0.0f, (float)((int)actor.eye * (int)actor.scaley >> 6), -500f);
        output = localToWorldMatrix.MultiplyVector(v) + transform.position;
    }

    private void LookSlow(Actor actor, Vector3 v)
    {
        float deltaX = v.x - actor.xl;
        float deltaY = v.y - actor.yl;
        float deltaZ = v.z - actor.zl;
        float num = this.distance(deltaX, deltaY, deltaZ);
        if ((double)num < (double)EventEngine.kLook)
            num = (float)EventEngine.kLook;
        actor.xl += (float)EventEngine.kLook * deltaX / num;
        actor.yl += (float)EventEngine.kLook * deltaY / num;
        actor.zl += (float)EventEngine.kLook * deltaZ / num;
        this.geoLookSet((PosObj)actor, actor.xl, actor.yl, actor.zl);
    }

    private void geoLookSet(PosObj po, float posx, float posy, float posz)
    {
        po.geo_struct_flags |= geo.GEO_FLAGS_LOOK;
        po.geo_struct_lookat.x = posx;
        po.geo_struct_lookat.y = posy;
        po.geo_struct_lookat.z = posz;
    }

    private void geoLookReset(PosObj po)
    {
        po.geo_struct_flags = (ushort)(po.geo_struct_flags & ~geo.GEO_FLAGS_LOOK);
    }

    private bool NeckTurn(Actor actor, int rot)
    {
        bool flag = false;
        if (((int)actor.actf & EventEngine.actLookedTalker) == 0 && (((int)actor.animFlag & EventEngine.afExec) == 0 && (int)actor.turninst0 == 167 && (int)actor.level > 0))
        {
            Obj obj = (Obj)actor;
            actor.turninst1 = (byte)(rot >> 4);
            Obj.neckTurnData[3] = actor.turninst1;
            this.Call(obj, 0, 0, false, Obj.neckTurnData);
        }
        return !flag;
    }
}
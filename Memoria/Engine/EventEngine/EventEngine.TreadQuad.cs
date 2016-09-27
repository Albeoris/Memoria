using System;
using UnityEngine;

public partial class EventEngine
{
    public Obj TreadQuad(PosObj po, Int32 mode)
    {
        Obj obj1 = null;
        Single x = po.go.transform.position.x;
        Single z = po.go.transform.position.z;
        Int32 tagID = (mode & 4) == 0 ? 2 : 3;
        for (ObjList objList = this._context.activeObj; objList != null; objList = objList.next)
        {
            Obj obj2 = objList.obj;
            if (obj2.cid == 3 && this.GetIP(obj2.sid, tagID, obj2.ebData) != this.nil && this.IsInQuadHotFix((Quad)obj2, x, z))
            {
                obj1 = obj2;
                break;
            }
        }
        return obj1;
    }

    private Boolean IsInQuad(Quad quad, Single x, Single z)
    {
        Int32 num = quad.n;
        for (Int32 index1 = 0; index1 < num; ++index1)
        {
            Int32 index2 = index1 % num;
            Int32 index3 = (index1 + 1) % num;
            Int32 index4 = (index1 + 2) % num;
            Vector3 vector3Val1 = quad.q[index2].Vector3Val;
            Vector3 vector3Val2 = quad.q[index3].Vector3Val;
            Vector3 vector3Val3 = quad.q[index4].Vector3Val;
            if (Math3D.PointInsideTriangleTestXZ(new Vector3(x, 0.0f, z), vector3Val1, vector3Val2, vector3Val3))
                return true;
        }
        return false;
    }

    private Boolean IsInQuadHotFix(Quad quad, Single x, Single z)
    {
        if (this.gMode != 1)
            return this.IsInQuad(quad, x, z);
        Int32 npcid = EMinigame.CreateNPCID((Int32)FF9StateSystem.Common.FF9.fldMapNo, (Int32)quad.sid);
        if (!EventEngineUtils.QuadCircleData.ContainsKey(npcid))
            return this.IsInQuad(quad, x, z);
        QuadCircle quadCircle = EventEngineUtils.QuadCircleData[npcid];
        if (!quadCircle.UseOriginalArea)
            return quadCircle.IsCollisionEnter(x, z);
        if (!quadCircle.IsCollisionEnter(x, z))
            return this.IsInQuad(quad, x, z);
        return true;
    }
}
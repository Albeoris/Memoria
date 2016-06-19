using UnityEngine;

public partial class EventEngine
{
    public Obj TreadQuad(PosObj po, int mode)
    {
        Obj obj1 = null;
        float x = po.go.transform.position.x;
        float z = po.go.transform.position.z;
        int tagID = (mode & 4) == 0 ? 2 : 3;
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

    private bool IsInQuad(Quad quad, float x, float z)
    {
        int num = quad.n;
        for (int index1 = 0; index1 < num; ++index1)
        {
            int index2 = index1 % num;
            int index3 = (index1 + 1) % num;
            int index4 = (index1 + 2) % num;
            Vector3 vector3Val1 = quad.q[index2].Vector3Val;
            Vector3 vector3Val2 = quad.q[index3].Vector3Val;
            Vector3 vector3Val3 = quad.q[index4].Vector3Val;
            if (Math3D.PointInsideTriangleTestXZ(new Vector3(x, 0.0f, z), vector3Val1, vector3Val2, vector3Val3))
                return true;
        }
        return false;
    }

    private bool IsInQuadHotFix(Quad quad, float x, float z)
    {
        if (this.gMode != 1)
            return this.IsInQuad(quad, x, z);
        int npcid = EMinigame.CreateNPCID(FF9StateSystem.Common.FF9.fldMapNo, quad.sid);
        if (EventEngineUtils.QuadCircleData.ContainsKey(npcid))
            return EventEngineUtils.QuadCircleData[npcid].IsCollisionEnter(x, z);
        return this.IsInQuad(quad, x, z);
    }
}
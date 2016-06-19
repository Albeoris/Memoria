using UnityEngine;

public partial class EventEngine
{
    private void ProcessTurn(Actor actor)
    {
        if (((int)actor.flags & 128) == 0)
            return;
        float num = actor.rotAngle.y;
        if ((double)num > 180.0)
            num -= 360f;
        else if ((double)num < -180.0)
            num += 360f;
        if ((double)actor.turnAdd == 32766.0)
        {
            float a = num + actor.trotAdd;
            if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2855 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 9370 && (int)actor.model == 273)
            {
                if ((double)a > 180.0)
                    a -= 360f;
                else if ((double)a < -180.0)
                    a += 360f;
            }
            if ((double)a < (double)actor.turnRot || EventEngineUtils.nearlyEqual(a, actor.turnRot))
            {
                a = actor.turnRot;
                actor.flags = (byte)((uint)actor.flags & 4294967167U);
            }
            actor.rotAngle[1] = a;
        }
        else if ((double)actor.turnAdd == (double)short.MaxValue)
        {
            float a = num + actor.trotAdd;
            if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2855 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 9370 && (int)actor.model == 273)
            {
                if ((double)a > 180.0)
                    a -= 360f;
                else if ((double)a < -180.0)
                    a += 360f;
            }
            if ((double)a > (double)actor.turnRot || EventEngineUtils.nearlyEqual(a, actor.turnRot))
            {
                a = actor.turnRot;
                actor.flags = (byte)((uint)actor.flags & 4294967167U);
            }
            actor.rotAngle[1] = a;
        }
        Vector3 eulerAngles = actor.go.transform.localRotation.eulerAngles;
    }

    private void DoTurn(Actor actor)
    {
        Vector3 vector3 = actor.rotAngle;
        if ((int)actor.sid != 15)
        ;
        vector3.y += actor.turnAdd;
        actor.rotAngle[1] += actor.turnAdd;
        actor.trot += actor.trotAdd;
        if ((int)actor.sid == 15)
        ;
    }

    private void FinishTurn(Actor actor)
    {
        Vector3 vector3 = actor.rotAngle;
        actor.go.GetComponent<FieldMapActorController>();
        vector3.y = actor.turnRot;
        actor.rotAngle[1] = actor.turnRot;
        this.SetAnim(actor, (int)actor.lastAnim);
        actor.flags = (byte)((uint)actor.flags & 4294967167U);
    }
}
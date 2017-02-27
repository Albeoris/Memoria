using System;
using UnityEngine;

public partial class EventEngine
{
    private bool MoveToward_mixed(float x, float y, float z, int flags, PosObj flagsPosObj)
    {
        Actor actor = (Actor)this.gCur;
        if (FF9StateSystem.Common.FF9.fldMapNo == 1823 && actor.sid == 13 && (int)x == -365 && (int)z == -373)
        {
            x = -389f;
            z = -600f;
        }
        if (FF9StateSystem.Common.FF9.fldMapNo == 1550 && actor.sid == 15 && (int)x == -1109 && (int)z == -1014)
        {
            x = -1155f;
            z = -1070f;
        }
        GameObject go = this.gCur.go;
        float num = (float)actor.speed;
        float num2 = 0f;
        PosObj posObj = (PosObj)null;
        FieldMapActorController fieldMapActorController = (FieldMapActorController)null;
        WMActor wmactor = (WMActor)null;
        float num3 = 0f;
        float num4 = 0f;
        float num5 = 0f;
        Vector3 vector = Vector3.zero;
        if (this.gMode == 1)
        {
            fieldMapActorController = actor.fieldMapActorController;
            if (fieldMapActorController == (UnityEngine.Object)null)
            {
                num3 = actor.pos[0];
                num4 = actor.pos[1];
                num5 = actor.pos[2];
            }
            else
            {
                num3 = fieldMapActorController.curPos.x;
                num4 = fieldMapActorController.curPos.y;
                num5 = fieldMapActorController.curPos.z;
            }
            vector = actor.rotAngle;
        }
        else if (this.gMode == 3)
        {
            wmactor = actor.wmActor;
            num3 = actor.pos[0];
            num4 = actor.pos[1];
            num5 = actor.pos[2];
        }
        float deltaX = x - num3;
        float num6 = y - num4;
        float deltaZ = z - num5;
        float num7;
        if (((int)actor.actf & EventEngine.actLockDir) != 0)
        {
            num7 = ((this.gMode != 1) ? wmactor.rot1 : vector.y);
            num7 = EventEngineUtils.ClampAngle(num7);
        }
        else
        {
            num7 = this.eBin.angleAsm(deltaX, deltaZ);
            if ((flags & 1) == 0 || flagsPosObj != null)
            {
                float num8 = (this.gMode != 1) ? wmactor.rot1 : vector.y;
                num8 = EventEngineUtils.ClampAngle(num8);
                float num9 = num7 - num8;
                if (num9 < -180f)
                {
                    num9 += 360f;
                }
                if (num9 > 180f || EventEngineUtils.nearlyEqual(num9, 180f))
                {
                    num9 -= 360f;
                }
                if ((FF9StateSystem.Common.FF9.fldMapNo != 307 || this.gCur.sid != 11) && (FF9StateSystem.Common.FF9.fldMapNo != 610 || this.gCur.sid != 3) && EventEngineUtils.nearlyEqual(num9, 0f))
                {
                    Actor actor2 = actor;
                    actor2.actf = (ushort)(actor2.actf | (ushort)EventEngine.actLockDir);
                }
                int num10 = (int)actor.omega << 3;
                float num11 = EventEngineUtils.ConvertFixedPointAngleToDegreeWithOutShiftRight((short)num10);
                if (num9 > 0f)
                {
                    if (num9 > num11)
                    {
                        num7 = num8 + num11;
                    }
                }
                else if (num9 < -num11)
                {
                    num7 = num8 - num11;
                }
                num7 = EventEngineUtils.ClampAngle(num7);
                if (this.gMode == 1)
                {
                    vector.y = num7;
                    actor.rotAngle[1] = num7;
                }
                else if (this.gMode == 3)
                {
                    wmactor.rot1 = num7;
                }
            }
        }
        float num13;
        if ((flags & 2) != 0 && flagsPosObj == null)
        {
            num6 = y - num4;
            float num12 = this.distance(deltaX, 0f, deltaZ);
            num13 = this.eBin.angleAsm(-num6, -num12);
            num13 = EventEngineUtils.ClampAngle(num13);
            float num8 = actor.rot0;
            num8 = EventEngineUtils.ClampAngle(num8);
            float num9 = num13 - num8;
            if (num9 < -180f)
            {
                num9 += 360f;
            }
            else if (num9 > 180f || EventEngineUtils.nearlyEqual(num9, 180f))
            {
                num9 -= 360f;
            }
            int num10 = (int)actor.omega << 3;
            float num14 = EventEngineUtils.ConvertFixedPointAngleToDegreeWithOutShiftRight((short)num10);
            if (num9 > 0f)
            {
                if (num9 > num14)
                {
                    num13 = num8 + num14;
                }
            }
            else if (num9 < -num14)
            {
                num13 = num8 - num14;
            }
            num13 = EventEngineUtils.ClampAngle(num13);
            actor.rot0 = num13;
        }
        else
        {
            num13 = 0f;
            num6 = 0f;
        }
        Vector3 vector2;
        EventEngine.GetMoveVector(out vector2, num13, num7, (float)actor.speed);
        if (fieldMapActorController != (UnityEngine.Object)null && fieldMapActorController.name == fieldMapActorController.fieldMap.debugObjName)
        {
            Vector3 a = new Vector3(num3, num4, num5);
            Vector3 vector3 = vector2;
            Vector3 b = vector3.normalized * 100f * vector3.magnitude;
            Vector3 b2 = new Vector3(0f, 100f, 0f);
            Vector3 vector4 = new Vector3(x, y, z);
            global::Debug.DrawLine(a + b2, a + b + b2, Color.magenta, 2f, true);
            global::Debug.DrawLine(a + b2, a + Vector3.up * 50f + b2, Color.blue, 2f, true);
        }
        Vector3 vector5 = new Vector3(num3, num4, num5) + vector2;
        if (this.gMode == 1)
        {
            fieldMapActorController = actor.fieldMapActorController;
            if (fieldMapActorController == (UnityEngine.Object)null)
            {
                actor.pos[0] = vector5.x;
                actor.pos[1] = vector5.y;
                actor.pos[2] = vector5.z;
            }
            else
            {
                fieldMapActorController.curPos += vector2;
                fieldMapActorController.SyncPosToTransform();
            }
        }
        else if (this.gMode == 3)
        {
            wmactor.SetPosition(vector5.x, vector5.y, vector5.z);
        }
        if (this.gMode == 1 && fieldMapActorController != (UnityEngine.Object)null)
        {
            if (fieldMapActorController.originalActor.uid == 2 && FF9StateSystem.Common.FF9.fldMapNo == 1605 && this.eBin.getVarManually(220) == 6622)
            {
                fieldMapActorController.originalActor.collRad = 10;
            }
            if (fieldMapActorController.originalActor.uid == 18 && FF9StateSystem.Common.FF9.fldMapNo == 575 && this.eBin.getVarManually(220) == 3165)
            {
                fieldMapActorController.originalActor.collRad = 34;
            }
            posObj = fieldMapActorController.walkMesh.Collision(fieldMapActorController, 0, out num2);
            if (posObj != null && (num2 < 0f || EventEngineUtils.nearlyEqual(num2, 0f)))
            {
                fieldMapActorController.curPos -= vector2;
            }
        }
        else if (this.gMode == 3)
        {
            posObj = (PosObj)this.Collision(this, actor, 0, ref num2);
            if (posObj != null && (num2 < 0f || EventEngineUtils.nearlyEqual(num2, 0f)))
            {
                wmactor.SetPosition(num3, num4, num5);
            }
            actor.pos[0] = vector5[0];
            actor.pos[1] = vector5[1];
            actor.pos[2] = vector5[2];
        }
        if (actor.loopCount != 0 && actor.loopCount != 255)
        {
            Actor actor3 = actor;
            actor3.loopCount = (byte)(actor3.loopCount - 1);
        }
        bool flag = actor.loopCount != 0;
        if (flagsPosObj != null)
        {
            bool flag2 = posObj != flagsPosObj;
            bool flag3 = num2 > 0f;
            flag = (flag2 || flag3);
        }
        num2 = this.dist64(deltaX, num6, deltaZ);
        bool flag4 = num2 > actor.lastdist;
        if (FF9StateSystem.Common.FF9.fldMapNo == 456 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 2800)
        {
            if (actor.sid == 2)
            {
                flag4 = (!EventEngineUtils.nearlyEqual(num2, actor.lastdist) && num2 > actor.lastdist);
            }
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 455 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 2800)
        {
            if (actor.sid == 1)
            {
                flag4 = (!EventEngineUtils.nearlyEqual(num2, actor.lastdist) && num2 > actor.lastdist);
            }
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 1055 && actor.sid == 11)
        {
            flag4 = (!EventEngineUtils.nearlyEqual(num2, actor.lastdist) && num2 > actor.lastdist);
        }
        if (num2 < num * num || (((int)actor.actf & EventEngine.actMove) != 0 && flag4))
        {
            if (num2 < num * num && FF9StateSystem.Common.FF9.fldMapNo != 2204 && (FF9StateSystem.Common.FF9.fldMapNo != 2209 || PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(220) != 9850) && this.gMode == 1 && fieldMapActorController != (UnityEngine.Object)null)
            {
                fieldMapActorController.curPos.x = x;
                fieldMapActorController.curPos.z = z;
                fieldMapActorController.SyncPosToTransform();
            }
            flag = false;
        }
        bool flag5 = false;
        if (FF9StateSystem.Common.FF9.fldMapNo == 901)
        {
            if (actor.sid == 1)
            {
                flag5 = true;
            }
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 1808)
        {
            if (actor.sid == 4 || actor.sid == 5)
            {
                flag5 = true;
            }
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 1810)
        {
            if (actor.sid == 5)
            {
                flag5 = true;
            }
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 456 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 2800)
        {
            if (actor.sid == 2)
            {
                flag5 = true;
            }
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 455 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 2800)
        {
            if (actor.sid == 1)
            {
                flag5 = true;
            }
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 1055 && actor.sid == 11)
        {
            flag5 = true;
        }
        if (flag5 && flag4 && posObj != null && posObj.uid == this.GetControlUID())
        {
            flag = true;
        }
        if (actor.lastdist < (float)EventEngine.kInitialDist && num2 < actor.lastdist)
        {
            Actor actor4 = actor;
            actor4.actf = (ushort)(actor4.actf | (ushort)EventEngine.actMove);
        }
        if (FF9StateSystem.Common.FF9.fldMapNo == 571 && actor.sid == 4 && EventEngineUtils.nearlyEqual(x, 887f) && EventEngineUtils.nearlyEqual(z, 1419f) && num2 > actor.lastdist)
        {
            fieldMapActorController.curPos.x = 887f;
            fieldMapActorController.curPos.z = 1419f;
            fieldMapActorController.SyncPosToTransform();
            return true;
        }
        actor.lastdist = num2;
        if (FF9StateSystem.Common.FF9.fldMapNo == 2954 && actor.sid == 11 && EventEngineUtils.nearlyEqual(num2, 32420f))
        {
            return false;
        }
        if (!flag)
        {
            this.clrdist(actor);
        }
        if (actor.uid == this._context.controlUID)
        {
            this.gAnimCount++;
        }
        return flag;
    }

    private static void GetMoveVector(out Vector3 oVector, Single rotx, Single roty, Single speed)
    {
        oVector = RotateVector(new Vector3(0.0f, 0.0f, -speed), rotx, roty);
        oVector.x = (Single)EventEngineUtils.CastFloatToIntWithChecking(oVector.x);
        oVector.y = (Single)EventEngineUtils.CastFloatToIntWithChecking(oVector.y);
        oVector.z = (Single)EventEngineUtils.CastFloatToIntWithChecking(oVector.z);
    }

    private static Vector3 RotateVector(Vector3 inputVector, Single rotx, Single roty)
    {
        return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rotx, roty, 0.0f), Vector3.one).MultiplyVector(inputVector);
    }
}
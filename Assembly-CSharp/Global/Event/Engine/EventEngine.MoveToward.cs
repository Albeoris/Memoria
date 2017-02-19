using System;
using UnityEngine;

public partial class EventEngine
{
    private Boolean MoveToward_mixed(Single x, Single y, Single z, Int32 flags, PosObj flagsPosObj)
    {
        Actor actor = (Actor)this.gCur;
        if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1823 && (Int32)actor.sid == 13 && ((Int32)x == -365 && (Int32)z == -373))
        {
            x = -389f;
            z = -600f;
        }
        if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1550 && (Int32)actor.sid == 15 && ((Int32)x == -1109 && (Int32)z == -1014))
        {
            x = -1155f;
            z = -1070f;
        }


        Single num1 = (Single)actor.speed;
        Single distance = 0.0f;
        PosObj posObj = (PosObj)null;
        FieldMapActorController fieldMapActorController = (FieldMapActorController)null;
        WMActor wmActor = (WMActor)null;
        Single x1 = 0.0f;
        Single y1 = 0.0f;
        Single z1 = 0.0f;
        Vector3 vector3_1 = Vector3.zero;
        if (this.gMode == 1)
        {
            fieldMapActorController = actor.fieldMapActorController;
            if ((UnityEngine.Object)fieldMapActorController == (UnityEngine.Object)null)
            {
                x1 = actor.pos[0];
                y1 = actor.pos[1];
                z1 = actor.pos[2];
            }
            else
            {
                x1 = fieldMapActorController.curPos.x;
                y1 = fieldMapActorController.curPos.y;
                z1 = fieldMapActorController.curPos.z;
            }
            vector3_1 = actor.rotAngle;
        }
        else if (this.gMode == 3)
        {
            wmActor = actor.wmActor;
            x1 = actor.pos[0];
            y1 = actor.pos[1];
            z1 = actor.pos[2];
        }
        Single deltaX = x - x1;
        Single num2 = y - y1;
        Single deltaZ = z - z1;
        Single num3;
        if (((Int32)actor.actf & EventEngine.actLockDir) != 0)
        {
            num3 = EventEngineUtils.ClampAngle(this.gMode != 1 ? wmActor.rot1 : vector3_1.y);
        }
        else
        {
            num3 = this.eBin.angleAsm(deltaX, deltaZ);
            if ((flags & 1) == 0 || flagsPosObj != null)
            {
                Single num4 = EventEngineUtils.ClampAngle(this.gMode != 1 ? wmActor.rot1 : vector3_1.y);
                Single a = num3 - num4;
                if ((Double)a < -180.0)
                    a += 360f;
                if ((Double)a > 180.0 || EventEngineUtils.nearlyEqual(a, 180f))
                    a -= 360f;
                if (((Int32)FF9StateSystem.Common.FF9.fldMapNo != 307 || (Int32)this.gCur.sid != 11) && ((Int32)FF9StateSystem.Common.FF9.fldMapNo != 610 || (Int32)this.gCur.sid != 3) && EventEngineUtils.nearlyEqual(a, 0.0f))
                    actor.actf |= (UInt16)EventEngine.actLockDir;
                Single withOutShiftRight = EventEngineUtils.ConvertFixedPointAngleToDegreeWithOutShiftRight((Int16)((Int32)actor.omega << 3));
                if ((Double)a > 0.0)
                {
                    if ((Double)a > (Double)withOutShiftRight)
                        num3 = num4 + withOutShiftRight;
                }
                else if ((Double)a < -(Double)withOutShiftRight)
                    num3 = num4 - withOutShiftRight;
                num3 = EventEngineUtils.ClampAngle(num3);
                if (this.gMode == 1)
                {
                    vector3_1.y = num3;
                    actor.rotAngle[1] = num3;
                }
                else if (this.gMode == 3)
                    wmActor.rot1 = num3;
            }
        }
        Single deltaY;
        Single rotx;
        if ((flags & 2) != 0 && flagsPosObj == null)
        {
            deltaY = y - y1;
            Single num4 = this.distance(deltaX, 0.0f, deltaZ);
            Single angle = EventEngineUtils.ClampAngle(this.eBin.angleAsm(-deltaY, -num4));
            Single num5 = EventEngineUtils.ClampAngle(actor.rot0);
            Single a = angle - num5;
            if ((Double)a < -180.0)
                a += 360f;
            else if ((Double)a > 180.0 || EventEngineUtils.nearlyEqual(a, 180f))
                a -= 360f;
            Single withOutShiftRight = EventEngineUtils.ConvertFixedPointAngleToDegreeWithOutShiftRight((Int16)((Int32)actor.omega << 3));
            if ((Double)a > 0.0)
            {
                if ((Double)a > (Double)withOutShiftRight)
                    angle = num5 + withOutShiftRight;
            }
            else if ((Double)a < -(Double)withOutShiftRight)
                angle = num5 - withOutShiftRight;
            rotx = EventEngineUtils.ClampAngle(angle);
            actor.rot0 = rotx;
        }
        else
        {
            rotx = 0.0f;
            deltaY = 0.0f;
        }
        Vector3 oVector;
        GetMoveVector(out oVector, rotx, num3, (Single)actor.speed);
        if ((UnityEngine.Object)fieldMapActorController != (UnityEngine.Object)null && fieldMapActorController.name == fieldMapActorController.fieldMap.debugObjName)
        {
            Vector3 vector3_2 = new Vector3(x1, y1, z1);
            Vector3 vector3_3 = oVector;
            Vector3 vector3_4 = vector3_3.normalized * 100f * vector3_3.magnitude;
            Vector3 vector3_5 = new Vector3(0.0f, 100f, 0.0f);
            Vector3 vector3_6 = new Vector3(x, y, z);
            Debug.DrawLine(vector3_2 + vector3_5, vector3_2 + vector3_4 + vector3_5, Color.magenta, 2f, true);
            Debug.DrawLine(vector3_2 + vector3_5, vector3_2 + Vector3.up * 50f + vector3_5, Color.blue, 2f, true);
        }
        Vector3 vector3_7 = new Vector3(x1, y1, z1) + oVector;
        if (this.gMode == 1)
        {
            fieldMapActorController = actor.fieldMapActorController;
            if ((UnityEngine.Object)fieldMapActorController == (UnityEngine.Object)null)
            {
                actor.pos[0] = vector3_7.x;
                actor.pos[1] = vector3_7.y;
                actor.pos[2] = vector3_7.z;
            }
            else
            {
                fieldMapActorController.curPos = fieldMapActorController.curPos + oVector;
                fieldMapActorController.SyncPosToTransform();
            }
        }
        else if (this.gMode == 3)
            wmActor.SetPosition(vector3_7.x, vector3_7.y, vector3_7.z);
        if (this.gMode == 1 && (UnityEngine.Object)fieldMapActorController != (UnityEngine.Object)null)
        {
            if ((Int32)fieldMapActorController.originalActor.uid == 2 && (Int32)FF9StateSystem.Common.FF9.fldMapNo == 1605 && this.eBin.getVarManually(220) == 6622)
                fieldMapActorController.originalActor.collRad = (Byte)10;
            if ((Int32)fieldMapActorController.originalActor.uid == 18 && (Int32)FF9StateSystem.Common.FF9.fldMapNo == 575 && this.eBin.getVarManually(220) == 3165)
                fieldMapActorController.originalActor.collRad = (Byte)34;
            posObj = fieldMapActorController.walkMesh.Collision(fieldMapActorController, 0, out distance);
            if (posObj != null && ((Double)distance < 0.0 || EventEngineUtils.nearlyEqual(distance, 0.0f)))
                fieldMapActorController.curPos = fieldMapActorController.curPos - oVector;
        }
        else if (this.gMode == 3)
        {
            posObj = (PosObj)this.Collision(this, (PosObj)actor, 0, ref distance);
            if (posObj != null && ((Double)distance < 0.0 || EventEngineUtils.nearlyEqual(distance, 0.0f)))
                wmActor.SetPosition(x1, y1, z1);
            actor.pos[0] = vector3_7[0];
            actor.pos[1] = vector3_7[1];
            actor.pos[2] = vector3_7[2];
        }
        if ((Int32)actor.loopCount != 0 && (Int32)actor.loopCount != (Int32)Byte.MaxValue)
            --actor.loopCount;
        Boolean flag1 = (Int32)actor.loopCount != 0;
        if (flagsPosObj != null)
            flag1 = posObj != flagsPosObj || (Double)distance > 0.0;
        Single a1 = this.dist64(deltaX, deltaY, deltaZ);
        if ((Double)a1 < (Double)num1 * (Double)num1 || ((Int32)actor.actf & EventEngine.actMove) != 0 && (Double)a1 > (Double)actor.lastdist)
        {
            if ((Double)a1 < (Double)num1 * (Double)num1 && (Int32)FF9StateSystem.Common.FF9.fldMapNo != 2204 && ((Int32)FF9StateSystem.Common.FF9.fldMapNo != 2209 || PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(220) != 9850) && (this.gMode == 1 && (UnityEngine.Object)fieldMapActorController != (UnityEngine.Object)null))
            {
                fieldMapActorController.curPos.x = x;
                fieldMapActorController.curPos.z = z;
                fieldMapActorController.SyncPosToTransform();
            }
            flag1 = false;
        }
        Boolean flag2 = false;
        if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 901)
        {
            if ((Int32)actor.sid == 1)
                flag2 = true;
        }
        else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1808)
        {
            if ((Int32)actor.sid == 4 || (Int32)actor.sid == 5)
                flag2 = true;
        }
        else if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 1810 && (Int32)actor.sid == 5)
            flag2 = true;
        if (flag2 && (Double)a1 > (Double)actor.lastdist && (posObj != null && (Int32)posObj.uid == (Int32)this.GetControlUID()))
            flag1 = true;
        if ((Double)actor.lastdist < (Double)EventEngine.kInitialDist && (Double)a1 < (Double)actor.lastdist)
            actor.actf |= (UInt16)EventEngine.actMove;
        if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 571 && (Int32)actor.sid == 4 && (EventEngineUtils.nearlyEqual(x, 887f) && EventEngineUtils.nearlyEqual(z, 1419f)) && (Double)a1 > (Double)actor.lastdist)
        {
            fieldMapActorController.curPos.x = 887f;
            fieldMapActorController.curPos.z = 1419f;
            fieldMapActorController.SyncPosToTransform();
            return true;
        }
        actor.lastdist = a1;
        if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2954 && (Int32)actor.sid == 11 && EventEngineUtils.nearlyEqual(a1, 32420f))
            return false;
        if (!flag1)
            this.clrdist(actor);
        if ((Int32)actor.uid == (Int32)this._context.controlUID)
            ++this.gAnimCount;
        return flag1;
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
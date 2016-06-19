using UnityEngine;

public partial class EventEngine
{
    private bool MoveToward_mixed(float x, float y, float z, int flags, PosObj flagsPosObj)
    {
        Actor actor = (Actor)this.gCur;
        if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1823 && (int)actor.sid == 13 && ((int)x == -365 && (int)z == -373))
        {
            x = -389f;
            z = -600f;
        }
        if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1550 && (int)actor.sid == 15 && ((int)x == -1109 && (int)z == -1014))
        {
            x = -1155f;
            z = -1070f;
        }


        float num1 = (float)actor.speed;
        float distance = 0.0f;
        PosObj posObj = (PosObj)null;
        FieldMapActorController fieldMapActorController = (FieldMapActorController)null;
        WMActor wmActor = (WMActor)null;
        float x1 = 0.0f;
        float y1 = 0.0f;
        float z1 = 0.0f;
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
        float deltaX = x - x1;
        float num2 = y - y1;
        float deltaZ = z - z1;
        float num3;
        if (((int)actor.actf & EventEngine.actLockDir) != 0)
        {
            num3 = EventEngineUtils.ClampAngle(this.gMode != 1 ? wmActor.rot1 : vector3_1.y);
        }
        else
        {
            num3 = this.eBin.angleAsm(deltaX, deltaZ);
            if ((flags & 1) == 0 || flagsPosObj != null)
            {
                float num4 = EventEngineUtils.ClampAngle(this.gMode != 1 ? wmActor.rot1 : vector3_1.y);
                float a = num3 - num4;
                if ((double)a < -180.0)
                    a += 360f;
                if ((double)a > 180.0 || EventEngineUtils.nearlyEqual(a, 180f))
                    a -= 360f;
                if (((int)FF9StateSystem.Common.FF9.fldMapNo != 307 || (int)this.gCur.sid != 11) && ((int)FF9StateSystem.Common.FF9.fldMapNo != 610 || (int)this.gCur.sid != 3) && EventEngineUtils.nearlyEqual(a, 0.0f))
                    actor.actf |= (ushort)EventEngine.actLockDir;
                float withOutShiftRight = EventEngineUtils.ConvertFixedPointAngleToDegreeWithOutShiftRight((short)((int)actor.omega << 3));
                if ((double)a > 0.0)
                {
                    if ((double)a > (double)withOutShiftRight)
                        num3 = num4 + withOutShiftRight;
                }
                else if ((double)a < -(double)withOutShiftRight)
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
        float deltaY;
        float rotx;
        if ((flags & 2) != 0 && flagsPosObj == null)
        {
            deltaY = y - y1;
            float num4 = this.distance(deltaX, 0.0f, deltaZ);
            float angle = EventEngineUtils.ClampAngle(this.eBin.angleAsm(-deltaY, -num4));
            float num5 = EventEngineUtils.ClampAngle(actor.rot0);
            float a = angle - num5;
            if ((double)a < -180.0)
                a += 360f;
            else if ((double)a > 180.0 || EventEngineUtils.nearlyEqual(a, 180f))
                a -= 360f;
            float withOutShiftRight = EventEngineUtils.ConvertFixedPointAngleToDegreeWithOutShiftRight((short)((int)actor.omega << 3));
            if ((double)a > 0.0)
            {
                if ((double)a > (double)withOutShiftRight)
                    angle = num5 + withOutShiftRight;
            }
            else if ((double)a < -(double)withOutShiftRight)
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
        GetMoveVector(out oVector, rotx, num3, (float)actor.speed);
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
            if ((int)fieldMapActorController.originalActor.uid == 2 && (int)FF9StateSystem.Common.FF9.fldMapNo == 1605 && this.eBin.getVarManually(220) == 6622)
                fieldMapActorController.originalActor.collRad = (byte)10;
            if ((int)fieldMapActorController.originalActor.uid == 18 && (int)FF9StateSystem.Common.FF9.fldMapNo == 575 && this.eBin.getVarManually(220) == 3165)
                fieldMapActorController.originalActor.collRad = (byte)34;
            posObj = fieldMapActorController.walkMesh.Collision(fieldMapActorController, 0, out distance);
            if (posObj != null && ((double)distance < 0.0 || EventEngineUtils.nearlyEqual(distance, 0.0f)))
                fieldMapActorController.curPos = fieldMapActorController.curPos - oVector;
        }
        else if (this.gMode == 3)
        {
            posObj = (PosObj)this.Collision(this, (PosObj)actor, 0, ref distance);
            if (posObj != null && ((double)distance < 0.0 || EventEngineUtils.nearlyEqual(distance, 0.0f)))
                wmActor.SetPosition(x1, y1, z1);
            actor.pos[0] = vector3_7[0];
            actor.pos[1] = vector3_7[1];
            actor.pos[2] = vector3_7[2];
        }
        if ((int)actor.loopCount != 0 && (int)actor.loopCount != (int)byte.MaxValue)
            --actor.loopCount;
        bool flag1 = (int)actor.loopCount != 0;
        if (flagsPosObj != null)
            flag1 = posObj != flagsPosObj || (double)distance > 0.0;
        float a1 = this.dist64(deltaX, deltaY, deltaZ);
        if ((double)a1 < (double)num1 * (double)num1 || ((int)actor.actf & EventEngine.actMove) != 0 && (double)a1 > (double)actor.lastdist)
        {
            if ((double)a1 < (double)num1 * (double)num1 && (int)FF9StateSystem.Common.FF9.fldMapNo != 2204 && ((int)FF9StateSystem.Common.FF9.fldMapNo != 2209 || PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(220) != 9850) && (this.gMode == 1 && (UnityEngine.Object)fieldMapActorController != (UnityEngine.Object)null))
            {
                fieldMapActorController.curPos.x = x;
                fieldMapActorController.curPos.z = z;
                fieldMapActorController.SyncPosToTransform();
            }
            flag1 = false;
        }
        bool flag2 = false;
        if ((int)FF9StateSystem.Common.FF9.fldMapNo == 901)
        {
            if ((int)actor.sid == 1)
                flag2 = true;
        }
        else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1808)
        {
            if ((int)actor.sid == 4 || (int)actor.sid == 5)
                flag2 = true;
        }
        else if ((int)FF9StateSystem.Common.FF9.fldMapNo == 1810 && (int)actor.sid == 5)
            flag2 = true;
        if (flag2 && (double)a1 > (double)actor.lastdist && (posObj != null && (int)posObj.uid == (int)this.GetControlUID()))
            flag1 = true;
        if ((double)actor.lastdist < (double)EventEngine.kInitialDist && (double)a1 < (double)actor.lastdist)
            actor.actf |= (ushort)EventEngine.actMove;
        if ((int)FF9StateSystem.Common.FF9.fldMapNo == 571 && (int)actor.sid == 4 && (EventEngineUtils.nearlyEqual(x, 887f) && EventEngineUtils.nearlyEqual(z, 1419f)) && (double)a1 > (double)actor.lastdist)
        {
            fieldMapActorController.curPos.x = 887f;
            fieldMapActorController.curPos.z = 1419f;
            fieldMapActorController.SyncPosToTransform();
            return true;
        }
        actor.lastdist = a1;
        if ((int)FF9StateSystem.Common.FF9.fldMapNo == 2954 && (int)actor.sid == 11 && EventEngineUtils.nearlyEqual(a1, 32420f))
            return false;
        if (!flag1)
            this.clrdist(actor);
        if ((int)actor.uid == (int)this._context.controlUID)
            ++this.gAnimCount;
        return flag1;
    }

    private static void GetMoveVector(out Vector3 oVector, float rotx, float roty, float speed)
    {
        oVector = RotateVector(new Vector3(0.0f, 0.0f, -speed), rotx, roty);
        oVector.x = (float)EventEngineUtils.CastFloatToIntWithChecking(oVector.x);
        oVector.y = (float)EventEngineUtils.CastFloatToIntWithChecking(oVector.y);
        oVector.z = (float)EventEngineUtils.CastFloatToIntWithChecking(oVector.z);
    }

    private static Vector3 RotateVector(Vector3 inputVector, float rotx, float roty)
    {
        return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rotx, roty, 0.0f), Vector3.one).MultiplyVector(inputVector);
    }
}
using System;
using UnityEngine;
using Memoria;

public partial class EventEngine
{
    private bool MoveToward_mixed(Single x, Single y, Single z, Int32 flags, PosObj followPosObj)
    {
        Actor actor = (Actor)this.gCur;
        if (FF9StateSystem.Common.FF9.fldMapNo == 1823 && actor.sid == 13 && x == -365 && z == -373)
        {
            // A. Castle/Hallway, Steiner, one of the mid-steps before saying "Princess, we are ready."
            x = -389f;
            z = -600f;
        }
        if (FF9StateSystem.Common.FF9.fldMapNo == 1550 && actor.sid == 15 && x == -1109 && z == -1014)
        {
            // Mountain Path/Trail, Dagger, the mid-step right after Eiko says "W-Wait!!!  Don’t leave me here!!!"
            x = -1155f;
            z = -1070f;
        }
        GameObject go = this.gCur.go;
        Single sqrSpeed = actor.speed * actor.speed;
        Single moveDistance = 0f;
        PosObj posObj = null;
        FieldMapActorController actorController = null;
        WMActor wmactor = null;
        Single curX = 0f;
        Single curY = 0f;
        Single curZ = 0f;
        Vector3 actorRotAngle = Vector3.zero;
        if (this.gMode == 1)
        {
            actorController = actor.fieldMapActorController;
            if (actorController == null)
            {
                curX = actor.pos[0];
                curY = actor.pos[1];
                curZ = actor.pos[2];
            }
            else
            {
                curX = actorController.curPos.x;
                curY = actorController.curPos.y;
                curZ = actorController.curPos.z;
            }
            actorRotAngle = actor.rotAngle;
        }
        else if (this.gMode == 3)
        {
            wmactor = actor.wmActor;
            curX = actor.pos[0];
            curY = actor.pos[1];
            curZ = actor.pos[2];
        }

        Single deltaX = x - curX;
        Single deltaY = y - curY;
        Single deltaZ = z - curZ;
        Single movingAngle;
        Single movingPitch;
        if ((actor.actf & EventEngine.actLockDir) != 0)
        {
            movingAngle = EventEngineUtils.ClampAngle(this.gMode != 1 ? wmactor.rot1 : actorRotAngle.y);
        }
        else
        {
            movingAngle = this.eBin.angleAsm(deltaX, deltaZ);
            if ((flags & 1) == 0 || followPosObj != null)
            {
                Single facingAngle = EventEngineUtils.ClampAngle(this.gMode != 1 ? wmactor.rot1 : actorRotAngle.y);
                Single angleMoveFaceDiff = movingAngle - facingAngle;
                if (angleMoveFaceDiff < -180f)
                    angleMoveFaceDiff += 360f;
                if (angleMoveFaceDiff > 180f || EventEngineUtils.nearlyEqual(angleMoveFaceDiff, 180f))
                    angleMoveFaceDiff -= 360f;
                if ((FF9StateSystem.Common.FF9.fldMapNo != 307 || this.gCur.sid != 11) && (FF9StateSystem.Common.FF9.fldMapNo != 610 || this.gCur.sid != 3) && EventEngineUtils.nearlyEqual(angleMoveFaceDiff, 0f))
                {
                    // Always except (Ice Cavern/Ice Path, Zidane) and (L. Castle/Castle Station, Lindblum_OldAviator)
                    actor.actf |= (UInt16)EventEngine.actLockDir;
                }
                Single actorOmega = EventEngineUtils.ConvertFixedPointAngleToDegreeWithOutShiftRight(actor.omega << 3);
                if (angleMoveFaceDiff > 0f)
                {
                    if (angleMoveFaceDiff > actorOmega)
                        movingAngle = facingAngle + actorOmega;
                }
                else if (angleMoveFaceDiff < -actorOmega)
                {
                    movingAngle = facingAngle - actorOmega;
                }
                movingAngle = EventEngineUtils.ClampAngle(movingAngle);
                if (this.gMode == 1)
                {
                    actorRotAngle.y = movingAngle;
                    actor.rotAngle[1] = movingAngle;
                }
                else if (this.gMode == 3)
                {
                    wmactor.rot1 = movingAngle;
                }
            }
        }
        if ((flags & 2) != 0 && followPosObj == null)
        {
            deltaY = y - curY;
            movingPitch = EventEngineUtils.ClampAngle(this.eBin.angleAsm(-deltaY, -this.distance(deltaX, 0f, deltaZ)));
            Single previousPitch = EventEngineUtils.ClampAngle(actor.rot0);
            Single pitchDiff = movingPitch - previousPitch;
            if (pitchDiff < -180f)
                pitchDiff += 360f;
            else if (pitchDiff > 180f || EventEngineUtils.nearlyEqual(pitchDiff, 180f))
                pitchDiff -= 360f;
            Single actorOmega = EventEngineUtils.ConvertFixedPointAngleToDegreeWithOutShiftRight(actor.omega << 3);
            if (pitchDiff > 0f)
            {
                if (pitchDiff > actorOmega)
                    movingPitch = previousPitch + actorOmega;
            }
            else if (pitchDiff < -actorOmega)
            {
                movingPitch = previousPitch - actorOmega;
            }
            movingPitch = EventEngineUtils.ClampAngle(movingPitch);
            actor.rot0 = movingPitch;
        }
        else
        {
            movingPitch = 0f;
            deltaY = 0f;
        }

        EventEngine.GetMoveVector(out Vector3 moveVec, movingPitch, movingAngle, actor.speed);
        if (Configuration.Control.PSXMovementMethod && (flags & 2) == 0 && actorController != null)
            moveVec *= actorController.fieldMap.walkMesh.GetTriangleSlopeFactor(actorController.activeTri);

        if (actorController != null && actorController.name == actorController.fieldMap.debugObjName)
        {
            Vector3 curPos = new Vector3(curX, curY, curZ);
            Vector3 lineDest = moveVec.normalized * 100f * moveVec.magnitude;
            Vector3 lineOffset = new Vector3(0f, 100f, 0f);
            Vector3 dest = new Vector3(x, y, z);
            global::Debug.DrawLine(curPos + lineOffset, curPos + lineDest + lineOffset, Color.magenta, 2f, true);
            global::Debug.DrawLine(curPos + lineOffset, curPos + Vector3.up * 50f + lineOffset, Color.blue, 2f, true);
        }

        Vector3 newPos = new Vector3(curX, curY, curZ) + moveVec;
        if (this.gMode == 1)
        {
            actorController = actor.fieldMapActorController;
            if (actorController == null)
            {
                actor.pos[0] = newPos.x;
                actor.pos[1] = newPos.y;
                actor.pos[2] = newPos.z;
            }
            else
            {
                actorController.curPos += moveVec;
                actorController.SyncPosToTransform();
            }
        }
        else if (this.gMode == 3)
        {
            wmactor.SetPosition(newPos.x, newPos.y, newPos.z);
        }

        if (this.gMode == 1 && actorController != null)
        {
            if (actorController.originalActor.uid == 2 && FF9StateSystem.Common.FF9.fldMapNo == 1605 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 6622)
            {
                // Mdn. Sari/Eidolon Wall, Moogle_Male, first entrance
                actorController.originalActor.collRad = 10;
            }
            if (actorController.originalActor.uid == 18 && FF9StateSystem.Common.FF9.fldMapNo == 575 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 3165)
            {
                // Lindblum/Festival, Steiner, cutscene with Steiner and Dagger
                actorController.originalActor.collRad = 34;
            }
            posObj = actorController.walkMesh.Collision(actorController, 0, out moveDistance);
            if (posObj != null && (moveDistance < 0f || EventEngineUtils.nearlyEqual(moveDistance, 0f)))
                actorController.curPos -= moveVec;
        }
        else if (this.gMode == 3)
        {
            posObj = (PosObj)this.Collision(this, actor, 0, ref moveDistance);
            if (posObj != null && (moveDistance < 0f || EventEngineUtils.nearlyEqual(moveDistance, 0f)))
                wmactor.SetPosition(curX, curY, curZ);
            actor.pos[0] = newPos[0];
            actor.pos[1] = newPos[1];
            actor.pos[2] = newPos[2];
        }

        if (actor.loopCount != 0 && actor.loopCount != 255)
            actor.loopCount--;
        Boolean syncMove = actor.loopCount != 0;
        if (followPosObj != null)
            syncMove = posObj != followPosObj || moveDistance > 0f;
        moveDistance = this.dist64(deltaX, deltaY, deltaZ);
        Boolean distanceHasIncreased = moveDistance > actor.lastdist;
        if (FF9StateSystem.Common.FF9.fldMapNo == 456 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 2800 && actor.sid == 2)
        {
            // Mountain/Summit, Grandpa
            distanceHasIncreased = distanceHasIncreased && !EventEngineUtils.nearlyEqual(moveDistance, actor.lastdist);
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 455 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 2800 && actor.sid == 1)
        {
            // Mountain/Base, Grandpa
            distanceHasIncreased = distanceHasIncreased && !EventEngineUtils.nearlyEqual(moveDistance, actor.lastdist);
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 1055 && actor.sid == 11)
        {
            // Cleyra/Town Area, Cleyran_Priest
            distanceHasIncreased = distanceHasIncreased && !EventEngineUtils.nearlyEqual(moveDistance, actor.lastdist);
        }
        if (moveDistance < sqrSpeed || ((actor.actf & EventEngine.actMove) != 0 && distanceHasIncreased))
        {
            if (moveDistance < sqrSpeed && this.gMode == 1 && actorController != null
                && FF9StateSystem.Common.FF9.fldMapNo != 2204
                && (FF9StateSystem.Common.FF9.fldMapNo != 2209 || PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) != 9850))
            {
                // Always except (Palace/Odyssey) and (Palace/Sanctum, cutscene in which Kuja steals the Gulug Stone)
                actorController.curPos.x = x;
                actorController.curPos.z = z;
                actorController.SyncPosToTransform();
            }
            syncMove = false;
        }
        Boolean forceSyncMove = false;
        if (FF9StateSystem.Common.FF9.fldMapNo == 901 && actor.sid == 1)
        {
            // Treno/Bishop's House, Gilgamesh
            forceSyncMove = true;
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 1808 && (actor.sid == 4 || actor.sid == 5))
        {
            // A. Castle/Library, Scholars
            forceSyncMove = true;
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 1810 && actor.sid == 5)
        {
            // A. Castle/Kitchen, Cook cooking a fish
            forceSyncMove = true;
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 456 && actor.sid == 2 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 2800)
        {
            // Mountain/Summit, Grandpa
            forceSyncMove = true;
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 455 && actor.sid == 1 && this.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 2800)
        {
            // Mountain/Base, Grandpa
            forceSyncMove = true;
        }
        else if (FF9StateSystem.Common.FF9.fldMapNo == 1055 && actor.sid == 11)
        {
            // Cleyra/Town Area, Cleyran_Priest
            forceSyncMove = true;
        }
        if (forceSyncMove && distanceHasIncreased && posObj != null && posObj.uid == this.GetControlUID())
            syncMove = true;

        if (actor.lastdist < EventEngine.kInitialDist && moveDistance < actor.lastdist)
            actor.actf |= (UInt16)EventEngine.actMove;
        if (FF9StateSystem.Common.FF9.fldMapNo == 571 && actor.sid == 4 && EventEngineUtils.nearlyEqual(x, 887f) && EventEngineUtils.nearlyEqual(z, 1419f) && moveDistance > actor.lastdist)
        {
            // Lindblum/The Doom Pub, Pub_Barman, repeated movement forward/backward while serving clients
            actorController.curPos.x = 887f;
            actorController.curPos.z = 1419f;
            actorController.SyncPosToTransform();
            return true;
        }
        actor.lastdist = moveDistance;
        if (FF9StateSystem.Common.FF9.fldMapNo == 2954 && actor.sid == 11 && EventEngineUtils.nearlyEqual(moveDistance, 32420f))
        {
            // Chocobo's Paradise, Moogle_Male, some movement when trying to leave without Choco
            return false;
        }

        if (!syncMove)
            this.clrdist(actor);
        if (actor.uid == this._context.controlUID)
            this.gAnimCount++;
        return syncMove;
    }

    private static void GetMoveVector(out Vector3 oVector, Single rotx, Single roty, Single speed)
    {
        oVector = RotateVector(new Vector3(0.0f, 0.0f, -speed), rotx, roty);
        oVector.x = EventEngineUtils.CastFloatToIntWithChecking(oVector.x);
        oVector.y = EventEngineUtils.CastFloatToIntWithChecking(oVector.y);
        oVector.z = EventEngineUtils.CastFloatToIntWithChecking(oVector.z);
    }

    private static Vector3 RotateVector(Vector3 inputVector, Single rotx, Single roty)
    {
        return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rotx, roty, 0.0f), Vector3.one).MultiplyVector(inputVector);
    }
}
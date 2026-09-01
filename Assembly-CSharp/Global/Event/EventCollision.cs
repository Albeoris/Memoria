using System;
using UnityEngine;

public class EventCollision
{
    public static Int32 CollisionAngle(PosObj po, Obj coll)
    {
        PosObj posObj = (PosObj)coll;
        Vector3 b = new Vector3(po.pos[0], po.pos[1], po.pos[2]);
        Vector3 a = new Vector3(posObj.pos[0], posObj.pos[1], posObj.pos[2]);
        Vector3 posObjRot = EventCollision.GetPosObjRot(po);
        Vector3 posObjRot2 = EventCollision.GetPosObjRot(posObj);
        Vector3 normalized = (a - b).normalized;
        if (normalized == Vector3.zero)
        {
            return 0;
        }
        Vector3 eulerAngles = Quaternion.LookRotation(normalized).eulerAngles;
        Vector3 vector = eulerAngles - posObjRot;
        vector.x = ((vector.x <= 180f) ? vector.x : (vector.x - 360f));
        vector.x = ((vector.x >= -180f) ? vector.x : (vector.x + 360f));
        vector.y = ((vector.y <= 180f) ? vector.y : (vector.y - 360f));
        vector.y = ((vector.y >= -180f) ? vector.y : (vector.y + 360f));
        vector.z = ((vector.z <= 180f) ? vector.z : (vector.z - 360f));
        vector.z = ((vector.z >= -180f) ? vector.z : (vector.z + 360f));
        Single floatAngle = vector.magnitude - 180f;
        return EventEngineUtils.ConvertFloatAngleToFixedPoint(floatAngle);
    }

    public static Vector3 GetPosObjRot(PosObj po)
    {
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        Vector3 result = Vector3.zero;
        if (instance.gMode == 1)
            result = po.rotAngle;
        else if (instance.gMode == 3)
            result = ((Actor)po).wmActor.rot;
        return result;
    }

    public static void BubbleUIListener(PosObj userObject, Obj collObj, UInt32 key)
    {
        if (userObject != null)
        {
            if (userObject.cid == 4)
                EventCollision.CheckNPCInput(userObject);
            else
                EventCollision.CheckQuadInput(userObject);
        }
    }

    public static Boolean CheckQuadInput(PosObj po)
    {
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        UInt32 interactInput = ETb.KeyOn() & (instance.gMode != 1 ? EventInput.Confirm : (EventInput.Confirm | EventInput.Special));
        if (interactInput > 0u)
        {
            Obj obj = instance.TreadQuad(po, 4);
            if (obj != null && EventCollision.IsQuadTalkable(po, obj))
            {
                if (interactInput == EventInput.Special && instance.Request(obj, 1, 8, false))
                {
                    EventCollision.ClearPathFinding(po);
                    EMinigame.SetQuadmistOpponentId(obj);
                    return true;
                }
                if (instance.Request(obj, 1, 3, false))
                {
                    EventCollision.ClearPathFinding(po);
                    return true;
                }
            }
        }
        return false;
    }

    public static Boolean CheckNPCInput(PosObj po)
    {
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        UInt32 interactInput = ETb.KeyOn() & (instance.gMode != 1 ? EventInput.Confirm : (EventInput.Confirm | EventInput.Special));
        if (interactInput > 0u)
        {
            Single distance = instance.nilFloat;
            Obj obj = EventCollision.Collision(instance, po, 4, ref distance);
            if (obj != null && EventCollision.IsNPCTalkable(obj))
            {
                EventCollision.sSysAngle = EventCollision.CollisionAngle(po, obj);
                if (EventCollision.sSysAngle > -1024 && EventCollision.sSysAngle < 1024)
                {
                    ((Actor)po).listener = obj.uid;
                    if (interactInput == EventInput.Special)
                    {
                        if (instance.Request(obj, 1, 8, false))
                        {
                            EventCollision.ClearPathFinding(po);
                            EMinigame.SetQuadmistOpponentId(obj);
                            return true;
                        }
                    }
                    if (instance.Request(obj, 1, 3, false))
                    {
                        EventCollision.ClearPathFinding(po);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private static void ShowDebugTalk(Actor actor1, Single r)
    {
        for (Int32 i = 0; i < 10; i++)
        {
            Vector3 position = actor1.go.transform.position;
            Vector3 position2 = actor1.go.transform.position;
            Vector3 vector = new Vector3((Single)i, 0f, (Single)(9 - i));
            global::Debug.DrawLine(position, position2 + vector.normalized * r, Color.blue, 0.5f, true);
            Vector3 position3 = actor1.go.transform.position;
            Vector3 position4 = actor1.go.transform.position;
            Vector3 vector2 = new Vector3((Single)(-(Single)i), 0f, (Single)(9 - i));
            global::Debug.DrawLine(position3, position4 + vector2.normalized * r, Color.blue, 0.5f, true);
            Vector3 position5 = actor1.go.transform.position;
            Vector3 position6 = actor1.go.transform.position;
            Vector3 vector3 = new Vector3((Single)i, 0f, (Single)(-9 + i));
            global::Debug.DrawLine(position5, position6 + vector3.normalized * r, Color.blue, 0.5f, true);
            Vector3 position7 = actor1.go.transform.position;
            Vector3 position8 = actor1.go.transform.position;
            Vector3 vector4 = new Vector3((Single)(-(Single)i), 0f, (Single)(-9 + i));
            global::Debug.DrawLine(position7, position8 + vector4.normalized * r, Color.blue, 0.5f, true);
        }
    }

    private static void ClearPathFinding(PosObj po)
    {
        if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
            po.go.GetComponent<FieldMapActorController>().ClearMoveTargetAndPath();
    }

    public static Obj Collision(EventEngine eventEngine, PosObj po, Int32 mode, ref Single distance)
    {
        if (eventEngine.gMode == 1)
        {
            // Field collision
            FieldMapActorController actorController = po.go.GetComponent<FieldMapActorController>();
            if (actorController == null)
                return null;
            return actorController.walkMesh.Collision(actorController, mode, out distance);
        }
        // World map collision (or battle collision?)
        Obj result = null;
        Single closestDist = Single.MaxValue;
        Boolean isTalk = (mode & 4) != 0;
        Byte poCollisionKind = (Byte)(po.uid != eventEngine.GetControlUID() ? 4 : 2);
        Int32 poCollRadius = 4 * (isTalk ? po.talkRad : po.collRad);
        Vector3 wmPos = Vector3.zero;
        if (eventEngine.gMode == 3)
            wmPos = ((Actor)po).wmActor.RealPosition;
        for (ObjList objList = eventEngine.GetActiveObjList(); objList != null; objList = objList.next)
        {
            Obj obj = objList.obj;
            if (obj != po && obj.cid == 4)
            {
                Byte collisionKind = (Byte)(obj.uid != eventEngine.GetControlUID() ? 4 : 2);
                Boolean disabledCollision = (po.flags & collisionKind) != 0;
                Byte interactMask = (Byte)(isTalk ? 8 : poCollisionKind);
                Boolean canInteract = (!isTalk && !disabledCollision) || (obj.flags & interactMask) == 0;
                if (canInteract)
                {
                    Boolean ignoreDisables = (mode & 6) == 0;
                    Boolean hasRelatedFunction = eventEngine.GetIP(obj.sid, isTalk ? 3 : 2, obj.ebData) != eventEngine.nil;
                    if (ignoreDisables || hasRelatedFunction)
                    {
                        Actor actor = (Actor)obj;
                        Single checkDist = 0f;
                        Int32 collRadius = 4 * (isTalk ? actor.talkRad : actor.collRad);
                        PosObj posObj = (PosObj)obj;
                        if (posObj.ovalRatio > 0)
                            collRadius = EventCollision.CalculateRadiusFromOvalRatio(po, posObj, collRadius);
                        collRadius += poCollRadius;
                        if ((mode & 6) != 0)
                            collRadius += actor.speed + 60;
                        if (eventEngine.gMode == 3)
                            checkDist = Vector3.Distance(wmPos, actor.wmActor.RealPosition) * 256f;
                        if (collRadius > checkDist && closestDist > checkDist)
                        {
                            result = actor;
                            closestDist = checkDist;
                        }
                    }
                }
            }
        }
        if (distance > 0f)
            distance = closestDist;
        return result;
    }

    private static Int32 CalculateRadiusFromOvalRatio(PosObj po, PosObj targetPosObj, Int32 radius)
    {
        Int32 fixedPointAngle = EventCollision.CollisionAngle(targetPosObj, po);
        Int32 collCos = ff9.rcos(fixedPointAngle);
        Int32 maxRadius = (collCos * collCos >> 4) * targetPosObj.ovalRatio + 16777216;
        radius = Convert.ToInt32(radius * ff9.SquareRoot0(maxRadius)) >> 12;
        return radius;
    }

    public static void CollisionRequest(PosObj po)
    {
        Boolean flag = false;
        EventEngine instance = PersistenSingleton<EventEngine>.Instance;
        Single nilFloat = instance.nilFloat;
        Obj obj;
        if (EventCollision.CheckNPCInput(po))
        {
            if (instance.gMode != 3)
                return;
            obj = EventCollision.Collision(instance, po, 2, ref nilFloat);
        }
        else
        {
            obj = EventCollision.Collision(instance, po, 4, ref nilFloat);
            if (obj != null)
            {
                EventCollision.sSysAngle = EventCollision.CollisionAngle(po, obj);
                if (EventCollision.sSysAngle > -1024 && EventCollision.sSysAngle < 1024)
                {
                    if (EventCollision.IsNPCTalkable(obj))
                        flag = EIcon.PollCollisionIcon(obj);
                    if (!flag)
                        obj = EventCollision.Collision(instance, po, 2, ref nilFloat);
                }
                else
                {
                    obj = EventCollision.Collision(instance, po, 2, ref nilFloat);
                }
            }
            else
            {
                obj = EventCollision.Collision(instance, po, 2, ref nilFloat);
                if (instance.gMode == 3 && obj != null)
                {
                    WMActor wmActor = ((Actor)po).wmActor;
                    if (wmActor.ControlNo == 0)
                        flag = EIcon.PollCollisionIcon(obj);
                }
            }
        }
        if (obj != null && EventCollision.CheckNPCPush((PosObj)obj))
            instance.Request(obj, 1, 2, false);
        if (EventCollision.CheckQuadInput(po))
            return;
        obj = instance.TreadQuad(po, 2);
        if (obj != null)
        {
            Boolean flag2 = EventCollision.CheckQuadPush(po, obj) && instance.Request(obj, 1, 2, false);
            if (flag2)
            {
                if (instance.GetIP((Int32)obj.sid, 8, obj.ebData) != instance.nil)
                {
                    EIcon.PollFIcon(BubbleUI.IconType.ExclamationAndDuel);
                }
                else
                {
                    Int16 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
                    if (fldMapNo == 2108)
                    {
                        if (EventCollision.CheckQuadTalk(po, obj))
                            EIcon.PollFIcon(BubbleUI.IconType.Exclamation);
                    }
                }
            }
        }
        obj = instance.TreadQuad(po, 4);
        if (obj != null && EventCollision.CheckQuadTalk(po, obj) && EventCollision.IsQuadTalkable(po, obj))
            EIcon.PollCollisionIcon(obj);
        if (instance.gMode == 3 && obj == null)
        {
            if (EventCollision.IsChocoboWalkingOrFlyingInForestArea())
                EIcon.PollFIcon(BubbleUI.IconType.Exclamation);
            else if (!flag && EMinigame.CheckBeachMinigame())
                EIcon.PollFIcon(BubbleUI.IconType.Beach);
        }
    }

    public static Boolean IsChocoboFlyingOverForest()
    {
        return WMUIData.ControlNo == 6 && WMUIData.StatusNo == 7 && ff9.m_GetIDTopograph(ff9.m_moveActorID) >= 36 && ff9.m_GetIDTopograph(ff9.m_moveActorID) <= 38;
    }

    public static Boolean IsChocoboWalkingOrFlyingInForestArea()
    {
        return (WMUIData.ControlNo == 5 || WMUIData.ControlNo == 6) && WMUIData.StatusNo == 7 && ff9.m_GetIDTopograph(ff9.m_moveActorID) >= 36 && ff9.m_GetIDTopograph(ff9.m_moveActorID) <= 38;
    }

    public static Boolean IsChocoboWalkingInForestArea()
    {
        return WMUIData.ControlNo == 5 && ff9.m_GetIDTopograph(ff9.m_moveActorID) >= 36 && ff9.m_GetIDTopograph(ff9.m_moveActorID) <= 38;
    }

    public static Boolean IsRidingChocobo()
    {
        return WMUIData.ControlNo >= 1 && WMUIData.ControlNo <= 6;
    }

    private static Boolean CheckNPCPush(PosObj po)
    {
        Boolean result = true;
        if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
        {
            Int16 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
            if (fldMapNo != 103 && fldMapNo != 107)
            {
                if (fldMapNo == 1856)
                {
                    Byte sid = po.sid;
                    if (sid == 5 || sid == 6)
                    {
                        result = false;
                    }
                }
            }
            else
            {
                Byte sid = po.sid;
                if (sid == 3 || sid == 4)
                {
                    result = false;
                }
            }
        }
        return result;
    }

    private static Boolean CheckQuadPush(PosObj ctrl, Obj quad)
    {
        Boolean result = true;
        if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
        {
            Int16 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
            if (fldMapNo != 2108)
            {
                if (fldMapNo != 2802)
                {
                    if (fldMapNo == 2914)
                    {
                        Byte sid = quad.sid;
                        if (sid == 13)
                        {
                            result = false;
                        }
                    }
                }
                else if (quad.sid == 24)
                {
                    result = false;
                }
            }
            else if (quad.sid == 6)
            {
                result = EventCollision.IsQuadTalkable(ctrl, quad);
            }
        }
        return result;
    }

    private static Boolean CheckQuadTalk(PosObj ctrl, Obj quad)
    {
        if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
        {
            Int16 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
            if (fldMapNo == 2504) // I. Castle/Small Room
                return quad.sid != 9;
            if (fldMapNo == 2108) // Lindblum/Synthesist
                return quad.sid != 7;
        }
        return true;
    }

    public static Boolean IsWorldTrigger()
    {
        WMActor controlChar = ff9.GetControlChar();
        if (controlChar != null)
        {
            ff9.s_moveCHRStatus s_moveCHRStatus = ff9.w_moveCHRStatus[controlChar.originalActor.index];
            return ff9.m_GetIDEvent(s_moveCHRStatus.id) != 0 && ff9.w_frameEventEnable;
        }
        return false;
    }

    private static Int32 GetDir(Actor actor)
    {
        Single floatAngle = actor.rotAngle[1];
        Int32 num = EventEngineUtils.ConvertFloatAngleToFixedPoint(floatAngle);
        return num >> 4 & 255;
    }

    private static Boolean IsQuadTalkable(PosObj ctrl, Obj quad)
    {
        if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
        {
            Obj obj = null;
            Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
            Int32 uid = quad.uid;
            Int32 key = EMinigame.CreateNPCID(fldMapNo, uid);
            if (EventEngineUtils.QuadTalkableData.ContainsKey(key))
                obj = PersistenSingleton<EventEngine>.Instance.GetObjByUID(EventEngineUtils.QuadTalkableData[key]);
            if (obj != null)
            {
                Int32 angle;
                if (fldMapNo == 2108) // Lindblum/Synthesist, Lindblum_ManB
                {
                    angle = EventCollision.GetDir((Actor)ctrl);
                    return angle > 90 && angle < 160;
                }
                if (fldMapNo == 2109) // Lindblum/Wpn. Shop, Lindblum_WorkerA
                {
                    angle = EventCollision.GetDir((Actor)ctrl);
                    return angle > 159 && angle < 223;
                }
                if (fldMapNo == 2103) // Lindblum/Inn, Zidane (ManA)
                {
                    angle = EventCollision.GetDir((Actor)ctrl);
                    return angle > 159 && angle < 223;
                }
                if (fldMapNo == 2802) // Daguerreo/Left Hall, Zidane (LibrarianA)
                {
                    Obj objUID = PersistenSingleton<EventEngine>.Instance.GetObjByUID(18); // Daguerreo_ElevatorA
                    Single elevatorHeight = 0f;
                    if (PersistenSingleton<EventEngine>.Instance.isPosObj(objUID))
                        elevatorHeight = -((PosObj)objUID).pos[1];
                    angle = EventCollision.GetDir((Actor)ctrl);
                    return angle > 16 && angle < 112 && elevatorHeight > 950f;
                }
                Int32 collAngle = EventCollision.CollisionAngle(ctrl, obj);
                return collAngle > -880 && collAngle < 880;
            }
        }
        return true;
    }

    private static Boolean IsNPCTalkable(Obj npc)
    {
        if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
        {
            Int32 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
            switch (fldMapNo)
            {
                case 656:
                case 657:
                case 658:
                case 659:
                    if (PersistenSingleton<EventEngine>.Instance.isPosObj(npc))
                    {
                        UInt16 model = ((PosObj)npc).model;
                        switch (model)
                        {
                            case 174:
                            case 175:
                            case 176:
                                break;
                            default:
                                if (model != EMinigame.GoldenFrogModelId)
                                    return true;
                                break;
                        }
                        Int32 varManually = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(157157);
                        if (fldMapNo == 657)
                            return varManually > 0 || npc.sid == 4;
                        return varManually > 0;
                    }
                    break;
                case 2950:
                    if (npc.sid == 9)
                    {
                        Int32 fieldEntrance = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR);
                        Int32 varManually3 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(8401);
                        return fieldEntrance != 2 && varManually3 == 1;
                    }
                    break;
                case 1856:
                    if (npc.uid == 4 && Singleton<BubbleUI>.Instance.IsActive && EIcon.SFIconType == BubbleUI.IconType.Exclamation)
                        return false;
                    break;
                case 1608:
                    if (npc.sid == 15)
                    {
                        Int32 scenarioCounter = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
                        return scenarioCounter >= 6850;
                    }
                    break;
                case 1603:
                {
                    Int32 scenarioCounter = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
                    if (npc.uid == 133 && scenarioCounter == 6810)
                        return false;
                    break;
                }
                case 611:
                    if (npc.sid == 7)
                    {
                        Int32 scenarioCounter = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
                        Int32 fieldEntrance = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR);
                        if (scenarioCounter == 3140 && fieldEntrance == 40)
                            return false;
                    }
                    break;
                case 566:
                    if (npc.uid == 10 && Singleton<BubbleUI>.Instance.IsActive && EIcon.SFIconType == BubbleUI.IconType.Exclamation)
                        return false;
                    break;
                case 507:
                    if (npc.sid == 15)
                    {
                        Int32 scenarioCounter = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
                        Int32 fieldEntrance = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR);
                        Obj objUID = PersistenSingleton<EventEngine>.Instance.GetObjByUID(10);
                        if (scenarioCounter == 2915 && fieldEntrance == 3 && objUID != null)
                            return false;
                    }
                    break;
                case 350:
                    if (npc.sid == 34)
                    {
                        Int32 scenarioCounter = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
                        Int32 fieldEntrance = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR);
                        if (scenarioCounter == 2600 && fieldEntrance == 2)
                            return false;
                    }
                    break;
            }
        }
        return true;
    }

    public const Single halfCircleDegree = 180f;
    public const Single fullCircleDegree = 360f;
    public const Int32 kDefaultHeight = 400;
    public const UInt16 kCollCutOff = 2048;
    public const Int32 kCollAngle = 1024;
    public const Int32 kQuadAngle = 880;

    public static Int32 sSysAngle;
}

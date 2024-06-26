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
		{
			result = po.rotAngle;
		}
		else if (instance.gMode == 3)
		{
			result = ((Actor)po).wmActor.rot;
		}
		return result;
	}

	public static void BubbleUIListener(PosObj userObject, Obj collObj, UInt32 key)
	{
		if (userObject != null)
		{
			if (userObject.cid == 4)
			{
				EventCollision.CheckNPCInput(userObject);
			}
			else
			{
				EventCollision.CheckQuadInput(userObject);
			}
		}
	}

	public static Boolean CheckQuadInput(PosObj po)
	{
		EventEngine instance = PersistenSingleton<EventEngine>.Instance;
		UInt32 num = ETb.KeyOn() & (UInt32)((instance.gMode != 1) ? EventInput.Lcircle : (EventInput.Lcircle | 524288u));
		if (num > 0u)
		{
			Obj obj = instance.TreadQuad(po, 4);
			if (obj != null && EventCollision.IsQuadTalkable(po, obj))
			{
				if (num == 524288u && instance.Request(obj, 1, 8, false))
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
		UInt32 num = ETb.KeyOn() & (UInt32)((instance.gMode != 1) ? EventInput.Lcircle : (EventInput.Lcircle | 524288u));
		if (num > 0u)
		{
			Int32 nil = instance.nil;
			Single nilFloat = instance.nilFloat;
			Obj obj = EventCollision.Collision(instance, po, 4, ref nilFloat);
			if (obj != null && EventCollision.IsNPCTalkable(obj))
			{
				EventCollision.sSysAngle = EventCollision.CollisionAngle(po, obj);
				if (EventCollision.sSysAngle > -1024 && EventCollision.sSysAngle < 1024)
				{
					((Actor)po).listener = obj.uid;
					if (num == 524288u)
					{
						Boolean flag = instance.Request(obj, 1, 8, false);
						if (flag)
						{
							EventCollision.ClearPathFinding(po);
							EMinigame.SetQuadmistOpponentId(obj);
							return flag;
						}
					}
					Boolean flag2 = instance.Request(obj, 1, 3, false);
					if (flag2)
					{
						EventCollision.ClearPathFinding(po);
						return flag2;
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
		{
			po.go.GetComponent<FieldMapActorController>().ClearMoveTargetAndPath();
		}
	}

	public static Obj Collision(EventEngine eventEngine, PosObj po, Int32 mode, ref Single distance)
	{
		Obj result = (Obj)null;
		Single num = Single.MaxValue;
		Boolean flag = (mode & 4) > 0;
		Int32 num2 = (Int32)(4 * (Byte)((!flag) ? po.collRad : po.talkRad));
		Vector3 a = Vector3.zero;
		if (eventEngine.gMode != 1)
		{
			if (eventEngine.gMode == 3)
			{
				WMActor wmActor = ((Actor)po).wmActor;
				a = wmActor.RealPosition;
			}
			for (ObjList objList = eventEngine.GetActiveObjList(); objList != null; objList = objList.next)
			{
				Obj obj = objList.obj;
				if (obj.sid != 5 || eventEngine.gMode == 3)
				{
				}
				Byte b = (Byte)((obj.uid != eventEngine.GetControlUID()) ? 4 : 2);
				Boolean flag2 = (po.flags & b) > 0;
				Single num3 = (Single)((!flag && !flag2) ? 0 : 1);
				Byte b2 = (Byte)((!flag) ? ((Byte)((po.uid != eventEngine.GetControlUID()) ? 4 : 2)) : 8);
				Single num4 = (Single)(obj.flags & b2);
				if (obj != po)
				{
					Boolean flag3 = num3 <= 0f;
					Boolean flag4 = num4 <= 0f;
					if (flag3 || flag4)
					{
						flag3 = ((mode & 6) <= 0);
						flag4 = (eventEngine.GetIP((Int32)obj.sid, (Int32)((!flag) ? 2 : 3), obj.ebData) != eventEngine.nil);
						if ((flag3 || flag4) && obj.cid == 4)
						{
							Actor actor = (Actor)obj;
							Single num5 = 0f;
							Int32 num6 = (Int32)(4 * (Byte)((!flag) ? actor.collRad : actor.talkRad));
							PosObj posObj = (PosObj)obj;
							if (posObj.ovalRatio > 0)
							{
								num6 = EventCollision.CalculateRadiusFromOvalRatio(po, posObj, num6);
							}
							num6 += num2;
							if ((mode & 6) != 0)
							{
								num6 += (Int32)(actor.speed + 60);
							}
							if (eventEngine.gMode == 3)
							{
								Single num7 = Vector3.Distance(a, actor.wmActor.RealPosition);
								Single num8 = num7 * 256f;
								num5 = num8;
							}
							if ((Single)num6 > num5 && num > num5)
							{
								result = actor;
								num = num5;
							}
						}
					}
				}
			}
			if (distance > 0f)
			{
				distance = num;
			}
			return result;
		}
		FieldMapActorController component = po.go.GetComponent<FieldMapActorController>();
		if (component == (UnityEngine.Object)null)
		{
			return (Obj)null;
		}
		return component.walkMesh.Collision(component, mode, out distance);
	}

	private static Int32 CalculateRadiusFromOvalRatio(PosObj po, PosObj targetPosObj, Int32 radius)
	{
		Int32 fixedPointAngle = EventCollision.CollisionAngle(targetPosObj, po);
		Int32 num = ff9.rcos(fixedPointAngle);
		Int32 num2 = (num * num >> 4) * (Int32)targetPosObj.ovalRatio + 16777216;
		radius = Convert.ToInt32((Single)radius * ff9.SquareRoot0((Single)num2)) >> 12;
		return radius;
	}

	public static void CollisionRequest(PosObj po)
	{
		Boolean flag = false;
		EventEngine instance = PersistenSingleton<EventEngine>.Instance;
		Int32 nil = instance.nil;
		Single nilFloat = instance.nilFloat;
		Obj obj;
		if (EventCollision.CheckNPCInput(po))
		{
			if (instance.gMode != 3)
			{
				return;
			}
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
					{
						flag = EIcon.PollCollisionIcon(obj);
					}
					if (!flag)
					{
						obj = EventCollision.Collision(instance, po, 2, ref nilFloat);
					}
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
					{
						flag = EIcon.PollCollisionIcon(obj);
					}
				}
			}
		}
		if (obj != null && EventCollision.CheckNPCPush((PosObj)obj))
		{
			instance.Request(obj, 1, 2, false);
		}
		if (EventCollision.CheckQuadInput(po))
		{
			return;
		}
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
						{
							EIcon.PollFIcon(BubbleUI.IconType.Exclamation);
						}
					}
				}
			}
		}
		obj = instance.TreadQuad(po, 4);
		if (obj != null && EventCollision.CheckQuadTalk(po, obj) && EventCollision.IsQuadTalkable(po, obj))
		{
			EIcon.PollCollisionIcon(obj);
		}
		if (instance.gMode == 3 && obj == null)
		{
			if (EventCollision.IsChocoboWalkingOrFlyingInForestArea())
			{
				EIcon.PollFIcon(BubbleUI.IconType.Exclamation);
			}
			else if (!flag && EMinigame.CheckBeachMinigame())
			{
				EIcon.PollFIcon(BubbleUI.IconType.Beach);
			}
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
		Boolean result = true;
		if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
		{
			Int16 fldMapNo = FF9StateSystem.Common.FF9.fldMapNo;
			if (fldMapNo != 2108)
			{
				if (fldMapNo == 2504)
				{
					Byte sid = quad.sid;
					if (sid == 9)
					{
						result = false;
					}
				}
			}
			else
			{
				Byte sid = quad.sid;
				if (sid == 7)
				{
					result = false;
				}
			}
		}
		return result;
	}

	public static Boolean IsWorldTrigger()
	{
		WMActor controlChar = ff9.GetControlChar();
		if (controlChar != (UnityEngine.Object)null)
		{
			ff9.s_moveCHRStatus s_moveCHRStatus = ff9.w_moveCHRStatus[(Int32)controlChar.originalActor.index];
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
			Obj obj = (Obj)null;
			Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
			Int32 uid = (Int32)quad.uid;
			Int32 key = EMinigame.CreateNPCID(fldMapNo, uid);
			if (EventEngineUtils.QuadTalkableData.ContainsKey(key))
			{
				obj = PersistenSingleton<EventEngine>.Instance.GetObjUID(EventEngineUtils.QuadTalkableData[key]);
			}
			if (obj != null)
			{
				Int32 num = fldMapNo;
				Int32 num2;
				if (num == 2108)
				{
					num2 = EventCollision.GetDir((Actor)ctrl);
					return num2 > 90 && num2 < 160;
				}
				if (num == 2109)
				{
					num2 = EventCollision.GetDir((Actor)ctrl);
					return num2 > 159 && num2 < 223;
				}
				if (num == 2103)
				{
					num2 = EventCollision.GetDir((Actor)ctrl);
					return num2 > 159 && num2 < 223;
				}
				if (num != 2802)
				{
					num2 = EventCollision.CollisionAngle(ctrl, obj);
					return num2 > -880 && num2 < 880;
				}
				Obj objUID = PersistenSingleton<EventEngine>.Instance.GetObjUID(18);
				Single num3 = 0f;
				if (PersistenSingleton<EventEngine>.Instance.isPosObj(objUID))
				{
					num3 = -((PosObj)objUID).pos[1];
				}
				num2 = EventCollision.GetDir((Actor)ctrl);
				return num2 > 16 && num2 < 112 && num3 > 950f;
			}
		}
		return true;
	}

	private static Boolean IsNPCTalkable(Obj npc)
	{
		Boolean flag = true;
		if (PersistenSingleton<EventEngine>.Instance.gMode == 1)
		{
			Int32 fldMapNo = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
			Int32 num = fldMapNo;
			switch (num)
			{
			case 656:
			case 657:
			case 658:
			case 659:
				if (PersistenSingleton<EventEngine>.Instance.isPosObj(npc))
				{
					PosObj posObj = (PosObj)npc;
					UInt16 model = posObj.model;
					switch (model)
					{
					case 174:
					case 175:
					case 176:
						break;
					default:
						if (model != EMinigame.GoldenFrogModelId)
						{
							goto IL_191;
						}
						break;
					}
					Int32 varManually = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(157157);
					flag = (varManually > 0);
					if (fldMapNo == 657)
					{
						flag = (flag || npc.sid == 4);
					}
					return flag;
				}
				IL_191:
				break;
			default:
				if (num != 350)
				{
					if (num != 507)
					{
						if (num != 566)
						{
							if (num != 611)
							{
								if (num != 1603)
								{
									if (num != 1608)
									{
										if (num != 1856)
										{
											if (num == 2950)
											{
												if (npc.sid == 9)
												{
													Int32 varManually2 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR);
													Int32 varManually3 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(8401);
													return varManually2 != 2 && varManually3 == 1;
												}
											}
										}
										else if (npc.uid == 4 && Singleton<BubbleUI>.Instance.IsActive && EIcon.SFIconType == BubbleUI.IconType.Exclamation)
										{
											flag = false;
										}
									}
									else if (npc.sid == 15)
									{
										Int32 varManually4 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
										return 6850 <= varManually4;
									}
								}
								else
								{
									Int32 varManually5 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
									if (npc.uid == 133 && varManually5 == 6810)
									{
										flag = false;
									}
								}
							}
							else if (npc.sid == 7)
							{
								Int32 varManually5 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
								Int32 varManually2 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR);
								if (varManually5 == 3140 && varManually2 == 40)
								{
									flag = false;
								}
							}
						}
						else if (npc.uid == 10 && Singleton<BubbleUI>.Instance.IsActive && EIcon.SFIconType == BubbleUI.IconType.Exclamation)
						{
							flag = false;
						}
					}
					else if (npc.sid == 15)
					{
						Int32 varManually5 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
						Int32 varManually2 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR);
						Obj objUID = PersistenSingleton<EventEngine>.Instance.GetObjUID(10);
						if (varManually5 == 2915 && varManually2 == 3 && objUID != null)
						{
							flag = false;
						}
					}
				}
				else if (npc.sid == 34)
				{
					Int32 varManually5 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR);
					Int32 varManually2 = PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.MAP_INDEX_SVR);
					if (varManually5 == 2600 && varManually2 == 2)
					{
						flag = false;
					}
				}
				break;
			}
		}
		return flag;
	}

	public const Single halfCircleDegree = 180f;

	public const Single fullCircleDegree = 360f;

	public const Int32 kDefaultHeight = 400;

	public const UInt16 kCollCutOff = 2048;

	public const Int32 kCollAngle = 1024;

	public const Int32 kQuadAngle = 880;

	public static Int32 sSysAngle;
}

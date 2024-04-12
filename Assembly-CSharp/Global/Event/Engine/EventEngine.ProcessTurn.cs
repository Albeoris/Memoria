using System;
using UnityEngine;

public partial class EventEngine
{
	private void ProcessTurn(Actor actor)
	{
		if (((Int32)actor.flags & 128) == 0)
			return;
		Single num = actor.rotAngle.y;
		if ((Double)num > 180.0)
			num -= 360f;
		else if ((Double)num < -180.0)
			num += 360f;
		if ((Double)actor.turnAdd == 32766.0)
		{
			Single a = num + actor.trotAdd;
			if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2855 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 9370 && (Int32)actor.model == 273)
			{
				if ((Double)a > 180.0)
					a -= 360f;
				else if ((Double)a < -180.0)
					a += 360f;
			}
			if ((Double)a < (Double)actor.turnRot || EventEngineUtils.nearlyEqual(a, actor.turnRot))
			{
				a = actor.turnRot;
				actor.flags = (Byte)((UInt32)actor.flags & 4294967167U);
			}
			actor.rotAngle[1] = a;
		}
		else if ((Double)actor.turnAdd == (Double)Int16.MaxValue)
		{
			Single a = num + actor.trotAdd;
			if ((Int32)FF9StateSystem.Common.FF9.fldMapNo == 2855 && PersistenSingleton<EventEngine>.Instance.eBin.getVarManually(EBin.SC_COUNTER_SVR) == 9370 && (Int32)actor.model == 273)
			{
				if ((Double)a > 180.0)
					a -= 360f;
				else if ((Double)a < -180.0)
					a += 360f;
			}
			if ((Double)a > (Double)actor.turnRot || EventEngineUtils.nearlyEqual(a, actor.turnRot))
			{
				a = actor.turnRot;
				actor.flags = (Byte)((UInt32)actor.flags & 4294967167U);
			}
			actor.rotAngle[1] = a;
		}
		Vector3 eulerAngles = actor.go.transform.localRotation.eulerAngles;
	}

	private void DoTurn(Actor actor)
	{
		Vector3 vector3 = actor.rotAngle;
		vector3.y += actor.turnAdd;
		actor.rotAngle[1] += actor.turnAdd;
		actor.trot += actor.trotAdd;
	}

	private void FinishTurn(Actor actor)
	{
		Vector3 vector3 = actor.rotAngle;
		actor.go.GetComponent<FieldMapActorController>();
		vector3.y = actor.turnRot;
		actor.rotAngle[1] = actor.turnRot;
		this.SetAnim(actor, (Int32)actor.lastAnim);
		actor.flags = (Byte)((UInt32)actor.flags & 4294967167U);
	}
}

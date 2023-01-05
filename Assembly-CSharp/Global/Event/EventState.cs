using System;
using System.Collections.Generic;
using UnityEngine;
using Memoria.Data;

public class EventState : MonoBehaviour
{
	public Int32 gStepCount;

	public Byte[] gEventGlobal = new Byte[2048];

	// Custom usage of "gEventGlobal":
	// The interest is that this array is saved in Game Saves (no need to touch the save format) and contains a lot of unused ranges
	// For instance, the bytes in the range 1100 to 2047 is completly unused by default
	public Int32 GetAAUsageCounter(BattleAbilityId abilityId)
	{
		Int32 abilId = (Int32)abilityId;
		return gEventGlobal[1100 + abilId];
	}

	public void IncreaseAAUsageCounter(BattleAbilityId abilityId)
	{
		Int32 abilId = (Int32)abilityId;
		if (gEventGlobal[1100 + abilId] < 255)
			++gEventGlobal[1100 + abilId];
	}

	public List<Int32> FindVariableInFieldScriptUsage(List<Int32> variableIndex, List<Boolean> asBool = null)
	{
		List<Int32> fieldList = new List<Int32>();
		foreach (KeyValuePair<Int32, String> pair in FF9DBAll.EventDB)
			if (EventEngineUtils.eventIDToFBGID.ContainsKey(pair.Key))
			{
				EventEngineUtils.BinaryScript script = EventEngineUtils.loadEventAsScript(pair.Value, EventEngineUtils.ebSubFolderField);
				HashSet<UInt32> varUsed = script.GetVariableUsage(true, false, false);
				for (Int32 i = 0; i < variableIndex.Count; i++)
					if (EventEngineUtils.BinaryScript.IsVariableInUsage(varUsed, EventEngineUtils.BinaryScript.GetVariableFromIndex(EBin.VariableSource.Global, (UInt16)variableIndex[i], asBool != null ? asBool[i] : false)))
					{
						fieldList.Add(pair.Key);
						break;
					}
			}
		return fieldList;
	}
}

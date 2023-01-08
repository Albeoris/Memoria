using System;
using System.Collections.Generic;
using UnityEngine;
using Memoria.Data;

public class EventState : MonoBehaviour
{
	public Int32 gStepCount;

	public Byte[] gEventGlobal = new Byte[2048];

	public Dictionary<BattleAbilityId, Int32> gAbilityUsage = new Dictionary<BattleAbilityId, Int32>();

	public Int32 GetAAUsageCounter(BattleAbilityId abilityId)
	{
		if (gAbilityUsage.TryGetValue(abilityId, out Int32 result))
			return result;
		return 0;
	}

	public void IncreaseAAUsageCounter(BattleAbilityId abilityId)
	{
		if (gAbilityUsage.ContainsKey(abilityId))
			++gAbilityUsage[abilityId];
		else
			gAbilityUsage[abilityId] = 1;
	}

	public List<Int32> FindVariableInFieldScriptUsage(List<Int32> variableIndex, List<Boolean> asBool = null)
	{
		List<Int32> fieldList = new List<Int32>();
		foreach (KeyValuePair<Int32, String> pair in FF9DBAll.EventDB)
		{
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
		}
		return fieldList;
	}
}

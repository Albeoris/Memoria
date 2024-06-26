using Memoria.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EventState : MonoBehaviour
{
    public Int32 gStepCount;

    public Byte[] gEventGlobal = new Byte[2048];

    public Dictionary<BattleAbilityId, Int32> gAbilityUsage = new Dictionary<BattleAbilityId, Int32>();
    public Dictionary<Int32, List<Int32>> gScriptVector = new Dictionary<Int32, List<Int32>>();
    public Dictionary<Int32, Dictionary<Int32, Int32>> gScriptDictionary = new Dictionary<Int32, Dictionary<Int32, Int32>>();

    public Int32 ScenarioCounter
    {
        get => gEventGlobal[1] << 8 | gEventGlobal[0];
        set
        {
            gEventGlobal[0] = (Byte)(value & 0xFF);
            gEventGlobal[1] = (Byte)(value >> 8 & 0xFF);
        }
    }

    public Int32 FieldEntrance
    {
        get => gEventGlobal[3] << 8 | gEventGlobal[2];
        set
        {
            gEventGlobal[2] = (Byte)(value & 0xFF);
            gEventGlobal[3] = (Byte)(value >> 8 & 0xFF);
        }
    }

    public Boolean IsEikoAbducted => ScenarioCounter >= 9860 && ScenarioCounter < 9990;

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

    public Int32 GetTreasureHunterPoints()
    {
        // Rank H:   0-199 pts
        // Rank G: 200-219 pts
        // Rank F: 220-239 pts
        // Rank E: 240-259 pts
        // Rank D: 260-279 pts
        // Rank C: 280-299 pts
        // Rank B: 300-319 pts
        // Rank A: 320-339 pts
        // Rank S: 340+ pts
        Int32 pts = 0;
        for (Int32 index = 896; index <= 960; index++)
            pts += FF9.Comn.countBits(gEventGlobal[index]);
        for (Int32 index = 966; index <= 975; index++)
            pts += FF9.Comn.countBits(gEventGlobal[index]);
        for (Int32 index = 182; index <= 186; index++)
            pts += 2 * FF9.Comn.countBits(gEventGlobal[index]);
        return pts;
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

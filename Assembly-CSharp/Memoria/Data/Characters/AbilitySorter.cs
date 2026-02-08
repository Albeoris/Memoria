using System;
using System.Collections.Generic;
using Memoria.Data;
using SimpleJSON;
using UnityEngine;

public static class AbilitySorter
{
    private static Dictionary<CharacterId, List<int>> _sortedMenuAA = new Dictionary<CharacterId, List<int>>();
    private static Dictionary<CharacterId, List<int>> _sortedMenuSA = new Dictionary<CharacterId, List<int>>();
    private static Dictionary<CharacterId, Dictionary<BattleCommandId, List<int>>> _sortedBattleAA = new Dictionary<CharacterId, Dictionary<BattleCommandId, List<int>>>();

    public static void SortMenuAA(CharacterId charId, List<int> list)
    {
        SortList(charId, list, _sortedMenuAA);
    }

    public static void SortMenuSA(CharacterId charId, List<int> list)
    {
        SortList(charId, list, _sortedMenuSA);
    }

    public static void UpdateMenuAA(CharacterId charId, List<int> list)
    {
        _sortedMenuAA[charId] = new List<int>(list);
    }

    public static void UpdateMenuSA(CharacterId charId, List<int> list)
    {
        _sortedMenuSA[charId] = new List<int>(list);
    }

    public static void SortBattle(CharacterId charId, BattleCommandId cmdId, List<int> list)
    {
        if (!_sortedBattleAA.ContainsKey(charId) || !_sortedBattleAA[charId].ContainsKey(cmdId))
            return;

        List<int> savedOrder = _sortedBattleAA[charId][cmdId];
        list.Sort((a, b) =>
        {
            int indexA = savedOrder.IndexOf(a);
            int indexB = savedOrder.IndexOf(b);
            if (indexA == -1 && indexB == -1) return 0;
            if (indexA == -1) return 1;
            if (indexB == -1) return -1;
            return indexA.CompareTo(indexB);
        });
    }

    public static void UpdateBattleOrder(CharacterId charId, BattleCommandId cmdId, List<int> list)
    {
        if (!_sortedBattleAA.ContainsKey(charId))
            _sortedBattleAA[charId] = new Dictionary<BattleCommandId, List<int>>();

        _sortedBattleAA[charId][cmdId] = new List<int>(list);
    }

    private static void SortList(CharacterId charId, List<int> list, Dictionary<CharacterId, List<int>> dict)
    {
        if (!dict.ContainsKey(charId)) return;
        List<int> savedOrder = dict[charId];
        list.Sort((a, b) =>
        {
            int indexA = savedOrder.IndexOf(a);
            int indexB = savedOrder.IndexOf(b);
            if (indexA == -1 && indexB == -1) return 0;
            if (indexA == -1) return 1;
            if (indexB == -1) return -1;
            return indexA.CompareTo(indexB);
        });
    }
    public static void WriteToJSON(JSONClass root)
    {
        JSONClass sortNode = new JSONClass();

        JSONClass menuAaNode = new JSONClass();
        foreach (var kvp in _sortedMenuAA)
        {
            JSONArray arr = new JSONArray();
            foreach (int id in kvp.Value) arr.Add(id.ToString());
            menuAaNode.Add(((int)kvp.Key).ToString(), arr);
        }
        sortNode.Add("MenuAA", menuAaNode);

        JSONClass menuSaNode = new JSONClass();
        foreach (var kvp in _sortedMenuSA)
        {
            JSONArray arr = new JSONArray();
            foreach (int id in kvp.Value) arr.Add(id.ToString());
            menuSaNode.Add(((int)kvp.Key).ToString(), arr);
        }
        sortNode.Add("MenuSA", menuSaNode);

        JSONClass battleNode = new JSONClass();
        foreach (var charKvp in _sortedBattleAA)
        {
            JSONClass cmdNode = new JSONClass();
            foreach (var cmdKvp in charKvp.Value)
            {
                JSONArray arr = new JSONArray();
                foreach (int id in cmdKvp.Value) arr.Add(id.ToString());
                cmdNode.Add(((int)cmdKvp.Key).ToString(), arr);
            }
            battleNode.Add(((int)charKvp.Key).ToString(), cmdNode);
        }
        sortNode.Add("BattleAA", battleNode);

        root.Add("AbilitySortOrder", sortNode);
    }

    public static void ReadFromJSON(JSONClass root)
    {
        _sortedMenuAA.Clear();
        _sortedMenuSA.Clear();
        _sortedBattleAA.Clear();

        if (root["AbilitySortOrder"] == null) return;
        JSONNode sortNode = root["AbilitySortOrder"];

        if (sortNode["MenuAA"] != null)
        {
            foreach (KeyValuePair<string, JSONNode> item in sortNode["MenuAA"].AsObject)
                if (int.TryParse(item.Key, out int id))
                    _sortedMenuAA[(CharacterId)id] = ParseIntList(item.Value.AsArray);
        }

        if (sortNode["MenuSA"] != null)
        {
            foreach (KeyValuePair<string, JSONNode> item in sortNode["MenuSA"].AsObject)
                if (int.TryParse(item.Key, out int id))
                    _sortedMenuSA[(CharacterId)id] = ParseIntList(item.Value.AsArray);
        }

        if (sortNode["BattleAA"] != null)
        {
            foreach (KeyValuePair<string, JSONNode> charItem in sortNode["BattleAA"].AsObject)
            {
                if (int.TryParse(charItem.Key, out int charId))
                {
                    CharacterId cId = (CharacterId)charId;
                    Dictionary<BattleCommandId, List<int>> cmdDict = new Dictionary<BattleCommandId, List<int>>();

                    foreach (KeyValuePair<string, JSONNode> cmdItem in charItem.Value.AsObject)
                    {
                        if (int.TryParse(cmdItem.Key, out int cmdId))
                            cmdDict[(BattleCommandId)cmdId] = ParseIntList(cmdItem.Value.AsArray);
                    }
                    _sortedBattleAA[cId] = cmdDict;
                }
            }
        }
    }

    private static List<int> ParseIntList(JSONArray arr)
    {
        List<int> list = new List<int>();
        foreach (JSONNode node in arr) list.Add(node.AsInt);
        return list;
    }
}

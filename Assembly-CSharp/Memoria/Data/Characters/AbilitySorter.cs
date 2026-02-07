using System;
using System.Collections.Generic;
using Memoria.Data;
using SimpleJSON;
using UnityEngine;

public static class AbilitySorter
{
    private static Dictionary<CharacterId, List<int>> _sortedAA = new Dictionary<CharacterId, List<int>>();
    private static Dictionary<CharacterId, List<int>> _sortedSA = new Dictionary<CharacterId, List<int>>();

    public static void Sort(CharacterId charId, List<int> listToSort, bool isSA)
    {
        var dict = isSA ? _sortedSA : _sortedAA;

        if (!dict.ContainsKey(charId))
            return;

        List<int> savedOrder = dict[charId];

        listToSort.Sort((a, b) =>
        {
            int indexA = savedOrder.IndexOf(a);
            int indexB = savedOrder.IndexOf(b);

            if (indexA == -1 && indexB == -1) return 0;
            if (indexA == -1) return 1;
            if (indexB == -1) return -1;

            return indexA.CompareTo(indexB);
        });
    }

    public static void UpdateOrder(CharacterId charId, List<int> newList, bool isSA)
    {
        var dict = isSA ? _sortedSA : _sortedAA;
        dict[charId] = new List<int>(newList);
    }

    public static void WriteToJSON(JSONClass root)
    {
        JSONClass sortNode = new JSONClass();

        JSONClass aaNode = new JSONClass();
        foreach (KeyValuePair<CharacterId, List<int>> kvp in _sortedAA)
        {
            JSONArray arr = new JSONArray();
            foreach (int id in kvp.Value) arr.Add(id.ToString());

            aaNode.Add(((int)kvp.Key).ToString(), arr);
        }
        sortNode.Add("AA", aaNode);

        JSONClass saNode = new JSONClass();
        foreach (KeyValuePair<CharacterId, List<int>> kvp in _sortedSA)
        {
            JSONArray arr = new JSONArray();
            foreach (int id in kvp.Value) arr.Add(id.ToString());

            saNode.Add(((int)kvp.Key).ToString(), arr);
        }
        sortNode.Add("SA", saNode);

        root.Add("AbilitySortOrder", sortNode);
    }

    public static void ReadFromJSON(JSONClass root)
    {
        _sortedAA.Clear();
        _sortedSA.Clear();

        if (root["AbilitySortOrder"] == null) return;

        JSONNode sortNode = root["AbilitySortOrder"];

        if (sortNode["AA"] != null)
        {
            foreach (KeyValuePair<string, JSONNode> item in sortNode["AA"].AsObject)
            {
                if (int.TryParse(item.Key, out int idInt))
                {
                    CharacterId charId = (CharacterId)idInt;
                    List<int> list = new List<int>();
                    foreach (JSONNode val in item.Value.AsArray) list.Add(val.AsInt);
                    _sortedAA[charId] = list;
                }
            }
        }

        if (sortNode["SA"] != null)
        {
            foreach (KeyValuePair<string, JSONNode> item in sortNode["SA"].AsObject)
            {
                if (int.TryParse(item.Key, out int idInt))
                {
                    CharacterId charId = (CharacterId)idInt;
                    List<int> list = new List<int>();
                    foreach (JSONNode val in item.Value.AsArray) list.Add(val.AsInt);
                    _sortedSA[charId] = list;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.Collections;
using Memoria.Prime.CSV;

public static partial class FF9BattleDB
{
    public static readonly Dictionary<BattleStatusIndex, BattleStatusEntry> StatusSets;
    public static readonly Dictionary<BattleAbilityId, AA_DATA> CharacterActions;
    public static readonly EntryCollection<STAT_DATA> StatusData;

    static FF9BattleDB()
	{
	    StatusSets = LoadStatusSets();
	    CharacterActions = LoadActions();
        StatusData = LoadStatusData();
    }

    private static Dictionary<BattleStatusIndex, BattleStatusEntry> LoadStatusSets()
    {
        try
        {
            Dictionary<BattleStatusIndex, BattleStatusEntry> result = new Dictionary<BattleStatusIndex, BattleStatusEntry>();
            BattleStatusEntry[] statusSets;
            String inputPath;
            String[] dir = Configuration.Mod.AllFolderNames;
            for (Int32 i = dir.Length - 1; i >= 0; --i)
            {
                inputPath = DataResources.Battle.ModDirectory(dir[i]) + DataResources.Battle.StatusSetsFile;
                if (File.Exists(inputPath))
                {
                    statusSets = CsvReader.Read<BattleStatusEntry>(inputPath);
                    for (Int32 j = 0; j < statusSets.Length; j++)
                        result[statusSets[j].Id] = statusSets[j];
                }
            }
            if (result.Count == 0)
                throw new FileNotFoundException($"Cannot load status sets because a file does not exist: [{DataResources.Battle.Directory + DataResources.Battle.StatusSetsFile}].", DataResources.Battle.Directory + DataResources.Battle.StatusSetsFile);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[FF9BattleDB] Load status sets failed.");
            UIManager.Input.ConfirmQuit();
            return null;
        }
    }

    private static Dictionary<BattleAbilityId, AA_DATA> LoadActions()
    {
        try
        {
            Dictionary<BattleAbilityId, AA_DATA> result = new Dictionary<BattleAbilityId, AA_DATA>();
            BattleActionEntry[] actions;
            String inputPath;
            String[] dir = Configuration.Mod.AllFolderNames;
            for (Int32 i = dir.Length - 1; i >= 0; --i)
            {
                inputPath = DataResources.Battle.ModDirectory(dir[i]) + DataResources.Battle.ActionsFile;
                if (File.Exists(inputPath))
                {
                    actions = CsvReader.Read<BattleActionEntry>(inputPath);
                    for (Int32 j = 0; j < actions.Length; j++)
                        result[actions[j].Id] = actions[j].ActionData;
                }
            }
            if (result.Count == 0)
                throw new FileNotFoundException($"Cannot load actions because a file does not exist: [{DataResources.Battle.Directory + DataResources.Battle.ActionsFile}].", DataResources.Battle.Directory + DataResources.Battle.ActionsFile);
            for (Int32 j = 0; j < 192; j++)
                if (!result.ContainsKey((BattleAbilityId)j))
                    throw new NotSupportedException($"You must define at least the 192 actions, with IDs between 0 and 191.");
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[FF9BattleDB] Load actions failed.");
            UIManager.Input.ConfirmQuit();
            return null;
        }
    }

    private static EntryCollection<STAT_DATA> LoadStatusData()
    {
        try
        {
            String inputPath = DataResources.Battle.Directory + DataResources.Battle.StatusDataFile;
            if (!File.Exists(inputPath))
                throw new FileNotFoundException($"File with character actions not found: [{inputPath}]");

            BattleStatusDataEntry[] statusData = CsvReader.Read<BattleStatusDataEntry>(inputPath);
            if (statusData.Length < BattleStatusDataEntry.StatusCount)
                throw new NotSupportedException($"You must set {BattleStatusDataEntry.StatusCount} status sets, but there {statusData.Length}.");

            EntryCollection<STAT_DATA> result = EntryCollection.CreateWithDefaultElement(statusData, e => e.Id, e => e.Value);
            for (Int32 i = Configuration.Mod.FolderNames.Length - 1; i >= 0; i--)
            {
                inputPath = DataResources.Battle.ModDirectory(Configuration.Mod.FolderNames[i]) + DataResources.Battle.StatusDataFile;
                if (File.Exists(inputPath))
                {
                    statusData = CsvReader.Read<BattleStatusDataEntry>(inputPath);
                    foreach (BattleStatusDataEntry it in statusData)
                        result[it.Id] = it.Value;
                }
            }
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[FF9BattleDB] Load base stats of characters failed.");
            UIManager.Input.ConfirmQuit();
            return null;
        }
    }
}

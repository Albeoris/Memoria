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
    public static readonly Dictionary<Int32, STAT_DATA> StatusData;

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
            String inputPath = DataResources.Battle.PureDirectory + DataResources.Battle.StatusSetsFile;
            Dictionary<BattleStatusIndex, BattleStatusEntry> result = new Dictionary<BattleStatusIndex, BattleStatusEntry>();
            foreach (BattleStatusEntry[] statusSets in AssetManager.EnumerateCsvFromLowToHigh<BattleStatusEntry>(inputPath))
                foreach (BattleStatusEntry set in statusSets)
                    result[set.Id] = set;
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
            String inputPath = DataResources.Battle.PureDirectory + DataResources.Battle.ActionsFile;
            Dictionary<BattleAbilityId, AA_DATA> result = new Dictionary<BattleAbilityId, AA_DATA>();
            foreach (BattleActionEntry[] actions in AssetManager.EnumerateCsvFromLowToHigh<BattleActionEntry>(inputPath))
                foreach (BattleActionEntry action in actions)
                    result[action.Id] = action.ActionData;
            if (result.Count == 0)
                throw new FileNotFoundException($"Cannot load actions because a file does not exist: [{DataResources.Battle.Directory + DataResources.Battle.ActionsFile}].", DataResources.Battle.Directory + DataResources.Battle.ActionsFile);
            for (Int32 i = 0; i < 192; i++)
                if (!result.ContainsKey((BattleAbilityId)i))
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

    private static Dictionary<Int32, STAT_DATA> LoadStatusData()
    {
        try
        {
            String inputPath = DataResources.Battle.PureDirectory + DataResources.Battle.StatusDataFile;
            Dictionary<Int32, STAT_DATA> result = new Dictionary<Int32, STAT_DATA>();
            foreach (BattleStatusDataEntry[] statusData in AssetManager.EnumerateCsvFromLowToHigh<BattleStatusDataEntry>(inputPath))
                foreach (BattleStatusDataEntry it in statusData)
                    result[it.Id] = it.Value;
            inputPath = DataResources.Battle.Directory + DataResources.Battle.StatusDataFile;
            if (result.Count == 0)
                throw new FileNotFoundException($"File with status datas not found: [{inputPath}]");
            for (Int32 i = 0; i < 32; i++)
                if (!result.ContainsKey(i))
                    throw new NotSupportedException($"You must define at least 32 status datas, with IDs between 0 and 31");
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

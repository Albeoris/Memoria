using FF9;
using Memoria;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.Collections;
using Memoria.Prime.CSV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static partial class FF9BattleDB
{
    public static readonly Dictionary<StatusSetId, BattleStatusEntry> StatusSets;
    public static readonly Dictionary<BattleAbilityId, AA_DATA> CharacterActions;
    public static readonly Dictionary<BattleStatusId, BattleStatusDataEntry> StatusData;
    public static readonly Dictionary<Int32, BattleMagicSwordSet> MagicSwordData;

    static FF9BattleDB()
    {
        StatusSets = LoadStatusSets();
        CharacterActions = LoadActions();
        StatusData = LoadStatusData();
        MagicSwordData = LoadMagicSwordSets();
    }

    private static Dictionary<StatusSetId, BattleStatusEntry> LoadStatusSets()
    {
        try
        {
            String inputPath = DataResources.Battle.PureDirectory + DataResources.Battle.StatusSetsFile;
            Dictionary<StatusSetId, BattleStatusEntry> result = new Dictionary<StatusSetId, BattleStatusEntry>();
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

    private static Dictionary<BattleStatusId, BattleStatusDataEntry> LoadStatusData()
    {
        try
        {
            String inputPath = DataResources.Battle.PureDirectory + DataResources.Battle.StatusDataFile;
            Dictionary<BattleStatusId, BattleStatusDataEntry> result = new Dictionary<BattleStatusId, BattleStatusDataEntry>();
            foreach (BattleStatusDataEntry[] statusData in AssetManager.EnumerateCsvFromLowToHigh<BattleStatusDataEntry>(inputPath))
                foreach (BattleStatusDataEntry it in statusData)
                    result[it.Id] = it;
            inputPath = DataResources.Battle.Directory + DataResources.Battle.StatusDataFile;
            if (result.Count == 0)
                throw new FileNotFoundException($"File with status datas not found: [{inputPath}]");
            for (Int32 i = 0; i < 33; i++)
                if (!result.ContainsKey((BattleStatusId)i))
                    throw new NotSupportedException($"You must define at least 33 status datas, with IDs between 0 and 32");
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[FF9BattleDB] Load base stats of characters failed.");
            UIManager.Input.ConfirmQuit();
            return null;
        }
    }

    private static Dictionary<Int32, BattleMagicSwordSet> LoadMagicSwordSets()
    {
        try
        {
            String inputPath = DataResources.Battle.PureDirectory + DataResources.Battle.MagicSwordSetFile;
            Dictionary<Int32, BattleMagicSwordSet> result = new Dictionary<Int32, BattleMagicSwordSet>();
            foreach (BattleMagicSwordSet[] magicSet in AssetManager.EnumerateCsvFromLowToHigh<BattleMagicSwordSet>(inputPath))
                foreach (BattleMagicSwordSet set in magicSet)
                    result[set.Id] = set;
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[FF9BattleDB] Load magic sword sets failed.");
            return new Dictionary<Int32, BattleMagicSwordSet>();
        }
    }
}

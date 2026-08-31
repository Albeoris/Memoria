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
    public static Dictionary<StatusSetId, BattleStatusEntry> StatusSets;
    public static Dictionary<BattleAbilityId, AA_DATA> CharacterActions;
    public static Dictionary<BattleStatusId, BattleStatusDataEntry> StatusData;
    public static Dictionary<Int32, BattleMagicSwordSet> MagicSwordData;
    public static readonly BattleStatus AllStatuses = 0;

    static FF9BattleDB()
    {
        LoadStatusSets();
        LoadActions();
        LoadStatusData();
        LoadMagicSwordSets();
        foreach (BattleStatusId statusId in StatusData.Keys)
            AllStatuses |= statusId.ToBattleStatus();
    }

    private static void LoadStatusSets()
    {
        try
        {
            StatusSets = new Dictionary<StatusSetId, BattleStatusEntry>();
            String inputPath = DataResources.Battle.PureDirectory + DataResources.Battle.StatusSetsFile;
            foreach (BattleStatusEntry[] statusSets in AssetManager.EnumerateCsvFromLowToHigh<BattleStatusEntry>(inputPath))
                foreach (BattleStatusEntry set in statusSets)
                    StatusSets[set.Id] = set;
            if (StatusSets.Count == 0)
                throw new FileNotFoundException($"Cannot load status sets because a file does not exist: [{DataResources.Battle.Directory + DataResources.Battle.StatusSetsFile}].", DataResources.Battle.Directory + DataResources.Battle.StatusSetsFile);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[FF9BattleDB] Load status sets failed.");
            UIManager.Input.ConfirmQuit();
        }
    }

    private static void LoadActions()
    {
        try
        {
            CharacterActions = new Dictionary<BattleAbilityId, AA_DATA>();
            String inputPath = DataResources.Battle.PureDirectory + DataResources.Battle.ActionsFile;
            foreach (BattleActionEntry[] actions in AssetManager.EnumerateCsvFromLowToHigh<BattleActionEntry>(inputPath))
                foreach (BattleActionEntry action in actions)
                    CharacterActions[action.Id] = action.ActionData;
            if (CharacterActions.Count == 0)
                throw new FileNotFoundException($"Cannot load actions because a file does not exist: [{DataResources.Battle.Directory + DataResources.Battle.ActionsFile}].", DataResources.Battle.Directory + DataResources.Battle.ActionsFile);
            for (Int32 i = 0; i < 192; i++)
                if (!CharacterActions.ContainsKey((BattleAbilityId)i))
                    throw new NotSupportedException($"You must define at least the 192 actions, with IDs between 0 and 191.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[FF9BattleDB] Load actions failed.");
            UIManager.Input.ConfirmQuit();
        }
    }

    private static void LoadStatusData()
    {
        try
        {
            StatusData = new Dictionary<BattleStatusId, BattleStatusDataEntry>();
            String inputPath = DataResources.Battle.PureDirectory + DataResources.Battle.StatusDataFile;
            foreach (BattleStatusDataEntry[] statusData in AssetManager.EnumerateCsvFromLowToHigh<BattleStatusDataEntry>(inputPath))
                foreach (BattleStatusDataEntry it in statusData)
                    StatusData[it.Id] = it.Data;
            inputPath = DataResources.Battle.Directory + DataResources.Battle.StatusDataFile;
            if (StatusData.Count == 0)
                throw new FileNotFoundException($"File with status datas not found: [{inputPath}]");
            for (Int32 i = 0; i < 33; i++)
                if (!StatusData.ContainsKey((BattleStatusId)i))
                    throw new NotSupportedException($"You must define at least 33 status datas, with IDs between 0 and 32");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[FF9BattleDB] Load base stats of characters failed.");
            UIManager.Input.ConfirmQuit();
        }
    }

    private static void LoadMagicSwordSets()
    {
        try
        {
            MagicSwordData = new Dictionary<Int32, BattleMagicSwordSet>();
            String inputPath = DataResources.Battle.PureDirectory + DataResources.Battle.MagicSwordSetFile;
            foreach (BattleMagicSwordSet[] magicSet in AssetManager.EnumerateCsvFromLowToHigh<BattleMagicSwordSet>(inputPath))
                foreach (BattleMagicSwordSet set in magicSet)
                    MagicSwordData[set.Id] = set.Data;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[FF9BattleDB] Load magic sword sets failed.");
        }
    }
}

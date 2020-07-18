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
    public static readonly EntryCollection<BattleStatusEntry> StatusSets;
    public static readonly EntryCollection<AA_DATA> CharacterActions;
    public static readonly EntryCollection<STAT_DATA> StatusData;

    static FF9BattleDB()
	{
	    StatusSets = LoadStatusSets();
	    CharacterActions = LoadActions();
        StatusData = LoadStatusData();
    }

    private static EntryCollection<BattleStatusEntry> LoadStatusSets()
    {
        try
        {
            String inputPath = DataResources.Battle.Directory + DataResources.Battle.StatusSetsFile;
            if (!File.Exists(inputPath))
                throw new FileNotFoundException($"File with status sets not found: [{inputPath}]");

            BattleStatusEntry[] statusSets = CsvReader.Read<BattleStatusEntry>(inputPath);
            if (statusSets.Length < BattleStatusEntry.SetsCount)
                throw new NotSupportedException($"You must set {BattleStatusEntry.SetsCount} status sets, but there {statusSets.Length}.");

            EntryCollection<BattleStatusEntry> result = EntryCollection.CreateWithDefaultElement(statusSets, e => e.Id);
            for (Int32 i = Configuration.Mod.FolderNames.Length - 1; i >= 0; i--)
            {
                inputPath = DataResources.Battle.ModDirectory(Configuration.Mod.FolderNames[i]) + DataResources.Battle.StatusSetsFile;
                if (File.Exists(inputPath))
                {
                    statusSets = CsvReader.Read<BattleStatusEntry>(inputPath);
                    foreach (BattleStatusEntry it in statusSets)
                        result[it.Id] = it;
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

    private static EntryCollection<AA_DATA> LoadActions()
    {
        try
        {
            String inputPath = DataResources.Battle.Directory + DataResources.Battle.ActionsFile;
            if (!File.Exists(inputPath))
                throw new FileNotFoundException($"File with character actions not found: [{inputPath}]");

            BattleActionEntry[] statusSets = CsvReader.Read<BattleActionEntry>(inputPath);
            if (statusSets.Length < BattleStatusEntry.SetsCount)
                throw new NotSupportedException($"You must set {BattleStatusEntry.SetsCount} status sets, but there {statusSets.Length}.");

            EntryCollection<AA_DATA> result = EntryCollection.CreateWithDefaultElement(statusSets, e => e.Id, e => e.ActionData);
            for (Int32 i = Configuration.Mod.FolderNames.Length - 1; i >= 0; i--)
            {
                inputPath = DataResources.Battle.ModDirectory(Configuration.Mod.FolderNames[i]) + DataResources.Battle.ActionsFile;
                if (File.Exists(inputPath))
                {
                    statusSets = CsvReader.Read<BattleActionEntry>(inputPath);
                    foreach (BattleActionEntry it in statusSets)
                        result[it.Id] = it.ActionData;
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

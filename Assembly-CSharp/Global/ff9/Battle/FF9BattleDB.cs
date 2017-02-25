using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FF9;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.Collections;
using Memoria.Prime.CSV;

public static partial class FF9BattleDB
{
    public static readonly EntryCollection<BattleStatusEntry> StatusSets;
    public static readonly EntryCollection<AA_DATA> CharacterActions;

    static FF9BattleDB()
	{
	    StatusSets = LoadStatusSets();
	    CharacterActions = LoadActions();
	}

    private static EntryCollection<BattleStatusEntry> LoadStatusSets()
    {
        try
        {
            String inputPath = DataResources.Battle.StatusSetsFile;
            if (!File.Exists(inputPath))
                throw new FileNotFoundException($"File with status sets not found: [{inputPath}]");

            BattleStatusEntry[] statusSets = CsvReader.Read<BattleStatusEntry>(inputPath);
            if (statusSets.Length < BattleStatusEntry.SetsCount)
                throw new NotSupportedException($"You must set {BattleStatusEntry.SetsCount} status sets, but there {statusSets.Length}.");

            return EntryCollection.CreateWithDefaultElement(statusSets, e => e.Id);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[ff9level] Load base stats of characters failed.");
            UIManager.Input.ConfirmQuit();
            return null;
        }
    }

    private static EntryCollection<AA_DATA> LoadActions()
    {
        try
        {
            String inputPath = DataResources.Battle.ActionsFile;
            if (!File.Exists(inputPath))
                throw new FileNotFoundException($"File with character actions not found: [{inputPath}]");

            BattleActionEntry[] statusSets = CsvReader.Read<BattleActionEntry>(inputPath);
            if (statusSets.Length < BattleStatusEntry.SetsCount)
                throw new NotSupportedException($"You must set {BattleStatusEntry.SetsCount} status sets, but there {statusSets.Length}.");

            return EntryCollection.CreateWithDefaultElement(statusSets, e => e.Id, e=>e.ActionData);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[ff9level] Load base stats of characters failed.");
            UIManager.Input.ConfirmQuit();
            return null;
        }
    }

	public static STAT_DATA[] status_data = new[]
	{
		new STAT_DATA(2, 0, 0, 134217728u, UInt32.MaxValue),
		new STAT_DATA(7, 20, 0, 98304u, 65538u),
		new STAT_DATA(0, 0, 0, 0u, 4u),
		new STAT_DATA(13, 0, 0, 0u, 8u),
		new STAT_DATA(16, 0, 0, 0u, 16u),
		new STAT_DATA(10, 0, 0, 0u, 32u),
		new STAT_DATA(9, 0, 0, 0u, 2147500097u),
		new STAT_DATA(0, 0, 0, 0u, 0u),
		new STAT_DATA(1, 0, 0, 4026506810u, 4294966975u),
		new STAT_DATA(5, 0, 0, 0u, 512u),
		new STAT_DATA(14, 0, 0, 0u, 1024u),
		new STAT_DATA(15, 0, 0, 0u, 2048u),
		new STAT_DATA(6, 0, 0, 0u, 4096u),
		new STAT_DATA(27, 0, 0, 0u, 8192u),
		new STAT_DATA(3, 0, 0, 2601720890u, 16448u),
		new STAT_DATA(0, 0, 0, 0u, 32768u),
		new STAT_DATA(8, 10, 30, 0u, 0u),
		new STAT_DATA(12, 0, 25, 32768u, 0u),
		new STAT_DATA(23, 10, 45, 0u, 0u),
		new STAT_DATA(22, 0, 40, 0u, 0u),
		new STAT_DATA(17, 0, 40, 0u, 0u),
		new STAT_DATA(26, 0, 30, 0u, 0u),
		new STAT_DATA(20, 0, 45, 0u, 0u),
		new STAT_DATA(21, 0, 45, 0u, 0u),
		new STAT_DATA(18, 0, 20, 33554432u, 0u),
		new STAT_DATA(19, 0, 20, 83886080u, 67108864u),
		new STAT_DATA(24, 0, 30, 0u, 0u),
		new STAT_DATA(4, 0, 25, 0u, 2281701376u),
		new STAT_DATA(11, 0, 20, 0u, 0u),
		new STAT_DATA(25, 0, 40, 0u, 0u),
		new STAT_DATA(0, 0, 10, 0u, 3221225471u),
		new STAT_DATA(0, 0, 25, 0u, 2281701440u)
	};

    
}

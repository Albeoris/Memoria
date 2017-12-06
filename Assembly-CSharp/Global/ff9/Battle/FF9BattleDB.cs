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
		new STAT_DATA(2, 0, 0, BattleStatus.Doom, (BattleStatus)UInt32.MaxValue),
		new STAT_DATA(7, 20, 0, BattleStatus.Defend | BattleStatus.Poison, BattleStatus.Venom | BattleStatus.Poison),
		new STAT_DATA(0, 0, 0, 0u, BattleStatus.Virus),
		new STAT_DATA(13, 0, 0, 0u, BattleStatus.Silence),
		new STAT_DATA(16, 0, 0, 0u, BattleStatus.Blind),
		new STAT_DATA(10, 0, 0, 0u, BattleStatus.Trouble),
		new STAT_DATA(9, 0, 0, 0u, BattleStatus.Petrify | BattleStatus.Zombie | BattleStatus.Trance | BattleStatus.GradualPetrify),
		new STAT_DATA(0, 0, 0, 0u, 0u),

        new STAT_DATA(1, 0, 0,
            BattleStatus.Venom | BattleStatus.Silence | BattleStatus.Blind | BattleStatus.Trouble | BattleStatus.LowHP | BattleStatus.Confuse | BattleStatus.Berserk | BattleStatus.Stop | BattleStatus.Defend | BattleStatus.Poison | BattleStatus.Sleep | BattleStatus.Regen | BattleStatus.Haste | BattleStatus.Slow | BattleStatus.Float | BattleStatus.Shell | BattleStatus.Protect | BattleStatus.Heat | BattleStatus.Freeze | BattleStatus.Vanish | BattleStatus.Doom | BattleStatus.Reflect | BattleStatus.Jump | BattleStatus.GradualPetrify,
            BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Virus | BattleStatus.Silence | BattleStatus.Blind | BattleStatus.Trouble | BattleStatus.EasyKill | BattleStatus.LowHP | BattleStatus.Confuse | BattleStatus.Berserk | BattleStatus.Stop | BattleStatus.AutoLife | BattleStatus.Trance | BattleStatus.Defend | BattleStatus.Poison | BattleStatus.Sleep | BattleStatus.Regen | BattleStatus.Haste | BattleStatus.Slow | BattleStatus.Float | BattleStatus.Shell | BattleStatus.Protect | BattleStatus.Heat | BattleStatus.Freeze | BattleStatus.Vanish | BattleStatus.Doom | BattleStatus.Mini | BattleStatus.Reflect | BattleStatus.Jump | BattleStatus.GradualPetrify),

		new STAT_DATA(5, 0, 0, 0u, BattleStatus.LowHP),
		new STAT_DATA(14, 0, 0, 0u, BattleStatus.Confuse),
		new STAT_DATA(15, 0, 0, 0u, BattleStatus.Berserk),
		new STAT_DATA(6, 0, 0, 0u, BattleStatus.Stop),
		new STAT_DATA(27, 0, 0, 0u, BattleStatus.AutoLife),

        new STAT_DATA(3, 0, 0,
            BattleStatus.Venom | BattleStatus.Silence | BattleStatus.Blind | BattleStatus.Trouble | BattleStatus.Confuse | BattleStatus.Berserk | BattleStatus.Stop | BattleStatus.Poison | BattleStatus.Sleep | BattleStatus.Slow | BattleStatus.Heat | BattleStatus.Freeze | BattleStatus.Doom | BattleStatus.Mini | BattleStatus.GradualPetrify,
		    BattleStatus.Zombie | BattleStatus.Trance),

        new STAT_DATA(0, 0, 0, 0u, BattleStatus.Defend),
		new STAT_DATA(8, 10, 30, 0u, 0u),
		new STAT_DATA(12, 0, 25, BattleStatus.Defend, 0u),
		new STAT_DATA(23, 10, 45, 0u, 0u),
		new STAT_DATA(22, 0, 40, 0u, 0u),
		new STAT_DATA(17, 0, 40, 0u, 0u),
		new STAT_DATA(26, 0, 30, 0u, 0u),
		new STAT_DATA(20, 0, 45, 0u, 0u),
		new STAT_DATA(21, 0, 45, 0u, 0u),
		new STAT_DATA(18, 0, 20, BattleStatus.Freeze, 0u),
		new STAT_DATA(19, 0, 20, BattleStatus.Heat | BattleStatus.Vanish, BattleStatus.Vanish),
		new STAT_DATA(24, 0, 30, 0u, 0u),
		new STAT_DATA(4, 0, 25, 0u, BattleStatus.Doom | BattleStatus.GradualPetrify),
		new STAT_DATA(11, 0, 20, 0u, 0u),
		new STAT_DATA(25, 0, 40, 0u, 0u),
		new STAT_DATA(0, 0, 10, 0u, BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Virus | BattleStatus.Silence | BattleStatus.Blind | BattleStatus.Trouble | BattleStatus.Zombie | BattleStatus.EasyKill | BattleStatus.Death | BattleStatus.LowHP | BattleStatus.Confuse | BattleStatus.Berserk | BattleStatus.Stop | BattleStatus.AutoLife | BattleStatus.Trance | BattleStatus.Defend | BattleStatus.Poison | BattleStatus.Sleep | BattleStatus.Regen | BattleStatus.Haste | BattleStatus.Slow | BattleStatus.Float | BattleStatus.Shell | BattleStatus.Protect | BattleStatus.Heat | BattleStatus.Freeze | BattleStatus.Vanish | BattleStatus.Doom | BattleStatus.Mini | BattleStatus.Reflect | BattleStatus.GradualPetrify),
		new STAT_DATA(0, 0, 25, 0u, BattleStatus.Zombie | BattleStatus.Doom | BattleStatus.GradualPetrify)
	};

    
}

using System;
using System.Collections.Generic;
using FF9;
using Memoria;
using Memoria.Data;

public partial class BattleHUD : UIScene
{
    private class DamageAnimationInfo
    {
        public Int32 IncrementStep;
        public Int32 CurrentValue;
        public Int32 RequiredValue;
        public Int32 FrameLeft;
    }

    private class BattleItemListData : ListDataTypeBase
    {
        public RegularItem Id;
        public Int32 Count;
    }

    private class BattleAbilityListData : ListDataTypeBase
    {
        public Int32 Id;
    }

    private enum CommandMenu
    {
        Attack,
        Defend,
        Ability1,
        Ability2,
        Item,
        Change,
        AccessMenu = 7,
    }

    private enum CursorGroup
    {
        Individual,
        AllPlayer,
        AllEnemy,
        All,
        None,
    }

    private enum AbilityStatus
    {
        None,
        Disable,
        Enable,
    }

    private enum ParameterStatus
    {
        Normal,
        Critical,
        Dead,
    }

    private enum SubMenuType
    {
        Normal,
        Ability,
        Item,
        Throw,
        Slide,
    }

    [Flags]
    public enum LibraInformation : uint
    {
        Name            = 0x1,
        Level           = 0x2,
        HP              = 0x4,
        MP              = 0x8,
        Category        = 0x10,
        ElementWeak     = 0x20,
        ItemSteal       = 0x40,
        BlueLearn       = 0x80,

        NameLevel = Name | Level,
        HPMP = HP | MP,

        Default = NameLevel | HPMP | Category | ElementWeak
    }

    private class AbilityPlayerDetail
    {
        public Character Player;
        public Boolean HasAp;
        public readonly Dictionary<Int32, Boolean> AbilityEquipList;
        public readonly Dictionary<Int32, Int32> AbilityPaList;
        public readonly Dictionary<Int32, Int32> AbilityMaxPaList;
        public readonly Dictionary<Int32, Boolean> AbilityTranceList;

        public AbilityPlayerDetail()
        {
            AbilityEquipList = new Dictionary<Int32, Boolean>();
            AbilityPaList = new Dictionary<Int32, Int32>();
            AbilityMaxPaList = new Dictionary<Int32, Int32>();
            AbilityTranceList = new Dictionary<Int32, Boolean>();
        }

        public void Clear()
        {
            AbilityEquipList.Clear();
            AbilityPaList.Clear();
            AbilityMaxPaList.Clear();
            AbilityTranceList.Clear();
        }
    }

    private class MagicSwordCondition
    {
        public Boolean IsViviExist;
        public Boolean IsViviDead;
        public Boolean IsSteinerMini;

        public Boolean Changed(MagicSwordCondition other)
        {
            return IsViviExist != other.IsViviExist || IsViviDead != other.IsViviDead || IsSteinerMini != other.IsSteinerMini;
        }
    }

    private class CommandMPCondition
    {
        public Boolean CantCastCommand1;
        public Boolean CantCastCommand2;

        public Boolean Changed(CommandMPCondition other)
        {
            return CantCastCommand1 != other.CantCastCommand1 || CantCastCommand2 != other.CantCastCommand2;
        }
    }

    private class CommandDetail
    {
        public BattleCommandId CommandId;
        public Int32 SubId;
        public UInt16 TargetId;
        public UInt32 TargetType;
    }

    private struct PairCharCommand
    {
        public Int32 PlayerIndex;
        public BattleCommandId CommandId;

        public PairCharCommand(Int32 pi, BattleCommandId ci)
		{
            PlayerIndex = pi;
            CommandId = ci;
        }
    }

    private sealed class KnownUnit
    {
        public Int32 Index { get; }
        public BattleUnit Unit { get; }

        public KnownUnit(Int32 index, BattleUnit unit)
        {
            Index = index;
            Unit = unit;
        }
    }

    private class Message
    {
        public String message;
        public Byte priority;
        public Single counter;
        public Boolean isRect;
        public CMD_DATA titleCmd;
    }

    private class PlayerMemo
	{
        public PlayerMemo(PLAYER p, Boolean updateRow)
		{
            original = p;
            if (p == null)
                return;
            max = new POINTS();
            btl_init.CopyPoints(max, p.max);
            elem = new ELEMENT();
            elem.dex = p.elem.dex;
            elem.str = p.elem.str;
            elem.mgc = p.elem.mgc;
            elem.wpr = p.elem.wpr;
            defence = new ItemDefence();
            defence.PhysicalDefence = p.defence.PhysicalDefence;
            defence.PhysicalEvade = p.defence.PhysicalEvade;
            defence.MagicalDefence = p.defence.MagicalDefence;
            defence.MagicalEvade = p.defence.MagicalEvade;
            saExtended = new HashSet<SupportAbility>(p.saExtended);
            serialNo = p.info.serial_no;
            row = p.info.row;
            battleRow = row;
            BTL_DATA btl = btl_util.getBattlePtr(p);
            if (updateRow && btl != null)
            {
                battleRow = btl.bi.row;
                p.info.row = battleRow;
            }
            battlePermanentStatus = 0;
        }

        public PLAYER original;
		public POINTS max;
		public ELEMENT elem;
		public ItemDefence defence;
		public HashSet<SupportAbility> saExtended;
        public CharacterSerialNumber serialNo;
        public Byte row;
        public Byte battleRow;
        public BattleStatus battlePermanentStatus;
    }
}
using System;
using System.Collections.Generic;
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
        public Int32 Id;
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
        Empty,
    }

    private enum SubMenuType
    {
        Normal,
        Ability,
        Item,
        Throw,
        Slide,
    }

    private class AbilityPlayerDetail
    {
        public Character Player;
        public Boolean HasAp;
        public readonly Dictionary<Int32, Boolean> AbilityEquipList;
        public readonly Dictionary<Int32, Int32> AbilityPaList;
        public readonly Dictionary<Int32, Int32> AbilityMaxPaList;

        public AbilityPlayerDetail()
        {
            AbilityEquipList = new Dictionary<Int32, Boolean>();
            AbilityPaList = new Dictionary<Int32, Int32>();
            AbilityMaxPaList = new Dictionary<Int32, Int32>();
        }

        public void Clear()
        {
            AbilityEquipList.Clear();
            AbilityPaList.Clear();
            AbilityMaxPaList.Clear();
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

    private class CommandDetail
    {
        public BattleCommandId CommandId;
        public UInt32 SubId;
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
    }
}
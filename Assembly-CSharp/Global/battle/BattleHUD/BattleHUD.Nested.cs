using FF9;
using Memoria;
using Memoria.Data;
using System;
using System.Collections.Generic;

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
        Instant,
    }

    [Flags]
    public enum LibraInformation : uint
    {
        Name = 0x1,
        Level = 0x2,
        HP = 0x4,
        MP = 0x8,
        Stats = 0x10,
        Category = 0x20,
        ItemSteal = 0x40,
        ItemBonus = 0x80,
        BlueLearn = 0x100,
        ElementWeak = 0x200,
        ElementResist = 0x400,
        ElementImmune = 0x800,
        ElementAbsorb = 0x1000,
        AttackList = 0x2000,
        StatusAuto = 0x4000,
        StatusImmune = 0x8000,
        StatusResist = 0x10000,

        NameLevel = Name | Level,
        HPMP = HP | MP,
        ElementalAffinities = ElementWeak | ElementResist | ElementImmune | ElementAbsorb,
        StatusAffinities = StatusAuto | StatusImmune | StatusResist,
        MobLoot = ItemSteal | ItemBonus,

        Default = NameLevel | HPMP | Category | ElementWeak,
        All = NameLevel | HPMP | Stats | ElementalAffinities | StatusAffinities | Category | MobLoot | BlueLearn | AttackList
    }

    private static readonly LibraInformation[] LibraAutoProcess =
    {
        LibraInformation.Stats,
        LibraInformation.Category,
        LibraInformation.ItemSteal,
        LibraInformation.ItemBonus,
        LibraInformation.BlueLearn,
        LibraInformation.ElementWeak,
        LibraInformation.ElementResist,
        LibraInformation.ElementImmune,
        LibraInformation.ElementAbsorb,
        LibraInformation.StatusAuto,
        LibraInformation.StatusImmune,
        LibraInformation.StatusResist,
        LibraInformation.AttackList
    };

    private class AbilityPlayerDetail
    {
        public PLAYER Player;
        public Boolean HasAp;
        public readonly Dictionary<Int32, Boolean> AbilityEquipList;
        public readonly Dictionary<Int32, Int32> AbilityPaList;
        public readonly Dictionary<Int32, Int32> AbilityMaxPaList;
        public readonly Dictionary<Int32, Boolean> AbilityTranceList;
        public readonly Dictionary<Int32, BattleMagicSwordSet> AbilityMagicSet;

        public AbilityPlayerDetail()
        {
            AbilityEquipList = new Dictionary<Int32, Boolean>();
            AbilityPaList = new Dictionary<Int32, Int32>();
            AbilityMaxPaList = new Dictionary<Int32, Int32>();
            AbilityTranceList = new Dictionary<Int32, Boolean>();
            AbilityMagicSet = new Dictionary<Int32, BattleMagicSwordSet>();
        }

        public void Clear()
        {
            AbilityEquipList.Clear();
            AbilityPaList.Clear();
            AbilityMaxPaList.Clear();
            AbilityTranceList.Clear();
            AbilityMagicSet.Clear();
        }
    }

    private class CommandDetail
    {
        public BattleCommandMenu Menu;
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
            playerPermanentStatus = 0;
        }

        public PLAYER original;
        public POINTS max;
        public ELEMENT elem;
        public ItemDefence defence;
        public HashSet<SupportAbility> saExtended;
        public CharacterSerialNumber serialNo;
        public Byte row;
        public Byte battleRow;
        public BattleStatus playerPermanentStatus;
    }
}

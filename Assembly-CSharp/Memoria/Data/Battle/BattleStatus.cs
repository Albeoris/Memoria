using System;
using System.Linq;
using System.Collections;
using FF9;

namespace Memoria.Data
{
    [Flags]
    public enum BattleStatus : ulong
    {
        Petrify = 1 << 0,
        Venom = 1 << 1,
        Virus = 1 << 2,
        Silence = 1 << 3,
        Blind = 1 << 4,
        Trouble = 1 << 5,
        Zombie = 1 << 6,
        EasyKill = 1 << 7,
        Death = 1 << 8,
        LowHP = 1 << 9,
        Confuse = 1 << 10,
        Berserk = 1 << 11,
        Stop = 1 << 12,
        AutoLife = 1 << 13,
        Trance = 1 << 14,
        Defend = 1 << 15,
        Poison = 1 << 16,
        Sleep = 1 << 17,
        Regen = 1 << 18,
        Haste = 1 << 19,
        Slow = 1 << 20,
        Float = 1 << 21,
        Shell = 1 << 22,
        Protect = 1 << 23,
        Heat = 1 << 24,
        Freeze = 1 << 25,
        Vanish = 1 << 26,
        Doom = 1 << 27,
        Mini = 1 << 28,
        Reflect = 1 << 29,
        Jump = 1 << 30,
        GradualPetrify = 1u << 31,
        ChangeStat = 0x100000000ul,
        CustomStatus1 = 0x200000000ul,
        CustomStatus2 = 0x400000000ul,
        CustomStatus3 = 0x800000000ul,
        CustomStatus4 = 0x1000000000ul,
        CustomStatus5 = 0x2000000000ul,
        CustomStatus6 = 0x4000000000ul,
        CustomStatus7 = 0x8000000000ul,
        CustomStatus8 = 0x10000000000ul,
        CustomStatus9 = 0x20000000000ul,
        CustomStatus10 = 0x40000000000ul,
        CustomStatus11 = 0x80000000000ul,
        CustomStatus12 = 0x100000000000ul,
        CustomStatus13 = 0x200000000000ul,
        CustomStatus14 = 0x400000000000ul,
        CustomStatus15 = 0x800000000000ul,
        CustomStatus16 = 0x1000000000000ul,
        CustomStatus17 = 0x2000000000000ul,
        CustomStatus18 = 0x4000000000000ul,
        CustomStatus19 = 0x8000000000000ul,
        CustomStatus20 = 0x10000000000000ul,
        CustomStatus21 = 0x20000000000000ul,
        CustomStatus22 = 0x40000000000000ul,
        CustomStatus23 = 0x80000000000000ul,
        CustomStatus24 = 0x100000000000000ul,
        CustomStatus25 = 0x200000000000000ul,
        CustomStatus26 = 0x400000000000000ul,
        CustomStatus27 = 0x800000000000000ul,
        CustomStatus28 = 0x1000000000000000ul,
        CustomStatus29 = 0x2000000000000000ul,
        CustomStatus30 = 0x4000000000000000ul,
        CustomStatus31 = 0x8000000000000000ul
    }

    // TODO
    // It may be too much code change to completly replace BattleStatus by BattleStatusExtended
    // Keep both? Completly replace BattleStatus by the HashSet? Keep BattleStatus only (limit to 64 different statuses at most)
    // For now, I [Tirlititi] decided to keep BattleStatus only; having both doesn't seem intersting
    // If we choose to replace it by BattleStatusExtended, I think there shouldn't be any constant like BattleStatus.Petrify, only BattleStatusId.Petrify
    // And uint <-> BattleStatusExtended conversion should be taken care of very cautiously in the different parts of the code
    /*
    public class BattleStatusExtended : HashSet<BattleStatusId>
    {
        public BattleStatusExtended() : base() { }
        public BattleStatusExtended(BattleStatus status) : base(Comn.getBitIndexes((UInt32)status).Cast<BattleStatusId>()) { }
        public BattleStatusExtended(IEnumerable<BattleStatusId> source) : base(source) { }
    
        public static operator BattleStatusExtended(UInt32 bitList) => new BattleStatusExtended(Comn.getBitIndexes(bitList).Cast<BattleStatusId>());
    
        public static BattleStatusExtended operator&(BattleStatusExtended left, BattleStatusExtended right)
        {
            if (left.Count <= right.Count)
            {
                BattleStatusExtended result = new BattleStatusExtended(left);
                result.IntersectWith(right);
                return result;
            }
            else
            {
                BattleStatusExtended result = new BattleStatusExtended(right);
                result.IntersectWith(left);
                return result;
            }
        }
    
        public static BattleStatusExtended operator|(BattleStatusExtended left, BattleStatusExtended right)
        {
            BattleStatusExtended result = new BattleStatusExtended(left);
            result.UnionWith(right);
            return result;
        }
    }
    */

    public static class BattleStatusConst
    {
        public static BattleStatus OutOfBattle = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Virus | BattleStatus.Silence | BattleStatus.Blind | BattleStatus.Trouble | BattleStatus.Zombie; // Death is not a status out of battles but only a HP check
        public static BattleStatus AnyPositive = BattleStatus.AutoLife | BattleStatus.Trance | BattleStatus.Defend | BattleStatus.Regen | BattleStatus.Haste | BattleStatus.Float | BattleStatus.Shell | BattleStatus.Protect | BattleStatus.Vanish | BattleStatus.Reflect;
        public static BattleStatus AnyNegative = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Virus | BattleStatus.Silence | BattleStatus.Blind | BattleStatus.Trouble | BattleStatus.Zombie | BattleStatus.Death | BattleStatus.Confuse | BattleStatus.Stop | BattleStatus.Poison | BattleStatus.Sleep | BattleStatus.Slow | BattleStatus.Heat | BattleStatus.Freeze | BattleStatus.Doom | BattleStatus.Mini | BattleStatus.GradualPetrify;
        public static BattleStatus Achievement = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Virus | BattleStatus.Silence | BattleStatus.Blind | BattleStatus.Trouble | BattleStatus.Zombie | BattleStatus.Death | BattleStatus.LowHP | BattleStatus.Confuse | BattleStatus.Berserk | BattleStatus.Stop | BattleStatus.AutoLife | BattleStatus.Poison | BattleStatus.Sleep | BattleStatus.Regen | BattleStatus.Haste | BattleStatus.Slow | BattleStatus.Float | BattleStatus.Shell | BattleStatus.Protect | BattleStatus.Heat | BattleStatus.Freeze | BattleStatus.Vanish | BattleStatus.Doom | BattleStatus.Mini | BattleStatus.Reflect | BattleStatus.GradualPetrify;
        public static BattleStatus ContiCount = BattleStatus.Poison | BattleStatus.Sleep | BattleStatus.Regen | BattleStatus.Haste | BattleStatus.Slow | BattleStatus.Float | BattleStatus.Shell | BattleStatus.Protect | BattleStatus.Heat | BattleStatus.Freeze | BattleStatus.Vanish | BattleStatus.Reflect; // 0xEFFF0000 originally, now without Jump, Doom and GPetrify
        public static BattleStatus OprCount = BattleStatus.Venom | BattleStatus.Poison | BattleStatus.Regen | BattleStatus.GradualPetrify | BattleStatus.Doom; // 0x50002 originally, now Doom and GPetrify also use opr
        public static BattleStatus CmdCancel = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Death | BattleStatus.Confuse | BattleStatus.Berserk | BattleStatus.Sleep; // 0x20D03
        public static BattleStatus NoInput = CmdCancel | BattleStatus.Stop | BattleStatus.Freeze | BattleStatus.Jump; // 0x42021D03;
        public static BattleStatus IdleDying = BattleStatus.Venom | BattleStatus.LowHP | BattleStatus.Poison | BattleStatus.Sleep;
        public static BattleStatus IdleDefend = BattleStatus.Defend;
        public static BattleStatus Immobilized = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Stop | BattleStatus.Freeze; // 0x2001003
        public static BattleStatus FrozenAnimation = Immobilized | BattleStatus.Jump;
        public static BattleStatus NoReaction = FrozenAnimation | BattleStatus.Death;
        public static BattleStatus NoDamageMotion = NoReaction | BattleStatus.Defend;
        public static BattleStatus StopAtb = BattleStatus.Petrify | BattleStatus.Death | BattleStatus.Stop | BattleStatus.Jump; // 0x40001101
        public static BattleStatus ChgPolyClut = BattleStatus.Petrify;
        public static BattleStatus CannotEscape = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Zombie | BattleStatus.Death | BattleStatus.Stop | BattleStatus.Sleep | BattleStatus.Freeze | BattleStatus.Jump;
        public static BattleStatus CannotTrance = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Zombie | BattleStatus.Death | BattleStatus.Stop | BattleStatus.Trance | BattleStatus.Freeze;
        public static BattleStatus NoRebirthFlame = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Zombie | BattleStatus.Stop;
        public static BattleStatus NoReset = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Virus | BattleStatus.Silence | BattleStatus.Blind | BattleStatus.Trouble | BattleStatus.Zombie | BattleStatus.Death | BattleStatus.LowHP | BattleStatus.Confuse | BattleStatus.Berserk | BattleStatus.Stop | BattleStatus.AutoLife | BattleStatus.Trance | BattleStatus.Defend | BattleStatus.Doom | BattleStatus.GradualPetrify; // 0x8800FF7F
        public static BattleStatus BattleEnd = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Stop;
        public static BattleStatus BattleEndFull = BattleEnd | BattleStatus.Death;
        public static BattleStatus BattleEndInMenu = BattleEnd & OutOfBattle;
        public static BattleStatus RemoveOnMainCommand = BattleStatus.Defend;
        public static BattleStatus RemoveOnMagicallyAttacked = BattleStatus.Vanish;
        public static BattleStatus RemoveOnPhysicallyAttacked = BattleStatus.Confuse | BattleStatus.Sleep; // 0x20400
        public static BattleStatus RemoveOnEvent = BattleStatus.Confuse | BattleStatus.Stop | BattleStatus.Defend | BattleStatus.Freeze;
        public static BattleStatus RemoveOnMonsterTransform = BattleStatus.ChangeStat;
        public static BattleStatus PreventEnemyCmd = Immobilized | BattleStatus.Death | BattleStatus.Sleep; // 0x2021103
        public static BattleStatus PreventCounter = PreventEnemyCmd; // 0x2021103
        public static BattleStatus CannotAct = Immobilized | BattleStatus.Jump | BattleStatus.Death | BattleStatus.Sleep;
        public static BattleStatus PenaltyEvade = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Virus | BattleStatus.Blind | BattleStatus.Confuse | BattleStatus.Stop | BattleStatus.Sleep | BattleStatus.Freeze;
        public static BattleStatus PreventAlternateCamera = BattleStatus.Vanish | BattleStatus.Mini;
        public static BattleStatus PreventATBConfirm = BattleStatus.Venom | BattleStatus.Sleep | BattleStatus.Freeze;
        public static BattleStatus PreventReflect = BattleStatus.Petrify;
        public static BattleStatus VictoryClear = BattleStatus.Confuse | BattleStatus.Berserk | BattleStatus.Stop | BattleStatus.AutoLife | BattleStatus.Defend | BattleStatus.Poison | BattleStatus.Sleep | BattleStatus.Regen | BattleStatus.Haste | BattleStatus.Slow | BattleStatus.Float | BattleStatus.Shell | BattleStatus.Protect | BattleStatus.Heat | BattleStatus.Freeze | BattleStatus.Vanish | BattleStatus.Doom | BattleStatus.Mini | BattleStatus.Reflect | BattleStatus.GradualPetrify;
        public static BattleStatus CannotUseAbilityInMenu = BattleStatus.Petrify | BattleStatus.Silence;
        public static BattleStatus CannotUseMagic = BattleStatus.Silence;
        public static BattleStatus ApplyReflect = BattleStatus.Reflect;
        public static BattleStatus ApplyTrouble = BattleStatus.Trouble;
        public static BattleStatus ZombieEffect = BattleStatus.Zombie;
        public static BattleStatus DisableRewards = BattleStatus.Petrify | BattleStatus.Virus | BattleStatus.Zombie;
        public static BattleStatus CannotSpeak = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Silence | BattleStatus.Death | BattleStatus.Stop | BattleStatus.Sleep | BattleStatus.Freeze | BattleStatus.Jump;

        public static void Update()
        {
            NoInput |= CmdCancel;
            FrozenAnimation |= Immobilized;
            BattleEndFull = BattleEnd | BattleStatus.Death;
            NoReaction |= FrozenAnimation;
            PreventEnemyCmd |= Immobilized;
            PreventCounter |= Immobilized;
            CannotAct |= Immobilized;
            BattleEndInMenu = BattleEnd & OutOfBattle;
        }
    }

    public static class ExtensionStatus
    {
        public static BattleStatus ToBattleStatus(this BattleStatusId statusId)
        {
            return (BattleStatus)(1ul << (Int32)statusId);
        }

        public static BattleStatus ToBattleStatus(this BattleStatusIdOldVersion oldId)
        {
            if (oldId == 0)
                return 0;
            return (BattleStatus)(1ul << ((Int32)oldId - 1));
        }

        public static IEnumerable ToStatusList(this BattleStatus status)
        {
            UInt64 bitList = (UInt64)status;
            Byte index = 0;
            while (bitList != 0 && index < 64)
            {
                if ((bitList & 1) != 0)
                    yield return (BattleStatusId)index;
                ++index;
                bitList >>= 1;
            }
            yield break;
        }

        public static BattleStatusDataEntry GetStatData(this BattleStatusId statusId)
        {
            if (FF9StateSystem.Battle.FF9Battle.status_data.TryGetValue(statusId, out BattleStatusDataEntry data))
                return data;
            throw new Exception($"[BattleStatus] Trying to use the non-existing status {statusId}");
        }
    }
}

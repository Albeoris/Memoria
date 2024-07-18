using System;
using System.Collections;

namespace Memoria.Data
{
    [Flags]
    public enum BattleStatus : uint
    {
        Petrify = 1 << 0, // 1,
        Venom = 1 << 1, //2
        Virus = 1 << 2, //4,
        Silence = 1 << 3, //8,
        Blind = 1 << 4, //16,
        Trouble = 1 << 5, //32,
        Zombie = 1 << 6, //64,
        EasyKill = 1 << 7, //128,
        Death = 1 << 8, //256,
        LowHP = 1 << 9, //512,
        Confuse = 1 << 10, //1024,
        Berserk = 1 << 11, //2048,
        Stop = 1 << 12, //4096,
        AutoLife = 1 << 13, //8192,
        Trance = 1 << 14, //16384,
        Defend = 1 << 15, //32768,
        Poison = 1 << 16, //65536,
        Sleep = 1 << 17, //131072,
        Regen = 1 << 18, //262144,
        Haste = 1 << 19, //524288,
        Slow = 1 << 20, //1048576,
        Float = 1 << 21, //2097152,
        Shell = 1 << 22, //4194304,
        Protect = 1 << 23, //8388608,
        Heat = 1 << 24, //16777216,
        Freeze = 1 << 25, //33554432,
        Vanish = 1 << 26, //67108864,
        Doom = 1 << 27, //134217728,
        Mini = 1 << 28, //268435456,
        Reflect = 1 << 29, //536870912,
        Jump = 1 << 30, //1073741824,
        GradualPetrify = 1u << 31 //2147483648
    }

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
        // TODO Replace by (NoInput | ParameterFromMagicSwordSets.csv)
        public static BattleStatus NoMagicSword = CmdCancel | BattleStatus.Silence | BattleStatus.Stop | BattleStatus.Heat | BattleStatus.Freeze | BattleStatus.Mini; // 0x13021D0B
        public static BattleStatus ChgPolyClut = BattleStatus.Petrify;
        public static BattleStatus CannotEscape = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Zombie | BattleStatus.Death | BattleStatus.Stop | BattleStatus.Sleep | BattleStatus.Freeze | BattleStatus.Jump;
        public static BattleStatus CannotTrance = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Zombie | BattleStatus.Death | BattleStatus.Stop | BattleStatus.Trance | BattleStatus.Freeze;
        public static BattleStatus NoRebirthFlame = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Zombie | BattleStatus.Stop;
        public static BattleStatus NoReset = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Virus | BattleStatus.Silence | BattleStatus.Blind | BattleStatus.Trouble | BattleStatus.Zombie | BattleStatus.Death | BattleStatus.LowHP | BattleStatus.Confuse | BattleStatus.Berserk | BattleStatus.Stop | BattleStatus.AutoLife | BattleStatus.Trance | BattleStatus.Defend | BattleStatus.Doom | BattleStatus.GradualPetrify; // 0x8800FF7F
        public static BattleStatus BattleEnd = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Stop;
        public static BattleStatus BattleEndFull = BattleEnd | BattleStatus.Death;
        public static BattleStatus RemoveOnMainCommand = BattleStatus.Defend;
        public static BattleStatus RemoveOnMagicallyAttacked = BattleStatus.Vanish;
        public static BattleStatus RemoveOnPhysicallyAttacked = BattleStatus.Confuse | BattleStatus.Sleep; // 0x20400
        public static BattleStatus RemoveOnEvent = BattleStatus.Confuse | BattleStatus.Stop | BattleStatus.Defend | BattleStatus.Freeze;
        public static BattleStatus PreventEnemyCmd = Immobilized | BattleStatus.Death | BattleStatus.Sleep; // 0x2021103
        public static BattleStatus PreventCounter = PreventEnemyCmd; // 0x2021103
        public static BattleStatus CannotAct = Immobilized | BattleStatus.Jump | BattleStatus.Death | BattleStatus.Sleep;
        public static BattleStatus PenaltyEvade = BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Virus | BattleStatus.Blind | BattleStatus.Confuse | BattleStatus.Stop | BattleStatus.Sleep | BattleStatus.Freeze;
        public static BattleStatus PreventAlternateCamera = BattleStatus.Vanish | BattleStatus.Mini;
        public static BattleStatus PreventATBConfirm = BattleStatus.Venom | BattleStatus.Sleep | BattleStatus.Freeze;
        public static BattleStatus PreventReflect = BattleStatus.Petrify;
        public static BattleStatus VictoryClear = BattleStatus.Confuse | BattleStatus.Berserk | BattleStatus.Stop | BattleStatus.AutoLife | BattleStatus.Defend | BattleStatus.Poison | BattleStatus.Sleep | BattleStatus.Regen | BattleStatus.Haste | BattleStatus.Slow | BattleStatus.Float | BattleStatus.Shell | BattleStatus.Protect | BattleStatus.Heat | BattleStatus.Freeze | BattleStatus.Vanish | BattleStatus.Doom | BattleStatus.Mini | BattleStatus.Reflect | BattleStatus.GradualPetrify;
        public static BattleStatus BattleEndInMenu = BattleEnd & OutOfBattle;
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
            NoMagicSword |= NoInput;
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
            return (BattleStatus)(1 << (Int32)statusId);
        }

        public static BattleStatus ToBattleStatus(this BattleStatusIdOldVersion oldId)
        {
            if (oldId == 0)
                return 0;
            return (BattleStatus)(1 << ((Int32)oldId - 1));
        }

        // TODO: BattleStatus as HashSet
        public static IEnumerable ToStatusList(this BattleStatus status)
        {
            UInt64 asBits = (UInt64)status;
            Int32 pos = 0;
            UInt64 bit = 1;
            while (bit != 0)
            {
                if ((asBits & bit) != 0)
                    yield return (BattleStatusId)pos;
                bit <<= 1;
                ++pos;
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

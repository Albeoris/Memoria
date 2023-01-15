using System;

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
        GradualPetrify = 1u << 31, //2147483648,

        OutOfBattle = Petrify | Venom | Virus | Silence | Blind | Trouble | Zombie, // Death is not a status out of battles but only a HP check
        Achievement = Petrify | Venom | Virus | Silence | Blind | Trouble | Zombie | Death | LowHP | Confuse | Berserk | Stop | AutoLife | Poison | Sleep | Regen | Haste | Slow | Float | Shell | Protect | Heat | Freeze | Vanish | Doom | Mini | Reflect | GradualPetrify,
        ContiGood = Regen | Haste | Float | Shell | Protect | Vanish | Reflect, // 0x24EC0000
        ContiBad = Poison | Sleep | Slow | Heat | Freeze | Doom | Mini | GradualPetrify, // 0x9B130000
        ContiCount = ContiGood | Jump | Poison | Sleep | Slow | Heat | Freeze | Doom | GradualPetrify, // 0xEFFF0000
        OprCount = Venom | Poison | Regen, // 0x50002
        NoInput = CmdCancel | Stop | Freeze | Jump, // 0x42021D03,
        CmdCancel = Petrify | Venom | Death | Confuse | Berserk | Sleep, // 0x20D03
        IdleDying = Venom | LowHP | Poison | Sleep,
        Immobilized = Petrify | Venom | Stop | Freeze, // 0x2001003
        FrozenAnimation = Immobilized | Jump,
        StopAtb = Petrify | Death | Stop | Jump, // 0x40001101
        NoMagic = CmdCancel | Silence | Stop | Heat | Freeze | Mini, // 0x13021D0B
        ChgPolyCol = Zombie | Berserk | Heat | Freeze,
        CannotEscape = Petrify | Venom | Zombie | Death | Stop | Sleep | Freeze | Jump,
        CannotTrance = Petrify | Venom | Zombie | Death | Stop | Trance | Freeze,
        NoReset = Petrify | Venom | Virus | Silence | Blind | Trouble | Zombie | Death | LowHP | Confuse | Berserk | Stop | AutoLife | Trance | Defend | Doom | GradualPetrify, // 0x8800FF7F
        BattleEnd = Petrify | Venom | Death | Stop,
        CancelPhysics = Confuse | Sleep, // 0x20400
        TimeOpr = OprCount | Trance | Doom | GradualPetrify, // 0x88054002
        NoReaction = FrozenAnimation | Death,
        CancelEvent = Confuse | Stop | Defend | Freeze,
        AlterNoSet = Petrify | Haste | Slow | Mini, // 0x10180001
        PreventEnemyCmd = Immobilized | Death | Sleep, // 0x2021103
        CannotAct = Immobilized | Jump | Death | Sleep,
        VictoryClear = Confuse | Berserk | Stop | AutoLife | Defend | Poison | Sleep | Regen | Haste | Slow | Float | Shell | Protect | Heat | Freeze | Vanish | Doom | Mini | Reflect | GradualPetrify,
        CannotSpeak = Petrify | Venom | Silence | Death | Stop | Sleep | Freeze | Jump
    }
}
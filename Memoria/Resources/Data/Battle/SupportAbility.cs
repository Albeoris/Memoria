using System;

namespace Memoria
{
    [Flags]
    public enum SupportAbility : ulong
    {
        /// <summary>
        /// Automatically casts {b}Reflect{/b} in battle.
        /// </summary>
        AutoReflect = 1 << 0000, // 1

        /// <summary>
        /// Automatically casts {b}Float{/b} in battle.
        /// </summary>
        AutoFloat = 1 << 0001, // 2

        /// <summary>
        /// Automatically casts {b}Haste{/b} in battle.
        /// </summary>
        AutoHaste = 1 << 0002, // 4

        /// <summary>
        /// Automatically casts {b}Regen{/b} in battle.
        /// </summary>
        AutoRegen = 1 << 0003, // 8

        /// <summary>
        /// Automatically casts {b}Life{/b} in battle.
        /// </summary>
        AutoLife = 1 << 0004, // 16

        /// <summary>
        /// Increases HP by 10%.
        /// </summary>
        HP10 = 1 << 0005, // 32

        /// <summary>
        /// Increases HP by 20%.
        /// </summary>
        HP20 = 1 << 0006, // 64

        /// <summary>
        /// Increases MP by 10%.
        /// </summary>
        MP10 = 1 << 0007, // 128

        /// <summary>
        /// Increases MP by 20%.
        /// </summary>
        MP20 = 1 << 0008, // 256

        /// <summary>
        /// Raises physical attack accuracy.
        /// </summary>
        Accuracy = 1 << 0009, // 512

        /// <summary>
        /// Lowers enemy’s physical attack accuracy.
        /// </summary>
        Distract = 1 << 0010, // 1024

        /// <summary>
        /// Back row attacks like front row.
        /// </summary>
        LongReach = 1 << 0011, // 2048

        /// <summary>
        /// Uses own MP to raise {b}Attack Pwr{/b}.
        /// </summary>
        MPAttack = 1 << 0012, // 4096

        /// <summary>
        /// Deals lethal damage to flying enemies.
        /// </summary>
        BirdKiller = 1 << 0013, // 8192

        /// <summary>
        /// Deals lethal damage to insects.
        /// </summary>
        BugKiller = 1 << 0014, // 16384

        /// <summary>
        /// Deals lethal damage to stone enemies.
        /// </summary>
        StoneKiller = 1 << 0015, // 32768

        /// <summary>
        /// Deals lethal damage to undead enemies.
        /// </summary>
        UndeadKiller = 1 << 0016, // 65536

        /// <summary>
        /// Deals lethal damage to dragons.
        /// </summary>
        DragonKiller = 1 << 0017, // 131072

        /// <summary>
        /// Deals lethal damage to demons.
        /// </summary>
        DevilKiller = 1 << 0018, // 262144

        /// <summary>
        /// Deals lethal damage to beasts.
        /// </summary>
        BeastKiller = 1 << 0019, // 524288

        /// <summary>
        /// Deals lethal damage to humans.
        /// </summary>
        ManEater = 1 << 0020, // 1048576

        /// <summary>
        /// Jump higher to raise jump attack power.
        /// </summary>
        HighJump = 1 << 0021, // 2097152

        /// <summary>
        /// Steal better items.
        /// </summary>
        MasterThief = 1 << 0022, // 4194304

        /// <summary>
        /// Steal Gil along with items.
        /// </summary>
        StealGil = 1 << 0023, // 8388608

        /// <summary>
        /// Restores target’s HP.
        /// </summary>
        Healer = 1 << 0024, // 16777216

        /// <summary>
        /// Adds weapon’s status effect (Add ST) when you Attack.
        /// </summary>
        AddStatus = 1 << 0025, // 33554432

        /// <summary>
        /// Raises {b}Defence{/b} occasionally.
        /// </summary>
        GambleDefence = 1 << 0026, // 67108864

        /// <summary>
        /// Doubles the potency of medicinal items.
        /// </summary>
        Chemist = 1 << 0027, // 134217728

        /// <summary>
        /// Raises the strength of Throw.
        /// </summary>
        PowerThrow = 1 << 0028, // 268435456

        /// <summary>
        /// Raises the strength of Chakra.
        /// </summary>
        PowerUp = 1 << 0029, // 536870912

        /// <summary>
        /// Nullifies {b}Reflect{/b} and attacks.
        /// </summary>
        ReflectNull = 1 << 0030, // 1073741824


        /// <summary>
        /// Doubles the strength of spells by using {b}Reflect{/b}.
        /// </summary>
        Reflectx2 = 1UL << 0031, // 2147483648

        /// <summary>
        /// Nullifies magic element.
        /// </summary>
        MagElemNull = 1 << 0032, // 4294967296

        /// <summary>
        /// Raises the strength of spells.
        /// </summary>
        Concentrate = 1 << 0033, // 8589934592

        /// <summary>
        /// Cuts MP use by half in battle.
        /// </summary>
        HalfMP = 1 << 0034, // 17179869184

        /// <summary>
        /// Allows you to Trance faster.
        /// </summary>
        HighTide = 1 << 0035, // 34359738368

        /// <summary>
        /// Counterattacks when physically attacked.
        /// </summary>
        Counter = 1 << 0036, // 68719476736

        /// <summary>
        /// You take damage in place of an ally.
        /// </summary>
        Cover = 1 << 0037, // 137438953472

        /// <summary>
        /// You take damage in place of a girl.
        /// </summary>
        ProtectGirls = 1 << 0038, // 274877906944

        /// <summary>
        /// Raises Counter activation rate.
        /// </summary>
        Eye4Eye = 1 << 0039, // 549755813888

        /// <summary>
        /// Prevents {b}Freeze{/b} and {b}Heat{/b}.
        /// </summary>
        BodyTemp = 1 << 0040, // 1099511627776

        /// <summary>
        /// Prevents back attacks.
        /// </summary>
        Alert = 1 << 0041, // 2199023255552

        /// <summary>
        /// Raises the chance of first strike.
        /// </summary>
        Initiative = 1 << 0042, // 4398046511104

        /// <summary>
        /// Characters level up faster.
        /// </summary>
        LevelUp = 1 << 0043, // 8796093022208

        /// <summary>
        /// Characters learn abilities faster.
        /// </summary>
        AbilityUp = 1 << 0044, // 17592186044416

        /// <summary>
        /// Receive more Gil after battle.
        /// </summary>
        Millionaire = 1 << 0045, // 35184372088832

        /// <summary>
        /// Receive Gil even when running from battle.
        /// </summary>
        FleeGil = 1 << 0046, // 70368744177664

        /// <summary>
        /// Mog protects with unseen forces.
        /// </summary>
        GuardianMog = 1 << 0047, // 140737488355328

        /// <summary>
        /// Prevents {b}Sleep{/b}.
        /// </summary>
        Insomniac = 1 << 0048, // 281474976710656

        /// <summary>
        /// Prevents {b}Poison{/b} and {b}Venom{/b}.
        /// </summary>
        Antibody = 1 << 0049, // 562949953421312

        /// <summary>
        /// Prevents {b}Darkness{/b}.
        /// </summary>
        BrightEyes = 1 << 0050, // 1125899906842624

        /// <summary>
        /// Prevents {b}Silence{/b}.
        /// </summary>
        Loudmouth = 1 << 0051, // 2251799813685248

        /// <summary>
        /// Restores HP automatically when {b}Near Death{/b}.
        /// </summary>
        RestoreHP = 1 << 0052, // 4503599627370496

        /// <summary>
        /// Prevents {b}Petrify{/b} and {b}Gradual Petrify{/b}.
        /// </summary>
        Jelly = 1 << 0053, // 9007199254740992

        /// <summary>
        /// Returns magic used by enemy.
        /// </summary>
        ReturnMagic = 1 << 0054, // 18014398509481984

        /// <summary>
        /// Absorbs MP used by enemy.
        /// </summary>
        AbsorbMP = 1 << 0055, // 36028797018963968

        /// <summary>
        /// Automatically uses {b}Potion{/b} when damaged.
        /// </summary>
        AutoPotion = 1 << 0056, // 72057594037927936

        /// <summary>
        /// Prevents {b}Stop{/b}.
        /// </summary>
        Locomotion = 1 << 0057, // 144115188075855872

        /// <summary>
        /// Prevents {b}Confusion{/b}.
        /// </summary>
        ClearHeaded = 1 << 0058, // 288230376151711744

        /// <summary>
        /// Raises strength of eidolons.
        /// </summary>
        Boost = 1 << 0059, // 576460752303423488

        /// <summary>
        /// Attacks with eidolon Odin.
        /// </summary>
        OdinSword = 1 << 0060, // 1152921504606846976

        /// <summary>
        /// Damages enemy when you Steal.
        /// </summary>
        Mug = 1 << 0061, // 2305843009213693952

        /// <summary>
        /// Raises success rate of Steal.
        /// </summary>
        Bandit = 1 << 0062, // 4611686018427387904

        /// <summary>
        /// Void
        /// </summary>
        Void = 1UL << 0063, // 9223372036854775808
    }
}
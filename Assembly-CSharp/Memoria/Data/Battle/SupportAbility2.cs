using System;

namespace Memoria.Data
{
    // C# bit shift loops such that (1u << (i + 32)) == (1u << i)
    [Flags]
    public enum SupportAbility2 : uint
    {
        /// <summary>
        /// Nullifies magic element.
        /// </summary>
        MagElemNull = 1u << SupportAbility.MagElemNull, // 1

        /// <summary>
        /// Raises the strength of spells.
        /// </summary>
        Concentrate = 1u << SupportAbility.Concentrate, // 2

        /// <summary>
        /// Cuts MP use by half in battle.
        /// </summary>
        HalfMP = 1u << SupportAbility.HalfMP, // 4

        /// <summary>
        /// Allows you to Trance faster.
        /// </summary>
        HighTide = 1u << SupportAbility.HighTide, // 8

        /// <summary>
        /// Counterattacks when physically attacked.
        /// </summary>
        Counter = 1u << SupportAbility.Counter, // 16

        /// <summary>
        /// You take damage in place of an ally.
        /// </summary>
        Cover = 1u << SupportAbility.Cover, // 32

        /// <summary>
        /// You take damage in place of a girl.
        /// </summary>
        ProtectGirls = 1u << SupportAbility.ProtectGirls, // 64

        /// <summary>
        /// Raises Counter activation rate.
        /// </summary>
        Eye4Eye = 1u << SupportAbility.Eye4Eye, // 128

        /// <summary>
        /// Prevents {b}Freeze{/b} and {b}Heat{/b}.
        /// </summary>
        BodyTemp = 1u << SupportAbility.BodyTemp, // 256

        /// <summary>
        /// Prevents back attacks.
        /// </summary>
        Alert = 1u << SupportAbility.Alert, // 512

        /// <summary>
        /// Raises the chance of first strike.
        /// </summary>
        Initiative = 1u << SupportAbility.Initiative, // 1024

        /// <summary>
        /// Characters level up faster.
        /// </summary>
        LevelUp = 1u << SupportAbility.LevelUp, // 2048

        /// <summary>
        /// Characters learn abilities faster.
        /// </summary>
        AbilityUp = 1u << SupportAbility.AbilityUp, // 4096

        /// <summary>
        /// Receive more Gil after battle.
        /// </summary>
        Millionaire = 1u << SupportAbility.Millionaire, // 8192

        /// <summary>
        /// Receive Gil even when running from battle.
        /// </summary>
        FleeGil = 1u << SupportAbility.FleeGil, // 16384

        /// <summary>
        /// Mog protects with unseen forces.
        /// </summary>
        GuardianMog = 1u << SupportAbility.GuardianMog, // 32768

        /// <summary>
        /// Prevents {b}Sleep{/b}.
        /// </summary>
        Insomniac = 1u << SupportAbility.Insomniac, // 65536

        /// <summary>
        /// Prevents {b}Poison{/b} and {b}Venom{/b}.
        /// </summary>
        Antibody = 1u << SupportAbility.Antibody, // 131072

        /// <summary>
        /// Prevents {b}Darkness{/b}.
        /// </summary>
        BrightEyes = 1u << SupportAbility.BrightEyes, // 262144

        /// <summary>
        /// Prevents {b}Silence{/b}.
        /// </summary>
        Loudmouth = 1u << SupportAbility.Loudmouth, // 524288

        /// <summary>
        /// Restores HP automatically when {b}Near Death{/b}.
        /// </summary>
        RestoreHP = 1u << SupportAbility.RestoreHP, // 1048576

        /// <summary>
        /// Prevents {b}Petrify{/b} and {b}Gradual Petrify{/b}.
        /// </summary>
        Jelly = 1u << SupportAbility.Jelly, // 2097152

        /// <summary>
        /// Returns magic used by enemy.
        /// </summary>
        ReturnMagic = 1u << SupportAbility.ReturnMagic, // 4194304

        /// <summary>
        /// Absorbs MP used by enemy.
        /// </summary>
        AbsorbMP = 1u << SupportAbility.AbsorbMP, // 8388608

        /// <summary>
        /// Automatically uses {b}Potion{/b} when damaged.
        /// </summary>
        AutoPotion = 1u << SupportAbility.AutoPotion, // 16777216

        /// <summary>
        /// Prevents {b}Stop{/b}.
        /// </summary>
        Locomotion = 1u << SupportAbility.Locomotion, // 33554432

        /// <summary>
        /// Prevents {b}Confusion{/b}.
        /// </summary>
        ClearHeaded = 1u << SupportAbility.ClearHeaded, // 67108864

        /// <summary>
        /// Raises strength of eidolons.
        /// </summary>
        Boost = 1u << SupportAbility.Boost, // 134217728

        /// <summary>
        /// Attacks with eidolon Odin.
        /// </summary268435456
        OdinSword = 1u << SupportAbility.OdinSword, // 268435456

        /// <summary>
        /// Damages enemy when you Steal.
        /// </summary>
        Mug = 1u << SupportAbility.Mug, // 536870912

        /// <summary>
        /// Raises success rate of Steal.
        /// </summary>
        Bandit = 1u << SupportAbility.Bandit, // 1073741824

        /// <summary>
        /// Void
        /// </summary>
        Void = 1u << SupportAbility.Void, // 2147483648
    }
}

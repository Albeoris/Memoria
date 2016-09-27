using System;

namespace Memoria
{
    [Flags]
    public enum SupportAbility2 : uint
    {
        /// <summary>
        /// Nullifies magic element.
        /// </summary>
        MagElemNull = (UInt32)(SupportAbility.MagElemNull >> 33), // 1

        /// <summary>
        /// Raises the strength of spells.
        /// </summary>
        Concentrate = (UInt32)(SupportAbility.Concentrate >> 33), // 2

        /// <summary>
        /// Cuts MP use by half in battle.
        /// </summary>
        HalfMP = (UInt32)(SupportAbility.HalfMP >> 33), // 4

        /// <summary>
        /// Allows you to Trance faster.
        /// </summary>
        HighTide = (UInt32)(SupportAbility.HighTide >> 33), // 8

        /// <summary>
        /// Counterattacks when physically attacked.
        /// </summary>
        Counter = (UInt32)(SupportAbility.Counter >> 33), // 16

        /// <summary>
        /// You take damage in place of an ally.
        /// </summary>
        Cover = (UInt32)(SupportAbility.Cover >> 33), // 32

        /// <summary>
        /// You take damage in place of a girl.
        /// </summary>
        ProtectGirls = (UInt32)(SupportAbility.ProtectGirls >> 33), // 64

        /// <summary>
        /// Raises Counter activation rate.
        /// </summary>
        Eye4Eye = (UInt32)(SupportAbility.Eye4Eye >> 33), // 128

        /// <summary>
        /// Prevents {b}Freeze{/b} and {b}Heat{/b}.
        /// </summary>
        BodyTemp = (UInt32)(SupportAbility.BodyTemp >> 33), // 256

        /// <summary>
        /// Prevents back attacks.
        /// </summary>
        Alert = (UInt32)(SupportAbility.Alert >> 33), // 512

        /// <summary>
        /// Raises the chance of first strike.
        /// </summary>
        Initiative = (UInt32)(SupportAbility.Initiative >> 33), // 1024

        /// <summary>
        /// Characters level up faster.
        /// </summary>
        LevelUp = (UInt32)(SupportAbility.LevelUp >> 33), // 2048

        /// <summary>
        /// Characters learn abilities faster.
        /// </summary>
        AbilityUp = (UInt32)(SupportAbility.AbilityUp >> 33), // 4096

        /// <summary>
        /// Receive more Gil after battle.
        /// </summary>
        Millionaire = (UInt32)(SupportAbility.Millionaire >> 33), // 8192

        /// <summary>
        /// Receive Gil even when running from battle.
        /// </summary>
        FleeGil = (UInt32)(SupportAbility.FleeGil >> 33), // 16384

        /// <summary>
        /// Mog protects with unseen forces.
        /// </summary>
        GuardianMog = (UInt32)(SupportAbility.GuardianMog >> 33), // 32768

        /// <summary>
        /// Prevents {b}Sleep{/b}.
        /// </summary>
        Insomniac = (UInt32)(SupportAbility.Insomniac >> 33), // 65536

        /// <summary>
        /// Prevents {b}Poison{/b} and {b}Venom{/b}.
        /// </summary>
        Antibody = (UInt32)(SupportAbility.Antibody >> 33), // 131072

        /// <summary>
        /// Prevents {b}Darkness{/b}.
        /// </summary>
        BrightEyes = (UInt32)(SupportAbility.BrightEyes >> 33), // 262144

        /// <summary>
        /// Prevents {b}Silence{/b}.
        /// </summary>
        Loudmouth = (UInt32)(SupportAbility.Loudmouth >> 33), // 524288

        /// <summary>
        /// Restores HP automatically when {b}Near Death{/b}.
        /// </summary>
        RestoreHP = (UInt32)(SupportAbility.RestoreHP >> 33), // 1048576

        /// <summary>
        /// Prevents {b}Petrify{/b} and {b}Gradual Petrify{/b}.
        /// </summary>
        Jelly = (UInt32)(SupportAbility.Jelly >> 33), // 2097152

        /// <summary>
        /// Returns magic used by enemy.
        /// </summary>
        ReturnMagic = (UInt32)(SupportAbility.ReturnMagic >> 33), // 4194304

        /// <summary>
        /// Absorbs MP used by enemy.
        /// </summary>
        AbsorbMP = (UInt32)(SupportAbility.AbsorbMP >> 33), // 8388608

        /// <summary>
        /// Automatically uses {b}Potion{/b} when damaged.
        /// </summary>
        AutoPotion = (UInt32)(SupportAbility.AutoPotion >> 33), // 16777216

        /// <summary>
        /// Prevents {b}Stop{/b}.
        /// </summary>
        Locomotion = (UInt32)(SupportAbility.Locomotion >> 33), // 33554432

        /// <summary>
        /// Prevents {b}Confusion{/b}.
        /// </summary>
        ClearHeaded = (UInt32)(SupportAbility.ClearHeaded >> 33), // 67108864

        /// <summary>
        /// Raises strength of eidolons.
        /// </summary>
        Boost = (UInt32)(SupportAbility.Boost >> 33), // 134217728

        /// <summary>
        /// Attacks with eidolon Odin.
        /// </summary268435456
        OdinSword = (UInt32)(SupportAbility.OdinSword >> 33), // 268435456

        /// <summary>
        /// Damages enemy when you Steal.
        /// </summary>
        Mug = (UInt32)(SupportAbility.Mug >> 33), // 536870912

        /// <summary>
        /// Raises success rate of Steal.
        /// </summary>
        Bandit = (UInt32)(SupportAbility.Bandit >> 33), // 1073741824

        /// <summary>
        /// Void
        /// </summary>
        Void = (UInt32)(SupportAbility.Void >> 33), // 2147483648
    }
}
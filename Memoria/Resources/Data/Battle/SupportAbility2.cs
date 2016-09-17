using System;

namespace Memoria
{
    [Flags]
    public enum SupportAbility2 : uint
    {
        /// <summary>
        /// Nullifies magic element.
        /// </summary>
        MagElemNull = (uint)(SupportAbility.MagElemNull >> 33), // 1

        /// <summary>
        /// Raises the strength of spells.
        /// </summary>
        Concentrate = (uint)(SupportAbility.Concentrate >> 33), // 2

        /// <summary>
        /// Cuts MP use by half in battle.
        /// </summary>
        HalfMP = (uint)(SupportAbility.HalfMP >> 33), // 4

        /// <summary>
        /// Allows you to Trance faster.
        /// </summary>
        HighTide = (uint)(SupportAbility.HighTide >> 33), // 8

        /// <summary>
        /// Counterattacks when physically attacked.
        /// </summary>
        Counter = (uint)(SupportAbility.Counter >> 33), // 16

        /// <summary>
        /// You take damage in place of an ally.
        /// </summary>
        Cover = (uint)(SupportAbility.Cover >> 33), // 32

        /// <summary>
        /// You take damage in place of a girl.
        /// </summary>
        ProtectGirls = (uint)(SupportAbility.ProtectGirls >> 33), // 64

        /// <summary>
        /// Raises Counter activation rate.
        /// </summary>
        Eye4Eye = (uint)(SupportAbility.Eye4Eye >> 33), // 128

        /// <summary>
        /// Prevents {b}Freeze{/b} and {b}Heat{/b}.
        /// </summary>
        BodyTemp = (uint)(SupportAbility.BodyTemp >> 33), // 256

        /// <summary>
        /// Prevents back attacks.
        /// </summary>
        Alert = (uint)(SupportAbility.Alert >> 33), // 512

        /// <summary>
        /// Raises the chance of first strike.
        /// </summary>
        Initiative = (uint)(SupportAbility.Initiative >> 33), // 1024

        /// <summary>
        /// Characters level up faster.
        /// </summary>
        LevelUp = (uint)(SupportAbility.LevelUp >> 33), // 2048

        /// <summary>
        /// Characters learn abilities faster.
        /// </summary>
        AbilityUp = (uint)(SupportAbility.AbilityUp >> 33), // 4096

        /// <summary>
        /// Receive more Gil after battle.
        /// </summary>
        Millionaire = (uint)(SupportAbility.Millionaire >> 33), // 8192

        /// <summary>
        /// Receive Gil even when running from battle.
        /// </summary>
        FleeGil = (uint)(SupportAbility.FleeGil >> 33), // 16384

        /// <summary>
        /// Mog protects with unseen forces.
        /// </summary>
        GuardianMog = (uint)(SupportAbility.GuardianMog >> 33), // 32768

        /// <summary>
        /// Prevents {b}Sleep{/b}.
        /// </summary>
        Insomniac = (uint)(SupportAbility.Insomniac >> 33), // 65536

        /// <summary>
        /// Prevents {b}Poison{/b} and {b}Venom{/b}.
        /// </summary>
        Antibody = (uint)(SupportAbility.Antibody >> 33), // 131072

        /// <summary>
        /// Prevents {b}Darkness{/b}.
        /// </summary>
        BrightEyes = (uint)(SupportAbility.BrightEyes >> 33), // 262144

        /// <summary>
        /// Prevents {b}Silence{/b}.
        /// </summary>
        Loudmouth = (uint)(SupportAbility.Loudmouth >> 33), // 524288

        /// <summary>
        /// Restores HP automatically when {b}Near Death{/b}.
        /// </summary>
        RestoreHP = (uint)(SupportAbility.RestoreHP >> 33), // 1048576

        /// <summary>
        /// Prevents {b}Petrify{/b} and {b}Gradual Petrify{/b}.
        /// </summary>
        Jelly = (uint)(SupportAbility.Jelly >> 33), // 2097152

        /// <summary>
        /// Returns magic used by enemy.
        /// </summary>
        ReturnMagic = (uint)(SupportAbility.ReturnMagic >> 33), // 4194304

        /// <summary>
        /// Absorbs MP used by enemy.
        /// </summary>
        AbsorbMP = (uint)(SupportAbility.AbsorbMP >> 33), // 8388608

        /// <summary>
        /// Automatically uses {b}Potion{/b} when damaged.
        /// </summary>
        AutoPotion = (uint)(SupportAbility.AutoPotion >> 33), // 16777216

        /// <summary>
        /// Prevents {b}Stop{/b}.
        /// </summary>
        Locomotion = (uint)(SupportAbility.Locomotion >> 33), // 33554432

        /// <summary>
        /// Prevents {b}Confusion{/b}.
        /// </summary>
        ClearHeaded = (uint)(SupportAbility.ClearHeaded >> 33), // 67108864

        /// <summary>
        /// Raises strength of eidolons.
        /// </summary>
        Boost = (uint)(SupportAbility.Boost >> 33), // 134217728

        /// <summary>
        /// Attacks with eidolon Odin.
        /// </summary268435456
        OdinSword = (uint)(SupportAbility.OdinSword >> 33), // 268435456

        /// <summary>
        /// Damages enemy when you Steal.
        /// </summary>
        Mug = (uint)(SupportAbility.Mug >> 33), // 536870912

        /// <summary>
        /// Raises success rate of Steal.
        /// </summary>
        Bandit = (uint)(SupportAbility.Bandit >> 33), // 1073741824

        /// <summary>
        /// Void
        /// </summary>
        Void = (uint)(SupportAbility.Void >> 33), // 2147483648
    }
}
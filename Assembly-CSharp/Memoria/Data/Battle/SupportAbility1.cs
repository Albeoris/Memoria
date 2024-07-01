using System;

namespace Memoria.Data
{
    [Flags]
    public enum SupportAbility1 : uint
    {
        /// <summary>
        /// Automatically casts {b}Reflect{/b} in battle.
        /// </summary>
        AutoReflect = 1u << SupportAbility.AutoReflect, // 1

        /// <summary>
        /// Automatically casts {b}Float{/b} in battle.
        /// </summary>
        AutoFloat = 1u << SupportAbility.AutoFloat, // 2

        /// <summary>
        /// Automatically casts {b}Haste{/b} in battle.
        /// </summary>
        AutoHaste = 1u << SupportAbility.AutoHaste, // 4

        /// <summary>
        /// Automatically casts {b}Regen{/b} in battle.
        /// </summary>
        AutoRegen = 1u << SupportAbility.AutoRegen, // 8

        /// <summary>
        /// Automatically casts {b}Life{/b} in battle.
        /// </summary>
        AutoLife = 1u << SupportAbility.AutoLife, // 16

        /// <summary>
        /// Increases HP by 10%.
        /// </summary>
        HP10 = 1u << SupportAbility.HP10, // 32

        /// <summary>
        /// Increases HP by 20%.
        /// </summary>
        HP20 = 1u << SupportAbility.HP20, // 64

        /// <summary>
        /// Increases MP by 10%.
        /// </summary>
        MP10 = 1u << SupportAbility.MP10, // 128

        /// <summary>
        /// Increases MP by 20%.
        /// </summary>
        MP20 = 1u << SupportAbility.MP20, // 256

        /// <summary>
        /// Raises physical attack accuracy.
        /// </summary>
        Accuracy = 1u << SupportAbility.Accuracy, // 512

        /// <summary>
        /// Lowers enemy’s physical attack accuracy.
        /// </summary>
        Distract = 1u << SupportAbility.Distract, // 1024

        /// <summary>
        /// Back row attacks like front row.
        /// </summary>
        LongReach = 1u << SupportAbility.LongReach, // 2048

        /// <summary>
        /// Uses own MP to raise {b}Attack Pwr{/b}.
        /// </summary>
        MPAttack = 1u << SupportAbility.MPAttack, // 4096

        /// <summary>
        /// Deals lethal damage to flying enemies.
        /// </summary>
        BirdKiller = 1u << SupportAbility.BirdKiller, // 8192

        /// <summary>
        /// Deals lethal damage to insects.
        /// </summary>
        BugKiller = 1u << SupportAbility.BugKiller, // 16384

        /// <summary>
        /// Deals lethal damage to stone enemies.
        /// </summary>
        StoneKiller = 1u << SupportAbility.StoneKiller, // 32768

        /// <summary>
        /// Deals lethal damage to undead enemies.
        /// </summary>
        UndeadKiller = 1u << SupportAbility.UndeadKiller, // 65536

        /// <summary>
        /// Deals lethal damage to dragons.
        /// </summary>
        DragonKiller = 1u << SupportAbility.DragonKiller, // 131072

        /// <summary>
        /// Deals lethal damage to demons.
        /// </summary>
        DevilKiller = 1u << SupportAbility.DevilKiller, // 262144

        /// <summary>
        /// Deals lethal damage to beasts.
        /// </summary>
        BeastKiller = 1u << SupportAbility.BeastKiller, // 524288

        /// <summary>
        /// Deals lethal damage to humans.
        /// </summary>
        ManEater = 1u << SupportAbility.ManEater, // 1048576

        /// <summary>
        /// Jump higher to raise jump attack power.
        /// </summary>
        HighJump = 1u << SupportAbility.HighJump, // 2097152

        /// <summary>
        /// Steal better items.
        /// </summary>
        MasterThief = 1u << SupportAbility.MasterThief, // 4194304

        /// <summary>
        /// Steal Gil along with items.
        /// </summary>
        StealGil = 1u << SupportAbility.StealGil, // 8388608

        /// <summary>
        /// Restores target’s HP.
        /// </summary>
        Healer = 1u << SupportAbility.Healer, // 16777216

        /// <summary>
        /// Adds weapon’s status effect (Add ST) when you Attack.
        /// </summary>
        AddStatus = 1u << SupportAbility.AddStatus, // 33554432

        /// <summary>
        /// Raises {b}Defence{/b} occasionally.
        /// </summary>
        GambleDefence = 1u << SupportAbility.GambleDefence, // 67108864

        /// <summary>
        /// Doubles the potency of medicinal items.
        /// </summary>
        Chemist = 1u << SupportAbility.Chemist, // 134217728

        /// <summary>
        /// Raises the strength of Throw.
        /// </summary>
        PowerThrow = 1u << SupportAbility.PowerThrow, // 268435456

        /// <summary>
        /// Raises the strength of Chakra.
        /// </summary>
        PowerUp = 1u << SupportAbility.PowerUp, // 536870912

        /// <summary>
        /// Nullifies {b}Reflect{/b} and attacks.
        /// </summary>
        ReflectNull = 1u << SupportAbility.ReflectNull, // 1073741824

        /// <summary>
        /// Doubles the strength of spells by using {b}Reflect{/b}.
        /// </summary>
        Reflectx2 = 1u << SupportAbility.Reflectx2, // 2147483648
    }
}

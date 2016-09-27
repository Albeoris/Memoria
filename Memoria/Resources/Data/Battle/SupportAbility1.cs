using System;

namespace Memoria
{
    [Flags]
    public enum SupportAbility1 : uint
    {
        /// <summary>
        /// Automatically casts {b}Reflect{/b} in battle.
        /// </summary>
        AutoReflect = (UInt32)SupportAbility.AutoReflect, // 1

        /// <summary>
        /// Automatically casts {b}Float{/b} in battle.
        /// </summary>
        AutoFloat = (UInt32)SupportAbility.AutoFloat, // 2

        /// <summary>
        /// Automatically casts {b}Haste{/b} in battle.
        /// </summary>
        AutoHaste = (UInt32)SupportAbility.AutoHaste, // 4

        /// <summary>
        /// Automatically casts {b}Regen{/b} in battle.
        /// </summary>
        AutoRegen = (UInt32)SupportAbility.AutoRegen, // 8

        /// <summary>
        /// Automatically casts {b}Life{/b} in battle.
        /// </summary>
        AutoLife = (UInt32)SupportAbility.AutoLife, // 16

        /// <summary>
        /// Increases HP by 10%.
        /// </summary>
        HP10 = (UInt32)SupportAbility.HP10, // 32

        /// <summary>
        /// Increases HP by 20%.
        /// </summary>
        HP20 = (UInt32)SupportAbility.HP20, // 64

        /// <summary>
        /// Increases MP by 10%.
        /// </summary>
        MP10 = (UInt32)SupportAbility.MP10, // 128

        /// <summary>
        /// Increases MP by 20%.
        /// </summary>
        MP20 = (UInt32)SupportAbility.MP20, // 256

        /// <summary>
        /// Raises physical attack accuracy.
        /// </summary>
        Accuracy = (UInt32)SupportAbility.Accuracy, // 512

        /// <summary>
        /// Lowers enemy’s physical attack accuracy.
        /// </summary>
        Distract = (UInt32)SupportAbility.Distract, // 1024

        /// <summary>
        /// Back row attacks like front row.
        /// </summary>
        LongReach = (UInt32)SupportAbility.LongReach, // 2048

        /// <summary>
        /// Uses own MP to raise {b}Attack Pwr{/b}.
        /// </summary>
        MPAttack = (UInt32)SupportAbility.MPAttack, // 4096

        /// <summary>
        /// Deals lethal damage to flying enemies.
        /// </summary>
        BirdKiller = (UInt32)SupportAbility.BirdKiller, // 8192

        /// <summary>
        /// Deals lethal damage to insects.
        /// </summary>
        BugKiller = (UInt32)SupportAbility.BugKiller, // 16384

        /// <summary>
        /// Deals lethal damage to stone enemies.
        /// </summary>
        StoneKiller = (UInt32)SupportAbility.StoneKiller, // 32768

        /// <summary>
        /// Deals lethal damage to undead enemies.
        /// </summary>
        UndeadKiller = (UInt32)SupportAbility.UndeadKiller, // 65536

        /// <summary>
        /// Deals lethal damage to dragons.
        /// </summary>
        DragonKiller = (UInt32)SupportAbility.DragonKiller, // 131072

        /// <summary>
        /// Deals lethal damage to demons.
        /// </summary>
        DevilKiller = (UInt32)SupportAbility.DevilKiller, // 262144

        /// <summary>
        /// Deals lethal damage to beasts.
        /// </summary>
        BeastKiller = (UInt32)SupportAbility.BeastKiller, // 524288

        /// <summary>
        /// Deals lethal damage to humans.
        /// </summary>
        ManEater = (UInt32)SupportAbility.ManEater, // 1048576

        /// <summary>
        /// Jump higher to raise jump attack power.
        /// </summary>
        HighJump = (UInt32)SupportAbility.HighJump, // 2097152

        /// <summary>
        /// Steal better items.
        /// </summary>
        MasterThief = (UInt32)SupportAbility.MasterThief, // 4194304

        /// <summary>
        /// Steal Gil along with items.
        /// </summary>
        StealGil = (UInt32)SupportAbility.StealGil, // 8388608

        /// <summary>
        /// Restores target’s HP.
        /// </summary>
        Healer = (UInt32)SupportAbility.Healer, // 16777216

        /// <summary>
        /// Adds weapon’s status effect (Add ST) when you Attack.
        /// </summary>
        AddStatus = (UInt32)SupportAbility.AddStatus, // 33554432

        /// <summary>
        /// Raises {b}Defence{/b} occasionally.
        /// </summary>
        GambleDefence = (UInt32)SupportAbility.GambleDefence, // 67108864

        /// <summary>
        /// Doubles the potency of medicinal items.
        /// </summary>
        Chemist = (UInt32)SupportAbility.Chemist, // 134217728

        /// <summary>
        /// Raises the strength of Throw.
        /// </summary>
        PowerThrow = (UInt32)SupportAbility.PowerThrow, // 268435456
        
        /// <summary>
        /// Raises the strength of Chakra.
        /// </summary>
        PowerUp = (UInt32)SupportAbility.PowerUp, // 536870912

        /// <summary>
        /// Nullifies {b}Reflect{/b} and attacks.
        /// </summary>
        ReflectNull = (UInt32)SupportAbility.ReflectNull, // 1073741824

        /// <summary>
        /// Doubles the strength of spells by using {b}Reflect{/b}.
        /// </summary>
        Reflectx2 = (UInt32)SupportAbility.Reflectx2, // 2147483648
    }
}
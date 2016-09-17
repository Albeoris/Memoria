using System;

namespace Memoria
{
    [Flags]
    public enum SupportAbility1 : uint
    {
        /// <summary>
        /// Automatically casts {b}Reflect{/b} in battle.
        /// </summary>
        AutoReflect = (uint)SupportAbility.AutoReflect, // 1

        /// <summary>
        /// Automatically casts {b}Float{/b} in battle.
        /// </summary>
        AutoFloat = (uint)SupportAbility.AutoFloat, // 2

        /// <summary>
        /// Automatically casts {b}Haste{/b} in battle.
        /// </summary>
        AutoHaste = (uint)SupportAbility.AutoHaste, // 4

        /// <summary>
        /// Automatically casts {b}Regen{/b} in battle.
        /// </summary>
        AutoRegen = (uint)SupportAbility.AutoRegen, // 8

        /// <summary>
        /// Automatically casts {b}Life{/b} in battle.
        /// </summary>
        AutoLife = (uint)SupportAbility.AutoLife, // 16

        /// <summary>
        /// Increases HP by 10%.
        /// </summary>
        HP10 = (uint)SupportAbility.HP10, // 32

        /// <summary>
        /// Increases HP by 20%.
        /// </summary>
        HP20 = (uint)SupportAbility.HP20, // 64

        /// <summary>
        /// Increases MP by 10%.
        /// </summary>
        MP10 = (uint)SupportAbility.MP10, // 128

        /// <summary>
        /// Increases MP by 20%.
        /// </summary>
        MP20 = (uint)SupportAbility.MP20, // 256

        /// <summary>
        /// Raises physical attack accuracy.
        /// </summary>
        Accuracy = (uint)SupportAbility.Accuracy, // 512

        /// <summary>
        /// Lowers enemy’s physical attack accuracy.
        /// </summary>
        Distract = (uint)SupportAbility.Distract, // 1024

        /// <summary>
        /// Back row attacks like front row.
        /// </summary>
        LongReach = (uint)SupportAbility.LongReach, // 2048

        /// <summary>
        /// Uses own MP to raise {b}Attack Pwr{/b}.
        /// </summary>
        MPAttack = (uint)SupportAbility.MPAttack, // 4096

        /// <summary>
        /// Deals lethal damage to flying enemies.
        /// </summary>
        BirdKiller = (uint)SupportAbility.BirdKiller, // 8192

        /// <summary>
        /// Deals lethal damage to insects.
        /// </summary>
        BugKiller = (uint)SupportAbility.BugKiller, // 16384

        /// <summary>
        /// Deals lethal damage to stone enemies.
        /// </summary>
        StoneKiller = (uint)SupportAbility.StoneKiller, // 32768

        /// <summary>
        /// Deals lethal damage to undead enemies.
        /// </summary>
        UndeadKiller = (uint)SupportAbility.UndeadKiller, // 65536

        /// <summary>
        /// Deals lethal damage to dragons.
        /// </summary>
        DragonKiller = (uint)SupportAbility.DragonKiller, // 131072

        /// <summary>
        /// Deals lethal damage to demons.
        /// </summary>
        DevilKiller = (uint)SupportAbility.DevilKiller, // 262144

        /// <summary>
        /// Deals lethal damage to beasts.
        /// </summary>
        BeastKiller = (uint)SupportAbility.BeastKiller, // 524288

        /// <summary>
        /// Deals lethal damage to humans.
        /// </summary>
        ManEater = (uint)SupportAbility.ManEater, // 1048576

        /// <summary>
        /// Jump higher to raise jump attack power.
        /// </summary>
        HighJump = (uint)SupportAbility.HighJump, // 2097152

        /// <summary>
        /// Steal better items.
        /// </summary>
        MasterThief = (uint)SupportAbility.MasterThief, // 4194304

        /// <summary>
        /// Steal Gil along with items.
        /// </summary>
        StealGil = (uint)SupportAbility.StealGil, // 8388608

        /// <summary>
        /// Restores target’s HP.
        /// </summary>
        Healer = (uint)SupportAbility.Healer, // 16777216

        /// <summary>
        /// Adds weapon’s status effect (Add ST) when you Attack.
        /// </summary>
        AddStatus = (uint)SupportAbility.AddStatus, // 33554432

        /// <summary>
        /// Raises {b}Defence{/b} occasionally.
        /// </summary>
        GambleDefence = (uint)SupportAbility.GambleDefence, // 67108864

        /// <summary>
        /// Doubles the potency of medicinal items.
        /// </summary>
        Chemist = (uint)SupportAbility.Chemist, // 134217728

        /// <summary>
        /// Raises the strength of Throw.
        /// </summary>
        PowerThrow = (uint)SupportAbility.PowerThrow, // 268435456
        
        /// <summary>
        /// Raises the strength of Chakra.
        /// </summary>
        PowerUp = (uint)SupportAbility.PowerUp, // 536870912

        /// <summary>
        /// Nullifies {b}Reflect{/b} and attacks.
        /// </summary>
        ReflectNull = (uint)SupportAbility.ReflectNull, // 1073741824

        /// <summary>
        /// Doubles the strength of spells by using {b}Reflect{/b}.
        /// </summary>
        Reflectx2 = (uint)SupportAbility.Reflectx2, // 2147483648
    }
}
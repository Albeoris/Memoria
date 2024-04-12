namespace Memoria.Data
{
	public enum SupportAbility : int
	{
		/// <summary>
		/// Automatically casts {b}Reflect{/b} in battle.
		/// </summary>
		AutoReflect,

		/// <summary>
		/// Automatically casts {b}Float{/b} in battle.
		/// </summary>
		AutoFloat,

		/// <summary>
		/// Automatically casts {b}Haste{/b} in battle.
		/// </summary>
		AutoHaste,

		/// <summary>
		/// Automatically casts {b}Regen{/b} in battle.
		/// </summary>
		AutoRegen,

		/// <summary>
		/// Automatically casts {b}Life{/b} in battle.
		/// </summary>
		AutoLife,

		/// <summary>
		/// Increases HP by 10%.
		/// </summary>
		HP10,

		/// <summary>
		/// Increases HP by 20%.
		/// </summary>
		HP20,

		/// <summary>
		/// Increases MP by 10%.
		/// </summary>
		MP10,

		/// <summary>
		/// Increases MP by 20%.
		/// </summary>
		MP20,

		/// <summary>
		/// Raises physical attack accuracy.
		/// </summary>
		Accuracy,

		/// <summary>
		/// Lowers enemy’s physical attack accuracy.
		/// </summary>
		Distract,

		/// <summary>
		/// Back row attacks like front row.
		/// </summary>
		LongReach,

		/// <summary>
		/// Uses own MP to raise {b}Attack Pwr{/b}.
		/// </summary>
		MPAttack,

		/// <summary>
		/// Deals lethal damage to flying enemies.
		/// </summary>
		BirdKiller,

		/// <summary>
		/// Deals lethal damage to insects.
		/// </summary>
		BugKiller,

		/// <summary>
		/// Deals lethal damage to stone enemies.
		/// </summary>
		StoneKiller,

		/// <summary>
		/// Deals lethal damage to undead enemies.
		/// </summary>
		UndeadKiller,

		/// <summary>
		/// Deals lethal damage to dragons.
		/// </summary>
		DragonKiller,

		/// <summary>
		/// Deals lethal damage to demons.
		/// </summary>
		DevilKiller,

		/// <summary>
		/// Deals lethal damage to beasts.
		/// </summary>
		BeastKiller,

		/// <summary>
		/// Deals lethal damage to humans.
		/// </summary>
		ManEater,

		/// <summary>
		/// Jump higher to raise jump attack power.
		/// </summary>
		HighJump,

		/// <summary>
		/// Steal better items.
		/// </summary>
		MasterThief,

		/// <summary>
		/// Steal Gil along with items.
		/// </summary>
		StealGil,

		/// <summary>
		/// Restores target’s HP.
		/// </summary>
		Healer,

		/// <summary>
		/// Adds weapon’s status effect (Add ST) when you Attack.
		/// </summary>
		AddStatus,

		/// <summary>
		/// Raises {b}Defence{/b} occasionally.
		/// </summary>
		GambleDefence,

		/// <summary>
		/// Doubles the potency of medicinal items.
		/// </summary>
		Chemist,

		/// <summary>
		/// Raises the strength of Throw.
		/// </summary>
		PowerThrow,

		/// <summary>
		/// Raises the strength of Chakra.
		/// </summary>
		PowerUp,

		/// <summary>
		/// Nullifies {b}Reflect{/b} and attacks.
		/// </summary>
		ReflectNull,

		/// <summary>
		/// Doubles the strength of spells by using {b}Reflect{/b}.
		/// </summary>
		Reflectx2,

		/// <summary>
		/// Nullifies magic element.
		/// </summary>
		MagElemNull,

		/// <summary>
		/// Raises the strength of spells.
		/// </summary>
		Concentrate,

		/// <summary>
		/// Cuts MP use by half in battle.
		/// </summary>
		HalfMP,

		/// <summary>
		/// Allows you to Trance faster.
		/// </summary>
		HighTide,

		/// <summary>
		/// Counterattacks when physically attacked.
		/// </summary>
		Counter,

		/// <summary>
		/// You take damage in place of an ally.
		/// </summary>
		Cover,

		/// <summary>
		/// You take damage in place of a girl.
		/// </summary>
		ProtectGirls,

		/// <summary>
		/// Raises Counter activation rate.
		/// </summary>
		Eye4Eye,

		/// <summary>
		/// Prevents {b}Freeze{/b} and {b}Heat{/b}.
		/// </summary>
		BodyTemp,

		/// <summary>
		/// Prevents back attacks.
		/// </summary>
		Alert,

		/// <summary>
		/// Raises the chance of first strike.
		/// </summary>
		Initiative,

		/// <summary>
		/// Characters level up faster.
		/// </summary>
		LevelUp,

		/// <summary>
		/// Characters learn abilities faster.
		/// </summary>
		AbilityUp,

		/// <summary>
		/// Receive more Gil after battle.
		/// </summary>
		Millionaire,

		/// <summary>
		/// Receive Gil even when running from battle.
		/// </summary>
		FleeGil,

		/// <summary>
		/// Mog protects with unseen forces.
		/// </summary>
		GuardianMog,

		/// <summary>
		/// Prevents {b}Sleep{/b}.
		/// </summary>
		Insomniac,

		/// <summary>
		/// Prevents {b}Poison{/b} and {b}Venom{/b}.
		/// </summary>
		Antibody,

		/// <summary>
		/// Prevents {b}Darkness{/b}.
		/// </summary>
		BrightEyes,

		/// <summary>
		/// Prevents {b}Silence{/b}.
		/// </summary>
		Loudmouth,

		/// <summary>
		/// Restores HP automatically when {b}Near Death{/b}.
		/// </summary>
		RestoreHP,

		/// <summary>
		/// Prevents {b}Petrify{/b} and {b}Gradual Petrify{/b}.
		/// </summary>
		Jelly,

		/// <summary>
		/// Returns magic used by enemy.
		/// </summary>
		ReturnMagic,

		/// <summary>
		/// Absorbs MP used by enemy.
		/// </summary>
		AbsorbMP,

		/// <summary>
		/// Automatically uses {b}Potion{/b} when damaged.
		/// </summary>
		AutoPotion,

		/// <summary>
		/// Prevents {b}Stop{/b}.
		/// </summary>
		Locomotion,

		/// <summary>
		/// Prevents {b}Confusion{/b}.
		/// </summary>
		ClearHeaded,

		/// <summary>
		/// Raises strength of eidolons.
		/// </summary>
		Boost,

		/// <summary>
		/// Attacks with eidolon Odin.
		/// </summary>
		OdinSword,

		/// <summary>
		/// Damages enemy when you Steal.
		/// </summary>
		Mug,

		/// <summary>
		/// Raises success rate of Steal.
		/// </summary>
		Bandit,

		/// <summary>
		/// Void
		/// </summary>
		Void,
	}
}

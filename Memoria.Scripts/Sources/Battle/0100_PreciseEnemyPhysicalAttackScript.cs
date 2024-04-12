using System;

namespace Memoria.Scripts.Battle
{
	/// <summary>
	/// Aerial Slash, Whirlwind, Flame Slash, Fire Blades, Jet Fire, Virus Crunch, Psychokinesis, Curse, Sandstorm, High Wind, Virus Fly, !!!, Leaf Swirl, Sweep, Fin, Boomerang, Paper Storm, Spin, Shockwave, Cleave, Raining Swords, Neutron Ring
	/// </summary>
	[BattleScript(Id)]
	public sealed class PreciseEnemyPhysicalAttackScript : IBattleScript
	{
		public const Int32 Id = 0100;

		private readonly BattleCalculator _v;

		public PreciseEnemyPhysicalAttackScript(BattleCalculator v)
		{
			_v = v;
		}

		public void Perform()
		{
			if (_v.Target.TryKillFrozen())
				return;

			_v.NormalPhysicalParams();
			_v.Caster.EnemyTranceBonusAttack();
			_v.Caster.PhysicalPenaltyAndBonusAttack();
			_v.Target.PhysicalPenaltyAndBonusAttack();
			_v.BonusBackstabAndPenaltyLongDistance();
			_v.BonusElement();
			if (!_v.CanAttackElementalCommand())
				return;

			_v.CalcHpDamage();
			_v.TryAlterMagicStatuses();
		}
	}
}

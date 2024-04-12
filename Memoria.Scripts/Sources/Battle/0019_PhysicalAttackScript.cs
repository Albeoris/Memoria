using System;

namespace Memoria.Scripts.Battle
{
	/// <summary>
	/// Free Energy, Tidal Flame, Scoop Art, Shift Break, Stellar Circle 5, Meo Twister, Solution 9, Grand Lethal, No Mercy, Stock Break, Shock
	/// </summary>
	[BattleScript(Id)]
	public sealed class PhysicalAttackScript : IBattleScript
	{
		public const Int32 Id = 0019;

		private readonly BattleCalculator _v;

		public PhysicalAttackScript(BattleCalculator v)
		{
			_v = v;
		}

		public void Perform()
		{
			_v.WeaponPhysicalParams();
			_v.Caster.EnemyTranceBonusAttack();
			_v.Caster.PhysicalPenaltyAndBonusAttack();
			_v.Target.PhysicalPenaltyAndBonusAttack();
			_v.BonusElement();
			if (_v.CanAttackWeaponElementalCommand())
			{
				_v.CalcHpDamage();
				_v.TryAlterMagicStatuses();
			}
		}
	}
}

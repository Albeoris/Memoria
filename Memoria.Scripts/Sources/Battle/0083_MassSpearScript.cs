using System;

namespace Memoria.Scripts.Battle
{
	/// <summary>
	/// Spear (mass)
	/// </summary>
	[BattleScript(Id)]
	public sealed class MassSpearScript : IBattleScript
	{
		public const Int32 Id = 0083;

		private readonly BattleCalculator _v;

		public MassSpearScript(BattleCalculator v)
		{
			_v = v;
		}

		public void Perform()
		{
			_v.Target.Flags |= CalcFlag.HpAlteration;

			_v.WeaponPhysicalParams(CalcAttackBonus.Random);
			_v.Context.Attack = _v.Context.Attack * 3 >> 1;

			_v.Caster.PenaltyMini();
			_v.Target.PhysicalPenaltyAndBonusAttack();

			if (_v.Context.Attack < 4)
				_v.Context.Attack = 4;
			_v.CalcHpDamage();
			_v.Target.HpDamage /= BattleState.TargetCount(false);
		}
	}
}

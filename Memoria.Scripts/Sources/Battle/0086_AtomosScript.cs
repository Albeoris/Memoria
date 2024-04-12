using Memoria.Data;
using System;

namespace Memoria.Scripts.Battle
{
	/// <summary>
	/// Atomos
	/// </summary>
	[BattleScript(Id)]
	public sealed class AtomosScript : IBattleScript
	{
		public const Int32 Id = 0086;

		private readonly BattleCalculator _v;

		public AtomosScript(BattleCalculator v)
		{
			_v = v;
		}

		public void Perform()
		{
			if (!_v.Target.CheckUnsafetyOrMiss())
				return;

			_v.SetCommandAttack();
			_v.BonusElement();
			if (!_v.CanAttackMagic())
				return;

			_v.Context.Attack += ff9item.FF9Item_GetCount(RegularItem.Amethyst);
			_v.CalcProportionDamage();
		}
	}
}

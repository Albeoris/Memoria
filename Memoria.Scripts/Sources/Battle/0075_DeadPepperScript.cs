using System;

namespace Memoria.Scripts.Battle
{
	/// <summary>
	/// Dead Pepper
	/// </summary>
	[BattleScript(Id)]
	public sealed class DeadPepperScript : IBattleScript
	{
		public const Int32 Id = 0075;

		private readonly BattleCalculator _v;

		public DeadPepperScript(BattleCalculator v)
		{
			_v = v;
		}

		public void Perform()
		{
			_v.Target.Flags |= CalcFlag.HpAlteration;
			_v.Target.HpDamage = _v.Command.Item.Power * (ff9item.FF9Item_GetCount(_v.Command.ItemId) + 1);
		}
	}
}

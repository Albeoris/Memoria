using Memoria.Data;
using System;

namespace Memoria.Scripts.Battle
{
	/// <summary>
	/// Iai Strike
	/// </summary>
	[BattleScript(Id)]
	public sealed class MiniScript : IBattleScript
	{
		public const Int32 Id = 0109;

		private readonly BattleCalculator _v;

		public MiniScript(BattleCalculator v)
		{
			_v = v;
		}

		public void Perform()
		{
			if (_v.Target.IsUnderAnyStatus(BattleStatus.Mini))
			{
				_v.TryAlterCommandStatuses();
				return;
			}

			_v.MagicAccuracy();
			_v.Target.PenaltyShellHitRate();
			_v.PenaltyCommandDividedHitRate();
			if (_v.TryMagicHit())
				_v.TryAlterCommandStatuses();
		}
	}
}

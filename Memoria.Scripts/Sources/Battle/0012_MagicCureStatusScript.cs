using Memoria.Data;
using System;

namespace Memoria.Scripts.Battle
{
	/// <summary>
	/// Panacea, Stona, Esuna, Dispel
	/// </summary>
	[BattleScript(Id)]
	public sealed class MagicCureStatusScript : IBattleScript, IEstimateBattleScript
	{
		public const Int32 Id = 0012;

		private readonly BattleCalculator _v;

		public MagicCureStatusScript(BattleCalculator v)
		{
			_v = v;
		}

		public void Perform()
		{
			_v.TryRemoveAbilityStatuses();
		}

		public Single RateTarget()
		{
			BattleStatus playerStatus = _v.Target.CurrentStatus;
			BattleStatus removeStatus = _v.Command.AbilityStatus;
			BattleStatus removedStatus = playerStatus & removeStatus;
			Int32 rating = BattleScriptStatusEstimate.RateStatuses(removedStatus);

			if (_v.Target.IsPlayer)
				return -1 * rating;

			return rating;
		}
	}
}

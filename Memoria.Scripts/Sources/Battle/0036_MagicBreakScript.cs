using Memoria.Data;
using System;

namespace Memoria.Scripts.Battle
{
	/// <summary>
	/// Magic Break
	/// </summary>
	[BattleScript(Id)]
	public sealed class MagicBreakScript : IBattleScript, IEstimateBattleScript
	{
		public const Int32 Id = 0036;

		private readonly BattleCalculator _v;

		public MagicBreakScript(BattleCalculator v)
		{
			_v = v;
		}

		public void Perform()
		{
			_v.MagicAccuracy();
			_v.Target.PenaltyShellHitRate();
			if (_v.TryMagicHit())
				_v.Target.Magic = (Byte)(_v.Target.Magic * 3 / 4);
		}

		public Single RateTarget()
		{
			Int32 magicDiff = _v.Target.Magic - _v.Target.Magic * 3 / 4;

			Single result = magicDiff * BattleScriptAccuracyEstimate.RatePlayerAttackEvade(_v.Context.Evade);

			if (_v.Target.IsUnderAnyStatus(BattleStatus.Shell))
				result *= BattleScriptAccuracyEstimate.RatePlayerAttackHit(_v.Context.HitRate >> 1);
			else
				result *= BattleScriptAccuracyEstimate.RatePlayerAttackHit(_v.Context.HitRate);

			if (_v.Target.IsPlayer)
				result *= -1;

			return result;
		}
	}
}

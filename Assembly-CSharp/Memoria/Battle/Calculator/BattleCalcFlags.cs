using System;

namespace Memoria
{
	[Flags]
	public enum BattleCalcFlags : ushort
	{
		Miss = 1,
		Dodge = 2,
		MpAttack = 4,
		Absorb = 8,
		TrueFB = 16,
		FalseFB = 32,
		Guard = 64,
		DirectHP = 128,
		AddStat = 256
	}
}

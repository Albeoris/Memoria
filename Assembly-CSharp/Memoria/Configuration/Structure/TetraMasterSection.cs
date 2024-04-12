using Memoria.Prime.Ini;
using System;
using System.Collections.Generic;

namespace Memoria
{
	public sealed partial class Configuration
	{
		private sealed class TetraMasterSection : IniSection
		{
			private static readonly HashSet<Int32> DefaultExclusions = new HashSet<Int32>(new[] { 56, 75, 76, 77, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 98, 99, 100 });

			public readonly IniValue<Int32> TripleTriad;
			public readonly IniValue<Int32> ReduceRandom;
			public readonly IniValue<Int32> MaxCardCount;

			public readonly IniValue<Boolean> DiscardAutoButton;
			public readonly IniValue<Boolean> DiscardAssaultCards;
			public readonly IniValue<Boolean> DiscardFlexibleCards;
			public readonly IniValue<Int32> DiscardMaxAttack;
			public readonly IniValue<Int32> DiscardMaxPDef;
			public readonly IniValue<Int32> DiscardMaxMDef;
			public readonly IniValue<Int32> DiscardMaxSum;
			public readonly IniValue<Int32> DiscardMinDeckSize;
			public readonly IniValue<Int32> DiscardKeepSameType;
			public readonly IniValue<Int32> DiscardKeepSameArrow;
			public readonly IniSet<Int32> DiscardExclusions;

			public TetraMasterSection() : base(nameof(TetraMasterSection), true)
			{
				TripleTriad = BindInt32(nameof(TripleTriad), 0);
				ReduceRandom = BindInt32(nameof(ReduceRandom), 0);
				MaxCardCount = BindInt32(nameof(MaxCardCount), 100);

				DiscardAutoButton = BindBoolean(nameof(DiscardAutoButton), true);
				DiscardAssaultCards = BindBoolean(nameof(DiscardAssaultCards), false);
				DiscardFlexibleCards = BindBoolean(nameof(DiscardFlexibleCards), true);
				DiscardMaxAttack = BindInt32(nameof(DiscardMaxAttack), 224);
				DiscardMaxPDef = BindInt32(nameof(DiscardMaxPDef), 255);
				DiscardMaxMDef = BindInt32(nameof(DiscardMaxMDef), 255);
				DiscardMaxSum = BindInt32(nameof(DiscardMaxSum), 480);
				DiscardMinDeckSize = BindInt32(nameof(DiscardMinDeckSize), 10);
				DiscardKeepSameType = BindInt32(nameof(DiscardKeepSameType), 1);
				DiscardKeepSameArrow = BindInt32(nameof(DiscardKeepSameArrow), 0);
				DiscardExclusions = BindInt32Set(nameof(DiscardExclusions), DefaultExclusions);
			}
		}
	}
}

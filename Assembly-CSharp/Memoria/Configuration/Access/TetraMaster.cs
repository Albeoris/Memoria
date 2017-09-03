using System;
using System.Collections.Generic;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class TetraMaster
        {
            public static Boolean Enabled => Instance._tetraMaster.Enabled.Value;
            public static Boolean IsEasyWin => Enabled && Instance._tetraMaster.ReduceRandom.Value == 2;
            public static Boolean IsReduceRandom => Enabled && Instance._tetraMaster.ReduceRandom.Value == 1;

            public static Boolean DiscardAutoButton => Enabled && Instance._tetraMaster.DiscardAutoButton.Value;
            public static Boolean DiscardAssaultCards => Instance._tetraMaster.DiscardAssaultCards.Value;
            public static Boolean DiscardFlexibleCards => Instance._tetraMaster.DiscardFlexibleCards.Value;
            public static Int32 DiscardMaxAttack => Instance._tetraMaster.DiscardMaxAttack.Value;
            public static Int32 DiscardMaxPDef => Instance._tetraMaster.DiscardMaxPDef.Value;
            public static Int32 DiscardMaxMDef => Instance._tetraMaster.DiscardMaxMDef.Value;
            public static Int32 DiscardMaxSum => Instance._tetraMaster.DiscardMaxSum.Value;
            public static Int32 DiscardMinDeckSize => Instance._tetraMaster.DiscardMinDeckSize.Value;
            public static Int32 DiscardKeepSameType => Instance._tetraMaster.DiscardKeepSameType.Value;
            public static Int32 DiscardKeepSameArrow => Instance._tetraMaster.DiscardKeepSameArrow.Value;
            public static HashSet<Int32> DiscardExclusions => Instance._tetraMaster.DiscardExclusions.Value;
        }
    }
}
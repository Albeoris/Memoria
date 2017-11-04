using System;
using System.Collections.Generic;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class TetraMaster
        {
            public static Boolean IsEasyWin => Instance._tetraMaster.ReduceRandom == 2;
            public static Boolean IsReduceRandom => Instance._tetraMaster.ReduceRandom == 1;

            public static Boolean DiscardAutoButton => Instance._tetraMaster.DiscardAutoButton;
            public static Boolean DiscardAssaultCards => Instance._tetraMaster.DiscardAssaultCards;
            public static Boolean DiscardFlexibleCards => Instance._tetraMaster.DiscardFlexibleCards;
            public static Int32 DiscardMaxAttack => Instance._tetraMaster.DiscardMaxAttack;
            public static Int32 DiscardMaxPDef => Instance._tetraMaster.DiscardMaxPDef;
            public static Int32 DiscardMaxMDef => Instance._tetraMaster.DiscardMaxMDef;
            public static Int32 DiscardMaxSum => Instance._tetraMaster.DiscardMaxSum;
            public static Int32 DiscardMinDeckSize => Instance._tetraMaster.DiscardMinDeckSize;
            public static Int32 DiscardKeepSameType => Instance._tetraMaster.DiscardKeepSameType;
            public static Int32 DiscardKeepSameArrow => Instance._tetraMaster.DiscardKeepSameArrow;
            public static HashSet<Int32> DiscardExclusions => Instance._tetraMaster.DiscardExclusions;
        }
    }
}
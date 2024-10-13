using System;
using System.Collections.Generic;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class TetraMaster
        {
            public static Boolean IsEnabled => Instance._tetraMaster.Enabled;
            public static Int32 TripleTriad => Instance._tetraMaster.TripleTriad;
            public static Boolean EasyWin => Instance._tetraMaster.EasyWin;
            public static Boolean ReduceRandom => Instance._tetraMaster.ReduceRandom;
            public static Int32 MaxCardCount => Instance._tetraMaster.MaxCardCount;

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

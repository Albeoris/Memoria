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
            public static Boolean IsEasyWin => IsEnabled && Instance._tetraMaster.ReduceRandom == 2;
            public static Boolean IsReduceRandom => IsEnabled && Instance._tetraMaster.ReduceRandom == 1;
            public static Int32 MaxCardCount => Instance._tetraMaster.MaxCardCount;
            public static Boolean ShowEnemyDeck => Instance._tetraMaster.ShowEnemyDeck;

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
            public static String ValueProbabilityCards => Instance._tetraMaster.ValueProbabilityCards;
            public static String FormulaProbabilityCards => Instance._tetraMaster.FormulaProbabilityCards;
            public static Int32 PreventDuplicateCard => Instance._tetraMaster.PreventDuplicateCard;
            public static HashSet<Int32> UniqueCard => Instance._tetraMaster.UniqueCard;
        }
    }
}
using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetRandomBattles
    /// Define random battles.
    /// 
    /// 1st argument: pattern, deciding the encounter chances and the topography (World Map only).
    ///  0: {0.375, 0.28, 0.22, 0.125}
    ///  1: {0.25, 0.25, 0.25, 0.25}
    ///  2: {0.35, 0.3, 0.3, 0.05}
    ///  3: {0.45, 0.4, 0.1, 0.05}
    /// 2nd to 5th arguments: possible random battles.
    /// AT_USPIN Pattern (1 bytes)
    /// AT_BATTLE Battle 1 (2 bytes)
    /// AT_BATTLE Battle 2 (2 bytes)
    /// AT_BATTLE Battle 3 (2 bytes)
    /// AT_BATTLE Battle 4 (2 bytes)
    /// ENCSCENE = 0x03C,
    /// </summary>
    internal sealed class ENCSCENE : JsmInstruction
    {
        private readonly IJsmExpression _pattern;

        private readonly IJsmExpression _battle1;

        private readonly IJsmExpression _battle2;

        private readonly IJsmExpression _battle3;

        private readonly IJsmExpression _battle4;

        private ENCSCENE(IJsmExpression pattern, IJsmExpression battle1, IJsmExpression battle2, IJsmExpression battle3, IJsmExpression battle4)
        {
            _pattern = pattern;
            _battle1 = battle1;
            _battle2 = battle2;
            _battle3 = battle3;
            _battle4 = battle4;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression pattern = reader.ArgumentByte();
            IJsmExpression battle1 = reader.ArgumentInt16();
            IJsmExpression battle2 = reader.ArgumentInt16();
            IJsmExpression battle3 = reader.ArgumentInt16();
            IJsmExpression battle4 = reader.ArgumentInt16();
            return new ENCSCENE(pattern, battle1, battle2, battle3, battle4);
        }
        public override String ToString()
        {
            return $"{nameof(ENCSCENE)}({nameof(_pattern)}: {_pattern}, {nameof(_battle1)}: {_battle1}, {nameof(_battle2)}: {_battle2}, {nameof(_battle3)}: {_battle3}, {nameof(_battle4)}: {_battle4})";
        }
    }
}
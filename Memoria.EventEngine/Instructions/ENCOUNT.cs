using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// Battle
    /// Start a battle (using a random enemy group).
    /// 
    /// 1st argument: rush type (unknown).
    /// 2nd argument: gathered battle and Steiner's state (highest bit) informations.
    /// AT_SPIN Rush Type (1 bytes)
    /// AT_BATTLE Battle (2 bytes)
    /// ENCOUNT = 0x02A,
    /// </summary>
    internal sealed class ENCOUNT : JsmInstruction
    {
        private readonly IJsmExpression _rushType;

        private readonly IJsmExpression _battle;

        private ENCOUNT(IJsmExpression rushType, IJsmExpression battle)
        {
            _rushType = rushType;
            _battle = battle;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression rushType = reader.ArgumentByte();
            IJsmExpression battle = reader.ArgumentInt16();
            return new ENCOUNT(rushType, battle);
        }
        public override String ToString()
        {
            return $"{nameof(ENCOUNT)}({nameof(_rushType)}: {_rushType}, {nameof(_battle)}: {_battle})";
        }
    }
}
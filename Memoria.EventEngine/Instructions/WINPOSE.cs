using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// EnableVictoryPose
    /// Enable or disable the victory pose at the end of battles for a specific character.
    /// 
    /// 1st argument: which character.
    /// 2nd argument: boolean activate/deactivate.
    /// AT_LCHARACTER Character (1 bytes)
    /// AT_BOOL Enable (1 bytes)
    /// WINPOSE = 0x0DB,
    /// </summary>
    internal sealed class WINPOSE : JsmInstruction
    {
        private readonly IJsmExpression _character;

        private readonly IJsmExpression _enable;

        private WINPOSE(IJsmExpression character, IJsmExpression enable)
        {
            _character = character;
            _enable = enable;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression character = reader.ArgumentByte();
            IJsmExpression enable = reader.ArgumentByte();
            return new WINPOSE(character, enable);
        }
        public override String ToString()
        {
            return $"{nameof(WINPOSE)}({nameof(_character)}: {_character}, {nameof(_enable)}: {_enable})";
        }
    }
}
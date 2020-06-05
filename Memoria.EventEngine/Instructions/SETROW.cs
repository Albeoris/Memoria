using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetRow
    /// Change the battle row of a party member.
    /// 
    /// 1st argument: party member.
    /// 2nd argument: boolean front/back.
    /// AT_LCHARACTER Character (1 bytes)
    /// AT_BOOL Row (1 bytes)
    /// SETROW = 0x062,
    /// </summary>
    internal sealed class SETROW : JsmInstruction
    {
        private readonly IJsmExpression _character;

        private readonly IJsmExpression _row;

        private SETROW(IJsmExpression character, IJsmExpression row)
        {
            _character = character;
            _row = row;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression character = reader.ArgumentByte();
            IJsmExpression row = reader.ArgumentByte();
            return new SETROW(character, row);
        }
        public override String ToString()
        {
            return $"{nameof(SETROW)}({nameof(_character)}: {_character}, {nameof(_row)}: {_row})";
        }
    }
}
using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetName
    /// Change the name of a party member. Clear the text opcodes from the chosen text.
    /// 
    /// 1st argument: character to rename.
    /// 2nd argument: new name.
    /// AT_LCHARACTER Character (1 bytes)
    /// AT_TEXT Text (2 bytes)
    /// PLAYERNAME = 0x0DE,
    /// </summary>
    internal sealed class PLAYERNAME : JsmInstruction
    {
        private readonly IJsmExpression _character;

        private readonly IJsmExpression _text;

        private PLAYERNAME(IJsmExpression character, IJsmExpression text)
        {
            _character = character;
            _text = text;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression character = reader.ArgumentByte();
            IJsmExpression text = reader.ArgumentInt16();
            return new PLAYERNAME(character, text);
        }
        public override String ToString()
        {
            return $"{nameof(PLAYERNAME)}({nameof(_character)}: {_character}, {nameof(_text)}: {_text})";
        }
    }
}
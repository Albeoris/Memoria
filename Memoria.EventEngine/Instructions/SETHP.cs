using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetHP
    /// Change the HP of a party's member.
    /// 
    /// 1st argument: character.
    /// 2nd argument: new HP value.
    /// AT_LCHARACTER Character (1 bytes)
    /// AT_USPIN HP (2 bytes)
    /// SETHP = 0x0F1,
    /// </summary>
    internal sealed class SETHP : JsmInstruction
    {
        private readonly IJsmExpression _character;

        private readonly IJsmExpression _hp;

        private SETHP(IJsmExpression character, IJsmExpression hp)
        {
            _character = character;
            _hp = hp;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression character = reader.ArgumentByte();
            IJsmExpression hp = reader.ArgumentInt16();
            return new SETHP(character, hp);
        }
        public override String ToString()
        {
            return $"{nameof(SETHP)}({nameof(_character)}: {_character}, {nameof(_hp)}: {_hp})";
        }
    }
}
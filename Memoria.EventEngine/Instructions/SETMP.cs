using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetMP
    /// Change the MP of a party's member.
    /// 
    /// 1st argument: character.
    /// 2nd argument: new MP value.
    /// AT_LCHARACTER Character (1 bytes)
    /// AT_USPIN MP (2 bytes)
    /// SETMP = 0x0F2,
    /// </summary>
    internal sealed class SETMP : JsmInstruction
    {
        private readonly IJsmExpression _character;

        private readonly IJsmExpression _mp;

        private SETMP(IJsmExpression character, IJsmExpression mp)
        {
            _character = character;
            _mp = mp;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression character = reader.ArgumentByte();
            IJsmExpression mp = reader.ArgumentInt16();
            return new SETMP(character, mp);
        }
        public override String ToString()
        {
            return $"{nameof(SETMP)}({nameof(_character)}: {_character}, {nameof(_mp)}: {_mp})";
        }
    }
}
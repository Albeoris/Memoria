using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RemoveParty
    /// Remove a character from the player's team.
    /// 
    /// 1st argument: character to remove.
    /// AT_LCHARACTER Character (1 bytes)
    /// PARTYDELETE = 0x0DD,
    /// </summary>
    internal sealed class PARTYDELETE : JsmInstruction
    {
        private readonly IJsmExpression _character;

        private PARTYDELETE(IJsmExpression character)
        {
            _character = character;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression character = reader.ArgumentByte();
            return new PARTYDELETE(character);
        }
        public override String ToString()
        {
            return $"{nameof(PARTYDELETE)}({nameof(_character)}: {_character})";
        }
    }
}
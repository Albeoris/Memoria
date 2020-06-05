using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// Party
    /// Allow the player to change the members of its party.
    /// 
    /// 1st argument: party size (if characters occupy slots beyond it, they are locked).
    /// 2nd argument: list of locked characters.
    /// AT_USPIN Party Size (1 bytes)
    /// AT_CHARACTER Locked Characters (2 bytes)
    /// PARTYMENU = 0x0B2,
    /// </summary>
    internal sealed class PARTYMENU : JsmInstruction
    {
        private readonly IJsmExpression _partySize;

        private readonly IJsmExpression _lockedCharacters;

        private PARTYMENU(IJsmExpression partySize, IJsmExpression lockedCharacters)
        {
            _partySize = partySize;
            _lockedCharacters = lockedCharacters;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression partySize = reader.ArgumentByte();
            IJsmExpression lockedCharacters = reader.ArgumentInt16();
            return new PARTYMENU(partySize, lockedCharacters);
        }
        public override String ToString()
        {
            return $"{nameof(PARTYMENU)}({nameof(_partySize)}: {_partySize}, {nameof(_lockedCharacters)}: {_lockedCharacters})";
        }
    }
}
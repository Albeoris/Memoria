using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetPartyReserve
    /// Define the party member availability for a future Party call.
    /// 
    /// 1st argument: list of available characters.
    /// AT_CHARACTER Characters available (2 bytes)
    /// FULLMEMBER = 0x0B4,
    /// </summary>
    internal sealed class FULLMEMBER : JsmInstruction
    {
        private readonly IJsmExpression _charactersAvailable;

        private FULLMEMBER(IJsmExpression charactersAvailable)
        {
            _charactersAvailable = charactersAvailable;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression charactersAvailable = reader.ArgumentInt16();
            return new FULLMEMBER(charactersAvailable);
        }
        public override String ToString()
        {
            return $"{nameof(FULLMEMBER)}({nameof(_charactersAvailable)}: {_charactersAvailable})";
        }
    }
}
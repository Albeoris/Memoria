using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// WaitTurn
    /// Wait until the character has turned.
    /// WAITTURN = 0x050,
    /// </summary>
    internal sealed class WAITTURN : JsmInstruction
    {
        private WAITTURN()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new WAITTURN();
        }
        public override String ToString()
        {
            return $"{nameof(WAITTURN)}()";
        }
    }
}
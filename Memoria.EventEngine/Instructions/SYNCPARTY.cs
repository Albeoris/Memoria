using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// UpdatePartyUI
    /// Update the party's menu icons and such.
    /// SYNCPARTY = 0x0E9,
    /// </summary>
    internal sealed class SYNCPARTY : JsmInstruction
    {
        private SYNCPARTY()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new SYNCPARTY();
        }
        public override String ToString()
        {
            return $"{nameof(SYNCPARTY)}()";
        }
    }
}
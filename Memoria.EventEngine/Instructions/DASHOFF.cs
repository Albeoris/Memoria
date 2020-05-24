using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// DisableRun
    /// Make the player's character always walk.
    /// DASHOFF = 0x06A,
    /// </summary>
    internal sealed class DASHOFF : JsmInstruction
    {
        private DASHOFF()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new DASHOFF();
        }
        public override String ToString()
        {
            return $"{nameof(DASHOFF)}()";
        }
    }
}
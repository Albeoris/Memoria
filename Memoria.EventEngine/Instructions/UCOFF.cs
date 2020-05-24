using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// DisableMove
    /// Disable the player's movement control.
    /// UCOFF = 0x02D,
    /// </summary>
    internal sealed class UCOFF : JsmInstruction
    {
        private UCOFF()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new UCOFF();
        }
        public override String ToString()
        {
            return $"{nameof(UCOFF)}()";
        }
    }
}
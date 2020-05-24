using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunJumpAnimation
    /// Make the character play its jumping animation.
    /// PREJUMP = 0x09C,
    /// </summary>
    internal sealed class PREJUMP : JsmInstruction
    {
        private PREJUMP()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new PREJUMP();
        }
        public override String ToString()
        {
            return $"{nameof(PREJUMP)}()";
        }
    }
}
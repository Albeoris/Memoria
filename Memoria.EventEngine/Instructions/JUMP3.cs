using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// Jump
    /// Perform a jumping animation. Must be used after a SetupJump call.
    /// JUMP3 = 0x0DC,
    /// </summary>
    internal sealed class JUMP3 : JsmInstruction
    {
        private JUMP3()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new JUMP3();
        }
        public override String ToString()
        {
            return $"{nameof(JUMP3)}()";
        }
    }
}
using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunLandAnimation
    /// Make the character play its landing animation (inverted jumping animation).
    /// POSTJUMP = 0x09D,
    /// </summary>
    internal sealed class POSTJUMP : JsmInstruction
    {
        private POSTJUMP()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new POSTJUMP();
        }
        public override String ToString()
        {
            return $"{nameof(POSTJUMP)}()";
        }
    }
}
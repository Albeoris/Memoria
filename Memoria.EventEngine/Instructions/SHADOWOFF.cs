using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// DisableShadow
    /// Disable the shadow for the entry's object.
    /// SHADOWOFF = 0x080,
    /// </summary>
    internal sealed class SHADOWOFF : JsmInstruction
    {
        private SHADOWOFF()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new SHADOWOFF();
        }
        public override String ToString()
        {
            return $"{nameof(SHADOWOFF)}()";
        }
    }
}
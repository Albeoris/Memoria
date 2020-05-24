using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// AddFrog
    /// Add one frog to the frog counter.
    /// INCFROG = 0x0E0,
    /// </summary>
    internal sealed class INCFROG : JsmInstruction
    {
        private INCFROG()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new INCFROG();
        }
        public override String ToString()
        {
            return $"{nameof(INCFROG)}()";
        }
    }
}
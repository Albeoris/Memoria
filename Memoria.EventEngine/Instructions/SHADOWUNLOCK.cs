using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// UnlockShadowRotation
    /// Make the shadow rotate accordingly with its object.
    /// SHADOWUNLOCK = 0x084,
    /// </summary>
    internal sealed class SHADOWUNLOCK : JsmInstruction
    {
        private SHADOWUNLOCK()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new SHADOWUNLOCK();
        }
        public override String ToString()
        {
            return $"{nameof(SHADOWUNLOCK)}()";
        }
    }
}
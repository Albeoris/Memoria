using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// DisableCameraFollow
    /// Stop making the camera follow the player's character.
    /// BGCUNLOCK = 0x074,
    /// </summary>
    internal sealed class BGCUNLOCK : JsmInstruction
    {
        private BGCUNLOCK()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new BGCUNLOCK();
        }
        public override String ToString()
        {
            return $"{nameof(BGCUNLOCK)}()";
        }
    }
}
using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// EnableCameraFollow
    /// Make the camera follow the player's character.
    /// BGCLOCK = 0x073,
    /// </summary>
    internal sealed class BGCLOCK : JsmInstruction
    {
        private BGCLOCK()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new BGCLOCK();
        }
        public override String ToString()
        {
            return $"{nameof(BGCLOCK)}()";
        }
    }
}
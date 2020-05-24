using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// WaitAnimation
    /// Wait until the current object's animation has ended.
    /// WAITANIM = 0x041,
    /// </summary>
    internal sealed class WAITANIM : JsmInstruction
    {
        private WAITANIM()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new WAITANIM();
        }
        public override String ToString()
        {
            return $"{nameof(WAITANIM)}()";
        }
    }
}
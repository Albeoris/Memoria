using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// StopAnimation
    /// Stop the character's animation.
    /// ENDANIM = 0x042,
    /// </summary>
    internal sealed class ENDANIM : JsmInstruction
    {
        private ENDANIM()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new ENDANIM();
        }
        public override String ToString()
        {
            return $"{nameof(ENDANIM)}()";
        }
    }
}
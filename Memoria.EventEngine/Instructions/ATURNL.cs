using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetLeftAnimation
    /// Change the left turning animation.
    /// 
    /// 1st argument: animation ID.
    /// AT_ANIMATION Animation (2 bytes)
    /// ATURNL = 0x07A,
    /// </summary>
    internal sealed class ATURNL : JsmInstruction
    {
        private readonly IJsmExpression _animation;

        private ATURNL(IJsmExpression animation)
        {
            _animation = animation;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression animation = reader.ArgumentInt16();
            return new ATURNL(animation);
        }
        public override String ToString()
        {
            return $"{nameof(ATURNL)}({nameof(_animation)}: {_animation})";
        }
    }
}
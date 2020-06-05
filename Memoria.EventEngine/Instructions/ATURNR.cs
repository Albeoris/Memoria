using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetRightAnimation
    /// Change the right turning animation.
    /// 
    /// 1st argument: animation ID.
    /// AT_ANIMATION Animation (2 bytes)
    /// ATURNR = 0x07B,
    /// </summary>
    internal sealed class ATURNR : JsmInstruction
    {
        private readonly IJsmExpression _animation;

        private ATURNR(IJsmExpression animation)
        {
            _animation = animation;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression animation = reader.ArgumentInt16();
            return new ATURNR(animation);
        }
        public override String ToString()
        {
            return $"{nameof(ATURNR)}({nameof(_animation)}: {_animation})";
        }
    }
}
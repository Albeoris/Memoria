using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetRunAnimation
    /// Change the running animation.
    /// 
    /// 1st argument: animation ID.
    /// AT_ANIMATION Animation (2 bytes)
    /// ARUN = 0x035,
    /// </summary>
    internal sealed class ARUN : JsmInstruction
    {
        private readonly IJsmExpression _animation;

        private ARUN(IJsmExpression animation)
        {
            _animation = animation;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression animation = reader.ArgumentInt16();
            return new ARUN(animation);
        }
        public override String ToString()
        {
            return $"{nameof(ARUN)}({nameof(_animation)}: {_animation})";
        }
    }
}
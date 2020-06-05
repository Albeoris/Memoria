using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetWalkAnimation
    /// Change the walking animation.
    /// 
    /// 1st argument: animation ID.
    /// AT_ANIMATION Animation (2 bytes)
    /// AWALK = 0x034,
    /// </summary>
    internal sealed class AWALK : JsmInstruction
    {
        private readonly IJsmExpression _animation;

        private AWALK(IJsmExpression animation)
        {
            _animation = animation;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression animation = reader.ArgumentInt16();
            return new AWALK(animation);
        }
        public override String ToString()
        {
            return $"{nameof(AWALK)}({nameof(_animation)}: {_animation})";
        }
    }
}
using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetStandAnimation
    /// Change the standing animation.
    /// 
    /// 1st argument: animation ID.
    /// AT_ANIMATION Animation (2 bytes)
    /// AIDLE = 0x033,
    /// </summary>
    internal sealed class AIDLE : JsmInstruction
    {
        private readonly IJsmExpression _animation;

        private AIDLE(IJsmExpression animation)
        {
            _animation = animation;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression animation = reader.ArgumentInt16();
            return new AIDLE(animation);
        }
        public override String ToString()
        {
            return $"{nameof(AIDLE)}({nameof(_animation)}: {_animation})";
        }
    }
}
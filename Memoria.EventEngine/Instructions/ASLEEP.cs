using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetInactiveAnimation
    /// Change the animation played when inactive for a long time. The inaction time required is:
    /// First Time = 200 + 4 * Random[0, 255]
    /// Subsequent Times = 200 + 2 * Random[0, 255]
    /// 
    /// 1st argument: animation ID.
    /// AT_ANIMATION Animation (2 bytes)
    /// ASLEEP = 0x052,
    /// </summary>
    internal sealed class ASLEEP : JsmInstruction
    {
        private readonly IJsmExpression _animation;

        private ASLEEP(IJsmExpression animation)
        {
            _animation = animation;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression animation = reader.ArgumentInt16();
            return new ASLEEP(animation);
        }
        public override String ToString()
        {
            return $"{nameof(ASLEEP)}({nameof(_animation)}: {_animation})";
        }
    }
}
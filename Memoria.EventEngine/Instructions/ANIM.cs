using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunAnimation
    /// Make the character play an animation.
    /// 
    /// 1st argument: animation ID.
    /// AT_ANIMATION Animation (2 bytes)
    /// ANIM = 0x040,
    /// </summary>
    internal sealed class ANIM : JsmInstruction
    {
        private readonly IJsmExpression _animation;

        private ANIM(IJsmExpression animation)
        {
            _animation = animation;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression animation = reader.ArgumentInt16();
            return new ANIM(animation);
        }
        public override String ToString()
        {
            return $"{nameof(ANIM)}({nameof(_animation)}: {_animation})";
        }
    }
}
using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetJumpAnimation
    /// Change the jumping animation.
    /// 
    /// 1st argument: animation ID.
    /// 2nd argument: unknown.
    /// 3rd argument: unknown.
    /// AT_ANIMATION Animation (2 bytes)
    /// AT_SPIN Unknown (1 bytes)
    /// AT_SPIN Unknown (1 bytes)
    /// AJUMP = 0x094,
    /// </summary>
    internal sealed class AJUMP : JsmInstruction
    {
        private readonly IJsmExpression _animation;

        private readonly IJsmExpression _unknown1;

        private readonly IJsmExpression _unknown2;

        private AJUMP(IJsmExpression animation, IJsmExpression unknown1, IJsmExpression unknown2)
        {
            _animation = animation;
            _unknown1 = unknown1;
            _unknown2 = unknown2;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression animation = reader.ArgumentInt16();
            IJsmExpression unknown1 = reader.ArgumentByte();
            IJsmExpression unknown2 = reader.ArgumentByte();
            return new AJUMP(animation, unknown1, unknown2);
        }
        public override String ToString()
        {
            return $"{nameof(AJUMP)}({nameof(_animation)}: {_animation}, {nameof(_unknown1)}: {_unknown1}, {nameof(_unknown2)}: {_unknown2})";
        }
    }
}
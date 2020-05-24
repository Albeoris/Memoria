using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetTileAnimationFrame
    /// Change the frame of a field tile animation (can be used to hide them all if the given frame is out of range, eg. 255).
    /// 
    /// 1st argument: background animation.
    /// 2nd argument: animation frame to display.
    /// AT_TILEANIM Animation (1 bytes)
    /// AT_USPIN Frame ID (1 bytes)
    /// BGAFRAME = 0x0E7,
    /// </summary>
    internal sealed class BGAFRAME : JsmInstruction
    {
        private readonly IJsmExpression _animation;

        private readonly IJsmExpression _frameId;

        private BGAFRAME(IJsmExpression animation, IJsmExpression frameId)
        {
            _animation = animation;
            _frameId = frameId;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression animation = reader.ArgumentByte();
            IJsmExpression frameId = reader.ArgumentByte();
            return new BGAFRAME(animation, frameId);
        }
        public override String ToString()
        {
            return $"{nameof(BGAFRAME)}({nameof(_animation)}: {_animation}, {nameof(_frameId)}: {_frameId})";
        }
    }
}
using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetTileAnimationPause
    /// Make a field tile animation pause at some frame in addition to its normal animation speed.
    /// 
    /// 1st argument: background animation.
    /// 2nd argument: animation frame.
    /// 3rd argument: wait time.
    /// AT_TILEANIM Tile Animation (1 bytes)
    /// AT_USPIN Frame ID (1 bytes)
    /// AT_USPIN Time (1 bytes)
    /// BGAWAIT = 0x063,
    /// </summary>
    internal sealed class BGAWAIT : JsmInstruction
    {
        private readonly IJsmExpression _tileAnimation;

        private readonly IJsmExpression _frameId;

        private readonly IJsmExpression _time;

        private BGAWAIT(IJsmExpression tileAnimation, IJsmExpression frameId, IJsmExpression time)
        {
            _tileAnimation = tileAnimation;
            _frameId = frameId;
            _time = time;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression tileAnimation = reader.ArgumentByte();
            IJsmExpression frameId = reader.ArgumentByte();
            IJsmExpression time = reader.ArgumentByte();
            return new BGAWAIT(tileAnimation, frameId, time);
        }
        public override String ToString()
        {
            return $"{nameof(BGAWAIT)}({nameof(_tileAnimation)}: {_tileAnimation}, {nameof(_frameId)}: {_frameId}, {nameof(_time)}: {_time})";
        }
    }
}
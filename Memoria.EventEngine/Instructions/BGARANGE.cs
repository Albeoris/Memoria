using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunTileAnimationEx
    /// Run a field tile animation and choose its frame range.
    /// 
    /// 1st argument: background animation.
    /// 2nd argument: starting frame.
    /// 3rd argument: ending frame.
    /// AT_TILEANIM Tile Animation (1 bytes)
    /// AT_USPIN Start (1 bytes)
    /// AT_USPIN End (1 bytes)
    /// BGARANGE = 0x065,
    /// </summary>
    internal sealed class BGARANGE : JsmInstruction
    {
        private readonly IJsmExpression _tileAnimation;

        private readonly IJsmExpression _start;

        private readonly IJsmExpression _end;

        private BGARANGE(IJsmExpression tileAnimation, IJsmExpression start, IJsmExpression end)
        {
            _tileAnimation = tileAnimation;
            _start = start;
            _end = end;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression tileAnimation = reader.ArgumentByte();
            IJsmExpression start = reader.ArgumentByte();
            IJsmExpression end = reader.ArgumentByte();
            return new BGARANGE(tileAnimation, start, end);
        }
        public override String ToString()
        {
            return $"{nameof(BGARANGE)}({nameof(_tileAnimation)}: {_tileAnimation}, {nameof(_start)}: {_start}, {nameof(_end)}: {_end})";
        }
    }
}
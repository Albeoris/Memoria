using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetTileAnimationFlags
    /// Add flags of a field tile animation.
    /// 
    /// 1st argument: background animation.
    /// 2nd argument: flags (only the flags 5 and 6 can be added).
    ///  5: unknown
    ///  6: loop back and forth
    /// AT_TILEANIM Tile Animation (1 bytes)
    /// AT_BOOLLIST Flags (1 bytes)
    /// BGAFLAG = 0x064,
    /// </summary>
    internal sealed class BGAFLAG : JsmInstruction
    {
        private readonly IJsmExpression _tileAnimation;

        private readonly IJsmExpression _flags;

        private BGAFLAG(IJsmExpression tileAnimation, IJsmExpression flags)
        {
            _tileAnimation = tileAnimation;
            _flags = flags;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression tileAnimation = reader.ArgumentByte();
            IJsmExpression flags = reader.ArgumentByte();
            return new BGAFLAG(tileAnimation, flags);
        }
        public override String ToString()
        {
            return $"{nameof(BGAFLAG)}({nameof(_tileAnimation)}: {_tileAnimation}, {nameof(_flags)}: {_flags})";
        }
    }
}
using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetTileAnimationSpeed
    /// Change the speed of a field tile animation.
    /// 
    /// 1st argument: background animation.
    /// 2nd argument: speed (256 = 1 tile/frame).
    /// AT_TILEANIM Tile Animation (1 bytes)
    /// AT_SPIN Speed (2 bytes)
    /// BGARATE = 0x061,
    /// </summary>
    internal sealed class BGARATE : JsmInstruction
    {
        private readonly IJsmExpression _tileAnimation;

        private readonly IJsmExpression _speed;

        private BGARATE(IJsmExpression tileAnimation, IJsmExpression speed)
        {
            _tileAnimation = tileAnimation;
            _speed = speed;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression tileAnimation = reader.ArgumentByte();
            IJsmExpression speed = reader.ArgumentInt16();
            return new BGARATE(tileAnimation, speed);
        }
        public override String ToString()
        {
            return $"{nameof(BGARATE)}({nameof(_tileAnimation)}: {_tileAnimation}, {nameof(_speed)}: {_speed})";
        }
    }
}
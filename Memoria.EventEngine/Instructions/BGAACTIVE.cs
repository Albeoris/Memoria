using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// ActivateTileAnimation
    /// Make a field tile animation active.
    /// 
    /// 1st argument: background animation.
    /// 2nd argument: boolean on/off.
    /// AT_TILEANIM Tile Animation (1 bytes)
    /// AT_BOOL Activate (1 bytes)
    /// BGAACTIVE = 0x060,
    /// </summary>
    internal sealed class BGAACTIVE : JsmInstruction
    {
        private readonly IJsmExpression _tileAnimation;

        private readonly IJsmExpression _activate;

        private BGAACTIVE(IJsmExpression tileAnimation, IJsmExpression activate)
        {
            _tileAnimation = tileAnimation;
            _activate = activate;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression tileAnimation = reader.ArgumentByte();
            IJsmExpression activate = reader.ArgumentByte();
            return new BGAACTIVE(tileAnimation, activate);
        }
        public override String ToString()
        {
            return $"{nameof(BGAACTIVE)}({nameof(_tileAnimation)}: {_tileAnimation}, {nameof(_activate)}: {_activate})";
        }
    }
}
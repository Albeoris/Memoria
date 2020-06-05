using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetShadowSize
    /// Set the entry's object shadow size.
    /// 
    /// 1st argument: size X.
    /// 2nd argument: size Y.
    /// AT_SPIN Size X (1 bytes)
    /// AT_SPIN Size Y (1 bytes)
    /// SHADOWSCALE = 0x081,
    /// </summary>
    internal sealed class SHADOWSCALE : JsmInstruction
    {
        private readonly IJsmExpression _sizeX;

        private readonly IJsmExpression _sizeY;

        private SHADOWSCALE(IJsmExpression sizeX, IJsmExpression sizeY)
        {
            _sizeX = sizeX;
            _sizeY = sizeY;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression sizeX = reader.ArgumentByte();
            IJsmExpression sizeY = reader.ArgumentByte();
            return new SHADOWSCALE(sizeX, sizeY);
        }
        public override String ToString()
        {
            return $"{nameof(SHADOWSCALE)}({nameof(_sizeX)}: {_sizeX}, {nameof(_sizeY)}: {_sizeY})";
        }
    }
}
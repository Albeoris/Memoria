using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// 0x3D
    /// Some animation flags.
    /// 
    /// 1st argument: unknown.
    /// 2nd argument: unknown.
    /// AT_USPIN In-Frame (1 bytes)
    /// AT_USPIN Out-Frame (1 bytes)
    /// AFRAME = 0x03D,
    /// </summary>
    internal sealed class AFRAME : JsmInstruction
    {
        private readonly IJsmExpression _inFrame;

        private readonly IJsmExpression _outFrame;

        private AFRAME(IJsmExpression inFrame, IJsmExpression outFrame)
        {
            _inFrame = inFrame;
            _outFrame = outFrame;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression inFrame = reader.ArgumentByte();
            IJsmExpression outFrame = reader.ArgumentByte();
            return new AFRAME(inFrame, outFrame);
        }
        public override String ToString()
        {
            return $"{nameof(AFRAME)}({nameof(_inFrame)}: {_inFrame}, {nameof(_outFrame)}: {_outFrame})";
        }
    }
}
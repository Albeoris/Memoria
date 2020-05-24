using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetObjectSize
    /// Set the size of a 3D model.
    /// 
    /// 1st argument: entry of the 3D model.
    /// 2nd to 4th arguments: size ratio in (Ratio X, Ratio Z, Ratio Y) format. A ratio of 64 is the default size.
    /// AT_ENTRY Object (1 bytes)
    /// AT_SPIN Size X (1 bytes)
    /// AT_SPIN Size Z (1 bytes)
    /// AT_SPIN Size Y (1 bytes)
    /// CHRSCALE = 0x09F,
    /// </summary>
    internal sealed class CHRSCALE : JsmInstruction
    {
        private readonly IJsmExpression _object;

        private readonly IJsmExpression _sizeX;

        private readonly IJsmExpression _sizeZ;

        private readonly IJsmExpression _sizeY;

        private CHRSCALE(IJsmExpression @object, IJsmExpression sizeX, IJsmExpression sizeZ, IJsmExpression sizeY)
        {
            _object = @object;
            _sizeX = sizeX;
            _sizeZ = sizeZ;
            _sizeY = sizeY;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @object = reader.ArgumentByte();
            IJsmExpression sizeX = reader.ArgumentByte();
            IJsmExpression sizeZ = reader.ArgumentByte();
            IJsmExpression sizeY = reader.ArgumentByte();
            return new CHRSCALE(@object, sizeX, sizeZ, sizeY);
        }
        public override String ToString()
        {
            return $"{nameof(CHRSCALE)}({nameof(_object)}: {_object}, {nameof(_sizeX)}: {_sizeX}, {nameof(_sizeZ)}: {_sizeZ}, {nameof(_sizeY)}: {_sizeY})";
        }
    }
}
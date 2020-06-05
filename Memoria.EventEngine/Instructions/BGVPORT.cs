using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetCameraBounds
    /// Redefine the field camera boundaries (default value is part of the background's data).
    /// 
    /// 1st argument: camera ID.
    /// 2nd to 5th arguments: Boundaries in (MinX, MaxX, MinY, MaxY) format.
    /// AT_USPIN Camera (1 bytes)
    /// AT_SPIN Min X (2 bytes)
    /// AT_SPIN Max X (2 bytes)
    /// AT_SPIN Min Y (2 bytes)
    /// AT_SPIN Max Y (2 bytes)
    /// BGVPORT = 0x01E,
    /// </summary>
    internal sealed class BGVPORT : JsmInstruction
    {
        private readonly IJsmExpression _camera;

        private readonly IJsmExpression _minX;

        private readonly IJsmExpression _maxX;

        private readonly IJsmExpression _minY;

        private readonly IJsmExpression _maxY;

        private BGVPORT(IJsmExpression camera, IJsmExpression minX, IJsmExpression maxX, IJsmExpression minY, IJsmExpression maxY)
        {
            _camera = camera;
            _minX = minX;
            _maxX = maxX;
            _minY = minY;
            _maxY = maxY;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression camera = reader.ArgumentByte();
            IJsmExpression minX = reader.ArgumentInt16();
            IJsmExpression maxX = reader.ArgumentInt16();
            IJsmExpression minY = reader.ArgumentInt16();
            IJsmExpression maxY = reader.ArgumentInt16();
            return new BGVPORT(camera, minX, maxX, minY, maxY);
        }
        public override String ToString()
        {
            return $"{nameof(BGVPORT)}({nameof(_camera)}: {_camera}, {nameof(_minX)}: {_minX}, {nameof(_maxX)}: {_maxX}, {nameof(_minY)}: {_minY}, {nameof(_maxY)}: {_maxY})";
        }
    }
}
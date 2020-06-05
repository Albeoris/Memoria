using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetFieldCamera
    /// Change the field's background camera.
    /// 
    /// 1st argument: camera ID.
    /// AT_USPIN Camera ID (1 bytes)
    /// SETCAM = 0x07E,
    /// </summary>
    internal sealed class SETCAM : JsmInstruction
    {
        private readonly IJsmExpression _cameraId;

        private SETCAM(IJsmExpression cameraId)
        {
            _cameraId = cameraId;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression cameraId = reader.ArgumentByte();
            return new SETCAM(cameraId);
        }
        public override String ToString()
        {
            return $"{nameof(SETCAM)}({nameof(_cameraId)}: {_cameraId})";
        }
    }
}
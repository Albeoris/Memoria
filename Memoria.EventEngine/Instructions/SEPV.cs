using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetSoundPosition
    /// Set the position of a 3D sound.
    /// 
    /// 1st to 3rd arguments: sound position.
    /// 4th argument: sound volume.
    /// AT_POSITION_X PositionX (2 bytes)
    /// AT_POSITION_Z PositionZ (2 bytes)
    /// AT_POSITION_Y PositionY (2 bytes)
    /// AT_SPIN Volume (1 bytes)
    /// SEPV = 0x089,
    /// </summary>
    internal sealed class SEPV : JsmInstruction
    {
        private readonly IJsmExpression _positionX;

        private readonly IJsmExpression _positionZ;

        private readonly IJsmExpression _positionY;

        private readonly IJsmExpression _volume;

        private SEPV(IJsmExpression positionX, IJsmExpression positionZ, IJsmExpression positionY, IJsmExpression volume)
        {
            _positionX = positionX;
            _positionZ = positionZ;
            _positionY = positionY;
            _volume = volume;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression positionX = reader.ArgumentInt16();
            IJsmExpression positionZ = reader.ArgumentInt16();
            IJsmExpression positionY = reader.ArgumentInt16();
            IJsmExpression volume = reader.ArgumentByte();
            return new SEPV(positionX, positionZ, positionY, volume);
        }
        public override String ToString()
        {
            return $"{nameof(SEPV)}({nameof(_positionX)}: {_positionX}, {nameof(_positionZ)}: {_positionZ}, {nameof(_positionY)}: {_positionY}, {nameof(_volume)}: {_volume})";
        }
    }
}
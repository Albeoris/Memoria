using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetSoundObjectPosition
    /// Set the position of a 3D sound to the object's position.
    /// 
    /// 1st argument: object.
    /// 2nd argument: sound volume.
    /// AT_ENTRY Object (1 bytes)
    /// AT_SPIN Volume (1 bytes)
    /// SEPVA = 0x08A,
    /// </summary>
    internal sealed class SEPVA : JsmInstruction
    {
        private readonly IJsmExpression _object;

        private readonly IJsmExpression _volume;

        private SEPVA(IJsmExpression @object, IJsmExpression volume)
        {
            _object = @object;
            _volume = volume;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @object = reader.ArgumentByte();
            IJsmExpression volume = reader.ArgumentByte();
            return new SEPVA(@object, volume);
        }
        public override String ToString()
        {
            return $"{nameof(SEPVA)}({nameof(_object)}: {_object}, {nameof(_volume)}: {_volume})";
        }
    }
}
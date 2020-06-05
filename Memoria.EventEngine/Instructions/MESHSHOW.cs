using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// ShowObject
    /// Show an object.
    /// 
    /// 1st argument: object.
    /// 2nd argument: unknown.
    /// AT_ENTRY Object (1 bytes)
    /// AT_USPIN Unknown (1 bytes)
    /// MESHSHOW = 0x039,
    /// </summary>
    internal sealed class MESHSHOW : JsmInstruction
    {
        private readonly IJsmExpression _object;

        private readonly IJsmExpression _unknown;

        private MESHSHOW(IJsmExpression @object, IJsmExpression unknown)
        {
            _object = @object;
            _unknown = unknown;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @object = reader.ArgumentByte();
            IJsmExpression unknown = reader.ArgumentByte();
            return new MESHSHOW(@object, unknown);
        }
        public override String ToString()
        {
            return $"{nameof(MESHSHOW)}({nameof(_object)}: {_object}, {nameof(_unknown)}: {_unknown})";
        }
    }
}
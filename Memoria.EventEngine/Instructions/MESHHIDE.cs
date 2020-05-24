using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// HideObject
    /// Hide an object.
    /// 
    /// 1st argument: object.
    /// 2nd argument: unknown.
    /// AT_ENTRY Object (1 bytes)
    /// AT_USPIN Unknown (1 bytes)
    /// MESHHIDE = 0x03A,
    /// </summary>
    internal sealed class MESHHIDE : JsmInstruction
    {
        private readonly IJsmExpression _object;

        private readonly IJsmExpression _unknown;

        private MESHHIDE(IJsmExpression @object, IJsmExpression unknown)
        {
            _object = @object;
            _unknown = unknown;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression @object = reader.ArgumentByte();
            IJsmExpression unknown = reader.ArgumentByte();
            return new MESHHIDE(@object, unknown);
        }
        public override String ToString()
        {
            return $"{nameof(MESHHIDE)}({nameof(_object)}: {_object}, {nameof(_unknown)}: {_unknown})";
        }
    }
}
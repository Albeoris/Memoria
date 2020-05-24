using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// Field
    /// Change the field scene.
    /// 
    /// 1st argument: field scene destination.
    /// AT_FIELD Field (2 bytes)
    /// MAPJUMP = 0x02B,
    /// </summary>
    internal sealed class MAPJUMP : JsmInstruction
    {
        private readonly IJsmExpression _field;

        private MAPJUMP(IJsmExpression field)
        {
            _field = field;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression field = reader.ArgumentInt16();
            return new MAPJUMP(field);
        }
        public override String ToString()
        {
            return $"{nameof(MAPJUMP)}({nameof(_field)}: {_field})";
        }
    }
}
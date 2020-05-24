using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// PreloadField
    /// Surely preload a field; ignored in the non-PSX versions.
    /// 
    /// 1st argument: unknown.
    /// 2nd argument: field to preload.
    /// AT_SPIN Unknown (1 bytes)
    /// AT_FIELD Field (2 bytes)
    /// HINT = 0x0FD,
    /// </summary>
    internal sealed class HINT : JsmInstruction
    {
        private readonly IJsmExpression _unknown;

        private readonly IJsmExpression _field;

        private HINT(IJsmExpression unknown, IJsmExpression field)
        {
            _unknown = unknown;
            _field = field;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression unknown = reader.ArgumentByte();
            IJsmExpression field = reader.ArgumentInt16();
            return new HINT(unknown, field);
        }
        public override String ToString()
        {
            return $"{nameof(HINT)}({nameof(_unknown)}: {_unknown}, {nameof(_field)}: {_field})";
        }
    }
}
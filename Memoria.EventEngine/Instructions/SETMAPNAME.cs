using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetFieldName
    /// Change the name of the field.
    /// 
    /// 1st argument: new name (unknown format).
    /// AT_SPIN Name (2 bytes)
    /// SETMAPNAME = 0x0B0,
    /// </summary>
    internal sealed class SETMAPNAME : JsmInstruction
    {
        private readonly IJsmExpression _name;

        private SETMAPNAME(IJsmExpression name)
        {
            _name = name;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression name = reader.ArgumentInt16();
            return new SETMAPNAME(name);
        }
        public override String ToString()
        {
            return $"{nameof(SETMAPNAME)}({nameof(_name)}: {_name})";
        }
    }
}
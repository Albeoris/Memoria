using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetObjectIndex
    /// Redefine the current object's index.
    /// 
    /// 1st argument: new index.
    /// AT_USPIN Index (1 bytes)
    /// OBJINDEX = 0x03B,
    /// </summary>
    internal sealed class OBJINDEX : JsmInstruction
    {
        private readonly IJsmExpression _index;

        private OBJINDEX(IJsmExpression index)
        {
            _index = index;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression index = reader.ArgumentByte();
            return new OBJINDEX(index);
        }
        public override String ToString()
        {
            return $"{nameof(OBJINDEX)}({nameof(_index)}: {_index})";
        }
    }
}
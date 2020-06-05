using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// SetTextVariable
    /// Set the value of a text number or item variable.
    /// 
    /// 1st argument: text variable's "Script ID".
    /// 2nd argument: depends on which text opcode is related to the text variable.
    ///  For [VAR_NUM]: integral value.
    ///  For [VAR_ITEM]: item ID.
    ///  For [VAR_TOKEN]: token number.
    /// AT_USPIN Variable ID (1 bytes)
    /// AT_ITEM Value (2 bytes)
    /// MESVALUE = 0x066,
    /// </summary>
    internal sealed class MESVALUE : JsmInstruction
    {
        private readonly IJsmExpression _variableId;

        private readonly IJsmExpression _value;

        private MESVALUE(IJsmExpression variableId, IJsmExpression value)
        {
            _variableId = variableId;
            _value = value;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression variableId = reader.ArgumentByte();
            IJsmExpression value = reader.ArgumentInt16();
            return new MESVALUE(variableId, value);
        }
        public override String ToString()
        {
            return $"{nameof(MESVALUE)}({nameof(_variableId)}: {_variableId}, {nameof(_value)}: {_value})";
        }
    }
}
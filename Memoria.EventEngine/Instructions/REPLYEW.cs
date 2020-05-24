using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunScriptObjectSync
    /// Wait until the entry's script level gets higher than the specified script level then run the script function and wait until it returns. Must only be used in response to a function call ; the argument's entry is the one that called this function.
    /// 
    /// Entry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not run. Otherwise, the entry's script level is set to the specified script level until the function returns.
    /// 
    /// 1st argument: script level.
    /// 2nd argument: function.
    /// AT_SCRIPTLVL Script Leve (1 bytes)
    /// AT_FUNCTION Function (1 bytes)
    /// REPLYEW = 0x01A,
    /// </summary>
    internal sealed class REPLYEW : JsmInstruction
    {
        private readonly IJsmExpression _scriptLeve;

        private readonly IJsmExpression _function;

        private REPLYEW(IJsmExpression scriptLeve, IJsmExpression function)
        {
            _scriptLeve = scriptLeve;
            _function = function;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression scriptLeve = reader.ArgumentByte();
            IJsmExpression function = reader.ArgumentByte();
            return new REPLYEW(scriptLeve, function);
        }
        public override String ToString()
        {
            return $"{nameof(REPLYEW)}({nameof(_scriptLeve)}: {_scriptLeve}, {nameof(_function)}: {_function})";
        }
    }
}
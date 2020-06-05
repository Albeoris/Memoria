using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunScriptSync
    /// Wait until the entry's script level gets higher than the specified script level then run the script function and wait until it returns.
    /// 
    /// Entry's script level is 0 until its main function returns, then it becomes 7. If the specified script level is higher than the entry's script level, the function is not run. Otherwise, the entry's script level is set to the specified script level until the function returns.
    /// 
    /// 1st argument: script level.
    /// 2nd argument: entry of the function.
    /// 3rd argument: function.
    /// AT_SCRIPTLVL Script Leve (1 bytes)
    /// AT_ENTRY Entry (1 bytes)
    /// AT_FUNCTION Function (1 bytes)
    /// REQEW = 0x014,
    /// </summary>
    internal sealed class REQEW : JsmInstruction
    {
        private readonly IJsmExpression _scriptLeve;

        private readonly IJsmExpression _entry;

        private readonly IJsmExpression _function;

        private REQEW(IJsmExpression scriptLeve, IJsmExpression entry, IJsmExpression function)
        {
            _scriptLeve = scriptLeve;
            _entry = entry;
            _function = function;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression scriptLeve = reader.ArgumentByte();
            IJsmExpression entry = reader.ArgumentByte();
            IJsmExpression function = reader.ArgumentByte();
            return new REQEW(scriptLeve, entry, function);
        }
        public override String ToString()
        {
            return $"{nameof(REQEW)}({nameof(_scriptLeve)}: {_scriptLeve}, {nameof(_entry)}: {_entry}, {nameof(_function)}: {_function})";
        }
    }
}
using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunSharedScript
    /// Run script passing the current object to it and continue executing the current function. If another shared script is already running for this object, it will be terminated.
    /// 
    /// 1st argument: entry (should be a one-function entry).
    /// AT_ENTRY Entry (1 bytes)
    /// STARTSEQ = 0x043,
    /// </summary>
    internal sealed class STARTSEQ : JsmInstruction
    {
        private readonly IJsmExpression _entry;

        private STARTSEQ(IJsmExpression entry)
        {
            _entry = entry;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression entry = reader.ArgumentByte();
            return new STARTSEQ(entry);
        }
        public override String ToString()
        {
            return $"{nameof(STARTSEQ)}({nameof(_entry)}: {_entry})";
        }
    }
}
using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// InitRegion
    /// Init a region code (associated with a region).
    /// 
    /// 1st argument: code entry to init.
    /// 2nd argument: Unique ID (defaulted to entry's ID if 0).
    /// AT_ENTRY Entry (1 bytes)
    /// AT_USPIN UID (1 bytes)
    /// NEW2 = 0x008,
    /// </summary>
    internal sealed class NEW2 : JsmInstruction
    {
        private readonly Byte _entry;

        private readonly Byte _uid;

        private NEW2(Byte entry, Byte uid)
        {
            _entry = entry;
            _uid = uid;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            Byte entry = reader.Arguments();
            Byte uid = reader.ReadByte();
            return new NEW2(entry, uid);
        }
        
        public override String ToString()
        {
            return $"{nameof(NEW2)}({nameof(_entry)}: {_entry}, {nameof(_uid)}: {_uid})";
        }
    }
}
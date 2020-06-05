using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// EXTENDED_CODE
    /// Not an opcode.
    /// EXT = 0x0FF,
    /// </summary>
    internal sealed class EXT : JsmInstruction
    {
        private EXT()
        {
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            return new EXT();
        }
        public override String ToString()
        {
            return $"{nameof(EXT)}()";
        }
    }
}
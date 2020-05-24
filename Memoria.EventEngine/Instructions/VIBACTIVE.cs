using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// ActivateVibration
    /// Make the controller's vibration active. If the current controller's frame is out of the vibration time range, the vibration lifespan is reinit.
    /// 
    /// 1st argument: boolean activate/deactivate.
    /// AT_BOOL Activate (1 bytes)
    /// VIBACTIVE = 0x0F7,
    /// </summary>
    internal sealed class VIBACTIVE : JsmInstruction
    {
        private readonly IJsmExpression _activate;

        private VIBACTIVE(IJsmExpression activate)
        {
            _activate = activate;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression activate = reader.ArgumentByte();
            return new VIBACTIVE(activate);
        }
        public override String ToString()
        {
            return $"{nameof(VIBACTIVE)}({nameof(_activate)}: {_activate})";
        }
    }
}
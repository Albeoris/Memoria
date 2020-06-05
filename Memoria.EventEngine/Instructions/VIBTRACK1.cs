using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// RunVibrationTrack
    /// Run a vibration track.
    /// 
    /// 1st argument: track ID.
    /// 2nd argument: sample (0 or 1).
    /// 3rd argument: boolean activate/deactivate.
    /// AT_USPIN Track (1 bytes)
    /// AT_USPIN Sample (1 bytes)
    /// AT_BOOL Activate (1 bytes)
    /// VIBTRACK1 = 0x0F8,
    /// </summary>
    internal sealed class VIBTRACK1 : JsmInstruction
    {
        private readonly IJsmExpression _track;

        private readonly IJsmExpression _sample;

        private readonly IJsmExpression _activate;

        private VIBTRACK1(IJsmExpression track, IJsmExpression sample, IJsmExpression activate)
        {
            _track = track;
            _sample = sample;
            _activate = activate;
        }

        public static JsmInstruction Create(JsmInstructionReader reader)
        {
            IJsmExpression track = reader.ArgumentByte();
            IJsmExpression sample = reader.ArgumentByte();
            IJsmExpression activate = reader.ArgumentByte();
            return new VIBTRACK1(track, sample, activate);
        }
        public override String ToString()
        {
            return $"{nameof(VIBTRACK1)}({nameof(_track)}: {_track}, {nameof(_sample)}: {_sample}, {nameof(_activate)}: {_activate})";
        }
    }
}
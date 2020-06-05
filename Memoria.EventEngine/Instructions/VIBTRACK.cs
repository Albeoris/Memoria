using System;

namespace FF8.JSM.Instructions
{
    /// <summary>
    /// ActivateVibrationTrack
    /// Activate a vibration track.
    /// 
    /// 1st argument: track ID.
    /// 2nd argument: sample (0 or 1).
    /// 3rd argument: boolean activate/deactivate.
    /// AT_USPIN Track (1 bytes)
    /// AT_USPIN Sample (1 bytes)
    /// AT_BOOL Activate (1 bytes)
    /// VIBTRACK = 0x0F9,
    /// </summary>
    internal sealed class VIBTRACK : JsmInstruction
    {
        private readonly IJsmExpression _track;

        private readonly IJsmExpression _sample;

        private readonly IJsmExpression _activate;

        private VIBTRACK(IJsmExpression track, IJsmExpression sample, IJsmExpression activate)
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
            return new VIBTRACK(track, sample, activate);
        }
        public override String ToString()
        {
            return $"{nameof(VIBTRACK)}({nameof(_track)}: {_track}, {nameof(_sample)}: {_sample}, {nameof(_activate)}: {_activate})";
        }
    }
}
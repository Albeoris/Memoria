using System;
using System.Runtime.InteropServices;

namespace Global.Sound.SaXAudio
{
    static class SaXAudio
    {
        public static Boolean Init()
        {
            Boolean result = Create();
            SetOnFinishedCallback(TriggerOnFinished);
            return result;
        }
        [DllImport("SaXAudio")]
        private static extern Boolean Create();
        [DllImport("SaXAudio")]
        public static extern void Release();

        [DllImport("SaXAudio")]
        public static extern void StartEngine();
        [DllImport("SaXAudio")]
        public static extern void StopEngine();

        [DllImport("SaXAudio")]
        public static extern void PauseAll(Single fade = 0.1f);
        [DllImport("SaXAudio")]
        public static extern void ResumeAll(Single fade = 0.1f);

        [DllImport("SaXAudio")]
        public static extern Int32 BankAddOgg(IntPtr buffer, UInt32 length);
        [DllImport("SaXAudio")]
        public static extern void BankRemove(Int32 bankID);

        [DllImport("SaXAudio")]
        public static extern Int32 CreateVoice(Int32 bankID, Boolean paused = true);
        [DllImport("SaXAudio")]
        public static extern Boolean VoiceExist(Int32 voiceID);

        [DllImport("SaXAudio")]
        public static extern Boolean Start(Int32 voiceID);
        [DllImport("SaXAudio")]
        public static extern Boolean StartAtSample(Int32 voiceID, UInt32 sample);
        [DllImport("SaXAudio")]
        public static extern Boolean StartAtTime(Int32 voiceID, Single time);
        [DllImport("SaXAudio")]
        public static extern Boolean Stop(Int32 voiceID, Single fade = 0.1f);

        [DllImport("SaXAudio")]
        public static extern UInt32 Pause(Int32 voiceID, Single fade = 0.1f);
        [DllImport("SaXAudio")]
        public static extern UInt32 Resume(Int32 voiceID, Single fade = 0.1f);
        [DllImport("SaXAudio")]
        public static extern UInt32 GetPauseStack(Int32 voiceID);

        [DllImport("SaXAudio")]
        public static extern void SetVolume(Int32 voiceID, Single volume, Single fade = 0.1f);
        [DllImport("SaXAudio")]
        public static extern void SetSpeed(Int32 voiceID, Single speed, Single fade = 0.1f);
        [DllImport("SaXAudio")]
        public static extern void SetPanning(Int32 voiceID, Single panning, Single fade = 0.1f);
        [DllImport("SaXAudio")]
        public static extern void SetLooping(Int32 voiceID, Boolean looping);
        [DllImport("SaXAudio")]
        public static extern void SetLoopPoints(Int32 voiceID, UInt32 start, UInt32 end);

        [DllImport("SaXAudio")]
        public static extern Single GetVolume(Int32 voiceID);
        [DllImport("SaXAudio")]
        public static extern Single GetSpeed(Int32 voiceID);
        [DllImport("SaXAudio")]
        public static extern Single GetPanning(Int32 voiceID);
        [DllImport("SaXAudio")]
        public static extern Boolean GetLooping(Int32 voiceID);

        [DllImport("SaXAudio")]
        public static extern UInt32 GetPositionSample(Int32 voiceID);
        [DllImport("SaXAudio")]
        public static extern Single GetPositionTime(Int32 voiceID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnFinishedDelegate(Int32 voiceID);

        [DllImport("SaXAudio")]
        private static extern void SetOnFinishedCallback(OnFinishedDelegate callback);

        private static void TriggerOnFinished(Int32 voiceID)
        {
            OnVoiceFinished(voiceID);
        }

        public static event OnFinishedDelegate OnVoiceFinished;
    }
}

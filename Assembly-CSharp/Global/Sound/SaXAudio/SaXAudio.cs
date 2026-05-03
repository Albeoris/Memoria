using System;
using System.Runtime.InteropServices;

namespace Global.Sound.SaXAudio
{
    static public class SaXAudio
    {
        public static event OnFinishedDelegate OnVoiceFinished;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ReverbParameters
        {
            // ratio of wet (processed) signal to dry (original) signal
            public Single WetDryMix = 100.0f;      // [0, 100] (percentage)

            // Delay times
            public UInt32 ReflectionsDelay = 5;    // [0, 300] in ms
            public Byte ReverbDelay = 5;           // [0, 85] in ms
            public Byte RearDelay = 5;             // 7.1: [0, 20] in ms, all other: [0, 5] in ms
            public Byte SideDelay = 5;             // 7.1: [0, 5] in ms, all other: not used, but still validated

            // Indexed parameters
            public Byte PositionLeft = 6;          // [0, 30] no units
            public Byte PositionRight = 6;         // [0, 30] no units, ignored when configured to mono
            public Byte PositionMatrixLeft = 27;   // [0, 30] no units
            public Byte PositionMatrixRight = 27;  // [0, 30] no units, ignored when configured to mono
            public Byte EarlyDiffusion = 8;        // [0, 15] no units
            public Byte LateDiffusion = 8;         // [0, 15] no units
            public Byte LowEQGain = 8;             // [0, 12] no units
            public Byte LowEQCutoff = 4;           // [0, 9] no units
            public Byte HighEQGain = 8;            // [0, 8] no units
            public Byte HighEQCutoff = 4;          // [0, 14] no units

            // Direct parameters
            public Single RoomFilterFreq = 5000f;  // [20, 20000] in Hz
            public Single RoomFilterMain = 0f;     // [-100, 0] in dB
            public Single RoomFilterHF = 0f;       // [-100, 0] in dB
            public Single ReflectionsGain = 0f;    // [-100, 20] in dB
            public Single ReverbGain = 0f;         // [-100, 20] in dB
            public Single DecayTime = 1f;          // [0.1, inf] in seconds
            public Single Density = 100f;          // [0, 100] (percentage)
            public Single RoomSize = 100f;         // [1, 100] in feet

            // component control
            public Boolean DisableLateField = false; // true to disable late field reflections

            public ReverbParameters() { }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EqParameters
        {
            public Single FrequencyCenter0 = 100f;   // [20, 20000] center frequency in Hz, band 0
            public Single Gain0 = 1f;                // [0.126, 7.94] boost/cut +/-18dB
            public Single Bandwidth0 = 1f;           // [0.1, 2] bandwidth, region of EQ is center frequency +/- bandwidth/2
            public Single FrequencyCenter1 = 800f;   // band 1
            public Single Gain1 = 1f;
            public Single Bandwidth1 = 1f;
            public Single FrequencyCenter2 = 2000f;  // band 2
            public Single Gain2 = 1f;
            public Single Bandwidth2 = 1f;
            public Single FrequencyCenter3 = 10000f; // band 3
            public Single Gain3 = 1f;
            public Single Bandwidth3 = 1f;

            public EqParameters() { }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EchoParameters
        {
            public Single WetDryMix = 0.5f; // [0f, 1f] ratio of wet (processed) signal to dry (original) signal
            public Single Feedback = 0.5f;  // [0f, 1f] amount of output fed back into input
            public Single Delay = 500f;     // [1f, 3000f] delay (all channels) in milliseconds

            public EchoParameters() { }
        }

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
        public static extern Int32 PlayOggFile(string filePath, Int32 busID = 0);

        [DllImport("SaXAudio")]
        public static extern void PauseAll(Single fade = 0.1f, Int32 busID = 0);
        [DllImport("SaXAudio")]
        public static extern void ResumeAll(Single fade = 0.1f, Int32 busID = 0);
        [DllImport("SaXAudio")]
        public static extern void StopAll(Single fade = 0.1f, Int32 busID = 0);
        [DllImport("SaXAudio")]
        public static extern void Protect(Int32 voiceID);

        [DllImport("SaXAudio")]
        public static extern Int32 BankAddOgg(IntPtr buffer, UInt32 length, OnDecodedDelegate callback);
        [DllImport("SaXAudio")]
        public static extern Int32 BankLoadOggFile(string filePath);
        [DllImport("SaXAudio")]
        public static extern void BankRemove(Int32 bankID);
        [DllImport("SaXAudio")]
        public static extern void BankAutoRemove(Int32 bankID);

        [DllImport("SaXAudio")]
        public static extern Int32 CreateVoice(Int32 bankID, Int32 busID = 0, Boolean paused = true);
        [DllImport("SaXAudio")]
        public static extern Boolean VoiceExist(Int32 voiceID);

        [DllImport("SaXAudio")]
        public static extern Int32 CreateBus();
        [DllImport("SaXAudio")]
        public static extern void RemoveBus(Int32 busID);

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
        public static extern void SetMasterVolume(Single volume, Single fade = 0.1f);
        [DllImport("SaXAudio")]
        public static extern void SetVolume(Int32 voiceID, Single volume, Single fade = 0.1f, Boolean isBus = false);
        [DllImport("SaXAudio")]
        public static extern void SetSpeed(Int32 voiceID, Single speed, Single fade = 0.1f);
        [DllImport("SaXAudio")]
        public static extern void SetPanning(Int32 voiceID, Single panning, Single fade = 0.1f);
        [DllImport("SaXAudio")]
        public static extern void SetLooping(Int32 voiceID, Boolean looping);
        [DllImport("SaXAudio")]
        public static extern void SetLoopPoints(Int32 voiceID, UInt32 start, UInt32 end);

        [DllImport("SaXAudio")]
        public static extern Single GetMasterVolume();
        [DllImport("SaXAudio")]
        public static extern Single GetVolume(Int32 voiceID);
        [DllImport("SaXAudio")]
        public static extern Single GetSpeed(Int32 voiceID);
        [DllImport("SaXAudio")]
        public static extern Single GetPanning(Int32 voiceID);
        [DllImport("SaXAudio")]
        public static extern Boolean GetLooping(Int32 voiceID);
        [DllImport("SaXAudio")]
        public static extern UInt32 GetLoopStart(Int32 voiceID);
        [DllImport("SaXAudio")]
        public static extern UInt32 GetLoopEnd(Int32 voiceID);

        [DllImport("SaXAudio")]
        public static extern void SetReverb(Int32 voiceID, ReverbParameters reverbParams, Single fade = 0, Boolean isBus = false);
        [DllImport("SaXAudio")]
        public static extern void RemoveReverb(Int32 voiceID, Single fade = 0, Boolean isBus = false);

        [DllImport("SaXAudio")]
        public static extern void SetEq(Int32 voiceID, EqParameters eqParams, Single fade = 0, Boolean isBus = false);
        [DllImport("SaXAudio")]
        public static extern void RemoveEq(Int32 voiceID, Single fade = 0, Boolean isBus = false);

        [DllImport("SaXAudio")]
        public static extern void SetEcho(Int32 voiceID, EchoParameters echoParams, Single fade = 0, Boolean isBus = false);
        [DllImport("SaXAudio")]
        public static extern void RemoveEcho(Int32 voiceID, Single fade = 0, Boolean isBus = false);

        [DllImport("SaXAudio")]
        public static extern UInt32 GetPositionSample(Int32 voiceID);
        [DllImport("SaXAudio")]
        public static extern Single GetPositionTime(Int32 voiceID);

        [DllImport("SaXAudio")]
        public static extern UInt32 GetTotalSample(Int32 voiceID);
        [DllImport("SaXAudio")]
        public static extern Single GetTotalTime(Int32 voiceID);

        [DllImport("SaXAudio")]
        public static extern UInt32 GetSampleRate(Int32 voiceID);
        [DllImport("SaXAudio")]
        public static extern UInt32 GetChannelCount(Int32 voiceID);

        [DllImport("SaXAudio")]
        public static extern UInt32 GetVoiceCount(Int32 bankID = 0, Int32 busID = 0);
        [DllImport("SaXAudio")]
        public static extern UInt32 GetBankCount();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnDecodedDelegate(Int32 bankID, IntPtr buffer);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnFinishedDelegate(Int32 voiceID);

        [DllImport("SaXAudio")]
        private static extern void SetOnFinishedCallback(OnFinishedDelegate callback);

        private static void TriggerOnFinished(Int32 voiceID)
        {
            OnVoiceFinished?.Invoke(voiceID);
        }
    }
}

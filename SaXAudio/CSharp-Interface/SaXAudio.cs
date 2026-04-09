using System;
using System.Runtime.InteropServices;

namespace SaXAudio
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

        /// <summary>
        /// Initialize the SaXAudio library and set up the voice finished callback
        /// </summary>
        /// <returns>True if initialization was successful</returns>
        public static Boolean Initialize()
        {
            Boolean result = Create();
            SetOnFinishedCallback(TriggerOnFinished);
            return result;
        }

        /// <summary>
        /// Initialize XAudio and create a mastering voice
        /// </summary>
        /// <returns>Return true if successful</returns>
        [DllImport("SaXAudio")]
        private static extern Boolean Create();

        /// <summary>
        /// Release everything
        /// </summary>
        [DllImport("SaXAudio")]
        public static extern void Release();

        /// <summary>
        /// Resume playing all voices
        /// </summary>
        [DllImport("SaXAudio")]
        public static extern void StartEngine();

        /// <summary>
        /// Pause all playing voices
        /// </summary>
        [DllImport("SaXAudio")]
        public static extern void StopEngine();

        /// <summary>
        /// That's the "Just play the darn thing!" button for ya
        /// Load and play audio from file path. It will also call Create() for you.
        /// Note that this will load the file from disc every single time
        /// </summary>
        /// <param name="filePath">Path to the ogg file</param>
        /// <param name="busID">The bus to play the audio on (0 for default bus)</param>
        /// <returns>voiceID for controlling the audio</returns>
        [DllImport("SaXAudio")]
        public static extern Int32 PlayOggFile(string filePath, Int32 busID = 0);

        /// <summary>
        /// Pause all playing voices
        /// </summary>
        /// <param name="fade">The duration of the fade in seconds</param>
        /// <param name="busID">Specific bus to pause (0 to pause all voices)</param>
        [DllImport("SaXAudio")]
        public static extern void PauseAll(Single fade = 0.1f, Int32 busID = 0);

        /// <summary>
        /// Resume playing all voices
        /// </summary>
        /// <param name="fade">The duration of the fade in seconds</param>
        /// <param name="busID">Specific bus to resume (0 to resume all voices)</param>
        [DllImport("SaXAudio")]
        public static extern void ResumeAll(Single fade = 0.1f, Int32 busID = 0);

        /// <summary>
        /// Stop all voices
        /// </summary>
        /// <param name="fade">Fade duration in seconds</param>
        /// <param name="busID">Specific bus to stop (0 to stop all voices)</param>
        [DllImport("SaXAudio")]
        public static extern void StopAll(Single fade = 0.1f, Int32 busID = 0);

        /// <summary>
        /// Protect a voice from PauseAll, ResumeAll and StopAll operations
        /// </summary>
        /// <param name="voiceID">The voice to protect</param>
        [DllImport("SaXAudio")]
        public static extern void Protect(Int32 voiceID);

        /// <summary>
        /// Add ogg audio data to the sound bank
        /// The data in the buffer will be decoded (async) and stored in memory
        /// The buffer will not be freed/deleted
        /// </summary>
        /// <param name="buffer">The ogg data buffer</param>
        /// <param name="length">The length in bytes of the data</param>
        /// <param name="callback">Gets called when decoding is done. Allows cleaning up resources (i.e. delete buffer)</param>
        /// <returns>unique bankID for that audio data</returns>
        [DllImport("SaXAudio")]
        public static extern Int32 BankAddOgg(IntPtr buffer, UInt32 length, OnDecodedDelegate callback);

        /// <summary>
        /// Load audio from file path into sound bank
        /// </summary>
        /// <param name="filePath">Path to the ogg file</param>
        /// <returns>bankID for the loaded audio</returns>
        [DllImport("SaXAudio")]
        public static extern Int32 BankLoadOggFile(string filePath);

        /// <summary>
        /// Remove and free the memory of the specified audio data
        /// </summary>
        /// <param name="bankID">The bankID of the data to remove</param>
        [DllImport("SaXAudio")]
        public static extern void BankRemove(Int32 bankID);

        /// <summary>
        /// Flags the bank entry to be automatically removed once all voices playing it finish
        /// At least one voice must play the entry once the flag is set for the removal to happen
        /// </summary>
        /// <param name="bankID">The bankID of the data to remove</param>
        [DllImport("SaXAudio")]
        public static extern void BankAutoRemove(Int32 bankID);

        /// <summary>
        /// Create a voice for playing the specified audio data
        /// When the audio data finished playing, the voice will be deleted
        /// </summary>
        /// <param name="bankID">The bankID of the data to play</param>
        /// <param name="busID">The bus to play the voice on</param>
        /// <param name="paused">when false, the audio will start playing immediately</param>
        /// <returns>unique voiceID</returns>
        [DllImport("SaXAudio")]
        public static extern Int32 CreateVoice(Int32 bankID, Int32 busID = 0, Boolean paused = true);

        /// <summary>
        /// Check if the specified voice exists
        /// </summary>
        /// <param name="voiceID">The voice to check</param>
        /// <returns>true if the specified voice exists</returns>
        [DllImport("SaXAudio")]
        public static extern Boolean VoiceExist(Int32 voiceID);

        /// <summary>
        /// Create a bus
        /// </summary>
        /// <returns>unique busID</returns>
        [DllImport("SaXAudio")]
        public static extern Int32 CreateBus();

        /// <summary>
        /// Removes a bus
        /// All voices on that bus will be stopped
        /// </summary>
        /// <param name="busID">The bus to remove</param>
        [DllImport("SaXAudio")]
        public static extern void RemoveBus(Int32 busID);

        /// <summary>
        /// Starts playing the specified voice
        /// Resets the pause stack
        /// </summary>
        /// <param name="voiceID">The voice to start</param>
        /// <returns>true if successful or already playing</returns>
        [DllImport("SaXAudio")]
        public static extern Boolean Start(Int32 voiceID);

        /// <summary>
        /// Starts playing the specified voice at a specific sample
        /// This can be called while playing to perform a seek
        /// </summary>
        /// <param name="voiceID">The voice to start</param>
        /// <param name="sample">The sample position to start at</param>
        /// <returns>true if successful</returns>
        [DllImport("SaXAudio")]
        public static extern Boolean StartAtSample(Int32 voiceID, UInt32 sample);

        /// <summary>
        /// Starts playing the specified voice at a specific time in seconds
        /// This can be called while playing to perform a seek
        /// </summary>
        /// <param name="voiceID">The voice to start</param>
        /// <param name="time">The time position in seconds to start at</param>
        /// <returns>true if successful</returns>
        [DllImport("SaXAudio")]
        public static extern Boolean StartAtTime(Int32 voiceID, Single time);

        /// <summary>
        /// Stops playing the specified voice
        /// This will also delete the voice
        /// </summary>
        /// <param name="voiceID">The voice to stop</param>
        /// <param name="fade">Fade duration in seconds</param>
        /// <returns>true if successful</returns>
        [DllImport("SaXAudio")]
        public static extern Boolean Stop(Int32 voiceID, Single fade = 0.1f);

        /// <summary>
        /// Pause the voice and increase the pause stack.
        /// About the pause stack: if a voice is paused more than once, it will take
        /// the same amount of resumes for it to start playing again.
        /// </summary>
        /// <param name="voiceID">The voice to pause</param>
        /// <param name="fade">The duration of the fade in seconds</param>
        /// <returns>value of the pause stack</returns>
        [DllImport("SaXAudio")]
        public static extern UInt32 Pause(Int32 voiceID, Single fade = 0.1f);

        /// <summary>
        /// Reduce the pause stack and resume playing the voice if the stack becomes empty
        /// </summary>
        /// <param name="voiceID">The voice to resume</param>
        /// <param name="fade">The duration of the fade in seconds</param>
        /// <returns>value of the pause stack</returns>
        [DllImport("SaXAudio")]
        public static extern UInt32 Resume(Int32 voiceID, Single fade = 0.1f);

        /// <summary>
        /// Gets the pause stack value of the specified voice, 0 if empty
        /// </summary>
        /// <param name="voiceID">The voice to check</param>
        /// <returns>The current pause stack value</returns>
        [DllImport("SaXAudio")]
        public static extern UInt32 GetPauseStack(Int32 voiceID);

        /// <summary>
        /// Set the master volume
        /// </summary>
        /// <param name="volume">Master volume [0, 1]</param>
        /// <param name="fade">Fade duration in seconds</param>
        [DllImport("SaXAudio")]
        public static extern void SetMasterVolume(Single volume, Single fade = 0.1f);

        /// <summary>
        /// Sets the volume of the voice or bus
        /// </summary>
        /// <param name="voiceID">The voice or bus ID to modify</param>
        /// <param name="volume">Volume level [0, 1]</param>
        /// <param name="fade">Fade duration in seconds</param>
        /// <param name="isBus">true if voiceID refers to a bus, false for voice</param>
        [DllImport("SaXAudio")]
        public static extern void SetVolume(Int32 voiceID, Single volume, Single fade = 0.1f, Boolean isBus = false);

        /// <summary>
        /// Sets the playback speed of the voice
        /// This will affect the pitch of the audio
        /// </summary>
        /// <param name="voiceID">The voice to modify</param>
        /// <param name="speed">Playback speed multiplier</param>
        /// <param name="fade">Fade duration in seconds</param>
        [DllImport("SaXAudio")]
        public static extern void SetSpeed(Int32 voiceID, Single speed, Single fade = 0.1f);

        /// <summary>
        /// Sets the panning of the voice
        /// </summary>
        /// <param name="voiceID">The voice to modify</param>
        /// <param name="panning">Pan position [-1, 1] where -1 is full left, 1 is full right</param>
        /// <param name="fade">Fade duration in seconds</param>
        [DllImport("SaXAudio")]
        public static extern void SetPanning(Int32 voiceID, Single panning, Single fade = 0.1f);

        /// <summary>
        /// Sets if the voice should loop
        /// </summary>
        /// <param name="voiceID">The voice to modify</param>
        /// <param name="looping">true to enable looping, false to disable</param>
        [DllImport("SaXAudio")]
        public static extern void SetLooping(Int32 voiceID, Boolean looping);

        /// <summary>
        /// Sets the start and end looping points
        /// By default the looping points are [0, last sample]
        /// </summary>
        /// <param name="voiceID">The voice to modify</param>
        /// <param name="start">Loop start position in samples</param>
        /// <param name="end">Loop end position in samples</param>
        [DllImport("SaXAudio")]
        public static extern void SetLoopPoints(Int32 voiceID, UInt32 start, UInt32 end);

        /// <summary>
        /// Get the master volume
        /// </summary>
        /// <returns>Master volume [0, 1]</returns>
        [DllImport("SaXAudio")]
        public static extern Single GetMasterVolume();

        /// <summary>
        /// Gets the volume of the specified voice
        /// </summary>
        /// <param name="voiceID">The voice to query</param>
        /// <returns>Volume level [0, 1]</returns>
        [DllImport("SaXAudio")]
        public static extern Single GetVolume(Int32 voiceID);

        /// <summary>
        /// Gets the speed of the specified voice
        /// </summary>
        /// <param name="voiceID">The voice to query</param>
        /// <returns>Playback speed multiplier</returns>
        [DllImport("SaXAudio")]
        public static extern Single GetSpeed(Int32 voiceID);

        /// <summary>
        /// Gets the panning of the specified voice
        /// </summary>
        /// <param name="voiceID">The voice to query</param>
        /// <returns>Pan position [-1, 1] where -1 is full left, 1 is full right</returns>
        [DllImport("SaXAudio")]
        public static extern Single GetPanning(Int32 voiceID);

        /// <summary>
        /// Gets whether the specified voice is looping
        /// </summary>
        /// <param name="voiceID">The voice to query</param>
        /// <returns>true if looping is enabled, false otherwise</returns>
        [DllImport("SaXAudio")]
        public static extern Boolean GetLooping(Int32 voiceID);

        /// <summary>
        /// Gets the loop start point
        /// </summary>
        /// <param name="voiceID">The voice to query</param>
        /// <returns>Loop start position in samples</returns>
        [DllImport("SaXAudio")]
        public static extern UInt32 GetLoopStart(Int32 voiceID);

        /// <summary>
        /// Gets the loop end point
        /// </summary>
        /// <param name="voiceID">The voice to query</param>
        /// <returns>Loop end position in samples</returns>
        [DllImport("SaXAudio")]
        public static extern UInt32 GetLoopEnd(Int32 voiceID);

        /// <summary>
        /// Add/Modify the reverb effect to a voice or bus
        /// </summary>
        /// <param name="voiceID">The voice or bus ID to modify</param>
        /// <param name="reverbParams">Reverb parameters</param>
        /// <param name="fade">Fade duration in seconds</param>
        /// <param name="isBus">true if voiceID refers to a bus, false for voice</param>
        [DllImport("SaXAudio")]
        public static extern void SetReverb(Int32 voiceID, ReverbParameters reverbParams, Single fade = 0, Boolean isBus = false);

        /// <summary>
        /// Remove the reverb effect from a voice or bus
        /// </summary>
        /// <param name="voiceID">The voice or bus ID to modify</param>
        /// <param name="fade">Fade duration in seconds</param>
        /// <param name="isBus">true if voiceID refers to a bus, false for voice</param>
        [DllImport("SaXAudio")]
        public static extern void RemoveReverb(Int32 voiceID, Single fade = 0, Boolean isBus = false);

        /// <summary>
        /// Add/Modify the EQ effect to a voice or bus
        /// </summary>
        /// <param name="voiceID">The voice or bus ID to modify</param>
        /// <param name="eqParams">EQ parameters</param>
        /// <param name="fade">Fade duration in seconds</param>
        /// <param name="isBus">true if voiceID refers to a bus, false for voice</param>
        [DllImport("SaXAudio")]
        public static extern void SetEq(Int32 voiceID, EqParameters eqParams, Single fade = 0, Boolean isBus = false);

        /// <summary>
        /// Remove the EQ effect from a voice or bus
        /// </summary>
        /// <param name="voiceID">The voice or bus ID to modify</param>
        /// <param name="fade">Fade duration in seconds</param>
        /// <param name="isBus">true if voiceID refers to a bus, false for voice</param>
        [DllImport("SaXAudio")]
        public static extern void RemoveEq(Int32 voiceID, Single fade = 0, Boolean isBus = false);

        /// <summary>
        /// Add/Modify the echo effect to a voice or bus
        /// </summary>
        /// <param name="voiceID">The voice or bus ID to modify</param>
        /// <param name="echoParams">Echo parameters</param>
        /// <param name="fade">Fade duration in seconds</param>
        /// <param name="isBus">true if voiceID refers to a bus, false for voice</param>
        [DllImport("SaXAudio")]
        public static extern void SetEcho(Int32 voiceID, EchoParameters echoParams, Single fade = 0, Boolean isBus = false);

        /// <summary>
        /// Remove the echo effect from a voice or bus
        /// </summary>
        /// <param name="voiceID">The voice or bus ID to modify</param>
        /// <param name="fade">Fade duration in seconds</param>
        /// <param name="isBus">true if voiceID refers to a bus, false for voice</param>
        [DllImport("SaXAudio")]
        public static extern void RemoveEcho(Int32 voiceID, Single fade = 0, Boolean isBus = false);

        /// <summary>
        /// Gets the position of the playing voice in samples
        /// </summary>
        /// <param name="voiceID">The voice to query</param>
        /// <returns>sample position if playing or 0</returns>
        [DllImport("SaXAudio")]
        public static extern UInt32 GetPositionSample(Int32 voiceID);

        /// <summary>
        /// Gets the position of the playing voice in seconds
        /// </summary>
        /// <param name="voiceID">The voice to query</param>
        /// <returns>position in seconds if playing or 0</returns>
        [DllImport("SaXAudio")]
        public static extern Single GetPositionTime(Int32 voiceID);

        /// <summary>
        /// Gets the total amount of samples of the voice
        /// </summary>
        /// <param name="voiceID">The voice to query</param>
        /// <returns>Total number of samples in the audio</returns>
        [DllImport("SaXAudio")]
        public static extern UInt32 GetTotalSample(Int32 voiceID);

        /// <summary>
        /// Gets the total time, in seconds, of the voice
        /// </summary>
        /// <param name="voiceID">The voice to query</param>
        /// <returns>Total duration in seconds</returns>
        [DllImport("SaXAudio")]
        public static extern Single GetTotalTime(Int32 voiceID);

        /// <summary>
        /// Get the sample rate of the audio data
        /// </summary>
        /// <param name="voiceID">The voice to query</param>
        /// <returns>sample rate in Hz</returns>
        [DllImport("SaXAudio")]
        public static extern UInt32 GetSampleRate(Int32 voiceID);

        /// <summary>
        /// Get the number of channels of the audio data
        /// </summary>
        /// <param name="voiceID">The voice to query</param>
        /// <returns>number of channels (1=mono, 2=stereo, etc)</returns>
        [DllImport("SaXAudio")]
        public static extern UInt32 GetChannelCount(Int32 voiceID);

        /// <summary>
        /// Get the number of voices
        /// </summary>
        /// <returns>Number of currently active voices</returns>
        [DllImport("SaXAudio")]
        public static extern UInt32 GetVoiceCount();

        /// <summary>
        /// Get the number of loaded audio banks
        /// </summary>
        /// <returns>Number of loaded banks</returns>
        [DllImport("SaXAudio")]
        public static extern UInt32 GetBankCount();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnDecodedDelegate(Int32 bankID, IntPtr buffer);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OnFinishedDelegate(Int32 voiceID);

        /// <summary>
        /// Sets a callback for when a voice finishes playing
        /// </summary>
        /// <param name="callback">The callback function to call when voices finish</param>
        [DllImport("SaXAudio")]
        private static extern void SetOnFinishedCallback(OnFinishedDelegate callback);

        private static void TriggerOnFinished(Int32 voiceID)
        {
            OnVoiceFinished?.Invoke(voiceID);
        }
    }
}

// MIT License
// 
// Copyright(c) 2025 SamsamTS
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and /or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#pragma once

#include "Includes.h"

namespace SaXAudio
{
    /// <summary>
    /// Initialize XAudio and create a mastering voice
    /// </summary>
    /// <returns>Return true if successful</returns>
    EXPORT BOOL Create();
    /// <summary>
    /// Release everything
    /// </summary>
    EXPORT void Release();

    /// <summary>
    /// Resume playing all voices
    /// </summary>
    EXPORT void StartEngine();
    /// <summary>
    /// Pause all playing voices
    /// </summary>
    EXPORT void StopEngine();


    /// <summary>
    /// That's the "Just play the darn thing!" button for ya
    /// Load and play audio from file path. It will also call Create() for you.
    /// Note that this will load the file from disc every single time
    /// </summary>
    /// <param name="filePath">Path to the ogg file</param>
    /// <param name="busID">The bus to play the audio on (0 for default bus)</param>
    /// <returns>voiceID for controlling the audio</returns>
    EXPORT INT32 PlayWavFile(const char* filePath, const INT32 busID = 0);
    /// <summary>
    /// That's the "Just play the darn thing!" button for ya
    /// Load and play audio from file path. It will also call Create() for you.
    /// Note that this will load the file from disc every single time
    /// </summary>
    /// <param name="filePath">Path to the ogg file</param>
    /// <param name="busID">The bus to play the audio on (0 for default bus)</param>
    /// <returns>voiceID for controlling the audio</returns>
    EXPORT INT32 PlayOggFile(const char* filePath, const INT32 busID = 0);

    /// <summary>
    /// Pause all playing voices
    /// </summary>
    /// <param name="fade">The duration of the fade in seconds</param>
    /// <param name="busID">Specific bus to pause (0 to pause all voices)</param>
    EXPORT void PauseAll(const FLOAT fade = 0.1f, const INT32 busID = 0);
    /// <summary>
    /// Resume playing all voices
    /// </summary>
    /// <param name="fade">The duration of the fade in seconds</param>
    /// <param name="busID">Specific bus to resume (0 to resume all voices)</param>
    EXPORT void ResumeAll(const FLOAT fade = 0.1f, const INT32 busID = 0);
    /// <summary>
    /// Stop all voices
    /// </summary>
    /// <param name="fade">Fade duration in seconds</param>
    /// <param name="busID">Specific bus to stop (0 to stop all voices)</param>
    EXPORT void StopAll(const FLOAT fade = 0.1f, const INT32 busID = 0);
    /// <summary>
    /// Protect a voice from PauseAll, ResumeAll and StopAll operations
    /// </summary>
    /// <param name="voiceID">The voice to protect</param>
    EXPORT void Protect(const INT32 voiceID);

    /// <summary>
    /// Add wav audio data to the sound bank
    /// The data in the buffer will be copied in memory
    /// The buffer can be freed/deleted immediately
    /// </summary>
    /// <param name="buffer">The wav data buffer</param>
    /// <param name="length">The length in bytes of the data</param>
    /// <returns>unique bankID for that audio data</returns>
    EXPORT INT32 BankAddWav(const BYTE* buffer, const UINT32 length);
    /// <summary>
    /// Load audio from file path into sound bank
    /// </summary>
    /// <param name="filePath">Path to the wav file</param>
    /// <returns>bankID for the loaded audio</returns>
    EXPORT INT32 BankLoadWavFile(const char* filePath);

    /// <summary>
    /// Add ogg audio data to the sound bank
    /// The data in the buffer will be decoded (async) and stored in memory
    /// The buffer cannot be freed/deleted immediately but can be done safely once decoded (i.e. during OnDecodedCallback)
    /// </summary>
    /// <param name="buffer">The ogg data buffer</param>
    /// <param name="length">The length in bytes of the data</param>
    /// <param name="callback">Gets called when decoding is done. Allows cleaning up resources (i.e. delete buffer)</param>
    /// <returns>unique bankID for that audio data</returns>
    EXPORT INT32 BankAddOgg(const BYTE* buffer, const UINT32 length, const OnDecodedCallback callback = nullptr);
    /// <summary>
    /// Load audio from file path into sound bank
    /// </summary>
    /// <param name="filePath">Path to the ogg file</param>
    /// <returns>bankID for the loaded audio</returns>
    EXPORT INT32 BankLoadOggFile(const char* filePath);
    /// <summary>
    /// Remove and free the memory of the specified audio data
    /// Voices still playing the bank data will not be stopped and will continue playing
    /// </summary>
    /// <param name="bankID">The bankID of the data to remove</param>
    EXPORT void BankRemove(const INT32 bankID);
    /// <summary>
    /// Flags the bank entry to be automatically removed once all voices playing it finish
    /// At least one voice must play the entry once the flag is set for the removal to happen
    /// </summary>
    /// <param name="bankID">The bankID of the data to remove</param>
    EXPORT void BankAutoRemove(const INT32 bankID);

    /// <summary>
    /// Create a voice for playing the specified audio data
    /// When the audio data finished playing, the voice will be deleted
    /// </summary>
    /// <param name="bankID">The bankID of the data to play</param>
    /// <param name="busID">The bus to play the voice on</param>
    /// <param name="paused">when false, the audio will start playing immediately</param>
    /// <returns>unique voiceID</returns>
    EXPORT INT32 CreateVoice(const INT32 bankID, const INT32 busID, const BOOL paused = true);
    /// <summary>
    /// Check if the specified voice exists
    /// </summary>
    /// <param name="voiceID">The voice to check</param>
    /// <returns>true if the specified voice exists</returns>
    EXPORT BOOL VoiceExist(const INT32 voiceID);

    /// <summary>
    /// Create a bus
    /// </summary>
    /// <returns>unique busID</returns>
    EXPORT INT32 CreateBus();
    /// <summary>
    /// Removes a bus
    /// All voices on that bus will be stopped
    /// </summary>
    /// <param name="busID">The bus to remove</param>
    EXPORT void RemoveBus(INT32 busID);

    /// <summary>
    /// Starts playing the specified voice
    /// Resets the pause stack
    /// </summary>
    /// <param name="voiceID">The voice to start</param>
    /// <returns>true if successful or already playing</returns>
    EXPORT BOOL Start(const INT32 voiceID);
    /// <summary>
    /// Starts playing the specified voice at a specific sample
    /// This can be called while playing to perform a seek
    /// </summary>
    /// <param name="voiceID">The voice to start</param>
    /// <param name="sample">The sample position to start at</param>
    /// <returns>true if successful</returns>
    EXPORT BOOL StartAtSample(const INT32 voiceID, const UINT32 sample);
    /// <summary>
    /// Starts playing the specified voice at a specific time in seconds
    /// This can be called while playing to perform a seek
    /// </summary>
    /// <param name="voiceID">The voice to start</param>
    /// <param name="time">The time position in seconds to start at</param>
    /// <returns>true if successful</returns>
    EXPORT BOOL StartAtTime(const INT32 voiceID, const FLOAT time);
    /// <summary>
    /// Stops playing the specified voice
    /// This will also delete the voice
    /// </summary>
    /// <param name="voiceID">The voice to stop</param>
    /// <param name="fade">Fade duration in seconds</param>
    /// <returns>true if successful</returns>
    EXPORT BOOL Stop(const INT32 voiceID, const FLOAT fade = 0.1f);

    /// <summary>
    /// Pause the voice and increase the pause stack.
    /// About the pause stack: if a voice is paused more than once, it will take
    /// the same amount of resumes for it to start playing again.
    /// </summary>
    /// <param name="voiceID">The voice to pause</param>
    /// <param name="fade">The duration of the fade in seconds</param>
    /// <returns>value of the pause stack</returns>
    EXPORT UINT32 Pause(const INT32 voiceID, const FLOAT fade = 0.1f);
    /// <summary>
    /// Reduce the pause stack and resume playing the voice if the stack becomes empty
    /// </summary>
    /// <param name="voiceID">The voice to resume</param>
    /// <param name="fade">The duration of the fade in seconds</param>
    /// <returns>value of the pause stack</returns>
    EXPORT UINT32 Resume(const INT32 voiceID, const FLOAT fade = 0.1f);
    /// <summary>
    /// Gets the pause stack value of the specified voice, 0 if empty
    /// </summary>
    /// <param name="voiceID">The voice to check</param>
    /// <returns>The current pause stack value</returns>
    EXPORT UINT32 GetPauseStack(const INT32 voiceID);

    /// <summary>
    /// Set the master volume
    /// </summary>
    /// <param name="volume">Master volume [0, 1]</param>
    /// <param name="fade">Fade duration in seconds</param>
    EXPORT void SetMasterVolume(const FLOAT volume, const FLOAT fade = 0);
    /// <summary>
    /// Sets the volume of the voice or bus
    /// </summary>
    /// <param name="voiceID">The voice or bus ID to modify</param>
    /// <param name="volume">Volume level [0, 1]</param>
    /// <param name="fade">Fade duration in seconds</param>
    /// <param name="isBus">true if voiceID refers to a bus, false for voice</param>
    EXPORT void SetVolume(const INT32 voiceID, const FLOAT volume, const FLOAT fade = 0, BOOL isBus = false);
    /// <summary>
    /// Sets the playback speed of the voice
    /// This will affect the pitch of the audio
    /// </summary>
    /// <param name="voiceID">The voice to modify</param>
    /// <param name="speed">Playback speed multiplier</param>
    /// <param name="fade">Fade duration in seconds</param>
    EXPORT void SetSpeed(const INT32 voiceID, const FLOAT speed, const FLOAT fade = 0);
    /// <summary>
    /// Sets the panning of the voice
    /// </summary>
    /// <param name="voiceID">The voice to modify</param>
    /// <param name="panning">Pan position [-1, 1] where -1 is full left, 1 is full right</param>
    /// <param name="fade">Fade duration in seconds</param>
    EXPORT void SetPanning(const INT32 voiceID, const FLOAT panning, const FLOAT fade = 0);
    /// <summary>
    /// Sets if the voice should loop
    /// </summary>
    /// <param name="voiceID">The voice to modify</param>
    /// <param name="looping">true to enable looping, false to disable</param>
    EXPORT void SetLooping(const INT32 voiceID, const BOOL looping);
    /// <summary>
    /// Sets the start and end looping points
    /// By default the looping points are [0, last sample]
    /// </summary>
    /// <param name="voiceID">The voice to modify</param>
    /// <param name="start">Loop start position in samples</param>
    /// <param name="end">Loop end position in samples</param>
    EXPORT void SetLoopPoints(const INT32 voiceID, const UINT32 start, const UINT32 end);

    /// <summary>
    /// Get the master volume
    /// </summary>
    /// <returns>Master volume [0, 1]</returns>
    EXPORT FLOAT GetMasterVolume();
    /// <summary>
    /// Gets the volume of the specified voice
    /// </summary>
    /// <param name="voiceID">The voice to query</param>
    /// <returns>Volume level [0, 1]</returns>
    EXPORT FLOAT GetVolume(const INT32 voiceID);
    /// <summary>
    /// Gets the speed of the specified voice
    /// </summary>
    /// <param name="voiceID">The voice to query</param>
    /// <returns>Playback speed multiplier</returns>
    EXPORT FLOAT GetSpeed(const INT32 voiceID);
    /// <summary>
    /// Gets the panning of the specified voice
    /// </summary>
    /// <param name="voiceID">The voice to query</param>
    /// <returns>Pan position [-1, 1] where -1 is full left, 1 is full right</returns>
    EXPORT FLOAT GetPanning(const INT32 voiceID);
    /// <summary>
    /// Gets whether the specified voice is looping
    /// </summary>
    /// <param name="voiceID">The voice to query</param>
    /// <returns>true if looping is enabled, false otherwise</returns>
    EXPORT BOOL GetLooping(const INT32 voiceID);
    /// <summary>
    /// Gets the loop start point
    /// </summary>
    /// <param name="voiceID">The voice to query</param>
    /// <returns>Loop start position in samples</returns>
    EXPORT UINT32 GetLoopStart(const INT32 voiceID);
    /// <summary>
    /// Gets the loop end point
    /// </summary>
    /// <param name="voiceID">The voice to query</param>
    /// <returns>Loop end position in samples</returns>
    EXPORT UINT32 GetLoopEnd(const INT32 voiceID);

    /// <summary>
    /// Add/Modify the reverb effect to a voice or bus
    /// </summary>
    /// <param name="voiceID">The voice or bus ID to modify</param>
    /// <param name="reverbParams">Reverb parameters</param>
    /// <param name="fade">Fade duration in seconds</param>
    /// <param name="isBus">true if voiceID refers to a bus, false for voice</param>
    EXPORT void SetReverb(const INT32 voiceID, const XAUDIO2FX_REVERB_PARAMETERS reverbParams, const FLOAT fade = 0, BOOL isBus = false);
    /// <summary>
    /// Remove the reverb effect from a voice or bus
    /// </summary>
    /// <param name="voiceID">The voice or bus ID to modify</param>
    /// <param name="fade">Fade duration in seconds</param>
    /// <param name="isBus">true if voiceID refers to a bus, false for voice</param>
    EXPORT void RemoveReverb(const INT32 voiceID, const FLOAT fade = 0, BOOL isBus = false);

    /// <summary>
    /// Add/Modify the EQ effect to a voice or bus
    /// </summary>
    /// <param name="voiceID">The voice or bus ID to modify</param>
    /// <param name="eqParams">EQ parameters</param>
    /// <param name="fade">Fade duration in seconds</param>
    /// <param name="isBus">true if voiceID refers to a bus, false for voice</param>
    EXPORT void SetEq(const INT32 voiceID, const FXEQ_PARAMETERS eqParams, const FLOAT fade = 0, BOOL isBus = false);
    /// <summary>
    /// Remove the EQ effect from a voice or bus
    /// </summary>
    /// <param name="voiceID">The voice or bus ID to modify</param>
    /// <param name="fade">Fade duration in seconds</param>
    /// <param name="isBus">true if voiceID refers to a bus, false for voice</param>
    EXPORT void RemoveEq(const INT32 voiceID, const FLOAT fade = 0, BOOL isBus = false);

    /// <summary>
    /// Add/Modify the echo effect to a voice or bus
    /// </summary>
    /// <param name="voiceID">The voice or bus ID to modify</param>
    /// <param name="echoParams">Echo parameters</param>
    /// <param name="fade">Fade duration in seconds</param>
    /// <param name="isBus">true if voiceID refers to a bus, false for voice</param>
    EXPORT void SetEcho(const INT32 voiceID, const FXECHO_PARAMETERS echoParams, const FLOAT fade = 0, BOOL isBus = false);
    /// <summary>
    /// Remove the echo effect from a voice or bus
    /// </summary>
    /// <param name="voiceID">The voice or bus ID to modify</param>
    /// <param name="fade">Fade duration in seconds</param>
    /// <param name="isBus">true if voiceID refers to a bus, false for voice</param>
    EXPORT void RemoveEcho(const INT32 voiceID, const FLOAT fade = 0, BOOL isBus = false);

    /// <summary>
    /// Gets the position of the playing voice in samples
    /// </summary>
    /// <param name="voiceID">The voice to query</param>
    /// <returns>sample position if playing or 0</returns>
    EXPORT UINT32 GetPositionSample(const INT32 voiceID);
    /// <summary>
    /// Gets the position of the playing voice in seconds
    /// </summary>
    /// <param name="voiceID">The voice to query</param>
    /// <returns>position in seconds if playing or 0</returns>
    EXPORT FLOAT GetPositionTime(const INT32 voiceID);

    /// <summary>
    /// Gets the total amount of samples of the voice
    /// </summary>
    /// <param name="voiceID">The voice to query</param>
    /// <returns>Total number of samples in the audio</returns>
    EXPORT UINT32 GetTotalSample(const INT32 voiceID);
    /// <summary>
    /// Gets the total time, in seconds, of the voice
    /// </summary>
    /// <param name="voiceID">The voice to query</param>
    /// <returns>Total duration in seconds</returns>
    EXPORT FLOAT GetTotalTime(const INT32 voiceID);

    /// <summary>
    /// Get the sample rate of the audio data
    /// </summary>
    /// <param name="voiceID">The voice to query</param>
    /// <returns>sample rate in Hz</returns>
    EXPORT UINT32 GetSampleRate(const INT32 voiceID);

    /// <summary>
    /// Get the number of channels of the audio data
    /// </summary>
    /// <param name="voiceID">The voice to query</param>
    /// <returns>number of channels (1=mono, 2=stereo, etc)</returns>
    EXPORT UINT32 GetChannelCount(const INT32 voiceID);

    /// <summary>
    /// Sets a callback for when a voice finishes playing
    /// </summary>
    /// <param name="callback">The callback function to call when voices finish</param>
    EXPORT void SetOnFinishedCallback(const OnFinishedCallback callback);

    /// <summary>
    /// Get the number of voices
    /// </summary>
    /// <param name="bankID">Filter by bankID or 0 for any bankID</param>
    /// <param name="busID">Filter by BusID or 0 for any busID</param>
    /// <returns>Number of currently active voices</returns>
    EXPORT UINT32 GetVoiceCount(const INT32 bankID = 0, const INT32 busID = 0);

    /// <summary>
    /// Get the number of loaded audio banks
    /// </summary>
    /// <returns>Number of loaded banks</returns>
    EXPORT UINT32 GetBankCount();

    /// <summary>
    /// Get the peak volume level (for VU meters, etc.)
    /// </summary>
    /// <param name="voiceID">The voice to query</param>
    /// <param name="channelIndex">Channel to get peak for (0=left, 1=right, etc.)</param>
    /// <returns>Peak level [0, 1]</returns>
    // TODO EXPORT FLOAT GetPeakLevel(const INT32 voiceID, const UINT32 channelIndex = 0);
}
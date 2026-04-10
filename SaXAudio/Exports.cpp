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

#include "SaXAudio.h"
#include "Exports.h"

BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

namespace SaXAudio
{
    EXPORT BOOL Create()
    {
        return SaXAudio::Instance.Init();
    }

    EXPORT void Release()
    {
        SaXAudio::Instance.Release();
    }

    EXPORT void StartEngine()
    {
        SaXAudio::Instance.StartEngine();
    }

    EXPORT void StopEngine()
    {
        SaXAudio::Instance.StopEngine();
    }

    EXPORT INT32 PlayWavFile(const char* filePath, const INT32 busID)
    {
        SaXAudio::Instance.Init();

        INT32 bankID = BankLoadWavFile(filePath);
        SaXAudio::Instance.AutoRemoveBank(bankID);
        return CreateVoice(bankID, busID, false);
    }

    EXPORT INT32 PlayOggFile(const char* filePath, const INT32 busID)
    {
        SaXAudio::Instance.Init();

        INT32 bankID = BankLoadOggFile(filePath);
        SaXAudio::Instance.AutoRemoveBank(bankID);
        return CreateVoice(bankID, busID, false);
    }

    EXPORT void PauseAll(const FLOAT fade, const INT32 busID)
    {
        SaXAudio::Instance.PauseAll(fade, busID);
    }

    EXPORT void ResumeAll(const FLOAT fade, const INT32 busID)
    {
        SaXAudio::Instance.ResumeAll(fade, busID);
    }

    EXPORT void StopAll(const FLOAT fade, const INT32 busID)
    {
        SaXAudio::Instance.StopAll(fade, busID);
    }

    EXPORT void Protect(const INT32 voiceID)
    {
        SaXAudio::Instance.Protect(voiceID);
    }

    // WAV file header structures
#pragma pack(push, 1)
    struct WavHeader
    {
        char riff[4];           // "RIFF"
        UINT32 fileSize;        // File size - 8
        char wave[4];           // "WAVE"
        char fmt[4];            // "fmt "
        UINT32 fmtSize;         // Format chunk size
        UINT16 audioFormat;     // Audio format (WAVE_FORMAT_IEEE_FLOAT)
        UINT16 channels;        // Number of channels
        UINT32 sampleRate;      // Sample rate
        UINT32 byteRate;        // Byte rate (nSamplesPerSec * nBlockAlign)
        UINT16 blockAlign;      // Block alignment (nChannels * wBitsPerSample / 8)
        UINT16 bitsPerSample;   // Bits per sample (32 for IEEE float)
        char data[4];           // "data"
        UINT32 dataSize;        // Data chunk size
    };
#pragma pack(pop)

    // Convert 8-bit PCM to 32-bit float
    void ConvertPCM8ToFloat(const BYTE* pcmData, FLOAT* floatData, UINT32 sampleCount)
    {
        for (UINT32 i = 0; i < sampleCount; i++)
        {
            // 8-bit PCM is unsigned, convert to signed then to float
            INT8 signedValue = static_cast<INT8>(pcmData[i] - 128);
            floatData[i] = static_cast<FLOAT>(signedValue) / 128.0f;
        }
    }

    // Convert 16-bit PCM to 32-bit float
    void ConvertPCM16ToFloat(const INT16* pcmData, FLOAT* floatData, UINT32 sampleCount)
    {
        for (UINT32 i = 0; i < sampleCount; i++)
        {
            floatData[i] = static_cast<FLOAT>(pcmData[i]) / 32768.0f;
        }
    }

    // Convert 24-bit PCM to 32-bit float
    void ConvertPCM24ToFloat(const BYTE* pcmData, FLOAT* floatData, UINT32 sampleCount)
    {
        for (UINT32 i = 0; i < sampleCount; i++)
        {
            // Extract 24-bit value (little endian)
            INT32 value = (pcmData[i * 3] << 8) | (pcmData[i * 3 + 1] << 16) | (pcmData[i * 3 + 2] << 24);
            value >>= 8; // Sign extend from 24 to 32 bits

            floatData[i] = static_cast<FLOAT>(value) / 8388608.0f; // 2^23
        }
    }

    // Convert 32-bit PCM to 32-bit float
    void ConvertPCM32ToFloat(const INT32* pcmData, FLOAT* floatData, UINT32 sampleCount)
    {
        for (UINT32 i = 0; i < sampleCount; i++)
        {
            floatData[i] = static_cast<FLOAT>(pcmData[i]) / 2147483648.0f; // 2^31
        }
    }

    /// <summary>
    /// Add wav audio data to the sound bank
    /// The data in the buffer will be copied in memory
    /// The buffer can be freed/deleted immediately
    /// </summary>
    /// <param name="buffer">The wav data buffer</param>
    /// <param name="length">The length in bytes of the data</param>
    /// <returns>unique bankID for that audio data</returns>
    EXPORT INT32 BankAddWav(const BYTE* buffer, const UINT32 length)
    {
        if (!buffer || length < sizeof(WavHeader))
            return 0;

        const WavHeader* header = reinterpret_cast<const WavHeader*>(buffer);

        // Validate WAV header
        if (memcmp(header->riff, "RIFF", 4) != 0 ||
            memcmp(header->wave, "WAVE", 4) != 0 ||
            memcmp(header->fmt, "fmt ", 4) != 0 ||
            memcmp(header->data, "data", 4) != 0)
            return 0;

        // Check supported formats
        if (header->audioFormat != WAVE_FORMAT_PCM &&
            header->audioFormat != WAVE_FORMAT_IEEE_FLOAT)
            return 0;

        // Check supported bit depths
        if (header->audioFormat == WAVE_FORMAT_PCM)
        {
            if (header->bitsPerSample != 8 && header->bitsPerSample != 16 && header->bitsPerSample != 24 && header->bitsPerSample != 32)
                return 0;
        }
        else if (header->audioFormat == WAVE_FORMAT_IEEE_FLOAT)
        {
            if (header->bitsPerSample != 32)
                return 0;
        }

        // Calculate total samples based on input format
        UINT32 bytesPerSample = header->bitsPerSample / 8;
        UINT32 totalSamples = header->dataSize / (header->channels * bytesPerSample);

        // Allocate buffer for float data (always 32-bit float output)
        Buffer data = SaXAudio::Instance.GetBuffer(totalSamples * header->channels);
        const BYTE* audioData = buffer + sizeof(WavHeader);

        // Convert based on input format
        if (header->audioFormat == WAVE_FORMAT_IEEE_FLOAT && header->bitsPerSample == 32)
        {
            // Already in correct format, validate format requirements
            UINT16 expectedBlockAlign = header->channels * 32 / 8;
            UINT32 expectedByteRate = header->sampleRate * expectedBlockAlign;

            if (header->blockAlign != expectedBlockAlign || header->byteRate != expectedByteRate)
            {
                SaXAudio::Instance.ReturnBuffer(data);
                return 0;
            }

            // Just copy the data
            memcpy(data.Data, audioData, header->dataSize);
        }
        else if (header->audioFormat == WAVE_FORMAT_PCM)
        {
            // Convert PCM to float
            UINT32 totalSampleCount = totalSamples * header->channels;

            switch (header->bitsPerSample)
            {
            case 8:
                ConvertPCM8ToFloat(audioData, data.Data, totalSampleCount);
                break;
            case 16:
                ConvertPCM16ToFloat(reinterpret_cast<const INT16*>(audioData), data.Data, totalSampleCount);
                break;
            case 24:
                ConvertPCM24ToFloat(audioData, data.Data, totalSampleCount);
                break;
            case 32:
                ConvertPCM32ToFloat(reinterpret_cast<const INT32*>(audioData), data.Data, totalSampleCount);
                break;
            default:
                SaXAudio::Instance.ReturnBuffer(data);
                return 0;
            }
        }

        return SaXAudio::Instance.AddBankData(data, header->channels, header->sampleRate, totalSamples);
    }

    /// <summary>
    /// Load audio from file path into sound bank
    /// </summary>
    /// <param name="filePath">Path to the wav file</param>
    /// <returns>bankID for the loaded audio</returns>
    EXPORT INT32 BankLoadWavFile(const char* filePath)
    {
        ifstream file(filePath, ios::binary | ios::ate);
        if (!file)
            return 0;

        size_t length = (size_t)file.tellg();
        file.seekg(0, ios::beg);

        if (length < sizeof(WavHeader))
            return 0;

        BYTE* buffer = new BYTE[length];
        file.read(reinterpret_cast<char*>(buffer), length);
        file.close();

        INT32 bankID = BankAddWav(buffer, (UINT32)length);
        delete[] buffer;

        return bankID;
    }

    EXPORT INT32 BankAddOgg(const BYTE* buffer, const UINT32 length, const OnDecodedCallback callback)
    {
        INT32 bankID = SaXAudio::Instance.AddBankEntry(callback);
        if (bankID > 0)
            SaXAudio::Instance.StartDecodeOgg(bankID, buffer, length);
        return bankID;
    }

    void DeleteFileBuffer(INT32 bankID, const BYTE* buffer)
    {
        delete[] buffer;
    }

    EXPORT INT32 BankLoadOggFile(const char* filePath)
    {
        ifstream file(filePath, ios::binary | ios::ate);
        if (!file)
            return 0;

        size_t length = (size_t)file.tellg();
        file.seekg(0, ios::beg);

        BYTE* buffer = new BYTE[length];
        file.read(reinterpret_cast<char*>(buffer), length);
        return BankAddOgg(buffer, (UINT32)length, DeleteFileBuffer);
    }

    EXPORT void BankRemove(const INT32 bankID)
    {
        SaXAudio::Instance.RemoveBankEntry(bankID);
    }

    EXPORT void BankAutoRemove(const INT32 bankID)
    {
        SaXAudio::Instance.AutoRemoveBank(bankID);
    }

    EXPORT INT32 CreateVoice(const INT32 bankID, const INT32 busID, const BOOL paused)
    {
        AudioVoice* voice = SaXAudio::Instance.CreateVoice(bankID, busID);

        if (!voice)
            return 0;

        if (!paused)
            voice->Start();

        return voice->VoiceID;
    }

    EXPORT BOOL VoiceExist(const INT32 voiceID)
    {
        return SaXAudio::Instance.GetVoice(voiceID) != nullptr;
    }

    EXPORT INT32 CreateBus()
    {
        return SaXAudio::Instance.AddBus();
    }

    EXPORT void RemoveBus(INT32 busID)
    {
        SaXAudio::Instance.RemoveBus(busID);
    }

    EXPORT BOOL Start(const INT32 voiceID)
    {
        return StartAtSample(voiceID, 0);
    }

    EXPORT BOOL StartAtSample(const INT32 voiceID, const UINT32 sample)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice)
        {
            return voice->Start(sample);
        }
        return false;
    }

    EXPORT BOOL StartAtTime(const INT32 voiceID, const FLOAT time)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice && voice->BankData && time >= 0)
        {
            UINT32 sample = (UINT32)(time * voice->BankData->sampleRate);
            return voice->Start(sample);
        }
        return false;
    }

    EXPORT BOOL Stop(const INT32 voiceID, const FLOAT fade)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice)
        {
            return voice->Stop(fade);
        }
        return false;
    }

    EXPORT UINT32 Pause(const INT32 voiceID, const FLOAT fade)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice)
        {
            return voice->Pause(fade);
        }
        return 0;
    }

    EXPORT UINT32 Resume(const INT32 voiceID, const FLOAT fade)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice)
        {
            return voice->Resume(fade);
        }
        return 0;
    }

    EXPORT UINT32 GetPauseStack(const INT32 voiceID)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice)
        {
            return voice->GetPauseStack();
        }
        return 0;
    }

    EXPORT void SetMasterVolume(const FLOAT volume, const FLOAT fade)
    {
        SaXAudio::Instance.SetBusVolume(0, volume, fade);
    }

    EXPORT void SetVolume(const INT32 voiceID, const FLOAT volume, const FLOAT fade, BOOL isBus)
    {
        if (isBus)
        {
            SaXAudio::Instance.SetBusVolume(voiceID, volume, fade);
            return;
        }

        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice)
        {
            voice->SetVolume(volume, fade);
        }
    }

    EXPORT void SetSpeed(const INT32 voiceID, const FLOAT speed, const FLOAT fade)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice)
        {
            voice->SetSpeed(speed, fade);
        }
    }

    EXPORT void SetPanning(const INT32 voiceID, const FLOAT panning, const FLOAT fade)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice)
        {
            voice->SetPanning(panning, fade);
        }
    }

    EXPORT void SetLooping(const INT32 voiceID, const BOOL looping)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice)
        {
            voice->SetLooping(looping);
        }
    }

    EXPORT void SetLoopPoints(const INT32 voiceID, const UINT32 start, const UINT32 end)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice)
        {
            voice->ChangeLoopPoints(start, end);
        }
    }

    EXPORT FLOAT GetMasterVolume()
    {
        return SaXAudio::Instance.GetBusVolume(0);
    }

    EXPORT FLOAT GetVolume(const INT32 voiceID)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice)
        {
            return voice->Volume;
        }
        return 1.0f;
    }

    EXPORT FLOAT GetSpeed(const INT32 voiceID)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice)
        {
            return voice->Speed;
        }
        return 1.0f;
    }

    EXPORT FLOAT GetPanning(const INT32 voiceID)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice)
        {
            return voice->Panning;
        }
        return 0.0f;
    }

    EXPORT BOOL GetLooping(const INT32 voiceID)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice)
        {
            return voice->Looping;
        }
        return false;
    }

    EXPORT UINT32 GetLoopStart(const INT32 voiceID)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice)
        {
            return voice->LoopStart;
        }
        return 0;
    }

    EXPORT UINT32 GetLoopEnd(const INT32 voiceID)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice)
        {
            return voice->LoopEnd;
        }
        return 0;
    }

    EXPORT void SetReverb(const INT32 voiceID, const XAUDIO2FX_REVERB_PARAMETERS reverbParams, const FLOAT fade, BOOL isBus)
    {
        SaXAudio::Instance.SetReverb(voiceID, isBus, &reverbParams, fade);
    }

    EXPORT void RemoveReverb(const INT32 voiceID, const FLOAT fade, BOOL isBus)
    {
        SaXAudio::Instance.RemoveReverb(voiceID, isBus, fade);
    }

    EXPORT void SetEq(const INT32 voiceID, const FXEQ_PARAMETERS eqParams, const FLOAT fade, BOOL isBus)
    {
        SaXAudio::Instance.SetEq(voiceID, isBus, &eqParams, fade);
    }

    EXPORT void RemoveEq(const INT32 voiceID, const FLOAT fade, BOOL isBus)
    {
        SaXAudio::Instance.RemoveEq(voiceID, isBus, fade);
    }

    EXPORT void SetEcho(const INT32 voiceID, const FXECHO_PARAMETERS echoParams, const FLOAT fade, BOOL isBus)
    {
        SaXAudio::Instance.SetEcho(voiceID, isBus, &echoParams, fade);
    }

    EXPORT void RemoveEcho(const INT32 voiceID, const FLOAT fade, BOOL isBus)
    {
        SaXAudio::Instance.RemoveEcho(voiceID, isBus, fade);
    }

    EXPORT UINT32 GetPositionSample(const INT32 voiceID)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice)
        {
            return voice->GetPosition();
        }
        return 0;
    }

    EXPORT FLOAT GetPositionTime(const INT32 voiceID)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice && voice->BankData)
        {
            return (FLOAT)voice->GetPosition() / voice->BankData->sampleRate;
        }
        return 0.0f;
    }

    EXPORT UINT32 GetTotalSample(const INT32 voiceID)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice && voice->BankData)
        {
            return voice->BankData->totalSamples;
        }
        return 0;
    }

    EXPORT FLOAT GetTotalTime(const INT32 voiceID)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice && voice->BankData)
        {
            return (FLOAT)voice->BankData->totalSamples / voice->BankData->sampleRate;
        }
        return 0.0f;
    }

    EXPORT UINT32 GetSampleRate(const INT32 voiceID)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice && voice->BankData)
        {
            return voice->BankData->sampleRate;
        }
        return 0;
    }

    EXPORT UINT32 GetChannelCount(const INT32 voiceID)
    {
        AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
        if (voice && voice->BankData)
        {
            return voice->BankData->channels;
        }
        return 0;
    }

    EXPORT void SetOnFinishedCallback(const OnFinishedCallback callback)
    {
        Log(0, 0, "[OnVoiceFinished]");
        SaXAudio::Instance.OnFinishedCallback = callback;
    }

    EXPORT UINT32 GetVoiceCount(const INT32 bankID, const INT32 busID)
    {
        return SaXAudio::Instance.GetVoiceCount(bankID, busID);
    }

    EXPORT UINT32 GetBankCount()
    {
        return SaXAudio::Instance.GetBankCount();
    }
}

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
#include "Structs.h"
#include "AudioVoice.h"

namespace SaXAudio
{
    class SaXAudio
    {
        friend class AudioVoice;
    private:
        SaXAudio() = default;

        IXAudio2* m_XAudio = nullptr;
        BusData m_masteringBus;

        unordered_map<INT32, BankData> m_bank;
        INT32 m_bankCounter = 1;
        mutex m_bankMutex;

        list<Buffer> m_bufferPool;

        unordered_map<INT32, AudioVoice*> m_voices;
        INT32 m_voiceCounter = 1;
        mutex m_voiceMutex;

        // XAudio will continue to send callbacks for a little while even after we destroyed the source voice
        // This is why we use a pool and don't delete the voices, it is not possible to know when it's safe to do so
        // So we put them back at the end the pool and hope XAudio is done with it by the time it gets reused
        queue<AudioVoice*> m_voicePool;

        unordered_map<INT32, BusData> m_buses;
        INT32 m_busCounter = 1;
        mutex m_busMutex;

        DWORD m_channelMask = 0;
        XAUDIO2_VOICE_DETAILS m_masterDetails = { 0 };

        static SaXAudio& getInstance()
        {
            static SaXAudio instance;
            return instance;
        }
        SaXAudio(const SaXAudio&) = delete;
        SaXAudio& operator=(const SaXAudio&) = delete;

    public:
        static SaXAudio& Instance;

        OnFinishedCallback OnFinishedCallback = nullptr;

        BOOL Init();

        void Release();

        void StopEngine();
        void StartEngine();

        void PauseAll(const FLOAT fade, const INT32 busID = 0);
        void ResumeAll(const FLOAT fade, const INT32 busID = 0);
        void StopAll(const FLOAT fade, const INT32 busID = 0);
        void Protect(const INT32 voiceID);

        INT32 AddBankEntry(const OnDecodedCallback callback);
        void RemoveBankEntry(const INT32 bankID);
        void AutoRemoveBank(const INT32 bankID);

        INT32 AddBus();
        void RemoveBus(const INT32 busID);
        BusData* GetBus(const INT32 busID);

        void SetBusVolume(const INT32 busID, const FLOAT volume, const FLOAT fade);
        FLOAT GetBusVolume(const INT32 busID);

        Buffer GetBuffer(UINT32 length);
        void ReturnBuffer(Buffer buffer);
        UINT32 AddBankData(Buffer buffer, UINT32 channels, UINT32 sampleRate, UINT32 totalSamples);
        BOOL StartDecodeOgg(const INT32 bankID, const BYTE* buffer, const UINT32 length);

        AudioVoice* CreateVoice(const INT32 bankID, const INT32 busID = 0);
        AudioVoice* GetVoice(const INT32 voiceID);

        void SetReverb(const INT32 voiceID, const BOOL isBus, const XAUDIO2FX_REVERB_PARAMETERS* params, const FLOAT fade);
        void RemoveReverb(const INT32 voiceID, const BOOL isBus, const FLOAT fade);

        void SetEq(const INT32 voiceID, const BOOL isBus, const FXEQ_PARAMETERS* params, const FLOAT fade);
        void RemoveEq(const INT32 voiceID, const BOOL isBus, const FLOAT fade);

        void SetEcho(const INT32 voiceID, const BOOL isBus, const FXECHO_PARAMETERS* params, const FLOAT fade);
        void RemoveEcho(const INT32 voiceID, const BOOL isBus, const FLOAT fade);

        UINT32 GetVoiceCount(const INT32 bankID = 0, const INT32 busID = 0);
        UINT32 GetBankCount();

    private:
        static void DecodeOgg(const INT32 bankID, stb_vorbis* vorbis);
        void RemoveVoice(const INT32 voiceID);
        void CreateEffectChain(IXAudio2Voice* voice, EffectData* data);

        static void OnFadeReverb(INT64 context, UINT32 count, FLOAT* newValues, BOOL hasFinished);
        static void OnFadeReverbDisable(INT64 context, UINT32 count, FLOAT* newValues, BOOL hasFinished);

        static void OnFadeEq(INT64 context, UINT32 count, FLOAT* newValues, BOOL hasFinished);
        static void OnFadeEqDisable(INT64 context, UINT32 count, FLOAT* newValues, BOOL hasFinished);

        static void OnFadeEcho(INT64 context, UINT32 count, FLOAT* newValues, BOOL hasFinished);
        static void OnFadeEchoDisable(INT64 context, UINT32 count, FLOAT* newValues, BOOL hasFinished);
    };
}

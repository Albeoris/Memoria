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
#include "Fader.h"

namespace SaXAudio
{
#define GetEntry(data, map, id) nullptr; auto it_##data = map.find(id); if (it_##data != map.end()) data = &it_##data->second
#define CHAIN_REVERB 0
#define CHAIN_EQ 1
#define CHAIN_ECHO 2
#define POOL_SIZE_VOICES 50

    SaXAudio& SaXAudio::Instance = SaXAudio::getInstance();

    BOOL SaXAudio::Init()
    {
        if (m_XAudio)
            return true;

        StartLogging();

        HRESULT hr = CoInitializeEx(nullptr, COINIT_MULTITHREADED);
        switch (hr)
        {
        case S_OK:
        case S_FALSE:
            break;
        case RPC_E_CHANGED_MODE:
            // COM initialized with different threading model
            // Try apartment threading instead
            hr = CoInitializeEx(nullptr, COINIT_APARTMENTTHREADED);
            if (SUCCEEDED(hr))
                break;
        default:
            Log(0, 0, "[Init] COM initialize failed", hr);
            return false;
        }

        // Create XAudio2 instance
        hr = XAudio2Create(&m_XAudio, 0, XAUDIO2_DEFAULT_PROCESSOR);
        if (FAILED(hr))
        {
            Log(0, 0, "[Init] XAudio2 creation failed", hr);
            m_XAudio = nullptr;
            return false;
        }

        // Create mastering voice
        IXAudio2MasteringVoice* masteringVoice;
        hr = m_XAudio->CreateMasteringVoice(&masteringVoice, 0, 48000);
        if (FAILED(hr))
        {
            Log(0, 0, "[Init] Mastering voice creation failed", hr);
            m_XAudio->Release();
            m_XAudio = nullptr;
            return false;
        }

        m_masteringBus.voice = masteringVoice;

        // Get channel mask
        hr = masteringVoice->GetChannelMask(&m_channelMask);
        if (FAILED(hr))
        {
            Log(0, 0, "[Init] Couldn't get channel mask", hr);
            m_XAudio->Release();
            m_XAudio = nullptr;
            return false;
        }

        string version = "Unknown";
        // Check which DLL is loaded
        HMODULE hXAudio2 = GetModuleHandle(L"XAudio2_9.dll");
        if (hXAudio2)
        {
            version = "XAudio2 2.9";
        }
        else
        {
            hXAudio2 = GetModuleHandle(L"XAudio2_8.dll");
            if (hXAudio2)
            {
                version = "XAudio2 2.8";
            }
            else
            {
                hXAudio2 = GetModuleHandle(L"XAudio2_7.dll");
                if (hXAudio2)
                {
                    version = "XAudio2 2.7";
                }
            }
        }

        // Get details
        masteringVoice->GetVoiceDetails(&m_masterDetails);
        Log(0, 0, "[Init] Initialization complete. Version: " + version + " Channels: " + to_string(m_masterDetails.InputChannels) + " Sample rate: " + to_string(m_masterDetails.InputSampleRate));

        return true;
    }

    void SaXAudio::Release()
    {
        if (!m_XAudio)
            return;

        m_XAudio->StopEngine();
        m_XAudio->Release();
        m_XAudio = nullptr;

        while (!m_bank.empty())
        {
            RemoveBankEntry(m_bank.begin()->first);
        }

        for (auto& it : m_bufferPool)
            delete[] it.Data;
        m_bufferPool.clear();

        while (!m_voicePool.empty())
        {
            delete m_voicePool.front();
            m_voicePool.pop();
        }

        StopLogging();

        m_voices.clear();
        m_masteringBus.voice = nullptr;
    }

    void SaXAudio::StopEngine()
    {
        if (!m_XAudio)
            return;
        Log(0, 0, "[StopEngine]");

        m_XAudio->StopEngine();
    }

    void SaXAudio::StartEngine()
    {
        if (!m_XAudio)
            return;
        Log(0, 0, "[StartEngine]");

        m_XAudio->StartEngine();
    }

    void SaXAudio::PauseAll(const FLOAT fade, const INT32 busID)
    {
        if (!m_XAudio)
            return;
        Log(0, 0, "[PauseAll]");

        for (auto& it : m_voices)
        {
            if (!it.second->IsProtected && (busID == 0 || it.second->BusID == busID))
                it.second->Pause(fade);
        }
    }

    void SaXAudio::ResumeAll(const FLOAT fade, const INT32 busID)
    {
        if (!m_XAudio)
            return;
        Log(0, 0, "[ResumeAll]");

        for (auto& it : m_voices)
        {
            if (!it.second->IsProtected && (busID == 0 || it.second->BusID == busID))
                it.second->Resume(fade);
        }
    }

    void SaXAudio::StopAll(const FLOAT fade, const INT32 busID)
    {
        if (!m_XAudio)
            return;
        Log(0, 0, "[StopAll]");

        for (auto& it : m_voices)
        {
            if (!it.second->IsProtected && (busID == 0 || it.second->BusID == busID))
                it.second->Stop(fade);
        }
    }

    void SaXAudio::Protect(const INT32 voiceID)
    {
        if (!m_XAudio)
            return;

        AudioVoice* voice = GetVoice(voiceID);
        if (voice)
        {
            voice->IsProtected = true;
            if (voice->IsPlaying)
                while (voice->Resume());

            Log(voice->BankID, voiceID, "[Protect]");
        }
    }

    INT32 SaXAudio::AddBankEntry(const OnDecodedCallback callback)
    {
        if (!m_XAudio)
            return 0;
        lock_guard<mutex> lock(m_bankMutex);

        Log(m_bankCounter, 0, "[AddBankEntry] entries: " + to_string(m_bank.size() + 1));

        m_bank[m_bankCounter].onDecodedCallback = callback;
        return m_bankCounter++;
    }

    void SaXAudio::RemoveBankEntry(const INT32 bankID)
    {
        lock_guard<mutex> lock(m_bankMutex);

        BankData* data = GetEntry(data, m_bank, bankID);
        if (!data) return;
        data->autoRemove = true;
        data->disposed = true;

        if (m_XAudio)
        {
            // Let voices finish before removing
            for (auto& it : m_voices)
            {
                if (it.second->BankID == bankID)
                {
                    // We let autoRemove delete the bankID
                    Log(bankID, 0, "[RemoveBankEntry] Waiting for voices to finish");
                    return;
                }
            }
        }

        // Return buffer to the pool
        m_bufferPool.push_back(data->buffer);
        Log(bankID, 0, "[RemoveBankEntry] Returned buffer size: " + to_string(data->buffer.Size / 1024) + "KB");

        // onDecodedCallback guarantied to be called
        if (data->onDecodedCallback)
        {
            (*data->onDecodedCallback)(bankID, data->Oggbuffer);
            data->onDecodedCallback = nullptr;
        }
        m_bank.erase(bankID);
    }

    void SaXAudio::AutoRemoveBank(const INT32 bankID)
    {
        if (!m_XAudio)
            return;
        lock_guard<mutex> lock(m_bankMutex);

        Log(bankID, 0, "[AutoRemoveBank]");

        BankData* data = GetEntry(data, m_bank, bankID);
        if (!data) return;

        data->autoRemove = true;
    }

    INT32 SaXAudio::AddBus()
    {
        if (!m_XAudio)
            return 0;
        lock_guard<mutex> lock(m_busMutex);

        Log(0, 0, "[AddBus]");

        IXAudio2SubmixVoice* bus;
        HRESULT hr = m_XAudio->CreateSubmixVoice(&bus, m_masterDetails.InputChannels, m_masterDetails.InputSampleRate);
        if (FAILED(hr))
        {
            Log(-1, -1, "Failed creating bus", hr);
            return 0;
        }

        BusData* data = &m_buses[m_busCounter];
        data->voice = bus;
        data->effectChain = { 3, nullptr };
        data->descriptors[0] = { nullptr, false, SaXAudio::m_masterDetails.InputChannels };
        data->descriptors[1] = { nullptr, false, SaXAudio::m_masterDetails.InputChannels };
        data->descriptors[2] = { nullptr, false, SaXAudio::m_masterDetails.InputChannels };

        return m_busCounter++;
    }

    void SaXAudio::RemoveBus(const INT32 busID)
    {
        if (!m_XAudio)
            return;
        lock_guard<mutex> lock(m_busMutex);

        Log(0, 0, "[RemoveBus] " + to_string(busID));

        BusData* bus = GetEntry(bus, m_buses, busID);
        if (!bus) return;

        for (auto& it : m_voices)
        {
            if (it.second->BusID == busID)
                it.second->Stop();
        }

        bus->voice->DestroyVoice();
        m_buses.erase(busID);
    }

    BusData* SaXAudio::GetBus(const INT32 busID)
    {
        if (!m_XAudio)
            return nullptr;
        lock_guard<mutex> lock(m_busMutex);

        BusData* bus = GetEntry(bus, m_buses, busID);
        return bus;
    }

    static void OnFadeVolume(INT64 busID, UINT32 count, FLOAT* newValues, BOOL hasFinished)
    {
        BusData* bus = SaXAudio::Instance.GetBus((INT32)busID);
        if (!bus || !bus->voice) return;
        bus->voice->SetVolume(newValues[0]);
    }

    void SaXAudio::SetBusVolume(const INT32 busID, const FLOAT volume, const FLOAT fade)
    {
        if (!m_XAudio)
            return;

        BusData* bus = busID == 0 ? &m_masteringBus : GetBus(busID);

        if (!bus || !bus->voice) return;
        Log(0, 0, "[SetBusVolume] " + to_string(busID) + " to: " + to_string(volume) + " fade: " + to_string(fade));

        Fader::Instance.StopFade(bus->fadeID);
        bus->fadeID = 0;

        if (fade > 0)
        {
            FLOAT current = 1.0f;
            bus->voice->GetVolume(&current);
            bus->fadeID = Fader::Instance.StartFade(current, volume, fade, OnFadeVolume, busID);
        }
        else
        {
            bus->voice->SetVolume(volume);
        }
    }

    FLOAT SaXAudio::GetBusVolume(const INT32 busID)
    {
        if (!m_XAudio)
            return 0.0f;

        BusData* bus = busID == 0 ? &m_masteringBus : GetBus(busID);
        if (!bus || !bus->voice) return 0.0f;

        FLOAT volume;
        bus->voice->GetVolume(&volume);
        Log(0, 0, "[GetBusVolume] " + to_string(busID) + " volume: " + to_string(volume));
        return volume;
    }

    Buffer SaXAudio::GetBuffer(UINT32 length)
    {
        Buffer buffer;
        if (length == 0) return buffer;
        buffer.Size = 1024;
        while (buffer.Size < length)
            buffer.Size <<= 1;

        auto it = m_bufferPool.begin();
        auto end = m_bufferPool.end();
        auto candidate = end;
        while (it != end)
        {
            if (it->Size == buffer.Size)
            {
                candidate = it;
                break;
            }
            if (it->Size > buffer.Size && it->Size < buffer.Size * 4)
            {
                if (candidate == end)
                    candidate = it;
                else if (it->Size < candidate->Size)
                    candidate = it;
            }
            it++;
        }

#ifdef LOGGING
        static UINT64 totalSize = 0;
        UINT64 poolSize = 0;
        for (auto& it : m_bufferPool)
            poolSize += it.Size;
#endif // LOGGING

        if (candidate != end)
        {
            Log(0, 0, "[GetBuffer] Found buffer size: " + to_string(candidate->Size / 1024) + "KB (" + to_string(buffer.Size / 1024) + "KB) pool size: " + to_string((poolSize - candidate->Size) / 1024) + "KB");
            buffer = *candidate;
            m_bufferPool.erase(candidate);
            return buffer;
        }

        buffer.Data = new FLOAT[buffer.Size];
#ifdef LOGGING
        totalSize += buffer.Size;
        Log(0, 0, "[GetBuffer] Created buffer size: " + to_string(buffer.Size / 1024) + "KB total size: " + to_string(totalSize / 1024 / 1024) + "MB pool size: " + to_string(poolSize / 1024) + "KB");
#endif // LOGGING
        return buffer;
    }

    void SaXAudio::ReturnBuffer(Buffer buffer)
    {
        lock_guard<mutex> bankLock(SaXAudio::Instance.m_bankMutex);
        m_bufferPool.push_back(buffer);
    }

    UINT32 SaXAudio::AddBankData(Buffer buffer, UINT32 channels, UINT32 sampleRate, UINT32 totalSamples)
    {
        if (!m_XAudio)
            return 0;

        lock_guard<mutex> bankLock(SaXAudio::Instance.m_bankMutex);
        Log(m_bankCounter, 0, "[AddBankData]");

        BankData* data = &m_bank[m_bankCounter];
        data->buffer = buffer;
        data->channels = channels;
        data->sampleRate = sampleRate;
        data->totalSamples = totalSamples;
        data->decodedSamples = data->totalSamples;

        return m_bankCounter++;
    }

    BOOL SaXAudio::StartDecodeOgg(const INT32 bankID, const BYTE* buffer, const UINT32 length)
    {
        int error;
        stb_vorbis* vorbis = stb_vorbis_open_memory(buffer, length, &error, NULL);

        if (!vorbis)
            return FALSE;

        lock_guard<mutex> bankLock(m_bankMutex);
        BankData* data = GetEntry(data, m_bank, bankID);
        if (!data) return FALSE;

        data->Oggbuffer = buffer;

        // Get file info
        stb_vorbis_info info = stb_vorbis_get_info(vorbis);
        data->channels = info.channels;
        data->sampleRate = info.sample_rate;

        // Get total samples count and allocate the buffer
        data->totalSamples = stb_vorbis_stream_length_in_samples(vorbis);
        data->buffer = GetBuffer(data->totalSamples * data->channels);

        thread decode(DecodeOgg, bankID, vorbis);
        decode.detach();
        return TRUE;
    }

    AudioVoice* SaXAudio::CreateVoice(const INT32 bankID, const INT32 busID)
    {
        if (!m_XAudio)
            return nullptr;
        lock_guard<mutex> bankLock(m_bankMutex);
        lock_guard<mutex> busLock(m_busMutex);
        lock_guard<mutex> voiceLock(m_voiceMutex);

        BankData* data = GetEntry(data, m_bank, bankID);
        if (!data || data->disposed) return nullptr;

        // Set up audio format
        WAVEFORMATEX wfx = { 0 };
        wfx.wFormatTag = WAVE_FORMAT_IEEE_FLOAT;  // 32-bit float format
        wfx.nChannels = static_cast<WORD>(data->channels);
        wfx.nSamplesPerSec = static_cast<DWORD>(data->sampleRate);
        wfx.wBitsPerSample = 32;  // 32-bit float
        wfx.nBlockAlign = wfx.nChannels * wfx.wBitsPerSample / 8;
        wfx.nAvgBytesPerSec = wfx.nSamplesPerSec * wfx.nBlockAlign;
        wfx.cbSize = 0;

        // Populate the pool if empty
        if (m_voicePool.empty())
        {
            for (UINT32 i = 0; i < POOL_SIZE_VOICES; i++)
                m_voicePool.push(new AudioVoice);
        }

        // Get an unused voice
        AudioVoice* voice = m_voicePool.front();
        m_voicePool.pop();

        BusData* bus = GetEntry(bus, m_buses, busID);

        voice->EffectData.effectChain = { 3, voice->EffectData.descriptors };
        voice->EffectData.descriptors[0] = { nullptr, false, data->channels };
        voice->EffectData.descriptors[1] = { nullptr, false, data->channels };
        voice->EffectData.descriptors[2] = { nullptr, false, data->channels };

        HRESULT hr = XAudio2CreateReverb(&voice->EffectData.descriptors[CHAIN_REVERB].pEffect);
        if (FAILED(hr))
        {
            Log(bankID, m_voiceCounter, "Failed to create reverb effect", hr);
        }

        hr = CreateFX(__uuidof(FXEQ), &voice->EffectData.descriptors[CHAIN_EQ].pEffect);
        if (FAILED(hr))
        {
            Log(bankID, m_voiceCounter, "Failed to create EQ effect", hr);
        }

        FXECHO_INITDATA init = { 3000 };
        hr = CreateFX(__uuidof(FXEcho), &voice->EffectData.descriptors[CHAIN_ECHO].pEffect, &init, sizeof(FXECHO_INITDATA));
        if (FAILED(hr))
        {
            Log(bankID, m_voiceCounter, "Failed to create echo effect", hr);
        }

        if (bus && bus->voice)
        {
            XAUDIO2_SEND_DESCRIPTOR sendDesc { 0, bus->voice };
            XAUDIO2_VOICE_SENDS sends { 1, &sendDesc };

            hr = m_XAudio->CreateSourceVoice(&voice->SourceVoice, &wfx, 0, XAUDIO2_MAX_FREQ_RATIO, voice, &sends, &voice->EffectData.effectChain);
        }
        else
        {
            hr = m_XAudio->CreateSourceVoice(&voice->SourceVoice, &wfx, 0, XAUDIO2_MAX_FREQ_RATIO, voice, nullptr, &voice->EffectData.effectChain);
        }

        if (FAILED(hr))
        {
            voice->Reset();
            Log(bankID, m_voiceCounter, "Failed to create voice on bus " + to_string(busID), hr);
            return nullptr;
        }
        voice->BankData = data;

        // Submit audio buffer
        voice->Buffer = { 0 };
        voice->Buffer.AudioBytes = static_cast<UINT32>(sizeof(float) * data->totalSamples * data->channels);
        voice->Buffer.pAudioData = reinterpret_cast<const BYTE*>(data->buffer.Data);
        voice->Buffer.Flags = XAUDIO2_END_OF_STREAM;

        voice->BankID = bankID;
        voice->VoiceID = m_voiceCounter++;
        voice->BusID = bus ? busID : 0;

        // Set up the output matrix
        voice->SetOutputMatrix(0.0f);

        m_voices[voice->VoiceID] = voice;

        Log(bankID, voice->VoiceID, "[CreateVoice]" + (bus ? " Created on bus " + to_string(busID) : ""));

        return voice;
    }

    AudioVoice* SaXAudio::GetVoice(const INT32 voiceID)
    {
        if (!m_XAudio)
            return nullptr;
        lock_guard<mutex> lock(m_voiceMutex);

        AudioVoice* voice = nullptr;
        auto it_voice = m_voices.find(voiceID);
        if (it_voice != m_voices.end())
            voice = it_voice->second;
        return voice;
    }

    inline void GetEffectData(INT32 voiceID, BOOL isBus, IXAudio2Voice** sourceVoice, EffectData** data)
    {
        if (isBus)
        {
            BusData* bus = SaXAudio::Instance.GetBus(voiceID);
            if (!bus || !bus->voice) return;

            *data = bus;
            *sourceVoice = bus->voice;
        }
        else
        {
            AudioVoice* voice = SaXAudio::Instance.GetVoice(voiceID);
            if (!voice || !voice->SourceVoice) return;

            *data = &voice->EffectData;
            *sourceVoice = voice->SourceVoice;
        }
    }

    void SaXAudio::SetReverb(const INT32 voiceID, const BOOL isBus, const XAUDIO2FX_REVERB_PARAMETERS* params, const FLOAT fade)
    {
        if (!m_XAudio)
            return;

        IXAudio2Voice* voice = nullptr;
        EffectData* data = nullptr;
        GetEffectData(voiceID, isBus, &voice, &data);
        if (!voice) return;

        if (!data->effectChain.pEffectDescriptors)
        {
            CreateEffectChain(voice, data);
        }

        HRESULT hr = voice->EnableEffect(CHAIN_REVERB);
        if (FAILED(hr))
        {
            Log(0, 0, "Failed to enable reverb", hr);
        }

        if (fade <= 0)
        {
            data->reverb = *params;
            hr = voice->SetEffectParameters(CHAIN_REVERB, &data->reverb, sizeof(XAUDIO2FX_REVERB_PARAMETERS), XAUDIO2_COMMIT_NOW);
            if (FAILED(hr))
            {
                Log(0, 0, "Failed to set reverb parameters", hr);
            }
            return;
        }

        // Can't quite fade a boolean
        data->reverb.DisableLateField = params->DisableLateField;

        FLOAT* current = nullptr;

        if (data->reverb.WetDryMix == 0)
        {
            current = new FLOAT[23]
            {
                data->reverb.WetDryMix,
                static_cast<FLOAT>(params->ReflectionsDelay),
                static_cast<FLOAT>(params->ReverbDelay),
                static_cast<FLOAT>(params->RearDelay),
                static_cast<FLOAT>(params->SideDelay),
                static_cast<FLOAT>(params->PositionLeft),
                static_cast<FLOAT>(params->PositionRight),
                static_cast<FLOAT>(params->PositionMatrixLeft),
                static_cast<FLOAT>(params->PositionMatrixRight),
                static_cast<FLOAT>(params->EarlyDiffusion),
                static_cast<FLOAT>(params->LateDiffusion),
                static_cast<FLOAT>(params->LowEQGain),
                static_cast<FLOAT>(params->LowEQCutoff),
                static_cast<FLOAT>(params->HighEQGain),
                static_cast<FLOAT>(params->HighEQCutoff),
                params->RoomFilterFreq,
                params->RoomFilterMain,
                params->RoomFilterHF,
                params->ReflectionsGain,
                params->ReverbGain,
                params->DecayTime,
                params->Density,
                params->RoomSize
            };
        }
        else
        {
            current = new FLOAT[23]
            {
                data->reverb.WetDryMix,
                static_cast<FLOAT>(data->reverb.ReflectionsDelay),
                static_cast<FLOAT>(data->reverb.ReverbDelay),
                static_cast<FLOAT>(data->reverb.RearDelay),
                static_cast<FLOAT>(data->reverb.SideDelay),
                static_cast<FLOAT>(data->reverb.PositionLeft),
                static_cast<FLOAT>(data->reverb.PositionRight),
                static_cast<FLOAT>(data->reverb.PositionMatrixLeft),
                static_cast<FLOAT>(data->reverb.PositionMatrixRight),
                static_cast<FLOAT>(data->reverb.EarlyDiffusion),
                static_cast<FLOAT>(data->reverb.LateDiffusion),
                static_cast<FLOAT>(data->reverb.LowEQGain),
                static_cast<FLOAT>(data->reverb.LowEQCutoff),
                static_cast<FLOAT>(data->reverb.HighEQGain),
                static_cast<FLOAT>(data->reverb.HighEQCutoff),
                data->reverb.RoomFilterFreq,
                data->reverb.RoomFilterMain,
                data->reverb.RoomFilterHF,
                data->reverb.ReflectionsGain,
                data->reverb.ReverbGain,
                data->reverb.DecayTime,
                data->reverb.Density,
                data->reverb.RoomSize
            };
        }

        FLOAT* targets = new FLOAT[23]
        {
            params->WetDryMix,
            static_cast<FLOAT>(params->ReflectionsDelay),
            static_cast<FLOAT>(params->ReverbDelay),
            static_cast<FLOAT>(params->RearDelay),
            static_cast<FLOAT>(params->SideDelay),
            static_cast<FLOAT>(params->PositionLeft),
            static_cast<FLOAT>(params->PositionRight),
            static_cast<FLOAT>(params->PositionMatrixLeft),
            static_cast<FLOAT>(params->PositionMatrixRight),
            static_cast<FLOAT>(params->EarlyDiffusion),
            static_cast<FLOAT>(params->LateDiffusion),
            static_cast<FLOAT>(params->LowEQGain),
            static_cast<FLOAT>(params->LowEQCutoff),
            static_cast<FLOAT>(params->HighEQGain),
            static_cast<FLOAT>(params->HighEQCutoff),
            params->RoomFilterFreq,
            params->RoomFilterMain,
            params->RoomFilterHF,
            params->ReflectionsGain,
            params->ReverbGain,
            params->DecayTime,
            params->Density,
            params->RoomSize
        };

        INT64 context = isBus ? -voiceID : voiceID;
        Fader::Instance.StartFadeMulti(23, current, targets, fade, OnFadeReverb, context);
    }

    void SaXAudio::RemoveReverb(const INT32 voiceID, const BOOL isBus, const FLOAT fade)
    {
        if (!m_XAudio)
            return;

        IXAudio2Voice* voice = nullptr;
        EffectData* data = nullptr;
        GetEffectData(voiceID, isBus, &voice, &data);
        if (!voice) return;

        if (fade <= 0)
        {
            voice->DisableEffect(CHAIN_REVERB);
            return;
        }

        FLOAT* current = new FLOAT[23]
        {
            data->reverb.WetDryMix,
            static_cast<FLOAT>(data->reverb.ReflectionsDelay),
            static_cast<FLOAT>(data->reverb.ReverbDelay),
            static_cast<FLOAT>(data->reverb.RearDelay),
            static_cast<FLOAT>(data->reverb.SideDelay),
            static_cast<FLOAT>(data->reverb.PositionLeft),
            static_cast<FLOAT>(data->reverb.PositionRight),
            static_cast<FLOAT>(data->reverb.PositionMatrixLeft),
            static_cast<FLOAT>(data->reverb.PositionMatrixRight),
            static_cast<FLOAT>(data->reverb.EarlyDiffusion),
            static_cast<FLOAT>(data->reverb.LateDiffusion),
            static_cast<FLOAT>(data->reverb.LowEQGain),
            static_cast<FLOAT>(data->reverb.LowEQCutoff),
            static_cast<FLOAT>(data->reverb.HighEQGain),
            static_cast<FLOAT>(data->reverb.HighEQCutoff),
            data->reverb.RoomFilterFreq,
            data->reverb.RoomFilterMain,
            data->reverb.RoomFilterHF,
            data->reverb.ReflectionsGain,
            data->reverb.ReverbGain,
            data->reverb.DecayTime,
            data->reverb.Density,
            data->reverb.RoomSize
        };

        FLOAT* targets = new FLOAT[23]
        {
            0,
            static_cast<FLOAT>(data->reverb.ReflectionsDelay),
            static_cast<FLOAT>(data->reverb.ReverbDelay),
            static_cast<FLOAT>(data->reverb.RearDelay),
            static_cast<FLOAT>(data->reverb.SideDelay),
            static_cast<FLOAT>(data->reverb.PositionLeft),
            static_cast<FLOAT>(data->reverb.PositionRight),
            static_cast<FLOAT>(data->reverb.PositionMatrixLeft),
            static_cast<FLOAT>(data->reverb.PositionMatrixRight),
            static_cast<FLOAT>(data->reverb.EarlyDiffusion),
            static_cast<FLOAT>(data->reverb.LateDiffusion),
            static_cast<FLOAT>(data->reverb.LowEQGain),
            static_cast<FLOAT>(data->reverb.LowEQCutoff),
            static_cast<FLOAT>(data->reverb.HighEQGain),
            static_cast<FLOAT>(data->reverb.HighEQCutoff),
            data->reverb.RoomFilterFreq,
            data->reverb.RoomFilterMain,
            data->reverb.RoomFilterHF,
            data->reverb.ReflectionsGain,
            data->reverb.ReverbGain,
            data->reverb.DecayTime,
            data->reverb.Density,
            data->reverb.RoomSize
        };

        INT64 context = isBus ? -voiceID : voiceID;
        Fader::Instance.StartFadeMulti(23, current, targets, fade, OnFadeReverbDisable, context);
    }

    void SaXAudio::SetEq(const INT32 voiceID, const BOOL isBus, const FXEQ_PARAMETERS* params, const FLOAT fade)
    {
        if (!m_XAudio)
            return;

        IXAudio2Voice* voice = nullptr;
        EffectData* data = nullptr;
        GetEffectData(voiceID, isBus, &voice, &data);
        if (!voice) return;

        if (!data->effectChain.pEffectDescriptors)
        {
            CreateEffectChain(voice, data);
        }

        HRESULT hr = voice->EnableEffect(CHAIN_EQ);
        if (FAILED(hr))
        {
            Log(0, 0, "Failed to enable EQ", hr);
        }

        if (fade <= 0)
        {
            data->eq = *params;
            hr = voice->SetEffectParameters(CHAIN_EQ, &data->eq, sizeof(FXEQ_PARAMETERS), XAUDIO2_COMMIT_NOW);
            if (FAILED(hr))
            {
                Log(0, 0, "Failed to set EQ effect parameters", hr);
            }
            return;
        }
        Log(0, 0, "FrequencyCenter0: " + to_string(data->eq.FrequencyCenter0) + " Gain0: " + to_string(data->eq.Gain0));
        FLOAT* current = new FLOAT[12]
        {
            data->eq.FrequencyCenter0,
            data->eq.Gain0,
            data->eq.Bandwidth0,
            data->eq.FrequencyCenter1,
            data->eq.Gain1,
            data->eq.Bandwidth1,
            data->eq.FrequencyCenter2,
            data->eq.Gain2,
            data->eq.Bandwidth2,
            data->eq.FrequencyCenter3,
            data->eq.Gain3,
            data->eq.Bandwidth3
        };

        FLOAT* targets = new FLOAT[12]
        {
            params->FrequencyCenter0,
            params->Gain0,
            params->Bandwidth0,
            params->FrequencyCenter1,
            params->Gain1,
            params->Bandwidth1,
            params->FrequencyCenter2,
            params->Gain2,
            params->Bandwidth2,
            params->FrequencyCenter3,
            params->Gain3,
            params->Bandwidth3
        };

        INT64 context = isBus ? -voiceID : voiceID;
        Fader::Instance.StartFadeMulti(12, current, targets, fade, OnFadeEq, context);
    }

    void SaXAudio::RemoveEq(const INT32 voiceID, const BOOL isBus, const FLOAT fade)
    {
        if (!m_XAudio)
            return;

        IXAudio2Voice* voice = nullptr;
        EffectData* data = nullptr;
        GetEffectData(voiceID, isBus, &voice, &data);
        if (!voice) return;

        if (fade <= 0)
        {
            voice->DisableEffect(CHAIN_EQ);
            return;
        }

        FLOAT* current = new FLOAT[12]
        {
            data->eq.FrequencyCenter0,
            data->eq.Gain0,
            data->eq.Bandwidth0,
            data->eq.FrequencyCenter1,
            data->eq.Gain1,
            data->eq.Bandwidth1,
            data->eq.FrequencyCenter2,
            data->eq.Gain2,
            data->eq.Bandwidth2,
            data->eq.FrequencyCenter3,
            data->eq.Gain3,
            data->eq.Bandwidth3
        };

        EffectData defaultData;
        FLOAT* targets = new FLOAT[12]
        {
            defaultData.eq.FrequencyCenter0,
            defaultData.eq.Gain0,
            defaultData.eq.Bandwidth0,
            defaultData.eq.FrequencyCenter1,
            defaultData.eq.Gain1,
            defaultData.eq.Bandwidth1,
            defaultData.eq.FrequencyCenter2,
            defaultData.eq.Gain2,
            defaultData.eq.Bandwidth2,
            defaultData.eq.FrequencyCenter3,
            defaultData.eq.Gain3,
            defaultData.eq.Bandwidth3
        };

        INT64 context = isBus ? -voiceID : voiceID;
        Fader::Instance.StartFadeMulti(12, current, targets, fade, OnFadeEqDisable, context);
    }

    void SaXAudio::SetEcho(const INT32 voiceID, const BOOL isBus, const FXECHO_PARAMETERS* params, const FLOAT fade)
    {
        if (!m_XAudio)
            return;

        IXAudio2Voice* voice = nullptr;
        EffectData* data = nullptr;
        GetEffectData(voiceID, isBus, &voice, &data);
        if (!voice) return;

        if (!data->effectChain.pEffectDescriptors)
        {
            CreateEffectChain(voice, data);
        }

        HRESULT hr = voice->EnableEffect(CHAIN_ECHO);
        if (FAILED(hr))
        {
            Log(0, 0, "Failed to enable echo", hr);
        }

        if (fade <= 0)
        {
            data->echo = *params;
            hr = voice->SetEffectParameters(CHAIN_ECHO, &data->echo, sizeof(FXECHO_PARAMETERS), XAUDIO2_COMMIT_NOW);
            if (FAILED(hr))
            {
                Log(0, 0, "Failed to set echo parameters", hr);
            }
            return;
        }

        FLOAT* current = nullptr;
        if (data->echo.WetDryMix == 0)
        {
            current = new FLOAT[3]
            {
                data->echo.WetDryMix,
                params->Feedback,
                params->Delay
            };
        }
        else
        {
            current = new FLOAT[3]
            {
                data->echo.WetDryMix,
                data->echo.Feedback,
                data->echo.Delay
            };
        }

        FLOAT* targets = new FLOAT[3]
        {
            params->WetDryMix,
            params->Feedback,
            params->Delay
        };

        INT64 context = isBus ? -voiceID : voiceID;
        Fader::Instance.StartFadeMulti(12, current, targets, fade, OnFadeEcho, context);
    }

    void SaXAudio::RemoveEcho(const INT32 voiceID, const BOOL isBus, const FLOAT fade)
    {
        if (!m_XAudio)
            return;

        IXAudio2Voice* voice = nullptr;
        EffectData* data = nullptr;
        GetEffectData(voiceID, isBus, &voice, &data);
        if (!voice) return;

        if (fade <= 0)
        {
            voice->DisableEffect(CHAIN_ECHO);
            return;
        }

        FLOAT* current = new FLOAT[12]
        {
            data->echo.WetDryMix,
            data->echo.Feedback,
            data->echo.Delay
        };

        FLOAT* targets = new FLOAT[12] { 0 };

        INT64 context = isBus ? -voiceID : voiceID;
        Fader::Instance.StartFadeMulti(3, current, targets, fade, OnFadeEchoDisable, context);
    }

    UINT32 SaXAudio::GetVoiceCount(const INT32 bankID, const INT32 busID)
    {
        if (!m_XAudio)
            return 0;
        lock_guard<mutex> lock(m_voiceMutex);

        UINT32 count = 0;
        for (auto& it : m_voices)
        {
            if (bankID > 0 && it.second->BankID != bankID)
                continue;
            if (busID > 0 && it.second->BusID != busID)
                continue;
            if (!it.second->SourceVoice)
                count++;
        }
        return count;
    }

    UINT32 SaXAudio::GetBankCount()
    {
        if (!m_XAudio)
            return 0;
        lock_guard<mutex> lock(m_bankMutex);

        UINT32 count = 0;
        for (auto& it : m_bank)
        {
            if (!it.second.disposed)
                count++;
        }

        return count;
    }

    void SaXAudio::DecodeOgg(const INT32 bankID, stb_vorbis* vorbis)
    {
        // Reset file position
        stb_vorbis_seek_start(vorbis);

        UINT32 samplesTotal = 0;
        UINT32 samplesDecoded = 0;
        UINT32 bufferSize = 4096;

        auto it = SaXAudio::Instance.m_bank.find(bankID);
        if (it != SaXAudio::Instance.m_bank.end())
        {
            BankData* data = &it->second;
            if (data)
            {
                data->decodedSamples = 0;
                samplesTotal = data->totalSamples;
            }
        }

        while (samplesTotal - samplesDecoded > 0)
        {
            if (bufferSize > samplesTotal)
                bufferSize = samplesTotal;

            lock_guard<mutex> lock(SaXAudio::Instance.m_bankMutex);

            BankData* data = nullptr;
            it = SaXAudio::Instance.m_bank.find(bankID);
            if (it != SaXAudio::Instance.m_bank.end())
                data = &it->second;

            // BankEntry removed
            if (!data || data->disposed) break;

            {
                lock_guard<mutex> lock(data->decodingMutex);

                // Read samples
                FLOAT* pBuffer = &data->buffer.Data[data->decodedSamples * data->channels];
                UINT32 decoded = stb_vorbis_get_samples_float_interleaved(vorbis, data->channels, pBuffer, bufferSize * data->channels);
                samplesDecoded += decoded;
                data->decodedSamples = samplesDecoded;

                if (decoded == 0)
                {
                    // Less samples decoded than expected
                    // we update the total samples to match
                    data->totalSamples = samplesDecoded;
                }

                data->decodingPerform.notify_one();
            }
        }

        // Close the Vorbis file
        stb_vorbis_close(vorbis);

        {
            // Calling back
            lock_guard<mutex> lock(SaXAudio::Instance.m_bankMutex);

            BankData* data = GetEntry(data, SaXAudio::Instance.m_bank, bankID);
            if (data && data->onDecodedCallback)
            {
                (*data->onDecodedCallback)(bankID, data->Oggbuffer);
                data->onDecodedCallback = nullptr;
            }
        }

        Log(bankID, 0, "[DecodeOgg] Decoding complete");
    }

    void SaXAudio::RemoveVoice(const INT32 voiceID)
    {
        BOOL autoRemove = false;
        INT32 bankID = 0;
        {
            lock_guard<mutex> lock(m_voiceMutex);

            AudioVoice* voice = nullptr;
            auto it_voice = m_voices.find(voiceID);
            if (it_voice != m_voices.end())
                voice = it_voice->second;
            if (!voice) return;
            bankID = voice->BankID;
            voice->BankID = 0;

            // Stop the voice
            if (voice->SourceVoice)
            {
                Log(bankID, voiceID, "[RemoveVoice] Stopping voice");
                voice->SourceVoice->DestroyVoice();
                voice->SourceVoice = nullptr;
            }

            // Callback
            if (voice->IsPlaying && OnFinishedCallback != nullptr)
            {
                thread onFinished(*OnFinishedCallback, voiceID);
                onFinished.detach();
            }

            // Auto remove
            BankData* data = GetEntry(data, m_bank, bankID);
            if (data && data->autoRemove)
            {
                autoRemove = true;
                for (auto& it : m_voices)
                {
                    // Look if any other voice is still playing the bank entry
                    if (it.second->BankID == bankID)
                    {
                        autoRemove = false;
                        break;
                    }
                }
            }

            // Voice ready to be reused
            voice->Reset();
            m_voicePool.push(voice);
            m_voices.erase(voiceID);

            Log(voice->BankID, voiceID, "[RemoveVoice] Deleted voice");
        }

        if (autoRemove)
            RemoveBankEntry(bankID);
    }

    void SaXAudio::CreateEffectChain(IXAudio2Voice* voice, EffectData* data)
    {
        HRESULT hr = XAudio2CreateReverb(&data->descriptors[CHAIN_REVERB].pEffect);
        if (FAILED(hr))
        {
            Log(0, 0, "Failed to create reverb effect", hr);
        }

        hr = CreateFX(__uuidof(FXEQ), &data->descriptors[CHAIN_EQ].pEffect);
        if (FAILED(hr))
        {
            Log(0, 0, "Failed to create EQ effect", hr);
        }

        FXECHO_INITDATA init = { 3000 };
        hr = CreateFX(__uuidof(FXEcho), &data->descriptors[CHAIN_ECHO].pEffect, &init, sizeof(FXECHO_INITDATA));
        if (FAILED(hr))
        {
            Log(0, 0, "Failed to create echo effect", hr);
        }

        data->effectChain.EffectCount = 3;
        data->effectChain.pEffectDescriptors = data->descriptors;

        hr = voice->SetEffectChain(&data->effectChain);
        if (FAILED(hr))
        {
            Log(0, 0, "Failed to set effect chain", hr);
        }
    }


    void SaXAudio::OnFadeReverb(INT64 context, UINT32 count, FLOAT* newValues, BOOL hasFinished)
    {
        BOOL isBus = context < 0;
        INT32 voiceID = isBus ? -(INT32)context : (INT32)context;

        IXAudio2Voice* voice = nullptr;
        EffectData* data = nullptr;
        GetEffectData(voiceID, isBus, &voice, &data);
        if (!voice) return;

        INT32 i = 0;
        data->reverb.WetDryMix = newValues[i++];
        data->reverb.ReflectionsDelay = static_cast<UINT32>(newValues[i++]);
        data->reverb.ReverbDelay = static_cast<BYTE>(newValues[i++]);
        data->reverb.RearDelay = static_cast<BYTE>(newValues[i++]);
        data->reverb.SideDelay = static_cast<BYTE>(newValues[i++]);
        data->reverb.PositionLeft = static_cast<BYTE>(newValues[i++]);
        data->reverb.PositionRight = static_cast<BYTE>(newValues[i++]);
        data->reverb.PositionMatrixLeft = static_cast<BYTE>(newValues[i++]);
        data->reverb.PositionMatrixRight = static_cast<BYTE>(newValues[i++]);
        data->reverb.EarlyDiffusion = static_cast<BYTE>(newValues[i++]);
        data->reverb.LateDiffusion = static_cast<BYTE>(newValues[i++]);
        data->reverb.LowEQGain = static_cast<BYTE>(newValues[i++]);
        data->reverb.LowEQCutoff = static_cast<BYTE>(newValues[i++]);
        data->reverb.HighEQGain = static_cast<BYTE>(newValues[i++]);
        data->reverb.HighEQCutoff = static_cast<BYTE>(newValues[i++]);
        data->reverb.RoomFilterFreq = newValues[i++];
        data->reverb.RoomFilterMain = newValues[i++];
        data->reverb.RoomFilterHF = newValues[i++];
        data->reverb.ReflectionsGain = newValues[i++];
        data->reverb.ReverbGain = newValues[i++];
        data->reverb.DecayTime = newValues[i++];
        data->reverb.Density = newValues[i++];
        data->reverb.RoomSize = newValues[i++];

        HRESULT hr = voice->SetEffectParameters(CHAIN_REVERB, &data->reverb, sizeof(XAUDIO2FX_REVERB_PARAMETERS), XAUDIO2_COMMIT_NOW);
        if (FAILED(hr))
        {
            Log(0, 0, "Failed to set reverb parameters", hr);
        }
    }

    void SaXAudio::OnFadeReverbDisable(INT64 context, UINT32 count, FLOAT* newValues, BOOL hasFinished)
    {
        BOOL isBus = context < 0;
        INT32 voiceID = isBus ? -(INT32)context : (INT32)context;

        IXAudio2Voice* voice = nullptr;
        EffectData* data = nullptr;
        GetEffectData(voiceID, isBus, &voice, &data);
        if (!voice) return;

        if (hasFinished)
        {
            voice->DisableEffect(CHAIN_REVERB);
            return;
        }

        OnFadeReverb(context, count, newValues, hasFinished);
    }

    void SaXAudio::OnFadeEq(INT64 context, UINT32 count, FLOAT* newValues, BOOL hasFinished)
    {
        BOOL isBus = context < 0;
        INT32 voiceID = isBus ? -(INT32)context : (INT32)context;

        IXAudio2Voice* voice = nullptr;
        EffectData* data = nullptr;
        GetEffectData(voiceID, isBus, &voice, &data);
        if (!voice) return;

        INT32 i = 0;
        data->eq.FrequencyCenter0 = newValues[i++];
        data->eq.Gain0 = newValues[i++];
        data->eq.Bandwidth0 = newValues[i++];
        data->eq.FrequencyCenter1 = newValues[i++];
        data->eq.Gain1 = newValues[i++];
        data->eq.Bandwidth1 = newValues[i++];
        data->eq.FrequencyCenter2 = newValues[i++];
        data->eq.Gain2 = newValues[i++];
        data->eq.Bandwidth2 = newValues[i++];
        data->eq.FrequencyCenter3 = newValues[i++];
        data->eq.Gain3 = newValues[i++];
        data->eq.Bandwidth3 = newValues[i++];

        HRESULT hr = voice->SetEffectParameters(CHAIN_EQ, &data->eq, sizeof(FXEQ_PARAMETERS), XAUDIO2_COMMIT_NOW);
        if (FAILED(hr))
        {
            Log(0, 0, "Failed to set EQ parameters", hr);
        }
    }

    void SaXAudio::OnFadeEqDisable(INT64 context, UINT32 count, FLOAT* newValues, BOOL hasFinished)
    {
        BOOL isBus = context < 0;
        INT32 voiceID = isBus ? -(INT32)context : (INT32)context;

        IXAudio2Voice* voice = nullptr;
        EffectData* data = nullptr;
        GetEffectData(voiceID, isBus, &voice, &data);
        if (!voice) return;

        if (hasFinished)
        {
            voice->DisableEffect(CHAIN_EQ);
            return;
        }

        OnFadeEq(context, count, newValues, hasFinished);
    }

    void SaXAudio::OnFadeEcho(INT64 context, UINT32 count, FLOAT* newValues, BOOL hasFinished)
    {
        BOOL isBus = context < 0;
        INT32 voiceID = isBus ? -(INT32)context : (INT32)context;

        IXAudio2Voice* voice = nullptr;
        EffectData* data = nullptr;
        GetEffectData(voiceID, isBus, &voice, &data);
        if (!voice) return;

        INT32 i = 0;
        data->echo.WetDryMix = newValues[i++];
        data->echo.Feedback = newValues[i++];
        data->echo.Delay = newValues[i++];

        HRESULT hr = voice->SetEffectParameters(CHAIN_ECHO, &data->echo, sizeof(FXECHO_PARAMETERS), XAUDIO2_COMMIT_NOW);
        if (FAILED(hr))
        {
            Log(0, 0, "Failed to set EQ parameters", hr);
        }
    }

    void SaXAudio::OnFadeEchoDisable(INT64 context, UINT32 count, FLOAT* newValues, BOOL hasFinished)
    {
        BOOL isBus = context < 0;
        INT32 voiceID = isBus ? -(INT32)context : (INT32)context;

        IXAudio2Voice* voice = nullptr;
        EffectData* data = nullptr;
        GetEffectData(voiceID, isBus, &voice, &data);
        if (!voice) return;

        if (hasFinished)
        {
            voice->DisableEffect(CHAIN_ECHO);
            return;
        }

        OnFadeEcho(context, count, newValues, hasFinished);
    }
}

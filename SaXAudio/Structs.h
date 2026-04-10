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
    typedef void (*OnDecodedCallback)(INT32 bankID, const BYTE* buffer);

    struct EffectData
    {
        XAUDIO2_EFFECT_CHAIN effectChain = { 0 };
        XAUDIO2_EFFECT_DESCRIPTOR descriptors[3] = { 0 };
        XAUDIO2FX_REVERB_PARAMETERS reverb = { 0 };
        FXEQ_PARAMETERS eq = {
            FXEQ_DEFAULT_FREQUENCY_CENTER_0,
            FXEQ_DEFAULT_GAIN,
            FXEQ_DEFAULT_BANDWIDTH,
            FXEQ_DEFAULT_FREQUENCY_CENTER_1,
            FXEQ_DEFAULT_GAIN,
            FXEQ_DEFAULT_BANDWIDTH,
            FXEQ_DEFAULT_FREQUENCY_CENTER_2,
            FXEQ_DEFAULT_GAIN,
            FXEQ_DEFAULT_BANDWIDTH,
            FXEQ_DEFAULT_FREQUENCY_CENTER_3,
            FXEQ_DEFAULT_GAIN,
            FXEQ_DEFAULT_BANDWIDTH
        };
        FXECHO_PARAMETERS echo = { 0 };
    };

    struct BusData : EffectData
    {
        IXAudio2Voice* voice = nullptr;
        UINT32 fadeID = 0;
    };

    struct Buffer
    {
        FLOAT* Data = nullptr;
        UINT32 Size = 0;
    };

    struct BankData
    {
        INT32 bankID = 0;
        BOOL autoRemove = false;
        BOOL disposed = false;

        Buffer buffer = { 0 };

        const BYTE* Oggbuffer = nullptr;
        OnDecodedCallback onDecodedCallback = nullptr;

        UINT32 channels = 0;
        UINT32 sampleRate = 0;
        UINT32 totalSamples = 0;

        atomic<UINT32> decodedSamples = 0;
        mutex decodingMutex;
        condition_variable decodingPerform;
    };
}
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
    typedef void (*OnFadeCallback)(INT64 context, UINT32 count, FLOAT* newValues, BOOL hasFinished);

    class Fader
    {
    private:
        static const INT32 INTERVAL = 10;
        Fader() = default;

        struct FaderData
        {
            UINT32 index = 0;
            BOOL hasFinished = false;

            BOOL paused = false;
            UINT32 count = 0;
            FLOAT* current = nullptr;
            FLOAT* target = nullptr;
            FLOAT* rate = nullptr;
            OnFadeCallback onFade = nullptr;
            INT64 context = 0;
        };
        unordered_map<UINT32, FaderData> m_jobs;
        UINT32 m_jobsCounter = 1;
        mutex m_jobsMutex;

        atomic<bool> m_running;
        unique_ptr<thread, default_delete<thread>> m_thread;

        static Fader& getInstance()
        {
            static Fader instance;
            return instance;
        }
        Fader(const Fader&) = delete;
        Fader& operator=(const Fader&) = delete;

        static void DoFade();

    public:
        static Fader& Instance;

        UINT32 StartFade(FLOAT currentValue, FLOAT target, const FLOAT duration, const OnFadeCallback onFade, INT64 context);
        UINT32 StartFadeMulti(const UINT32 count, FLOAT* currentValues, FLOAT* targets, const FLOAT duration, const OnFadeCallback onFade, INT64 context);
        void StopFade(const UINT32 fadeID);
        void PauseFade(const UINT32 fadeID);
        void ResumeFade(const UINT32 fadeID);
    };
}
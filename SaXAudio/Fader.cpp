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

#include "Fader.h"
#include "AudioVoice.h"

namespace SaXAudio
{
    Fader& Fader::Instance = Fader::getInstance();

    inline FLOAT MoveToTarget(const FLOAT start, const FLOAT end, const FLOAT rate)
    {
        if (rate > 0 && start < end)
        {
            FLOAT result = start + rate;
            return result > end ? end : result;
        }
        if (rate < 0 && start > end)
        {
            FLOAT result = start + rate;
            return result < end ? end : result;
        }
        return end;
    }

    void Fader::DoFade()
    {
        auto start = chrono::steady_clock::now();
        chrono::milliseconds interval = chrono::milliseconds(INTERVAL);
        UINT64 count = 0;

        do
        {
            count++;
            auto target_time = start + (interval * count);
            this_thread::sleep_until(target_time);

            queue<FaderData> callbackQueue;
            {
                lock_guard<mutex> lock(Instance.m_jobsMutex);

                if (!Instance.m_running || Instance.m_jobs.empty())
                    break;

                for (auto it = Instance.m_jobs.begin(); it != Instance.m_jobs.end(); it++)
                {
                    if (it->second.paused || it->second.hasFinished)
                        continue;

                    it->second.hasFinished = true;
                    for (UINT32 i = 0; i < it->second.count; i++)
                    {
                        it->second.current[i] = MoveToTarget(it->second.current[i], it->second.target[i], it->second.rate[i]);

                        if (it->second.current[i] != it->second.target[i])
                            it->second.hasFinished = false;
                    }

                    callbackQueue.push(it->second);
                }
            }

            while (!callbackQueue.empty())
            {
                FaderData data = callbackQueue.front();
                callbackQueue.pop();
                data.onFade(data.context, data.count, data.current, data.hasFinished);
                if (data.hasFinished)
                {
                    Instance.StopFade(data.index);
                }
            }
        }
        while (Instance.m_running);

        Instance.m_running = false;
    }

    UINT32 Fader::StartFade(FLOAT currentValue, FLOAT target, const FLOAT duration, const OnFadeCallback onFade, INT64 context)
    {
        return StartFadeMulti(1, new FLOAT[1] { currentValue }, new FLOAT[1] { target }, duration, onFade, context);
    }

    UINT32 Fader::StartFadeMulti(const UINT32 count, FLOAT* currentValues, FLOAT* targets, const FLOAT duration, const OnFadeCallback onFade, INT64 context)
    {
        lock_guard<mutex> lock(m_jobsMutex);

        FLOAT* rates = new FLOAT[count];
        for (UINT32 i = 0; i < count; i++)
        {
            rates[i] = ((targets[i] - currentValues[i]) / (duration * 1000.0f / INTERVAL));
        }
        m_jobs[m_jobsCounter] = {
            m_jobsCounter,
            false,
            false,
            count,
            currentValues,
            targets,
            rates,
            onFade,
            context
        };
        if (!m_running.exchange(true))
        {
            m_thread = make_unique<thread>(DoFade);
            m_thread->detach();
        }
        return m_jobsCounter++;
    }

    void Fader::StopFade(const UINT32 fadeID)
    {
        if (fadeID == 0) return;
        lock_guard<mutex> lock(m_jobsMutex);

        auto it = m_jobs.find(fadeID);
        if (it == m_jobs.end())
            return;

        delete[] it->second.current;
        delete[] it->second.target;
        delete[] it->second.rate;
        m_jobs.erase(fadeID);
    }

    void Fader::PauseFade(const UINT32 fadeID)
    {
        if (fadeID == 0) return;
        lock_guard<mutex> lock(m_jobsMutex);

        auto it = m_jobs.find(fadeID);
        if (it == m_jobs.end())
            return;

        it->second.paused = true;
    }

    void Fader::ResumeFade(const UINT32 fadeID)
    {
        if (fadeID == 0) return;
        lock_guard<mutex> lock(m_jobsMutex);

        auto it = m_jobs.find(fadeID);
        if (it == m_jobs.end())
            return;

        it->second.paused = false;
    }
}

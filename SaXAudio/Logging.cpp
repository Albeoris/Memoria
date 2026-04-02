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

#ifdef LOGGING

#include <fstream>
#include <iomanip>
#include <queue>

#define GetTime() chrono::duration_cast<chrono::milliseconds>(chrono::system_clock::now().time_since_epoch()).count()


namespace SaXAudio
{
    struct LogEntry
    {
        INT64 timestamp;
        INT32 bankID;
        INT32 voiceID;
        string message;
        HRESULT result;
    };

    struct LogData
    {
        atomic<bool> logging = false;

        mutex mutex;
        condition_variable condition;
        thread workThread;

        queue<LogEntry> queue;

        ofstream file;
        INT64 startTime = GetTime();
    };

    static LogData g_logData;

    void LogWorker()
    {
        g_logData.file.open("SaXAudio.log", ios::trunc | ios::out);

        vector<LogEntry> batch;
        batch.reserve(32);

        while (true)
        {
            // Wait for entries
            unique_lock<mutex> lock(g_logData.mutex);
            g_logData.condition.wait(lock, [] { return !g_logData.queue.empty() || !g_logData.logging; });

            // Move entries to local batch
            while (!g_logData.queue.empty())
            {
                batch.push_back(move(g_logData.queue.front()));
                g_logData.queue.pop();
            }
            lock.unlock();

            // Process batch
            for (const auto& entry : batch)
            {
                INT64 millisec = entry.timestamp;
                INT64 seconds = millisec / 1000;
                INT32 minutes = (INT32)(seconds / 60);
                INT32 hours = minutes / 60;

                g_logData.file << hours << ":"
                    << setw(2) << setfill('0') << right  << minutes % 60 << ":"
                    << setw(2) << setfill('0') << right  << seconds % 60 << "."
                    << setw(3) << setfill('0') << right  << millisec % 1000;

                if (entry.bankID > 0)
                    g_logData.file << " | " << setw(5) << setfill(' ') << left << ("B" + to_string(entry.bankID));
                else
                    g_logData.file << " |      ";

                if (entry.voiceID > 0)
                    g_logData.file << " | " << setw(6) << setfill(' ') << left << ("V" + to_string(entry.voiceID)) << " | ";
                else
                    g_logData.file << " |        | ";

                if (FAILED(entry.result))
                    g_logData.file << " ERROR (0x" << hex << entry.result << ") | ";

                g_logData.file << entry.message << endl;
            }

            batch.clear();
            g_logData.file.flush();

            if (!g_logData.logging)
                break;
        }

        g_logData.file.close();
    }

    void StartLogging()
    {
        if (!g_logData.workThread.joinable())
        {
            g_logData.logging = true;
            g_logData.workThread = thread(LogWorker);
        }
    }

    void StopLogging()
    {
        g_logData.logging = false;
        g_logData.condition.notify_one();

        if (g_logData.workThread.joinable())
            g_logData.workThread.join();
    }

    void Log(const INT32 bankID, const INT32 voiceId, const string& message, HRESULT result)
    {
        if (!g_logData.logging)
            return;

        INT64 timestamp = GetTime() - g_logData.startTime;

        {
            lock_guard<mutex> lock(g_logData.mutex);
            g_logData.queue.push({ timestamp, bankID, voiceId, message, 0 });
        }

        g_logData.condition.notify_one();
    }
}
#endif // LOGGING

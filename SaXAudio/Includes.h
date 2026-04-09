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

//---------------------------------------------------------------------
// Enables logging
//---------------------------------------------------------------------

//#define LOGGING

// Include minimal required stuff from windows
#ifdef _M_X64
#define _AMD64_
#elif defined _M_IX86
#define _X86_
#endif

#pragma comment(lib, "xaudio2.lib")

#include <minwindef.h>
#include <winnt.h>
#include <xaudio2.h>
#include <xaudio2fx.h>
#include <xapofx.h>
#include <unordered_map>
#include <queue>
#include <thread>
#include <atomic>
#include <mutex>
#include <condition_variable>
#include <chrono>
#include <string>
#include <fstream>
using namespace std;

#define STB_VORBIS_HEADER_ONLY
#include "stb_vorbis.c"

#define EXPORT extern "C" __declspec(dllexport)

#ifdef LOGGING
#pragma once
namespace SaXAudio
{
    void Log(const INT32 bankID, const INT32 voiceId, const string& message, HRESULT hr = 0);
    void StartLogging();
    void StopLogging();
}
#else
#pragma warning( disable : 4003)
#define Log(bankID, voiceId, message, hr)
#define StartLogging()
#define StopLogging()
#endif // LOGGING
# SaXAudio
SaXAudio is a _**S**imple **a**udio_ library using _**XAudio**_\
It provides an easy-to-use C++ interface for loading, playing, and manipulating audio with advanced features like real-time effects and audio routing.

## Features

### Core Audio Management
- **Audio Bank System**: Efficiently manage multiple audio assets with automatic memory handling
- **Ogg Vorbis Support**: Decode and play Ogg Vorbis audio files with asynchronous loading
- **Multiple Voice Playback**: Create multiple simultaneous audio voices from the same or different audio sources

### Voice Control & Effects
- **Real-time Parameter Control**: Adjust volume, speed, and panning during playback with smooth fading
- **Advanced Looping**: Set custom loop points with sample-accurate precision
- **Audio Effects**: Built-in reverb, EQ, and echo effects for voices and buses
- **Audio Routing**: Use buses to group and process multiple voices together

### Playback Features
- **Flexible Playback Control**: Start, stop, pause, and resume with fade transitions
- **Seek Support**: Jump to specific positions in audio (by sample or time)
- **Pause Stack System**: Handle multiple pause/resume operations gracefully
- **Voice Protection**: Protect specific voices from global pause/resume operations

### Advanced Features
- **Callback System**: Get notified when voices finish playing
- **Position Tracking**: Monitor playback position in real-time
- **Fade Transitions**: Smooth parameter changes and playback control
- **Resource Management**: Automatic cleanup and memory management

## Quick Start

### 1. Initialize the Audio System
```cpp
#include "SaXAudio.h"

// Initialize XAudio2 and create master voice
BOOL success = SaXAudio::Create();
if (!success) {
    // Handle initialization failure
    return false;
}
```

### 2. Load Audio Data
```cpp
// Option 1: Load directly from file (easiest)
INT32 bankID = SaXAudio::BankLoadOggFile("audio.ogg");
if (bankID == 0) {
    // Handle loading failure
}

// Option 2: Load from memory buffer
BYTE* audioBuffer = /* your audio data */;
UINT32 bufferSize = /* size of your audio data */;
INT32 bankID = SaXAudio::BankAddOgg(audioBuffer, bufferSize);
```

### 3. Create and Configure Voice
```cpp
// Create a bus for audio routing (optional)
INT32 busID = SaXAudio::CreateBus();

// Create a voice for playback
INT32 voiceID = SaXAudio::CreateVoice(bankID, busID, true); // starts paused
if (voiceID == 0) {
    // Handle voice creation failure
}

// Configure voice settings
SaXAudio::SetVolume(voiceID, 0.8f);        // 80% volume
SaXAudio::SetPanning(voiceID, -0.5f);      // Pan left
SaXAudio::SetLooping(voiceID, true);       // Enable looping
```

### 4. Start Playback
```cpp
// Start playing the voice
BOOL playing = SaXAudio::Start(voiceID);
if (!playing) {
    // Handle playback failure
}

// Or start at a specific time
SaXAudio::StartAtTime(voiceID, 10.5f); // Start at 10.5 seconds
```
**Note**: Voices are automatically destroyed when they finish playing (unless looping). Use the `SetOnFinishedCallback()` to get notified when this happens.

### 5. Runtime Control
```cpp
// Adjust parameters during playback with smooth transitions
SaXAudio::SetSpeed(voiceID, 0.5f, 2.0f);    // Slow to half speed over 2 seconds
SaXAudio::SetVolume(voiceID, 0.3f, 1.0f);   // Fade to 30% volume over 1 second

// Control playback
SaXAudio::Pause(voiceID, 0.5f);             // Pause with 0.5s fade
SaXAudio::Resume(voiceID, 0.5f);            // Resume with 0.5s fade
SaXAudio::Stop(voiceID, 1.0f);              // Stop with 1s fade (destroys voice)
```

### 6. Cleanup
```cpp
// Remove audio from bank when no longer needed
SaXAudio::BankRemove(bankID);

// Or use auto-removal (removes when all voices finish)
SaXAudio::BankAutoRemove(bankID);

// Shutdown audio system
SaXAudio::Release();
```

## Super Simple Example

For quick testing, you can use the one-liner function:

```cpp
#include "SaXAudio.h"

int main() {
    // Play a file with one function call (handles initialization too)
    INT32 voiceID = SaXAudio::PlayOggFile("sound.ogg");
    
    if (voiceID != 0) {
        // Optionally control the voice
        SaXAudio::SetVolume(voiceID, 0.5f);
        
        std::cout << "Playing... Press Enter to stop." << std::endl;
        std::cin.get();
        
        SaXAudio::Stop(voiceID);
    }
    
    SaXAudio::Release();
    return 0;
}
```

## Complete Example

```cpp
#include <iostream>
#include "SaXAudio.h"

// Callback function for when voices finish playing
void OnVoiceFinished(INT32 voiceID) {
    std::cout << "Voice " << voiceID << " finished playing" << std::endl;
}

int main() {
    // Initialize audio system
    if (!SaXAudio::Create()) {
        std::cerr << "Failed to initialize SaXAudio" << std::endl;
        return 1;
    }

    // Set up finished callback
    SaXAudio::SetOnFinishedCallback(OnVoiceFinished);

    // Load audio file - now much easier!
    INT32 bankID = SaXAudio::BankLoadOggFile("sound.ogg");
    if (bankID == 0) {
        std::cerr << "Failed to load audio file" << std::endl;
        SaXAudio::Release();
        return 1;
    }

    // Create bus and voice
    INT32 busID = SaXAudio::CreateBus();
    INT32 voiceID = SaXAudio::CreateVoice(bankID, busID);
    
    if (voiceID == 0) {
        std::cerr << "Failed to create voice" << std::endl;
        SaXAudio::BankRemove(bankID);
        SaXAudio::Release();
        return 1;
    }

    // Configure and start playback
    SaXAudio::SetVolume(voiceID, 0.7f);
    SaXAudio::SetLooping(voiceID, true);
    
    if (!SaXAudio::Start(voiceID)) {
        std::cerr << "Failed to start playback" << std::endl;
    } else {
        std::cout << "Playing audio... Press Enter to stop." << std::endl;
        std::cin.get();
        
        // Fade out and stop
        SaXAudio::Stop(voiceID, 1.0f);
    }

    // Cleanup
    SaXAudio::BankRemove(bankID);
    SaXAudio::Release();
    
    return 0;
}
```

## API Reference

### System Management
- `Create()` - Initialize XAudio2 and create master voice
- `Release()` - Clean up all resources
- `StartEngine()` / `StopEngine()` - Start/stop the audio engine

### Quick Play Functions
- `PlayOggFile(filePath, busID)` - One-call file loading and playback

### Global Controls
- `PauseAll(fade, busID)` - Pause all voices (or specific bus)
- `ResumeAll(fade, busID)` - Resume all voices (or specific bus)
- `StopAll(fade, busID)` - Stop all voices (or specific bus)
- `Protect(voiceID)` - Protect voice from global operations

### Audio Bank Management
- `BankAddOgg(buffer, length, callback)` - Add Ogg Vorbis data from memory
- `BankLoadOggFile(filePath)` - Load Ogg file directly into bank
- `BankRemove(bankID)` - Remove audio data from bank
- `BankAutoRemove(bankID)` - Auto-remove bank when all voices finish

### Voice Management
- `CreateVoice(bankID, busID, paused)` - Create new voice
- `VoiceExist(voiceID)` - Check if voice exists

### Bus Management
- `CreateBus()` - Create audio bus for grouping voices
- `RemoveBus(busID)` - Remove bus (stops all voices on it)

### Playback Control
- `Start(voiceID)` - Start voice playback
- `StartAtSample(voiceID, sample)` - Start/seek to specific sample
- `StartAtTime(voiceID, time)` - Start/seek to specific time
- `Stop(voiceID, fade)` - Stop voice with fade
- `Pause(voiceID, fade)` - Pause voice with fade (stacks)
- `Resume(voiceID, fade)` - Resume voice with fade (unstacks)
- `GetPauseStack(voiceID)` - Get current pause stack count

### Volume & Audio Parameters
- `SetMasterVolume(volume, fade)` - Set global master volume
- `SetVolume(voiceID, volume, fade, isBus)` - Set voice/bus volume [0.0-1.0]
- `SetSpeed(voiceID, speed, fade)` - Set playback speed (affects pitch)
- `SetPanning(voiceID, panning, fade)` - Set stereo panning [-1.0-1.0]
- `SetLooping(voiceID, looping)` - Enable/disable looping
- `SetLoopPoints(voiceID, start, end)` - Set custom loop boundaries

### Volume & Parameter Queries
- `GetMasterVolume()` - Get global master volume
- `GetVolume(voiceID)` - Get voice volume [0.0-1.0]
- `GetSpeed(voiceID)` - Get playback speed multiplier
- `GetPanning(voiceID)` - Get stereo panning [-1.0-1.0]
- `GetLooping(voiceID)` - Get looping state
- `GetLoopStart(voiceID)` / `GetLoopEnd(voiceID)` - Get loop boundaries

### Audio Effects
- `SetReverb(voiceID, params, fade, isBus)` - Apply reverb effect
- `RemoveReverb(voiceID, fade, isBus)` - Remove reverb effect
- `SetEq(voiceID, params, fade, isBus)` - Apply EQ effect
- `RemoveEq(voiceID, fade, isBus)` - Remove EQ effect
- `SetEcho(voiceID, params, fade, isBus)` - Apply echo effect
- `RemoveEcho(voiceID, fade, isBus)` - Remove echo effect

### Position & Timing Information
- `GetPositionSample(voiceID)` - Get current playback position in samples
- `GetPositionTime(voiceID)` - Get current playback position in seconds
- `GetTotalSample(voiceID)` - Get total audio length in samples
- `GetTotalTime(voiceID)` - Get total audio duration in seconds
- `GetSampleRate(voiceID)` - Get audio sample rate in Hz
- `GetChannelCount(voiceID)` - Get number of audio channels

### System Information
- `GetVoiceCount()` - Get number of currently active voices
- `GetBankCount()` - Get number of loaded audio banks

### Callbacks
- `SetOnFinishedCallback(callback)` - Set callback for when voices finish playing

## Requirements

- Windows 10 or later
- XAudio2 (included with Windows)
- Visual Studio 2019 or later (for building)

## Dependencies

- [XAudio2](https://learn.microsoft.com/en-us/windows/win32/xaudio2/) - Microsoft's audio API
- [stb_vorbis](https://github.com/nothings/stb/blob/master/stb_vorbis.c) - Ogg Vorbis decoder

## License

MIT License - see LICENSE file for details.

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.


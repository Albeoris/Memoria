using Memoria.Prime;
using Memoria.Prime.AKB2;
using SoLoud;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Global.Sound.SoLoud
{
    internal class SdLibAPIWithSoloud : ISdLibAPI
    {
        public static bool isQuitting = false;

        private static Soloud soloud = null;
        private static Bus sfxBus = null;
        private static Bus voiceBus = null;

        public override Int32 LastSoundID { get; protected set; } = -1;

        private class StreamInfo
        {
            public AKB2Header akbHeader;
            public WavStream data;
            public SoundProfile profile;
        }

        private class Sound
        {
            public Sound(Int32 BankID)
            {
                bankID = BankID;
            }
            public Int32 bankID;
            public Single volume = 1f;
            public Int32 pauseStack = 0;
        }

        private Dictionary<Int32, StreamInfo> streams = new Dictionary<Int32, StreamInfo>();
        private Dictionary<Int32, Sound> sounds = new Dictionary<Int32, Sound>();

        private void CleanUpSounds()
        {
            List<Int32> toRemove = new List<Int32>();
            foreach (var id in sounds.Keys)
            {
                if (soloud.isValidVoiceHandle((uint)id) == 0) toRemove.Add(id);
            }
            foreach (var id in toRemove)
            {
                sounds.Remove(id);
            }
            SoundLib.Log($"Total sounds: {sounds.Count}");
        }

        public override Int32 SdSoundSystem_Create(String config)
        {
            SoundLib.Log("Create");
            // Initialize SoLoud
            if (soloud == null)
            {
                soloud = new Soloud();
                soloud.init(1, 0, 48000);
                Log.Message($"[Soloud] backend: {soloud.getBackendString()} samplerate:{soloud.getBackendSamplerate()}");

                // Create buses
                sfxBus = new Bus();
                voiceBus = new Bus();
                soloud.play(sfxBus, 1);
                soloud.play(voiceBus, 1);

                GameLoopManager.Quit += () => { isQuitting = true; };
            }

            return 0;
        }

        public override void SdSoundSystem_Release()
        {
            SoundLib.Log("Release");

            soloud.stopAll();
            lock (streams) streams.Clear();
            soloud.deinit();
        }

        public override Int32 SdSoundSystem_Suspend()
        {
            SoundLib.Log("Suspend");
            foreach (var sound in sounds)
                SdSoundSystem_SoundCtrl_SetPause(sound.Key, 1, 0);
            return 0;
        }

        public override Int32 SdSoundSystem_Resume()
        {
            SoundLib.Log("Resume");
            foreach (var sound in sounds)
                SdSoundSystem_SoundCtrl_SetPause(sound.Key, 0, 0);
            return 0;
        }

        public override Int32 SdSoundSystem_AddData(IntPtr akb, SoundProfile profile)
        {
            SoundLib.Log($"AddData({akb})");
            unsafe
            {
                // Get the akb header
                Byte[] akbBin = new byte[304];
                Marshal.Copy(akb, akbBin, 0, 304);

                StreamInfo stream = new StreamInfo();
                UInt32 headerSize = stream.akbHeader.ReadFromBytes(akbBin);
                stream.data = new WavStream();
                stream.data.loadMem((IntPtr)((Byte*)akb + headerSize), stream.akbHeader.ContentSize, true, true);
                stream.profile = profile;

                Int32 bankID = (Int32)stream.data.objhandle;
                lock (streams) streams.Add(bankID, stream);
                return bankID;
            }
        }

        public override Int32 SdSoundSystem_AddStreamDataFromPath(String akbpath)
        {
            // Doesn't seem to be used
            SoundLib.Log($"No Implementation - AddStreamDataFromPath({akbpath})");
            return 0;
        }

        public override Int32 SdSoundSystem_RemoveData(Int32 bankID)
        {
            SoundLib.Log($"RemoveData({bankID})");
            lock (streams) return streams.Remove(bankID) ? 1 : 0;
        }

        public override Int32 SdSoundSystem_CreateSound(Int32 bankID)
        {
            if (!streams.ContainsKey(bankID)) return 0;
            StreamInfo stream = streams[bankID];

            SoundLib.Log($"CreateSound({stream.profile.ResourceID})");
            CleanUpSounds();

            Int32 soundID;
            switch (stream.profile.SoundProfileType)
            {
                case SoundProfileType.SoundEffect:
                case SoundProfileType.Sfx:
                    soundID = (Int32)sfxBus.play(stream.data, 1, 0, 1);
                    break;
                case SoundProfileType.Voice:
                    soundID = (Int32)voiceBus.play(stream.data, 1, 0, 1);
                    break;
                default:
                    soundID = (Int32)soloud.play(stream.data, 1, 0, 1);
                    break;
            }

            Sound voice = new Sound(bankID);
            sounds.Add(soundID, voice);

            // Is it a loop?
            if (stream.akbHeader.LoopEnd > 0)
            {
                double start = stream.akbHeader.LoopStart / (double)stream.akbHeader.SampleRate;
                double end = stream.akbHeader.LoopEnd / (double)stream.akbHeader.SampleRate;

                // Adjustment for looping sound at the top of Alexendria Castle (A. Castle/Altar)
                // It still pops after a while, Soloud just isn't precise enough with the looping
                if (stream.profile.ResourceID == "Sounds03/SE12/se120018")
                {
                    start = 104120 / (double)stream.akbHeader.SampleRate;
                    end = 216632 / (double)stream.akbHeader.SampleRate;
                }

                SoundLib.Log($"LoopStart: {start} LoopEnd: {end} Length: {stream.data.getLength()}");

                soloud.setLoopStartPoint((uint)soundID, start);
                soloud.setLoopEndPoint((uint)soundID, end);
            }

            LastSoundID = soundID;
            return soundID;
        }

        public override Int32 SdSoundSystem_SoundCtrl_GetElapsedPlaybackTime(Int32 soundID)
        {
            //SoundLib.Log($"SoundCtrl_GetElapsedPlaybackTime({soundID})");
            return (Int32)(soloud.getStreamTime((uint)soundID) * 1000);
        }

        public override Int32 SdSoundSystem_SoundCtrl_GetPlayTime(Int32 soundID)
        {
            // Doesn't seem to be used
            SoundLib.Log($"No Implementation - SoundCtrl_GetPlayTime({soundID}");
            return 0;
        }

        public override Int32 SdSoundSystem_SoundCtrl_Start(Int32 soundID, Int32 offsetTimeMSec)
        {
            if (!sounds.ContainsKey(soundID)) return 0;

            Int32 bankID = sounds[soundID].bankID;
            StreamInfo stream = streams[bankID];
            SoundLib.Log($"SoundCtrl_Start({stream.profile.ResourceID}({soundID}), {offsetTimeMSec})");

            Int32 count = 0;
            Int32 stopSoundID = 0;
            Double maxPos = 0d;
            foreach (var sound in sounds)
            {
                if (sound.Value.bankID == bankID && soloud.getPause((uint)sound.Key) == 0)
                {
                    Double pos = soloud.getStreamTime((uint)sound.Key);
                    if (pos < 0.01d)
                    {
                        // Prevent same sound to play more than once at very close interval (<10ms)
                        SoundLib.Log($"Sound already playing ({stream.profile.ResourceID}). Stopping {sound.Key} pos {(Int32)(pos * 1000)}ms");
                        soloud.stop((uint)sound.Key);
                    }
                    else
                    {
                        SoundLib.Log($"Sound already playing ({stream.profile.ResourceID}). Id {sound.Key} pos {(Int32)(pos * 1000)}ms");
                        count++;
                        if (pos > maxPos)
                        {
                            stopSoundID = sound.Key;
                            maxPos = pos;
                        }
                    }
                }
            }

            if (count >= 2)
            {
                // Prevent same sound to play more than twice at the same time by stopping the oldest
                SoundLib.Log($"Stopping {stopSoundID} pos {(Int32)(maxPos * 1000)}ms");
                soloud.fadeVolume((uint)stopSoundID, 0, 100 / 1000d);
                soloud.scheduleStop((uint)stopSoundID, 100 / 1000d);
            }

            if (offsetTimeMSec > 0)
            {
                soloud.seek((uint)soundID, offsetTimeMSec / 1000d);
            }
            if (sounds[soundID].pauseStack == 0) // SdLib doesn't resume paused sounds, we replicate the behavior
                soloud.setPause((uint)soundID, 0);

            return 1;
        }

        public override void SdSoundSystem_SoundCtrl_Stop(Int32 soundID, Int32 transTimeMSec)
        {
            if (!sounds.ContainsKey(soundID)) return;
            SoundLib.Log($"SoundCtrl_Stop({streams[sounds[soundID].bankID].profile.ResourceID}({soundID}), {transTimeMSec})");

            if (transTimeMSec < 100) transTimeMSec = 100; // Add a small fade

            soloud.fadeVolume((uint)soundID, 0, transTimeMSec / 1000d);
            soloud.scheduleStop((uint)soundID, transTimeMSec / 1000d);
        }

        public override Int32 SdSoundSystem_SoundCtrl_IsExist(Int32 soundID)
        {
            return soloud.isValidVoiceHandle((uint)soundID);
        }

        public override void SdSoundSystem_SoundCtrl_SetPause(Int32 soundID, Int32 pauseOn, Int32 transTimeMSec)
        {
            if (!sounds.ContainsKey(soundID)) return;
            SoundLib.Log($"SoundCtrl_SetPause({streams[sounds[soundID].bankID].profile.ResourceID}({soundID}), {pauseOn}, {transTimeMSec})");

            if (pauseOn > 0)
                sounds[soundID].pauseStack++;
            else if (sounds[soundID].pauseStack > 0)
                sounds[soundID].pauseStack--;

            if (pauseOn == 0 && sounds[soundID].pauseStack != 0) return;

            uint h = (uint)soundID;
            double t = transTimeMSec / 1000d;

            if (transTimeMSec > 0)
            {
                if (pauseOn != 0)
                {
                    soloud.fadeVolume(h, 0, t);
                    soloud.schedulePause(h, t);
                }
                else
                {
                    soloud.setPause(h, pauseOn);
                    soloud.setVolume(h, 0);
                    soloud.fadeVolume(h, sounds[soundID].volume, t);
                }
            }
            else
            {
                //Should a small fade (100ms) be added?
                soloud.setPause(h, pauseOn);
                soloud.setVolume(h, sounds[soundID].volume);
            }
        }

        public override Int32 SdSoundSystem_SoundCtrl_IsPaused(Int32 soundID)
        {
            SoundLib.Log("SoundCtrl_IsPaused");
            return soloud.getPause((uint)soundID);
        }

        public override void SdSoundSystem_SoundCtrl_SetVolume(Int32 soundID, Single volume, Int32 transTimeMSec)
        {
            if (!sounds.ContainsKey(soundID)) return;
            SoundLib.Log($"SoundCtrl_SetVolume({streams[sounds[soundID].bankID].profile.ResourceID}({soundID}), {volume}, {transTimeMSec})");

            if (volume < 0f || volume > 5f) Log.Warning($"[SoLoud] Unexpected volume value. ResourceID = {streams[sounds[soundID].bankID].profile.ResourceID} Volume = {volume}\n{Environment.StackTrace}");
            volume = Mathf.Clamp(volume, 0f, 5f);
            sounds[soundID].volume = volume;

            if (transTimeMSec > 0)
            {
                soloud.fadeVolume((uint)soundID, volume, transTimeMSec / 1000d);
            }
            else
            {
                soloud.setVolume((uint)soundID, volume);
            }
        }

        public override Single SdSoundSystem_SoundCtrl_GetVolume(Int32 soundID)
        {
            SoundLib.Log($"SoundCtrl_GetVolume({soundID})");
            return soloud.getVolume((uint)soundID);
        }

        public override void SdSoundSystem_SoundCtrl_SetPitch(Int32 soundID, Single pitch, Int32 transTimeMSec)
        {
            // Soloud doesn't handle pitch (yet)
            if (!sounds.ContainsKey(soundID)) return;
            SoundLib.Log($"SoundCtrl_SetPitch({streams[sounds[soundID].bankID].profile.ResourceID}({soundID}), {pitch}, {transTimeMSec})");

            // Play at different speed (changes the pitch)
            if (transTimeMSec > 0)
            {
                soloud.fadeRelativePlaySpeed((uint)soundID, pitch, transTimeMSec / 1000d);
            }
            else
            {
                soloud.setRelativePlaySpeed((uint)soundID, pitch);
            }
        }

        public override void SdSoundSystem_SoundCtrl_SetPanning(Int32 soundID, Single panning, Int32 transTimeMSec)
        {
            if (!sounds.ContainsKey(soundID)) return;
            SoundLib.Log($"SoundCtrl_SetPanning({streams[sounds[soundID].bankID].profile.ResourceID}({soundID}), {panning}, {transTimeMSec})");

            if (transTimeMSec > 0)
            {
                soloud.fadePan((uint)soundID, panning, transTimeMSec / 1000d);
            }
            else
            {
                soloud.setPan((uint)soundID, panning);
            }
        }

        public override void SdSoundSystem_SoundCtrl_SetNextLoopRegion(Int32 soundID)
        {
            if (!sounds.ContainsKey(soundID)) return;
            SoundLib.Log($"SoundCtrl_SetNextLoopRegion({streams[sounds[soundID].bankID].profile.ResourceID}({soundID}))");

            StreamInfo stream = streams[sounds[soundID].bankID];
            if (stream.akbHeader.LoopEndAlternate > 0)
            {
                double start = stream.akbHeader.LoopStartAlternate / (double)stream.akbHeader.SampleRate;
                double end = stream.akbHeader.LoopEndAlternate / (double)stream.akbHeader.SampleRate;

                SoundLib.Log($"LoopStart: ({start}) LoopEnd: {end}");

                soloud.setLoopStartPoint((uint)soundID, start);
                soloud.setLoopEndPoint((uint)soundID, end);
            }
        }

        public override Int32 SdSoundSystem_BankCtrl_IsLoop(Int32 bankID, Int32 soundID)
        {
            // Doesn't seem to be used
            SoundLib.Log($"No Implementation - BankCtrl_IsLoop({bankID}, {soundID})");
            return 0;
        }
    }
}

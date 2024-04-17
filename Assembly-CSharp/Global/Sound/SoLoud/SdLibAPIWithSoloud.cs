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
        private const Int32 AkbHeaderSize = 304;

        private static Soloud soloud = null;
        private static Bus sfxBus = null;
        private static Bus voiceBus = null;

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
                Memoria.Prime.Log.Message($"[Soloud] backend: {soloud.getBackendString()} samplerate:{soloud.getBackendSamplerate()}");

                // Create buses
                sfxBus = new Bus();
                voiceBus = new Bus();
                soloud.play(sfxBus, 1);
                soloud.play(voiceBus, 1);
            }

            return 0;
        }

        public override void SdSoundSystem_Release()
        {
            SoundLib.Log("Release");

            soloud.stopAll();
            streams.Clear();
            soloud.deinit();
        }

        public override Int32 SdSoundSystem_Suspend()
        {
            SoundLib.Log("Suspend");
            soloud.setPauseAll(1);
            return 0;
        }

        public override Int32 SdSoundSystem_Resume()
        {
            SoundLib.Log("Resume");
            soloud.setPauseAll(0);
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
                stream.akbHeader.ReadFromBytes(akbBin);
                stream.data = new WavStream();
                stream.data.loadMem((IntPtr)((Byte*)akb + AkbHeaderSize), (uint)stream.akbHeader.ContentSize, true, true);
                stream.profile = profile;

                Int32 bankID = (Int32)stream.data.objhandle;
                streams.Add(bankID, stream);
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
            return streams.Remove(bankID) ? 1 : 0;
        }

        public override Int32 SdSoundSystem_CreateSound(Int32 bankID)
        {
            SoundLib.Log($"CreateSound({bankID})");
            CleanUpSounds();

            StreamInfo stream = streams[bankID];

            Int32 soundID = 0;
            switch (stream.profile.SoundProfileType)
            {
                case SoundProfileType.Sfx:
                    soundID = (Int32)sfxBus.play(streams[bankID].data, 1, 0, 1);
                    break;
                case SoundProfileType.Voice:
                    soundID = (Int32)voiceBus.play(streams[bankID].data, 1, 0, 1);
                    break;
                default:
                    soundID = (Int32)soloud.play(streams[bankID].data, 1, 0, 1);
                    break;
            }

            Sound voice = new Sound(bankID);
            sounds.Add(soundID, voice);

            // Is it a loop?
            if (stream.akbHeader.LoopEnd > 0)
            {
                double start = stream.akbHeader.LoopStart / (double)stream.akbHeader.SampleRate;
                double end = stream.akbHeader.LoopEnd / (double)stream.akbHeader.SampleRate;

                SoundLib.Log($"LoopStart: {start} LoopEnd: {end} Length: {stream.data.getLength()}");

                soloud.setLoopStartPoint((uint)soundID, start);
                soloud.setLoopEndPoint((uint)soundID, end);
            }

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
            SoundLib.Log($"SoundCtrl_Start({soundID}, {offsetTimeMSec})");
            if (!sounds.ContainsKey(soundID)) return 0;

            if (offsetTimeMSec > 0)
            {
                soloud.seek((uint)soundID, offsetTimeMSec / 1000d);
            }
            soloud.setPause((uint)soundID, 0);

            var stream = streams[sounds[soundID].bankID];
            if (stream.akbHeader.LoopEnd > 0)
            {
                Memoria.Prime.Log.Message($"[SoLoud] Starting loop: {stream.profile.ResourceID}");
            }
            return 1;
        }

        public override void SdSoundSystem_SoundCtrl_Stop(Int32 soundID, Int32 transTimeMSec)
        {
            SoundLib.Log($"SoundCtrl_Stop({soundID}, {transTimeMSec})");
            if (!sounds.ContainsKey(soundID)) return;

            if (transTimeMSec < 100) transTimeMSec = 100; // Add a small fade

            soloud.fadeVolume((uint)soundID, 0, transTimeMSec / 1000d);
            soloud.scheduleStop((uint)soundID, transTimeMSec / 1000d);
        }

        public override Int32 SdSoundSystem_SoundCtrl_IsExist(Int32 soundID)
        {
            return soloud.isValidVoiceHandle((uint)soundID); ;
        }

        public override void SdSoundSystem_SoundCtrl_SetPause(Int32 soundID, Int32 pauseOn, Int32 transTimeMSec)
        {
            SoundLib.Log($"SoundCtrl_SetPause({soundID}, {pauseOn}, {transTimeMSec})");
            if (!sounds.ContainsKey(soundID)) return;

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
            SoundLib.Log($"SoundCtrl_SetVolume({soundID}, {volume}, {transTimeMSec})");
            if (!sounds.ContainsKey(soundID)) return;

            if (volume < 0f || volume > 1f) Memoria.Prime.Log.Message($"[SoLoud] Warning! Unexpected volume value. Volume = {volume} SoundID = {soundID}\n{Environment.StackTrace}");
            volume = Mathf.Clamp01(volume);
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
            SoundLib.Log($"SoundCtrl_SetPitch({soundID}, {pitch}, {transTimeMSec})");
            //if (pitch != HonoBehaviorSystem.Instance.GetFastForwardFactor()) Memoria.Prime.Log.Message($"[SoLoud] SetPitch {streams[sounds[soundID].bankID].profile.ResourceID}, {pitch}\n{Environment.StackTrace}");

            // Play at higher speed for fast forward
            float speed = soloud.getRelativePlaySpeed((uint)soundID);
            if (pitch == HonoBehaviorSystem.Instance.GetFastForwardFactor() && speed != pitch)
            {
                if (transTimeMSec > 0)
                {
                    soloud.fadeRelativePlaySpeed((uint)soundID, pitch, transTimeMSec / 1000d);
                }
                else
                {
                    soloud.setRelativePlaySpeed((uint)soundID, pitch);
                }
            }
        }

        public override void SdSoundSystem_SoundCtrl_SetPanning(Int32 soundID, Single panning, Int32 transTimeMSec)
        {
            SoundLib.Log($"SoundCtrl_SetPanning({soundID}, {panning}, {transTimeMSec})");
            if (!sounds.ContainsKey(soundID)) return;

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
            SoundLib.Log($"SoundCtrl_SetNextLoopRegion({soundID})");
            if (!sounds.ContainsKey(soundID)) return;

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

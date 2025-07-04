using Memoria.Prime;
using Memoria.Prime.AKB2;
using SoLoud;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Global.Sound.SaXAudio
{
    internal class SdLibAPIWithSaXAudio : ISdLibAPI
    {
        private struct BankData
        {
            public Int32 SoundID;

            public SoundProfile Profile;

            public UInt32 LoopStart;
            public UInt32 LoopEnd;
            public UInt32 LoopStart2;
            public UInt32 LoopEnd2;
        }

        private static readonly Dictionary<Int32, BankData> bankData = new Dictionary<Int32, BankData>();
        private static readonly Dictionary<Int32, Int32> sounds = new Dictionary<Int32, Int32>();

        public Int32 BusMusic { get; private set; }
        public Int32 BusAmbient { get; private set; }
        public Int32 BusSoundEffect { get; private set; }
        public Int32 BusVoice { get; private set; }

        public override Int32 LastSoundID { get; protected set; } = -1;

        public override Int32 SdSoundSystem_Create(String config)
        {
            SoundLib.Log("Create");
            if (!SaXAudio.Init())
            {
                Log.Warning($"[SaXAudio] Initialization failed");
                return -1;
            }
            BusMusic = SaXAudio.CreateBus();
            BusAmbient = SaXAudio.CreateBus();
            BusSoundEffect = SaXAudio.CreateBus();
            BusVoice = SaXAudio.CreateBus();
            AudioEffectManager.Initialize();
            Log.Message($"[SaXAudio] Initialized");
            return 0;
        }

        public override void SdSoundSystem_Release()
        {
            SoundLib.Log("Release");
            // Release is only called when the game closes which at this point the DLL might already be unloaded,
            // leading to an unnecessary crash. Let Windows do the clean-up.
            //SaXAudio.Release();
        }

        public override Int32 SdSoundSystem_Suspend()
        {
            SoundLib.Log("Suspend");
            SaXAudio.StopEngine();
            return 0;
        }

        public override Int32 SdSoundSystem_Resume()
        {
            SoundLib.Log("Resume");
            SaXAudio.StartEngine();
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

                AKB2Header header = new AKB2Header();
                UInt32 headerSize = header.ReadFromBytes(akbBin);

                static void DeleteAkbBin(Int32 bankID, IntPtr buffer)
                {
                    if (!bankData.ContainsKey(bankID))
                        return;
                    IntPtr akbBin = bankData[bankID].Profile.AkbBin;
                    if (akbBin != IntPtr.Zero)
                    {
                        bankData[bankID].Profile.AkbBin = IntPtr.Zero;
                        Marshal.FreeHGlobal(akbBin);
                    }
                }

                Int32 bankID = SaXAudio.BankAddOgg((IntPtr)((Byte*)akb + headerSize), header.ContentSize, DeleteAkbBin);
                bankData[bankID] = new BankData
                {
                    Profile = profile,
                    LoopStart = header.LoopStart,
                    LoopEnd = header.LoopEnd,
                    LoopStart2 = header.LoopStartAlternate,
                    LoopEnd2 = header.LoopEndAlternate
                };
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
            SaXAudio.BankRemove(bankID);
            bankData.Remove(bankID);
            return 0;
        }

        public override Int32 SdSoundSystem_CreateSound(Int32 bankID)
        {
            if (!bankData.ContainsKey(bankID))
            {
                Log.Warning($"The BankID {bankID} was not found");
            }
            BankData data = bankData[bankID];
            Int32 busID = 0;

            switch (data.Profile.SoundProfileType)
            {
                case SoundProfileType.Music:
                    busID = BusMusic;
                    break;
                case SoundProfileType.Voice:
                    busID = BusVoice;
                    break;
                case SoundProfileType.SoundEffect:
                case SoundProfileType.Sfx:
                case SoundProfileType.Song:
                    if (!data.Profile.ResourceID.StartsWith("Sounds02/SE00"))
                        busID = data.LoopEnd != 0 ? BusAmbient : BusSoundEffect;
                    break;
            }

            AudioEffectManager.EffectPreset? preset = AudioEffectManager.GetPreset(data.Profile, busID);
            if (preset != null && busID != 0)
            {
                Log.Message($"[AudioEffectManager] Filtered '{data.Profile.ResourceID}' from bus {busID}");
                busID = 0;
            }

            Int32 soundID = SaXAudio.CreateVoice(bankID, busID);
            if (soundID >= 0)
            {
                if (data.LoopEnd > 0)
                {
                    SaXAudio.SetLoopPoints(soundID, data.LoopStart, data.LoopEnd);
                    SaXAudio.SetLooping(soundID, true);
                    data.SoundID = soundID;
                    bankData[bankID] = data;
                }
            }

            if (preset != null)
                AudioEffectManager.ApplyPresetOnSound(preset.Value, soundID, data.Profile.Name);

            sounds[soundID] = bankID;
            LastSoundID = soundID;
            return soundID;
        }

        public override Int32 SdSoundSystem_SoundCtrl_GetElapsedPlaybackTime(Int32 soundID)
        {
            return (Int32)(SaXAudio.GetPositionTime(soundID) * 1000f);
        }

        public override Int32 SdSoundSystem_SoundCtrl_GetPlayTime(Int32 soundID)
        {
            return (Int32)(SaXAudio.GetTotalTime(soundID) * 1000f);
        }

        public override Int32 SdSoundSystem_SoundCtrl_Start(Int32 soundID, Int32 offsetTimeMSec)
        {
            Int32 bankID = sounds[soundID];
            HashSet<Int32> toDelete = new HashSet<Int32>();
            foreach (var sound in sounds)
            {
                if(!SaXAudio.VoiceExist(sound.Key))
                {
                    toDelete.Add(sound.Key);
                    continue;
                }
                if (sound.Value == bankID && SaXAudio.GetPauseStack(soundID) == 0)
                {
                    Double pos = SaXAudio.GetPositionTime(sound.Key);
                    if (pos < 0.01d)
                    {
                        // Prevent same sound to play more than once at very close interval (<10ms)
                        SoundLib.Log($"Sound already playing ({bankData[sound.Value].Profile.ResourceID}). Stopping {sound.Key} pos {(Int32)(pos * 1000)}ms");
                        SaXAudio.Stop(sound.Key);
                    }
                }
            }
            foreach(Int32 key in toDelete)
                sounds.Remove(key);

            return SaXAudio.StartAtTime(soundID, offsetTimeMSec / 1000f) ? 1 : 0;
        }

        public override void SdSoundSystem_SoundCtrl_Stop(Int32 soundID, Int32 transTimeMSec)
        {
            if (transTimeMSec < 100) transTimeMSec = 100; // Add a small fade
            SaXAudio.Stop(soundID, transTimeMSec / 1000f);
        }

        public override Int32 SdSoundSystem_SoundCtrl_IsExist(Int32 soundID)
        {
            return SaXAudio.VoiceExist(soundID) ? 1 : 0;
        }

        public override void SdSoundSystem_SoundCtrl_SetPause(Int32 soundID, Int32 pauseOn, Int32 transTimeMSec)
        {
            if (pauseOn != 0)
                SaXAudio.Pause(soundID, transTimeMSec / 1000f);
            else
                SaXAudio.Resume(soundID, transTimeMSec / 1000f);
        }

        public override Int32 SdSoundSystem_SoundCtrl_IsPaused(Int32 soundID)
        {
            SoundLib.Log("SoundCtrl_IsPaused");
            return SaXAudio.GetPauseStack(soundID) > 0 ? 1 : 0;
        }

        public override void SdSoundSystem_SoundCtrl_SetVolume(Int32 soundID, Single volume, Int32 transTimeMSec)
        {
            SaXAudio.SetVolume(soundID, volume, transTimeMSec / 1000f);
        }

        public override Single SdSoundSystem_SoundCtrl_GetVolume(Int32 soundID)
        {
            SoundLib.Log($"SoundCtrl_GetVolume({soundID})");
            return SaXAudio.GetVolume(soundID);
        }

        public override void SdSoundSystem_SoundCtrl_SetPitch(Int32 soundID, Single pitch, Int32 transTimeMSec)
        {
            SaXAudio.SetSpeed(soundID, pitch, transTimeMSec / 1000f);
        }

        public override void SdSoundSystem_SoundCtrl_SetPanning(Int32 soundID, Single panning, Int32 transTimeMSec)
        {
            SaXAudio.SetPanning(soundID, panning, transTimeMSec / 1000f);
        }

        public override void SdSoundSystem_SoundCtrl_SetNextLoopRegion(Int32 soundID)
        {
            if (!sounds.TryGetValue(soundID, out Int32 bankID) || !bankData.ContainsKey(bankID))
                return;

            UInt32 start = bankData[bankID].LoopStart2;
            UInt32 end = bankData[bankID].LoopEnd2;
            if (end > 0)
            {
                SaXAudio.SetLoopPoints(soundID, start, end);
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

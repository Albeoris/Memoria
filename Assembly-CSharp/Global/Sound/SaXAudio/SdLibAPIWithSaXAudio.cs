using Memoria.Prime;
using Memoria.Prime.AKB2;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Global.Sound.SaXAudio
{
    internal class SdLibAPIWithSaXAudio : ISdLibAPI
    {
        private const Int32 AkbHeaderSize = 304;
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

        private Int32 busSoundEffects;
        private Int32 busVoices;

        public Int32 LastSoundID { get; private set; } = -1;

        public override Int32 SdSoundSystem_Create(String config)
        {
            SoundLib.Log("Create");
            if (SaXAudio.Init())
            {
                busSoundEffects = SaXAudio.CreateBus();
                busVoices = SaXAudio.CreateBus();
                Log.Message($"[SaXAudio] Initialized");
                return 0;
            }
            Log.Warning($"[SaXAudio] Initialization failed");
            return -1;
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
                header.ReadFromBytes(akbBin);

                static void DeleteAkbBin(Int32 bankID)
                {
                    if (!bankData.ContainsKey(bankID))
                        return;
                    IntPtr akbBin = bankData[bankID].Profile.AkbBin;
                    if (akbBin != IntPtr.Zero)
                    {
                        Log.Message($"[SaXAudio] Deleting B{bankID}");
                        Marshal.FreeHGlobal(akbBin);
                        bankData[bankID].Profile.AkbBin = IntPtr.Zero;
                    }
                }

                Int32 bankID = SaXAudio.BankAddOgg((IntPtr)((Byte*)akb + AkbHeaderSize), header.ContentSize, DeleteAkbBin);
                bankData[bankID] = new BankData
                {
                    Profile = profile,
                    LoopStart = header.LoopStart,
                    LoopEnd = header.LoopEnd,
                    LoopStart2 = header.LoopStartAlternate,
                    LoopEnd2 = header.LoopEndAlternate
                };
                Log.Message($"[SaXAudio] Added B{bankID} '{profile.ResourceID}'");
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
            SaXAudio.BankRemove(bankID);
            bankData.Remove(bankID);
            return 0;
        }

        public override Int32 SdSoundSystem_CreateSound(Int32 bankID)
        {
            BankData data = bankData[bankID];
            Int32 busID = -1;

            if (data.Profile.SoundProfileType == SoundProfileType.Voice)
            {
                busID = busVoices;
            }
            else if ((data.Profile.SoundProfileType == SoundProfileType.SoundEffect || data.Profile.SoundProfileType == SoundProfileType.Sfx)
                && data.LoopEnd == 0
                && !data.Profile.ResourceID.StartsWith("Sounds02/SE00"))
            {
                busID = busSoundEffects;
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
            LastSoundID = soundID;
            return soundID;
        }

        public override Int32 SdSoundSystem_SoundCtrl_GetElapsedPlaybackTime(Int32 soundID)
        {
            //SoundLib.Log($"SoundCtrl_GetElapsedPlaybackTime({soundID})");
            return (Int32)(SaXAudio.GetPositionTime(soundID) * 1000f);
        }

        public override Int32 SdSoundSystem_SoundCtrl_GetPlayTime(Int32 soundID)
        {
            return (Int32)(SaXAudio.GetTotalTime(soundID) * 1000f);
        }

        public override Int32 SdSoundSystem_SoundCtrl_Start(Int32 soundID, Int32 offsetTimeMSec)
        {
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
            foreach (BankData loop in bankData.Values)
            {
                if (loop.SoundID == soundID)
                {
                    SaXAudio.SetLoopPoints(soundID, loop.LoopStart2, loop.LoopEnd2);
                    break;
                }
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

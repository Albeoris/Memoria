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
        private struct LoopData
        {
            public Int32 SoundID;
            public UInt32 Start;
            public UInt32 End;
            public UInt32 Start2;
            public UInt32 End2;
        }

        private Dictionary<Int32, LoopData> loopData = new Dictionary<Int32, LoopData>();

        public override Int32 SdSoundSystem_Create(String config)
        {
            SoundLib.Log("Create");
            if (SaXAudio.Init())
            {
                Log.Message($"[SaXAudio] Initialized");
                return 0;
            }
            Log.Warning($"[SaXAudio] Initialization failed");
            return -1;
        }

        public override void SdSoundSystem_Release()
        {
            SoundLib.Log("Release");
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

                Int32 bankID = SaXAudio.BankAddOgg((IntPtr)((Byte*)akb + AkbHeaderSize), header.ContentSize);
                if (bankID >= 0 && header.LoopEnd > 0)
                {
                    loopData[bankID] = new LoopData
                    {
                        Start = header.LoopStart,
                        End = header.LoopEnd,
                        Start2 = header.LoopStartAlternate,
                        End2 = header.LoopEndAlternate
                    };
                }
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
            loopData.Remove(bankID);
            return 0;
        }

        public override Int32 SdSoundSystem_CreateSound(Int32 bankID)
        {
            Int32 soundID = SaXAudio.CreateVoice(bankID);
            if (soundID >= 0)
            {
                if (loopData.ContainsKey(bankID))
                {
                    SaXAudio.SetLoopPoints(soundID, loopData[bankID].Start, loopData[bankID].End);
                    SaXAudio.SetLooping(soundID, true);
                    LoopData loop = loopData[bankID];
                    loop.SoundID = soundID;
                    loopData[bankID] = loop;
                }
            }
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
            if (transTimeMSec < 100) transTimeMSec = 200; // Add a small fade
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
            foreach (LoopData loop in loopData.Values)
            {
                if (loop.SoundID == soundID)
                {
                    SaXAudio.SetLoopPoints(soundID, loop.Start2, loop.End2);
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

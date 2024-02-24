using SoLoud;
using System;
using System.Collections.Generic;

namespace Global.Sound.SoLoud
{
    internal class SdLibAPIWithSoloud : ISdLibAPI
    {
        private const Int32 AkbHeaderSize = 304;

        private Soloud soloud;
        private Dictionary<Int32, WavStream> streams = new Dictionary<Int32, WavStream>();

        public override Int32 SdSoundSystem_Create(String config)
        {
            // Initialize SoLoud
            soloud = new Soloud();
            soloud.init(1, 0, 48000);
            SoundLib.Log($"Create backend: {soloud.getBackendString()} samplerate:{soloud.getBackendSamplerate()}");
            
            return 0;
        }

        public override void SdSoundSystem_Release()
        {
            SoundLib.Log("Release");
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

        public override Int32 SdSoundSystem_AddData(IntPtr akb)
        {
            SoundLib.Log("AddData");
            unsafe
            {
                // Get the file size from the akb header (3rd Int32)
                Int32 size = *(((Int32*)akb) + 2);
                //Log.Message($"[DEBUG] size: {size}");
                WavStream stream = new WavStream();
                SoundLib.Log($"size {(uint)(size - AkbHeaderSize)} IntPtr {akb} OggPtr {(IntPtr)((Byte*)akb + AkbHeaderSize)}");
                stream.loadMem((IntPtr)((Byte*)akb + AkbHeaderSize), (uint)(size - AkbHeaderSize));

                Int32 bankID = (Int32)stream.objhandle;
                streams.Add(bankID, stream);
                return bankID;
            }
        }

        public override Int32 SdSoundSystem_AddStreamDataFromPath(String akbpath)
        {
            SoundLib.Log("No Implementation - AddStreamDataFromPath");
            return 0;
        }

        public override Int32 SdSoundSystem_RemoveData(Int32 bankID)
        {
            SoundLib.Log("RemoveData");
            return streams.Remove(bankID) ? 1 : 0;
        }

        public override Int32 SdSoundSystem_CreateSound(Int32 bankID)
        {
            SoundLib.Log("CreateSound");
            return (Int32)soloud.play(streams[bankID], 1, 0, 1);
        }

        public override Int32 SdSoundSystem_SoundCtrl_GetElapsedPlaybackTime(Int32 soundID)
        {
            SoundLib.Log("SoundCtrl_GetElapsedPlaybackTime");
            return (Int32)(soloud.getStreamTime((uint)soundID) * 1000);
        }

        public override Int32 SdSoundSystem_SoundCtrl_GetPlayTime(Int32 soundID)
        {
            SoundLib.Log("No Implementation - SoundCtrl_GetPlayTime");
            return 0;
        }

        public override Int32 SdSoundSystem_SoundCtrl_Start(Int32 soundID, Int32 offsetTimeMSec)
        {
            SoundLib.Log("SoundCtrl_Start");
            if (offsetTimeMSec > 0)
            {
                soloud.seek((uint)soundID, offsetTimeMSec / 1000d);
            }
            soloud.setPause((uint)soundID, 0);
            return 1;
        }

        public override void SdSoundSystem_SoundCtrl_Stop(Int32 soundID, Int32 transTimeMSec)
        {
            SoundLib.Log("SoundCtrl_Stop");
            // TODO: fade if transTimeMSec > 0
            soloud.stop((uint)soundID);
        }

        public override Int32 SdSoundSystem_SoundCtrl_IsExist(Int32 soundID)
        {
            SoundLib.Log("SoundCtrl_IsExist");
            return soloud.isValidVoiceHandle((uint)soundID);
        }

        public override void SdSoundSystem_SoundCtrl_SetPause(Int32 soundID, Int32 pauseOn, Int32 transTimeMSec)
        {
            SoundLib.Log("SoundCtrl_SetPause");
            // TODO: fade if transTimeMSec > 0
            soloud.setPause((uint)soundID, pauseOn);
        }

        public override Int32 SdSoundSystem_SoundCtrl_IsPaused(Int32 soundID)
        {
            SoundLib.Log("SoundCtrl_IsPaused");
            return soloud.getPause((uint)soundID);
        }

        public override void SdSoundSystem_SoundCtrl_SetVolume(Int32 soundID, Single volume, Int32 transTimeMSec)
        {
            SoundLib.Log("SoundCtrl_SetVolume");
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
            SoundLib.Log("SoundCtrl_GetVolume");
            return soloud.getVolume((uint)soundID);
        }

        public override void SdSoundSystem_SoundCtrl_SetPitch(Int32 soundID, Single pitch, Int32 transTimeMSec)
        {
            SoundLib.Log("No Implementation - SoundCtrl_SetPitch");
        }

        public override void SdSoundSystem_SoundCtrl_SetPanning(Int32 soundID, Single panning, Int32 transTimeMSec)
        {
            SoundLib.Log("SoundCtrl_SetPanning");
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
            SoundLib.Log("No Implementation - SoundCtrl_SetNextLoopRegion");
        }

        public override Int32 SdSoundSystem_BankCtrl_IsLoop(Int32 bankID, Int32 soundID)
        {
            SoundLib.Log("No Implementation - BankCtrl_IsLoop");
            return 0;
        }
    }
}

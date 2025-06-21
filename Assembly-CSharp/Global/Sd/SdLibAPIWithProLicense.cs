using System;
using System.Runtime.InteropServices;

public class SdLibAPIWithProLicense : ISdLibAPI
{
    [DllImport("SdLib", CharSet = CharSet.Ansi, EntryPoint = "SdSoundSystem_SoundCtrl_GetElapsedPlaybackTime")]
    private static extern Int32 DLLSdSoundSystem_SoundCtrl_GetElapsedPlaybackTime(Int32 soundID);

    [DllImport("SdLib", CharSet = CharSet.Ansi, EntryPoint = "SdSoundSystem_SoundCtrl_GetPlayTime")]
    private static extern Int32 DLLSdSoundSystem_SoundCtrl_GetPlayTime(Int32 soundID);

    public override Int32 LastSoundID { get; protected set; } = -1;

    public override Int32 SdSoundSystem_SoundCtrl_GetElapsedPlaybackTime(Int32 soundID)
    {
        // The duration during which the sound has played; it turns back to 0 when the sound stops
        return DLLSdSoundSystem_SoundCtrl_GetElapsedPlaybackTime(soundID);
    }

    public override Int32 SdSoundSystem_SoundCtrl_GetPlayTime(Int32 soundID)
    {
        // Some loop duration (either the time at which the sound loops or the loop duration); 0 if it doesn't loop
        return DLLSdSoundSystem_SoundCtrl_GetPlayTime(soundID);
    }

    public override Int32 SdSoundSystem_Create(String config)
    {
        LastSoundID = SdLibAPI.SdSoundSystem_Create(config);
        return LastSoundID;
    }

    public override void SdSoundSystem_Release()
    {
        SdLibAPI.SdSoundSystem_Release();
    }

    public override Int32 SdSoundSystem_Suspend()
    {
        return SdLibAPI.SdSoundSystem_Suspend();
    }

    public override Int32 SdSoundSystem_Resume()
    {
        return SdLibAPI.SdSoundSystem_Resume();
    }

    public override Int32 SdSoundSystem_AddData(IntPtr akb, SoundProfile profile)
    {
        return SdLibAPI.SdSoundSystem_AddData(akb);
    }

    public override Int32 SdSoundSystem_AddStreamDataFromPath(String akbpath)
    {
        return SdLibAPI.SdSoundSystem_AddStreamDataFromPath(akbpath);
    }

    public override Int32 SdSoundSystem_RemoveData(Int32 bankID)
    {
        return SdLibAPI.SdSoundSystem_RemoveData(bankID);
    }

    public override Int32 SdSoundSystem_CreateSound(Int32 bankID)
    {
        return SdLibAPI.SdSoundSystem_CreateSound(bankID, 0);
    }

    public override Int32 SdSoundSystem_SoundCtrl_Start(Int32 soundID, Int32 offsetTimeMSec)
    {
        return SdLibAPI.SdSoundSystem_SoundCtrl_Start(soundID, offsetTimeMSec);
    }

    public override void SdSoundSystem_SoundCtrl_Stop(Int32 soundID, Int32 transTimeMSec)
    {
        SdLibAPI.SdSoundSystem_SoundCtrl_Stop(soundID, transTimeMSec);
    }

    public override Int32 SdSoundSystem_SoundCtrl_IsExist(Int32 soundID)
    {
        return SdLibAPI.SdSoundSystem_SoundCtrl_IsExist(soundID);
    }

    public override void SdSoundSystem_SoundCtrl_SetPause(Int32 soundID, Int32 pauseOn, Int32 transTimeMSec)
    {
        SdLibAPI.SdSoundSystem_SoundCtrl_SetPause(soundID, pauseOn, transTimeMSec);
    }

    public override Int32 SdSoundSystem_SoundCtrl_IsPaused(Int32 soundID)
    {
        return SdLibAPI.SdSoundSystem_SoundCtrl_IsPaused(soundID);
    }

    public override void SdSoundSystem_SoundCtrl_SetVolume(Int32 soundID, Single volume, Int32 transTimeMSec)
    {
        SdLibAPI.SdSoundSystem_SoundCtrl_SetVolume(soundID, volume, transTimeMSec);
    }

    public override Single SdSoundSystem_SoundCtrl_GetVolume(Int32 soundID)
    {
        return SdLibAPI.SdSoundSystem_SoundCtrl_GetVolume(soundID);
    }

    public override void SdSoundSystem_SoundCtrl_SetPitch(Int32 soundID, Single pitch, Int32 transTimeMSec)
    {
        SdLibAPI.SdSoundSystem_SoundCtrl_SetPitch(soundID, pitch, transTimeMSec);
    }

    public override void SdSoundSystem_SoundCtrl_SetPanning(Int32 soundID, Single panning, Int32 transTimeMSec)
    {
        SdLibAPI.SdSoundSystem_SoundCtrl_SetPanning(soundID, panning, transTimeMSec);
    }

    public override void SdSoundSystem_SoundCtrl_SetNextLoopRegion(Int32 soundID)
    {
        SdLibAPI.SdSoundSystem_SoundCtrl_SetNextLoopRegion(soundID);
    }

    public override Int32 SdSoundSystem_BankCtrl_IsLoop(Int32 bankID, Int32 soundID)
    {
        return SdLibAPI.SdSoundSystem_BankCtrl_IsLoop(bankID, soundID);
    }
}

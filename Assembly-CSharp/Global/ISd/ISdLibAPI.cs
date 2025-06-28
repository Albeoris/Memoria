using System;

public class ISdLibAPI
{
    public virtual Int32 LastSoundID { get; protected set; }

    public virtual Int32 SdSoundSystem_Create(String config)
    {
        SoundLib.Log("No Implementation");
        return 0;
    }

    public virtual void SdSoundSystem_Release()
    {
        SoundLib.Log("No Implementation");
    }

    public virtual Int32 SdSoundSystem_Suspend()
    {
        SoundLib.Log("No Implementation");
        return 0;
    }

    public virtual Int32 SdSoundSystem_Resume()
    {
        SoundLib.Log("No Implementation");
        return 0;
    }

    public virtual Int32 SdSoundSystem_AddData(IntPtr akb, SoundProfile profile)
    {
        SoundLib.Log("No Implementation");
        return 0;
    }

    public virtual Int32 SdSoundSystem_AddStreamDataFromPath(String akbpath)
    {
        SoundLib.Log("No Implementation");
        return 0;
    }

    public virtual Int32 SdSoundSystem_RemoveData(Int32 bankID)
    {
        SoundLib.Log("No Implementation");
        return 0;
    }

    public virtual Int32 SdSoundSystem_CreateSound(Int32 bankID)
    {
        SoundLib.Log("No Implementation");
        return 0;
    }

    public virtual Int32 SdSoundSystem_SoundCtrl_GetElapsedPlaybackTime(Int32 soundID)
    {
        SoundLib.Log("No Implementation");
        return 0;
    }

    public virtual Int32 SdSoundSystem_SoundCtrl_GetPlayTime(Int32 soundID)
    {
        SoundLib.Log("No Implementation");
        return 0;
    }

    public virtual Int32 SdSoundSystem_SoundCtrl_Start(Int32 soundID, Int32 offsetTimeMSec)
    {
        SoundLib.Log("No Implementation");
        return 0;
    }

    public virtual void SdSoundSystem_SoundCtrl_Stop(Int32 soundID, Int32 transTimeMSec)
    {
        SoundLib.Log("No Implementation");
    }

    public virtual Int32 SdSoundSystem_SoundCtrl_IsExist(Int32 soundID)
    {
        SoundLib.Log("No Implementation");
        return 0;
    }

    public virtual void SdSoundSystem_SoundCtrl_SetPause(Int32 soundID, Int32 pauseOn, Int32 transTimeMSec)
    {
        SoundLib.Log("No Implementation");
    }

    public virtual Int32 SdSoundSystem_SoundCtrl_IsPaused(Int32 soundID)
    {
        SoundLib.Log("No Implementation");
        return 0;
    }

    public virtual void SdSoundSystem_SoundCtrl_SetVolume(Int32 soundID, Single volume, Int32 transTimeMSec)
    {
        SoundLib.Log("No Implementation");
    }

    public virtual Single SdSoundSystem_SoundCtrl_GetVolume(Int32 soundID)
    {
        SoundLib.Log("No Implementation");
        return 0f;
    }

    public virtual void SdSoundSystem_SoundCtrl_SetPitch(Int32 soundID, Single pitch, Int32 transTimeMSec)
    {
        SoundLib.Log("No Implementation");
    }

    public virtual void SdSoundSystem_SoundCtrl_SetPanning(Int32 soundID, Single panning, Int32 transTimeMSec)
    {
        SoundLib.Log("No Implementation");
    }

    public virtual void SdSoundSystem_SoundCtrl_SetNextLoopRegion(Int32 soundID)
    {
        SoundLib.Log("No Implementation");
    }

    public virtual Int32 SdSoundSystem_BankCtrl_IsLoop(Int32 bankID, Int32 soundID)
    {
        SoundLib.Log("No Implementation");
        return 0;
    }
}

using System;
using UnityEngine;

public class SoundLibInitializer : MonoBehaviour
{
    private void Start()
    {
        SoundLib.Log("SoundLibInitializer.Start()");
        SoundLoaderProxy.Instance.Initial();
    }

    private void OnDestroy()
    {
        SoundLib.Log("Unload all sound resources");
        SoundLib.UnloadAllSoundEffect();
        SoundLib.UnloadMusic();
        SoundLib.UnloadAllResidentSfxSoundData();
    }
}

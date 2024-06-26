using System;

public class SongPlayer : SoundEffectPlayer
{
    public void PlaySong(Int32 soundIndex, Single soundVolume = 1f, Single panning = 0f, Single pitch = 1f)
    {
        base.PlaySoundEffect(soundIndex, soundVolume, panning, pitch, SoundProfileType.Song);
    }

    public void StopSong(Int32 soundIndex)
    {
        base.StopSoundEffect(soundIndex);
    }

    public void StopAllSongs()
    {
        base.StopAllSoundEffects();
    }
}

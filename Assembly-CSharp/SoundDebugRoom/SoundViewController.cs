using System;
using System.Collections.Generic;

namespace SoundDebugRoom
{
    public class SoundViewController
    {
        public SoundViewController(SoundView soundView)
        {
            this.soundView = soundView;
            this.ParseSoundList();
            this.SetActiveSoundType(SoundProfileType.Sfx);
        }

        private void ParseSoundList()
        {
            this.GenerateAllSongIndex();
            this.currentPlaylist = 0;
            this.GeneratePlaylistData(this.currentPlaylist);
        }

        public void SetActiveSoundType(SoundProfileType type)
        {
            if (this.activeSound != null)
            {
                SoundLib.StopMusic();
                this.IsPlay = false;
            }
            this.activeType = type;
            this.currentPlaylist = 0;
            this.GeneratePlaylistData(this.currentPlaylist);
            if (this.playlist != null)
            {
                if (this.playlist.Count > 0)
                {
                    this.activeSound = this.playlist[0];
                }
            }
            else
            {
                SoundLib.LogWarning("SetActiveSoundType, playlist is null!");
            }
            if (type == SoundProfileType.Sfx)
            {
            }
        }

        public SoundProfileType GetActiveSoundType()
        {
            return this.activeType;
        }

        public SoundProfile GetActiveSound()
        {
            return this.activeSound;
        }

        public List<SoundProfile> GetPlaylist()
        {
            return this.playlist;
        }

        public void SelectSound(SoundProfile sound)
        {
            this.activeSound = sound;
            this.PlayActiveSound();
        }

        public void PlayActiveSound()
        {
            if (this.activeSound.SoundProfileType == SoundProfileType.SoundEffect)
            {
                this.IsPlay = false;
                SoundLib.PlaySoundEffect(this.activeSound.SoundIndex, this.soundView.SoundVolume, this.soundView.PanningPosition, this.soundView.PitchPosition);
            }
            else if (this.activeSound.SoundProfileType == SoundProfileType.Song)
            {
                this.IsPlay = false;
                SoundLib.PlaySong(this.activeSound.SoundIndex, this.soundView.SoundVolume, this.soundView.PanningPosition, this.soundView.PitchPosition);
            }
            else if (this.activeSound.SoundProfileType == SoundProfileType.Music)
            {
                this.IsPlay = true;
                SoundLib.PlayMusic(this.activeSound.SoundIndex);
            }
            else if (this.activeSound.SoundProfileType == SoundProfileType.Sfx)
            {
                this.IsPlay = false;
                SoundLib.PlaySfxSound(this.activeSound.SoundIndex, this.soundView.SoundVolume, this.soundView.PanningPosition, this.soundView.PitchPosition);
            }
            else if (this.activeSound.SoundProfileType == SoundProfileType.MovieAudio)
            {
                this.IsPlay = true;
                String[] array = this.activeSound.Name.Split(new Char[]
                {
                    '/'
                });
                SoundLib.PlayMovieMusic(array[(Int32)array.Length - 1], 0);
            }
            else
            {
                this.IsPlay = false;
            }
        }

        public void PauseActiveSound()
        {
            SoundLib.PauseMusic();
            this.IsPlay = false;
        }

        public void SetMusicVolume(Single volume)
        {
            if (this.activeSound.SoundProfileType == SoundProfileType.Music)
            {
                SoundLib.SetMusicVolume(volume);
            }
        }

        public void SetMusicPanning(Single panning)
        {
            if (this.activeSound.SoundProfileType == SoundProfileType.Music)
            {
                SoundLib.SetMusicPanning(panning);
            }
        }

        public void SetMusicPitch(Single pitch)
        {
            if (this.activeSound.SoundProfileType == SoundProfileType.Music)
            {
                SoundLib.SetMusicPitch(pitch);
            }
            else if (this.activeSound.SoundProfileType == SoundProfileType.MovieAudio)
            {
                SoundLib.MovieAudioPlayer.SetMusicPitch(pitch);
            }
        }

        public void SeekMusic(Single position)
        {
            SoundLib.SeekMusic(position);
        }

        public void StopActiveSound()
        {
            if (this.activeSound.SoundProfileType == SoundProfileType.Music)
            {
                SoundLib.StopMusic();
                this.IsPlay = false;
            }
            else if (this.activeSound.SoundProfileType == SoundProfileType.SoundEffect)
            {
                SoundLib.StopAllSoundEffects();
            }
            else if (this.activeSound.SoundProfileType == SoundProfileType.Song)
            {
                SoundLib.StopAllSongs();
            }
            else if (this.activeSound.SoundProfileType == SoundProfileType.Sfx)
            {
                SoundLib.StopSfxSound(this.activeSound.SoundIndex);
            }
            else if (this.activeSound.SoundProfileType == SoundProfileType.MovieAudio)
            {
                SoundLib.StopMovieMusic(this.activeSound.Name, true);
            }
        }

        public void NextSound()
        {
            Boolean flag = true;
            this.IsPlay = false;
            List<SoundProfile> list = this.playlist;
            Int32 count = list.Count;
            Int32 num = 0;
            Int32 num2 = Int32.MaxValue;
            foreach (SoundProfile soundProfile in list)
            {
                if (num2 == num)
                {
                    this.activeSound = soundProfile;
                    break;
                }
                if (soundProfile.Code == this.activeSound.Code && num < count - 1)
                {
                    num2 = num + 1;
                }
                num++;
            }
            if (flag)
            {
                this.PlayActiveSound();
            }
        }

        public void PreviousSound()
        {
            Boolean isPlay = this.IsPlay;
            this.IsPlay = false;
            List<SoundProfile> list = this.playlist;
            Int32 count = list.Count;
            Int32 num = 0;
            SoundProfile soundProfile = (SoundProfile)null;
            foreach (SoundProfile soundProfile2 in list)
            {
                if (soundProfile2.Code == this.activeSound.Code && num != 0)
                {
                    this.activeSound = soundProfile;
                    break;
                }
                soundProfile = soundProfile2;
                num++;
            }
            if (isPlay)
            {
                this.PlayActiveSound();
            }
        }

        public void ReloadSoundMetaData()
        {
            SoundMetaData.LoadMetaData();
            this.ParseSoundList();
            SoundLib.UnloadSoundEffect();
            SoundLib.LoadGameSoundEffect(SoundMetaData.SoundEffectMetaData);
            SoundLib.UnloadMusic();
            SoundLib.LoadMusic(SoundMetaData.MusicMetaData);
        }

        public void NextPlayList()
        {
            List<Int32> list = null;
            if (this.activeType == SoundProfileType.Music)
            {
                list = this.allMusicIndex;
            }
            else if (this.activeType == SoundProfileType.SoundEffect)
            {
                list = this.allSoundEffectIndex;
            }
            else if (this.activeType == SoundProfileType.Song)
            {
                list = this.allSongIndex;
            }
            else if (this.activeType == SoundProfileType.Sfx)
            {
                SoundLib.LogWarning("Does not support SoundProfileType.Sfx");
            }
            else if (this.activeType == SoundProfileType.MovieAudio)
            {
                list = this.allMovieAudioIndex;
            }
            Int32 num = (Int32)((list.Count % 100 != 0) ? 1 : 0);
            Int32 num2 = list.Count / 100 + num;
            if (this.currentPlaylist < num2 - 1)
            {
                this.currentPlaylist++;
            }
            this.GeneratePlaylistData(this.currentPlaylist);
            if (this.playlist.Count > 0)
            {
                this.activeSound = this.playlist[0];
            }
        }

        public void PreviousPlayList()
        {
            if (this.currentPlaylist > 0)
            {
                this.currentPlaylist--;
            }
            this.GeneratePlaylistData(this.currentPlaylist);
            if (this.playlist.Count > 0)
            {
                this.activeSound = this.playlist[0];
            }
        }

        private void GenerateAllSongIndex()
        {
            this.allSoundEffectIndex = new List<Int32>();
            if (SoundMetaData.SoundEffectIndex != null)
            {
                foreach (KeyValuePair<Int32, String> keyValuePair in SoundMetaData.SoundEffectIndex)
                {
                    this.allSoundEffectIndex.Add(keyValuePair.Key);
                }
            }
            this.allMusicIndex = new List<Int32>();
            if (SoundMetaData.MusicIndex != null)
            {
                foreach (KeyValuePair<Int32, String> keyValuePair2 in SoundMetaData.MusicIndex)
                {
                    this.allMusicIndex.Add(keyValuePair2.Key);
                }
            }
            this.allMovieAudioIndex = new List<Int32>();
            if (SoundMetaData.MovieAudioIndex != null)
            {
                foreach (KeyValuePair<Int32, String> keyValuePair3 in SoundMetaData.MovieAudioIndex)
                {
                    this.allMovieAudioIndex.Add(keyValuePair3.Key);
                }
            }
            this.allSongIndex = new List<Int32>();
            if (SoundMetaData.SongIndex != null)
            {
                foreach (KeyValuePair<Int32, String> keyValuePair4 in SoundMetaData.SongIndex)
                {
                    this.allSongIndex.Add(keyValuePair4.Key);
                }
            }
            this.AllSfxGroupSongIndex = new List<Int32>();
            if (SoundMetaData.SfxSoundIndex != null)
            {
                foreach (Int32 item in SoundMetaData.SfxSoundIndex.Keys)
                {
                    this.AllSfxGroupSongIndex.Add(item);
                }
            }
        }

        private void GeneratePlaylistData(Int32 playlistIndex)
        {
            this.playlist = this.GetPlaylist(this.currentPlaylist);
            if (this.playlist == null)
            {
                SoundLib.LogWarning("GeneratePlaylistData, playlist is null!");
                return;
            }
            this.SetPlaylistInfo(this.playlist);
        }

        private List<SoundProfile> GetPlaylist(Int32 playListIndex)
        {
            List<Int32> list = null;
            if (this.activeType == SoundProfileType.Music)
            {
                list = this.allMusicIndex;
            }
            else if (this.activeType == SoundProfileType.SoundEffect)
            {
                list = this.allSoundEffectIndex;
            }
            else if (this.activeType == SoundProfileType.Song)
            {
                list = this.allSongIndex;
            }
            else if (this.activeType == SoundProfileType.Sfx)
            {
                SoundLib.LogWarning("GetPlaylist does not support SoundProfileType.Sfx");
            }
            else if (this.activeType == SoundProfileType.MovieAudio)
            {
                list = this.allMovieAudioIndex;
            }
            if (list == null)
            {
                return null;
            }
            if (this.currentPlaylist < 0)
            {
                return null;
            }
            if (this.currentPlaylist > list.Count)
            {
                return null;
            }
            Int32 num = this.currentPlaylist * 100;
            List<SoundProfile> list2 = new List<SoundProfile>();
            Int32 num2 = num;
            while (num2 < num + 100 && num2 < list.Count)
            {
                list2.Add(SoundMetaData.GetSoundProfile(list[num2], this.activeType));
                num2++;
            }
            return list2;
        }

        private void SetPlaylistInfo(List<SoundProfile> allSoundProfile)
        {
            List<Int32> list = null;
            if (this.activeType == SoundProfileType.Music)
            {
                list = this.allMusicIndex;
            }
            else if (this.activeType == SoundProfileType.SoundEffect)
            {
                list = this.allSoundEffectIndex;
            }
            else if (this.activeType == SoundProfileType.Song)
            {
                list = this.allSongIndex;
            }
            else if (this.activeType == SoundProfileType.Sfx)
            {
                SoundLib.LogWarning("SetPlaylistInfo does not support SoundProfileType.Sfx");
            }
            else if (this.activeType == SoundProfileType.MovieAudio)
            {
                list = this.allMovieAudioIndex;
            }
            this.PlaylistInfo = this.currentPlaylist + 1 + "/" + (list.Count / 100 + 1);
            Int32 num = this.currentPlaylist * 100;
            this.PlaylistDetail = String.Concat(new Object[]
            {
                num + 1,
                "-",
                num + allSoundProfile.Count,
                " of ",
                list.Count
            });
        }

        public List<String> GetSfxSoundPlaylist(Int32 specialEffectID)
        {
            if (this.currentSpecialEffectID != specialEffectID)
            {
                List<String> list = new List<String>();
                foreach (String item in SoundMetaData.ResidentSfxSoundIndex[0])
                {
                    list.Add(item);
                }
                foreach (String item2 in SoundMetaData.SfxSoundIndex[specialEffectID])
                {
                    list.Add(item2);
                }
                this.allSpecialEffectPath = list;
            }
            this.currentSpecialEffectID = specialEffectID;
            return this.allSpecialEffectPath;
        }

        public void LoadSfxSoundGroup(Int32 specialEffectID)
        {
            DateTime now = DateTime.Now;
            SoundLib.LoadSfxSoundData(specialEffectID);
            DateTime now2 = DateTime.Now;
            SoundLib.Log("Sound effect Loading time: " + (now2 - now).Milliseconds);
        }

        public void SelectSound(Int32 soundIndexInSpecialEffect)
        {
            SoundProfile soundProfile = SoundLib.PlaySfxSound(soundIndexInSpecialEffect, this.soundView.SoundVolume, this.soundView.PanningPosition, this.soundView.PitchPosition);
            this.activeSound = soundProfile;
        }

        private const Int32 playlistSoundCount = 100;

        private SoundView soundView;

        public Boolean IsPlay;

        private SoundProfileType activeType = SoundProfileType.SoundEffect;

        private SoundProfile activeSound;

        public String PlaylistInfo = String.Empty;

        public String PlaylistDetail = String.Empty;

        private List<SoundProfile> playlist = new List<SoundProfile>();

        private Int32 currentPlaylist;

        private List<Int32> allSoundEffectIndex;

        private List<Int32> allSongIndex;

        private List<Int32> allMusicIndex;

        private List<Int32> allMovieAudioIndex;

        public List<Int32> AllSfxGroupSongIndex;

        private Int32 currentSpecialEffectID = -1;

        private List<String> allSpecialEffectPath;
    }
}

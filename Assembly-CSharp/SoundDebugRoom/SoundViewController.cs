using Global.Sound.SaXAudio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
            LoadVoiceModList();
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
            if (IsPlay) StopActiveSound();
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
                this.activeSound = SoundLib.PlaySfxSound(this.activeSound.SoundIndex, this.soundView.SoundVolume, this.soundView.PanningPosition, this.soundView.PitchPosition);
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
            else if (this.activeSound.SoundProfileType == SoundProfileType.Voice)
            {
                this.IsPlay = false;
                VoicePlayer.CreateLoadThenPlayVoice(activeSound.SoundIndex, activeSound.ResourceID);
            }
            else
            {
                this.IsPlay = false;
                return;
            }
            if (IsSaXAudio)
            {
                this.activeSound.SoundID = SaXAudioApi.LastSoundID;
                if (IsReverbEnabled) SetReverb(ref Reverb);
                if (IsEqEnabled) SetEq(ref Eq);
                if (IsEchoEnabled) SetEcho(ref Echo);
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

        public void SetReverb(ref SaXAudio.ReverbParameters parameters)
        {
            SaXAudio.SetReverb(activeSound.SoundID, parameters, 0, false);
        }
        public void RemoveReverb()
        {
            SaXAudio.RemoveReverb(activeSound.SoundID, 0, false);
        }

        public void SetEq(ref SaXAudio.EqParameters parameters)
        {
            SaXAudio.SetEq(activeSound.SoundID, parameters, 0, false);
        }
        public void RemoveEq()
        {
            SaXAudio.RemoveEq(activeSound.SoundID, 0, false);
        }
        public void SetEcho(ref SaXAudio.EchoParameters parameters)
        {
            SaXAudio.SetEcho(activeSound.SoundID, parameters, 0, false);
        }
        public void RemoveEcho()
        {
            SaXAudio.RemoveEcho(activeSound.SoundID, 0, false);
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
            this.IsPlay = false;
            List<SoundProfile> list = this.playlist;
            for (Int32 i = 0; i < list.Count - 1; i++)
            {
                if (list[i].Code == this.activeSound.Code)
                {
                    this.activeSound = list[i + 1];
                    this.PlayActiveSound();
                    return;
                }
            }
        }

        public void PreviousSound()
        {
            List<SoundProfile> list = this.playlist;
            this.IsPlay = false;
            for (Int32 i = 1; i < list?.Count; i++)
            {
                if (list[i].Code == this.activeSound.Code)
                {
                    this.activeSound = list[i - 1];
                    this.PlayActiveSound();
                    return;
                }
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

        private void LoadVoiceModList()
        {
            foreach (var folder in AssetManager.FolderHighToLow)
            {
                String path = Path.Combine(folder.FolderPath, @"StreamingAssets\Assets\Resources\Sounds\Voices");
                if (!Directory.Exists(path)) continue;

                HashSet<String> voicesSet = new HashSet<String>();

                DirectoryInfo dir = new DirectoryInfo(path);
                var files = dir.GetFiles("*.akb.bytes", SearchOption.AllDirectories);
                foreach (FileInfo file in files)
                {
                    String resourceID = Regex.Replace(file.FullName, @".*?StreamingAssets\\Assets\\Resources\\Sounds\\", "");
                    resourceID = Regex.Replace(resourceID, @"\.akb\.bytes", "");
                    voicesSet.Add(resourceID);
                }
                files = dir.GetFiles("*.ogg", SearchOption.AllDirectories);
                foreach (FileInfo file in files)
                {
                    String resourceID = Regex.Replace(file.FullName, @".*?StreamingAssets\\Assets\\Resources\\Sounds\\", "");
                    resourceID = Regex.Replace(resourceID, @"\.ogg", "");
                    voicesSet.Add(resourceID);
                }

                List<SoundProfile> soundProfiles = new List<SoundProfile>();
                foreach (String resourceID in voicesSet)
                {
                    Int32 soundIndex = resourceID.GetHashCode();
                    soundProfiles.Add(new SoundProfile()
                    {
                        Code = soundIndex.ToString(),
                        Name = resourceID,
                        SoundIndex = soundIndex,
                        ResourceID = resourceID,
                        SoundProfileType = SoundProfileType.Voice,
                        SoundVolume = 1f,
                        Panning = 0f,
                        Pitch = 1f
                    });
                }

                if (soundProfiles.Count > 0)
                {
                    soundProfiles.Sort((a, b) => ComparePaths(a.ResourceID, b.ResourceID));
                    ModVoiceDictionary.Add(folder.FolderPath, soundProfiles);
                }
            }
        }
        private int ComparePaths(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            // Split strings into parts (text and numbers)
            var xParts = SplitIntoNaturalParts(x);
            var yParts = SplitIntoNaturalParts(y);

            int minLength = Math.Min(xParts.Count, yParts.Count);

            for (int i = 0; i < minLength; i++)
            {
                var xPart = xParts[i];
                var yPart = yParts[i];

                // Check if both parts are numeric
                bool xIsNumeric = long.TryParse(xPart, out long xNum);
                bool yIsNumeric = long.TryParse(yPart, out long yNum);

                if (xIsNumeric && yIsNumeric)
                {
                    // Compare as numbers
                    int numComparison = xNum.CompareTo(yNum);
                    if (numComparison != 0)
                        return numComparison;
                }
                else
                {
                    // Compare as strings
                    int strComparison = string.Compare(xPart, yPart, StringComparison.OrdinalIgnoreCase);
                    if (strComparison != 0)
                        return strComparison;
                }
            }

            // If all compared parts are equal, the shorter string comes first
            return xParts.Count.CompareTo(yParts.Count);
        }
        private List<string> SplitIntoNaturalParts(string input)
        {
            // Split on transitions between digits and non-digits
            var parts = new List<string>();
            var regex = new Regex(@"(\d+|\D+)");
            var matches = regex.Matches(input);

            foreach (Match match in matches)
            {
                parts.Add(match.Value);
            }

            return parts;
        }

        public void GenerateVoiceList(String modLocation)
        {
            currentVoiceMod = modLocation;
            currentPlaylist = 0;
            GeneratePlaylistData(currentPlaylist);
        }

        public void FilterVoiceList(String filter)
        {
            voiceFilter = filter;
            currentPlaylist = 0;
            GeneratePlaylistData(currentPlaylist);
        }

        public void NextPlayList()
        {
            Int32 count = 0;
            if (this.activeType == SoundProfileType.Music)
            {
                count = this.allMusicIndex.Count;
            }
            else if (this.activeType == SoundProfileType.SoundEffect)
            {
                count = this.allSoundEffectIndex.Count;
            }
            else if (this.activeType == SoundProfileType.Song)
            {
                count = this.allSongIndex.Count;
            }
            else if (this.activeType == SoundProfileType.Sfx)
            {
                SoundLib.LogWarning("Does not support SoundProfileType.Sfx");
            }
            else if (this.activeType == SoundProfileType.MovieAudio)
            {
                count = this.allMovieAudioIndex.Count;
            }
            else if (this.activeType == SoundProfileType.Voice)
            {
                count = ModVoiceDictionary.Count;
            }
            Int32 num = (Int32)((count % 100 != 0) ? 1 : 0);
            Int32 num2 = count / 100 + num;
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
            else if (this.activeType == SoundProfileType.Voice && ModVoiceDictionary.ContainsKey(currentVoiceMod))
            {
                List<SoundProfile> voicelist = ModVoiceDictionary[currentVoiceMod];
                if (voiceFilter.Length > 0)
                {
                    voicelist = voicelist.Where((p) => p.ResourceID.Contains(voiceFilter)).ToList();
                }

                Int32 index = Math.Min(currentPlaylist * 100, voicelist.Count - 1);
                Int32 count = (100 + index) > voicelist.Count ? voicelist.Count - index : 100;
                return voicelist.GetRange(index, count);
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
            Int32 count = 0;
            if (this.activeType == SoundProfileType.Music)
            {
                count = this.allMusicIndex.Count;
            }
            else if (this.activeType == SoundProfileType.SoundEffect)
            {
                count = this.allSoundEffectIndex.Count;
            }
            else if (this.activeType == SoundProfileType.Song)
            {
                count = this.allSongIndex.Count;
            }
            else if (this.activeType == SoundProfileType.Sfx)
            {
                SoundLib.LogWarning("SetPlaylistInfo does not support SoundProfileType.Sfx");
            }
            else if (this.activeType == SoundProfileType.MovieAudio)
            {
                count = this.allMovieAudioIndex.Count;
            }
            else if (activeType == SoundProfileType.Voice)
            {
                count = ModVoiceDictionary[currentVoiceMod].Where((p) => p.ResourceID.Contains(voiceFilter)).Count();
            }
            this.PlaylistInfo = this.currentPlaylist + 1 + "/" + (count / 100 + 1);
            Int32 num = this.currentPlaylist * 100;
            this.PlaylistDetail = String.Concat(new Object[]
            {
                num + 1,
                "-",
                num + allSoundProfile.Count,
                " of ",
                count
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

        public Boolean IsSaXAudio = ISdLibAPIProxy.Instance is SdLibAPIWithSaXAudio;

        SdLibAPIWithSaXAudio SaXAudioApi = ISdLibAPIProxy.Instance as SdLibAPIWithSaXAudio;

        public SaXAudio.EqParameters Eq = new SaXAudio.EqParameters();

        public SaXAudio.ReverbParameters Reverb = new SaXAudio.ReverbParameters();

        public SaXAudio.EchoParameters Echo = new SaXAudio.EchoParameters();

        public Boolean IsReverbEnabled = false;

        public Boolean IsEqEnabled = false;

        public Boolean IsEchoEnabled = false;

        public Dictionary<String, List<SoundProfile>> ModVoiceDictionary = new Dictionary<String, List<SoundProfile>>();

        private String currentVoiceMod = "";

        private String voiceFilter = "";

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

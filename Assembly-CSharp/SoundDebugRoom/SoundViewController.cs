using Global.Sound.SaXAudio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using static Global.Sound.SaXAudio.AudioEffectManager;

namespace SoundDebugRoom
{
    public class SoundViewController
    {
        public SoundViewController(SoundView view)
        {
            soundView = view;
            ParseSoundList();
            SetActiveSoundType(SoundProfileType.Sfx);
            if (IsSaXAudio) SaXAudio.OnVoiceFinished += OnFinished;
        }
        ~SoundViewController()
        {
            if (IsSaXAudio) SaXAudio.OnVoiceFinished -= OnFinished;
        }

        private void OnFinished(Int32 soundId)
        {
            IsPlay = false;
        }

        private void ParseSoundList()
        {
            GenerateAllSongIndex();
            currentPlaylist = 0;
            GeneratePlaylistData(currentPlaylist);
            LoadVoiceModList();
        }

        public void SetActiveSoundType(SoundProfileType type)
        {
            activeType = type;
            currentPlaylist = 0;
            GeneratePlaylistData(currentPlaylist);
        }

        public SoundProfileType GetActiveSoundType()
        {
            return activeType;
        }

        public SoundProfile GetActiveSound()
        {
            return activeSound;
        }

        public List<SoundProfile> GetPlaylist()
        {
            return playlist;
        }

        public void SelectSound(SoundProfile sound)
        {
            StopActiveSound();
            activeSound = sound;
            activeSound.SoundID = -1;
            PlayActiveSound();
        }

        public void PlayActiveSound()
        {
            if (activeSound == null) return;

            if (activeSound.SoundID != -1 && ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_IsPaused(activeSound.SoundID) > 0)
            {
                IsPlay = true;
                ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(activeSound.SoundID, 0, 0);
                return;
            }

            if (activeSound.SoundProfileType == SoundProfileType.SoundEffect)
            {
                SoundLib.PlaySoundEffect(activeSound.SoundIndex, soundView.SoundVolume, soundView.PanningPosition, soundView.PitchPosition);
            }
            else if (activeSound.SoundProfileType == SoundProfileType.Song)
            {
                SoundLib.PlaySong(activeSound.SoundIndex, soundView.SoundVolume, soundView.PanningPosition, soundView.PitchPosition);
            }
            else if (activeSound.SoundProfileType == SoundProfileType.Music)
            {
                SoundLib.StopMusic();
                SoundLib.PlayMusic(activeSound.SoundIndex);
            }
            else if (activeSound.SoundProfileType == SoundProfileType.Sfx)
            {
                activeSound = SoundLib.PlaySfxSound(activeSound.SoundIndex, soundView.SoundVolume, soundView.PanningPosition, soundView.PitchPosition);
            }
            else if (activeSound.SoundProfileType == SoundProfileType.MovieAudio)
            {
                String[] array = activeSound.Name.Split(new Char[]
                {
                    '/'
                });
                SoundLib.PlayMovieMusic(array[(Int32)array.Length - 1], 0);
            }
            else if (activeSound.SoundProfileType == SoundProfileType.Voice)
            {
                VoicePlayer.CreateLoadThenPlayVoice(activeSound.SoundIndex, activeSound.ResourceID);
            }
            else
            {
                activeSound = null;
                IsPlay = false;
                return;
            }

            IsPlay = true;
            activeSound.SoundID = ISdLibAPIProxy.Instance.LastSoundID;
            SetVolume(soundView.SoundVolume);
            SetPanning(soundView.PanningPosition);
            SetPitch(soundView.PitchPosition);

            if (IsSaXAudio)
            {
                if (IsReverbEnabled) SetReverb(ref Reverb);
                if (IsEqEnabled) SetEq(ref Eq);
                if (IsEchoEnabled) SetEcho(ref Echo);
                IsLooping = SaXAudio.GetLooping(activeSound.SoundID);
            }
        }

        public void PauseActiveSound()
        {
            if (activeSound == null) return;
            ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPause(activeSound.SoundID, 1, 0);
            IsPlay = false;
        }

        public Single GetActivePosition()
        {
            if (activeSound == null || !IsSaXAudio) return 0;
            return SaXAudio.GetPositionTime(activeSound.SoundID);
        }

        public Single GetActiveLength()
        {
            if (activeSound == null || !IsSaXAudio) return 0.0001f;
            return Math.Max(0.0001f, SaXAudio.GetTotalTime(activeSound.SoundID));
        }

        public void SeekActive(Single position)
        {
            if (activeSound == null || !IsSaXAudio || !IsPlay) return;
            SaXAudio.SetVolume(activeSound.SoundID, soundView.SoundVolume * 0.25f, 0f);
            SaXAudio.StartAtTime(activeSound.SoundID, position);
            SaXAudio.SetVolume(activeSound.SoundID, soundView.SoundVolume);
        }

        public void SetVolume(Single volume)
        {
            if (activeSound == null) return;
            ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetVolume(activeSound.SoundID, volume, 0);
        }

        public void SetPanning(Single panning)
        {
            if (activeSound == null) return;
            ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPanning(activeSound.SoundID, panning, 0);
        }

        public void SetPitch(Single pitch)
        {
            if (activeSound == null) return;
            ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_SetPitch(activeSound.SoundID, pitch, 0);
        }

        public void SetLooping(Boolean looping)
        {
            if (!IsPlay) PlayActiveSound();
            if (IsLooping == looping || activeSound == null) return;

            SaXAudio.SetLooping(activeSound.SoundID, looping);
            IsLooping = SaXAudio.GetLooping(activeSound.SoundID);
        }

        public void SetReverb(ref SaXAudio.ReverbParameters parameters)
        {
            if (activeSound != null)
                SaXAudio.SetReverb(activeSound.SoundID, parameters, 0, false);
        }
        public void RemoveReverb()
        {
            if (activeSound != null)
                SaXAudio.RemoveReverb(activeSound.SoundID, 0, false);
        }

        public void SetEq(ref SaXAudio.EqParameters parameters)
        {
            if (activeSound != null)
                SaXAudio.SetEq(activeSound.SoundID, parameters, 0, false);
        }
        public void RemoveEq()
        {
            if (activeSound != null)
                SaXAudio.RemoveEq(activeSound.SoundID, 0, false);
        }
        public void SetEcho(ref SaXAudio.EchoParameters parameters)
        {
            if (activeSound != null)
                SaXAudio.SetEcho(activeSound.SoundID, parameters, 0, false);
        }
        public void RemoveEcho()
        {
            if (activeSound != null)
                SaXAudio.RemoveEcho(activeSound.SoundID, 0, false);
        }

        public void StopActiveSound()
        {
            if (activeSound == null) return;
            ISdLibAPIProxy.Instance.SdSoundSystem_SoundCtrl_Stop(activeSound.SoundID, 0);
            IsLooping = false;
            IsPlay = false;
        }

        public void NextSound()
        {
            StopActiveSound();
            List<SoundProfile> list = playlist;
            for (Int32 i = 0; i < list.Count - 1; i++)
            {
                if (list[i].Code == activeSound.Code)
                {
                    activeSound = list[i + 1];
                    activeSound.SoundID = -1;
                    PlayActiveSound();
                    return;
                }
            }
        }

        public void PreviousSound()
        {
            StopActiveSound();
            List<SoundProfile> list = playlist;
            IsPlay = false;
            for (Int32 i = 1; i < list?.Count; i++)
            {
                if (list[i].Code == activeSound.Code)
                {
                    activeSound = list[i - 1];
                    activeSound.SoundID = -1;
                    PlayActiveSound();
                    return;
                }
            }
        }

        public void ReloadSoundMetaData()
        {
            LoadVoiceModList();
            SoundMetaData.LoadMetaData();
            ParseSoundList();
            SoundLib.UnloadSoundEffect();
            SoundLib.UnloadMusic();
            // This loads every sounds which is slow and very unnecessary
            /*SoundLib.LoadGameSoundEffect(SoundMetaData.SoundEffectMetaData);
            SoundLib.LoadMusic(SoundMetaData.MusicMetaData);*/
        }

        private void LoadVoiceModList()
        {
            ModVoiceDictionary.Clear();
            new Thread(() =>
            {
                foreach (var folder in AssetManager.FolderHighToLow)
                {
                    String path = Path.Combine(folder.FolderPath, @"StreamingAssets\Assets\Resources\Sounds\Voices");
                    if (!Directory.Exists(path)) continue;

                    HashSet<String> voicesSet = new HashSet<String>();

                    String[] files = Directory.GetFiles(path, "*.akb.bytes", SearchOption.AllDirectories);
                    foreach (String file in files)
                    {
                        String resourceID = Regex.Replace(file, @".*?StreamingAssets\\Assets\\Resources\\Sounds\\", "");
                        resourceID = Regex.Replace(resourceID, @"\.akb\.bytes", "");
                        voicesSet.Add(resourceID);
                    }
                    files = Directory.GetFiles(path, "*.ogg", SearchOption.AllDirectories);
                    foreach (String file in files)
                    {
                        String resourceID = Regex.Replace(file, @".*?StreamingAssets\\Assets\\Resources\\Sounds\\", "");
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
            }).Start();
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

        public void SaveEffectPresets(String modLocation, Boolean backup = false)
        {
            SavePresets(EffectPresetDictionary, modLocation, backup);
        }

        public void LoadEffectPresets(String modLocation, Boolean backup = false)
        {
            EffectPresetDictionary = LoadPresets(modLocation, backup);
        }

        public void SaveEffectPreset(String name, String modLocation)
        {
            if (String.IsNullOrEmpty(name)) return;
            try
            {
                EffectPreset preset = new EffectPreset();
                preset.Name = name;
                preset.Flags = (IsReverbEnabled ? EffectPreset.Flag.Reverb : 0) | (IsEqEnabled ? EffectPreset.Flag.Eq : 0) | (IsEchoEnabled ? EffectPreset.Flag.Echo : 0);
                preset.Reverb = Reverb;
                preset.Eq = Eq;
                preset.Echo = Echo;

                EffectPresetDictionary[name] = preset;
            }
            catch (Exception e)
            {
                SoundLib.LogError(e);
                return;
            }
            SavePresets(EffectPresetDictionary, modLocation);
        }

        public void DeleteEffectPreset(String name, String modLocation)
        {
            if (String.IsNullOrEmpty(name) || !EffectPresetDictionary.ContainsKey(name)) return;

            EffectPresetDictionary.Remove(name);
            SavePresets(EffectPresetDictionary, modLocation);
        }

        public void ApplyEffectPreset(String name)
        {
            if (!EffectPresetDictionary.ContainsKey(name)) return;

            EffectPreset preset = EffectPresetDictionary[name];
            IsReverbEnabled = (preset.Flags & EffectPreset.Flag.Reverb) != 0;
            IsEqEnabled = (preset.Flags & EffectPreset.Flag.Eq) != 0;
            IsEchoEnabled = (preset.Flags & EffectPreset.Flag.Echo) != 0;
            Reverb = preset.Reverb;
            Eq = preset.Eq;
            Echo = preset.Echo;
            if (IsReverbEnabled) SetReverb(ref Reverb);
            if (IsEqEnabled) SetEq(ref Eq);
            if (IsEchoEnabled) SetEcho(ref Echo);
        }

        public void NextPlayList()
        {
            Int32 count = 0;
            if (activeType == SoundProfileType.Music)
            {
                count = allMusicIndex.Count;
            }
            else if (activeType == SoundProfileType.SoundEffect)
            {
                count = allSoundEffectIndex.Count;
            }
            else if (activeType == SoundProfileType.Song)
            {
                count = allSongIndex.Count;
            }
            else if (activeType == SoundProfileType.Sfx)
            {
                SoundLib.LogWarning("Does not support SoundProfileType.Sfx");
            }
            else if (activeType == SoundProfileType.MovieAudio)
            {
                count = allMovieAudioIndex.Count;
            }
            else if (activeType == SoundProfileType.Voice)
            {
                count = ModVoiceDictionary.Count;
            }
            Int32 num = (Int32)((count % 100 != 0) ? 1 : 0);
            Int32 num2 = count / 100 + num;
            if (currentPlaylist < num2 - 1)
            {
                currentPlaylist++;
            }
            GeneratePlaylistData(currentPlaylist);
        }

        public void PreviousPlayList()
        {
            if (currentPlaylist > 0)
            {
                currentPlaylist--;
            }
            GeneratePlaylistData(currentPlaylist);
        }

        private void GenerateAllSongIndex()
        {
            allSoundEffectIndex = new List<Int32>();
            if (SoundMetaData.SoundEffectIndex != null)
            {
                foreach (KeyValuePair<Int32, String> keyValuePair in SoundMetaData.SoundEffectIndex)
                {
                    allSoundEffectIndex.Add(keyValuePair.Key);
                }
            }
            allMusicIndex = new List<Int32>();
            if (SoundMetaData.MusicIndex != null)
            {
                foreach (KeyValuePair<Int32, String> keyValuePair2 in SoundMetaData.MusicIndex)
                {
                    allMusicIndex.Add(keyValuePair2.Key);
                }
            }
            allMovieAudioIndex = new List<Int32>();
            if (SoundMetaData.MovieAudioIndex != null)
            {
                foreach (KeyValuePair<Int32, String> keyValuePair3 in SoundMetaData.MovieAudioIndex)
                {
                    allMovieAudioIndex.Add(keyValuePair3.Key);
                }
            }
            allSongIndex = new List<Int32>();
            if (SoundMetaData.SongIndex != null)
            {
                foreach (KeyValuePair<Int32, String> keyValuePair4 in SoundMetaData.SongIndex)
                {
                    allSongIndex.Add(keyValuePair4.Key);
                }
            }
            AllSfxGroupSongIndex = new List<Int32>();
            if (SoundMetaData.SfxSoundIndex != null)
            {
                foreach (Int32 item in SoundMetaData.SfxSoundIndex.Keys)
                {
                    AllSfxGroupSongIndex.Add(item);
                }
            }
        }

        private void GeneratePlaylistData(Int32 playlistIndex)
        {
            playlist = GetPlaylist(currentPlaylist);
            if (playlist == null)
            {
                SoundLib.LogWarning("GeneratePlaylistData, playlist is null!");
                return;
            }
            SetPlaylistInfo(playlist);
        }

        private List<SoundProfile> GetPlaylist(Int32 playListIndex)
        {
            List<Int32> list = null;
            if (activeType == SoundProfileType.Music)
            {
                list = allMusicIndex;
            }
            else if (activeType == SoundProfileType.SoundEffect)
            {
                list = allSoundEffectIndex;
            }
            else if (activeType == SoundProfileType.Song)
            {
                list = allSongIndex;
            }
            else if (activeType == SoundProfileType.Sfx)
            {
                SoundLib.LogWarning("GetPlaylist does not support SoundProfileType.Sfx");
            }
            else if (activeType == SoundProfileType.MovieAudio)
            {
                list = allMovieAudioIndex;
            }
            else if (activeType == SoundProfileType.Voice && ModVoiceDictionary.ContainsKey(currentVoiceMod))
            {
                List<SoundProfile> voiceList = ModVoiceDictionary[currentVoiceMod];
                if (voiceFilter.Length > 0)
                {
                    voiceList = voiceList.Where((p) => p.ResourceID.Contains(voiceFilter)).ToList();
                }

                Int32 index = Math.Min(currentPlaylist * 100, voiceList.Count - 1);
                Int32 count = (100 + index) > voiceList.Count ? voiceList.Count - index : 100;
                return voiceList.GetRange(index, count);
            }
            if (list == null)
            {
                return null;
            }
            if (currentPlaylist < 0)
            {
                return null;
            }
            if (currentPlaylist > list.Count)
            {
                return null;
            }
            Int32 num = currentPlaylist * 100;
            List<SoundProfile> list2 = new List<SoundProfile>();
            Int32 num2 = num;
            while (num2 < num + 100 && num2 < list.Count)
            {
                list2.Add(SoundMetaData.GetSoundProfile(list[num2], activeType));
                num2++;
            }
            return list2;
        }

        private void SetPlaylistInfo(List<SoundProfile> allSoundProfile)
        {
            Int32 count = 0;
            if (activeType == SoundProfileType.Music)
            {
                count = allMusicIndex.Count;
            }
            else if (activeType == SoundProfileType.SoundEffect)
            {
                count = allSoundEffectIndex.Count;
            }
            else if (activeType == SoundProfileType.Song)
            {
                count = allSongIndex.Count;
            }
            else if (activeType == SoundProfileType.Sfx)
            {
                SoundLib.LogWarning("SetPlaylistInfo does not support SoundProfileType.Sfx");
            }
            else if (activeType == SoundProfileType.MovieAudio)
            {
                count = allMovieAudioIndex.Count;
            }
            else if (activeType == SoundProfileType.Voice)
            {
                count = ModVoiceDictionary[currentVoiceMod].Where((p) => p.ResourceID.Contains(voiceFilter)).Count();
            }
            PlaylistInfo = currentPlaylist + 1 + "/" + (count / 100 + 1);
            Int32 num = currentPlaylist * 100;
            PlaylistDetail = String.Concat(new Object[]
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
            if (currentSpecialEffectID != specialEffectID)
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
                allSpecialEffectPath = list;
            }
            currentSpecialEffectID = specialEffectID;
            return allSpecialEffectPath;
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
            SoundProfile soundProfile = SoundLib.PlaySfxSound(soundIndexInSpecialEffect, soundView.SoundVolume, soundView.PanningPosition, soundView.PitchPosition);
            activeSound = soundProfile;
            activeSound.SoundID = -1;
        }

        private const Int32 playlistSoundCount = 100;

        private SoundView soundView;

        public Boolean IsPlay;

        public Boolean IsSaXAudio = ISdLibAPIProxy.Instance is SdLibAPIWithSaXAudio;

        public SaXAudio.EqParameters Eq = new SaXAudio.EqParameters();

        public SaXAudio.ReverbParameters Reverb = new SaXAudio.ReverbParameters();

        public SaXAudio.EchoParameters Echo = new SaXAudio.EchoParameters();

        public Boolean IsReverbEnabled = false;

        public Boolean IsEqEnabled = false;

        public Boolean IsEchoEnabled = false;

        public Boolean IsLooping = false;

        public SortedDictionary<String, AudioEffectManager.EffectPreset> EffectPresetDictionary = new SortedDictionary<String, AudioEffectManager.EffectPreset>();

        public Dictionary<String, List<SoundProfile>> ModVoiceDictionary = new Dictionary<String, List<SoundProfile>>();

        private String currentVoiceMod = "";

        private String voiceFilter = "";

        private SoundProfileType activeType = SoundProfileType.SoundEffect;

        private SoundProfile activeSound = null;

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

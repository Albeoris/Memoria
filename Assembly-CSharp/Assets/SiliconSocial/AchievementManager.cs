using Memoria.Prime;
using SiliconStudio;
using System;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

namespace Assets.SiliconSocial
{
    public static class AchievementManager
    {
        public static void NewGame()
        {
            for (Int32 i = 0; i < (Int32)AchievementManager.AchievementRetryCount.Length; i++)
            {
                AchievementManager.AchievementRetryCount[i] = 0;
            }
        }

        public static AchievementStatusesEnum GetAchievementStatus(AcheivementKey key)
        {
            AchievementStatusesEnum result;
            if (AchievementState.IsSystemAchievement(key))
            {
                Byte data = FF9StateSystem.Settings.SystemAchievementStatuses[0];
                Int32 bitShifted = 0;
                if (key != AcheivementKey.CompleteGame)
                {
                    if (key == AcheivementKey.Blackjack)
                    {
                        bitShifted = 2;
                    }
                }
                result = AchievementState.ConvertDataToAchievementStatus((Int32)data, bitShifted);
            }
            else
            {
                result = FF9StateSystem.Achievement.GetNormalAchievementStatuses(key);
            }
            return result;
        }

        public static void SetAchievementStatus(AcheivementKey key, AchievementStatusesEnum status)
        {
            if (AchievementState.IsSystemAchievement(key))
            {
                Int32 num = 0;
                if (key != AcheivementKey.CompleteGame)
                {
                    if (key == AcheivementKey.Blackjack)
                    {
                        num = 2;
                    }
                }
                Byte b = (Byte)AchievementState.ConvertAchievementStatusToData(status, num);
                Byte b2 = (Byte)(3 << num);
                Byte[] systemAchievementStatuses = FF9StateSystem.Settings.SystemAchievementStatuses;
                Int32 num2 = 0;
                systemAchievementStatuses[num2] = (Byte)(systemAchievementStatuses[num2] & (Byte)(~b2));
                Byte[] systemAchievementStatuses2 = FF9StateSystem.Settings.SystemAchievementStatuses;
                Int32 num3 = 0;
                systemAchievementStatuses2[num3] = (Byte)(systemAchievementStatuses2[num3] | b);
                FF9StateSystem.Serializer.SetSystemAchievementStatuses(FF9StateSystem.Settings.SystemAchievementStatuses, delegate (DataSerializerErrorCode errNo)
                {
                });
            }
            else
            {
                FF9StateSystem.Achievement.SetNormalAchievementStatuses(key, status);
            }
        }

        public static void ReportAchievement(AcheivementKey key, Int32 totalProgress)
        {
            if (key == AcheivementKey.AllAbility || key == AcheivementKey.CardWinAll)
                return;
            AchievementStatusesEnum achievementStatus = AchievementManager.GetAchievementStatus(key);
            if (FF9StateSystem.Settings.IsFastTrophyMode)
                totalProgress = AchievementManager.GetFastTrophyModeProgess(key, totalProgress);
            Int32 percentProgress = AchievementManager.CalculateProgressPercent(key, totalProgress);
            Int32 retryCount = 1;
            if (achievementStatus == AchievementStatusesEnum.NotUnlockYet)
            {
                if (percentProgress >= 100)
                {
                    AchievementManager.SetAchievementStatus(key, AchievementStatusesEnum.ReadyToUnlock);
                    retryCount = 5;
                }
                AchievementManager.ProcessAchievementReport(key, totalProgress, percentProgress, retryCount);
            }
            else
            {
                Debug.Log("status != NotUnlockYet. Do nothing.");
            }
        }

        private static Int32 CalculateProgressPercent(AcheivementKey key, Int32 progress)
        {
            Single target = AchievementManager.Data[key].Target;
            return (Int32)Math.Floor(progress / target * 100f);
        }

        public static void ProcessAchievementReport(AcheivementKey key, Int32 progress, Int32 percentProgress, Int32 retryCount)
        {
            if (!Social.IsSocialPlatformAuthenticated())
                return;
            AchievementManager.AchievementRetryCount[(Int32)key] = retryCount;
            AchievementManager.SendAchievementRequestPerPlatform(key, progress, percentProgress);
        }

        private static void SendAchievementRequestPerPlatform(AcheivementKey key, Int32 progress, Int32 percentProgress)
        {
            String refKey_SteamStat = AchievementManager.Data[key].refKey_SteamStat;
            if (!String.IsNullOrEmpty(refKey_SteamStat))
            {
                Social.UpdateStatSteam(refKey_SteamStat, progress, delegate (Boolean success)
                {
                    AchievementManager.ReportCallback(key, progress, percentProgress, success);
                });
            }
            else if (percentProgress >= 100)
            {
                Social.ReportAchievement(key, AchievementManager.GetRefKeyForPlatform(key), percentProgress, delegate (Boolean success)
                {
                    AchievementManager.ReportCallback(key, progress, percentProgress, success);
                });
            }
        }

        public static void ResyncSystemAchievements()
        {
            if (!Social.IsSocialPlatformAuthenticated())
            {
                Debug.Log("Player try to resync system achievement but he did not login or social was disabled!!!");
                return;
            }
            Debug.Log("ResyncSystemAchievements START");
            AchievementManager.ResyncAchievementsWithLocalStatus(Social.steamAchievementData, true);
            Debug.Log("ResyncSystemAchievements (SteamAchievementData) DONE");
        }

        public static void ResyncNormalAchievements()
        {
            if (!Social.IsSocialPlatformAuthenticated())
            {
                Debug.Log("Player try to resync normal achievement but he did not login or social was disabled!!!");
                return;
            }
            Debug.Log("ResyncNormalAchievements START");
            AchievementManager.ResyncAchievementsWithLocalStatus(Social.steamAchievementData, false);
            Debug.Log("ResyncNormalAchievements (SteamAchievementData) DONE");
        }

        public static void ResyncAchievementsWithLocalStatus(IAchievement[] loadedAchievements, Boolean isSystemAchievement)
        {
            try
            {
                foreach (KeyValuePair<AcheivementKey, AchievementInfo> keyValuePair in AchievementManager.Data)
                {

                    AcheivementKey key = keyValuePair.Key;
                    if (key != AcheivementKey.AchievementKeyCount && isSystemAchievement == AchievementState.IsSystemAchievement(key))
                    {
                        AchievementStatusesEnum achievementStatus = AchievementManager.GetAchievementStatus(key);
                        if (achievementStatus == AchievementStatusesEnum.ReadyToUnlock || achievementStatus == AchievementStatusesEnum.UnlockComplete)
                        {
                            Boolean flag = false;
                            String refKeyForPlatform = AchievementManager.GetRefKeyForPlatform(key);
                            for (Int32 i = 0; i < (Int32)loadedAchievements.Length; i++)
                            {
                                if (String.Compare(loadedAchievements[i].id, refKeyForPlatform) == 0)
                                {
                                    flag = true;
                                    if (!loadedAchievements[i].completed)
                                    {
                                        Debug.Log("ResyncAchievementsWithLocalStatus : need to resync system achievement key = " + key);
                                        AchievementManager.ProcessAchievementReport(key, (Int32)AchievementManager.Data[key].Target, 100, 3);
                                    }
                                    else
                                    {
                                        Debug.Log(String.Concat(new Object[]
                                        {
                                            "ResyncAchievementsWithLocalStatus : do nothing loadedAch[",
                                            i,
                                            "].completed = ",
                                            loadedAchievements[i].completed,
                                            ", key = ",
                                            key,
                                            ", localStatus = ",
                                            achievementStatus
                                        }));
                                    }
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                Debug.Log("ResyncAchievementsWithLocalStatus : need to resync key = " + key + " that has no progress");
                                AchievementManager.ProcessAchievementReport(key, (Int32)AchievementManager.Data[key].Target, 100, 3);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public static void ResyncAchievementsWithLocalStatus(SteamAchievementData[] loadedAchievements, Boolean isSystemAchievement)
        {
            try
            {
                Debug.Log(String.Concat(new Object[]
                {
                    "loadedAchievements.Length = ",
                    (Int32)loadedAchievements.Length,
                    ", isSystem = ",
                    isSystemAchievement
                }));

                foreach (KeyValuePair<AcheivementKey, AchievementInfo> keyValuePair in AchievementManager.Data)
                {
                    AcheivementKey key = keyValuePair.Key;
                    if (key != AcheivementKey.AchievementKeyCount && isSystemAchievement == AchievementState.IsSystemAchievement(key))
                    {
                        AchievementStatusesEnum achievementStatus = AchievementManager.GetAchievementStatus(key);
                        Debug.Log(String.Concat(new Object[]
                        {
                            "loadedAchievements key = ",
                            key,
                            ", localStatus = ",
                            achievementStatus
                        }));
                        if (achievementStatus == AchievementStatusesEnum.ReadyToUnlock || achievementStatus == AchievementStatusesEnum.UnlockComplete)
                        {
                            Boolean flag = false;
                            String refKeyForPlatform = AchievementManager.GetRefKeyForPlatform(key);
                            for (Int32 i = 0; i < (Int32)loadedAchievements.Length; i++)
                            {
                                if (String.Compare(loadedAchievements[i].id, refKeyForPlatform) == 0)
                                {
                                    flag = true;
                                    if (loadedAchievements[i].completed == 0)
                                    {
                                        Debug.Log("ResyncAchievementsWithLocalStatus : need to resync system achievement key = " + key);
                                        AchievementManager.ProcessAchievementReport(key, (Int32)AchievementManager.Data[key].Target, 100, 3);
                                    }
                                    else
                                    {
                                        Debug.Log(String.Concat(new Object[]
                                        {
                                            "ResyncAchievementsWithLocalStatus : do nothing loadedAch[",
                                            i,
                                            "].completed = ",
                                            loadedAchievements[i].completed,
                                            ", localStatus = ",
                                            achievementStatus
                                        }));
                                    }
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                Debug.Log("ResyncAchievementsWithLocalStatus : need to resync key = " + key + " that has no progress");
                                AchievementManager.ProcessAchievementReport(key, (Int32)AchievementManager.Data[key].Target, 100, 3);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        private static Int32 GetFastTrophyModeProgess(AcheivementKey key, Int32 originalProgress)
        {
            Int32 factor = 1;
            switch (key)
            {
                case AcheivementKey.Synth10:
                    factor = 10; break;
                case AcheivementKey.Synth30:
                    factor = 15; break;
                case AcheivementKey.BlkMag100:
                    factor = 50; break;
                case AcheivementKey.WhtMag200:
                    factor = 100; break;
                case AcheivementKey.BluMag100:
                    factor = 50; break;
                case AcheivementKey.Summon50:
                    factor = 25; break;
                case AcheivementKey.Defeat100:
                    factor = 100; break;
                case AcheivementKey.Defeat1000:
                    factor = 200; break;
                case AcheivementKey.Defeat10000:
                    factor = 1000; break;
                case AcheivementKey.ChocoboLv99:
                    factor = 50; break;
                case AcheivementKey.Frog99:
                    factor = 33; break;
                case AcheivementKey.Auction10:
                    factor = 10; break;
                case AcheivementKey.CardWin10:
                    factor = 10; break;
                case AcheivementKey.CardWin100:
                    factor = 50; break;
                case AcheivementKey.CardWinAll:
                    factor = 80; break;
                case AcheivementKey.BackAttack30:
                    factor = 30; break;
                case AcheivementKey.Steal50:
                    factor = 50; break;
                case AcheivementKey.Defense50:
                    factor = 50; break;
                case AcheivementKey.Trance50:
                    factor = 50; break;
                case AcheivementKey.QueenReward10:
                    factor = 10; break;
                case AcheivementKey.ATE80:
                    factor = 79; break;
                case AcheivementKey.AllStiltzkinItem:
                case AcheivementKey.AllPasssiveAbility:
                case AcheivementKey.AllAbility:
                case AcheivementKey.EidolonMural:
                case AcheivementKey.ProvokeMoogle:
                case AcheivementKey.AllSandyBeach:
                case AcheivementKey.AllTreasure:
                case AcheivementKey.MognetCentral:
                case AcheivementKey.SuperSlickOil:
                case AcheivementKey.GoldenFrog:
                case AcheivementKey.Airship:
                case AcheivementKey.PartyWomen:
                case AcheivementKey.PartyMen:
                case AcheivementKey.AbnormalStatus:
                case AcheivementKey.Trance1:
                default:
                    break;
            }
            return originalProgress * factor;
        }

        public static String GetRefKeyForPlatform(AcheivementKey key)
        {
            return AchievementManager.Data[key].refKey_Steam;
        }

        public static void ReportCallback(AcheivementKey Key, Int32 progress, Int32 percentProgress, Boolean isSuccess)
        {
            Debug.Log($"reportachievement callback isSuccess = {isSuccess}, Key = {Key}, keyID = {(Int32)Key}, progress = {progress}, percent = {percentProgress}");
            if (isSuccess)
            {
                if (percentProgress >= 100)
                {
                    progress = (Int32)AchievementManager.Data[Key].Target;
                    percentProgress = 100;
                    Debug.Log("achievement Key = " + Key + " is unlocked. Mark status to UnlockComplete");
                    AchievementManager.SetAchievementStatus(Key, AchievementStatusesEnum.UnlockComplete);
                }
                AchievementManager.AchievementRetryCount[(Int32)Key] = 0;
                return;
            }
            if (AchievementManager.AchievementRetryCount[(Int32)Key] == 0)
            {
                return;
            }
            AchievementManager.AchievementRetryCount[(Int32)Key]--;
            Debug.Log(String.Concat(new Object[]
            {
                "SendAchievementRequestPerPlatform again key = ",
                Key,
                ", retryCount = ",
                AchievementManager.AchievementRetryCount[(Int32)Key]
            }));
            AchievementManager.SendAchievementRequestPerPlatform(Key, progress, percentProgress);
        }

        public static Int32[] AchievementRetryCount = new Int32[87];

        public static readonly Dictionary<AcheivementKey, AchievementInfo> DataJapanese = new Dictionary<AcheivementKey, AchievementInfo>
        {
            {
                AcheivementKey.Synth10,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQCA",
                    refKey_iOS = "ff9_acv001jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQAA",
                    refKey_aaaa = "none",
                    Target = 10f,
                    refKey_SteamStat = "stat_synth"
                }
            },
            {
                AcheivementKey.Synth30,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQCQ",
                    refKey_iOS = "ff9_acv002jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQAQ",
                    refKey_aaaa = "none",
                    Target = 30f,
                    refKey_SteamStat = "stat_synth"
                }
            },
            {
                AcheivementKey.BlkMag100,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQCg",
                    refKey_iOS = "ff9_acv003jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQAg",
                    refKey_aaaa = "none",
                    Target = 100f,
                    refKey_SteamStat = "stat_blkMag"
                }
            },
            {
                AcheivementKey.WhtMag200,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQCw",
                    refKey_iOS = "ff9_acv004jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQAw",
                    refKey_aaaa = "none",
                    Target = 200f,
                    refKey_SteamStat = "stat_whtMag"
                }
            },
            {
                AcheivementKey.BluMag100,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQDA",
                    refKey_iOS = "ff9_acv005jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQBA",
                    refKey_aaaa = "none",
                    Target = 100f,
                    refKey_SteamStat = "stat_bluMag"
                }
            },
            {
                AcheivementKey.Summon50,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQDQ",
                    refKey_iOS = "ff9_acv006jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQBQ",
                    refKey_aaaa = "none",
                    Target = 50f,
                    refKey_SteamStat = "stat_summon"
                }
            },
            {
                AcheivementKey.Defeat100,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQDg",
                    refKey_iOS = "ff9_acv007jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQBg",
                    refKey_aaaa = "none",
                    Target = 100f,
                    refKey_SteamStat = "stat_defeat"
                }
            },
            {
                AcheivementKey.Defeat1000,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQDw",
                    refKey_iOS = "ff9_acv008jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQBw",
                    refKey_aaaa = "none",
                    Target = 1000f,
                    refKey_SteamStat = "stat_defeat"
                }
            },
            {
                AcheivementKey.Defeat10000,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQEA",
                    refKey_iOS = "ff9_acv009jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQCA",
                    refKey_aaaa = "1",
                    Target = 10000f,
                    refKey_SteamStat = "stat_defeat"
                }
            },
            {
                AcheivementKey.AllStiltzkinItem,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQEQ",
                    refKey_iOS = "ff9_acv010jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQCQ",
                    refKey_aaaa = "none",
                    Target = 8f,
                    refKey_SteamStat = "stat_item"
                }
            },
            {
                AcheivementKey.AllPasssiveAbility,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQEg",
                    refKey_iOS = "ff9_acv011jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQCg",
                    refKey_aaaa = "none",
                    Target = 63f,
                    refKey_SteamStat = "stat_passiveAbility"
                }
            },
            {
                AcheivementKey.AllAbility,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQEw",
                    refKey_iOS = "ff9_acv012jpn",
                    refKey_Andrd = String.Empty,
                    refKey_aaaa = "2",
                    Target = 183f,
                    refKey_SteamStat = "stat_allAbility"
                }
            },
            {
                AcheivementKey.EidolonMural,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQFA",
                    refKey_iOS = "ff9_acv013jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQCw",
                    refKey_aaaa = "3",
                    Target = 1f
                }
            },
            {
                AcheivementKey.ProvokeMoogle,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQFQ",
                    refKey_iOS = "ff9_acv014jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQDA",
                    refKey_aaaa = "4",
                    Target = 1f
                }
            },
            {
                AcheivementKey.ChocoboLv99,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQFg",
                    refKey_iOS = "ff9_acv015jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQDQ",
                    refKey_aaaa = "5",
                    Target = 99f,
                    refKey_SteamStat = "stat_chocoboLV"
                }
            },
            {
                AcheivementKey.AllSandyBeach,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQFw",
                    refKey_iOS = "ff9_acv016jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQDg",
                    refKey_aaaa = "6",
                    Target = 21f,
                    refKey_SteamStat = "stat_beach"
                }
            },
            {
                AcheivementKey.AllTreasure,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQGA",
                    refKey_iOS = "ff9_acv017jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQDw",
                    refKey_aaaa = "7",
                    Target = 24f,
                    refKey_SteamStat = "stat_treasure"
                }
            },
            {
                AcheivementKey.MognetCentral,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQGQ",
                    refKey_iOS = "ff9_acv018jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQEA",
                    refKey_aaaa = "8",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SuperSlickOil,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQGg",
                    refKey_iOS = "ff9_acv019jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQEQ",
                    refKey_aaaa = "9",
                    Target = 1f
                }
            },
            {
                AcheivementKey.Frog99,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQGw",
                    refKey_iOS = "ff9_acv020jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQEg",
                    refKey_aaaa = "10",
                    Target = 99f,
                    refKey_SteamStat = "stat_frog"
                }
            },
            {
                AcheivementKey.GoldenFrog,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQHA",
                    refKey_iOS = "ff9_acv021jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQEw",
                    refKey_aaaa = "11",
                    Target = 1f
                }
            },
            {
                AcheivementKey.Auction10,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQHQ",
                    refKey_iOS = "ff9_acv022jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQFA",
                    refKey_aaaa = "12",
                    Target = 10f,
                    refKey_SteamStat = "stat_auction"
                }
            },
            {
                AcheivementKey.Excalibur,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQHg",
                    refKey_iOS = "ff9_acv023jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQFQ",
                    refKey_aaaa = "13",
                    Target = 1f
                }
            },
            {
                AcheivementKey.AllOX,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQHw",
                    refKey_iOS = "ff9_acv024jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQFg",
                    refKey_aaaa = "14",
                    Target = 1f
                }
            },
            {
                AcheivementKey.YanBlessing,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQIA",
                    refKey_iOS = "ff9_acv025jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQFw",
                    refKey_aaaa = "15",
                    Target = 1f
                }
            },
            {
                AcheivementKey.DefeatOzma,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQIQ",
                    refKey_iOS = "ff9_acv026jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQGA",
                    refKey_aaaa = "16",
                    Target = 1f
                }
            },
            {
                AcheivementKey.ShipMaquette,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQIg",
                    refKey_iOS = "ff9_acv027jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQGQ",
                    refKey_aaaa = "17",
                    Target = 1f
                }
            },
            {
                AcheivementKey.QueenReward10,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQIw",
                    refKey_iOS = "ff9_acv028jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQGg",
                    refKey_aaaa = "18",
                    Target = 10f,
                    refKey_SteamStat = "stat_queenReward"
                }
            },
            {
                AcheivementKey.Hammer,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQJA",
                    refKey_iOS = "ff9_acv029jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQGw",
                    refKey_aaaa = "19",
                    Target = 1f
                }
            },
            {
                AcheivementKey.TreasureHuntS,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQJQ",
                    refKey_iOS = "ff9_acv030jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQHA",
                    refKey_aaaa = "20",
                    Target = 1f
                }
            },
            {
                AcheivementKey.DefeatBehemoth,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQJg",
                    refKey_iOS = "ff9_acv031jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQHQ",
                    refKey_aaaa = "21",
                    Target = 1f
                }
            },
            {
                AcheivementKey.Rope1000,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQJw",
                    refKey_iOS = "ff9_acv032jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQHg",
                    refKey_aaaa = "22",
                    Target = 1f
                }
            },
            {
                AcheivementKey.Rope100,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQKA",
                    refKey_iOS = "ff9_acv033jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQHw",
                    refKey_aaaa = "23",
                    Target = 1f
                }
            },
            {
                AcheivementKey.Encore,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQKQ",
                    refKey_iOS = "ff9_acv034jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQIA",
                    refKey_aaaa = "24",
                    Target = 1f
                }
            },
            {
                AcheivementKey.ViviWinHunt,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQKg",
                    refKey_iOS = "ff9_acv035jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQIQ",
                    refKey_aaaa = "25",
                    Target = 1f
                }
            },
            {
                AcheivementKey.CompleteGame,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQKw",
                    refKey_iOS = "ff9_acv036jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQIg",
                    refKey_aaaa = "26",
                    Target = 1f
                }
            },
            {
                AcheivementKey.CharLv99,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQLA",
                    refKey_iOS = "ff9_acv037jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQIw",
                    refKey_aaaa = "27",
                    Target = 1f
                }
            },
            {
                AcheivementKey.MadainRing,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQLQ",
                    refKey_iOS = "ff9_acv038jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQJA",
                    refKey_aaaa = "28",
                    Target = 1f
                }
            },
            {
                AcheivementKey.Kuppo,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQLg",
                    refKey_iOS = "ff9_acv039jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQJQ",
                    refKey_aaaa = "29",
                    Target = 1f
                }
            },
            {
                AcheivementKey.AthleteQueen,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQLw",
                    refKey_iOS = "ff9_acv040jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQJg",
                    refKey_aaaa = "30",
                    Target = 1f
                }
            },
            {
                AcheivementKey.Shuffle9,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQMA",
                    refKey_iOS = "ff9_acv041jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQJw",
                    refKey_aaaa = "31",
                    Target = 1f
                }
            },
            {
                AcheivementKey.Blackjack,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQMQ",
                    refKey_iOS = "ff9_acv042jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQKA",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.CardWin1,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQMg",
                    refKey_iOS = "ff9_acv043jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQKQ",
                    refKey_aaaa = "none",
                    Target = 1f,
                    refKey_SteamStat = "stat_winCardBattle"
                }
            },
            {
                AcheivementKey.CardWin10,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQMw",
                    refKey_iOS = "ff9_acv044jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQKg",
                    refKey_aaaa = "32",
                    Target = 10f,
                    refKey_SteamStat = "stat_winCardBattle"
                }
            },
            {
                AcheivementKey.CardWin100,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQNA",
                    refKey_iOS = "ff9_acv045jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQKw",
                    refKey_aaaa = "33",
                    Target = 100f,
                    refKey_SteamStat = "stat_winCardBattle"
                }
            },
            {
                AcheivementKey.CardWinAll,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQNQ",
                    refKey_iOS = "ff9_acv046jpn",
                    refKey_Andrd = String.Empty,
                    refKey_aaaa = "34",
                    Target = 235f,
                    refKey_SteamStat = "stat_winCardBattle"
                }
            },
            {
                AcheivementKey.Airship,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQNg",
                    refKey_iOS = "ff9_acv047jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQLA",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.BackAttack30,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQNw",
                    refKey_iOS = "ff9_acv048jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQLQ",
                    refKey_aaaa = "none",
                    Target = 30f,
                    refKey_SteamStat = "stat_backAttack"
                }
            },
            {
                AcheivementKey.Steal50,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQOA",
                    refKey_iOS = "ff9_acv049jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQLg",
                    refKey_aaaa = "none",
                    Target = 50f,
                    refKey_SteamStat = "stat_steal"
                }
            },
            {
                AcheivementKey.Defense50,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQOQ",
                    refKey_iOS = "ff9_acv050jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQLw",
                    refKey_aaaa = "none",
                    Target = 50f,
                    refKey_SteamStat = "stat_defense"
                }
            },
            {
                AcheivementKey.PartyWomen,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQOg",
                    refKey_iOS = "ff9_acv051jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQMA",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.PartyMen,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQOw",
                    refKey_iOS = "ff9_acv052jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQMQ",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.AbnormalStatus,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQPA",
                    refKey_iOS = "ff9_acv053jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQMg",
                    refKey_aaaa = "35",
                    Target = 1f
                }
            },
            {
                AcheivementKey.Trance1,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQPQ",
                    refKey_iOS = "ff9_acv054jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQMw",
                    refKey_aaaa = "none",
                    Target = 1f,
                    refKey_SteamStat = "stat_trance"
                }
            },
            {
                AcheivementKey.Trance50,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQPg",
                    refKey_iOS = "ff9_acv055jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQNA",
                    refKey_aaaa = "36",
                    Target = 50f,
                    refKey_SteamStat = "stat_trance"
                }
            },
            {
                AcheivementKey.RebirthFlame,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQPw",
                    refKey_iOS = "ff9_acv056jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQNQ",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonShiva,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQQA",
                    refKey_iOS = "ff9_acv057jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQNg",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonIfrit,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQQQ",
                    refKey_iOS = "ff9_acv058jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQNw",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonRamuh,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQQg",
                    refKey_iOS = "ff9_acv059jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQOA",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonAtomos,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQQw",
                    refKey_iOS = "ff9_acv060jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQOQ",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonOdin,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQRA",
                    refKey_iOS = "ff9_acv061jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQOg",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonLeviathan,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQRQ",
                    refKey_iOS = "ff9_acv062jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQOw",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonBahamut,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQRg",
                    refKey_iOS = "ff9_acv063jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQPA",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonArk,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQRw",
                    refKey_iOS = "ff9_acv064jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQPQ",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonCarbuncle,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQSA",
                    refKey_iOS = "ff9_acv065jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQPg",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonFenrir,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQSQ",
                    refKey_iOS = "ff9_acv066jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQPw",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonPhoenix,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQSg",
                    refKey_iOS = "ff9_acv067jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQQA",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonMadeen,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQSw",
                    refKey_iOS = "ff9_acv068jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQQQ",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.ATE80,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQTA",
                    refKey_iOS = "ff9_acv069jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQQg",
                    refKey_aaaa = "37",
                    Target = 79f,
                    refKey_SteamStat = "stat_ATE"
                }
            },
            {
                AcheivementKey.Moonstone4,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQTQ",
                    refKey_iOS = "ff9_acv070jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQQw",
                    refKey_aaaa = "38",
                    Target = 4f,
                    refKey_SteamStat = "stat_moonStone"
                }
            },
            {
                AcheivementKey.KainLance,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQTg",
                    refKey_iOS = "ff9_acv071jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQRA",
                    refKey_aaaa = "39",
                    Target = 1f
                }
            },
            {
                AcheivementKey.TheTower,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQTw",
                    refKey_iOS = "ff9_acv072jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQRQ",
                    refKey_aaaa = "40",
                    Target = 1f
                }
            },
            {
                AcheivementKey.RuneClaws,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQUA",
                    refKey_iOS = "ff9_acv073jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQRg",
                    refKey_aaaa = "41",
                    Target = 1f
                }
            },
            {
                AcheivementKey.AngelFlute,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQUQ",
                    refKey_iOS = "ff9_acv074jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQRw",
                    refKey_aaaa = "42",
                    Target = 1f
                }
            },
            {
                AcheivementKey.MaceOfZeus,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQUg",
                    refKey_iOS = "ff9_acv075jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQSA",
                    refKey_aaaa = "43",
                    Target = 1f
                }
            },
            {
                AcheivementKey.GastroFork,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQUw",
                    refKey_iOS = "ff9_acv076jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQSQ",
                    refKey_aaaa = "44",
                    Target = 1f
                }
            },
            {
                AcheivementKey.ExcaliburII,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQVA",
                    refKey_iOS = "ff9_acv077jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQSg",
                    refKey_aaaa = "45",
                    Target = 1f
                }
            },
            {
                AcheivementKey.WhaleWhisker,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQVQ",
                    refKey_iOS = "ff9_acv078jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQSw",
                    refKey_aaaa = "46",
                    Target = 1f
                }
            },
            {
                AcheivementKey.TigerHands,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQVg",
                    refKey_iOS = "ff9_acv079jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQTA",
                    refKey_aaaa = "47",
                    Target = 1f
                }
            },
            {
                AcheivementKey.UltimaWeapon,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQVw",
                    refKey_iOS = "ff9_acv080jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQTQ",
                    refKey_aaaa = "48",
                    Target = 1f
                }
            },
            {
                AcheivementKey.GenjiSet,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQWA",
                    refKey_iOS = "ff9_acv081jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQTg",
                    refKey_aaaa = "49",
                    Target = 1f
                }
            },
            {
                AcheivementKey.ExcellentLuck,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQWQ",
                    refKey_iOS = "ff9_acv082jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQTw",
                    refKey_aaaa = "50",
                    Target = 1f
                }
            },
            {
                AcheivementKey.CleyraVictimAll,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQWg",
                    refKey_iOS = "ff9_acv083jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQUA",
                    refKey_aaaa = "51",
                    Target = 1f
                }
            },
            {
                AcheivementKey.DefeatMaliris,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQWw",
                    refKey_iOS = "ff9_acv084jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQUQ",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.DefeatTiamat,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQXA",
                    refKey_iOS = "ff9_acv085jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQUg",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.DefeatKraken,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQXQ",
                    refKey_iOS = "ff9_acv086jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQUw",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.DefeatLich,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQXg",
                    refKey_iOS = "ff9_acv087jpn",
                    refKey_Andrd = "CgkI5MCfo_0aEAIQVA",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            }
        };

        public static readonly Dictionary<AcheivementKey, AchievementInfo> DataWorld = new Dictionary<AcheivementKey, AchievementInfo>
        {
            {
                AcheivementKey.Synth10,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQCA",
                    refKey_iOS = "ff9_acv001",
                    refKey_Andrd = "CgkIloijmJcXEAIQAA",
                    refKey_aaaa = "none",
                    Target = 10f,
                    refKey_SteamStat = "stat_synth"
                }
            },
            {
                AcheivementKey.Synth30,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQCQ",
                    refKey_iOS = "ff9_acv002",
                    refKey_Andrd = "CgkIloijmJcXEAIQAQ",
                    refKey_aaaa = "none",
                    Target = 30f,
                    refKey_SteamStat = "stat_synth"
                }
            },
            {
                AcheivementKey.BlkMag100,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQCg",
                    refKey_iOS = "ff9_acv003",
                    refKey_Andrd = "CgkIloijmJcXEAIQAg",
                    refKey_aaaa = "none",
                    Target = 100f,
                    refKey_SteamStat = "stat_blkMag"
                }
            },
            {
                AcheivementKey.WhtMag200,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQCw",
                    refKey_iOS = "ff9_acv004",
                    refKey_Andrd = "CgkIloijmJcXEAIQAw",
                    refKey_aaaa = "none",
                    Target = 200f,
                    refKey_SteamStat = "stat_whtMag"
                }
            },
            {
                AcheivementKey.BluMag100,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQDA",
                    refKey_iOS = "ff9_acv005",
                    refKey_Andrd = "CgkIloijmJcXEAIQBA",
                    refKey_aaaa = "none",
                    Target = 100f,
                    refKey_SteamStat = "stat_bluMag"
                }
            },
            {
                AcheivementKey.Summon50,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQDQ",
                    refKey_iOS = "ff9_acv006",
                    refKey_Andrd = "CgkIloijmJcXEAIQBQ",
                    refKey_aaaa = "none",
                    Target = 50f,
                    refKey_SteamStat = "stat_summon"
                }
            },
            {
                AcheivementKey.Defeat100,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQDg",
                    refKey_iOS = "ff9_acv007",
                    refKey_Andrd = "CgkIloijmJcXEAIQBg",
                    refKey_aaaa = "none",
                    Target = 100f,
                    refKey_SteamStat = "stat_defeat"
                }
            },
            {
                AcheivementKey.Defeat1000,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQDw",
                    refKey_iOS = "ff9_acv008",
                    refKey_Andrd = "CgkIloijmJcXEAIQBw",
                    refKey_aaaa = "none",
                    Target = 1000f,
                    refKey_SteamStat = "stat_defeat"
                }
            },
            {
                AcheivementKey.Defeat10000,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQEA",
                    refKey_iOS = "ff9_acv009",
                    refKey_Andrd = "CgkIloijmJcXEAIQCA",
                    refKey_aaaa = "1",
                    Target = 10000f,
                    refKey_SteamStat = "stat_defeat"
                }
            },
            {
                AcheivementKey.AllStiltzkinItem,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQEQ",
                    refKey_iOS = "ff9_acv010",
                    refKey_Andrd = "CgkIloijmJcXEAIQCQ",
                    refKey_aaaa = "none",
                    Target = 8f,
                    refKey_SteamStat = "stat_item"
                }
            },
            {
                AcheivementKey.AllPasssiveAbility,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQEg",
                    refKey_iOS = "ff9_acv011",
                    refKey_Andrd = "CgkIloijmJcXEAIQCg",
                    refKey_aaaa = "none",
                    Target = 63f,
                    refKey_SteamStat = "stat_passiveAbility"
                }
            },
            {
                AcheivementKey.AllAbility,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQEw",
                    refKey_iOS = "ff9_acv012",
                    refKey_Andrd = string.Empty,
                    refKey_aaaa = "2",
                    Target = 183f,
                    refKey_SteamStat = "stat_allAbility"
                }
            },
            {
                AcheivementKey.EidolonMural,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQFA",
                    refKey_iOS = "ff9_acv013",
                    refKey_Andrd = "CgkIloijmJcXEAIQCw",
                    refKey_aaaa = "3",
                    Target = 1f
                }
            },
            {
                AcheivementKey.ProvokeMoogle,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQFQ",
                    refKey_iOS = "ff9_acv014",
                    refKey_Andrd = "CgkIloijmJcXEAIQDA",
                    refKey_aaaa = "4",
                    Target = 1f
                }
            },
            {
                AcheivementKey.ChocoboLv99,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQFg",
                    refKey_iOS = "ff9_acv015",
                    refKey_Andrd = "CgkIloijmJcXEAIQDQ",
                    refKey_aaaa = "5",
                    Target = 99f,
                    refKey_SteamStat = "stat_chocoboLV"
                }
            },
            {
                AcheivementKey.AllSandyBeach,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQFw",
                    refKey_iOS = "ff9_acv016",
                    refKey_Andrd = "CgkIloijmJcXEAIQDg",
                    refKey_aaaa = "6",
                    Target = 21f,
                    refKey_SteamStat = "stat_beach"
                }
            },
            {
                AcheivementKey.AllTreasure,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQGA",
                    refKey_iOS = "ff9_acv017",
                    refKey_Andrd = "CgkIloijmJcXEAIQDw",
                    refKey_aaaa = "7",
                    Target = 24f,
                    refKey_SteamStat = "stat_treasure"
                }
            },
            {
                AcheivementKey.MognetCentral,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQGQ",
                    refKey_iOS = "ff9_acv018",
                    refKey_Andrd = "CgkIloijmJcXEAIQEA",
                    refKey_aaaa = "8",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SuperSlickOil,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQGg",
                    refKey_iOS = "ff9_acv019",
                    refKey_Andrd = "CgkIloijmJcXEAIQEQ",
                    refKey_aaaa = "9",
                    Target = 1f
                }
            },
            {
                AcheivementKey.Frog99,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQGw",
                    refKey_iOS = "ff9_acv020",
                    refKey_Andrd = "CgkIloijmJcXEAIQEg",
                    refKey_aaaa = "10",
                    Target = 99f,
                    refKey_SteamStat = "stat_frog"
                }
            },
            {
                AcheivementKey.GoldenFrog,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQHA",
                    refKey_iOS = "ff9_acv021",
                    refKey_Andrd = "CgkIloijmJcXEAIQEw",
                    refKey_aaaa = "11",
                    Target = 1f
                }
            },
            {
                AcheivementKey.Auction10,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQHQ",
                    refKey_iOS = "ff9_acv022",
                    refKey_Andrd = "CgkIloijmJcXEAIQFA",
                    refKey_aaaa = "12",
                    Target = 10f,
                    refKey_SteamStat = "stat_auction"
                }
            },
            {
                AcheivementKey.Excalibur,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQHg",
                    refKey_iOS = "ff9_acv023",
                    refKey_Andrd = "CgkIloijmJcXEAIQFQ",
                    refKey_aaaa = "13",
                    Target = 1f
                }
            },
            {
                AcheivementKey.AllOX,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQHw",
                    refKey_iOS = "ff9_acv024",
                    refKey_Andrd = "CgkIloijmJcXEAIQFg",
                    refKey_aaaa = "14",
                    Target = 1f
                }
            },
            {
                AcheivementKey.YanBlessing,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQIA",
                    refKey_iOS = "ff9_acv025",
                    refKey_Andrd = "CgkIloijmJcXEAIQFw",
                    refKey_aaaa = "15",
                    Target = 1f
                }
            },
            {
                AcheivementKey.DefeatOzma,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQIQ",
                    refKey_iOS = "ff9_acv026",
                    refKey_Andrd = "CgkIloijmJcXEAIQGA",
                    refKey_aaaa = "16",
                    Target = 1f
                }
            },
            {
                AcheivementKey.ShipMaquette,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQIg",
                    refKey_iOS = "ff9_acv027",
                    refKey_Andrd = "CgkIloijmJcXEAIQGQ",
                    refKey_aaaa = "17",
                    Target = 1f
                }
            },
            {
                AcheivementKey.QueenReward10,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQIw",
                    refKey_iOS = "ff9_acv028",
                    refKey_Andrd = "CgkIloijmJcXEAIQGg",
                    refKey_aaaa = "18",
                    Target = 10f,
                    refKey_SteamStat = "stat_queenReward"
                }
            },
            {
                AcheivementKey.Hammer,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQJA",
                    refKey_iOS = "ff9_acv029",
                    refKey_Andrd = "CgkIloijmJcXEAIQGw",
                    refKey_aaaa = "19",
                    Target = 1f
                }
            },
            {
                AcheivementKey.TreasureHuntS,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQJQ",
                    refKey_iOS = "ff9_acv030",
                    refKey_Andrd = "CgkIloijmJcXEAIQHA",
                    refKey_aaaa = "20",
                    Target = 1f
                }
            },
            {
                AcheivementKey.DefeatBehemoth,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQJg",
                    refKey_iOS = "ff9_acv031",
                    refKey_Andrd = "CgkIloijmJcXEAIQHQ",
                    refKey_aaaa = "21",
                    Target = 1f
                }
            },
            {
                AcheivementKey.Rope1000,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQJw",
                    refKey_iOS = "ff9_acv032",
                    refKey_Andrd = "CgkIloijmJcXEAIQHg",
                    refKey_aaaa = "22",
                    Target = 1f
                }
            },
            {
                AcheivementKey.Rope100,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQKA",
                    refKey_iOS = "ff9_acv033",
                    refKey_Andrd = "CgkIloijmJcXEAIQHw",
                    refKey_aaaa = "23",
                    Target = 1f
                }
            },
            {
                AcheivementKey.Encore,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQKQ",
                    refKey_iOS = "ff9_acv034",
                    refKey_Andrd = "CgkIloijmJcXEAIQIA",
                    refKey_aaaa = "24",
                    Target = 1f
                }
            },
            {
                AcheivementKey.ViviWinHunt,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQKg",
                    refKey_iOS = "ff9_acv035",
                    refKey_Andrd = "CgkIloijmJcXEAIQIQ",
                    refKey_aaaa = "25",
                    Target = 1f
                }
            },
            {
                AcheivementKey.CompleteGame,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQKw",
                    refKey_iOS = "ff9_acv036",
                    refKey_Andrd = "CgkIloijmJcXEAIQIg",
                    refKey_aaaa = "26",
                    Target = 1f
                }
            },
            {
                AcheivementKey.CharLv99,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQLA",
                    refKey_iOS = "ff9_acv037",
                    refKey_Andrd = "CgkIloijmJcXEAIQIw",
                    refKey_aaaa = "27",
                    Target = 1f
                }
            },
            {
                AcheivementKey.MadainRing,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQLQ",
                    refKey_iOS = "ff9_acv038",
                    refKey_Andrd = "CgkIloijmJcXEAIQJA",
                    refKey_aaaa = "28",
                    Target = 1f
                }
            },
            {
                AcheivementKey.Kuppo,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQLg",
                    refKey_iOS = "ff9_acv039",
                    refKey_Andrd = "CgkIloijmJcXEAIQJQ",
                    refKey_aaaa = "29",
                    Target = 1f
                }
            },
            {
                AcheivementKey.AthleteQueen,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQLw",
                    refKey_iOS = "ff9_acv040",
                    refKey_Andrd = "CgkIloijmJcXEAIQJg",
                    refKey_aaaa = "30",
                    Target = 1f
                }
            },
            {
                AcheivementKey.Shuffle9,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQMA",
                    refKey_iOS = "ff9_acv041",
                    refKey_Andrd = "CgkIloijmJcXEAIQJw",
                    refKey_aaaa = "31",
                    Target = 1f
                }
            },
            {
                AcheivementKey.Blackjack,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQMQ",
                    refKey_iOS = "ff9_acv042",
                    refKey_Andrd = "CgkIloijmJcXEAIQKA",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.CardWin1,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQMg",
                    refKey_iOS = "ff9_acv043",
                    refKey_Andrd = "CgkIloijmJcXEAIQKQ",
                    refKey_aaaa = "none",
                    Target = 1f,
                    refKey_SteamStat = "stat_winCardBattle"
                }
            },
            {
                AcheivementKey.CardWin10,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQMw",
                    refKey_iOS = "ff9_acv044",
                    refKey_Andrd = "CgkIloijmJcXEAIQKg",
                    refKey_aaaa = "32",
                    Target = 10f,
                    refKey_SteamStat = "stat_winCardBattle"
                }
            },
            {
                AcheivementKey.CardWin100,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQNA",
                    refKey_iOS = "ff9_acv045",
                    refKey_Andrd = "CgkIloijmJcXEAIQKw",
                    refKey_aaaa = "33",
                    Target = 100f,
                    refKey_SteamStat = "stat_winCardBattle"
                }
            },
            {
                AcheivementKey.CardWinAll,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQNQ",
                    refKey_iOS = "ff9_acv046",
                    refKey_Andrd = string.Empty,
                    refKey_aaaa = "34",
                    Target = 235f,
                    refKey_SteamStat = "stat_winCardBattle"
                }
            },
            {
                AcheivementKey.Airship,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQNg",
                    refKey_iOS = "ff9_acv047",
                    refKey_Andrd = "CgkIloijmJcXEAIQLA",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.BackAttack30,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQNw",
                    refKey_iOS = "ff9_acv048",
                    refKey_Andrd = "CgkIloijmJcXEAIQLQ",
                    refKey_aaaa = "none",
                    Target = 30f,
                    refKey_SteamStat = "stat_backAttack"
                }
            },
            {
                AcheivementKey.Steal50,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQOA",
                    refKey_iOS = "ff9_acv049",
                    refKey_Andrd = "CgkIloijmJcXEAIQLg",
                    refKey_aaaa = "none",
                    Target = 50f,
                    refKey_SteamStat = "stat_steal"
                }
            },
            {
                AcheivementKey.Defense50,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQOQ",
                    refKey_iOS = "ff9_acv050",
                    refKey_Andrd = "CgkIloijmJcXEAIQLw",
                    refKey_aaaa = "none",
                    Target = 50f,
                    refKey_SteamStat = "stat_defense"
                }
            },
            {
                AcheivementKey.PartyWomen,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQOg",
                    refKey_iOS = "ff9_acv051",
                    refKey_Andrd = "CgkIloijmJcXEAIQMA",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.PartyMen,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQOw",
                    refKey_iOS = "ff9_acv052",
                    refKey_Andrd = "CgkIloijmJcXEAIQMQ",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.AbnormalStatus,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQPA",
                    refKey_iOS = "ff9_acv053",
                    refKey_Andrd = "CgkIloijmJcXEAIQMg",
                    refKey_aaaa = "35",
                    Target = 1f
                }
            },
            {
                AcheivementKey.Trance1,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQPQ",
                    refKey_iOS = "ff9_acv054",
                    refKey_Andrd = "CgkIloijmJcXEAIQMw",
                    refKey_aaaa = "none",
                    Target = 1f,
                    refKey_SteamStat = "stat_trance"
                }
            },
            {
                AcheivementKey.Trance50,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQPg",
                    refKey_iOS = "ff9_acv055",
                    refKey_Andrd = "CgkIloijmJcXEAIQNA",
                    refKey_aaaa = "36",
                    Target = 50f,
                    refKey_SteamStat = "stat_trance"
                }
            },
            {
                AcheivementKey.RebirthFlame,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQPw",
                    refKey_iOS = "ff9_acv056",
                    refKey_Andrd = "CgkIloijmJcXEAIQNQ",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonShiva,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQQA",
                    refKey_iOS = "ff9_acv057",
                    refKey_Andrd = "CgkIloijmJcXEAIQNg",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonIfrit,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQQQ",
                    refKey_iOS = "ff9_acv058",
                    refKey_Andrd = "CgkIloijmJcXEAIQNw",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonRamuh,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQQg",
                    refKey_iOS = "ff9_acv059",
                    refKey_Andrd = "CgkIloijmJcXEAIQOA",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonAtomos,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQQw",
                    refKey_iOS = "ff9_acv060",
                    refKey_Andrd = "CgkIloijmJcXEAIQOQ",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonOdin,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQRA",
                    refKey_iOS = "ff9_acv061",
                    refKey_Andrd = "CgkIloijmJcXEAIQOg",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonLeviathan,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQRQ",
                    refKey_iOS = "ff9_acv062",
                    refKey_Andrd = "CgkIloijmJcXEAIQOw",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonBahamut,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQRg",
                    refKey_iOS = "ff9_acv063",
                    refKey_Andrd = "CgkIloijmJcXEAIQPA",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonArk,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQRw",
                    refKey_iOS = "ff9_acv064",
                    refKey_Andrd = "CgkIloijmJcXEAIQPQ",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonCarbuncle,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQSA",
                    refKey_iOS = "ff9_acv065",
                    refKey_Andrd = "CgkIloijmJcXEAIQPg",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonFenrir,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQSQ",
                    refKey_iOS = "ff9_acv066",
                    refKey_Andrd = "CgkIloijmJcXEAIQPw",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonPhoenix,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQSg",
                    refKey_iOS = "ff9_acv067",
                    refKey_Andrd = "CgkIloijmJcXEAIQQA",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.SummonMadeen,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQSw",
                    refKey_iOS = "ff9_acv068",
                    refKey_Andrd = "CgkIloijmJcXEAIQQQ",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.ATE80,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQTA",
                    refKey_iOS = "ff9_acv069",
                    refKey_Andrd = "CgkIloijmJcXEAIQQg",
                    refKey_aaaa = "37",
                    Target = 79f,
                    refKey_SteamStat = "stat_ATE"
                }
            },
            {
                AcheivementKey.Moonstone4,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQTQ",
                    refKey_iOS = "ff9_acv070",
                    refKey_Andrd = "CgkIloijmJcXEAIQQw",
                    refKey_aaaa = "38",
                    Target = 4f,
                    refKey_SteamStat = "stat_moonStone"
                }
            },
            {
                AcheivementKey.KainLance,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQTg",
                    refKey_iOS = "ff9_acv071",
                    refKey_Andrd = "CgkIloijmJcXEAIQRA",
                    refKey_aaaa = "39",
                    Target = 1f
                }
            },
            {
                AcheivementKey.TheTower,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQTw",
                    refKey_iOS = "ff9_acv072",
                    refKey_Andrd = "CgkIloijmJcXEAIQRQ",
                    refKey_aaaa = "40",
                    Target = 1f
                }
            },
            {
                AcheivementKey.RuneClaws,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQUA",
                    refKey_iOS = "ff9_acv073",
                    refKey_Andrd = "CgkIloijmJcXEAIQRg",
                    refKey_aaaa = "41",
                    Target = 1f
                }
            },
            {
                AcheivementKey.AngelFlute,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQUQ",
                    refKey_iOS = "ff9_acv074",
                    refKey_Andrd = "CgkIloijmJcXEAIQRw",
                    refKey_aaaa = "42",
                    Target = 1f
                }
            },
            {
                AcheivementKey.MaceOfZeus,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQUg",
                    refKey_iOS = "ff9_acv075",
                    refKey_Andrd = "CgkIloijmJcXEAIQSA",
                    refKey_aaaa = "43",
                    Target = 1f
                }
            },
            {
                AcheivementKey.GastroFork,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQUw",
                    refKey_iOS = "ff9_acv076",
                    refKey_Andrd = "CgkIloijmJcXEAIQSQ",
                    refKey_aaaa = "44",
                    Target = 1f
                }
            },
            {
                AcheivementKey.ExcaliburII,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQVA",
                    refKey_iOS = "ff9_acv077",
                    refKey_Andrd = "CgkIloijmJcXEAIQSg",
                    refKey_aaaa = "45",
                    Target = 1f
                }
            },
            {
                AcheivementKey.WhaleWhisker,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQVQ",
                    refKey_iOS = "ff9_acv078",
                    refKey_Andrd = "CgkIloijmJcXEAIQSw",
                    refKey_aaaa = "46",
                    Target = 1f
                }
            },
            {
                AcheivementKey.TigerHands,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQVg",
                    refKey_iOS = "ff9_acv079",
                    refKey_Andrd = "CgkIloijmJcXEAIQTA",
                    refKey_aaaa = "47",
                    Target = 1f
                }
            },
            {
                AcheivementKey.UltimaWeapon,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQVw",
                    refKey_iOS = "ff9_acv080",
                    refKey_Andrd = "CgkIloijmJcXEAIQTQ",
                    refKey_aaaa = "48",
                    Target = 1f
                }
            },
            {
                AcheivementKey.GenjiSet,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQWA",
                    refKey_iOS = "ff9_acv081",
                    refKey_Andrd = "CgkIloijmJcXEAIQTg",
                    refKey_aaaa = "49",
                    Target = 1f
                }
            },
            {
                AcheivementKey.ExcellentLuck,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQWQ",
                    refKey_iOS = "ff9_acv082",
                    refKey_Andrd = "CgkIloijmJcXEAIQTw",
                    refKey_aaaa = "50",
                    Target = 1f
                }
            },
            {
                AcheivementKey.CleyraVictimAll,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQWg",
                    refKey_iOS = "ff9_acv083",
                    refKey_Andrd = "CgkIloijmJcXEAIQUA",
                    refKey_aaaa = "51",
                    Target = 1f
                }
            },
            {
                AcheivementKey.DefeatMaliris,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQWw",
                    refKey_iOS = "ff9_acv084",
                    refKey_Andrd = "CgkIloijmJcXEAIQUQ",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.DefeatTiamat,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQXA",
                    refKey_iOS = "ff9_acv085",
                    refKey_Andrd = "CgkIloijmJcXEAIQUg",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.DefeatKraken,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQXQ",
                    refKey_iOS = "ff9_acv086",
                    refKey_Andrd = "CgkIloijmJcXEAIQUw",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            },
            {
                AcheivementKey.DefeatLich,
                new AchievementInfo
                {
                    refKey_Steam = "CgkIgrPmjcATEAIQXg",
                    refKey_iOS = "ff9_acv087",
                    refKey_Andrd = "CgkIloijmJcXEAIQVA",
                    refKey_aaaa = "none",
                    Target = 1f
                }
            }
        };

        public static readonly Dictionary<AcheivementKey, AchievementInfo> Data = GetData();

        private static Dictionary<AcheivementKey, AchievementInfo> GetData()
        {
            SharedDataBytesStorage.SetPCPath();
            if (SharedDataBytesStorage.MetaData.FilePath == String.Empty)
                throw new Exception("SharedDataBytesStorage not initialized.");

            return SharedDataBytesStorage.MetaData.FilePath.EndsWith("_jp.dat")
                ? DataJapanese
                : DataWorld;
        }
    }
}

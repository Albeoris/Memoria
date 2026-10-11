using Assets.Scripts.Common;
using Memoria.Data;
using Memoria.Prime;
using System;
using UnityEngine;

namespace Memoria.Debugging
{
    public static class BattleLaunch
    {
        private static GameLaunchOptions Options => Configuration.Debug.LaunchOptions;
        private static Boolean _started;

        public static Boolean IsPending => Configuration.Debug.StartBattle && !_started;

        public static void ApplyBattleSeed()
        {
            if (Configuration.Debug.StartBattle && Options.Seed.HasValue)
                UnityEngine.Random.seed = Options.Seed.Value;
        }

        public static void Start(Action<Action> hideTitle)
        {
            _started = true;
            FF9StateSystem.Serializer.Autoload(null, delegate (DataSerializerErrorCode error, Boolean success)
            {
                if (!success)
                {
                    Log.Error("[BattleLaunch] Cannot load the autosave: " + error);
                    Application.Quit();
                    return;
                }
                hideTitle(EnterBattle);
            });
        }

        private static void EnterBattle()
        {
            FF9StateSystem.Settings.UpdateSetting();
            Int32 battleId = Options.Battle.Value;
            if (!FF9StateSystem.Battle.mapName.ContainsKey(battleId))
                throw new ArgumentException("Unknown battle ID: " + battleId);
            Int32[] party = Options.Party;
            if (party != null)
            {
                for (Int32 i = 0; i < 4; i++)
                {
                    CharacterId character = i < party.Length ? (CharacterId)party[i] : CharacterId.NONE;
                    if (character != CharacterId.NONE && !FF9StateSystem.Common.FF9.player.ContainsKey(character))
                        throw new ArgumentException("Unknown party character ID: " + party[i]);
                    ff9play.FF9Play_SetParty(i, character);
                }
            }
            FF9StateSystem state = PersistenSingleton<FF9StateSystem>.Instance;
            state.prevMode = state.mode == 3 ? (Byte)3 : (Byte)1;
            state.mode = 2;
            FF9StateSystem.Common.FF9.btlMapNo = (Int16)battleId;
            FF9StateSystem.Common.FF9.btlSubMapNo = -1;
            FF9StateSystem.Common.FF9.steiner_state = 0;
            FF9StateSystem.Battle.battleMapIndex = battleId;
            FF9StateSystem.Battle.isDebug = false;
            FF9StateSystem.Battle.mappingBattleIDWithMapList = false;
            FF9StateSystem.Battle.isRandomEncounter = false;
            FF9StateSystem.Battle.isEncount = false;
            FF9StateSystem.Battle.IsPlayFieldBGMInCurrentBattle = false;
            FF9Snd.sndFuncPtr = FF9Snd.FF9BattleSoundDispatch;
            SoundLib.StopMovieMusic("FMV000", true);
            Log.Message("[BattleLaunch] ENCOUNT(_rushType: 0, _battle: " + battleId + "); autosave=true, seed=" + Options.Seed);
            SceneDirector.Replace("BattleMap", SceneTransition.FadeOutToBlack_FadeIn, true);
        }
    }
}

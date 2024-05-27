using System;
using System.IO;
using System.Collections.Generic;
using Memoria;
using Memoria.Data;
using Memoria.Assets;
using Memoria.Prime;
using Memoria.Prime.CSV;
using Memoria.Prime.Collections;
using UnityEngine;

namespace FF9
{
	public static class btl_mot
	{
		public static BattlePlayerCharacter.PlayerMotionStance[,] mot_stance;
		public static HashSet<BattlePlayerCharacter.PlayerMotionIndex> unstoppable_mot;
		public static Dictionary<CharacterSerialNumber, CharacterBattleParameter> BattleParameterList;

		static btl_mot()
		{
			// Battle motion IDs for player characters (BattlePlayerCharacter.PlayerMotionIndex) are sorted like these:
			//  0:  stand (MP_IDLE_NORMAL),
			//  1:  low HP (MP_IDLE_DYING),
			//  2:  hitted (MP_DAMAGE1),
			//  3:  hitted strongly (MP_DAMAGE2),
			//  4:  KO (MP_DISABLE),
			//  5:  low HP -> stand (MP_GET_UP_DYING),
			//  6:  KO -> low HP (MP_GET_UP_DISABLE),
			//  7:  stand -> low HP (MP_DOWN_DYING),
			//  8:  low HP -> KO (MP_DOWN_DISABLE),
			//  9:  ready (MP_IDLE_CMD),
			//  10: stand -> ready (MP_NORMAL_TO_CMD),
			//  11: low HP -> ready (MP_DYING_TO_CMD),
			//  12: stand -> defend (MP_IDLE_TO_DEF),
			//  13: defend (MP_DEFENCE),
			//  14: defend -> stand (MP_DEF_TO_IDLE),
			//  15: cover (MP_COVER),
			//  16: dodge (MP_AVOID),
			//  17: flee (MP_ESCAPE),
			//  18: victory (MP_WIN),
			//  19: stand victory (MP_WIN_LOOP),
			//  20: stand -> run (MP_SET),
			//  21: run (MP_RUN),
			//  22: run -> attack (MP_RUN_TO_ATTACK),
			//  23: attack (MP_ATTACK),
			//  24: jump back (MP_BACK),
			//  25: jump back -> stand (MP_ATK_TO_NORMAL),
			//  26: stand -> cast (MP_IDLE_TO_CHANT),
			//  27: cast (MP_CHANT),
			//  28: cast -> stand (MP_MAGIC),
			//  29: move forward (MP_STEP_FORWARD),
			//  30: move backward (MP_STEP_BACK),
			//  31: item (MP_ITEM1),
			//  32: ready -> stand (MP_CMD_TO_NORMAL),
			//  33: cast alternate (MP_SPECIAL1)

			// Starting and ending stances of each regular animation
			btl_mot.mot_stance = new BattlePlayerCharacter.PlayerMotionStance[35, 2]
			{
				{ BattlePlayerCharacter.PlayerMotionStance.NORMAL, BattlePlayerCharacter.PlayerMotionStance.NORMAL }, // 0:  MP_IDLE_NORMAL
				{ BattlePlayerCharacter.PlayerMotionStance.DYING, BattlePlayerCharacter.PlayerMotionStance.DYING }, // 1:  MP_IDLE_DYING
				{ BattlePlayerCharacter.PlayerMotionStance.NORMAL, BattlePlayerCharacter.PlayerMotionStance.NORMAL }, // 2:  MP_DAMAGE1
				{ BattlePlayerCharacter.PlayerMotionStance.NORMAL, BattlePlayerCharacter.PlayerMotionStance.DISABLE }, // 3:  MP_DAMAGE2
				{ BattlePlayerCharacter.PlayerMotionStance.DISABLE, BattlePlayerCharacter.PlayerMotionStance.DISABLE }, // 4:  MP_DISABLE
				{ BattlePlayerCharacter.PlayerMotionStance.DYING, BattlePlayerCharacter.PlayerMotionStance.NORMAL }, // 5:  MP_GET_UP_DYING
				{ BattlePlayerCharacter.PlayerMotionStance.DISABLE, BattlePlayerCharacter.PlayerMotionStance.DYING }, // 6:  MP_GET_UP_DISABLE
				{ BattlePlayerCharacter.PlayerMotionStance.NORMAL, BattlePlayerCharacter.PlayerMotionStance.DYING }, // 7:  MP_DOWN_DYING
				{ BattlePlayerCharacter.PlayerMotionStance.DYING, BattlePlayerCharacter.PlayerMotionStance.DISABLE }, // 8:  MP_DOWN_DISABLE
				{ BattlePlayerCharacter.PlayerMotionStance.CMD, BattlePlayerCharacter.PlayerMotionStance.CMD }, // 9:  MP_IDLE_CMD
				{ BattlePlayerCharacter.PlayerMotionStance.NORMAL, BattlePlayerCharacter.PlayerMotionStance.CMD }, // 10: MP_NORMAL_TO_CMD
				{ BattlePlayerCharacter.PlayerMotionStance.DYING, BattlePlayerCharacter.PlayerMotionStance.CMD }, // 11: MP_DYING_TO_CMD
				{ BattlePlayerCharacter.PlayerMotionStance.CMD, BattlePlayerCharacter.PlayerMotionStance.DEFEND }, // 12: MP_IDLE_TO_DEF
				{ BattlePlayerCharacter.PlayerMotionStance.DEFEND, BattlePlayerCharacter.PlayerMotionStance.DEFEND }, // 13: MP_DEFENCE
				{ BattlePlayerCharacter.PlayerMotionStance.DEFEND, BattlePlayerCharacter.PlayerMotionStance.CMD }, // 14: MP_DEF_TO_IDLE
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_ANY_IDLE }, // 15: MP_COVER
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_ANY_IDLE }, // 16: MP_AVOID
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_ANY_IDLE }, // 17: MP_ESCAPE
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT, BattlePlayerCharacter.PlayerMotionStance.WIN }, // 18: MP_WIN
				{ BattlePlayerCharacter.PlayerMotionStance.WIN, BattlePlayerCharacter.PlayerMotionStance.WIN }, // 19: MP_WIN_LOOP
				{ BattlePlayerCharacter.PlayerMotionStance.CMD, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT }, // 20: MP_SET
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT }, // 21: MP_RUN
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT }, // 22: MP_RUN_TO_ATTACK
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT }, // 23: MP_ATTACK
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT }, // 24: MP_BACK
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT, BattlePlayerCharacter.PlayerMotionStance.NORMAL }, // 25: MP_ATK_TO_NORMAL
				{ BattlePlayerCharacter.PlayerMotionStance.CMD, BattlePlayerCharacter.PlayerMotionStance.NORMAL }, // 26: MP_IDLE_TO_CHANT
				{ BattlePlayerCharacter.PlayerMotionStance.NORMAL, BattlePlayerCharacter.PlayerMotionStance.NORMAL }, // 27: MP_CHANT
				{ BattlePlayerCharacter.PlayerMotionStance.NORMAL, BattlePlayerCharacter.PlayerMotionStance.CMD }, // 28: MP_MAGIC
				{ BattlePlayerCharacter.PlayerMotionStance.CMD, BattlePlayerCharacter.PlayerMotionStance.CMD }, // 29: MP_STEP_FORWARD
				{ BattlePlayerCharacter.PlayerMotionStance.CMD, BattlePlayerCharacter.PlayerMotionStance.CMD }, // 30: MP_STEP_BACK
				{ BattlePlayerCharacter.PlayerMotionStance.CMD, BattlePlayerCharacter.PlayerMotionStance.CMD }, // 31: MP_ITEM1
				{ BattlePlayerCharacter.PlayerMotionStance.CMD, BattlePlayerCharacter.PlayerMotionStance.NORMAL }, // 32: MP_CMD_TO_NORMAL
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_UNKNOWN, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_UNKNOWN }, // 33: MP_SPECIAL1
				{ BattlePlayerCharacter.PlayerMotionStance.SPECIAL_UNKNOWN, BattlePlayerCharacter.PlayerMotionStance.SPECIAL_UNKNOWN }  // 34: MP_MAX
			};
			// List of animations that can't be interrupted by eg. the escape key
			unstoppable_mot = new HashSet<BattlePlayerCharacter.PlayerMotionIndex>
			{
				BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE1,
				BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2,
				BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DISABLE,
				BattlePlayerCharacter.PlayerMotionIndex.MP_GET_UP_DISABLE
			};
		}

		public static void Init()
		{
			BattleParameterList = LoadCharacterBattleParameters();
			foreach (CharacterBattleParameter param in BattleParameterList.Values)
				AssetManager.UpdateAutoAnimMapping(param.ModelId, param.AnimationId);
		}

		private static Dictionary<CharacterSerialNumber, CharacterBattleParameter> LoadCharacterBattleParameters()
		{
			try
			{
				String inputPath = DataResources.Characters.PureDirectory + DataResources.Characters.CharacterBattleParametersFile;
				Dictionary<CharacterSerialNumber, CharacterBattleParameter> result = new Dictionary<CharacterSerialNumber, CharacterBattleParameter>();
				foreach (CharacterBattleParameter[] btlParams in AssetManager.EnumerateCsvFromLowToHigh<CharacterBattleParameter>(inputPath))
					foreach (CharacterBattleParameter it in btlParams)
						result[it.Id] = it;
				if (result.Count == 0)
					throw new FileNotFoundException($"File with character battle parameters not found: [{DataResources.Characters.Directory + DataResources.Characters.CharacterBattleParametersFile}].", DataResources.Characters.Directory + DataResources.Characters.CharacterBattleParametersFile);
				for (Int32 i = 0; i < 19; i++)
					if (!result.ContainsKey((CharacterSerialNumber)i))
						throw new NotSupportedException($"You must define at least the 19 battle parameters, with IDs between 0 and 18.");
				return result;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "[btl_mot] Load character battle parameters failed.");
				UIManager.Input.ConfirmQuit();
				return null;
			}
		}

		public static void setMotion(BattleUnit btl, Byte index)
		{
			setMotion(btl.Data, btl.Data.mot[index]);
		}

		public static void setMotion(BTL_DATA btl, Byte index)
		{
			setMotion(btl, btl.mot[index]);
		}

		public static void setMotion(BTL_DATA btl, BattlePlayerCharacter.PlayerMotionIndex index)
		{
			setMotion(btl, btl.mot[(Int32)index]);
		}

		public static BattlePlayerCharacter.PlayerMotionIndex getMotion(BattleUnit btl)
		{
			return getMotion(btl.Data);
		}

		public static BattlePlayerCharacter.PlayerMotionIndex getMotion(BTL_DATA btl)
		{
			for (Int32 i = 0; i < btl.mot.Length; i++)
				if (btl.currentAnimationName == btl.mot[i])
					return (BattlePlayerCharacter.PlayerMotionIndex)i;
			return BattlePlayerCharacter.PlayerMotionIndex.MP_MAX;
		}

		public static void setMotion(BTL_DATA btl, String name)
		{
			btl.currentAnimationName = name;
			btl.animSpeed = 1f;
			btl.animFlag = 0;
			btl.animEndFrame = IsAnimationFrozen(btl);
		}

		public static Boolean checkMotion(BTL_DATA btl, Byte index)
		{
			if ((Int32)index > (Int32)btl.mot.Length)
				return false;
			String b = btl.mot[(Int32)index];
			return btl.currentAnimationName == b;
		}

		public static Boolean checkMotion(BTL_DATA btl, BattlePlayerCharacter.PlayerMotionIndex index)
		{
			if ((Int32)index > (Int32)btl.mot.Length)
				return false;
			String b = btl.mot[(Int32)index];
			return btl.currentAnimationName == b;
		}

		public static Boolean IsLoopingMotion(BattlePlayerCharacter.PlayerMotionIndex index)
		{
			return index == BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_NORMAL
				|| index == BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING
				|| index == BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD
				|| index == BattlePlayerCharacter.PlayerMotionIndex.MP_DEFENCE
				|| index == BattlePlayerCharacter.PlayerMotionIndex.MP_COVER
				|| index == BattlePlayerCharacter.PlayerMotionIndex.MP_AVOID
				|| index == BattlePlayerCharacter.PlayerMotionIndex.MP_ESCAPE
				|| index == BattlePlayerCharacter.PlayerMotionIndex.MP_WIN_LOOP
				|| index == BattlePlayerCharacter.PlayerMotionIndex.MP_RUN
				|| index == BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT;
		}

		public static void PlayAnim(BTL_DATA btl)
		{
			btl._smoothUpdatePlayingAnim = false;
			if (btl.currentAnimationName == null)
				return;
			GameObject gameObject = btl.gameObject;
			String currentAnimationName = btl.currentAnimationName;
			UInt16 animMaxFrame = GeoAnim.geoAnimGetNumFrames(btl, currentAnimationName);
			Boolean reverseSpeed = btl.animSpeed < 0f;
			Single animFrame = btl.evt.animFrame; // + (reverseSpeed ? -btl.animFrameFrac : btl.animFrameFrac);
			if (!gameObject.GetComponent<Animation>().IsPlaying(currentAnimationName))
			{
				if (gameObject.GetComponent<Animation>().GetClip(currentAnimationName) == null)
					return;
				gameObject.GetComponent<Animation>().Play(currentAnimationName);
			}
			AnimationState clipState = gameObject.GetComponent<Animation>()[currentAnimationName];
			Single time = animMaxFrame == 0 ? 0f : Mathf.Clamp(animFrame / animMaxFrame * clipState.length, 0f, clipState.length);
			Int32 animLoopFrame = GeoAnim.getAnimationLoopFrame(btl);
			clipState.speed = 0f;
			btl._smoothUpdatePlayingAnim = true;
			btl._smoothUpdateAnimTimePrevious = btl._smoothUpdateAnimTimeActual = time;

			if (btl.evt.animFrame == 0 && !reverseSpeed)
				btl._smoothUpdateAnimTimePrevious = 0;
			else if (btl.evt.animFrame == animLoopFrame && reverseSpeed)
				btl._smoothUpdateAnimTimePrevious = clipState.length;

			if (animMaxFrame != 0 && btl.bi.disappear == 0 && !btl_mot.IsAnimationFrozen(btl))
			{
				btl._smoothUpdateAnimTimeActual = Mathf.Clamp(time + btl.animSpeed / animMaxFrame * clipState.length, 0f, clipState.length);
			}
			clipState.time = time;

			/*if ((btl._smoothUpdateAnimTimePrevious == 0 || btl._smoothUpdateAnimTimePrevious == clipState.length) && btl.btl_id == 1)
			{
				Log.Message($"[PlayAnim] {clipState.name} length: {clipState.length} prev: {btl._smoothUpdateAnimTimePrevious} actual: {btl._smoothUpdateAnimTimeActual}");
			}*/

			gameObject.GetComponent<Animation>().Sample();
			// Couldn't see a difference:
			/*if (btl.evt.animFrame == animLoopFrame && !reverseSpeed)
			{
				// Try to smoothen standard animation chains
				if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_RUN))
				{
					gameObject.GetComponent<Animation>().CrossFade(btl.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_RUN_TO_ATTACK], 1f / FPSManager.GetMainLoopSpeed());
					btl._smoothUpdatePlayingAnim = false;
				}
				else if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_RUN_TO_ATTACK))
				{
					gameObject.GetComponent<Animation>().CrossFade(btl.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_ATTACK], 1f / FPSManager.GetMainLoopSpeed());
					btl._smoothUpdatePlayingAnim = false;
				}
				else if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_ATTACK))
				{
					gameObject.GetComponent<Animation>().CrossFade(btl.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_BACK], 1f / FPSManager.GetMainLoopSpeed());
					btl._smoothUpdatePlayingAnim = false;
				}
				else if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_BACK))
				{
					gameObject.GetComponent<Animation>().CrossFade(btl.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_ATK_TO_NORMAL], 1f / FPSManager.GetMainLoopSpeed());
					btl._smoothUpdatePlayingAnim = false;
				}
				else if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_ATK_TO_NORMAL))
				{
					gameObject.GetComponent<Animation>().CrossFade(btl.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_NORMAL], 1f / FPSManager.GetMainLoopSpeed());
					btl._smoothUpdatePlayingAnim = false;
				}
			}*/
		}

		public static Int32 GetDirection(BTL_DATA btl)
		{
			return (Int32)btl.evt.rotBattle.eulerAngles[1];
		}

		public static Int32 GetDirection(BattleUnit btl)
		{
			return (Int32)btl.Data.evt.rotBattle.eulerAngles[1];
		}

		public static void setSlavePos(BTL_DATA btl, ref Vector3 pos)
		{
			pos[0] = btl.gameObject.transform.GetChildByName("bone" + btl.tar_bone.ToString("D3")).position.x;
			pos[1] = 0f;
			pos[2] = btl.gameObject.transform.GetChildByName("bone" + btl.tar_bone.ToString("D3")).position.z;
		}

		public static void setBasePos(BTL_DATA btl)
		{
			btl.pos[0] = btl.base_pos[0];
			btl.pos[1] = btl.base_pos[1];
			btl.pos[2] = btl.base_pos[2];
		}

		public static Boolean IsAnimationFrozen(BTL_DATA btl)
		{
			return btl.bi.slave != 0
				|| btl_stat.CheckStatus(btl, BattleStatusConst.Immobilized)
				|| (btl.animFlag & EventEngine.afFreeze) != 0
				|| btl.bi.stop_anim != 0;
		}

		public static Boolean IsEnemyDyingAnimation(BTL_DATA btl, String anim)
		{
			return anim == btl.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE]
				|| anim == btl.mot[5]
				|| (btl_util.getEnemyPtr(btl).info.die_dmg != 0 && anim == btl.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2]);
		}

		public static void ToggleIdleAnimation(BTL_DATA btl, Boolean alternateOn)
		{
			if (btl.bi.player != 0 && !btl.is_monster_transform)
				return;
			if (alternateOn == (btl.bi.def_idle != 0))
				return;
			btl.bi.def_idle = (Byte)(alternateOn ? 1 : 0);
			if (!btl.is_monster_transform)
				return;
			btl.mot = alternateOn ? btl.monster_transform.motion_alternate : btl.monster_transform.motion_normal;
			btl_cmd.KillMainCommand(btl);
		}

		public static void DieSequence(BTL_DATA btl)
		{
			if (btl.die_seq != 0)
			{
				if (btl.bi.player == 0)
				{
					FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
					ENEMY enemyPtr = btl_util.getEnemyPtr(btl);
					if (btl.bi.slave != 0 && btl.die_seq < 5)
						btl.die_seq = 5;
					switch (btl.die_seq)
					{
						case 1:
							if (btl_util.IsBtlUsingCommandMotion(btl, true) || btl_util.IsBtlBusy(btl, btl_util.BusyMode.QUEUED_CASTER))
								return;
							//btl_mot.setMotion(btl, (Byte)(4 + btl.bi.def_idle));
							//btl.evt.animFrame = 0;
							btl.die_seq++;
							btl_util.SetEnemyDieSound(btl, enemyPtr.et.die_snd_no);
							break;
						case 2:
							//if ((UInt16)btl.evt.animFrame >= GeoAnim.getAnimationLoopFrame(btl))
							if (btl.animEndFrame && btl_mot.IsEnemyDyingAnimation(btl, btl.endedAnimationName))
							{
								btl.evt.animFrame = (Byte)GeoAnim.geoAnimGetNumFrames(btl);
								btl.bi.stop_anim = 1;
								btl.die_seq++;
							}
							break;
						case 3:
							if (Configuration.Battle.Speed >= 3 && (btl_util.IsBtlUsingCommand(btl) || (enemyPtr.info.die_fade_rate >= 32 && btl_util.IsBtlTargetOfCommand(btl))))
								return;
							btl_util.SetBattleSfx(btl, 121, 127);
							btl_util.SetBattleSfx(btl, 120, 127);
							btl.die_seq++;
							btl_util.SetEnemyFadeToPacket(btl, enemyPtr.info.die_fade_rate);
							if (enemyPtr.info.die_fade_rate == 0)
								btl.die_seq = 5;
							else
								enemyPtr.info.die_fade_rate -= 2;
							break;
						case 4:
							btl_util.SetEnemyFadeToPacket(btl, enemyPtr.info.die_fade_rate);
							if (enemyPtr.info.die_fade_rate == 0)
								btl.die_seq = 5;
							else if (Configuration.Battle.Speed < 3 || enemyPtr.info.die_fade_rate > 2 || !btl_util.IsBtlBusy(btl, btl_util.BusyMode.ANY_CURRENT))
								enemyPtr.info.die_fade_rate -= 2;
							break;
						case 5:
							if (!btl_util.IsBtlBusy(btl, btl_util.BusyMode.ANY_CURRENT))
							{
								FF9StateGlobal ff = FF9StateSystem.Common.FF9;
								for (Int32 category = 0; category < 8; category++)
									if (btl_util.CheckEnemyCategory(btl, (Byte)(1 << category)) && ff.categoryKillCount[category] < 9999)
										ff.categoryKillCount[category]++;
								if (btl.dms_geo_id >= 0)
									ff.modelKillCount[btl.dms_geo_id]++;
								if (ff.btl_result != 4)
									btl_sys.SetBonus(enemyPtr);
								btl.die_seq++;
								if (ff9Battle.btl_phase != 5 || (ff9Battle.btl_seq != 3 && ff9Battle.btl_seq != 2))
									btl_sys.CheckBattlePhase(btl);
								btl_sys.DelCharacter(btl);
							}
							break;
					}
				}
				else
				{
					if (btl.die_seq < 4 && (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DISABLE) || btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE)))
					{
						if (btl.is_monster_transform)
							btl_util.SetBattleSfx(btl, btl.monster_transform.death_sound, 127);
						btl.die_seq = 4;
					}
					else if ((btl.die_seq == 4 && btl.animEndFrame) || btl.die_seq == 5)
					{
						if (btl_mot.DecidePlayerDieSequence(btl))
							btl_sys.CheckBattlePhase(btl);
					}
					// Old version
					//switch (btl.die_seq)
					//{
					//	case 1:
					//		if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD))
					//		{
					//			btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_CMD_TO_NORMAL);
					//			btl.evt.animFrame = 0;
					//		}
					//		else if (!btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_CMD_TO_NORMAL) || (UInt16)btl.evt.animFrame >= GeoAnim.geoAnimGetNumFrames(btl))
					//		{
					//			btl.die_seq = 2;
					//			if (btl.bi.def_idle == 1)
					//			{
					//				btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DISABLE);
					//				btl.die_seq = 4;
					//				if (btl.is_monster_transform)
					//					btl_util.SetBattleSfx(btl, btl.monster_transform.death_sound, 127);
					//			}
					//			else
					//			{
					//				btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DYING);
					//				btl.die_seq = 3;
					//			}
					//			btl.evt.animFrame = 0;
					//		}
					//		break;
					//	case 2:
					//		if (btl.bi.def_idle == 1)
					//		{
					//			btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DISABLE);
					//			btl.die_seq = 4;
					//			if (btl.is_monster_transform)
					//				btl_util.SetBattleSfx(btl, btl.monster_transform.death_sound, 127);
					//		}
					//		else
					//		{
					//			btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DYING);
					//			btl.die_seq = 3;
					//		}
					//		btl.evt.animFrame = 0;
					//		break;
					//	case 3:
					//		if ((UInt16)btl.evt.animFrame >= GeoAnim.geoAnimGetNumFrames(btl))
					//		{
					//			btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DISABLE);
					//			btl.evt.animFrame = 0;
					//			if (btl.is_monster_transform)
					//				btl_util.SetBattleSfx(btl, btl.monster_transform.death_sound, 127);
					//			btl.die_seq++;
					//		}
					//		break;
					//	case 4:
					//		if ((UInt16)btl.evt.animFrame >= GeoAnim.geoAnimGetNumFrames(btl))
					//			if (btl_mot.DecidePlayerDieSequence(btl))
					//				btl_sys.CheckBattlePhase(btl);
					//		break;
					//	case 5:
					//		if (btl_mot.DecidePlayerDieSequence(btl))
					//			btl_sys.CheckBattlePhase(btl);
					//		break;
					//}
				}
			}
		}

		public static Boolean DecidePlayerDieSequence(BTL_DATA btl)
		{
			Boolean cancelMonsterTransform = btl.is_monster_transform && btl.monster_transform.cancel_on_death;
			if (cancelMonsterTransform)
				new BattleUnit(btl).ReleaseChangeToMonster();
			GeoTexAnim.geoTexAnimStop(btl.texanimptr, 2);
			GeoTexAnim.geoTexAnimPlayOnce(btl.texanimptr, 0);
			if (btl.bi.player != 0)
			{
				GeoTexAnim.geoTexAnimStop(btl.tranceTexanimptr, 2);
				GeoTexAnim.geoTexAnimPlayOnce(btl.tranceTexanimptr, 0);
			}
			if (btl_stat.CheckStatus(btl, BattleStatus.AutoLife))
			{
				//btl.die_seq = 7;
				//btl_cmd.SetCommand(btl.cmd[5], BattleCommandId.SysReraise, 0u, btl.btl_id, 0u);
				btl_stat.RemoveStatus(btl, BattleStatus.AutoLife);
				btl.cur.hp = 1;
				btl_stat.RemoveStatus(btl, BattleStatus.Death);
				// Auto-life has triggered
				BattleVoice.TriggerOnStatusChange(btl, "Used", BattleStatus.AutoLife);
				FF9StateSystem.Settings.SetHPFull();
				if (!cancelMonsterTransform)
					btl_mot.SetDefaultIdle(btl);
				return false;
			}
			btl.die_seq = 6;
			if (btl.is_monster_transform && !btl.monster_transform.cancel_on_death)
			{
				btl.evt.animFrame = (Byte)GeoAnim.geoAnimGetNumFrames(btl);
				btl.bi.stop_anim = 1;
			}
			return true;
		}

		public static BattlePlayerCharacter.PlayerMotionStance StartingMotionStance(BattlePlayerCharacter.PlayerMotionIndex motion)
		{
			return btl_mot.mot_stance[(Int32)motion, 0];
		}

		public static BattlePlayerCharacter.PlayerMotionStance EndingMotionStance(BattlePlayerCharacter.PlayerMotionIndex motion)
		{
			return btl_mot.mot_stance[(Int32)motion, 1];
		}

		public static BattlePlayerCharacter.PlayerMotionIndex StanceTransition(BattlePlayerCharacter.PlayerMotionStance from, BattlePlayerCharacter.PlayerMotionStance to)
		{
			if (from == BattlePlayerCharacter.PlayerMotionStance.NORMAL)
			{
				if (to == BattlePlayerCharacter.PlayerMotionStance.DYING || to == BattlePlayerCharacter.PlayerMotionStance.DISABLE)
					return BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DYING;
				if (to == BattlePlayerCharacter.PlayerMotionStance.CMD || to == BattlePlayerCharacter.PlayerMotionStance.DEFEND)
					return BattlePlayerCharacter.PlayerMotionIndex.MP_NORMAL_TO_CMD;
			}
			if (from == BattlePlayerCharacter.PlayerMotionStance.DYING)
			{
				if (to == BattlePlayerCharacter.PlayerMotionStance.NORMAL)
					return BattlePlayerCharacter.PlayerMotionIndex.MP_GET_UP_DYING;
				if (to == BattlePlayerCharacter.PlayerMotionStance.DISABLE)
					return BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DISABLE;
				if (to == BattlePlayerCharacter.PlayerMotionStance.CMD || to == BattlePlayerCharacter.PlayerMotionStance.DEFEND)
					return BattlePlayerCharacter.PlayerMotionIndex.MP_DYING_TO_CMD;
			}
			if (from == BattlePlayerCharacter.PlayerMotionStance.DISABLE)
				return BattlePlayerCharacter.PlayerMotionIndex.MP_GET_UP_DISABLE;
			if (from == BattlePlayerCharacter.PlayerMotionStance.CMD)
			{
				if (to == BattlePlayerCharacter.PlayerMotionStance.NORMAL || to == BattlePlayerCharacter.PlayerMotionStance.DYING || to == BattlePlayerCharacter.PlayerMotionStance.DISABLE)
					return BattlePlayerCharacter.PlayerMotionIndex.MP_CMD_TO_NORMAL;
				if (to == BattlePlayerCharacter.PlayerMotionStance.DEFEND)
					return BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_TO_DEF;
			}
			if (from == BattlePlayerCharacter.PlayerMotionStance.DEFEND)
				return BattlePlayerCharacter.PlayerMotionIndex.MP_DEF_TO_IDLE;
			if (to == BattlePlayerCharacter.PlayerMotionStance.WIN)
				return BattlePlayerCharacter.PlayerMotionIndex.MP_WIN;
			return BattlePlayerCharacter.PlayerMotionIndex.MP_MAX;
		}

		public static void SetDefaultIdle(BTL_DATA btl, Boolean isEndOfAnim = false)
		{
			BattlePlayerCharacter.PlayerMotionIndex targetAnim = (BattlePlayerCharacter.PlayerMotionIndex)btl.bi.def_idle;
			BattlePlayerCharacter.PlayerMotionIndex currentAnim = btl_mot.getMotion(btl);
			if (btl_stat.CheckStatus(btl, BattleStatusConst.Immobilized))
			{
				if (btl.bi.player != 0 && !btl.is_monster_transform && btl_stat.CheckStatus(btl, BattleStatus.Venom & BattleStatusConst.IdleDying))
				{
					btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING);
					btl.evt.animFrame = 0;
				}
				return;
			}
			Boolean useCmdMotion = btl_util.IsBtlUsingCommandMotion(btl, false, out CMD_DATA cmdUsed);
			if (btl.bi.player == 0 || (btl.is_monster_transform && (btl.die_seq != 0 || (useCmdMotion && (cmdUsed.cmd_no == btl.monster_transform.new_command || cmdUsed.cmd_no == BattleCommandId.Attack)))))
			{
				if (useCmdMotion && currentAnim != BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2 && currentAnim != BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE1)
					targetAnim = currentAnim;
				else if ((btl.die_seq == 1 || btl.die_seq == 2) && btl.bi.player == 0 && btl_util.getEnemyPtr(btl).info.die_dmg != 0)
					targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2;
				else if ((btl.die_seq == 1 || btl.die_seq == 2) && btl.is_monster_transform)
					targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DISABLE;
				else if (btl.die_seq == 1 || btl.die_seq == 2)
					targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE + btl.bi.def_idle;
				else if (btl.die_seq != 0)
					targetAnim = currentAnim;
				if (currentAnim == targetAnim)
				{
					if (isEndOfAnim)
						btl.evt.animFrame = 0;
					return;
				}
				btl_mot.setMotion(btl, targetAnim);
				btl.evt.animFrame = 0;
				return;
			}
			if (FF9StateSystem.Battle.FF9Battle.btl_phase == 6 && !btl_stat.CheckStatus(btl, BattleStatusConst.BattleEndFull) && FF9StateSystem.Battle.FF9Battle.btl_scene.Info.WinPose && btl_util.getPlayerPtr(btl).info.win_pose != 0)
				targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_WIN_LOOP;
			else if (btl.bi.cover != 0)
				targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_COVER;
			else if (useCmdMotion && isEndOfAnim)
				targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD;
			else if (useCmdMotion)
				return;
			else if (!isEndOfAnim && unstoppable_mot.Contains(currentAnim))
				return;
			else if ((FF9StateSystem.Battle.FF9Battle.cmd_status & 1) != 0 && !btl_stat.CheckStatus(btl, BattleStatusConst.CannotEscape))
				targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_ESCAPE;
			else if (Status.checkCurStat(btl, BattleStatus.Death))
				targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE;
			else if (btl.bi.dodge != 0)
				targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_AVOID;
			else if (btl_stat.CheckStatus(btl, BattleStatus.Defend))
				targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_DEFENCE;
			else if (FF9StateSystem.Battle.FF9Battle.btl_escape_key != 0 && !btl_stat.CheckStatus(btl, BattleStatusConst.CannotEscape))
				targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_ESCAPE;
			else if (btl.bi.cmd_idle == 1)
				targetAnim = BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD;
			if (currentAnim == targetAnim)
			{
				if (isEndOfAnim)
					btl.evt.animFrame = 0;
				return;
			}
			BattlePlayerCharacter.PlayerMotionStance previousStance = btl_mot.EndingMotionStance(currentAnim);
			BattlePlayerCharacter.PlayerMotionStance nextStance = btl_mot.StartingMotionStance(targetAnim);
			if (previousStance == BattlePlayerCharacter.PlayerMotionStance.SPECIAL_ANY_IDLE)
			{
				if (nextStance == BattlePlayerCharacter.PlayerMotionStance.NORMAL || nextStance == BattlePlayerCharacter.PlayerMotionStance.CMD || nextStance == BattlePlayerCharacter.PlayerMotionStance.DYING || nextStance == BattlePlayerCharacter.PlayerMotionStance.DEFEND)
				{
					btl_mot.setMotion(btl, targetAnim);
					btl.evt.animFrame = 0;
					return;
				}
				if (btl_stat.CheckStatus(btl, BattleStatus.Defend))
					previousStance = BattlePlayerCharacter.PlayerMotionStance.DEFEND;
				else if (useCmdMotion || btl.bi.cmd_idle == 1)
					previousStance = BattlePlayerCharacter.PlayerMotionStance.CMD;
				else if (btl.bi.def_idle == 1)
					previousStance = BattlePlayerCharacter.PlayerMotionStance.DYING;
				else
					previousStance = BattlePlayerCharacter.PlayerMotionStance.NORMAL;
			}
			if (previousStance == nextStance || previousStance == BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT || previousStance == BattlePlayerCharacter.PlayerMotionStance.SPECIAL_UNKNOWN || nextStance == BattlePlayerCharacter.PlayerMotionStance.SPECIAL_INDIFFERENT)
			{
				btl_mot.setMotion(btl, targetAnim);
				btl.evt.animFrame = 0;
				return;
			}
			if (btl.is_monster_transform && ((previousStance == BattlePlayerCharacter.PlayerMotionStance.NORMAL && nextStance == BattlePlayerCharacter.PlayerMotionStance.DYING) || (previousStance == BattlePlayerCharacter.PlayerMotionStance.DYING && nextStance == BattlePlayerCharacter.PlayerMotionStance.NORMAL)))
			{
				btl_mot.setMotion(btl, targetAnim);
				btl.evt.animFrame = 0;
				return;
			}
			BattlePlayerCharacter.PlayerMotionIndex transition = btl_mot.StanceTransition(previousStance, nextStance);
			if (transition == BattlePlayerCharacter.PlayerMotionIndex.MP_MAX)
			{
				btl_mot.setMotion(btl, targetAnim);
				btl.evt.animFrame = 0;
			}
			else
			{
				btl_mot.setMotion(btl, transition);
				btl.evt.animFrame = 0;
			}
		}

		public static Boolean SetDefaultIdleOld(BTL_DATA btl) // Not used anymore
		{
			FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
			CMD_DATA cur_cmd = ff9Battle.cur_cmd;
			if (Status.checkCurStat(btl, BattleStatus.Death))
			{
				if (btl.bi.player != 0 && btl.bi.dmg_mot_f == 0 && cur_cmd != null && btl != cur_cmd.regist && btl.die_seq == 0 && !btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE) && !btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD))
					btl_mot.setMotion(btl, btl.bi.def_idle);
				return false;
			}
			if (cur_cmd != null && (btl_stat.CheckStatus(btl, BattleStatusConst.FrozenAnimation) || btl.bi.dmg_mot_f != 0 || (btl_util.getSerialNumber(btl) == CharacterSerialNumber.VIVI && cur_cmd.cmd_no == BattleCommandId.MagicSword)))
				return false;
			if (cur_cmd != null && btl == cur_cmd.regist && (cur_cmd.cmd_no < BattleCommandId.EnemyReaction || cur_cmd.cmd_no > BattleCommandId.SysReraise))
			{
				//if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_NORMAL_TO_CMD) || btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DYING_TO_CMD))
				if (btl.bi.player != 0)
					btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD);
				return false;
			}
			if (btl.bi.player != 0)
			{
				if (btl.bi.cover == 0 && btl.bi.dodge == 0)
				{
					if ((ff9Battle.btl_escape_key != 0 || (ff9Battle.cmd_status & 1) != 0) && !btl_stat.CheckStatus(btl, BattleStatusConst.CannotEscape))
						btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_ESCAPE);
					else if (btl_stat.CheckStatus(btl, BattleStatus.Defend))
						btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DEFENCE);
					else if (btl.bi.cmd_idle != 0)
					{
						if (btl_mot.checkMotion(btl, btl.bi.def_idle))
							btl_mot.setMotion(btl, (Byte)(10 + btl.bi.def_idle));
						else
							btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD);
					}
					else if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_NORMAL) && btl_stat.CheckStatus(btl, BattleStatusConst.IdleDying))
					{
						global::Debug.LogWarning(btl.gameObject.name + " Dead");
						btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DYING);
					}
					else if ((btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING) || btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_GET_UP_DISABLE)) && !btl_stat.CheckStatus(btl, BattleStatusConst.IdleDying))
						btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_GET_UP_DYING);
					else
						btl_mot.setMotion(btl, btl.bi.def_idle);
				}
			}
			else
			{
				btl_mot.setMotion(btl, btl.bi.def_idle);
			}
			btl.evt.animFrame = 0;
			return true;
		}

		public static void SetDamageMotion(BattleUnit btl, CMD_DATA cmd)
		{
			if (btl_util.IsBtlUsingCommandMotion(btl.Data, true))
			{
				if (!btl.IsPlayer && btl.Data.cur.hp == 0 && btl.Enemy.Data.info.die_atk == 0)
					btl_util.SetEnemyDieSound(btl.Data, btl.EnemyType.die_snd_no);
				return;
			}
			// TODO: have some system to handle critical damage and evade movements (in SBattleCalculator.CalcResult) for potential other movement conflicts
			if ((btl.Data.fig_info & Param.FIG_INFO_HP_CRITICAL) != 0)
				btl.Data.pos[2] += (((btl.Data.rot.eulerAngles[1] + 90) % 360) < 180 ? 400 : -400) >> 1;
			btl.Data.bi.dmg_mot_f = 1;

			if (btl.IsPlayer)
			{
				if (cmd != null && (cmd.AbilityType & 129) == 129 && !btl.IsMonsterTransform)
				{
					btl_mot.setMotion(btl.Data, BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2);
					if (btl.Data.cur.hp == 0)
						btl.Data.die_seq = 1;
				}
				else
				{
					btl_mot.setMotion(btl.Data, BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE1);
				}
				btl.Data.evt.animFrame = 0;
			}
			else if (btl.Data.cur.hp == 0 && btl.Enemy.Data.info.die_dmg != 0 && btl.Enemy.Data.info.die_atk == 0)
			{
				btl_mot.setMotion(btl.Data, BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2);
				btl.Data.evt.animFrame = 0;
				btl.Data.die_seq = 1;
			}
			else
			{
				if (btl.IsSlave)
					btl = btl_util.GetMasterEnemyBtlPtr();
				btl_mot.setMotion(btl.Data, BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE1 + btl.Data.bi.def_idle);
				btl.Data.evt.animFrame = 0;
			}
		}

		public static void EndCommandMotion(CMD_DATA cmd)
		{
			// Ensure the dying sequence after a command (including BattleCommandId.EnemyDying)
			if (cmd.regist != null && cmd.regist.die_seq == 6)
				btl_sys.CheckBattlePhase(cmd.regist);
			if (cmd.cmd_no > BattleCommandId.EnemyReaction && cmd.cmd_no < BattleCommandId.ScriptCounter1)
				return;
			if (cmd.regist != null && Status.checkCurStat(cmd.regist, BattleStatus.Death) && cmd.regist.die_seq == 0)
			{
				if (cmd.regist.bi.player == 0 && btl_util.getEnemyPtr(cmd.regist).info.die_atk != 0 && cmd.cmd_no != BattleCommandId.EnemyDying)
					return;
				if (cmd.regist.bi.player == 0 && btl_mot.IsEnemyDyingAnimation(cmd.regist, cmd.regist.currentAnimationName))
					cmd.regist.die_seq = 2;
				else
					cmd.regist.die_seq = 1;
			}
		}

		public static Boolean ControlDamageMotion(CMD_DATA cmd) // Unused anymore
		{
			Boolean result = true;
			if (cmd.info.cover == 0)
			{
				for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
				{
					if (next.bi.dmg_mot_f != 0)
					{
						result = false;
						if (next.animEndFrame)
						{
							next.pos[2] = next.base_pos[2];
							if (next.bi.player != 0)
								btl_mot.PlayerDamageMotion(next);
							else
								btl_mot.EnemyDamageMotion(next);
							btl_mot.SetDefaultIdle(next);
						}
					}
				}
			}
			return result;
		}

		private static void PlayerDamageMotion(BTL_DATA btl) // Unused anymore
		{
			if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE1))
			{
				if (Status.checkCurStat(btl, BattleStatus.Death))
				{
					btl.die_seq = 1;
					btl.bi.dmg_mot_f = 0;
				}
				else if (btl.bi.cmd_idle != 0)
				{
					btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_NORMAL_TO_CMD);
				}
				else
				{
					btl.bi.dmg_mot_f = 0;
				}
			}
			else if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2))
			{
				if (Status.checkCurStat(btl, BattleStatus.Death))
				{
					btl.die_seq = 5;
					btl.bi.dmg_mot_f = 0;
					btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE);
				}
				else
				{
					btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_GET_UP_DISABLE);
				}
			}
			else if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_GET_UP_DISABLE))
			{
				if (btl.bi.cmd_idle != 0)
					btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DYING_TO_CMD);
				else if (btl_stat.CheckStatus(btl, BattleStatusConst.IdleDying))
					btl.bi.dmg_mot_f = 0;
				else
					btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_GET_UP_DYING);
			}
			else
			{
				btl.bi.dmg_mot_f = 0;
			}
			btl.evt.animFrame = 0;
		}

		public static void EnemyDamageMotion(BTL_DATA btl) // Unused anymore
		{
			btl.bi.dmg_mot_f = 0;
			if (Status.checkCurStat(btl, BattleStatus.Death))
			{
				if (btl_mot.checkMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2) && btl_util.getEnemyPtr(btl).info.die_dmg != 0)
				{
					btl_mot.setMotion(btl, BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE);
					btl.evt.animFrame--;
					btl.bi.stop_anim = 1;
					btl.die_seq = 3;
				}
				else if (btl_util.getEnemyPtr(btl).info.die_atk == 0 && btl.bi.death_f == 0)
				{
					btl.die_seq = 1;
				}
				else
				{
					btl_mot.setMotion(btl, btl.bi.def_idle);
				}
			}
		}

		public static void HideMesh(BTL_DATA btl, UInt16 mesh, Boolean isBanish = false)
		{
			String path = (btl.dms_geo_id == -1) ? String.Empty : FF9BattleDB.GEO.GetValue((Int32)btl.dms_geo_id);
			if (ModelFactory.IsUseAsEnemyCharacter(path) && isBanish)
				mesh = UInt16.MaxValue;
			for (Int32 i = 0; i < 16; i++)
				if (((Int32)mesh & 1 << i) != 0)
					geo.geoMeshHide(btl, i);
			if (mesh == 65535)
				for (Int32 i = 0; i < btl.weaponMeshCount; i++)
					geo.geoWeaponMeshHide(btl, i);
		}

		public static void ShowMesh(BTL_DATA btl, UInt16 mesh, Boolean isBanish = false)
		{
			String path = (btl.dms_geo_id == -1) ? String.Empty : FF9BattleDB.GEO.GetValue((Int32)btl.dms_geo_id);
			if (ModelFactory.IsUseAsEnemyCharacter(path) && isBanish)
				mesh = UInt16.MaxValue;
			if (btl.bi.player == 0)
				btl.flags &= (UInt16)~geo.GEO_FLAGS_RENDER;
			for (Int32 i = 0; i < 16; i++)
				if (((Int32)mesh & 1 << i) != 0)
					geo.geoMeshShow(btl, i);
			if (mesh == 65535)
				for (Int32 i = 0; i < btl.weaponMeshCount; i++)
					geo.geoWeaponMeshShow(btl, i);
		}

		public static void HideWeapon(BTL_DATA btl)
		{
			for (Int32 i = 0; i < btl.weaponMeshCount; i++)
				geo.geoWeaponMeshHide(btl, i);
		}

		public static void ShowWeapon(BTL_DATA btl)
		{
			for (Int32 i = 0; i < btl.weaponMeshCount; i++)
				geo.geoWeaponMeshShow(btl, i);
		}

		public static void SetPlayerDefMotion(BTL_DATA btl, CharacterSerialNumber serial_no, UInt32 cnt)
		{
			String[] animSource = btl_mot.BattleParameterList[serial_no].AnimationId;
			String[] animDest = FF9StateSystem.Battle.FF9Battle.p_mot[cnt];
			for (Int32 i = 0; i < 34; i++)
				animDest[i] = animSource[i];
			btl.mot = animDest;
			btl.animFlag = 0;
			btl.animSpeed = 1f;
			btl.animFrameFrac = 0f;
			btl.animEndFrame = false;
		}
	}
}

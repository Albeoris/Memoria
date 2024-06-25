using System;
using System.Collections.Generic;
using Memoria;
using Memoria.Data;
using Memoria.Database;
using Memoria.Scripts;
using UnityEngine;

namespace FF9
{
	public class btl_util
	{
		public static List<BTL_DATA> findAllBtlData(UInt16 id)
		{
			List<BTL_DATA> result = new List<BTL_DATA>();
			for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
				if ((next.btl_id & id) != 0)
					result.Add(next);
			return result;
		}

		public static BTL_DATA getBattlePtr(PLAYER player)
		{
			for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
				if (btl_util.getPlayerPtr(btl) == player)
					return btl;
			return null;
		}

		public static BTL_DATA getBattlePtr(ENEMY enemy)
		{
			for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
				if (btl_util.getEnemyPtr(btl) == enemy)
					return btl;
			return null;
		}

		public static PLAYER getPlayerPtr(BTL_DATA btl)
		{
			return btl.bi.player != 0 ? FF9StateSystem.Common.FF9.player[(CharacterId)btl.bi.slot_no] : null;
		}

		public static ENEMY getEnemyPtr(BTL_DATA btl)
		{
			return FF9StateSystem.Battle.FF9Battle.enemy[btl.bi.slot_no];
		}

		public static ENEMY_TYPE getEnemyTypePtr(BTL_DATA btl)
		{
			return FF9StateSystem.Battle.FF9Battle.enemy[btl.bi.slot_no].et;
		}

		public static RegularItem getWeaponNumber(BTL_DATA btl)
		{
			return btl.bi.player != 0 ? FF9StateSystem.Common.FF9.player[(CharacterId)btl.bi.slot_no].equip[0] : RegularItem.NoItem;
		}

		public static CharacterSerialNumber getSerialNumber(BTL_DATA btl)
		{
			return btl.bi.player != 0 ? FF9StateSystem.Common.FF9.player[(CharacterId)btl.bi.slot_no].info.serial_no : CharacterSerialNumber.NONE;
		}

		public static CMD_DATA getCurCmdPtr()
		{
			return FF9StateSystem.Battle.FF9Battle.cur_cmd;
		}

		public static Boolean IsBtlUsingCommand(BTL_DATA btl, out CMD_DATA cmdUsed)
		{
			foreach (CMD_DATA cmd in FF9StateSystem.Battle.FF9Battle.cur_cmd_list)
				if (cmd.regist == btl)
				{
					cmdUsed = cmd;
					return true;
				}
			cmdUsed = null;
			return false;
		}

		public static Boolean IsBtlTargetOfCommand(BTL_DATA btl, List<CMD_DATA> cmdList = null)
		{
			foreach (CMD_DATA cmd in FF9StateSystem.Battle.FF9Battle.cur_cmd_list)
				if ((cmd.tar_id & btl.btl_id) != 0)
				{
					if (cmdList != null)
						cmdList.Add(cmd);
					else
						return true;
				}
			return cmdList != null && cmdList.Count > 0;
		}

		public static Boolean IsBtlUsingCommand(BTL_DATA btl)
		{
			return IsBtlUsingCommand(btl, out _);
		}

		public static Boolean IsBtlUsingCommandMotion(BTL_DATA btl, Boolean includeSysCmd, out CMD_DATA cmdUsed)
		{
			cmdUsed = null;
			foreach (CMD_DATA cmd in FF9StateSystem.Battle.FF9Battle.cur_cmd_list)
				if (cmd.regist == btl && cmd.info.cmd_motion && (includeSysCmd || cmd.cmd_no <= BattleCommandId.EnemyReaction || cmd.cmd_no >= BattleCommandId.ScriptCounter1))
				{
					cmdUsed = cmd;
					return true;
				}
			return false;
		}

		public static Boolean IsBtlUsingCommandMotion(BTL_DATA btl, Boolean includeSysCmd = false)
		{
			return IsBtlUsingCommandMotion(btl, includeSysCmd, out _);
		}

		public static Boolean IsBtlBusy(BTL_DATA btl, BusyMode mode)
		{
			if ((mode & BusyMode.ANY_CURRENT) != 0)
				foreach (CMD_DATA cmd in FF9StateSystem.Battle.FF9Battle.cur_cmd_list)
				{
					if ((mode & BusyMode.CASTER) != 0 && cmd.regist == btl)
						return true;
					if ((mode & BusyMode.TARGET) != 0 && ((cmd.tar_id & btl.btl_id) != 0 || (btl_cmd.MergeReflecTargetID(cmd.reflec) & btl.btl_id) != 0))
						return true;
					if ((mode & BusyMode.MAGIC_CASTER) != 0 && cmd.cmd_no == BattleCommandId.MagicSword && btl.bi.player != 0 && btl.bi.slot_no == 1)
						return true;
				}
			if ((mode & BusyMode.ANY_QUEUED) != 0)
				for (CMD_DATA cmd = FF9StateSystem.Battle.FF9Battle.cmd_queue.next; cmd != null; cmd = cmd.next)
				{
					if ((mode & BusyMode.QUEUED_CASTER) != 0 && cmd.regist == btl)
						return true;
					if ((mode & BusyMode.QUEUED_TARGET) != 0 && (cmd.tar_id & btl.btl_id) != 0)
						return true;
					if ((mode & BusyMode.QUEUED_MAGIC_CASTER) != 0 && cmd.cmd_no == BattleCommandId.MagicSword && btl.bi.player != 0 && btl.bi.slot_no == 1)
						return true;
				}
			return false;
		}

		public static BattleUnit GetMasterEnemyBtlPtr()
	    {
	        foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
	            if (!unit.IsPlayer && !unit.IsSlave && unit.Enemy.Data.info.multiple != 0)
	                return unit;

	        return null;
	    }

	    public static UInt32 SumOfTarget(UInt32 player)
		{
			UInt32 count = 0u;
			for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
				if ((UInt32)next.bi.player == player && next.bi.target != 0 && !Status.checkCurStat(next, BattleStatus.Death))
					count++;
			return count;
		}

		public static UInt32 SumOfTarget(CMD_DATA cmd)
		{
			UInt32 count = 0u;
			for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
				if (next.bi.target != 0 && (next.btl_id & cmd.tar_id) != 0)
					count++;
			return count;
		}

		public static UInt16 GetRandomBtlID(UInt32 player, Boolean allowDead = false)
		{
			UInt16[] array = new UInt16[4];
			UInt16 btlCount = 0;
			if (player != 0u)
				player = 1u;
			for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
				if ((UInt32)next.bi.player == player && (!Status.checkCurStat(next, BattleStatus.Death) || allowDead) && next.bi.target != 0)
					array[btlCount++] = next.btl_id;
			if (btlCount == 0)
				return 0;
			return array[Comn.random8() % (Int32)btlCount];
		}

		public static Boolean ManageBattleSong(FF9StateGlobal sys, Int32 ticks, Int32 song_id)
		{
			if ((sys.btl_flag & 16) == 0)
			{
				btlsnd.ff9btlsnd_song_vol_intplall(ticks, 0);
				sys.btl_flag |= 16;
			}
			if ((sys.btl_flag & 2) == 0)
			{
				if ((FF9StateSystem.Battle.FF9Battle.player_load_fade += 4) < ticks)
					return false;
				AllSoundDispatchPlayer player = SoundLib.GetAllSoundDispatchPlayer();
				Int32 currentMusicId = FF9Snd.GetCurrentMusicId();
				Int32 suspendedMusicId = player.GetSuspendSongID();
				if (currentMusicId >= 0 && suspendedMusicId < 0 && currentMusicId != 0 && currentMusicId != 35 && currentMusicId != 61)
				{
					// Don't suspend the musics Battle / Boss Battle / Faerie Battle, but assume that the other musics could rightfully be resumed in the field
					player.FF9SOUND_SONG_VOL(currentMusicId, AllSoundDispatchPlayer.VOLUME_MAX);
					player.FF9SOUND_SONG_SUSPEND(currentMusicId);
				}
				if (song_id >= 0)
					btlsnd.ff9btlsnd_song_load(song_id);
				sys.btl_flag |= 2;
			}
			if (song_id >= 0 && btlsnd.ff9btlsnd_sync() != 0)
				return false;
			if ((sys.btl_flag & 32) == 0)
			{
				if (song_id >= 0)
					btlsnd.ff9btlsnd_song_play(song_id);
				sys.btl_flag |= 32;
			}
			return true;
		}

		public static UInt16 GetStatusBtlID(UInt32 list_no, BattleStatus status)
		{
			UInt16 num = 0;
			for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
			{
				if ((status == 0u || btl_stat.CheckStatus(next, status)) && next.bi.target != 0)
				{
					switch (list_no)
					{
					case 0u:
						if (next.bi.player != 0)
							num = (UInt16)(num | next.btl_id);
						break;
					case 1u:
						if (next.bi.player == 0)
							num = (UInt16)(num | next.btl_id);
						break;
					case 2u:
						num = (UInt16)(num | next.btl_id);
						break;
					}
				}
			}
			return num;
		}

		public static Boolean CheckEnemyCategory(BTL_DATA btl, Byte category)
		{
			return btl.bi.player == 0 && (btl_util.getEnemyTypePtr(btl).category & category) != 0;
		}

		public static Boolean IsAttackShortRange(CMD_DATA cmd)
		{
			// Custom usage of "aa.Type & 0x8" (unused by default): flag for short range attacks
			// One might want to check using "cmd.aa.Info.VfxIndex" and "cmd.aa.Vfx2" instead
			if (cmd.aa == null)
				return false;
			if (cmd.regist == null)
				return false;
			if (cmd.regist.weapon != null && (cmd.regist.weapon.Category & WeaponCategory.ShortRange) == 0)
				return false;
			if (Configuration.Battle.CustomBattleFlagsMeaning == 1 && (cmd.AbilityType & 0x8) != 0)
				return true;
			if (Configuration.Battle.CustomBattleFlagsMeaning != 1 && (cmd.regist.bi.player == 0 || GetCommandMainActionIndex(cmd) == BattleAbilityId.Attack))
				return true;
			return false;
		}

		public static Boolean IsCommandMonsterTransform(CMD_DATA cmd)
		{
			return cmd.regist != null && cmd.regist.is_monster_transform && (cmd.cmd_no == cmd.regist.monster_transform.new_command || cmd.cmd_no == BattleCommandId.EnemyCounter);
		}

		public static Boolean IsCommandMonsterTransformAttack(CMD_DATA cmd)
		{
			return btl_util.IsCommandMonsterTransformAttack(cmd.regist, cmd.cmd_no, cmd.sub_no);
		}

		public static Boolean IsCommandMonsterTransformAttack(BTL_DATA btl, BattleCommandId commandId, Int32 sub_no)
		{
			return btl != null && btl.is_monster_transform && (commandId == BattleCommandId.Attack || commandId == BattleCommandId.Counter || commandId == BattleCommandId.EnemyCounter || commandId == BattleCommandId.RushAttack) && sub_no == (Int32)BattleAbilityId.Attack;
		}

		public static Boolean IsCommandDeclarable(BattleCommandId cmdNo)
		{
			return cmdNo < BattleCommandId.BoundaryCheck || cmdNo > BattleCommandId.BoundaryUpperCheck;
		}

        public static CharacterCommandType GetCommandTypeSafe(BattleCommandId cmdNo)
        {
            if (cmdNo == BattleCommandId.Item || cmdNo == BattleCommandId.AutoPotion)
                return CharacterCommandType.Item;
            if (cmdNo == BattleCommandId.Throw)
                return CharacterCommandType.Throw;
            if (CharacterCommands.Commands.TryGetValue(cmdNo, out CharacterCommand chcmd))
                return chcmd.Type;
            return CharacterCommandType.Normal;
        }

		public static BattleAbilityId GetCommandMainActionIndex(CMD_DATA cmd)
		{
            if (cmd.regist != null && cmd.regist.bi.player == 0)
				return BattleAbilityId.Void;
			if (IsCommandMonsterTransform(cmd))
				return BattleAbilityId.Void;
			if (IsCommandMonsterTransformAttack(cmd))
				return BattleAbilityId.Attack;
            CharacterCommandType cmdType = GetCommandTypeSafe(cmd.cmd_no);
            if (cmdType == CharacterCommandType.Item)
                return BattleAbilityId.Void;
            else if (cmdType == CharacterCommandType.Throw)
                return BattleAbilityId.Throw;
            switch (cmd.cmd_no)
			{
				case BattleCommandId.SysEscape:
					return BattleAbilityId.Flee2;
				case BattleCommandId.Throw:
					return BattleAbilityId.Throw;
				case BattleCommandId.Item:
				case BattleCommandId.AutoPotion:
				case BattleCommandId.SysDead:
				case BattleCommandId.SysReraise:
				case BattleCommandId.SysStone:
					return BattleAbilityId.Void;
				case BattleCommandId.MagicCounter:
					return BattleAbilityId.Void;
				default:
					return (BattleAbilityId)cmd.sub_no;
			}
		}

        public static RegularItem GetCommandItem(CMD_DATA cmd)
        {
            if (cmd.regist != null && cmd.regist.bi.player == 0)
                return RegularItem.NoItem;
            if (IsCommandMonsterTransform(cmd) || IsCommandMonsterTransformAttack(cmd))
                return RegularItem.NoItem;
            if (BattleHUD.MixCommandSet.ContainsKey(cmd.cmd_no) && ff9mixitem.MixItemsData.TryGetValue(cmd.sub_no, out MixItems MixChoosen))
                return MixChoosen.Result;
            CharacterCommandType cmdType = GetCommandTypeSafe(cmd.cmd_no);
            if (cmdType == CharacterCommandType.Item || cmdType == CharacterCommandType.Throw)
                return (RegularItem)cmd.sub_no;
            return RegularItem.NoItem;
        }

        public static AA_DATA GetCommandMonsterAttack(CMD_DATA cmd)
		{
			if (IsCommandMonsterTransform(cmd))
				return cmd.regist.monster_transform.spell[cmd.sub_no];
			if (IsCommandMonsterTransformAttack(cmd))
				return cmd.regist.monster_transform.attack[cmd.regist.bi.def_idle];
			return null;
		}

		public static AA_DATA GetCommandAction(CMD_DATA cmd)
		{
			if (cmd.cmd_no == BattleCommandId.SysTrans)
				return FF9StateSystem.Battle.FF9Battle.aa_data[BattleAbilityId.Void];
			if ((cmd.regist != null && cmd.regist.bi.player == 0) || cmd.cmd_no == BattleCommandId.MagicCounter)
				return FF9StateSystem.Battle.FF9Battle.enemy_attack[cmd.sub_no];
			AA_DATA monsterAA = GetCommandMonsterAttack(cmd);
			if (monsterAA != null)
				return monsterAA;
			return FF9StateSystem.Battle.FF9Battle.aa_data[GetCommandMainActionIndex(cmd)];
		}

		public static Int32 GetCommandScriptId(CMD_DATA cmd)
		{
            if (cmd.regist != null && cmd.regist.bi.player != 0 && GetCommandTypeSafe(cmd.cmd_no) == CharacterCommandType.Item)
            {
                RegularItem cmdItem = GetCommandItem(cmd);
                if (cmdItem != RegularItem.NoItem)
                    return ff9item.GetItemEffect(cmdItem).Ref.ScriptId;
                return cmd.ScriptId;
            }
            switch (cmd.cmd_no)
			{
				case BattleCommandId.Jump:
				case BattleCommandId.Jump2:
				case BattleCommandId.SysTrans:
					return 0;
				case BattleCommandId.Item:
				case BattleCommandId.AutoPotion:
					return ff9item.GetItemEffect(GetCommandItem(cmd)).Ref.ScriptId;
				default:
					if (cmd.regist?.weapon != null && GetCommandMainActionIndex(cmd) == BattleAbilityId.Attack && !IsCommandMonsterTransformAttack(cmd))
						return cmd.regist.weapon.Ref.ScriptId;
					return cmd.ScriptId;
			}
		}

		public static void SetEnemyDieSound(BTL_DATA btl, UInt16 snd_no)
		{
			FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
			if (ff9Battle.enemy_die == 0 && btl.bi.die_snd_f == 0)
			{
				ff9Battle.enemy_die = 8;
				btl_util.SetBattleSfx(btl, snd_no, 127);
				btl.bi.die_snd_f = 1;
			}
		}

		public static void SetBattleSfx(BTL_DATA btl, UInt16 snd_no, Byte volume)
		{
			if (snd_no != 65535)
			{
				UInt64 id = 0UL;
				btlsnd.ff9btlsnd_sndeffect_play((Int32)snd_no, 0, (Int32)volume, SeSnd.S_SeGetPos(id));
			}
		}

		public static void SetBBGColor(GameObject geo)
		{
			BBGINFO bbginfo = battlebg.nf_GetBbgInfoPtr();
			btl_util.GeoSetColor2Source(geo, bbginfo.chr_r, bbginfo.chr_g, bbginfo.chr_b);
		}

		public static void SetShadow(BTL_DATA btl, UInt16 x_radius, UInt32 z_radius)
		{
			btl.shadow_x = (Byte)(x_radius * 16 / 224);
			btl.shadow_z = (Byte)(z_radius * 16u / 192u);
			btlshadow.FF9ShadowSetScaleBattle(btl, btl.shadow_x, btl.shadow_z);
		}

		public static void SetFadeRate(BTL_DATA btl, Int32 rate)
		{
			if (rate >= 0 && rate < 32)
			{
				BBGINFO bbginfo = battlebg.nf_GetBbgInfoPtr();
				btl_util.GeoSetABR(btl.gameObject, "GEO_POLYFLAGS_TRANS_100_PLUS_25");
				btl_util.GeoSetColor2Source(btl.gameObject, (Byte)((Int32)bbginfo.chr_r * rate >> 5), (Byte)((Int32)bbginfo.chr_g * rate >> 5), (Byte)((Int32)bbginfo.chr_b * rate >> 5));
				if (btl.weapon_geo)
				{
					btl_util.GeoSetABR(btl.weapon_geo, "GEO_POLYFLAGS_TRANS_100_PLUS_25");
					btl_util.GeoSetColor2Source(btl.weapon_geo, (Byte)((Int32)bbginfo.chr_r * rate >> 5), (Byte)((Int32)bbginfo.chr_g * rate >> 5), (Byte)((Int32)bbginfo.chr_b * rate >> 5));
				}
			}
		}

		public static void SetEnemyFadeToPacket(BTL_DATA btl, Int32 rate)
		{
			BBGINFO bbginfo = battlebg.nf_GetBbgInfoPtr();
			btl_util.GeoSetABR(btl.gameObject, "GEO_POLYFLAGS_TRANS_100_PLUS_25");
			btl_util.GeoSetColor2DrawPacket(btl.gameObject, (Byte)((Int32)bbginfo.chr_r * rate >> 5), (Byte)((Int32)bbginfo.chr_g * rate >> 5), (Byte)((Int32)bbginfo.chr_b * rate >> 5), Byte.MaxValue);
			if (btl.bi.shadow != 0)
			{
				btl_util.GeoSetABR(btl.getShadow(), "GEO_POLYFLAGS_TRANS_100_PLUS_25");
				btl_util.GeoSetColor2DrawPacket(btl.getShadow(), (Byte)((Int32)bbginfo.chr_r * rate >> 5), (Byte)((Int32)bbginfo.chr_g * rate >> 5), (Byte)((Int32)bbginfo.chr_b * rate >> 5), Byte.MaxValue);
			}
			if (rate == 0)
				btl.SetDisappear(true, 5);
		}

		public static void GeoSetABR(GameObject go, String type)
		{
			Shader shader;
			if (type == "GEO_POLYFLAGS_TRANS_100_PLUS_25")
				shader = FF9StateSystem.Battle.fadeShader;
			else if (type == "SEMI_TRANS_50_PLUS_50" || type == "PSX/BattleMap_StatusEffect")
            {
                shader = FF9StateSystem.Battle.battleShader;
            }

			else if (type == "SHADOW" || type == "PSX/BattleMap_Abr_2")
				shader = FF9StateSystem.Battle.shadowShader;
			else
				shader = ShadersLoader.Find(type);
			SkinnedMeshRenderer[] componentsInChildren = go.GetComponentsInChildren<SkinnedMeshRenderer>();
			for (Int32 i = 0; i < (Int32)componentsInChildren.Length; i++)
			{
				componentsInChildren[i].material.shader = shader;
				componentsInChildren[i].material.SetFloat("_Cutoff", 0.5f);
				componentsInChildren[i].material.SetTexture("_DetailTex", FF9StateSystem.Battle.detailTexture);
			}
			MeshRenderer[] componentsInChildren2 = go.GetComponentsInChildren<MeshRenderer>();
			for (Int32 j = 0; j < (Int32)componentsInChildren2.Length; j++)
			{
				Material[] materials = componentsInChildren2[j].materials;
				for (Int32 k = 0; k < (Int32)materials.Length; k++)
				{
					Material material = materials[k];
					material.shader = shader;
					material.SetFloat("_Cutoff", 0.5f);
					material.SetTexture("_DetailTex", FF9StateSystem.Battle.detailTexture);
				}
			}
		}
        
        public static void GeoSetABR(GameObject go, String type, BTL_DATA btl)
        {
            Shader shader;
            if (type == "GEO_POLYFLAGS_TRANS_100_PLUS_25")
                shader = FF9StateSystem.Battle.fadeShader;
            else if (type == "SEMI_TRANS_50_PLUS_50" || type == "PSX/BattleMap_StatusEffect")
            {
                shader = FF9StateSystem.Battle.battleShader;
            }

            else if (type == "SHADOW" || type == "PSX/BattleMap_Abr_2")
                shader = FF9StateSystem.Battle.shadowShader;
            else
                shader = ShadersLoader.Find(type);
            SkinnedMeshRenderer[] componentsInChildren = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (Int32 i = 0; i < (Int32)componentsInChildren.Length; i++)
            {
                componentsInChildren[i].material.shader = shader;
                componentsInChildren[i].material.SetFloat("_Cutoff", 0.5f);
                componentsInChildren[i].material.SetFloat("_OutlineWidth", 2.3f);
                componentsInChildren[i].material.SetFloat("_ShowOutline", Configuration.Graphics.OutlineForBattleCharacter == 1 ? 1f : 0f);
                componentsInChildren[i].material.SetFloat("_IsEnemy", btl.bi.player == 0 ? 1 : 0);
                componentsInChildren[i].material.SetInt("_StencilOp", btl.bi.player == 0 ? 2 : 1);
                componentsInChildren[i].material.SetTexture("_DetailTex", FF9StateSystem.Battle.detailTexture);
            }
            MeshRenderer[] componentsInChildren2 = go.GetComponentsInChildren<MeshRenderer>();
            for (Int32 j = 0; j < (Int32)componentsInChildren2.Length; j++)
            {
                Material[] materials = componentsInChildren2[j].materials;
                for (Int32 k = 0; k < (Int32)materials.Length; k++)
                {
                    Material material = materials[k];
                    material.shader = shader;
                    material.SetFloat("_Cutoff", 0.5f);
                    material.SetTexture("_DetailTex", FF9StateSystem.Battle.detailTexture);
                    material.SetFloat("_OutlineWidth", 2.3f);
                    material.SetFloat("_ShowOutline", 1f);
                    material.SetFloat("_IsEnemy", btl.bi.player == 0 ? 1 : 0);
                    material.SetInt("_StencilOp", btl.bi.player == 0 ? 2 : 1);
                }
            }
        }

		public static void GeoSetColor2Source(GameObject go, Byte r, Byte g, Byte b)
		{
			btl_util.GeoSetColor2DrawPacket(go, r, g, b, Byte.MaxValue);
		}

		public static void GeoSetColor2DrawPacket(GameObject go, Byte r, Byte g, Byte b, Byte a = 255)
		{
			if (r > 255)
				r = Byte.MaxValue;
			if (g > 255)
				g = Byte.MaxValue;
			if (b > 255)
				b = Byte.MaxValue;
			SkinnedMeshRenderer[] componentsInChildren = go.GetComponentsInChildren<SkinnedMeshRenderer>();
			for (Int32 i = 0; i < (Int32)componentsInChildren.Length; i++)
				componentsInChildren[i].material.SetColor("_Color", new Color32(r, g, b, a));
			MeshRenderer[] componentsInChildren2 = go.GetComponentsInChildren<MeshRenderer>();
			for (Int32 j = 0; j < (Int32)componentsInChildren2.Length; j++)
			{
				Material[] materials = componentsInChildren2[j].materials;
				for (Int32 k = 0; k < (Int32)materials.Length; k++)
				{
					Material material = materials[k];
					material.SetColor("_Color", new Color32(r, g, b, a));
				}
			}
		}

		[Flags]
		public enum BusyMode
		{
			CASTER = 1,
			TARGET = 2,
			MAGIC_CASTER = 4,
			QUEUED_CASTER = 8,
			QUEUED_TARGET = 16,
			QUEUED_MAGIC_CASTER = 32,
			ANY_CURRENT = CASTER | TARGET | MAGIC_CASTER,
			ANY_QUEUED = QUEUED_CASTER | QUEUED_TARGET | QUEUED_MAGIC_CASTER,
			ANY = ANY_CURRENT | ANY_QUEUED
		}
	}
}

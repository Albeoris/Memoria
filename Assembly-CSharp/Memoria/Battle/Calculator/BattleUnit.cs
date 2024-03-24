using System;
using System.Collections.Generic;
using FF9;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Data;
using Memoria.Database;
using Memoria.Assets;
using UnityEngine;
using Object = System.Object;

namespace Memoria
{
    public class BattleUnit
    {
        public CalcFlag Flags;
        public Int32 HpDamage;
        public Int32 MpDamage;

        public BTL_DATA Data;

        public BattleUnit(BTL_DATA data)
        {
            Data = data;
        }

        public static implicit operator BTL_DATA(BattleUnit unit) => unit.Data;

        public UInt16 Id => Data.btl_id;
        public Boolean IsPlayer => Data.bi.player != 0;
        public Boolean IsNonMorphedPlayer => Data.bi.player != 0 && !Data.is_monster_transform;
        public Boolean IsTargetable => Data.bi.target != 0;
        public Boolean IsSlave => Data.bi.slave != 0;
        public Boolean IsOutOfReach
        {
            get => Data.out_of_reach;
            set => Data.out_of_reach = value;
        }
        public Boolean CanMove => Data.bi.atb != 0;
        public CharacterId PlayerIndex => IsPlayer ? (CharacterId)Data.bi.slot_no : CharacterId.NONE;

        public Byte Level => Data.special_status_old ? (byte)((Data.level >> 3) < 1 ? 1 : Data.level >> 3) : Data.level;
        public Byte Position => Data.bi.line_no;

        public Byte Row
        {
            get => Data.bi.row;
            set
            {
                if (value != Data.bi.row)
                    btl_para.SwitchPlayerRow(Data, false);
            }
        }
        public Boolean IsCovering => Data.bi.cover != 0;

        public Boolean IsDodged
        {
            get => Data.bi.dodge != 0;
            set => Data.bi.dodge = (Byte)(value ? 1 : 0);
        }

        public UInt32 MaximumHp
        {
            get => btl_para.GetLogicalHP(Data, true);
            set => btl_para.SetLogicalHP(Data, value, true);
        }
        public UInt32 CurrentHp
        {
            get => btl_para.GetLogicalHP(Data, false);
            set => btl_para.SetLogicalHP(Data, value, false);
        }

        public UInt32 MaximumMp
        {
            get => Data.max.mp;
            set => Data.max.mp = value;
        }
        public UInt32 CurrentMp
        {
            get => Data.cur.mp;
            set => Data.cur.mp = value;
        }

        public Int16 MaximumAtb => Data.max.at;
        public Int16 CurrentAtb
        {
            get => Data.cur.at;
            set => Data.cur.at = value;
        }

        public Int32 PhysicalDefence
        {
            get => Data.special_status_old ? ((Data.defence.PhysicalDefence >> 3) < 1 ? 1 : Data.defence.PhysicalDefence >> 3) : Data.defence.PhysicalDefence;
            set => Data.defence.PhysicalDefence = value;
        }

        public Int32 PhysicalEvade
        {
            get => Data.special_status_old ? ((Data.defence.PhysicalEvade >> 3) < 1 ? 1 : Data.defence.PhysicalEvade >> 3) : Data.defence.PhysicalEvade;
            set => Data.defence.PhysicalEvade = value;
        }

        public Int32 MagicDefence
        {
            get => Data.special_status_old ? ((Data.defence.MagicalDefence >> 3) < 1 ? 1 : Data.defence.MagicalDefence >> 3) : Data.defence.MagicalDefence;
            set => Data.defence.MagicalDefence = value;
        }

        public Int32 MagicEvade
        {
            get => Data.special_status_old ? ((Data.defence.MagicalEvade >> 3) < 1 ? 1 : Data.defence.MagicalEvade >> 3) : Data.defence.MagicalEvade;
            set => Data.defence.MagicalEvade = value;
        }

        public Byte Strength
        {
            get => Data.special_status_old ? (Byte)((Data.elem.str >> 3) < 1 ? 1 : Data.elem.str >> 3) : Data.elem.str;
            set => Data.elem.str = value;
        }

        public Byte Magic
        {
            get => Data.special_status_old ? (Byte)((Data.elem.mgc >> 3) < 1 ? 1 : Data.elem.mgc >> 3) : Data.elem.mgc;
            set => Data.elem.mgc = value;
        }

        public Byte Dexterity
        {
            get => Data.special_status_old ? (Byte)((Data.elem.dex >> 3) < 1 ? 1 : Data.elem.dex >> 3) : Data.elem.dex;
            set => Data.elem.dex = value;
        }

        public Byte Will
        {
            get => Data.special_status_old ? (Byte)((Data.elem.wpr >> 3) < 1 ? 1 : Data.elem.wpr >> 3) : Data.elem.wpr;
            set => Data.elem.wpr = value;
        }

        public UInt32 MaxDamageLimit
        {
            get => Data.maxDamageLimit;
            set => Data.maxDamageLimit = value;
        }

        public UInt32 MaxMpDamageLimit
        {
            get => Data.maxMpDamageLimit;
            set => Data.maxMpDamageLimit = value;
        }

        public Boolean HasTrance => Data.bi.t_gauge != 0;
        public Boolean InTrance => (CurrentStatus & BattleStatus.Trance) != 0;
        public Byte Trance
        {
            get => Data.trance;
            set => Data.trance = value;
        }

        public Int32 Fig
        {
            get => Data.fig;
            set => Data.fig = value;
        }

        public Int32 MagicFig
        {
            get => Data.m_fig;
            set => Data.m_fig = value;
        }
        public UInt16 FigInfo
        {
            get => Data.fig_info;
            set => Data.fig_info = value;
        }
        public Int32 WeaponRate => Data.weapon != null ? Data.weapon.Ref.Rate : 0;
        public Int32 WeaponPower => Data.weapon != null ? Data.weapon.Ref.Power : 0;
        public EffectElement WeaponElement => (EffectElement)(Data.weapon != null ? Data.weapon.Ref.Elements : 0);
        public BattleStatus WeaponStatus => Data.weapon != null ? FF9StateSystem.Battle.FF9Battle.add_status[Data.weapon.StatusIndex].Value : 0;
        public Int32 GetWeaponPower(BattleCommand cmd)
        {
            return Data.weapon == null ? 0
              : Configuration.Battle.CustomBattleFlagsMeaning == 1 && FF9StateSystem.Battle.FF9Battle.btl_scene.Info.ReverseAttack && cmd != null && (cmd.AbilityType & 0x8) != 0 ? Math.Max(1, 60 - Data.weapon.Ref.Power)
              : Data.weapon.Ref.Power;
        }

        public Character Player => Character.Find(this);
        public CharacterSerialNumber SerialNumber => btl_util.getSerialNumber(Data);
        public CharacterCategory PlayerCategory => IsPlayer ? Player.Category : 0;
        public EnemyCategory Category => IsPlayer ? EnemyCategory.Humanoid : (EnemyCategory)btl_util.getEnemyTypePtr(Data).category;
        public WeaponCategory WeapCategory => Data.weapon != null ? Data.weapon.Category : 0;
        public BattleEnemy Enemy => new BattleEnemy(btl_util.getEnemyPtr(Data));
        public ENEMY_TYPE EnemyType => btl_util.getEnemyTypePtr(Data);
        public String Name => IsPlayer ? Player.Name : Enemy.Name;

        public BattleStatus CurrentStatus
        {
            get => Data.stat.cur;
            set => Data.stat.cur = value;
        }

        public BattleStatus PermanentStatus
        {
            get => Data.stat.permanent;
            set => Data.stat.permanent = value;
        }

        public BattleStatus ResistStatus
        {
            get => Data.stat.invalid;
            set
            {
                Data.stat.invalid = value;
                if (!IsPlayer)
                    Data.bi.t_gauge = (Byte)((value & BattleStatus.Trance) == 0 ? 1 : 0);
            }
        }

        public StatusModifier PartialResistStatus => Data.stat_partial_resist;
        public StatusModifier StatusDurationFactor => Data.stat_duration_factor;

        public EffectElement BonusElement
        {
            get => (EffectElement)Data.p_up_attr;
            set => Data.p_up_attr = (Byte)value;
        }
        public EffectElement WeakElement
        {
            get => (EffectElement)Data.def_attr.weak;
            set => Data.def_attr.weak = (Byte)value;
        }
        public EffectElement GuardElement
        {
            get => (EffectElement)Data.def_attr.invalid;
            set => Data.def_attr.invalid = (Byte)value;
        }
        public EffectElement AbsorbElement
        {
            get => (EffectElement)Data.def_attr.absorb;
            set => Data.def_attr.absorb = (Byte)value;
        }
        public EffectElement HalfElement
        {
            get => (EffectElement)Data.def_attr.half;
            set => Data.def_attr.half = (Byte)value;
        }

        public Boolean IsLevitate => HasCategory(EnemyCategory.Flight) || IsUnderAnyStatus(BattleStatus.Float);
        public Boolean IsZombie => HasCategory(EnemyCategory.Undead) || IsUnderAnyStatus(BattleStatus.Zombie);
        public Boolean HasLongRangeWeapon => HasCategory(WeaponCategory.LongRange);

        public RegularItem Weapon => btl_util.getWeaponNumber(Data);
        public RegularItem Head => IsPlayer ? FF9StateSystem.Common.FF9.GetPlayer(PlayerIndex).equip.Head : RegularItem.NoItem;
        public RegularItem Wrist => IsPlayer ? FF9StateSystem.Common.FF9.GetPlayer(PlayerIndex).equip.Wrist : RegularItem.NoItem;
        public RegularItem Armor => IsPlayer ? FF9StateSystem.Common.FF9.GetPlayer(PlayerIndex).equip.Armor : RegularItem.NoItem;
        public RegularItem Accessory => IsPlayer ? FF9StateSystem.Common.FF9.GetPlayer(PlayerIndex).equip.Accessory : RegularItem.NoItem;
        public Boolean IsHealingRod => IsPlayer && Weapon == RegularItem.HealingRod;

        public Boolean[] StatModifier => Data.stat_modifier;

        public BattleUnit GetKiller()
        {
            return Data.killer_track != null ? new BattleUnit(Data.killer_track) : null;
        }

        public void AddDelayedModifier(BTL_DATA.DelayedModifier.IsDelayedDelegate delayDelegate, BTL_DATA.DelayedModifier.ApplyDelegate applyDelegate)
        {
            if (applyDelegate == null)
                return;
            if (delayDelegate == null)
            {
                Data.delayedModifierList.Add(new BTL_DATA.DelayedModifier()
                {
                    isDelayed = btl => false,
                    apply = applyDelegate
                });
                return;
            }
            if (!delayDelegate(this))
            {
                applyDelegate(this);
                return;
            }
            Data.delayedModifierList.Add(new BTL_DATA.DelayedModifier()
            {
                isDelayed = delayDelegate,
                apply = applyDelegate
            });
        }

        public Boolean IsPlayingMotion(BattlePlayerCharacter.PlayerMotionIndex motionIndex) => btl_mot.checkMotion(Data, motionIndex);
        public Boolean IsPlayingIdleMotion() => btl_mot.checkMotion(Data, Data.bi.def_idle);

        public UInt16 SummonCount
        {
            get => Data.summon_count;
            set => Data.summon_count = value;
        }
        public Int16 CriticalRateBonus
        {
            get => Data.critical_rate_deal_bonus;
            set => Data.critical_rate_deal_bonus = value;
        }
        public Int16 CriticalRateWeakening
        {
            get => Data.critical_rate_receive_bonus;
            set => Data.critical_rate_receive_bonus = value;
        }

        public Boolean IsMonsterTransform => Data.is_monster_transform;

        public Int32 BattlePosX
        {
            get => btl_scrp.GetCharacterData(Data, 140);
            set
            {
                btl_scrp.SetCharacterData(Data, 140, value);
                Data.base_pos.x = Data.pos.x;
            }
        }
        public Int32 BattlePosY
        {
            get => btl_scrp.GetCharacterData(Data, 141);
            set
            {
                btl_scrp.SetCharacterData(Data, 141, value);
                Data.base_pos.y = Data.pos.y;
            }
        }
        public Int32 BattlePosZ
        {
            get => btl_scrp.GetCharacterData(Data, 142);
            set
            {
                btl_scrp.SetCharacterData(Data, 142, value);
                Data.base_pos.z = Data.pos.z;
            }
        }
        public Int32 BattleScaleX
        {
            get => Data.geo_scale_x;
            set => geo.geoScaleSetXYZ(Data, value, Data.geo_scale_y, Data.geo_scale_z, false);
        }
        public Int32 BattleScaleY
        {
            get => Data.geo_scale_y;
            set => geo.geoScaleSetXYZ(Data, Data.geo_scale_x, value, Data.geo_scale_z, false);
        }
        public Int32 BattleScaleZ
        {
            get => Data.geo_scale_z;
            set => geo.geoScaleSetXYZ(Data, Data.geo_scale_x, Data.geo_scale_y, value, false);
        }

        public void ScaleModel(Int32 size, Boolean setDefault = false) // 4096 is the default size
        {
            if (setDefault)
                Data.geo_scale_default = size;
            geo.geoScaleSet(Data, size, true);
        }

        public Boolean IsUnderStatus(BattleStatus status)
        {
            return (CurrentStatus & status) != 0;
        }

        public Boolean IsUnderPermanentStatus(BattleStatus status)
        {
            return (PermanentStatus & status) != 0;
        }

        public Boolean IsUnderAnyStatus(BattleStatus status)
        {
            return ((CurrentStatus | PermanentStatus) & status) != 0;
        }

        public Boolean HasCategory(CharacterCategory category)
        {
            return (PlayerCategory & category) != 0;
        }

        public Boolean HasCategory(EnemyCategory category)
        {
            return btl_util.CheckEnemyCategory(Data, (Byte)category);
        }

        public Boolean HasCategory(WeaponCategory category)
        {
            if (Data.weapon == null)
                return false;

            return (Data.weapon.Category & category) != 0;
        }

        public Boolean HasSupportAbility(SupportAbility1 ability)
        {
            return (Data.sa[0] & (UInt32)ability) != 0;
        }

        public Boolean HasSupportAbility(SupportAbility2 ability)
        {
            return (Data.sa[1] & (UInt32)ability) != 0;
        }

        public Boolean HasSupportAbilityByIndex(SupportAbility saIndex)
        {
            return Data.saExtended.Contains(saIndex);
            //Int32 index = (Int32)saIndex;
            //if (abilId < 0) return false;
            //if (abilId < 32) return HasSupportAbility((SupportAbility1)(1u << index));
            //if (abilId < 64) return HasSupportAbility((SupportAbility2)(1u << index));
            //return Data.saExtended.Contains(saIndex);
        }

        public Boolean TryRemoveStatuses(BattleStatus status)
        {
            return btl_stat.RemoveStatuses(Data, status) == 2U;
        }

        public void RemoveStatus(BattleStatus status)
        {
            btl_stat.RemoveStatus(Data, status);
        }

        public void AlterStatus(BattleStatus status, BattleUnit inflicter = null)
        {
            btl_stat.AlterStatus(Data, status, inflicter?.Data);
        }

        public void Kill(BattleUnit killer)
        {
            Kill(killer.Data);
        }
        public void Kill(BTL_DATA killer = null)
        {
            CurrentHp = 0; // When using the 10 000 HP enemy threshold system (with CustomBattleFlagsMeaning == 1), Kill only set the enemy's HP to 1 assuming it will trigger its dying sequence
            if (Data.cur.hp > 0) // Also, let the script handle the animations and sounds in that case
                return;

            Data.killer_track = killer;
            Data.bi.death_f = 1;
            if (!IsPlayer && btl_util.getEnemyPtr(Data).info.die_atk == 0)
            {
                btl_util.SetEnemyDieSound(Data, btl_util.getEnemyTypePtr(Data).die_snd_no);
                Data.die_seq = 3;
            }

            //if (!btl_mot.checkMotion(Data, Data.bi.def_idle) && (Data.bi.player == 0 || !btl_mot.checkMotion(Data, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD)) && !btl_util.IsBtlUsingCommand(Data))
            //{
            //    btl_mot.setMotion(Data, Data.bi.def_idle);
            //    Data.evt.animFrame = 0;
            //}
        }

        public void Remove(Boolean makeDisappear = true)
        {
            battle.btl_bonus.member_flag &= (Byte)~(1 << Data.bi.line_no);
            btl_cmd.ClearSysPhantom(Data);
            btl_cmd.KillCommand3(Data);
            btl_sys.SavePlayerData(Data, true);
            btl_sys.DelCharacter(Data);
            if (makeDisappear)
                Data.SetDisappear(true, 5);
            // The two following lines have been switched for fixing an UI bug (ATB bar glowing, etc... when an ally is snorted)
            // It seems to fix the bug without introducing another one (the HP/MP figures update strangely but that's because of how the UI cells are managed)
            UIManager.Battle.RemovePlayerFromAction(Data.btl_id, true);
            UIManager.Battle.DisplayParty(true);
        }

        public void FaceTheEnemy()
        {
            FaceAsUnit(this);
        }

        public void FaceAsUnit(BattleUnit unit)
        {
            Int32 angle = btl_mot.GetDirection(unit);
            Data.rot.eulerAngles = new Vector3(Data.rot.eulerAngles.x, angle, Data.rot.eulerAngles.z);
        }

        public void ChangeRowToDefault()
        {
            if (IsPlayer && Row != Player.Row)
                btl_para.SwitchPlayerRow(Data);
        }

        public void ChangeRow()
        {
            btl_para.SwitchPlayerRow(Data);
        }

        public void Change(BattleUnit unit)
        {
            Data = unit.Data;
        }

        public void Libra(BattleHUD.LibraInformation infos = BattleHUD.LibraInformation.Default)
        {
            UIManager.Battle.SetBattleLibra(this, infos);
        }

        public void Detect(Boolean reverseOrder = true)
        {
            UIManager.Battle.SetBattlePeeping(this, reverseOrder);
        }

        public Int32 GetIndex()
        {
            Int32 index = 0;

            while (1 << index != Data.btl_id)
                ++index;

            return index;
        }

        public Boolean IsAbilityAvailable(BattleAbilityId abilId)
        {
            return UIManager.Battle.IsAbilityAvailable(this, ff9abil.GetAbilityIdFromActiveAbility(abilId));
        }

        public Boolean IsAbilityAvailable(SupportAbility abilId)
        {
            return UIManager.Battle.IsAbilityAvailable(this, ff9abil.GetAbilityIdFromSupportAbility(abilId));
        }

        public void ChangeToMonster(String btlName, Int32 monsterIndex, BattleCommandId commandToReplace, BattleCommandId commandAsMonster, Boolean cancelOnDeath, Boolean updatePts, Boolean updateStat, Boolean updateDef, Boolean updateElement, List<BattleCommandId> disableCommands = null)
        {
            if (!IsPlayer) // In order to implement something similar for enemies, script has to be update for that enemy's entry, among other things
                return;
            BTL_SCENE scene = new BTL_SCENE();
            scene.ReadBattleScene(btlName);
            if (scene.header.TypCount <= 0)
                return;
            if (monsterIndex < 0)
                monsterIndex = Comn.random8() % scene.header.TypCount;
            if (monsterIndex >= scene.header.TypCount)
                return;
            SB2_MON_PARM monsterParam = scene.MonAddr[monsterIndex];
            Int32 i;
            if (updatePts)
            {
                Data.max.hp = monsterParam.MaxHP;
                Data.max.mp = monsterParam.MaxMP;
                Data.cur.hp = Math.Min(Data.cur.hp, Data.max.hp);
                Data.cur.mp = Math.Min(Data.cur.mp, Data.max.mp);
            }
            if (updateStat)
            {
                Strength = monsterParam.Element.Strength;
                Magic = monsterParam.Element.Magic;
                Dexterity = monsterParam.Element.Speed;
                Will = monsterParam.Element.Spirit;
            }
            if (updateDef)
            {
                Data.defence.PhysicalDefence = monsterParam.PhysicalDefence;
                Data.defence.PhysicalEvade = monsterParam.PhysicalEvade;
                Data.defence.MagicalDefence = monsterParam.MagicalDefence;
                Data.defence.MagicalEvade = monsterParam.MagicalEvade;
            }
            if (updateElement)
            {
                Data.def_attr.invalid = monsterParam.GuardElement;
                Data.def_attr.absorb = monsterParam.AbsorbElement;
                Data.def_attr.half = monsterParam.HalfElement;
                Data.def_attr.weak = monsterParam.WeakElement;
            }
            Data.mesh_current = monsterParam.Mesh[0];
            Data.mesh_banish = monsterParam.Mesh[1];
            Data.tar_bone = monsterParam.Bone[3];
            Data.shadow_bone[0] = monsterParam.ShadowBone;
            Data.shadow_bone[1] = monsterParam.ShadowBone2;
            btl_util.SetShadow(Data, monsterParam.ShadowX, monsterParam.ShadowZ);
            foreach (SupportingAbilityFeature feature in monsterParam.SupportingAbilityFeatures)
                if (feature.EnableAsMonsterTransform)
                    Data.saMonster.Add(feature);
            btlseq.btlseqinstance seqreader = new btlseq.btlseqinstance();
            btlseq.ReadBattleSequence(btlName, ref seqreader);
            seqreader.FixBuggedAnimations(scene);
            List<AA_DATA> aaList = new List<AA_DATA>();
            List<Int32> usableAbilList = new List<Int32>();
            AA_DATA[] attackAA = new AA_DATA[] { null, null };
            List<Int32>[] attackAnims = new List<Int32>[] { null, null };
            Int32 animOffset = 0;
            String[] battleRawText = FF9TextTool.GetBattleText(FF9BattleDB.SceneData["BSC_" + btlName]);
            if (battleRawText == null)
                battleRawText = new String[0];
            for (i = 0; i < scene.header.AtkCount; i++)
            {
                if (seqreader.GetEnemyIndexOfSequence(i) != monsterIndex)
                    continue;
                AA_DATA ability = scene.atk[i];
                aaList.Add(ability);
                // Swap the TargetType but keep the DefaultAlly flag since it is on by default only for curative/buffing enemy spells
                if (ability.Info.Target == TargetType.AllAlly)
                    ability.Info.Target = TargetType.AllEnemy;
                else if (ability.Info.Target == TargetType.AllEnemy)
                    ability.Info.Target = TargetType.AllAlly;
                else if (ability.Info.Target == TargetType.ManyAlly)
                    ability.Info.Target = TargetType.ManyEnemy;
                else if (ability.Info.Target == TargetType.ManyEnemy)
                    ability.Info.Target = TargetType.ManyAlly;
                else if (ability.Info.Target == TargetType.RandomAlly)
                    ability.Info.Target = TargetType.RandomEnemy;
                else if (ability.Info.Target == TargetType.RandomEnemy)
                    ability.Info.Target = TargetType.RandomAlly;
                else if (ability.Info.Target == TargetType.SingleAlly)
                    ability.Info.Target = TargetType.SingleEnemy;
                else if (ability.Info.Target == TargetType.SingleEnemy)
                    ability.Info.Target = TargetType.SingleAlly;
                if (scene.header.TypCount + i < battleRawText.Length)
                    ability.Name = battleRawText[scene.header.TypCount + i];
                animOffset = seqreader.seq_work_set.AnmOfsList[i];
                Int32 sequenceSfx = seqreader.GetSFXOfSequence(i, out Boolean sequenceChannel, out Boolean sequenceContact);
                if (sequenceSfx >= 0)
                    ability.Info.VfxIndex = (Int16)sequenceSfx;
                if (Configuration.Battle.SFXRework && ability.Info.VfxAction == null)
                    ability.Info.VfxAction = new UnifiedBattleSequencer.BattleAction(scene, seqreader, textid => battleRawText[textid], i);
                if (!ability.MorphDisableAccess && (ability.MorphForceAccess || ability.Ref.ScriptId != 64)) // 64 (no effect) is usually scripted dialogs
                {
                    if (sequenceSfx >= 0 && sequenceContact && !ability.MorphForceAccess)
                    {
                        attackAA[ability.AlternateIdleAccess ? 1 : 0] = ability;
                        attackAnims[ability.AlternateIdleAccess ? 1 : 0] = seqreader.GetAnimationsOfSequence(i);
                    }
                    else
                    {
                        usableAbilList.Add(aaList.Count - 1);
                    }
                }
            }
            CharacterCommands.Commands[commandAsMonster].Type = CharacterCommandType.Ability;
            CharacterCommands.Commands[commandAsMonster].ListEntry = usableAbilList.ToArray();
            Data.is_monster_transform = true;
            UIManager.Battle.ClearCursorMemorize(Position, commandAsMonster);
            btl_cmd.KillSpecificCommand(Data, BattleCommandId.Attack);
            BTL_DATA.MONSTER_TRANSFORM monsterTransform = (Data.monster_transform = new BTL_DATA.MONSTER_TRANSFORM());
            monsterTransform.base_command = commandToReplace;
            monsterTransform.new_command = commandAsMonster;
            monsterTransform.attack = attackAA;
            monsterTransform.spell = aaList;
            monsterTransform.replace_point = updatePts;
            monsterTransform.replace_stat = updateStat;
            monsterTransform.replace_defence = updateDef;
            monsterTransform.replace_element = updateElement;
            monsterTransform.cancel_on_death = cancelOnDeath;
            monsterTransform.death_sound = monsterParam.DieSfx;
            monsterTransform.fade_counter = 0;
            for (i = 0; i < 3; i++)
                monsterTransform.cam_bone[i] = monsterParam.Bone[i];
            for (i = 0; i < 6; i++)
            {
                monsterTransform.icon_bone[i] = monsterParam.IconBone[i];
                monsterTransform.icon_y[i] = monsterParam.IconY[i];
                monsterTransform.icon_z[i] = monsterParam.IconZ[i];
            }
            if (disableCommands == null)
                monsterTransform.disable_commands = new List<BattleCommandId>();
            else
                monsterTransform.disable_commands = disableCommands;
            // Let the spell sequence handle the model fadings (in and out)
            //Data.SetActiveBtlData(false);
            String geoName = FF9BattleDB.GEO.GetValue(monsterParam.Geo);
            Data.gameObject = ModelFactory.CreateModel(geoName, true);
            Data.bi.t_gauge = 0;
            if (IsUnderAnyStatus(BattleStatus.Trance))
            {
                Data.stat.permanent &= ~BattleStatus.Trance;
                Data.stat.cur &= ~BattleStatus.Trance;
                if (Trance == Byte.MaxValue)
                    Trance = Byte.MaxValue - 1;
            }
            //Data.SetActiveBtlData(true);
            geoName = geoName.Substring(4);
            monsterTransform.motion_normal = Data.mot;
            monsterTransform.motion_alternate = new String[34];
            for (i = 0; i < 34; i++)
            {
                monsterTransform.motion_normal[i] = String.Empty;
                monsterTransform.motion_alternate[i] = String.Empty;
            }
            Boolean useDieDmg = (monsterParam.Flags & 2) != 0;
            monsterTransform.motion_normal[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_NORMAL] = FF9BattleDB.Animation[monsterParam.Mot[0]];
            monsterTransform.motion_normal[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING] = monsterTransform.motion_normal[0];
            monsterTransform.motion_normal[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE1] = FF9BattleDB.Animation[monsterParam.Mot[2]];
            monsterTransform.motion_normal[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2] = monsterTransform.motion_normal[2];
            monsterTransform.motion_normal[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE] = String.Empty;
            monsterTransform.motion_normal[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DISABLE] = FF9BattleDB.Animation[monsterParam.Mot[useDieDmg ? 3 : 4]];
            monsterTransform.motion_normal[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD] = monsterTransform.motion_normal[0];
            monsterTransform.motion_normal[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DEFENCE] = monsterTransform.motion_normal[0];
            monsterTransform.motion_normal[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_COVER] = monsterTransform.motion_normal[0];
            monsterTransform.motion_normal[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_AVOID] = monsterTransform.motion_normal[0];
            monsterTransform.motion_normal[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_ESCAPE] = monsterTransform.motion_normal[0];
            monsterTransform.motion_normal[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_WIN_LOOP] = monsterTransform.motion_normal[0];
            monsterTransform.motion_normal[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT] = monsterTransform.motion_normal[0];
            monsterTransform.motion_normal[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_STEP_FORWARD] = monsterTransform.motion_normal[0];
            monsterTransform.motion_normal[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_STEP_BACK] = monsterTransform.motion_normal[0];
            monsterTransform.motion_alternate[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_NORMAL] = FF9BattleDB.Animation[monsterParam.Mot[1]];
            monsterTransform.motion_alternate[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING] = monsterTransform.motion_alternate[0];
            monsterTransform.motion_alternate[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE1] = FF9BattleDB.Animation[monsterParam.Mot[3]];
            monsterTransform.motion_alternate[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2] = monsterTransform.motion_alternate[2];
            monsterTransform.motion_alternate[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE] = String.Empty;
            monsterTransform.motion_alternate[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DISABLE] = FF9BattleDB.Animation[monsterParam.Mot[4]];
            monsterTransform.motion_alternate[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD] = monsterTransform.motion_alternate[0];
            monsterTransform.motion_alternate[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DEFENCE] = monsterTransform.motion_alternate[0];
            monsterTransform.motion_alternate[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_COVER] = monsterTransform.motion_alternate[0];
            monsterTransform.motion_alternate[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_AVOID] = monsterTransform.motion_alternate[0];
            monsterTransform.motion_alternate[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_ESCAPE] = monsterTransform.motion_alternate[0];
            monsterTransform.motion_alternate[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_WIN_LOOP] = monsterTransform.motion_alternate[0];
            monsterTransform.motion_alternate[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT] = monsterTransform.motion_alternate[0];
            monsterTransform.motion_alternate[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_STEP_FORWARD] = monsterTransform.motion_alternate[0];
            monsterTransform.motion_alternate[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_STEP_BACK] = monsterTransform.motion_alternate[0];
            // Try to automatically get a few animations
            // Physical attack
            for (i = 0; i < 6; i++)
            {
                if (attackAnims[0] != null && i < attackAnims[0].Count)
                    monsterTransform.motion_normal[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_SET + i] = attackAnims[0][i] == 0xFF ? monsterTransform.motion_normal[0] : FF9BattleDB.Animation[seqreader.seq_work_set.AnmAddrList[animOffset + attackAnims[0][i]]];
                if (attackAnims[1] != null && i < attackAnims[1].Count)
                    monsterTransform.motion_alternate[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_SET + i] = attackAnims[1][i] == 0xFF ? monsterTransform.motion_alternate[0] : FF9BattleDB.Animation[seqreader.seq_work_set.AnmAddrList[animOffset + attackAnims[1][i]]];
            }
            // Cast Init / Loop / End
            if (geoName.CompareTo("MON_B3_199") == 0) // Necron
            {
                ChangeToMonster_SetClip(monsterTransform.motion_normal, "ANH_" + geoName + "_030", BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_TO_CHANT);
                ChangeToMonster_SetClip(monsterTransform.motion_normal, "ANH_" + geoName + "_031", BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT);
                ChangeToMonster_SetClip(monsterTransform.motion_normal, "ANH_" + geoName + "_032", BattlePlayerCharacter.PlayerMotionIndex.MP_MAGIC);
                ChangeToMonster_SetClip(monsterTransform.motion_alternate, "ANH_" + geoName + "_020", BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_TO_CHANT);
                ChangeToMonster_SetClip(monsterTransform.motion_alternate, "ANH_" + geoName + "_021", BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT);
                ChangeToMonster_SetClip(monsterTransform.motion_alternate, "ANH_" + geoName + "_022", BattlePlayerCharacter.PlayerMotionIndex.MP_MAGIC);
            }
            else
            {
                ChangeToMonster_SetClip(monsterTransform.motion_normal, "ANH_" + geoName + "_020", BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_TO_CHANT);
                ChangeToMonster_SetClip(monsterTransform.motion_normal, "ANH_" + geoName + "_021", BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT);
                ChangeToMonster_SetClip(monsterTransform.motion_normal, "ANH_" + geoName + "_022", BattlePlayerCharacter.PlayerMotionIndex.MP_MAGIC);
                if (geoName.CompareTo("MON_B3_050") == 0 || geoName.CompareTo("MON_B3_051") == 0 || geoName.CompareTo("MON_B3_061") == 0 || geoName.CompareTo("MON_B3_115") == 0 || geoName.CompareTo("MON_B3_119") == 0 || geoName.CompareTo("MON_B3_120") == 0)
                {
                    // Xylomid [MON_B3_050], Movers [MON_B3_051], Pampa [MON_B3_061], Black Waltz 3 [MON_B3_115], Zorn [MON_B3_119], Thorn [MON_B3_120]
                    ChangeToMonster_SetClip(monsterTransform.motion_alternate, "ANH_" + geoName + "_040", BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_TO_CHANT);
                    ChangeToMonster_SetClip(monsterTransform.motion_alternate, "ANH_" + geoName + "_041", BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT);
                    ChangeToMonster_SetClip(monsterTransform.motion_alternate, "ANH_" + geoName + "_042", BattlePlayerCharacter.PlayerMotionIndex.MP_MAGIC);
                }
                else if (geoName.CompareTo("MON_B3_126") == 0 || geoName.CompareTo("MON_B3_167") == 0)
                {
                    // Tantarian [MON_B3_126], Gimme Cat [MON_B3_167]
                    ChangeToMonster_SetClip(monsterTransform.motion_alternate, "ANH_" + geoName + "_050", BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_TO_CHANT);
                    ChangeToMonster_SetClip(monsterTransform.motion_alternate, "ANH_" + geoName + "_051", BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT);
                    ChangeToMonster_SetClip(monsterTransform.motion_alternate, "ANH_" + geoName + "_052", BattlePlayerCharacter.PlayerMotionIndex.MP_MAGIC);
                }
                else if (geoName.CompareTo("MON_B3_146") == 0)
                {
                    // Hades [MON_B3_146]
                    ChangeToMonster_SetClip(monsterTransform.motion_alternate, "ANH_" + geoName + "_080", BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_TO_CHANT);
                    ChangeToMonster_SetClip(monsterTransform.motion_alternate, "ANH_" + geoName + "_081", BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT);
                    ChangeToMonster_SetClip(monsterTransform.motion_alternate, "ANH_" + geoName + "_082", BattlePlayerCharacter.PlayerMotionIndex.MP_MAGIC);
                }
                else
                {
                    // Serpion [MON_B3_046], Torama [MON_B3_082], Lich [MON_B3_140], Crystal Lich [MON_B3_191], Deathguise [MON_B3_147], Gargoyle (?) [MON_B3_072]
                    ChangeToMonster_SetClip(monsterTransform.motion_alternate, "ANH_" + geoName + "_030", BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_TO_CHANT);
                    ChangeToMonster_SetClip(monsterTransform.motion_alternate, "ANH_" + geoName + "_031", BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT);
                    ChangeToMonster_SetClip(monsterTransform.motion_alternate, "ANH_" + geoName + "_032", BattlePlayerCharacter.PlayerMotionIndex.MP_MAGIC);
                }
            }
            // Duplicate existing animations or create dummy ones but have different names for btl_mot.checkMotion proper functioning
            HashSet<String> uniqueAnimList = new HashSet<String>();
            for (i = 0; i < 68; i++)
            {
                String baseName = i < 34 ? monsterTransform.motion_normal[i] : monsterTransform.motion_alternate[i - 34];
                if (String.IsNullOrEmpty(baseName) || uniqueAnimList.Contains(baseName))
                {
                    String newName = "ANH_" + geoName + "_DUMMY_" + i;
                    if (!String.IsNullOrEmpty(baseName) && Data.gameObject.GetComponent<Animation>().GetClip(baseName) != null)
                        Data.gameObject.GetComponent<Animation>().AddClip(Data.gameObject.GetComponent<Animation>().GetClip(baseName), newName);
                    else
                        AnimationClipReader.CreateDummyAnimationClip(Data.gameObject, newName);
                    if (i < 34)
                        monsterTransform.motion_normal[i] = newName;
                    else
                        monsterTransform.motion_alternate[i - 34] = newName;
                }
                else
                {
                    if (Data.gameObject.GetComponent<Animation>().GetClip(baseName) == null)
                        AnimationClipReader.CreateDummyAnimationClip(Data.gameObject, baseName);
                    uniqueAnimList.Add(baseName);
                }
            }
            Data.bi.def_idle = 0;
            btl_mot.setMotion(Data, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_NORMAL);
            // Add monster statuses and status resistances
            BattleStatus current_added = 0;
            monsterTransform.resist_added = 0;
            monsterTransform.auto_added = 0;
            if (attackAA[0] == null)
                monsterTransform.resist_added |= BattleStatus.Berserk | BattleStatus.Confuse;
            foreach (SupportingAbilityFeature saFeature in Data.saMonster)
            {
                saFeature.GetStatusInitQuietly(this, out BattleStatus permanent, out BattleStatus initial, out BattleStatus resist, out StatusModifier partialResist, out StatusModifier durationFactor, out Int16 atb);
                current_added |= initial;
                monsterTransform.resist_added |= resist;
                monsterTransform.auto_added |= permanent;
            }
            btl_stat.RemoveStatuses(Data, monsterTransform.resist_added);
            monsterTransform.resist_added &= ~ResistStatus;
            monsterTransform.auto_added &= ~PermanentStatus;
            ResistStatus |= monsterTransform.resist_added;
            monsterTransform.auto_added &= ~ResistStatus;
            btl_stat.AlterStatuses(Data, current_added);
            btl_stat.MakeStatusesPermanent(Data, monsterTransform.auto_added, true);
            // TODO: handle "partialResist" and "durationFactor" properly (now, they are most likely applied but persist after "ReleaseChangeToMonster")
        }

        private void ChangeToMonster_SetClip(String[] array, String animName, BattlePlayerCharacter.PlayerMotionIndex motion)
        {
            if (Data.gameObject.GetComponent<Animation>().GetClip(animName) != null)
                array[(Int32)motion] = animName;
        }

        public void ReleaseChangeToMonster()
        {
            PLAYER p = FF9StateSystem.Common.FF9.party.member[Position];
            if (Data.monster_transform.replace_point)
            {
                Data.max.hp = p.max.hp;
                Data.max.mp = p.max.mp;
                Data.cur.hp = Math.Min(Data.cur.hp, Data.max.hp);
                Data.cur.mp = Math.Min(Data.cur.mp, Data.max.mp);
            }
            if (Data.monster_transform.replace_stat)
            {
                Strength = p.elem.str;
                Magic = p.elem.mgc;
                Dexterity = p.elem.dex;
                Will = p.elem.wpr;
            }
            if (Data.monster_transform.replace_defence)
            {
                Data.defence.PhysicalDefence = p.defence.PhysicalDefence;
                Data.defence.PhysicalEvade = p.defence.PhysicalEvade;
                Data.defence.MagicalDefence = p.defence.MagicalDefence;
                Data.defence.MagicalEvade = p.defence.MagicalEvade;
            }
            if (Data.monster_transform.replace_element)
                btl_eqp.InitEquipPrivilegeAttrib(p, Data);
            ResistStatus &= ~Data.monster_transform.resist_added;
            btl_stat.MakeStatusesPermanent(Data, Data.monster_transform.auto_added, false);
            Data.mesh_current = 0;
            Data.mesh_banish = UInt16.MaxValue;
            Data.tar_bone = 0;
            CharacterBattleParameter btlParam = btl_mot.BattleParameterList[p.info.serial_no];
            Data.shadow_bone[0] = btlParam.ShadowData[0];
            Data.shadow_bone[1] = btlParam.ShadowData[1];
            btl_util.SetShadow(Data, btlParam.ShadowData[2], btlParam.ShadowData[3]);
            Data.saMonster.Clear();
            btl_cmd.KillSpecificCommand(Data, Data.monster_transform.new_command);
            btl_cmd.KillSpecificCommand(Data, BattleCommandId.EnemyCounter);
            Data.gameObject.SetActive(false);
            Data.gameObject = Data.originalGo;
            Data.geo_scale_default = 4096;
            geo.geoScaleReset(Data);
            if (battle.TRANCE_GAUGE_FLAG != 0 && (p.category & 16) == 0 && (Data.bi.slot_no != (Byte)CharacterId.Garnet || battle.GARNET_DEPRESS_FLAG == 0))
                Data.bi.t_gauge = 1;
            // Reset the position even if ChangeToMonster doesn't change it by itself
            Data.pos.x = (Data.evt.posBattle.x = (Data.evt.pos[0] = (Data.base_pos.x = Data.original_pos.x)));
            Data.pos.y = (Data.evt.posBattle.y = (Data.evt.pos[1] = (Data.base_pos.y = (Data.original_pos.y + (!btl_stat.CheckStatus(Data, BattleStatus.Float) ? 0 : -200)))));
            Data.pos.z = (Data.evt.posBattle.z = (Data.evt.pos[2] = (Data.base_pos.z = (Data.original_pos.z + (Data.bi.row == 0 ? -400 : 0)))));
            Data.mot = Data.monster_transform.motion_normal;
            for (Int32 i = 0; i < 34; i++)
                Data.mot[i] = btlParam.AnimationId[i];
            if (Data.cur.hp == 0)
                btl_mot.setMotion(Data, BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE);
            else
                btl_mot.setMotion(Data, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_NORMAL);
            Data.evt.animFrame = 0;
            Data.gameObject.SetActive(true);
            btl_mot.HideMesh(Data, UInt16.MaxValue);
            Data.monster_transform.fade_counter = 2;
            UIManager.Battle.ClearCursorMemorize(Position, Data.monster_transform.new_command);
            Data.is_monster_transform = false;
        }
    }
}
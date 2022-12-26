using System;
using System.Linq;
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

        internal BTL_DATA Data;

        internal BattleUnit(BTL_DATA data)
        {
            Data = data;
        }

        public BTL_DATA GetData
        {
            get => Data;
        }

        public UInt16 Id => Data.btl_id;
        public Boolean IsPlayer => Data.bi.player != 0;
        public Boolean IsSelected => Data.bi.target != 0;
        public Boolean IsSlave => Data.bi.slave != 0;
        public Boolean IsOutOfReach
        {
            get => Data.out_of_reach;
            set => Data.out_of_reach = value;
        }
        public Boolean CanMove => Data.bi.atb != 0;
        public CharacterId PlayerIndex => IsPlayer ? (CharacterId)Data.bi.slot_no : CharacterId.NONE;

        public Byte Level => Data.level;
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

        public Byte Strength
        {
            get => Data.elem.str;
            set => Data.elem.str = value;
        }

        public Byte PhisicalDefence
        {
            get => Data.defence.PhisicalDefence;
            set => Data.defence.PhisicalDefence = value;
        }

        public Byte PhisicalEvade
        {
            get => Data.defence.PhisicalEvade;
            set => Data.defence.PhisicalEvade = value;
        }

        public Byte Magic
        {
            get => Data.elem.mgc;
            set => Data.elem.mgc = value;
        }

        public Byte MagicDefence
        {
            get => Data.defence.MagicalDefence;
            set => Data.defence.MagicalDefence = value;
        }

        public Byte MagicEvade
        {
            get => Data.defence.MagicalEvade;
            set => Data.defence.MagicalEvade = value;
        }

        public Byte Dexterity
        {
            get => Data.elem.dex;
            set => Data.elem.dex = value;
        }
        public Byte Will
        {
            get => Data.elem.wpr;
            set => Data.elem.wpr = value;
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

        public Byte WeaponRate => Data.weapon != null ? Data.weapon.Ref.Rate : (Byte)0;
        public Byte WeaponPower => Data.weapon != null ? Data.weapon.Ref.Power : (Byte)0;
        public EffectElement WeaponElement => (EffectElement)(Data.weapon != null ? Data.weapon.Ref.Elements : 0);
        public BattleStatus WeaponStatus => Data.weapon != null ? FF9StateSystem.Battle.FF9Battle.add_status[Data.weapon.StatusIndex].Value : 0;
        public Byte GetWeaponPower(BattleCommand cmd)
        {
            return Data.weapon == null ? (Byte)0
              : Configuration.Battle.CustomBattleFlagsMeaning == 1 && FF9StateSystem.Battle.FF9Battle.btl_scene.Info.ReverseAttack && cmd != null && (cmd.AbilityType & 0x8) != 0 ? (Byte)Math.Max(1, 60 - Data.weapon.Ref.Power)
              : Data.weapon.Ref.Power;
        }

        public Character Player => Character.Find(this);
        public CharacterSerialNumber SerialNumber => btl_util.getSerialNumber(Data);
        public CharacterCategory PlayerCategory => IsPlayer ? Player.Category : 0;
        public EnemyCategory Category => IsPlayer ? EnemyCategory.Humanoid : (EnemyCategory)btl_util.getEnemyTypePtr(Data).category;
        public WeaponCategory WeapCategory => (WeaponCategory)(Data.weapon != null ? Data.weapon.Category : 0);
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
            set => Data.stat.invalid = value;
        }

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

        public WeaponItem Weapon => (WeaponItem)btl_util.getWeaponNumber(Data);
        public Byte Head => IsPlayer ? FF9StateSystem.Common.FF9.GetPlayer(PlayerIndex).equip.Head : (Byte)255;
        public Byte Wrist => IsPlayer ? FF9StateSystem.Common.FF9.GetPlayer(PlayerIndex).equip.Wrist : (Byte)255;
        public Byte Armor => IsPlayer ? FF9StateSystem.Common.FF9.GetPlayer(PlayerIndex).equip.Armor : (Byte)255;
        public Byte Accessory => IsPlayer ? FF9StateSystem.Common.FF9.GetPlayer(PlayerIndex).equip.Accessory : (Byte)255;
        public Boolean IsHealingRod => IsPlayer && Weapon == WeaponItem.HealingRod;

        public Boolean[] StatModifier => this.Data.stat_modifier;

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

        public UInt16 SummonCount => Data.summon_count;
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

        public MutableBattleCommand AttackCommand => Commands[0];

        private MutableBattleCommand[] _commands;
        public MutableBattleCommand[] Commands => _commands ??= Data.cmd.Select(cmd => new MutableBattleCommand(cmd)).ToArray();

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

            return ((WeaponCategory)Data.weapon.Category & category) != 0;
        }

        public Boolean HasSupportAbility(SupportAbility1 ability)
        {
            return (Data.sa[0] & (UInt32)ability) != 0;
        }

        public Boolean HasSupportAbility(SupportAbility2 ability)
        {
            return (Data.sa[1] & (UInt32)ability) != 0;
        }

        public Boolean TryRemoveStatuses(BattleStatus status)
        {
            return btl_stat.RemoveStatuses(Data, status) == 2U;
        }

        public void RemoveStatus(BattleStatus status)
        {
            btl_stat.RemoveStatus(Data, status);
        }

        public void AlterStatus(BattleStatus status)
        {
            btl_stat.AlterStatus(Data, status);
        }

        public void Kill()
        {
            CurrentHp = 0; // When using the 10 000 HP enemy threshold system (with CustomBattleFlagsMeaning == 1), Kill only set the enemy's HP to 1 assuming it will trigger its dying sequence
            if (Data.cur.hp > 0) // Also, let the script handle the animations and sounds in that case
                return;

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

        public void Remove()
        {
            battle.btl_bonus.member_flag &= (Byte)~(1 << Data.bi.line_no);
            btl_cmd.ClearSysPhantom(Data);
            btl_cmd.KillCommand3(Data);
            btl_sys.SavePlayerData(Data, true);
            btl_sys.DelCharacter(Data);
            Data.SetDisappear(true, 5);
            // The two following lines have been switched for fixing an UI bug (ATB bar glowing, etc... when an ally is snorted)
            // It seems to fix the bug without introducing another one (the HP/MP figures update strangely but that's because of how the UI cells are managed)
            UIManager.Battle.RemovePlayerFromAction(Data.btl_id, true);
            UIManager.Battle.DisplayParty();
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

        public void Libra()
        {
            UIManager.Battle.SetBattleLibra(this);
        }

        public void Detect()
        {
            UIManager.Battle.SetBattlePeeping(this);
        }

        public Int32 GetIndex()
        {
            Int32 index = 0;

            while (1 << index != Data.btl_id)
                ++index;

            return index;
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
            Int32 i;
            SB2_MON_PARM monsterParam = scene.MonAddr[monsterIndex];
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
                Data.defence.PhisicalDefence = monsterParam.PhysicalDefence;
                Data.defence.PhisicalEvade = monsterParam.PhysicalEvade;
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
            btlseq.btlseqinstance seqreader = new btlseq.btlseqinstance();
            btlseq.ReadBattleSequence(btlName, ref seqreader);
            seqreader.FixBuggedAnimations(scene);
            List<AA_DATA> aaList = new List<AA_DATA>();
            AA_DATA attackAA = null;
            String[] battleRawText = FF9TextTool.GetBattleText(FF9BattleDB.SceneData["BSC_" + btlName]);
            if (battleRawText == null)
                battleRawText = new String[0];
            Int32 sequenceSfx;
            Boolean sequenceChannel, sequenceContact;
            // In order to have more than 1 character transformation, one needs to rewrite the way the new AA are handled (either by rewriting BattleHUD thoroughly or using a better way to insert new AA in the FF9Battle.aa_data list)
            for (i = 0; i < scene.header.AtkCount && aaList.Count < 63; i++)
			{
                if (seqreader.GetEnemyIndexOfSequence(i) != monsterIndex)
                    continue;
                if (scene.atk[i].Ref.ScriptId == 64) // Usually scripted dialogs
                    continue;
                //if (scene.atk[i].Ref.ScriptId == 8 || scene.atk[i].Ref.ScriptId == 100) // Enemy attack / Enemy accurate attack
				//{
                //    attackAA = scene.atk[i];
                //    continue;
                //}
                sequenceSfx = seqreader.GetSFXOfSequence(i, out sequenceChannel, out sequenceContact);
                if (sequenceSfx >= 0)
				{
                    scene.atk[i].Info.VfxIndex = (Int16)sequenceSfx;
                    if (Configuration.Battle.SFXRework && scene.atk[i].Info.VfxAction == null)
                        scene.atk[i].Info.VfxAction = new UnifiedBattleSequencer.BattleAction(scene, seqreader, textid => battleRawText[textid], i);
                    if (sequenceContact)
                    {
                        attackAA = scene.atk[i];
                        continue;
                    }
				}
                // Swap the TargetType but keep the DefaultAlly flag since it is on by default only for curative/buffing enemy spells
                if (scene.atk[i].Info.Target == TargetType.AllAlly)
                    scene.atk[i].Info.Target = TargetType.AllEnemy;
                else if (scene.atk[i].Info.Target == TargetType.AllEnemy)
                    scene.atk[i].Info.Target = TargetType.AllAlly;
                else if (scene.atk[i].Info.Target == TargetType.ManyAlly)
                    scene.atk[i].Info.Target = TargetType.ManyEnemy;
                else if (scene.atk[i].Info.Target == TargetType.ManyEnemy)
                    scene.atk[i].Info.Target = TargetType.ManyAlly;
                else if (scene.atk[i].Info.Target == TargetType.RandomAlly)
                    scene.atk[i].Info.Target = TargetType.RandomEnemy;
                else if (scene.atk[i].Info.Target == TargetType.RandomEnemy)
                    scene.atk[i].Info.Target = TargetType.RandomAlly;
                else if (scene.atk[i].Info.Target == TargetType.SingleAlly)
                    scene.atk[i].Info.Target = TargetType.SingleEnemy;
                else if (scene.atk[i].Info.Target == TargetType.SingleEnemy)
                    scene.atk[i].Info.Target = TargetType.SingleAlly;
                if (scene.header.TypCount + i < battleRawText.Length)
                    scene.atk[i].Name = battleRawText[scene.header.TypCount + i];
                aaList.Add(scene.atk[i]);
            }
            CharacterCommands.Commands[(Byte)commandAsMonster].Type = CharacterCommandType.Ability;
            CharacterCommands.Commands[(Byte)commandAsMonster].Abilities = new Byte[aaList.Count];
            for (i = 0; i < aaList.Count; i++)
            {
                CharacterCommands.Commands[(Byte)commandAsMonster].Abilities[i] = (Byte)(192 + i);
                FF9StateSystem.Battle.FF9Battle.aa_data[192 + i] = aaList[i];
                FF9TextTool.SetActionAbilityName(192 + i, aaList[i].Name);
                //FF9TextTool.SetActionAbilityHelpDesc(192 + i, "");
            }
            FF9StateSystem.Battle.FF9Battle.aa_data[192 + aaList.Count] = attackAA;
            FF9TextTool.SetActionAbilityName(192 + aaList.Count, String.Empty);
            Data.is_monster_transform = true;
            UIManager.Battle.ClearCursorMemorize(Position, commandAsMonster);
            Data.monster_transform = new BTL_DATA.MONSTER_TRANSFORM();
            Data.monster_transform.base_command = commandToReplace;
            Data.monster_transform.new_command = commandAsMonster;
            Data.monster_transform.attack = attackAA == null ? 0 : (UInt32)(192 + aaList.Count);
            Data.monster_transform.replace_point = updatePts;
            Data.monster_transform.replace_stat = updateStat;
            Data.monster_transform.replace_defence = updateDef;
            Data.monster_transform.replace_element = updateElement;
            Data.monster_transform.cancel_on_death = cancelOnDeath;
            Data.monster_transform.death_sound = monsterParam.DieSfx;
            Data.monster_transform.fade_counter = 0;
            for (i = 0; i < 3; i++)
                Data.monster_transform.cam_bone[i] = monsterParam.Bone[i];
            for (i = 0; i < 6; i++)
            {
                Data.monster_transform.icon_bone[i] = monsterParam.IconBone[i];
                Data.monster_transform.icon_y[i] = monsterParam.IconY[i];
                Data.monster_transform.icon_z[i] = monsterParam.IconZ[i];
            }
            if (disableCommands == null)
                Data.monster_transform.disable_commands = new List<BattleCommandId>();
            else
                Data.monster_transform.disable_commands = disableCommands;
            Data.monster_transform.resist_added = 0;
            if (attackAA == null)
                Data.monster_transform.resist_added |= BattleStatus.Berserk | BattleStatus.Confuse;
            btl_stat.RemoveStatuses(Data, Data.monster_transform.resist_added);
            Data.monster_transform.resist_added &= ~ResistStatus;
            ResistStatus |= Data.monster_transform.resist_added;
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
            for (i = 0; i < Data.mot.Length; i++)
                Data.mot[i] = String.Empty;
            Boolean useAlternateAnim = geoName.CompareTo("MON_B3_072") == 0; // Gargoyle (to be completed)
            Boolean useDieDmg = (monsterParam.Flags & 2) != 0;
            Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_NORMAL] = FF9BattleDB.Animation[monsterParam.Mot[useAlternateAnim ? 1 : 0]];
            Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_DYING] = Data.mot[0];
            Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE1] = FF9BattleDB.Animation[monsterParam.Mot[useAlternateAnim ? 3 : 2]];
            Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DAMAGE2] = Data.mot[2];
            Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DISABLE] = String.Empty;
            Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DOWN_DISABLE] = FF9BattleDB.Animation[monsterParam.Mot[useDieDmg ? 3 : useAlternateAnim ? 5 : 4]];
            Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_CMD] = Data.mot[0];
            Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_DEFENCE] = Data.mot[0];
            Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_COVER] = Data.mot[0];
            Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_AVOID] = Data.mot[0];
            Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_ESCAPE] = Data.mot[0];
            Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_WIN_LOOP] = Data.mot[0];
            Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT] = Data.mot[0];
            Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_STEP_FORWARD] = Data.mot[0];
            Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_STEP_BACK] = Data.mot[0];
            // Try to automatically get a few animations
            // Physical attack
            if (geoName.CompareTo("MON_B3_147") == 0 && Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_040") != null)
            {
                Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_SET] = "ANH_" + geoName + "_040"; // Deathguise's Spin
                if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_041") != null)
                    Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_RUN] = "ANH_" + geoName + "_041";
                if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_042") != null)
                    Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_RUN_TO_ATTACK] = "ANH_" + geoName + "_042";
                if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_043") != null)
                    Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_ATTACK] = "ANH_" + geoName + "_043";
                if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_044") != null)
                    Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_BACK] = "ANH_" + geoName + "_044";
                if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_045") != null)
                    Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_ATK_TO_NORMAL] = "ANH_" + geoName + "_045";
            }
            else if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_010") != null)
            {
                Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_SET] = "ANH_" + geoName + "_010";
                if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_011") != null)
                    Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_RUN] = "ANH_" + geoName + "_011";
                if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_012") != null)
                    Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_RUN_TO_ATTACK] = "ANH_" + geoName + "_012";
                if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_013") != null)
                    Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_ATTACK] = "ANH_" + geoName + "_013";
                if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_014") != null)
                    Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_BACK] = "ANH_" + geoName + "_014";
                if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_015") != null)
                    Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_ATK_TO_NORMAL] = "ANH_" + geoName + "_015";
            }
            else if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_030") != null)
            {
                Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_SET] = "ANH_" + geoName + "_030";
                if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_031") != null)
                    Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_RUN] = "ANH_" + geoName + "_031";
                if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_032") != null)
                    Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_RUN_TO_ATTACK] = "ANH_" + geoName + "_032";
                if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_033") != null)
                    Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_ATTACK] = "ANH_" + geoName + "_033";
                if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_034") != null)
                    Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_BACK] = "ANH_" + geoName + "_034";
                if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_035") != null)
                    Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_ATK_TO_NORMAL] = "ANH_" + geoName + "_035";
            }
            // Cast Init / Loop / End
            if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_020") != null)
                Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_TO_CHANT] = "ANH_" + geoName + "_020";
            if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_021") != null)
                Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_CHANT] = "ANH_" + geoName + "_021";
            if (Data.gameObject.GetComponent<Animation>().GetClip("ANH_" + geoName + "_022") != null)
                Data.mot[(Int32)BattlePlayerCharacter.PlayerMotionIndex.MP_MAGIC] = "ANH_" + geoName + "_022";
            // Duplicate existing animations or create dummy ones but have different names for btl_mot.checkMotion proper functioning
            HashSet<String> uniqueAnimList = new HashSet<String>();
            for (i = 0; i < Data.mot.Length; i++)
			{
                if (String.IsNullOrEmpty(Data.mot[i]) || uniqueAnimList.Contains(Data.mot[i]))
                {
                    String newName = "ANH_" + geoName + "_DUMMY_" + i;
                    if (!String.IsNullOrEmpty(Data.mot[i]) && Data.gameObject.GetComponent<Animation>().GetClip(Data.mot[i]) != null)
                        Data.gameObject.GetComponent<Animation>().AddClip(Data.gameObject.GetComponent<Animation>().GetClip(Data.mot[i]), newName);
                    else
                        AnimationClipReader.CreateDummyAnimationClip(Data.gameObject, newName);
                    Data.mot[i] = newName;
                }
                else
				{
                    if (Data.gameObject.GetComponent<Animation>().GetClip(Data.mot[i]) == null)
                        AnimationClipReader.CreateDummyAnimationClip(Data.gameObject, Data.mot[i]);
                    uniqueAnimList.Add(Data.mot[i]);
                }
            }
            btl_mot.setMotion(Data, BattlePlayerCharacter.PlayerMotionIndex.MP_IDLE_NORMAL);
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
                Data.defence.PhisicalDefence = p.defence.PhisicalDefence;
                Data.defence.PhisicalEvade = p.defence.PhisicalEvade;
                Data.defence.MagicalDefence = p.defence.MagicalDefence;
                Data.defence.MagicalEvade = p.defence.MagicalEvade;
            }
            if (Data.monster_transform.replace_element)
                btl_eqp.InitEquipPrivilegeAttrib(p, Data);
            ResistStatus &= ~Data.monster_transform.resist_added;
            Data.mesh_current = 0;
            Data.mesh_banish = UInt16.MaxValue;
            Data.tar_bone = 0;
            CharacterBattleParameter btlParam = btl_mot.BattleParameterList[(Int16)p.info.serial_no];
            Data.shadow_bone[0] = btlParam.ShadowData[0];
            Data.shadow_bone[1] = btlParam.ShadowData[1];
            btl_util.SetShadow(Data, btlParam.ShadowData[2], btlParam.ShadowData[3]);
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
            Data.is_monster_transform = false;
        }
    }
}
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria.Data;
using Memoria.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;

namespace Memoria
{
    public static class UiState
    {
        public static void SetBattleFollowMessage(BattleMesages message, EffectElement element)
        {
            UIManager.Battle.SetBattleFollowMessage(message, UIManager.Battle.BtlGetAttrName((Int32)element));
        }

        public static void SetBattleFollowFormatMessage(BattleMesages message, params Object[] args)
        {
            UIManager.Battle.SetBattleFollowMessage(message, args != null && args.Length > 0 ? args[0] : null);
        }

        public static void SetBattleFollowFormatMessage(Byte priority, String formatMessage, params Object[] args)
        {
            UIManager.Battle.SetBattleFollowMessage(priority, formatMessage, args);
        }
    }

    public static class GameState
    {
        public static Int16 Frogs => FF9StateSystem.Common.FF9.Frogs.Number;
        public static Int16[] CategoryKillCount => FF9StateSystem.Common.FF9.categoryKillCount;
        public static Int16 ModelKillCount(Int16 modelId) => FF9StateSystem.Common.FF9.modelKillCount[modelId];
        public static Int32 TotalKillCount => FF9StateSystem.Common.FF9.modelKillCount.Values.Sum(count => count);
        public static Byte Tonberies => battle.TONBERI_COUNT;
        public static UInt16 EscapeCount => FF9StateSystem.Common.FF9.party.escape_no;
        public static Int32 BattleCount => FF9StateSystem.Common.FF9.party.battle_no;
        public static Int32 StepCount => FF9StateSystem.EventState.gStepCount;
        public static Int16 TetraMasterWin => FF9StateSystem.MiniGame.SavedData.sWin;
        public static Int16 TetraMasterLoss => FF9StateSystem.MiniGame.SavedData.sLose;
        public static Int16 TetraMasterDraw => FF9StateSystem.MiniGame.SavedData.sDraw;
        public static Int32 TetraMasterCardCount => QuadMistDatabase.MiniGame_GetAllCardCount();
        public static Int32 TetraMasterPlayerPoints => QuadMistDatabase.MiniGame_GetPlayerPoints();
        public static Int32 TetraMasterPlayerRank => QuadMistDatabase.MiniGame_GetCollectorLevel();
        public static Int32 TreasureHunterPoints => FF9StateSystem.EventState.GetTreasureHunterPoints();
        public static Int32 GameTime => Convert.ToInt32(FF9StateSystem.Settings.time);
        public static Int32 AbilityUsage(BattleAbilityId index) => FF9StateSystem.EventState.GetAAUsageCounter(index);
        public static Int32 ItemCount(RegularItem id) => ff9item.FF9Item_GetCount(id);
        public static Boolean HasKeyItem(Int32 id) => ff9item.FF9Item_IsExistImportant(id);
        public static Int32 GetCardCountOfType(TetraMasterCardId id) => QuadMistDatabase.MiniGame_GetCardCount(id);
        public static Int32 PartyLevel(Int32 index) => index < 0 || index >= 4 ? 0 : FF9StateSystem.Common.FF9.party.member[index]?.level ?? 0;
        public static Single PartyAverageLevel => (Single)(FF9StateSystem.Common.FF9.party.member.Average(p => p?.level) ?? 0);
        public static CharacterId PartyCharacterId(Int32 index) => index < 0 || index >= 4 ? CharacterId.NONE : FF9StateSystem.Common.FF9.party.GetCharacterId(index);
        public static Byte[] GeneralVariable => FF9StateSystem.EventState.gEventGlobal;
        public static Int32 ScenarioCounter => FF9StateSystem.EventState.ScenarioCounter;

        public static UInt32 Gil
        {
            get { return FF9StateSystem.Common.FF9.party.gil; }
            set { FF9StateSystem.Common.FF9.party.gil = Math.Min(9999999U, value); }
        }

        public static Int16 Thefts
        {
            get { return FF9StateSystem.Common.FF9.steal_no; }
            set { FF9StateSystem.Common.FF9.steal_no = Math.Min((Int16)9999, value); }
        }
    }

    public static class BattleState
    {
        public static Boolean IsSpecialStart => FF9StateSystem.Battle.FF9Battle.btl_scene.Info.SpecialStart;
        public static Boolean IsBattleStateEnabled => UIManager.Battle.FF9BMenu_IsEnable();
        public static Boolean IsATBEnabled => HonoluluBattleMain.counterATB > 0 || (UIManager.Battle.FF9BMenu_IsEnable() && UIManager.Battle.FF9BMenu_IsEnableAtb());
        public static Int32 ATBTickCount => HonoluluBattleMain.counterATB; // Number of times the ATB advanced this tick (it can be 2 or more only in turn-based and fast speed modes)
        public static Int32 SharedATBSpeedCoef => btl_para.GetATBCoef(); // Default increment for each ATB advancement
        public static Boolean IsRandomBattle => FF9StateSystem.Battle.isRandomEncounter && !IsFriendlyBattle;
        public static Boolean IsFriendlyBattle => ff9.w_friendlyBattles.Contains((UInt16)FF9StateSystem.Battle.battleMapIndex);
        public static Boolean IsRagtimeBattle => ff9.w_ragtimeBattles.Contains((UInt16)FF9StateSystem.Battle.battleMapIndex);
        public static Boolean IsFlee => FF9StateSystem.Common.FF9.btl_result == FF9StateGlobal.BTL_RESULT_ESCAPE;
        public static Boolean IsFleeByLuck => FF9StateSystem.Common.FF9.btl_result == FF9StateGlobal.BTL_RESULT_ESCAPE && (FF9StateSystem.Common.FF9.btl_flag & battle.BTL_FLAG_ABILITY_FLEE) == 0;
        public static BattleCommand EscapeCommand => new BattleCommand(FF9StateSystem.Battle.FF9Battle.cmd_escape);
        public static Int32 TargetCount(Boolean isPlayer)
        {
            return (Int32)btl_util.SumOfTarget(isPlayer ? 1u : 0u);
        }

        public static Int32 BattleUnitCount(Boolean isPlayer)
        {
            Int32 count = 0;
            Byte playerByte = isPlayer ? (Byte)1 : (Byte)0;
            for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
                if (next.bi.player == playerByte)
                    count++;
            return count;
        }

        public static IEnumerable<BattleUnit> EnumerateUnits()
        {
            for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
                yield return new BattleUnit(next);
        }

        public static void Unit2DReq(BattleUnit unit)
        {
            btl2d.Btl2dReq(unit.Data);
        }

        public static void EnqueueCommand(BattleCommand escapeCommand, BattleCommandId commandId, BattleAbilityId abilityId, UInt16 targetId, Boolean isManyTarget)
        {
            btl_cmd.SetCommand(escapeCommand.Data, commandId, (Int32)abilityId, targetId, isManyTarget ? 1u : 0u);
        }

        public static void EnqueueCounter(BattleUnit unit, BattleCommandId commandId, BattleAbilityId abilityId, UInt16 target)
        {
            btl_cmd.SetCounter(unit.Data, commandId, (Int32)abilityId, target);
        }

        public static UInt16 GetRandomUnitId(Boolean isPlayer)
        {
            return btl_util.GetRandomBtlID(isPlayer ? 1U : 0U);
        }

        public static UInt16 GetUnitIdsUnderStatus(Boolean? isEnemy, BattleStatus dying)
        {
            if (isEnemy == false)
                return btl_util.GetStatusBtlID(0, dying);
            if (isEnemy == true)
                return btl_util.GetStatusBtlID(1, dying);

            return btl_util.GetStatusBtlID(2, dying);
        }

        public static void RaiseAbilitiesAchievement(Int32 abilityId)
        {
            BattleAchievement.UpdateAbilitiesAchievement(abilityId, true);
        }

        public static BattleUnit GetPlayerUnit(CharacterId id)
        {
            foreach (BattleUnit unit in EnumerateUnits())
                if (unit.PlayerIndex == id)
                    return unit;
            return null;
        }
    }

    public sealed class BattleCalculator
    {
        public static readonly IOverloadDamageModifierScript DamageModifierScript = ScriptsLoader.GetOverloadedMethod(typeof(IOverloadDamageModifierScript)) as IOverloadDamageModifierScript;
        public static readonly List<BattleCalculator> FrameAppliedEffectList = new List<BattleCalculator>();

        public readonly CalcContext Context;
        public readonly BattleCommand Command;
        public readonly BattleCaster Caster;
        public readonly BattleTarget Target;
        public Boolean PerformCalcResult = true;

        public BattleCalculator()
        {
            Context = null;
            Command = null;
            Caster = null;
            Target = null;
        }

        public BattleCalculator(BTL_DATA caster, BTL_DATA target, BattleCommand command)
        {
            Context = new CalcContext(this);
            Command = command;
            Caster = new BattleCaster(caster, Context);
            Target = new BattleTarget(target, Context);
            Context.TranceIncrease = (Int16)(Comn.random16() % Target.Will);
        }

        public void NormalMagicParams()
        {
            SetCommandPower();
            Caster.SetMagicAttack();
            Target.SetMagicDefense();
        }

        public void OriginalMagicParams()
        {
            SetWeaponPower();
            Caster.SetLowPhysicalAttack();
            Target.SetMagicDefense();
        }

        public void NormalPhysicalParams()
        {
            SetCommandPower();
            Caster.SetPhysicalAttack();
            Target.SetPhysicalDefense();
        }

        public void WeaponPhysicalParams()
        {
            if (Caster.IsNonMorphedPlayer)
            {
                SetWeaponPower();
                Caster.SetLowPhysicalAttack();
                Target.SetPhysicalDefense();
            }
            else
            {
                NormalPhysicalParams();
            }
        }

        public void WeaponPhysicalParams(CalcAttackBonus bonus)
        {
            Int32 baseDamage = Comn.random16() % (1 + (Caster.Level + Caster.Strength >> 3));
            Context.AttackPower = Caster.GetWeaponPower(Command);
            Target.SetPhysicalDefense();
            switch (bonus)
            {
                case CalcAttackBonus.Simple:
                    Context.Attack = Caster.Strength + baseDamage;
                    break;
                case CalcAttackBonus.WillPower:
                    Context.Attack = (Caster.Strength + Caster.Will >> 1) + baseDamage;
                    break;
                case CalcAttackBonus.Dexterity:
                    Context.Attack = (Caster.Strength + Caster.Data.elem.dex >> 1) + baseDamage;
                    break;
                case CalcAttackBonus.Magic:
                    Context.Attack = (Caster.Strength + Caster.Data.elem.mgc >> 1) + baseDamage;
                    break;
                case CalcAttackBonus.Random:
                    Context.Attack = Comn.random16() % Caster.Strength + baseDamage;
                    break;
                case CalcAttackBonus.Level:
                    Context.AttackPower += Caster.Data.level;
                    Context.Attack = Caster.Strength + baseDamage;
                    break;
            }
        }

        public void MagicAccuracy()
        {
            Context.HitRate = (Int16)(Command.HitRate + (Caster.Magic >> 2) + Caster.Level - Target.Level);

            if (Context.HitRate > 100)
                Context.HitRate = 100;
            else if (Context.HitRate < 1)
                Context.HitRate = 1;

            Context.Evade = Target.MagicEvade;
        }

        public void PhysicalAccuracy()
        {
            Context.HitRate = 100;
            Context.Evade = Target.PhysicalEvade;
        }

        public Boolean CanAttackMagic()
        {
            return CanAttackElementalCommand() && CanUseCommandForFlight();
        }

        public Boolean CanAttackElementalCommand()
        {
            return /*Caster.HasSupportAbility(SupportAbility2.MagElemNull) || */ Target.CanAttackElement(Command.Element);
        }

        public Boolean CanAttackWeaponElementalCommand()
        {
            return Target.CanAttackElement(Caster.WeaponElement);
        }

        public Boolean CanUseCommandForFlight()
        {
            if (Target.IsLevitate && Command.IsGround)
            {
                Context.Flags |= BattleCalcFlags.Miss;
                return false;
            }

            return true;
        }

        public Boolean CanEscape()
        {
            if (!FF9StateSystem.Battle.FF9Battle.btl_scene.Info.Runaway)
                return false;
            return BattleState.EnumerateUnits().Any(unit => unit.IsPlayer && !unit.IsUnderAnyStatus(BattleStatusConst.CannotEscape));
        }

        public Boolean IsCasterNotTarget()
        {
            if (Target.Data != Caster.Data)
                return true;

            Context.Flags |= BattleCalcFlags.Miss;
            return false;
        }

        public Boolean IsCasterSameDirectionTarget()
        {
            // Doesn't take running away or Confuse into account but takes back attack into account
            return Math.Abs(Caster.Data.evt.rotBattle.eulerAngles.y - Target.Data.evt.rotBattle.eulerAngles.y) < 0.1;
        }

        public Boolean IsCasterVisuallySameDirectionTarget()
        {
            // Takes back attacks, Confuse and running away (animation, not key press) into account
            Quaternion target_angle = Target.Data.rot;
            if (Target.IsPlayer && btl_mot.checkMotion(Target.Data, BattlePlayerCharacter.PlayerMotionIndex.MP_ESCAPE))
                target_angle = new Quaternion(0f, 1f, 0f, 0f) * target_angle;
            return Mathf.Abs(Quaternion.Angle(Caster.Data.rot, target_angle)) < 90;
        }

        public void PrepareHpDraining()
        {
            Target.Flags |= CalcFlag.HpAlteration;
            Caster.Flags |= CalcFlag.HpAlteration;

            if (Target.IsZombie)
                Target.Flags |= CalcFlag.HpRecovery;
            else
                Caster.Flags |= CalcFlag.HpRecovery;

            Context.IsDrain = true;
        }

        public void TryEscape()
        {
            CMD_DATA curCmdPtr = btl_util.getCurCmdPtr();
            if (curCmdPtr != null && curCmdPtr.regist.bi.player == 0) // Any enemy is attacking
                return;

            SByte playerCount = 0;
            SByte enemyCount = 0;
            Int16 playerLevels = 0;
            Int16 enemyLevels = 0;

            for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
            {
                BattleUnit unit = new BattleUnit(next);
                if (unit.IsPlayer)
                {
                    ++playerCount;
                    playerLevels += next.level;
                }
                else
                {
                    ++enemyCount;
                    enemyLevels += next.level;
                }
            }

            if (enemyCount == 0)
                enemyCount = 1;
            if (playerCount == 0)
                playerCount = 1;
            if (enemyLevels == 0)
                enemyLevels = 1;
            if (playerLevels == 0)
                playerLevels = 1;

            Int16 rate = (Int16)(200 / (enemyLevels / enemyCount) * (playerLevels / playerCount) / 16);
            if (rate > Comn.random16() % 100)
                btl_cmd.SetCommand(FF9StateSystem.Battle.FF9Battle.cmd_escape, BattleCommandId.SysEscape, 1, 15, 1u);
        }

        public Boolean TryPhysicalHit()
        {
            Caster.PenaltyPhysicalHitRate();
            Caster.BonusPhysicalEvade();
            Target.PenaltyPhysicalEvade();
            Target.PenaltyDefenceHitRate();
            Target.PenaltyBanishHitRate();
            if (Target.IsUnderAnyStatus(BattleStatus.Float))
                Context.Evade += Configuration.Battle.FloatEvadeBonus;

            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Caster))
                saFeature.TriggerOnAbility(this, "HitRateSetup", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Target))
                saFeature.TriggerOnAbility(this, "HitRateSetup", true);

            if (Context.HitRate <= Comn.random16() % 100)
            {
                this.Context.Flags |= BattleCalcFlags.Miss;
                return false;
            }
            if (Target.Data == Caster.Data || Context.Evade <= Comn.random16() % 100)
                return true;

            Context.Flags |= BattleCalcFlags.Miss;
            if (!Target.IsCovering)
                Context.Flags |= BattleCalcFlags.Dodge;

            return false;
        }

        public Boolean TryMagicHit()
        {
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Caster))
                saFeature.TriggerOnAbility(this, "HitRateSetup", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Target))
                saFeature.TriggerOnAbility(this, "HitRateSetup", true);

            if (Context.HitRate <= Comn.random16() % 100)
            {
                Context.Flags |= BattleCalcFlags.Miss;
                return false;
            }

            if (Context.Evade > Comn.random16() % 100)
            {
                Context.Flags |= BattleCalcFlags.Miss;
                return false;
            }

            return true;
        }

        public void TryDirectHPDamage()
        {
            if (Command.Power == 0)
            {
                if (Target.IsZombie)
                {
                    if (Target.CanBeAttacked())
                        Target.CurrentHp = Target.MaximumHp;
                }
                else if (Target.IsUnderAnyStatus(BattleStatus.EasyKill))
                {
                    Context.Flags |= BattleCalcFlags.Guard;
                }
                else
                {
                    Target.Kill(Caster.Data);
                }
                return;
            }

            if (Target.IsUnderStatus(BattleStatus.Death))
            {
                Context.Flags |= BattleCalcFlags.Miss;
                return;
            }

            Context.Flags |= BattleCalcFlags.DirectHP;
            Target.CurrentHp = (UInt32)Command.Power;
            Target.FaceTheEnemy();
        }

        public void TryAlterMagicStatuses()
        {
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Caster))
                saFeature.TriggerOnAbility(this, "HitRateSetup", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Target))
                saFeature.TriggerOnAbility(this, "HitRateSetup", true);

            if (Command.HitRate > Comn.random16() % 100)
                Target.TryAlterStatuses(Command.AbilityStatus, false, Caster);
        }

        public void TryAlterCommandStatuses()
        {
            BattleStatus status = Command.AbilityStatus;
            if (!Command.IsShortSummon && Command.Id == BattleCommandId.SummonEiko)
                status |= BattleStatus.Protect;

            Target.TryAlterStatuses(status, true, Caster);
        }

        public void TryRemoveAbilityStatuses()
        {
            if (!Target.TryRemoveStatuses(Command.AbilityStatus))
                Context.Flags |= BattleCalcFlags.Miss;
        }

        public void TryRemoveItemStatuses()
        {
            if (!Target.TryRemoveStatuses(Command.ItemStatus))
                Context.Flags |= BattleCalcFlags.Miss;
        }

        public Boolean IsTargetLevelMultipleOfCommandRate()
        {
            if (Target.Level % Command.HitRate == 0)
                return true;

            Context.Flags |= BattleCalcFlags.Miss;
            return false;
        }

        public void CalcDamageCommon()
        {
            Target.Flags |= CalcFlag.HpAlteration;
            if (Context.IsAbsorb)
                Target.Flags |= CalcFlag.HpRecovery;

            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Caster))
                saFeature.TriggerOnAbility(this, "CalcDamage", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Target))
                saFeature.TriggerOnAbility(this, "CalcDamage", true);
        }

        public void CalcPhysicalHpDamage()
        {
            CalcDamageCommon();

            Target.HpDamage = Context.EnsureAttack * Context.EnsurePowerDifference;
            if ((Target.Flags & CalcFlag.HpRecovery) != 0)
                Target.FaceTheEnemy();
        }

        public void CalcHpDamage()
        {
            CalcDamageCommon();

            Int32 damage = Context.EnsureAttack * Context.EnsurePowerDifference;
            if (Command.IsShortSummon)
                damage = damage * 2 / 3;

            Target.HpDamage = damage;
        }

        public void CalcMpDamage()
        {
            Target.Flags |= CalcFlag.MpAlteration;
            if (Context.IsAbsorb)
            {
                Target.Flags |= CalcFlag.MpRecovery;
                Context.DefensePower = 0;
            }
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Caster))
                saFeature.TriggerOnAbility(this, "CalcDamage", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Target))
                saFeature.TriggerOnAbility(this, "CalcDamage", true);

            Target.MpDamage = Math.Max(0, Context.PowerDifference) * Context.EnsureAttack >> 2;
        }

        public void CalcProportionDamage()
        {
            CalcDamageCommon();

            if (Context.Attack > 100)
                Context.Attack = 100;

            Int32 damage = (Int32)Target.MaximumHp * Context.Attack / 100;
            if (Command.IsShortSummon)
                damage = damage * 2 / 3;

            Target.HpDamage = damage;
        }

        public void CalcCannonProportionDamage()
        {
            CalcDamageCommon();

            if (Context.Attack > 100)
                Context.Attack = 100;
            Target.HpDamage = (Int32)Target.CurrentHp * Context.Attack / 100;
        }

        public void CalcHpMagicRecovery()
        {
            if (!Target.IsZombie)
                Target.Flags |= CalcFlag.HpRecovery;

            CalcDamageCommon();

            Target.HpDamage = Context.AttackPower * Context.Attack;
        }

        public void CalcMpMagicRecovery()
        {
            Target.Flags |= CalcFlag.MpAlteration;
            if (!Target.IsZombie)
                Target.Flags |= CalcFlag.MpRecovery;

            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Caster))
                saFeature.TriggerOnAbility(this, "CalcDamage", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Target))
                saFeature.TriggerOnAbility(this, "CalcDamage", true);

            Target.MpDamage = Context.AttackPower * Context.Attack;
        }

        public void BonusKillerAbilities()
        {
            // Dummied
            var hasCategory = new Func<ENEMY_TYPE, EnemyCategory, Boolean>((enemy, category) => (enemy.category & (Int16)category) != 0);

            if (!Target.IsPlayer)
            {
                ENEMY_TYPE enemy = btl_util.getEnemyTypePtr(Target.Data);
                if (Caster.HasSupportAbility(SupportAbility1.BirdKiller) && hasCategory(enemy, EnemyCategory.Flight) ||
                    Caster.HasSupportAbility(SupportAbility1.BugKiller) && hasCategory(enemy, EnemyCategory.Soul) ||
                    Caster.HasSupportAbility(SupportAbility1.StoneKiller) && hasCategory(enemy, EnemyCategory.Stone) ||
                    Caster.HasSupportAbility(SupportAbility1.UndeadKiller) && hasCategory(enemy, EnemyCategory.Undead) ||
                    Caster.HasSupportAbility(SupportAbility1.DragonKiller) && hasCategory(enemy, EnemyCategory.Dragon) ||
                    Caster.HasSupportAbility(SupportAbility1.DevilKiller) && hasCategory(enemy, EnemyCategory.Devil) ||
                    Caster.HasSupportAbility(SupportAbility1.BeastKiller) && hasCategory(enemy, EnemyCategory.Beast) ||
                    Caster.HasSupportAbility(SupportAbility1.ManEater) && hasCategory(enemy, EnemyCategory.Humanoid))
                    ++Context.DamageModifierCount;
            }
        }
        public void BonusMpAttack()
        {
            // Dummied
            if (Caster.HasSupportAbility(SupportAbility1.MPAttack) && Caster.CurrentMp > 0)
            {
                ++Context.DamageModifierCount;
                Context.Flags |= BattleCalcFlags.MpAttack;
            }
        }

        public void BonusSupportAbilitiesAttack()
        {
            // Dummied
            BonusKillerAbilities();
            BonusMpAttack();
        }

        public void BonusBackstabAndPenaltyLongDistance()
        {
            if (IsCasterSameDirectionTarget() || Target.IsRunningAway())
                ++Context.DamageModifierCount;

            // Note that there are two weapon categories: SHORT_RANGE and LONG_RANGE
            // LONG_RANGE is used for this penalty while SHORT_RANGE is used both for that and for "out of range" enemies
            if (Mathf.Abs(Caster.Row - Target.Row) > 1 && !Caster.HasLongRangeWeapon && Command.IsShortRange)
                --Context.DamageModifierCount;
        }

        public void BonusBackstabAndPenaltyLongDistanceVisually()
        {
            if (IsCasterVisuallySameDirectionTarget())
                ++Context.DamageModifierCount;

            if (Mathf.Abs(Caster.Row - Target.Row) > 1 && !Caster.HasLongRangeWeapon && Command.IsShortRange)
                --Context.DamageModifierCount;
        }

        public void PenaltyReverseAttack()
        {
            // Ipsen's Castle
            if (!FF9StateSystem.Battle.FF9Battle.btl_scene.Info.ReverseAttack)
                return;

            Context.AttackPower = 60 - Context.AttackPower;
            if (Context.AttackPower < 1)
                Context.AttackPower = 1;
        }

        public void TryCriticalHit()
        {
            if ((Target.Flags & CalcFlag.Critical) != 0) // In case another system triggered a critical strike (with possibly other consequences)
                return;
            Int32 quarterWill = Caster.Data.elem.wpr >> 2;
            if (quarterWill != 0 && (Comn.random16() % quarterWill) + Caster.CriticalRateBonus - Target.CriticalRateResistance > Comn.random16() % 100)
            {
                Context.Attack *= 2; // In case TryCriticalHit is called before "Calc...HpDamage"
                Target.HpDamage *= 2; // In case TryCriticalHit is called after "Calc...HpDamage"
                Target.MpDamage *= 2;
                Target.Flags |= CalcFlag.Critical;
            }
        }

        public void ConsumeMpAttack()
        {
            if ((Context.Flags & BattleCalcFlags.MpAttack) != 0)
                Caster.Data.cur.mp = (UInt32)Math.Max(0, (Int32)Caster.Data.cur.mp - (Caster.MaximumMp >> 3));
        }

        public void SetCommandPower()
        {
            Context.AttackPower = Command.Power;
        }

        public void SetWeaponPower()
        {
            if (Caster.IsNonMorphedPlayer)
                Context.AttackPower = (Int16)(Caster.GetWeaponPower(Command) * Command.Power / 10);
            else
                SetCommandPower();
        }

        public void SetWeaponPowerSum()
        {
            Context.AttackPower = (Int16)(Caster.GetWeaponPower(Command) + Command.Power);
        }

        public void SetCommandAttack()
        {
            Context.Attack = Command.Power;
        }

        public void PenaltyCommandDividedAttack()
        {
            if (Command.IsDevided)
                --Context.DamageModifierCount;
        }

        public void PenaltyCommandDividedHitRate()
        {
            if (Command.IsDevided)
                Context.HitRate /= 2;
        }

        public Boolean CheckHasCommandItem()
        {
            if (ff9item.FF9Item_GetCount((RegularItem)Command.Power) > 0)
                return true;

            UIManager.Battle.SetBattleFollowMessage(BattleMesages.NotEnoughItems);
            Context.Flags |= BattleCalcFlags.Miss;
            return false;
        }

        public void TryAddWeaponStatus()
        {
            // Dummied
            if (Caster.HasSupportAbility(SupportAbility1.AddStatus))
            {
                Context.StatusRate = Caster.WeaponRate;
                if (Context.StatusRate > GameRandom.Next16() % 100)
                    Context.Flags |= BattleCalcFlags.AddStat;
            }
        }

        public Boolean ApplyElementFullStack(EffectElement element, EffectElement elementForBonus)
        {
            if ((element & Target.GuardElement) != 0)
            {
                Context.Flags |= BattleCalcFlags.Guard;
                return false;
            }
            if ((element & Target.AbsorbElement) != 0)
                Context.Flags |= BattleCalcFlags.Absorb;

            Context.DamageModifierCount += Comn.countBits((UInt16)(Caster.BonusElement & elementForBonus));
            Context.DamageModifierCount += Comn.countBits((UInt16)(Target.WeakElement & element));
            Context.DamageModifierCount -= Comn.countBits((UInt16)(Target.HalfElement & element));
            return true;
        }

        public void BonusElement()
        {
            if ((Command.ElementForBonus & Caster.BonusElement) != 0)
                ++Context.DamageModifierCount;
        }

        public void StealItem(BattleEnemy enemy, Int32 slot)
        {
            Context.ItemSteal = enemy.StealableItems[slot];
            if (Context.ItemSteal == RegularItem.NoItem)
            {
                UiState.SetBattleFollowFormatMessage(BattleMesages.CouldNotStealAnything);
                return;
            }

            enemy.StealableItems[slot] = RegularItem.NoItem;
            GameState.Thefts++;

            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Caster))
                saFeature.TriggerOnAbility(this, "Steal", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Target))
                saFeature.TriggerOnAbility(this, "Steal", true);

            BattleItem.AddToInventory(Context.ItemSteal);
            UiState.SetBattleFollowFormatMessage(BattleMesages.Stole, FF9TextTool.ItemName(Context.ItemSteal));
        }

        public void RaiseTrouble()
        {
            if (Command.Data.tar_id == Target.Id && Target.IsUnderAnyStatus(BattleStatusConst.ApplyTrouble & ~Context.AddedStatuses))
                Target.Data.fig_info |= Param.FIG_INFO_TROUBLE;
        }
    }
}

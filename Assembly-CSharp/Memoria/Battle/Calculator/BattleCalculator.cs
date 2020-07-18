using System;
using System.Collections.Generic;
using FF9;
using Memoria.Data;
using UnityEngine;
using Object = System.Object;
using Assets.Sources.Scripts.UI.Common;

namespace Memoria
{
    public static class UiState
    {
        public static void SetBattleFollowMessage(BattleMesages message, EffectElement element)
        {
            UIManager.Battle.SetBattleFollowMessage((Int32)message, UIManager.Battle.BtlGetAttrName((Int32)element));
        }

        public static void SetBattleFollowFormatMessage(BattleMesages message, params Object[] args)
        {
            UIManager.Battle.SetBattleFollowMessage((Int32)message, args);
        }

        public static void SetBattleFollowFormatMessage(Byte priority, String formatMessage, params Object[] args)
        {
            UIManager.Battle.SetBattleFollowMessage(priority, formatMessage, args);
        }
    }

    public static class GameState
    {
        public static Int16 Frogs => FF9StateSystem.Common.FF9.Frogs.Number;
        public static Int16 Dragons => FF9StateSystem.Common.FF9.dragon_no;
        public static Byte Tonberies => battle.TONBERI_COUNT;
        public static Byte AbilityUsage(Byte index) => FF9StateSystem.EventState.GetAAUsageCounter(index);

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
        public static Boolean IsSpecialStart => FF9StateSystem.Battle.FF9Battle.btl_scene.Info.SpecialStart != 0;
        public static BattleCommand EscapeCommand => new BattleCommand(FF9StateSystem.Battle.FF9Battle.cmd_escape);

        public static Int32 TargetCount(Boolean isPlayer)
        {
            return (Int32)btl_util.SumOfTarget(isPlayer ? 1u : 0u);
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
            btl_cmd.SetCommand(escapeCommand.Data, commandId, (UInt32)abilityId, targetId, isManyTarget ? 1u : 0u);
        }

        public static void EnqueueConter(BattleUnit unit, BattleCommandId commandId, BattleAbilityId abilityId, UInt16 target)
        {
            btl_cmd.SetCounter(unit.Data, commandId, (Int32)abilityId, target);
        }

        public static UInt16 GetRandomUnitId(Boolean isPlayer)
        {
            return btl_util.GetRandomBtlID(isPlayer ? 1U : 0U);
        }

        public static UInt16 GetUnitIdsUnderStatus(Boolean? isPlayer, BattleStatus dying)
        {
            if (isPlayer == false)
                return btl_util.GetStatusBtlID(0, dying);
            if (isPlayer == true)
                return btl_util.GetStatusBtlID(1, dying);

            return btl_util.GetStatusBtlID(2, dying);
        }

        public static void RaiseAbilitiesAchievement(Int32 abilityId)
        {
            BattleAchievement.UpdateAbilitiesAchievement(abilityId, true);
        }
    }

    public sealed class BattleCalculator
    {
        public readonly CalcContext Context;
        public readonly BattleCommand Command;
        public readonly BattleCaster Caster;
        public readonly BattleTarget Target;
        public readonly CalcCasterCommand CasterCommand;
        public readonly CalcTargetCommand TargetCommand;

        public BattleCalculator()
        {
            Context = null;
            Command = null;
            Caster = null;
            Target = null;
            CasterCommand = null;
            TargetCommand = null;
        }

        public BattleCalculator(BTL_DATA caster, BTL_DATA target, BattleCommand command)
        {
            Context = new CalcContext();
            Command = command;
            Caster = new BattleCaster(caster, Context);
            Target = new BattleTarget(target, Context);
            CasterCommand = new CalcCasterCommand(Caster, Command, Context);
            TargetCommand = new CalcTargetCommand(Target, Command, Context);
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
            CasterCommand.SetWeaponPower();
            Caster.SetLowPhisicalAttack();
            Target.SetMagicDefense();
        }

        public void NormalPhisicalParams()
        {
            SetCommandPower();
            Caster.SetPhisicalAttack();
            Target.SetPhisicalDefense();
        }

        public void WeaponPhisicalParams()
        {
            CasterCommand.SetWeaponPower();
            Caster.SetLowPhisicalAttack();
            Target.SetPhisicalDefense();
        }

        public void WeaponPhisicalParams(CalcAttackBonus bonus)
        {
            Int32 baseDamage = Comn.random16() % (1 + (Caster.Level + Caster.Strength >> 3));
            Context.AttackPower = Caster.WeaponPower;
            Target.SetPhisicalDefense();
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
            Context.Evade = Target.PhisicalEvade;
        }

        public Boolean CanAttackMagic()
        {
            return CanAttackElementalCommand() && TargetCommand.CanUseCommandForFlight();
        }

        public Boolean CanAttackElementalCommand()
        {
            return /*Caster.HasSupportAbility(SupportAbility2.MagElemNull) || */ Target.CanAttackElement(Command.Element);
        }

        public Boolean CanAttackWeaponElementalCommand()
        {
            return Target.CanAttackElement(Caster.WeaponElement);
        }

        public Boolean CanEscape()
        {
            FF9StateBattleSystem ff9Battle = FF9StateSystem.Battle.FF9Battle;
            if (ff9Battle.btl_scene.Info.Runaway == 0)
                return false;

            for (BTL_DATA next = ff9Battle.btl_list.next; next != null; next = next.next)
            {
                BattleUnit unit = new BattleUnit(next);
                const BattleStatus status = BattleStatus.Petrify | BattleStatus.Poison | BattleStatus.Zombie | BattleStatus.Death | BattleStatus.Stop | BattleStatus.Sleep | BattleStatus.Freeze | BattleStatus.Jump;
                if (next.bi.player != 0 && !unit.IsUnderStatus(status))
                    return true;
            }

            return false;
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
            if (Target.IsPlayer && btl_mot.checkMotion(Target.Data, 17))
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
            if (curCmdPtr != null && curCmdPtr.regist.bi.player == 0)
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
                btl_cmd.SetCommand(FF9StateSystem.Battle.FF9Battle.cmd_escape, BattleCommandId.SysEscape, 1U, 15, 1U);
        }

        public Boolean TryPhysicalHit()
        {
            Caster.PenaltyPhysicalHitRate();
            Caster.BonusPhysicalEvade();
            Target.PenaltyPhysicalEvade();
            Target.PenaltyDefenceHitRate();
            Target.PenaltyBanishHitRate();
            if (Target.IsUnderAnyStatus(BattleStatus.Float))
                Context.Evade += (Int16)Configuration.Battle.FloatEvadeBonus;

            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Caster.Data.sa))
                saFeature.TriggerOnAbility(this, "HitRateSetup", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Target.Data.sa))
                saFeature.TriggerOnAbility(this, "HitRateSetup", true);

            if (Context.HitRate <= Comn.random16() % 100)
            {
                Context.Flags |= BattleCalcFlags.Miss;
                return false;
            }

            if (Target.Data == Caster.Data || Context.Evade <= Comn.random16() % 100)
                return true;

            Context.Flags |= BattleCalcFlags.Miss;
            if (!Target.IsCovered)
                Context.Flags |= BattleCalcFlags.Dodge;

            return false;
        }

        public void TryAlterMagicStatuses()
        {
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Caster.Data.sa))
                saFeature.TriggerOnAbility(this, "HitRateSetup", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Target.Data.sa))
                saFeature.TriggerOnAbility(this, "HitRateSetup", true);

            if (Command.HitRate > Comn.random16() % 100)
                Target.TryAlterStatuses(Command.AbilityStatus, false);
        }

        public Boolean TryMagicHit()
        {
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Caster.Data.sa))
                saFeature.TriggerOnAbility(this, "HitRateSetup", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Target.Data.sa))
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
            {
                Target.Flags |= CalcFlag.HpRecovery;
                Context.DefensePower = 0;
            }

            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Caster.Data.sa))
                saFeature.TriggerOnAbility(this, "CalcDamage", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Target.Data.sa))
                saFeature.TriggerOnAbility(this, "CalcDamage", true);
        }

        public void CalcPhysicalHpDamage()
        {
            if (!Target.IsZombie && Caster.IsHealingRod)
                Target.Flags |= CalcFlag.HpRecovery;

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
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Caster.Data.sa))
                saFeature.TriggerOnAbility(this, "CalcDamage", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Target.Data.sa))
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

            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Caster.Data.sa))
                saFeature.TriggerOnAbility(this, "CalcDamage", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Target.Data.sa))
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
                    Context.Attack = Context.Attack * 3 >> 1;
            }
        }
        public void BonusMpAttack()
        {
            // Dummied
            if (Caster.HasSupportAbility(SupportAbility1.MPAttack) && Caster.CurrentMp > 0)
            {
                Context.Attack = Context.Attack * 3 >> 1;
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
                Context.Attack = Context.Attack * 3 >> 1;

            // Note that there are two weapon categories: SHORT_RANGE and LONG_RANGE
            // LONG_RANGE is used for this penalty while SHORT_RANGE is used both for that and for "out of range" enemies
            if (Mathf.Abs(Caster.Row - Target.Row) > 1 && !Caster.HasLongRangeWeapon && Command.IsShortRange)
                Context.Attack /= 2;
        }

        public void BonusBackstabAndPenaltyLongDistanceAsDamageModifiers()
        {
            if (IsCasterVisuallySameDirectionTarget())
                ++Context.DamageModifierCount;

            if (Mathf.Abs(Caster.Row - Target.Row) > 1 && !Caster.HasLongRangeWeapon && Command.IsShortRange)
                --Context.DamageModifierCount;
        }

        public void PenaltyReverseAttack()
        {
            // Ipsen's Castle
            if (FF9StateSystem.Battle.FF9Battle.btl_scene.Info.ReverseAttack == 0)
                return;

            Context.AttackPower = 60 - Context.AttackPower;
            if (Context.AttackPower < 1)
                Context.AttackPower = 1;
        }

        public void TryCriticalHit()
        {
            Int32 quarterWill = Caster.Data.elem.wpr >> 2;
            if (quarterWill != 0 && (Comn.random16() % quarterWill) + Caster.Data.critical_rate_deal_bonus + Target.Data.critical_rate_receive_bonus > Comn.random16() % 100)
            {
                Context.Attack *= 2;
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

        public void SetCommandAttack()
        {
            Context.Attack = Command.Power;
        }

        public void PenaltyCommandDividedAttack()
        {
            if (Command.IsDevided)
                Context.Attack /= 2;
        }

        public void PenaltyCommandDividedHitRate()
        {
            if (Command.IsDevided)
                Context.HitRate /= 2;
        }

        public Boolean CheckHasCommandItem()
        {
            if (ff9item.FF9Item_GetCount(Command.Power) > 0)
                return true;

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

        public Boolean ApplyElementAsDamageModifiers(EffectElement element, EffectElement elementForBonus)
		{
            if ((element & Target.GuardElement) != 0)
			{
                Context.Flags |= BattleCalcFlags.Guard;
                return false;
            }
            if ((element & Target.AbsorbElement) != 0)
                Context.Flags |= BattleCalcFlags.Absorb;

            Context.DamageModifierCount += (SByte)Comn.countBits((UInt16)(Caster.BonusElement & elementForBonus));
            Context.DamageModifierCount += (SByte)Comn.countBits((UInt16)(Target.WeakElement & element));
            Context.DamageModifierCount -= (SByte)Comn.countBits((UInt16)(Target.HalfElement & element));
            return true;
        }

        public void StealItem(BattleEnemy enemy, Int32 slot)
        {
            Context.ItemSteal = enemy.StealableItems[slot];
            if (Context.ItemSteal == Byte.MaxValue)
            {
                UiState.SetBattleFollowFormatMessage(BattleMesages.CouldNotStealAnything);
                return;
            }

            enemy.StealableItems[slot] = Byte.MaxValue;
            GameState.Thefts++;

            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Caster.Data.sa))
                saFeature.TriggerOnAbility(this, "Steal", false);
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(Target.Data.sa))
                saFeature.TriggerOnAbility(this, "Steal", true);

            BattleItem.AddToInventory(Context.ItemSteal);
            UiState.SetBattleFollowFormatMessage(BattleMesages.Stole, FF9TextTool.ItemName(Context.ItemSteal));
        }

        public void RaiseTrouble()
        {
            if (Command.Data.tar_id == Target.Id && Target.IsUnderStatus(BattleStatus.Trouble) && (Context.AddedStatuses & BattleStatus.Trouble) == 0 && (Target.Flags & CalcFlag.HpRecovery) == 0)
                Target.Data.fig_info |= Param.FIG_INFO_TROUBLE;
        }
    }
}
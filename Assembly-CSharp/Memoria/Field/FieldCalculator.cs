using System;
using System.Collections.Generic;
using UnityEngine;
using Memoria.Data;
using FF9;

namespace Memoria
{
    public sealed class FieldCalculator
    {
        public static void ApplyRecover(PLAYER player, Int32 recover)
        {
            player.cur.hp = (UInt32)Mathf.Clamp(player.cur.hp + recover, 0, player.max.hp);
        }

        public static void ApplyMpRecover(PLAYER player, Int32 recover)
        {
            player.cur.mp = (UInt32)Mathf.Clamp(player.cur.mp + recover, 0, player.max.mp);
        }

        public static Int32 AlterStatus(PLAYER player, BattleStatusId statusId)
        {
            BattleStatus status = statusId.ToBattleStatus();
            if ((player.status & status) != 0)
                return 1;
            Dictionary<BattleStatusId, BattleStatusDataEntry> statusDatabase = FF9StateSystem.Battle.FF9Battle.status_data;
            BattleStatus invalidStatuses = player.cur.hp == 0 ? statusDatabase[BattleStatusId.Death].ImmunityProvided : 0;
            foreach (BattleStatusId curStatus in player.status.ToStatusList())
                invalidStatuses |= statusDatabase[curStatus].ImmunityProvided;
            if ((status & invalidStatuses) != 0)
                return 1;
            player.status |= status;
            return 2;
        }

        public static Int32 AlterStatuses(PLAYER player, BattleStatus statuses)
        {
            Int32 success = 0;
            foreach (BattleStatusId statusId in statuses.ToStatusList())
                success = Math.Max(success, AlterStatus(player, statusId));
            return success;
        }

        public static Int32 RemoveStatus(PLAYER player, BattleStatusId statusId)
        {
            BattleStatus status = statusId.ToBattleStatus();
            if ((player.permanent_status & status) != 0)
                return 0;
            if ((player.status & status) == 0)
                return 1;
            player.status &= ~status;
            return 2;
        }

        public static Int32 RemoveStatuses(PLAYER player, BattleStatus statuses)
        {
            Int32 success = 0;
            foreach (BattleStatusId statusId in statuses.ToStatusList())
                success = Math.Max(success, RemoveStatus(player, statusId));
            return success;
        }

        public static Boolean CheckStatus(PLAYER player, BattleStatus status)
        {
            return (player.status & status) != 0;
        }

        public PLAYER Caster;
        public PLAYER Target;
        public ActionData Action;
        public UInt32 Cursor;
        public BattleCalcFlags Flags = 0;
        public Int32 AttackPower = 0;
        public Int32 DefencePower = 0;
        public Int32 Attack = 0;
        public Int32 CasterRecoverHp = 0;
        public Int32 CasterRecoverMp = 0;
        public Int32 TargetRecoverHp = 0;
        public Int32 TargetRecoverMp = 0;

        public Boolean IsDivided => Cursor == 1 && Action.Info.Target >= TargetType.ManyAny && Action.Info.Target <= TargetType.ManyEnemy;
        public Boolean IsItem => Action.ItemId != RegularItem.NoItem;

        public Boolean CanBeHealed(PLAYER player, Boolean healHp, Boolean healMp)
        {
            healHp = healHp && player.cur.hp != player.max.hp;
            healMp = healMp && player.cur.mp != player.max.mp;
            if ((healHp || healMp) && player.cur.hp != 0 && !CheckStatus(player, BattleStatusConst.CannotHealInMenu))
                return true;
            Flags |= BattleCalcFlags.Miss;
            return false;
        }

        public Boolean CanBeHealed(Boolean healHp, Boolean healMp)
        {
            return CanBeHealed(Target, healHp, healMp);
        }

        public Boolean CanBeRevived(PLAYER player)
        {
            if (player.cur.hp == 0 && !CheckStatus(player, BattleStatusConst.CannotHealInMenu))
                return true;
            Flags |= BattleCalcFlags.Miss;
            return false;
        }

        public Boolean CanBeRevived()
        {
            return CanBeRevived(Target);
        }

        public Boolean CanBeDamaged(PLAYER player)
        {
            if (player.cur.hp > 0 && !CheckStatus(player, BattleStatusConst.CannotHealInMenu))
                return true;
            Flags |= BattleCalcFlags.Miss;
            return false;
        }

        public Boolean CanBeDamaged()
        {
            return CanBeDamaged(Target);
        }

        public Boolean CanBeMPDamaged(PLAYER player)
        {
            if (player.cur.mp > 0 && !CheckStatus(player, BattleStatusConst.CannotHealInMenu))
                return true;
            Flags |= BattleCalcFlags.Miss;
            return false;
        }

        public Boolean CanBeMPDamaged()
        {
            return CanBeMPDamaged(Target);
        }

        public void SetupSpellHeal()
        {
            Attack = Caster.elem.mgc + Comn.random16() % (1 + (Caster.level + Caster.elem.mgc >> 3));
            AttackPower = Action.Ref.Power;
            DefencePower = Target.defence.MagicalDefence;
        }

        public void SetupItemHeal()
        {
            Attack = 10;
            AttackPower = Action.Ref.Power;
            DefencePower = 0;
        }

        public void SetupJewelHeal()
        {
            Attack = ff9item.FF9Item_GetCount(Action.ItemId);
            AttackPower = Action.Ref.Power;
            DefencePower = 0;
        }

        public void ApplyConcentrate()
        {
            if (ff9abil.FF9Abil_IsEnableSA(Caster.saExtended, SupportAbility.Concentrate))
                Attack = (Int16)(Attack * 3 >> 1);
        }

        public void ApplyMultiTarget()
        {
            if (IsDivided)
                Attack /= 2;
        }

        public void HealHp()
        {
            Int32 recover = AttackPower * Attack;
            TargetRecoverHp = recover;
        }

        public void DrainHp()
        {
            Int32 drain = Math.Max(0, AttackPower - DefencePower) * Math.Max(1, Attack);
            CasterRecoverHp = drain;
            TargetRecoverHp = -drain;
        }

        public void ReviveSpell()
        {
            Int32 recover = (Int32)(Target.max.hp * (Target.elem.wpr + Action.Ref.Power));
            recover /= ff9abil.FF9Abil_IsEnableSA(Caster.saExtended, SupportAbility.Concentrate) ? 50 : 100;
            TargetRecoverHp = recover;
        }

        public void ReviveLow()
        {
            TargetRecoverHp = 1 + Comn.random8() % 10;
        }

        public void HealMp()
        {
            Int32 recover = AttackPower * Attack;
            TargetRecoverMp = recover;
        }

        public void DrainMp()
        {
            Int32 drain = (Math.Max(0, AttackPower - DefencePower) * Math.Max(1, Attack)) >> 4;
            CasterRecoverMp = drain;
            TargetRecoverMp = -drain;
        }

        public void HealFull()
        {
            TargetRecoverHp = (Int32)Target.max.hp;
            TargetRecoverMp = (Int32)Target.max.mp;
        }

        public void RecoverHalfHpMp()
        {
            TargetRecoverHp = (Int32)(Target.max.hp >> 1);
            TargetRecoverMp = (Int32)(Target.max.mp >> 1);
        }

        public void CureActionStatuses()
        {
            if (RemoveStatuses(Target, Action.Status) != 2)
                Flags |= BattleCalcFlags.Miss;
        }

        public void ApplyActionStatuses()
        {
            if (AlterStatuses(Target, Action.Status) != 2)
                Flags |= BattleCalcFlags.Miss;
        }

        public sealed class ActionData
        {
            public BattleCommandInfo Info;
            public BTL_REF Ref;
            public Int32 MP;
            public Byte Category;
            public Byte Type;
            public BattleStatus Status;
            public RegularItem ItemId;
            public BattleAbilityId AbilityId;

            public ActionData(RegularItem itemId, ITEM_DATA item)
            {
                Info = item.info;
                Ref = item.Ref;
                MP = 0;
                Category = 0;
                Type = 0;
                Status = item.status & BattleStatusConst.OutOfBattle;
                ItemId = itemId;
                AbilityId = BattleAbilityId.Void;
            }

            public ActionData(BattleAbilityId abilId, AA_DATA aa)
            {
                Info = aa.Info;
                Ref = aa.Ref;
                MP = aa.MP;
                Category = aa.Category;
                Type = aa.Type;
                Status = FF9BattleDB.StatusSets[aa.AddStatusNo].Value & BattleStatusConst.OutOfBattle;
                ItemId = RegularItem.NoItem;
                AbilityId = abilId;
            }
        }
    }
}

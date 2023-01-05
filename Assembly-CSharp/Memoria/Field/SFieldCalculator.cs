using System;
using FF9;
using Memoria.Data;

namespace Memoria.Field
{
    public static class SFieldCalculator
    {
        public static Boolean FieldCalcMain(PLAYER caster, PLAYER target, AA_DATA tbl, Byte scriptId, UInt32 cursor)
        {
            ItemActionData tbl1 = new ItemActionData(tbl);
            return FieldCalcMain(caster, target, tbl1, scriptId, cursor);
        }

        public static Boolean FieldCalcMain(PLAYER caster, PLAYER target, ITEM_DATA tbl, Byte scriptId, UInt32 cursor)
        {
            ItemActionData tbl1 = new ItemActionData(tbl);
            return FieldCalcMain(caster, target, tbl1, scriptId, cursor);
        }

        private static Boolean FieldCalcMain(PLAYER caster, PLAYER target, ItemActionData tbl, Byte scriptId, UInt32 cursor)
        {
            Context v = new Context
            {
                Caster = caster,
                Target = target,
                Tbl = tbl,
                Cursor = cursor,
                Flags = 0
            };
            v.TargetHp = v.TargetMp = 0;
            switch (scriptId)
            {
                case 10: // Magic Recovery
                    if (CanBeHealed(v))
                    {
                        if (target.cur.hp == target.max.hp)
                        {
                            v.Flags |= BattleCalcFlags.Miss;
                        }
                        else
                        {
                            SetupSpellHeal(v);
                            ApplyConcentrate(v);
                            ApplyNullStatus(v);
                            ApplyMultiTarget(v);
                            HealHp(v);
                        }
                    }
                    break;
                case 12: // Magic Cure Status
                    CureSpellStatus(v);
                    break;
                case 13: // Revive
                    if (CanBeRevived(v))
                        ReviveSpell(v);
                    break;
                case 62: // Item Soft
                case 73: // Item Cure Status
                    if (tbl.Info.VfxIndex == (Int32)SpecialEffect.Gysahl_Greens)
                        return PersistenSingleton<UIManager>.Instance.ItemScene.FF9FItem_Vegetable();
                    CureItemStatus(v);
                    break;
                case 69: // Item Potion
                    if (CanBeHealed(v))
                    {
                        if (target.cur.hp == target.max.hp)
                        {
                            v.Flags |= BattleCalcFlags.Miss;
                        }
                        else
                        {
                            SetupItemHeal(v);
                            HealHp(v);
                        }
                    }
                    break;
                case 70: // Item Ether
                    if (CanBeHealed(v))
                    {
                        if (target.cur.mp == target.max.mp)
                        {
                            v.Flags |= BattleCalcFlags.Miss;
                        }
                        else
                        {
                            SetupItemHeal(v);
                            HealMp(v);
                        }
                    }
                    break;
                case 71: // Item Elixir
                    if (CanBeHealed(v))
                    {
                        if (target.cur.hp == target.max.hp && target.cur.mp == target.max.mp)
                            v.Flags |= BattleCalcFlags.Miss;
                        else
                            HealFull(v);
                    }
                    break;
                case 72: // Item Phoenix
                    if (CanBeRevived(v))
                        ReviveLow(v);
                    break;
                case 74: // Item Gem
                    return PersistenSingleton<UIManager>.Instance.ItemScene.FF9FItem_Vegetable();
                case 76: // Item Tent
                    if (CanBeHealed(v))
                        RecoverHalfHpMp(v);
                    break;
            }
            return FieldCalcResult(v);
        }

        private static Boolean FieldCalcResult(Context v)
        {
            if ((v.Flags & BattleCalcFlags.Miss) != 0)
            {
                v.TargetInfo |= Param.FIG_INFO_MISS;
                return false;
            }
            if (v.TargetHp > 0)
                FieldSetRecover(v.Target, (UInt32)v.TargetHp);
            if (v.TargetMp > 0)
                FieldSetMpRecover(v.Target, (UInt32)v.TargetMp);
            return true;
        }

        private static Boolean CanBeHealed(Context v)
        {
            if (!FieldCheckStatus(v.Target, BattleStatus.Petrify | BattleStatus.Zombie) && v.Target.cur.hp != 0)
                return true;
            v.Flags |= BattleCalcFlags.Miss;
            return false;
        }

        private static Boolean CanBeRevived(Context v)
        {
            if (!FieldCheckStatus(v.Target, BattleStatus.Petrify) && v.Target.cur.hp <= 0 && (!FieldCheckStatus(v.Target, BattleStatus.Zombie) || v.Target.cur.hp != 0))
                return true;
            v.Flags |= BattleCalcFlags.Miss;
            return false;
        }

        private static void SetupSpellHeal(Context v)
        {
            v.AttackNumber = v.Caster.elem.mgc + Comn.random16() % (1 + (v.Caster.level + v.Caster.elem.mgc >> 3));
            v.AttackPower = v.Tbl.Ref.Power;
            v.DefencePower = v.Target.defence.MagicalDefence;
        }

        private static void SetupItemHeal(Context v)
        {
            v.AttackNumber = 10;
            v.AttackPower = v.Tbl.Ref.Power;
            v.DefencePower = 0;
        }

        private static void ApplyNullStatus(Context v)
        {
            if (FieldCheckStatus(v.Caster, 0))
                v.AttackNumber /= 2;
        }

        private static void ApplyConcentrate(Context v)
        {
            if (ff9abil.FF9Abil_IsEnableSA(v.Caster.saExtended, SupportAbility.Concentrate))
                v.AttackNumber = (Int16)(v.AttackNumber * 3 >> 1);
        }

        private static void ApplyMultiTarget(Context v)
        {
            if (v.Cursor == 1 && v.Tbl.Info.Target >= TargetType.ManyAny && v.Tbl.Info.Target <= TargetType.ManyEnemy)
                v.AttackNumber /= 2;
        }

        private static void HealHp(Context v)
        {
            Int32 newHp = v.AttackPower * v.AttackNumber;
            if (newHp > ff9play.FF9PLAY_HP_MAX)
                newHp = ff9play.FF9PLAY_HP_MAX;
            v.TargetHp = newHp;
        }

        private static void ReviveSpell(Context v)
        {
            Int32 newHp = (Int32)(v.Target.max.hp * (v.Target.elem.wpr + v.Tbl.Ref.Power));
            newHp /= ff9abil.FF9Abil_IsEnableSA(v.Caster.saExtended, SupportAbility.Concentrate) ? 50 : 100;
            if (newHp > ff9play.FF9PLAY_HP_MAX)
                newHp = ff9play.FF9PLAY_HP_MAX;
            v.TargetHp = newHp;
        }

        private static void HealMp(Context v)
        {
            Int32 newMp = v.AttackPower * v.AttackNumber;
            if (newMp > ff9play.FF9PLAY_MP_MAX)
                newMp = ff9play.FF9PLAY_MP_MAX;
            v.TargetMp = newMp;
        }

        private static void HealFull(Context v)
        {
            v.Target.cur.hp = v.Target.max.hp;
            v.Target.cur.mp = v.Target.max.mp;
        }

        private static void ReviveLow(Context v)
        {
            v.Target.cur.hp = (UInt32)(1 + Comn.random8() % 10);
        }

        private static void RecoverHalfHpMp(Context v)
        {
            v.TargetHp = (Int32)(v.Target.max.hp >> 1);
            v.TargetMp = (Int32)(v.Target.max.mp >> 1);
        }

        private static void CureSpellStatus(Context v)
        {
            // Use the status set list initialized through CSV reading
            // Might use "....Value & 127" instead of the plain Value although that doesn't seem required
            if (FieldRemoveStatuses(v.Target, (Byte)FF9BattleDB.StatusSets[v.Tbl.AddNo].Value) != 2)
                v.Flags |= BattleCalcFlags.Miss;
        }

        private static void CureItemStatus(Context v)
        {
            if (FieldRemoveStatuses(v.Target, (Byte)v.Tbl.Status) != 2)
                v.Flags |= BattleCalcFlags.Miss;
        }

        private static void FieldSetRecover(PLAYER player, UInt32 recover)
        {
            if (FieldCheckStatus(player, BattleStatus.Petrify))
                return;
            player.cur.hp += recover;
            if (player.cur.hp <= player.max.hp)
                return;
            player.cur.hp = player.max.hp;
        }

        private static void FieldSetMpRecover(PLAYER player, UInt32 recover)
        {
            if (FieldCheckStatus(player, BattleStatus.Petrify))
                return;
            player.cur.mp += recover;
            if (player.cur.mp <= player.max.mp)
                return;
            player.cur.mp = player.max.mp;
        }

        public static Int32 FieldRemoveStatus(PLAYER player, Byte status)
        {
            if ((player.status & status) == 0)
                return 1;
            player.status &= unchecked((Byte)~status);
            return 2;
        }

        private static Int32 FieldRemoveStatuses(PLAYER player, Byte statuses)
        {
            Int32 success = 1;
            for (Int32 i = 0; i < 8; ++i)
            {
                Byte status = (Byte)(1 << i);
                if ((statuses & status) != 0 && FieldRemoveStatus(player, status) == 2)
                    success = 2;
            }
            return success;
        }

        private static Boolean FieldCheckStatus(PLAYER player, BattleStatus status)
        {
            return (player.status & (Int32)status) != 0;
        }

        private sealed class ItemActionData
        {
            public BattleCommandInfo Info;
            public BTL_REF Ref;
            public Byte Category;
            public BattleStatusIndex AddNo;
            public Int32 MP;
            public Byte Type;
            public UInt16 Vfx2;
            public String Name;
            public UInt32 Status;

            public ItemActionData(ITEM_DATA item)
            {
                Info = item.info;
                Ref = item.Ref;
                Status = item.status;
            }

            public ItemActionData(AA_DATA aa)
            {
                Info = aa.Info;
                Ref = aa.Ref;
                Category = aa.Category;
                AddNo = aa.AddStatusNo;
                MP = aa.MP;
                Type = aa.Type;
                Vfx2 = aa.Vfx2;
                Name = aa.Name;
            }
        }

        private sealed class Context
        {
            public PLAYER Caster;
            public PLAYER Target;
            public ItemActionData Tbl;
            public UInt32 Cursor;
            public BattleCalcFlags Flags;
            public UInt16 TargetInfo;
            public Int32 AttackPower;
            public Int32 DefencePower;
            public Int32 AttackNumber;
            public Int32 TargetHp;
            public Int32 TargetMp;
        }
    }
}

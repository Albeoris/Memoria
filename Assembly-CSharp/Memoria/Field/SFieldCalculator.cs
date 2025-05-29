using System;
using FF9;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Scripts;

namespace Memoria
{
    public static class SFieldCalculator
    {
        public static Boolean FieldCalcMain(PLAYER caster, PLAYER target, BattleAbilityId abilId, AA_DATA aaData, UInt32 cursor)
        {
            FieldCalculator.ActionData action = new FieldCalculator.ActionData(abilId, aaData);
            return FieldCalcMain(caster, target, action, aaData.Ref.ScriptId, cursor);
        }

        public static Boolean FieldCalcMain(PLAYER caster, PLAYER target, RegularItem itemId, UInt32 cursor)
        {
            ITEM_DATA itemData = ff9item.GetItemEffect(itemId);
            FieldCalculator.ActionData action = new FieldCalculator.ActionData(itemId, itemData);
            return FieldCalcMain(caster, target, action, itemData.Ref.ScriptId, cursor);
        }

        private static Boolean FieldCalcMain(PLAYER caster, PLAYER target, FieldCalculator.ActionData action, Int32 scriptId, UInt32 cursor)
        {
            FieldCalculator v = new FieldCalculator
            {
                Caster = caster,
                Target = target,
                Action = action,
                Cursor = cursor
            };
            try
            {
                FieldAbilityScriptBase script = ScriptsLoader.GetFieldAbilityScript(scriptId);
                if (script != null)
                    script.Apply(v);
                else
                    DefaultFieldScript(scriptId, v);
            }
            catch (Exception err)
            {
                Log.Error(err);
            }
            return FieldCalcResult(v);
        }

        private static void DefaultFieldScript(Int32 scriptId, FieldCalculator v)
        {
            switch (scriptId)
            {
                case 10: // Magic Recovery
                    if (v.CanBeHealed(true, false))
                    {
                        v.SetupSpellHeal();
                        v.ApplyConcentrate();
                        v.ApplyMultiTarget();
                        v.HealHp();
                    }
                    break;
                case 12: // Magic Cure Status
                    v.CureActionStatuses();
                    break;
                case 13: // Revive
                    if (v.CanBeRevived())
                        v.ReviveSpell();
                    break;
                case 15: // Drain MP
                    if (v.CanBeHealed(v.Caster, false, true) && v.CanBeMPDamaged(v.Target))
                    {
                        v.SetupSpellHeal();
                        v.ApplyConcentrate();
                        v.DrainMp();
                    }
                    break;
                case 16: // Drain HP
                    if (v.CanBeHealed(v.Caster, true, false) && v.CanBeDamaged(v.Target))
                    {
                        v.SetupSpellHeal();
                        v.ApplyConcentrate();
                        v.DrainHp();
                    }
                    break;
                case 30: // White Wind
                    if (v.CanBeHealed(true, false))
                    {
                        if (v.Action.Ref.Power == 0) // Value in vanilla
                            v.TargetRecoverHp = (Int32)v.Caster.max.hp / 3;
                        else
                            v.TargetRecoverHp = (Int32)v.Caster.max.hp * v.Action.Ref.Power / 100;
                    }
                    break;
                case 37: // Chakra
                    if (v.CanBeHealed(true, true))
                    {
                        v.TargetRecoverHp = (Int32)v.Target.max.hp * v.Action.Ref.Power / 100;
                        v.TargetRecoverMp = (Int32)v.Target.max.mp * v.Action.Ref.Power / 100;
                    }
                    break;
                case 50: // Six Dragons
                    if (v.CanBeHealed(true, true) && v.CanBeDamaged() && v.CanBeMPDamaged())
                    {
                        Int32 percent = GameRandom.Next16() % 100;
                        if (percent < 10)
                        {
                            v.TargetRecoverHp = (Int32)v.Target.max.hp;
                            v.TargetRecoverMp = (Int32)v.Target.max.mp;
                        }
                        else if (percent < 30)
                        {
                            v.TargetRecoverHp = (Int32)v.Target.max.hp;
                        }
                        else if (percent < 50)
                        {
                            v.TargetRecoverMp = (Int32)v.Target.max.mp;
                        }
                        else if (percent < 65)
                        {
                            v.TargetRecoverHp = (Int32)(1 - v.Target.cur.hp);
                        }
                        else if (percent < 80)
                        {
                            v.TargetRecoverMp = (Int32)(1 - v.Target.cur.mp);
                        }
                        else
                        {
                            v.TargetRecoverHp = (Int32)(1 - v.Target.cur.hp);
                            v.TargetRecoverMp = (Int32)(1 - v.Target.cur.mp);
                        }
                    }
                    break;
                case 62: // Item Soft
                case 73: // Item Cure Status
                    v.CureActionStatuses();
                    break;
                case 69: // Item Potion
                {
                    Boolean hasHeal = v.Action.Ref.Power > 0;
                    Boolean hasStatus = v.Action.Status != 0;
                    if (hasHeal && v.CanBeHealed(true, false))
                    {
                        v.SetupItemHeal();
                        v.HealHp();
                    }
                    if (hasHeal && hasStatus)
                        v.Flags &= ~BattleCalcFlags.Miss;
                    if (hasStatus)
                        v.ApplyActionStatuses();
                    break;
                }
                case 70: // Item Ether
                    if (v.CanBeHealed(false, true))
                    {
                        v.SetupItemHeal();
                        v.HealMp();
                    }
                    break;
                case 71: // Item Elixir
                    if (v.CanBeHealed(true, true))
                        v.HealFull();
                    break;
                case 72: // Item Phoenix
                    if (v.CanBeRevived())
                        v.ReviveLow();
                    break;
                case 74: // Item Gem
                    if (v.CanBeHealed(true, false))
                    {
                        v.SetupJewelHeal();
                        v.HealHp();
                    }
                    break;
                case 76: // Item Tent
                    if (v.CanBeHealed(true, true))
                        v.RecoverHalfHpMp();
                    break;
                case 89: // HP Switching
                {
                    Boolean success = false;
                    if (v.Caster.cur.hp > v.Target.cur.hp)
                        success = v.CanBeHealed(v.Target, true, false) && v.CanBeDamaged(v.Caster);
                    else if (v.Caster.cur.hp < v.Target.cur.hp)
                        success = v.CanBeHealed(v.Caster, true, false) && v.CanBeDamaged(v.Target);
                    if (success)
                    {
                        v.CasterRecoverHp = (Int32)(v.Target.cur.hp - v.Caster.cur.hp);
                        v.TargetRecoverHp = (Int32)(v.Caster.cur.hp - v.Target.cur.hp);
                    }
                    break;
                }
                case 103: // Positive status
                    if (FieldCalculator.AlterStatuses(v.Target, v.Action.Status) < 2)
                        v.Flags = BattleCalcFlags.Miss;
                    break;
                default:
                    v.Flags = BattleCalcFlags.Miss;
                    break;
            }
        }

        private static Boolean FieldCalcResult(FieldCalculator v)
        {
            if ((v.Flags & BattleCalcFlags.Miss) != 0)
                return false;
            if (!v.IsItem && v.CasterRecoverHp > 0)
                FieldCalculator.ApplyRecover(v.Caster, v.CasterRecoverHp);
            if (!v.IsItem && v.CasterRecoverMp > 0)
                FieldCalculator.ApplyMpRecover(v.Caster, v.CasterRecoverMp);
            if (v.TargetRecoverHp > 0)
                FieldCalculator.ApplyRecover(v.Target, v.TargetRecoverHp);
            if (v.TargetRecoverMp > 0)
                FieldCalculator.ApplyMpRecover(v.Target, v.TargetRecoverMp);
            return true;
        }
    }
}

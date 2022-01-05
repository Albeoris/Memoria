using Memoria;
using Memoria.Data;
using System;
using System.Linq;

namespace FF9
{
    public class btl_abil
    {
        private const byte potionItemId = 236;
        private const byte hiPotionItemId = 237;

        public static Boolean CheckPartyAbility(UInt32 sa_buf_no, UInt32 sa_bit)
        {
            for (Int32 i = 0; i < 4; i++)
            {
                PLAYER player = FF9StateSystem.Common.FF9.party.member[i];
                if (player != null && (player.sa[(Int32)((UIntPtr)sa_buf_no)] & sa_bit) != 0u)
                {
                    return true;
                }
            }
            return false;
        }

        public static Boolean TryReturnMagic(BattleUnit returner, BattleUnit originalCaster, BattleCommand command)
        {
            if (returner.IsUnderAnyStatus(BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Stop | BattleStatus.Sleep | BattleStatus.Freeze | BattleStatus.Death) || FF9StateSystem.Battle.FF9Battle.btl_phase != 4)
                return false;
            BattleCommandId cmdId = originalCaster.IsPlayer ? BattleCommandId.Counter : BattleCommandId.MagicCounter;
            if (Configuration.Battle.CountersBetterTarget)
            {
                if (command.Data.tar_id == returner.Id) // Single-target magic
                {
                    if (originalCaster.IsUnderStatus(BattleStatus.Death))
                    {
                        UInt16 retarget_id = (UInt16)(originalCaster.IsPlayer ? 1 : 16);
                        for (Int32 i = 0; i < 4; i++)
                        {
                            BattleUnit new_target = btl_scrp.FindBattleUnit((UInt16)(retarget_id << i));
                            if (new_target != null && new_target.Data.bi.target != 0 && !new_target.IsUnderStatus(BattleStatus.Death))
                            {
                                btl_cmd.SetCounter(returner.Data, cmdId, (Int32)command.Data.sub_no, new_target.Id);
                                return true;
                            }
                        }
                        return false;
                    }
                    else
                        btl_cmd.SetCounter(returner.Data, cmdId, (int)command.Data.sub_no, originalCaster.Id);
                }
                else if ((command.Data.tar_id & 0xF) != 0 && (command.Data.tar_id & 0xF0) != 0) // Most likely targeting everyone
                    btl_cmd.SetCounter(returner.Data, cmdId, (int)command.Data.sub_no, btl_scrp.GetBattleID(2u));
                else // Multi-target magic
                    btl_cmd.SetCounter(returner.Data, cmdId, (int)command.Data.sub_no, btl_scrp.GetBattleID(originalCaster.IsPlayer ? 0u : 1u));
            }
            else
            {
                btl_cmd.SetCounter(returner.Data, cmdId, (int)command.Data.sub_no, originalCaster.Id);
            }
            return true;
        }

        public static Boolean CheckCounterAbility(BattleTarget defender, BattleCaster attacker, BattleCommand command)
        {
            // Dummied
            if (defender.IsUnderStatus(BattleStatus.NoReaction) || command.Id > BattleCommandId.EnemyAtk)
                return false;

            if (defender.HasSupportAbility(SupportAbility2.Counter) && (command.AbilityCategory & 8) != 0) // Physical
            {

                Int32 chance = defender.Will;
                if (defender.HasSupportAbility(SupportAbility2.Eye4Eye))
                    chance *= 2;

                if (chance > Comn.random16() % 100)
                {
                    btl_cmd.SetCounter(defender.Data, BattleCommandId.Counter, 176, attacker.Id);
                    return true;
                }
            }

            if (defender.HasSupportAbility(SupportAbility2.ReturnMagic) && (command.AbilityCategory & 128) != 0) // Magic
                return TryReturnMagic(defender, attacker, command);

            return false;
        }

        public static void CheckAutoItemAbility(BattleTarget defender, BattleCommand command)
        {
            if (!defender.HasSupportAbility(SupportAbility2.AutoPotion))
                return;

            if (defender.IsUnderStatus(BattleStatus.NoReaction) || command.Id > BattleCommandId.EnemyAtk)
                return;

            byte itemId = IsDefualtAutoPotionBehaviourEnabled()
                ? GetFirstPotionUseableByAutoItemAbility()
                : FindSuitablePotion(defender, Configuration.Battle.AutoPotionOverhealLimit);

            // No suitable potion was found to perform a counter.
            if (itemId == 0)
                return;

            // Prepare and set counter.
            UIManager.Battle.ItemRequest(itemId);
            btl_cmd.SetCounter(defender.Data, BattleCommandId.AutoPotion, itemId, defender.Id);
        }

        private static bool IsDefualtAutoPotionBehaviourEnabled()
        {
            return Configuration.Battle.AutoPotionOverhealLimit < 0;
        }

        /// <summary>
        /// Returns Potion id or Hi-Potion id if they are available in inventory. Priority has Potion over Hi-Potion.
        /// If both potions are not available, returns 0.
        /// </summary>
        private static byte GetFirstPotionUseableByAutoItemAbility()
        {
            return new[] { potionItemId, hiPotionItemId }.FirstOrDefault(id => ff9item.FF9Item_GetCount(id) > 0);
        }

        /// <summary>
        /// Evaluates which potion should be used in depending on how much HP left character has.
        /// </summary>
        /// <param name="defender"></param>
        /// <returns>Returns id of a Potion or a Hi-Potion depending on the amount of HP left on character with Auto-Potion ability.
        /// Hi-Potion is returned when regular Potion is not sufficient or is not available.
        /// If both items are not available, method returns 0.
        /// </returns>
        private static byte FindSuitablePotion(BattleTarget defender, int autoPotionOverhealLimitInPercent)
        {
            byte id = 0;
            foreach (Byte itemId in new[] { potionItemId, hiPotionItemId })
            {
                if (ff9item.FF9Item_GetCount(itemId) < 1)
                    continue;

                BattleCalculator calc = PerformScriptOnPotion(defender, itemId);

                // Every value below is in Hit Points, expect if specified otherwise.
                uint heal = (uint)calc.Target.HpDamage;
                uint toGain = defender.HasSupportAbility(SupportAbility1.Chemist) ? heal * 2 : heal;
                uint missing = calc.Target.MaximumHp - calc.Target.CurrentHp;

                // If character gets healed by value smaller than missing hp it means
                // there is no over healing done yet. Otherwise, check over healing limit set by user.
                if (toGain <= missing)
                    id = itemId;
                else
                {
                    uint overhealDone = toGain - missing;
                    uint overhealLimit = (uint)(toGain * (autoPotionOverhealLimitInPercent / 100));

                    if (overhealDone <= overhealLimit)
                        id = itemId;
                    else
                        break; // if regular potion already exceeds over heal limit, no need to check better potion.
                }
            }
            return id;
        }

        /// <summary>
        /// Perform script to evaluate the healing power of a potion.
        /// </summary>
        /// <param name="defender">Target on which action will be performed.</param>
        /// <param name="potionId">Id of an potion which will be used on target.</param>
        /// <returns>Instance of an object which holds the outcome of executed script.</returns>
        private static BattleCalculator PerformScriptOnPotion(BattleTarget defender, byte potionId)
        {
            const Byte potionScriptId = 069;

            // Making empty command does not trigger any animations. Useful for purely calculation purposes.
            BattleCommand command = MakeAutoPotionCommandWithoutAnimations(defender, potionId);

            var calc = new BattleCalculator(defender.Data, defender.Data, command);
            BattleScriptFactory factory = SBattleCalculator.FindScriptFactory(potionScriptId);
            if (factory != null)
            {
                IBattleScript script = factory(calc);
                script.Perform();
            }

            return calc;
        }

        private static BattleCommand MakeAutoPotionCommandWithoutAnimations(BattleTarget defender, byte potionId)
        {
            var command = new CMD_DATA
            {
                cmd_no = BattleCommandId.AutoPotion,
                sub_no = potionId,
                tar_id = defender.Id,
                info = new CMD_DATA.SELECT_INFO()
            };

            AA_DATA emptyAbilityData = FF9StateSystem.Battle.FF9Battle.aa_data[0];
            command.SetAAData(emptyAbilityData);
            return new BattleCommand(command);
        }

        public static UInt16 CheckCoverAbility(BTL_DATA target, UInt16 candidates)
        {
            // If a unit (even not candidate) is already covering the target, let cover apply there as well
            for (BTL_DATA next = FF9StateSystem.Battle.FF9Battle.btl_list.next; next != null; next = next.next)
                if (next.bi.cover_unit == target)
                    return next.btl_id;

            BTL_DATA coverBy = FindStrongestDefender(target, candidates);
            if (coverBy == null)
                return 0;

            new BattleUnit(coverBy).FaceTheEnemy();
            coverBy.pos[0] = target.pos[0];
            coverBy.pos[2] = target.pos[2];

            target.pos[2] -= 400f;

            //btl_mot.setMotion(coverBy.Data, BattlePlayerCharacter.PlayerMotionIndex.MP_COVER);
            //coverBy.Data.evt.animFrame = 0;

            coverBy.bi.cover_unit = target;
            btl_mot.SetDefaultIdle(coverBy);

            return coverBy.btl_id;
        }

        private static BTL_DATA FindStrongestDefender(BTL_DATA target, UInt16 candidates)
        {
            BattleUnit coverBy = null;
            foreach (BTL_DATA next in btl_util.findAllBtlData(candidates))
            {
                BattleUnit unit = new BattleUnit(next);
                if (unit.Id == target.btl_id || unit.IsCovering || btl_util.IsBtlBusy(next, btl_util.BusyMode.CASTER | btl_util.BusyMode.MAGIC_CASTER))
                    continue;
                if (coverBy == null || coverBy.CurrentHp < unit.CurrentHp)
                    coverBy = unit;
            }
            return coverBy != null ? coverBy.Data : null;
        }

        public static void CheckReactionAbility(BTL_DATA btl, AA_DATA aa)
        {
            // Dummied
            if (!btl_stat.CheckStatus(btl, BattleStatus.NoReaction))
            {
                if ((btl.sa[1] & 1048576u) != 0u && btl.cur.hp != 0 && Status.checkCurStat(btl, BattleStatus.LowHP))
                {
                    if (btl.cur.hp + btl.max.hp / 2 < btl.max.hp)
                    {
                        btl.cur.hp += btl.max.hp / 2;
                    }
                    else
                    {
                        btl.cur.hp = btl.max.hp;
                    }
                }
                if ((btl.sa[1] & 8388608u) != 0u && aa.MP != 0)
                {
                    if (btl.cur.mp + aa.MP < btl.max.mp)
                    {
                        btl.cur.mp += aa.MP;
                    }
                    else
                    {
                        btl.cur.mp = btl.max.mp;
                    }
                }
            }
        }

        public static void CheckStatusAbility(BattleUnit btl)
        {
            foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(btl.Data.sa))
                saFeature.TriggerOnStatusInit(btl);

            /*if (btl.HasSupportAbility(SupportAbility1.AutoReflect))
           {
                btl.PermanentStatus |= BattleStatus.Reflect;
//				HonoluluBattleMain.battleSPS.AddBtlSPSObj(btl, BattleStatus.Reflect);
			}

			if (btl.HasSupportAbility(SupportAbility1.AutoFloat))
			    btl.PermanentStatus |= BattleStatus.Float;

            if (btl.HasSupportAbility(SupportAbility1.AutoHaste))
           {
                btl.PermanentStatus |= BattleStatus.Haste;
                btl.ResistStatus |= BattleStatus.Slow;
//				btl.Data.cur.at_coef = (SByte)(btl.Data.cur.at_coef * 3 / 2);
//				HonoluluBattleMain.battleSPS.AddBtlSPSObj(btl, BattleStatus.Haste);
			}

            if (btl.HasSupportAbility(SupportAbility1.AutoRegen))
           {
                btl.PermanentStatus |= BattleStatus.Regen;
//				btl_stat.SetOprStatusCount(btl.Data, 18u);
			}

            if (btl.HasSupportAbility(SupportAbility1.AutoLife))
                btl.CurrentStatus |= BattleStatus.AutoLife;

            if (btl.HasSupportAbility(SupportAbility2.BodyTemp))
                btl.ResistStatus |= (BattleStatus.Freeze | BattleStatus.Heat);

            if (btl.HasSupportAbility(SupportAbility2.Insomniac))
                btl.ResistStatus |= BattleStatus.Sleep;

            if (btl.HasSupportAbility(SupportAbility2.Antibody))
                btl.ResistStatus |= (BattleStatus.Poison | BattleStatus.Venom);

            if (btl.HasSupportAbility(SupportAbility2.BrightEyes))
                btl.ResistStatus |= BattleStatus.Blind;

            if (btl.HasSupportAbility(SupportAbility2.Loudmouth))
			    btl.ResistStatus |= BattleStatus.Silence;

            if (btl.HasSupportAbility(SupportAbility2.Jelly))
                btl.ResistStatus |= (BattleStatus.Petrify | BattleStatus.GradualPetrify);

		    if (btl.HasSupportAbility(SupportAbility2.Locomotion))
		        btl.ResistStatus |= BattleStatus.Stop;

		    if (btl.HasSupportAbility(SupportAbility2.ClearHeaded))
		        btl.ResistStatus |= BattleStatus.Confuse;*/
        }
    }
}
using System;
using System.Collections.Generic;
using FF9;
using Memoria;
using Memoria.Data;

// NCalc source code embedded in Assembly-CSharp.dll for avoiding a DLL dependency
// Original author of NCalc: sebastienros, https://archive.codeplex.com/?p=ncalc
// Author of this version of NCalc: pitermarx, https://github.com/pitermarx/NCalc-Edge

namespace NCalc
{
	public class NCalcUtility
    {
        public static readonly Type[] UsableEnumTypes = new Type[]
        {
            typeof(CharacterId),
            typeof(BattleCommandId),
            typeof(BattleAbilityId),
            typeof(WeaponItem),
            typeof(AccessoryItem),
            typeof(BattleStatus),
            typeof(EffectElement),
            typeof(CharacterCategory),
            typeof(EnemyCategory),
            typeof(BattleCalcFlags),
            typeof(CalcFlag)
        };

        public static Int64 ConvertNCalcResult(Object obj, Int64 noChangeValue)
        {
            if (obj is SByte) return (SByte)obj;
            if (obj is Byte) return (Byte)obj;
            if (obj is Int16) return (Int16)obj;
            if (obj is UInt16) return (UInt16)obj;
            if (obj is Int32) return (Int32)obj;
            if (obj is UInt32) return (UInt32)obj;
            if (obj is Int64) return (Int64)obj;
            if (obj is UInt64) return (Int64)(UInt64)obj;
            if (obj is Single) return (Int64)Math.Round((Single)obj);
            if (obj is Double) return (Int64)Math.Round((Double)obj);
            if (obj is Decimal) return (Int64)Math.Round((Decimal)obj);
            return noChangeValue;
        }

        public static Single ConvertNCalcResult(Object obj, Single noChangeValue)
        {
            if (obj is SByte) return (SByte)obj;
            if (obj is Byte) return (Byte)obj;
            if (obj is Int16) return (Int16)obj;
            if (obj is UInt16) return (UInt16)obj;
            if (obj is Int32) return (Int32)obj;
            if (obj is UInt32) return (UInt32)obj;
            if (obj is Int64) return (Int64)obj;
            if (obj is UInt64) return (UInt64)obj;
            if (obj is Single) return (Single)obj;
            if (obj is Double) return (Single)(Double)obj;
            if (obj is Decimal) return (Single)(Decimal)obj;
            return noChangeValue;
        }

        public static Boolean EvaluateNCalcCondition(Object obj, Boolean defaultResult = false)
        {
            if (obj is Boolean) return (Boolean)obj;
            return ConvertNCalcResult(obj, defaultResult ? 1 : 0) != 0;
        }

        public static EvaluateFunctionHandler commonNCalcFunctions = delegate (String name, FunctionArgs args)
        {
            if (name == "GetRandom" && args.Parameters.Length == 0)
                args.Result = Comn.random8();
            else if (name == "GetRandom" && args.Parameters.Length == 2)
                args.Result = UnityEngine.Random.Range((Int32)NCalcUtility.ConvertNCalcResult(args.Parameters[0].Evaluate(), 0), (Int32)NCalcUtility.ConvertNCalcResult(args.Parameters[1].Evaluate(), 0));
            else if (name == "GetRandomBit" && args.Parameters.Length == 1)
                args.Result = Comn.randomID((UInt32)NCalcUtility.ConvertNCalcResult(args.Parameters[0].Evaluate(), 0));
            else if (name == "GetAbilityUsageCount" && args.Parameters.Length == 1)
                args.Result = (Int32)GameState.AbilityUsage((Byte)NCalcUtility.ConvertNCalcResult(args.Parameters[0].Evaluate(), 0));
            else if (name == "GetItemCount" && args.Parameters.Length == 1)
                args.Result = GameState.ItemCount((Int32)NCalcUtility.ConvertNCalcResult(args.Parameters[0].Evaluate(), Byte.MaxValue));
            else if (name == "GetEventGlobalByte" && args.Parameters.Length == 1)
            {
                Int32 index = (Int32)NCalcUtility.ConvertNCalcResult(args.Parameters[0].Evaluate(), 0);
                if (index >= 0 && index < FF9StateSystem.EventState.gEventGlobal.Length)
                    args.Result = (Int32)FF9StateSystem.EventState.gEventGlobal[index];
                else
                    args.Result = 0;
            }
            else if ((name == "CheckAnyStatus" || name == "CheckAllStatus") && args.Parameters.Length >= 2) // operators & and | are only working with UInt16 and smaller types in NCalc...
            {
                UInt64 v1 = (UInt64)NCalcUtility.ConvertNCalcResult(args.Parameters[0].Evaluate(), 0);
                UInt64 v2 = (UInt64)NCalcUtility.ConvertNCalcResult(args.Parameters[1].Evaluate(), 0);
                for (Int32 i = 2; i < args.Parameters.Length; i++)
                    v2 |= (UInt64)NCalcUtility.ConvertNCalcResult(args.Parameters[i].Evaluate(), 0);
                args.Result = name == "CheckAnyStatus" ? (v1 & v2) != 0 : (v1 & v2) == v2;
            }
            else if ((name == "CombineStatuses" || name == "RemoveStatuses") && args.Parameters.Length >= 2) // operators & and | are only working with UInt16 and smaller types in NCalc...
            {
                UInt64 v1 = (UInt64)NCalcUtility.ConvertNCalcResult(args.Parameters[0].Evaluate(), 0);
                UInt64 v2 = (UInt64)NCalcUtility.ConvertNCalcResult(args.Parameters[1].Evaluate(), 0);
                for (Int32 i = 2; i < args.Parameters.Length; i++)
                    v2 |= (UInt64)NCalcUtility.ConvertNCalcResult(args.Parameters[i].Evaluate(), 0);
                args.Result = name == "CombineStatuses" ? v1 | v2 : v1 & ~v2;
            }
            else if (name == "BattleFilter" && args.Parameters.Length >= 1)
            {
                UInt16 btlId = (UInt16)NCalcUtility.ConvertNCalcResult(args.Parameters[0].Evaluate(), 0);
                UInt16 btlIdFiltered = 0;
                Nullable<Boolean> playerFilter = null;
                Nullable<Boolean> targetableFilter = null;
                UInt64 statusFilter = 0;
                if (args.Parameters.Length >= 2)
                {
                    Int64 arg = NCalcUtility.ConvertNCalcResult(args.Parameters[1].Evaluate(), -1);
                    playerFilter = arg == 0 ? false : (arg == 1 ? true : null);
                }
                if (args.Parameters.Length >= 3)
                {
                    Int64 arg = NCalcUtility.ConvertNCalcResult(args.Parameters[2].Evaluate(), -1);
                    targetableFilter = arg == 0 ? false : (arg == 1 ? true : null);
                }
                if (args.Parameters.Length >= 4)
                    statusFilter = (UInt64)NCalcUtility.ConvertNCalcResult(args.Parameters[3].Evaluate(), 0);
                foreach (BattleUnit unit in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
                    if ((unit.Id & btlId) != 0
                      && (playerFilter == null || playerFilter.Value == unit.IsPlayer)
                      && (targetableFilter == null || targetableFilter.Value == unit.IsSelected)
                      && !unit.IsUnderAnyStatus((BattleStatus)statusFilter))
                        btlIdFiltered |= unit.Id;
                args.Result = btlIdFiltered;
            }
            else if (name == "Min" && args.Parameters.Length == 2)
            {
                Int64 v1 = NCalcUtility.ConvertNCalcResult(args.Parameters[0].Evaluate(), Int64.MaxValue);
                Int64 v2 = NCalcUtility.ConvertNCalcResult(args.Parameters[1].Evaluate(), Int64.MaxValue);
                if (v1 != Int64.MaxValue || v2 != Int64.MaxValue)
                    args.Result = Math.Min(v1, v2);
            }
            else if (name == "Max" && args.Parameters.Length == 2)
            {
                Int64 v1 = NCalcUtility.ConvertNCalcResult(args.Parameters[0].Evaluate(), Int64.MinValue);
                Int64 v2 = NCalcUtility.ConvertNCalcResult(args.Parameters[1].Evaluate(), Int64.MinValue);
                if (v1 != Int64.MinValue || v2 != Int64.MinValue)
                    args.Result = Math.Max(v1, v2);
            }else if(name == "IsStolenItem" && args.Parameters.Length == 1)
            {
                if (BattleVoice.ItemStolen != null)
                {
                    String itemName = args.Parameters[0].ParsedExpression.ToString().Trim(new char[] { '[', ']' });
                    args.Result = itemName.ToLower().Equals(BattleVoice.ItemStolen.ToLower());
                }
                else
                {
                    args.Result = false;
                }
            }else if(name == "HasStolenItem" && args.Parameters.Length == 0)
            {
                args.Result = BattleVoice.ItemStolen != null;
            }
        };

        public static EvaluateParameterHandler commonNCalcParameters = delegate (String name, ParameterArgs args)
        {
            if (name == "Gil") args.Result = GameState.Gil;
            else if (name == "DragonCount") args.Result = (Int32)GameState.Dragons;
            else if (name == "FrogCount") args.Result = (Int32)GameState.Frogs;
            else if (name == "StealCount") args.Result = (Int32)GameState.Thefts;
            else if (name == "EscapeCount") args.Result = (Int32)GameState.EscapeCount;
            else if (name == "StepCount") args.Result = (Int32)GameState.StepCount;
            else if (name == "TonberryCount") args.Result = (Int32)GameState.Tonberies;
            else if (name == "TetraMasterWinCount") args.Result = (Int32)GameState.TetraMasterWin;
            else if (name == "TetraMasterLossCount") args.Result = (Int32)GameState.TetraMasterLoss;
            else if (name == "TetraMasterDrawCount") args.Result = (Int32)GameState.TetraMasterDraw;
            else if (name == "GameTime") args.Result = (Int32)GameState.GameTime;
            else if (name == "BattleId") args.Result = (Int32)FF9StateSystem.Battle.battleMapIndex;
            else if (name == "FieldId") args.Result = (Int32)FF9StateSystem.Common.FF9.fldMapNo;
            else if (name == "IsRandomBattle") args.Result = Memoria.BattleState.IsRandomBattle;
            else if (name == "IsFriendlyBattle") args.Result = Memoria.BattleState.IsFriendlyBattle;
            else if (name == "IsRagtimeBattle") args.Result = Memoria.BattleState.IsRagtimeBattle;
            else if (name == "CurrentPartyCount") args.Result = Memoria.BattleState.BattleUnitCount(true);
            else if (name == "CurrentEnemyCount") args.Result = Memoria.BattleState.BattleUnitCount(false);
            else if (name == "IsBattlePreemptive") args.Result = FF9StateSystem.Battle?.FF9Battle?.btl_scene?.Info != null && FF9StateSystem.Battle.FF9Battle.btl_scene.Info.StartType == battle_start_type_tags.BTL_START_FIRST_ATTACK;
            else if (name == "IsBattleBackAttack") args.Result = FF9StateSystem.Battle?.FF9Battle?.btl_scene?.Info != null && FF9StateSystem.Battle.FF9Battle.btl_scene.Info.StartType == battle_start_type_tags.BTL_START_BACK_ATTACK;
            else if (name == "ScenarioCounter") args.Result = (Int32)((FF9StateSystem.EventState.gEventGlobal[1] << 8) | FF9StateSystem.EventState.gEventGlobal[0]);
            else if (name == "UseSFXRework") args.Result = Configuration.Battle.SFXRework;
            else
			{
                foreach (Type t in UsableEnumTypes)
				{
                    if (name.StartsWith(t.ToString() + "_"))
					{
                        String enumValueStr = name.Substring(t.ToString().Length + 1);
                        args.Result = (Int32)Enum.Parse(t, enumValueStr);
                        return;
                    }
				}
			}
        };

        public static EvaluateParameterHandler worldNCalcParameters = delegate (String name, ParameterArgs args)
        {
            if (name == "WorldDisc") args.Result = (Int32)ff9.w_frameDisc;
        };

        public static void InitializeExpressionPlayer(ref Expression expr, PLAYER play)
        {
            expr.Parameters["MaxHP"] = play.max.hp;
            expr.Parameters["MaxMP"] = play.max.mp;
            expr.Parameters["Level"] = (Int32)play.level;
            expr.Parameters["Exp"] = play.exp;
            expr.Parameters["Speed"] = (Int32)play.basis.dex;
            expr.Parameters["Strength"] = (Int32)play.basis.str;
            expr.Parameters["Magic"] = (Int32)play.basis.mgc;
            expr.Parameters["Spirit"] = (Int32)play.basis.wpr;
            expr.Parameters["Defence"] = (Int32)play.defence.PhisicalDefence;
            expr.Parameters["Evade"] = (Int32)play.defence.PhisicalEvade;
            expr.Parameters["MagicDefence"] = (Int32)play.defence.MagicalDefence;
            expr.Parameters["MagicEvade"] = (Int32)play.defence.MagicalEvade;
            expr.Parameters["PlayerCategory"] = (Int32)play.category;
            expr.Parameters["MPCostFactor"] = (Int32)play.mpCostFactor;
            expr.Parameters["CharacterIndex"] = (Int32)play.Index;
            expr.Parameters["SerialNumber"] = (Int32)play.info.serial_no;
            expr.Parameters["WeaponId"] = (Int32)play.equip.Weapon;
            expr.Parameters["WeaponShape"] = (Int32)ff9item._FF9Item_Data[play.equip.Weapon].shape;
            expr.Parameters["HeadId"] = (Int32)play.equip.Head;
            expr.Parameters["WristId"] = (Int32)play.equip.Wrist;
            expr.Parameters["ArmorId"] = (Int32)play.equip.Armor;
            expr.Parameters["AccessoryId"] = (Int32)play.equip.Accessory;
            expr.EvaluateFunction += delegate (String name, FunctionArgs args)
            {
                if (name == "HasSA" && args.Parameters.Length == 1)
                {
                    Int32 saIndex = (Int32)NCalcUtility.ConvertNCalcResult(args.Parameters[0].Evaluate(), -1);
                    if (saIndex < 0) args.Result = false;
                    else args.Result = ff9abil.FF9Abil_IsEnableSA(play.sa, 192 + (Int32)args.Parameters[0].Evaluate());
                }
            };
        }

        public static void InitializeExpressionBonus(ref Expression expr, BONUS bonus, List<FF9ITEM> bonus_item)
        {
            expr.Parameters["BonusAP"] = (Int32)bonus.ap;
            expr.Parameters["BonusCard"] = (Int32)bonus.card;
            expr.Parameters["BonusExp"] = bonus.exp;
            expr.Parameters["BonusGil"] = bonus.gil;
            for (Int32 i = 0; i < BattleResultUI.ItemMax; i++)
                if (i < bonus_item.Count)
                {
                    expr.Parameters["BonusItem" + (i + 1)] = (Int32)bonus_item[i].id;
                    expr.Parameters["BonusItemCount" + (i + 1)] = (Int32)bonus_item[i].count;
                }
                else
                {
                    expr.Parameters["BonusItem" + (i + 1)] = (Int32)Byte.MaxValue;
                    expr.Parameters["BonusItemCount" + (i + 1)] = 0;
                }
        }

        public static void InitializeExpressionUnit(ref Expression expr, BattleUnit unit, String prefix = "")
        {
            expr.Parameters[prefix + "MaxHP"] = unit.MaximumHp;
            expr.Parameters[prefix + "MaxMP"] = unit.MaximumMp;
            expr.Parameters[prefix + "MaxATB"] = (Int32)unit.MaximumAtb;
            expr.Parameters[prefix + "HP"] = unit.CurrentHp;
            expr.Parameters[prefix + "MP"] = unit.CurrentMp;
            expr.Parameters[prefix + "ATB"] = (Int32)unit.CurrentAtb;
            expr.Parameters[prefix + "Trance"] = (Int32)unit.Trance;
            expr.Parameters[prefix + "InTrance"] = unit.InTrance;
            expr.Parameters[prefix + "CurrentStatus"] = (UInt32)unit.CurrentStatus;
            expr.Parameters[prefix + "PermanentStatus"] = (UInt32)unit.PermanentStatus;
            expr.Parameters[prefix + "ResistStatus"] = (UInt32)unit.ResistStatus;
            expr.Parameters[prefix + "HalfElement"] = (Int32)unit.HalfElement;
            expr.Parameters[prefix + "GuardElement"] = (Int32)unit.GuardElement;
            expr.Parameters[prefix + "AbsorbElement"] = (Int32)unit.AbsorbElement;
            expr.Parameters[prefix + "WeakElement"] = (Int32)unit.WeakElement;
            expr.Parameters[prefix + "BonusElement"] = (Int32)unit.BonusElement;
            expr.Parameters[prefix + "WeaponPower"] = (Int32)unit.WeaponPower;
            expr.Parameters[prefix + "WeaponRate"] = (Int32)unit.WeaponRate;
            expr.Parameters[prefix + "WeaponElement"] = (Int32)unit.WeaponElement;
            expr.Parameters[prefix + "WeaponStatus"] = (UInt32)unit.WeaponStatus;
            expr.Parameters[prefix + "WeaponCategory"] = (Int32)unit.WeapCategory;
            expr.Parameters[prefix + "SerialNumber"] = (Int32)unit.SerialNumber;
            expr.Parameters[prefix + "Row"] = (Int32)unit.Row;
            expr.Parameters[prefix + "Position"] = (Int32)unit.Position;
            expr.Parameters[prefix + "SummonCount"] = (Int32)unit.SummonCount;
            expr.Parameters[prefix + "IsPlayer"] = unit.IsPlayer;
            expr.Parameters[prefix + "IsSlave"] = unit.Data.bi.slave == 1;
            expr.Parameters[prefix + "IsOutOfReach"] = unit.IsOutOfReach;
            expr.Parameters[prefix + "Level"] = (Int32)unit.Level;
            expr.Parameters[prefix + "Exp"] = unit.IsPlayer ? unit.Player.Data.exp : 0;
            expr.Parameters[prefix + "Speed"] = (Int32)unit.Dexterity;
            expr.Parameters[prefix + "Strength"] = (Int32)unit.Strength;
            expr.Parameters[prefix + "Magic"] = (Int32)unit.Magic;
            expr.Parameters[prefix + "Spirit"] = (Int32)unit.Will;
            expr.Parameters[prefix + "Defence"] = (Int32)unit.PhisicalDefence;
            expr.Parameters[prefix + "Evade"] = (Int32)unit.PhisicalEvade;
            expr.Parameters[prefix + "MagicDefence"] = (Int32)unit.MagicDefence;
            expr.Parameters[prefix + "MagicEvade"] = (Int32)unit.MagicEvade;
            expr.Parameters[prefix + "PlayerCategory"] = (Int32)unit.PlayerCategory;
            expr.Parameters[prefix + "Category"] = (Int32)unit.Category;
            expr.Parameters[prefix + "CharacterIndex"] = (Int32)unit.PlayerIndex;
            expr.Parameters[prefix + "IsStrengthModified"] = unit.StatModifier[0];
            expr.Parameters[prefix + "IsMagicModified"] = unit.StatModifier[1];
            expr.Parameters[prefix + "IsDefenceModified"] = unit.StatModifier[2];
            expr.Parameters[prefix + "IsEvadeModified"] = unit.StatModifier[3];
            expr.Parameters[prefix + "IsMagicDefenceModified"] = unit.StatModifier[4];
            expr.Parameters[prefix + "IsMagicEvadeModified"] = unit.StatModifier[5];
            expr.Parameters[prefix + "CriticalRateBonus"] = (Int32)unit.CriticalRateBonus;
            expr.Parameters[prefix + "CriticalRateWeakening"] = (Int32)unit.CriticalRateWeakening;
            expr.Parameters[prefix + "WeaponId"] = (Int32)unit.Weapon;
            expr.Parameters[prefix + "HeadId"] = (Int32)unit.Head;
            expr.Parameters[prefix + "WristId"] = (Int32)unit.Wrist;
            expr.Parameters[prefix + "ArmorId"] = (Int32)unit.Armor;
            expr.Parameters[prefix + "AccessoryId"] = (Int32)unit.Accessory;
            expr.Parameters[prefix + "ModelId"] = (Int32)unit.Data.dms_geo_id;
            expr.EvaluateFunction += delegate (String name, FunctionArgs args)
            {
                if (name == prefix + "HasSA" && args.Parameters.Length == 1)
                {
                    Int32 saIndex = (Int32)NCalcUtility.ConvertNCalcResult(args.Parameters[0].Evaluate(), 64);
                    if (saIndex < 32) args.Result = unit.HasSupportAbility((SupportAbility1)(1 << saIndex));
                    else if (saIndex < 64) args.Result = unit.HasSupportAbility((SupportAbility2)(1 << saIndex));
                }
            };
        }

        public static void InitializeExpressionAbilityContext(ref Expression expr, BattleCalculator calc)
        {
            BattleCaster caster = calc.Caster;
            BattleTarget target = calc.Target;
            CalcContext context = calc.Context;
            Int32 reflectCount = 0;
            if (calc.Command.Data.info.reflec == 1)
                for (UInt16 index = 0; index < 4; ++index)
                    if ((calc.Command.Data.reflec.tar_id[index] & target.Id) != 0)
                        ++reflectCount;
            expr.Parameters["ReflectFactor"] = reflectCount;
            expr.Parameters["EffectCasterFlags"] = (Int32)caster.Flags;
            expr.Parameters["CasterHPDamage"] = caster.HpDamage;
            expr.Parameters["CasterMPDamage"] = caster.MpDamage;
            expr.Parameters["EffectTargetFlags"] = (Int32)target.Flags;
            expr.Parameters["HPDamage"] = target.HpDamage;
            expr.Parameters["MPDamage"] = target.MpDamage;
            expr.Parameters["FigureInfo"] = target.Data.fig_info;
            expr.Parameters["Attack"] = context.Attack;
            expr.Parameters["AttackPower"] = context.AttackPower;
            expr.Parameters["DefencePower"] = context.DefensePower;
            expr.Parameters["StatusRate"] = (Int32)context.StatusRate;
            expr.Parameters["HitRate"] = (Int32)context.HitRate;
            expr.Parameters["Evade"] = (Int32)context.Evade;
            expr.Parameters["EffectFlags"] = (UInt16)context.Flags;
            expr.Parameters["StatusesInflicted"] = (UInt32)context.AddedStatuses;
            expr.Parameters["DamageModifierCount"] = (Int32)context.DamageModifierCount;
            expr.Parameters["TranceIncrease"] = (Int32)context.TranceIncrease;
            expr.Parameters["ItemSteal"] = (Int32)context.ItemSteal;
            expr.Parameters["IsDrain"] = context.IsDrain;
            InitializeExpressionCommand(ref expr, calc.Command);
        }

        public static void InitializeExpressionCommand(ref Expression expr, BattleCommand command)
        {
            expr.Parameters["CommandId"] = (Int32)command.Id;
            expr.Parameters["AbilityId"] = (Int32)command.AbilityId;
            expr.Parameters["ScriptId"] = (Int32)command.ScriptId;
            expr.Parameters["Power"] = (Int32)command.Power;
            expr.Parameters["AbilityStatus"] = (UInt32)command.AbilityStatus;
            expr.Parameters["AbilityElement"] = (Int32)command.Element;
            expr.Parameters["AbilityElementForBonus"] = (Int32)command.ElementForBonus;
            expr.Parameters["ItemUseId"] = command.Id == BattleCommandId.Item || command.Id == BattleCommandId.AutoPotion ? (Int32)command.AbilityId : Byte.MaxValue;
            expr.Parameters["WeaponThrowShape"] = command.Id == BattleCommandId.Throw ? ff9item._FF9Item_Data[(Int32)command.AbilityId].shape : -1;
            expr.Parameters["SpecialEffectId"] = (Int32)command.SpecialEffect;
            expr.Parameters["TargetType"] = (Int32)command.TargetType;
            expr.Parameters["IsATBCommand"] = command.IsATBCommand;
            expr.Parameters["IsAbilityMultiTarget"] = command.IsManyTarget;
            expr.Parameters["IsShortSummon"] = command.IsShortSummon;
            expr.Parameters["IsSpellReflected"] = command.Data.info.reflec == 1; // "reflec == 2" for the first effect when there are both reflected and non-reflected instances
            expr.Parameters["IsCovered"] = command.Data.info.cover == 1;
            expr.Parameters["IsDodged"] = command.Data.info.dodge == 1;
            expr.Parameters["IsShortRanged"] = command.IsShortRange;
            expr.Parameters["IsReflectNull"] = command.IsReflectNull;
            expr.Parameters["IsMeteorMiss"] = command.IsMeteorMiss;
            expr.Parameters["AbilityCategory"] = (Int32)command.AbilityCategory;
            expr.Parameters["MPCost"] = (Int32)command.Data.aa.MP;
            expr.Parameters["AbilityFlags"] = (Int32)command.AbilityType;
            expr.Parameters["CommandTargetId"] = (Int32)command.Data.tar_id;
            expr.Parameters["CalcMainCounter"] = (Int32)command.Data.info.effect_counter;
        }
    }
}

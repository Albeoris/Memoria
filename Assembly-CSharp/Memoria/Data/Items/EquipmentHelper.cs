using FF9;
using Memoria.Prime;
using Memoria.Prime.Text;
using NCalc;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Memoria.Data
{
    public static class EquipmentHelper
    {
        public class ItemFeatureEffect
        {
            public String Condition = "";
            public Expression ConditionExpression = null;

            public Dictionary<String, String> Formula = new Dictionary<String, String>();
            public Dictionary<String, Expression> FormulaExpression = new Dictionary<String, Expression>();

            public String ModFilePath = null;
            public Int32 FeatureLineNumber = -1;
        }

        public class ItemSpecialEffect
        {
            public String Condition = "";
            public Expression ConditionExpression = null;

            public Dictionary<String, String> Formula = new Dictionary<String, String>();
            public Dictionary<String, List<Expression>> FormulaExpressions = new Dictionary<String, List<Expression>>();

            public String ModFilePath = null;
            public Int32 FeatureLineNumber = -1;
        }

        public class ItemStatusInitEffect
        {
            public String Condition = "";
            public Expression ConditionExpression = null;

            public String ModFilePath = null;
            public Int32 FeatureLineNumber = -1;

            public BattleStatus PermanentStatus = 0;
            public BattleStatus InitialStatus = 0;
            public BattleStatus ResistStatus = 0;

            public List<KeyValuePair<BattleStatusId, Expression>> PartialResistStatus = new List<KeyValuePair<BattleStatusId, Expression>>();
            public List<KeyValuePair<BattleStatusId, Expression>> DurationFactorStatus = new List<KeyValuePair<BattleStatusId, Expression>>();

            public Int32 InitialATB = -1;
        }

        public static Dictionary<RegularItem, List<ItemFeatureEffect>> WeaponFeatures = new Dictionary<RegularItem, List<ItemFeatureEffect>>();
        public static Dictionary<RegularItem, List<ItemFeatureEffect>> HeadFeatures = new Dictionary<RegularItem, List<ItemFeatureEffect>>();
        public static Dictionary<RegularItem, List<ItemFeatureEffect>> WristFeatures = new Dictionary<RegularItem, List<ItemFeatureEffect>>();
        public static Dictionary<RegularItem, List<ItemFeatureEffect>> ArmorFeatures = new Dictionary<RegularItem, List<ItemFeatureEffect>>();
        public static Dictionary<RegularItem, List<ItemFeatureEffect>> AccessoryFeatures = new Dictionary<RegularItem, List<ItemFeatureEffect>>();
        public static Dictionary<RegularItem, List<ItemFeatureEffect>> GenericFeatures = new Dictionary<RegularItem, List<ItemFeatureEffect>>();
        public static List<ItemFeatureEffect> FlexibleItemFeatures = new List<ItemFeatureEffect>();

        public static Dictionary<RegularItem, List<ItemSpecialEffect>> WeaponSpecialFeatures = new Dictionary<RegularItem, List<ItemSpecialEffect>>();
        public static Dictionary<RegularItem, List<ItemSpecialEffect>> HeadSpecialFeatures = new Dictionary<RegularItem, List<ItemSpecialEffect>>();
        public static Dictionary<RegularItem, List<ItemSpecialEffect>> WristSpecialFeatures = new Dictionary<RegularItem, List<ItemSpecialEffect>>();
        public static Dictionary<RegularItem, List<ItemSpecialEffect>> ArmorSpecialFeatures = new Dictionary<RegularItem, List<ItemSpecialEffect>>();
        public static Dictionary<RegularItem, List<ItemSpecialEffect>> AccessorySpecialFeatures = new Dictionary<RegularItem, List<ItemSpecialEffect>>();
        public static Dictionary<RegularItem, List<ItemSpecialEffect>> GenericSpecialFeatures = new Dictionary<RegularItem, List<ItemSpecialEffect>>();
        public static List<ItemSpecialEffect> FlexibleSpecialFeatures = new List<ItemSpecialEffect>();

        public static Dictionary<RegularItem, List<ItemStatusInitEffect>> WeaponStatusFeatures = new Dictionary<RegularItem, List<ItemStatusInitEffect>>();
        public static Dictionary<RegularItem, List<ItemStatusInitEffect>> HeadStatusFeatures = new Dictionary<RegularItem, List<ItemStatusInitEffect>>();
        public static Dictionary<RegularItem, List<ItemStatusInitEffect>> WristStatusFeatures = new Dictionary<RegularItem, List<ItemStatusInitEffect>>();
        public static Dictionary<RegularItem, List<ItemStatusInitEffect>> ArmorStatusFeatures = new Dictionary<RegularItem, List<ItemStatusInitEffect>>();
        public static Dictionary<RegularItem, List<ItemStatusInitEffect>> AccessoryStatusFeatures = new Dictionary<RegularItem, List<ItemStatusInitEffect>>();
        public static Dictionary<RegularItem, List<ItemStatusInitEffect>> GenericStatusFeatures = new Dictionary<RegularItem, List<ItemStatusInitEffect>>();
        public static List<ItemStatusInitEffect> FlexibleStatusFeatures = new List<ItemStatusInitEffect>();

        private static String lastErrorMessage = null;

        public static void ClearItemFeature(RegularItem itemId)
        {
            if (WeaponFeatures.ContainsKey(itemId)) WeaponFeatures.Remove(itemId);
            if (HeadFeatures.ContainsKey(itemId)) HeadFeatures.Remove(itemId);
            if (WristFeatures.ContainsKey(itemId)) WristFeatures.Remove(itemId);
            if (ArmorFeatures.ContainsKey(itemId)) ArmorFeatures.Remove(itemId);
            if (AccessoryFeatures.ContainsKey(itemId)) AccessoryFeatures.Remove(itemId);
            if (GenericFeatures.ContainsKey(itemId)) GenericFeatures.Remove(itemId);

            if (WeaponSpecialFeatures.ContainsKey(itemId)) WeaponSpecialFeatures.Remove(itemId);
            if (HeadSpecialFeatures.ContainsKey(itemId)) HeadSpecialFeatures.Remove(itemId);
            if (WristSpecialFeatures.ContainsKey(itemId)) WristSpecialFeatures.Remove(itemId);
            if (ArmorSpecialFeatures.ContainsKey(itemId)) ArmorSpecialFeatures.Remove(itemId);
            if (AccessorySpecialFeatures.ContainsKey(itemId)) AccessorySpecialFeatures.Remove(itemId);
            if (GenericSpecialFeatures.ContainsKey(itemId)) GenericSpecialFeatures.Remove(itemId);

            if (WeaponStatusFeatures.ContainsKey(itemId)) WeaponStatusFeatures.Remove(itemId);
            if (HeadStatusFeatures.ContainsKey(itemId)) HeadStatusFeatures.Remove(itemId);
            if (WristStatusFeatures.ContainsKey(itemId)) WristStatusFeatures.Remove(itemId);
            if (ArmorStatusFeatures.ContainsKey(itemId)) ArmorStatusFeatures.Remove(itemId);
            if (AccessoryStatusFeatures.ContainsKey(itemId)) AccessoryStatusFeatures.Remove(itemId);
            if (GenericStatusFeatures.ContainsKey(itemId)) GenericStatusFeatures.Remove(itemId);
        }

        public static void ClearFlexibleItemFeature()
        {
            FlexibleItemFeatures.Clear();
            FlexibleSpecialFeatures.Clear();
            FlexibleStatusFeatures.Clear();
        }

        public static void ParseItemFeature(String featureCode, String modFilePath = null, Int32 initialLineNumber = 0)
        {
            ParseItemFeature(RegularItem.NoItem, featureCode, modFilePath, initialLineNumber, null);
        }

        public static void ParseItemFeature(RegularItem itemId, String featureCode, String modFilePath, Int32 initialLineNumber, String slotName)
        {
            MatchCollection codeMatches = new Regex(@"^(Permanent|StatusInit)\b", RegexOptions.Multiline).Matches(featureCode);

            if (codeMatches.Count == 0)
            {
                ParsePermanentBlock(itemId, featureCode, modFilePath, initialLineNumber, slotName);
                return;
            }

            for (Int32 i = 0; i < codeMatches.Count; i++)
            {
                String codeType = codeMatches[i].Groups[1].Value;
                Int32 endPos, startPos = codeMatches[i].Index + codeMatches[i].Length;

                if (i + 1 == codeMatches.Count) endPos = featureCode.Length;
                else endPos = codeMatches[i + 1].Index;

                String blockContent = featureCode.Substring(startPos, endPos - startPos);
                Int32 blockLine = initialLineNumber + Regex.Matches(featureCode.Substring(0, startPos), @"\n", RegexOptions.Singleline).Count;

                if (String.Equals(codeType, "Permanent"))
                    ParsePermanentBlock(itemId, blockContent, modFilePath, blockLine, slotName);
                else if (String.Equals(codeType, "StatusInit"))
                    ParseStatusInitBlock(itemId, blockContent, modFilePath, blockLine, slotName);
            }
        }

        private static void ParsePermanentBlock(RegularItem itemId, String content, String modFilePath, Int32 lineNumber, String slotName)
        {
            List<ItemFeatureEffect> targetStatList;
            List<ItemSpecialEffect> targetSpecialList;

            if (itemId == RegularItem.NoItem)
            {
                targetStatList = FlexibleItemFeatures;
                targetSpecialList = FlexibleSpecialFeatures;
            }
            else
            {
                Dictionary<RegularItem, List<ItemFeatureEffect>> statDict;
                Dictionary<RegularItem, List<ItemSpecialEffect>> specialDict;

                if (String.Equals(slotName, "Weapon")) { statDict = WeaponFeatures; specialDict = WeaponSpecialFeatures; }
                else if (String.Equals(slotName, "Head")) { statDict = HeadFeatures; specialDict = HeadSpecialFeatures; }
                else if (String.Equals(slotName, "Wrist")) { statDict = WristFeatures; specialDict = WristSpecialFeatures; }
                else if (String.Equals(slotName, "Armor")) { statDict = ArmorFeatures; specialDict = ArmorSpecialFeatures; }
                else if (String.Equals(slotName, "Accessory")) { statDict = AccessoryFeatures; specialDict = AccessorySpecialFeatures; }
                else { statDict = GenericFeatures; specialDict = GenericSpecialFeatures; }

                if (!statDict.ContainsKey(itemId)) statDict[itemId] = new List<ItemFeatureEffect>();
                if (!specialDict.ContainsKey(itemId)) specialDict[itemId] = new List<ItemSpecialEffect>();

                targetStatList = statDict[itemId];
                targetSpecialList = specialDict[itemId];
            }

            String condition = "";
            Dictionary<String, String> formulas = new Dictionary<String, String>();

            foreach (Match formula in new Regex(@"\[code=(.*?)\](.*?)\[/code\]").Matches(content))
            {
                if (String.Equals(formula.Groups[1].Value, "Condition"))
                    condition = formula.Groups[2].Value;
                else
                    formulas[formula.Groups[1].Value] = formula.Groups[2].Value;
            }

            Expression compiledCondition = null;
            if (!String.IsNullOrEmpty(condition))
                compiledCondition = NCalcUtility.PrepareExpression(condition);

            ItemFeatureEffect statEffect = new ItemFeatureEffect { ModFilePath = modFilePath, FeatureLineNumber = lineNumber, Condition = condition, ConditionExpression = compiledCondition };
            ItemSpecialEffect specialEffect = new ItemSpecialEffect { ModFilePath = modFilePath, FeatureLineNumber = lineNumber, Condition = condition, ConditionExpression = compiledCondition };

            foreach (var kvp in formulas)
            {
                if (IsSpecialKey(kvp.Key))
                {
                    specialEffect.Formula.Add(kvp.Key, kvp.Value);
                    List<Expression> compiledParts = new List<Expression>();
                    foreach (String part in kvp.Value.Split(';'))
                    {
                        compiledParts.Add(NCalcUtility.PrepareExpression(part));
                    }
                    specialEffect.FormulaExpressions.Add(kvp.Key, compiledParts);
                }
                else
                {
                    statEffect.Formula.Add(kvp.Key, kvp.Value);
                    statEffect.FormulaExpression.Add(kvp.Key, NCalcUtility.PrepareExpression(kvp.Value));
                }
            }

            if (statEffect.FormulaExpression.Count > 0) targetStatList.Add(statEffect);
            if (specialEffect.FormulaExpressions.Count > 0) targetSpecialList.Add(specialEffect);
        }

        private static void ParseStatusInitBlock(RegularItem itemId, String content, String modFilePath, Int32 lineNumber, String slotName)
        {
            List<ItemStatusInitEffect> targetList;
            if (itemId == RegularItem.NoItem) targetList = FlexibleStatusFeatures;
            else
            {
                Dictionary<RegularItem, List<ItemStatusInitEffect>> targetDict;
                if (String.Equals(slotName, "Weapon")) targetDict = WeaponStatusFeatures;
                else if (String.Equals(slotName, "Head")) targetDict = HeadStatusFeatures;
                else if (String.Equals(slotName, "Wrist")) targetDict = WristStatusFeatures;
                else if (String.Equals(slotName, "Armor")) targetDict = ArmorStatusFeatures;
                else if (String.Equals(slotName, "Accessory")) targetDict = AccessoryStatusFeatures;
                else targetDict = GenericStatusFeatures;

                if (!targetDict.ContainsKey(itemId)) targetDict[itemId] = new List<ItemStatusInitEffect>();
                targetList = targetDict[itemId];
            }

            ItemStatusInitEffect newEffect = new ItemStatusInitEffect();
            newEffect.ModFilePath = modFilePath;
            newEffect.FeatureLineNumber = lineNumber;

            foreach (Match formula in new Regex(@"\[code=(.*?)\](.*?)\[/code\]").Matches(content))
            {
                String codeName = formula.Groups[1].Value;
                if (String.Equals(codeName, "Condition"))
                {
                    newEffect.Condition = formula.Groups[2].Value;
                    newEffect.ConditionExpression = NCalcUtility.PrepareExpression(newEffect.Condition);
                }
                else if (codeName.StartsWith("PartialResist"))
                {
                    if (codeName.Substring("PartialResist".Length).TryEnumParse(out BattleStatusId status))
                        newEffect.PartialResistStatus.Add(new KeyValuePair<BattleStatusId, Expression>(status, NCalcUtility.PrepareExpression(formula.Groups[2].Value)));
                }
                else if (codeName.StartsWith("DurationFactor"))
                {
                    if (codeName.Substring("DurationFactor".Length).TryEnumParse(out BattleStatusId status))
                        newEffect.DurationFactorStatus.Add(new KeyValuePair<BattleStatusId, Expression>(status, NCalcUtility.PrepareExpression(formula.Groups[2].Value)));
                }
            }

            foreach (Match statusMatch in new Regex(@"\b((Auto|Initial|Resist)Status|InitialATB)\s+(\w+|\d+)\b").Matches(content))
            {
                if (String.Equals(statusMatch.Groups[1].Value, "InitialATB"))
                {
                    Int32.TryParse(statusMatch.Groups[3].Value, out newEffect.InitialATB);
                }
                else
                {
                    if (statusMatch.Groups[3].Value.TryEnumParse(out BattleStatus status))
                    {
                        if (String.Equals(statusMatch.Groups[2].Value, "Auto")) newEffect.PermanentStatus |= status;
                        else if (String.Equals(statusMatch.Groups[2].Value, "Initial")) newEffect.InitialStatus |= status;
                        else if (String.Equals(statusMatch.Groups[2].Value, "Resist")) newEffect.ResistStatus |= status;
                    }
                }
            }
            targetList.Add(newEffect);
        }

        private static Boolean IsSpecialKey(String key)
        {
            return String.Equals(key, "ActivateFreeSA") ||
                   String.Equals(key, "BanishSA") ||
                   String.Equals(key, "HiddenSA") ||
                   String.Equals(key, "ActivateFreeSAByLvl") ||
                   String.Equals(key, "BanishSAByLvl");
        }

        public static void TriggerOnEnable(PLAYER play)
        {
            foreach (ItemFeatureEffect effect in FlexibleItemFeatures) ApplyEffect(play, effect);
            if (play.equip.Weapon != RegularItem.NoItem) ApplySlotEffects(play, play.equip.Weapon, WeaponFeatures, GenericFeatures);
            if (play.equip.Head != RegularItem.NoItem) ApplySlotEffects(play, play.equip.Head, HeadFeatures, GenericFeatures);
            if (play.equip.Wrist != RegularItem.NoItem) ApplySlotEffects(play, play.equip.Wrist, WristFeatures, GenericFeatures);
            if (play.equip.Armor != RegularItem.NoItem) ApplySlotEffects(play, play.equip.Armor, ArmorFeatures, GenericFeatures);
            if (play.equip.Accessory != RegularItem.NoItem) ApplySlotEffects(play, play.equip.Accessory, AccessoryFeatures, GenericFeatures);
        }

        public static void TriggerSpecialFeature(PLAYER play)
        {
            foreach (ItemSpecialEffect effect in FlexibleSpecialFeatures) ApplySpecialEffect(play, effect);
            if (play.equip.Weapon != RegularItem.NoItem) ApplySlotSpecialEffects(play, play.equip.Weapon, WeaponSpecialFeatures, GenericSpecialFeatures);
            if (play.equip.Head != RegularItem.NoItem) ApplySlotSpecialEffects(play, play.equip.Head, HeadSpecialFeatures, GenericSpecialFeatures);
            if (play.equip.Wrist != RegularItem.NoItem) ApplySlotSpecialEffects(play, play.equip.Wrist, WristSpecialFeatures, GenericSpecialFeatures);
            if (play.equip.Armor != RegularItem.NoItem) ApplySlotSpecialEffects(play, play.equip.Armor, ArmorSpecialFeatures, GenericSpecialFeatures);
            if (play.equip.Accessory != RegularItem.NoItem) ApplySlotSpecialEffects(play, play.equip.Accessory, AccessorySpecialFeatures, GenericSpecialFeatures);
        }

        public static void TriggerOnStatusInit(BattleUnit unit)
        {
            if (!unit.IsPlayer) return;
            PLAYER play = unit.Player;
            foreach (ItemStatusInitEffect effect in FlexibleStatusFeatures) ApplyStatusEffect(unit, effect);
            if (play.equip.Weapon != RegularItem.NoItem) ApplySlotStatusEffects(unit, play.equip.Weapon, WeaponStatusFeatures, GenericStatusFeatures);
            if (play.equip.Head != RegularItem.NoItem) ApplySlotStatusEffects(unit, play.equip.Head, HeadStatusFeatures, GenericStatusFeatures);
            if (play.equip.Wrist != RegularItem.NoItem) ApplySlotStatusEffects(unit, play.equip.Wrist, WristStatusFeatures, GenericStatusFeatures);
            if (play.equip.Armor != RegularItem.NoItem) ApplySlotStatusEffects(unit, play.equip.Armor, ArmorStatusFeatures, GenericStatusFeatures);
            if (play.equip.Accessory != RegularItem.NoItem) ApplySlotStatusEffects(unit, play.equip.Accessory, AccessoryStatusFeatures, GenericStatusFeatures);
        }

        private static void ApplySlotEffects(PLAYER play, RegularItem itemId, Dictionary<RegularItem, List<ItemFeatureEffect>> specificDict, Dictionary<RegularItem, List<ItemFeatureEffect>> genericDict)
        {
            if (specificDict.ContainsKey(itemId)) foreach (var effect in specificDict[itemId]) ApplyEffect(play, effect);
            if (genericDict.ContainsKey(itemId)) foreach (var effect in genericDict[itemId]) ApplyEffect(play, effect);
        }

        private static void ApplySlotSpecialEffects(PLAYER play, RegularItem itemId, Dictionary<RegularItem, List<ItemSpecialEffect>> specificDict, Dictionary<RegularItem, List<ItemSpecialEffect>> genericDict)
        {
            if (specificDict.ContainsKey(itemId)) foreach (var effect in specificDict[itemId]) ApplySpecialEffect(play, effect);
            if (genericDict.ContainsKey(itemId)) foreach (var effect in genericDict[itemId]) ApplySpecialEffect(play, effect);
        }

        private static void ApplySlotStatusEffects(BattleUnit unit, RegularItem itemId, Dictionary<RegularItem, List<ItemStatusInitEffect>> specificDict, Dictionary<RegularItem, List<ItemStatusInitEffect>> genericDict)
        {
            if (specificDict.ContainsKey(itemId)) foreach (var effect in specificDict[itemId]) ApplyStatusEffect(unit, effect);
            if (genericDict.ContainsKey(itemId)) foreach (var effect in genericDict[itemId]) ApplyStatusEffect(unit, effect);
        }

        private static void ApplyEffect(PLAYER play, ItemFeatureEffect effect)
        {
            try
            {
                if (effect.ConditionExpression != null)
                {
                    Expression c = effect.ConditionExpression;
                    NCalcUtility.InitializeExpressionPlayer(ref c, play);
                    if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate())) return;
                }

                foreach (KeyValuePair<String, Expression> formula in effect.FormulaExpression)
                {
                    Expression e = formula.Value;
                    NCalcUtility.InitializeExpressionPlayer(ref e, play);

                    if (String.Equals(formula.Key, "MaxHP")) play.max.hp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.max.hp);
                    else if (String.Equals(formula.Key, "MaxMP")) play.max.mp = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.max.mp);
                    else if (String.Equals(formula.Key, "Speed")) play.elem.dex = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.elem.dex);
                    else if (String.Equals(formula.Key, "Strength")) play.elem.str = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.elem.str);
                    else if (String.Equals(formula.Key, "Magic")) play.elem.mgc = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.elem.mgc);
                    else if (String.Equals(formula.Key, "Spirit")) play.elem.wpr = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.elem.wpr);
                    else if (String.Equals(formula.Key, "Defence")) play.defence.PhysicalDefence = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.defence.PhysicalDefence);
                    else if (String.Equals(formula.Key, "Evade")) play.defence.PhysicalEvade = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.defence.PhysicalEvade);
                    else if (String.Equals(formula.Key, "MagicDefence")) play.defence.MagicalDefence = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.defence.MagicalDefence);
                    else if (String.Equals(formula.Key, "MagicEvade")) play.defence.MagicalEvade = (Int32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.defence.MagicalEvade);
                    else if (String.Equals(formula.Key, "PlayerCategory")) play.category = (Byte)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.category);
                    else if (String.Equals(formula.Key, "MPCostFactor")) play.mpCostFactor = (Int16)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.mpCostFactor);
                    else if (String.Equals(formula.Key, "MaxHPLimit")) play.maxHpLimit = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.maxHpLimit);
                    else if (String.Equals(formula.Key, "MaxMPLimit")) play.maxMpLimit = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.maxMpLimit);
                    else if (String.Equals(formula.Key, "MaxDamageLimit")) play.maxDamageLimit = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.maxDamageLimit);
                    else if (String.Equals(formula.Key, "MaxMPDamageLimit")) play.maxMpDamageLimit = (UInt32)NCalcUtility.ConvertNCalcResult(e.Evaluate(), play.maxMpDamageLimit);
                    else if (String.Equals(formula.Key, "PlayerPermanentStatus")) play.SetPermanentStatus((BattleStatus)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int64)play.permanent_status));
                    else if (String.Equals(formula.Key, "MaxGems"))
                    {
                        UInt32 newCapa = (UInt32)Math.Max(0, NCalcUtility.ConvertNCalcResult(e.Evaluate(), 0));
                        if (newCapa == UInt32.MaxValue) play.cur.capa = UInt32.MaxValue;
                        else if (play.cur.capa + newCapa - play.max.capa >= 0) play.cur.capa += newCapa - play.max.capa;
                        else play.cur.capa = 0;
                        play.max.capa = newCapa;
                    }
                }
            }
            catch (Exception err)
            {
                Log.Error($"Error in '{effect.ModFilePath}' item effect (line {effect.FeatureLineNumber}): {err}");
            }
        }

        private static void ApplySpecialEffect(PLAYER play, ItemSpecialEffect effect)
        {
            try
            {
                if (effect.ConditionExpression != null)
                {
                    Expression c = effect.ConditionExpression;
                    NCalcUtility.InitializeExpressionPlayer(ref c, play);
                    if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate())) return;
                }

                foreach (KeyValuePair<String, List<Expression>> kvp in effect.FormulaExpressions)
                {
                    List<Expression> expressions = kvp.Value;
                    for (Int32 j = 0; j < expressions.Count; j++)
                    {
                        Expression e = expressions[j];
                        NCalcUtility.InitializeExpressionPlayer(ref e, play);

                        SupportAbility sa = (SupportAbility)NCalcUtility.ConvertNCalcResult(e.Evaluate(), (Int32)SupportAbility.Void);

                        if (sa == SupportAbility.Void) continue;

                        if (String.Equals(kvp.Key, "ActivateFreeSA"))
                        {
                            play.saForced.Add(sa);
                        }
                        else if (String.Equals(kvp.Key, "BanishSA"))
                        {
                            if (!play.saBanish.Contains(sa))
                            {
                                ff9abil.DisableHierarchyFromSA(play, sa);
                                play.saBanish.Add(sa);
                            }
                        }
                        else if (String.Equals(kvp.Key, "HiddenSA"))
                        {
                            foreach (SupportAbility saToHide in ff9abil.GetHierarchyFromAnySA(sa))
                                play.saHidden.Add(saToHide);
                        }
                        else if (String.Equals(kvp.Key, "ActivateFreeSAByLvl"))
                        {
                            if (expressions.Count <= j + 1) break;

                            SupportAbility baseSA = ff9abil.GetBaseAbilityFromBoostedAbility(sa);

                            Expression elvl = expressions[j + 1];
                            NCalcUtility.InitializeExpressionPlayer(ref elvl, play);

                            Int32 levelSA = Math.Min((Int32)NCalcUtility.ConvertNCalcResult(elvl.Evaluate(), 0), ff9abil.GetBoostedAbilityMaxLevel(play, baseSA));
                            j++;

                            Boolean allSA = levelSA == -1;
                            Boolean skip = true;
                            foreach (SupportAbility saToForce in ff9abil.GetHierarchyFromAnySA(baseSA))
                            {
                                if (skip && saToForce != sa && !allSA) continue;
                                if (levelSA < 0 && !allSA) break;
                                skip = false;
                                play.saForced.Add(saToForce);
                                levelSA--;
                            }
                        }
                        else if (String.Equals(kvp.Key, "BanishSAByLvl"))
                        {
                            if (expressions.Count <= j + 1) break;

                            SupportAbility baseSA = ff9abil.GetBaseAbilityFromBoostedAbility(sa);

                            Expression elvl = expressions[j + 1];
                            NCalcUtility.InitializeExpressionPlayer(ref elvl, play);

                            Int32 levelSA = (Int32)NCalcUtility.ConvertNCalcResult(elvl.Evaluate(), 0);
                            j++;

                            Boolean allSA = levelSA == -1;
                            List<SupportAbility> listSAToBanish = ff9abil.GetHierarchyFromAnySA(baseSA);
                            listSAToBanish.Reverse();
                            foreach (SupportAbility saToForce in listSAToBanish)
                            {
                                if (levelSA <= 0 && !allSA) break;
                                levelSA--;
                                play.saBanish.Add(saToForce);
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Log.Error($"Error in '{effect.ModFilePath}' item special effect (line {effect.FeatureLineNumber}): {err}");
            }
        }

        private static void ApplyStatusEffect(BattleUnit unit, ItemStatusInitEffect effect)
        {
            try
            {
                if (effect.ConditionExpression != null)
                {
                    Expression c = effect.ConditionExpression;
                    NCalcUtility.InitializeExpressionUnit(ref c, unit);
                    if (!NCalcUtility.EvaluateNCalcCondition(c.Evaluate())) return;
                }

                unit.Data.stat.permanent |= effect.PermanentStatus;
                unit.Data.stat.cur |= effect.InitialStatus;
                unit.ResistStatus |= effect.ResistStatus;

                foreach (KeyValuePair<BattleStatusId, Expression> kvp in effect.PartialResistStatus)
                {
                    Expression e = kvp.Value;
                    NCalcUtility.InitializeExpressionUnit(ref e, unit);
                    unit.PartialResistStatus[kvp.Key] = NCalcUtility.ConvertNCalcResult(e.Evaluate(), 0f);
                }
                foreach (KeyValuePair<BattleStatusId, Expression> kvp in effect.DurationFactorStatus)
                {
                    Expression e = kvp.Value;
                    NCalcUtility.InitializeExpressionUnit(ref e, unit);
                    unit.StatusDurationFactor[kvp.Key] = NCalcUtility.ConvertNCalcResult(e.Evaluate(), 1f);
                }
                if (effect.InitialATB >= 0)
                    unit.CurrentAtb = (Int16)Math.Min(unit.MaximumAtb - 1, unit.MaximumAtb * effect.InitialATB / 100);
            }
            catch (Exception err)
            {
                Log.Error($"Error in '{effect.ModFilePath}' item status effect (line {effect.FeatureLineNumber}): {err}");
            }
        }
    }
}

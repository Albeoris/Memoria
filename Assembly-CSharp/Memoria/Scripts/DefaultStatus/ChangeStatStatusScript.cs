using System;
using UnityEngine;
using Memoria.Data;
using Object = System.Object;

namespace Memoria.DefaultScripts
{
    [StatusScript(BattleStatusId.ChangeStat)]
    public class ChangeStatStatusScript : StatusScriptBase
    {
        public Int32 DiffLevel = 0;
        public Int32 DiffStrength = 0;
        public Int32 DiffMagic = 0;
        public Int32 DiffDexterity = 0;
        public Int32 DiffWill = 0;
        public Int32 DiffMaximumHp = 0;
        public Int32 DiffMaximumMp = 0;
        public Int32 DiffCriticalRateBonus = 0;
        public Int32 DiffCriticalRateResistance = 0;
        public Int32 DiffPhysicalDefence = 0;
        public Int32 DiffPhysicalEvade = 0;
        public Int32 DiffMagicDefence = 0;
        public Int32 DiffMagicEvade = 0;

        public override UInt32 Apply(BattleUnit target, BattleUnit inflicter, params Object[] parameters)
        {
            base.Apply(target, inflicter, parameters);
            Boolean success = false;
            for (Int32 i = 0; i < parameters.Length; i += 2)
            {
                String kind = parameters[i] as String;
                if (kind == null)
                    continue;
                if (parameters[i + 1] is not Int32 && parameters[i + 1] is not UInt32 && parameters[i + 1] is not Int16 && parameters[i + 1] is not Byte)
                    continue;
                Int32 amount = Convert.ToInt32(parameters[i + 1]);
                switch (kind)
                {
                    case "Level":
                        amount = Mathf.Clamp(amount, 1, ff9level.LEVEL_COUNT);
                        if (amount != target.Level)
                        {
                            DiffLevel += amount - target.Level;
                            target.Level = (Byte)amount;
                            success = true;
                        }
                        break;
                    case "Strength":
                        amount = Mathf.Clamp(amount, 0, Byte.MaxValue);
                        if (amount != target.Strength)
                        {
                            DiffStrength += amount - target.Strength;
                            target.Strength = (Byte)amount;
                            success = true;
                        }
                        break;
                    case "Magic":
                        amount = Mathf.Clamp(amount, 0, Byte.MaxValue);
                        if (amount != target.Magic)
                        {
                            DiffMagic += amount - target.Magic;
                            target.Magic = (Byte)amount;
                            success = true;
                        }
                        break;
                    case "Dexterity":
                        amount = Mathf.Clamp(amount, 0, Byte.MaxValue);
                        if (amount != target.Dexterity)
                        {
                            DiffDexterity += amount - target.Dexterity;
                            target.Dexterity = (Byte)amount;
                            success = true;
                        }
                        break;
                    case "Will":
                        amount = Mathf.Clamp(amount, 0, Byte.MaxValue);
                        if (amount != target.Will)
                        {
                            DiffWill += amount - target.Will;
                            target.Will = (Byte)amount;
                            success = true;
                        }
                        break;
                    case "MaximumHp":
                        amount = Math.Max(amount, 1);
                        if (amount != target.MaximumHp)
                        {
                            DiffMaximumHp += amount - (Int32)target.MaximumHp;
                            target.MaximumHp = (UInt32)amount;
                            success = true;
                        }
                        break;
                    case "MaximumMp":
                        amount = Math.Max(amount, 0);
                        if (amount != target.MaximumMp)
                        {
                            DiffMaximumMp += amount - (Int32)target.MaximumMp;
                            target.MaximumMp = (UInt32)amount;
                            success = true;
                        }
                        break;
                    case "CriticalRateBonus":
                        amount = Mathf.Clamp(amount, Int16.MinValue, Int16.MaxValue);
                        if (amount != target.CriticalRateBonus)
                        {
                            DiffCriticalRateBonus += amount - target.CriticalRateBonus;
                            target.CriticalRateBonus = (Int16)amount;
                            success = true;
                        }
                        break;
                    case "CriticalRateResistance":
                        amount = Mathf.Clamp(amount, Int16.MinValue, Int16.MaxValue);
                        if (amount != target.CriticalRateResistance)
                        {
                            DiffCriticalRateResistance += amount - target.CriticalRateResistance;
                            target.CriticalRateResistance = (Int16)amount;
                            success = true;
                        }
                        break;
                    case "PhysicalDefence":
                        amount = Math.Max(amount, 0);
                        if (amount != target.PhysicalDefence)
                        {
                            DiffPhysicalDefence += amount - target.PhysicalDefence;
                            target.PhysicalDefence = amount;
                            success = true;
                        }
                        break;
                    case "PhysicalEvade":
                        amount = Math.Max(amount, 0);
                        if (amount != target.PhysicalEvade)
                        {
                            DiffPhysicalEvade += amount - target.PhysicalEvade;
                            target.PhysicalEvade = amount;
                            success = true;
                        }
                        break;
                    case "MagicDefence":
                        amount = Math.Max(amount, 0);
                        if (amount != target.MagicDefence)
                        {
                            DiffMagicDefence += amount - target.MagicDefence;
                            target.MagicDefence = amount;
                            success = true;
                        }
                        break;
                    case "MagicEvade":
                        amount = Math.Max(amount, 0);
                        if (amount != target.MagicEvade)
                        {
                            DiffMagicEvade += amount - target.MagicEvade;
                            target.MagicEvade = amount;
                            success = true;
                        }
                        break;
                }
            }
            return success ? btl_stat.ALTER_SUCCESS : btl_stat.ALTER_INVALID;
        }

        public override Boolean Remove()
        {
            Target.Level = (Byte)Mathf.Clamp(Target.Level - DiffLevel, 1, ff9level.LEVEL_COUNT);
            Target.Strength = (Byte)Mathf.Clamp(Target.Strength - DiffStrength, 1, Byte.MaxValue);
            Target.Magic = (Byte)Mathf.Clamp(Target.Magic - DiffMagic, 1, Byte.MaxValue);
            Target.Dexterity = (Byte)Mathf.Clamp(Target.Dexterity - DiffDexterity, 1, Byte.MaxValue);
            Target.Will = (Byte)Mathf.Clamp(Target.Will - DiffWill, 1, Byte.MaxValue);
            Target.MaximumHp = (UInt32)Math.Max(Target.MaximumHp - DiffMaximumHp, 0);
            Target.MaximumMp = (UInt32)Math.Max(Target.MaximumMp - DiffMaximumMp, 0);
            Target.CriticalRateBonus = (Int16)Mathf.Clamp(Target.CriticalRateBonus - DiffCriticalRateBonus, Int16.MinValue, Int16.MaxValue);
            Target.CriticalRateResistance = (Int16)Mathf.Clamp(Target.CriticalRateResistance - DiffCriticalRateResistance, Int16.MinValue, Int16.MaxValue);
            Target.PhysicalDefence = Math.Max(Target.PhysicalDefence - DiffPhysicalDefence, 0);
            Target.PhysicalEvade = Math.Max(Target.PhysicalEvade - DiffPhysicalEvade, 0);
            Target.MagicDefence = Math.Max(Target.MagicDefence - DiffMagicDefence, 0);
            Target.MagicEvade = Math.Max(Target.MagicEvade - DiffMagicEvade, 0);
            return true;
        }

        public Boolean RemovePartly(Boolean positivePart)
        {
            Boolean doneSomething = false;
            Int32 compareFactor = positivePart ? 1 : -1;
            if (compareFactor * DiffLevel > 0)
            {
                Target.Level = (Byte)Mathf.Clamp(Target.Level - DiffLevel, 1, ff9level.LEVEL_COUNT);
                DiffLevel = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffStrength > 0)
            {
                Target.Strength = (Byte)Mathf.Clamp(Target.Strength - DiffStrength, 1, Byte.MaxValue);
                DiffStrength = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffMagic > 0)
            {
                Target.Magic = (Byte)Mathf.Clamp(Target.Magic - DiffMagic, 1, Byte.MaxValue);
                DiffMagic = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffDexterity > 0)
            {
                Target.Dexterity = (Byte)Mathf.Clamp(Target.Dexterity - DiffDexterity, 1, Byte.MaxValue);
                DiffDexterity = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffWill > 0)
            {
                Target.Will = (Byte)Mathf.Clamp(Target.Will - DiffWill, 1, Byte.MaxValue);
                DiffWill = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffMaximumHp > 0)
            {
                Target.MaximumHp = (UInt32)Math.Max(Target.MaximumHp - DiffMaximumHp, 1);
                DiffMaximumHp = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffMaximumMp > 0)
            {
                Target.MaximumMp = (UInt32)Math.Max(Target.MaximumMp - DiffMaximumMp, 0);
                DiffMaximumMp = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffCriticalRateBonus > 0)
            {
                Target.CriticalRateBonus = (Int16)Mathf.Clamp(Target.CriticalRateBonus - DiffCriticalRateBonus, Int16.MinValue, Int16.MaxValue);
                DiffCriticalRateBonus = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffCriticalRateResistance > 0)
            {
                Target.CriticalRateResistance = (Int16)Mathf.Clamp(Target.CriticalRateResistance - DiffCriticalRateResistance, Int16.MinValue, Int16.MaxValue);
                DiffCriticalRateResistance = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffPhysicalDefence > 0)
            {
                Target.PhysicalDefence = Math.Max(Target.PhysicalDefence - DiffPhysicalDefence, 0);
                DiffPhysicalDefence = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffPhysicalEvade > 0)
            {
                Target.PhysicalEvade = Math.Max(Target.PhysicalEvade - DiffPhysicalEvade, 0);
                DiffPhysicalEvade = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffMagicDefence > 0)
            {
                Target.MagicDefence = Math.Max(Target.MagicDefence - DiffMagicDefence, 0);
                DiffMagicDefence = 0;
                doneSomething = true;
            }
            if (compareFactor * DiffMagicEvade > 0)
            {
                Target.MagicEvade = Math.Max(Target.MagicEvade - DiffMagicEvade, 0);
                DiffMagicEvade = 0;
                doneSomething = true;
            }
            return doneSomething;
        }
    }
}

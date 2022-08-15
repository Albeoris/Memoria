using System;
using FF9;
using Memoria.Data;

namespace Memoria
{
    public sealed class BattleItem
    {
        private readonly ITEM_DATA _data;

        internal BattleItem(ITEM_DATA data)
        {
            _data = data;
        }

        public static BattleItem Find(Byte itemId)
        {
            return new BattleItem(ff9item._FF9Item_Info[btl_util.btlItemNum(itemId)]);
        }

        public Byte ScriptId => _data.Ref.ScriptId;
        public Byte HitRate => _data.Ref.Rate;
        public Byte Power => _data.Ref.Power;
        public BattleStatus Status => (BattleStatus)_data.status;

        public static void AddToInventory(Int32 itemId)
        {
            UIManager.Battle.ItemAdd(itemId);
        }

        public static void RemoveFromInventory(Byte itemId)
        {
            UIManager.Battle.ItemRemove(itemId);
        }
    }

    public sealed class BattleEnemy
    {
        internal readonly ENEMY Data;

        internal BattleEnemy(ENEMY data)
        {
            Data = data;
        }

        public String Name => Data.et.name;
        public Byte[] StealableItems => Data.steal_item;
        public UInt16[] StealableItemRates => Data.steal_item_rate;
        public ENEMY GetData
        {
            get => Data;
        }

        public static BattleEnemy Find(BattleUnit unit)
        {
            ENEMY data = btl_util.getEnemyPtr(unit.Data);
            return new BattleEnemy(data);
        }
    }

    public sealed class BattleEnemyPrototype
    {
        private readonly ENEMY_TYPE _data;

        internal BattleEnemyPrototype(ENEMY_TYPE data)
        {
            _data = data;
        }

        public Byte BlueMagicId => _data.blue_magic_no;

        public static BattleEnemyPrototype Find(BattleUnit unit)
        {
            ENEMY_TYPE data = btl_util.getEnemyTypePtr(unit.Data);
            return new BattleEnemyPrototype(data);
        }
    }

    public class BattleCommand
    {
        internal readonly CMD_DATA Data;

        // Maybe move all these inside CMD_DATA...?

        internal BattleCommand(CMD_DATA data)
        {
            Data = data;
        }

        public BattleCommandId Id => (BattleCommandId)Data.cmd_no;
        public BattleAbilityId AbilityId => (BattleAbilityId)Data.sub_no;
        public Boolean IsManyTarget => Data.info.cursor == 1;
        public TargetType TargetType => (TargetType)Data.aa.Info.Target;
        public BattleStatusIndex AbilityStatusIndex => (BattleStatusIndex)Data.aa.AddStatusNo;
        public SpecialEffect SpecialEffect => (SpecialEffect)Data.aa.Info.VfxIndex;
        public Boolean IsMeteorMiss => Data.info.meteor_miss != 0;
        public Boolean IsShortSummon
        {
            get => Data.info.short_summon != 0;
            set => Data.info.short_summon = (Byte)(value ? 1 : 0);
        }
        public Boolean IsZeroMP => Data.info.IsZeroMP;

        public Boolean IsDevided => IsManyTarget && (Int32)Data.aa.Info.Target > 2 && (Int32)Data.aa.Info.Target < 6;

        public BattleItem Item => BattleItem.Find((Byte)AbilityId);
        public BattleStatus ItemStatus => Item.Status;
        public Weapon Weapon => Weapon.Find((Byte)AbilityId);

        public Boolean IsHeat => HasElement(EffectElement.Fire);
        public Boolean IsQuench => HasElement(EffectElement.Aqua | EffectElement.Cold);
        public Boolean IsGround => HasElement(EffectElement.Earth);

        public Boolean IsShortRange
		{
            get => Data.IsShortRange;
            set => Data.IsShortRange = value;
        }
        public Boolean IsReflectNull
        {
            get => Data.info.ReflectNull;
            set => Data.info.ReflectNull = value;
        }
        public Byte Power
        {
            get => Data.Power;
            set => Data.Power = value;
        }
        public Byte ScriptId
        {
            get => Data.ScriptId;
            set => Data.ScriptId = value;
        }
        public Byte HitRate
        {
            get => Data.HitRate;
            set => Data.HitRate = value;
        }
        public EffectElement Element
        {
            get => Data.Element;
            set => Data.Element = value;
        }
        public EffectElement ElementForBonus
        {
            get => Data.ElementForBonus;
            set => Data.ElementForBonus = value;
        }
        public BattleStatus AbilityStatus
        {
            get => Data.AbilityStatus;
            set => Data.AbilityStatus = value;
        }
        public Byte AbilityCategory
        {
            get => Data.AbilityCategory;
            set => Data.AbilityCategory = value;
        }
        public Byte AbilityType
        {
            get => Data.AbilityType;
            set => Data.AbilityType = value;
        }

        public Boolean HasElement(EffectElement element)
        {
            return (Element & element) != 0;
        }
    }
}
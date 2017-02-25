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
        private readonly ENEMY _data;

        internal BattleEnemy(ENEMY data)
        {
            _data = data;
        }

        public Byte[] StealableItems => _data.steal_item;

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

        internal BattleCommand(CMD_DATA data)
        {
            Data = data;
        }

        public BattleCommandId Id => (BattleCommandId)Data.cmd_no;
        public BattleAbilityId AbilityId => (BattleAbilityId)Data.sub_no;
        public Byte ScriptId => Data.aa.Ref.ScriptId;
        public Byte HitRate => Data.aa.Ref.Rate;
        public Byte Power => Data.aa.Ref.Power;
        public Boolean IsManyTarget => Data.info.cursor == 1;
        public TargetType TargetType => (TargetType)Data.aa.Info.Target;
        public BattleStatusIndex AbilityStatusIndex => (BattleStatusIndex)Data.aa.AddNo;
        public EffectElement Element => (EffectElement)Data.aa.Ref.Elements;
        public SpecialEffect SpecialEffect => (SpecialEffect)Data.aa.Info.VfxIndex;
        public Boolean IsMeteorMiss => Data.info.meteor_miss != 0;
        public Boolean IsShortSummon => Data.info.short_summon != 0;

        public BattleStatus AbilityStatus => FF9StateSystem.Battle.FF9Battle.add_status[Data.aa.AddNo].Value;
        public Boolean IsDevided => IsManyTarget && (Int32)Data.aa.Info.Target > 2 && (Int32)Data.aa.Info.Target < 6;

        public BattleItem Item => BattleItem.Find((Byte)AbilityId);
        public BattleStatus ItemStatus => Item.Status;
        public Weapon Weapon => Weapon.Find((Byte)AbilityId);

        public Boolean IsHeat => HasElement(EffectElement.Fire);
        public Boolean IsQuench => HasElement(EffectElement.Aqua | EffectElement.Cold);
        public Boolean IsGround => HasElement(EffectElement.Earth);

        public Boolean HasElement(EffectElement element)
        {
            return (Element & element) != 0;
        }
    }
}
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria.Data;
using System;

namespace Memoria
{
    public sealed class BattleItem
    {
        private readonly ITEM_DATA _data;

        public BattleItem(ITEM_DATA data)
        {
            _data = data;
        }

        public static implicit operator ITEM_DATA(BattleItem item) => item._data;

        public static BattleItem Find(RegularItem itemId)
        {
            return new BattleItem(ff9item.GetItemEffect(itemId));
        }

        public Int32 ScriptId => _data.Ref.ScriptId;
        public Int32 HitRate => _data.Ref.Rate;
        public Int32 Power => _data.Ref.Power;
        public BattleStatus Status => _data.status;

        public static void AddToInventory(RegularItem itemId)
        {
            UIManager.Battle.ItemAdd(itemId);
        }

        public static void RemoveFromInventory(RegularItem itemId)
        {
            UIManager.Battle.ItemRemove(itemId);
        }
    }

    public sealed class BattleEnemy
    {
        public readonly ENEMY Data;

        public BattleEnemy(ENEMY data)
        {
            Data = data;
        }

        public static implicit operator ENEMY(BattleEnemy enemy) => enemy.Data;

        public String Name => Data.et.name;
        public RegularItem[] StealableItems => Data.steal_item;
        public UInt16[] StealableItemRates => Data.steal_item_rate;
        public RegularItem[] DroppableItems => Data.bonus_item;
        public UInt16[] DroppableItemRates => Data.bonus_item_rate;
        public TetraMasterCardId DroppableCard => Data.bonus_card;
        public UInt16 DroppableCardRate => Data.bonus_card_rate;
        public UInt32 BonusExperience => Data.bonus_exp;
        public UInt32 BonusGil => Data.bonus_gil;
        public Boolean AttackOnDeath => Data.info.die_atk;
        public Boolean AlternateDeathAnim => Data.info.die_dmg;

        public static BattleEnemy Find(BattleUnit unit)
        {
            ENEMY data = btl_util.getEnemyPtr(unit.Data);
            return new BattleEnemy(data);
        }
    }

    public sealed class BattleEnemyPrototype
    {
        private readonly ENEMY_TYPE _data;

        public BattleEnemyPrototype(ENEMY_TYPE data)
        {
            _data = data;
        }

        public static implicit operator ENEMY_TYPE(BattleEnemyPrototype enemy) => enemy._data;

        public Int32 BlueMagicId => _data.blue_magic_no;

        public static BattleEnemyPrototype Find(BattleUnit unit)
        {
            ENEMY_TYPE data = btl_util.getEnemyTypePtr(unit.Data);
            return new BattleEnemyPrototype(data);
        }
    }

    public class BattleCommand
    {
        public readonly CMD_DATA Data;

        // Maybe move all these inside CMD_DATA...?

        public BattleCommand(CMD_DATA data)
        {
            Data = data;
        }

        public static implicit operator CMD_DATA(BattleCommand cmd) => cmd.Data;

        public BattleCommandId Id => Data.cmd_no;
        public Int32 RawIndex => Data.sub_no;
        public String AbilityName => AbilityId != BattleAbilityId.Void ? FF9TextTool.ActionAbilityName(AbilityId) : ItemId != RegularItem.NoItem ? FF9TextTool.ItemName(ItemId) : Data.aa.Name;
        public String AbilityCastingName => UIManager.Battle.GetBattleCommandTitle(Data);
        public BattleAbilityId AbilityId => btl_util.GetCommandMainActionIndex(Data);
        public RegularItem ItemId => btl_util.GetCommandItem(Data);
        public Boolean IsManyTarget => (Data.info.cursor & 1) != 0;
        public Int32 TargetCount => (Int32)btl_util.SumOfTarget(Data);
        public TargetType TargetType => Data.aa.Info.Target;
        public StatusSetId AbilityStatusIndex => Data.aa.AddStatusNo;
        public SpecialEffect SpecialEffect => (SpecialEffect)Data.aa.Info.VfxIndex;
        public BattleUnit Caster => Data.regist != null ? new BattleUnit(Data.regist) : null;
        public Boolean IsATBCommand => Data.regist != null && Data == Data.regist.cmd[0];
        public Boolean IsMeteorMiss => Data.info.meteor_miss != 0;
        public Boolean IsShortSummon
        {
            get => Data.info.short_summon != 0;
            set => Data.info.short_summon = (Byte)(value ? 1 : 0);
        }
        public Boolean IsZeroMP => Data.info.IsZeroMP;
        public Int32 CommandMPCost => Data.GetCommandMPCost(); // This takes AA features into account but not increased summon cost early on or player MP cost factor
        public BattleCommandMenu CommandMenu => Data.info.cmdMenu;
        public command_mode_index ExecutionStep => Data.info.mode;

        public Boolean IsDevided => IsManyTarget && Data.aa.Info.Target >= TargetType.ManyAny && Data.aa.Info.Target <= TargetType.ManyEnemy;

        public BattleItem Item => BattleItem.Find(ItemId);
        public BattleStatus ItemStatus => Item.Status;
        public Weapon Weapon => Weapon.Find(ItemId);

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
        public Int32 Power
        {
            get => Data.Power;
            set => Data.Power = value;
        }
        public Int32 ScriptId
        {
            get => Data.ScriptId;
            set => Data.ScriptId = value;
        }
        public Int32 HitRate
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

        public Int32 GetReflectMultiplierOnTarget(UInt16 targetId)
        {
            // Always return at least 1 even if not part of the command's targets, in order to take possible .seq target changes into account properly
            if (Data.info.reflec != 1)
                return 1;
            Int32 reflectMultiplier = 0;
            for (UInt16 index = 0; index < 4; ++index)
                if ((Data.reflec.tar_id[index] & targetId) != 0)
                    ++reflectMultiplier;
            return Math.Max(1, reflectMultiplier);
        }
    }
}

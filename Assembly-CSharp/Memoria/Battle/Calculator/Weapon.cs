using System;
using FF9;
using Memoria.Data;

namespace Memoria
{
    public sealed class Character
    {
        private readonly PLAYER _data;

        internal Character(PLAYER data)
        {
            _data = data;
        }

        public static Character Find(BattleUnit unit)
        {
            PLAYER data = FF9StateSystem.Common.FF9.player[(Int32)unit.PresetId];
            return new Character(data);
        }

        public Byte Row => _data.info.row;
        public CharacterCategory Category => (CharacterCategory)_data.category;
    }

    public sealed class Weapon
    {
        private readonly ItemAttack _data;

        internal Weapon(ItemAttack data)
        {
            _data = data;
        }

        public WeaponCategory Category => (WeaponCategory)_data.Category;
        public BattleStatus Status => FF9StateSystem.Battle.FF9Battle.add_status[_data.StatusIndex].Value;
        public UInt16 ModelId => _data.ModelId;
        public Byte ScriptId => _data.Ref.ScriptId;
        public Byte Power => _data.Ref.Power;
        public EffectElement Element => (EffectElement)_data.Ref.Elements;
        public Byte HitRate => _data.Ref.Rate;

        public static Weapon Find(Byte weaponId)
        {
            return new Weapon(ff9weap.WeaponData[btl_util.ff9WeaponNum(weaponId)]);
        }
    }
}
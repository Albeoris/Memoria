using Memoria.Data;
using System;

namespace Memoria
{
	public sealed class Character
	{
		public readonly PLAYER Data;

		public Character(PLAYER data)
		{
			Data = data;
		}

		public static implicit operator PLAYER(Character character) => character.Data;

		public String Name => Data.Name;
		public CharacterId Index => Data.Index;
		public CharacterPresetId PresetId => Data.PresetId;
		public CharacterEquipment Equipment => Data.equip;
		public Byte Row => Data.info.row;
		public CharacterCategory Category => (CharacterCategory)Data.category;
		public Boolean IsMainCharacter => !Data.IsSubCharacter;
		public Boolean IsSubCharacter => Data.IsSubCharacter;

		public static Character Find(BattleUnit unit)
		{
			PLAYER data = FF9StateSystem.Common.FF9.GetPlayer(unit.PlayerIndex);
			return new Character(data);
		}
	}

	public sealed class Weapon
	{
		private readonly ItemAttack _data;

		public Weapon(ItemAttack data)
		{
			_data = data;
		}

		public static implicit operator ItemAttack(Weapon weap) => weap._data;

		public WeaponCategory Category => _data.Category;
		public BattleStatus Status => FF9StateSystem.Battle.FF9Battle.add_status[_data.StatusIndex].Value;
		public UInt16 ModelId => _data.ModelId;
		public Int32 ScriptId => _data.Ref.ScriptId;
		public Int32 Power => _data.Ref.Power;
		public EffectElement Element => (EffectElement)_data.Ref.Elements;
		public Int32 HitRate => _data.Ref.Rate;

		public static Weapon Find(RegularItem weaponId)
		{
			return new Weapon(ff9item.GetItemWeapon(weaponId));
		}
	}
}

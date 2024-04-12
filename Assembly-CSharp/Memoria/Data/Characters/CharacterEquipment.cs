using Memoria.Prime.CSV;
using System;

namespace Memoria.Data
{
	public sealed class CharacterEquipment : ICsvEntry
	{
		public const Int32 Length = 5;

		public String Comment;
		public EquipmentSetId Id;

		public RegularItem Weapon;
		public RegularItem Head;
		public RegularItem Wrist;
		public RegularItem Armor;
		public RegularItem Accessory;

		public void Absorb(CharacterEquipment other)
		{
			Weapon = other.Weapon;
			Head = other.Head;
			Wrist = other.Wrist;
			Armor = other.Armor;
			Accessory = other.Accessory;
		}

		public CharacterEquipment Clone()
		{
			return new CharacterEquipment { Weapon = Weapon, Head = Head, Wrist = Wrist, Armor = Armor, Accessory = Accessory };
		}

		public void ParseEntry(String[] raw, CsvMetaData metadata)
		{
			Comment = CsvParser.String(raw[0]);
			Id = (EquipmentSetId)CsvParser.Int32(raw[1]);

			Weapon = (RegularItem)CsvParser.Item(raw[2]);
			Head = (RegularItem)CsvParser.Item(raw[3]);
			Wrist = (RegularItem)CsvParser.Item(raw[4]);
			Armor = (RegularItem)CsvParser.Item(raw[5]);
			Accessory = (RegularItem)CsvParser.Item(raw[6]);
		}

		public void WriteEntry(CsvWriter writer, CsvMetaData metadata)
		{
			writer.String(Comment);
			writer.Int32((Int32)Id);

			writer.Item((Int32)Weapon);
			writer.Item((Int32)Head);
			writer.Item((Int32)Wrist);
			writer.Item((Int32)Armor);
			writer.Item((Int32)Accessory);
		}

		public RegularItem this[Int32 index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return Weapon;
					case 1:
						return Head;
					case 2:
						return Wrist;
					case 3:
						return Armor;
					case 4:
						return Accessory;
					default:
						throw new ArgumentOutOfRangeException(nameof(index));
				}
			}
			set
			{
				switch (index)
				{
					case 0:
						Weapon = value;
						break;
					case 1:
						Head = value;
						break;
					case 2:
						Wrist = value;
						break;
					case 3:
						Armor = value;
						break;
					case 4:
						Accessory = value;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(index));
				}
			}
		}

		public void Change(Int32 itemType, RegularItem itemId)
		{
			RegularItem currentItemId = this[itemType];
			if (currentItemId == itemId)
				return;

			// Unequip
			if (currentItemId != RegularItem.NoItem)
			{
				if (currentItemId == RegularItem.Moonstone)
					ff9item.DecreaseMoonStoneCount();

				ff9item.FF9Item_Add(currentItemId, 1);
			}

			// Equip
			if (itemId != RegularItem.NoItem)
				ff9item.FF9Item_Remove(itemId, 1); // Ignore missing item

			this[itemType] = itemId;
		}
	}
}

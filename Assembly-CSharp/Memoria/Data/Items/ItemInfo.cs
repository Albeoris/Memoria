using System;
using Memoria.Prime.CSV;

namespace Memoria.Data
{
    public class ItemInfo : ICsvEntry
    {
        public UInt16 Price;
        public ItemCharacter CharacterMask;
        public Byte GraphicsId;
        public Byte ColorId;
        public Byte Quality;
        public Byte BonusId;
        public Byte[] AbilityIds;
        public ItemType TypeMask;
        public Byte Order;

        public void ParseEntry(String[] raw)
        {
            Int32 index = 0;

            Price = CsvParser.UInt16(raw[index++]);
            GraphicsId = CsvParser.Byte(raw[index++]);
            ColorId = CsvParser.Byte(raw[index++]);
            Quality = CsvParser.Byte(raw[index++]);
            BonusId = CsvParser.Byte(raw[index++]);
            AbilityIds = CsvParser.ByteArray(raw[index++]);

            Byte type = 0;
            for (Int32 i = 0; i < 8; i++)
            {
                type <<= 1;
                type |= CsvParser.Byte(raw[index++]);
            }
            TypeMask = (ItemType)type;

            Order = CsvParser.Byte(raw[index++]);

            UInt16 equippable = 0;
            for (Int32 i = 0; i < 12; i++)
            {
                equippable <<= 1;
                equippable |= CsvParser.Byte(raw[index++]);
            }

            CharacterMask = (ItemCharacter)equippable;
        }

        public void WriteEntry(CsvWriter writer)
        {
            writer.UInt16(Price);
            writer.Byte(GraphicsId);
            writer.Byte(ColorId);
            writer.Byte(Quality);
            writer.Byte(BonusId);
            writer.ByteArray(AbilityIds);

            writer.Boolean(Weapon);
            writer.Boolean(Armlet);
            writer.Boolean(Helmet);
            writer.Boolean(Armor);
            writer.Boolean(Accessory);
            writer.Boolean(Item);
            writer.Boolean(Gem);
            writer.Boolean(Usable);

            writer.Byte(Order);

            writer.Boolean(Zidane);
            writer.Boolean(Vivi);
            writer.Boolean(Garnet);
            writer.Boolean(Steiner);
            writer.Boolean(Freya);
            writer.Boolean(Quina);
            writer.Boolean(Eiko);
            writer.Boolean(Amarant);
            writer.Boolean(Cinna);
            writer.Boolean(Marcus);
            writer.Boolean(Blank);
            writer.Boolean(Beatrix);
        }

        public FF9ITEM_DATA ToItemData()
        {
            return new FF9ITEM_DATA(0, 0, Price, (UInt16)CharacterMask, GraphicsId, ColorId, Quality, BonusId, AbilityIds, (Byte)TypeMask, Order, 0);
        }

        public static ItemInfo FromItemData(FF9ITEM_DATA entry)
        {
            return new ItemInfo
            {
                Price = entry.price,
                CharacterMask = (ItemCharacter)entry.equip,
                GraphicsId = entry.shape,
                ColorId = entry.color,
                Quality = entry.eq_lv,
                BonusId = entry.bonus,
                AbilityIds = entry.ability,
                TypeMask = (ItemType)entry.type,
                Order = entry.sort
            };
        }

        public Boolean Weapon => (TypeMask & ItemType.Weapon) == ItemType.Weapon;
        public Boolean Armlet => (TypeMask & ItemType.Armlet) == ItemType.Armlet;
        public Boolean Helmet => (TypeMask & ItemType.Helmet) == ItemType.Helmet;
        public Boolean Armor => (TypeMask & ItemType.Armor) == ItemType.Armor;
        public Boolean Accessory => (TypeMask & ItemType.Accessory) == ItemType.Accessory;
        public Boolean Item => (TypeMask & ItemType.Item) == ItemType.Item;
        public Boolean Gem => (TypeMask & ItemType.Gem) == ItemType.Gem;
        public Boolean Usable => (TypeMask & ItemType.Usable) == ItemType.Usable;

        public Boolean Zidane => (CharacterMask & ItemCharacter.Zidane) == ItemCharacter.Zidane;
        public Boolean Vivi => (CharacterMask & ItemCharacter.Vivi) == ItemCharacter.Vivi;
        public Boolean Garnet => (CharacterMask & ItemCharacter.Garnet) == ItemCharacter.Garnet;
        public Boolean Steiner => (CharacterMask & ItemCharacter.Steiner) == ItemCharacter.Steiner;
        public Boolean Freya => (CharacterMask & ItemCharacter.Freya) == ItemCharacter.Freya;
        public Boolean Quina => (CharacterMask & ItemCharacter.Quina) == ItemCharacter.Quina;
        public Boolean Eiko => (CharacterMask & ItemCharacter.Eiko) == ItemCharacter.Eiko;
        public Boolean Amarant => (CharacterMask & ItemCharacter.Amarant) == ItemCharacter.Amarant;
        public Boolean Cinna => (CharacterMask & ItemCharacter.Cinna) == ItemCharacter.Cinna;
        public Boolean Marcus => (CharacterMask & ItemCharacter.Marcus) == ItemCharacter.Marcus;
        public Boolean Blank => (CharacterMask & ItemCharacter.Blank) == ItemCharacter.Blank;
        public Boolean Beatrix => (CharacterMask & ItemCharacter.Beatrix) == ItemCharacter.Beatrix;
    }

    [Flags]
    public enum ItemType : byte
    {
        Weapon = 128,
        Armlet = 64,
        Helmet = 32,
        Armor = 16,
        Accessory = 8,

        Item = 4,
        Gem = 2,
        Usable = 1
    }

    [Flags]
    public enum ItemCharacter : ushort
    {
        Zidane = 2048,
        Vivi = 1024,
        Garnet = 512,
        Steiner = 256,
        Freya = 128,
        Quina = 64,
        Eiko = 32,
        Amarant = 16,
        Cinna = 8,
        Marcus = 4,
        Blank = 2,
        Beatrix = 1
    }
}
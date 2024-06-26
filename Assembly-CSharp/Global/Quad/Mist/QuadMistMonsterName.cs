using System;
using System.Collections.Generic;

public class QuadMistMonsterName
{
	public static String GetMonsterName(Int32 monsterIndex)
	{
		if (QuadMistMonsterName.MonsterNameIndex == null)
		{
			QuadMistMonsterName.MonsterNameIndex = new Dictionary<Int32, String>();
			QuadMistMonsterName.CreateMonsterIndex();
		}
		return QuadMistMonsterName.MonsterNameIndex[monsterIndex];
	}

	private static void CreateMonsterIndex()
	{
		Int32 num = -1;
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Goblin");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Fang");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Skeleton");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Flan");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Zaghnol");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Lizardman");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Zombie");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Bomb");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Ironite");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Sahagin");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Yeti");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Mimic");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Wyerd");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Mandragora");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Crawler");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "S. Scorpion");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Nymph");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Sand Golem");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Zuu");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Dragonfly");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Carrion Worm");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Cerberus");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Antlion");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Catuar");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Gimme Cat");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Ragtimer");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Hedgehog Pie");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Ralvuimago");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Ochu");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Troll");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Blazer Beetle");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Abomination");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Zemzelett");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Stroper");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Tantarian");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Grand Dragon");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Feather Circle");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Hecteyes");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Orge");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Armstrong");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Ash");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Wraith");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Gargoyle");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Vepal");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Grimlock");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Tonberry");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Veteran");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Garuda");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Malboro");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Mover");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Abadon");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Behemoth");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Iron Man");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Nova Dragon");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Ozma");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Hades");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Holy");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Meteor");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Flare");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Shiva");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Ifrit");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Ramuh");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Atomos");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Odin");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Leviathan");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Bahamut");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Ark");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Fenrir");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Madeen");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Alexander");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Excalibur 2");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Ultima Weapon");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Masamune");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Elixer");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Dark Matter");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Ribbon");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Tiger Paw Racket");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Save the Queen");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Genji");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Mythril Sword");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Blue Narciss");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Hilda Garde 3");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Invincible");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Cargo Ship");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Hilda Garde 1");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Red Rose");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Theater Ship");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Viltgance");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Chocobo");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Fat Chocobo");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Mog");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Frog");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Oglop");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Alexandria");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Lindblum");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Twin Moons");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Gargant");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Namingway");
		QuadMistMonsterName.MonsterNameIndex.Add(++num, "Boco THE Chocobo");
		QuadMistMonsterName.MonsterNameIndex.Add(num + 1, "Airship");
	}

	private static Dictionary<Int32, String> MonsterNameIndex;
}

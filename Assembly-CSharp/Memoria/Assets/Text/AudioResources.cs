using System;
using System.Collections.Generic;
using System.Text;

namespace Memoria.Assets
{
	public static class AudioResources
	{
		public static class Embaded
		{
			public static String GetSoundPath(String relativePath)
			{
				return "Sounds/" + relativePath + ".akb";
			}
		}

		public static class Export
		{
			public static String GetSoundPath(String relativePath)
			{
				String path = Configuration.Export.Path;
				StringBuilder sb = new StringBuilder(path.Length + 32);
				sb.Append(path);
				if (sb.Length > 0 && sb[sb.Length - 1] != '/' && sb[sb.Length - 1] != '\\')
					sb.Append('/');
				sb.Append("Sounds/");
				sb.Append(relativePath);
				return sb.ToString();
			}
		}

		public static class Import
		{
			public static String GetSoundPath(String relativePath)
			{
				String path = Configuration.Import.Path;
				StringBuilder sb = new StringBuilder(path.Length + 32);
				sb.Append(path);
				if (sb.Length > 0 && sb[sb.Length - 1] != '/' && sb[sb.Length - 1] != '\\')
					sb.Append('/');
				sb.Append("Sounds/");
				sb.Append(relativePath);
				return sb.ToString();
			}
		}

		public static Boolean TryAppendDisplayName(String relativePath, out String directoryPath, out String fileName, out String newRelativePath)
		{
			newRelativePath = relativePath;
			if (String.IsNullOrEmpty(relativePath))
			{
				directoryPath = relativePath;
				fileName = relativePath;
				return false;
			}

			// Split the path
			Int32 index = relativePath.LastIndexOf('/');
			if (index == relativePath.Length - 1)
			{
				directoryPath = relativePath;
				fileName = String.Empty;
				return false;
			}

			if (index == 0)
			{
				directoryPath = String.Empty;
				fileName = relativePath.Substring(1);
			}
			else if (index < 0)
			{
				directoryPath = null;
				fileName = relativePath;
			}
			else
			{
				directoryPath = relativePath.Substring(0, index);
				fileName = relativePath.Substring(index + 1);
			}

			//bool skipRename = false;
			// Find a display name
			String displayName;
			if (fileName.Length == 8 && fileName.StartsWith("music")) // music???
			{
				if (!MusicDisplayNames.TryGetValue(fileName, out displayName))
					return false;
			}
			else if (fileName.Length == 8 && fileName.StartsWith("se"))
			{
				if (!SoundDisplayNames.TryGetValue(fileName, out displayName))
					return false;
			}
			else if (fileName.StartsWith("va_"))
			{
				string[] pathParts = directoryPath.Split(new char[] { '/' });
				displayName = fileName;
				//skipRename = true;
			}
			// TODO: FMV, SE, Songs
			else
			{
				return false;
			}

			// Join it to the new path
			if (directoryPath == null)
			{
				newRelativePath = fileName + " - " + displayName;
			}
			else
			{
				newRelativePath = directoryPath + '/' + fileName + " - " + displayName;
			}

			return true;
		}

		private static readonly Dictionary<String, String> MusicDisplayNames = new Dictionary<string, string>
		{
			{@"music000", "Victory Fanfare"},
			{@"music001", "Game Over"},
			{@"music002", "Dali Village"},
			{@"music003", "Evil Forest"},
			{@"music004", "Passive Sorrow (Music that plays at the beginning of Disc 4)"},
			{@"music005", "Amarant's Theme"},
			{@"music006", "Battle Theme"},
			{@"music007", "Steiner's Theme (Disc 1)"},
			{@"music008", "Vivi's Theme (Disc 1)"},
			{@"music009", "Quina's Theme (Frog Catching)"},
			{@"music010", "Garnet's Theme (Reminisce of the past, her childhood) - Disc 2"},
			{@"music011", "Freya's Theme (Gizamaluke's Grotto)"},
			{@"music012", "Aloha de chocobo (Chocobo's Forest)"},
			{@"music013", "Decisive Action (First meeting Steiner, Disc 1)"},
			{@"music014", "Stolen Eyes (Zidane and Dagger first talk on the Prima Vista Airship) Disc 1"},
			{@"music015", "Vamo' Alla Flamenco (Chocobo Hot & Cold)"},
			{@"music016", "King Leo (Prima Vista Theatre Stage - 3 Bell strike, under the axe you shall be) Disc 1"},
			{@"music017", "Sword of Fury (Prima Vista first battle on stage)"},
			{@"music018", "Strategy Conference (Beginning of Disc 1, planning kidnap Garnet)"},
			{@"music020", "Queen of the Abyss (Queen Branhe's Theme)"},
			{@"music021", "The Fated Hour - (Prima Vista, Dagger takes the stage with Ruby's role)"},
			{@"music022", "Mistaken Love (Marcus kills Cornelia instead)"},
			{@"music023", "Zidane's Theme (Provocative Zidane - boarding the Cargo Ship) Disc 1"},
			{@"music024", "Boss Battle Theme"},
			{@"music025", "Oeilvert"},
			{@"music026", "Tantalus Theme (...Disc 3, cutscene, after Mount Gulug, finding Dagger somewhere in Alexandria)"},
			{@"music027", "One Danger put behind us (Disc 1, the pub, meeting Freya first time)"},
			{@"music028", "You're Not Alone"},
			{@"music029", "Unforgettable Face (Sir Fratly and Freya's Theme)"},
			{@"music030", "Memories of that Day (Disc 1, Zidane Reminisces about meeting Garnet)"},
			{@"music031", "Ice Cavern"},
			{@"music032", "Qu's Marsh"},
			{@"music033", "Title Music"},
			{@"music036", "RUN!"},
			{@"music037", "Jesters of the Moon (Zorn and Thorn's theme)"},
			{@"music038", "Faerie Battle"},
			{@"music039", "Reckless Steiner (Steiner's Theme Pt. II)"},
			{@"music041", "Prima Vista, Music Room"},
			{@"music042", "Quad Mist (Tetra Master)"},
			{@"music043", "Far away in the village (Discover the underground production area of Dali, Disc 1)"},
			{@"music044", "Burmecia"},
			{@"music045", "Crossing those Hills (World Map Theme)"},
			{@"music046", "Mognet Central"},
			{@"music047", "Lindblum Theme (Town areas)"},
			{@"music048", "Fossil Roo"},
			{@"music049", "Cleyra Settlement"},
			{@"music050", "Eidolon Wall (Madian Sari)"},
			{@"music051", "Unfathomed Reminisce (Alexandria's Theme Disc 3-4)"},
			{@"music052", "Orchestra in the Forest (Disc 1, ATE, Evil Forest)"},
			{@"music053", "Vivi's Theme pt. II (Disc 1, Cargo Ship, other black mages refuse to talk with Vivi)"},
			{@"music054", "Black Mage Village"},
			{@"music055", "Eternal Harvest - (Ceremonial Dance, strengthen Cleyra's Sandstorm)"},
			{@"music056", "Pandemonium Theme (After 'You're not alone' montage fights)"},
			{@"music057", "Dark City Treno"},
			{@"music058", "Sneaky frog & the scoundrel (Cid's Red light, Green light game with Hedgehog pie)"},
			{@"music059", "Bran Bal"},
			{@"music060", "Eiko's Theme"},
			{@"music061", "Greive over the skies (After ceremonial dance, Cleyra) Disc 2 (Terrible Omen)"},
			{@"music062", "Conde Petie"},
			{@"music063", "Gargant Roo"},
			{@"music064", "Cleyra's Trunk"},
			{@"music066", "Kuja's Theme"},
			{@"music067", "Kuja's Theme Millennium (Desert Palace)"},
			{@"music068", "Immoral Melody (Kuja's Theme Pt. II)"},
			{@"music069", "Footsteps of Desire (Disc 3, fall into Kuja's Trap)"},
			{@"music070", "Ambush Attack (Attack at the Iifa Tree, Cleyra, rescuing Dagger, Disc 2)"},
			{@"music071", "Conde Petie Marriage Ceremony"},
			{@"music072", "Ukulele de chocobo (Chocobo's Theme)"},
			{@"music073", "The Four medallions (After Ipsen's Castle, talk about four mirrors) Disc 3"},
			{@"music075", "Ipsen's Heritage (Ipsen's Castle)"},
			{@"music077", "A transient Past (Oeilvert- Area with the faces, speaking of Terra)"},
			{@"music078", "Emiko's Vocals (Ending Theme)"},
			{@"music079", "South Border Crossing (South Gate)"},
			{@"music080", "Iifa Tree"},
			{@"music081", "Mount Gulug"},
			{@"music082", "Hunter's Chance (Festival of the hunt)"},
			{@"music083", "Hilda Garde 3 Airship Theme"},
			{@"music084", "Emiko's Vocals (Disc 2 Madian Sari)"},
			{@"music085", "Emiko's Vocals (Solo)"},
			{@"music087", "Crystal World"},
			{@"music088", "The chosen summoner (Dagger's guide to Alexandria's Altar) Eidolon Alexander"},
			{@"music089", "Protecting my devotion (Disc 3, Steiner & Beatrix montage fight)"},
			{@"music090", "Loss of me (Beatrix Theme)"},
			{@"music091", "Mystery sword (Battle with Beatrix)"},
			{@"music092", "Pandemonium Theme (Before 'You're not alone' montage fights)"},
			{@"music093", "Secret Library of Daguerreo"},
			{@"music094", "Madian Sari Theme"},
			{@"music095", "Terra"},
			{@"music096", "Place of memories (Memoria)"},
			{@"music097", "Cid's Theme (Lindblum Castle)"},
			{@"music098", "Dark Messenger (Trance Kuja battle)"},
			{@"music101", "The Final battle"},
			{@"music102", "Emiko's Vocals (Solo) Madian Sari, Eidolon Wall appears on fire."},
			{@"music105", "Eiko's Theme pt. II"},
			{@"music106", "We are theives! (Beginning of Disc 3)"},
			{@"music108", "Extraction"},
			{@"music109", "Black Waltz Theme"},
			{@"music110", "Ending Theme Pt. I (Prima Vista Theatre Stage)"},
			{@"music111", "Ending Theme Pt. II (Prima Vista Theatre Stage)"},
			{@"music112", "Ending Theme Pt. III (Prima Vista Theatre Stage)"},
			{@"music113", "Esto Gaza"},
			{@"music114", "Heart of Melting Magic (Cid & Hilda Garde theme)"},
			{@"music115", "Ending Theme Pt. II (Prima Vista Theatre Stage)"},
			{@"music116", "Slew of love letters"},
			{@"music117", "The Evil Mist's Rebirth (Disc 4 world map)"},
			{@"music118", "Successive battles"},
			{@"music120", "Final Fantasy IX - Prelude"},
			{@"music121", "Final Fantasy III Theme"},
			{@"music122", "Assault of the white dragons};"}
		};

		private static readonly Dictionary<String, String> SoundDisplayNames = new Dictionary<string, string>
		{
			{@"se000001", "Menu Select"},
			{@"se000002", "Menu Error"},
			{@"se000003", "Menu Cancel"},
			{@"se000004", "Recieve Items"},
			{@"se000005", "Equip Armor"},
			{@"se000006", "Menu Item Heal"},
			{@"se000007", "EXP recieveing (looped)"},
			{@"se000008", "Gil recieveing (looped)"},
			{@"se000009", "Random Encounter (Part 1)"},
			{@"se000010", "Random Encounter (Part 2)"},
			{@"se000011", "Unknown"},
			{@"se000012", "Open a door"},
			{@"se000013", "Treasures"},
			{@"se000014", "Treasure chest open (Begin)"},
			{@"se000015", "Moogle (Help)"},
			{@"se000016", "Level up"},
			{@"se000017", "Ability Learned"},
			{@"se000018", "Battle command window"},
			{@"se000019", "Purchase"},
			{@"se000020", "Info"},
			{@"se000021", "Menu (L1 and R1) switch between players"},
			{@"se000022", "Tent (Part 1)"},
			{@"se000023", "Tent (Part 2) Random interval, timing based on part 3"},
			{@"se000024", "Tent (Part 3) Random interval, timing based on part 2"},
			{@"se000025", "Tent (Part 4) Random interval, timing based on part 1"},
			{@"se000026", "Save and Load game confirmed"},
			{@"se000027", "Jump"},
			{@"se000028", "Unknown"},
			{@"se000029", "ATE"},
			{@"se000030", "Ladder climb"},
			{@"se000031", "Knight footland (from jump)"},
			{@"se000032", "Moogle welcome"},
			{@"se000033", "Moogle tent use"},
			{@"se000034", "Knight footsteps"},
			{@"se000035", "Jump (Part 2)"},
			{@"se000036", "Land (from jump)"},
			{@"se000037", "Moogle flip-land (from save)"},
			{@"se000038", "Memoria save portal"},
			{@"se000040", "Return from Memoria save portal"},
			{@"se000042", "Moogle save book open"},
			{@"se000043", "Locked Game renew"},
			{@"se000047", "Unknown"},
			{@"se000049", "Moogle Land"},

			{@"se010001", "Enemy (possible Lamia) attack #1"},
			{@"se010002", "Enemy attack #2"},
			{@"se010003", "Enemy attack #3"},
			{@"se010005", "Enemy attack #4"},
			{@"se010006", "Enemy attack #5 Oink"},
			{@"se010007", "Enemy attack #6"},
			{@"se010009", "Enemy attack #7"},
			{@"se010010", "Enemy attack #8"},
			{@"se010011", "Enemy attack #9"},
			{@"se010013", "Enemy attack #10"},
			{@"se010014", "Enemy attack #11"},
			{@"se010015", "Enemy attack #12"},
			{@"se010017", "Weapon (Rod) attack"},
			{@"se010018", "Weapon (Staff and Flute) attack"},
			{@"se010019", "Weapon Attack"},
			{@"se010021", "Weapon (Rod) attack 2"},
			{@"se010022", "Weapon (Staff) attack 2"},
			{@"se010025", "Enemy attack #13"},
			{@"se010029", "Enemy attack #14"},
			{@"se010032", "Weapon Swing (Miss)"},
			{@"se010033", "Weapon Swing (Miss) 2"},
			{@"se010034", "Weapon Swing (Miss) 3"},
			{@"se010035", "Weapon Swing (Miss) 4"},
			{@"se010036", "Weapon Swing (Miss) 5"},
			{@"se010037", "Weapon Swing (Miss) 6"},
			{@"se010038", "Weapon Swing (Miss) 7"},
			{@"se010039", "Air racket swing"},
			{@"se010040", "Jump (Spear)"},

			{@"se010128", "Enemy attack #15"}, // "Battle Sound Knight Sword Slash" Thunder Slash;Stock Break;Slash;Attack;Climhazzard;Hack;Sword Quiver;Cleave;Helm Divide;Judgment Sword;Battlemes1;Battlemes2;Battlemes3;Rrrragh!;Gwahaha!;MESAttack0;MESAttack1;Get some!23;Taste steel!
            {@"se010132", "Enemy attack #16"}, // "Battle Sound Frontal Knock" Tail;HP Switching;StrikeBC;Devil's Kiss;Dive;Counter;Open and Close
            {@"se010136", "Enemy strike"}, // "Battle Sound Claw & Sting" Strike;Counter;Poison Claw;Claws;Scratch;Silent Claw;Claw;Dive;Attack;Battlemes1;Battlemes2;Battlemes3;Demon's Claw
            {@"se010138", "Enemy attack #17"},
			{@"se010140", "Enemy attack #18"}, // "Battle Sound Metallic Slice" Trouble Knife;Knife;Rusty Knife;Claws;Slash;Clamp Pinch;Attack;Battlemes1;Battlemes2;Battlemes3;Chop
            {@"se010144", "Enemy attack #19"}, // "Battle Sound Spear Hit" Counter;Spear;Impale
            {@"se010148", "Enemy attack #20"}, // "Battle Sound Bite & Scratch" Bite;Fang
            {@"se010152", "Enemy attack #21"}, // "Battle Sound Steal" Steal;Mug;Hit
            {@"se010156", "Enemy attack #22"}, // "Battle Sound Rusted Slice" Charge;Claws;Hit;Slice;Blade;Crush;Chop
            {@"se010160", "Enemy attack #23"}, // "Battle Sound Stab & Suck" Tongue;Absorb even more;Stab;Absorb more even more;Rapid Fire;Poison Counter;Stinger
            {@"se010164", "Enemy attack #24"}, // "Battle Sound Heavy Slice" Axe;Trouble Knife;Attack;MEScounter;Dagger's first hit;MEShit1;MEShit2;Hatchet;Mask Jump
            {@"se010168", "Enemy attack #25"}, // "Battle Sound Tongue Knock" Tongue;Stomach
            {@"se010172", "Enemy attack #26"}, // "Battle Sound Wing Uppercut" Wings
            {@"se010176", "Enemy attack #27"}, // "Battle Sound Slam" Counter;Stretch;Antenna;Edge;Strike;Attack;Mug;Hit;Battlemes1;Battlemes2;Silent Kiss;Battlemes3;The Drop
            {@"se010180", "Enemy attack #28"}, // "Battle Sound Charge & Fist" Head Attack;Body Ram;Knock Down;Smash;Ram;Teleport;YEOWWW!;Fist;Open and Close;Battlemes3;Freaked out;Battlemes2;Oww!;Battlemes1;Attack
            {@"se010182", "Enemy attack #29"}, // "Battle Sound Baku Crash" ARGHHH!;Oww!;YEOWWW!
            {@"se010184", "Enemy attack #30"}, // "Battle Sound Beak" Beak
            {@"se010188", "Unknown - Botched"}, // "Battle Sound Rush" Rush;Charge;Fat Press;Crash;ARGHHH!;Hiphop;Ram
            {@"se010192", "Enemy attack - Oink 2"}, // "Battle Sound Spike Hit" Battlemes1;Battlemes2;Battlemes3;Knife;Sting;Spear;Attack;Rolling Attack
            {@"se010196", "Enemy attack - Oink 3"}, // "Battle Sound Heave" Heave;Charge;End3;Counter
            {@"se010197", "Enemy Heave"},
			{@"se010200", "Unknown - Botched"}, // "Battle Sound Power Up" Power Thorn;Power Zorn
            {@"se010204", "Enemy head attack"}, // "Battle Sound Horn Gore" Head Attack;Charge;Horn;Stab
            {@"se010208", "Enemy Wing attack"}, // "Battle Sound Slap" Silent Slap;Trouble Tail;Tail;Hiphop;Strike;Fin;Tentacle;Wings;Wing
            {@"se010212", "Enemy Slap attack"}, // "Battle Sound Whip" Thorn Whip;Slap;Left Stem;Right Stem;Right Tentacle;Left Tentacle;Trouble Counter;Spin;Leg
            {@"se010216", "Unknown - Botched"}, // "Battle Sound Soft Tail" Virus Tentacles;Blind Tail
            {@"se010220", "Unknown - Botched"}, // "Battle Sound Nymph Happy" Happy
            {@"se010224", "Unknown - Botched (Pumpkin Head 1)"},
			{@"se010225", "Unknown - Botched"}, // "Battle Sound Mimic Call" Call
            {@"se010226", "Unknown - Botched (Aqua Breath 1)"}, // "Battle Sound Teleport" Teleport
            {@"se010228", "Enemy Charge"}, // "Battle Sound Head Attack" Head Attack
            {@"se010229", "Enemy Saw"}, // "Battle Sound Saw" Saw
            {@"se010232", "Enemy Slice"}, // "Battle Sound Lich Cutter" Death Cutter;Double Slash
            {@"se010256", "Enemy Fade away (Die)"},
			{@"se010257", "Taharka Ipsen cutscene death (downsampled)"},
			{@"se010258", "Unknown - Botched (Flee) (Part 1)"},
			{@"se010259", "Unknown - Botched (Flee) (Part 2)"},
			{@"se010260", "Unknown - Botched (Flee) (Part 3)"},

			{@"se030005", "Enemy attack #31"}, // "Battle Sound Bomb Grow" Grow
            {@"se030022", "Enemy attack #32"}, // "Battle Sound Vice Appearance" Appearance
            {@"se030048", "Enemy attack #33"}, // "Battle Sound Groan" Groan
            {@"se030109", "Enemy attack #34"}, // "Battle Sound Prison Cage Escape" Event of death
            {@"se030284", "Enemy attack #35"}, // "Battle Sound Sand Golem Death" Golem Death
            {@"se030303", "Enemy attack #36"}, // "Battle Sound Armodullahan Death" Death
            {@"se030309", "Enemy attack #37"}, // "Battle Sound Prison Cage Death" Death
            {@"se030314", "Enemy attack #38"}, // "Battle Sound Gizamaluke Death" Death
            {@"se030318", "Enemy attack #39"}, // "Battle Sound Antlion Death" Death
            {@"se030336", "Enemy attack #40"}, // "Battle Sound Silver Dragon Death" Death
            {@"se030338", "Enemy attack #41"}, // "Battle Sound Nova Dragon Death" Death
            {@"se030346", "Enemy attack #42"}, // "Battle Sound Hades Death" Death by idle2;Death
            {@"se030347", "Enemy attack #43"}, // "Battle Sound Deathguise Death" Death1;Death0

            {@"se500432", "Enemy attack #44"}, // "Battle Sound Flee 1" Escape;Flee;HappyCALC;RefuseEVT
            {@"se500433", "Enemy attack #45"}, // "Battle Sound Flee 2" Escape;Refuse;Happy;FleeEVTCALC
            {@"se500434", "Enemy attack #46"}, // "Battle Sound Flee 3" Escape;Refuse;Happy;FleeEVTCALC
        };
	}
}

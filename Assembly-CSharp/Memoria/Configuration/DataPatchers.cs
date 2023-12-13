using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Assets.Sources.Scripts.UI.Common;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Assets;
using Memoria.Prime.Text;
using static Memoria.Assets.DataResources;

namespace Memoria
{
	// Patchable fields: add this attribute to class fields to flag them as patchable and allow them to be textually modified
	// A patchable field must satisfy the following conditions:
	// 1) It belongs to a class that is handled by the patching code (see the method "PatchBattles")
	// 2) It has a type T that is either a String or is convertible from a string using a static method "T.TryParse(string, out T)" or it is an enum type
	[AttributeUsage(AttributeTargets.Field)]
	public class PatchableFieldAttribute : Attribute
	{
	}

	static class DataPatchers
	{
		public const String MemoriaDictionaryPatcherPath = "DictionaryPatch.txt";
		public const String MemoriaBattlePatcherPath = "BattlePatch.txt";

		public static void Initialize()
		{
			// Apply patches; the default folder (out of any mod folder) is ignored
			try
			{
				foreach (AssetManager.AssetFolder folder in AssetManager.FolderLowToHigh)
				{
					if (String.IsNullOrEmpty(folder.FolderPath))
						continue;
					if (folder.TryFindAssetInModOnDisc(DataPatchers.MemoriaDictionaryPatcherPath, out String dictionaryPath))
					{
						String[] patch = File.ReadAllLines(dictionaryPath);
						DataPatchers.PatchDictionaries(patch);
					}
					if (folder.TryFindAssetInModOnDisc(DataPatchers.MemoriaBattlePatcherPath, out String battlePath))
					{
						String[] patch = File.ReadAllLines(battlePath);
						DataPatchers.PatchBattles(patch);
					}
				}
			}
			catch (Exception err)
			{
				Log.Error(err);
			}
		}

		public static void ApplyBattlePatch(BTL_SCENE scene)
		{
			try
			{
				// Use the English (US) strings for selection of enemies/attacks by name and remove the STRT tag
				EmbadedTextResources.CurrentSymbol = "US";
				String[] battleText = FF9TextTool.GetBattleText(FF9BattleDB.SceneData[scene.nameIdentifier]);
				EmbadedTextResources.CurrentSymbol = null;
				for (Int32 i = 0; i < battleText.Length; i++)
					if (battleText[i].StartsWith("[STRT="))
						battleText[i] = battleText[i].Substring(battleText[i].IndexOf(']') + 1);
				// Change the field values to the ones pre-parsed by "PatchBattles"
				foreach (BattlePatch patch in _battlePatch)
				{
					if (!patch.IsSceneApplicable(scene, battleText))
						continue;
					if (patch.TokenType == BattlePatch.BattleTokenType.Scene)
					{
						foreach (KeyValuePair<FieldInfo, object> field in patch.CustomValue)
						{
							if (field.Key.DeclaringType == typeof(BTL_SCENE_INFO))
								field.Key.SetValue(scene.Info, field.Value);
							else if (field.Key.DeclaringType == typeof(SB2_HEAD))
								field.Key.SetValue(scene.header, field.Value);
						}
					}
					else if (patch.TokenType == BattlePatch.BattleTokenType.Pattern)
					{
						for (Int32 index = 0; index < scene.header.PatCount; index++)
						{
							if (!patch.IsTokenApplicable(scene, battleText, index))
								continue;
							foreach (KeyValuePair<FieldInfo, object> field in patch.CustomValue)
							{
								if (field.Key.DeclaringType == typeof(SB2_PATTERN))
									field.Key.SetValue(scene.PatAddr[index], field.Value);
							}
						}
					}
					else if (patch.TokenType == BattlePatch.BattleTokenType.Enemy)
					{
						for (Int32 index = 0; index < scene.header.TypCount; index++)
						{
							if (!patch.IsTokenApplicable(scene, battleText, index))
								continue;
							foreach (KeyValuePair<FieldInfo, object> field in patch.CustomValue)
							{
								if (field.Key.DeclaringType == typeof(SB2_MON_PARM))
									field.Key.SetValue(scene.MonAddr[index], field.Value);
								else if (field.Key.DeclaringType == typeof(SB2_ELEMENT))
									field.Key.SetValue(scene.MonAddr[index].Element, field.Value);
							}
						}
					}
					else if (patch.TokenType == BattlePatch.BattleTokenType.Attack)
					{
						for (Int32 index = 0; index < scene.header.AtkCount; index++)
						{
							if (!patch.IsTokenApplicable(scene, battleText, index))
								continue;
							foreach (KeyValuePair<FieldInfo, object> field in patch.CustomValue)
							{
								if (field.Key.DeclaringType == typeof(AA_DATA))
									field.Key.SetValue(scene.atk[index], field.Value);
								else if (field.Key.DeclaringType == typeof(BTL_REF))
									field.Key.SetValue(scene.atk[index].Ref, field.Value);
								else if (field.Key.DeclaringType == typeof(BattleCommandInfo))
									field.Key.SetValue(scene.atk[index].Info, field.Value);
							}
						}
					}
				}
			}
			catch (Exception err)
			{
				Log.Error(err);
			}
		}

		private static void PatchDictionaries(String[] patchCode)
		{
			Boolean shouldUpdateBattleStatus = false;
			// Process each line one by one
			foreach (String s in patchCode)
			{
				String[] entry = s.Split(' ');
				if (entry.Length < 3)
					continue;
				if (String.Compare(entry[0], "MessageFile") == 0)
				{
					// eg.: MessageFile 2000 MES_CUSTOM_PLACE
					if (FF9DBAll.MesDB == null)
						continue;
					Int32 ID;
					if (!Int32.TryParse(entry[1], out ID))
						continue;
					FF9DBAll.MesDB[ID] = entry[2];
				}
				else if (String.Compare(entry[0], "IconSprite") == 0)
				{
					// eg.: IconSprite 19 arrow_down
					if (FF9UIDataTool.IconSpriteName == null)
						continue;
					Int32 ID;
					if (!Int32.TryParse(entry[1], out ID))
						continue;
					FF9UIDataTool.IconSpriteName[ID] = entry[2];
				}
				else if (String.Compare(entry[0], "DebuffIcon") == 0)
				{
					// eg.: DebuffIcon 0 188
					// or : DebuffIcon 0 ability_stone
					if (BattleHUD.DebuffIconNames == null)
						continue;
					Int32 ID, iconID;
					if (!Int32.TryParse(entry[1], out ID))
						continue;
					if (Int32.TryParse(entry[2], out iconID))
					{
						if (!FF9UIDataTool.IconSpriteName.ContainsKey(iconID))
						{
							Log.Message("[AssetManager.PatchDictionaries] Trying to use the invalid sprite index " + iconID + " for the icon of status " + ID);
							continue;
						}
						BattleHUD.DebuffIconNames[(BattleStatus)(1 << ID)] = FF9UIDataTool.IconSpriteName[iconID];
						if (BattleResultUI.BadIconDict == null || FF9UIDataTool.status_id == null)
							continue;
						BattleResultUI.BadIconDict[(BattleStatus)(1 << ID)] = (Byte)iconID;
						if (ID < FF9UIDataTool.status_id.Length)
							FF9UIDataTool.status_id[ID] = iconID;
						// Todo: debuff icons in the main menus (status menu, items...) are UISprite components of CharacterDetailHUD and are enabled/disabled in FF9UIDataTool.DisplayCharacterDetail
						// Maybe add UISprite components at runtime? The width of the window may require adjustments then
						// By design (in FF9UIDataTool.DisplayCharacterDetail for instance), permanent debuffs must be the first ones of the list of statuses
					}
					else
					{
						BattleHUD.DebuffIconNames[(BattleStatus)(1 << ID)] = entry[2]; // When adding a debuff icon by sprite name, not all the dictionaries are updated
					}
				}
				else if (String.Compare(entry[0], "BuffIcon") == 0)
				{
					// eg.: BuffIcon 18 188
					// or : BuffIcon 18 ability_stone
					if (BattleHUD.BuffIconNames == null)
						continue;
					Int32 ID, iconID;
					if (!Int32.TryParse(entry[1], out ID))
						continue;
					if (Int32.TryParse(entry[2], out iconID))
					{
						if (!FF9UIDataTool.IconSpriteName.ContainsKey(iconID))
						{
							Log.Message("[AssetManager.PatchDictionaries] Trying to use the invalid sprite index " + iconID + " for the icon of status " + ID);
							continue;
						}
						BattleHUD.BuffIconNames[(BattleStatus)(1 << ID)] = FF9UIDataTool.IconSpriteName[iconID];
					}
					else
					{
						BattleHUD.BuffIconNames[(BattleStatus)(1 << ID)] = entry[2];
					}
				}
				else if (String.Compare(entry[0], "BoostedAbilityColor") == 0 && entry.Length >= 5)
				{
					// eg.: BoostedAbilityColor 3 0.2 1.0 0.8
					if (AbilityUI.BoostedAbilityColor == null)
						continue;
					Int32 index;
					Single r, g, b;
					if (!Int32.TryParse(entry[1], out index))
						continue;
					if (!Single.TryParse(entry[2], out r) || !Single.TryParse(entry[3], out g) || !Single.TryParse(entry[4], out b))
						continue;
					if (index >= AbilityUI.BoostedAbilityColor.Length)
					{
						Int32 oldLength = AbilityUI.BoostedAbilityColor.Length;
						Array.Resize(ref AbilityUI.BoostedAbilityColor, index + 1);
						for (Int32 i = oldLength; i < index; i++)
							AbilityUI.BoostedAbilityColor[i] = new Color(1f, 1f, 1f);
					}
					AbilityUI.BoostedAbilityColor[index] = new Color(r, g, b);
				}
				else if (String.Compare(entry[0], "BattleStatus") == 0)
				{
					// eg.: BattleStatus IdleDying Remove Poison Venom
					// eg.: BattleStatus BattleEnd Add Zombie
					if (!entry[1].TryStaticFieldParse(typeof(BattleStatusConst), out FieldInfo field))
						continue;
					BattleStatus fieldStatus = 0;
					Boolean add = String.Compare(entry[2], "Remove") != 0;
					if (String.Compare(entry[2], "Set") != 0)
						fieldStatus = (BattleStatus)field.GetValue(null);
					for (Int32 i = 3; i < entry.Length; i++)
					{
						if (entry[i].TryEnumParse(out BattleStatus status))
						{
							if (add)
								fieldStatus |= status;
							else
								fieldStatus &= ~status;
						}
					}
					field.SetValue(null, fieldStatus);
					shouldUpdateBattleStatus = true;
				}
				else if (String.Compare(entry[0], "HalfTranceCommand") == 0)
				{
					// eg.: HalfTranceCommand Set DoubleWhiteMagic DoubleBlackMagic HolySword2
					if (btl_cmd.half_trance_cmd_list == null)
						continue;
					Boolean add = String.Compare(entry[1], "Remove") != 0;
					if (String.Compare(entry[1], "Set") == 0)
						btl_cmd.half_trance_cmd_list.Clear();
					for (Int32 i = 2; i < entry.Length; i++)
					{
						if (entry[i].TryEnumParse(out BattleCommandId cmdId))
						{
							if (add && !btl_cmd.half_trance_cmd_list.Contains(cmdId))
								btl_cmd.half_trance_cmd_list.Add(cmdId);
							else if (!add)
								btl_cmd.half_trance_cmd_list.Remove(cmdId);
						}
					}
				}
				else if (String.Compare(entry[0], "DoubleCastCommand") == 0)
				{
					// eg.: DoubleCastCommand Add RedMagic1
					Boolean add = String.Compare(entry[1], "Remove") != 0;
					if (String.Compare(entry[1], "Set") == 0)
						BattleHUD.DoubleCastSet.Clear();
					for (Int32 i = 2; i < entry.Length; i++)
					{
						if (entry[i].TryEnumParse(out BattleCommandId cmdId))
						{
							if (add)
								BattleHUD.DoubleCastSet.Add(cmdId);
							else
								BattleHUD.DoubleCastSet.Remove(cmdId);
						}
					}
				}
				else if (String.Compare(entry[0], "WorldMusicList") == 0 && entry.Length >= 7)
				{
					// eg.: WorldMusicList 69 22 112 45 95 96 61 62
					if (ff9.w_musicSet == null)
						continue;
					Int32 arraySize = entry.Length - 1;
					if (arraySize > ff9.w_musicSet.Length)
						ff9.w_musicSet = new Byte[arraySize];
					for (Int32 i = 0; i < arraySize; i++)
						Byte.TryParse(entry[1 + i], out ff9.w_musicSet[i]);
				}
				else if (String.Compare(entry[0], "TetraMasterSound") == 0)
				{
					// eg.: TetraMasterSound CANCEL 101
					if (SoundEffect.soundIdMap == null)
						continue;
					QuadMistSoundID effectID;
					Int32 soundID;
					if (!entry[1].StartsWith("MINI_SE_"))
						entry[1] = "MINI_SE_" + entry[1];
					if (!entry[1].TryEnumParse(out effectID))
						continue;
					if (!Int32.TryParse(entry[2], out soundID))
						continue;
					SoundEffect.soundIdMap[effectID] = soundID;
				}
				else if (String.Compare(entry[0], "MoogleFieldList") == 0)
				{
					// eg.: MoogleFieldList Remove 2905 2909 2916 2919
					Boolean add = String.Compare(entry[1], "Remove") != 0;
					if (String.Compare(entry[1], "Set") == 0)
					{
						EventEngine.moogleFldMap.Clear();
						EventEngine.moogleFldSpecialMap.Clear();
					}
					Int16 fldid;
					for (Int32 i = 2; i < entry.Length; i++)
					{
						if (Int16.TryParse(entry[i], out fldid))
						{
							EventEngine.moogleFldSpecialMap.Remove(fldid);
							if (add)
								EventEngine.moogleFldMap.Add(fldid);
							else
								EventEngine.moogleFldMap.Remove(fldid);
						}
					}
				}
				else if (String.Compare(entry[0], "BattleMapModel") == 0)
				{
					// eg.: BattleMapModel BSC_CUSTOM_FIELD BBG_B065
					// Can also be modified using "BattleScene"
					if (FF9BattleDB.MapModel == null)
						continue;
					FF9BattleDB.MapModel[entry[1]] = entry[2];
				}
				else if (String.Compare(entry[0], "FieldScene") == 0 && entry.Length >= 6)
				{
					// eg.: FieldScene 4000 57 CUSTOM_FIELD CUSTOM_FIELD 2000
					if (FF9DBAll.EventDB == null || EventEngineUtils.eventIDToFBGID == null || EventEngineUtils.eventIDToMESID == null)
						continue;
					Int32 ID, mesID, areaID;
					if (!Int32.TryParse(entry[1], out ID))
						continue;
					if (!Int32.TryParse(entry[2], out areaID))
						continue;
					if (!Int32.TryParse(entry[5], out mesID))
						continue;
					if (!FF9DBAll.MesDB.ContainsKey(mesID))
					{
						Log.Message("[AssetManager.PatchDictionaries] Trying to use the invalid message file ID " + mesID + " for the field map field scene " + entry[3] + " (" + ID + ")");
						continue;
					}
					String fieldMapName = "FBG_N" + areaID + "_" + entry[3];
					EventEngineUtils.eventIDToFBGID[ID] = fieldMapName;
					FF9DBAll.EventDB[ID] = "EVT_" + entry[4];
					EventEngineUtils.eventIDToMESID[ID] = mesID;
					// p0data1X:
					//  Assets/Resources/FieldMaps/{fieldMapName}/atlas.png
					//  Assets/Resources/FieldMaps/{fieldMapName}/{fieldMapName}.bgi.bytes
					//  Assets/Resources/FieldMaps/{fieldMapName}/{fieldMapName}.bgs.bytes
					//  [Optional] Assets/Resources/FieldMaps/{fieldMapName}/spt.tcb.bytes
					//  [Optional for each sps] Assets/Resources/FieldMaps/{fieldMapName}/{spsID}.sps.bytes
					// p0data7:
					//  Assets/Resources/CommonAsset/EventEngine/EventBinary/Field/LANG/EVT_{entry[4]}.eb.bytes
					//  [Optional] Assets/Resources/CommonAsset/EventEngine/EventAnimation/EVT_{entry[4]}.txt.bytes
					//  [Optional] Assets/Resources/CommonAsset/MapConfigData/EVT_{entry[4]}.bytes
					//  [Optional] Assets/Resources/CommonAsset/VibrationData/EVT_{entry[4]}.bytes
				}
				else if (String.Compare(entry[0], "BattleScene") == 0 && entry.Length >= 4)
				{
					// eg.: BattleScene 5000 CUSTOM_BATTLE BBG_B065
					if (FF9DBAll.EventDB == null || FF9BattleDB.SceneData == null || FF9BattleDB.MapModel == null)
						continue;
					Int32 ID;
					if (!Int32.TryParse(entry[1], out ID))
						continue;
					FF9DBAll.EventDB[ID] = "EVT_BATTLE_" + entry[2];
					FF9BattleDB.SceneData["BSC_" + entry[2]] = ID;
					FF9BattleDB.MapModel["BSC_" + entry[2]] = entry[3];
					// p0data2:
					//  Assets/Resources/BattleMap/BattleScene/EVT_BATTLE_{entry[2]}/{ID}.raw17.bytes
					//  Assets/Resources/BattleMap/BattleScene/EVT_BATTLE_{entry[2]}/dbfile0000.raw16.bytes
					// p0data7:
					//  Assets/Resources/CommonAsset/EventEngine/EventBinary/Battle/{Lang}/EVT_BATTLE_{entry[2]}.eb.bytes
					// resources:
					//  EmbeddedAsset/Text/{Lang}/Battle/{ID}.mes
				}
				else if (String.Compare(entry[0], "CharacterDefaultName") == 0 && entry.Length >= 4)
				{
					// eg.: CharacterDefaultName 0 US Zinedine
					// REMARK: Character default names can also be changed with the option "[Import] Text = 1" although it would monopolise the whole machinery of text importing
					// "[Import] Text = 1" has the priority over DictionaryPatch
					if (CharacterNamesFormatter._characterNames == null)
						continue;
					Int32 ID;
					if (!Int32.TryParse(entry[1], out ID))
						continue;
					Dictionary<CharacterId, String> nameDict;
					if (!CharacterNamesFormatter._characterNames.TryGetValue(entry[2], out nameDict))
						nameDict = new Dictionary<CharacterId, String>();
					nameDict[(CharacterId)ID] = String.Join(" ", entry, 3, entry.Length - 3);
					CharacterNamesFormatter._characterNames[entry[2]] = nameDict;
					if (Localization.GetSymbol() == entry[2] && FF9StateSystem.Common?.FF9?.player != null && FF9StateSystem.Common.FF9.player.ContainsKey((CharacterId)ID))
					{
						FF9StateSystem.Common.FF9.GetPlayer((CharacterId)ID).Name = nameDict[(CharacterId)ID];
						FF9TextTool.ChangeCharacterName((CharacterId)ID, nameDict[(CharacterId)ID]);
					}
				}
				else if (String.Compare(entry[0], "3DModel") == 0)
				{
					// For both field models and enemy battle models (+ animations)
					// eg.:
					// 3DModel 98 GEO_NPC_F0_RMF
					// 3DModelAnimation 200 ANH_NPC_F0_RMF_IDLE
					// 3DModelAnimation 25 ANH_NPC_F0_RMF_WALK
					// 3DModelAnimation 38 ANH_NPC_F0_RMF_RUN
					// 3DModelAnimation 40 ANH_NPC_F0_RMF_TURN_L
					// 3DModelAnimation 41 ANH_NPC_F0_RMF_TURN_R
					// 3DModelAnimation 54 55 56 57 59 ANH_NPC_F0_RMF_ANGRY_INN
					if (FF9BattleDB.GEO == null)
						continue;
					Int32 idcount = entry.Length - 2;
					Int32[] ID = new Int32[entry.Length - 2];
					Boolean formatOK = true;
					for (Int32 idindex = 0; formatOK && idindex < idcount; ++idindex)
						if (!Int32.TryParse(entry[idindex + 1], out ID[idindex]))
							formatOK = false;
					if (!formatOK)
						continue;
					for (Int32 idindex = 0; formatOK && idindex < idcount; ++idindex)
					{
						FF9BattleDB.GEO[ID[idindex]] = entry[entry.Length - 1];
					}
					// TODO: make it work for replacing battle weapon models
					// Currently, a line like "3DModel 476 GEO_ACC_F0_OPB" for replacing the dagger by a book freezes the game on black screen when battle starts
				}
				else if (String.Compare(entry[0], "3DModelAnimation") == 0)
				{
					// eg.: See above
					// When adding custom animations, the name must follow the following pattern:
					//   ANH_[MODEL TYPE]_[MODEL VERSION]_[MODEL 3 LETTER CODE]_[WHATEVER]
					// in such a way that the model's name GEO_[...] and its new animation ANH_[...] have the middle block in common in their name
					// Then that custom animation's file must be placed in that model's animation folder
					// (eg. "assets/resources/animations/98/100000.anim" for a custom animation of Zidane with ID 100000)
					if (FF9DBAll.AnimationDB == null || FF9BattleDB.Animation == null)
						continue;
					Int32 idcount = entry.Length - 2;
					Int32[] ID = new Int32[entry.Length - 2];
					Boolean formatOK = true;
					for (Int32 idindex = 0; formatOK && idindex < idcount; ++idindex)
						if (!Int32.TryParse(entry[idindex + 1], out ID[idindex]))
							formatOK = false;
					if (!formatOK)
						continue;
					for (Int32 idindex = 0; formatOK && idindex < idcount; ++idindex)
					{
						FF9DBAll.AnimationDB[ID[idindex]] = entry[entry.Length - 1];
						FF9BattleDB.Animation[ID[idindex]] = entry[entry.Length - 1];
					}
				}
				else if (String.Compare(entry[0], "SwapFieldModelTexture") == 0)
				{
                    // eg.: SwapFieldModelTexture 2250 GEO_MON_B3_093 CustomTextures/OeilvertGuardian/342_0.png CustomTextures/OeilvertGuardian/342_1.png CustomTextures/OeilvertGuardian/342_2.png CustomTextures/OeilvertGuardian/342_3.png CustomTextures/OeilvertGuardian/342_4.png CustomTextures/OeilvertGuardian/342_5.png
                    List<string> TexturesList = new List<string>();
                    if (!Int32.TryParse(entry[1], out Int32 FieldID))
                        continue;
                    if (entry[1] == null || entry[2] == null)
						continue;
					for (Int32 i = 3; i < entry.Length; i++)
					{
                        TexturesList.Add(entry[i]);
                    }
                    String[] TexturesCustomModel = TexturesList.ToArray();
					ModelFactory.CustomModelField.Add(entry[1]+"#"+entry[2], TexturesCustomModel);
                }
                else if (String.Compare(entry[0], "SPSTexture") == 0)
                {
					// eg.: SPSTexture customfireorb shp 3 400 0 0 5 5
					if (entry[1] == null || entry[2] == null)
						continue;

                    if (!Int32.TryParse(entry[3], out Int32 numbertexture))
                        continue;
                    if (!float.TryParse(entry[4], out float posx))
                        continue;
                    if (!float.TryParse(entry[5], out float posy))
                        continue;
                    if (!float.TryParse(entry[6], out float posz))
                        continue;

                    if (!float.TryParse(entry[7], out float SPSScale))
                        SPSScale = 4;
                    if (!float.TryParse(entry[8], out float SPSDistance))
                        SPSDistance = 5;

                    Vector3 SPSpos = new Vector3(posx, posy, posz);
                    BattleSPSSystem.statusTextures.Add(new BattleSPSSystem.SPSTexture(entry[1], entry[2], numbertexture, SPSpos, SPSScale, SPSDistance));
                }
            }
			if (shouldUpdateBattleStatus)
				BattleStatusConst.Update();
		}

		private static void PatchBattles(String[] patchCode)
		{
			// Parse each line one by one and fill "_battlePatch" with "Field to object" dictionaries
			FieldInfo[][][] battleFields = new FieldInfo[][][]{
				new FieldInfo[][]{ typeof(SB2_HEAD).GetFields(), typeof(BTL_SCENE_INFO).GetFields() },
				new FieldInfo[][]{ typeof(SB2_PATTERN).GetFields() },
				new FieldInfo[][]{ typeof(SB2_MON_PARM).GetFields(), typeof(SB2_ELEMENT).GetFields() },
				new FieldInfo[][]{ typeof(AA_DATA).GetFields(), typeof(BTL_REF).GetFields(), typeof(BattleCommandInfo).GetFields() }
			};
			BattlePatch currentPatch = null;
			_selectedBattleId = -1;
			foreach (String s in patchCode)
			{
				if (s.StartsWith("//"))
					continue;
				String[] entry = s.Trim(new char[] { ' ', '\t', '\r', '\n', '=' }).Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
				if (entry.Length != 2)
					continue;
				String opcode = entry[0].TrimEnd(':');
				String oparg = entry[1];
				if (!TryParseBattleSelector(opcode, oparg, ref currentPatch) && currentPatch != null)
				{
					if (_selectedBattleId >= 0 && String.Compare(opcode, "Music") == 0)
					{
						if (Int32.TryParse(oparg.Trim(), out Int32 musicId))
							FF9SndMetaData.BtlBgmPatcherMapper[_selectedBattleId] = musicId;
					}
					foreach (FieldInfo[] fieldArr in battleFields[(Int32)currentPatch.TokenType])
					{
						foreach (FieldInfo field in fieldArr.Where(h => h.IsDefined(typeof(PatchableFieldAttribute), false)))
						{
							if (String.Compare(opcode, field.Name) == 0)
							{
								object obj;
								if (field.FieldType.IsArray)
								{
									if (oparg.Split(' ').TryArrayParse(field.FieldType.GetElementType(), out obj))
										currentPatch.CustomValue[field] = obj;
								}
								else
								{
									if (oparg.TryTypeParse(field.FieldType, out obj))
										currentPatch.CustomValue[field] = obj;
								}
							}
						}
					}
				}
			}
			List<BattlePatch> emptyPatch = new List<BattlePatch>();
			foreach (BattlePatch bp in _battlePatch)
				if (bp.CustomValue.Count == 0)
					emptyPatch.Add(bp);
			foreach (BattlePatch bp in emptyPatch)
				_battlePatch.Remove(bp);
		}

		private static Boolean TryParseBattleSelector(String opcode, String oparg, ref BattlePatch patch)
		{
			String idstr;
			Int32 idint;
			if (String.Compare(opcode, "Battle") == 0)
			{
				String selectedBattle;
				if (Int32.TryParse(oparg, out idint) && FF9BattleDB.SceneData.TryGetKey(idint, out idstr))
					selectedBattle = idstr;
				else
					selectedBattle = oparg;
				if (!FF9BattleDB.SceneData.TryGetValue(selectedBattle, out _selectedBattleId))
					_selectedBattleId = -1;
				patch = new BattlePatch(BattlePatch.BattleTokenType.Scene,
					(scene, str) => String.Compare(scene.nameIdentifier, selectedBattle) == 0,
					(scene, str, index) => false);
				_battlePatch.Add(patch);
				return true;
			}
			else if (String.Compare(opcode, "AnyEnemyByName") == 0)
			{
				_selectedBattleId = -1;
				patch = new BattlePatch(BattlePatch.BattleTokenType.Enemy,
					(scene, str) => Array.Exists(str, (name) => String.Compare(name, oparg) == 0),
					(scene, str, index) => String.Compare(str[index], oparg) == 0);
				_battlePatch.Add(patch);
				return true;
			}
			else if (String.Compare(opcode, "AnyAttackByName") == 0)
			{
				_selectedBattleId = -1;
				patch = new BattlePatch(BattlePatch.BattleTokenType.Attack,
					(scene, str) => Array.Exists(str, (name) => String.Compare(name, oparg) == 0),
					(scene, str, index) => String.Compare(str[scene.header.TypCount + index], oparg) == 0);
				_battlePatch.Add(patch);
				return true;
			}
			else if (patch != null)
			{
				if (String.Compare(opcode, "Pattern") == 0)
				{
					if (!Int32.TryParse(oparg, out idint))
						return false;
					patch = new BattlePatch(BattlePatch.BattleTokenType.Pattern,
						patch.IsSceneApplicable,
						(scene, str, index) => index == idint);
					_battlePatch.Add(patch);
					return true;
				}
				else if (String.Compare(opcode, "Enemy") == 0)
				{
					if (!Int32.TryParse(oparg, out idint))
						return false;
					patch = new BattlePatch(BattlePatch.BattleTokenType.Enemy,
						patch.IsSceneApplicable,
						(scene, str, index) => index == idint);
					_battlePatch.Add(patch);
					return true;
				}
				else if (String.Compare(opcode, "Attack") == 0)
				{
					if (!Int32.TryParse(oparg, out idint))
						return false;
					patch = new BattlePatch(BattlePatch.BattleTokenType.Attack,
						patch.IsSceneApplicable,
						(scene, str, index) => index == idint);
					_battlePatch.Add(patch);
					return true;
				}
				else if (String.Compare(opcode, "EnemyByName") == 0)
				{
					patch = new BattlePatch(BattlePatch.BattleTokenType.Enemy,
						patch.IsSceneApplicable,
						(scene, str, index) => String.Compare(str[index], oparg) == 0);
					_battlePatch.Add(patch);
					return true;
				}
				else if (String.Compare(opcode, "AttackByName") == 0)
				{
					patch = new BattlePatch(BattlePatch.BattleTokenType.Attack,
						patch.IsSceneApplicable,
						(scene, str, index) => String.Compare(str[scene.header.TypCount + index], oparg) == 0);
					_battlePatch.Add(patch);
					return true;
				}
			}
			return false;
		}

		private class BattlePatch
		{
			public enum BattleTokenType
			{
				Scene,
				Pattern,
				Enemy,
				Attack
			}

			public BattleTokenType TokenType;
			public Func<BTL_SCENE, String[], Boolean> IsSceneApplicable;
			public Func<BTL_SCENE, String[], Int32, Boolean> IsTokenApplicable;

			public Dictionary<FieldInfo, object> CustomValue = new Dictionary<FieldInfo, object>();

			public BattlePatch(BattleTokenType tok, Func<BTL_SCENE, String[], Boolean> sceneCheck, Func<BTL_SCENE, String[], Int32, Boolean> tokCheck)
			{
				TokenType = tok;
				IsSceneApplicable = sceneCheck;
				IsTokenApplicable = tokCheck;
			}
		}

		private static List<BattlePatch> _battlePatch = new List<BattlePatch>();
		private static Int32 _selectedBattleId;
	}
}

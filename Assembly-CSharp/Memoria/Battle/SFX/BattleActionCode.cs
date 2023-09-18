using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Assets.Sources.Scripts.UI.Common;
using UnityEngine;
using FF9;
using Memoria;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Assets;
using NCalc;

public class BattleActionCode
{
	public String operation;
	public Dictionary<String, String> argument;

	public BattleActionCode()
	{
		operation = "";
		argument = new Dictionary<String, String>();
	}

	public BattleActionCode(String op, String key1 = "", String arg1 = "", String key2 = "", String arg2 = "",
		String key3 = "", String arg3 = "", String key4 = "", String arg4 = "",
		String key5 = "", String arg5 = "", String key6 = "", String arg6 = "")
	{
		operation = op;
		if (key6.Length > 0)
			argument = new Dictionary<String, String> { { key1, arg1 }, { key2, arg2 }, { key3, arg3 }, { key4, arg4 }, { key5, arg5 }, { key6, arg6 } };
		else if (key5.Length > 0)
			argument = new Dictionary<String, String> { { key1, arg1 }, { key2, arg2 }, { key3, arg3 }, { key4, arg4 }, { key5, arg5 } };
		else if (key4.Length > 0)
			argument = new Dictionary<String, String> { { key1, arg1 }, { key2, arg2 }, { key3, arg3 }, { key4, arg4 } };
		else if (key3.Length > 0)
			argument = new Dictionary<String, String> { { key1, arg1 }, { key2, arg2 }, { key3, arg3 } };
		else if (key2.Length > 0)
			argument = new Dictionary<String, String> { { key1, arg1 }, { key2, arg2 } };
		else if (key1.Length > 0)
			argument = new Dictionary<String, String> { { key1, arg1 } };
		else
			argument = new Dictionary<String, String>();
	}

	public static Dictionary<String, String[]> operationArguments = new Dictionary<String, String[]>
	{
		{ "Wait", new String[]{ "Time" } },
		{ "WaitAnimation", new String[]{ "Char" } },
		{ "WaitMove", new String[]{ "Char" } },
		{ "WaitTurn", new String[]{ "Char" } },
		{ "WaitSize", new String[]{ "Char" } },
		{ "WaitMonsterSFXLoaded", new String[]{ "SFX", "Instance" } },
		{ "WaitMonsterSFXDone", new String[]{ "SFX", "Instance" } },
		{ "WaitSFXLoaded", new String[]{ "SFX", "Instance" } },
		{ "WaitSFXDone", new String[]{ "SFX", "Instance" } },
		{ "WaitReflect", null },
		{ "Channel", new String[]{ "Type", "Char", "SkipFlute" } },
		{ "StopChannel", new String[]{ "Char" } },
		{ "LoadMonsterSFX", new String[]{ "SFX", "Char", "Target", "TargetPosition", "UseCamera", "FirstBone", "SecondBone", "Args", "MagicCaster" } },
		{ "PlayMonsterSFX", new String[]{ "SFX", "Instance" } },
		{ "LoadSFX", new String[]{ "SFX", "Char", "Target", "TargetPosition", "UseCamera", "FirstBone", "SecondBone", "Args", "MagicCaster" } },
		{ "PlaySFX", new String[]{ "SFX", "Instance", "JumpToFrame", "SkipSequence", "HideMeshes", "MeshColors" } },
		{ "Turn", new String[]{ "Char", "BaseAngle", "Angle", "Time", "UsePitch" } },
		{ "PlayAnimation", new String[]{ "Char", "Anim", "Speed", "Loop", "Palindrome", "Frame" } },
		{ "PlayTextureAnimation", new String[]{ "Char", "Anim", "Once", "Stop" } },
		{ "ToggleStandAnimation", new String[]{ "Char", "Alternate" } },
		{ "MoveToTarget", new String[]{ "Char", "Target", "Offset", "Distance", "Time", "Anim", "MoveHeight", "UseCollisionRadius", "IsRelativeDistance" } },
		{ "MoveToPosition", new String[]{ "Char", "AbsolutePosition", "RelativePosition", "Time", "Anim", "MoveHeight" } },
		{ "ChangeSize", new String[]{ "Char", "Size", "Time", "ScaleShadow", "IsRelative" } },
		{ "ShowMesh", new String[]{ "Char", "Enable", "Mesh", "Time", "IsDisappear", "IsPermanent", "Priority" } },
		{ "ShowShadow", new String[]{ "Char", "Enable" } },
		{ "ChangeCharacterProperty", new String[]{ "Char", "Property", "Value" } },
		{ "PlayCamera", new String[]{ "Camera", "Char", "IsAlternate", "Start" } },
		{ "ResetCamera", null },
		{ "PlaySound", new String[]{ "Sound", "SoundType", "Volume", "Pitch", "Panning", "Start" } },
		{ "StopSound", new String[]{ "Sound", "SoundType" } },
		{ "EffectPoint", new String[]{ "Char", "Type" } },
		{ "Message", new String[]{ "Text", "Title", "Priority" } },
		{ "SetBackgroundIntensity", new String[]{ "Intensity", "Time", "HoldDuration" } },
		{ "SetVariable", new String[]{ "Variable", "Value", "Index" } },
		{ "SetupReflect", new String[]{ "Delay" } },
		{ "ActivateReflect", null },
		{ "StartThread", new String[]{ "Condition", "LoopCount", "Target", "TargetLoop", "Chain", "Sync" } },
		{ "ElseThread", new String[]{ "Condition", "LoopCount", "Target", "TargetLoop", "Chain", "Sync" } },
		{ "MOVE_WATER", new String[]{ "Char", "Type", "Time" } }
	};

	public Boolean TryGetArgSingle(String key, out Single value)
	{
		String args;
		if (argument.TryGetValue(key, out args))
			if (Single.TryParse(args, out value))
				return true;
		value = 0f;
		return false;
	}

	public Boolean TryGetArgInt32(String key, out Int32 value)
	{
		String args;
		if (argument.TryGetValue(key, out args))
			if (Int32.TryParse(args, out value))
				return true;
		value = 0;
		return false;
	}

	public Boolean TryGetArgBoolean(String key, out Boolean value)
	{
		String args;
		if (argument.TryGetValue(key, out args))
		{
			if (Boolean.TryParse(args, out value))
				return true;
			if (String.Equals(args, "true", StringComparison.OrdinalIgnoreCase))
			{
				value = true;
				return true;
			}
			if (String.Equals(args, "false", StringComparison.OrdinalIgnoreCase))
			{
				value = false;
				return true;
			}
		}
		value = false;
		return false;
	}

	public Boolean TryGetArgSound(String key, BTL_DATA caster, out Int32 value)
	{
		String args;
		value = -1;
		if (argument.TryGetValue(key, out args))
		{
			if (Int32.TryParse(args, out value))
				return true;
			if (caster.bi.player == 0)
				return false;
			if (args == "WeaponAttack")
			{
				value = btlsnd.ff9btlsnd_weapon_sfx(caster.bi.line_no, FF9BatteSoundWeaponSndEffectType.FF9BTLSND_WEAPONSNDEFFECTTYPE_ATTACK);
				return value >= 0;
			}
			if (args == "WeaponHit")
			{
				value = btlsnd.ff9btlsnd_weapon_sfx(caster.bi.line_no, FF9BatteSoundWeaponSndEffectType.FF9BTLSND_WEAPONSNDEFFECTTYPE_HIT);
				return value >= 0;
			}
		}
		return false;
	}

	public Boolean TryGetArgSoundType(String key, out SoundProfileType value)
	{
		String args;
		value = SoundProfileType.SoundEffect;
		if (argument.TryGetValue(key, out args))
		{
			try
			{
				value = (SoundProfileType)Enum.Parse(typeof(SoundProfileType), args);
				return true;
			}
			catch (Exception err)
			{
				return false;
			}
		}
		return false;
	}

	public Boolean TryGetArgSFX(String key, BTL_DATA caster, out SpecialEffect value)
	{
		String args;
		value = SpecialEffect.Special_No_Effect;
		if (argument.TryGetValue(key, out args))
		{
			if (args == "Weapon")
			{
				if (caster.weapon == null)
					return false;
				value = btl_vfx.GetPlayerAttackVfx(caster);
				return true;
			}
			Int32 asInt;
			if (Int32.TryParse(args, out asInt))
			{
				value = (SpecialEffect)asInt;
				return true;
			}
			try
			{
				value = (SpecialEffect)Enum.Parse(typeof(SpecialEffect), args);
				return true;
			}
			catch (Exception err)
			{
				return false;
			}
		}
		return false;
	}

	public Boolean TryGetArgSFXInstance(String sfxKey, String instanceKey, BTL_DATA caster, List<SFXData> sfxList, Int32 defaultSFX, out Int32 value)
	{
		value = -1;
		if (sfxList.Count == 0)
			return false;
		String str;
		if (argument.TryGetValue(sfxKey, out str) && str == "All")
			return true;
		if (argument.TryGetValue(instanceKey, out str) && str == "All")
			return true;
		SpecialEffect sfxId;
		Int32 sfxNum;
		Boolean sfxGiven = TryGetArgSFX(sfxKey, caster, out sfxId);
		Boolean numGiven = TryGetArgInt32(instanceKey, out sfxNum);
		if (!sfxGiven && !numGiven)
		{
			value = defaultSFX;
			return value >= 0;
		}
		sfxNum--;
		if (!sfxGiven)
		{
			if (sfxNum >= 0 && sfxNum < sfxList.Count)
			{
				value = sfxNum;
				return true;
			}
			return false;
		}
		else if (!numGiven)
		{
			if (defaultSFX >= 0 && sfxList[defaultSFX].id == sfxId)
			{
				value = defaultSFX;
				return true;
			}
			for (Int32 i = sfxList.Count - 1; i >= 0; i--)
				if (sfxList[i].id == sfxId)
				{
					value = i;
					return true;
				}
			return false;
		}
		for (Int32 i = 0; i < sfxList.Count; i++)
			if (sfxList[i].id == sfxId)
			{
				if (sfxNum <= 0)
				{
					value = i;
					return true;
				}
				sfxNum--;
			}
		return false;
	}

	public Boolean TryGetArgCamera(String key, out Int32 value) // TODO
	{
		String args;
		if (argument.TryGetValue(key, out args))
			if (Int32.TryParse(args, out value))
				return true;
		value = 0;
		return false;
	}

	public Boolean TryGetArgAnimation(String key, BTL_DATA btl, out String value)
	{
		String args;
		value = "";
		if (argument.TryGetValue(key, out args))
		{
			Int32 motId;
			if (args == "Current")
				value = btl.currentAnimationName;
			else if (args == "Idle")
				value = btl.mot[btl.bi.def_idle];
			else if (Int32.TryParse(args, out motId) && motId >= 0 && motId < btl.mot.Length)
				value = btl.mot[motId];
			else
			{
				try
				{
					BattlePlayerCharacter.PlayerMotionIndex motCode;
					motCode = (BattlePlayerCharacter.PlayerMotionIndex)Enum.Parse(typeof(BattlePlayerCharacter.PlayerMotionIndex), args);
					if ((Int32)motCode < btl.mot.Length)
						value = btl.mot[(Int32)motCode];
					else
						return false;
				}
				catch (Exception err)
				{
					value = args;
					try
					{
						if (btl.gameObject.GetComponent<Animation>().GetClip(args) == null)
							AnimationFactory.AddAnimWithAnimatioName(btl.gameObject, args);
					}
					catch (Exception err2)
					{
						Log.Error(err2);
					}
					if (btl.gameObject.GetComponent<Animation>().GetClip(args) == null)
						return false;
				}
			}
			return true;
		}
		return false;
	}

	public Boolean TryGetArgVector(String key, out Vector3 value)
	{
		String args;
		value = new Vector3();
		if (argument.TryGetValue(key, out args))
			return ParseStringToVector(args, out value);
		return false;
	}

	private Boolean ParseStringToVector(String str, out Vector3 value)
	{
		value = new Vector3();
		str = str.Trim();
		if (str.StartsWith("(") && str.EndsWith(")"))
			str = str.Substring(1, str.Length - 2);
		String[] coords = str.Split(',');
		if (coords.Length == 1 && Single.TryParse(coords[0], out value.x))
		{
			value.y = value.z = value.x;
			return true;
		}
		if (coords.Length == 2 && Single.TryParse(coords[0], out value.x) && Single.TryParse(coords[1], out value.z))
			return true;
		if (coords.Length == 3 && Single.TryParse(coords[0], out value.x) && Single.TryParse(coords[1], out value.y) && Single.TryParse(coords[2], out value.z))
			return true;
		return false;
	}

	private Boolean ParseStringToColor(String str, out Color value)
	{
		value = Color.white;
		str = str.Trim();
		if (str.StartsWith("(") && str.EndsWith(")"))
			str = str.Substring(1, str.Length - 2);
		String[] coords = str.Split(',');
		if (coords.Length >= 3)
		{
			if (!Single.TryParse(coords[0], out value.r) || !Single.TryParse(coords[1], out value.g) || !Single.TryParse(coords[2], out value.b))
				return false;
			if (coords.Length >= 4 && !Single.TryParse(coords[3], out value.a))
				return false;
			return true;
		}
		return false;
	}

	public Boolean TryGetArgMeshList(String key, out List<UInt32> keyList, out List<UInt32> indexList)
	{
		String args;
		keyList = new List<UInt32>();
		indexList = new List<UInt32>();
		if (argument.TryGetValue(key, out args))
		{
			UInt32 num;
			if (args.StartsWith("(") && args.EndsWith(")"))
				args = args.Substring(1, args.Length - 2);
			String[] meshIdList = args.Split(',');
			foreach (String meshId in meshIdList)
			{
				String trimmed = meshId.Trim();
				if (trimmed.StartsWith("0x"))
				{
					if (UInt32.TryParse(trimmed.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num))
						keyList.Add(num);
				}
				else if (UInt32.TryParse(trimmed, out num))
					indexList.Add(num);
			}
			return true;
		}
		return false;
	}

	public Boolean TryGetArgMeshColors(String key, out Dictionary<UInt32, Color> meshKeyColors, out Dictionary<UInt32, Color> meshIndexColors)
	{
		String args;
		meshKeyColors = new Dictionary<UInt32, Color>();
		meshIndexColors = new Dictionary<UInt32, Color>();
		if (argument.TryGetValue(key, out args))
		{
			if (args.StartsWith("(") && args.EndsWith(")"))
			{
				args = args.Substring(1, args.Length - 2);
				if (!ParseStringToColor(args, out Color sharedColor))
					return false;
				meshKeyColors[0] = sharedColor;
				return true;
			}
			foreach (Match meshPattern in new Regex(@"([^:]*):\s*\(([^\)]*)\)\s*(,|$)").Matches(args))
			{
				String keyCode = meshPattern.Groups[1].Value.Trim();
				String colorCode = meshPattern.Groups[2].Value.Trim();
				if (keyCode.StartsWith("0x"))
				{
					if (!UInt32.TryParse(keyCode.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out UInt32 meshKey) || !ParseStringToColor(colorCode, out Color meshColor))
						return false;
					meshKeyColors[meshKey] = meshColor;
				}
				else
				{
					if (!UInt32.TryParse(keyCode, out UInt32 index) || !ParseStringToColor(colorCode, out Color meshColor))
						return false;
					meshIndexColors[index] = meshColor;
				}
			}
			return true;
		}
		return false;
	}

	public Boolean TryGetArgMessage(String key, CMD_DATA cmd, out String value)
	{
		String args;
		if (!argument.TryGetValue(key + Localization.GetSymbol(), out args) && !argument.TryGetValue(key, out args))
		{
			value = "";
			return false;
		}
		Int32 codeStart, codeEnd, codeNum, codeLen;
		args = args.Replace("[CastName]", UIManager.Battle.GetBattleCommandTitle(cmd));
		args = args.Replace("[MagicSword]", BattleHUD.FormatMagicSwordAbility(cmd));
		while ((codeStart = args.IndexOf("[CommandTitle=")) >= 0)
		{
			codeLen = "[CommandTitle=".Length;
			if ((codeEnd = args.IndexOf(']', codeStart + codeLen)) < 0)
				args = args.Remove(codeStart);
			else if (Int32.TryParse(args.Substring(codeStart + codeLen, codeEnd - codeStart - codeLen), out codeNum))
				args = args.Substring(0, codeStart) + FF9TextTool.BattleCommandTitleText(codeNum) + args.Substring(codeEnd + 1);
			else
				args = args.Substring(0, codeStart) + args.Substring(codeEnd + 1);
		}
		while ((codeStart = args.IndexOf("[AbilityName=")) >= 0)
		{
			codeLen = "[AbilityName=".Length;
			if ((codeEnd = args.IndexOf(']', codeStart + codeLen)) < 0)
				args = args.Remove(codeStart);
			else if (Int32.TryParse(args.Substring(codeStart + codeLen, codeEnd - codeStart - codeLen), out codeNum))
				args = args.Substring(0, codeStart) + FF9TextTool.ActionAbilityName((BattleAbilityId)codeNum) + args.Substring(codeEnd + 1);
			else
				args = args.Substring(0, codeStart) + args.Substring(codeEnd + 1);
		}
		value = args;
		return true;
	}

	public Boolean TryGetArgCharacter(String key, UInt16 caster, UInt16 target, out UInt16 value)
	{
		String args;
		value = 0;
		if (argument.TryGetValue(key, out args))
		{
			if (args.StartsWith("MatchingCondition(") && args.EndsWith(")"))
			{
				value = 0;
				String conditionStr = args.Substring("MatchingCondition(".Length, args.Length - "MatchingCondition()".Length);
				Expression c = new Expression(conditionStr);
				c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
				c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
				NCalcUtility.InitializeExpressionUnit(ref c, btl_scrp.FindBattleUnit(caster), "Caster");
				foreach (BattleUnit unit in Memoria.BattleState.EnumerateUnits())
				{
					c.Parameters["IsTargeted"] = (unit.Id & target) != 0;
					c.Parameters["IsTheCaster"] = (unit.Id & caster) != 0;
					NCalcUtility.InitializeExpressionUnit(ref c, unit);
					if (NCalcUtility.EvaluateNCalcCondition(c.Evaluate(), false))
						value |= unit.Id;
				}
				return true;
			}
			if (args == "AllTargets")
			{
				value = target;
				return true;
			}
			if (args == "RandomTarget")
			{
				value = (UInt16)Comn.randomID(target);
				return true;
			}
			if (args == "Caster")
			{
				value = caster;
				return true;
			}
			if (args == "AllPlayers")
			{
				value = btl_scrp.GetBattleID(0);
				return true;
			}
			if (args == "AllEnemies")
			{
				value = btl_scrp.GetBattleID(1);
				return true;
			}
			if (args == "Everyone")
			{
				value = btl_scrp.GetBattleID(2);
				return true;
			}
			Dictionary<CharacterId, String> partyNames = CharacterNamesFormatter.CharacterScriptNames();
			foreach (KeyValuePair<CharacterId, String> pair in partyNames)
				if (args == pair.Value)
					for (BTL_DATA btl = FF9StateSystem.Battle.FF9Battle.btl_list.next; btl != null; btl = btl.next)
						if (btl.bi.player != 0 && (CharacterId)btl.bi.slot_no == pair.Key)
						{
							value = btl.btl_id;
							return true;
						}
			Int32 targetIndex;
			if (args == "FirstTarget")
				targetIndex = 0;
			else if (args == "SecondTarget")
				targetIndex = 1;
			else if (args == "ThirdTarget")
				targetIndex = 2;
			else if (args == "FourthTarget")
				targetIndex = 3;
			else if (args == "FifthTarget")
				targetIndex = 4;
			else if (args == "SixthTarget")
				targetIndex = 5;
			else if (args == "SeventhTarget")
				targetIndex = 6;
			else if (args == "EighthTarget")
				targetIndex = 7;
			else if (UInt16.TryParse(args, out value))
				return true;
			else
				return false;
			value = 1;
			while (value != 0)
			{
				while (value != 0 && (value & target) == 0)
					value <<= 1;
				if (--targetIndex < 0)
					break;
				value <<= 1;
			}
			return value != 0;
		}
		return false;
	}

	public Boolean TryGetArgBaseAngle(String key, BTL_DATA btl, UInt16 caster, UInt16 target, Boolean isPitch, out Single angle, out UInt16 targetId)
	{
		String args;
		angle = 0;
		targetId = 0;
		if (argument.TryGetValue(key, out args))
		{
			if (args == "Current")
			{
				angle = isPitch ? btl.rot.eulerAngles.x : btl.rot.eulerAngles.y;
				return true;
			}
			else if (args == "Default")
			{
				angle = isPitch ? btl.evt.rotBattle.eulerAngles.x : btl.evt.rotBattle.eulerAngles.y;
				return true;
			}
			else if (Single.TryParse(args, out angle))
			{
				return true;
			}
			else if (TryGetArgCharacter(key, caster, target, out targetId))
			{
				return true;
			}
		}
		return false;
	}

	public Boolean TryChangeCharacterProperty(String keyType, String keyVal, BTL_DATA btl)
	{
		String property, value;
		if (!argument.TryGetValue(keyType, out property) || !argument.TryGetValue(keyVal, out value))
			return false;
		switch (property)
		{
			case "base_pos":
				Vector3 newPos;
				if (value == "Original")
					btl.base_pos = btl.original_pos;
				else if (value == "Current")
					btl.base_pos = btl.pos;
				else
				{
					Boolean plus = false;
					Boolean minus = false;
					value = value.Trim();
					if (value.StartsWith("+"))
					{
						plus = true;
						value = value.Substring(1);
					}
					else if (value.StartsWith("-"))
					{
						minus = true;
						value = value.Substring(1);
					}
					if (ParseStringToVector(value, out newPos))
					{
						if (plus)
							btl.base_pos += newPos;
						else if (minus)
							btl.base_pos -= newPos;
						else
							btl.base_pos = newPos;
						return true;
					}
					return false;
				}
				return true;
			case "tar_bone":
				btl.tar_bone = (Byte)ChangeVariable(btl.tar_bone, value);
				return true;
		}
		return false;
	}

	public Boolean TrySetVariable(String keyVar, String keyVal, String keyIndex)
	{
		String varName, valStr;
		if (!argument.TryGetValue(keyVar, out varName))
			return false;
		if (!argument.TryGetValue(keyVal, out valStr))
			return false;
		Int32 arr = -1;
		String arrStr;
		if (argument.TryGetValue(keyIndex, out arrStr))
			Int32.TryParse(arrStr, out arr);
		switch (varName)
		{
			case "btl_seq":
				FF9StateSystem.Battle.FF9Battle.btl_seq = (Byte)ChangeVariable(FF9StateSystem.Battle.FF9Battle.btl_seq, valStr);
				return true;
			case "cmd_status":
				FF9StateSystem.Battle.FF9Battle.cmd_status = (Byte)ChangeVariable(FF9StateSystem.Battle.FF9Battle.cmd_status, valStr);
				return true;
			case "gEventGlobal":
				if (arr < 0)
					return false;
				FF9StateSystem.EventState.gEventGlobal[arr] = (Byte)ChangeVariable(FF9StateSystem.EventState.gEventGlobal[arr], valStr);
				return true;
			case "_ZWrite":
				foreach (Material material in battlebg.GetShaders(2))
					material.SetInt("_ZWrite", ChangeVariable(material.GetInt("_ZWrite"), valStr));
				return true;
		}
		return false;
	}

	private Int32 ChangeVariable(Int32 currentValue, String valStr)
	{
		Int32 op = 0;
		valStr = valStr.Trim();
		if (valStr.StartsWith("+"))
		{
			op = 1;
			valStr = valStr.Substring(1);
		}
		else if (valStr.StartsWith("|"))
		{
			op = 3;
			valStr = valStr.Substring(1);
		}
		else if (valStr.StartsWith("&"))
		{
			op = 4;
			valStr = valStr.Substring(1);
		}
		Int32 value;
		if (!Int32.TryParse(valStr, out value))
			return currentValue;
		if (op == 0)
			return value;
		if (op == 1)
			return currentValue + value;
		//if (op == 2)
		//	return currentValue - value;
		if (op == 3)
			return currentValue | value;
		if (op == 4)
			return currentValue & value;
		return currentValue;
	}

	public static Vector3 TargetAveragePos(UInt16 pTarID)
	{
		List<BTL_DATA> targList = btl_util.findAllBtlData(pTarID);
		if (targList.Count == 0)
		{
			return new Vector3();
		}
		else if (targList.Count == 1)
		{
			Transform targBone = targList[0].gameObject.transform.GetChildByName("bone" + targList[0].tar_bone.ToString("D3"));
			if (targBone == null)
				return new Vector3(targList[0].pos.x, 0f, targList[0].pos.z);
			Matrix4x4 tarmat = targBone.localToWorldMatrix;
			return new Vector3(tarmat.m03, 0f, tarmat.m23);
		}
		Vector3 result = new Vector3();
		foreach (BTL_DATA btl in targList)
			result += btl.pos;
		result /= targList.Count;
		result.y = 0f;
		return result;
	}

	public static Vector3 PolarVector(Vector3 orientation)
	{
		if (orientation.x == 0 && orientation.z == 0)
			return new Vector3();
		Single angle = Mathf.Atan2(orientation.z, orientation.x);
		return new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
	}
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using NCalc;
using Memoria.Assets;
using Memoria.Prime;
using Memoria.Prime.CSV;
using Memoria.Prime.Text;

namespace Memoria
{
	public static class WorldConfiguration
	{
		public static void PatchAllWorldConfig()
		{
			PatchWorldEnvironment();
			PatchWorldCHRControl();
			PatchWorldWeatherColor();
		}

		public static void ExportAllCSV()
		{
			try
			{
				String relativePath = DataResources.World.PureDirectory;
				String outputDirectory = Path.Combine(Configuration.Export.Path, relativePath);
				Directory.CreateDirectory(outputDirectory);
				CsvMetaData csvOptions = new CsvMetaData();
				using (CsvWriter csv = new CsvWriter(outputDirectory + DataResources.World.CHRControlFile))
				{
					csv.WriteMetaData(csvOptions);
					csv.WriteLine("# This file contains world map transport control parameters.");
					csv.WriteLine("# ----------------------------------------------------------------------------------------------------------------------------------");
					csv.WriteLine("# type;flg_gake;speed_move;speed_rotation;speed_updown;speed_roll;speed_rollback;flg_fly;flg_upcam;fetchdist;music;se;encount;radius;camrot;type_cam;pad2;limit0;limit1;");
					csv.WriteLine("# Byte;Byte;Int16;Byte;Byte;SByte;SByte;Boolean;Boolean;Byte;Byte;Byte;Boolean;Int16;Boolean;Byte;UInt16;UInt32;UInt32;");
					csv.WriteLine("# ----------------------------------------------------------------------------------------------------------------------------------");
					csv.WriteEntry(ff9.w_moveCHRControl[0], csvOptions, "Walking");
					csv.WriteEntry(ff9.w_moveCHRControl[1], csvOptions, "Yellow Chocobo");
					csv.WriteEntry(ff9.w_moveCHRControl[2], csvOptions, "Light Blue Chocobo");
					csv.WriteEntry(ff9.w_moveCHRControl[3], csvOptions, "Red Chocobo");
					csv.WriteEntry(ff9.w_moveCHRControl[4], csvOptions, "Deep Blue Chocobo");
					csv.WriteEntry(ff9.w_moveCHRControl[5], csvOptions, "Gold Chocobo (ground)");
					csv.WriteEntry(ff9.w_moveCHRControl[6], csvOptions, "Gold Chocobo (air)");
					csv.WriteEntry(ff9.w_moveCHRControl[7], csvOptions, "Blue Narciss");
					csv.WriteEntry(ff9.w_moveCHRControl[8], csvOptions, "Hilda Garde III");
					csv.WriteEntry(ff9.w_moveCHRControl[9], csvOptions, "Invincible");
					csv.WriteEntry(ff9.w_moveCHRControl[10], csvOptions, "???");
					csv.WriteEntry(ff9.w_moveCHRControl[11], csvOptions, "???");
					for (Int32 i = 12; i < ff9.w_moveCHRControl.Length; i++)
						csv.WriteEntry(ff9.w_moveCHRControl[i], csvOptions, "???");
				}
				using (CsvWriter csv = new CsvWriter(outputDirectory + DataResources.World.WeatherColorFile))
				{
					csv.WriteMetaData(csvOptions);
					csv.WriteLine("# This file contains world map weather colors.");
					csv.WriteLine("# ----------------------------------------------------------------------------------------------------------------------------------");
					csv.WriteLine("# light0.vx;light0.vy;light0.vz;light1.vx;light1.vy;light1.vz;light2.vx;light2.vy;light2.vz;light0c.vx;light0c.vy;light0c.vz;ambient.vx;ambient.vy;ambient.vz;ambientcl.vx;ambientcl.vy;ambientcl.vz;goffsetup;toffsetup;fogUP.vx;fogUP.vy;fogUP.vz;goffsetdw;toffsetdw;fogDW.vx;fogDW.vy;fogDW.vz;goffsetcl;toffsetcl;fogCL.vx;fogCL.vy;fogCL.vz;chrBIAS.vx;chrBIAS.vy;chrBIAS.vz;fogAMP;offsetX;scaleY;skyBgColor.r;skyBgColor.g;skyBgColor.b;skyBgColor.a;skyFogColor.r;skyFogColor.g;skyFogColor.b;skyFogColor.a;lightColorFactor;");
					csv.WriteLine("# Int16;Int16;Int16;Int16;Int16;Int16;Int16;Int16;Int16;Int16;Int16;Int16;Int16;Int16;Int16;Int16;Int16;Int16;UInt16;UInt16;Int16;Int16;Int16;UInt16;UInt16;Int16;Int16;Int16;UInt16;UInt16;Int16;Int16;Int16;Int16;Int16;Int16;UInt16;Single;Single;Single;Single;Single;Single;Single;Single;Single;Single;Single;");
					csv.WriteLine("# ----------------------------------------------------------------------------------------------------------------------------------");
					csv.WriteEntry(ff9.w_weatherColor.Color[0], csvOptions, "Daylight 0");
					csv.WriteEntry(ff9.w_weatherColor.Color[1], csvOptions, "Daylight 1");
					csv.WriteEntry(ff9.w_weatherColor.Color[2], csvOptions, "Daylight 2");
					csv.WriteEntry(ff9.w_weatherColor.Color[3], csvOptions, "Daylight 3");
					csv.WriteEntry(ff9.w_weatherColor.Color[4], csvOptions, "Evening 0");
					csv.WriteEntry(ff9.w_weatherColor.Color[5], csvOptions, "Evening 1");
					csv.WriteEntry(ff9.w_weatherColor.Color[6], csvOptions, "Evening 2");
					csv.WriteEntry(ff9.w_weatherColor.Color[7], csvOptions, "Evening 3");
					csv.WriteEntry(ff9.w_weatherColor.Color[8], csvOptions, "Night 0");
					csv.WriteEntry(ff9.w_weatherColor.Color[9], csvOptions, "Night 1");
					csv.WriteEntry(ff9.w_weatherColor.Color[10], csvOptions, "Night 2");
					csv.WriteEntry(ff9.w_weatherColor.Color[11], csvOptions, "Night 3");
					csv.WriteEntry(ff9.w_weatherColor.Color[12], csvOptions, "Purple Sky 0");
					csv.WriteEntry(ff9.w_weatherColor.Color[13], csvOptions, "Purple Sky 1");
					csv.WriteEntry(ff9.w_weatherColor.Color[14], csvOptions, "Purple Sky 2");
					csv.WriteEntry(ff9.w_weatherColor.Color[15], csvOptions, "Purple Sky 3");
					for (Int32 i = 16; i < ff9.w_weatherColor.Color.Length; i++)
						csv.WriteEntry(ff9.w_weatherColor.Color[i], csvOptions, "Runtime color");
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"[WorldConfiguration] {nameof(ExportAllCSV)} failed.");
			}
		}

		public static Boolean PatchWorldEnvironment()
		{
			_customPlaceModifier.Clear();
			_customEffectModifier.Clear();
			_customMistModifier.Clear();
			_customDiscModifier.Clear();
			_customRainModifier.Clear();
			_customLightModifier.Clear();
			try
			{
				Boolean foundOne = false;
				String inputPath = DataResources.World.Directory + DataResources.World.EnvironmentPatchFile;
				if (File.Exists(inputPath))
					foundOne = LoadWorldEnvironmentFile(File.ReadAllText(inputPath)) || foundOne;
				for (Int32 i = Configuration.Mod.FolderNames.Length - 1; i >= 0; i--)
				{
					inputPath = DataResources.World.ModDirectory(Configuration.Mod.FolderNames[i]) + DataResources.World.EnvironmentPatchFile;
					if (File.Exists(inputPath))
						foundOne = LoadWorldEnvironmentFile(File.ReadAllText(inputPath)) || foundOne;
				}
				return foundOne;
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"[WorldConfiguration] {nameof(PatchWorldEnvironment)} failed.");
				return false;
			}
		}

		public static Boolean PatchWorldCHRControl()
		{
			try
			{
				String[] dir = Configuration.Mod.AllFolderNames;
				String inputPath;
				for (Int32 i = 0; i < dir.Length; i++)
				{
					if (String.IsNullOrEmpty(dir[i]))
						inputPath = DataResources.World.Directory + DataResources.World.CHRControlFile;
					else
						inputPath = DataResources.World.ModDirectory(dir[i]) + DataResources.World.CHRControlFile;
					if (File.Exists(inputPath))
					{
						ff9.s_moveCHRControl[] controls = CsvReader.Read<ff9.s_moveCHRControl>(inputPath);
						if (controls.Length < 12)
						{
							Log.Error($"[WorldConfiguration] You must set at least 12 controls, but there are {controls.Length} in '{inputPath}'.");
							return false;
						}
						ff9.w_moveCHRControl = controls;
						return true;
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"[WorldConfiguration] {nameof(PatchWorldCHRControl)} failed.");
				return false;
			}
		}

		public static Boolean PatchWorldWeatherColor()
		{
			try
			{
				String[] dir = Configuration.Mod.AllFolderNames;
				String inputPath;
				for (Int32 i = 0; i < dir.Length; i++)
				{
					if (String.IsNullOrEmpty(dir[i]))
						inputPath = DataResources.World.Directory + DataResources.World.WeatherColorFile;
					else
						inputPath = DataResources.World.ModDirectory(dir[i]) + DataResources.World.WeatherColorFile;
					if (File.Exists(inputPath))
					{
						ff9.sw_weatherColorElement[] colors = CsvReader.Read<ff9.sw_weatherColorElement>(inputPath);
						if (colors.Length != 23)
						{
							Log.Error($"[WorldConfiguration] You must set 23 colors, but there are {colors.Length} in '{inputPath}'.");
							return false;
						}
						ff9.w_weatherColor.Color = colors;
						return true;
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"[WorldConfiguration] {nameof(PatchWorldWeatherColor)} failed.");
				return false;
			}
		}

		public static Boolean UsePlaceAlternateForm(WorldPlace place)
		{
			// Custom usage condition
			if (_customPlaceModifier.ContainsKey(place) && _customPlaceModifier[place].HasCondition)
				return _customPlaceModifier[place].IsActive;
			// Default usage condition
			switch (place)
			{
				case WorldPlace.SouthGate_Gate: // South Gate destroyed
					return ff9.w_frameDisc == 1 && ff9.w_frameScenePtr >= 2990 && ff9.w_frameScenePtr < 6990;
				case WorldPlace.Alexandria: // Alexandria destroyed
					return ff9.w_frameDisc == 1 && ff9.w_frameScenePtr >= 8800;
				case WorldPlace.FireShrine: // Fire Shrine opened
					return ff9.w_frameDisc == 1 && ff9.w_frameScenePtr >= 10600 && ff9.w_frameScenePtr < 10700;
				case WorldPlace.Lindblum: // Lindblum destroyed
					return ff9.w_frameDisc == 1 && ff9.w_frameScenePtr >= 5598;
				case WorldPlace.Cleyra: // Cleyra destroyed
					return ff9.w_frameDisc == 1 && ff9.w_frameScenePtr >= 4990;
				case WorldPlace.ChocoboParadise: // Chocobo Paradise discovered
					return (ff9.byte_gEventGlobal(101) & 0x40) != 0;
				case WorldPlace.BlackMageVillage: // Black Mage Village entered (?)
					return ff9.w_frameDisc == 1 && ff9.w_frameScenePtr >= 6200;
				case WorldPlace.WaterShrine: // Water Shrine opened
					return ff9.w_frameDisc == 1 && ff9.w_frameScenePtr >= 10600 && ff9.w_frameScenePtr < 10700;
				case WorldPlace.MognetCentral: // Mognet Central discovered
					return (ff9.byte_gEventGlobal(101) & 0x80) != 0;
			}
			return false;
		}

		public static Boolean UseWorldEffect(WorldEffect eff)
		{
			// Custom usage condition
			if (_customEffectModifier.ContainsKey(eff) && _customEffectModifier[eff].HasCondition)
				return _customEffectModifier[eff].IsActive;
			// Default usage condition
			switch (eff)
			{
				case WorldEffect.FireShrine:
					return true;
				case WorldEffect.SandPit:
					return ff9.w_frameScenePtr >= 9450 && ff9.w_frameScenePtr <= 9890;
				case WorldEffect.SandStorm:
					return ff9.w_frameDisc == 1 && !UsePlaceAlternateForm(WorldPlace.Cleyra);
				case WorldEffect.AlexandriaWaterfall:
					return true;
				case WorldEffect.Memoria:
					return ff9.w_frameDisc == 4 || ff9.tweaker.HaskEffectBlockEva;
				case WorldEffect.Windmill:
					return ff9.w_frameScenePtr < 6990 || ff9.w_frameScenePtr >= 11090;
				case WorldEffect.WaterShrine:
					return UsePlaceAlternateForm(WorldPlace.WaterShrine);
				case WorldEffect.WindShrine:
					return false;
			}
			return false;
		}

		public static Boolean UseMist()
		{
			// Custom usage condition
			if (_customMistModifier.HasCondition)
				return _customMistModifier.IsActive;
			// Default usage condition
			if (FF9StateSystem.World.IsBeeScene)
				return ff9.tweaker.w_frameFog;
			return ff9.w_frameScenePtr < 5990 || ff9.w_frameScenePtr > 11090; // Before reached Exterior Continent or after Disc 4
		}

		public static Byte GetDisc()
		{
			// Custom usage setting
			if (_customDiscModifier.HasCondition)
				return (Byte)(_customDiscModifier.IsActive ? 4 : 1);
			// Default usage setting
			return (Byte)(ff9.w_frameScenePtr >= 11090 ? 4 : 1);
		}

		public static void GetRainParameters(out Byte rainStrength, out Int32 rainSpeed)
		{
			rainStrength = 0;
			rainSpeed = 0;
			// Custom usage setting
			if (_customRainModifier.Count > 0)
			{
				foreach (RainModifier modifier in _customRainModifier)
					modifier.RainUpdate(ref rainStrength, ref rainSpeed);
				return;
			}
			// Default usage setting
			Vector3 burmeciaPos = new Vector3(30986f, 101f, -23243f); // (247888, 808, -185944) in fixed-point coordinates
			Byte wmID = ff9.byte_gEventGlobal(102);
			if (wmID == 0 || wmID == 8) // No transport / Invincible
			{
				Vector3 playerPos = ff9.w_moveActorPtr.RealPosition;
				playerPos *= 32f;
				Int32 dist = Mathf.FloorToInt((playerPos - burmeciaPos).magnitude);
				if (dist < 1148) // 9184 in fixed-point size
				{
					Int32 str = 1148 - dist;
					if (str > 1024) // 8192 in fixed-point size
						str = 1024;
					if (str > 16)
					{
						rainStrength = (Byte)(str >> 4);
						rainSpeed = 64;
					}
				}
			}
		}

		public static Int32 GetWeatherLight()
		{
			Int32 lightIndex = 0;
			// Custom usage setting
			if (_customLightModifier.Count > 0)
			{
				foreach (LightModifier modifier in _customLightModifier)
					modifier.LightUpdate(ref lightIndex);
				return lightIndex;
			}
			// Default usage setting
			LightModifier[] defaultLight = new LightModifier[4]
			{
				new LightModifier // Treno (evening)
				{
					_position = new Vector3(ff9.S(329728), 0f, ff9.S(-240896)),
					_radius = ff9.S(22016),
					_light = 1
				},
				new LightModifier // Treno (night)
				{
					_position = new Vector3(ff9.S(330496), 0f, ff9.S(-237568)),
					_radius = ff9.S(15104),
					_light = 2
				},
				new LightModifier // South Forgotten Continent (evening)
				{
					_position = new Vector3(ff9.S(128000), 0f, ff9.S(-202240)),
					_radius = ff9.S(46080),
					_light = 1
				},
				new LightModifier // Memoria's entrance (purple light)
				{
					_position = new Vector3(ff9.S(196158), 0f, ff9.S(-81825)),
					_radius = ff9.S(26080),
					_light = 3
				}
			};
			for (Int32 i = 0; i < 3; i++)
				defaultLight[i].LightUpdate(ref lightIndex);
			if (ff9.IsBeeScene || WorldConfiguration.UseWorldEffect(WorldEffect.Memoria))
				defaultLight[3].LightUpdate(ref lightIndex);
			return lightIndex;
		}

		private static Boolean LoadWorldEnvironmentFile(String input)
		{
			Boolean doneSomething = false;
			MatchCollection codeMatches = new Regex(@"^(Place|Effect|Mist|Disc4|Rain|Light)\s+(.*)$", RegexOptions.Multiline).Matches(input);
			for (Int32 i = 0; i < codeMatches.Count; i++)
			{
				MatchCollection argMatches = new Regex(@"\s*(\[[^\]]*\]|[^\]][^\s]*)").Matches(codeMatches[i].Groups[2].Value.Trim());
				String[] args = new String[argMatches.Count];
				for (Int32 j = 0; j < argMatches.Count; j++)
					args[j] = argMatches[j].Groups[1].Value;
				if (String.Compare(codeMatches[i].Groups[1].Value, "Place") == 0)
					doneSomething = LoadWorldEnvironmentDictionaryToken(args, _customPlaceModifier) || doneSomething;
				else if (String.Compare(codeMatches[i].Groups[1].Value, "Effect") == 0)
					doneSomething = LoadWorldEnvironmentDictionaryToken(args, _customEffectModifier) || doneSomething;
				else if (String.Compare(codeMatches[i].Groups[1].Value, "Mist") == 0)
					doneSomething = LoadWorldEnvironmentSimpleToken(args, _customMistModifier) || doneSomething;
				else if (String.Compare(codeMatches[i].Groups[1].Value, "Disc4") == 0)
					doneSomething = LoadWorldEnvironmentSimpleToken(args, _customDiscModifier) || doneSomething;
				else if (String.Compare(codeMatches[i].Groups[1].Value, "Rain") == 0)
					doneSomething = LoadWorldEnvironmentRainToken(args, _customRainModifier) || doneSomething;
				else if (String.Compare(codeMatches[i].Groups[1].Value, "Light") == 0)
					doneSomething = LoadWorldEnvironmentLightToken(args, _customLightModifier) || doneSomething;
			}
			return doneSomething;
		}

		private static Boolean LoadWorldEnvironmentDictionaryToken<T>(String[] args, Dictionary<T, ConditionalModifier> dict) where T : Enum
		{
			if (args.Length == 0)
				return false;
			T key;
			if (String.Compare(args[0], "Clear") == 0)
			{
				dict.Clear();
				return true;
			}
			if (args[0].TryEnumParse(out key) && args.Length >= 2)
			{
				if (String.Compare(args[1], "Clear") == 0)
				{
					dict.Remove(key);
					return true;
				}
				else if (args[1].StartsWith("[Condition=") && args[1].EndsWith("]"))
				{
					ConditionalModifier modifier;
					if (!dict.TryGetValue(key, out modifier))
					{
						modifier = new ConditionalModifier();
						dict.Add(key, modifier);
					}
					modifier._condition.Add(args[1].Substring("[Condition=".Length, args[1].Length - "[Condition=]".Length));
					return true;
				}
			}
			return false;
		}

		private static Boolean LoadWorldEnvironmentSimpleToken(String[] args, ConditionalModifier modifier)
		{
			if (args.Length == 0)
				return false;
			if (String.Compare(args[0], "Clear") == 0)
			{
				modifier._condition.Clear();
				return true;
			}
			if (args[0].StartsWith("[Condition=") && args[0].EndsWith("]"))
			{
				modifier._condition.Add(args[0].Substring("[Condition=".Length, args[0].Length - "[Condition=]".Length));
				return true;
			}
			return false;
		}

		private static Boolean LoadWorldEnvironmentRainToken(String[] args, List<RainModifier> modifierList)
		{
			if (args.Length == 0)
				return false;
			if (String.Compare(args[0], "Clear") == 0)
			{
				modifierList.Clear();
				return true;
			}
			if (String.Compare(args[0], "Add") != 0)
				return false;
			RainModifier modifier = new RainModifier();
			Int32 coord;
			for (Int32 i = 1; i < args.Length; i++)
			{
				if (args[i].StartsWith("[Condition=") && args[i].EndsWith("]"))
				{
					modifier._condition.Add(args[i].Substring("[Condition=".Length, args[i].Length - "[Condition=]".Length));
				}
				else if (args[i].StartsWith("[Position=") && args[i].EndsWith("]"))
				{
					String posstr = args[i].Substring("[Position=".Length, args[i].Length - "[Position=]".Length);
					if (posstr.StartsWith("(") && posstr.EndsWith(")"))
						posstr = posstr.Substring(1, posstr.Length - 2);
					String[] coords = posstr.Split(',');
					if (coords.Length == 2)
					{
						Int32.TryParse(coords[0], out coord);
						modifier._position.x = ff9.S(coord);
						Int32.TryParse(coords[1], out coord);
						modifier._position.z = ff9.S(coord);
					}
					else if (coords.Length == 3)
					{
						Int32.TryParse(coords[0], out coord);
						modifier._position.x = ff9.S(coord);
						Int32.TryParse(coords[1], out coord);
						modifier._position.y = ff9.S(coord);
						Int32.TryParse(coords[2], out coord);
						modifier._position.z = ff9.S(coord);
					}
				}
				else if (args[i].StartsWith("[RadiusLarge=") && args[i].EndsWith("]"))
				{
					if (Int32.TryParse(args[i].Substring("[RadiusLarge=".Length, args[i].Length - "[RadiusLarge=]".Length), out coord))
						modifier._radiusLarge = ff9.S(coord);
				}
				else if (args[i].StartsWith("[RadiusSmall=") && args[i].EndsWith("]"))
				{
					if (Int32.TryParse(args[i].Substring("[RadiusSmall=".Length, args[i].Length - "[RadiusSmall=]".Length), out coord))
						modifier._radiusSmall = ff9.S(coord);
				}
				else if (args[i].StartsWith("[RainSpeed=") && args[i].EndsWith("]"))
				{
					if (Int32.TryParse(args[i].Substring("[RainSpeed=".Length, args[i].Length - "[RainSpeed=]".Length), out coord))
						modifier._rainSpeed = coord;
				}
				else if (args[i].StartsWith("[RainStrength=") && args[i].EndsWith("]"))
				{
					if (Int32.TryParse(args[i].Substring("[RainStrength=".Length, args[i].Length - "[RainStrength=]".Length), out coord))
						modifier._rainMaxStrength = (Byte)coord;
				}
			}
			modifierList.Add(modifier);
			return false;
		}

		private static Boolean LoadWorldEnvironmentLightToken(String[] args, List<LightModifier> modifierList)
		{
			if (args.Length == 0)
				return false;
			if (String.Compare(args[0], "Clear") == 0)
			{
				modifierList.Clear();
				return true;
			}
			if (String.Compare(args[0], "Add") != 0)
				return false;
			LightModifier modifier = new LightModifier();
			Int32 coord;
			for (Int32 i = 1; i < args.Length; i++)
			{
				if (args[i].StartsWith("[Condition=") && args[i].EndsWith("]"))
				{
					modifier._condition.Add(args[i].Substring("[Condition=".Length, args[i].Length - "[Condition=]".Length));
				}
				else if (args[i].StartsWith("[Position=") && args[i].EndsWith("]"))
				{
					String posstr = args[i].Substring("[Position=".Length, args[i].Length - "[Position=]".Length);
					if (posstr.StartsWith("(") && posstr.EndsWith(")"))
						posstr = posstr.Substring(1, posstr.Length - 2);
					String[] coords = posstr.Split(',');
					if (coords.Length == 2)
					{
						Int32.TryParse(coords[0], out coord);
						modifier._position.x = ff9.S(coord);
						Int32.TryParse(coords[1], out coord);
						modifier._position.z = ff9.S(coord);
					}
					else if (coords.Length == 3)
					{
						Int32.TryParse(coords[0], out coord);
						modifier._position.x = ff9.S(coord);
						Int32.TryParse(coords[1], out coord);
						modifier._position.y = ff9.S(coord);
						Int32.TryParse(coords[2], out coord);
						modifier._position.z = ff9.S(coord);
					}
				}
				else if (args[i].StartsWith("[Radius=") && args[i].EndsWith("]"))
				{
					if (Int32.TryParse(args[i].Substring("[Radius=".Length, args[i].Length - "[Radius=]".Length), out coord))
						modifier._radius = ff9.S(coord);
				}
				else if (args[i].StartsWith("[Light=") && args[i].EndsWith("]"))
				{
					if (Int32.TryParse(args[i].Substring("[Light=".Length, args[i].Length - "[Light=]".Length), out coord))
						modifier._light = coord;
				}
			}
			modifierList.Add(modifier);
			return false;
		}

		private class ConditionalModifier
		{
			public List<String> _condition = new List<String>();
			public Boolean _isEvaluated = false;
			public Boolean _valueAsBool;

			public Boolean HasCondition => _condition.Count > 0;

			public Boolean IsActive
			{
				get
				{
					if (_isEvaluated)
						return _valueAsBool;
					_valueAsBool = false;
					_isEvaluated = true;
					foreach (String s in _condition)
					{
						Expression c = new Expression(s);
						c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
						c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
						c.EvaluateParameter += NCalcUtility.worldNCalcParameters;
						_valueAsBool = NCalcUtility.EvaluateNCalcCondition(c.Evaluate());
						if (_valueAsBool)
							return true;
					}
					return _valueAsBool;
				}
			}

			public void Clear()
			{
				_condition.Clear();
				_isEvaluated = false;
			}
		}

		private class RainModifier : ConditionalModifier
		{
			public Vector3 _position = new Vector3(ff9.S(247888), ff9.S(808), ff9.S(-185944));
			public Single _radiusLarge = ff9.S(9184);
			public Single _radiusSmall = ff9.S(992);
			public Byte _rainMaxStrength = 64;
			public Int32 _rainSpeed = 64;

			public void RainUpdate(ref Byte rainStrength, ref Int32 rainSpeed)
			{
				if (HasCondition && !IsActive)
					return;
				Single dist = ff9.PosDiff(_position, ff9.w_moveActorPtr.RealPosition).magnitude;
				if (dist >= _radiusLarge)
					return;
				Byte strength;
				if (_radiusLarge <= _radiusSmall || dist <= _radiusSmall)
					strength = _rainMaxStrength;
				else
					strength = (Byte)Mathf.FloorToInt((_radiusLarge - dist) / (_radiusLarge - _radiusSmall) * _rainMaxStrength);
				if (strength > rainStrength)
				{
					rainStrength = strength;
					rainSpeed = _rainSpeed;
				}
			}
		}

		private class LightModifier : ConditionalModifier
		{
			public Vector3 _position = new Vector3(ff9.S(196158), 0f, ff9.S(-81825));
			public Single _radius = ff9.S(26080);
			public Int32 _light = 3;

			public void LightUpdate(ref Int32 light)
			{
				if (HasCondition && !IsActive)
					return;
				_position.y = ff9.w_moveActorPtr.RealPosition.y;
				Single dist = ff9.PosDiff(_position, ff9.w_moveActorPtr.RealPosition).magnitude;
				if (dist < _radius)
					light = _light;
			}
		}

		private static Dictionary<WorldPlace, ConditionalModifier> _customPlaceModifier = new Dictionary<WorldPlace, ConditionalModifier>();
		private static Dictionary<WorldEffect, ConditionalModifier> _customEffectModifier = new Dictionary<WorldEffect, ConditionalModifier>();
		private static ConditionalModifier _customMistModifier = new ConditionalModifier();
		private static ConditionalModifier _customDiscModifier = new ConditionalModifier();
		private static List<RainModifier> _customRainModifier = new List<RainModifier>();
		private static List<LightModifier> _customLightModifier = new List<LightModifier>();
	}
}

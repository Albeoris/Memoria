using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Memoria.Prime;
using SimpleJSON;

namespace Memoria.Assets
{
	public class AnimationClipReader
	{
		public static Dictionary<String, KeyValuePair<AnimationClip, AnimationClipReader>> LoadedClips = new Dictionary<String, KeyValuePair<AnimationClip, AnimationClipReader>>();

		public static AnimationClip ReadAnimationClipFromDisc(String filepath)
		{
			return ReadAnimationClipFromDisc(filepath, out _);
		}

		public static AnimationClip ReadAnimationClipFromDisc(String filepath, out AnimationClipReader clipIO)
		{
			try
			{
				KeyValuePair<AnimationClip, AnimationClipReader> cacheClip;
				if (LoadedClips.TryGetValue(filepath, out cacheClip))
				{
					clipIO = cacheClip.Value;
					return cacheClip.Key;
				}
				Boolean isBinary;
				using (BinaryReader reader = new BinaryReader(File.Open(filepath, FileMode.Open)))
				{
					Char firstChar = reader.ReadChar();
					isBinary = firstChar != '{';
				}
				AnimationClip clip;
				if (isBinary)
				{
					using (FileStream stream = File.Open(filepath, FileMode.Open))
					using (BinaryReader reader = new BinaryReader(stream))
						clip = ReadAnimationClip_Binary(reader, out clipIO);
				}
				else
				{
					clip = ReadAnimationClip_JSON(File.ReadAllText(filepath), out clipIO);
				}
				LoadedClips[filepath] = new KeyValuePair<AnimationClip, AnimationClipReader>(clip, clipIO);
				return clip;
			}
			catch (Exception ex)
			{
				Log.Message("[AnimationClipReader] Error when trying to read " + filepath + ": " + ex.Message);
				clipIO = null;
				return new AnimationClip();
			}
		}

		private static AnimationClip ReadAnimationClip_Binary(BinaryReader reader, out AnimationClipReader clipIO)
		{
			AnimationClip clip = new AnimationClip();
			clipIO = new AnimationClipReader();
			clip.legacy = true;
			UInt32 nameLen = reader.ReadUInt32();
			clip.name = "";
			for (UInt32 i = 0; i < nameLen; ++i)
				clip.name += reader.ReadChar();
			if (nameLen % 4 != 0)
				reader.ReadChars(4 - (Int32)(nameLen % 4));
			clipIO.name = clip.name;
			clip.name = "CUSTOM_MUST_RENAME"; // Instead of using the internal .anim name, we use the anim string name from "_animationInFolder"
			UInt16 numUnk1 = reader.ReadUInt16();
			UInt16 numUnk2 = reader.ReadUInt16();
			AddAnimationCurves_Binary(reader, ref clip, typeof(Transform), "localRotation", new String[] { "x", "y", "z", "w" }, ref clipIO);
			reader.ReadUInt32();
			AddAnimationCurves_Binary(reader, ref clip, typeof(Transform), "localPosition", new String[] { "x", "y", "z" }, ref clipIO);
			AddAnimationCurves_Binary(reader, ref clip, typeof(Transform), "localScale", new String[] { "x", "y", "z" }, ref clipIO);
			reader.ReadUInt32();
			reader.ReadUInt32();
			clip.frameRate = reader.ReadSingle();
			clipIO.frameRate = clip.frameRate;
			// reader.ReadUInt32();
			// TODO: read the end
			return clip;
		}

		private static void AddAnimationCurves_Binary(BinaryReader reader, ref AnimationClip clip, Type type, String propertyName, String[] propertyField, ref AnimationClipReader clipIO)
		{
			UInt32 i, j, k;
			Keyframe[][] keys = new Keyframe[propertyField.Length][];
			Single timeKey;
			AnimationCurve newCurve;
			UInt32 boneCount = reader.ReadUInt32();
			UInt32 frameCount;
			for (i = 0; i < boneCount; ++i)
			{
				frameCount = reader.ReadUInt32();
				for (k = 0; k < propertyField.Length; ++k)
					keys[k] = new Keyframe[frameCount];
				for (j = 0; j < frameCount; ++j)
				{
					timeKey = reader.ReadSingle();
					for (k = 0; k < propertyField.Length; ++k)
						keys[k][j] = new Keyframe(timeKey, reader.ReadSingle());
					for (k = 0; k < propertyField.Length; ++k)
						keys[k][j].inTangent = reader.ReadSingle();
					for (k = 0; k < propertyField.Length; ++k)
						keys[k][j].outTangent = reader.ReadSingle();
				}
				reader.ReadUInt32();
				reader.ReadUInt32();
				UInt32 nameLen = reader.ReadUInt32();
				String boneName = "";
				for (j = 0; j < nameLen; j++)
					boneName += reader.ReadChar();
				if (nameLen % 4 != 0)
					reader.ReadChars(4 - (Int32)(nameLen % 4));
				for (k = 0; k < propertyField.Length; ++k)
				{
					newCurve = new AnimationCurve(keys[k]);
					clip.SetCurve(boneName, type, propertyName + "." + propertyField[k], newCurve);
				}
				BoneAnimation ba = clipIO.boneAnimList.Find(a => a.boneNameInHierarchy == boneName);
				if (ba == null)
				{
					ba = new BoneAnimation();
					ba.boneNameInHierarchy = boneName;
					clipIO.boneAnimList.Add(ba);
				}
				BoneAnimation.TransformAnimation ta = new BoneAnimation.TransformAnimation();
				ta.transformType = propertyName;
				for (j = 0; j < frameCount; ++j)
				{
					BoneAnimation.TransformAnimation.FrameAnimation fa = new BoneAnimation.TransformAnimation.FrameAnimation();
					fa.time = keys[0][j].time;
					for (k = 0; k < propertyField.Length; ++k)
					{
						fa.pos[(Int32)k] = keys[k][j].value;
						fa.posInnerTangent[(Int32)k] = keys[k][j].inTangent;
						fa.posOuterTangent[(Int32)k] = keys[k][j].outTangent;
					}
					ta.frameAnimList.Add(fa);
				}
				ba.transformAnimList.Add(ta);
			}
		}

		private static AnimationClip ReadAnimationClip_JSON(String filecontent, out AnimationClipReader clipIO)
		{
			AnimationClip clip = new AnimationClip();
			clipIO = new AnimationClipReader();
			clip.legacy = true;
			JSONClass jsonTopLevel = (JSONClass)JSONNode.Parse(filecontent);
			JSONNode nameNode, frameRateNode, transformNode, localNode, timeNode, keyNode;
			if (jsonTopLevel.Dict.TryGetValue("name", out nameNode))
				clip.name = nameNode.Value;
			clip.frameRate = 30f;
			if (jsonTopLevel.Dict.TryGetValue("frameRate", out frameRateNode))
				clip.frameRate = frameRateNode.AsFloat;
			clipIO.name = clip.name;
			clipIO.frameRate = clip.frameRate;
			clip.name = "CUSTOM_MUST_RENAME"; // Instead of using the internal name, we use the anim string name from "_animationInFolder"
			if (jsonTopLevel.Dict.TryGetValue("transform", out transformNode))
			{
				AnimationCurve animCurve;
				JSONArray transfArray = transformNode.AsArray;
				foreach (JSONNode boneNode in transfArray.Childs)
				{
					BoneAnimation ba = new BoneAnimation();
					JSONClass boneClass = boneNode as JSONClass;
					if (!boneClass.Dict.TryGetValue("bone", out nameNode))
						continue;
					String boneName = nameNode.Value;
					ba.boneNameInHierarchy = boneName;
					foreach (String localType in new String[] { "localRotation", "localPosition", "localScale" })
					{
						if (boneClass.Dict.TryGetValue(localType, out localNode))
						{
							BoneAnimation.TransformAnimation ta = new BoneAnimation.TransformAnimation();
							Dictionary<Single, BoneAnimation.TransformAnimation.FrameAnimation> faDict = new Dictionary<Single, BoneAnimation.TransformAnimation.FrameAnimation>();
							JSONArray rotArray = localNode.AsArray;
							List<Keyframe> keys_x = new List<Keyframe>();
							List<Keyframe> keys_y = new List<Keyframe>();
							List<Keyframe> keys_z = new List<Keyframe>();
							List<Keyframe> keys_w = new List<Keyframe>();
							ta.transformType = localType;
							foreach (JSONNode frameNode in rotArray.Childs)
							{
								BoneAnimation.TransformAnimation.FrameAnimation fa;
								JSONClass frameClass = frameNode as JSONClass;
								if (!frameClass.Dict.TryGetValue("time", out timeNode))
									continue;
								Single time = timeNode.AsFloat;
								if (!faDict.TryGetValue(time, out fa))
								{
									fa = new BoneAnimation.TransformAnimation.FrameAnimation();
									fa.time = time;
									faDict.Add(time, fa);
								}
								if (frameClass.Dict.TryGetValue("x", out keyNode))
								{
									fa.pos.x = keyNode.AsFloat;
									keys_x.Add(new Keyframe(time, fa.pos.x));
								}
								if (frameClass.Dict.TryGetValue("y", out keyNode))
								{
									fa.pos.y = keyNode.AsFloat;
									keys_y.Add(new Keyframe(time, fa.pos.y));
								}
								if (frameClass.Dict.TryGetValue("z", out keyNode))
								{
									fa.pos.z = keyNode.AsFloat;
									keys_z.Add(new Keyframe(time, fa.pos.z));
								}
								if (frameClass.Dict.TryGetValue("w", out keyNode))
								{
									fa.pos.w = keyNode.AsFloat;
									keys_w.Add(new Keyframe(time, fa.pos.w));
								}
								// TODO: read inward/outward tangent (with keys "xInnerTangent", "xOuterTangent", etc.)
							}
							if (!keys_x.IsNullOrEmpty())
							{
								animCurve = new AnimationCurve(keys_x.ToArray());
								clip.SetCurve(boneName, typeof(Transform), localType + ".x", animCurve);
							}
							if (!keys_y.IsNullOrEmpty())
							{
								animCurve = new AnimationCurve(keys_y.ToArray());
								clip.SetCurve(boneName, typeof(Transform), localType + ".y", animCurve);
							}
							if (!keys_z.IsNullOrEmpty())
							{
								animCurve = new AnimationCurve(keys_z.ToArray());
								clip.SetCurve(boneName, typeof(Transform), localType + ".z", animCurve);
							}
							if (!keys_w.IsNullOrEmpty())
							{
								animCurve = new AnimationCurve(keys_w.ToArray());
								clip.SetCurve(boneName, typeof(Transform), localType + ".w", animCurve);
							}
							ta.frameAnimList.AddRange(faDict.Values);
							ba.transformAnimList.Add(ta);
						}
					}
					clipIO.boneAnimList.Add(ba);
				}
			}
			return clip;
		}

		public static void CreateDummyAnimationClip(GameObject geo, String name, Single length = 0f)
		{
			AnimationClip clip = new AnimationClip();
			clip.legacy = true;
			clip.name = name;
			clip.frameRate = 30f;
			Keyframe[] keys = length > 0f ? new Keyframe[] {
				new Keyframe( 0f, 1f ),
				new Keyframe( length, 1f )
			} : new Keyframe[] {
				new Keyframe( 0f, 1f )
			};
			AnimationCurve curve = new AnimationCurve(keys);
			clip.SetCurve("bone000", typeof(Transform), "localScale.x", curve);
			clip.SetCurve("bone000", typeof(Transform), "localScale.y", curve);
			clip.SetCurve("bone000", typeof(Transform), "localScale.z", curve);
			geo.GetComponent<Animation>().AddClip(clip, name);
		}

		public String name;
		public Single frameRate = 60f;
		public List<BoneAnimation> boneAnimList = new List<BoneAnimation>();

		public String ParseToJSON()
		{
			List<String> childStr = new List<String>();
			String json = "{\n";
			String indent = "    ";
			if (!String.IsNullOrEmpty(name))
				json += indent + $"\"name\": \"{name}\",\n";
			json += indent + $"\"frameRate\": {frameRate},\n";
			json += indent + $"\"transform\": [\n";
			indent += "    ";
			foreach (BoneAnimation ba in boneAnimList)
				childStr.Add(ba.ParseToJSON(ref indent));
			indent = indent.Substring(4);
			json += indent + "{\n" + String.Join(indent + "},\n" + indent + "{\n", childStr.ToArray()) + indent + "}]\n";
			indent = indent.Substring(4);
			json += indent + "}\n";
			return json;
		}

		public class BoneAnimation
		{
			public String boneNameInHierarchy;
			public List<TransformAnimation> transformAnimList = new List<TransformAnimation>();

			public String ParseToJSON(ref String indent)
			{
				List<String> childStr = new List<String>();
				String json = "";
				if (!String.IsNullOrEmpty(boneNameInHierarchy))
					json += indent + $"\"bone\": \"{boneNameInHierarchy}\",\n";
				foreach (TransformAnimation ta in transformAnimList)
					childStr.Add(ta.ParseToJSON(ref indent));
				json += String.Join(",\n", childStr.ToArray()) + "\n";
				return json;
			}

			public class TransformAnimation
			{
				public String transformType;
				public List<FrameAnimation> frameAnimList = new List<FrameAnimation>();

				public String ParseToJSON(ref String indent)
				{
					List<String> childStr = new List<String>();
					String json = "";
					json += indent + $"\"{transformType}\": [\n";
					indent += "    ";
					foreach (FrameAnimation fa in frameAnimList)
						childStr.Add(fa.ParseToJSON(ref indent, transformType == "localRotation"));
					json += String.Join(",\n", childStr.ToArray()) + "\n";
					indent = indent.Substring(4);
					json += indent + $"]";
					return json;
				}

				public class FrameAnimation
				{
					public Single time;
					public Vector4 pos;
					public Vector4 posInnerTangent;
					public Vector4 posOuterTangent;

					public String ParseToJSON(ref String indent, Boolean writeW)
					{
						List<String> param = new List<String>();
						param.Add($"\"time\": {time}");
						param.Add($"\"x\": {pos.x}");
						param.Add($"\"y\": {pos.y}");
						param.Add($"\"z\": {pos.z}");
						if (writeW)
							param.Add($"\"w\": {pos.w}");
						param.Add($"\"xInnerTangent\": {posInnerTangent.x}");
						param.Add($"\"yInnerTangent\": {posInnerTangent.y}");
						param.Add($"\"zInnerTangent\": {posInnerTangent.z}");
						if (writeW)
							param.Add($"\"wInnerTangent\": {posInnerTangent.w}");
						param.Add($"\"xOuterTangent\": {posOuterTangent.x}");
						param.Add($"\"yOuterTangent\": {posOuterTangent.y}");
						param.Add($"\"zOuterTangent\": {posOuterTangent.z}");
						if (writeW)
							param.Add($"\"wOuterTangent\": {posOuterTangent.w}");
						return indent + "{ " + String.Join(", ", param.ToArray()) + "}";
					}
				}
			}
		}
	}
}

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Memoria.Prime;
using SimpleJSON;

namespace Memoria.Assets
{
	public static class AnimationClipReader
	{
		public static AnimationClip ReadAnimationClipFromDisc(String filepath)
		{
			try
			{
				Boolean isBinary;
				using (BinaryReader reader = new BinaryReader(File.Open(filepath, FileMode.Open)))
				{
					char firstChar = reader.ReadChar();
					isBinary = firstChar != '{';
				}
				if (isBinary)
					return ReadAnimationClip_Binary(filepath);
				return ReadAnimationClip_JSON(filepath);
			}
			catch (Exception ex)
			{
				Log.Message("[AnimationClipReader] Error when trying to read " + filepath + ": " + ex.Message);
				return new AnimationClip();
			}
		}

		private static AnimationClip ReadAnimationClip_Binary(String filepath)
		{
			using (BinaryReader reader = new BinaryReader(File.Open(filepath, FileMode.Open)))
			{
				AnimationClip clip = new AnimationClip();
				clip.legacy = true;
				UInt32 nameLen = reader.ReadUInt32();
				clip.name = "";
				for (UInt32 i = 0; i < nameLen; ++i)
					clip.name += reader.ReadChar();
				if (nameLen % 4 != 0)
					reader.ReadChars(4 - (Int32)(nameLen % 4));
				clip.name = "CUSTOM_MUST_RENAME"; // Instead of using the internal .anim name, we use the anim string name from "_animationInFolder"
				UInt16 numUnk1 = reader.ReadUInt16();
				UInt16 numUnk2 = reader.ReadUInt16();
				AddAnimationCurves_Binary(reader, ref clip, typeof(Transform), "localRotation", new String[] { "x", "y", "z", "w" });
				reader.ReadUInt32();
				AddAnimationCurves_Binary(reader, ref clip, typeof(Transform), "localPosition", new String[] { "x", "y", "z" });
				AddAnimationCurves_Binary(reader, ref clip, typeof(Transform), "localScale", new String[] { "x", "y", "z" });
				reader.ReadUInt32();
				reader.ReadUInt32();
				clip.frameRate = reader.ReadSingle();
				// reader.ReadUInt32();
				// TODO: read the end
				return clip;
			}
		}

		private static void AddAnimationCurves_Binary(BinaryReader reader, ref AnimationClip clip, Type type, String propertyName, String[] propertyField)
		{
			UInt32 i, j, k;
			Keyframe[][] keys = new Keyframe[propertyField.Length][];
			float timeKey;
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
					for (k = 0; k < propertyField.Length; ++k) // TODO: inward tangent
						reader.ReadSingle();
					for (k = 0; k < propertyField.Length; ++k) // TODO: outward tangent
						reader.ReadSingle();
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
			}
		}

		private static AnimationClip ReadAnimationClip_JSON(String filepath)
		{
			AnimationClip clip = new AnimationClip();
			clip.legacy = true;
			String jsonText = File.ReadAllText(filepath);
			JSONClass jsonTopLevel = (JSONClass)JSONNode.Parse(jsonText);
			JSONNode nameNode, frameRateNode, transformNode, localNode, timeNode, keyNode;
			if (jsonTopLevel.Dict.TryGetValue("name", out nameNode))
				clip.name = nameNode.Value;
			clip.name = "CUSTOM_MUST_RENAME"; // Instead of using the internal name, we use the anim string name from "_animationInFolder"
			clip.frameRate = 30.0f;
			if (jsonTopLevel.Dict.TryGetValue("frameRate", out frameRateNode))
				clip.frameRate = frameRateNode.AsFloat;
			if (jsonTopLevel.Dict.TryGetValue("transform", out transformNode))
			{
				AnimationCurve animCurve;
				JSONArray transfArray = transformNode.AsArray;
				foreach (JSONNode boneNode in transfArray.Childs)
				{
					JSONClass boneClass = boneNode as JSONClass;
					if (!boneClass.Dict.TryGetValue("bone", out nameNode))
						continue;
					String boneName = nameNode.Value;
					foreach (String localType in new String[] { "localRotation", "localPosition", "localScale" })
					{
						if (boneClass.Dict.TryGetValue(localType, out localNode))
						{
							JSONArray rotArray = localNode.AsArray;
							List<Keyframe> keys_x = new List<Keyframe>();
							List<Keyframe> keys_y = new List<Keyframe>();
							List<Keyframe> keys_z = new List<Keyframe>();
							List<Keyframe> keys_w = new List<Keyframe>();
							foreach (JSONNode frameNode in rotArray.Childs)
							{
								JSONClass frameClass = frameNode as JSONClass;
								if (!frameClass.Dict.TryGetValue("time", out timeNode))
									continue;
								Single time = timeNode.AsFloat;
								if (frameClass.Dict.TryGetValue("x", out keyNode))
									keys_x.Add(new Keyframe(time, keyNode.AsFloat));
								if (frameClass.Dict.TryGetValue("y", out keyNode))
									keys_y.Add(new Keyframe(time, keyNode.AsFloat));
								if (frameClass.Dict.TryGetValue("z", out keyNode))
									keys_z.Add(new Keyframe(time, keyNode.AsFloat));
								if (frameClass.Dict.TryGetValue("w", out keyNode))
									keys_w.Add(new Keyframe(time, keyNode.AsFloat));
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
						}
					}
				}
			}
			return clip;
		}
	}
}

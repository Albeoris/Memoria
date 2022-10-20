using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Memoria.Prime;
using Memoria.Assets;
using UnityEngine;

namespace Memoria.Assets
{
	public static class ModelViewerScene
	{
		public static Boolean initialized = false;
		public static Boolean displayBones = false;
		public static Boolean displayBoneConnections = false;
		public static Boolean displayBoneNames = false;
		public static Boolean displayModelAnimNames = false;
		public static List<String> geoList;
		public static List<String> animList;
		public static HashSet<Int32> geoArchetype;
		public static Int32 currentGeoIndex;
		public static Int32 currentAnimIndex;
		public static String currentAnimName;
		public static GameObject currentModel;
		public static Vector3 scaleFactor;
		public static Single speedFactor;
		public static String savedAnimationPath;

		public static void Init()
		{
			ModelViewerScene.isLoadingModel = false;
			ModelViewerScene.scaleFactor = new Vector3(1f, 1f, 1f);
			ModelViewerScene.geoList = new List<String>();
			ModelViewerScene.geoArchetype = new HashSet<Int32>();
			ModelViewerScene.speedFactor = 1f;
			ModelViewerScene.savedAnimationPath = null;
			foreach (KeyValuePair<Int32, String> geo in FF9BattleDB.GEO)
				ModelViewerScene.geoList.Add(geo.Value);
			ModelViewerScene.geoArchetype.Add(0);
			String lastArchetype = ModelViewerScene.geoList[0].Substring(0, 8);
			for (Int32 i = 0; i < ModelViewerScene.geoList.Count; i++)
			{
				if (!ModelViewerScene.geoList[i].StartsWith(lastArchetype))
				{
					ModelViewerScene.geoArchetype.Add(i);
					lastArchetype = ModelViewerScene.geoList[i].Substring(0, 8);
				}
			}
			ModelViewerScene.geoArchetype.Add(ModelViewerScene.geoList.Count);
			ChangeModel(0);
			SceneDirector.ClearFadeColor();
			Camera camera = GetCamera();
			camera.transform.position = new Vector3(0f, 0f, -1000f);
			camera.transform.LookAt(Vector3.zero, Vector3.down);
			FPSManager.SetTargetFPS(Configuration.Graphics.MenuFPS);
			FPSManager.SetMainLoopSpeed(Configuration.Graphics.MenuTPS);
			ModelViewerScene.initialized = true;
		}

		public static void Update()
		{
			try
			{
				if (ModelViewerScene.isLoadingModel)
					return;
				Boolean mouseLeftWasPressed = ModelViewerScene.mouseLeftPressed;
				Boolean mouseRightWasPressed = ModelViewerScene.mouseRightPressed;
				ModelViewerScene.mouseLeftPressed = false;
				ModelViewerScene.mouseRightPressed = false;
				if (Input.GetKeyDown(KeyCode.RightArrow))
				{
					Int32 nextIndex = ModelViewerScene.currentGeoIndex + 1;
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
						while (!ModelViewerScene.geoArchetype.Contains(nextIndex))
							nextIndex++;
					ChangeModel(nextIndex);
					while (ModelViewerScene.currentModel == null)
						ChangeModel(++nextIndex);
				}
				else if (Input.GetKeyDown(KeyCode.LeftArrow))
				{
					Int32 prevIndex = ModelViewerScene.currentGeoIndex - 1;
					if (prevIndex < 0)
						prevIndex += ModelViewerScene.geoList.Count;
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
						while (!ModelViewerScene.geoArchetype.Contains(prevIndex))
							prevIndex--;
					ChangeModel(prevIndex);
					while (ModelViewerScene.currentModel == null)
						ChangeModel(--prevIndex);
				}
				if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
					ModelViewerScene.speedFactor = 1f;
				else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
					ModelViewerScene.speedFactor = 0.5f;
				if (Input.GetKeyDown(KeyCode.B))
				{
					ModelViewerScene.displayBones = !ModelViewerScene.displayBones;
					ModelViewerScene.displayBoneNames = !ModelViewerScene.displayBoneNames;
				}
				if (Input.GetKeyDown(KeyCode.N))
					ModelViewerScene.displayModelAnimNames = !ModelViewerScene.displayModelAnimNames;
				if (ModelViewerScene.currentModel == null)
					return;
				if (Input.GetKeyDown(KeyCode.UpArrow))
					ChangeAnimation(ModelViewerScene.currentAnimIndex + 1);
				else if (Input.GetKeyDown(KeyCode.DownArrow))
					ChangeAnimation(ModelViewerScene.currentAnimIndex - 1);
				else if (Input.GetKeyDown(KeyCode.L))
				{
					Animation anim = ModelViewerScene.currentModel.GetComponent<Animation>();
					if (anim != null && !String.IsNullOrEmpty(ModelViewerScene.savedAnimationPath))
					{
						AnimationClip clip = AnimationClipReader.ReadAnimationClipFromDisc(ModelViewerScene.savedAnimationPath);
						if (clip != null)
						{
							ModelViewerScene.currentAnimName = "CUSTOM_CLIP";
							anim.RemoveClip("CUSTOM_CLIP");
							anim.AddClip(clip, "CUSTOM_CLIP");
							anim.Play("CUSTOM_CLIP");
							anim["CUSTOM_CLIP"].speed = ModelViewerScene.speedFactor;
							if (ModelViewerScene.modelAnimDialog != null)
							{
								ModelViewerScene.modelAnimDialog.ForceClose();
								ModelViewerScene.modelAnimDialog = null;
							}
						}
						else
						{
							FF9Sfx.FF9SFX_Play(102);
						}
					}
					else
					{
						FF9Sfx.FF9SFX_Play(102);
					}
				}
				if (Input.GetKey(KeyCode.Space) && ModelViewerScene.animList.Count > 0 && !String.IsNullOrEmpty(ModelViewerScene.currentAnimName))
				{
					Animation anim = ModelViewerScene.currentModel.GetComponent<Animation>();
					if (anim != null && !anim.IsPlaying(ModelViewerScene.currentAnimName))
					{
						anim.Play(ModelViewerScene.currentAnimName);
						if (anim[ModelViewerScene.currentAnimName] != null)
							anim[ModelViewerScene.currentAnimName].speed = ModelViewerScene.speedFactor;
					}
				}
				if (Input.mouseScrollDelta.y != 0f)
				{
					if (Input.mouseScrollDelta.y > 0f)
						ModelViewerScene.scaleFactor *= 1f + 0.05f * Input.mouseScrollDelta.y;
					else
						ModelViewerScene.scaleFactor /= 1f - 0.05f * Input.mouseScrollDelta.y;
					ModelViewerScene.currentModel.transform.localScale = ModelViewerScene.scaleFactor;
				}
				if (Input.GetMouseButton(0))
				{
					if (mouseLeftWasPressed)
					{
						Vector3 mouseDelta = Input.mousePosition - ModelViewerScene.mousePreviousPosition;
						if (Math.Abs(mouseDelta.x) >= Math.Abs(mouseDelta.y))
						{
							ModelViewerScene.currentModel.transform.localRotation *= Quaternion.Euler(0f, mouseDelta.x, 0f);
						}
						else
						{
							Quaternion angles = ModelViewerScene.currentModel.transform.localRotation;
							Single angley = angles.eulerAngles[1];
							Single factorx = -(Single)Math.Cos(Math.PI * angley / 180f);
							Single factorz = -(Single)Math.Sin(Math.PI * angley / 180f);
							Quaternion performedRot = Quaternion.Euler(factorx * mouseDelta.y, 0f, factorz * mouseDelta.y);
							Single horizontalFactor = (angles * performedRot * Vector3.up).y;
							if (horizontalFactor > 0.5f)
								ModelViewerScene.currentModel.transform.localRotation *= performedRot;
						}
					}
					ModelViewerScene.mouseLeftPressed = true;
				}
				if (Input.GetMouseButton(1))
				{
					if (mouseRightWasPressed)
					{
						Vector3 mouseDelta = Input.mousePosition - ModelViewerScene.mousePreviousPosition;
						ModelViewerScene.currentModel.transform.localPosition -= 0.5f * mouseDelta.y * Vector3.up;
					}
					ModelViewerScene.mouseRightPressed = true;
				}
				if (Input.GetKeyDown(KeyCode.E) && ModelViewerScene.animList.Count > 0)
				{
					List<String> exportedAnims;
					if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
						exportedAnims = ModelViewerScene.animList;
					else
						exportedAnims = new List<String>() { ModelViewerScene.animList[ModelViewerScene.currentAnimIndex] };
					ExportAnimation(exportedAnims);
				}
				UpdateRender();
				ModelViewerScene.mousePreviousPosition = Input.mousePosition;
			}
			catch (Exception err)
			{
				ModelViewerScene.isLoadingModel = false;
				Log.Error(err);
			}
		}

		public static void UpdateRender()
		{
			Int32 boneCount = 0;
			Int32 boneConnectionCount = 0;
			Int32 boneDialogCount = 0;
			Camera camera = GetCamera();
			if (ModelViewerScene.currentModel != null && ModelViewerScene.currentModelBones != null)
			{
				foreach (BoneHierarchyNode bone in ModelViewerScene.currentModelBones)
				{
					if (ModelViewerScene.displayBoneNames)
					{
						Vector3 dialPos = camera.WorldToScreenPoint(bone.Position) / 4; // TODO: fix offsetting
						dialPos.y = camera.pixelHeight / 4 - dialPos.y;
						dialPos.x *= 5f / 4f;
						dialPos.y *= 5f / 4f;
						while (boneDialogCount >= ModelViewerScene.boneDialogs.Count)
							ModelViewerScene.boneDialogs.Add(Singleton<DialogManager>.Instance.AttachDialog($"[STRT=20,1][IMME][NFOC]{bone.Id}[ENDN]", 20, 1, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStyleTransparent, dialPos, Dialog.CaptionType.None));
						Dialog dialog = ModelViewerScene.boneDialogs[boneDialogCount];
						dialog.Position = dialPos; // TODO: updating position is not currently working
						boneDialogCount++;
					}
					if (ModelViewerScene.displayBones)
					{
						while (boneCount >= ModelViewerScene.boneModels.Count)
							ModelViewerScene.boneModels.Add(CreateModelForBone());
						ModelViewerScene.boneModels[boneCount].transform.position = bone.Position;
						boneCount++;
					}
					if (ModelViewerScene.displayBoneConnections && bone.Parent != null)
					{
						while (boneConnectionCount >= ModelViewerScene.boneConnectModels.Count)
							ModelViewerScene.boneConnectModels.Add(CreateModelForBoneConnection());
						MoveBoneConnection(ModelViewerScene.boneConnectModels[boneConnectionCount], bone.Parent.Position, bone.Position);
						boneConnectionCount++;
					}
				}
			}
			for (Int32 i = boneCount; i < ModelViewerScene.boneModels.Count; i++)
				UnityEngine.Object.Destroy(ModelViewerScene.boneModels[i]);
			for (Int32 i = boneConnectionCount; i < ModelViewerScene.boneConnectModels.Count; i++)
				UnityEngine.Object.Destroy(ModelViewerScene.boneConnectModels[i]);
			for (Int32 i = boneDialogCount; i < ModelViewerScene.boneDialogs.Count; i++)
				ModelViewerScene.boneDialogs[i].ForceClose();
			if (boneCount < ModelViewerScene.boneModels.Count)
				ModelViewerScene.boneModels.RemoveRange(boneCount, ModelViewerScene.boneModels.Count - boneCount);
			if (boneConnectionCount < ModelViewerScene.boneConnectModels.Count)
				ModelViewerScene.boneConnectModels.RemoveRange(boneConnectionCount, ModelViewerScene.boneConnectModels.Count - boneConnectionCount);
			if (boneDialogCount < ModelViewerScene.boneDialogs.Count)
				ModelViewerScene.boneDialogs.RemoveRange(boneDialogCount, ModelViewerScene.boneDialogs.Count - boneDialogCount);
			if (ModelViewerScene.displayModelAnimNames)
			{
				if (ModelViewerScene.modelAnimDialog == null)
					ModelViewerScene.modelAnimDialog = Singleton<DialogManager>.Instance.AttachDialog($"[STRT=200,2][IMME][NFOC]{ModelViewerScene.geoList[ModelViewerScene.currentGeoIndex]}\n{ModelViewerScene.currentAnimName}[ENDN]", 200, 2, Dialog.TailPosition.Center, Dialog.WindowStyle.WindowStyleTransparent, new Vector2(1f, 1f), Dialog.CaptionType.None);
			}
			else if (ModelViewerScene.modelAnimDialog != null)
			{
				ModelViewerScene.modelAnimDialog.ForceClose();
				ModelViewerScene.modelAnimDialog = null;
			}
		}

		private static Camera GetCamera()
		{
			return GameObject.Find("FieldMap Camera").GetComponent<Camera>();
		}

		private static List<String> GetAnimationsOfModel(String model)
		{
			List<String> result = new List<String>();
			String identifier = model.Substring(4);
			foreach (KeyValuePair<Int32, String> anim in FF9DBAll.AnimationDB)
				if (anim.Value.Substring(4).StartsWith(identifier))
					result.Add(anim.Value);
			Log.Message($"[ModelViewerScene] Animation set: {String.Join(", ", result.ToArray())}");
			return result;
		}

		private static void ChangeModel(Int32 index)
		{
			ModelViewerScene.isLoadingModel = true;
			while (index < 0)
				index += ModelViewerScene.geoList.Count;
			while (index >= ModelViewerScene.geoList.Count)
				index -= ModelViewerScene.geoList.Count;
			ModelViewerScene.currentGeoIndex = index;
			ModelViewerScene.animList = GetAnimationsOfModel(ModelViewerScene.geoList[index]);
			if (currentModel != null)
				UnityEngine.Object.Destroy(currentModel);
			Log.Message($"[ModelViewerScene] Change model: {ModelViewerScene.geoList[index]}");
			ModelViewerScene.currentModel = ModelFactory.CreateModel(ModelViewerScene.geoList[index]);
			ModelViewerScene.currentModelBones = null;
			if (ModelViewerScene.modelAnimDialog != null)
			{
				ModelViewerScene.modelAnimDialog.ForceClose();
				ModelViewerScene.modelAnimDialog = null;
			}
			if (ModelViewerScene.currentModel != null)
			{
				if (ModelFactory.garnetShortHairTable.Contains(ModelViewerScene.geoList[index]))
				{
					Boolean garnetShortHair =  ModelViewerScene.geoList[index] == "GEO_MAIN_F1_GRN"
											|| ModelViewerScene.geoList[index] == "GEO_MAIN_B0_004"
											|| ModelViewerScene.geoList[index] == "GEO_MAIN_B0_005"
											|| ModelViewerScene.geoList[index] == "GEO_MON_B3_169"
											|| ModelViewerScene.geoList[index] == "GEO_MAIN_B0_026"
											|| ModelViewerScene.geoList[index] == "GEO_MAIN_B0_027";
					Renderer[] longHairRenderers = ModelViewerScene.currentModel.transform.GetChildByName("long_hair").GetComponentsInChildren<Renderer>();
					Renderer[] shortHairRenderers = ModelViewerScene.currentModel.transform.GetChildByName("short_hair").GetComponentsInChildren<Renderer>();
					foreach (Renderer renderer in longHairRenderers)
						renderer.enabled = !garnetShortHair;
					foreach (Renderer renderer in shortHairRenderers)
						renderer.enabled = garnetShortHair;
				}
				ModelViewerScene.currentModel.transform.position = Vector3.zero;
				ModelViewerScene.currentModel.transform.localScale = ModelViewerScene.scaleFactor;
				ModelViewerScene.currentModel.transform.localRotation = Quaternion.Euler(45f, 0f, 0f);
				foreach (String anim in ModelViewerScene.animList)
					AnimationFactory.AddAnimWithAnimatioName(ModelViewerScene.currentModel, anim);
				ModelViewerScene.currentAnimIndex = 0;
				ModelViewerScene.currentAnimName = ModelViewerScene.animList.Count > 0 ? ModelViewerScene.animList[0] : "";
				ModelViewerScene.currentModelBones = BoneHierarchyNode.CreateFromModel(ModelViewerScene.currentModel);
			}
			ModelViewerScene.isLoadingModel = false;
		}

		private static void ChangeAnimation(Int32 index)
		{
			if (ModelViewerScene.animList.Count == 0)
				return;
			while (index < 0)
				index += ModelViewerScene.animList.Count;
			while (index >= ModelViewerScene.animList.Count)
				index -= ModelViewerScene.animList.Count;
			ModelViewerScene.currentAnimIndex = index;
			ModelViewerScene.currentAnimName = ModelViewerScene.animList[index];
			Animation anim = ModelViewerScene.currentModel.GetComponent<Animation>();
			if (anim != null)
			{
				anim.Play(currentAnimName);
				anim[currentAnimName].speed = ModelViewerScene.speedFactor;
			}
			if (ModelViewerScene.modelAnimDialog != null)
			{
				ModelViewerScene.modelAnimDialog.ForceClose();
				ModelViewerScene.modelAnimDialog = null;
			}
		}

		private static GameObject CreateModelForBone()
		{
			Mesh mesh = new Mesh();
			Single radius = 2f;
			Int32 vertCount = 20;
			Vector3[] meshVert = new Vector3[vertCount];
			for (Int32 i = 0; i < vertCount; i++)
			{
				Double angle = Math.PI * 2 * i / vertCount;
				meshVert[i] = new Vector3((Single)(radius * Math.Cos(angle)), (Single)(radius * Math.Sin(angle)), 0f);
			}
			Int32[] meshIndex = new Int32[3 * (vertCount - 2)];
			for (Int32 i = 0; i + 2 < vertCount; i++)
			{
				meshIndex[3 * i] = 0;
				meshIndex[3 * i + 1] = i + 1;
				meshIndex[3 * i + 2] = i + 2;
			}
			mesh.vertices = meshVert;
			mesh.uv = null;
			mesh.triangles = meshIndex;
			GameObject go = ModelFactory.CreateModel("GEO_ACC_F0_BON");
			go.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh = mesh;
			go.GetComponentInChildren<SkinnedMeshRenderer>().material.shader = Shader.Find("SFX_RUSH_ADD");
			return go;
		}

		private static GameObject CreateModelForBoneConnection()
		{
			Mesh mesh = new Mesh();
			Vector3[] meshVert = new Vector3[4];
			Int32[] meshIndex = new Int32[]
			{
				0, 1, 2,
				1, 2, 3
			};
			mesh.vertices = meshVert;
			mesh.uv = null;
			mesh.triangles = meshIndex;
			GameObject go = ModelFactory.CreateModel("GEO_ACC_F0_BON");
			go.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh = mesh;
			go.GetComponentInChildren<SkinnedMeshRenderer>().material.shader = Shader.Find("SFX_RUSH_ADD");
			go.transform.localRotation = Quaternion.Euler(0f, 180f, -90f);
			return go;
		}

		private static void MoveBoneConnection(GameObject connectionMesh, Vector3 parentPos, Vector3 childPos)
		{
			Mesh mesh = connectionMesh.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
			Vector3[] meshVert = new Vector3[4];
			meshVert[0] = parentPos + new Vector3(0, 1f, 0f);
			meshVert[1] = parentPos + new Vector3(0, -1f, 0f);
			meshVert[2] = childPos + new Vector3(0, 2f, 0f);
			meshVert[3] = childPos + new Vector3(0, -2f, 0f);
			mesh.vertices = meshVert;
		}

		private static void ExportAnimation(List<String> animNameList)
		{
			String exportConf = "MemoriaExportAnim.txt";
			if (File.Exists(exportConf))
			{
				if (ProcessExportAnimation(File.ReadAllText(exportConf)))
					File.Delete(exportConf);
			}
			else
			{
				if (GenerateExportAnimationFile(exportConf, animNameList))
					Process.Start(Path.GetFullPath(exportConf));
			}
		}

		private static Boolean GenerateExportAnimationFile(String filepath, List<String> animNameList)
		{
			String[] animNameToken = animNameList[0].Split('_');
			String animModelName = "GEO_" + animNameToken[1] + "_" + animNameToken[2] + "_" + animNameToken[3];
			String assetPath = "Animations/" + animModelName + "/" + animNameList[0];
			assetPath = AnimationFactory.GetRenameAnimationPath(assetPath);
			AnimationClipReader clipIO;
			AnimationClipReader.ReadAnimationClipFromDisc("StreamingAssets/Assets/Resources/" + assetPath + ".anim", out clipIO);
			if (clipIO == null)
			{
				FF9Sfx.FF9SFX_Play(102);
				return false;
			}
			String config = "// EXPORT ANIMATION\n";
			config += "// Write the configurations for exporting an animation, then save the file and press \"E\" again on the model viewer\n\n";
			foreach (String animName in animNameList)
				config += $"Animation:{animName}\n";
			foreach (String animName in animNameList)
			{
				animNameToken = animName.Split('_');
				animModelName = "GEO_" + animNameToken[1] + "_" + animNameToken[2] + "_" + animNameToken[3];
				assetPath = "Animations/" + animModelName + "/" + animName;
				assetPath = AnimationFactory.GetRenameAnimationPath(assetPath);
				config += $"ExportPath:StreamingAssets/Assets/Resources/{assetPath}.anim\n";
			}
			config += $"DeleteThisOnSuccess:true\n";
			config += $"Reverse:false\n";
			config += $"//AnimPatch:bone000;localRotation;Euler(0, 0, 0)\n";
			config += $"//AnimPatch:bone000;localRotation;Quaternion(0, 0, 0, 1)\n";
			config += $"//AnimPatch:bone000;localPosition;(0, 0, 0)\n";
			config += $"//AnimPatch:bone000;localScale;(1, 1, 1)\n";
			config += $"FrameRate:{clipIO.frameRate}\n";
			config += $"BoneHierarchy:\n";
			foreach (var bone in clipIO.boneAnimList)
				config += $"{bone.boneNameInHierarchy}\n";
			File.WriteAllText(filepath, config);
			return true;
		}

		private static Boolean ProcessExportAnimation(String config)
		{
			Dictionary<String, Dictionary<String, Vector4>> animPatch = new Dictionary<String, Dictionary<String, Vector4>>();
			List<String> animNameList = new List<String>();
			List<String> outputPathList = new List<String>();
			Boolean deleteConfig = true;
			Boolean reverseAnim = false;
			Single frameRate = 60f;
			List<String> boneHierarchy = null;
			Boolean readingBoneHierarchy = false;
			String[] lines = config.Split('\n');
			foreach (String ntline in lines)
			{
				String line = ntline.Trim();
				if (readingBoneHierarchy)
				{
					if (String.IsNullOrEmpty(line))
						boneHierarchy.Add("");
					else if (!line.StartsWith("bone"))
						readingBoneHierarchy = false;
					else
						boneHierarchy.Add(line);
				}
				if (!readingBoneHierarchy)
				{
					if (line.StartsWith("Animation:"))
						animNameList.Add(line.Substring("Animation:".Length));
					else if (line.StartsWith("ExportPath:"))
						outputPathList.Add(line.Substring("ExportPath:".Length));
					else if (line.StartsWith("DeleteThisOnSuccess:"))
						deleteConfig = Boolean.Parse(line.Substring("DeleteThisOnSuccess:".Length));
					else if (line.StartsWith("FrameRate:"))
						frameRate = Single.Parse(line.Substring("FrameRate:".Length));
					else if (line.StartsWith("Reverse:"))
						reverseAnim = Boolean.Parse(line.Substring("Reverse:".Length));
					else if (line.StartsWith("AnimPatch:"))
					{
						String[] param = line.Substring("AnimPatch:".Length).Split(';');
						if (param.Length != 3)
							continue;
						Dictionary<String, Vector4> baPatch;
						if (!animPatch.TryGetValue(param[0], out baPatch))
						{
							baPatch = new Dictionary<String, Vector4>();
							animPatch.Add(param[0], baPatch);
						}
						if (param[1] == "localRotation")
						{
							Quaternion arg;
							if (param[2].StartsWith("Euler"))
								arg = Quaternion.Euler(StringToVector(param[2].Substring("Euler".Length)));
							else if (param[2].StartsWith("Quaternion"))
								arg = VectorToQuaternion(StringToVector(param[2].Substring("Quaternion".Length)));
							else
								arg = VectorToQuaternion(StringToVector(param[2]));
							baPatch[param[1]] = QuaternionToVector(arg);
						}
						else
						{
							baPatch[param[1]] = StringToVector(param[2]);
						}
					}
					else if (line.StartsWith("BoneHierarchy:"))
					{
						readingBoneHierarchy = true;
						boneHierarchy = new List<String>();
					}
				}
			}
			if (animNameList.Count == 0 || outputPathList.Count == 0)
			{
				FF9Sfx.FF9SFX_Play(102);
				return false;
			}
			Boolean allGood = true;
			for (Int32 i = 0; i < animNameList.Count; i++)
			{
				if (i >= outputPathList.Count)
					continue;
				String[] animNameToken = animNameList[i].Split('_');
				String animModelName = "GEO_" + animNameToken[1] + "_" + animNameToken[2] + "_" + animNameToken[3];
				String assetPath = "Animations/" + animModelName + "/" + animNameList[i];
				assetPath = AnimationFactory.GetRenameAnimationPath(assetPath);
				AnimationClipReader clipIO;
				AnimationClipReader.ReadAnimationClipFromDisc("StreamingAssets/Assets/Resources/" + assetPath + ".anim", out clipIO);
				if (clipIO == null)
				{
					allGood = false;
					continue;
				}
				if (reverseAnim)
				{
					Single duration = 0f;
					foreach (var ba in clipIO.boneAnimList)
						foreach (var ta in ba.transformAnimList)
							foreach (var fa in ta.frameAnimList)
								if (duration < fa.time)
									duration = fa.time;
					foreach (var ba in clipIO.boneAnimList)
						foreach (var ta in ba.transformAnimList)
							foreach (var fa in ta.frameAnimList)
								fa.time = duration - fa.time;
				}
				clipIO.frameRate = frameRate;
				if (boneHierarchy != null)
				{
					Int32 lineIndex = 0;
					for (Int32 j = 0; j < clipIO.boneAnimList.Count; j++)
					{
						if (lineIndex >= boneHierarchy.Count || String.IsNullOrEmpty(boneHierarchy[lineIndex]))
							clipIO.boneAnimList.RemoveAt(j--);
						else
							clipIO.boneAnimList[j].boneNameInHierarchy = boneHierarchy[lineIndex];
						lineIndex++;
					}
				}
				foreach (var bonePatch in animPatch)
				{
					var ba = clipIO.boneAnimList.Find(b => b.boneNameInHierarchy == bonePatch.Key);
					if (ba == null)
					{
						ba = new AnimationClipReader.BoneAnimation();
						ba.boneNameInHierarchy = bonePatch.Key;
						clipIO.boneAnimList.Add(ba);
					}
					foreach (var transfPatch in bonePatch.Value)
					{
						var ta = ba.transformAnimList.Find(t => t.transformType == transfPatch.Key);
						if (ta == null)
						{
							ta = new AnimationClipReader.BoneAnimation.TransformAnimation();
							ta.transformType = transfPatch.Key;
							ba.transformAnimList.Add(ta);
						}
						if (ta.frameAnimList.Count == 0)
						{
							var fa = new AnimationClipReader.BoneAnimation.TransformAnimation.FrameAnimation();
							fa.time = 0f;
							fa.pos = transfPatch.Value;
							fa.posInnerTangent = Vector4.zero;
							fa.posOuterTangent = Vector4.zero;
							ta.frameAnimList.Add(fa);
						}
						else
						{
							foreach (var fa in ta.frameAnimList)
							{
								if (transfPatch.Key == "localRotation")
									fa.pos = QuaternionToVector(VectorToQuaternion(fa.pos) * VectorToQuaternion(transfPatch.Value));
								else if (transfPatch.Key == "localPosition")
									fa.pos += transfPatch.Value;
								else if (transfPatch.Key == "localScale")
									fa.pos = new Vector3(fa.pos.x * transfPatch.Value.x, fa.pos.y * transfPatch.Value.y, fa.pos.z * transfPatch.Value.z);
							}
						}
					}
				}
				String outputPath = outputPathList[i];
				if (!String.IsNullOrEmpty(Path.GetDirectoryName(outputPath)))
					Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
				File.WriteAllText(outputPath, clipIO.ParseToJSON());
				AnimationClipReader.LoadedClips.Remove("StreamingAssets/Assets/Resources/" + assetPath + ".anim");
				AnimationClipReader.LoadedClips.Remove(outputPath);
				ModelViewerScene.savedAnimationPath = outputPath;
			}
			if (!allGood)
			{
				FF9Sfx.FF9SFX_Play(102);
				return false;
			}
			return deleteConfig;
		}

		// TODO: maybe add that kind of API somewhere else (ExtensionMethodsVector3?)
		private static Quaternion VectorToQuaternion(Vector4 val)
		{
			return new Quaternion(val.x, val.y, val.z, val.w);
		}
		private static Vector4 QuaternionToVector(Quaternion val)
		{
			return new Vector4(val.x, val.y, val.z, val.w);
		}
		private static Vector4 StringToVector(String val)
		{
			if (val.StartsWith("(") && val.EndsWith(")"))
				val = val.Substring(1, val.Length - 2);
			String[] coordStr = val.Split(',');
			if (coordStr.Length == 0)
				return Vector4.zero;
			Single[] coordNum = new Single[coordStr.Length];
			for (Int32 i = 0; i < coordStr.Length; i++)
				if (!Single.TryParse(coordStr[i], out coordNum[i]))
					return Vector4.zero;
			if (coordNum.Length == 1)
				return new Vector4(coordNum[0], coordNum[0], coordNum[0], coordNum[0]);
			if (coordNum.Length == 2)
				return new Vector4(coordNum[0], coordNum[1], 0f, 0f);
			if (coordNum.Length == 3)
				return new Vector4(coordNum[0], coordNum[1], coordNum[2], 0f);
			return new Vector4(coordNum[0], coordNum[1], coordNum[2], coordNum[3]);
		}

		private static Boolean isLoadingModel;
		private static Boolean mouseLeftPressed;
		private static Boolean mouseRightPressed;
		private static Vector3 mousePreviousPosition;
		private static BoneHierarchyNode currentModelBones;
		private static List<GameObject> boneModels = new List<GameObject>();
		private static List<GameObject> boneConnectModels = new List<GameObject>();
		private static List<Dialog> boneDialogs = new List<Dialog>();
		private static Dialog modelAnimDialog;
	}
}

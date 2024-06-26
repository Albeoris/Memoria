using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using Memoria.Data;
using Memoria.Scripts;
using UnityEngine;
using SimpleJSON;
using FF9;

using TextureKind = PSXTextureMgr.Kind;

public abstract class SFXDataMesh
{
	public static Mesh sharedUnityMesh;
	public static Material sharedUnityMaterial;
	public static Color? SFXColor;
	protected BTL_DATA caster = null;
	protected BTL_DATA target = null;
	protected Vector3 averageTarget = default(Vector3);

	static SFXDataMesh()
	{
		sharedUnityMesh = new Mesh();
		sharedUnityMesh.MarkDynamic();
		sharedUnityMaterial = new Material(ShadersLoader.Find(SFXMesh.shaderNames[0]));
		SFXColor = null;
	}

	public virtual void SetupPositions(BTL_DATA c, BTL_DATA t, Vector3 at)
	{
		caster = c;
		target = t;
		averageTarget = at;
	}

	public abstract void Begin();
	public abstract Boolean Render(Int32 frame, SFXData.RunningInstance run = null);
	public abstract void End();

	public class EffectMaterial
	{
		public UInt32 meshKey = 0;

		public String shaderName = "";
		public Vector4 textureParam = default(Vector4);
		public Color colorIntensity = Color.white;
		public Single threshold = 0.05f;
		public FilterMode filterMode = FilterMode.Point;
		public TextureWrapMode wrapMode = TextureWrapMode.Clamp;

		public TextureKind textureKind = 0;
		public String texturePath = "";
		public Texture texture = null;
		public List<TextureChanger> textureChanger = new List<TextureChanger>();
		public Int32 textureFrameLoaded = -1;

		public List<BackgroundCaptureParam> backgroundCapture = new List<BackgroundCaptureParam>();
		public Int32 backgroundCaptureLoaded = -1;

		public List<ScreenshotParam> screenshot = new List<ScreenshotParam>();

		public static EffectMaterial LoadMaterial(JSONClass matClass, String defaultFolder)
		{
			if (matClass == null)
				return null;
			EffectMaterial material = new EffectMaterial();
			if (matClass["Key"] != null)
				UInt32.TryParse(matClass["Key"], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out material.meshKey);
			if (matClass["TextureKind"] != null)
				material.textureKind = (TextureKind)matClass["TextureKind"].AsUInt;
			if (matClass["TexturePath"] != null)
			{
				material.texturePath = matClass["TexturePath"].Value;
				if (!material.texturePath.Contains("/"))
					material.texturePath = defaultFolder + "/" + material.texturePath;
				else if (material.texturePath.StartsWith("./"))
					material.texturePath = defaultFolder + material.texturePath.Substring(1);
				material.textureKind = TextureKind.IMAGE;
			}
			if (matClass["GlobalColor"] != null)
				material.colorIntensity = matClass["GlobalColor"].AsVector;
			if (matClass["Threshold"] != null)
				material.threshold = matClass["Threshold"].AsFloat;
			if (matClass["TextureParameter"] != null)
				material.textureParam = matClass["TextureParameter"].AsVector;
			if (matClass["Shader"] != null)
				material.shaderName = matClass["Shader"].Value;
			return material;
		}

		public void StoreInUnityMaterial(Int32 frame, Material mat)
		{
			if (Assets.Scripts.Common.SceneDirector.IsFieldScene())
			{
				Dictionary<String, String> shaderReplacer = new Dictionary<String, String>
				{
					{ "SFX_OPA_GT", "Unlit/Transparent Cutout" },
					{ "SFX_ADD_GT", "Unlit/Transparent Cutout" },
					{ "SFX_SUB_GT", "Unlit/Transparent Cutout" },
					{ "SFX_OPA_G", "Unlit/Transparent Cutout" },
					{ "SFX_ADD_G", "Unlit/Transparent Cutout" },
					{ "SFX_SUB_G", "Unlit/Transparent Cutout" },
				};
				if (shaderReplacer.TryGetValue(shaderName, out String rep))
					shaderName = rep;
			}
			mat.shader = ShadersLoader.Find(shaderName);
			if (SFXDataMesh.SFXColor.HasValue)
				mat.SetColor(SFXMesh.Color, colorIntensity * SFXDataMesh.SFXColor.Value);
			else
				mat.SetColor(SFXMesh.Color, colorIntensity);
			mat.SetFloat(SFXMesh.Threshold, threshold);
			for (Int32 i = 0; i < textureChanger.Count; i++)
				if (frame >= textureChanger[i].frame && textureFrameLoaded < textureChanger[i].frame)
				{
					textureKind = textureChanger[i].textureKind;
					texturePath = textureChanger[i].texturePath;
					texture = textureChanger[i].texture;
					textureFrameLoaded = textureChanger[i].frame;
				}
			if (textureKind == TextureKind.BACKGROUND)
				for (Int32 i = backgroundCapture.Count - 1; i >= 0; i--)
					if (frame >= backgroundCapture[i].frame && backgroundCaptureLoaded < backgroundCapture[i].frame)
					{
						texture = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
						RenderTexture renderTexture = texture as RenderTexture;
						renderTexture.enableRandomWrite = false;
						renderTexture.wrapMode = TextureWrapMode.Clamp;
						renderTexture.filterMode = FilterMode.Bilinear;
						renderTexture.Create();
						RenderTexture active = RenderTexture.active;
						RenderTexture.active = renderTexture;
						PSXTextureMgr.CaptureBG(backgroundCapture[i].screen, backgroundCapture[i].offset);
						RenderTexture.active = active;
						backgroundCaptureLoaded = backgroundCapture[i].frame;
						break;
					}
			if (textureKind == TextureKind.SCREENSHOT)
				for (Int32 i = screenshot.Count - 1; i >= 0 ; i--)
					if (frame >= screenshot[i].frame)
					{
						SFXMeshBase.ssOffsetX = (Int32)screenshot[i].screen.x;
						SFXMeshBase.ssOffsetY = (Int32)screenshot[i].screen.y;
						new SFXScreenShot().Render(0);
						texture = UnityEngine.Object.Instantiate(SFXScreenShot.screenshot);
						break;
					}
			if (textureKind != TextureKind.NONE)
			{
				if (texture == null)
				{
					if (textureKind == TextureKind.IMAGE)
						texture = AssetManager.Load<Texture2D>(texturePath);
					else if (textureKind == TextureKind.BLUR)
						texture = PSXTextureMgr.blurTexture;
					else if (textureKind == TextureKind.BACKGROUND)
						texture = PSXTextureMgr.bgTexture;
					else if (textureKind == TextureKind.GENERATED)
						texture = PSXTextureMgr.genTexture;
					else if (textureKind == TextureKind.SCREENSHOT)
						texture = SFXScreenShot.screenshot;
				}
				mat.mainTexture = texture;
				if (mat.mainTexture == null)
					return;
				mat.mainTexture.filterMode = filterMode;
				mat.mainTexture.wrapMode = wrapMode;
				mat.SetVector(SFXMesh.TexParam, textureParam);
			}
			else
			{
				mat.mainTexture = null;
			}
		}

		public void ConvertFromSFXMesh(SFXMesh sfxmesh)
		{
			meshKey = sfxmesh._key;
			if (SFXKey.IsTexture(meshKey))
			{
				texture = sfxmesh.GetTexture(out textureKind);
				Boolean isNecronSpecialTexture = SFX.currentEffectID == SpecialEffect.Special_Necron_Death && sfxmesh._constTexParam.w < 0;
				if (textureKind == TextureKind.IMAGE && !isNecronSpecialTexture)
					texture = UnityEngine.Object.Instantiate(texture);
				textureParam = sfxmesh._constTexParam;
				UInt32 filter = SFXKey.GetFilter(meshKey);
				if (filter == SFXKey.FILLTER_POINT)
					filterMode = FilterMode.Point;
				else if (filter == SFXKey.FILLTER_BILINEAR)
					filterMode = FilterMode.Bilinear;
				else
					filterMode = (!SFX.isDebugFillter) ? FilterMode.Point : FilterMode.Bilinear;
				wrapMode = TextureWrapMode.Clamp;
			}
			else if (SFXScreenShot.IsSpecialSlowTexture(meshKey))
			{
				textureKind = TextureKind.IMAGE;
				texture = UnityEngine.Object.Instantiate(PSXTextureMgr.GetTexture(1, 1, 8, 247, 0).texture);
				textureParam = new Vector4(SFXMesh.HALF_PIXEL, SFXMesh.HALF_PIXEL, 256f, 256f);
				filterMode = FilterMode.Point;
				wrapMode = TextureWrapMode.Clamp;
			}
			else
			{
				textureKind = TextureKind.NONE;
			}
			shaderName = SFXMesh.shaderNames[sfxmesh._shaderIndex];
			colorIntensity = SFXMesh.ColorData[SFX.colIntensity];
			threshold = SFX.colThreshold != 0 ? 0.05f : 0.0295f;
		}

		public void AddTextureChanger(Int32 f, TextureKind tk, String tp, Texture t = null)
		{
			TextureChanger changer = new TextureChanger();
			changer.frame = f;
			changer.textureKind = tk;
			changer.texturePath = tp;
			changer.texture = t;
			for (Int32 i = 0; i < textureChanger.Count; i++)
			{
				if (textureChanger[i].frame == f)
				{
					textureChanger[i] = changer;
					return;
				}
				else if (textureChanger[i].frame > f)
				{
					textureChanger.Insert(i, changer);
					return;
				}
			}
			textureChanger.Add(changer);
		}

		public void PushBackgroundCapture(Int32 f)
		{
			if (backgroundCapture.Count > 0 && backgroundCapture[backgroundCapture.Count - 1].frame == f)
				return;
			BackgroundCaptureParam bgCapture = new BackgroundCaptureParam();
			bgCapture.frame = f;
			bgCapture.screen.x = -(PSXTextureMgr.bgParam[2] >> 1) - (PSXTextureMgr.bgParam[0] & 63);
			bgCapture.screen.y = (PSXTextureMgr.bgParam[3] >> 1) + (PSXTextureMgr.bgParam[1] & 255);
			bgCapture.offset.x = -PSXTextureMgr.bgParam[4];
			bgCapture.offset.y = PSXTextureMgr.bgParam[5];
			bgCapture.offset.z = -(10496 + PSXTextureMgr.bgParam[6]);
			backgroundCapture.Add(bgCapture);
		}

		public void PushScreenshot(Int32 f)
		{
			if (screenshot.Count > 0 && screenshot[screenshot.Count - 1].frame == f)
				return;
			ScreenshotParam scr = new ScreenshotParam();
			scr.frame = f;
			scr.screen.x = SFXMeshBase.ssOffsetX;
			scr.screen.y = SFXMeshBase.ssOffsetY;
			screenshot.Add(scr);
		}

		public class TextureChanger
		{
			public Int32 frame = 0;
			public TextureKind textureKind = 0;
			public String texturePath = "";
			public Texture texture = null;
		}

		public class BackgroundCaptureParam
		{
			public Int32 frame = 0;
			public Vector2 screen = default(Vector2);
			public Vector3 offset = default(Vector3);
		}

		public class ScreenshotParam
		{
			public Int32 frame = 0;
			public Vector2 screen = default(Vector2);
		}
	}

	public class Raw : SFXDataMesh
	{
		public Dictionary<EffectMaterial, RMesh> data = new Dictionary<EffectMaterial, RMesh>();
		public List<KeyValuePair<EffectMaterial, RMesh>> genTextureMesh = new List<KeyValuePair<EffectMaterial, RMesh>>();
		public RenderTexture genTexture = null;
		public Int32 firstFrame = Int32.MaxValue;
		public Int32 lastFrame = -1;

		public static Int32 RenderingCount;

		public override void Begin()
		{
			if (genTextureMesh.Count > 0)
			{
				genTexture = new RenderTexture(PSXTextureMgr.GEN_TEXTURE_W, PSXTextureMgr.GEN_TEXTURE_H, 0, RenderTextureFormat.RGB565);
				genTexture.enableRandomWrite = false;
				genTexture.wrapMode = TextureWrapMode.Clamp;
				genTexture.filterMode = FilterMode.Bilinear;
				genTexture.Create();
			}
			Raw.RenderingCount++;
		}
		public override Boolean Render(int frame, SFXData.RunningInstance run = null)
		{
			Camera camera = Camera.main ? Camera.main : GameObject.Find("Battle Camera")?.GetComponent<BattleMapCameraController>()?.GetComponent<Camera>();
			if (camera == null)
				camera = GameObject.Find("FieldMap Camera")?.GetComponent<Camera>();
			Matrix4x4 worldToCameraMatrix = camera.worldToCameraMatrix;
			RenderTexture activeRender = RenderTexture.active;
			Boolean renderSomething = false;
			if (genTextureMesh.Count > 0)
			{
				SFXRenderTextureBegin.PrepareCamera();
				RenderTexture.active = genTexture;
				foreach (KeyValuePair<EffectMaterial, RMesh> p in genTextureMesh)
				{
					RMesh.Frame meshFrame;
					if (!p.Value.raw.TryGetValue(frame, out meshFrame))
						continue;
					meshFrame.Render(frame, p.Key, p.Value.isPolyline, run);
				}
				SFX.ResetViewPort();
			}
			RenderTexture.active = null;
			//camera.worldToCameraMatrix = Matrix4x4.identity;
			for (Int32 priority = 0; priority < 10; priority++)
				foreach (KeyValuePair<EffectMaterial, RMesh> p in data)
				{
					RMesh.Frame meshFrame;
					if (!p.Value.raw.TryGetValue(frame, out meshFrame))
						continue;
					if (meshFrame.renderPriority != priority)
						continue;
					if (run != null)
					{
						if (!run.meshKeyList.Contains(p.Key.meshKey))
						{
							if (run.preventedMeshIndices.Contains((UInt32)run.meshKeyList.Count))
								run.preventedMeshKeys.Add(p.Key.meshKey);
							if (run.coloredMeshIndices.TryGetValue((UInt32)run.meshKeyList.Count, out Color color))
								run.coloredMeshes[p.Key.meshKey] = color;
							run.meshKeyList.Add(p.Key.meshKey);
						}
						if (run.preventedMeshKeys.Contains(p.Key.meshKey))
							continue;
					}
					if (p.Key.textureKind == TextureKind.GENERATED)
						p.Key.AddTextureChanger(frame, TextureKind.IMAGE, "", genTexture);
					meshFrame.Render(frame, p.Key, p.Value.isPolyline, run);
					renderSomething = true;
				}
			RenderTexture.active = activeRender;
			camera.worldToCameraMatrix = worldToCameraMatrix;
			if (run != null && run.cancel && !renderSomething)
				return true;
			return frame > lastFrame;
		}
		public override void End()
		{
			if (genTexture != null)
				UnityEngine.Object.Destroy(genTexture);
			genTexture = null;
			Raw.RenderingCount--;
		}

		public static Raw LoadSFXMesh(String jsonText, String defaultFolder)
		{
			JSONNode root = JSONNode.Parse(jsonText);
			if (root == null || root["Object"] == null)
				return null;
			return LoadSFXMesh(root["Object"] as JSONArray, defaultFolder);
		}

		public static Raw LoadSFXMesh(Byte[] raw, String defaultFolder)
		{
			JSONNode root = JSONClass.Deserialize(new BinaryReader(new MemoryStream(raw)));
			if (root == null || root["Object"] == null)
				return null;
			return LoadSFXMesh(root["Object"] as JSONArray, defaultFolder);
		}

		public static Raw LoadSFXMesh(JSONArray meshArray, String defaultFolder)
		{
			if (meshArray == null)
				return null;
			Raw rawMesh = new Raw();
			foreach (JSONNode meshNode in meshArray)
			{
				if (meshNode["Mesh"] == null || meshNode["Mesh"] is not JSONArray)
					continue;
				EffectMaterial material = new EffectMaterial();
				if (meshNode["Material"] != null)
					material = EffectMaterial.LoadMaterial(meshNode["Material"] as JSONClass, defaultFolder);
				RMesh mesh = new RMesh();
				rawMesh.data[material] = mesh;
				if (meshNode["Movement"] != null)
					mesh.position.LoadFromJSON(meshNode["Movement"]);
				foreach (JSONNode frameNode in meshNode["Mesh"] as JSONArray)
				{
					Int32 time;
					if (frameNode["Frame"] != null)
						Int32.TryParse(frameNode["Frame"], out time);
					else
						continue;
					rawMesh.firstFrame = Math.Min(rawMesh.firstFrame, time);
					rawMesh.lastFrame = Math.Max(rawMesh.lastFrame, time);
					RMesh.Frame frame = new RMesh.Frame();
					mesh.raw[time] = frame;
					if (meshNode["RenderPriority"] != null)
						frame.renderPriority = meshNode["RenderPriority"].AsInt;
					JSONArray vertArray = frameNode["Vertices"] as JSONArray;
					JSONArray colorArray = frameNode["Colors"] as JSONArray;
					JSONArray uvArray = frameNode["UV"] as JSONArray;
					JSONArray indexArray = frameNode["Indices"] as JSONArray;
					Int32 vCount = 0, cCount = 0, uvCount = 0, iCount = 0;
					if (vertArray != null) vCount = vertArray.Count;
					if (colorArray != null) cCount = colorArray.Count;
					if (uvArray != null) uvCount = uvArray.Count;
					if (indexArray != null) iCount = indexArray.Count;
					frame.vertex = new Vector3[vCount];
					frame.color = new Color32[cCount];
					frame.uv = new Vector2[uvCount];
					frame.index = new Int32[iCount];
					for (Int32 i = 0; i < vCount; i++)
						frame.vertex[i] = vertArray[i].AsVector;
					for (Int32 i = 0; i < cCount; i++)
						frame.color[i] = (Color)colorArray[i].AsVector;
					for (Int32 i = 0; i < uvCount; i++)
						frame.uv[i] = uvArray[i].AsVector;
					for (Int32 i = 0; i < iCount; i++)
						frame.index[i] = indexArray[i].AsInt;
				}
			}
			if (rawMesh.firstFrame == Int32.MaxValue)
				rawMesh.firstFrame = 0;
			if (rawMesh.lastFrame == -1)
				rawMesh.lastFrame = rawMesh.firstFrame;
			return rawMesh;
		}

		public class RMesh
		{
			public Dictionary<Int32, Frame> raw = new Dictionary<Int32, Frame>();
			public ParametricMovement position = new ParametricMovement(); // TODO: use it
			public Boolean isPolyline = false;
			public Boolean hasColor = true;
			public Boolean hasUV = true;

			public class Frame
			{
				public Int32 renderPriority; // Should be between 0 (rendered first) and 9 (rendered last)
				public Vector3[] vertex;
				public Color32[] color;
				public Vector2[] uv;
				public Int32[] index;

				public void Render(Int32 frame, EffectMaterial material, Boolean isPolyline, SFXData.RunningInstance run)
				{
					sharedUnityMesh.Clear();
					sharedUnityMesh.vertices = vertex;
					sharedUnityMesh.colors32 = color;
					sharedUnityMesh.uv = uv.Length > 0 ? uv : null;
					if (isPolyline)
						sharedUnityMesh.SetIndices(index, MeshTopology.Lines, 0);
					else
						sharedUnityMesh.triangles = index;
					SFXDataMesh.SFXColor = run?.TryGetCustomColor(material.meshKey);
					material.StoreInUnityMaterial(frame, sharedUnityMaterial);
					sharedUnityMaterial.SetPass(0);
					Graphics.DrawMeshNow(sharedUnityMesh, Matrix4x4.identity);
				}
			}

			public void ConvertFromSFXMesh(Int32 frame, SFXMesh sfxmesh)
			{
				Frame sfxframe = new Frame();
				Boolean isSpecialSlowTexture = SFXScreenShot.IsSpecialSlowTexture(sfxmesh._key);
				hasUV = SFXKey.IsTexture(sfxmesh._key) || isSpecialSlowTexture;
				sfxframe.renderPriority = SFXRender.GetRenderPriority(sfxmesh._key);
				sfxframe.vertex = new Vector3[sfxmesh.VbOffset];
				sfxframe.color = new Color32[sfxmesh.VbOffset];
				sfxframe.uv = new Vector2[hasUV ? sfxmesh.VbOffset : 0];
				sfxframe.index = new Int32[sfxmesh.IbOffset];
				Array.Copy(sfxmesh.VbPos, sfxframe.vertex, sfxframe.vertex.Length);
				Array.Copy(sfxmesh.VbCol, sfxframe.color, sfxframe.color.Length);
				if (isSpecialSlowTexture)
					Array.Copy(SFXScreenShot.slowClockUV, sfxframe.uv, Math.Min(SFXScreenShot.slowClockUV.Length, sfxframe.uv.Length));
				else
					Array.Copy(sfxmesh.VbTex, sfxframe.uv, sfxframe.uv.Length);
				Array.Copy(sfxmesh.IbIndex, sfxframe.index, sfxframe.index.Length);
				isPolyline = SFXKey.isLinePolygon(sfxmesh._key);
				hasColor = true;
				raw[frame] = sfxframe;
			}
		}
	}

	public class Runtime : SFXDataMesh
	{
		public Dictionary<UInt32, EffectMaterial> matList = new Dictionary<UInt32, EffectMaterial>();

		public SpecialEffect effNum;
		public CMD_DATA cmd;
		public BTL_VFX_REQ sfxRequest;

		public static Int32 RenderingCount;

		public static Runtime SetupRuntimeMesh(SpecialEffect effNum, CMD_DATA cmd, BTL_VFX_REQ req, out Int32 firstMeshFrame)
		{
			Runtime runtimeRequest = new Runtime();
			runtimeRequest.effNum = effNum;
			runtimeRequest.cmd = cmd;
			runtimeRequest.sfxRequest = req;
			SFXData.BattleCallbackDummyLoadImages = true;
			SFXData.BattleCallbackDummyUpdateVib = false;
			unsafe
			{
				SFX.hijackedCallback = SFXData.BattleCallbackDummy;
			}
			SFXDataCamera.currentCameraEngine = SFXDataCamera.CameraEngine.NONE;
			runtimeRequest.Load(0);
			for (Int32 f = 1; f < 100 && SFX.isRunning; f++)
			{
				SFXDataCamera.SavePluginCamera();
				runtimeRequest.Load(f);
				if (SFX.isUpdated)
				{
					SFX.isUpdated = false;
					SFX.SFX_LateUpdate();
					SFXRender.Update();
					if (effNum == SpecialEffect.Boomerang && SFX.frameIndex == 34)
					{
						firstMeshFrame = SFX.frameIndex - 1;
						SFX.hijackedCallback = null;
						return runtimeRequest;
					}
					if (effNum == SpecialEffect.Special_Necron_Engage && SFX.frameIndex == 26)
					{
						firstMeshFrame = SFX.frameIndex - 1;
						SFX.hijackedCallback = null;
						return runtimeRequest;
					}
					if (SFXRender.commandBuffer.Count > 0)
					{
						firstMeshFrame = SFX.frameIndex - 1;
						SFX.hijackedCallback = null;
						return runtimeRequest;
					}
				}
			}
			firstMeshFrame = 0;
			SFX.hijackedCallback = null;
			return runtimeRequest;
		}

		public void Load(Int32 frame)
		{
			Boolean enterLoop = true;
			if (!SFX.isRunning || SFX.currentEffectID != effNum || SFX.frameIndex > frame)
			{
				if (SFX.currentEffectID != SpecialEffect.Special_No_Effect)
					while (SFX.isRunning)
						SFX.isRunning = SFX.SFX_Update(ref SFX.frameIndex);
				sfxRequest.UpdateTargetAveragePosition();
				SFX.Begin(sfxRequest.flgs, sfxRequest.arg0, sfxRequest.monbone, sfxRequest.trgcpos);
				SFX.SetExe(sfxRequest.exe);
				SFX.SetMExe(sfxRequest.mexe);
				SFX.SetTrg(sfxRequest.trg, sfxRequest.trgno);
				SFX.SetRTrg(sfxRequest.rtrg, sfxRequest.rtrgno);
				SFX.Play(effNum);
				if (frame == 0)
					enterLoop = false;
			}
			if (SFX.frameIndex == frame)
				enterLoop = false;
			if (enterLoop)
			{
				while (SFX.isRunning && SFX.frameIndex < frame)
				{
					if ((sfxRequest.flgs & 1) == 1 && SFX.frameIndex == 0)
						SFX.SetTaskMonsteraStart();
					SFX.isRunning = SFX.SFX_Update(ref SFX.frameIndex);
					if (SFX.isRunning)
					{
						SFX.isUpdated = true;
						PSXTextureMgr.isCaptureBlur = true;
						if (effNum == SpecialEffect.Ark__Full)
						{
							if (SFX.frameIndex == 1004)
								SFX.subOrder = 2;
							if (SFX.frameIndex == 1193)
								SFX.subOrder = 0;
						}
					}
				}
			}
		}

		public override void Begin()
		{
			SFXData.BattleCallbackDummyLoadImages = true;
			SFXData.BattleCallbackDummyUpdateVib = false;
			unsafe
			{
				SFX.hijackedCallback = SFXData.BattleCallbackDummy;
			}
			SFXDataCamera.currentCameraEngine = SFXDataCamera.CameraEngine.SFX_PLUGIN;
			Runtime.RenderingCount++;
		}
		public override Boolean Render(int frame, SFXData.RunningInstance run = null)
		{
			Load(frame);
			if (!SFX.isRunning)
				return true;
			if (SFX.isUpdated)
			{
				SFX.isUpdated = false;
				SFX.SFX_LateUpdate();
				SFXRender.Update();
				if (run != null)
					for (Int32 i = 0; i < SFXRender.commandBuffer.Count; i++)
						if (SFXRender.commandBuffer[i] is SFXMesh)
						{
							SFXMesh mesh = SFXRender.commandBuffer[i] as SFXMesh;
							if (!run.meshKeyList.Contains(mesh._key))
							{
								if (run.preventedMeshIndices.Contains((UInt32)run.meshKeyList.Count))
									run.preventedMeshKeys.Add(mesh._key);
								if (run.coloredMeshIndices.TryGetValue((UInt32)run.meshKeyList.Count, out Color color))
									run.coloredMeshes[mesh._key] = color;
								run.meshKeyList.Add(mesh._key);
							}
						}
			}
			Camera camera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
			Matrix4x4 worldToCameraMatrix = camera.worldToCameraMatrix;
			RenderTexture activeRender = RenderTexture.active;
			RenderTexture.active = null;
			PSXTextureMgr.CaptureBG();
			//camera.worldToCameraMatrix = Matrix4x4.identity;
			for (Int32 i = 0; i < SFXRender.commandBuffer.Count; i++)
			{
				if (SFXRender.commandBuffer[i] is SFXMesh)
				{
					SFXMesh sfxMesh = SFXRender.commandBuffer[i] as SFXMesh;
					if (run == null || !run.preventedMeshKeys.Contains(sfxMesh._key))
					{
						SFXDataMesh.SFXColor = run?.TryGetCustomColor(sfxMesh._key);
						SFXRender.commandBuffer[i].Render(i);
					}
				}
				else
				{
					SFXRender.commandBuffer[i].Render(i);
				}
			}
			RenderTexture.active = activeRender;
			camera.worldToCameraMatrix = worldToCameraMatrix;
			if (run != null && run.cancel && SFXRender.commandBuffer.Count == 0)
				return true;
			return false;
		}
		public override void End()
		{
			if (SFX.isRunning && SFX.currentEffectID == effNum)
			{
				SFXData.BattleCallbackDummyLoadImages = false;
				SFXData.BattleCallbackDummyUpdateVib = false;
				unsafe
				{
					SFX.hijackedCallback = SFXData.BattleCallbackDummy;
				}
				while (SFX.isRunning)
					SFX.isRunning = SFX.SFX_Update(ref SFX.frameIndex);
			}
			SFX.currentEffectID = SpecialEffect.Special_No_Effect;
			SFX.hijackedCallback = null;
			SFXData.lockLoading = false;
			SFXData.LoadCur = null;
			Runtime.RenderingCount--;
		}
	}

	public class JSON : SFXDataMesh
	{
		public List<ModelSequence> model = new List<ModelSequence>();
		public Boolean emit = true;

		public static Int32 RenderingCount;

		public JSON CopyWeak()
		{
			JSON copy = new JSON();
			foreach (ModelSequence copyseq in model)
			{
				ModelSequence seq = new ModelSequence();
				seq.key = copyseq.key;
				seq.minimalDuration = copyseq.minimalDuration;
				seq.firstFrame = copyseq.firstFrame;
				seq.lastFrame = copyseq.lastFrame;
				seq.defaultFolder = copyseq.defaultFolder;
				foreach (ModelSequence.FBX copyfbx in copyseq.fbxList)
				{
					ModelSequence.FBX fbx = new ModelSequence.FBX();
					fbx.fbxPath = copyfbx.fbxPath;
					fbx.animPath = new List<String>(copyfbx.animPath);
					fbx.movement = new ParametricMovement(copyfbx.movement);
					fbx.startFrame = copyfbx.startFrame;
					fbx.endFrame = copyfbx.endFrame;
					fbx.scale = copyfbx.scale;
					seq.fbxList.Add(fbx);
				}
				foreach (ModelSequence.Sprite copysprite in copyseq.spriteList)
				{
					ModelSequence.Sprite sprite = new ModelSequence.Sprite();
					sprite.duration = copysprite.duration;
					sprite.material = copysprite.material;
					sprite.uv = copysprite.uv;
					sprite.vertexColor = copysprite.vertexColor;
					sprite.emission = copysprite.emission;
					sprite.spriteEmissionLink = copysprite.spriteEmissionLink;
					sprite.scaling = copysprite.scaling;
					sprite.uvInterpolateType = copysprite.uvInterpolateType;
					sprite.colorInterpolateType = copysprite.colorInterpolateType;
					sprite.scalingInterpolateType = copysprite.scalingInterpolateType;
					sprite.baseMovement = new ParametricMovement(copysprite.baseMovement);
					sprite.vertex = copysprite.vertex;
					sprite.index = copysprite.index;
					sprite.useScreenSize = copysprite.useScreenSize;
					seq.spriteList.Add(sprite);
				}
				copy.model.Add(seq);
			}
			return copy;
		}

		public override void Begin()
		{
			foreach (ModelSequence mseq in model)
			{
				foreach (ModelSequence.FBX tok in mseq.fbxList)
				{
					tok.unityObject = ModelFactory.CreateModel(tok.fbxPath);
					if (tok.unityObject == null)
						continue;
					Animation component = tok.unityObject.GetComponent<Animation>();
					Int32 animDuration = 0;
					foreach (String anim in tok.animPath)
					{
						String animName = Path.GetFileNameWithoutExtension(anim);
						if (component.GetClip(animName) == null)
						{
							AnimationClip clip = AssetManager.Load<AnimationClip>(anim, false);
							if (anim != null)
							{
								component.AddClip(clip, animName);
								animDuration += GeoAnim.geoAnimGetNumFrames(tok.unityObject, animName);
							}
						}
					}
					tok.unityObject.SetActive(false);
					tok.unityObject.transform.localScale = new Vector3(tok.scale, tok.scale, tok.scale);
					if (tok.endFrame == tok.startFrame)
					{
						tok.endFrame += animDuration;
						mseq.lastFrame = Math.Max(mseq.lastFrame, tok.endFrame);
					}
				}
			}
			JSON.RenderingCount++;
		}
		public override Boolean Render(int frame, SFXData.RunningInstance run = null)
		{
			if (run != null && run.cancel)
				emit = false;
			Boolean ended = true;
			Boolean renderSomething = false;
			for (Int32 modelIndex = 0; modelIndex < model.Count; modelIndex++)
			{
				ModelSequence mseq = model[modelIndex];
				if (run != null)
				{
					if (mseq.key != 0)
					{
						if (run.meshKeyList.Contains(mseq.key))
							run.preventedMeshIndices.Add((UInt32)modelIndex);
					}
					if (run.preventedMeshIndices.Contains((UInt32)modelIndex))
						continue;
				}
				foreach (ModelSequence.FBX tok in mseq.fbxList)
				{
					if (tok.unityObject == null)
						continue;
					if (frame < tok.startFrame || frame >= tok.endFrame)
					{
						tok.unityObject.SetActive(false);
						continue;
					}
					renderSomething = true;
					tok.unityObject.SetActive(true);
					tok.unityObject.transform.position = tok.movement.GetPosition(frame, null, caster, target, averageTarget);
					tok.unityObject.transform.rotation = Quaternion.Euler(0f, 0f, 180f); // TODO: add a way to rotate the model, maybe similar to ParamtricMovement
					if (tok.animPath.Count == 0)
						continue;
					Int32 animIndex = 0;
					Int32 frameCounter = 0;
					Int32[] animMaxFrame = new Int32[tok.animPath.Count];
					for (Int32 i = 0; i < tok.animPath.Count; i++)
						animMaxFrame[i] = Math.Max(0, GeoAnim.geoAnimGetNumFrames(tok.unityObject, Path.GetFileNameWithoutExtension(tok.animPath[i])));
					while (animIndex < tok.animPath.Count && frame >= tok.startFrame + frameCounter + animMaxFrame[animIndex])
						frameCounter += animMaxFrame[animIndex++];
					Int32 animFrame = frame - tok.startFrame - frameCounter;
					if (animIndex >= tok.animPath.Count)
					{
						animIndex = tok.animPath.Count - 1;
						animFrame = animMaxFrame[animIndex];
					}
					String animName = Path.GetFileNameWithoutExtension(tok.animPath[animIndex]);
					AnimationState clipState = tok.unityObject.GetComponent<Animation>()[animName];
					tok.unityObject.GetComponent<Animation>().Play(animName);
					clipState.speed = 0f;
					clipState.time = (Single)animFrame / (Single)animMaxFrame[animIndex] * clipState.length;
					tok.unityObject.GetComponent<Animation>().Sample();
				}
				RenderTexture activeRender = RenderTexture.active;
				RenderTexture.active = null;
				if (run == null || !run.cancel)
					foreach (ModelSequence.Sprite tok in mseq.spriteList)
						foreach (ModelSequence.Sprite.Emission em in tok.emission)
							if (em.frame > tok.lastFrameRendered && em.frame <= frame && (emit || em.frame < mseq.minimalDuration))
								for (Int32 i = 0; i < em.count; i++)
								{
									ModelSequence.Sprite.Particle particle = new ModelSequence.Sprite.Particle(em, tok.baseMovement);
									tok.particle.Add(particle);
									foreach (Int32 link in tok.spriteEmissionLink)
										mseq.spriteList[link].particle.Add(new ModelSequence.Sprite.Particle(particle));
								}
				foreach (ModelSequence.Sprite tok in mseq.spriteList)
				{
					for (Int32 pIndex = 0; pIndex < tok.particle.Count; pIndex++)
						if (frame > tok.particle[pIndex].frameStart + tok.duration)
							tok.particle.RemoveAt(pIndex--);
					if (tok.VertexCount > 0)
					{
						renderSomething = true;
						Vector3[] meshVert = new Vector3[tok.VertexCount];
						Vector2[] meshUV = new Vector2[tok.VertexCount];
						Color32[] meshColor = new Color32[tok.VertexCount];
						Int32[] meshIndex = new Int32[tok.IndexCount];
						Int32 vIndex = 0;
						Int32 iIndex = 0;
						Mesh umeshtmp = new Mesh();
						umeshtmp.MarkDynamic();
						tok.AppendVertices(frame, caster, target, averageTarget, meshVert, meshUV, meshColor, meshIndex, ref vIndex, ref iIndex);
						umeshtmp.vertices = meshVert;
						umeshtmp.uv = tok.material.textureKind != 0 ? meshUV : null;
						umeshtmp.colors32 = meshColor;
						umeshtmp.triangles = meshIndex;
						Material umattmp = new Material(ShadersLoader.Find(tok.material.shaderName));
						SFXDataMesh.SFXColor = run?.TryGetCustomColor(tok.material.meshKey);
						tok.material.StoreInUnityMaterial(frame, umattmp);
						umattmp.SetPass(0);
						Graphics.DrawMeshNow(umeshtmp, Matrix4x4.identity);
					}
					tok.lastFrameRendered = frame;
				}
				RenderTexture.active = activeRender;
				if (frame <= mseq.lastFrame && (emit || renderSomething))
					ended = false;
			}
			if (run != null && run.cancel && !renderSomething)
				return true;
			return ended;
		}
		public override void End()
		{
			foreach (ModelSequence mseq in model)
			{
				foreach (ModelSequence.FBX tok in mseq.fbxList)
				{
					if (tok.unityObject == null)
						continue;
					UnityEngine.Object.Destroy(tok.unityObject);
					tok.unityObject = null;
				}
			}
			JSON.RenderingCount--;
		}
	}

	public class ModelSequence
	{
		public UInt32 key = 0;
		public Int32 minimalDuration = 0;
		public Int32 firstFrame = Int32.MaxValue;
		public Int32 lastFrame = -1;
		public String defaultFolder = "";
		public List<FBX> fbxList = new List<FBX>();
		public List<Sprite> spriteList = new List<Sprite>();

		public static ModelSequence Load(String path)
		{
			String fileStr = AssetManager.LoadString(path);
			if (fileStr == null)
				return null;
			JSONNode rootNode = JSONNode.Parse(fileStr);
			if (rootNode == null)
				return null;
			ModelSequence modelSeq = new ModelSequence();
			modelSeq.defaultFolder = Path.GetDirectoryName(path);
			modelSeq.firstFrame = Int32.MaxValue;
			modelSeq.lastFrame = -1;
			if (rootNode["Key"] != null)
				UInt32.TryParse(rootNode["Key"], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out modelSeq.key);
			if (rootNode["MinimalDuration"] != null)
				modelSeq.minimalDuration = rootNode["MinimalDuration"].AsInt;
			if (rootNode["FBX"] != null)
				LoadFBX(modelSeq, rootNode["FBX"] as JSONArray);
			if (rootNode["Sprite"] != null)
				LoadSprite(modelSeq, rootNode["Sprite"] as JSONArray);
			foreach (FBX fbx in modelSeq.fbxList)
			{
				modelSeq.firstFrame = Math.Min(modelSeq.firstFrame, fbx.startFrame);
				modelSeq.lastFrame = Math.Max(modelSeq.lastFrame, fbx.endFrame);
			}
			foreach (Sprite sprite in modelSeq.spriteList)
			{
				foreach (Sprite.Emission em in sprite.emission)
				{
					modelSeq.firstFrame = Math.Min(modelSeq.firstFrame, em.frame);
					modelSeq.lastFrame = Math.Max(modelSeq.lastFrame, em.frame + sprite.duration);
				}
			}
			if (modelSeq.firstFrame == Int32.MaxValue)
				modelSeq.firstFrame = 0;
			if (modelSeq.lastFrame == -1)
				modelSeq.lastFrame = modelSeq.firstFrame;
			return modelSeq;
		}

		private static void LoadFBX(ModelSequence modelSeq, JSONArray arrNode)
		{
			if (arrNode == null)
				return;
			foreach (JSONNode objectNode in arrNode)
			{
				if (objectNode["Path"] == null)
					continue;
				FBX fbx = new FBX();
				modelSeq.fbxList.Add(fbx);
				fbx.fbxPath = objectNode["Path"].Value;
				if (!fbx.fbxPath.Contains("/"))
					fbx.fbxPath = modelSeq.defaultFolder + "/" + fbx.fbxPath;
				else if (fbx.fbxPath.StartsWith("./"))
					fbx.fbxPath = modelSeq.defaultFolder + fbx.fbxPath.Substring(1);
				if (objectNode["Start"] != null)
					fbx.startFrame = objectNode["Start"].AsInt;
				if (objectNode["End"] != null)
					fbx.endFrame = objectNode["End"].AsInt;
				if (objectNode["Scale"] != null)
					fbx.scale = objectNode["Scale"].AsFloat;
				if (objectNode["Movement"] != null && objectNode["Movement"].AsObject != null)
					fbx.movement.LoadFromJSON(objectNode["Movement"].AsObject);
				if (objectNode["Animations"] == null)
					continue;
				foreach (JSONNode animNode in objectNode["Animations"] as JSONArray)
				{
					String animPath = animNode;
					if (!animPath.Contains("/"))
						animPath = modelSeq.defaultFolder + "/" + animPath;
					else if (animPath.StartsWith("./"))
						animPath = modelSeq.defaultFolder + animPath.Substring(1);
					fbx.animPath.Add(animPath);
				}
				if (fbx.startFrame == Int32.MaxValue)
					fbx.startFrame = 0;
				if (fbx.endFrame == -1)
					fbx.endFrame = fbx.startFrame;
			}
		}

		private static void LoadSprite(ModelSequence modelSeq, JSONArray arrNode)
		{
			if (arrNode == null)
				return;
			foreach (JSONNode objectNode in arrNode)
			{
				Sprite sprite = new Sprite();
				modelSeq.spriteList.Add(sprite);
				if (objectNode["Duration"] != null)
					sprite.duration = objectNode["Duration"].AsInt;
				if (objectNode["Movement"] != null)
					sprite.baseMovement.LoadFromJSON(objectNode["Movement"]);
				if (objectNode["ScreenSize"] != null)
					sprite.useScreenSize = objectNode["ScreenSize"].AsBool;
				if (objectNode["Material"] != null)
					sprite.material = EffectMaterial.LoadMaterial(objectNode["Material"] as JSONClass, modelSeq.defaultFolder);
				if (objectNode["Vertices"] != null && objectNode["Vertices"] is JSONArray)
				{
					JSONArray vertNode = objectNode["Vertices"] as JSONArray;
					sprite.vertex = new Vector2[vertNode.Count];
					for (Int32 i = 0; i < vertNode.Count; i++)
						sprite.vertex[i] = vertNode[i].AsVector;
				}
				if (objectNode["Indices"] != null && objectNode["Indices"] is JSONArray)
				{
					JSONArray indexNode = objectNode["Indices"] as JSONArray;
					sprite.index = new Int32[indexNode.Count];
					for (Int32 i = 0; i < indexNode.Count; i++)
						sprite.index[i] = indexNode[i].AsInt;
				}
				if (objectNode["UV"] != null && objectNode["UV"] is JSONArray)
				{
					JSONArray uvArray = objectNode["UV"] as JSONArray;
					sprite.uv[0] = new Vector2[uvArray.Count];
					for (Int32 i = 0; i < uvArray.Count; i++)
						sprite.uv[0][i] = uvArray[i].AsVector;
				}
				if (objectNode["TextureInterpolation"] != null)
				{
					if (objectNode["TextureInterpolation"] is JSONArray)
					{
						JSONArray interpArray = objectNode["TextureInterpolation"] as JSONArray;
						sprite.uvInterpolateType = new ParametricMovement.InterpolateType[interpArray.Count];
						for (Int32 i = 0; i < interpArray.Count; i++)
							ParametricMovement.TryParseInterpolateType(interpArray[i], out sprite.uvInterpolateType[i]);
					}
					else
					{
						ParametricMovement.TryParseInterpolateType(objectNode["TextureInterpolation"], out sprite.uvInterpolateType[0]);
					}
				}
				if (objectNode["TextureAnimation"] != null && objectNode["TextureAnimation"] is JSONArray)
					foreach (JSONNode uvNode in objectNode["TextureAnimation"] as JSONArray)
						if (uvNode["Frame"] != null && uvNode["UV"] != null && uvNode["UV"] is JSONArray)
						{
							JSONArray uvArray = uvNode["UV"] as JSONArray;
							sprite.uv[uvNode["Frame"].AsInt] = new Vector2[uvArray.Count];
							for (Int32 i = 0; i < uvArray.Count; i++)
								sprite.uv[uvNode["Frame"].AsInt][i] = uvArray[i].AsVector;
						}
				if (objectNode["VertexColors"] != null && objectNode["VertexColors"] is JSONArray)
				{
					JSONArray colorArray = objectNode["VertexColors"] as JSONArray;
					sprite.vertexColor[0] = new Color32[colorArray.Count];
					for (Int32 i = 0; i < colorArray.Count; i++)
						sprite.vertexColor[0][i] = (Color)colorArray[i].AsVector;
				}
				if (objectNode["ColorInterpolation"] != null)
				{
					if (objectNode["ColorInterpolation"] is JSONArray)
					{
						JSONArray interpArray = objectNode["ColorInterpolation"] as JSONArray;
						sprite.colorInterpolateType = new ParametricMovement.InterpolateType[interpArray.Count];
						for (Int32 i = 0; i < interpArray.Count; i++)
							ParametricMovement.TryParseInterpolateType(interpArray[i], out sprite.colorInterpolateType[i]);
					}
					else
					{
						ParametricMovement.TryParseInterpolateType(objectNode["ColorInterpolation"], out sprite.colorInterpolateType[0]);
					}
				}
				if (objectNode["ColorAnimation"] != null && objectNode["ColorAnimation"] is JSONArray)
					foreach (JSONNode colorNode in objectNode["ColorAnimation"] as JSONArray)
						if (colorNode["Frame"] != null && colorNode["VertexColors"] != null && colorNode["VertexColors"] is JSONArray)
						{
							JSONArray colorArray = colorNode["VertexColors"] as JSONArray;
							sprite.vertexColor[colorNode["Frame"].AsInt] = new Color32[colorArray.Count];
							for (Int32 i = 0; i < colorArray.Count; i++)
								sprite.vertexColor[colorNode["Frame"].AsInt][i] = (Color)colorArray[i].AsVector;
						}
				if (objectNode["Emission"] != null)
				{
					if (objectNode["Emission"] is JSONArray)
					{
						foreach (JSONNode emNode in objectNode["Emission"] as JSONArray)
						{
							List<Int32> frameList = new List<Int32>();
							if (emNode["Frame"] != null)
							{
								if (emNode["Frame"] is JSONArray)
									foreach (JSONNode frameNode in emNode["Frame"] as JSONArray)
										frameList.Add(frameNode.AsInt);
								else
									frameList.Add(emNode["Frame"].AsInt);
							}
							else
							{
								frameList.Add(0);
							}
							Int32 emCount = emNode["Count"] != null ? emNode["Count"].AsInt : 1;
							Dictionary<Int32, Int32> emIPMin = new Dictionary<Int32, Int32>();
							Dictionary<Int32, Int32> emIPMax = new Dictionary<Int32, Int32>();
							Dictionary<Int32, Single> emFPMin = new Dictionary<Int32, Single>();
							Dictionary<Int32, Single> emFPMax = new Dictionary<Int32, Single>();
							Int32 paramKey, paramIntValue;
							if (emNode is JSONClass)
							{
								foreach (KeyValuePair<String, JSONNode> p in (emNode as JSONClass).Dict)
								{
									if (p.Key.StartsWith("ParameterMin") && Int32.TryParse(p.Key.Substring(12), out paramKey))
									{
										if (Int32.TryParse(p.Value.Value, out paramIntValue))
											emIPMin[paramKey] = paramIntValue;
										else
											emFPMin[paramKey] = p.Value.AsFloat;
									}
									else if (p.Key.StartsWith("ParameterMax") && Int32.TryParse(p.Key.Substring(12), out paramKey))
									{
										if (Int32.TryParse(p.Value.Value, out paramIntValue))
											emIPMax[paramKey] = paramIntValue;
										else
											emFPMax[paramKey] = p.Value.AsFloat;
									}
									else if (p.Key.StartsWith("Parameter") && Int32.TryParse(p.Key.Substring(9), out paramKey))
									{
										if (Int32.TryParse(p.Value.Value, out paramIntValue))
											emIPMin[paramKey] = emIPMax[paramKey] = paramIntValue;
										else
											emFPMin[paramKey] = emFPMax[paramKey] = p.Value.AsFloat;
									}
								}
							}
							foreach (Int32 f in frameList)
							{
								Sprite.Emission emission = new Sprite.Emission();
								modelSeq.firstFrame = Math.Min(modelSeq.firstFrame, f);
								modelSeq.lastFrame = Math.Max(modelSeq.lastFrame, f + sprite.duration);
								emission.frame = f;
								emission.count = emCount;
								emission.iParamRangeMin = emIPMin;
								emission.iParamRangeMax = emIPMax;
								emission.fParamRangeMin = emFPMin;
								emission.fParamRangeMax = emFPMax;
								sprite.emission.Add(emission);
							}
						}
					}
					else if (objectNode["Emission"].Value.StartsWith("Follow "))
					{
						Int32 spriteLink;
						if (Int32.TryParse(objectNode["Emission"].Value.Substring(7), out spriteLink))
							sprite.spriteEmissionLink.Add(spriteLink);
					}
				}
				if (objectNode["Scale"] != null)
					sprite.scaling[0] = objectNode["Scale"].AsFloat;
				if (objectNode["ScaleInterpolation"] != null)
				{
					if (objectNode["ScaleInterpolation"] is JSONArray)
					{
						JSONArray interpArray = objectNode["ScaleInterpolation"] as JSONArray;
						sprite.scalingInterpolateType = new ParametricMovement.InterpolateType[interpArray.Count];
						for (Int32 i = 0; i < interpArray.Count; i++)
							ParametricMovement.TryParseInterpolateType(interpArray[i], out sprite.scalingInterpolateType[i]);
					}
					else
					{
						ParametricMovement.TryParseInterpolateType(objectNode["ScaleInterpolation"], out sprite.scalingInterpolateType[0]);
					}
				}
				if (objectNode["ScaleAnimation"] != null && objectNode["ScaleAnimation"] is JSONArray)
					foreach (JSONNode scaleNode in objectNode["ScaleAnimation"] as JSONArray)
						if (scaleNode["Frame"] != null && scaleNode["Scale"] != null)
							sprite.scaling[scaleNode["Frame"].AsInt] = scaleNode["Scale"].AsFloat;
				if (sprite.emission.Count == 0 && sprite.spriteEmissionLink.Count == 0)
				{
					sprite.emission.Add(new Sprite.Emission());
					sprite.emission[0].frame = 0;
					sprite.emission[0].count = 1;
					sprite.emission[0].iParamRangeMin = new Dictionary<Int32, Int32>();
					sprite.emission[0].iParamRangeMax = new Dictionary<Int32, Int32>();
					sprite.emission[0].fParamRangeMin = new Dictionary<Int32, Single>();
					sprite.emission[0].fParamRangeMax = new Dictionary<Int32, Single>();
				}
			}
			for (Int32 i = 0; i < modelSeq.spriteList.Count; i++)
				foreach (Int32 link in modelSeq.spriteList[i].spriteEmissionLink)
					if (link >= 0 && link < modelSeq.spriteList.Count && modelSeq.spriteList[link].spriteEmissionLink.FindIndex(j => j == i) < 0)
						modelSeq.spriteList[link].spriteEmissionLink.Add(i);
		}

		public class FBX
		{
			public String fbxPath = "";
			public List<String> animPath = new List<String>();
			public GameObject unityObject = null;
			public ParametricMovement movement = new ParametricMovement();
			public Int32 startFrame = Int32.MaxValue;
			public Int32 endFrame = -1;
			public Single scale = 1f;
		}

		public class Sprite
		{
			public Int32 duration = 0;
			public EffectMaterial material = new EffectMaterial();
			public List<Emission> emission = new List<Emission>();
			public List<Int32> spriteEmissionLink = new List<Int32>();
			public Dictionary<Int32, Vector2[]> uv = new Dictionary<Int32, Vector2[]>();
			public Dictionary<Int32, Color32[]> vertexColor = new Dictionary<Int32, Color32[]>();
			public Dictionary<Int32, Single> scaling = new Dictionary<Int32, Single>();
			public ParametricMovement.InterpolateType[] uvInterpolateType = new ParametricMovement.InterpolateType[] { ParametricMovement.InterpolateType.Constant };
			public ParametricMovement.InterpolateType[] colorInterpolateType = new ParametricMovement.InterpolateType[] { ParametricMovement.InterpolateType.Constant };
			public ParametricMovement.InterpolateType[] scalingInterpolateType = new ParametricMovement.InterpolateType[] { ParametricMovement.InterpolateType.Linear };
			public ParametricMovement baseMovement = new ParametricMovement();
			public Vector2[] vertex = new Vector2[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(1, 1) };
			public Int32[] index = new Int32[] { 0, 1, 2, 1, 3, 2 };
			public Int32 lastFrameRendered = -1;
			public Boolean useScreenSize = false;

			public List<Particle> particle = new List<Particle>();

			public Int32 VertexCount => vertex.Length * particle.Count;
			public Int32 IndexCount => index.Length * particle.Count;

			public void AppendVertices(Int32 frame, BTL_DATA caster, BTL_DATA target, Vector3 averageTarget, Vector3[] meshVert, Vector2[] meshUV, Color32[] meshColor, Int32[] meshIndex, ref Int32 vIndex, ref Int32 iIndex)
			{
				Camera camera = Camera.main ? Camera.main : GameObject.Find("Battle Camera").GetComponent<BattleMapCameraController>().GetComponent<Camera>();
				foreach (Particle p in particle)
				{
					Vector3 basePos = p.movement.GetPosition(frame - p.frameStart, p.param, caster, target, averageTarget);
					basePos = camera.WorldToScreenPoint(basePos); // World -> Screen
					Single scale = GetScalingFactor(frame - p.frameStart);
					Vector2[] uv = GetTimedUV(frame - p.frameStart);
					Color32[] col = GetTimedColor(frame - p.frameStart);
					for (Int32 i = 0; i < vertex.Length; i++)
					{
						Vector2 vLocalPos = scale * vertex[i];
						if (!useScreenSize)
						{
							if (basePos.z >= 0f)
							{
								vLocalPos.x *= 7000f / Math.Max(1f, basePos.z); // 7000 is arbitrary
								vLocalPos.y *= 7000f / Math.Max(1f, basePos.z); // sizes are similar with and without useScreenSize for mid-distance camera
							}
							else
							{
								vLocalPos.x *= 7000f / Math.Min(-1f, basePos.z);
								vLocalPos.y *= 7000f / Math.Min(-1f, basePos.z);
							}
						}
						Vector3 posScreen = basePos + (Vector3)vLocalPos;
						posScreen.x = (posScreen.x - SFX.screenWidthOffset) * FieldMap.PsxScreenWidth / SFX.screenWidth; // Screen -> Render projection space
						posScreen.y *= (Single)FieldMap.PsxScreenHeightNative / (Single)SFX.screenHeight;
						posScreen.y = FieldMap.PsxScreenHeightNative - posScreen.y;
						posScreen.z *= -1;
						meshVert[vIndex + i] = posScreen;
						meshUV[vIndex + i] = uv != null && i < uv.Length ? uv[i] : default(Vector2);
						meshColor[vIndex + i] = col != null && i < col.Length ? col[i] : new Color32(255, 255, 255, 255);
						if (posScreen.z > 0f)
							meshColor[vIndex + i].a = 0;
					}
					for (Int32 i = 0; i < index.Length; i++)
						meshIndex[iIndex++] = vIndex + index[i];
					vIndex += vertex.Length;
				}
			}

			public Single GetScalingFactor(Int32 frame)
			{
				return ParametricMovement.GetInterpolatedDictionaryValue((a, b, c, d) => a * b + c * d, scaling, frame, 1f, scalingInterpolateType[0], scalingInterpolateType);
			}

			public Vector2[] GetTimedUV(Int32 frame)
			{
				return ParametricMovement.GetInterpolatedDictionaryValue(UVInterpolateCombination, uv, frame, null, uvInterpolateType[0], uvInterpolateType);
			}

			public Color32[] GetTimedColor(Int32 frame)
			{
				return ParametricMovement.GetInterpolatedDictionaryValue(ColorInterpolateCombination, vertexColor, frame, null, colorInterpolateType[0], colorInterpolateType);
			}

			private Vector2[] UVInterpolateCombination(Single a, Vector2[] origin, Single b, Vector2[] dest)
			{
				if (a >= 1f)
					return origin;
				if (b >= 1f)
					return dest;
				if (origin == null || dest == null || origin.Length != dest.Length)
					return origin;
				Vector2[] combination = new Vector2[origin.Length];
				for (Int32 i = 0; i < origin.Length; i++)
					combination[i] = a * origin[i] + b * dest[i];
				return combination;
			}

			private Color32[] ColorInterpolateCombination(Single a, Color32[] origin, Single b, Color32[] dest)
			{
				if (a >= 1f)
					return origin;
				if (b >= 1f)
					return dest;
				if (origin == null || dest == null || origin.Length != dest.Length)
					return origin;
				Color32[] combination = new Color32[origin.Length];
				for (Int32 i = 0; i < origin.Length; i++)
					combination[i] = a * (Color)origin[i] + b * (Color)dest[i];
				return combination;
			}

			public class Emission
			{
				public Int32 frame;
				public Int32 count;
				public Dictionary<Int32, Single> fParamRangeMin;
				public Dictionary<Int32, Single> fParamRangeMax;
				public Dictionary<Int32, Int32> iParamRangeMin;
				public Dictionary<Int32, Int32> iParamRangeMax;
			}

			public class Particle
			{
				public Int32 frameStart;
				public Dictionary<Int32, Single> param = null;
				public ParametricMovement movement;

				public Particle(Emission em, ParametricMovement baseMovement)
				{
					frameStart = em.frame;
					movement = new ParametricMovement(baseMovement);
					if (em.iParamRangeMin.Count > 0 || em.fParamRangeMin.Count > 0)
					{
						param = new Dictionary<Int32, Single>();
						Single fpmaxValue;
						Int32 ipmaxValue;
						foreach (KeyValuePair<Int32, Int32> pmin in em.iParamRangeMin)
						{
							if (!em.iParamRangeMax.TryGetValue(pmin.Key, out ipmaxValue))
								continue;
							param[pmin.Key] = UnityEngine.Random.RandomRange(pmin.Value, ipmaxValue);
						}
						foreach (KeyValuePair<Int32, Single> pmin in em.fParamRangeMin)
						{
							if (!em.fParamRangeMax.TryGetValue(pmin.Key, out fpmaxValue))
								continue;
							param[pmin.Key] = UnityEngine.Random.RandomRange(pmin.Value, fpmaxValue);
						}
					}
				}

				public Particle(Particle basis)
				{
					frameStart = basis.frameStart;
					param = basis.param;
					movement = basis.movement;
				}
			}
		}
	}
}

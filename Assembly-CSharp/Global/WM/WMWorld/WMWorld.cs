using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Object = System.Object;

public class WMWorld : Singleton<WMWorld>
{
	public WMBlock[,] Blocks { get; private set; }

	public WMBlock[,] InitialBlocks { get; private set; }

	public Camera MainCamera { get; private set; }

	public Single Width { get; private set; }

	public Single Height { get; private set; }

	public Boolean FinishedLoadingBlocks { get; private set; }

	public WMWorldLoadState LoadState { get; private set; }

	public Int32 BackgroundLoadingCount { get; private set; }

	public Int32 ActiveBlockCount { get; private set; }

	public GlobalFog GlobalFog
	{
		get
		{
			if (this._globalFog == (UnityEngine.Object)null)
			{
				this._globalFog = this.MainCamera.GetComponent<GlobalFog>();
			}
			return this._globalFog;
		}
	}

	private static ObjList CreateObjList_DebugScene()
	{
		ObjList objList = new ObjList();
		Actor obj = new Actor();
		objList.obj = obj;
		objList.next = (ObjList)null;
		return objList;
	}

	public void Initialize()
	{
		if (FF9StateSystem.World.IsBeeScene)
		{
			GameObject gameObject = GameObject.Find("WorldMapRoot");
			Transform original = Resources.Load<Transform>("EmbeddedAsset/WorldMap_Local/Prefabs/TranslatingActors");
			this.TranslatingObjectsGroup = UnityEngine.Object.Instantiate<Transform>(original);
			this.TranslatingObjectsGroup.name = "TranslatingObjectsGroup";
			this.TranslatingObjectsGroup.parent = gameObject.transform;
			foreach (Object obj in this.TranslatingObjectsGroup)
			{
				Transform transform = (Transform)obj;
				WMActor component = transform.GetComponent<WMActor>();
				ObjList objList = WMWorld.CreateObjList_DebugScene();
				((Actor)objList.obj).wmActor = component;
				component.originalActor = (Actor)objList.obj;
				if (component)
				{
					if (this.ActorList == null)
					{
						this.ActorList = objList;
					}
					else
					{
						ObjList objList2 = this.ActorList;
						while (objList2.next != null)
						{
							objList2 = objList2.next;
						}
						objList2.next = objList;
					}
				}
				else
				{
					global::Debug.LogWarning(transform.name + " is not WMActor!");
				}
			}
		}
		else
		{
			GameObject gameObject2 = GameObject.Find("WorldMapRoot");
			this.TranslatingObjectsGroup = new GameObject("TranslatingObjectsGroup").transform;
			this.TranslatingObjectsGroup.parent = gameObject2.transform;
		}
		this.InitialBlocks = WMWorld.BuildBlockArray(this.WorldDisc);
		this.MainCamera = GameObject.Find("WorldCamera").GetComponent<Camera>();
		GameObject original2 = AssetManager.Load<GameObject>("EmbeddedAsset/WorldMap_Local/MoveFromBundle/kWorldPackEffectSky_Sky", out _, false);
		this.SkyDome_Sky = UnityEngine.Object.Instantiate<GameObject>(original2).transform;
		this.SkyDome_Sky.parent = this.WorldMapEffectRoot;
		this.SkyDome_SkyMaterial = this.SkyDome_Sky.GetComponentInChildren<MeshRenderer>().material;
		GameObject original3 = AssetManager.Load<GameObject>("EmbeddedAsset/WorldMap_Local/MoveFromBundle/kWorldPackEffectSky_Bg", out _, false);
		this.SkyDome_Bg = UnityEngine.Object.Instantiate<GameObject>(original3).transform;
		this.SkyDome_Bg.parent = this.WorldMapEffectRoot;
		this.SkyDowm_BgMaterial = this.SkyDome_Bg.GetComponentInChildren<MeshRenderer>().material;
		GameObject original4 = AssetManager.Load<GameObject>("WorldMap/Prefabs/Effects/kWorldPackEffectSky_Fog", out _, false);
		this.SkyDome_Fog = UnityEngine.Object.Instantiate<GameObject>(original4).transform;
		this.SkyDome_Fog.parent = this.WorldMapEffectRoot;
		this.SkyDowm_FogMaterial = this.SkyDome_Fog.GetComponentInChildren<MeshRenderer>().material;
		if (FF9StateSystem.World.FixTypeCam)
		{
			this.SkyDome_Sky.localScale = new Vector3(1f, 1f, 1f);
			this.SkyDome_Bg.localScale = new Vector3(1.01f, 1.01f, 1.01f);
		}
		if (FF9StateSystem.World.IsBeeScene)
		{
			if (ff9.w_frameDisc == 0)
			{
				ff9.w_frameDisc = 1;
			}
		}
		else
		{
			ff9.w_frameScenePtr = ff9.ushort_gEventGlobal(0);
			if (ff9.w_frameScenePtr >= 11090)
			{
				ff9.w_frameDisc = 4;
			}
			else
			{
				ff9.w_frameDisc = 1;
			}
		}
		this.currentDisc = (Int32)ff9.w_frameDisc;
		if (!this.WorldDisc)
		{
			global::Debug.LogError("WorldDisc can't be null. Please set it in the scene.");
			this.LoadState = WMWorldLoadState.Initializing;
		}
		else
		{
			if (this.currentDisc == -1)
			{
				ff9.w_frameDisc = 1;
				this.currentDisc = 1;
			}
			this.LoadState = WMWorldLoadState.Finished;
			this.LoadState = WMWorldLoadState.Initializing;
		}
		this.LoadEffects();
		this.LoadSPS();
	}

	private void UpscaleMeshBounds()
	{
		MeshFilter[] componentsInChildren = base.GetComponentsInChildren<MeshFilter>(true);
		MeshFilter[] array = componentsInChildren;
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			MeshFilter meshFilter = array[i];
			Bounds bounds = meshFilter.sharedMesh.bounds;
			Vector3 size = bounds.size;
			size.y = 51f;
			bounds.size = size;
			meshFilter.sharedMesh.bounds = bounds;
		}
	}

	public void addWMActor(Actor actor)
	{
		String name = actor.go.name;
		GameObject gameObject = new GameObject(name + "WM");
		WMActor wmactor = gameObject.AddComponent<WMActor>();
		wmactor.Animation = actor.go.GetComponent<Animation>();
		gameObject.transform.parent = this.TranslatingObjectsGroup;
		actor.wmActor = wmactor;
		actor.wmActor.Intialize();
		actor.wmActor.originalActor = actor;
		actor.wmActor.SetScale(64, 64, 64);
		if (ff9.w_moveCHRStatus[(Int32)actor.index].cache >= 4 && ff9.w_moveCHRStatus[(Int32)actor.index].cache <= 8)
		{
			GameObject gameObject2 = new GameObject(name + "Rot");
			gameObject2.transform.parent = gameObject.transform;
			actor.go.transform.parent = gameObject2.transform;
		}
		else
		{
			actor.go.transform.parent = gameObject.transform;
		}
		actor.go.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
	}

	public void addWMActorOnly(Actor actor)
	{
		String str = "obj" + actor.uid;
		GameObject gameObject = new GameObject(str + "_WM");
		WMActor wmActor = gameObject.AddComponent<WMActor>();
		gameObject.transform.parent = this.TranslatingObjectsGroup;
		actor.wmActor = wmActor;
		actor.wmActor.Intialize();
		actor.wmActor.originalActor = actor;
	}

	public void addGameObjectToWMActor(GameObject go, WMActor wa)
	{
		wa.originalActor.go = go;
		wa.Animation = go.GetComponent<Animation>();
		Actor originalActor = wa.originalActor;
		originalActor.wmActor.SetScale(64, 64, 64);
		String str = "obj" + originalActor.uid;
		Transform transform = this.TranslatingObjectsGroup.Find(str + "_WM");
		if (ff9.w_moveCHRStatus[(Int32)originalActor.index].cache >= 4 && ff9.w_moveCHRStatus[(Int32)originalActor.index].cache <= 8)
		{
			GameObject gameObject = new GameObject(str + "_Rot");
			gameObject.transform.parent = transform.transform;
			originalActor.go.transform.parent = gameObject.transform;
		}
		else
		{
			originalActor.go.transform.parent = transform.transform;
		}
		originalActor.go.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
	}

	public Vector3 GetAbsolutePositionOf(Transform target)
	{
		if (this.Blocks == null)
		{
			return target.position;
		}
		Vector3 position = target.position;
		Vector3 vector = new Vector3(position.x % 64f, 0f, position.z % 64f);
		Int32 num = (Int32)(position.x / 64f);
		Int32 num2 = (Int32)(Mathf.Abs(position.z) / 64f);
		Int32 length = this.Blocks.GetLength(0);
		Int32 length2 = this.Blocks.GetLength(1);
		if (num < 0)
		{
			return target.position;
		}
		if (num >= length)
		{
			num = length - 1;
		}
		if (num2 < 0)
		{
			return target.position;
		}
		if (num2 >= length2)
		{
			num2 = length2 - 1;
		}
		WMBlock wmblock = this.Blocks[num, num2];
		WMBlock component = wmblock.GetComponent<WMBlock>();
		Vector3 result = new Vector3((Single)(component.InitialX * 64) + vector.x, position.y, (Single)(-(Single)component.InitialY * 64) + vector.z);
		return result;
	}

	public Vector3 GetAbsolutePositionOf(Vector3 vector)
	{
		if (this.Blocks == null)
		{
			return vector;
		}
		Vector3 vector2 = vector;
		Vector3 vector3 = new Vector3(vector2.x % 64f, 0f, vector2.z % 64f);
		Int32 num = (Int32)(vector2.x / 64f);
		Int32 num2 = (Int32)(Mathf.Abs(vector2.z) / 64f);
		WMBlock wmblock = this.Blocks[num, num2];
		WMBlock component = wmblock.GetComponent<WMBlock>();
		Vector3 result = new Vector3((Single)(component.InitialX * 64) + vector3.x, vector2.y, (Single)(-(Single)component.InitialY * 64) + vector3.z);
		return result;
	}

	public void GetAbsolutePositionOf_FixedPoint(ref Int32 posX, ref Int32 posY, ref Int32 posZ)
	{
		if (this.Blocks == null)
		{
			return;
		}
		Int32 num = posX;
		Int32 num2 = posY;
		Int32 num3 = posZ;
		Int32 num4 = num % 16384;
		Int32 num5 = num3 % 16384;
		Int32 num6 = num / 16384;
		Int32 num7 = Math.Abs(num3) / 16384;
		WMBlock wmblock = this.Blocks[num6, num7];
		WMBlock component = wmblock.GetComponent<WMBlock>();
		posX = component.InitialX * 16384 + num4;
		posZ = -component.InitialY * 16384 + num5;
	}

	public void GetUnityPositionOf_FixedPoint(ref Int32 posX, ref Int32 posY, ref Int32 posZ)
	{
		if (this.Blocks == null)
		{
			return;
		}
		Int32 num = posX;
		Int32 num2 = posZ;
		Int32 num3 = num % 16384;
		Int32 num4 = num2 % 16384;
		Int32 num5 = num / 16384;
		Int32 num6 = Math.Abs(num2) / 16384;
		WMBlock wmblock = this.InitialBlocks[num5, num6];
		WMBlock component = wmblock.GetComponent<WMBlock>();
		posX = component.CurrentX * 16384 + num3;
		posZ = -component.CurrentY * 16384 + num4;
	}

	public WMBlock GetAbsoluteBlock(Transform target)
	{
		return this.GetAbsoluteBlock(target.position);
	}

	public WMBlock GetAbsoluteBlock(Vector3 position)
	{
		if (this.Blocks == null)
		{
			return (WMBlock)null;
		}
		Int32 num = (Int32)(position.x / 64f);
		Int32 num2 = (Int32)(Mathf.Abs(position.z) / 64f);
		if (num >= 24)
		{
			return (WMBlock)null;
		}
		if (num2 >= 20)
		{
			return (WMBlock)null;
		}
		WMBlock wmblock = this.Blocks[num, num2];
		return this.Blocks[wmblock.CurrentX, wmblock.CurrentY];
	}

	public void SetAbsolutePositionOf(Transform target, Vector3 position, Single rotationY = 0f)
	{
		if (this.Blocks == null)
		{
			return;
		}
		Vector3 vector = new Vector3(position.x % 64f, 0f, position.z % 64f);
		Int32 num = (Int32)(position.x / 64f);
		Int32 num2 = (Int32)(Mathf.Abs(position.z) / 64f);
		WMBlock wmblock = (WMBlock)null;
		WMBlock[,] blocks = this.Blocks;
		Int32 length = blocks.GetLength(0);
		Int32 length2 = blocks.GetLength(1);
		for (Int32 i = 0; i < length; i++)
		{
			for (Int32 j = 0; j < length2; j++)
			{
				WMBlock wmblock2 = blocks[i, j];
				if (num == wmblock2.InitialX && num2 == wmblock2.InitialY)
				{
					wmblock = wmblock2;
					goto IL_D0;
				}
			}
		}
		IL_D0:
		if (wmblock != (UnityEngine.Object)null)
		{
			Vector3 position2 = new Vector3((Single)(wmblock.CurrentX * 64) + vector.x, position.y, (Single)(-(Single)wmblock.CurrentY * 64) + vector.z);
			target.position = position2;
		}
	}

	public void SetAbsolutePositionOf(out Vector3 outPosition, Vector3 position)
	{
		outPosition = Vector3.zero;
		if (this.Blocks == null)
		{
			return;
		}
		Vector3 vector = new Vector3(position.x % 64f, 0f, position.z % 64f);
		Int32 num = (Int32)(position.x / 64f);
		Int32 num2 = (Int32)(Mathf.Abs(position.z) / 64f);
		WMBlock wmblock = (WMBlock)null;
		WMBlock[,] blocks = this.Blocks;
		Int32 length = blocks.GetLength(0);
		Int32 length2 = blocks.GetLength(1);
		for (Int32 i = 0; i < length; i++)
		{
			for (Int32 j = 0; j < length2; j++)
			{
				WMBlock wmblock2 = blocks[i, j];
				if (num == wmblock2.InitialX && num2 == wmblock2.InitialY)
				{
					wmblock = wmblock2;
					goto IL_DB;
				}
			}
		}
		IL_DB:
		if (wmblock != (UnityEngine.Object)null)
		{
			Vector3 vector2 = new Vector3((Single)(wmblock.CurrentX * 64) + vector.x, position.y, (Single)(-(Single)wmblock.CurrentY * 64) + vector.z);
			outPosition = vector2;
		}
	}

	private void CaculateBoundsOfTranslatingObjects()
	{
		this.Width = 1536f;
		this.Height = 1280f;
		this.centerOfMesh.x = 800f;
		this.centerOfMesh.y = 0f;
		this.centerOfMesh.z = -672f;
		this.wrapWorldLeftBound = this.centerOfMesh.x - 32f;
		this.wrapWorldRightBound = this.centerOfMesh.x + 32f;
		this.wrapWorldUpperBound = this.centerOfMesh.z + 32f;
		this.wrapWorldLowerBound = this.centerOfMesh.z - 32f;
	}

	private void UpdateInputDebug()
	{
		Int32 num = 0;
		for (Int32 i = 0; i < this.WorldSPSSystem.SpsList.Count; i++)
		{
			WorldSPS worldSPS = this.WorldSPSSystem.SpsList[i];
			if (worldSPS.spsBin != null)
			{
				if (worldSPS.no != -1)
				{
					num++;
				}
			}
		}
		if (this.activeSpsPeak < num)
		{
			this.activeSpsPeak = num;
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			ff9.s_moveCHRStatus s_moveCHRStatus = ff9.w_moveCHRStatus[(Int32)ff9.w_moveActorPtr.originalActor.index];
			global::Debug.Log("status.id = " + s_moveCHRStatus.id);
		}
		Vector3 realPosition = ff9.w_moveActorPtr.RealPosition;
		if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			global::Debug.Log("ff9.w_moveActorPtr.RealPosition = " + ff9.w_moveActorPtr.RealPosition);
		}
		if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			FF9StateSystem.World.FixTypeCam = !FF9StateSystem.World.FixTypeCam;
			if (FF9StateSystem.World.FixTypeCam)
			{
				ff9.w_moveCHRControl[0].type_cam = 0;
				ff9.w_moveCHRControl[1].type_cam = 1;
				ff9.w_moveCHRControl[2].type_cam = 1;
				ff9.w_moveCHRControl[3].type_cam = 1;
				ff9.w_moveCHRControl[4].type_cam = 1;
				ff9.w_moveCHRControl[5].type_cam = 1;
				ff9.w_moveCHRControl[6].type_cam = 4;
				ff9.w_moveCHRControl[7].type_cam = 2;
				ff9.w_moveCHRControl[8].type_cam = 3;
				ff9.w_moveCHRControl[9].type_cam = 3;
				ff9.w_moveCHRControl[10].type_cam = 0;
				this.SkyDome_Sky.localScale = new Vector3(1f, 1.08f, 1f);
				this.SkyDome_Bg.localScale = new Vector3(1f, 1.081f, 1f);
			}
			else
			{
				ff9.w_moveCHRControl[0].type_cam = 0;
				ff9.w_moveCHRControl[1].type_cam = 0;
				ff9.w_moveCHRControl[2].type_cam = 0;
				ff9.w_moveCHRControl[3].type_cam = 0;
				ff9.w_moveCHRControl[4].type_cam = 0;
				ff9.w_moveCHRControl[5].type_cam = 0;
				ff9.w_moveCHRControl[6].type_cam = 0;
				ff9.w_moveCHRControl[7].type_cam = 0;
				ff9.w_moveCHRControl[8].type_cam = 0;
				ff9.w_moveCHRControl[9].type_cam = 0;
				ff9.w_moveCHRControl[10].type_cam = 0;
				this.SkyDome_Sky.localScale = new Vector3(1f, 1f, 1f);
				this.SkyDome_Bg.localScale = new Vector3(1f, 1.001f, 1f);
			}
		}
		Vector3 pos = ff9.w_moveActorPtr.pos;
		pos.y = 0f;
		if (ff9.w_movePlanePtr != (UnityEngine.Object)null)
		{
			Vector3 pos2 = ff9.w_movePlanePtr.pos;
			pos2.y = 0f;
			Single num2 = Vector3.Distance(pos, pos2);
		}
		if (ff9.w_moveChocoboPtr != (UnityEngine.Object)null)
		{
			Vector3 pos3 = ff9.w_moveChocoboPtr.pos;
			pos3.y = 0f;
			Single num3 = Vector3.Distance(pos, pos3);
		}
	}

	public void OnInitialize()
	{
		if (this.Blocks == null)
		{
			this.Blocks = WMWorld.BuildBlockArray(this.WorldDisc);
			for (Int32 i = 0; i < 20; i++)
			{
				for (Int32 j = 0; j < 24; j++)
				{
					WMBlock wmblock = this.Blocks[j, i];
					Vector3 position = default(Vector3);
					position.x = (Single)(j * 16384) * 0.00390625f;
					position.z = (Single)(i * -16384) * 0.00390625f;
					wmblock.transform.position = position;
				}
			}
			this.CaculateBoundsOfTranslatingObjects();
			this.ActorList = ff9.GetActiveObjList();
			if (this.Settings.WrapWorld)
			{
				while (!this.Wrap())
				{
				}
			}
			this.root = this.WorldDisc;
			this.RetrieveUsingTextures();
			this.LoadState = WMWorldLoadState.ReadyToLoadBlocks;
		}
	}

	public void OnUpdateLoading()
	{
		if (FF9StateSystem.World.LoadingType == WorldState.Loading.AllAtOnce)
		{
			this.FinishedLoadingBlocks = true;
			this.CheckIfLoadingBlocksIsFinished();
		}
		else
		{
			this.UpdateLoadBlocks();
			this.CheckIfLoadingBlocksIsFinished();
		}
	}

	private void LoadBlock(Int32 disc, WMBlock block)
	{
		Int32 initialX = block.InitialX;
		Int32 initialY = block.InitialY;
		block.IsReady = true;
		if (block.IsSea)
		{
			this.LoadBlock(this.SeaBlockPrefab, block);
		}
		else
		{
			String arg = String.Format("Block[{0}][{1}]", initialX, initialY);
			String name = String.Format("WorldMap/Prefabs/WorldDisc{0}/r{1}/{2}", disc, initialY, arg);
			GameObject blockObjectPrefab = AssetManager.Load<GameObject>(name, out _, false);
			this.LoadBlock(blockObjectPrefab, block);
		}
	}

	private void LoadBlock(GameObject blockObjectPrefab, WMBlock block)
	{
		WMBlockPrefab component = blockObjectPrefab.GetComponent<WMBlockPrefab>();
		block.Form1WalkMeshes = new List<WMMesh>();
		block.Form2WalkMeshes = new List<WMMesh>();
		block.Form1Transforms = new List<Transform>();
		if (component.ObjectForm1)
		{
			Transform transform = UnityEngine.Object.Instantiate<Transform>(component.ObjectForm1);
			transform.name = component.ObjectForm1.name;
			transform.parent = block.transform;
			transform.localPosition = Vector3.zero;
			transform.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh = transform.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm1(sharedMesh);
			block.AddForm1Transform(transform);
		}
		if (component.TerrainForm1)
		{
			Transform transform2 = UnityEngine.Object.Instantiate<Transform>(component.TerrainForm1);
			transform2.name = component.TerrainForm1.name;
			transform2.parent = block.transform;
			transform2.localPosition = Vector3.zero;
			transform2.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh2 = transform2.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm1(sharedMesh2);
			block.AddForm1Transform(transform2);
		}
		if (component.ObjectForm2)
		{
			Transform transform3 = UnityEngine.Object.Instantiate<Transform>(component.ObjectForm2);
			transform3.name = component.ObjectForm2.name;
			transform3.parent = block.transform;
			transform3.localPosition = Vector3.zero;
			transform3.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh3 = transform3.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm2(sharedMesh3);
			block.AddForm2Transform(transform3);
		}
		if (component.TerrainForm2)
		{
			Transform transform4 = UnityEngine.Object.Instantiate<Transform>(component.TerrainForm2);
			transform4.name = component.TerrainForm2.name;
			transform4.parent = block.transform;
			transform4.localPosition = Vector3.zero;
			transform4.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh4 = transform4.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm2(sharedMesh4);
			block.AddForm2Transform(transform4);
		}
		if (block.Number == 219)
		{
			if (component.Sea3)
			{
				Transform transform5 = UnityEngine.Object.Instantiate<Transform>(component.Sea3);
				transform5.name = component.Sea3.name;
				transform5.parent = block.transform;
				transform5.localPosition = Vector3.zero;
				transform5.localScale = new Vector3(1f, 1f, 1f);
				Mesh sharedMesh5 = transform5.GetComponent<MeshFilter>().sharedMesh;
				block.AddWalkMeshForm1(sharedMesh5);
				block.AddForm1Transform(transform5);
				block.HasSea = true;
			}
			if (component.Sea4)
			{
				Transform transform6 = UnityEngine.Object.Instantiate<Transform>(component.Sea4);
				transform6.name = component.Sea4.name;
				transform6.parent = block.transform;
				transform6.localPosition = Vector3.zero;
				transform6.localScale = new Vector3(1f, 1f, 1f);
				Mesh sharedMesh6 = transform6.GetComponent<MeshFilter>().sharedMesh;
				block.AddWalkMeshForm1(sharedMesh6);
				block.AddForm1Transform(transform6);
				block.HasSea = true;
			}
			if (component.Sea5)
			{
				Transform transform7 = UnityEngine.Object.Instantiate<Transform>(component.Sea5);
				transform7.name = component.Sea5.name;
				transform7.parent = block.transform;
				transform7.localPosition = Vector3.zero;
				transform7.localScale = new Vector3(1f, 1f, 1f);
				Mesh sharedMesh7 = transform7.GetComponent<MeshFilter>().sharedMesh;
				block.AddWalkMeshForm1(sharedMesh7);
				block.AddForm1Transform(transform7);
				block.HasSea = true;
			}
			if (component.Sea3_2)
			{
				Transform transform8 = UnityEngine.Object.Instantiate<Transform>(component.Sea3_2);
				transform8.name = component.Sea3_2.name;
				transform8.parent = block.transform;
				transform8.localPosition = Vector3.zero;
				transform8.localScale = new Vector3(1f, 1f, 1f);
				Mesh sharedMesh8 = transform8.GetComponent<MeshFilter>().sharedMesh;
				block.AddWalkMeshForm2(sharedMesh8);
				block.AddForm2Transform(transform8);
			}
			if (component.Sea4_2)
			{
				Transform transform9 = UnityEngine.Object.Instantiate<Transform>(component.Sea4_2);
				transform9.name = component.Sea4_2.name;
				transform9.parent = block.transform;
				transform9.localPosition = Vector3.zero;
				transform9.localScale = new Vector3(1f, 1f, 1f);
				Mesh sharedMesh9 = transform9.GetComponent<MeshFilter>().sharedMesh;
				block.AddWalkMeshForm2(sharedMesh9);
				block.AddForm2Transform(transform9);
			}
			if (component.Sea5_2)
			{
				Transform transform10 = UnityEngine.Object.Instantiate<Transform>(component.Sea5_2);
				transform10.name = component.Sea5_2.name;
				transform10.parent = block.transform;
				transform10.localPosition = Vector3.zero;
				transform10.localScale = new Vector3(1f, 1f, 1f);
				Mesh sharedMesh10 = transform10.GetComponent<MeshFilter>().sharedMesh;
				block.AddWalkMeshForm2(sharedMesh10);
				block.AddForm2Transform(transform10);
			}
			GameObject gameObject = AssetManager.Load<GameObject>("WorldMap/Prefabs/Effects/WaterShrine", out _, false);
			if (gameObject)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
				gameObject2.transform.parent = block.transform;
				gameObject2.transform.localPosition = new Vector3(32.11f, -2.46f, -31.69f);
				block.AddForm2Transform(gameObject2.transform);
			}
			block.ApplyForm();
			return;
		}
		if (component.Beach1)
		{
			Transform transform11 = UnityEngine.Object.Instantiate<Transform>(component.Beach1);
			transform11.name = component.Beach1.name;
			transform11.parent = block.transform;
			transform11.localPosition = Vector3.zero;
			transform11.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh11 = transform11.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm1(sharedMesh11);
			block.AddWalkMeshForm2(sharedMesh11);
			block.AddForm1Transform(transform11);
			block.AddForm2Transform(transform11);
			block.HasBeach1 = true;
		}
		if (component.Beach2)
		{
			Transform transform12 = UnityEngine.Object.Instantiate<Transform>(component.Beach2);
			transform12.name = component.Beach2.name;
			transform12.parent = block.transform;
			transform12.localPosition = Vector3.zero;
			transform12.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh12 = transform12.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm1(sharedMesh12);
			block.AddWalkMeshForm2(sharedMesh12);
			block.AddForm1Transform(transform12);
			block.AddForm2Transform(transform12);
			block.HasBeach2 = true;
		}
		if (component.Stream)
		{
			Transform transform13 = UnityEngine.Object.Instantiate<Transform>(component.Stream);
			transform13.name = component.Stream.name;
			transform13.parent = block.transform;
			transform13.localPosition = Vector3.zero;
			transform13.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh13 = transform13.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm1(sharedMesh13);
			block.AddWalkMeshForm2(sharedMesh13);
			block.AddForm1Transform(transform13);
			block.AddForm2Transform(transform13);
			block.HasStream = true;
		}
		if (component.River)
		{
			Transform transform14 = UnityEngine.Object.Instantiate<Transform>(component.River);
			transform14.name = component.River.name;
			transform14.parent = block.transform;
			transform14.localPosition = Vector3.zero;
			transform14.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh14 = transform14.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm1(sharedMesh14);
			block.AddWalkMeshForm2(sharedMesh14);
			block.AddForm1Transform(transform14);
			block.AddForm2Transform(transform14);
			block.HasRiver = true;
		}
		if (component.RiverJoint)
		{
			Transform transform15 = UnityEngine.Object.Instantiate<Transform>(component.RiverJoint);
			transform15.name = component.RiverJoint.name;
			transform15.parent = block.transform;
			transform15.localPosition = Vector3.zero;
			transform15.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh15 = transform15.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm1(sharedMesh15);
			block.AddWalkMeshForm2(sharedMesh15);
			block.AddForm1Transform(transform15);
			block.AddForm2Transform(transform15);
			block.HasRiverJoint = true;
		}
		if (component.Falls)
		{
			Transform transform16 = UnityEngine.Object.Instantiate<Transform>(component.Falls);
			transform16.name = component.Falls.name;
			transform16.parent = block.transform;
			transform16.localPosition = Vector3.zero;
			transform16.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh16 = transform16.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm1(sharedMesh16);
			block.AddWalkMeshForm2(sharedMesh16);
			block.AddForm1Transform(transform16);
			block.AddForm2Transform(transform16);
			block.HasFalls = true;
		}
		if (component.Sea1)
		{
			Transform transform17 = UnityEngine.Object.Instantiate<Transform>(component.Sea1);
			transform17.name = component.Sea1.name;
			transform17.parent = block.transform;
			transform17.localPosition = Vector3.zero;
			transform17.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh17 = transform17.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm1(sharedMesh17);
			block.AddWalkMeshForm2(sharedMesh17);
			block.AddForm1Transform(transform17);
			block.AddForm2Transform(transform17);
			block.HasSea = true;
		}
		if (component.Sea2)
		{
			Transform transform18 = UnityEngine.Object.Instantiate<Transform>(component.Sea2);
			transform18.name = component.Sea2.name;
			transform18.parent = block.transform;
			transform18.localPosition = Vector3.zero;
			transform18.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh18 = transform18.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm1(sharedMesh18);
			block.AddWalkMeshForm2(sharedMesh18);
			block.AddForm1Transform(transform18);
			block.AddForm2Transform(transform18);
			block.HasSea = true;
		}
		if (component.Sea3)
		{
			Transform transform19 = UnityEngine.Object.Instantiate<Transform>(component.Sea3);
			transform19.name = component.Sea3.name;
			transform19.parent = block.transform;
			transform19.localPosition = Vector3.zero;
			transform19.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh19 = transform19.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm1(sharedMesh19);
			block.AddWalkMeshForm2(sharedMesh19);
			block.AddForm1Transform(transform19);
			block.AddForm2Transform(transform19);
			block.HasSea = true;
		}
		if (component.Sea4)
		{
			Transform transform20 = UnityEngine.Object.Instantiate<Transform>(component.Sea4);
			transform20.name = component.Sea4.name;
			transform20.parent = block.transform;
			transform20.localPosition = Vector3.zero;
			transform20.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh20 = transform20.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm1(sharedMesh20);
			block.AddWalkMeshForm2(sharedMesh20);
			block.AddForm1Transform(transform20);
			block.AddForm2Transform(transform20);
			block.HasSea = true;
		}
		if (component.Sea5)
		{
			Transform transform21 = UnityEngine.Object.Instantiate<Transform>(component.Sea5);
			transform21.name = component.Sea5.name;
			transform21.parent = block.transform;
			transform21.localPosition = Vector3.zero;
			transform21.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh21 = transform21.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm1(sharedMesh21);
			block.AddWalkMeshForm2(sharedMesh21);
			block.AddForm1Transform(transform21);
			block.AddForm2Transform(transform21);
			block.HasSea = true;
		}
		if (component.Sea6)
		{
			Transform transform22 = UnityEngine.Object.Instantiate<Transform>(component.Sea6);
			transform22.name = component.Sea6.name;
			transform22.parent = block.transform;
			transform22.localPosition = Vector3.zero;
			transform22.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh22 = transform22.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm1(sharedMesh22);
			block.AddWalkMeshForm2(sharedMesh22);
			block.AddForm1Transform(transform22);
			block.AddForm2Transform(transform22);
			block.HasSea = true;
		}
		if (component.VolcanoCrater1)
		{
			Transform transform23 = UnityEngine.Object.Instantiate<Transform>(component.VolcanoCrater1);
			transform23.name = component.VolcanoCrater1.name;
			transform23.parent = block.transform;
			transform23.localPosition = Vector3.zero;
			transform23.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh23 = transform23.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm1(sharedMesh23);
			block.AddForm1Transform(transform23);
			block.HasVolcanoCrater = true;
		}
		if (component.VolcanoLava1)
		{
			Transform transform24 = UnityEngine.Object.Instantiate<Transform>(component.VolcanoLava1);
			transform24.name = component.VolcanoLava1.name;
			transform24.parent = block.transform;
			transform24.localPosition = Vector3.zero;
			transform24.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh24 = transform24.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm1(sharedMesh24);
			block.AddForm1Transform(transform24);
			block.HasVolcanoLava = true;
		}
		if (component.VolcanoCrater2)
		{
			Transform transform25 = UnityEngine.Object.Instantiate<Transform>(component.VolcanoCrater2);
			transform25.name = component.VolcanoCrater2.name;
			transform25.parent = block.transform;
			transform25.localPosition = Vector3.zero;
			transform25.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh25 = transform25.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm2(sharedMesh25);
			block.AddForm2Transform(transform25);
			block.HasVolcanoCrater = true;
		}
		if (component.VolcanoLava2)
		{
			Transform transform26 = UnityEngine.Object.Instantiate<Transform>(component.VolcanoLava2);
			transform26.name = component.VolcanoLava2.name;
			transform26.parent = block.transform;
			transform26.localPosition = Vector3.zero;
			transform26.localScale = new Vector3(1f, 1f, 1f);
			Mesh sharedMesh26 = transform26.GetComponent<MeshFilter>().sharedMesh;
			block.AddWalkMeshForm2(sharedMesh26);
			block.AddForm2Transform(transform26);
			block.HasVolcanoLava = true;
		}
		if (block.Number == 91)
		{
			GameObject gameObject3 = AssetManager.Load<GameObject>("WorldMap/Prefabs/Effects/Quicksand", out _, false);
			if (gameObject3)
			{
				GameObject gameObject4 = UnityEngine.Object.Instantiate<GameObject>(gameObject3);
				gameObject4.transform.parent = block.transform;
				gameObject4.transform.localPosition = new Vector3(8f, 1.66f, -60f);
			}
		}
		if (block.Number == 115)
		{
			GameObject gameObject5 = AssetManager.Load<GameObject>("WorldMap/Prefabs/Effects/Quicksand", out _, false);
			if (gameObject5)
			{
				GameObject gameObject6 = UnityEngine.Object.Instantiate<GameObject>(gameObject5);
				gameObject6.transform.parent = block.transform;
				gameObject6.transform.localPosition = new Vector3(23.68f, 1.18f, -23.98f);
				gameObject6 = UnityEngine.Object.Instantiate<GameObject>(gameObject5);
				gameObject6.transform.parent = block.transform;
				gameObject6.transform.localPosition = new Vector3(4f, 1.18f, -16f);
				gameObject6 = UnityEngine.Object.Instantiate<GameObject>(gameObject5);
				gameObject6.transform.parent = block.transform;
				gameObject6.transform.localPosition = new Vector3(28f, 1.18f, -4f);
			}
		}
		if (block.Number == 158 && ff9.w_frameDisc == 4)
		{
			GameObject gameObject7 = AssetManager.Load<GameObject>("EmbeddedAsset/WorldMap_Local/Prefabs/Block[14][6] Object", out _, false);
			if (gameObject7)
			{
				Transform transform27 = block.transform.Find("Object");
				if (transform27)
				{
					Renderer component2 = transform27.GetComponent<Renderer>();
					component2.enabled = false;
				}
				GameObject gameObject8 = UnityEngine.Object.Instantiate<GameObject>(gameObject7);
				gameObject8.transform.parent = block.transform;
				gameObject8.transform.localPosition = new Vector3(0f, 0f, 0f);
			}
			gameObject7 = AssetManager.Load<GameObject>("EmbeddedAsset/WorldMap_Local/Prefabs/Block[14][6] Object_tree", out _, false);
			if (gameObject7)
			{
				GameObject gameObject9 = UnityEngine.Object.Instantiate<GameObject>(gameObject7);
				gameObject9.transform.parent = block.transform;
				gameObject9.transform.localPosition = new Vector3(0f, 0f, 0f);
			}
		}
		if (block.Number == 219)
		{
		}
		if (block.Number == 31)
		{
		}
		if (block.Number == 115)
		{
		}
		if (block.Number == 283)
		{
		}
		if (block.Number == 219)
		{
		}
		if (block.Number == 397)
		{
		}
		if (block.Number == 389)
		{
			GameObject gameObject10 = AssetManager.Load<GameObject>("EmbeddedAsset/WorldMap_Local/Prefabs/Block[5][16] Object", out _, false);
			if (gameObject10)
			{
				Transform transform28 = block.transform.Find("Object");
				if (transform28)
				{
					Renderer component3 = transform28.GetComponent<Renderer>();
					component3.enabled = false;
				}
				GameObject gameObject11 = UnityEngine.Object.Instantiate<GameObject>(gameObject10);
				gameObject11.transform.parent = block.transform;
				gameObject11.transform.localPosition = new Vector3(0f, 0f, 0f);
			}
		}
		block.ApplyForm();
	}

	private IEnumerator LoadBlockAsync(Int32 disc, WMBlock block)
	{
		this.loadingInBackground = true;
		this.BackgroundLoadingCount++;
		Int32 x = block.InitialX;
		Int32 y = block.InitialY;
		String blockName = String.Format("Block[{0}][{1}]", x, y);
		String prefabName = String.Format("WorldMap/Prefabs/WorldDisc{0}/r{1}/{2}", disc, y, blockName);
		AssetManagerRequest request = (AssetManagerRequest)null;
		Boolean loadFromBundle = AssetManager.ForceUseBundles;
		request = AssetManager.LoadAsync<GameObject>(prefabName);
		if (FF9StateSystem.World.PrintLogOnLoadingBlocksAsync)
		{
			global::Debug.Log("Loading " + blockName + " in the background. " + ((!loadFromBundle) ? "[Resources]" : "[Bundle]"));
		}
		yield return request;
		if (FF9StateSystem.World.PrintLogOnLoadingBlocksAsync)
		{
			global::Debug.Log("Done loading " + blockName);
		}
		GameObject blockObjectPrefab = (GameObject)request.asset;
		this.LoadBlock(blockObjectPrefab, block);
		block.IsReady = true;
		this.loadingInBackground = false;
		this.BackgroundLoadingCount--;
		yield break;
	}

	public void OnUpdate20FPS()
	{
		if (this.Settings.WrapWorld)
		{
			while (!this.Wrap())
			{
			}
		}
		if (ff9.w_moveActorPtr != (UnityEngine.Object)null)
		{
			Vector3 vector = new Vector3(0f, 0f, 0f);
			vector.x += ff9.w_moveActorPtr.transform.position.x;
			vector.z += ff9.w_moveActorPtr.transform.position.z;
			WMTweaker instance = Singleton<WMTweaker>.Instance;
			vector = new Vector3(0f, 0f, 0f);
			vector.x += ff9.w_moveActorPtr.transform.position.x;
			vector.z += ff9.w_moveActorPtr.transform.position.z;
			vector.y = instance.skydomeOffsetY;
			this.SkyDome_Sky.position = vector;
			vector.y = instance.cloudOffsetY;
			this.SkyDome_Bg.position = vector;
			this.SkyDome_Fog.position = vector + new Vector3(0f, -2f, 0f);
			Vector3 b = ff9.w_effectLastPos + new Vector3(0f, -15.15f, 0f);
			if (ff9.w_movePlanePtr != (UnityEngine.Object)null && ff9.w_frameCounter >= 10)
			{
				Vector3 absolutePositionOf = this.GetAbsolutePositionOf(ff9.w_frameCameraPtr.transform.position);
				Single num = Vector3.Distance(absolutePositionOf, b);
				if (num < 12f)
				{
					if (this.treeMeshRenderers == null || this.treeMeshRenderers.Length == 0)
					{
						this.treeMeshRenderers = new MeshRenderer[4];
					}
					if (this.treeMeshRenderers[0] == (UnityEngine.Object)null)
					{
						WMBlock wmblock = this.InitialBlocks[11, 4];
						if (wmblock.transform.childCount != 0)
						{
							MeshRenderer component = wmblock.transform.FindChild("Object").GetComponent<MeshRenderer>();
							this.treeMeshRenderers[0] = component;
						}
					}
					if (this.treeMeshRenderers[1] == (UnityEngine.Object)null)
					{
						WMBlock wmblock2 = this.InitialBlocks[11, 5];
						if (wmblock2.transform.childCount != 0)
						{
							MeshRenderer component2 = wmblock2.transform.FindChild("Object").GetComponent<MeshRenderer>();
							this.treeMeshRenderers[1] = component2;
						}
					}
					if (this.treeMeshRenderers[2] == (UnityEngine.Object)null)
					{
						WMBlock wmblock3 = this.InitialBlocks[12, 4];
						if (wmblock3.transform.childCount != 0)
						{
							MeshRenderer component3 = wmblock3.transform.FindChild("Object").GetComponent<MeshRenderer>();
							this.treeMeshRenderers[2] = component3;
						}
					}
					if (this.treeMeshRenderers[3] == (UnityEngine.Object)null)
					{
						WMBlock wmblock4 = this.InitialBlocks[12, 5];
						if (wmblock4.transform.childCount != 0)
						{
							MeshRenderer component4 = wmblock4.transform.FindChild("Object").GetComponent<MeshRenderer>();
							this.treeMeshRenderers[3] = component4;
						}
					}
					for (Int32 i = 0; i < (Int32)this.treeMeshRenderers.Length; i++)
					{
						this.treeMeshRenderers[i].enabled = false;
					}
				}
				else
				{
					for (Int32 j = 0; j < (Int32)this.treeMeshRenderers.Length; j++)
					{
						if (this.treeMeshRenderers[j] != (UnityEngine.Object)null && !this.treeMeshRenderers[j].enabled)
						{
							this.treeMeshRenderers[j].enabled = true;
						}
					}
				}
				if (this.HasAirGardenShadow && !this.DidCheckIfAirGardenIsInSpecialZone)
				{
					Vector3 absolutePositionOf2 = this.GetAbsolutePositionOf(this.AirGardenShadow.transform.position);
					Vector3 b2 = new Vector3(1307.2f, 26.1f, -644.7f);
					Single num2 = Vector3.Distance(absolutePositionOf2, b2);
					if (num2 < 30f)
					{
						this.AirGardenIsInSpecialZone = true;
					}
				}
			}
		}
	}

	private void DrawWalkMeshes()
	{
		for (Int32 i = 0; i < 20; i++)
		{
			for (Int32 j = 0; j < 24; j++)
			{
				WMBlock wmblock = this.InitialBlocks[j, i];
				if (wmblock.Number == 417)
				{
					if (wmblock.ActiveWalkMeshes == null)
					{
						break;
					}
					for (Int32 k = 0; k < wmblock.ActiveWalkMeshes.Count; k++)
					{
						if (k == this.myActiveWalkMesh)
						{
							WMMesh wmmesh = wmblock.ActiveWalkMeshes[k];
							Vector3[] vertices = wmmesh.Vertices;
							Int32[] triangles = wmmesh.Triangles;
							Vector3[] normals = wmmesh.Normals;
							Transform transform = wmblock.Transform;
							Vector4[] tangents = wmmesh.Tangents;
							for (Int32 l = 0; l < (Int32)vertices.Length / 3; l++)
							{
								if (this.myTriangleIndex == l)
								{
									Vector3 vector = vertices[triangles[l * 3]];
									Vector3 vector2 = vertices[triangles[l * 3 + 1]];
									Vector3 vector3 = vertices[triangles[l * 3 + 2]];
									vector = transform.TransformPoint(vector);
									vector2 = transform.TransformPoint(vector2);
									vector3 = transform.TransformPoint(vector3);
									global::Debug.DrawLine(vector, vector2, Color.red, ff9.honoUpdateTime, true);
									global::Debug.DrawLine(vector2, vector3, Color.red, ff9.honoUpdateTime, true);
									global::Debug.DrawLine(vector3, vector, Color.red, ff9.honoUpdateTime, true);
									Int32 num = (Int32)tangents[triangles[l * 3]].x;
								}
							}
						}
					}
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.I))
		{
			this.myTriangleIndex--;
		}
		else if (Input.GetKeyDown(KeyCode.O))
		{
			this.myTriangleIndex++;
		}
		else if (Input.GetKeyDown(KeyCode.L))
		{
			this.myTriangleIndex++;
		}
	}

	private void TestWalkMeshes()
	{
		for (Int32 i = 0; i < 20; i++)
		{
			for (Int32 j = 0; j < 24; j++)
			{
				WMBlock wmblock = this.InitialBlocks[j, i];
				if (wmblock.ActiveWalkMeshes == null)
				{
					break;
				}
				if (wmblock.Number != 358)
				{
					break;
				}
				for (Int32 k = 0; k < wmblock.ActiveWalkMeshes.Count; k++)
				{
					WMMesh wmmesh = wmblock.ActiveWalkMeshes[k];
					Vector3[] vertices = wmmesh.Vertices;
					Int32[] triangles = wmmesh.Triangles;
					Vector3[] normals = wmmesh.Normals;
					Transform transform = wmblock.Transform;
					Vector4[] tangents = wmmesh.Tangents;
					for (Int32 l = 0; l < (Int32)vertices.Length / 3; l++)
					{
						Vector3 vector = vertices[triangles[l * 3]];
						Vector3 vector2 = vertices[triangles[l * 3 + 1]];
						Vector3 vector3 = vertices[triangles[l * 3 + 2]];
						vector = transform.TransformPoint(vector);
						vector2 = transform.TransformPoint(vector2);
						vector3 = transform.TransformPoint(vector3);
						Int32 num = (Int32)tangents[triangles[l * 3]].x;
						global::Debug.DrawLine(vector, vector2, Color.yellow, ff9.honoUpdateTime, true);
						global::Debug.DrawLine(vector2, vector3, Color.yellow, ff9.honoUpdateTime, true);
						global::Debug.DrawLine(vector3, vector, Color.yellow, ff9.honoUpdateTime, true);
					}
				}
			}
		}
	}

	public void OnUpdate()
	{
	}

	private void DrawMoveCache(ff9.s_moveCHRCache cache, Color color, Single time)
	{
		if (cache != null)
		{
			for (Int32 i = 0; i < 10; i++)
			{
				Int32 num = (cache.Number + i) % 10;
				WMBlock wmblock = cache.Blocks[num];
				if (!(wmblock == (UnityEngine.Object)null))
				{
					Int32 num2 = cache.TriangleIndices[num];
					WMMesh wmmesh = wmblock.ActiveWalkMeshes[cache.WalkMeshIndices[num]];
					Vector3[] vertices = wmmesh.Vertices;
					Int32[] triangles = wmmesh.Triangles;
					Transform transform = wmblock.Transform;
					Vector3 vector = vertices[triangles[num2 * 3]];
					Vector3 vector2 = vertices[triangles[num2 * 3 + 1]];
					Vector3 vector3 = vertices[triangles[num2 * 3 + 2]];
					vector = transform.TransformPoint(vector);
					vector2 = transform.TransformPoint(vector2);
					vector3 = transform.TransformPoint(vector3);
					global::Debug.DrawLine(vector, vector2, color, time, true);
					global::Debug.DrawLine(vector2, vector3, color, time, true);
					global::Debug.DrawLine(vector3, vector, color, time, true);
				}
			}
		}
	}

	private Boolean Wrap()
	{
		if (ff9.w_moveActorPtr == (UnityEngine.Object)null)
		{
			return true;
		}
		if (ff9.w_moveActorPtr == ff9.w_moveDummyCharacter)
		{
			return true;
		}
		Vector3 pos = ff9.w_moveActorPtr.pos;
		Boolean result = true;
		if (pos.x < this.wrapWorldLeftBound)
		{
			this.ShiftRightAllBlocks();
			result = false;
		}
		if (pos.x > this.wrapWorldRightBound)
		{
			this.ShiftLeftAllBlocks();
			result = false;
		}
		if (pos.z > this.wrapWorldUpperBound)
		{
			this.ShiftDownAllBlocks();
			result = false;
		}
		if (pos.z < this.wrapWorldLowerBound)
		{
			this.ShiftUpAllBlocks();
			result = false;
		}
		return result;
	}

	private void RetrieveUsingTextures()
	{
		this.textures = new List<Texture2D>();
		foreach (Object obj in this.root.transform)
		{
			Transform transform = (Transform)obj;
			Boolean flag = false;
			foreach (Object obj2 in transform)
			{
				Transform transform2 = (Transform)obj2;
				Renderer component = transform2.GetComponent<Renderer>();
				if (component)
				{
					Texture2D item = transform2.GetComponent<Renderer>().sharedMaterial.mainTexture as Texture2D;
					if (!this.textures.Contains(item))
					{
						this.textures.Add(item);
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				break;
			}
		}
	}

	public void SetTextureFilterMode(FilterMode mode)
	{
		foreach (Texture2D texture2D in this.textures)
		{
			texture2D.filterMode = mode;
		}
	}

	public void LoadBlocks(Boolean loadOnlyInSight)
	{
		if (this.LoadState != WMWorldLoadState.ReadyToLoadBlocks)
		{
			global::Debug.LogWarning("LoadState is not ReadyToLoad!");
			return;
		}
		WMProfiler.Begin("WMWorld.LoadBlocks()");
		String arg = String.Format("Block[{0}][{1}]f", 12, 0);
		String name = String.Format("WorldMap/Prefabs/WorldDisc{0}/r{1}/{2}", 1, 0, arg);
		this.SeaBlockPrefab = AssetManager.Load<GameObject>(name, out _, false);
		this.DetectUnseenBlocks();
		for (Int32 i = 0; i < 20; i++)
		{
			for (Int32 j = 0; j < 24; j++)
			{
				WMBlock wmblock = this.Blocks[j, i];
				if (loadOnlyInSight)
				{
					if (wmblock.IsInsideSight && !wmblock.IsReady)
					{
						this.LoadBlock(this.currentDisc, wmblock);
						wmblock.IsReady = true;
					}
				}
				else if (!wmblock.IsReady)
				{
					this.LoadBlock(this.currentDisc, wmblock);
					wmblock.IsReady = true;
				}
			}
		}
		this.LoadState = WMWorldLoadState.LoadingTheRestOfBlocksInBackground;
		GameObject gameObject = GameObject.Find("obj9_WM");
		if (gameObject)
		{
			this.AirGardenShadow = gameObject.transform;
			this.HasAirGardenShadow = true;
		}
		WMProfiler.End();
	}

	private void CheckIfLoadingBlocksIsFinished()
	{
		this.ActiveBlockCount = 0;
		for (Int32 i = 0; i < 20; i++)
		{
			for (Int32 j = 0; j < 24; j++)
			{
				WMBlock wmblock = this.Blocks[j, i];
				if (wmblock.IsReady)
				{
					this.ActiveBlockCount++;
				}
			}
		}
		if (this.ActiveBlockCount == 480)
		{
			this.FinishedLoadingBlocks = true;
			this.LoadState = WMWorldLoadState.Finished;
			global::Debug.Log("Finished Loading Blocks!");
		}
		else
		{
			this.FinishedLoadingBlocks = false;
		}
	}

	private void UpdateLoadBlocks()
	{
		if (this.Blocks == null)
		{
			return;
		}
		Int32 length = this.Blocks.GetLength(0);
		Int32 length2 = this.Blocks.GetLength(1);
		this.DetectUnseenBlocks();
		for (Int32 i = 0; i < length; i++)
		{
			for (Int32 j = 0; j < length2; j++)
			{
				WMBlock wmblock = this.Blocks[i, j];
				if (!wmblock.StartedLoadAsync && !wmblock.IsReady && wmblock.IsInsideSight)
				{
					if (wmblock.IsSea)
					{
						this.LoadBlock(this.SeaBlockPrefab, wmblock);
						wmblock.IsReady = true;
					}
					else if (this.BackgroundLoadingCount < FF9StateSystem.World.MaximumLoadAsynce)
					{
						base.StartCoroutine(this.LoadBlockAsync(this.currentDisc, wmblock));
						wmblock.StartedLoadAsync = true;
					}
				}
			}
		}
		for (Int32 k = 0; k < length; k++)
		{
			for (Int32 l = 0; l < length2; l++)
			{
				WMBlock wmblock2 = this.Blocks[k, l];
				if (!wmblock2.IsInsideSight && FF9StateSystem.World.DiscardBlockWhenStreaming)
				{
					if (wmblock2.transform.childCount != 0)
					{
						wmblock2.Form1Transforms.Clear();
						wmblock2.Form2Transforms.Clear();
						foreach (Object obj in wmblock2.transform)
						{
							Transform transform = (Transform)obj;
							UnityEngine.Object.Destroy(transform.gameObject);
						}
					}
					wmblock2.IsReady = false;
					wmblock2.StartedLoadAsync = false;
				}
			}
		}
	}

	public void ShiftRightAllBlocks()
	{
		for (Int32 i = 0; i < 20; i++)
		{
			this.ShiftBlocks(i, 0, WMWorld.ShiftDirection.Right);
		}
		for (ObjList objList = this.ActorList; objList != null; objList = objList.next)
		{
			if (objList.obj.cid == 4)
			{
				WMActor wmActor = ((Actor)objList.obj).wmActor;
				Vector3 position = wmActor.transform.position;
				position.x += 64f;
				if (position.x >= 1536f)
				{
					position.x -= 1536f;
				}
				wmActor.pos = position;
				wmActor.lastx = position.x;
				wmActor.lasty = position.y;
				wmActor.lastz = position.z;
			}
		}
		this.WorldSPSSystem.ShiftRight();
		for (Int32 j = 0; j < (Int32)this.kWorldPackEffectThunder1s.Length; j++)
		{
			Transform transform = this.kWorldPackEffectThunder1s[j];
			if (transform)
			{
				Vector3 position2 = transform.transform.position;
				position2.x += 64f;
				if (position2.x >= 1536f)
				{
					position2.x -= 1536f;
				}
				transform.transform.position = position2;
			}
		}
		for (Int32 k = 0; k < (Int32)this.kWorldPackEffectThunder2s.Length; k++)
		{
			Transform transform2 = this.kWorldPackEffectThunder2s[k];
			if (transform2)
			{
				Vector3 position3 = transform2.transform.position;
				position3.x += 64f;
				if (position3.x >= 1536f)
				{
					position3.x -= 1536f;
				}
				transform2.transform.position = position3;
			}
		}
	}

	private void ShiftLeftAllBlocks()
	{
		for (Int32 i = 0; i < 20; i++)
		{
			this.ShiftBlocks(i, 0, WMWorld.ShiftDirection.Left);
		}
		for (ObjList objList = this.ActorList; objList != null; objList = objList.next)
		{
			if (objList.obj.cid == 4)
			{
				WMActor wmActor = ((Actor)objList.obj).wmActor;
				Vector3 position = wmActor.transform.position;
				position.x -= 64f;
				if (position.x < 0f)
				{
					position.x += 1536f;
				}
				wmActor.pos = position;
				wmActor.lastx = position.x;
				wmActor.lasty = position.y;
				wmActor.lastz = position.z;
			}
		}
		this.WorldSPSSystem.ShiftLeft();
		for (Int32 j = 0; j < (Int32)this.kWorldPackEffectThunder1s.Length; j++)
		{
			Transform transform = this.kWorldPackEffectThunder1s[j];
			if (transform)
			{
				Vector3 position2 = transform.transform.position;
				position2.x -= 64f;
				if (position2.x < 0f)
				{
					position2.x += 1536f;
				}
				transform.transform.position = position2;
			}
		}
		for (Int32 k = 0; k < (Int32)this.kWorldPackEffectThunder2s.Length; k++)
		{
			Transform transform2 = this.kWorldPackEffectThunder2s[k];
			if (transform2)
			{
				Vector3 position3 = transform2.transform.position;
				position3.x -= 64f;
				if (position3.x < 0f)
				{
					position3.x += 1536f;
				}
				transform2.transform.position = position3;
			}
		}
	}

	private void ShiftDownAllBlocks()
	{
		for (Int32 i = 0; i < 24; i++)
		{
			this.ShiftBlocks(0, i, WMWorld.ShiftDirection.Down);
		}
		for (ObjList objList = this.ActorList; objList != null; objList = objList.next)
		{
			if (objList.obj.cid == 4)
			{
				WMActor wmActor = ((Actor)objList.obj).wmActor;
				Vector3 position = wmActor.transform.position;
				position.z -= 64f;
				if (position.z <= -1280f)
				{
					position.z += 1280f;
				}
				wmActor.pos = position;
				wmActor.lastx = position.x;
				wmActor.lasty = position.y;
				wmActor.lastz = position.z;
			}
		}
		this.WorldSPSSystem.ShiftDown();
		for (Int32 j = 0; j < (Int32)this.kWorldPackEffectThunder1s.Length; j++)
		{
			Transform transform = this.kWorldPackEffectThunder1s[j];
			if (transform)
			{
				Vector3 position2 = transform.transform.position;
				position2.z -= 64f;
				if (position2.z <= -1280f)
				{
					position2.z += 1280f;
				}
				transform.transform.position = position2;
			}
		}
		for (Int32 k = 0; k < (Int32)this.kWorldPackEffectThunder2s.Length; k++)
		{
			Transform transform2 = this.kWorldPackEffectThunder2s[k];
			if (transform2)
			{
				Vector3 position3 = transform2.transform.position;
				position3.z -= 64f;
				if (position3.z <= -1280f)
				{
					position3.z += 1280f;
				}
				transform2.transform.position = position3;
			}
		}
	}

	private void ShiftUpAllBlocks()
	{
		for (Int32 i = 0; i < 24; i++)
		{
			this.ShiftBlocks(0, i, WMWorld.ShiftDirection.Up);
		}
		for (ObjList objList = this.ActorList; objList != null; objList = objList.next)
		{
			if (objList.obj.cid == 4)
			{
				WMActor wmActor = ((Actor)objList.obj).wmActor;
				Vector3 position = wmActor.transform.position;
				position.z += 64f;
				if (position.z > 0f)
				{
					position.z -= 1280f;
				}
				wmActor.pos = position;
				wmActor.lastx = position.x;
				wmActor.lasty = position.y;
				wmActor.lastz = position.z;
			}
		}
		this.WorldSPSSystem.ShiftUp();
		for (Int32 j = 0; j < (Int32)this.kWorldPackEffectThunder1s.Length; j++)
		{
			Transform transform = this.kWorldPackEffectThunder1s[j];
			if (transform)
			{
				Vector3 position2 = transform.transform.position;
				position2.z += 64f;
				if (position2.z > 0f)
				{
					position2.z -= 1280f;
				}
				transform.transform.position = position2;
			}
		}
		for (Int32 k = 0; k < (Int32)this.kWorldPackEffectThunder2s.Length; k++)
		{
			Transform transform2 = this.kWorldPackEffectThunder2s[k];
			if (transform2)
			{
				Vector3 position3 = transform2.transform.position;
				position3.z += 64f;
				if (position3.z > 0f)
				{
					position3.z -= 1280f;
				}
				transform2.transform.position = position3;
			}
		}
	}

	public void ShiftBlocks(Int32 row, Int32 column, WMWorld.ShiftDirection direction)
	{
		if (direction == WMWorld.ShiftDirection.Left)
		{
			for (Int32 i = 0; i < 23; i++)
			{
				this.SwapBlock(i, row, i + 1, row);
			}
			for (Int32 j = 0; j < 24; j++)
			{
				WMBlock wmblock = this.Blocks[j, row];
				Vector3 position = wmblock.transform.position;
				position.x -= 64f;
				if (position.x < 0f)
				{
					position.x += 1536f;
				}
				wmblock.transform.position = position;
			}
		}
		else if (direction == WMWorld.ShiftDirection.Right)
		{
			for (Int32 k = 23; k > 0; k--)
			{
				this.SwapBlock(k, row, k - 1, row);
			}
			for (Int32 l = 0; l < 24; l++)
			{
				WMBlock wmblock2 = this.Blocks[l, row];
				Vector3 position2 = wmblock2.transform.position;
				position2.x += 64f;
				if (position2.x >= 1536f)
				{
					position2.x -= 1536f;
				}
				wmblock2.transform.position = position2;
			}
		}
		else if (direction == WMWorld.ShiftDirection.Up)
		{
			for (Int32 m = 0; m < 19; m++)
			{
				this.SwapBlock(column, m, column, m + 1);
			}
			for (Int32 n = 0; n < 20; n++)
			{
				WMBlock wmblock3 = this.Blocks[column, n];
				Vector3 position3 = wmblock3.transform.position;
				position3.z += 64f;
				if (position3.z > 0f)
				{
					position3.z -= 1280f;
				}
				wmblock3.transform.position = position3;
			}
		}
		else if (direction == WMWorld.ShiftDirection.Down)
		{
			for (Int32 num = 19; num > 0; num--)
			{
				this.SwapBlock(column, num, column, num - 1);
			}
			for (Int32 num2 = 0; num2 < 20; num2++)
			{
				WMBlock wmblock4 = this.Blocks[column, num2];
				Vector3 position4 = wmblock4.transform.position;
				position4.z -= 64f;
				if (position4.z <= -1280f)
				{
					position4.z += 1280f;
				}
				wmblock4.transform.position = position4;
			}
		}
	}

	private void SwapBlock(Int32 x1, Int32 y1, Int32 x2, Int32 y2)
	{
		WMBlock wmblock = this.Blocks[x1, y1];
		Int32 currentX = wmblock.CurrentX;
		Int32 currentY = wmblock.CurrentY;
		this.Blocks[x1, y1].CurrentX = this.Blocks[x2, y2].CurrentX;
		this.Blocks[x1, y1].CurrentY = this.Blocks[x2, y2].CurrentY;
		this.Blocks[x1, y1] = this.Blocks[x2, y2];
		this.Blocks[x2, y2].CurrentX = currentX;
		this.Blocks[x2, y2].CurrentY = currentY;
		this.Blocks[x2, y2] = wmblock;
	}

	private void DetectUnseenBlocks()
	{
		if (FF9StateSystem.Settings.IsFastForward)
		{
			this.DetectUnseenBlocksFastForward();
		}
		else
		{
			this.DetectUnseenBlocksNormal();
		}
	}

	private void DetectUnseenBlocksNormal()
	{
		Int32 length = this.Blocks.GetLength(0);
		Int32 length2 = this.Blocks.GetLength(1);
		WMBlock wmblock;
		for (Int32 i = 0; i < length; i++)
		{
			for (Int32 j = 0; j < length2; j++)
			{
				wmblock = this.Blocks[i, j];
				wmblock.IsInsideSight = false;
			}
		}
		WMBlock absoluteBlock = this.GetAbsoluteBlock(this.SkyDome_Sky);
		Int32 currentX = absoluteBlock.CurrentX;
		Int32 currentY = absoluteBlock.CurrentY;
		wmblock = this.Blocks[currentX - 2, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 2, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 2, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 2, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 2, currentY + 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY + 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY + 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY + 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY + 2];
		wmblock.IsInsideSight = true;
	}

	private void DetectUnseenBlocksFastForward()
	{
		Int32 length = this.Blocks.GetLength(0);
		Int32 length2 = this.Blocks.GetLength(1);
		WMBlock wmblock;
		for (Int32 i = 0; i < length; i++)
		{
			for (Int32 j = 0; j < length2; j++)
			{
				wmblock = this.Blocks[i, j];
				wmblock.IsInsideSight = false;
			}
		}
		WMBlock absoluteBlock = this.GetAbsoluteBlock(this.SkyDome_Sky);
		Int32 currentX = absoluteBlock.CurrentX;
		Int32 currentY = absoluteBlock.CurrentY;
		wmblock = this.Blocks[currentX - 3, currentY - 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 2, currentY - 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY - 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY - 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY - 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY - 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 3, currentY - 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 3, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 2, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 3, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 3, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 2, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 3, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 3, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 2, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 3, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 3, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 2, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 3, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 3, currentY + 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 2, currentY + 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY + 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY + 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY + 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY + 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 3, currentY + 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 3, currentY + 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 2, currentY + 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY + 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY + 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY + 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY + 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 3, currentY + 3];
		wmblock.IsInsideSight = true;
	}

	private void DetectUnseenBlocksBy(Int32 radius)
	{
		Int32 length = this.Blocks.GetLength(0);
		Int32 length2 = this.Blocks.GetLength(1);
		WMBlock wmblock;
		for (Int32 i = 0; i < length; i++)
		{
			for (Int32 j = 0; j < length2; j++)
			{
				wmblock = this.Blocks[i, j];
				wmblock.IsInsideSight = false;
			}
		}
		WMBlock absoluteBlock = this.GetAbsoluteBlock(this.SkyDome_Sky);
		Int32 currentX = absoluteBlock.CurrentX;
		Int32 currentY = absoluteBlock.CurrentY;
		wmblock = this.Blocks[currentX - 3, currentY - 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 2, currentY - 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY - 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY - 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY - 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY - 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 3, currentY - 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 3, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 2, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 3, currentY - 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 3, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 2, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 3, currentY - 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 3, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 2, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 3, currentY];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 3, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 2, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 3, currentY + 1];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 3, currentY + 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 2, currentY + 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY + 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY + 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY + 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY + 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 3, currentY + 2];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 3, currentY + 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 2, currentY + 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX - 1, currentY + 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX, currentY + 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 1, currentY + 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 2, currentY + 3];
		wmblock.IsInsideSight = true;
		wmblock = this.Blocks[currentX + 3, currentY + 3];
		wmblock.IsInsideSight = true;
	}

	public void ResetBlockForms()
	{
		for (Int32 i = 0; i < 24; i++)
		{
			for (Int32 j = 0; j < 20; j++)
			{
				WMBlock wmblock = this.InitialBlocks[i, j];
				wmblock.SetForm(1);
				wmblock.ApplyForm();
			}
		}
	}

	public void SetBlockForms(Int32 form)
	{
		for (Int32 i = 0; i < 24; i++)
		{
			for (Int32 j = 0; j < 20; j++)
			{
				WMBlock wmblock = this.InitialBlocks[i, j];
				wmblock.SetForm(form);
				wmblock.ApplyForm();
			}
		}
	}

	public void SetDisc(Int32 disc)
	{
		if (disc != 1 && disc != 4)
		{
			global::Debug.LogError("Only disc1 and dic4 are available.");
		}
		if (disc != this.currentDisc)
		{
			ff9.w_frameDisc = (Byte)disc;
			this.currentDisc = disc;
			SceneDirector.Replace("WorldMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
	}

	public static WMBlock[,] BuildBlockArray(Transform worldDisc)
	{
		WMBlock[,] array = new WMBlock[24, 20];
		for (Int32 i = 0; i < 24; i++)
		{
			for (Int32 j = 0; j < 20; j++)
			{
				foreach (Object obj in worldDisc.transform)
				{
					Transform transform = (Transform)obj;
					WMBlock component = transform.GetComponent<WMBlock>();
					if (component.InitialX == i && component.InitialY == j)
					{
						array[i, j] = component;
					}
				}
			}
		}
		return array;
	}

	public WMBlock Block31
	{
		get
		{
			if (this._block31 == (UnityEngine.Object)null)
			{
				this._block31 = this.InitialBlocks[7, 1];
			}
			return this._block31;
		}
	}

	public WMBlock Block115
	{
		get
		{
			if (this._block115 == (UnityEngine.Object)null)
			{
				this._block115 = this.InitialBlocks[19, 14];
			}
			return this._block115;
		}
	}

	public WMBlock Block219
	{
		get
		{
			if (this._block219 == (UnityEngine.Object)null)
			{
				this._block219 = this.InitialBlocks[3, 9];
			}
			return this._block219;
		}
	}

	public Transform WorldMapRoot
	{
		get
		{
			if (this._worldMapRoot == (UnityEngine.Object)null)
			{
				this._worldMapRoot = GameObject.Find("WorldMapRoot").transform;
			}
			return this._worldMapRoot;
		}
	}

	public Transform WorldMapEffectRoot
	{
		get
		{
			if (this._worldMapEffectRoot == (UnityEngine.Object)null)
			{
				this._worldMapEffectRoot = new GameObject("EffectRoot").transform;
				this._worldMapEffectRoot.transform.parent = this.WorldMapRoot;
			}
			return this._worldMapEffectRoot;
		}
	}

	public Transform GetkWorldPackEffectThunder1()
	{
		return this.kWorldPackEffectThunder1s[this.kWorldPackEffectThunder1Index++ % 10];
	}

	public Transform GetkWorldPackEffectThunder2()
	{
		return this.kWorldPackEffectThunder2s[this.kWorldPackEffectThunder2Index++ % 10];
	}

	public Transform GetkWorldPackEffectArch()
	{
		return this.kWorldPackEffectArchs[this.kWorldPackEffectArchIndex++ % 10];
	}

	public void LoadEffects()
	{
		if (ff9.w_frameDisc == 1)
		{
			this.kWorldPackEffectTwister = this.LoadEffect("kWorldPackEffectTwister");
			this.kWorldPackEffectSpiral0 = this.LoadEffect("kWorldPackEffectSpiral0");
			this.kWorldPackEffectSpiral1 = this.LoadEffect("kWorldPackEffectSpiral1");
			this.kWorldPackEffectSpiral2 = this.LoadEffect("kWorldPackEffectSpiral2");
			this.kWorldPackEffectCore = this.LoadEffect("kWorldPackEffectCore");
			this.SetTwisterRenderQueue(2450);
		}
		this.kWorldPackEffectWindmill = this.LoadEffect("kWorldPackEffectWindmill");
		if (ff9.w_frameDisc == 4 || Singleton<WMTweaker>.Instance.HaskEffectBlockEva)
		{
			this.kWorldPackEffectSphere1 = this.LoadEffect("kWorldPackEffectSphere1");
			this.kWorldPackEffectSphere2 = this.LoadEffect("kWorldPackEffectSphere2");
			this.kWorldPackEffectArchs = this.LoadEffects("kWorldPackEffectArch", 10, true);
			this.kWorldPackEffectBlack = this.LoadEffect("kWorldPackEffectBlack");
			this.kWorldPackEffectThunder1s = this.LoadEffects("kWorldPackEffectThunder1", 10, false);
			this.kWorldPackEffectThunder2s = this.LoadEffects("kWorldPackEffectThunder2", 10, false);
		}
	}

	public void SetTwisterRenderQueue(Int32 startRenderQueue)
	{
		Material material = this.kWorldPackEffectCore.GetComponentInChildren<Renderer>().material;
		material.renderQueue = startRenderQueue;
		Material material2 = this.kWorldPackEffectTwister.GetComponentInChildren<Renderer>().material;
		material2.renderQueue = startRenderQueue + 1;
		Material material3 = this.kWorldPackEffectSpiral0.GetComponentInChildren<Renderer>().material;
		material3.renderQueue = startRenderQueue + 1;
		Material material4 = this.kWorldPackEffectSpiral1.GetComponentInChildren<Renderer>().material;
		material4.renderQueue = startRenderQueue + 1;
		Material material5 = this.kWorldPackEffectSpiral2.GetComponentInChildren<Renderer>().material;
		material5.renderQueue = startRenderQueue + 1;
	}

	private Transform LoadEffect(String effect)
	{
		Transform transform = AssetManager.Load<GameObject>("WorldMap/Prefabs/Effects/" + effect, out _, false).transform;
		Transform transform2 = UnityEngine.Object.Instantiate<Transform>(transform);
		transform2.parent = this.WorldMapEffectRoot;
		return transform2;
	}

	private Transform[] LoadEffects(String effect, Int32 count, Boolean createNewMaterial = false)
	{
		Transform transform = AssetManager.Load<GameObject>("WorldMap/Prefabs/Effects/" + effect, out _, false).transform;
		Transform[] array = new Transform[count];
		for (Int32 i = 0; i < count; i++)
		{
			array[i] = UnityEngine.Object.Instantiate<Transform>(transform);
			array[i].parent = this.WorldMapEffectRoot;
		}
		return array;
	}

	public void LoadSPS()
	{
		this.WorldSPSSystem = new GameObject("WorldSPSSystem").AddComponent<WorldSPSSystem>();
		this.WorldSPSSystem.transform.parent = this.WorldMapRoot;
		this.WorldSPSSystem.Init();
	}

	public void LoadChocoboTextures()
	{
		this.NormalChocoboTexture = new Texture2D[3];
		this.AsaseChocoboTexture = new Texture2D[3];
		this.YamaChocoboTexture = new Texture2D[3];
		this.UmiChocoboTexture = new Texture2D[3];
		this.SoraChocoboTexture = new Texture2D[3];
		this.NormalChocoboTexture[0] = this.LoadChocoboTexture("00NormalMaterial", 0);
		this.NormalChocoboTexture[1] = this.LoadChocoboTexture("00NormalMaterial", 1);
		this.NormalChocoboTexture[2] = this.LoadChocoboTexture("00NormalMaterial", 2);
		this.AsaseChocoboTexture[0] = this.LoadChocoboTexture("01AssaseMaterial", 0);
		this.AsaseChocoboTexture[1] = this.LoadChocoboTexture("01AssaseMaterial", 1);
		this.AsaseChocoboTexture[2] = this.LoadChocoboTexture("01AssaseMaterial", 2);
		this.YamaChocoboTexture[0] = this.LoadChocoboTexture("02YamaMaterial", 0);
		this.YamaChocoboTexture[1] = this.LoadChocoboTexture("02YamaMaterial", 1);
		this.YamaChocoboTexture[2] = this.LoadChocoboTexture("02YamaMaterial", 2);
		this.UmiChocoboTexture[0] = this.LoadChocoboTexture("03UmiMaterial", 0);
		this.UmiChocoboTexture[1] = this.LoadChocoboTexture("03UmiMaterial", 1);
		this.UmiChocoboTexture[2] = this.LoadChocoboTexture("03UmiMaterial", 2);
		this.SoraChocoboTexture[0] = this.LoadChocoboTexture("04SoraMaterial", 0);
		this.SoraChocoboTexture[1] = this.LoadChocoboTexture("04SoraMaterial", 1);
		this.SoraChocoboTexture[2] = this.LoadChocoboTexture("04SoraMaterial", 2);
	}

	public void LoadCurrentChocoboTexture(Int32 index)
	{
		this.CurrentChocoboTextures = new Texture2D[3];
		if (index == 0)
		{
			this.CurrentChocoboTextures[0] = this.LoadChocoboTexture("00NormalMaterial", 0);
			this.CurrentChocoboTextures[1] = this.LoadChocoboTexture("00NormalMaterial", 1);
			this.CurrentChocoboTextures[2] = this.LoadChocoboTexture("00NormalMaterial", 2);
		}
		else if (index == 1)
		{
			this.CurrentChocoboTextures[0] = this.LoadChocoboTexture("01AssaseMaterial", 0);
			this.CurrentChocoboTextures[1] = this.LoadChocoboTexture("01AssaseMaterial", 1);
			this.CurrentChocoboTextures[2] = this.LoadChocoboTexture("01AssaseMaterial", 2);
		}
		else if (index == 2)
		{
			this.CurrentChocoboTextures[0] = this.LoadChocoboTexture("02YamaMaterial", 0);
			this.CurrentChocoboTextures[1] = this.LoadChocoboTexture("02YamaMaterial", 1);
			this.CurrentChocoboTextures[2] = this.LoadChocoboTexture("02YamaMaterial", 2);
		}
		else if (index == 3)
		{
			this.CurrentChocoboTextures[0] = this.LoadChocoboTexture("03UmiMaterial", 0);
			this.CurrentChocoboTextures[1] = this.LoadChocoboTexture("03UmiMaterial", 1);
			this.CurrentChocoboTextures[2] = this.LoadChocoboTexture("03UmiMaterial", 2);
		}
		else if (index == 4)
		{
			this.CurrentChocoboTextures[0] = this.LoadChocoboTexture("04SoraMaterial", 0);
			this.CurrentChocoboTextures[1] = this.LoadChocoboTexture("04SoraMaterial", 1);
			this.CurrentChocoboTextures[2] = this.LoadChocoboTexture("04SoraMaterial", 2);
		}
	}

	private Texture2D LoadChocoboTexture(String chocoboName, Int32 index)
	{
		String str = String.Empty;
		switch (index)
		{
		case 0:
			str = "geo_sub_w0_003_0";
			break;
		case 1:
			str = "geo_sub_w0_003_1";
			break;
		case 2:
			str = "geo_sub_w0_003_2";
			break;
		}
		String[] pngInfo;
		return AssetManager.Load<Texture2D>("EmbeddedAsset/WorldMap_Local/Characters/Chocobo/" + chocoboName + "/" + str, out pngInfo, false);
	}

	public void LoadQuicksandMaterial()
	{
		this.QuicksandMaterial = AssetManager.Load<Material>("WorldMap/Materials/quicksand_mat", out _, false);
		String[] pngInfo;
		Texture2D texture = AssetManager.Load<Texture2D>("EmbeddedAsset/WorldMap_Local/Textures/SeamlessRock", out pngInfo, false);
		this.QuicksandMaterial.SetTexture("_DetailTex", texture);
	}

	public void LoadWaterShrineMaterial()
	{
		this.WaterShrineMaterial = AssetManager.Load<Material>("WorldMap/Materials/WaterShrine_mat", out _, false);
	}

	public void CreateProjectionMatrix()
	{
		this.MainCamera.projectionMatrix = this.PsxProj2UnityProj((Single)this.PsxGeomScreen, (Single)this.ClipDistance);
	}

	private Matrix4x4 PsxProj2UnityProj(Single psxGeomScreen, Single clipDistance)
	{
		Single num = 0.625f;
		Single num2 = 0.4375f;
		Single num3 = psxGeomScreen + clipDistance;
		return this.PerspectiveOffCenter(-num, num, -num2, num2, psxGeomScreen * 0.00390625f, num3 * 0.00390625f);
	}

	private Matrix4x4 PerspectiveOffCenter(Single left, Single right, Single bottom, Single top, Single near, Single far)
	{
		Single num = this.OffCenterOffsetX * 0.00390625f;
		Single num2 = this.OffCenterOffsetY * 0.00390625f;
		left += num;
		right += num;
		top += num2;
		bottom += num2;
		Single value = 2f * near / (right - left);
		Single value2 = 2f * near / (top - bottom);
		Single value3 = (right + left) / (right - left);
		Single value4 = (top + bottom) / (top - bottom);
		Single value5 = -(far + near) / (far - near);
		Single value6 = -(2f * far * near) / (far - near);
		Single value7 = -1f;
		Matrix4x4 result = default(Matrix4x4);
		result[0, 0] = value;
		result[0, 1] = 0f;
		result[0, 2] = value3;
		result[0, 3] = 0f;
		result[1, 0] = 0f;
		result[1, 1] = value2;
		result[1, 2] = value4;
		result[1, 3] = 0f;
		result[2, 0] = 0f;
		result[2, 1] = 0f;
		result[2, 2] = value5;
		result[2, 3] = value6;
		result[3, 0] = 0f;
		result[3, 1] = 0f;
		result[3, 2] = value7;
		result[3, 3] = 0f;
		return result;
	}

	private void CreateShadows()
	{
		this.Shadows = new List<WMShadow>();
	}

	public WMShadow GetShadow(PosObj chr)
	{
		if (this.Shadows == null)
		{
			this.CreateShadows();
		}
		for (Int32 i = 0; i < this.Shadows.Count; i++)
		{
			WMShadow wmshadow = this.Shadows[i];
			if (wmshadow.PosObj == chr)
			{
				return wmshadow;
			}
		}
		return (WMShadow)null;
	}

	public WMShadow GetShadow(Byte uid)
	{
		if (this.Shadows == null)
		{
			this.CreateShadows();
		}
		for (Int32 i = 0; i < this.Shadows.Count; i++)
		{
			WMShadow wmshadow = this.Shadows[i];
			if (wmshadow.PosObj.uid == uid)
			{
				return wmshadow;
			}
		}
		return (WMShadow)null;
	}

	public WMShadow AddShadow(PosObj chr, Vector3 scale)
	{
		GameObject original = Resources.Load<GameObject>("EmbeddedAsset/WorldMap_Local/Shadow/WMShadow");
		WMShadow wmshadow = UnityEngine.Object.Instantiate<GameObject>(original).AddComponent<WMShadow>();
		wmshadow.transform.localScale = scale;
		wmshadow.transform.parent = this.WorldMapEffectRoot;
		wmshadow.PosObj = chr;
		wmshadow.Material = wmshadow.GetComponent<Renderer>().material;
		this.Shadows.Add(wmshadow);
		return wmshadow;
	}

	private const Int32 BlockDistance = 64;

	private const Int32 CellDistance = 32;

	private Transform root;

	public Int32 Version;

	public Transform WorldDisc;

	private List<Texture2D> textures;

	private Vector3 centerOfMesh;

	private Single wrapWorldUpperBound;

	private Single wrapWorldLowerBound;

	private Single wrapWorldLeftBound;

	private Single wrapWorldRightBound;

	public Transform TranslatingObjectsGroup;

	public ObjList ActorList;

	public WMWorldSettings Settings;

	private Boolean loadingInBackground;

	public Transform SkyDome_Sky;

	public Material SkyDome_SkyMaterial;

	public Transform SkyDome_Bg;

	public Material SkyDowm_BgMaterial;

	public Transform SkyDome_Fog;

	public Material SkyDowm_FogMaterial;

	public GameObject SeaBlockPrefab;

	public WorldSPSSystem WorldSPSSystem;

	private GlobalFog _globalFog;

	private Int32 activeSpsPeak;

	private Int32 currentDisc = -1;

	public Single offsetY = -44.68f;

	public Single FineOffsetX = 0.78f;

	public Single FineScaleY = 0.07f;

	public Single SunsetOffsetX = 0.16f;

	public Single SunsetScaleY = 2.76f;

	public Single NightOffsetX = 0.35f;

	public Single NightScaleY = 0.12f;

	public MeshRenderer[] treeMeshRenderers;

	public Transform AirGardenShadow;

	public Boolean HasAirGardenShadow;

	public Boolean AirGardenIsInSpecialZone;

	public Boolean DidCheckIfAirGardenIsInSpecialZone;

	public Boolean DidLoadNewAirGardenShader;

	public Int32 myActiveWalkMesh;

	public Int32 myTriangleIndex;

	private WMBlock _block31;

	private WMBlock _block115;

	private WMBlock _block219;

	private Transform _worldMapRoot;

	private Transform _worldMapEffectRoot;

	public Transform kWorldPackEffectTwister;

	public Transform kWorldPackEffectSpiral0;

	public Transform kWorldPackEffectSpiral1;

	public Transform kWorldPackEffectSpiral2;

	public Transform kWorldPackEffectWindmill;

	public Transform kWorldPackEffectCore;

	public Transform kWorldPackEffectSky;

	public Transform kWorldPackEffectSphere1;

	public Transform kWorldPackEffectSphere2;

	public Transform kWorldPackEffectArch;

	public Transform kWorldPackEffectBlack;

	public Transform kWorldPackEffectThunder1;

	public Transform kWorldPackEffectThunder2;

	public Transform[] kWorldPackEffectThunder1s = new Transform[10];

	private Int32 kWorldPackEffectThunder1Index;

	public Transform[] kWorldPackEffectThunder2s = new Transform[10];

	private Int32 kWorldPackEffectThunder2Index;

	public Transform[] kWorldPackEffectArchs = new Transform[10];

	private Int32 kWorldPackEffectArchIndex;

	public Texture2D[] NormalChocoboTexture;

	public Texture2D[] AsaseChocoboTexture;

	public Texture2D[] YamaChocoboTexture;

	public Texture2D[] UmiChocoboTexture;

	public Texture2D[] SoraChocoboTexture;

	public Texture2D[] CurrentChocoboTextures;

	public Material QuicksandMaterial;

	public Material WaterShrineMaterial;

	public Quaternion Rotation = Quaternion.identity;

	public Vector3 Position;

	public Int32 PsxGeomScreen = 220;

	public Int32 ClipDistance = 300000;

	public Single OffCenterOffsetX;

	public Single OffCenterOffsetY;

	public List<WMShadow> Shadows;

	public enum ShiftDirection
	{
		Left,
		Right,
		Up,
		Down
	}
}

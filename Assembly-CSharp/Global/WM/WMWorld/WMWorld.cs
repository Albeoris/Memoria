using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Memoria;
using Object = System.Object;

public class WMWorld : Singleton<WMWorld>
{
	// A block is 64x64 in Unity size and 16384x16384 in fixed-point size
	// The world is 1536x1280 in Unity size and 393216x327680 in fixed-point size (the 2nd coordinate being negative)
	public WMBlock[,] Blocks { get; private set; }

	public WMBlock[,] InitialBlocks { get; private set; }

	public Camera MainCamera { get; private set; }

	public Single Width { get; private set; }

	public Single Height { get; private set; }

	public Boolean FinishedLoadingBlocks { get; private set; }

	public WMWorldLoadState LoadState { get; private set; }

	public Int32 BackgroundLoadingCount { get; private set; }

	public Int32 ActiveBlockCount { get; private set; }

	public Vector3 BlockShift { get; private set; }

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
		WMBlock.LoadMaterialsFromDisc();
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
							objList2 = objList2.next;
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
		this.BlockShift = new Vector3(0f, 0f, 0f);
		this.InitialBlocks = WMWorld.BuildBlockArray(this.WorldDisc);
		this.MainCamera = GameObject.Find("WorldCamera").GetComponent<Camera>();
		GameObject original2 = AssetManager.Load<GameObject>("EmbeddedAsset/WorldMap_Local/MoveFromBundle/kWorldPackEffectSky_Sky", false);
		this.SkyDome_Sky = UnityEngine.Object.Instantiate<GameObject>(original2).transform;
		this.SkyDome_Sky.parent = this.WorldMapEffectRoot;
		this.SkyDome_SkyMaterial = this.SkyDome_Sky.GetComponentInChildren<MeshRenderer>().material;
		GameObject original3 = AssetManager.Load<GameObject>("EmbeddedAsset/WorldMap_Local/MoveFromBundle/kWorldPackEffectSky_Bg", false);
		this.SkyDome_Bg = UnityEngine.Object.Instantiate<GameObject>(original3).transform;
		this.SkyDome_Bg.parent = this.WorldMapEffectRoot;
		this.SkyDowm_BgMaterial = this.SkyDome_Bg.GetComponentInChildren<MeshRenderer>().material;
		GameObject original4 = AssetManager.Load<GameObject>("WorldMap/Prefabs/Effects/kWorldPackEffectSky_Fog", false);
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
				ff9.w_frameDisc = 1;
		}
		else
		{
			ff9.w_frameScenePtr = ff9.ushort_gEventGlobal(0);
			ff9.w_frameDisc = WorldConfiguration.GetDisc();
		}
		this.currentDisc = ff9.w_frameDisc;
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
		for (Int32 i = 0; i < array.Length; i++)
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
		if (ff9.w_moveCHRStatus[actor.index].cache >= 4 && ff9.w_moveCHRStatus[actor.index].cache <= 8)
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
		if (ff9.w_moveCHRStatus[originalActor.index].cache >= 4 && ff9.w_moveCHRStatus[originalActor.index].cache <= 8)
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
			return target.position;
		Vector3 position = target.position;
		Vector3 localVec = new Vector3(position.x % 64f, 0f, position.z % 64f);
		Int32 blockX = (Int32)(position.x / 64f);
		Int32 blockZ = (Int32)(Mathf.Abs(position.z) / 64f);
		Int32 lengthX = this.Blocks.GetLength(0);
		Int32 lengthZ = this.Blocks.GetLength(1);
		if (blockX < 0)
			return target.position;
		if (blockX >= lengthX)
			blockX = lengthX - 1;
		if (blockZ < 0)
			return target.position;
		if (blockZ >= lengthZ)
			blockZ = lengthZ - 1;
		WMBlock wmblock = this.Blocks[blockX, blockZ];
		WMBlock component = wmblock.GetComponent<WMBlock>();
		Vector3 result = new Vector3(component.InitialX * 64 + localVec.x, position.y, -component.InitialY * 64 + localVec.z);
		return result;
	}

	public Vector3 GetAbsolutePositionOf(Vector3 vector)
	{
		if (this.Blocks == null)
			return vector;
		Vector3 localVec = new Vector3(vector.x % 64f, 0f, vector.z % 64f);
		Int32 blockX = (Int32)(vector.x / 64f);
		Int32 blockZ = (Int32)(Mathf.Abs(vector.z) / 64f);
		WMBlock wmblock = this.Blocks[blockX, blockZ];
		WMBlock component = wmblock.GetComponent<WMBlock>();
		Vector3 result = new Vector3(component.InitialX * 64 + localVec.x, vector.y, -component.InitialY * 64 + localVec.z);
		return result;
	}

	public void GetAbsolutePositionOf_FixedPoint(ref Int32 posX, ref Int32 posY, ref Int32 posZ)
	{
		if (this.Blocks == null)
			return;
		Int32 localX = posX % 16384;
		Int32 localZ = posZ % 16384;
		Int32 blockX = posX / 16384;
		Int32 blockZ = Math.Abs(posZ) / 16384;
		WMBlock wmblock = this.Blocks[blockX, blockZ];
		WMBlock component = wmblock.GetComponent<WMBlock>();
		posX = component.InitialX * 16384 + localX;
		posZ = -component.InitialY * 16384 + localZ;
	}

	public void GetUnityPositionOf_FixedPoint(ref Int32 posX, ref Int32 posY, ref Int32 posZ)
	{
		if (this.Blocks == null)
			return;
		Int32 localX = posX % 16384;
		Int32 localZ = posZ % 16384;
		Int32 blockX = posX / 16384;
		Int32 blockZ = Math.Abs(posZ) / 16384;
		WMBlock wmblock = this.InitialBlocks[blockX, blockZ];
		WMBlock component = wmblock.GetComponent<WMBlock>();
		posX = component.CurrentX * 16384 + localX;
		posZ = -component.CurrentY * 16384 + localZ;
	}

	public WMBlock GetAbsoluteBlock(Transform target)
	{
		return this.GetAbsoluteBlock(target.position);
	}

	public WMBlock GetAbsoluteBlock(Vector3 position)
	{
		if (this.Blocks == null)
			return null;
		Int32 x = (Int32)(position.x / 64f);
		Int32 z = (Int32)(Mathf.Abs(position.z) / 64f);
		if (x >= 24)
			return null;
		if (z >= 20)
			return null;
		WMBlock wmblock = this.Blocks[x, z];
		return this.Blocks[wmblock.CurrentX, wmblock.CurrentY];
	}

	public void SetAbsolutePositionOf(Transform target, Vector3 position, Single rotationY = 0f)
	{
		if (this.Blocks == null)
			return;
		Vector3 vector = new Vector3(position.x % 64f, 0f, position.z % 64f);
		Int32 x = (Int32)(position.x / 64f);
		Int32 z = (Int32)(Mathf.Abs(position.z) / 64f);
		WMBlock[,] blocks = this.Blocks;
		Int32 length = blocks.GetLength(0);
		Int32 length2 = blocks.GetLength(1);
		for (Int32 i = 0; i < length; i++)
		{
			for (Int32 j = 0; j < length2; j++)
			{
				WMBlock wmblock = blocks[i, j];
				if (x == wmblock.InitialX && z == wmblock.InitialY)
				{
					target.position = new Vector3(wmblock.CurrentX * 64 + vector.x, position.y, -wmblock.CurrentY * 64 + vector.z);
					return;
				}
			}
		}
	}

	public void SetAbsolutePositionOf(out Vector3 outPosition, Vector3 position)
	{
		outPosition = Vector3.zero;
		if (this.Blocks == null)
			return;
		Vector3 vector = new Vector3(position.x % 64f, 0f, position.z % 64f);
		Int32 x = (Int32)(position.x / 64f);
		Int32 z = (Int32)(Mathf.Abs(position.z) / 64f);
		WMBlock[,] blocks = this.Blocks;
		Int32 length = blocks.GetLength(0);
		Int32 length2 = blocks.GetLength(1);
		for (Int32 i = 0; i < length; i++)
		{
			for (Int32 j = 0; j < length2; j++)
			{
				WMBlock wmblock = blocks[i, j];
				if (x == wmblock.InitialX && z == wmblock.InitialY)
				{
					outPosition = new Vector3(wmblock.CurrentX * 64 + vector.x, position.y, -wmblock.CurrentY * 64 + vector.z);
					return;
				}
			}
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
		Int32 peak = 0;
		for (Int32 i = 0; i < this.WorldSPSSystem.SpsList.Count; i++)
		{
			SPSEffect worldSPS = this.WorldSPSSystem.SpsList[i];
			if (worldSPS.spsBin != null && worldSPS.spsId != -1)
				peak++;
		}
		if (this.activeSpsPeak < peak)
			this.activeSpsPeak = peak;
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			ff9.s_moveCHRStatus s_moveCHRStatus = ff9.w_moveCHRStatus[(Int32)ff9.w_moveActorPtr.originalActor.index];
			global::Debug.Log("status.id = " + s_moveCHRStatus.id);
		}
		Vector3 realPosition = ff9.w_moveActorPtr.RealPosition;
		if (Input.GetKeyDown(KeyCode.Alpha7))
			global::Debug.Log("ff9.w_moveActorPtr.RealPosition = " + ff9.w_moveActorPtr.RealPosition);
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
		if (ff9.w_movePlanePtr != null)
		{
			pos = ff9.w_movePlanePtr.pos;
			pos.y = 0f;
		}
		if (ff9.w_moveChocoboPtr != null)
		{
			pos = ff9.w_moveChocoboPtr.pos;
			pos.y = 0f;
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
					position.x = j * 16384 * 0.00390625f;
					position.z = i * -16384 * 0.00390625f;
					wmblock.transform.position = position;
				}
			}
			this.CaculateBoundsOfTranslatingObjects();
			this.ActorList = ff9.GetActiveObjList();
			if (this.Settings.WrapWorld)
			{
				this.BlockShift = new Vector3(0f, 0f, 0f);
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
			GameObject blockObjectPrefab = AssetManager.Load<GameObject>(name, false);
			this.LoadBlock(blockObjectPrefab, block);
		}
	}

    private void LoadBlock(GameObject blockObjectPrefab, WMBlock block)
    {
        WMBlockPrefab prefab = blockObjectPrefab.GetComponent<WMBlockPrefab>();
        block.Form1WalkMeshes = new List<WMMesh>();
        block.Form2WalkMeshes = new List<WMMesh>();
        block.Form1Transforms = new List<Transform>();
        if (prefab.ObjectForm1)
            RegisterBlockComponent(block, prefab.ObjectForm1, true, false);
        if (prefab.TerrainForm1)
            RegisterBlockComponent(block, prefab.TerrainForm1, true, false);
        if (prefab.ObjectForm2)
            RegisterBlockComponent(block, prefab.ObjectForm2, false, true);
        if (prefab.TerrainForm2)
            RegisterBlockComponent(block, prefab.TerrainForm2, false, true);
        if (block.Number == 219)
        {
            if (prefab.Sea3)
            {
                RegisterBlockComponent(block, prefab.Sea3, true, false);
                block.HasSea = true;
            }
            if (prefab.Sea4)
            {
                RegisterBlockComponent(block, prefab.Sea4, true, false);
                block.HasSea = true;
            }
            if (prefab.Sea5)
            {
                RegisterBlockComponent(block, prefab.Sea5, true, false);
                block.HasSea = true;
            }
            if (prefab.Sea3_2)
                RegisterBlockComponent(block, prefab.Sea3_2, false, true);
            if (prefab.Sea4_2)
                RegisterBlockComponent(block, prefab.Sea4_2, false, true);
            if (prefab.Sea5_2)
                RegisterBlockComponent(block, prefab.Sea5_2, false, true);
            GameObject waterShrineGo = AssetManager.Load<GameObject>("WorldMap/Prefabs/Effects/WaterShrine", false);
            if (waterShrineGo)
            {
                GameObject waterShrineCopy = UnityEngine.Object.Instantiate<GameObject>(waterShrineGo);
                waterShrineCopy.transform.name = waterShrineGo.transform.name;
                waterShrineCopy.transform.parent = block.transform;
                waterShrineCopy.transform.localPosition = new Vector3(32.11f, -2.46f, -31.69f);
                block.AddForm2Transform(waterShrineCopy.transform);
            }
            block.SetupPreloadedMaterials();
            block.ApplyForm();
            return;
        }
        if (prefab.Beach1)
        {
            RegisterBlockComponent(block, prefab.Beach1, true, true);
            block.HasBeach1 = true;
        }
        if (prefab.Beach2)
        {
            RegisterBlockComponent(block, prefab.Beach2, true, true);
            block.HasBeach2 = true;
        }
        if (prefab.Stream)
        {
            RegisterBlockComponent(block, prefab.Stream, true, true);
            block.HasStream = true;
        }
        if (prefab.River)
        {
            RegisterBlockComponent(block, prefab.River, true, true);
            block.HasRiver = true;
        }
        if (prefab.RiverJoint)
        {
            RegisterBlockComponent(block, prefab.RiverJoint, true, true);
            block.HasRiverJoint = true;
        }
        if (prefab.Falls)
        {
            RegisterBlockComponent(block, prefab.Falls, true, true);
            block.HasFalls = true;
        }
        if (prefab.Sea1)
        {
            RegisterBlockComponent(block, prefab.Sea1, true, true);
            block.HasSea = true;
        }
        if (prefab.Sea2)
        {
            RegisterBlockComponent(block, prefab.Sea2, true, true);
            block.HasSea = true;
        }
        if (prefab.Sea3)
        {
            RegisterBlockComponent(block, prefab.Sea3, true, true);
            block.HasSea = true;
        }
        if (prefab.Sea4)
        {
            RegisterBlockComponent(block, prefab.Sea4, true, true);
            block.HasSea = true;
        }
        if (prefab.Sea5)
        {
            RegisterBlockComponent(block, prefab.Sea5, true, true);
            block.HasSea = true;
        }
        if (prefab.Sea6)
        {
            RegisterBlockComponent(block, prefab.Sea6, true, true);
            block.HasSea = true;
        }
        if (prefab.VolcanoCrater1)
        {
            RegisterBlockComponent(block, prefab.VolcanoCrater1, true, false);
            block.HasVolcanoCrater = true;
        }
        if (prefab.VolcanoLava1)
        {
            RegisterBlockComponent(block, prefab.VolcanoLava1, true, false);
            block.HasVolcanoLava = true;
        }
        if (prefab.VolcanoCrater2)
        {
            RegisterBlockComponent(block, prefab.VolcanoCrater2, false, true);
            block.HasVolcanoCrater = true;
        }
        if (prefab.VolcanoLava2)
        {
            RegisterBlockComponent(block, prefab.VolcanoLava2, false, true);
            block.HasVolcanoLava = true;
        }
        if (block.Number == 91)
        {
            GameObject quicksandGo = AssetManager.Load<GameObject>("WorldMap/Prefabs/Effects/Quicksand", false);
            if (quicksandGo)
            {
                GameObject quicksandCopy = UnityEngine.Object.Instantiate<GameObject>(quicksandGo);
                quicksandCopy.transform.name = quicksandGo.transform.name;
                quicksandCopy.transform.parent = block.transform;
                quicksandCopy.transform.localPosition = new Vector3(8f, 1.66f, -60f);
            }
        }
        if (block.Number == 115)
        {
            GameObject quicksandGo = AssetManager.Load<GameObject>("WorldMap/Prefabs/Effects/Quicksand", false);
            if (quicksandGo)
            {
                GameObject quicksandCopy = UnityEngine.Object.Instantiate<GameObject>(quicksandGo);
                quicksandCopy.transform.name = quicksandGo.transform.name;
                quicksandCopy.transform.parent = block.transform;
                quicksandCopy.transform.localPosition = new Vector3(23.68f, 1.18f, -23.98f);
                quicksandCopy = UnityEngine.Object.Instantiate<GameObject>(quicksandGo);
                quicksandCopy.transform.name = quicksandGo.transform.name;
                quicksandCopy.transform.parent = block.transform;
                quicksandCopy.transform.localPosition = new Vector3(4f, 1.18f, -16f);
                quicksandCopy = UnityEngine.Object.Instantiate<GameObject>(quicksandGo);
                quicksandCopy.transform.name = quicksandGo.transform.name;
                quicksandCopy.transform.parent = block.transform;
                quicksandCopy.transform.localPosition = new Vector3(28f, 1.18f, -4f);
            }
        }
        if (block.Number == 158 && ff9.w_frameDisc == 4)
        {
            GameObject block146Go = AssetManager.Load<GameObject>("EmbeddedAsset/WorldMap_Local/Prefabs/Block[14][6] Object", false);
            if (block146Go)
            {
                Transform obj = block.transform.Find("Object");
                if (obj)
                    obj.GetComponent<Renderer>().enabled = false;
                GameObject block146Copy = UnityEngine.Object.Instantiate<GameObject>(block146Go);
                block146Copy.transform.name = block146Go.transform.name;
                block146Copy.transform.parent = block.transform;
                block146Copy.transform.localPosition = new Vector3(0f, 0f, 0f);
            }
            block146Go = AssetManager.Load<GameObject>("EmbeddedAsset/WorldMap_Local/Prefabs/Block[14][6] Object_tree", false);
            if (block146Go)
            {
                GameObject block146Copy = UnityEngine.Object.Instantiate<GameObject>(block146Go);
                block146Copy.transform.name = block146Go.transform.name;
                block146Copy.transform.parent = block.transform;
                block146Copy.transform.localPosition = new Vector3(0f, 0f, 0f);
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
            GameObject block516Go = AssetManager.Load<GameObject>("EmbeddedAsset/WorldMap_Local/Prefabs/Block[5][16] Object", false);
            if (block516Go)
            {
                Transform obj = block.transform.Find("Object");
                if (obj)
                    obj.GetComponent<Renderer>().enabled = false;
                GameObject block516Copy = UnityEngine.Object.Instantiate<GameObject>(block516Go);
                block516Copy.transform.name = block516Go.transform.name;
                block516Copy.transform.parent = block.transform;
                block516Copy.transform.localPosition = new Vector3(0f, 0f, 0f);
            }
        }
        block.SetupPreloadedMaterials();
        block.ApplyForm();
    }

    private void RegisterBlockComponent(WMBlock block, Transform transform, Boolean form1, Boolean form2)
    {
        Transform copy = UnityEngine.Object.Instantiate<Transform>(transform);
        copy.name = transform.name;
        copy.parent = block.transform;
        copy.localPosition = Vector3.zero;
        copy.localScale = new Vector3(1f, 1f, 1f);
        Mesh mesh = copy.GetComponent<MeshFilter>().sharedMesh;
        if (form1)
        {
            block.AddWalkMeshForm1(mesh);
            block.AddForm1Transform(copy);
        }
        if (form2)
        {
            block.AddWalkMeshForm2(mesh);
            block.AddForm2Transform(copy);
        }
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
			global::Debug.Log("Loading " + blockName + " in the background. " + ((!loadFromBundle) ? "[Resources]" : "[Bundle]"));
		yield return request;
		if (FF9StateSystem.World.PrintLogOnLoadingBlocksAsync)
			global::Debug.Log("Done loading " + blockName);
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
			this.BlockShift = new Vector3(0f, 0f, 0f);
			while (!this.Wrap())
			{
			}
		}
		if (ff9.w_moveActorPtr != null)
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
			if (ff9.w_movePlanePtr != null && ff9.w_frameCounter >= 10)
			{
				Vector3 absolutePositionOf = this.GetAbsolutePositionOf(ff9.w_frameCameraPtr.transform.position);
				Single num = Vector3.Distance(absolutePositionOf, b);
				if (num < 12f)
				{
					if (this.treeMeshRenderers == null || this.treeMeshRenderers.Length == 0)
						this.treeMeshRenderers = new MeshRenderer[4];
					if (this.treeMeshRenderers[0] == null)
					{
						WMBlock wmblock = this.InitialBlocks[11, 4];
						if (wmblock.transform.childCount != 0)
							this.treeMeshRenderers[0] = wmblock.transform.FindChild("Object").GetComponent<MeshRenderer>();
					}
					if (this.treeMeshRenderers[1] == null)
					{
						WMBlock wmblock2 = this.InitialBlocks[11, 5];
						if (wmblock2.transform.childCount != 0)
							this.treeMeshRenderers[1] = wmblock2.transform.FindChild("Object").GetComponent<MeshRenderer>();
					}
					if (this.treeMeshRenderers[2] == null)
					{
						WMBlock wmblock3 = this.InitialBlocks[12, 4];
						if (wmblock3.transform.childCount != 0)
							this.treeMeshRenderers[2] = wmblock3.transform.FindChild("Object").GetComponent<MeshRenderer>();
					}
					if (this.treeMeshRenderers[3] == null)
					{
						WMBlock wmblock4 = this.InitialBlocks[12, 5];
						if (wmblock4.transform.childCount != 0)
							this.treeMeshRenderers[3] = wmblock4.transform.FindChild("Object").GetComponent<MeshRenderer>();
					}
					for (Int32 i = 0; i < this.treeMeshRenderers.Length; i++)
						this.treeMeshRenderers[i].enabled = false;
				}
				else
				{
					for (Int32 j = 0; j < this.treeMeshRenderers.Length; j++)
						if (this.treeMeshRenderers[j] != null && !this.treeMeshRenderers[j].enabled)
							this.treeMeshRenderers[j].enabled = true;
				}
				if (this.HasAirGardenShadow && !this.DidCheckIfAirGardenIsInSpecialZone)
				{
					Vector3 absolutePositionOf2 = this.GetAbsolutePositionOf(this.AirGardenShadow.transform.position);
					Vector3 b2 = new Vector3(1307.2f, 26.1f, -644.7f);
					Single num2 = Vector3.Distance(absolutePositionOf2, b2);
					if (num2 < 30f)
						this.AirGardenIsInSpecialZone = true;
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
		if (ff9.w_moveActorPtr == null)
			return true;
		if (ff9.w_moveActorPtr == ff9.w_moveDummyCharacter)
			return true;
		Vector3 pos = ff9.w_moveActorPtr.pos;
		Boolean result = true;
		if (pos.x < this.wrapWorldLeftBound)
		{
			this.BlockShift += new Vector3(64f, 0f, 0f);
			this.ShiftRightAllBlocks();
			result = false;
		}
		if (pos.x > this.wrapWorldRightBound)
		{
			this.BlockShift += new Vector3(-64f, 0f, 0f);
			this.ShiftLeftAllBlocks();
			result = false;
		}
		if (pos.z > this.wrapWorldUpperBound)
		{
			this.BlockShift += new Vector3(0f, 0f, -64f);
			this.ShiftDownAllBlocks();
			result = false;
		}
		if (pos.z < this.wrapWorldLowerBound)
		{
			this.BlockShift += new Vector3(0f, 0f, 64f);
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
		this.SeaBlockPrefab = AssetManager.Load<GameObject>(name, false);
		this.DetectUnseenBlocks();
		for (Int32 y = 0; y < 20; y++)
		{
			for (Int32 x = 0; x < 24; x++)
			{
				WMBlock wmblock = this.Blocks[x, y];
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
		for (Int32 x = 0; x < length; x++)
		{
			for (Int32 y = 0; y < length2; y++)
			{
				WMBlock wmblock = this.Blocks[x, y];
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
		for (Int32 x = 0; x < length; x++)
		{
			for (Int32 y = 0; y < length2; y++)
			{
				WMBlock wmblock2 = this.Blocks[x, y];
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
			this.DetectUnseenBlocksFastForward();
		else
			this.DetectUnseenBlocksNormal();
	}

	private void DetectUnseenBlocksNormal()
	{
		Int32 width = this.Blocks.GetLength(0);
		Int32 height = this.Blocks.GetLength(1);
		for (Int32 i = 0; i < width; i++)
			for (Int32 j = 0; j < height; j++)
				this.Blocks[i, j].IsInsideSight = false;
		WMBlock absoluteBlock = this.GetAbsoluteBlock(this.SkyDome_Sky);
		Int32 currentX = absoluteBlock.CurrentX;
		Int32 currentY = absoluteBlock.CurrentY;
		this.Blocks[currentX - 2, currentY - 2].IsInsideSight = true;
		this.Blocks[currentX - 1, currentY - 2].IsInsideSight = true;
		this.Blocks[currentX, currentY - 2].IsInsideSight = true;
		this.Blocks[currentX + 1, currentY - 2].IsInsideSight = true;
		this.Blocks[currentX + 2, currentY - 2].IsInsideSight = true;
		this.Blocks[currentX - 2, currentY - 1].IsInsideSight = true;
		this.Blocks[currentX - 1, currentY - 1].IsInsideSight = true;
		this.Blocks[currentX, currentY - 1].IsInsideSight = true;
		this.Blocks[currentX + 1, currentY - 1].IsInsideSight = true;
		this.Blocks[currentX + 2, currentY - 1].IsInsideSight = true;
		this.Blocks[currentX - 2, currentY].IsInsideSight = true;
		this.Blocks[currentX - 1, currentY].IsInsideSight = true;
		this.Blocks[currentX, currentY].IsInsideSight = true;
		this.Blocks[currentX + 1, currentY].IsInsideSight = true;
		this.Blocks[currentX + 2, currentY].IsInsideSight = true;
		this.Blocks[currentX - 2, currentY + 1].IsInsideSight = true;
		this.Blocks[currentX - 1, currentY + 1].IsInsideSight = true;
		this.Blocks[currentX, currentY + 1].IsInsideSight = true;
		this.Blocks[currentX + 1, currentY + 1].IsInsideSight = true;
		this.Blocks[currentX + 2, currentY + 1].IsInsideSight = true;
		this.Blocks[currentX - 2, currentY + 2].IsInsideSight = true;
		this.Blocks[currentX - 1, currentY + 2].IsInsideSight = true;
		this.Blocks[currentX, currentY + 2].IsInsideSight = true;
		this.Blocks[currentX + 1, currentY + 2].IsInsideSight = true;
		this.Blocks[currentX + 2, currentY + 2].IsInsideSight = true;
	}

	private void DetectUnseenBlocksFastForward()
	{
		Int32 width = this.Blocks.GetLength(0);
		Int32 height = this.Blocks.GetLength(1);
		for (Int32 i = 0; i < width; i++)
			for (Int32 j = 0; j < height; j++)
				this.Blocks[i, j].IsInsideSight = false;
		WMBlock absoluteBlock = this.GetAbsoluteBlock(this.SkyDome_Sky);
		Int32 currentX = absoluteBlock.CurrentX;
		Int32 currentY = absoluteBlock.CurrentY;
		this.Blocks[currentX - 3, currentY - 3].IsInsideSight = true;
		this.Blocks[currentX - 2, currentY - 3].IsInsideSight = true;
		this.Blocks[currentX - 1, currentY - 3].IsInsideSight = true;
		this.Blocks[currentX, currentY - 3].IsInsideSight = true;
		this.Blocks[currentX + 1, currentY - 3].IsInsideSight = true;
		this.Blocks[currentX + 2, currentY - 3].IsInsideSight = true;
		this.Blocks[currentX + 3, currentY - 3].IsInsideSight = true;
		this.Blocks[currentX - 3, currentY - 2].IsInsideSight = true;
		this.Blocks[currentX - 2, currentY - 2].IsInsideSight = true;
		this.Blocks[currentX - 1, currentY - 2].IsInsideSight = true;
		this.Blocks[currentX, currentY - 2].IsInsideSight = true;
		this.Blocks[currentX + 1, currentY - 2].IsInsideSight = true;
		this.Blocks[currentX + 2, currentY - 2].IsInsideSight = true;
		this.Blocks[currentX + 3, currentY - 2].IsInsideSight = true;
		this.Blocks[currentX - 3, currentY - 1].IsInsideSight = true;
		this.Blocks[currentX - 2, currentY - 1].IsInsideSight = true;
		this.Blocks[currentX - 1, currentY - 1].IsInsideSight = true;
		this.Blocks[currentX, currentY - 1].IsInsideSight = true;
		this.Blocks[currentX + 1, currentY - 1].IsInsideSight = true;
		this.Blocks[currentX + 2, currentY - 1].IsInsideSight = true;
		this.Blocks[currentX + 3, currentY - 1].IsInsideSight = true;
		this.Blocks[currentX - 3, currentY].IsInsideSight = true;
		this.Blocks[currentX - 2, currentY].IsInsideSight = true;
		this.Blocks[currentX - 1, currentY].IsInsideSight = true;
		this.Blocks[currentX, currentY].IsInsideSight = true;
		this.Blocks[currentX + 1, currentY].IsInsideSight = true;
		this.Blocks[currentX + 2, currentY].IsInsideSight = true;
		this.Blocks[currentX + 3, currentY].IsInsideSight = true;
		this.Blocks[currentX - 3, currentY + 1].IsInsideSight = true;
		this.Blocks[currentX - 2, currentY + 1].IsInsideSight = true;
		this.Blocks[currentX - 1, currentY + 1].IsInsideSight = true;
		this.Blocks[currentX, currentY + 1].IsInsideSight = true;
		this.Blocks[currentX + 1, currentY + 1].IsInsideSight = true;
		this.Blocks[currentX + 2, currentY + 1].IsInsideSight = true;
		this.Blocks[currentX + 3, currentY + 1].IsInsideSight = true;
		this.Blocks[currentX - 3, currentY + 2].IsInsideSight = true;
		this.Blocks[currentX - 2, currentY + 2].IsInsideSight = true;
		this.Blocks[currentX - 1, currentY + 2].IsInsideSight = true;
		this.Blocks[currentX, currentY + 2].IsInsideSight = true;
		this.Blocks[currentX + 1, currentY + 2].IsInsideSight = true;
		this.Blocks[currentX + 2, currentY + 2].IsInsideSight = true;
		this.Blocks[currentX + 3, currentY + 2].IsInsideSight = true;
		this.Blocks[currentX - 3, currentY + 3].IsInsideSight = true;
		this.Blocks[currentX - 2, currentY + 3].IsInsideSight = true;
		this.Blocks[currentX - 1, currentY + 3].IsInsideSight = true;
		this.Blocks[currentX, currentY + 3].IsInsideSight = true;
		this.Blocks[currentX + 1, currentY + 3].IsInsideSight = true;
		this.Blocks[currentX + 2, currentY + 3].IsInsideSight = true;
		this.Blocks[currentX + 3, currentY + 3].IsInsideSight = true;
	}

	private void DetectUnseenBlocksBy(Int32 radius)
	{
		Int32 width = this.Blocks.GetLength(0);
		Int32 height = this.Blocks.GetLength(1);
		for (Int32 i = 0; i < width; i++)
			for (Int32 j = 0; j < height; j++)
				this.Blocks[i, j].IsInsideSight = false;
		WMBlock absoluteBlock = this.GetAbsoluteBlock(this.SkyDome_Sky);
		Int32 currentX = absoluteBlock.CurrentX;
		Int32 currentY = absoluteBlock.CurrentY;
		for (Int32 i = -radius; i <= radius; i++)
			for (Int32 j = -radius; j <= radius; j++)
				this.Blocks[currentX + i, currentY + j].IsInsideSight = true;
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
						array[i, j] = component;
				}
			}
		}
		return array;
	}

	public WMBlock Block31
	{
		get
		{
			if (this._block31 == null)
				this._block31 = this.InitialBlocks[7, 1];
			return this._block31;
		}
	}

	public WMBlock Block115
	{
		get
		{
			if (this._block115 == null)
				this._block115 = this.InitialBlocks[19, 14];
			return this._block115;
		}
	}

	public WMBlock Block219
	{
		get
		{
			if (this._block219 == null)
				this._block219 = this.InitialBlocks[3, 9];
			return this._block219;
		}
	}

	public Transform WorldMapRoot
	{
		get
		{
			if (this._worldMapRoot == null)
				this._worldMapRoot = GameObject.Find("WorldMapRoot").transform;
			return this._worldMapRoot;
		}
	}

	public Transform WorldMapEffectRoot
	{
		get
		{
			if (this._worldMapEffectRoot == null)
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
		if (WorldConfiguration.UseWorldEffect(WorldEffect.SandStorm))
		{
			this.kWorldPackEffectTwister = this.LoadEffect("kWorldPackEffectTwister");
			this.kWorldPackEffectSpiral0 = this.LoadEffect("kWorldPackEffectSpiral0");
			this.kWorldPackEffectSpiral1 = this.LoadEffect("kWorldPackEffectSpiral1");
			this.kWorldPackEffectSpiral2 = this.LoadEffect("kWorldPackEffectSpiral2");
			this.kWorldPackEffectCore = this.LoadEffect("kWorldPackEffectCore");
			this.SetTwisterRenderQueue(2450);
		}
		this.kWorldPackEffectWindmill = this.LoadEffect("kWorldPackEffectWindmill");
		if (WorldConfiguration.UseWorldEffect(WorldEffect.Memoria))
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
		Transform transform = AssetManager.Load<GameObject>("WorldMap/Prefabs/Effects/" + effect, false).transform;
		Transform transform2 = UnityEngine.Object.Instantiate<Transform>(transform);
		transform2.parent = this.WorldMapEffectRoot;
		return transform2;
	}

	private Transform[] LoadEffects(String effect, Int32 count, Boolean createNewMaterial = false)
	{
		Transform transform = AssetManager.Load<GameObject>("WorldMap/Prefabs/Effects/" + effect, false).transform;
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
		return AssetManager.Load<Texture2D>("EmbeddedAsset/WorldMap_Local/Characters/Chocobo/" + chocoboName + "/" + str, false);
	}

	public void LoadQuicksandMaterial()
	{
		if (!WMBlock.MaterialDatabase.TryGetValue("Quicksand", out this.QuicksandMaterial))
			this.QuicksandMaterial = AssetManager.Load<Material>("WorldMap/Materials/quicksand_mat", false);
		Texture2D detailTexture = AssetManager.Load<Texture2D>("EmbeddedAsset/WorldMap_Local/Textures/SeamlessRock", false);
		this.QuicksandMaterial.SetTexture("_DetailTex", detailTexture);
	}

	public void LoadWaterShrineMaterial()
	{
		if (!WMBlock.MaterialDatabase.TryGetValue("WaterShrine", out this.WaterShrineMaterial))
			this.WaterShrineMaterial = AssetManager.Load<Material>("WorldMap/Materials/WaterShrine_mat", false);
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

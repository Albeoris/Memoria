using System;
using UnityEngine;
using Object = System.Object;

public static class WMWorldPrefabMaker
{
	public static Transform LoadModelAsset(Int32 disc, Boolean fillEmptyBlock)
	{
		String name = "WorldDisc" + disc;
		Transform transform = new GameObject(name).transform;
		Int32 num = 0;
		for (Int32 i = 0; i < 20; i++)
		{
			for (Int32 j = 0; j < 24; j++)
			{
				String text = String.Concat(new Object[]
				{
					"Block[",
					j,
					"][",
					i,
					"]"
				});
				Transform transform2 = new GameObject(text).transform;
				transform2.parent = transform;
				WMBlockPrefab wmblockPrefab = transform2.gameObject.AddComponent<WMBlockPrefab>();
				wmblockPrefab.InitialX = j;
				wmblockPrefab.InitialY = i;
				wmblockPrefab.CurrentX = j;
				wmblockPrefab.CurrentY = i;
				Boolean flag = false;
				Boolean isSea = false;
				Boolean hasSpecialObject = false;
				Boolean isSwitchable = false;
				String str = String.Format("WorldMap/Disc{0}/0_1/r{1}/", disc, i);
				String str2 = String.Format("WorldMap/Disc{0}/0_2/r{1}/", disc, i);
				String assetPath = str + text + " Terrain";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Terrain", WMWorldPrefabMaker.TerrainMaterial))
				{
					flag = true;
				}
				assetPath = str + text + " Sea1";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Sea1", WMWorldPrefabMaker.Sea1Material))
				{
					flag = true;
				}
				assetPath = str + text + " Sea2";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Sea2", WMWorldPrefabMaker.Sea2Material))
				{
					flag = true;
				}
				assetPath = str + text + " Sea3";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Sea3", WMWorldPrefabMaker.Sea3Material))
				{
					flag = true;
				}
				assetPath = str + text + " Sea4";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Sea4", WMWorldPrefabMaker.Sea4Material))
				{
					flag = true;
				}
				assetPath = str + text + " Sea5";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Sea5", WMWorldPrefabMaker.Sea5Material))
				{
					flag = true;
				}
				assetPath = str + text + " Sea6";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Sea6", WMWorldPrefabMaker.Sea6Material))
				{
					flag = true;
				}
				assetPath = str + text + " Beach1";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Beach1", WMWorldPrefabMaker.BeachMaterial))
				{
					flag = true;
				}
				assetPath = str + text + " Beach2";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Beach2", WMWorldPrefabMaker.Beach2Material))
				{
					flag = true;
				}
				assetPath = str + text + " Stream";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Stream", WMWorldPrefabMaker.StreamMaterial))
				{
					flag = true;
				}
				assetPath = str + text + " River";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "River", WMWorldPrefabMaker.RiverMaterial))
				{
					flag = true;
				}
				assetPath = str + text + " RiverJoint";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "RiverJoint", WMWorldPrefabMaker.RiverJointMaterial))
				{
					flag = true;
				}
				assetPath = str + text + " Falls";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Falls", WMWorldPrefabMaker.FallsMaterial))
				{
					flag = true;
				}
				assetPath = str + text + " Object";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Object", WMWorldPrefabMaker.ObjectMaterial))
				{
					flag = true;
					hasSpecialObject = true;
				}
				assetPath = str + text + " VolcanoCrater";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "VolcanoCrater1", WMWorldPrefabMaker.VolcanoCrater1Material))
				{
					flag = true;
					hasSpecialObject = true;
				}
				assetPath = str + text + " VolcanoLava";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "VolcanoLava1", WMWorldPrefabMaker.VolcanoLava1Material))
				{
					flag = true;
					hasSpecialObject = true;
				}
				assetPath = str2 + text + " Terrain";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Terrain2", WMWorldPrefabMaker.TerrainMaterial))
				{
					isSwitchable = true;
				}
				assetPath = str2 + text + " Object";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Object2", WMWorldPrefabMaker.ObjectMaterial))
				{
					isSwitchable = true;
				}
				assetPath = str2 + text + " VolcanoCrater";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "VolcanoCrater2", WMWorldPrefabMaker.VolcanoCrater2Material))
				{
					isSwitchable = true;
				}
				assetPath = str2 + text + " VolcanoLava";
				if (WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "VolcanoLava2", WMWorldPrefabMaker.VolcanoLava2Material))
				{
					isSwitchable = true;
				}
				if (!flag)
				{
					str = String.Format("WorldMap/Disc1/0_1/r0/", new Object[0]);
					text = String.Concat(new Object[]
					{
						"Block[",
						12,
						"][",
						0,
						"]"
					});
					assetPath = str + text + " Terrain";
					WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Terrain", WMWorldPrefabMaker.TerrainMaterial);
					assetPath = str + text + " Sea4";
					WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Sea4", WMWorldPrefabMaker.Sea4Material);
					assetPath = str + text + " Sea6";
					WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Sea6", WMWorldPrefabMaker.Sea6Material);
					isSea = true;
				}
				if (j == 3 && i == 9)
				{
					assetPath = str2 + text + " Sea3";
					WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Sea3_2", WMWorldPrefabMaker.Sea3Material);
					assetPath = str2 + text + " Sea4";
					WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Sea4_2", WMWorldPrefabMaker.Sea4Material);
					assetPath = str2 + text + " Sea5";
					WMWorldPrefabMaker.LoadMesh(wmblockPrefab, assetPath, "Sea5_2", WMWorldPrefabMaker.Sea5Material);
				}
				wmblockPrefab.IsSea = isSea;
				wmblockPrefab.HasSpecialObject = hasSpecialObject;
				wmblockPrefab.IsSwitchable = isSwitchable;
				wmblockPrefab.Number = num;
				num++;
			}
		}
		WMBlockPrefab[,] wmBlocks = WMWorldPrefabMaker.BuildBlockArray(transform);
		WMWorldPrefabMaker.SetupPosition(wmBlocks);
		return transform;
	}

	private static Boolean LoadMesh(WMBlockPrefab block, String assetPath, String objectName, Material material)
	{
		Mesh mesh = Resources.Load(assetPath) as Mesh;
		Boolean result = false;
		if (mesh != (UnityEngine.Object)null)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = objectName;
			gameObject.transform.parent = block.transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = mesh;
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.material = material;
			if (objectName == "Terrain")
			{
				block.TerrainForm1 = gameObject.transform;
			}
			else if (objectName == "Terrain2")
			{
				block.TerrainForm2 = gameObject.transform;
			}
			else if (objectName == "Object")
			{
				block.ObjectForm1 = gameObject.transform;
			}
			else if (objectName == "Object2")
			{
				block.ObjectForm2 = gameObject.transform;
			}
			else if (objectName == "Beach1")
			{
				block.Beach1 = gameObject.transform;
			}
			else if (objectName == "Beach2")
			{
				block.Beach2 = gameObject.transform;
			}
			else if (objectName == "Stream")
			{
				block.Stream = gameObject.transform;
			}
			else if (objectName == "River")
			{
				block.River = gameObject.transform;
			}
			else if (objectName == "RiverJoint")
			{
				block.RiverJoint = gameObject.transform;
			}
			else if (objectName == "Falls")
			{
				block.Falls = gameObject.transform;
			}
			else if (objectName == "Sea1")
			{
				block.Sea1 = gameObject.transform;
			}
			else if (objectName == "Sea2")
			{
				block.Sea2 = gameObject.transform;
			}
			else if (objectName == "Sea3")
			{
				block.Sea3 = gameObject.transform;
			}
			else if (objectName == "Sea4")
			{
				block.Sea4 = gameObject.transform;
			}
			else if (objectName == "Sea5")
			{
				block.Sea5 = gameObject.transform;
			}
			else if (objectName == "Sea6")
			{
				block.Sea6 = gameObject.transform;
			}
			else if (objectName == "Sea3_2")
			{
				block.Sea3_2 = gameObject.transform;
			}
			else if (objectName == "Sea4_2")
			{
				block.Sea4_2 = gameObject.transform;
			}
			else if (objectName == "Sea5_2")
			{
				block.Sea5_2 = gameObject.transform;
			}
			else if (objectName == "VolcanoCrater1")
			{
				block.VolcanoCrater1 = gameObject.transform;
			}
			else if (objectName == "VolcanoLava1")
			{
				block.VolcanoLava1 = gameObject.transform;
			}
			else if (objectName == "VolcanoCrater2")
			{
				block.VolcanoCrater2 = gameObject.transform;
			}
			else if (objectName == "VolcanoLava2")
			{
				block.VolcanoLava2 = gameObject.transform;
			}
			result = true;
		}
		return result;
	}

	public static WMBlockPrefab[,] BuildBlockArray(Transform worldDisc)
	{
		WMBlockPrefab[,] array = new WMBlockPrefab[24, 20];
		for (Int32 i = 0; i < 24; i++)
		{
			for (Int32 j = 0; j < 20; j++)
			{
				foreach (Object obj in worldDisc.transform)
				{
					Transform transform = (Transform)obj;
					WMBlockPrefab component = transform.GetComponent<WMBlockPrefab>();
					if (component.InitialX == i && component.InitialY == j)
					{
						array[i, j] = component;
					}
				}
			}
		}
		return array;
	}

	private static void SetupPosition(WMBlockPrefab[,] wmBlocks)
	{
		for (Int32 i = 0; i < 24; i++)
		{
			for (Int32 j = 0; j < 20; j++)
			{
				WMBlockPrefab wmblockPrefab = wmBlocks[i, j];
				Vector3 position = new Vector3((Single)(i * 64), 0f, (Single)(-(Single)j * 64));
				wmblockPrefab.transform.position = position;
			}
		}
	}

	public static Material TerrainMaterial;

	public static Material ObjectMaterial;

	public static Material BeachMaterial;

	public static Material Beach2Material;

	public static Material RiverMaterial;

	public static Material RiverJointMaterial;

	public static Material StreamMaterial;

	public static Material FallsMaterial;

	public static Material SeaMaterial;

	public static Material Sea1Material;

	public static Material Sea2Material;

	public static Material Sea3Material;

	public static Material Sea4Material;

	public static Material Sea5Material;

	public static Material Sea6Material;

	public static Material VolcanoCrater1Material;

	public static Material VolcanoLava1Material;

	public static Material VolcanoCrater2Material;

	public static Material VolcanoLava2Material;
}

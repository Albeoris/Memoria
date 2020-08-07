using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Sources.Scripts.Common;
using UnityEngine;
using Object = System.Object;

public class FieldMapUIButton : MonoBehaviour
{
	private void Start()
	{
		this.changeUI = base.GetComponent<ChangeUI>();
		this.bg = GameObject.Find("FieldMap").GetComponent<FieldMap>();
		this._sceneList = new List<FieldMapUIButton.SceneDef>();
		String name = "EmbeddedAsset/Manifest/FieldMap/mapList.txt";
		String text = AssetManager.LoadString(name, out _, false);
		String[] array = text.Split(new Char[]
		{
			"\n"[0]
		});
		Int32 num = 0;
		String[] array2 = array;
		for (Int32 i = 0; i < (Int32)array2.Length; i++)
		{
			String text2 = array2[i];
			if (text2 == String.Empty)
			{
				break;
			}
			String[] array3 = text2.Split(new Char[]
			{
				','
			});
			FieldMapUIButton.SceneDef item;
			item.index = Int32.Parse(array3[0]);
			item.name = array3[1];
			item.zExtra = new Int32[10];
			Int32 num2 = 0;
			for (Int32 j = 0; j < 10; j++)
			{
				num2 = 0;
				if (Int32.TryParse(array3[2 + j], out num2))
				{
					item.zExtra[j] = num2;
				}
			}
			this._sceneList.Add(item);
			if (item.name == this.changeUI.Scene)
			{
				this.stringToEdit = (num + 1).ToString();
				this.index = num;
				this.changeUI.zExtra = item.zExtra[0];
			}
			num++;
		}
		this.isBilinear = false;
	}

	private void OnGUI()
	{
		Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
		DebugGuiSkin.ApplySkin();
		GUILayout.BeginArea(fullscreenRect);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("Back", new GUILayoutOption[0]))
		{
			SceneDirector.Replace("MainMenu", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("Debug UI", new GUILayoutOption[0]))
		{
			FF9StateSystem.Field.isOpenFieldMapDebugPanel = !FF9StateSystem.Field.isOpenFieldMapDebugPanel;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		if (FF9StateSystem.Field.isOpenFieldMapDebugPanel)
		{
			this.BuildBattleMapDebugTopPanel();
		}
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		if (GUILayout.Button((!FF9StateSystem.Field.UseUpscalFM) ? "Original" : "Upscaled", new GUILayoutOption[0]))
		{
			FF9StateSystem.Field.UseUpscalFM = !FF9StateSystem.Field.UseUpscalFM;
			SceneDirector.Replace("FieldMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		if (!FF9StateSystem.Field.UseUpscalFM && GUILayout.Button((!this.isBilinear) ? "Point" : "Bilinear", new GUILayoutOption[0]))
		{
			this.isBilinear = !this.isBilinear;
			if (this.isBilinear)
			{
				this.bg.scene.atlas.filterMode = FilterMode.Bilinear;
			}
			else
			{
				this.bg.scene.atlas.filterMode = FilterMode.Point;
			}
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		if (FF9StateSystem.Field.isOpenFieldMapDebugPanel)
		{
			this.BuildBattleMapDebugBottomPanel();
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private void BuildBattleMapDebugTopPanel()
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		for (Int32 i = 0; i < this.bg.scene.cameraList.Count; i++)
		{
			if (GUILayout.Button("Camera" + i, new GUILayoutOption[0]))
			{
				this.changeUI.CamIdx = i;
			}
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		for (Int32 j = 0; j < this.bg.scene.animList.Count; j++)
		{
			if ((Int32)this.bg.scene.animList[j].camNdx == this.changeUI.CamIdx && this.changeUI.AnimIdx.Length != 0)
			{
				String text = (!this.changeUI.AnimIdx[j]) ? "OFF" : "ON";
				if (GUILayout.Button(String.Concat(new Object[]
				{
					"Anm",
					j,
					": ",
					text
				}), new GUILayoutOption[0]))
				{
					this.changeUI.AnimIdx[j] = !this.changeUI.AnimIdx[j];
					if (!this.changeUI.AnimIdx[j])
					{
						BGANIM_DEF bganim_DEF = this.bg.scene.animList[j];
						this.bg.scene.animList[j].counter = 0;
						for (Int32 k = 1; k < bganim_DEF.frameList.Count; k++)
						{
							this.bg.scene.overlayList[(Int32)bganim_DEF.frameList[k].target].transform.gameObject.SetActive(false);
						}
						this.bg.scene.overlayList[(Int32)bganim_DEF.frameList[0].target].transform.gameObject.SetActive(true);
					}
				}
			}
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		Boolean activeSelf = this.bg.walkMesh.ProjectedWalkMesh.activeSelf;
		String str = (!activeSelf) ? "OFF" : "ON";
		if (GUILayout.Button("Walkmesh: " + str, new GUILayoutOption[0]))
		{
			this.bg.walkMesh.ProjectedWalkMesh.SetActive(!activeSelf);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
	}

	private void BuildBattleMapDebugBottomPanel()
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("(   <   )", new GUILayoutOption[0]))
		{
			this.index--;
			if (this.index < 0)
			{
				this.index = this._sceneList.Count - 1;
			}
			FF9StateSystem.Field.SceneName = this._sceneList[this.index].name;
			FF9StateSystem.Field.index = this.index;
			SceneDirector.Replace("FieldMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		this.stringToEdit = GUILayout.TextField(this.stringToEdit, new GUILayoutOption[]
		{
			GUILayout.Width(200f)
		});
		if (GUILayout.Button("Jump", new GUILayoutOption[0]))
		{
			if (this.stringToEdit == (this.index + 1).ToString())
			{
				return;
			}
			if (Int32.TryParse(this.stringToEdit, out this.index))
			{
				this.index--;
				if (this.index < 0 || this.index > this._sceneList.Count - 1)
				{
					this.index = 0;
				}
				FF9StateSystem.Field.SceneName = this._sceneList[this.index].name;
				FF9StateSystem.Field.index = this.index;
				SoundLib.StopAllSounds(true);
				SceneDirector.Replace("FieldMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
			}
		}
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal("Box", new GUILayoutOption[0]);
		GUILayout.Label(this._sceneList[this.index].name, new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("(   >   )", new GUILayoutOption[0]))
		{
			this.index++;
			if (this.index > this._sceneList.Count - 1)
			{
				this.index = 0;
			}
			FF9StateSystem.Field.SceneName = this._sceneList[this.index].name;
			FF9StateSystem.Field.index = this.index;
			SceneDirector.Replace("FieldMapDebug", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		GUILayout.EndHorizontal();
	}

	public GUIStyle style;

	private List<FieldMapUIButton.SceneDef> _sceneList;

	private Int32 index;

	private ChangeUI changeUI;

	private FieldMap bg;

	private String stringToEdit = String.Empty;

	private Boolean isBilinear = true;

	public struct SceneDef
	{
		public Int32 index;

		public String name;

		public Int32[] zExtra;
	}
}

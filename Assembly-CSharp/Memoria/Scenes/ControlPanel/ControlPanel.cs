using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Memoria.Prime;
using Assets.Sources.Scripts.UI.Common;

namespace Memoria.Scenes
{
	public class ControlPanel
	{
		public Int32 rowHeight = 50;
		public Int32 rowSeparatorHeight = 10;
		public Int32 elementSeparatorWidth = 10;

		public UIWidget BasePanel => basePanel;
		public List<UIWidget> AllPanels => allPanels;
		public Int32 ActivePanelIndex => activePanelIndex;
		public Boolean Show
		{
			get => allPanels[activePanelIndex].gameObject.activeSelf;
			set => allPanels[activePanelIndex].gameObject.SetActive(value);
		}

		public ControlPanel(Transform parent, String name)
		{
			baseAnchor = parent;
			basePanel = InstantiatePanel(baseAnchor, name, true);
			panelParentLink[0] = -1;
		}

		public Int32 CreateSubPanel(String name, Int32 parentPanelIndex = 0)
		{
			Int32 panelIndex = allPanels.Count;
			UIWidget panel = InstantiatePanel(allPanels[parentPanelIndex].transform, name, false);
			panelParentLink[panelIndex] = parentPanelIndex;
			return panelIndex;
		}

		public void EndInitialization()
		{
			foreach (UIWidget panel in allPanels)
			{
				ExtendLastRowToMax(panel, false);
				panel.pivot = UIWidget.Pivot.Right;
				panel.SetRect(0f, 0f, 700f, 1000f);
				panel.gameObject.SetActive(false);
			}
			activePanelIndex = 0;
		}

		public void SetActivePanel(Boolean show, Int32 panelIndex = -1)
		{
			if (panelIndex < 0)
				panelIndex = activePanelIndex;
			UIWidget panel = allPanels[panelIndex];
			if (show == panel.gameObject.activeSelf)
				return;
			foreach (UIWidget otherPanel in allPanels)
				otherPanel.gameObject.SetActive(false);
			panel.gameObject.SetActive(show);
			if (show)
			{
				activePanelIndex = panelIndex;
			}
			else
			{
				Int32 parentPanelIndex = GetParentPanelIndex(panel);
				if (parentPanelIndex >= 0)
				{
					allPanels[parentPanelIndex].gameObject.SetActive(true);
					activePanelIndex = parentPanelIndex;
				}
				else
				{
					activePanelIndex = 0;
				}
			}
		}

		public UIWidget GetPanel(Int32 panelIndex)
		{
			return allPanels[panelIndex];
		}

		public UIWidget GetElement(Int32 panelIndex, Int32 row, Int32 column = 0)
		{
			return panelRegister[allPanels[panelIndex]][row][column];
		}

		public UILabel AddSimpleLabel(String message, NGUIText.Alignment alignment, Int32 lineCount, Int32 panelIndex = 0)
		{
			UIWidget panel = GetPanel(panelIndex);
			UILabel label = CreateUIElementForPanel<UILabel>(panel);
			label.overflowMethod = UILabel.Overflow.ClampContent;
			label.text = message;
			label.alignment = alignment;
			if (lineCount < 0)
				label.bottomAnchor.Set(panel.transform, 0f, 50);
			else if (lineCount > 1)
				label.bottomAnchor.absolute -= (lineCount - 1) * rowHeight;
			return label;
		}

		public ControlHelp AddHelpSubPanel(String name, String help, Int32 panelIndex = 0)
		{
			ControlHelp control = new ControlHelp(this, panelIndex, name, help);
			control.HelpLabel.alignment = NGUIText.Alignment.Center;
			control.HelpLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
			return control;
		}

		public ControlToggle AddToggleOption(String description, Boolean isToggled, Action<Boolean> toggleAction, Int32 panelIndex = 0)
		{
			ControlToggle control = new ControlToggle(this, panelIndex, toggleAction);
			control.IsToggled = isToggled;
			control.Label.text = description;
			return control;
		}

		public ControlHitBox AddHitBoxOption(String description, Action hitAction, Int32 panelIndex = 0)
		{
			ControlHitBox control = new ControlHitBox(this, panelIndex, hitAction);
			control.Label.alignment = NGUIText.Alignment.Center;
			control.Label.text = description;
			return control;
		}

		public ControlInput AddInputField(String defaultValue, UIInput.Validation kind, Int32 panelIndex = 0)
		{
			ControlInput control = new ControlInput(this, panelIndex, kind);
			control.Input.label.alignment = NGUIText.Alignment.Left;
			control.Input.value = defaultValue;
			return control;
		}

		public ControlSlider AddSlider(String description, Single initial, Func<Single, Single> valueToSlide, Func<Single, Single> slideToValue, Action<Single> slideAction, Int32 panelIndex = 0)
		{
			ControlSlider control = new ControlSlider(this, panelIndex, slideAction);
			control.SetupScale(valueToSlide, slideToValue, initial);
			control.Label.text = description;
			return control;
		}

		public ControlRoll<T> AddRollOption<T>(List<T> list, Func<T, String> labelPicker, Action<T> selectAction, Int32 panelIndex = 0)
		{
			ControlRoll<T> control = new ControlRoll<T>(this, panelIndex, list, labelPicker, selectAction);
			control.Loop = true;
			return control;
		}

		public ControlList<T> AddListOneTimeSelection<T>(String name, List<T> list, Func<T, String> labelPicker, Func<T, Boolean> validChecker, Action<T> selectAction, Int32 panelIndex = 0)
		{
			ControlList<T> control = new ControlList<T>(this, panelIndex, list, name, false, labelPicker, validChecker, obj => false, selectAction);
			return control;
		}

		public ControlList<T> AddListMultipleSelection<T>(String name, List<T> list, Func<T, String> labelPicker, Func<T, Boolean> validChecker, Func<T, Boolean> enabledChecker, Action<T> selectAction, Int32 panelIndex = 0)
		{
			ControlList<T> control = new ControlList<T>(this, panelIndex, list, name, true, labelPicker, validChecker, enabledChecker, selectAction);
			return control;
		}

		public void PanelAddRow(Int32 panelIndex = 0)
		{
			PanelAddRow(allPanels[panelIndex]);
		}

		private UIWidget InstantiatePanel(Transform anchorTarget, String name, Boolean topLevelPanel)
		{
			GameObject panelGo = UnityEngine.Object.Instantiate(PrefabBasis);
			UIWidget panel = panelGo.GetComponent<UIWidget>();
			GameObject frameGo = UnityEngine.Object.Instantiate(PrefabBasis.GetChild(2));
			UIWidget frame = frameGo.GetComponent<UIWidget>();
			panelRegister[panel] = new List<List<UIWidget>>();
			allPanels.Add(panel);
			panelGo.transform.DestroyChildren();
			frameGo.transform.parent = panelGo.transform;
			panelGo.name = name;
			frameGo.GetComponentInChildren<UILocalize>().key = String.Empty;
			frameGo.GetComponentInChildren<UILabel>().text = name;
			frame.leftAnchor.target = panel.transform;
			frame.rightAnchor.target = panel.transform;
			frame.topAnchor.target = panel.transform;
			frame.bottomAnchor.target = panel.transform;
			anchorTarget = null;
			panelGo.transform.parent = anchorTarget;
			panel.leftAnchor.Set(anchorTarget, 0f, 0);
			panel.rightAnchor.Set(anchorTarget, 1f, 0);
			panel.topAnchor.Set(anchorTarget, 1f, 0);
			panel.bottomAnchor.Set(anchorTarget, 0f, 0);
			PanelAddRow(panel);
			return panel;
		}

		public T CreateUIElementForPanel<T>(UIWidget panel) where T: Component
		{
			T result = InstantiateUIElement<T>(out GameObject go, out Int32 defaultWidth, out Int32 lineCount);
			if (go != null)
			{
				if (typeof(T) == typeof(UIInput))
					result.transform.parent = panel.transform;
				else
					go.transform.parent = panel.transform;
				UIWidget element = go.GetComponent<UIWidget>();
				List<UIWidget> row = GetPanelLastRow(panel);
				if (lineCount > 1 && row.Count > 0)
				{
					PanelAddRow(panel);
					row = GetPanelLastRow(panel);
				}
				if (row.Count == 0)
				{
					List<UIWidget> previousRow = panelRegister[panel].LastOrDefault(row => row.Count > 0);
					if (previousRow == null)
					{
						// Anchor to the panel itself
						element.leftAnchor.Set(panel.transform, 0f, 50);
						element.rightAnchor.Set(panel.transform, 0f, 50 + defaultWidth);
						element.topAnchor.Set(panel.transform, 1f, -28);
						element.bottomAnchor.Set(panel.transform, 1f, -28 - lineCount * rowHeight);
					}
					else
					{
						// Anchor to the last row's first element
						UIWidget above = previousRow[0];
						element.leftAnchor.Set(above.transform, 0f, 0);
						element.rightAnchor.Set(above.transform, 0f, defaultWidth);
						element.topAnchor.Set(above.transform, 0f, -rowSeparatorHeight);
						element.bottomAnchor.Set(above.transform, 0f, -rowSeparatorHeight - lineCount * rowHeight);
					}
				}
				else
				{
					// Anchor to the element on the left on the same row
					UIWidget lastElement = row[row.Count - 1];
					element.leftAnchor.Set(lastElement.transform, 1f, elementSeparatorWidth);
					element.rightAnchor.Set(lastElement.transform, 1f, elementSeparatorWidth + defaultWidth);
					element.topAnchor.Set(lastElement.transform, 1f, 0);
					element.bottomAnchor.Set(lastElement.transform, 0f, 0);
				}
				row.Add(element);
				if (lineCount > 1)
					PanelAddRow(panel);
			}
			return result;
		}

		public T InstantiateUIElement<T>(out GameObject go, out Int32 defaultWidth, out Int32 lineCount) where T : Component
		{
			T result = null;
			go = null;
			defaultWidth = 50;
			lineCount = 1;
			if (typeof(T) == typeof(UISprite))
			{
				go = UnityEngine.Object.Instantiate(PrefabBasis.transform.GetChild(0).GetChild(0).gameObject);
				go.transform.DestroyChildren();
			}
			else if (typeof(T) == typeof(UILabel))
			{
				go = UnityEngine.Object.Instantiate(PrefabBasis.transform.GetChild(0).GetChild(1).gameObject);
				go.transform.DestroyChildren();
				UILabel label = go.GetComponent<UILabel>();
				label.overflowMethod = UILabel.Overflow.ResizeFreely;
				label.alignment = NGUIText.Alignment.Left;
				defaultWidth = 200;
				result = label as T;
			}
			else if (typeof(T) == typeof(UISlider))
			{
				go = UnityEngine.Object.Instantiate(PersistenSingleton<UIManager>.Instance.ConfigScene.ConfigList.GetChild(1).GetChild(0).GetChild(8).GetChild(1).GetChild(1).gameObject);
				UISlider slider = go.GetComponent<UISlider>();
				slider.numberOfSteps = 0;
				defaultWidth = 350;
				result = slider as T;
			}
			else if (typeof(T) == typeof(UIInput))
			{
				go = UnityEngine.Object.Instantiate(PersistenSingleton<UIManager>.Instance.NameSettingScene.NameInputField.gameObject);
				UIInput input = go.GetComponent<UIInput>();
				GameObject backgroundGo = go.GetChild(0);
				GameObject labelGo = go.GetChild(1);
				UISprite background = backgroundGo.GetComponent<UISprite>();
				background.spriteName = "battle_bar_white";
				background.color = FF9TextTool.White;
				input.label = labelGo.GetComponent<UILabel>();
				input.inputType = UIInput.InputType.Standard;
				input.onReturnKey = UIInput.OnReturnKey.Default;
				input.onValidate = null;
				input.characterLimit = -1;
				input.label.overflowMethod = UILabel.Overflow.ResizeFreely;
				input.label.color = FF9TextTool.Black;
				input.label.leftAnchor.Set(go.transform, 0f, 0);
				input.label.rightAnchor.Set(go.transform, 1f, 0);
				input.label.topAnchor.Set(go.transform, 1f, 0);
				input.label.bottomAnchor.Set(go.transform, 0f, 0);
				input.label.SetRect(0f, 0f, 500f, 1.5f * rowHeight);
				input.label.depth = 6;
				background.depth = 5;
				background.leftAnchor.Set(labelGo.transform, 0f, -25);
				background.rightAnchor.Set(labelGo.transform, 1f, 25);
				background.topAnchor.Set(labelGo.transform, 1f, 25);
				background.bottomAnchor.Set(labelGo.transform, 1f, -1.5f * rowHeight - 20);
				backgroundGo.transform.parent = go.transform;
				labelGo.transform.parent = go.transform;
				go = labelGo;
				defaultWidth = 500;
				result = input as T;
			}
			else if (typeof(T) == typeof(RecycleListPopulator))
			{
				go = UnityEngine.Object.Instantiate(PersistenSingleton<UIManager>.Instance.AbilityScene.SupportAbilityListPanel.gameObject);
				go.GetChild(2).transform.DestroyChildren();
				GameObject listPanelGo = go.GetChild(1);
				RecycleListPopulator populator = listPanelGo.GetComponent<RecycleListPopulator>();
				populator.table = listPanelGo.GetChild(0).GetComponent<UITable>();
				populator.table.columns = 1;
				populator.draggablePanel = listPanelGo.GetComponent<UIScrollView>();
				populator.panel = listPanelGo.GetComponent<UIPanel>();
				populator.panel.baseClipRegion = new Vector4(populator.panel.baseClipRegion.x, populator.panel.baseClipRegion.y, 600f, 19 * populator.cellHeight);
				populator.ScrollButton = go.GetChild(0).GetComponent<ScrollButton>();
				populator.itemPrefab = UnityEngine.Object.Instantiate(populator.itemPrefab);
				populator.itemPrefab.gameObject.SetActive(false);
				go.SetActive(true);
				defaultWidth = 600;
				lineCount = 19;
				result = populator as T;
			}
			if (result != null)
				return result;
			return go.GetComponentInChildren<T>();
		}

		private void ExtendLastRowToMax(UIWidget panel, Boolean includeVertical)
		{
			List<List<UIWidget>> panelRows = panelRegister[panel];
			if (panelRows.Count > 0 && panelRows[panelRows.Count - 1].Count > 0)
			{
				UIWidget element = panelRows[panelRows.Count - 1][panelRows[panelRows.Count - 1].Count - 1];
				if (!DisableExtending.Contains(element.GetType()))
					element.rightAnchor.Set(panel.transform, 1f, -50);
			}
			if (includeVertical)
			{
				List<UIWidget> lastRow = panelRegister[panel].LastOrDefault(row => row.Count > 0);
				if (lastRow != null)
					foreach (UIWidget element in lastRow)
						if (!DisableExtending.Contains(element.GetType()))
							element.bottomAnchor.Set(panel.transform, 0f, 28);
			}
		}

		private void PanelAddRow(UIWidget panel)
		{
			ExtendLastRowToMax(panel, false);
			panelRegister[panel].Add(new List<UIWidget>());
		}

		private List<UIWidget> GetPanelLastRow(UIWidget panel)
		{
			List<List<UIWidget>> panelRows = panelRegister[panel];
			return panelRows[panelRows.Count - 1];
		}

		private IEnumerable<UIWidget> GetPanelSubPanels(UIWidget panel)
		{
			return allPanels.Where(p => GetParentPanel(p) == panel);
		}

		private Int32 GetParentPanelIndex(UIWidget panel)
		{
			return panelParentLink[allPanels.IndexOf(panel)];
		}

		private UIWidget GetParentPanel(UIWidget panel)
		{
			Int32 parentIndex = GetParentPanelIndex(panel);
			return parentIndex >= 0 ? allPanels[parentIndex] : null;
		}

		private static GameObject PrefabBasis => PersistenSingleton<UIManager>.Instance.MainMenuScene.GenericInfoPanel;
		private static HashSet<Type> DisableExtending = new HashSet<Type>() { typeof(UISprite) };

		private Transform baseAnchor;
		private UIWidget basePanel;
		private List<UIWidget> allPanels = new List<UIWidget>();
		private Dictionary<UIWidget, List<List<UIWidget>>> panelRegister = new Dictionary<UIWidget, List<List<UIWidget>>>();
		private Dictionary<Int32, Int32> panelParentLink = new Dictionary<Int32, Int32>();
		private Int32 activePanelIndex = 0;

		private static void DebugLogComponents(GameObject startGo, Boolean recursive, Boolean recChild, Func<Component, String> logger)
		{
			foreach (Component comp in startGo.GetComponents<Component>())
			{
				String log = logger(comp);
				if (!String.IsNullOrEmpty(log))
					Log.Message($"[PANEL] {startGo} logs: {log}");
			}
			if (!recursive)
				return;
			if (recChild)
			{
				for (Int32 i = 0; i < startGo.transform.childCount; i++)
					DebugLogComponents(startGo.GetChild(i), recursive, recChild, logger);
			}
			else if (startGo.transform.parent != null)
			{
				DebugLogComponents(startGo.transform.parent.gameObject, recursive, recChild, logger);
			}
		}
	}
}

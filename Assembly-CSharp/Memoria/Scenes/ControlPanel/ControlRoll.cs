using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Sources.Scripts.UI.Common;

namespace Memoria.Scenes
{
	public class ControlRoll<T>
	{
		// List passed to this control should never be empty
		public readonly UISprite DecreaseArrow;
		public readonly UISprite IncreaseArrow;
		public readonly UILabel Label;

		private Int32 _selectionIndex;
		private Func<T, String> LabelPicker;
		private Action<T> SelectAction;

		public List<T> ObjectList { get; private set; }
		public Boolean Loop { get; set; }

		public Boolean IsEnabled
		{
			get => Label.color != FF9TextTool.Gray;
			set => Label.color = value ? FF9TextTool.White : FF9TextTool.Gray;
		}

		public Int32 SelectionIndex
		{
			get => _selectionIndex;
			set
			{
				_selectionIndex = value;
				Label.text = LabelPicker(Selection);
			}
		}

		public T Selection
		{
			get => ObjectList[SelectionIndex];
			set
			{
				Int32 index = ObjectList.IndexOf(value);
				if (index >= 0)
					SelectionIndex = index;
			}
		}

		public ControlRoll(ControlPanel control, Int32 panelIndex, List<T> objList, Func<T, String> labelPicker, Action<T> selectAction)
		{
			ObjectList = objList;
			LabelPicker = labelPicker;
			SelectAction = selectAction;
			UIWidget panel = control.GetPanel(panelIndex);
			DecreaseArrow = control.CreateUIElementForPanel<UISprite>(panel);
			Label = control.CreateUIElementForPanel<UILabel>(panel);
			IncreaseArrow = control.CreateUIElementForPanel<UISprite>(panel);
			Label.text = labelPicker(ObjectList[0]);
			Label.alignment = NGUIText.Alignment.Center;
			DecreaseArrow.atlas = FF9UIDataTool.ScreenButtonAtlas;
			IncreaseArrow.atlas = FF9UIDataTool.ScreenButtonAtlas;
			DecreaseArrow.spriteName = "button_left_idle";
			IncreaseArrow.spriteName = "button_right_idle";
			NGUITools.AddWidgetCollider(DecreaseArrow.gameObject);
			NGUITools.AddWidgetCollider(IncreaseArrow.gameObject);
			UIEventListener decreaseListener = UIEventListener.Get(DecreaseArrow.gameObject);
			UIEventListener increaseListener = UIEventListener.Get(IncreaseArrow.gameObject);
			decreaseListener.onClick += go =>
			{
				if (!IsEnabled)
					return;
				if (SelectionIndex > 0)
				{
					SelectionIndex--;
				}
				else
				{
					if (!Loop)
					{
						SelectionIndex = 0;
						return;
					}
					SelectionIndex = ObjectList.Count - 1;
				}
				SelectAction(Selection);
			};
			increaseListener.onClick += go =>
			{
				if (!IsEnabled)
					return;
				if (SelectionIndex < ObjectList.Count - 1)
				{
					SelectionIndex++;
				}
				else
				{
					if (!Loop)
					{
						SelectionIndex = ObjectList.Count - 1;
						return;
					}
					SelectionIndex = 0;
				}
				SelectAction(Selection);
			};
		}

		public void ChangeList(List<T> newList, Int32 newSelection = 0, Boolean invokeAction = false)
		{
			ObjectList = newList;
			SelectionIndex = newSelection;
			if (invokeAction)
				SelectAction(Selection);
		}
	}
}

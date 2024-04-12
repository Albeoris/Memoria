using System;

namespace Memoria.Scenes
{
	public class ControlInput
	{
		public readonly UIInput Input;
		public readonly UISprite Background;

		public Boolean IsEnabled { get; set; }

		public ControlInput(ControlPanel control, Int32 panelIndex, UIInput.Validation validation)
		{
			UIWidget panel = control.GetPanel(panelIndex);
			Input = control.CreateUIElementForPanel<UIInput>(panel);
			Input.validation = validation;
			Background = Input.transform.GetChild(0).GetComponent<UISprite>();
			IsEnabled = true;
			NGUITools.AddWidgetCollider(Input.label.gameObject);
			UIEventListener eventListener = UIEventListener.Get(Input.label.gameObject);
			eventListener.onClick += go => Input.isSelected = true;
		}
	}
}

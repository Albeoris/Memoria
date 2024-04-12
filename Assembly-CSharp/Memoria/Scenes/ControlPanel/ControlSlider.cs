using Assets.Sources.Scripts.UI.Common;
using System;
using UnityEngine;

namespace Memoria.Scenes
{
	public class ControlSlider
	{
		public readonly UISlider Slider;
		public readonly UISprite Background;
		public readonly UISprite Foreground;
		public readonly UISprite Thumb;
		public readonly UILabel Label;
		public readonly UILabel ValueLabel;

		private Single _value;
		private Action<Single> SlideAction;
		private Func<Single, Single> ValueToSlide = null;
		private Func<Single, Single> SlideToValue = null;

		public Single Value
		{
			get => _value;
			set
			{
				_value = value;
				ValueLabel.text = _value.ToString("0.##");
				if (ValueToSlide != null)
					Slider.value = Mathf.Clamp01(ValueToSlide(value));
				else
					Slider.value = Mathf.Clamp01(value);
			}
		}

		public Boolean IsEnabled
		{
			get => Label.color != FF9TextTool.Gray;
			set => Label.color = value ? FF9TextTool.White : FF9TextTool.Gray;
		}

		public ControlSlider(ControlPanel control, Int32 panelIndex, Boolean shiftAnchor, Action<Single> slideAction)
		{
			SlideAction = slideAction;
			UIWidget panel = control.GetPanel(panelIndex);
			Label = control.CreateUIElementForPanel<UILabel>(panel);
			Slider = control.CreateUIElementForPanel<UISlider>(panel);
			Background = Slider.gameObject.GetComponent<UISprite>();
			Foreground = Slider.transform.GetChild(0).gameObject.GetComponent<UISprite>();
			Thumb = Slider.transform.GetChild(1).gameObject.GetComponent<UISprite>();
			ValueLabel = control.InstantiateUIElement<UILabel>(out GameObject valueLabelGo, out _, out _);
			Label.rightAnchor.absolute = 100;
			if (shiftAnchor)
			{
				Label.topAnchor.absolute -= 20;
				Label.bottomAnchor.absolute -= 20;
			}
			valueLabelGo.transform.parent = panel.transform;
			ValueLabel.leftAnchor.Set(Background.transform, 0f, 0);
			ValueLabel.rightAnchor.Set(Background.transform, 1f, 0);
			ValueLabel.topAnchor.Set(Background.transform, 1f, 30);
			ValueLabel.bottomAnchor.Set(Background.transform, 1f, 2);
			ValueLabel.alignment = NGUIText.Alignment.Center;
			ValueLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
			UIEventListener sliderListener = UIEventListener.Get(Slider.gameObject);
			sliderListener.onPress += (go, enable) =>
			{
				if (!IsEnabled)
					return;
				if (SlideToValue != null)
					_value = SlideToValue(Mathf.Clamp01(Slider.ScreenToValue(UICamera.lastEventPosition)));
				else
					_value = Mathf.Clamp01(Slider.ScreenToValue(UICamera.lastEventPosition));
				ValueLabel.text = _value.ToString("0.##");
				SlideAction(Value);
			};
			sliderListener.onDrag += (go, delta) =>
			{
				if (!IsEnabled)
					return;
				if (SlideToValue != null)
					_value = SlideToValue(Mathf.Clamp01(Slider.ScreenToValue(UICamera.lastEventPosition)));
				else
					_value = Mathf.Clamp01(Slider.ScreenToValue(UICamera.lastEventPosition));
				ValueLabel.text = _value.ToString("0.##");
				SlideAction(Value);
			};
		}

		public void SetupScale(Func<Single, Single> valueToSlide, Func<Single, Single> slideToValue, Single initialValue)
		{
			ValueToSlide = valueToSlide;
			SlideToValue = slideToValue;
			Value = initialValue;
		}

		public static Single LinearScaleIn(Single value, Single min, Single max)
		{
			return (value - min) / (max - min);
		}

		public static Single LinearScaleOut(Single t, Single min, Single max)
		{
			return min + t * (max - min);
		}

		public static Single LogScaleIn(Single value, Single rangeFactor)
		{
			Double mlogr = Math.Log(1f / rangeFactor);
			return (Single)((mlogr - Math.Log(value)) / (2f * mlogr));
		}

		public static Single LogScaleOut(Single t, Single rangeFactor)
		{
			return (Single)Math.Pow(rangeFactor * rangeFactor, t - 0.5f);
		}
	}
}

using Assets.Scripts.Common;
using Memoria;
using System;
using UnityEngine;

namespace Assets.Sources.Scripts.Common
{
	public class ControllingCameraAspect : MonoBehaviour
	{
		private void LateUpdate()
		{
			Camera component = base.GetComponent<Camera>();
			Rect rect = component.rect;

			if (Configuration.Graphics.WidescreenSupport)
			{
				rect.width = 1;
				rect.height = 1;
				rect.x = 0;
				rect.y = 0;
			}
			else
			{
				Vector2 vector = new Vector2(FieldMap.PsxFieldWidth, FieldMap.PsxFieldHeightNative);
				if (PersistenSingleton<SceneDirector>.Instance.CurrentScene == "BattleMap" || PersistenSingleton<SceneDirector>.Instance.CurrentScene == "BattleMapDebug")
				{
					vector = new Vector2(FieldMap.PsxScreenWidth, FieldMap.PsxScreenHeightNative);
				}

				Single num = Mathf.Min((Single)Screen.width / vector.x, (Single)Screen.height / vector.y);
				Vector2 vector2 = new Vector2(vector.x * num, vector.y * num);
				Single num2 = ((Single)Screen.width - vector2.x) / (Single)Screen.width;
				Single num3 = ((Single)Screen.height - vector2.y) / (Single)Screen.height;

				rect.width = vector2.x / (Single)Screen.width;
				rect.height = vector2.y / (Single)Screen.height;
				rect.x = num2 / 2f;
				rect.y = num3 / 2f;
			}
			component.rect = rect;
		}
	}
}

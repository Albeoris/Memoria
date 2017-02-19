using System;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Sources.Scripts.Common
{
	public class ControllingCameraAspect : MonoBehaviour
	{
		private void LateUpdate()
		{
			Vector2 vector = new Vector2(320f, 224f);
			if (PersistenSingleton<SceneDirector>.Instance.CurrentScene == "BattleMap" || PersistenSingleton<SceneDirector>.Instance.CurrentScene == "BattleMapDebug")
			{
				vector = new Vector2(320f, 220f);
			}
			Single num = Mathf.Min((Single)Screen.width / vector.x, (Single)Screen.height / vector.y);
			Vector2 vector2 = new Vector2(vector.x * num, vector.y * num);
			Single num2 = ((Single)Screen.width - vector2.x) / (Single)Screen.width;
			Single num3 = ((Single)Screen.height - vector2.y) / (Single)Screen.height;
			Camera component = base.GetComponent<Camera>();
			Rect rect = component.rect;
			rect.width = vector2.x / (Single)Screen.width;
			rect.height = vector2.y / (Single)Screen.height;
			rect.x = num2 / 2f;
			rect.y = num3 / 2f;
			component.rect = rect;
		}
	}
}

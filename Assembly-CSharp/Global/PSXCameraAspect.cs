using Assets.Scripts.Common;
using Memoria;
using System;
using UnityEngine;

public class PSXCameraAspect : MonoBehaviour
{
	public Vector3 GetLocalMousePos()
	{
		Single num = FieldMap.PsxFieldWidth;
		Single num2 = FieldMap.PsxFieldHeightNative;
		if (PersistenSingleton<SceneDirector>.Instance.CurrentScene == "BattleMap" || PersistenSingleton<SceneDirector>.Instance.CurrentScene == "BattleMapDebug")
		{
			num = FieldMap.PsxScreenWidth;
			num2 = FieldMap.PsxScreenHeightNative;
		}
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.y = -((Single)Screen.height - mousePosition.y);
		mousePosition.x = (mousePosition.x - this.Border.x) / this.Size.x * num;
		mousePosition.y = (mousePosition.y + this.Border.y) / this.Size.y * num2;
		mousePosition.x -= num * 0.5f;
		mousePosition.y += num2 * 0.5f;
		return mousePosition;
	}

	public Vector3 GetLocalMousePosRelative()
	{
		Vector3 localPosition = base.transform.localPosition;
		Vector3 localMousePos = this.GetLocalMousePos();
		localMousePos.x += localPosition.x;
		localMousePos.y += localPosition.y;
		return localMousePos;
	}

	private void Start()
	{
		this.MainCamera = base.GetComponent<Camera>();
	}

	private void LateUpdate()
	{
		Rect rect = base.GetComponent<Camera>().rect;

		if (Configuration.Graphics.WidescreenSupport)
		{
			this.Ratio = 1f;

			rect.width = 1;
			rect.height = 1;
			rect.x = 0;
			rect.y = 0;

			this.Size = new Vector2(Screen.width, Screen.height);
		}
		else
		{
			Single originalWidth = FieldMap.PsxFieldWidth;
			Single originalHeight = FieldMap.PsxFieldHeightNative;

			if (PersistenSingleton<SceneDirector>.Instance.CurrentScene == "BattleMap" || PersistenSingleton<SceneDirector>.Instance.CurrentScene == "BattleMapDebug")
			{
				originalWidth = FieldMap.PsxScreenWidth;
				originalHeight = FieldMap.PsxScreenHeightNative;
			}

			this.Ratio = Mathf.Min((Single)Screen.width / originalWidth, (Single)Screen.height / originalHeight);

			Vector2 scaledSize = new Vector2(originalWidth * this.Ratio, originalHeight * this.Ratio);
			Single normalizedWidth = ((Single)Screen.width - scaledSize.x) / (Single)Screen.width;
			Single normalizedHeight = ((Single)Screen.height - scaledSize.y) / (Single)Screen.height;
			rect.width = scaledSize.x / (Single)Screen.width;
			rect.height = scaledSize.y / (Single)Screen.height;
			rect.x = normalizedWidth / 2f;
			rect.y = normalizedHeight / 2f;

			this.Size = new Vector2(scaledSize.x, scaledSize.y);
		}

		this.Border = new Vector2(rect.x * (Single)Screen.width, rect.y * (Single)Screen.height);
		this.MainCamera.rect = rect;
	}

	public Single Ratio;

	public Vector2 Border;

	public Vector2 Size;

	public Camera MainCamera;

	public Camera BgCamera;
}

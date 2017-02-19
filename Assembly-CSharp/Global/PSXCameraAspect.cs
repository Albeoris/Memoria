using System;
using Assets.Scripts.Common;
using UnityEngine;

public class PSXCameraAspect : MonoBehaviour
{
	public Vector3 GetLocalMousePos()
	{
		Single num = 320f;
		Single num2 = 224f;
		if (PersistenSingleton<SceneDirector>.Instance.CurrentScene == "BattleMap" || PersistenSingleton<SceneDirector>.Instance.CurrentScene == "BattleMapDebug")
		{
			num = 320f;
			num2 = 220f;
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
		Single num = 320f;
		Single num2 = 224f;
		if (PersistenSingleton<SceneDirector>.Instance.CurrentScene == "BattleMap" || PersistenSingleton<SceneDirector>.Instance.CurrentScene == "BattleMapDebug")
		{
			num = 320f;
			num2 = 220f;
		}
		this.Ratio = Mathf.Min((Single)Screen.width / num, (Single)Screen.height / num2);
		Vector2 vector = new Vector2(num * this.Ratio, num2 * this.Ratio);
		Single num3 = ((Single)Screen.width - vector.x) / (Single)Screen.width;
		Single num4 = ((Single)Screen.height - vector.y) / (Single)Screen.height;
		Rect rect = base.GetComponent<Camera>().rect;
		rect.width = vector.x / (Single)Screen.width;
		rect.height = vector.y / (Single)Screen.height;
		rect.x = num3 / 2f;
		rect.y = num4 / 2f;
		this.Border = new Vector2(rect.x * (Single)Screen.width, rect.y * (Single)Screen.height);
		this.Size = new Vector2(vector.x, vector.y);
		this.MainCamera.rect = rect;
	}

	public Single Ratio;

	public Vector2 Border;

	public Vector2 Size;

	public Camera MainCamera;

	public Camera BgCamera;
}

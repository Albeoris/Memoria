using System;
using UnityEngine;

public class ModelButton : MonoBehaviour
{
	private void Start()
	{
		this.worldCam = PersistenSingleton<UIManager>.Instance.BattleCamera;
	}

	private void LateUpdate()
	{
	}

	public void UpdateModelButton()
	{
		if (this.worldCam != (UnityEngine.Object)null && this.uiCam != (UnityEngine.Object)null && this.target != (UnityEngine.Object)null)
		{
			this.UpdateProperties();
		}
	}

	private void UpdateProperties()
	{
		if ((FF9StateSystem.Battle.FF9Battle.cmd_status & 2) != 0)
		{
			UIPointer pointerFromButton = Singleton<PointerManager>.Instance.GetPointerFromButton(base.gameObject);
			if (pointerFromButton == (UnityEngine.Object)null || !pointerFromButton.enabled)
			{
				Singleton<PointerManager>.Instance.SetPointerVisibility(base.gameObject, true);
			}
			if (this.lastPos != this.target.position || !this.worldCam.worldToCameraMatrix.Equals(this.lastWorldMatrix))
			{
				this.lastPos = this.target.position;
				this.lastWorldMatrix = this.worldCam.worldToCameraMatrix;
				this.worldBottomPos = this.target.position;
				this.worldTopPos = this.worldBottomPos + new Vector3(0f, this.height, 0f);
				Vector3 position = this.worldCam.WorldToScreenPoint(this.worldTopPos);
				Vector3 vector = this.uiCam.ScreenToWorldPoint(position);
				Vector3 position2 = this.worldCam.WorldToScreenPoint(this.worldBottomPos);
				Vector3 vector2 = this.uiCam.ScreenToWorldPoint(position2);
				Single num = vector.y - vector2.y;
				Single num2 = num * this.width / this.height;
				base.transform.position = new Vector3(vector2.x, vector2.y, base.transform.position.z);
				base.GetComponent<UIWidget>().height = (Int32)(num * this.multiplier / this.scaleRatio.y);
				base.GetComponent<UIWidget>().width = (Int32)(num2 * this.multiplier / this.scaleRatio.y);
			}
		}
		else
		{
			Singleton<PointerManager>.Instance.SetPointerVisibility(base.gameObject, false);
		}
	}

	public void Show(Int32 index, Boolean isEnemy, Transform target, Single width, Single height, Single scale)
	{
		this.target = target;
		this.width = width;
		this.height = height;
		this.scaleRatio = UIRoot.list[0].gameObject.transform.lossyScale;
		this.multiplier = scale;
		this.index = index;
		this.isEnemy = isEnemy;
		this.lastPos = new Vector3(Single.PositiveInfinity, Single.PositiveInfinity, Single.PositiveInfinity);
		this.lastWorldMatrix = default(Matrix4x4);
		base.transform.position = new Vector3(10000f, 0f, 0f);
	}

	public Transform GetTargetTransform()
	{
		return this.target;
	}

	public Int32 index = -1;

	public Boolean isEnemy;

	public Camera worldCam;

	public Camera uiCam;

	private Transform target;

	private Vector3 lastPos;

	private Matrix4x4 lastWorldMatrix;

	private Single height;

	private Single width;

	private Vector3 worldBottomPos;

	private Vector3 worldTopPos;

	private Vector3 scaleRatio;

	private Single multiplier;
}

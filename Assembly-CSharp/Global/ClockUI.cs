using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ClockUI : MonoBehaviour
{
	private void Start()
	{
		if (Application.isMobilePlatform || Application.isEditor)
		{
			this.origClockContainerScale = this.clockContainer.transform.localScale;
			this.clockContainerRectTrans = this.clockContainer.GetComponent<RectTransform>();
			this.origClockContainerPos = this.clockContainerRectTrans.anchoredPosition;
			this.UpdateClockSize();
			if (this.isMonitor)
			{
				base.StartCoroutine(this.WaitForUpdateMonitor());
			}
		}
		else
		{
			this.clockContainer.SetActive(false);
		}
	}

	public void Restart()
	{
		if ((Application.isMobilePlatform || Application.isEditor) && !this.isMonitor)
		{
			this.isMonitor = true;
			this.clockContainer.SetActive(true);
			this.UpdateClockSize();
			if (this.isMonitor)
			{
				base.StartCoroutine(this.WaitForUpdateMonitor());
			}
		}
	}

	private IEnumerator WaitForUpdateMonitor()
	{
		while (this.isMonitor)
		{
			this.UpdateClockText();
			yield return new WaitForSeconds(30f);
		}
		yield break;
	}

	private void LateUpdate()
	{
	}

	private void UpdateClockSize()
	{
		if (!this.isMonitor)
		{
			return;
		}
		Single num;
		if (this.hideVector.x >= 1f)
		{
			num = (Single)Screen.width / OverlayCanvas.ReferenceScreenSize.x;
		}
		else
		{
			num = (Single)Screen.height / OverlayCanvas.ReferenceScreenSize.y;
		}
		Single num2 = this.clockContainerRectTrans.sizeDelta.x * (this.origClockContainerScale.x * num);
		Single num3 = this.clockContainerRectTrans.sizeDelta.y * (this.origClockContainerScale.y * num);
		if ((UIManager.UIPillarBoxSize.x < num2 && this.hideVector.x >= 1f) || (UIManager.UIPillarBoxSize.y < num3 && this.hideVector.y >= 1f))
		{
			this.isMonitor = false;
			this.clockContainer.SetActive(false);
			return;
		}
		Single x = -1f * this.origClockContainerPos.x * (this.origClockContainerScale.x - this.origClockContainerScale.x * num) + this.origClockContainerPos.x;
		Single y = -1f * this.origClockContainerPos.y * (this.origClockContainerScale.y - this.origClockContainerScale.y * num) + this.origClockContainerPos.y;
		this.clockContainerRectTrans.anchoredPosition = new Vector2(x, y);
		this.clockContainer.transform.localScale = new Vector3(this.origClockContainerScale.x * num, this.origClockContainerScale.y * num, this.origClockContainerScale.z * num);
	}

	private void UpdateClockText()
	{
		Int32 hour = DateTime.Now.Hour;
		Int32 minute = DateTime.Now.Minute;
		this.UpdateHourImage(hour);
		this.UpdateMinuteImage(minute);
	}

	private void UpdateHourImage(Int32 hour)
	{
		Int32 number = hour % 10;
		Int32 number2 = hour / 10;
		this.clockH0Image.sprite = this.GetNumberSprite(number);
		this.clockH1Image.sprite = this.GetNumberSprite(number2);
	}

	private void UpdateMinuteImage(Int32 minute)
	{
		Int32 number = minute % 10;
		Int32 number2 = minute / 10;
		this.clockM0Image.sprite = this.GetNumberSprite(number);
		this.clockM1Image.sprite = this.GetNumberSprite(number2);
	}

	private Sprite GetNumberSprite(Int32 number)
	{
		switch (number)
		{
			case 0:
				return this.clock0Sprite;
			case 1:
				return this.clock1Sprite;
			case 2:
				return this.clock2Sprite;
			case 3:
				return this.clock3Sprite;
			case 4:
				return this.clock4Sprite;
			case 5:
				return this.clock5Sprite;
			case 6:
				return this.clock6Sprite;
			case 7:
				return this.clock7Sprite;
			case 8:
				return this.clock8Sprite;
			case 9:
				return this.clock9Sprite;
			default:
				return this.clock0Sprite;
		}
	}

	private const Single monitorUpdateTime = 30f;

	public GameObject clockContainer;

	public Image clockH0Image;

	public Image clockH1Image;

	public Image clockM0Image;

	public Image clockM1Image;

	public Boolean isMonitor = true;

	public Vector2 hideVector;

	[Header("Clock Sprite")]
	public Sprite clock0Sprite;

	public Sprite clock1Sprite;

	public Sprite clock2Sprite;

	public Sprite clock3Sprite;

	public Sprite clock4Sprite;

	public Sprite clock5Sprite;

	public Sprite clock6Sprite;

	public Sprite clock7Sprite;

	public Sprite clock8Sprite;

	public Sprite clock9Sprite;

	private Vector3 origClockContainerScale;

	private Vector3 origClockContainerPos;

	private RectTransform clockContainerRectTrans;
}

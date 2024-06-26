using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
	private void Start()
	{
		if ((Application.isMobilePlatform || Application.isEditor) && !FF9StateSystem.AndroidTVPlatform)
		{
			this.origBatteryImageSize = this.batteryImage.rectTransform.sizeDelta;
			this.origChargeImageSize = this.batteryChargeImage.rectTransform.sizeDelta;
			this.UpdateBatterySize();
			if (this.isMonitor)
			{
				base.StartCoroutine(this.WaitForUpdateMonitor());
			}
		}
		else
		{
			this.batteryImage.gameObject.SetActive(false);
			this.batteryChargeImage.gameObject.SetActive(false);
		}
	}

	public void Restart()
	{
		if ((Application.isMobilePlatform || Application.isEditor) && !FF9StateSystem.AndroidTVPlatform && !this.isMonitor)
		{
			this.isMonitor = true;
			this.batteryImage.gameObject.SetActive(true);
			this.batteryChargeImage.gameObject.SetActive(true);
			this.UpdateBatterySize();
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
			this.UpdateBatteryImage();
			yield return new WaitForSeconds(30f);
		}
		yield break;
	}

	private void LateUpdate()
	{
	}

	private void UpdateBatterySize()
	{
		if (!this.isMonitor)
		{
			return;
		}
		Single num = (Single)Screen.width / OverlayCanvas.ReferenceScreenSize.x;
		Single num2 = (Single)Screen.height / OverlayCanvas.ReferenceScreenSize.y;
		Single num3 = this.origBatteryImageSize.x * num;
		Single num4 = this.origBatteryImageSize.y * num2;
		if ((UIManager.UIPillarBoxSize.x < num3 && this.hideVector.x >= 1f) || (UIManager.UIPillarBoxSize.y < num4 && this.hideVector.y >= 1f))
		{
			this.isMonitor = false;
			this.batteryImage.gameObject.SetActive(false);
			this.batteryChargeImage.gameObject.SetActive(false);
			return;
		}
		this.batteryImage.rectTransform.sizeDelta = new Vector2(this.origBatteryImageSize.x * num, this.origBatteryImageSize.y * num2);
		this.batteryChargeImage.rectTransform.sizeDelta = new Vector2(this.origChargeImageSize.x * num, this.origChargeImageSize.y * num2);
	}

	private void UpdateBatteryImage()
	{
		Single batteryLevel = BatteryMonitor.GetBatteryLevel();
		Boolean flag = BatteryMonitor.IsBatteryCharging();
		if (batteryLevel < 0.02f)
		{
			this.batteryImage.sprite = this.battery1Sprite;
		}
		else if (batteryLevel < 0.1f)
		{
			this.batteryImage.sprite = this.battery10Sprite;
		}
		else if (batteryLevel < 0.2f)
		{
			this.batteryImage.sprite = this.battery20Sprite;
		}
		else if (batteryLevel < 0.3f)
		{
			this.batteryImage.sprite = this.battery30Sprite;
		}
		else if (batteryLevel < 0.4f)
		{
			this.batteryImage.sprite = this.battery40Sprite;
		}
		else if (batteryLevel < 0.5f)
		{
			this.batteryImage.sprite = this.battery50Sprite;
		}
		else if (batteryLevel < 0.6f)
		{
			this.batteryImage.sprite = this.battery60Sprite;
		}
		else if (batteryLevel < 0.7f)
		{
			this.batteryImage.sprite = this.battery70Sprite;
		}
		else if (batteryLevel < 0.8f)
		{
			this.batteryImage.sprite = this.battery80Sprite;
		}
		else if (batteryLevel < 0.9f)
		{
			this.batteryImage.sprite = this.battery90Sprite;
		}
		else
		{
			this.batteryImage.sprite = this.battery100Sprite;
		}
		if (flag)
		{
			this.batteryChargeImage.gameObject.SetActive(true);
		}
		else
		{
			this.batteryChargeImage.gameObject.SetActive(false);
		}
	}

	private const Single monitorUpdateTime = 30f;

	public Image batteryImage;

	public Image batteryChargeImage;

	public Boolean isMonitor = true;

	public Vector2 hideVector;

	[Header("Battery Sprite")]
	public Sprite battery100Sprite;

	public Sprite battery90Sprite;

	public Sprite battery80Sprite;

	public Sprite battery70Sprite;

	public Sprite battery60Sprite;

	public Sprite battery50Sprite;

	public Sprite battery40Sprite;

	public Sprite battery30Sprite;

	public Sprite battery20Sprite;

	public Sprite battery10Sprite;

	public Sprite battery1Sprite;

	private Vector2 origBatteryImageSize;

	private Vector2 origChargeImageSize;
}

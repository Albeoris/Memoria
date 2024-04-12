using Memoria;
using System;
using System.Collections;
using UnityEngine;

public class HUDMessage : Singleton<HUDMessage>
{
	public Single Speed => FF9StateSystem.Settings.IsFastForward ? this.speed * (Single)FF9StateSystem.Settings.FastForwardFactor : this.speed;

	public Boolean Ready => this.ready;

	public Camera WorldCamera
	{
		set => this.worldCamera = value;
	}

	private void Start()
	{
		this.worldCamera = PersistenSingleton<UIManager>.Instance.BattleCamera;
		this.activeIndexList = new Byte[(Int32)this.instanceNumber];
		if (this.childHud == null)
		{
			this.childHud = new HUDMessageChild[(Int32)this.instanceNumber];
		}
		Int32 childCount = base.transform.childCount;
		for (Byte b = 0; b < this.instanceNumber; b = (Byte)(b + 1))
		{
			this.activeIndexList[(Int32)b] = Byte.MaxValue;
			if (childCount == 0)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prototype);
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = Vector3.one;
				Transform child = gameObject.transform.GetChild(0);
				this.childHud[(Int32)b] = child.GetComponent<HUDMessageChild>();
				this.childHud[(Int32)b].Initial();
				this.childHud[(Int32)b].MessageId = b;
				this.childHud[(Int32)b].SetupCamera(this.worldCamera, this.uiCamera);
			}
			else
			{
				this.childHud[(Int32)b].SetupCamera(this.worldCamera, this.uiCamera);
				this.childHud[(Int32)b].Pause(false);
				this.childHud[(Int32)b].Clear();
			}
		}
		this.ready = true;
	}

	private void OnEnable()
	{
		if (this.ready)
		{
			this.Start();
		}
	}

	private Byte GetReadyObjectIndex()
	{
		Byte b = 0;
		Byte b2 = Byte.MaxValue;
		HUDMessageChild[] array = this.childHud;
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			HUDMessageChild message = array[i];
			if (this.IsMessageAvailable(message))
			{
				b2 = b;
				break;
			}
			b = (Byte)(b + 1);
		}
		if (b2 == 255)
		{
			throw new Exception("HUD message is not available. We will throw System.Exception");
		}
		return b2;
	}

	private Boolean IsMessageIdAvailable(Byte messageId)
	{
		Boolean result = true;
		Byte[] array = this.activeIndexList;
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			Byte b = array[i];
			if (b == messageId)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	private Boolean SetMessagIdToActive(Byte messageId)
	{
		Int32 num = 0;
		Boolean result = false;
		Byte[] array = this.activeIndexList;
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			Byte b = array[i];
			if (b == 255)
			{
				this.activeIndexList[num] = messageId;
				result = true;
				break;
			}
			num++;
		}
		return result;
	}

	private Boolean IsMessageAvailable(HUDMessageChild message)
	{
		Boolean flag = !message.gameObject.activeInHierarchy && this.IsMessageIdAvailable(message.MessageId);
		return (!flag) ? flag : this.SetMessagIdToActive(message.MessageId);
	}

	private void RemoveFromActiveList(Byte messageId)
	{
		Int32 num = 0;
		Byte[] array = this.activeIndexList;
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			Byte b = array[i];
			if (b == messageId)
			{
				this.activeIndexList[num] = Byte.MaxValue;
				break;
			}
			num++;
		}
	}

	public HUDMessageChild Show(Transform target, String message, HUDMessage.MessageStyle style, Vector3 offset, Byte delay = 0)
	{
		HUDMessageChild hudmessageChild = null;
		if (base.gameObject.activeInHierarchy)
		{
			Byte readyObjectIndex = this.GetReadyObjectIndex();
			hudmessageChild = this.childHud[readyObjectIndex];
			if (delay > 0)
				base.StartCoroutine(this.ShowProcess(hudmessageChild, target, message, style, offset, delay));
			else
				hudmessageChild.Show(target, message, style, offset);
		}
		return hudmessageChild;
	}

	private IEnumerator WaitForOriginalDelay(Byte delay)
	{
		Single cumulativeTime = 0f;
		Single frameTime = 1f / Configuration.Graphics.BattleTPS;
		Boolean exitLoop = false;
		while (!exitLoop)
		{
			if (!PersistenSingleton<UIManager>.Instance.IsPause)
			{
				cumulativeTime += FF9StateSystem.Settings.IsFastForward ? FF9StateSystem.Settings.FastForwardFactor * Time.deltaTime : Time.deltaTime;
				while (cumulativeTime >= frameTime)
				{
					cumulativeTime -= frameTime;
					if (delay == 0)
					{
						exitLoop = true;
						break;
					}
					delay--;
				}
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	private IEnumerator ShowProcess(HUDMessageChild messsageObject, Transform target, String message, HUDMessage.MessageStyle style, Vector3 offset, Byte delay = 0)
	{
		yield return base.StartCoroutine(this.WaitForOriginalDelay(delay));
		while (PersistenSingleton<UIManager>.Instance.IsPause)
			yield return new WaitForEndOfFrame();
		messsageObject.Show(target, message, style, offset);
		yield break;
	}

	public void ReleaseObject(HUDMessageChild messageObject)
	{
		messageObject.Clear();
	}

	public void FinishMessage(Byte messageId)
	{
		this.RemoveFromActiveList(messageId);
	}

	public void UpdateChildPosition()
	{
		HUDMessageChild[] array = this.childHud;
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			HUDMessageChild hudmessageChild = array[i];
			if (hudmessageChild.gameObject.activeInHierarchy)
			{
				hudmessageChild.Follower.UpdateUIPosition();
			}
		}
	}

	public void Pause(Boolean isPause)
	{
		HUDMessageChild[] array = this.childHud;
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			HUDMessageChild hudmessageChild = array[i];
			if (hudmessageChild.gameObject.activeInHierarchy)
			{
				hudmessageChild.Pause(isPause);
			}
		}
	}

	[SerializeField]
	private Camera worldCamera;

	public Camera uiCamera;

	public GameObject prototype;

	public Byte instanceNumber = 10;

	[SerializeField]
	private Single speed = 1f;

	private Boolean ready;

	public AnimationCurve alphaTweenCurve;

	public Color damageColor;

	public AnimationCurve damageTweenCurve;

	public Color restoreColor;

	public AnimationCurve restoreTweenCurve;

	public Color criticalColor;

	public AnimationCurve criticalTweenCurve;

	public static readonly Vector3 NormalTargetPosition = new Vector3(0f, 25f * UIManager.ResourceYMultipier);

	public static readonly Vector3 RecoverTargetPosition = new Vector3(0f, 15f * UIManager.ResourceYMultipier);

	private HUDMessageChild[] childHud;

	[SerializeField]
	private Byte[] activeIndexList;

	public enum MessageStyle
	{
		NONE,
		DAMAGE,
		GUARD,
		MISS,
		DEATH,
		RESTORE_HP,
		RESTORE_MP,
		DEATH_SENTENCE,
		PETRIFY,
		CRITICAL
	}
}

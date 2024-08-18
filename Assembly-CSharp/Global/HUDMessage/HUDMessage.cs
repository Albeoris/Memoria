using Memoria;
using System;
using System.Collections;
using UnityEngine;

public class HUDMessage : Singleton<HUDMessage>
{
    public Single Speed => FF9StateSystem.Settings.IsFastForward ? this.speed * (Single)FF9StateSystem.Settings.FastForwardFactor : this.speed;
    public Boolean Ready => this.ready;
    public HUDMessageChild[] AllMessagePool => this.childHud;

    public Camera WorldCamera
    {
        set => this.worldCamera = value;
    }

    private void CreateMessageInstance(Byte id)
    {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.prototype);
        gameObject.transform.parent = base.transform;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localScale = Vector3.one;
        Transform child = gameObject.transform.GetChild(0);
        this.childHud[id] = child.GetComponent<HUDMessageChild>();
        this.childHud[id].Initial();
        this.childHud[id].MessageId = id;
        this.childHud[id].SetupCamera(this.worldCamera, this.uiCamera);
    }

    private void Start()
    {
        this.worldCamera = PersistenSingleton<UIManager>.Instance.BattleCamera;
        this.activeIndexList = new Byte[this.instanceNumber];
        if (this.childHud == null)
            this.childHud = new HUDMessageChild[this.instanceNumber];
        Int32 childCount = base.transform.childCount;
        for (Int32 i = 0; i < this.instanceNumber; i++)
        {
            this.activeIndexList[i] = Byte.MaxValue;
            if (childCount == 0)
            {
                this.CreateMessageInstance((Byte)i);
            }
            else
            {
                this.childHud[i].SetupCamera(this.worldCamera, this.uiCamera);
                this.childHud[i].Pause(false);
                this.childHud[i].Clear();
            }
        }
        this.ready = true;
    }

    private void OnEnable()
    {
        if (this.ready)
            this.Start();
    }

    private Byte GetReadyObjectIndex()
    {
        Byte poolCount = (Byte)this.childHud.Length;
        for (Byte i = 0; i < poolCount; i++)
            if (this.ActivateMessageIfAvailable(this.childHud[i]))
                return i;
        Array.Resize(ref this.childHud, poolCount + 1);
        Array.Resize(ref this.activeIndexList, poolCount + 1);
        this.CreateMessageInstance(poolCount);
        this.activeIndexList[poolCount] = Byte.MaxValue;
        this.instanceNumber++;
        return poolCount;
    }

    private Boolean IsMessageIdAvailable(Byte messageId)
    {
        foreach (Byte index in this.activeIndexList)
            if (index == messageId)
                return false;
        return true;
    }

    private Boolean SetMessagIdToActive(Byte messageId)
    {
        for (Int32 i = 0; i < this.activeIndexList.Length; i++)
        {
            if (this.activeIndexList[i] == Byte.MaxValue)
            {
                this.activeIndexList[i] = messageId;
                return true;
            }
        }
        return false;
    }

    private Boolean ActivateMessageIfAvailable(HUDMessageChild message)
    {
        Boolean isAvailable = !message.gameObject.activeInHierarchy && this.IsMessageIdAvailable(message.MessageId);
        return isAvailable ? this.SetMessagIdToActive(message.MessageId) : false;
    }

    private void RemoveFromActiveList(Byte messageId)
    {
        for (Int32 i = 0; i < this.activeIndexList.Length; i++)
        {
            if (this.activeIndexList[i] == messageId)
            {
                this.activeIndexList[i] = Byte.MaxValue;
                return;
            }
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
        foreach (HUDMessageChild message in this.childHud)
            if (message.gameObject.activeInHierarchy)
                message.Follower.UpdateUIPosition();
    }

    public void Pause(Boolean isPause)
    {
        foreach (HUDMessageChild message in this.childHud)
            if (message.gameObject.activeInHierarchy)
                message.Pause(isPause);
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

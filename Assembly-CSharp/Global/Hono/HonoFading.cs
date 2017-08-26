using System;
using System.Collections;
using Memoria;
using UnityEngine;

public class HonoFading : MonoBehaviour
{
	public UISprite ForegroundSprite
	{
		get
		{
			return this.foregroundSprite;
		}
	}

	private void Awake()
	{
		this.foregroundSprite = base.GetComponent<UISprite>();
		this.tweenAlpha.enabled = false;
		this.mTransform = base.transform;
	}

	public void SetAlphaTween(Single alphaFrom, Single alphaTo, Single duration, Single delay, AnimationCurve animCurve)
	{
		this.tweenAlpha.from = alphaFrom;
		this.tweenAlpha.to = alphaTo;
		this.tweenAlpha.duration = duration;
		this.tweenAlpha.delay = delay;
		this.tweenAlpha.animationCurve = animCurve;
	}

	public void Fade(Single alphaFrom, Single alphaTo, Single duration, Single delay, AnimationCurve animCurve, UIScene.SceneVoidDelegate callback = null)
	{
        if (base.gameObject.activeInHierarchy && !this.busy)
		{
		    if (Configuration.Graphics.WidescreenSupport)
		    {
		        UISprite faddingSprite = gameObject.GetComponent<UISprite>();
		        faddingSprite.width = faddingSprite.height * Screen.width / Screen.height;
		    }

            this.busy = true;
			this.SetAlphaTween(alphaFrom, alphaTo, duration, delay, animCurve);
			this.tweenAlpha.ResetToBeginning();
			this.tweenAlpha.enabled = true;
			base.gameObject.SetActive(true);

		    if (callback != null)
			{
				base.StartCoroutine("WaitForAnimation", callback);
			}
			else
			{
				base.StartCoroutine(this.WaitForAnimation(callback));
			}
		}
	}

	public void FadeIn(UIScene.SceneVoidDelegate callback = null)
	{
		this.Fade(this.fadeInFrom, this.fadeInTo, this.fadeInDuration, this.fadeInDelay, this.fadeInCurve, callback);
	}

	public void FadeOut(UIScene.SceneVoidDelegate callback = null)
	{
		this.Fade(this.fadeOutFrom, this.fadeOutTo, this.fadeOutDuration, this.fadeOutDelay, this.fadeOutCurve, callback);
	}

	public void FadePingPong(UIScene.SceneVoidDelegate blackSceneCallback = null, UIScene.SceneVoidDelegate finishCallback = null)
	{
		if (base.gameObject.activeInHierarchy)
		{
            base.StartCoroutine(this.PingPongProcess(blackSceneCallback, finishCallback));
		}
	}

	public void Stop()
	{
		this.tweenAlpha.enabled = false;
		base.StopCoroutine("WaitForAnimation");
		this.busy = false;
	}

	private IEnumerator PingPongProcess(UIScene.SceneVoidDelegate blackSceneCallback = null, UIScene.SceneVoidDelegate finishCallback = null)
	{
		this.FadeIn(blackSceneCallback);
		yield return new WaitForSeconds(this.fadeInDuration + this.fadeInDelay + 0.01f);
		this.FadeOut(finishCallback);
		yield break;
	}

    private IEnumerator SkipPingPongProcess(UIScene.SceneVoidDelegate blackSceneCallback = null, UIScene.SceneVoidDelegate finishCallback = null)
    {
        blackSceneCallback?.Invoke();
        yield return new WaitForEndOfFrame();
        finishCallback?.Invoke();
    }

    private IEnumerator WaitForAnimation(UIScene.SceneVoidDelegate callback)
	{
		yield return new WaitForSeconds(this.tweenAlpha.duration + this.tweenAlpha.delay);
		this.busy = false;
		if (callback != null)
		{
			callback();
		}
		yield break;
	}

    private IEnumerator SkipAnimation(UIScene.SceneVoidDelegate callback)
    {
        this.busy = false;
        if (callback != null)
        {
            callback();
        }
        yield break;
    }

    private void OnDisable()
	{
		this.busy = false;
		this.tweenAlpha.enabled = false;
	}

	[SerializeField]
	private Boolean busy;

	public TweenAlpha tweenAlpha;

	public AnimationCurve fadeInCurve;

	public Single fadeInFrom;

	public Single fadeInTo = 1f;

	public Single fadeInDuration = 0.4f;

	public Single fadeInDelay;

	public AnimationCurve fadeOutCurve;

	public Single fadeOutFrom = 1f;

	public Single fadeOutTo;

	public Single fadeOutDuration = 0.4f;

	public Single fadeOutDelay;

	public Boolean debug;

	private UISprite foregroundSprite;

	private Transform mTransform;
}

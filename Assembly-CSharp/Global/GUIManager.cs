using System;
using Assets.Scripts.Common;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
	private void Awake()
	{
		global::Debug.Log("50 GUIManager.Awake");
		ExpansionVerifier.printLog("GUIManager : awake");
		this._skipMenuScene = false;
		this._isChangingScene = false;
		FF9StateSystem.Settings.ReadSystemData(delegate
		{
			this._skipMenuScene = true;
		});
		ExpansionVerifier.printLog("GUIManager : _skipMenuScene = " + this._skipMenuScene);
	}

	private void LateUpdate()
	{
		if (this._skipMenuScene && !this._isChangingScene)
		{
			this._isChangingScene = true;
			TimerUI.SetEnable(false);
			ExpansionVerifier.printLog("GUIManager : goto Title");
			SceneDirector.Replace("Title", SceneTransition.FadeOutToBlack_FadeIn, true);
			SceneDirector.ToggleFadeAll(false);
		}
	}

	private void OnDestroy()
	{
		GameObject gameObject = GameObject.Find("Camera_SplashTitle");
		if (gameObject != (UnityEngine.Object)null)
		{
			global::Debug.Log("destroy camera_splashTitle");
			UnityEngine.Object.Destroy(gameObject);
		}
		else
		{
			global::Debug.Log("CANNOT FIND camera_splashTitle");
		}
	}

	private void OnGUI()
	{
		if (this._skipMenuScene)
		{
			return;
		}
	}
	
	private void SetFieldMap(short no, int sc = 0, int id = -1)
	{
		FF9StateSystem.Common.FF9.fldMapNo = no;
		PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(EBin.SC_COUNTER_SVR, sc);
		PersistenSingleton<EventEngine>.Instance.eBin.setVarManually(EBin.MAP_INDEX_SVR, id);
	}

	private Vector2 scrollPosition = new Vector2(0f, 0f);

	private Boolean _skipMenuScene;

	private Boolean _isChangingScene;
}

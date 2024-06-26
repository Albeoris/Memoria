using System;
using Assets.Scripts.Common;
using Assets.SiliconSocial;
using UnityEngine;

public class EndingMain : HonoBehavior
{
	public override void HonoAwake()
	{
		base.HonoAwake();
		this.fadeblack = false;
	}

	public override void HonoStart()
	{
		base.HonoStart();
		PersistenSingleton<UIManager>.Instance.ChangeUIState(UIManager.UIState.Ending);
		if (PersistenSingleton<UIManager>.Instance.PreviousState == UIManager.UIState.EndGame)
		{
			this.ff9endingState = EndingMain.FF9EndingState.ENDING_STATE_IMAGE;
		}
		else
		{
			this.ff9endingState = EndingMain.FF9EndingState.ENDING_STATE_FMV059;
		}
		SceneDirector.InitFade(FadeMode.Sub, 0, new Color32(0, 0, 0, Byte.MaxValue));
	}

	public override void HonoUpdate()
	{
		base.HonoUpdate();
		if (this.isFirstFrame)
		{
			this.isFirstFrame = false;
			return;
		}
		switch (this.ff9endingState)
		{
		case EndingMain.FF9EndingState.ENDING_STATE_FMV059:
			MBG.Instance.LoadMovie(MBG.MBGDiscTable[4][18].name);
			MBG.Instance.SetModeEnding();
			MBG.Instance.SetFinishCallback(delegate
			{
				this.ff9endingState = EndingMain.FF9EndingState.ENDING_STATE_TEXT;
				MBG.Instance.Purge();
			});
			MBG.Instance.Play();
			this.ff9endingState = EndingMain.FF9EndingState.WAIT_FMV059_END;
			break;
		case EndingMain.FF9EndingState.WAIT_FMV059_END:
			if (!this.fadeblack && MBG.Instance.GetFrameCount - MBG.Instance.GetFrame < 60)
			{
				this.fadeblack = true;
				SceneDirector.InitFade(FadeMode.Sub, 0, new Color32(0, 0, 0, Byte.MaxValue));
				SceneDirector.InitFade(FadeMode.Sub, 60, new Color32(Byte.MaxValue, Byte.MaxValue, Byte.MaxValue, Byte.MaxValue));
			}
			break;
		case EndingMain.FF9EndingState.ENDING_STATE_TEXT:
			SceneDirector.InitFade(FadeMode.Sub, 1, new Color32(0, 0, 0, Byte.MaxValue));
			PersistenSingleton<UIManager>.Instance.EndingScene.endingSlideshow.PlayEndingText(delegate
			{
				this.ff9endingState = EndingMain.FF9EndingState.ENDING_STATE_FMV060;
			});
			this.ff9endingState = EndingMain.FF9EndingState.WAIT_ENDING_STATE_TEXT;
			break;
		case EndingMain.FF9EndingState.ENDING_STATE_FMV060:
			MBG.Instance.LoadMovie(MBG.MBGDiscTable[4][19].name);
			MBG.Instance.SetModeEnding();
			MBG.Instance.Play();
			MBG.Instance.SetFinishCallback(delegate
			{
				this.ff9endingState = EndingMain.FF9EndingState.ENDING_STATE_IMAGE;
			});
			this.ff9endingState = EndingMain.FF9EndingState.WAIT_FMV060_END;
			break;
		case EndingMain.FF9EndingState.ENDING_STATE_IMAGE:
			PersistenSingleton<UIManager>.Instance.EndingScene.endingSlideshow.PlayLastEndingText(delegate
			{
				this.ff9endingState = EndingMain.FF9EndingState.ENDING_STATE_END;
				PersistenSingleton<UIManager>.Instance.SetMenuControlEnable(true);
				PersistenSingleton<UIManager>.Instance.SetUIPauseEnable(true);
				PersistenSingleton<UIManager>.Instance.EndingScene.ReadyToBlackjack();
				FF9Snd.ff9endsnd_song_play(156);
				FF9Snd.ff9endsnd_song_vol_fade(156, 90, 0, 127);
				FF9StateSystem.Serializer.SetGameFinishFlagWithTrue(delegate(DataSerializerErrorCode errNo, Boolean isSuccess)
				{
					if (errNo != DataSerializerErrorCode.Success || !isSuccess)
					{
					}
				});
				AchievementManager.ReportAchievement(AcheivementKey.CompleteGame, 1);
			});
			this.ff9endingState = EndingMain.FF9EndingState.WAITENDINGSTATEIMAGE;
			break;
		}
		SceneDirector.ServiceFade();
	}

	private const Int32 ENDFMV_PARM_FMV_FADEFRAME = 60;

	private const Int32 NONE = -1;

	private EndingMain.FF9EndingState ff9endingState;

	private Boolean isFirstFrame;

	private Boolean fadeblack;

	public enum FF9EndingState
	{
		ENDING_STATE_FMV059,
		WAIT_FMV059_END,
		ENDING_STATE_TEXT,
		WAIT_ENDING_STATE_TEXT,
		ENDING_STATE_FMV060,
		WAIT_FMV060_END,
		ENDING_STATE_IMAGE,
		WAITENDINGSTATEIMAGE,
		ENDING_STATE_MINIGAME,
		ENDING_STATE_END
	}
}

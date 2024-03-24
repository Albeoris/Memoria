using System;
using Assets.Sources.Scripts.UI.Common;
using UnityEngine;
using Memoria;

public class TimerUI : Singleton<TimerUI>
{
	public static Boolean Play
	{
		get
		{
			return TimerUI.playFlag;
		}
		set
		{
			TimerUI.playFlag = value;
			if (value)
			{
				TimerUI.digit3.color = TimerUI.clearColor;
				TimerUI.digit2.color = TimerUI.clearColor;
				TimerUI.digit1.color = TimerUI.clearColor;
				TimerUI.digit0.color = TimerUI.clearColor;
				TimerUI.collon.color = TimerUI.clearColor;
			}
			else
			{
				TimerUI.digit3.color = TimerUI.fadeColor;
				TimerUI.digit2.color = TimerUI.fadeColor;
				TimerUI.digit1.color = TimerUI.fadeColor;
				TimerUI.digit0.color = TimerUI.fadeColor;
				TimerUI.collon.color = TimerUI.fadeColor;
			}
		}
	}

	public static void SetTime(Int32 second)
	{
		if (!TimerUI.isCreated)
		{
			TimerUI.Create(true);
		}
		TimerUI.time = (Single)second - 0.01f;
		if (TimerUI.time < 0f)
		{
			TimerUI.time = 0f;
		}
		TimerUI.SetDisplayTime((Double)TimerUI.time);
	}

	public static void SetTimeFromAutoSave(Int32 second)
	{
		FF9StateSystem.Settings.UpdateTickTime();
		TimerUI.SetTime(second);
	}

	public static Single Time
	{
		get
		{
			return TimerUI.time;
		}
	}

	public static Boolean Enable
	{
		get
		{
			return TimerUI.enable;
		}
	}

	public static void SetEnable(Boolean enable)
	{
		if (!TimerUI.isCreated)
		{
			TimerUI.Create(true);
		}
		TimerUI.enable = enable;
		TimerUI.Play = false;
		TimerUI.backgroundSprite.atlas = ((FF9StateSystem.Settings.cfg.win_type != 0UL) ? FF9UIDataTool.BlueAtlas : FF9UIDataTool.GrayAtlas);
		TimerUI.SetActive(enable);
		TimerUI.blinkState = (TimeSpan.FromSeconds((Double)TimerUI.time).Milliseconds > 500);
		if (TimerUI.lastBlinkState != TimerUI.blinkState)
		{
			TimerUI.UpdateBlinkState();
		}
	}

	public static void SetPlay(Boolean play)
	{
		if (!TimerUI.enable)
		{
			return;
		}
		if (!TimerUI.isCreated)
		{
			TimerUI.Create(true);
		}
		TimerUI.Play = play;
		FF9StateSystem.Settings.UpdateTickTime();
		TimerUI.lastTime = FF9StateSystem.Settings.time;
	}

	public static Boolean GetDisplay()
	{
		return TimerUI.show;
	}

	public static void SetDisplay(Boolean isShown)
	{
		if (!TimerUI.isCreated)
		{
			return;
		}
		TimerUI.show = isShown;
		TimerUI.SetActive(isShown);
		TimerUI.UpdateBlinkState();
	}

	public static void Init()
	{
		if (TimerUI.enable && !TimerUI.isCreated)
		{
			TimerUI.Create(false);
		}
	}

	private static void Create(Boolean resetFlag)
	{
		GameObject prefab = Resources.Load("EmbeddedAsset/UI/Prefabs/Timer Container") as GameObject;
		GameObject gameObject = NGUITools.AddChild(PersistenSingleton<UIManager>.Instance.gameObject, prefab);
		gameObject.name = "Timer Container";
		TimerUI.digit0 = gameObject.gameObject.FindChild("Digit Panel/Digit0").GetComponent<UISprite>();
		TimerUI.digit1 = gameObject.gameObject.FindChild("Digit Panel/Digit1").GetComponent<UISprite>();
		TimerUI.digit2 = gameObject.gameObject.FindChild("Digit Panel/Digit2").GetComponent<UISprite>();
		TimerUI.digit3 = gameObject.gameObject.FindChild("Digit Panel/Digit3").GetComponent<UISprite>();
		TimerUI.collon = gameObject.gameObject.FindChild("Digit Panel/Collon").GetComponent<UISprite>();
		TimerUI.background = gameObject.gameObject.FindChild("Background");
		TimerUI.backgroundSprite = gameObject.gameObject.FindChild("Background").GetComponent<UISprite>();
		if (resetFlag)
		{
			TimerUI.ResetFlag();
		}
		TimerUI.SetActive(false);
	}

	private static void ResetFlag()
	{
		TimerUI.digit3Enabled = true;
		TimerUI.blinkState = true;
		TimerUI.lastBlinkState = true;
		TimerUI.enable = false;
		TimerUI.Play = false;
		TimerUI.show = true;
	}

	private static void SetDisplayTime(Double second)
	{
		TimerUI.SetDisplayTime(TimeSpan.FromSeconds(second));
	}

	private static void SetDisplayTime(TimeSpan t)
	{
		Int32 num = (t.Minutes + t.Hours * 60) % 100;
		Int32 seconds = t.Seconds;
		String text = num.ToString("D2");
		String text2 = seconds.ToString("D2");
		TimerUI.SetActive(true);
		TimerUI.digit0.spriteName = "counter_" + text2[1];
		TimerUI.digit1.spriteName = "counter_" + text2[0];
		TimerUI.digit2.spriteName = "counter_" + text[1];
		TimerUI.digit3.spriteName = "counter_" + text[0];
		TimerUI.digit3Enabled = (text[0] != '0');
		TimerUI.SetActive(TimerUI.enable);
		TimerUI.UpdateBlinkState();
	}

	private static void UpdateBlinkState()
	{
		TimerUI.digit0.gameObject.SetActive(TimerUI.show && TimerUI.enable && (TimerUI.blinkState || TimerUI.Play));
		TimerUI.digit1.gameObject.SetActive(TimerUI.show && TimerUI.enable && (TimerUI.blinkState || TimerUI.Play));
		TimerUI.digit2.gameObject.SetActive(TimerUI.show && TimerUI.enable && (TimerUI.blinkState || TimerUI.Play));
		TimerUI.digit3.gameObject.SetActive(TimerUI.show && TimerUI.enable && (TimerUI.blinkState || TimerUI.Play) && TimerUI.digit3Enabled);
		TimerUI.collon.gameObject.SetActive(TimerUI.show && TimerUI.enable && (TimerUI.blinkState || TimerUI.Play));
		TimerUI.lastBlinkState = TimerUI.blinkState;
	}

	private static void SetActive(Boolean value)
	{
		TimerUI.digit0.gameObject.SetActive(value);
		TimerUI.digit1.gameObject.SetActive(value);
		TimerUI.digit2.gameObject.SetActive(value);
		TimerUI.digit3.gameObject.SetActive(value);
		TimerUI.collon.gameObject.SetActive(value);
		TimerUI.background.SetActive(TimerUI.show && TimerUI.enable && value);
	}

	protected override void Awake()
	{
		TimerUI.isCreated = true;
	}

	private void Update()
	{
		if (TimerUI.Play)
		{
			if (PersistenSingleton<UIManager>.Instance.State != UIManager.UIState.Pause && !PersistenSingleton<UIManager>.Instance.QuitScene.isShowQuitUI)
				FF9StateSystem.Settings.UpdateTickTime();
			Single diffTime = (Single)(FF9StateSystem.Settings.time - TimerUI.lastTime);
			Int32 Speedtimer = Configuration.Cheats.SpeedTimer == 0 ? 1 : Configuration.Cheats.SpeedTimer; // Prevent old Memoria.ini file to disable timer (0 by default)
            TimerUI.lastTime = FF9StateSystem.Settings.time;
			if (Configuration.Cheats.SpeedTimer != -1)
			{
                if (FF9StateSystem.Settings.IsFastForward && Configuration.Cheats.SpeedTimer >= 0)
                    TimerUI.time -= diffTime * Configuration.Cheats.SpeedFactor * Speedtimer;
                else if (Configuration.Cheats.SpeedTimer >= 0)
                    TimerUI.time -= diffTime * Speedtimer;
                else if (TimerUI.time < 0f)
                    TimerUI.time = 0f;
				else
					TimerUI.time -= diffTime;
            }
			TimeSpan displayTime = TimeSpan.FromSeconds(TimerUI.time);
			TimerUI.blinkState = displayTime.Milliseconds > 500;
			if (TimerUI.lastBlinkState != TimerUI.blinkState)
				TimerUI.UpdateBlinkState();
			if (TimerUI.lastTimespan.Seconds != displayTime.Seconds)
				TimerUI.SetDisplayTime(displayTime);
			TimerUI.lastTimespan = displayTime;
		}
		else if (TimerUI.enable)
		{
			if (PersistenSingleton<UIManager>.Instance.State != UIManager.UIState.Pause && !PersistenSingleton<UIManager>.Instance.QuitScene.isShowQuitUI)
				FF9StateSystem.Settings.UpdateTickTime();
			TimerUI.blinkState = TimeSpan.FromSeconds(FF9StateSystem.Settings.time).Milliseconds > 500;
			if (TimerUI.lastBlinkState != TimerUI.blinkState)
				TimerUI.UpdateBlinkState();
		}
	}

	private void OnDestroy()
	{
		TimerUI.isCreated = false;
	}

	private static Boolean isCreated = false;

	private static UISprite digit3;

	private static UISprite digit2;

	private static UISprite digit1;

	private static UISprite digit0;

	private static UISprite collon;

	private static GameObject background;

	private static UISprite backgroundSprite;

	private static Boolean digit3Enabled;

	private static Single time;

	private static Boolean enable;

	private static Boolean show;

	private static Boolean blinkState;

	private static Boolean lastBlinkState;

	private static Boolean playFlag;

	private static Double lastTime;

	private static TimeSpan lastTimespan;

	private static Color clearColor = new Color(1f, 1f, 1f, 1f);

	private static Color fadeColor = new Color(1f, 1f, 1f, 0.5f);
}

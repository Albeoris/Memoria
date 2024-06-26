using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Assets.Sources.Scripts.Common;
using UnityEngine;
using Object = System.Object;

public class OSDLogger
{
	public Boolean IsShowLog { get; set; }

	public static void AddStaticMessage(String msg)
	{
		OSDLogger.Instance._staticMsgs.AppendLine(msg);
	}

	public static void SetBundleVersion(String verstr)
	{
		OSDLogger.Instance._bundleVersion = verstr;
		OSDLogger.Instance._title = String.Concat(new String[]
		{
			"[Bundle Version : ",
			OSDLogger.Instance._bundleVersion,
			" / Assembly Date : ",
			OSDLogger.Instance._asmVersion,
			"]"
		});
	}

	public void Init()
	{
		this._ReadAssemblyVersion();
		this.IsShowLog = false;
		this._title = "Logger";
		this._screenTex = Texture2D.whiteTexture;
		this._state = 0;
		this._maxStateCount = (Int32)Enum.GetNames(typeof(OSDLogger.LogState)).Length;
		this._maxLogEntry = 128;
		this._logCounter = 0;
		this._lastLogType = LogType.Error;
		this._lastLogString = String.Empty;
		this._logTypes = new LinkedList<LogType>();
		this._logs = new LinkedList<String>();
		this._stackTraces = new LinkedList<String>();
		this._logMsgs = new StringBuilder();
		this._stats = new StringBuilder();
		this._staticMsgs = new StringBuilder();
		this._staticMsgs.AppendLine("------------------------------");
		this._staticMsgs.AppendLine("Static string:");
		this._staticMsgs.AppendLine("------------------------------");
		this._scrollPosition = Vector2.zero;
		this._touchPos = Vector2.zero;
		this._isTouching = false;
		this._momentumY = 0f;
		this._titleRect = default(Rect);
		this._elapseTime = 0f;
		this._accum = 0f;
		this._frames = 0;
		this._timeLeft = this.UpdateInterval;
		this._fpsText = String.Empty;
		this._maxUsedMemSize = 0L;
	}

	private void _ReadAssemblyVersion()
	{
		Version version = Assembly.GetExecutingAssembly().GetName().Version;
		DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0);
		this._asmVersion = dateTime.AddDays((Double)version.Build).AddSeconds((Double)(version.Revision * 2)).ToString("dd/MM/yyyy, HH:mm");
	}

	public void Update()
	{
	}

	private void _UpdateFPS()
	{
		this._timeLeft -= Time.deltaTime;
		this._accum += Time.timeScale / Time.deltaTime;
		this._frames++;
		if ((Double)this._timeLeft <= 0.0)
		{
			Single num = this._accum / (Single)this._frames;
			this._fpsText = String.Format("{0:F2} FPS", num);
			this._timeLeft = this.UpdateInterval;
			this._accum = 0f;
			this._frames = 0;
		}
	}

	public void EnableLog()
	{
		Application.logMessageReceived += this._HandleLog;
	}

	public void DisableLog()
	{
		Application.logMessageReceived -= this._HandleLog;
	}

	private void _HandleLog(String logString, String stackTrace, LogType type)
	{
		this._logCounter++;
		if (type == this._lastLogType)
		{
			if (String.CompareOrdinal(logString, this._lastLogString) == 0)
			{
				return;
			}
		}
		else
		{
			this._lastLogType = type;
			this._lastLogString = logString;
		}
		String[] array = stackTrace.Split(new Char[]
		{
			'\n'
		});
		StringBuilder stringBuilder = new StringBuilder();
		Int32 num = 0;
		stringBuilder.Append(array[1]);
		Int32 num2 = 2;
		while (num2 < (Int32)array.Length && num < 5)
		{
			if (!String.IsNullOrEmpty(array[num2]))
			{
				stringBuilder.Append("\n" + array[num2]);
			}
			num2++;
			num++;
		}
		String str = stringBuilder.ToString();
		String str2 = String.Concat(new Object[]
		{
			"{",
			this._logCounter,
			"}:[",
			type,
			"]"
		});
		if (this._logs.Count >= this._maxLogEntry)
		{
			LinkedListNode<LogType> linkedListNode = this._logTypes.First;
			LinkedListNode<String> linkedListNode2 = this._logs.First;
			LinkedListNode<String> linkedListNode3 = this._stackTraces.First;
			this._logTypes.RemoveFirst();
			this._logs.RemoveFirst();
			this._stackTraces.RemoveFirst();
			linkedListNode.Value = type;
			linkedListNode2.Value = str2 + logString;
			linkedListNode3.Value = str2 + str;
			this._logTypes.AddLast(linkedListNode);
			this._logs.AddLast(linkedListNode2);
			this._stackTraces.AddLast(linkedListNode3);
		}
		else
		{
			LinkedListNode<LogType> linkedListNode = new LinkedListNode<LogType>(type);
			LinkedListNode<String> linkedListNode2 = new LinkedListNode<String>(str2 + logString);
			LinkedListNode<String> linkedListNode3 = new LinkedListNode<String>(str2 + str);
			this._logTypes.AddLast(linkedListNode);
			this._logs.AddLast(linkedListNode2);
			this._stackTraces.AddLast(linkedListNode3);
		}
		if (!this._isTouching && this._momentumY <= 0f)
		{
			this._scrollPosition.y = Single.PositiveInfinity;
		}
	}

	private void _BuildProfilerStatsString()
	{
		Int64 totalMemory = GC.GetTotalMemory(false);
		if (totalMemory > this._maxUsedMemSize)
		{
			this._maxUsedMemSize = totalMemory;
		}
		this._stats.Length = 0;
		this._stats.Append("FPS " + this._fpsText);
		this._stats.AppendLine();
		this._stats.Append("Mono Memory ");
		this._stats.Append(((Single)totalMemory / 1048576f).ToString("00.0") + " / ");
		this._stats.Append((Profiler.GetMonoHeapSize() / 1048576f).ToString("00.0") + " MB");
		if (Profiler.GetMonoHeapSize() > 0u)
		{
			this._stats.Append(" (" + ((Single)totalMemory * 100f / Profiler.GetMonoHeapSize()).ToString("00.0") + "%)");
		}
		this._stats.AppendLine();
		this._stats.Append("Assets Memory ");
		this._stats.Append((Profiler.GetTotalAllocatedMemory() / 1048576f).ToString("00.0") + " / ");
		this._stats.Append((Profiler.GetTotalReservedMemory() / 1048576f).ToString("00.0") + " MB (");
		this._stats.Append((Profiler.GetTotalAllocatedMemory() * 100f / Profiler.GetTotalReservedMemory()).ToString("00.0") + "%)");
		this._stats.AppendLine();
		this._stats.Append("System Memory ");
		this._stats.Append(SystemInfo.systemMemorySize.ToString("0000") + " MB");
		this._stats.AppendLine();
		this._stats.Append("Graphics Memory ");
		this._stats.Append(SystemInfo.graphicsMemorySize.ToString("000") + " MB");
		this._stats.AppendLine();
	}

	private void _SetLogContentColor(LogType logType)
	{
		if (logType == LogType.Log)
		{
			GUI.contentColor = Color.white;
		}
		else if (logType == LogType.Warning)
		{
			GUI.contentColor = Color.yellow;
		}
		else if (logType == LogType.Error)
		{
			GUI.contentColor = Color.red;
		}
		else if (logType == LogType.Assert)
		{
			GUI.contentColor = Color.magenta;
		}
		else if (logType == LogType.Exception)
		{
			GUI.contentColor = Color.cyan;
		}
	}

	public void OnGUI()
	{
		if (this._state != 3)
		{
			Matrix4x4 matrix = GUI.matrix;
			Color color = GUI.color;
			Color black = Color.black;
			black.a = 0.75f;
			GUI.color = black;
			GUI.DrawTexture(new Rect(0f, 0f, (Single)Screen.width, (Single)Screen.height), this._screenTex, ScaleMode.StretchToFill, true);
			GUI.color = color;
			GUI.matrix = matrix;
		}
		Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
		GUILayout.BeginArea(fullscreenRect);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Label(this._title, new GUILayoutOption[0]);
		if (Event.current.type == EventType.Repaint)
		{
			this._titleRect = GUILayoutUtility.GetLastRect();
		}
		this._scrollPosition = GUILayout.BeginScrollView(this._scrollPosition, new GUILayoutOption[]
		{
			GUILayout.Width(fullscreenRect.width),
			GUILayout.Height(fullscreenRect.height - this._titleRect.height)
		});
		if (this._state == 1)
		{
			if (this._logCounter > 0 && this._logTypes.Count > 0)
			{
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				LinkedListNode<LogType> linkedListNode = this._logTypes.First;
				LinkedListNode<String> linkedListNode2 = this._logs.First;
				LinkedListNode<String> linkedListNode3 = this._stackTraces.First;
				this._logMsgs.Length = 0;
				Color contentColor = GUI.contentColor;
				LogType value = linkedListNode.Value;
				this._SetLogContentColor(value);
				do
				{
					if (linkedListNode.Value != value)
					{
						if (this._logMsgs.Length > 0)
						{
							GUILayout.Label(this._logMsgs.ToString(), new GUILayoutOption[0]);
							this._logMsgs.Length = 0;
						}
						value = linkedListNode.Value;
						this._SetLogContentColor(value);
					}
					else if (this._logMsgs.Length > 4096)
					{
						GUILayout.Label(this._logMsgs.ToString(), new GUILayoutOption[0]);
						this._logMsgs.Length = 0;
					}
					else if (this._logMsgs.Length > 0)
					{
						this._logMsgs.AppendLine();
						this._logMsgs.AppendLine();
					}
					this._logMsgs.Append(linkedListNode2.Value);
					this._logMsgs.AppendLine();
					this._logMsgs.Append(linkedListNode3.Value);
					linkedListNode = linkedListNode.Next;
					linkedListNode2 = linkedListNode2.Next;
					linkedListNode3 = linkedListNode3.Next;
				}
				while (linkedListNode != null);
				GUILayout.Label(this._logMsgs.ToString(), new GUILayoutOption[0]);
				GUI.contentColor = contentColor;
				GUILayout.EndVertical();
			}
		}
		else if (this._state == 2 || this._state == 3)
		{
			if (this._elapseTime >= 0.5f)
			{
				this._BuildProfilerStatsString();
				this._elapseTime = 0f;
			}
			GUILayout.Label(this._stats.ToString(), new GUILayoutOption[0]);
			if (this._state == 2)
			{
				GUILayout.Label(this._staticMsgs.ToString(), new GUILayoutOption[0]);
			}
			if (this._staticMsgs.Length > 4096)
			{
				this._staticMsgs.Length = 0;
			}
		}
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	public static void LogToFile(Object message)
	{
		using (StreamWriter streamWriter = File.AppendText("pLog.txt"))
		{
			streamWriter.WriteLine(message);
			global::Debug.Log(message);
		}
	}

	private const Int32 SIZE_KB = 1024;

	private const Int32 SIZE_MB = 1048576;

	public static OSDLogger Instance;

	public Single UpdateInterval = 0.5f;

	private Texture2D _screenTex;

	private String _asmVersion;

	private String _bundleVersion;

	private String _title;

	private Int32 _state;

	private Int32 _maxStateCount;

	private Int32 _maxLogEntry;

	private Int32 _logCounter;

	private LogType _lastLogType;

	private String _lastLogString;

	private LinkedList<LogType> _logTypes;

	private LinkedList<String> _logs;

	private LinkedList<String> _stackTraces;

	private StringBuilder _logMsgs;

	private StringBuilder _stats;

	private StringBuilder _staticMsgs;

	private Vector2 _scrollPosition;

	private Vector2 _touchPos;

	private Boolean _isTouching;

	private Single _momentumY;

	private Rect _titleRect;

	private Single _elapseTime;

	private Single _accum;

	private Int32 _frames;

	private Single _timeLeft;

	private String _fpsText;

	private Int64 _maxUsedMemSize;

	private enum LogState
	{
		Hide,
		ShowLogs,
		ShowProfilerStats,
		ShowProfilerStatsNoBG
	}
}

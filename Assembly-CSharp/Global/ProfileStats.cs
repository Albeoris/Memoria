using System;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = System.Object;

[ExecuteInEditMode]
public class ProfileStats : MonoBehaviour
{
    private void Start()
    {
        base.useGUILayout = false;
        this.m_timeRecord = new Single[5];
        this.m_nameRecord = new String[5];
        this.m_logRecord = new String[5];
        for (Int32 i = 0; i < 5; i++)
        {
            this.m_nameRecord[i] = String.Empty;
        }
    }

    private void OnGUI()
    {
        this.profileStats();
        if (this.m_IsShow)
        {
            if (Application.isPlaying)
            {
                this.drawStats();
            }
            else if (this.m_IsShowEditor)
            {
                this.drawStats();
            }
        }
    }

    private void profileStats()
    {
        this.m_UsedMemSize = GC.GetTotalMemory(false);
        if (this.m_UsedMemSize > this.m_MaxUsedMemSize)
        {
            this.m_MaxUsedMemSize = this.m_UsedMemSize;
        }
    }

    private void drawStats()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("FPS " + (1f / Time.deltaTime).ToString("00.000")).AppendLine();
        stringBuilder.Append("Mono Memory ").Append(((Single)this.m_UsedMemSize / 1048576f).ToString("00.0") + " / ").Append((Profiler.GetMonoHeapSize() / 1048576f).ToString("00.0") + " MB");
        if (Profiler.GetMonoHeapSize() > 0u)
        {
            stringBuilder.Append(" (" + ((Single)this.m_UsedMemSize * 100f / Profiler.GetMonoHeapSize()).ToString("00.0") + "%)");
        }
        stringBuilder.AppendLine();
        stringBuilder.Append("Assets Memory ").Append((Profiler.GetTotalAllocatedMemory() / 1048576f).ToString("00.0") + " / ").Append((Profiler.GetTotalReservedMemory() / 1048576f).ToString("00.0") + " MB (").Append((Profiler.GetTotalAllocatedMemory() * 100f / Profiler.GetTotalReservedMemory()).ToString("00.0") + "%)").AppendLine();
        stringBuilder.Append("System Memory ").Append(SystemInfo.systemMemorySize.ToString("0000") + " MB").AppendLine();
        stringBuilder.Append("Graphics Memory ").Append(SystemInfo.graphicsMemorySize.ToString("000") + " MB").AppendLine();
        for (Int32 i = 0; i < 5; i++)
        {
            if (this.m_nameRecord[i].CompareTo(String.Empty) != 0)
            {
                stringBuilder.Append(String.Concat(new String[]
                {
                    "Profile ",
                    this.m_nameRecord[i],
                    " ",
                    (this.m_timeRecord[i] * 1000f).ToString("00.00"),
                    " ms"
                })).AppendLine();
            }
        }
        for (Int32 j = 0; j < this.logMax; j++)
        {
            if (this.m_logRecord[j] != null)
            {
                stringBuilder.Append(this.m_logRecord[j]).AppendLine();
            }
        }
        Int32 num = (from c in stringBuilder.ToString().ToList<Char>()
                     where c.Equals('\n')
                     select c).Count<Char>();
        GUI.Box(new Rect(this.m_locX, this.m_locY, 250f, (Single)((Int32)(this.m_FontSizeBase * (Single)num) + 5)), String.Empty);
        GUI.Label(new Rect(this.m_locX + 5f, this.m_locY + 2f, (Single)Screen.width, (Single)Screen.height), stringBuilder.ToString());
    }

    public void BeginTime(String name)
    {
        this.UpdateTime(0, name);
    }

    public void EndTime(String name)
    {
        this.UpdateTime(1, name);
    }

    public void UpdateTime(Int32 mode, String name)
    {
        Int32 num = this.FindRacord(name);
        if (mode == 0)
        {
            this.m_timeRecord[num] = Time.realtimeSinceStartup;
            this.m_nameRecord[num] = name;
        }
        else
        {
            Single num2 = this.m_timeRecord[num];
            Single realtimeSinceStartup = Time.realtimeSinceStartup;
            this.m_timeRecord[num] = realtimeSinceStartup - num2;
        }
    }

    private Int32 FindRacord(String name)
    {
        for (Int32 i = 0; i < 5; i++)
        {
            if (this.m_nameRecord[i].CompareTo(name) == 0)
            {
                return i;
            }
        }
        for (Int32 j = 0; j < 5; j++)
        {
            if (this.m_nameRecord[j].CompareTo(String.Empty) == 0)
            {
                return j;
            }
        }
        return 4;
    }

    public void ShowProfile(Boolean show = true)
    {
        this.m_IsShow = show;
        this.m_IsShowEditor = show;
    }

    public void SetPosition(Single x, Single y)
    {
        this.m_locX = x;
        this.m_locY = y;
    }

    public void ShowLog(Boolean stack = false, Int32 max = 5)
    {
        this.m_IsShowLog = true;
        this.m_IsShowStack = stack;
        this.logMax = max;
        this.m_logRecord = new String[max];
    }

    public void HideLog()
    {
        this.m_IsShowLog = false;
    }

    private void OnEnable()
    {
        Application.logMessageReceived += this.HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= this.HandleLog;
    }

    private void HandleLog(String logString, String stackTrace, LogType type)
    {
        if (this.m_IsShowLog)
        {
            Int32 num = this.FindLog();
            this.m_logRecord[num] = String.Concat(new Object[]
            {
                "[",
                type,
                "]",
                logString
            });
            if (this.m_IsShowStack)
            {
                String[] logRecord = this.m_logRecord;
                Int32 num2 = num;
                logRecord[num2] = logRecord[num2] + ":" + stackTrace;
            }
        }
    }

    private Int32 FindLog()
    {
        for (Int32 i = 0; i < this.logMax; i++)
        {
            if (this.m_logRecord[i] == null)
            {
                return i;
            }
        }
        for (Int32 j = 1; j < this.logMax; j++)
        {
            this.m_logRecord[j - 1] = this.m_logRecord[j];
        }
        return this.logMax - 1;
    }

    private const Int32 SIZE_KB = 1024;

    private const Int32 SIZE_MB = 1048576;

    private const Int32 RECORD_MAX = 5;

    private const Int32 LOG_MAX = 5;

    private Int32 logMax = 5;

    private Boolean m_IsShowStack;

    public Boolean m_IsShow;

    public Boolean m_IsShowEditor;

    public Boolean m_IsShowLog;

    public Single m_FontSizeBase = 15.175f;

    private Int64 m_UsedMemSize;

    private Int64 m_MaxUsedMemSize;

    private Single[] m_timeRecord;

    private String[] m_nameRecord;

    private String[] m_logRecord;

    private Single m_locX = 5f;

    private Single m_locY = 25f;
}

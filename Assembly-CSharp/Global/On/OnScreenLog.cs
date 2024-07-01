using System;
using System.Collections.Generic;
using UnityEngine;

public class OnScreenLog : MonoBehaviour
{
    private void Start()
    {
    }

    private void Update()
    {
        this.frameCount++;
    }

    private void OnGUI()
    {
        GUIStyle style = GUI.skin.GetStyle("Label");
        style.fontSize = OnScreenLog.fontSize;
        style.alignment = TextAnchor.UpperLeft;
        style.wordWrap = false;
        Single num = 0f;
        String text = String.Empty;
        foreach (String str in OnScreenLog.log)
        {
            text = text + " " + str;
            text += "\n";
            num += style.lineHeight;
        }
        num += 6f;
        GUI.Label(new Rect(0f, 0f, (Single)(Screen.width - 1), num), text, style);
        num = style.lineHeight + 4f;
    }

    public static void Add(String msg)
    {
        String text = msg.Replace("\r", " ");
        text = text.Replace("\n", " ");
        Console.WriteLine("[APP] " + text);
        OnScreenLog.log.Add(text);
        OnScreenLog.msgCount++;
        if (OnScreenLog.msgCount > OnScreenLog.maxLines)
        {
            OnScreenLog.log.RemoveAt(0);
        }
    }

    private static Int32 msgCount = 0;

    private static List<String> log = new List<String>();

    private static Int32 maxLines = 16;

    private static Int32 fontSize = 24;

    private Int32 frameCount;
}

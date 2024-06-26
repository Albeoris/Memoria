using System;
using UnityEngine;

public static class WMOutput
{
    public static String OutputPath
    {
        get
        {
            return Application.dataPath + "\\..\\..\\WMOutput\\";
        }
    }

    public static String LogsPath
    {
        get
        {
            return WMOutput.OutputPath + "Logs\\";
        }
    }

    public static String TexturesPath
    {
        get
        {
            return WMOutput.OutputPath + "Textures\\";
        }
    }
}

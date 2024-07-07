using Global.Sound.SoLoud;
using Memoria;
using UnityEngine;

public class ISdLibAPIProxy
{
    private ISdLibAPIProxy()
    {
    }

    public static ISdLibAPI Instance
    {
        get
        {
            if (ISdLibAPIProxy.instance == null)
            {
                Initialize();
            }
            return ISdLibAPIProxy.instance;
        }
        set { ISdLibAPIProxy.instance = value; }
    }

    private static void Initialize()
    {
        if (Application.HasProLicense())
        {
            if (Configuration.Audio.Backend == 0)
            {
                ISdLibAPIProxy.instance = new SdLibAPIWithProLicense();
            }
            else
            {
                ISdLibAPIProxy.instance = new SdLibAPIWithSoloud();
            }
        }
        else
        {
            ISdLibAPIProxy.instance = new SdLibAPIWithSoloud();
        }
    }

    private static ISdLibAPI instance;
}

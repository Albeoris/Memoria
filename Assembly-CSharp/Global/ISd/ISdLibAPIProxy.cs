using Global.Sound.SaXAudio;
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
            switch (Configuration.Audio.Backend)
            {
                case 0:
                    ISdLibAPIProxy.instance = new SdLibAPIWithProLicense();
                    break;
                case 1:
                    ISdLibAPIProxy.instance = new SdLibAPIWithSaXAudio();
                    break;
                case 2:
                    ISdLibAPIProxy.instance = new SdLibAPIWithSoloud();
                    break;
            }
        }
        else
        {
            ISdLibAPIProxy.instance = new SdLibAPIWithSaXAudio();
        }
    }

    private static ISdLibAPI instance;
}

using Global.Sound.SoLoud;
using System;
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
            //ISdLibAPIProxy.instance = new SdLibAPIWithProLicense();
            ISdLibAPIProxy.instance = new SdLibAPIWithSoloud();
        }
        else
        {
            ISdLibAPIProxy.instance = new SdLibApiWithoutProLicense();
        }
    }

    private static ISdLibAPI instance;
}

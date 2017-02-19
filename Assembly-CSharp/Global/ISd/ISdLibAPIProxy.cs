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
				if (Application.HasProLicense())
				{
					ISdLibAPIProxy.instance = new SdLibAPIWithProLicense();
				}
				else
				{
					ISdLibAPIProxy.instance = new SdLibApiWithoutProLicense();
				}
			}
			return ISdLibAPIProxy.instance;
		}
	}

	private static ISdLibAPI instance;
}

using System;
using UnityEngine;

public class AppArgumentsCheck : MonoBehaviour
{
	private void Awake()
	{
		this.ForceQuitIfRequiredArguementIsMissing();
	}

	private void ForceQuitIfRequiredArguementIsMissing()
	{
		String[] commandLineArgs = Environment.GetCommandLineArgs();
		Boolean flag = false;
		Boolean flag2 = false;
		String[] array = commandLineArgs;
		for (Int32 i = 0; i < (Int32)array.Length; i++)
		{
			String a = array[i];
			if (a == "-runbylauncher")
			{
				flag = true;
			}
			if (a == "-screen-quality")
			{
				flag2 = true;
			}
		}
		if (!flag)
		{
			Application.Quit();
		}
		if (flag2)
		{
			Application.Quit();
		}
	}
}

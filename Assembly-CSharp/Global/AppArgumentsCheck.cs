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
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i] == "-runbylauncher")
			{
				flag = true;
			}
			if (commandLineArgs[i] == "-screen-quality")
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

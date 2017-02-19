using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Object = System.Object;

public class SoundLibWndProc : MonoBehaviour
{
	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
	public static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll")]
	private static extern IntPtr SetWindowLong(IntPtr hWnd, Int32 nIndex, IntPtr dwNewLong);

	[DllImport("user32.dll")]
	private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, Int32 nIndex, IntPtr dwNewLong);

	[DllImport("user32.dll")]
	private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll")]
	private static extern IntPtr GetActiveWindow();

	public Boolean Is64Bit()
	{
		String environmentVariable = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
		return !String.IsNullOrEmpty(environmentVariable) && !(environmentVariable.Substring(0, 3) == "x86");
	}

	private void Start()
	{
		IntPtr windowHandle = SoundLibWndProc.GetWindowHandle();
		this.hMainWindow = SoundLibWndProc.GetForegroundWindow();
		this.newWndProc = new WndProcDelegate(this.WndProc);
		this.newWndProcPtr = Marshal.GetFunctionPointerForDelegate(this.newWndProc);
		if (this.Is64Bit())
		{
			this.oldWndProcPtr = SoundLibWndProc.SetWindowLongPtr(this.hMainWindow, -4, this.newWndProcPtr);
		}
		else
		{
			this.oldWndProcPtr = SoundLibWndProc.SetWindowLong(this.hMainWindow, -4, this.newWndProcPtr);
		}
	}

	public static IntPtr GetWindowHandle()
	{
		return SoundLibWndProc.GetActiveWindow();
	}

	private static IntPtr StructToPtr(Object obj)
	{
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(obj));
		Marshal.StructureToPtr(obj, intPtr, false);
		return intPtr;
	}

	private void OnDisable()
	{
		if (this.Is64Bit())
		{
			SoundLibWndProc.SetWindowLongPtr(this.hMainWindow, -4, this.oldWndProcPtr);
		}
		else
		{
			SoundLibWndProc.SetWindowLong(this.hMainWindow, -4, this.oldWndProcPtr);
		}
		this.hMainWindow = IntPtr.Zero;
		this.oldWndProcPtr = IntPtr.Zero;
		this.newWndProcPtr = IntPtr.Zero;
		this.newWndProc = (WndProcDelegate)null;
	}

	private void Update()
	{
		if (this.isSEADSuspendedByTitleBar)
		{
			ISdLibAPIProxy.Instance.SdSoundSystem_Resume();
			this.isSEADSuspendedByTitleBar = false;
		}
	}

	private IntPtr WndProc(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam)
	{
		if (msg == 161u && (Int64)wParam.ToInt32() == 2L)
		{
			ISdLibAPIProxy.Instance.SdSoundSystem_Suspend();
			this.isSEADSuspendedByTitleBar = true;
		}
		if (msg == 260u && (Int64)wParam.ToInt32() == 13L)
		{
			msg = 0u;
		}
		return SoundLibWndProc.CallWindowProc(this.oldWndProcPtr, hWnd, msg, wParam, lParam);
	}

	private const Int32 GWLP_WNDPROC = -4;

	private const UInt32 WM_NCLBUTTONDOWN = 161u;

	private const UInt32 HTCAPTION = 2u;

	private const UInt32 WM_SYSKEYDOWN = 260u;

	private const UInt32 VK_RETURN = 13u;

	private IntPtr hMainWindow;

	private IntPtr oldWndProcPtr;

	private IntPtr newWndProcPtr;

	private WndProcDelegate newWndProc;

	private Boolean isSEADSuspendedByTitleBar;
}

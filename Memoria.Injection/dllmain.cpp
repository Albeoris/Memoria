#include "Windows.h"
#include "Unity.h"
#include "proxy.h"

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		// Load real system version.dll and resolve exports so we act as a proxy
		if (LoadRealVersion())
			ResolveAll();

		InstallUnityBootstrapHook();
		break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}
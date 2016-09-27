#include "Unity.h"
#include "LoadMonoDynamically.h"

void mono_set_commandline_arguments_hook(int argc, const char **argv, char *baseDir)
{
	const char *jitOptions = "--debugger-agent=transport=dt_socket,embedding=1,server=y,defer=y";
	char *jitArguments[1];
	jitArguments[0] = _strdup(jitOptions);
	mono_jit_parse_options(1, jitArguments);
	mono_debug_init(1);

	free(jitArguments[0]);

	mono_set_commandline_arguments(argc, argv, baseDir);
}

void UnityInit()
{
	SetupMono();

	HMODULE mainModule = GetModuleHandle(0);
	fp_mono_set_commandline_arguments *val = (fp_mono_set_commandline_arguments *)GetProcAddress(mainModule, "mono_set_commandline_arguments");
	*val = (fp_mono_set_commandline_arguments)mono_set_commandline_arguments_hook;
}
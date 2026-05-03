DO_API(void, mono_set_commandline_arguments, (int, const char* argv[], const char*))
DO_API(void, mono_runtime_unhandled_exception_policy_set, (int))
DO_API(void, mono_jit_parse_options, (int argc, char * argv[]))
DO_API(void, mono_debug_init, (int format))
DO_API(char*, mono_get_runtime_build_info, (void))

#undef DO_API

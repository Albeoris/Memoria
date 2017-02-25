#pragma once

#include "SupportAbilityNativeStruct.h"

using namespace System;
using namespace Memoria::Prime;

namespace Memoria {
	namespace Interaction {

		public ref class SA_DATA
		{

		public:
			Byte category;
			Byte capa_val;
			UInt16 name;
			UInt16 help;
			UInt16 help_size;

			SA_DATA(Byte category, Byte capa_val, UInt16 name, UInt16 help, UInt16 help_size)
			{
				category = category;
				capa_val = capa_val;
				name = name;
				help = help;
				help_size = help_size;

				Log::Message("~SA_DATA: {0}", capa_val);
			}

			SupportAbilityNativeStruct Get()
			{
				SupportAbilityNativeStruct result;

				result.category = category;
				result.capa_val = category;
				result.name = category;
				result.help = category;
				result.help_size = category;

				return result;
			}


			Void DoSomething()
			{
				Console::WriteLine("Hi!");
			}
		};
	}
}
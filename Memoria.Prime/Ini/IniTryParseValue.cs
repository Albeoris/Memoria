using System;

namespace Memoria.Prime.Ini
{
    public delegate Boolean IniTryParseValue<T>(String rawString, out T value);
}

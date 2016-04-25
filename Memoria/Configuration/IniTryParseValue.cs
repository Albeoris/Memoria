using System;

namespace Memoria
{
    public delegate Boolean IniTryParseValue<T>(String rawString, out T value);
}
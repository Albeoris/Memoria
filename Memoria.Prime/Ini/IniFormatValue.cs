using System;

namespace Memoria.Prime.Ini
{
    public delegate String IniFormatValue<in T>(T value);
}
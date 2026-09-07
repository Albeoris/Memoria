using System;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal interface ID3DShaderAssembler
    {
        D3DShaderAssemblyResult Assemble(String source, String sourceName);
    }
}
using System;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal sealed class D3DShaderAssemblyResult
    {
        public D3DShaderAssemblyResult(Boolean succeeded, String diagnostics)
        {
            Succeeded = succeeded;
            Diagnostics = diagnostics ?? String.Empty;
        }

        public Boolean Succeeded { get; }
        public String Diagnostics { get; }
    }
}
using System;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal sealed class D3D9ShaderProgram
    {
        public D3D9ShaderProgram(String profile, String source, Int32 sourceLine)
        {
            Profile = profile;
            Source = source;
            SourceLine = sourceLine;
        }

        public String Profile { get; }
        public String Source { get; }
        public Int32 SourceLine { get; }
    }
}
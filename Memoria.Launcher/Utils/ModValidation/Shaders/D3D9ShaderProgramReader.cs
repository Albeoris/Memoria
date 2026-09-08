using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal static class D3D9ShaderProgramReader
    {
        private static readonly Regex ProgramPattern = new("\"(?<source>(?<profile>(?:ps|vs)_[A-Za-z0-9_]+)[\\s\\S]*?)\"", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public static IReadOnlyList<D3D9ShaderProgram> Read(String shaderText)
        {
            if (shaderText == null)
                throw new ArgumentNullException(nameof(shaderText));

            List<D3D9ShaderProgram> result = new();
            foreach (Match match in ProgramPattern.Matches(shaderText))
            {
                Group source = match.Groups["source"];
                String profile = match.Groups["profile"].Value;
                Int32 sourceLine = 1;
                for (Int32 index = 0; index < source.Index; index++)
                    if (shaderText[index] == '\n')
                        sourceLine++;
                result.Add(new D3D9ShaderProgram(profile, source.Value, sourceLine));
            }
            return result;
        }
    }
}
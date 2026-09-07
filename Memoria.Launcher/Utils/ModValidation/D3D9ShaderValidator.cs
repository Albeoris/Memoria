#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Memoria.Launcher.Utils.IO;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal sealed class D3D9ShaderValidator : IModFileValidator
    {
        private static readonly Regex DiagnosticLocationPattern = new Regex(@"^(?<indent>[ \t]*)\((?<line>[0-9]+)(?<position>,[^)\r\n]+)?\)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);
        private readonly Lazy<ID3DShaderAssembler> _assembler;

        public D3D9ShaderValidator(ID3DShaderAssembler assembler)
        {
            if (assembler == null)
                throw new ArgumentNullException(nameof(assembler));
            _assembler = new Lazy<ID3DShaderAssembler>(() => assembler);
        }

        public D3D9ShaderValidator(Func<ID3DShaderAssembler> assemblerFactory)
        {
            _assembler = new Lazy<ID3DShaderAssembler>(assemblerFactory ?? throw new ArgumentNullException(nameof(assemblerFactory)), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public String Name => "Direct3D 9 shader assembler";

        public Boolean CanValidate(ModValidationFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            return PathSegmentMatcher.Contains(file.RelativePath, StringComparison.OrdinalIgnoreCase, "StreamingAssets", "Shaders");
        }

        public ModValidationResult Validate(ModValidationFile file, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            String shaderText = File.ReadAllText(file.FullPath);
            IReadOnlyList<D3D9ShaderProgram> programs = D3D9ShaderProgramReader.Read(shaderText);
            if (programs.Count == 0)
                return new ModValidationResult(file, ModValidationStatus.Skipped, Name, new[] { new ModValidationDiagnostic(ModValidationDiagnosticSeverity.Information, "This file contains no embedded Direct3D 9 assembly programs to validate, so it was skipped.") });

            Boolean succeeded = true;
            List<ModValidationDiagnostic> diagnostics = new();
            for (Int32 index = 0; index < programs.Count; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                D3D9ShaderProgram program = programs[index];
                String sourceName = $"{file.FullPath} ({program.Profile}, program {index + 1})";
                D3DShaderAssemblyResult assembly = _assembler.Value.Assemble(program.Source, sourceName);
                succeeded &= assembly.Succeeded;
                if (!String.IsNullOrWhiteSpace(assembly.Diagnostics))
                {
                    ModValidationDiagnosticSeverity severity = assembly.Succeeded ? ModValidationDiagnosticSeverity.Warning : ModValidationDiagnosticSeverity.Error;
                    String compilerDiagnostics = MapDiagnosticLinesToFile(assembly.Diagnostics.Replace(sourceName, String.Empty).Trim(), program.SourceLine);
                    diagnostics.Add(new ModValidationDiagnostic(severity, $"{program.Profile}, embedded program {index + 1}, source starts at file line {program.SourceLine}:{Environment.NewLine}{compilerDiagnostics}"));
                }
            }

            if (succeeded && diagnostics.Count == 0)
                diagnostics.Add(new ModValidationDiagnostic(ModValidationDiagnosticSeverity.Information, $"Successfully assembled {programs.Count} Direct3D 9 program{(programs.Count == 1 ? String.Empty : "s")} using the Windows D3DCompiler component."));
            return new ModValidationResult(file, succeeded ? ModValidationStatus.Valid : ModValidationStatus.Invalid, Name, diagnostics);
        }

        private static String MapDiagnosticLinesToFile(String diagnostics, Int32 sourceLine)
        {
            return DiagnosticLocationPattern.Replace(diagnostics, match =>
            {
                if (!Int32.TryParse(match.Groups["line"].Value, out Int32 programLine))
                    return match.Value;
                Int64 fileLine = (Int64)sourceLine + programLine - 1;
                if (fileLine < 1 || fileLine > Int32.MaxValue)
                    return match.Value;
                return $"{match.Groups["indent"].Value}file({fileLine}{match.Groups["position"].Value})";
            });
        }
    }
}

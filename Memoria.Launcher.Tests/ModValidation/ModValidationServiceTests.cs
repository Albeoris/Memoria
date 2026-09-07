using System.Collections.Concurrent;
using Memoria.Launcher.Tests.Infrastructure;
using Memoria.Launcher.Utils.ModValidation;
using Xunit;

namespace Memoria.Launcher.Tests.ModValidation;

public sealed class ModValidationServiceTests
{
    [Fact]
    public async Task ValidateDirectoryAsync_processes_every_file_and_preserves_shader_diagnostics()
    {
        using TemporaryDirectory directory = new();
        String shaderDirectory = Path.Combine(directory.FullPath, "StreamingAssets", "Shaders", "PSX");
        String similarlyNamedDirectory = Path.Combine(directory.FullPath, "NotStreamingAssets", "Shaders");
        Directory.CreateDirectory(shaderDirectory);
        Directory.CreateDirectory(similarlyNamedDirectory);
        File.WriteAllText(Path.Combine(directory.FullPath, "ModDescription.xml"), "<Mod />");
        File.WriteAllText(Path.Combine(shaderDirectory, "valid.txt"), CreateShader("ps_2_0\nmov oC0, c0"));
        File.WriteAllText(Path.Combine(shaderDirectory, "invalid.txt"), CreateShader("ps_2_0\ninvalid_instruction r0"));
        File.WriteAllText(Path.Combine(similarlyNamedDirectory, "not-a-shader.txt"), CreateShader("ps_2_0\nmov oC0, c0"));
        RecordingAssembler assembler = new();
        ModValidationService service = new(new IModFileValidator[] { new D3D9ShaderValidator(assembler) }, 2);
        RecordingProgress progress = new();

        ModValidationReport report = await service.ValidateDirectoryAsync(directory.FullPath, "Test mod", progress, CancellationToken.None);

        IReadOnlyList<ModValidationResult> files = report.FlattenFiles();
        Assert.Equal(4, files.Count);
        Assert.Equal(ModValidationStatus.Skipped, files.Single(file => file.File.RelativePath == "ModDescription.xml").Status);
        Assert.Equal(ModValidationStatus.Skipped, files.Single(file => Path.GetFileName(file.File.RelativePath) == "not-a-shader.txt").Status);
        Assert.Equal(ModValidationStatus.Valid, files.Single(file => Path.GetFileName(file.File.RelativePath) == "valid.txt").Status);
        ModValidationResult invalid = files.Single(file => Path.GetFileName(file.File.RelativePath) == "invalid.txt");
        Assert.Equal(ModValidationStatus.Invalid, invalid.Status);
        Assert.Contains("X9999: synthetic assembler error", invalid.Diagnostics.Single().Message);
        Assert.Contains("file(6,1)", invalid.Diagnostics.Single().Message);
        Assert.DoesNotContain(directory.FullPath, invalid.Diagnostics.Single().Message);
        Assert.Equal(2, assembler.Sources.Count);
        Assert.Equal(4, progress.Last.DiscoveredFiles);
        Assert.Equal(4, progress.Last.CompletedFiles);
        Assert.True(progress.Last.EnumerationCompleted);
    }

    [Fact]
    public void ShaderProgramReader_ignores_ShaderLab_strings_and_reports_original_source_lines()
    {
        String shader = "Shader \"Name\" {\nProperties { _Name (\"Label\", Float) = 0 }\nSubProgram \"d3d9 \" {\n\"vs_2_0\nmov oPos, v0\n\"\n\"ps_2_0\nmov oC0, c0\n\"\n} }";

        IReadOnlyList<D3D9ShaderProgram> programs = D3D9ShaderProgramReader.Read(shader);

        Assert.Equal(2, programs.Count);
        Assert.Equal("vs_2_0", programs[0].Profile);
        Assert.Equal(4, programs[0].SourceLine);
        Assert.Equal("ps_2_0", programs[1].Profile);
        Assert.Equal(7, programs[1].SourceLine);
    }

    private static String CreateShader(String assembly) => $"Shader \"Test\" {{\nSubShader {{\nPass {{\nSubProgram \"d3d9 \" {{\n\"{assembly}\"\n}}\n}}\n}}\n}}";

    private sealed class RecordingAssembler : ID3DShaderAssembler
    {
        public ConcurrentBag<String> Sources { get; } = new();

        public D3DShaderAssemblyResult Assemble(String source, String sourceName)
        {
            Sources.Add(source);
            return source.Contains("invalid_instruction", StringComparison.Ordinal) ? new D3DShaderAssemblyResult(false, $"{sourceName}(2,1): error X9999: synthetic assembler error") : new D3DShaderAssemblyResult(true, String.Empty);
        }
    }

    private sealed class RecordingProgress : IProgress<ModValidationProgress>
    {
        private readonly Object _sync = new();
        private ModValidationProgress _last = new(String.Empty, 0, 0, false, null!);

        public ModValidationProgress Last
        {
            get
            {
                lock (_sync)
                    return _last;
            }
        }

        public void Report(ModValidationProgress value)
        {
            lock (_sync)
                _last = value;
        }
    }
}

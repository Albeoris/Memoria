using System.Collections.Concurrent;
using System.Globalization;
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

    [Fact]
    public void X5213AddFixer_changes_only_the_exact_file_line_reported_for_ps_2_0()
    {
        using TemporaryDirectory directory = new();
        String shaderDirectory = Path.Combine(directory.FullPath, "StreamingAssets", "Shaders");
        Directory.CreateDirectory(shaderDirectory);
        String path = Path.Combine(shaderDirectory, "shader.txt");
        String source = "first\nadd r0, c0, c1\nthird\n\tadd r2.xyz, -c9.yyy, c3.xxx // reported\nfifth\nadd r1, c4, c5\n";
        File.WriteAllText(path, source);
        ModValidationFile file = new(directory.FullPath, path);
        ModValidationResult validation = CreateX5213Result(file, 4);
        D3D9X5213AddFixer fixer = new();

        Assert.True(fixer.CanFix(validation));
        ModFileFixResult result = fixer.Fix(validation, CancellationToken.None);

        Assert.True(result.Changed);
        ModFixChange change = Assert.Single(result.Changes);
        Assert.Equal(4, change.FileLine);
        Assert.Equal("\tadd r2.xyz, -c9.yyy, c3.xxx // reported", change.Before);
        Assert.Equal("\tmov r2.xyz, -c9.yyy\n\tadd r2.xyz, r2.xyz, c3.xxx // reported", change.After);
        Assert.Equal("first\nadd r0, c0, c1\nthird\n\tmov r2.xyz, -c9.yyy\n\tadd r2.xyz, r2.xyz, c3.xxx // reported\nfifth\nadd r1, c4, c5\n", File.ReadAllText(path));
    }

    [Theory]
    [InlineData("vs_2_0", "X5213", "add r0, c0, c1")]
    [InlineData("ps_2_0", "X5204", "add r0, c0, c1")]
    [InlineData("ps_2_0", "X5213", "mul r0, c0, c1")]
    [InlineData("ps_2_0", "X5213", "add r0, c01, c1")]
    [InlineData("ps_2_0", "X5213", "add_pp r0, c0, c1")]
    [InlineData("ps_2_0", "X5213", "add oC0, c0, c1")]
    public void X5213AddFixer_rejects_other_profiles_errors_and_instruction_shapes(String profile, String errorCode, String instruction)
    {
        using TemporaryDirectory directory = new();
        String path = Path.Combine(directory.FullPath, "shader.txt");
        File.WriteAllText(path, instruction);
        ModValidationFile file = new(directory.FullPath, path);
        ModValidationResult validation = new(file, ModValidationStatus.Invalid, "validator", new[] { new ModValidationDiagnostic(ModValidationDiagnosticSeverity.Error, $"{profile}, embedded program 1, source starts at file line 1:{Environment.NewLine}file(1,1-3): error {errorCode}: diagnostic") });

        Assert.False(new D3D9X5213AddFixer().CanFix(validation));
    }

    [Fact]
    public void X5213MultiplyFixer_rewrites_only_the_reported_mul_instruction()
    {
        using TemporaryDirectory directory = new();
        String path = Path.Combine(directory.FullPath, "shader.txt");
        File.WriteAllText(path, "mul r0.x, c0.x, c1.x\n\tmul r2.x, c3.x, c6.x\n");
        ModValidationFile file = new(directory.FullPath, path);
        ModValidationResult validation = CreateX5213Result(file, 2);
        D3D9X5213MultiplyFixer fixer = new();

        Assert.True(fixer.CanFix(validation));
        ModFileFixResult result = fixer.Fix(validation, CancellationToken.None);

        ModFixChange change = Assert.Single(result.Changes);
        Assert.Equal(2, change.FileLine);
        Assert.Equal("\tmul r2.x, c3.x, c6.x", change.Before);
        Assert.Equal("\tmov r2.x, c3.x\n\tmul r2.x, r2.x, c6.x", change.After);
        Assert.Equal("mul r0.x, c0.x, c1.x\n\tmov r2.x, c3.x\n\tmul r2.x, r2.x, c6.x\n", File.ReadAllText(path));
        Assert.False(new D3D9X5213AddFixer().CanFix(validation));
    }

    [Fact]
    public async Task FixCoordinator_revalidates_between_different_fixers_and_preserves_the_complete_changelog()
    {
        using TemporaryDirectory directory = new();
        String shaderDirectory = Path.Combine(directory.FullPath, "StreamingAssets", "Shaders");
        Directory.CreateDirectory(shaderDirectory);
        String path = Path.Combine(shaderDirectory, "mixed.txt");
        File.WriteAllText(path, CreateShader("ps_2_0\nadd r0, c0, c1\nmul r1, c2, c3\nmov oC0, r0"));
        ModValidationFile file = new(directory.FullPath, path);
        X5213Assembler assembler = new();
        ModValidationService validationService = new(new IModFileValidator[] { new D3D9ShaderValidator(assembler) });
        ModFixService fixService = new(new IModFileFixer[] { new D3D9X5213AddFixer(), new D3D9X5213MultiplyFixer() });
        ModFixChangelogStore changelogStore = new(directory.FullPath);
        ModFileFixCoordinator coordinator = new(validationService, fixService, changelogStore);
        ModValidationResult initialResult = await validationService.ValidateFileAsync(file, CancellationToken.None);

        ModFileFixOutcome outcome = await coordinator.FixAsync(initialResult, CancellationToken.None);

        Assert.True(outcome.Changed);
        Assert.Equal(2, outcome.AppliedFixerCount);
        Assert.Equal(ModValidationStatus.Fixed, outcome.ValidationResult.Status);
        Assert.Equal(3, assembler.CallCount);
        Assert.Contains("mov r0, c0\nadd r0, r0, c1\nmov r1, c2\nmul r1, r1, c3", File.ReadAllText(path));
        Assert.Contains("add fixer", outcome.ValidationResult.FixChangelog.FixerName);
        Assert.Contains("multiply fixer", outcome.ValidationResult.FixChangelog.FixerName);
        Assert.Equal(2, outcome.ValidationResult.FixChangelog.Changes.Count);
        Assert.Equal(outcome.ValidationResult.FixChangelog.Timestamp, changelogStore.FindCurrent(file).Timestamp);
    }

    [Fact]
    public void FixChangelog_recognizes_unchanged_fixed_file_and_invalidates_external_edits()
    {
        using TemporaryDirectory directory = new();
        String path = Path.Combine(directory.FullPath, "shader.txt");
        File.WriteAllText(path, "fixed shader");
        ModValidationFile file = new(directory.FullPath, path);
        ModFileFixResult result = new(file, "test fixer", new[] { new ModFixChange(7, "before", "after") });
        ModFixChangelogStore store = new(directory.FullPath);

        ModFixChangelogEntry appended = store.Append(result);
        ModFixChangelogEntry current = store.FindCurrent(file);

        Assert.NotNull(current);
        Assert.Equal(appended.Timestamp, current.Timestamp);
        Assert.True(DateTime.TryParseExact(current.Timestamp, "yyyy.MM.dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out _));
        Assert.Contains("shader.txt", File.ReadAllText(Path.Combine(directory.FullPath, ModFixChangelogStore.FileName)));

        File.AppendAllText(path, " externally changed");

        Assert.Null(store.FindCurrent(file));
    }

    private static ModValidationResult CreateX5213Result(ModValidationFile file, Int32 fileLine)
    {
        String diagnostic = $"ps_2_0, embedded program 10, source starts at file line 1:{Environment.NewLine}file({fileLine},7-9): error X5213: 2 different constant registers (c#) read by instruction.";
        return new ModValidationResult(file, ModValidationStatus.Invalid, "validator", new[] { new ModValidationDiagnostic(ModValidationDiagnosticSeverity.Error, diagnostic) });
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

    private sealed class X5213Assembler : ID3DShaderAssembler
    {
        private static readonly System.Text.RegularExpressions.Regex InvalidInstructionPattern = new(@"^[ \t]*(?:add|mul)[ \t]+r[0-9]+(?:\.[xyzw]{1,4})?[ \t]*,[ \t]*[+-]?c[0-9]+(?:\.[xyzw]{1,4})?[ \t]*,[ \t]*[+-]?c[0-9]+(?:\.[xyzw]{1,4})?", System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        public Int32 CallCount { get; private set; }

        public D3DShaderAssemblyResult Assemble(String source, String sourceName)
        {
            CallCount++;
            String[] lines = source.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            List<String> diagnostics = new();
            for (Int32 index = 0; index < lines.Length; index++)
                if (InvalidInstructionPattern.IsMatch(lines[index]))
                    diagnostics.Add($"{sourceName}({index + 1},1-3): error X5213: 2 different constant registers (c#) read by instruction.");
            return new D3DShaderAssemblyResult(diagnostics.Count == 0, String.Join(Environment.NewLine, diagnostics));
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

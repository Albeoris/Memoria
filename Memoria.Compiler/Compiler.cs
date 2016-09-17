using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Memoria.Compiler
{
    internal sealed class Compiler
    {
        private readonly String _referencesDirectoryPath;
        private readonly String _sourcesDirectoryPath;
        private readonly String _outputDirectoryPath;
        private readonly String _outputFileName;

        private readonly LinkedList<SyntaxTree> _sources = new LinkedList<SyntaxTree>();

        public Compiler(String referencesDirectoryPath, String sourcesDirectoryPath, String outputDirectoryPath, String outputFileName)
        {
            _referencesDirectoryPath = referencesDirectoryPath;
            _sourcesDirectoryPath = sourcesDirectoryPath;
            _outputDirectoryPath = outputDirectoryPath;
            _outputFileName = outputFileName;
        }

        public void Compile()
        {
            MetadataReference[] references =
            {
                MetadataReference.CreateFromFile(Path.Combine(_referencesDirectoryPath, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(_referencesDirectoryPath, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(_referencesDirectoryPath, "System.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(_referencesDirectoryPath, "System.Data.DataSetExtensions.dll")),
                MetadataReference.CreateFromFile(Path.Combine(_referencesDirectoryPath, "Memoria.dll")),
                MetadataReference.CreateFromFile(Path.Combine(_referencesDirectoryPath, "UnityEngine.dll")),
                MetadataReference.CreateFromFile(Path.Combine(_referencesDirectoryPath, "XInputDotNetPure.dll"))
            };

            ParseSources();

            String outputPath = Path.Combine(_outputDirectoryPath, _outputFileName);
            String pdbPath = Path.ChangeExtension(outputPath, ".pdb");

            String tempPath = outputPath + ".tmp";
            String tempPdbPath = pdbPath + ".tmp";

            CSharpCompilation compilation = Prepare(references);
            Emit(compilation, tempPath, tempPdbPath);

            // Replace dll
            String backupPath = outputPath + ".bak";
            if (File.Exists(outputPath))
            {
                File.Delete(backupPath);
                File.Move(outputPath, backupPath);
                File.Delete(outputPath);
            }

            File.Move(tempPath, outputPath);

            // Replace pdb
            File.Delete(pdbPath);
            File.Move(tempPdbPath, pdbPath);
        }

        private void ParseSources()
        {
            foreach (String filePath in Directory.EnumerateFiles(_sourcesDirectoryPath, "*.cs", SearchOption.AllDirectories))
                ParseSource(filePath);
        }

        private void ParseSource(String filePath)
        {
            using (StreamReader source = File.OpenText(filePath))
            {
                String text = source.ReadToEnd();
                Encoding encoding = source.CurrentEncoding;
                _sources.AddLast(CSharpSyntaxTree.ParseText(text, path: filePath, encoding: encoding));
            }
        }

        private CSharpCompilation Prepare(MetadataReference[] references)
        {
            CSharpCompilationOptions options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                allowUnsafe: true);

            CSharpCompilation compilation = CSharpCompilation.Create("Memoria.Scripts")
                .WithOptions(options)
                .AddReferences(references)
                .AddSyntaxTrees(_sources);

            _sources.Clear();
            return compilation;
        }

        private static void Emit(CSharpCompilation compilation, String outputPath, String pdbPath)
        {
            EmitResult result = compilation.Emit(outputPath, pdbPath);
            if (result.Success)
                return;

            foreach (Diagnostic diag in result.Diagnostics)
            {
                if (diag.Severity == DiagnosticSeverity.Error)
                    Console.WriteLine(diag);
            }

            throw new Exception("Failed to compile sources. Fix errors and try again.");
        }
    }
}
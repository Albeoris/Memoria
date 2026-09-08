#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Memoria.Launcher.Utils.ModValidation
{
    internal abstract class D3D9X5213BinaryOperationFixer : IModFileFixer
    {
        private static readonly Regex PixelShader20HeaderPattern = new(@"^ps_2_0, embedded program [0-9]+,", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private static readonly Regex X5213LocationPattern = new(@"^file\((?<line>[0-9]+),[^)]*\): error X5213:", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private readonly String _instruction;
        private readonly Regex _instructionPattern;

        protected D3D9X5213BinaryOperationFixer(String instruction, String name)
        {
            if (String.IsNullOrWhiteSpace(instruction))
                throw new ArgumentException("An instruction is required.", nameof(instruction));
            _instruction = instruction;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _instructionPattern = new Regex(@"^(?<indent>[ \t]*)" + Regex.Escape(instruction) + @"[ \t]+(?<destination>r[0-9]+(?:\.[xyzw]{1,4})?)[ \t]*,[ \t]*(?<source1>[+-]?c(?<constant1>[0-9]+)(?:\.[xyzw]{1,4})?)[ \t]*,[ \t]*(?<source2>[+-]?c(?<constant2>[0-9]+)(?:\.[xyzw]{1,4})?)(?<suffix>[ \t]*(?://.*)?)$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public String Name { get; }

        public Boolean CanFix(ModValidationResult validationResult)
        {
            if (validationResult == null || !validationResult.HasProblem || !File.Exists(validationResult.File.FullPath))
                return false;
            String text = File.ReadAllText(validationResult.File.FullPath);
            IReadOnlyList<TextLine> lines = ReadLines(text);
            return GetTargetLines(validationResult).Any(fileLine => TryCreateReplacement(lines, fileLine, out _, out _));
        }

        public ModFileFixResult Fix(ModValidationResult validationResult, CancellationToken cancellationToken)
        {
            if (validationResult == null)
                throw new ArgumentNullException(nameof(validationResult));

            TextFile file = TextFile.Read(validationResult.File.FullPath);
            IReadOnlyList<TextLine> lines = ReadLines(file.Text);
            List<PendingReplacement> replacements = new();
            foreach (Int32 fileLine in GetTargetLines(validationResult).Distinct().OrderByDescending(line => line))
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (TryCreateReplacement(lines, fileLine, out TextLine line, out String replacement))
                    replacements.Add(new PendingReplacement(fileLine, line, replacement));
            }

            if (replacements.Count == 0)
                return new ModFileFixResult(validationResult.File, Name, Array.Empty<ModFixChange>());

            StringBuilder updated = new(file.Text);
            List<ModFixChange> changes = new();
            foreach (PendingReplacement replacement in replacements)
            {
                cancellationToken.ThrowIfCancellationRequested();
                updated.Remove(replacement.Line.Start, replacement.Line.Length);
                updated.Insert(replacement.Line.Start, replacement.Replacement);
                changes.Add(new ModFixChange(replacement.FileLine, replacement.Line.Content, replacement.Replacement));
            }
            file.Write(updated.ToString());
            changes.Reverse();
            return new ModFileFixResult(validationResult.File, Name, changes);
        }

        private static IEnumerable<Int32> GetTargetLines(ModValidationResult validationResult)
        {
            foreach (ModValidationDiagnostic diagnostic in validationResult.Diagnostics)
            {
                if (!PixelShader20HeaderPattern.IsMatch(diagnostic.Message))
                    continue;
                foreach (Match match in X5213LocationPattern.Matches(diagnostic.Message))
                    if (Int32.TryParse(match.Groups["line"].Value, out Int32 fileLine))
                        yield return fileLine;
            }
        }

        private Boolean TryCreateReplacement(IReadOnlyList<TextLine> lines, Int32 fileLine, out TextLine line, out String replacement)
        {
            line = fileLine > 0 && fileLine <= lines.Count ? lines[fileLine - 1] : null;
            replacement = null;
            if (line == null)
                return false;

            Match match = _instructionPattern.Match(line.Content);
            if (!match.Success || !Int32.TryParse(match.Groups["constant1"].Value, out Int32 constant1) || !Int32.TryParse(match.Groups["constant2"].Value, out Int32 constant2) || constant1 == constant2)
                return false;

            String indent = match.Groups["indent"].Value;
            String destination = match.Groups["destination"].Value;
            String source1 = match.Groups["source1"].Value;
            String source2 = match.Groups["source2"].Value;
            String suffix = match.Groups["suffix"].Value;
            String newLine = String.IsNullOrEmpty(line.NewLine) ? Environment.NewLine : line.NewLine;
            replacement = $"{indent}mov {destination}, {source1}{newLine}{indent}{_instruction} {destination}, {destination}, {source2}{suffix}";
            return true;
        }

        private static IReadOnlyList<TextLine> ReadLines(String text)
        {
            List<TextLine> result = new();
            Int32 start = 0;
            while (start < text.Length)
            {
                Int32 end = start;
                while (end < text.Length && text[end] != '\r' && text[end] != '\n')
                    end++;
                Int32 newLineLength = end < text.Length && text[end] == '\r' && end + 1 < text.Length && text[end + 1] == '\n' ? 2 : end < text.Length ? 1 : 0;
                result.Add(new TextLine(start, end - start, text.Substring(start, end - start), newLineLength == 0 ? String.Empty : text.Substring(end, newLineLength)));
                start = end + newLineLength;
            }
            return result;
        }

        private sealed class PendingReplacement
        {
            public PendingReplacement(Int32 fileLine, TextLine line, String replacement)
            {
                FileLine = fileLine;
                Line = line;
                Replacement = replacement;
            }

            public Int32 FileLine { get; }
            public TextLine Line { get; }
            public String Replacement { get; }
        }

        private sealed class TextLine
        {
            public TextLine(Int32 start, Int32 length, String content, String newLine)
            {
                Start = start;
                Length = length;
                Content = content;
                NewLine = newLine;
            }

            public Int32 Start { get; }
            public Int32 Length { get; }
            public String Content { get; }
            public String NewLine { get; }
        }

        private sealed class TextFile
        {
            private readonly String _path;
            private readonly Encoding _encoding;
            private readonly Byte[] _preamble;

            private TextFile(String path, String text, Encoding encoding, Byte[] preamble)
            {
                _path = path;
                Text = text;
                _encoding = encoding;
                _preamble = preamble;
            }

            public String Text { get; }

            public static TextFile Read(String path)
            {
                Byte[] bytes = File.ReadAllBytes(path);
                Encoding encoding = DetectEncoding(bytes, out Int32 preambleLength);
                Byte[] preamble = new Byte[preambleLength];
                Buffer.BlockCopy(bytes, 0, preamble, 0, preambleLength);
                return new TextFile(path, encoding.GetString(bytes, preambleLength, bytes.Length - preambleLength), encoding, preamble);
            }

            public void Write(String text)
            {
                Byte[] content = _encoding.GetBytes(text);
                Byte[] bytes = new Byte[_preamble.Length + content.Length];
                Buffer.BlockCopy(_preamble, 0, bytes, 0, _preamble.Length);
                Buffer.BlockCopy(content, 0, bytes, _preamble.Length, content.Length);
                File.WriteAllBytes(_path, bytes);
            }

            private static Encoding DetectEncoding(Byte[] bytes, out Int32 preambleLength)
            {
                if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                {
                    preambleLength = 3;
                    return new UTF8Encoding(false, true);
                }
                if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
                {
                    preambleLength = 2;
                    return new UnicodeEncoding(false, false, true);
                }
                if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
                {
                    preambleLength = 2;
                    return new UnicodeEncoding(true, false, true);
                }

                preambleLength = 0;
                UTF8Encoding utf8 = new(false, true);
                try
                {
                    utf8.GetString(bytes);
                    return utf8;
                }
                catch (DecoderFallbackException)
                {
                    return Encoding.Default;
                }
            }
        }
    }
}

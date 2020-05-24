using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FF8.JSM;
using FF8.JSM.Format;
using FF8.JSM.Instructions;
using Memoria.EventEngine.Execution;
using Memoria.Prime;

namespace Memoria.EventEngine.EV
{
    public sealed class EVFileReader
    {
        private readonly Stream _input;

        public EVFileReader(Stream input)
        {
            _input = input;
        }

        public static EVObject[] Read(String evPath)
        {
            using var input = File.OpenRead(evPath);
            var reader = new EVFileReader(input);
            return reader.Read();
        }

        public unsafe EVObject[] Read()
        {
            Int64 readed = 0;

            EVFileHeader fileHeader = _input.ReadStruct<EVFileHeader>();
            fileHeader.Check();
            readed += sizeof(EVFileHeader);

            EVFileObject[] objects = _input.ReadStructs<EVFileObject>(fileHeader.ObjectCount);
            readed += sizeof(EVFileObject) * fileHeader.ObjectCount;

            EVObject[] objectCode = new EVObject[objects.Length];
            for (Int32 i = 0; i < objects.Length; i++)
            {
                EVFileObject obj = objects[i];
                EVScript[] scripts = Array<EVScript>.Empty;

                if (obj.Size > sizeof(EVFileScriptsHeader))
                {
                    Int32 offset = obj.Offset + sizeof(EVFileHeader);

                    _input.SetPosition(offset);

                    EVFileScriptsHeader header = _input.ReadStruct<EVFileScriptsHeader>();
                    offset = sizeof(EVFileScriptsHeader); // Exclude header from binary code

                    EVFileScriptInfo[] scriptInfos = _input.ReadStructs<EVFileScriptInfo>(header.ScriptCount);
                    offset += sizeof(EVFileScriptInfo) * header.ScriptCount; // Exclude script infos from binary code

                    Byte[] code = new Byte[obj.Size - offset];
                    _input.EnsureRead(code, 0, code.Length);
                    readed += code.Length;
                    readed += offset;
                    offset -= sizeof(EVFileScriptsHeader); // Script offset starts from script but not included header

                    Int32 codeUsage = 0;
                    scripts = new EVScript[header.ScriptCount];
                    for (Int32 s = 0; s < header.ScriptCount; s++)
                    {
                        EVFileScriptInfo info = scriptInfos[s];

                        Int32 position = info.Offset - offset;
                        Int32 size = code.Length;
                        if (s < header.ScriptCount - 1)
                            size = scriptInfos[s + 1].Offset - offset;
                        size -= position;
                        codeUsage += size;

                        EVScriptMaker scriptMaker = new EVScriptMaker(code.Segment(position, size));
                        Jsm.ExecutableSegment scriptSegment = MakeScript(scriptMaker);
                        scripts[s] = new EVScript(info.Id, scriptSegment);

                        var sw = new ScriptWriter();
                        scripts[s].FormatMethod(sw, new DummyFormatterContext(), StatelessServices.Instance);
                        var result = sw.Release();
                    }

                    if (codeUsage != code.Length)
                        throw new NotSupportedException("if (codeUsage != code.Length)");

                }

                objectCode[i] = new EVObject(i, obj.VariableCount, obj.Flags, scripts);
            }

            if (readed != _input.Length)
                throw new NotSupportedException("if (readed != _input.Length)");

            return objectCode;
        }

        private static Jsm.ExecutableSegment MakeScript(EVScriptMaker maker)
        {
            List<JsmInstruction> instructions = new List<JsmInstruction>();
            Jsm.LabeledStack stack = new Jsm.LabeledStack();
            Jsm.LabelBuilder labelBuilder = new Jsm.LabelBuilder(maker.Size);

            var currentLabel = maker.Offset;

            while (maker.TryReadOpcode(out Jsm.Opcode opcode))
            {
                if (opcode == Jsm.Opcode.NOP)
                    throw new InvalidProgramException("if (opcode == Jsm.Opcode.NOP)");

                if (opcode == Jsm.Opcode.EXPR)
                {
                    currentLabel = maker.Offset - 1;
                        
                    stack.Clear();
                    Jsm.Expression.Evaluate(maker, stack);

                    if (stack.TryPeek() is JsmInstruction setter)
                    {
                        labelBuilder.TraceInstruction(maker.Offset, currentLabel, new Jsm.IndexedInstruction(instructions.Count, setter));
                        instructions.Add(setter);
                        currentLabel = maker.Offset;
                    }

                    continue;
                }

                
                JsmInstruction instruction = JsmInstruction.TryMake(opcode, maker, stack);
                if (instruction != null)
                {
                    labelBuilder.TraceInstruction(maker.Offset, currentLabel, new Jsm.IndexedInstruction(instructions.Count, instruction));
                    instructions.Add(instruction);
                    currentLabel = maker.Offset;
                    continue;
                }

                throw new NotSupportedException(opcode.ToString());
            }

            stack.Clear();

            // if (!(instructions.First() is LBL))
            //     throw new InvalidProgramException("Script must start with a label.");
            //
            
            if (!(instructions.Last() is RETURN))
                throw new InvalidProgramException("Script must end with a return.");

            // Switch from opcodes to instructions
            HashSet<Int32> labelIndices = labelBuilder.Commit();

            // Merge similar instructions
            // instructions = InstructionMerger.Merge(instructions, labelIndices);

            // Combine instructions to logical blocks
            Jsm.IJsmControl[] controls = Jsm.Control.Builder.Build(instructions);

            // Arrange instructions by segments and return root
            return Jsm.Segment.Builder.Build(instructions, controls);
        }
    }
}
using System;
using System.Collections.Generic;
using FF8.Core;
using FF8.JSM.Format;
using Memoria.Prime.Collections;

namespace FF8.JSM.Instructions
{
    public abstract class JsmInstruction : IJsmInstruction, IFormattableScript
    {
        protected JsmInstruction()
        {
        }

        public virtual void Format(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices services)
        {
            sw.AppendLine(this.ToString());
        }

        public virtual IAwaitable Execute(IServices services)
        {
            return TestExecute(services);
        }

        public virtual IAwaitable TestExecute(IServices services)
        {
            throw new NotImplementedException($"The instruction {GetType()} is not implemented yet. Please override \"{nameof(Execute)}\" method if you know the correct behavior or \"{nameof(TestExecute)}\" method for test environment.");
        }

        public static JsmInstruction TryMake(Jsm.Opcode opcode, Int32 parameter, IStack<IJsmExpression> stack)
        {
            if (Factories.TryGetValue(opcode, out var make))
                return make(stack);

            return null;
        }
        
            
        private delegate JsmInstruction Make(IStack<IJsmExpression> stack);

        private static readonly Dictionary<Jsm.Opcode, Make> Factories = new Dictionary<Jsm.Opcode, Make>
        {
            {Jsm.Opcode.NOP, s => new NOP(s)},
        };
    }

    public interface IJsmInstruction
    {
    }
}
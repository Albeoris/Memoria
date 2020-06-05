using System;
using FF8.Core;
using Memoria.EventEngine.EV;
using Memoria.Prime.Collections;

namespace FF8.JSM
{
    public sealed class JsmExpressionEvaluator
    {
        private readonly EVScriptMaker _maker;
        private readonly IStack<IJsmExpression> _stack;
        private readonly IServices _services;

        public JsmExpressionEvaluator(EVScriptMaker maker, IStack<IJsmExpression> stack, IServices services)
        {
            _maker = maker;
            _stack = stack;
            _services = services;
        }

        public Byte ReadByte() => _maker.ReadByte();
        public Int16 ReadInt16() => _maker.ReadInt16();
        public Int32 ReadInt24() => _maker.ReadInt24();
        public Int32 ReadInt26() => _maker.ReadInt26();
        public IJsmExpression Pop() => _stack.Pop();
        public IValueExpression PopValue() => (IValueExpression) _stack.Pop();
        public IVariableExpression PopVariable() => (IVariableExpression) _stack.Pop();
        public void Push(IJsmExpression expression) => _stack.Push(expression);
    }
}
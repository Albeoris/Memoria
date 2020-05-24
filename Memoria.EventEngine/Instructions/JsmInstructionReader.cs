using System;
using Memoria.EventEngine.EV;
using Memoria.Prime.Collections;

namespace FF8.JSM.Instructions
{
    public sealed class JsmInstructionReader
    {
        private readonly EVScriptMaker _maker;
        private readonly IStack<IJsmExpression> _stack;

        private Int32 _argumentMask = -1;
        private Boolean _noArguments = false;

        public JsmInstructionReader(EVScriptMaker maker, IStack<IJsmExpression> stack)
        {
            _maker = maker;
            _stack = stack;
        }

        public Byte Arguments()
        {
            if (_noArguments)
                throw new InvalidOperationException("if (_noArguments)");
            if (_argumentMask != -1)
                throw new InvalidOperationException("if (_argumentMask != -1)");

            _noArguments = true;
            return _maker.ReadByte();
        }

        public IJsmExpression ArgumentByte() => Read(m => m.ReadByte());
        public IJsmExpression ArgumentInt16() => Read(m => m.ReadInt16());
        public IJsmExpression ArgumentInt24() => Read(m => m.ReadInt24());
        
        public IJsmExpression Pop() => _stack.Pop();

        public Byte ReadByte() => _maker.ReadByte();
        public Int16 ReadInt16() => _maker.ReadInt16();
        public UInt16 ReadUInt16() => _maker.ReadUInt16();
        public Int32 ReadInt24() => _maker.ReadInt24();

        public IJsmExpression Read(Func<EVScriptMaker, Int64> reader)
        {
            if (_noArguments)
                throw new InvalidOperationException("if (_noArguments)");
            
            if (_argumentMask == -1)
                _argumentMask = _maker.ReadByte();

            IJsmExpression result;
            if ((_argumentMask & 1) == 1)
            {
                Jsm.Expression.Evaluate(_maker, _stack);
                result = _stack.Pop();
            }
            else
            {
                result = Jsm.Expression.ValueExpression.Create(reader(_maker));
            }

            _argumentMask >>= 1;
            return result;
        }
    }
}
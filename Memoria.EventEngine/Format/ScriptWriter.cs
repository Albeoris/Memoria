using System;
using System.Text;
using Memoria.Prime.Text;

namespace FF8.JSM.Format
{
    public sealed class ScriptWriter
    {
        public Int32 Indent { get; set; }

        private readonly StringBuilder _sb;
        private Boolean _newLine;
        public Boolean HasWhiteLine { get; private set; }

        public ScriptWriter(Int32 capacity = 8096)
        {
            _sb = new StringBuilder(capacity);
        }

        public void AppendLine()
        {
            if (_newLine)
                HasWhiteLine = true;

            _sb.AppendLine();
            _newLine = true;
        }

        public void AppendLine(String text)
        {
            AppendIndent();

            _sb.AppendLine(text);
            _newLine = true;
            HasWhiteLine = (text == "{");
        }

        public void Append(String text)
        {
            AppendIndent();

            _sb.Append(text);
        }

        private void AppendIndent()
        {
            if (_newLine)
            {
                for (Int32 i = 0; i < Indent; i++)
                    _sb.Append("    ");

                _newLine = false;
                HasWhiteLine = false;
            }
        }

        public String Release()
        {
            String result = _sb.ToString();

            _sb.Clear();
            Indent = 0;
            _newLine = true;
            HasWhiteLine = false;

            return result;
        }

        public override String ToString()
        {
            return _sb.ToString();
        }

        public State RememberState()
        {
            return new State(this);
        }

        public sealed class State
        {
            private readonly ScriptWriter _sw;

            private Int32 _indent;
            private Boolean _newLine;
            private Boolean _hasEmptyLine;
            private Int32 _length;

            public State(ScriptWriter sw)
            {
                _sw = sw;

                _indent = _sw.Indent;
                _newLine = _sw._newLine;
                _length = _sw._sb.Length;
                _hasEmptyLine = _sw.HasWhiteLine;
            }

            public Boolean IsChanged => _length != _sw._sb.Length;

            public void Cancel()
            {
                _sw.Indent = _indent;
                _sw._newLine = _newLine;
                _sw.HasWhiteLine = _hasEmptyLine;
                _sw._sb.Length = _length;
            }
        }
    }
}
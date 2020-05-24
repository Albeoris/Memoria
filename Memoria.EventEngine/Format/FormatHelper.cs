using System;
using System.Globalization;
using FF8.Core;
using Jsm = FF8.JSM.Jsm;

namespace FF8.JSM.Format
{
    public static class FormatHelper
    {
        private static readonly String[] Separators = {"\r\n", "\n", "{Line}", "{Next}"};

        public static void FormatMonologue(ScriptWriter sw, String message)
        {
            if (!sw.HasWhiteLine)
                sw.AppendLine();

            foreach (String str in SplitMonologue(message))
            {
                if (String.IsNullOrEmpty(str))
                    continue;

                sw.Append("// ");
                sw.AppendLine(str);
            }
        }

        public static void FormatAnswers(ScriptWriter sw, String message, IJsmExpression top, IJsmExpression bottom, IJsmExpression begin, IJsmExpression cancel)
        {
            if (!(top is IConstExpression t) || !(bottom is IConstExpression b))
            {
                FormatMonologue(sw, message);
                return;
            }

            if (!sw.HasWhiteLine)
                sw.AppendLine();

            // Question
            //☞ Answer 1
            //   Answer 2
            //   Answer 3
            //☜ Answer 4

            Int32 to = t.Int32();
            Int32 bo = b.Int32();
            Int32 be = -1;
            Int32 ca = -1;

            if (begin is IConstExpression beg)
                be = beg.Int32();

            if (cancel is IConstExpression can)
                ca = can.Int32();

            String[] lines = SplitMonologue(message);

            for (Int32 i = 0; i < lines.Length; i++)
            {
                sw.Append("//");
                if (i >= to && i <= bo)
                {
                    if (i == be)
                        sw.Append("☞ ");
                    else if (i == ca)
                        sw.Append("☜ ");
                    else
                        sw.Append("   ");
                }
                else
                {
                    sw.Append(" ");
                }

                sw.AppendLine(lines[i]);
            }
        }

        public static String[] SplitMonologue(String message)
        {
            return message.Split(Separators, StringSplitOptions.None);
        }

        public static void FormatGlobalGet<T>(GlobalVariableId globalVariable, Int32[] knownVariables, ScriptWriter sw, IScriptFormatterContext formatterContext, IServices executionContext) where T: unmanaged
        {
            if (knownVariables == null || Array.BinarySearch(knownVariables, globalVariable.VariableId) < 0)
            {
                sw.Append("(");
                sw.Append(globalVariable.TypeName);
                sw.Append(")");
                sw.Append("G");
            }
            else
            {
                sw.Append("G");
                sw.Append(globalVariable.TypeName);
            }

            sw.Append("[");
            sw.Append(globalVariable.VariableId.ToString(CultureInfo.InvariantCulture));
            sw.Append("]");
        }

        public static void FormatGlobalSet<T>(GlobalVariableId globalVariable, IJsmExpression value, Int32[] knownVariables, ScriptWriter sw, IScriptFormatterContext formatterContext, IServices executionContext) where T: unmanaged
        {
            sw.Append("G");

            if (knownVariables == null || Array.BinarySearch(knownVariables, globalVariable.VariableId) < 0)
            {
                sw.Append("[");
                sw.Append(globalVariable.VariableId.ToString(CultureInfo.InvariantCulture));
                sw.Append("]");
                sw.Append(" = ");
                sw.Append("(");
                sw.Append(globalVariable.TypeName);
                sw.Append(")");
            }
            else
            {
                sw.Append(globalVariable.TypeName);
                sw.Append("[");
                sw.Append(globalVariable.VariableId.ToString(CultureInfo.InvariantCulture));
                sw.Append("]");
                sw.Append(" = ");
            }

            value.Format(sw, formatterContext, executionContext);
            sw.AppendLine(";");
        }

        public static Formatter Format(this ScriptWriter sw, IScriptFormatterContext formatterContext, IServices executionContext)
        {
            return new Formatter(sw, formatterContext, executionContext);
        }

        public sealed class Formatter
        {
            public ScriptWriter Sw { get; }
            public IScriptFormatterContext FormatterContext { get; }
            public IServices ExecutionContext { get; }

            public Formatter(ScriptWriter sw, IScriptFormatterContext formatterContext, IServices executionContext)
            {
                Sw = sw;
                FormatterContext = formatterContext;
                ExecutionContext = executionContext;
            }

            public Formatter Await()
            {
                Sw.Append("await ");

                return this;
            }

            public PropertyFormatter Property(String methodName)
            {
                Sw.Append("this.");
                Sw.Append(methodName);

                return new PropertyFormatter(this);
            }

            public MethodFormatter Method(String methodName)
            {
                Sw.Append("this.");
                Sw.Append(methodName);
                Sw.Append("(");
                return new MethodFormatter(this);
            }

            public StaticTypeFormatter StaticType(String typeName)
            {
                Sw.Append(typeName);
                return new StaticTypeFormatter(this);
            }

            public Formatter CommentLine(String text)
            {
                if (!Sw.HasWhiteLine)
                    Sw.AppendLine();

                Sw.Append("// ");
                Sw.AppendLine(text);
                return this;
            }
        }

        public sealed class StaticTypeFormatter
        {
            public Formatter Formatter { get; }

            public StaticTypeFormatter(Formatter formatter)
            {
                Formatter = formatter;
            }

            public PropertyFormatter Property(String propertyName)
            {
                Formatter.Sw.Append(".");
                Formatter.Sw.Append(propertyName);

                return new PropertyFormatter(Formatter);
            }

            public MethodFormatter Method(String methodName)
            {
                ScriptWriter sw = Formatter.Sw;
                sw.Append(".");
                sw.Append(methodName);
                sw.Append("(");
                return new MethodFormatter(Formatter);
            }
        }

        public sealed class PropertyFormatter
        {
            public Formatter Formatter { get; }

            public PropertyFormatter(Formatter formatter)
            {
                Formatter = formatter;
            }

            public PropertyFormatter Property(String propertyName)
            {
                Formatter.Sw.Append(".");
                Formatter.Sw.Append(propertyName);

                return new PropertyFormatter(Formatter);
            }

            public MethodFormatter Method(String methodName)
            {
                ScriptWriter sw = Formatter.Sw;
                sw.Append(".");
                sw.Append(methodName);
                sw.Append("(");
                return new MethodFormatter(Formatter);
            }

            public PropertyFormatter Assign(Boolean value)
            {
                ScriptWriter sw = Formatter.Sw;
                sw.Append(" = ");
                sw.Append(value ? "true" : "false");
                sw.Append(";");
                return this;
            }

            public PropertyFormatter Assign(Int64 value)
            {
                ScriptWriter sw = Formatter.Sw;
                sw.Append(" = ");
                sw.Append(value.ToString(CultureInfo.InvariantCulture));
                sw.Append(";");
                return this;
            }

            public PropertyFormatter Assign(IJsmExpression assignExpression)
            {
                ScriptWriter sw = Formatter.Sw;
                sw.Append(" = ");
                assignExpression.Format(sw, Formatter.FormatterContext, Formatter.ExecutionContext);
                sw.Append(";");
                return this;
            }

            public void Comment(String nativeName)
            {
                Formatter.Sw.Append(" // ");
                Formatter.Sw.AppendLine(nativeName);
            }
        }

        public sealed class MethodFormatter
        {
            public Formatter Formatter { get; }

            public MethodFormatter(Formatter formatter)
            {
                Formatter = formatter;
            }

            private Boolean _hasArguments = false;

            public MethodFormatter Enum<T>(T argumentValue) where T : struct
            {
                return Argument(typeof(T).Name + '.' + argumentValue);
            }

            public MethodFormatter Enum<T>(IJsmExpression argumentExpression) where T : struct
            {
                if (argumentExpression is IConstExpression expr)
                    return Argument(typeof(T).Name + '.' + (T)(Object)expr.Int32());

                return Argument<T>(null, argumentExpression);
            }

            public MethodFormatter Argument(String argumentName, Int32 argumentValue)
            {
                return Argument(argumentName, argumentValue.ToString(CultureInfo.InvariantCulture));
            }

            public MethodFormatter Argument(String argumentName, Boolean argumentValue)
            {
                return Argument(argumentName, argumentValue ? "true" : "false");
            }

            public MethodFormatter Argument(String argumentName, IJsmExpression argumentExpression)
            {
                ScriptWriter sw = Formatter.Sw;
                
                if (_hasArguments)
                    sw.Append(", ");

                if (argumentName != null)
                {
                    sw.Append(argumentName);
                    sw.Append(": ");
                }

                argumentExpression.Format(sw, Formatter.FormatterContext, Formatter.ExecutionContext);

                _hasArguments = true;
                return this;
            }

            public MethodFormatter Argument<T>(String argumentName, IJsmExpression argumentExpression)
            {
                ScriptWriter sw = Formatter.Sw;
                
                if (_hasArguments)
                    sw.Append(", ");

                if (argumentName != null)
                {
                    sw.Append(argumentName);
                    sw.Append(": ");
                }

                sw.Append("(");
                sw.Append(typeof(T).Name);
                sw.Append(")");

                argumentExpression.Format(sw, Formatter.FormatterContext, Formatter.ExecutionContext);

                _hasArguments = true;
                return this;
            }

            public MethodFormatter Argument(String argumentValue)
            {
                ScriptWriter sw = Formatter.Sw;
                
                if (_hasArguments)
                    sw.Append(", ");

                sw.Append(argumentValue);

                _hasArguments = true;
                return this;
            }

            public MethodFormatter Argument(String argumentName, String argumentValue)
            {
                ScriptWriter sw = Formatter.Sw;
                
                if (_hasArguments)
                    sw.Append(", ");

                sw.Append(argumentName);
                sw.Append(": ");
                sw.Append(argumentValue);

                _hasArguments = true;
                return this;
            }

            public void Comment(String nativeName)
            {
                Formatter.Sw.Append("); // ");
                Formatter.Sw.AppendLine(nativeName);
            }
        }
    }
}
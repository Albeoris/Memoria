using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Memoria
{
    public sealed class Log
    {
        #region Lazy

        public static readonly String LogFileName = Assembly.GetExecutingAssembly().GetName().Name + ".log";

        private static readonly Log Instance = Initialize();

        private static Log Initialize()
        {
            try
            {
                return new Log(new FileStream(LogFileName, FileMode.Create, FileAccess.Write, FileShare.Read));
            }
            catch
            {
                Environment.Exit(1);
                return null;
            }
        }

        #endregion

        public static void Message(string format, params object[] args)
        {
            Instance.Write('M', 0, format, args);
        }

        public static void Warning(string format, params object[] args)
        {
            Instance.Write('W', 0, format, args);
        }

        public static void Error(string format, params object[] args)
        {
            Instance.Write('E', 0, format, args);
        }

        public static void Warning(Exception ex, string format = null, params object[] args)
        {
            Instance.Write('W', 0, FormatException(ex, format, args));
        }

        public static void Error(Exception ex, string format = null, params object[] args)
        {
            Instance.Write('E', 0, FormatException(ex, format, args));
        }

        private static string FormatException(Exception ex, string format, params object[] args)
        {
            StringBuilder sb = new StringBuilder(256);
            if (!string.IsNullOrEmpty(format))
            {
                if (args.IsNullOrEmpty())
                    sb.AppendLine(format);
                else
                    sb.AppendFormatLine(format, args);
            }
            sb.Append(ex);
            return sb.ToString();
        }

        #region Instance

        private readonly StreamWriter _sw;

        private Log(Stream stream)
        {
            Exceptions.CheckArgumentNull(stream, "stream");

            _sw = new StreamWriter(stream);
        }

        public void Write(char type, int offset, string format, params object[] args)
        {
            Monitor.Enter(_sw);
            try
            {
                if (string.IsNullOrEmpty(format))
                    return;

                DateTime time = DateTime.Now;
                string text = args.IsNullOrEmpty() ? format : String.Format(format, args);

                WritePrefix(time, type, offset);

                for (int i = 0; i < text.Length; i++)
                {
                    char ch = text[i];
                    if (ch == '\n')
                    {
                        _sw.WriteLine();
                        if (i + 2 < text.Length && text[i + 2] != '\r' || i + 1 < text.Length && text[i + 1] != '\r')
                            WritePrefix(time, type, offset);
                    }
                    else if (ch != '\r')
                    {
                        _sw.Write(ch);
                    }
                }

                _sw.WriteLine();
                _sw.Flush();
            }
            catch
            {
            }
            finally
            {
                Monitor.Exit(_sw);
            }
        }

        private void WritePrefix(DateTime time, char type, int offset)
        {
            _sw.Write(time.ToString("dd.MM.yyyy hh:mm:ss "));
            _sw.Write('|');
            _sw.Write(type);
            _sw.Write("| ");
            while (offset-- > 0)
                _sw.Write('\t');
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.IO;

namespace Memoria.Assets
{
    public class TextCSVReader
    {
        public TextCSVReader(String[] content)
        {
            mBuffer = content;
            mBufferLine = 0;
        }

        public static TextCSVReader Open(String path)
        {
            String[] content = File.ReadAllLines(path);
            if (content != null && content.Length > 0)
                return new TextCSVReader(content);
            return null;
        }

        public Boolean HasMoreEntries => mBuffer != null && mBufferLine < mBuffer.Length;

        public String ReadLine(ref Int32 lineNo, Boolean skipEmptyLines = true)
        {
            if (skipEmptyLines)
                while (lineNo < mBuffer.Length && String.IsNullOrEmpty(mBuffer[lineNo]))
                    ++lineNo;
            if (lineNo < mBuffer.Length)
                return mBuffer[lineNo++];
            return null;
        }

        public List<String> ReadCSV()
        {
            TextCSVReader.mTemp.Clear();
            String fullField = String.Empty;
            Boolean insideQuotes = false;
            Int32 entryStart = 0;
            Int32 lineCount = mBuffer.Length;
            while (mBufferLine < lineCount)
            {
                if (insideQuotes)
                {
                    String continuedEntry = ReadLine(ref mBufferLine, false);
                    if (continuedEntry == null)
                        return null;
                    continuedEntry = continuedEntry.Replace("\\n", "\n");
                    fullField = fullField + "\n" + continuedEntry;
                }
                else
                {
                    fullField = ReadLine(ref mBufferLine, true);
                    if (fullField == null)
                        return null;
                    fullField = fullField.Replace("\\n", "\n");
                    entryStart = 0;
                }
                Int32 entryEnd = entryStart;
                Int32 length = fullField.Length;
                while (entryEnd < length)
                {
                    Char c = fullField[entryEnd];
                    if (c == ',')
                    {
                        if (!insideQuotes)
                        {
                            TextCSVReader.mTemp.Add(fullField.Substring(entryStart, entryEnd - entryStart));
                            entryStart = entryEnd + 1;
                        }
                    }
                    else if (c == '"')
                    {
                        if (insideQuotes)
                        {
                            if (entryEnd + 1 >= length)
                            {
                                TextCSVReader.mTemp.Add(fullField.Substring(entryStart, entryEnd - entryStart).Replace("\"\"", "\""));
                                return TextCSVReader.mTemp;
                            }
                            if (fullField[entryEnd + 1] != '"')
                            {
                                TextCSVReader.mTemp.Add(fullField.Substring(entryStart, entryEnd - entryStart).Replace("\"\"", "\""));
                                insideQuotes = false;
                                if (fullField[entryEnd + 1] == ',')
                                {
                                    entryEnd++;
                                    entryStart = entryEnd + 1;
                                }
                            }
                            else
                            {
                                entryEnd++;
                            }
                        }
                        else
                        {
                            entryStart = entryEnd + 1;
                            insideQuotes = true;
                        }
                    }
                    entryEnd++;
                }
                if (entryStart < fullField.Length)
                {
                    if (insideQuotes)
                        continue;
                    TextCSVReader.mTemp.Add(fullField.Substring(entryStart, fullField.Length - entryStart));
                }
                return TextCSVReader.mTemp;
            }
            return null;
        }

        private String[] mBuffer;
        private Int32 mBufferLine;

        private static List<String> mTemp = new List<String>();
    }
}

using System;
using System.IO;

public class FileLogger
{
    public FileLogger(String filename)
    {
        this._writer = new StreamWriter(filename);
        this.Enabled = true;
    }

    public static void SetDumpDataPtr(Byte[] ptr)
    {
        FileLogger.data = ptr;
    }

    public static void DumpFile(String filename)
    {
        if (FileLogger.data == null)
        {
            return;
        }
        if (!FileLogger.firstTime)
        {
            return;
        }
        FileLogger.firstTime = false;
        using (BinaryWriter binaryWriter = new BinaryWriter(new FileStream(filename, FileMode.Create)))
        {
            binaryWriter.Write(FileLogger.data);
            binaryWriter.Flush();
        }
        Debug.Log("DumpFile - Success : <" + filename + ">");
    }

    ~FileLogger()
    {
        this._writer.Close();
        this._writer.Dispose();
        this._writer = (StreamWriter)null;
    }

    public void Write(String message)
    {
        if (this._writer == null)
        {
            return;
        }
        if (!this.Enabled)
        {
            return;
        }
        this._writer.Write(message);
        this._writer.Flush();
    }

    public void WriteLine(String message)
    {
        if (this._writer == null)
        {
            return;
        }
        if (!this.Enabled)
        {
            return;
        }
        this._writer.WriteLine(message);
        this._writer.Flush();
    }

    public Boolean Enabled;

    private StreamWriter _writer;

    private static Byte[] data;

    private static Boolean firstTime = true;
}

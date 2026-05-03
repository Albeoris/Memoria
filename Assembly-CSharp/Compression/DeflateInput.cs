namespace Compression;

internal class DeflateInput
{
    private byte[] buffer;
    private int count;
    private int startIndex;

    internal byte[] Buffer
    {
        get => buffer;
        set => buffer = value;
    }

    internal int Count
    {
        get => count;
        set => count = value;
    }

    internal int StartIndex
    {
        get => startIndex;
        set => startIndex = value;
    }

    internal void ConsumeBytes(int n)
    {
        startIndex += n;
        count -= n;
    }
}

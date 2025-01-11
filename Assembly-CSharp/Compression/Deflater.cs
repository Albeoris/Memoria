namespace Compression;

internal class Deflater
{
    private FastEncoder encoder;

    public Deflater(bool doGZip)
    {
        encoder = new FastEncoder(doGZip);
    }

    public void SetInput(byte[] input, int startIndex, int count)
    {
        encoder.SetInput(input, startIndex, count);
    }

    public int GetDeflateOutput(byte[] output)
    {
        return encoder.GetCompressedOutput(output);
    }

    public bool NeedsInput()
    {
        return encoder.NeedsInput();
    }

    public int Finish(byte[] output)
    {
        return encoder.Finish(output);
    }
}

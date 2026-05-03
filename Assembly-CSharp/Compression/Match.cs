namespace Compression;

internal class Match
{
    private MatchState state;
    private int pos;
    private int len;
    private byte symbol;

    internal MatchState State
    {
        get => state;
        set => state = value;
    }

    internal int Position
    {
        get => pos;
        set => pos = value;
    }

    internal int Length
    {
        get => len;
        set => len = value;
    }

    internal byte Symbol
    {
        get => symbol;
        set => symbol = value;
    }
}

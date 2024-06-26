using System;

public class FF9BattleLoadState
{
    public FF9BattleLoadState()
    {
        this.state = new Int16[16];
    }

    private Int16[] state;

    private Byte[] addr;

    private Byte[] usage;
}

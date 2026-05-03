using System;

public class InputResult
{
    public InputResult()
    {
        this.valid = false;
        this.selectedCard = new QuadMistCard();
    }

    public void Used()
    {
        this.valid = true;
    }

    public void Clear()
    {
        this.valid = false;
    }

    public Boolean IsValid()
    {
        return this.valid;
    }

    public QuadMistCard selectedCard;

    public Int32 index;

    public Int32 x;

    public Int32 y;

    private Boolean valid;

    public Int32 selectedHandIndex;
}

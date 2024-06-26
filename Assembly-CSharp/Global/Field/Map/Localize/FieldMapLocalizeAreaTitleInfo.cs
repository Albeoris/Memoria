using System;

public class FieldMapLocalizeAreaTitleInfo
{
    public FieldMapLocalizeAreaTitleInfo()
    {
        this.data = new Int32[14];
    }

    public Int32 GetSpriteCount()
    {
        Int32 num = 6;
        Int32 num2 = num * 2;
        return this.data[num2] + this.data[num2 + 1];
    }

    public Int32 GetSpriteStartIndex(String localizeSymbol)
    {
        Int32 idx = 0;
        switch (localizeSymbol)
        {
            case "UK" when !this.hasUK:
                return 0;
            case "UK":
                idx = 6;
                break;
            case "JP":
                idx = 1;
                break;
            case "ES":
                idx = 2;
                break;
            case "FR":
                idx = 3;
                break;
            case "GR":
                idx = 4;
                break;
            case "IT":
                idx = 5;
                break;
        }

        Int32 num2 = idx * 2;
        return this.data[num2];
    }

    public Int32 GetSpriteTitleCount(String localizeSymbol)
    {
        Int32 num = 0;
        if (localizeSymbol == "UK")
        {
            if (!this.hasUK)
            {
                return 0;
            }
            num = 6;
        }
        if (localizeSymbol == "JP")
        {
            num = 1;
        }
        else if (localizeSymbol == "ES")
        {
            num = 2;
        }
        else if (localizeSymbol == "FR")
        {
            num = 3;
        }
        else if (localizeSymbol == "GR")
        {
            num = 4;
        }
        else if (localizeSymbol == "IT")
        {
            num = 5;
        }
        Int32 num2 = num * 2;
        return this.data[num2 + 1];
    }

    public String mapName;

    // public Int32 atlasWidth;
    //
    // public Int32 atlasHeight;

    public Int32 startOvrIdx;

    public Int32 endOvrIdx;

    public Boolean hasUK;

    public Int32[] data;
}

#r "..\Output\Assembly-CSharp.dll"

using System;
using System.Linq;
using System.Text;
using Memoria.Data;

public static class Utils
{
    public static String Print(uint uiValue)
    {
        BattleStatus value = (BattleStatus)uiValue;
        if (value == 0)
            return string.Empty;

        StringBuilder sb = new StringBuilder();
        BattleStatus[] items = (BattleStatus[])Enum.GetValues(typeof(BattleStatus));
        foreach (var item in items)
        {
            if ((value & item) == item)
            {
                value &= ~item;

                if (sb.Length > 0)
                    sb.Append(" | ");

                sb.Append("BattleStatus.");
                sb.Append(item);
            }
        }

        return sb.ToString();
    }
}
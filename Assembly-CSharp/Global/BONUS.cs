using Memoria.Data;
using System;
using System.Collections.Generic;

public class BONUS
{
    public BONUS()
    {
        this.item = new List<RegularItem>();
    }

    public Int32 gil;
    public UInt32 exp;
    public UInt16 ap;

    public Byte member_flag;
    public Boolean Event;

    public List<RegularItem> item;
    public TetraMasterCardId card;
    public Boolean escape_gil;
}

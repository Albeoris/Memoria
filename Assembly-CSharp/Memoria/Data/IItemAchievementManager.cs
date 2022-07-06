using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Memoria.Data
{
    public interface IItemAchievementManager
    {
        void FF9Item_Achievement(Int32 id, Int32 count, FF9ITEM[] items, PLAYER[] party);
    }
}

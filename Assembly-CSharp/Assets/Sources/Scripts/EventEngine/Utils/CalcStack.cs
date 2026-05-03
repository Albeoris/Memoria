using Memoria.Prime;
using System;
using System.Collections.Generic;

namespace Assets.Sources.Scripts.EventEngine.Utils
{
    public class CalcStack
    {
        public Boolean push(Int32 val)
        {
            while (topOfStackID >= stack.Count)
                stack.Add(0);
            stack[topOfStackID++] = val;
            return true;
        }

        public Boolean pop(out Int32 output)
        {
            if (topOfStackID == 0)
            {
                Log.Error($"[{nameof(CalcStack)}.{nameof(pop)}] topOfStackID == 0");
                output = default;
                return false;
            }
            output = stack[--topOfStackID];
            return true;
        }

        public Boolean advanceTopOfStack()
        {
            topOfStackID++;
            while (topOfStackID > stack.Count)
                stack.Add(0);
            return true;
        }

        public Boolean retreatTopOfStack()
        {
            if (topOfStackID == 0)
                return false;
            topOfStackID--;
            return true;
        }

        public void emptyCalcStack()
        {
            substack.Clear();
            for (Int32 i = 0; i < stack.Count; i++)
                stack[i] = 0;
            topOfStackID = 0;
        }

        public Int32 getTopOfStackID()
        {
            return topOfStackID;
        }

        public Int32 getValueAtOffset(Int32 offset)
        {
            return stack[topOfStackID + offset];
        }

        public void pushSubs(params Int32[] val)
        {
            substack[topOfStackID] = new List<Int32>(val);
        }

        public List<Int32> getSubs()
        {
            if (substack.TryGetValue(topOfStackID, out List<Int32> result))
                return result;
            return new List<Int32>();
        }

        private Dictionary<Int32, List<Int32>> substack = new Dictionary<Int32, List<Int32>>();
        private List<Int32> stack = new List<Int32>(16);
        private Int32 topOfStackID;
    }
}

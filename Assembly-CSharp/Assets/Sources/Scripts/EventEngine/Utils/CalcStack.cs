using System;
using Memoria.Prime;

namespace Assets.Sources.Scripts.EventEngine.Utils
{
	public class CalcStack
	{
		public Boolean push(Int32 arg0)
		{
			if (this.topOfStackID >= this.stack.Length - 1)
				return false;
			this.stack[this.topOfStackID] = arg0;
			this.topOfStackID++;
			return true;
		}

		public Boolean pop(out Int32 output)
		{
			if (this.topOfStackID == 0)
			{
				Log.Error($"[{nameof(CalcStack)}.{nameof(pop)}] this.topOfStackID == 0");
				output = default;
				return false;
			}
			output = this.stack[this.topOfStackID - 1];
			this.topOfStackID--;
			return true;
		}

		public Boolean advanceTopOfStack()
		{
			if (this.topOfStackID >= this.stack.Length - 1)
				return false;
			this.topOfStackID++;
			return true;
		}

		public Boolean retreatTopOfStack()
		{
			if (this.topOfStackID == 0)
				return false;
			this.topOfStackID--;
			return true;
		}

		public void emptyCalcStack()
		{
			for (Int32 i = 0; i < this.stack.Length; i++)
				this.stack[i] = 0;
			this.topOfStackID = 0;
		}

		public Int32 getTopOfStackID()
		{
			return this.topOfStackID;
		}

		public Int32 getValueAtOffset(Int32 offset)
		{
			return this.stack[this.topOfStackID + offset];
		}

		private const Int32 STACK_SIZE = 16;
		private Int32[] stack = new Int32[STACK_SIZE];
		private Int32 topOfStackID;
	}
}

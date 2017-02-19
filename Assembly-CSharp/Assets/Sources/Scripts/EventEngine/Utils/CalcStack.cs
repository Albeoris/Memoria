using System;

namespace Assets.Sources.Scripts.EventEngine.Utils
{
	public class CalcStack
	{
		public Boolean push(Int32 arg0)
		{
			if (this.topOfStackID >= (Int32)this.stack.Length - 1)
			{
				return false;
			}
			this.stack[this.topOfStackID] = arg0;
			this.topOfStackID++;
			return true;
		}

		public Boolean pop(ref Int32 output)
		{
			if (this.topOfStackID == 0)
			{
				return false;
			}
			output = this.stack[this.topOfStackID - 1];
			this.topOfStackID--;
			return true;
		}

		public Boolean advanceTopOfStack()
		{
			if (this.topOfStackID >= (Int32)this.stack.Length - 1)
			{
				return false;
			}
			this.topOfStackID++;
			return true;
		}

		public Boolean retreatTopOfStack()
		{
			if (this.topOfStackID == 0)
			{
				return false;
			}
			this.topOfStackID--;
			return true;
		}

		public void emptyCalcStack()
		{
			for (Int32 i = 0; i < (Int32)this.stack.Length; i++)
			{
				this.stack[i] = 0;
			}
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

		private const Int32 stackSize = 16;

		private Int32[] stack = new Int32[16];

		private Int32 topOfStackID;
	}
}

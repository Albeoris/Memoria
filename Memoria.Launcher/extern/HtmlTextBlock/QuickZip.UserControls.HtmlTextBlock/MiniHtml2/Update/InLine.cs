/*
 * Created by SharpDevelop.
 * User: LYCJ
 * Date: 20/10/2007
 * Time: 20:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Drawing;

namespace QuickZip.MiniHtml2
{
	/// <summary>
	/// Description of InLine.
	/// </summary>
	public class Inline : TextElement
	{
		protected Inline nextInLine = null;
		protected Inline previousInLine = null;
		internal void setNextInLine(Inline aInline) { nextInLine = aInline; }
		
		public Inline NextInLine { get { return nextInLine; } }
		public Inline PreviousInLine { get { return previousInLine; } }
		
		
//		public InlineCollection InLines 
		
		
		public override FontStyle FontStyle 
		{ 
			get { 
					if (NextInLine != null)
						return fontStyle | PreviousInLine.FontStyle;
					return fontStyle;
				}
		 	set { fontStyle = value; } 
		}
		
		
		public Inline()
		{
		}
		
		public override string ToString()
		{
			string retVal = this.GetType().Name + "|";
			if (PreviousInLine != null)
				return retVal + PreviousInLine.ToString();
			return retVal;
		}
	}
}

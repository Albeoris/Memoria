/*
 * Created by SharpDevelop.
 * User: LYCJ
 * Date: 20/10/2007
 * Time: 20:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;

namespace QuickZip.MiniHtml2
{
	/// <summary>
	/// Description of Run.
	/// </summary>
	public class Run : Inline
	{
		private string text;
		public string Text { get { return text; } }		
		
		public Run()
		{
			text = "";
		}
		
		public Run(string aText)
		{
			text = aText;
		}
		
		public override string ToString()
		{
			return Text;
		}
				
	}
}

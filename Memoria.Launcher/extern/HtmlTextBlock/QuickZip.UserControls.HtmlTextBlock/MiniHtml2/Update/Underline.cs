/*
 * Created by SharpDevelop.
 * User: LYCJ
 * Date: 21/10/2007
 * Time: 11:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;

namespace QuickZip.MiniHtml2
{
	/// <summary>
	/// Description of Underline.
	/// </summary>
	public class Underline : Span
	{
		
		private void init()
		{
			fontStyle = FontStyle.Underline;
		}
		
		public Underline()
		{
			init();
		}
		
		public Underline(Inline aInline) : base(aInline) 
		{
			init();
		}
		
		
	}
}

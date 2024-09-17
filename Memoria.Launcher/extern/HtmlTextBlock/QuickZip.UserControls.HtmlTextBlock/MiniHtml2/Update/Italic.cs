/*
 * Created by SharpDevelop.
 * User: LYCJ
 * Date: 20/10/2007
 * Time: 21:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;

namespace QuickZip.MiniHtml2
{
	/// <summary>
	/// Description of Italic.
	/// </summary>
	public class Italic : Span
	{
		private void init()
		{
			fontStyle = FontStyle.Italic;
		}
		
		public Italic()
		{
			init();
		}
		
		public Italic(Inline aInline) : base(aInline) 
		{
			init();
		}
	}
}

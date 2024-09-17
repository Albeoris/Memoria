/*
 * Created by SharpDevelop.
 * User: LYCJ
 * Date: 20/10/2007
 * Time: 20:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;

namespace QuickZip.MiniHtml2
{
	/// <summary>
	/// Description of Bold.
	/// </summary>
	public class Bold : Span
	{
		
		private void init()
		{
			fontStyle = FontStyle.Bold;
		}
		
		public Bold()
		{
			init();
		}
		
		public Bold(Inline aInline) : base(aInline) 
		{
			init();
		}
		
		
	}
}

/*
 * Created by SharpDevelop.
 * User: LYCJ
 * Date: 21/10/2007
 * Time: 10:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace QuickZip.MiniHtml2
{
	/// <summary>
	/// Description of Paragraph.
	/// </summary>
	public class Paragraph : Block
	{
		public InlineCollection Inlines = new InlineCollection();
		
		public Paragraph()
		{
		}
		
		public Paragraph(Inline aInline)
		{
			Inlines.Add(aInline);
		}
	}
}

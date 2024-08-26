/*
 * Created by SharpDevelop.
 * User: LYCJ
 * Date: 20/10/2007
 * Time: 20:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;

namespace QuickZip.MiniHtml2
{
	/// <summary>
	/// Description of TextBlock.
	/// </summary>
	public class TextBlock
	{
		private TextWrapping textWrapping;
		
		public InlineCollection Inlines = new InlineCollection();
		public TextWrapping TextWrapping { get { return textWrapping; } set { textWrapping = value; } }
		
		public TextBlock()
		{			
		}
	}
}

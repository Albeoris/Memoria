/*
 * Created by SharpDevelop.
 * User: LYCJ
 * Date: 21/10/2007
 * Time: 10:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace QuickZip.MiniHtml2
{
	/// <summary>
	/// Description of FlowDocument.
	/// </summary>
	public class FlowDocument
	{
		public BlockCollection Blocks;
		public FlowDirection FlowDirection;
		public Double PageWidth;
		public Double PageHeight;
		
		public FlowDocument()
		{
			Blocks = new BlockCollection();
			
		}
	}
}

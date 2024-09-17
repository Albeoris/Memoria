/*
 * Created by SharpDevelop.
 * User: LYCJ
 * Date: 21/10/2007
 * Time: 10:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace QuickZip.MiniHtml2
{
	/// <summary>
	/// Description of Block.
	/// </summary>
	public class Block : TextElement
	{
		protected FlowDirection flowDirection;
		public FlowDirection FlowDirection { get { return flowDirection; } set { flowDirection = value; } }
		
		protected Block()
		{
		}
	}
}

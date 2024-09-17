/*
 * Created by SharpDevelop.
 * User: LYCJ
 * Date: 20/10/2007
 * Time: 20:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;

namespace QuickZip.MiniHtml2
{
	/// <summary>
	/// Description of TextElement.
	/// </summary>
	public class TextElement : FrameworkContentElement
	{				
		protected FontStyle fontStyle = FontStyle.Regular;		
		protected Color fontColor = Color.Black;
		
		public virtual FontStyle FontStyle { get { return fontStyle; } set { fontStyle = value; } }
		public virtual Color FontColor { get { return fontColor; } set { fontColor = value; } }
		
		public TextElement()
		{
		}
	}
}

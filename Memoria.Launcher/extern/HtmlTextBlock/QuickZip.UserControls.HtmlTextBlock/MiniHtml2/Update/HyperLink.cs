/*
 * Created by SharpDevelop.
 * User: LYCJ
 * Date: 21/10/2007
 * Time: 11:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;

namespace QuickZip.MiniHtml2
{
	/// <summary>
	/// Description of HyperLink.
	/// </summary>
	public class HyperLink : Span
	{
		private Uri baseURI;		
		protected Uri BaseURI { get { return baseURI; } set { baseURI = value; } }
		
		private void init()
		{
			fontStyle = FontStyle.Underline;
			fontColor = Color.Blue; 
		}
		
		public HyperLink()
		{
			init();
		}
		
		public HyperLink(Inline aInline) : base(aInline) 
		{
			init();
		}
	}
}

/*
 * Created by SharpDevelop.
 * User: LYCJ
 * Date: 20/10/2007
 * Time: 20:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;

namespace QuickZip.MiniHtml2
{
	/// <summary>
	/// Description of Span.
	/// </summary>
	public class Span : Inline
	{														
		public Span()
		{
		}
		
		public Span(Inline aInline)
		{
			previousInLine = aInline;
			aInline.setNextInLine(this);
		}
		
	}
}

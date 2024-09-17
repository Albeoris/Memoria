/*
 * Created by SharpDevelop.
 * User: LYCJ
 * Date: 20/10/2007
 * Time: 20:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;

namespace QuickZip.MiniHtml2
{
	/// <summary>
	/// Description of InlineCollection.
	/// </summary>
	public class InlineCollection : List<Inline>
	{
		public InlineCollection() : base()
		{
		}
		
		public void Add(string aText)
		{
			Add(new Run(aText));
		}		
	}
}

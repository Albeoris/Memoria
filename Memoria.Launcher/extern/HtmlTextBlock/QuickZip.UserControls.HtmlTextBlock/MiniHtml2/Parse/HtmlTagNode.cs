/*
 * Created by SharpDevelop.
 * User: LYCJ
 * Date: 18/10/2007
 * Time: 23:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Collections;

namespace QuickZip.MiniHtml2
{
	public class HtmlTagNode : IEnumerable
	{
		protected bool isRoot;
		protected HtmlTag tag;
		protected List<HtmlTagNode> childTags;
		protected HtmlTagNode parentNode;
				
		///<summary> Gets the embedded HtmlTag of this node. <> </summary>
		public HtmlTag Tag { get { return tag; } }
		///<summary> Gets parent node of this node. <> </summary>
		public HtmlTagNode Parent { get { return parentNode; } }
		///<summary> Gets subnodes of this node. </summary>
		public List<HtmlTagNode> Items { get { return childTags; } }
		///<summary> Gets subnodes emuerator. </summary>
		public IEnumerator GetEnumerator() { return childTags.GetEnumerator(); }
		///<summary> Gets whether this node is root of all nodes. </summary>
		public bool IsRoot { get { return isRoot; } }
		///<summary> Gets whether this node contain other nodes. </summary>
		public bool isContainer { get { return childTags.Count > 0; } }
		
		public bool isBlock { get { return Defines.BuiltinTags[tag.ID].flags == HTMLFlag.Region; } }		
								
		public virtual bool CanAdd(HtmlTag aTag) 
		{
			if (tag.IsEndTag)
				return false;
			
			if ((aTag.Name == '/' + tag.Name) ||
			    (aTag.Level < tag.Level))
				return true;
			return false;
		}
		
		public HtmlTagNode Add(HtmlTag aTag)
		{
			if (!CanAdd(aTag))
				throw new Exception("Cannot add here, check your coding.");
			
			HtmlTagNode retVal = new HtmlTagNode(this, aTag);
			Items.Add(retVal);
			
			if (aTag.Name == '/' + tag.Name)
				return Parent;
			else return retVal;
		}
			
		///<summary> Constructor, hide from user view. </summary>
		private HtmlTagNode(HtmlTag aTag) { ;}
		///<summary> Constructor. </summary>
		public HtmlTagNode(HtmlTagNode aParentNode, HtmlTag aTag) : base()
		{
			isRoot = false;
			parentNode = aParentNode;
			tag = aTag;
			childTags = new List<HtmlTagNode>();			
		}
		
		public List<HtmlTag> ToHtmlTagList()
		{
			List<HtmlTag> retVal = new List<HtmlTag>();
			retVal.Add(Tag);
			
			foreach (HtmlTagNode subnode in this)
				retVal.AddRange(subnode.ToHtmlTagList());
			
			return retVal;
		}
		
		public override string ToString()
		{
			return tag.ToString();
		}
		
		///<summary> Debug this component. </summary>
		public void PrintItems()
		{
			foreach (HtmlTag t in this)
				Console.WriteLine(t);
		}
	}
	
	
		
}

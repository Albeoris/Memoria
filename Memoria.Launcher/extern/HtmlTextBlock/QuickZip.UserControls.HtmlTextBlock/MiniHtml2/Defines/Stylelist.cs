/*
 * Created by SharpDevelop.
 * User: Joseph Leung
 * Date: 8/9/2006
 * Time: 11:14 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Collections.Specialized;
using QuickZip.MiniHtml2;

namespace QuickZip.MiniCss
{	
	/// <summary>
	/// Convert Header string to useable type.
	/// </summary>
	public class CssHeaderStyleType
	{
		public static string UnspecifiedTagName = "n0ne";
		public enum ElementType {Unknown, FirstChar, FirstLine, FirstLetter,
						Link, Visited, Focused, Hover, Active};
		public ElementType elements;
		public string tagName;
		public string tagClass;
		public string tagID; 
		public bool familyTag;
		public bool noOtherClassID;

		public static ElementType ElementsToCssElementType(string input)
		{
			switch (input)
			{
				case ("first-char"):
					return ElementType.FirstChar;
				case ("first-line"):
					return ElementType.FirstLine;
				case ("first-letter"):
					return ElementType.FirstLetter;
				case ("link"):
					return ElementType.Link;
				case ("visited"):
					return ElementType.Visited;
				case ("focused"):
					return ElementType.Focused;
				case ("hover"):
					return ElementType.Hover;
				default:
					return ElementType.Active;
			}
		}
		public void PrintItems()
		{
			string fmt = "Name:{0}, ID:{1}, Cls:{2}, EleIdx:{3}, Flags:{4}{5}";
			string f = "_"; if (familyTag) f = "F"; 
			string n = "_"; if (noOtherClassID) n = "N"; 
			Console.WriteLine(String.Format(fmt, tagName, tagID, tagClass, 
			                              (Int32)elements, f, n));
		}
		
		public string Css()
		{
			string idStr = "";
			string classStr = "";
			if (tagID != "") idStr = "#" + tagID;
			if (tagClass != "") classStr = "." + tagClass;
			return tagName + idStr + classStr + " ";				
		}
		
		public CssHeaderStyleType(string header)
		{
			elements = ElementType.Unknown;
			familyTag = false;
			noOtherClassID = false;
			tagID = "";
			tagClass = "";
			
			string k = header;
			
			char lastChar = k[k.Length-1];
			switch (lastChar)
			{
				case '+':
					familyTag = true;
					k = k.Substring(0, k.Length - 1);
					break;
				case '>':
					noOtherClassID = true;
					k = k.Substring(0, k.Length - 1);
					break;
			}
			
			if (k.IndexOf('#') > -1)
				tagID = Utils.ExtractAfter(ref k, '#');
			if (k.IndexOf('.') > -1)
				tagClass = Utils.ExtractAfter(ref k, '.');
			if (k.IndexOf(':') > -1)
				elements = CssHeaderStyleType.ElementsToCssElementType(
				           		Utils.ExtractAfter(ref k, ':'));
			tagName = k;
			if (tagName.Trim() == "")
				tagName = UnspecifiedTagName;

		}
	}
	
	/// <summary>
	/// Store one record of a full css style (header and styles)
	/// </summary>
	public class CssStyleType
	{
		public string tagName;
		public string styleTagName;
		public ArrayList parentTagName;
		public string styleClass;
		public string styleID;
		public PropertyList styles;
		
		public CssStyleType()
		{
			parentTagName = new ArrayList();
			styles = new PropertyList();
		}
		~CssStyleType()
		{			
			styles = null;
			parentTagName = null;
		}
		public string printParentTagName()
		{
			string retVal = "";
			foreach (object o in parentTagName)
			{
				retVal += ',' + (string)o;
			}
			return retVal.Trim(',');
		}
	}
	
	public class CssStyleGroupType
	{
		public string styleTagName;
		public ArrayList parentTagName;
		
		public CssStyleGroupType()
		{
			parentTagName = new ArrayList();
		}
		~CssStyleGroupType()
		{
			parentTagName = null;
		}
	}
	
	/// <summary>
	/// A List of CssStyleType
	/// </summary>
	public class CssStyleList : CollectionBase
	{
		private CssStyleType getCssStyle(Int32 index)
		{
			return (CssStyleType)List[index];
		}
		
		private void setCssStyle(Int32 index, CssStyleType value)
		{
			List[index] = value;
		}
		
		private void verifyType(object value)
		{
			if (value == null)
				throw new ArgumentException("Nil exception");
			if (!(value is CssStyleType))
				throw new ArgumentException("Invalid Type - " + value.ToString());
		}
		
		protected override void OnInsert(int index, object value)
		{
			verifyType(value);
			base.OnInsert(index, value);
		}
		
		protected override void OnSet(int index, object oldValue, object newValue)
		{
			verifyType(newValue);
			base.OnSet(index, oldValue, newValue);
		}
		
		protected override void OnValidate(object value)
		{
			verifyType(value);
			base.OnValidate(value);
		}
		
		public CssStyleList() : base()
		{
			
		}
		
		~CssStyleList()
		{
			List.Clear();
		}
		
		public Int32 Add(CssStyleType value)
		{
			return List.Add(value);
		}
		
		public void Insert(Int32 index, CssStyleType value)
		{
			List.Insert(index, value);
		}
		
		public void Remove(CssStyleType value)
		{
			List.Remove(value);
		}
		
		public bool Contains(CssStyleType value)
		{
			return List.Contains(value);
		}
		
		public void PrintItems()
		{
			for (Int32 i = 0; i < Count; i++)
			{
				CssStyleType c = this[i];
				CssHeaderStyleType style = new CssHeaderStyleType(c.styleTagName);
				style.PrintItems();
				
				for (Int32 j = 0; j < c.styles.Count; j++)
				{
					string output = String.Format("[key: {0} = {1}]", 
					                              c.styles[j].key, c.styles[j].value);
					Console.WriteLine(output);					
				}
				Console.WriteLine("");
			}
		}
		
		public string Css()
		{
			string retVal = "";
			for (Int32 i = 0; i < Count; i++)
			{
				CssStyleType c = this[i];
				CssHeaderStyleType style = new CssHeaderStyleType(c.styleTagName);
				retVal += style.Css() + "{ ";
				
				for (Int32 j = 0; j < c.styles.Count; j++)
					retVal += c.styles[j].key + ":" + c.styles[j].value + ";";													
				
				retVal += " }" + Defines.lineBreak;
			}
			return retVal;
		}
		
		public CssStyleType this[Int32 index]
		{
			get
			{
				return getCssStyle(index);
			}
			set
			{
				setCssStyle(index, value);
			}
		}
	}
	
	/// <summary>
	/// A List of CssStyleType for specified tag
	/// </summary>
	public class TagCssStyleType
	{
		public string tagName;
		public CssStyleList cssStyles;
		public TagCssStyleType(string aTagName) : base()
		{
			tagName = aTagName;
			cssStyles = new CssStyleList();
		}
		~TagCssStyleType() 
		{
			cssStyles = null;
		}
		public void AddCssStyle(CssStyleType aStyle)
		{
			cssStyles.Add(aStyle);
		}
		
	}
	
	/// <summary>
	/// Container of all TagCssStyleType (All cssStyles for all tags)
	/// </summary>
	public class TagCssStyleDictionary : ListDictionary
	{
		private TagCssStyleType getCssTagStyle(string key)
		{
			if (Contains(key))
			{
				foreach (DictionaryEntry de in this)
					if ((string)de.Key == key)
					return (TagCssStyleType)(de.Value);
			}
			
			return new TagCssStyleType(key);
		}
		private TagCssStyleType getCssTagStyle(Int32 id)
		{
			IDictionaryEnumerator em = this.GetEnumerator();
			if (Count >= id)
			{
				for (Int32 i = 0; i <= id; i++)
					em.MoveNext();
				return (TagCssStyleType)(em.Value);
			}
			return new TagCssStyleType(id.ToString());
		}
		private void setCssTagStyle(string key, TagCssStyleType value)
		{
			if (Contains(key))
			{
				foreach (DictionaryEntry de in this)
					if ((string)de.Key == key)
					{
						DictionaryEntry te = de;
						te.Value = value;
						return;
					}				
			}
			else
				Add(key, value);
		}		
		private void verifyType(object value)
		{
			if (!(value is TagCssStyleType))
				throw new ArgumentException("Invalid Type");
		}
		
		public TagCssStyleDictionary() : base()
		{
			
		}
		
		~TagCssStyleDictionary()
		{
			
		}
		
		public void PrintItems()
		{
			IDictionaryEnumerator em = this.GetEnumerator();						
			while (em.MoveNext())			
				((TagCssStyleType)(em.Value)).cssStyles.PrintItems();			
		}
		
		public string Css()
		{
			string retVal = "";
			IDictionaryEnumerator em = this.GetEnumerator();						
			while (em.MoveNext())			
				retVal += ((TagCssStyleType)(em.Value)).cssStyles.Css();
			return retVal;
		}
		
		public void AddCssStyle(string input)
		{
			ArrayList css = Utils.DecodeCssStyle(input);
			for (Int32 i = 0; i < css.Count; i++)
			{
				CssStyleType cssStyle = (CssStyleType)(css[i]);								
				if (cssStyle.styleTagName != "")
				{
					if (!(Contains(cssStyle.tagName)))
						Add(cssStyle.tagName, new TagCssStyleType(cssStyle.tagName));											
					this[cssStyle.tagName].AddCssStyle(cssStyle);						
				}
			}			
		}
		
		public TagCssStyleType this[string input]
		{
			get
			{
				return getCssTagStyle(input);
			}
			set
			{
				setCssTagStyle(input, value);
			}			
		}
		
		public TagCssStyleType this[Int32 id]
		{
			get
			{
				return getCssTagStyle(id);
			}
		}
		
		public CssStyleList ListAllCssStyle(string tagName)
		{
			CssStyleList retVal = new CssStyleList();
			
			for (Int32 i = 0; i < this[tagName].cssStyles.Count; i++)
				retVal.Add(this[tagName].cssStyles[i]);
			for (Int32 i = 0; i < this[CssHeaderStyleType.UnspecifiedTagName].cssStyles.Count; i++)
				retVal.Add(this[CssHeaderStyleType.UnspecifiedTagName].cssStyles[i]);
			
			return retVal;
		}
		
	}

	
}

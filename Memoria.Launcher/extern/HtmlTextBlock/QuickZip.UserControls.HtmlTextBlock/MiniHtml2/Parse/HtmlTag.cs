/*
 * Created by SharpDevelop.
 * User: LYCJ
 * Date: 18/10/2007
 * Time: 21:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace QuickZip.MiniHtml2
{
	/// <summary>
	/// Represent an element Tag in Html code
	/// </summary>
	public class HtmlTag
	{				
        private string name;                                     //HtmlTag name without <>        
        private PropertyList variables = new PropertyList();     //Variable List and values
            
        ///<summary> Gets HtmlTag ID in BuiltInTags. (without <>) </summary>
        internal Int32 ID { get { return Math.Abs(Utils.LocateTag(Utils.RemoveFrontSlash(name))); } }
        ///<summary> Gets HtmlTag Level in BuiltInTags. (without <>) </summary>
        internal Int32 Level { get { if (ID == -1) return 0; else return Defines.BuiltinTags[ID].tagLevel; } }
        
        internal bool IsEndTag { get {  return ((name.IndexOf('/') == 0) ||(variables.Contains('/'))); } }
        
        ///<summary> Gets HtmlTag name. (without <>) </summary>
        public string Name { get { return name; } }
        ///<summary> Gets variable value. </summary>
        public string this[string key] { get { return variables[key].value; } }        
        ///<summary> Gets whether variable list contains the specified key. </summary>
        public bool Contains(string key) { return variables.Contains(key); }
        ///<summary> Returns the string representation of the value of this instance.  </summary>
		public override string ToString()
		{
			return String.Format("<{0}> : {1}", name, variables.ToString());
		}
        
        /// <summary>
        /// Initialite procedure, can be used by child tags.
        /// </summary>
        protected void init(string aName, PropertyList aVariables)
        {                        
            name = aName.ToLower();
            if (aVariables == null)
                variables = new PropertyList();
            else
                variables = aVariables;            
        }
        
		///<summary> Constructor. </summary>
		public HtmlTag(string aName, string aVarString)
		{
			init(aName, PropertyList.FromString(aVarString));
		}
		
		public HtmlTag(string aText)
		{
			PropertyList aList = new PropertyList();
			aList.Add("value", aText);
			init("text", aList);
		}
	}		
		
}

/*
 * Created by SharpDevelop.
 * User: LYCJ
 * Date: 19/10/2007
 * Time: 3:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.Text;

namespace QuickZip.MiniHtml2
{
	/// <summary>
    /// MiniHtml internal Html Paraser, used since D7 version of TQzHtmlLabel2,
    /// not too efficient as it does a lot of string swapping.
    /// </summary>
    public class HtmlParser
    {        
        private HtmlTagTree tree;
    	internal HtmlTagNode previousNode = null;
        
        /// <summary>
        /// Constructor
        /// </summary>        
        public HtmlParser(HtmlTagTree aTree)
        {
        	tree = aTree;
        }
        	        
        /// <summary> Return true if both < and > found in input. </summary>        
        private bool haveClosingTag(string input)
        {
            if ((input.IndexOf('[') != -1) && (input.IndexOf(']') != -1))
                return false;
            return true;
        }
        /// <summary> Add a Non TextTag to Tag List </summary>        
        internal void addTag(HtmlTag aTag)
        {
//            HtmlTagNode newNode = new HtmlTagNode(
        	if (previousNode == null) { previousNode = tree; }
            
        	while (!previousNode.CanAdd(aTag))
        		previousNode = previousNode.Parent;
        	        	        	
        	previousNode = previousNode.Add(aTag);
        }
        /// <summary>
        /// Parse a string and return text before a tag, the tag and it's variables, and the string after that tag.
        /// </summary>
        private static void readNextTag(string s, ref string beforeTag, ref string afterTag, ref string tagName, 
                                          ref string tagVars, char startBracket, char endBracket)
        {
            Int32 pos1 = s.IndexOf(startBracket);
            Int32 pos2 = s.IndexOf(endBracket);

            if ((pos1 == -1) || (pos2 == -1) || (pos2 < pos1))
            {
                tagVars = "";
                beforeTag = s;
                afterTag = "";
            }
            else
            {
                String tagStr = s.Substring(pos1 + 1, pos2 - pos1 - 1);
                beforeTag = s.Substring(0, pos1);
                afterTag = s.Substring(pos2 + 1, s.Length - pos2 - 1);

                Int32 pos3 = tagStr.IndexOf(' ');
                if ((pos3 != -1) && (tagStr != ""))
                {
                    tagName = tagStr.Substring(0, pos3);
                    tagVars = tagStr.Substring(pos3+1, tagStr.Length-pos3-1);
                }
                else
                {
                    tagName = tagStr;
                    tagVars = "";
                }

                if (tagName.StartsWith("!--"))
                {
                    if ((tagName.Length < 6) || (!(tagName.EndsWith("--"))))
                    {
                        Int32 pos4 = afterTag.IndexOf("-->");
                        if (pos4 != -1)
                            afterTag = afterTag.Substring(pos4 + 2, afterTag.Length-pos4-1);
                    }
                    tagName = "";
                    tagVars = "";
                }

            }    
        }     
        /// <summary>
        /// Parse a string and return text before a tag, the tag and it's variables, and the string after that tag.
        /// </summary>
       	private static void readNextTag(string s, ref string beforeTag, ref string afterTag, ref string tagName, ref string tagVars)
       	{
       		HtmlParser.readNextTag(s, ref beforeTag, ref afterTag, ref tagName, ref tagVars, '[',']');
       	}
        /// <summary>
        /// Recrusive paraser.
        /// </summary>        
        private void parseHtml(ref string s)
        {
            string beforeTag="", afterTag="", tagName="", tagVar="";
            readNextTag(s, ref beforeTag, ref afterTag, ref tagName, ref tagVar);
            
            if (beforeTag != "")
            	addTag(new HtmlTag(beforeTag));   		//Text
            if (tagName != "")
            	addTag(new HtmlTag(tagName, tagVar));
            
            s = afterTag;
        }
        /// <summary>
        /// Parse Html
        /// </summary>        
        public void Parse(TextReader reader)
        {        	
        	previousNode = null;

            string input = reader.ReadToEnd();

            while (input != "")
                parseHtml(ref input);                
        }

//        public static void DebugUnit()
//        {
//            //string beforeTag="", afterTag="", tagName="", tagVar="";
//            //readNextTag("<!-- xyz --><a href=\"xyz\"><b>", ref beforeTag, ref afterTag, ref tagName, ref tagVar);
//            //readNextTag(afterTag, ref beforeTag, ref afterTag, ref tagName, ref tagVar);
//            //Console.WriteLine(beforeTag);
//            //Console.WriteLine(afterTag);
//            //Console.WriteLine(tagName);
//            //Console.WriteLine(tagVar);
//            string Html = "<b>test</b>";
////            
////            mh.parser.Parse((new StringReader(Html)));
////            mh.masterTag.childTags.PrintItems();
//        }
    }
}

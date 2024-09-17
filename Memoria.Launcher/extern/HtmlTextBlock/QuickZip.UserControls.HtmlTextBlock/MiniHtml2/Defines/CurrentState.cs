using System;
using System.Text;
//using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Windows.Media;

namespace QuickZip.MiniHtml2
{

    public class CurrentStateType
    {
        private List<HtmlTag> activeStyle = new List<HtmlTag>();
        private bool bold;
        private bool italic;
        private bool underline;
        private bool subscript;
        private bool superscript;
        private string hyperlink = null;
        private Color? foreground;
        private string font = null;
        private double? fontSize;


        public bool Bold { get { return bold; } }
        public bool Italic { get { return italic; } }
        public bool Underline { get { return underline; } }
        public bool SubScript { get { return subscript; } }
        public bool SuperScript { get { return superscript; } }
        public string HyperLink { get { return hyperlink; } }
        public Color? Foreground { get { return foreground; } }
        public string Font { get { return font; } }
        public double? FontSize { get { return fontSize; } }

        public void UpdateStyle(HtmlTag aTag)
        {
            if (!aTag.IsEndTag)
                activeStyle.Add(aTag);
            else
                for (int i = activeStyle.Count - 1; i >= 0; i--)
                    if ('/' + activeStyle[i].Name == aTag.Name)
                    {
                        activeStyle.RemoveAt(i);
                        break;
                    }
            updateStyle();
        }


        private void updateStyle()
        {
            bold = false;
            italic = false;
            underline = false;
            subscript = false;
            superscript = false;
            foreground = null;
            font = null;
            hyperlink = "";
            fontSize = null;

            foreach (HtmlTag aTag in activeStyle)
                switch (aTag.Name)
                {
                    case "b": bold = true; break;
                    case "i": italic = true; break;
                    case "u": underline = true; break;
                    case "sub": subscript = true; break;
                    case "sup": superscript = true; break;
                    case "a": if (aTag.Contains("href")) hyperlink = aTag["href"]; break;
                    case "font" :
                        if (aTag.Contains("color"))
                            try { foreground = (Color)ColorConverter.ConvertFromString(aTag["color"]); }
                            catch { foreground = Colors.Black; }
                        if (aTag.Contains("face"))
                            font = aTag["face"];
                        if (aTag.Contains("size"))
                            try { fontSize= Double.Parse(aTag["size"]); }
                            catch { };
                        break;
                }
        }

        public CurrentStateType()
        {

        }



    }
	
}

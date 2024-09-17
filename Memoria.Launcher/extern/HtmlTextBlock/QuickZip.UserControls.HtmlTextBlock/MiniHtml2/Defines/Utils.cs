using System;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using QuickZip.MiniCss;
//using QuickZip.MiniHtml.Controls;

namespace QuickZip.MiniHtml2
{
    
    public class Utils
    {
        // Text based conversion and replacement routines //
        #region Char Type detection
        public static bool isNumber(char c) { return Char.IsNumber(c); }
        public static bool isAlpha(char c) { return Char.IsLetter(c); }
        public static bool iaSymbol(char c) { return !((isNumber(c)) && (isAlpha(c))); }
        public static elementType charType(char c)
        {
            if ((Utils.isNumber(c)) || (Utils.isAlpha(c))) { return elementType.eText; }
            else
                if (c == ' ') { return elementType.eSpace; }
                else
                    if (c == '#') { return elementType.eClass; }
                    else
                        if (c == '.') { return elementType.eId; }
                        else
                            if (c == ':') { return elementType.eStyle; }
                            else
                                if (c == '-') { return elementType.eDash; }
                                else return elementType.eSymbol;
        }
        #endregion
        #region Replacement routines
        /// <summary>
        /// Replace a char in text
        /// </summary>
        public static String Replace(string input, char fromChar, char toChar)
        {
            return input.Replace(fromChar, toChar);
        }
        /// <summary>
        /// Pop out string before the char looking for.
        /// </summary>
        public static String ExtractBefore(ref string input, char lookFor)
        {
            Int32 pos = input.IndexOf(lookFor);
            String retVal = "";
            if (pos == -1)
            { retVal = input; input = ""; }
            else
            {
                retVal = input.Substring(0, pos);
                input = input.Substring(pos+1, input.Length - pos -1);
            }

            return retVal;
        }
        /// <summary>
        /// Pop out string after the char looking for.
        /// </summary>
        public static String ExtractAfter(ref string input, char lookFor)
        {
            Int32 pos = input.IndexOf(lookFor);
            String retVal = "";
            if (pos == -1)
            {
                retVal = input;
                input = "";
            }
            else
            {
                retVal = input.Substring(pos + 1, input.Length - pos - 1);
                input = input.Substring(0, pos);
            }

            return retVal;
        }
        /// <summary>
        /// Pop out string between the startChar and endChar.
        /// </summary>        
        public static String ExtractBetween(string input, char startChar, char endChar)
        {
            string retVal = ExtractAfter(ref input, startChar);
            return ExtractBefore(ref retVal, endChar);
        }
        /// <summary>
        /// Pop out string after the char looking for(Seperator).
        /// </summary>
        public static String ExtractNextItem(ref string input, char seperator)
        {
            string retVal = ExtractBefore(ref input, seperator);
            if (retVal == "")
            {
                retVal = input;
                input = "";
            }
            input = input.Trim(seperator);
            return retVal;
        }
        /// <summary>
        /// Turn a string (e.g. Commatext) to a List.
        /// </summary>        
        public static ArrayList ExtractList(string input, char seperator)
        {
            ArrayList retVal = new ArrayList();
            string k = input;
            string newItem = "";
            while (k != "")
            {
                newItem = ExtractNextItem(ref k, seperator);
                if (newItem.IndexOf("'") != -1) { newItem = ExtractBetween(newItem, "'"[0], "'"[0]); }
                if (newItem.IndexOf('"') != -1) { newItem = ExtractBetween(newItem, '"', '"'); }
                if (newItem != "") { retVal.Add(newItem); }
            }
            return retVal;
        }
        /// <summary>
        /// Remove slash (\) in front of a strinig.
        /// </summary>        
        public static string RemoveFrontSlash(string input)
        {
            if ((input.Length > 0) && ((input[0] == '/') || (input[0] == '\\')))
            { return input.Substring(1); }
            else
            { return input; }
        }
        /// <summary>
        /// First letter to uppercase, the rest lowercase
        /// </summary>
        public static string Capitalize(string input)
        {
            string retVal = input;
            if (retVal.Length > 1)
            { return Char.ToUpper(retVal[0]) + retVal.Substring(1); }
            else
                if (retVal.Length == 1) { return retVal.ToUpper(); }
                else
                { return ""; }
        }
        /// <summary>
        /// Create hash string from input
        /// </summary>        
        public static string SimpleHash(string input)
        {
            Int32 a = (Int32)('a');
            string retVal = "";
            foreach (char c in input)
                retVal += (char)(((Int32)(c) % 25) + a);
            return retVal;
        }
        /// <summary>
        /// Add a slash "\" to end of input if not exists
        /// </summary>        
        public static string AppendSlash(string input)
        {
            if (input.EndsWith(@"\")) { return input; }
            else
            { return input + @"\"; }
        }
        /// <summary>
        /// Remove slash end of input
        /// </summary>        
        public static string RemoveSlash(string input)
        {
            if (input.EndsWith(@"\")) { return input.Substring(0, input.Length - 1); }
            else
            { return input; }
        }
        /// <summary>
        /// Transfer text case based on transformType
        /// </summary>
        public static string TransformText(string input, textTransformType transformType)
        {
            switch (transformType)
            {
                case textTransformType.Capitalize :                    
                    return input.Substring(0,1).ToUpper() + input.Substring(1);
                case textTransformType.Lowercase :
                    return input.ToLower();
                case textTransformType.None :
                    return input;
                case textTransformType.Uppercase :
                    return input.ToUpper();
            }
            return input;
        }

        static char quote = '\'';
        private static void locateNextVariable(ref string working, ref string varName, ref string varValue)
        {            
            working = working.Trim();
            
            Int32 pos1 = working.IndexOf('=');
            if (pos1 != -1)
            {
                varName = working.Substring(0, pos1);
                Int32 j = working.IndexOf(quote);
                Int32 f1 = working.IndexOf(' ');
                Int32 f2 = working.IndexOf('=');
                if (f1 == -1) { f1 = f2 + 1; }

                if ((j == -1) || (j > f1))
                {
                    varValue = working.Substring(f2 + 1, working.Length - f2 - 1);
                    f1 = working.IndexOf(' ');
                    if (f1 == -1)
                    {
                        working = "";
                    }
                    else
                    {
                        working = working.Substring(f1 + 1, working.Length - f1 - 1);
                    }

                }
                else
                {
                    working = working.Substring(j + 1, working.Length - j - 1);
                    j = working.IndexOf(quote);
                    if (j != -1)
                    {
                        varValue = working.Substring(0, j);
                        working = working.Substring(j + 1, working.Length - j - 1);
                    }
                }
            }
            else
            {
                varName = working;
                varValue = "TRUE";
                working = "";
            }

        }
        /// <summary>
        /// Extract html tag variables (e.g. href="xyz" name="abc")
        /// </summary>
        public static PropertyList ExtravtVariables(string input)
        {
            PropertyList retVal = new PropertyList();
            string working = input;
            string varName = "", varValue = "";
            while (working != "")
            {
                locateNextVariable(ref working, ref varName, ref varValue);
                retVal.Add(varName, varValue);
            }
            return retVal;
        }
        /// <summary>
       /// Extract CSS tag id (e.g. hl em {})
       /// </summary>       
       public static String ExtractNextElement(ref string input, ref elementType element)
       {
           input = input.Trim();
           if (input == "") { return ""; }
           string retVal = "";

           elementType e = charType(input[0]);
           element = e;
           char nextChar = input[0];


           while (input != "")
           {
               retVal += nextChar;
               input = input.Substring(1);
               string temp = input.Trim();
               if ((temp != "") && (charType(temp[0]) == elementType.eSymbol))
               {
                   input = temp.Substring(1);
                   return retVal + temp[0];
               }
               switch (e)
               {
                   case elementType.eSymbol:
                       return retVal;
                   case elementType.eDash:
                       retVal += ExtractNextElement(ref input, ref element);
                       break;
                   case elementType.eStyle:
                       retVal += ExtractNextElement(ref input, ref element);
                       break;
                   case elementType.eClass:
                       retVal += ExtractNextElement(ref input, ref element);
                       break;
                   case elementType.eId:
                       retVal += ExtractNextElement(ref input, ref element);
                       break;
               }

               if (input == "") { return retVal; }
               else
               {
                   e = charType(input[0]);
                   nextChar = input[0];
                   if (e == elementType.eSpace) { return retVal; }
               }

           }
           return retVal;

       }
        #endregion
        #region Symbol related routines
        /// <summary>
        /// Return a html that list all symbol.
        /// </summary>
        public static String SymbolHtml()
        {
            String retValue = "";
            foreach (SymbolInfo si in Defines.BuiltinSymbols)
            {
                retValue += '&' + si.symbol + ';';
            }
            return retValue;
        }
        /// <summary>
        /// Decode a html symbol 
        /// </summary>        
        public static Int32 LocateSymbol(string input)
        {
            string k = ExtractBetween(input, '&', ';');
            if (k.Length > 1)
            {
                foreach (SymbolInfo symb in Defines.BuiltinSymbols)
                {
                    Int32 symbolNumber = -1;
                    
                    try
                    {   symbolNumber = Convert.ToInt32(k);}
                    catch
                    {
                        symbolNumber = -1;
                    }

                    if ((symb.symbol.Equals(k)) || (symb.code == symbolNumber))
                    {
                        return symb.code;
                    }
                }
            }
            return -1;
        }
        /// <summary>
        /// Decode all html symbol in text (e.g. &amp;) to actual text.
        /// </summary>
        public static string DecodeSymbol(string input)
        {
            Int32 Idx1 = input.IndexOf('&');
            Int32 Idx2 = 0;
            string retVal = input;
            if (input != "") { Idx2 = input.IndexOf(';', Idx1 + 1); }
            if ((Idx1 != -1) && (Idx2 > Idx1))
            {
                string text = ExtractBefore(ref input, '&');
                string symb = ExtractBefore(ref input, ';');

                if ((symb.ToLower() == "amp") || (symb == "0038"))
                { text = text + "&"; }
                else
                {
                    Int32 symbIndex = LocateSymbol('&' + symb + ';');
                    if (symbIndex != -1)
                    { text = text + (char)(symbIndex); }
                }
                retVal = text + input;               
            }

            if (retVal == input) { return input; }
            else
            { return DecodeSymbol(retVal); }
            
        }

        #endregion        
        #region Record Type Conversion routines

        /// <summary>
        /// Convert Text Color(Blue) to System.Drawing.Color
        /// </summary>
        /// <param name="colorString"></param>
        /// <returns></returns>
        public static Color String2Color(string colorString) {
          #region String2ColorList
          if (colorString.ToLower().Equals("aliceblue")    ) {return Color.AliceBlue ;} else
          if (colorString.ToLower().Equals("antiquewhite") ) {return Color.AntiqueWhite ;} else
          if (colorString.ToLower().Equals("aqua")         ) {return Color.Aqua  ;} else
          if (colorString.ToLower().Equals("aquamarine")   ) {return Color.Aquamarine ;} else
          if (colorString.ToLower().Equals("azure")        ) {return Color.Azure ;} else
          if (colorString.ToLower().Equals("beige")        ) {return Color.Beige ;} else
          if (colorString.ToLower().Equals("bisque")       ) {return Color.Bisque ;} else
          if (colorString.ToLower().Equals("black")        ) {return Color.Black ;} else
          if (colorString.ToLower().Equals("blanchedalmond") ) {return Color.BlanchedAlmond ;} else
          if (colorString.ToLower().Equals("blue")         ) {return Color.Blue ;} else
          if (colorString.ToLower().Equals("blueviolet")   ) {return Color.BlueViolet ;} else
          if (colorString.ToLower().Equals("brown")        ) {return Color.Brown ;} else
          if (colorString.ToLower().Equals("burlywood")    ) {return Color.BurlyWood ;} else
          if (colorString.ToLower().Equals("cadetblue")    ) {return Color.CadetBlue ;} else
          if (colorString.ToLower().Equals("chartreuse")   ) {return Color.Chartreuse ;} else
          if (colorString.ToLower().Equals("chocolate")    ) {return Color.Chocolate ;} else
          if (colorString.ToLower().Equals("coral")        ) {return Color.Coral ;} else
          if (colorString.ToLower().Equals("cornflowerblue") ) {return Color.CornflowerBlue ;} else
          if (colorString.ToLower().Equals("cornsilk")     ) {return Color.Cornsilk ;} else
          if (colorString.ToLower().Equals("crimson")      ) {return Color.Crimson ;} else
          if (colorString.ToLower().Equals("cyan")         ) {return Color.Cyan ;} else
          if (colorString.ToLower().Equals("darkblue")     ) {return Color.DarkBlue ;} else
          if (colorString.ToLower().Equals("darkcyan")     ) {return Color.DarkCyan ;} else
          if (colorString.ToLower().Equals("darkgoldenrod")) {return Color.DarkGoldenrod ;} else
          if (colorString.ToLower().Equals("darkgray")     ) {return Color.DarkGray ;} else
          if (colorString.ToLower().Equals("darkgreen")    ) {return Color.DarkGreen ;} else
          if (colorString.ToLower().Equals("darkkhaki")    ) {return Color.DarkKhaki ;} else
          if (colorString.ToLower().Equals("darkmagenta")  ) {return Color.DarkMagenta ;} else
          if (colorString.ToLower().Equals("darkolivegreen") ) {return Color.DarkOliveGreen ;} else
          if (colorString.ToLower().Equals("darkorange")   ) {return Color.DarkOrange ;} else
          if (colorString.ToLower().Equals("darkorchid")   ) {return Color.DarkOrchid ;} else
          if (colorString.ToLower().Equals("darkred")      ) {return Color.DarkRed ;} else
          if (colorString.ToLower().Equals("darksalmon")   ) {return Color.DarkSalmon ;} else
          if (colorString.ToLower().Equals("darkseagreen") ) {return Color.DarkSeaGreen ;} else
          if (colorString.ToLower().Equals("darkslateblue")) {return Color.DarkSlateBlue ;} else
          if (colorString.ToLower().Equals("darkslategray")) {return Color.DarkSlateGray ;} else
          if (colorString.ToLower().Equals("darkturquoise")) {return Color.DarkTurquoise ;} else
          if (colorString.ToLower().Equals("darkviolet")   ) {return Color.DarkViolet ;} else
          if (colorString.ToLower().Equals("deeppink")     ) {return Color.DeepPink ;} else
          if (colorString.ToLower().Equals("deepskyblue")  ) {return Color.DeepSkyBlue ;} else
          if (colorString.ToLower().Equals("dimgray")      ) {return Color.DimGray ;} else
          if (colorString.ToLower().Equals("dodgerblue")   ) {return Color.DodgerBlue ;} else
          if (colorString.ToLower().Equals("firebrick")    ) {return Color.Firebrick ;} else
          if (colorString.ToLower().Equals("floralwhite")  ) {return Color.FloralWhite ;} else
          if (colorString.ToLower().Equals("forestgreen")  ) {return Color.ForestGreen ;} else
          if (colorString.ToLower().Equals("fuchsia")      ) {return Color.Fuchsia ;} else
          if (colorString.ToLower().Equals("gainsboro")    ) {return Color.Gainsboro ;} else
          if (colorString.ToLower().Equals("ghostwhite")   ) {return Color.GhostWhite ;} else
          if (colorString.ToLower().Equals("gold")         ) {return Color.Gold ;} else
          if (colorString.ToLower().Equals("goldenrod")    ) {return Color.Goldenrod ;} else
          if (colorString.ToLower().Equals("gray")         ) {return Color.Gray ;} else
          if (colorString.ToLower().Equals("green")        ) {return Color.Green ;} else
          if (colorString.ToLower().Equals("greenyellow")  ) {return Color.GreenYellow ;} else
          if (colorString.ToLower().Equals("honeydew")     ) {return Color.Honeydew ;} else
          if (colorString.ToLower().Equals("hotpink")      ) {return Color.HotPink ;} else
          if (colorString.ToLower().Equals("indianred")    ) {return Color.IndianRed ;} else
          if (colorString.ToLower().Equals("indigo")       ) {return Color.Indigo ;} else
          if (colorString.ToLower().Equals("ivory")        ) {return Color.Ivory ;} else
          if (colorString.ToLower().Equals("khaki")        ) {return Color.Khaki ;} else
          if (colorString.ToLower().Equals("lavender")     ) {return Color.Lavender ;} else
          if (colorString.ToLower().Equals("lavenderblush")) {return Color.LavenderBlush ;} else
          if (colorString.ToLower().Equals("lawngreen")    ) {return Color.LawnGreen ;} else
          if (colorString.ToLower().Equals("lemonchiffon") ) {return Color.LemonChiffon ;} else
          if (colorString.ToLower().Equals("lightblue")    ) {return Color.LightBlue ;} else
          if (colorString.ToLower().Equals("lightcoral")   ) {return Color.LightCoral ;} else
          if (colorString.ToLower().Equals("lightcyan")    ) {return Color.LightCyan ;} else
          if (colorString.ToLower().Equals("lightgoldenrodyellow") ) {return Color.LightGoldenrodYellow ;} else
          if (colorString.ToLower().Equals("lightgray")    ) {return Color.LightGray ;} else
          if (colorString.ToLower().Equals("lightgreen")   ) {return Color.LightGreen ;} else
          if (colorString.ToLower().Equals("lightpink")    ) {return Color.LightPink ;} else
          if (colorString.ToLower().Equals("lightsalmon")  ) {return Color.LightSalmon ;} else
          if (colorString.ToLower().Equals("lightseagreen")) {return Color.LightSeaGreen ;} else
          if (colorString.ToLower().Equals("lightskyblue") ) {return Color.LightSkyBlue ;} else
          if (colorString.ToLower().Equals("lightslategray") ) {return Color.LightSlateGray ;} else
          if (colorString.ToLower().Equals("lightsteelblue") ) {return Color.LightSteelBlue ;} else
          if (colorString.ToLower().Equals("lightyellow")  ) {return Color.LightYellow ;} else
          if (colorString.ToLower().Equals("lime")         ) {return Color.Lime ;} else
          if (colorString.ToLower().Equals("limegreen")    ) {return Color.LimeGreen ;} else
          if (colorString.ToLower().Equals("linen")        ) {return Color.Linen ;} else
          if (colorString.ToLower().Equals("magenta")      ) {return Color.Magenta ;} else
          if (colorString.ToLower().Equals("maroon")       ) {return Color.Maroon ;} else
          if (colorString.ToLower().Equals("mediumaquamarine") ) {return Color.MediumAquamarine ;} else
          if (colorString.ToLower().Equals("mediumblue")   ) {return Color.MediumBlue ;} else
          if (colorString.ToLower().Equals("mediumorchid") ) {return Color.MediumOrchid ;} else
          if (colorString.ToLower().Equals("mediumpurple") ) {return Color.MediumPurple ;} else
          if (colorString.ToLower().Equals("mediumseagreen")   ) {return Color.MediumSeaGreen ;} else
          if (colorString.ToLower().Equals("mediumslateblue")  ) {return Color.MediumSlateBlue ;} else
          if (colorString.ToLower().Equals("mediumspringgreen")) {return Color.MediumSpringGreen ;} else
          if (colorString.ToLower().Equals("mediumturquoise")  ) {return Color.MediumTurquoise ;} else
          if (colorString.ToLower().Equals("mediumvioletred")  ) {return Color.MediumVioletRed ;} else
          if (colorString.ToLower().Equals("midnightblue") ) {return Color.MidnightBlue ;} else
          if (colorString.ToLower().Equals("mintcream")    ) {return Color.MintCream ;} else
          if (colorString.ToLower().Equals("mistyrose")    ) {return Color.MistyRose ;} else
          if (colorString.ToLower().Equals("moccasin")     ) {return Color.Moccasin ;} else
          if (colorString.ToLower().Equals("navajowhite")  ) {return Color.NavajoWhite ;} else
          if (colorString.ToLower().Equals("navy")         ) {return Color.Navy ;} else
          if (colorString.ToLower().Equals("oldlace")      ) {return Color.OldLace ;} else
          if (colorString.ToLower().Equals("olive")        ) {return Color.Olive ;} else
          if (colorString.ToLower().Equals("olivedrab")    ) {return Color.OliveDrab ;} else
          if (colorString.ToLower().Equals("orange")       ) {return Color.Orange ;} else
          if (colorString.ToLower().Equals("orangered")    ) {return Color.OrangeRed ;} else
          if (colorString.ToLower().Equals("orchid")       ) {return Color.Orchid ;} else
          if (colorString.ToLower().Equals("palegoldenrod")) {return Color.PaleGoldenrod ;} else
          if (colorString.ToLower().Equals("palegreen")    ) {return Color.PaleGreen ;} else
          if (colorString.ToLower().Equals("paleturquoise")) {return Color.PaleTurquoise ;} else
          if (colorString.ToLower().Equals("palevioletred")) {return Color.PaleVioletRed ;} else
          if (colorString.ToLower().Equals("papayawhip")   ) {return Color.PapayaWhip ;} else
          if (colorString.ToLower().Equals("peachpuff")    ) {return Color.PeachPuff ;} else
          if (colorString.ToLower().Equals("peru")         ) {return Color.Peru ;} else
          if (colorString.ToLower().Equals("pink")         ) {return Color.Pink ;} else
          if (colorString.ToLower().Equals("plum")         ) {return Color.Plum ;} else
          if (colorString.ToLower().Equals("powderblue") ) {return Color.PowderBlue ;} else
          if (colorString.ToLower().Equals("purple")       ) {return Color.Purple ;} else
          if (colorString.ToLower().Equals("red")          ) {return Color.Red ;} else
          if (colorString.ToLower().Equals("rosybrown")    ) {return Color.RosyBrown ;} else
          if (colorString.ToLower().Equals("royalblue")    ) {return Color.RoyalBlue ;} else
          if (colorString.ToLower().Equals("saddlebrown")  ) {return Color.SaddleBrown ;} else
          if (colorString.ToLower().Equals("salmon")       ) {return Color.Salmon ;} else
          if (colorString.ToLower().Equals("sandybrown")   ) {return Color.SandyBrown ;} else
          if (colorString.ToLower().Equals("seagreen")     ) {return Color.SeaGreen ;} else
          if (colorString.ToLower().Equals("seashell")     ) {return Color.SeaShell ;} else
          if (colorString.ToLower().Equals("sienna")       ) {return Color.Sienna ;} else
          if (colorString.ToLower().Equals("silver")       ) {return Color.Silver ;} else
          if (colorString.ToLower().Equals("skyblue")      ) {return Color.SkyBlue ;} else
          if (colorString.ToLower().Equals("slateblue")    ) {return Color.SlateBlue ;} else
          if (colorString.ToLower().Equals("slategray")    ) {return Color.SlateGray ;} else
          if (colorString.ToLower().Equals("snow")         ) {return Color.Snow ;} else
          if (colorString.ToLower().Equals("springgreen")  ) {return Color.SpringGreen ;} else
          if (colorString.ToLower().Equals("steelblue")    ) {return Color.SteelBlue ;} else
          if (colorString.ToLower().Equals("tan")          ) {return Color.Tan ;} else
          if (colorString.ToLower().Equals("teal")         ) {return Color.Teal ;} else
          if (colorString.ToLower().Equals("thistle")      ) {return Color.Thistle ;} else
          if (colorString.ToLower().Equals("tomato")       ) {return Color.Tomato ;} else
          if (colorString.ToLower().Equals("transparent")  ) {return Color.Transparent ;} else
          if (colorString.ToLower().Equals("turquoise")    ) {return Color.Turquoise ;} else
          if (colorString.ToLower().Equals("violet")       ) {return Color.Violet ;} else
          if (colorString.ToLower().Equals("wheat")        ) {return Color.Wheat ;} else
          if (colorString.ToLower().Equals("white")        ) {return Color.White ;} else
          if (colorString.ToLower().Equals("whitesmoke")   ) {return Color.WhiteSmoke ;} else
          if (colorString.ToLower().Equals("yellow")       ) {return Color.Yellow ;} else
          if (colorString.ToLower().Equals("yellowgreen")  ) {return Color.YellowGreen ;} else
          return Color.Black;
          #endregion
        }
        /// <summary>
        /// Convert Html Color(#FFFFFF) or Text Color(Blue) to System.Drawing.Color
        /// </summary>
        /// <param name="colorString"></param>
        /// <returns></returns>
        public static Color WebColor2Color(string colorString) {
            try
            {
                if ((colorString == "") ||
                    (colorString.IndexOf("<") > -1) ||
                    (colorString.IndexOf(">") > -1))
                    return Color.Black;
                if (colorString[0] == '#')
                {
                    Int32 red = Convert.ToInt32(colorString.Substring(1, 2), 16);
                    Int32 green = Convert.ToInt32(colorString.Substring(3, 2), 16);
                    Int32 blue = Convert.ToInt32(colorString.Substring(5, 2), 16);
                    return Color.FromArgb(red, green, blue);
                }
                return String2Color(colorString);
            }
            catch
            {
                return Color.Transparent;
            }
        
        }
        /// <summary>
        /// Convert Html Align (left, right, centre) to AlignType
        /// </summary>
        /// <param name="alignString"></param>
        /// <returns></returns>
        public static hAlignType StrAlign2Align(string alignString) {
            if (alignString.ToLower().Equals("left")) { return hAlignType.Left; }
            else
                if (alignString.ToLower().Equals("right")) { return hAlignType.Right; }
                else
                    if (alignString.ToLower().Equals("centre")) { return hAlignType.Centre; }
                    else
                        return hAlignType.Left;

        }
        /// <summary>
        /// Multipurpose pixel converter to integer, support (%, em, px),
        /// not support (points,cm,mm,picas) yet.
        /// </summary>
        /// <param name="sizeString"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static Int32 StrSize2PixelSize(string sizeString, Int32 def)
        {            
            bool plusSign = false;
            bool negativeSign = false;
            string sizeStr = sizeString.Trim();
            if (sizeStr.Length == 0) { return 0; }
            if ((sizeStr[0] == '+') || (sizeStr[0] == '-'))
            {
                if (sizeStr[0] == '+') { plusSign = true; } else { negativeSign = true; }
                sizeStr = sizeStr.Substring(1);
            }
            Int32 intPos = 0;
            while ((sizeStr.Length - 1 > intPos) && ((Utils.isNumber(sizeStr[intPos + 1])) ||
                    (sizeStr[intPos+1] == '.'))) { intPos++; }
            
            Single number = Convert.ToSingle(sizeStr.Substring(0,intPos+1));
            string symbol = sizeStr.Substring(intPos+1,sizeStr.Length-intPos-1);
            if (Utils.isNumber(sizeStr[intPos]))
            {                 
                if (symbol.ToLower().Equals("%")) {number = def * number / 100; } else
                    if (symbol.ToLower().Equals("em")) {number = def * number / 100; } else
                        if (symbol.ToLower().Equals("px")) { } else
                            //Below not supported
                            if (symbol.ToLower().Equals("points")) {number = def; } else
                                if (symbol.ToLower().Equals("cm")) {number = def; } else
                                    if (symbol.ToLower().Equals("mm")) {number = def; } else
                                        if (symbol.ToLower().Equals("picas")) {number = def; } 	
            }
            if (plusSign) {return Convert.ToInt32(def + number); } else
                if (negativeSign) {return Convert.ToInt32(def - number);} else
                    return Convert.ToInt32(number);

        }
        /// <summary>
        /// Convert Css position type (relative,absolute,fixed,inherited to record type.
        /// </summary>
        /// <param name="positionString"></param>
        /// <returns></returns>
        public static positionStyleType StrPosition2PositionType(string positionString)
        {            
            if (positionString.ToLower().Equals("fixed")) {return positionStyleType.Fixed; } 
            else
                if (positionString.ToLower().Equals("inherited")) { return positionStyleType.Inherited; }
                else
                    if (positionString.ToLower().Equals("relative")) { return positionStyleType.Relative; }
                    else
                        if (positionString.ToLower().Equals("static")) { return positionStyleType.Static; }
                        else               
                            return positionStyleType.Absolute;
        }
        /// <summary>
        /// Convert Css Border type(dotted,dashed,solid,double,groove,ridge,
        /// </summary>
        /// <param name="borderString"></param>
        /// <returns></returns>
        public static borderStyleType StrBorder2BorderType(string borderString)
        {
            if (borderString.ToLower().Equals("fixed")) { return borderStyleType.Dashed; }
            else
                if (borderString.ToLower().Equals("dotted")) { return borderStyleType.Dotted; }
                else
                    if (borderString.ToLower().Equals("double")) { return borderStyleType.Double; }
                    else
                        if (borderString.ToLower().Equals("groove")) { return borderStyleType.Groove; }
                        else
                            if (borderString.ToLower().Equals("inherit")) { return borderStyleType.Inherit; }
                            else
                                if (borderString.ToLower().Equals("inset")) { return borderStyleType.Inset; }
                                else
                                    if (borderString.ToLower().Equals("none")) { return borderStyleType.None; }
                                    else
                                        if (borderString.ToLower().Equals("outset")) { return borderStyleType.Outset; }
                                        else
                                            if (borderString.ToLower().Equals("ridge")) { return borderStyleType.Ridge; }
                                            else
                                                if (borderString.ToLower().Equals("solid")) { return borderStyleType.Solid; }
                                                else
                                                    return borderStyleType.None;   
        }
        /// <summary>
        /// Convert Css Bullet type(circle,square,decimal,upper-alpha,lower-alpha,
        ///     upper-roman,lower-roman) to record type.
        /// </summary>
        /// <param name="bulletString"></param>
        /// <returns></returns>
        public static bulletStyleType StrBullet2BulletType(string bulletString)
        {
        	bulletString = bulletString.Replace("-","");
            if (bulletString.ToLower().Equals("circle")) { return bulletStyleType.Circle; }
            else
                if (bulletString.ToLower().Equals("decimal")) { return bulletStyleType.Decimal; }
                else
                    if (bulletString.ToLower().Equals("loweralpha")) { return bulletStyleType.LowerAlpha; }
                    else
                        if (bulletString.ToLower().Equals("lowerroman")) { return bulletStyleType.LowerRoman; }
                        else
                            if (bulletString.ToLower().Equals("none")) { return bulletStyleType.None; }
                            else
                                if (bulletString.ToLower().Equals("square")) { return bulletStyleType.Square; }
                                else
                                    if (bulletString.ToLower().Equals("upperalpha")) { return bulletStyleType.UpperAlpha; }
                                    else
                                        if (bulletString.ToLower().Equals("upperroman")) { return bulletStyleType.UpperRoman; }
                                        else
                                            return bulletStyleType.Circle;      
        }
        /// <summary>
        /// Convert Css Cursor type (default,pointer,crosshair,move,wait,help,text) to
        ///     record type.
        /// </summary>
        /// <param name="cursorString"></param>
        /// <returns></returns>
        public static Cursor StrCursor2CursorType(string cursorString)
        {
            if (cursorString.ToLower().Equals("default")) { return Cursors.Default; }
            else
                if (cursorString.ToLower().Equals("pointer")) { return Cursors.Hand; }
                else
                    if (cursorString.ToLower().Equals("crosshair")) { return Cursors.Cross; }
                    else
                        if (cursorString.ToLower().Equals("move")) { return Cursors.SizeNWSE; }
                        else
                            if (cursorString.ToLower().Equals("wait")) { return Cursors.WaitCursor; }
                            else
                                if (cursorString.ToLower().Equals("help")) { return Cursors.Help; }
                                else
                                    if (cursorString.ToLower().Equals("test")) { return Cursors.IBeam; }
                                    else
                                        return Cursors.Default; 
        }
        /// <summary>
        /// Convert Form Method type to record type.
        /// </summary>
        /// <param name="methodString"></param>
        /// <returns></returns>
        public static formMethodType StrMethod2FormMethodType(string methodString)
        {
            if (methodString.ToLower().Equals("get")) { return formMethodType.Get; }
            else
                if (methodString.ToLower().Equals("post")) { return formMethodType.Post; }
                else
                    return formMethodType.Default;
        }
        /// <summary>
        /// Convert Variable Type to record type (for search)
        /// </summary>
        /// <param name="typeString"></param>
        /// <returns></returns>
        public static variableType StrType2VariableType(string typeString)
        {
            if (typeString.ToLower().Equals("alpha")) { return variableType.Alpha; }
            else
                if (typeString.ToLower().Equals("formated")) { return variableType.Formated; }
                else
                    if (typeString.ToLower().Equals("number")) { return variableType.Number; }
                    else
                        if (typeString.ToLower().Equals("paragraph")) { return variableType.Paragraph; }
                        else
                            return variableType.String;

        }
        public static textTransformType StrTransform2TextTransformType(string transformString)
        {
        	if (transformString.ToLower().Equals("lowercase")) { return textTransformType.Lowercase; }
        	else
        		if (transformString.ToLower().Equals("uppercase")) { return textTransformType.Uppercase; }
        		else
        			if (transformString.ToLower().Equals("capitalize")) { return textTransformType.Capitalize; }
        			else
        				if (transformString.ToLower().Equals("none")) { return textTransformType.None; }
        				else 
        					return textTransformType.None;
        }
        public static textTransformType StrFontVariant2TextTransformType(string variantString)
        {
        	if (variantString.ToLower().Equals("small-caps")) { return textTransformType.Lowercase; }
        	else
        		return textTransformType.None;
        }
        public static string Number2Romans(UInt32 value)
        {
        	StringBuilder retVal = new StringBuilder();
        	Int32 romIndex = 0;
        	while (value > 0)
        	{
        		UInt32 romVal = Defines.BuiltinRomans[romIndex].value;
        		if (value >= romVal)
        		{
        			value -= romVal;
        			retVal.Append(Defines.BuiltinRomans[romIndex].rep);        			
        		}
        		else { romIndex += 1; }
        	}
        	
        	return retVal.ToString();
        }
        
        public static string Number2BulletValue(UInt32 value, bulletStyleType styleType)
        {
        	switch (styleType)
        	{
        		case(bulletStyleType.Decimal):
        			return value.ToString() + ".";
        		case (bulletStyleType.LowerAlpha):
        			Char a = 'a';
        			return (Char)((Int32)(a) + value - 1) + ".";
        		case (bulletStyleType.UpperAlpha):
        			Char A = 'A';
        			return (Char)((Int32)(A) + value - 1) + ".";
        		case (bulletStyleType.LowerRoman):
        			return Number2Romans(value).ToLower() + ".";
        		case (bulletStyleType.UpperRoman):
        			return Number2Romans(value) + ".";
        	}
        	return "";
        }
		#endregion
		#region Record Type Locating routines
		public static Int32 LocateTag(string tagName)
        {
            tagName = tagName.ToLower();
            for (int i = 0; i < Defines.BuiltinTags.Length; i++)
            {
                HTMLTagInfo tagInfo = Defines.BuiltinTags[i];
                if (tagInfo.Html == tagName)
                {
                    return i;
                }
            }
            return -1;
        }
		public static Int32 LocateBBCode(string tagName)
        {
            tagName = tagName.ToLower();
            for (int i = 0; i < Defines.BuiltinBBCodes.Length; i++)
            {
                HTMLTagInfo tagInfo = Defines.BuiltinBBCodes[i];
                if (tagInfo.Html == tagName)
                {
                    return i;
                }
            }
            return -1;
        }
		/// <summary>
        /// Return hash code for specified styleName
        /// </summary>
        public static Int32 LocateStyle(string styleName)
        {
        	styleName = styleName.ToLower();
        	for (Int32 i = 0; i < Defines.BuiltinStyles.Length; i++)
        		if (Defines.BuiltinStyles[i] == styleName)
        			return i;
        	return -1;
        }  
		#endregion
        // Drawing based routines //
        #region Font and Pen related.
        /// <summary>
        /// Create a new Font Object
        /// </summary>
        public static Font CreateFont(string aFontName, Int32 aFontSize, bool isBold, bool isItalic,
            bool isUnderline, bool isStrikeout, bool isURL)
        {
            FontStyle fs = new FontStyle();
            if (isBold) { fs |= FontStyle.Bold; }
            if (isItalic) { fs |= FontStyle.Italic; }
            if ((isUnderline) || (isURL)) { fs |= FontStyle.Underline; }
            if (isStrikeout) { fs |= FontStyle.Strikeout; }

            return new Font(aFontName, aFontSize, fs);            
        }
        /// <summary>
        /// Create Pen object based on color and size.
        /// </summary>
        public static Pen CreatePen(Color aColor, Int32 aSize)
        {
#if CF
            return new Pen(aColor);
#else
            return new Pen(aColor, aSize);
#endif
        }
        /// <summary>
        /// Create Pen object based on brush and size.
        /// </summary>
        public static Pen CreatePen(Brush aBrush, Int32 aSize)
        {
#if CF
            return new Pen(Color.Black);
#else
            return new Pen(aBrush, aSize);
#endif
        }
        /// <summary>
        /// Check if Font exist in system.
        /// </summary>        
        public static bool FontExists(string fontName, Graphics g)
        {
#if CF
            return true;
#else
            if (fontName == "") { return false; }
            FontFamily[] families = FontFamily.GetFamilies(g);
            String fontname = fontName.ToLower();
            foreach (FontFamily family in families)
            {
                if (family.GetName(0).ToLower().Equals(fontname))
                {
                    return true;
                }
            }
            return false;
#endif
        }
        /// <summary>
        /// Check if Font exist in the specified PrivateFontCollection. 
        /// *CF Not supported*
        /// </summary>
#if !CF
        public static bool UserFontExists(string fontName, PrivateFontCollection pfc)
        {
            if (fontName == "") { return false; }
            if (pfc == null) { return false; }

            String fontname = fontName.ToLower();
            foreach (FontFamily family in pfc.Families)
            {
                if (family.GetName(0).ToLower().Equals(fontname))
                {
                    return true;
                }
            }
            return false;
        }
#endif

        /// <summary>
        /// Load font resource to PrivateFontCollection.
        /// *CF Not supported*
        /// </summary>
#if !CF
        public static bool LoadFont(Stream resourceStream, ref PrivateFontCollection pfc)
        {
            try
            {
                int len = (int)resourceStream.Length;
                IntPtr data = Marshal.AllocCoTaskMem(len);
                Byte[] fontData = new Byte[len];
                resourceStream.Read(fontData, 0, len);
                Marshal.Copy(fontData, 0, data, len);
                if (pfc == null) { pfc = new PrivateFontCollection(); }
                pfc.AddMemoryFont(data, len);
                Marshal.FreeCoTaskMem(data);
                return true;
            }
            catch
            {
                return false;
            }
        }
#endif
        /// <summary>
        /// Load font resource to PrivateFontCollection.
        /// </summary>
#if !CF
        public static bool LoadFont(string filename, ref PrivateFontCollection pfc)
        {
            Stream resourceStream = new FileStream(filename, FileMode.Open);
            return LoadFont(resourceStream, ref pfc);
        }
#endif
        /// <summary>
        /// Load font resource to PrivateFontCollection.
        /// Note that you have to embeded a font resource first (e.g. {$R Yourfont.tif})
        /// then Lowd the font (LoadFontFromResource('Yourfont.tif',yourform,miniHtml.userFontCollection)
        /// then you can use the font as usual.
        /// </summary>
#if !CF
        public static bool LoadFont(string resourceName, Form f, ref PrivateFontCollection pfc)
        {
            try
            {
                Stream fontStream = f.GetType().Assembly.GetManifestResourceStream(resourceName);
                int len = (int)fontStream.Length;
                IntPtr data = Marshal.AllocCoTaskMem(len);
                Byte[] fontData = new Byte[len];
                fontStream.Read(fontData, 0, len);
                Marshal.Copy(fontData, 0, data, len);
                if (pfc == null) { pfc = new PrivateFontCollection(); }
                pfc.AddMemoryFont(data, len);
                fontStream.Close();
                Marshal.FreeCoTaskMem(data);
                return true;
            }
            catch
            {
                return false;
            }
        }
#endif

        #endregion
        #region Measure routines
        /// <summary>
        /// Calculate widthLimit/height of a text (used by CF), 
        /// Less precision compared with TextSize.
        /// Better than none....
        /// </summary>
        public static SizeF TextSize2(Graphics g, string aText, Font aFont)
        {
        	if (aText.Trim() == "") return new SizeF(0, 0);
        	if (aText[aText.Length - 1] == ' ') { aText = aText.Substring(0, aText.Length - 1) + '/'; }
            SizeF defSize = g.MeasureString(aText, aFont);
            Int32 precision = (Int32)(defSize.Width / 50);                        
            if (precision == 0) { precision = 1; }            

            Bitmap b = new Bitmap((int)defSize.Width + 1, (int)defSize.Height + 1);
            Graphics tempGraphics = Graphics.FromImage(b);
            try
            {
                tempGraphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, (int)defSize.Width + 1, (int)defSize.Height + 1));
                tempGraphics.DrawString(aText, aFont, new SolidBrush(Color.Black), new PointF(0, 0));
                for (int x = (int)defSize.Width - 1; x > 0; x = x - precision)
                {                    
                    for (int y = (int)defSize.Height - 1; y > 0; y--)
                    {
                        if ((b.GetPixel(x, y).R < 200))
                        {
                            return new SizeF((float)x, defSize.Height);
                        }
                    }
                }
            }
            catch
            {
                return defSize;
            }
            finally
            {
                tempGraphics.Dispose();
                b.Dispose();
            }
            return defSize;
        }
        /// <summary>
        /// Calculate widthLimit/height of a text (
        /// </summary>
        public static SizeF TextSize(Graphics g, string aText, Font aFont)
        {                    	
        	if (aText.Trim() == "") return new SizeF(0, 0);
        	if (aText[aText.Length - 1] == ' ') { aText = aText.Substring(0, aText.Length - 1) + "/"; }
#if CF
            return TextSize2(g, aText, aFont);
#else
            try
            {
                if (aFont.Style == FontStyle.Bold)
                { return TextSize2(g, aText, aFont); } //MeasureCharacterRanges return wrong value if bold.
                
                StringFormat aFormat = StringFormat.GenericTypographic;
                //aFormat.Trimming = System.Drawing.StringTrimming.Character;
                //aFormat.FormatFlags += 16384;
                //aFormat.FormatFlags = aFormat.FormatFlags & StringFormatFlags.NoClip; 

                CharacterRange[] cr = new CharacterRange[1];
                cr[0] = new CharacterRange(0, aText.Length);

                aFormat.SetMeasurableCharacterRanges(cr);

                RectangleF aRect = Screen.PrimaryScreen.Bounds;
                aRect = g.MeasureCharacterRanges(aText, aFont, aRect, aFormat)[0].GetBounds(g);
                return new SizeF(aRect.Right - aRect.Left, aRect.Height);
            }
            catch
            {
                return TextSize2(g, aText, aFont);
            }

#endif
        }
        /// <summary>
        /// Calculate widthLimit/height of a text (
        /// </summary>
        public static SizeF TextSize(string aText, Font aFont)
        {
            Bitmap b = new Bitmap(10, 10);
            Graphics g = Graphics.FromImage(b);
            try
            {
                return TextSize(g, aText, aFont);
            }
            finally
            {
                g.Dispose();
                b.Dispose();
            }
        }
        /// <summary>
        /// Calculate the text position based on x axis.
        /// </summary>
        public static Int32 TextPosition(Graphics g, string aText, Font aFont, float x)
        {
            float fullWidth = TextSize(g, aText, aFont).Width;
            Int32 precision = aText.Length / 5;
            Int32 start = (Int32)(Math.Round(x / fullWidth * aText.Length)) - precision + 1;
            if (start < 1) {start = 1;}
			
            for (int i = start; i <= aText.Length; i++)
            {
                float currentWidth = TextSize(g, aText.Substring(0, i), aFont).Width;
                if (currentWidth > x)
                {
                    if (i == 1) { return 0; }

                    float lastWidth = TextSize(g, aText.Substring(0, i-1), aFont).Width;

                    if ((x - lastWidth) < (currentWidth - x))
                    {
                        return i - 1;
                    }
                    else
                    {
                        return i;
                    }
                }
            }
            return aText.Length;
        }
        /// <summary>
        /// Exact copy of TextPosition except force to use TextSize2, for debug.        
        /// </summary>
        public static Int32 TextPosition2(Graphics g, string aText, Font aFont, float x)
        {
        	if (aText == "") 
        		return 0;
        	
            float fullWidth = TextSize2(g, aText, aFont).Width;
            Int32 precision = aText.Length / 5;
            Int32 start = (Int32)(Math.Round(x / fullWidth * aText.Length)) - precision + 1;
            if (start < 1) { start = 1; }

            for (int i = start; i <= aText.Length; i++)
            {
                float currentWidth = TextSize(g, aText.Substring(0, i), aFont).Width;
                if (currentWidth > x)
                {
                    if (i == 1) { return 0; }

                    float lastWidth = TextSize2(g, aText.Substring(0, i - 1), aFont).Width;

                    if ((x - lastWidth) < (currentWidth - x))
                    {
                        return i - 1;
                    }
                    else
                    {
                        return i;
                    }
                }
            }
            return aText.Length;
        }
        #endregion
        #region Drawing routines
        /// <summary>
        /// Output a circle.
        /// </summary>        
        public static void DrawCircle(Graphics g, Int32 x, Int32 y, Int32 size)
        {
            Point[] pts = new Point[3];
            pts[0] = new Point(x + (size / 2), y + (size / 2));
            pts[1] = new Point(x + size, y);
            pts[2] = new Point(x + size, y + size);
            g.FillEllipse(Brushes.Lime, x, y, (float)size, (float)size);

        }
        /// <summary>
        /// Output a triangle.
        /// </summary>            
        public static void DrawTriangle(Graphics g, Int32 x, Int32 y, Int32 width, Int32 height, bool down)
        {
            SolidBrush sBrush, bBrush;
            if (down)
            {
                sBrush = new SolidBrush(Color.Black);
                bBrush = new SolidBrush(Color.White);
            }
            else
            {
                bBrush = new SolidBrush(Color.Black);
                sBrush = new SolidBrush(Color.White);
            }
            Point[] pts = new Point[3];
            pts[0] = new Point(x + width - 11, y + Convert.ToInt32(((height + 4) / 2)) - 1);
            pts[1] = new Point(x + width - 05, y + Convert.ToInt32(((height + 4) / 2)) - 1);
            pts[2] = new Point(x + width - 08, y + Convert.ToInt32(((height + 4) / 2)) + 2);
            g.DrawPolygon(new Pen(Color.Black), pts);
            g.FillPolygon(sBrush, pts);
        }
        /// <summary>
        /// Graphics.DrawString add some space in front, use Utils.DrawString will offset this problem.
        /// </summary>
        public static void DrawString(Graphics g, String s, Font font, Brush brush, PointF point)
        {
        	float AlterX = font.Size / 4;
            if (font.Style == FontStyle.Bold)
            	 if (point.X - 1 > 0)
            {            	
            	AlterX = 1;                
            }
            g.DrawString(s, font, brush, new PointF(point.X - AlterX, point.Y));
        }    
        /// <summary>
        /// Call Graphics.DrawImage
        /// </summary>
        public static void DrawImage(Graphics g, Image img, PointF point)
        {
        	g.DrawImage(img, point);
        }
        #endregion
        #region Conversion routines
        /// <summary>
        /// Return negative Image
        /// </summary>
        /// <url>http://www.bobpowell.net/negativeimage.htm</url>
        public static Image NegativeImage(Image img)
        {        	        	
			ImageAttributes ia = new ImageAttributes();
			ColorMatrix cm=new ColorMatrix();
			cm.Matrix00 = -1;
			cm.Matrix11 = -1;
			cm.Matrix22 = -1;
//			cm.Matrix00=cm.Matrix11=cm.Matrix22=0.99f;
//      		cm.Matrix33=cm.Matrix44=1;
//      		cm.Matrix40=cm.Matrix41=cm.Matrix42=.04f;
//      		cm.Matrix33=cm.Matrix44=1;
//      		cm.Matrix40=cm.Matrix41=cm.Matrix42=.04f;
			ia.SetColorMatrix(cm);
			
			Bitmap output = new Bitmap(img.Width,img.Height);
			Graphics g=Graphics.FromImage(output);
			g.DrawImage(img,new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
			g.Dispose();

			return output;
        }
        /// <summary>
        /// Resize a Image and return it
        /// </summary>
        /// <urlhttp://www.bobpowell.net/highqualitythumb.htm</url>
        public static Image ResizeImage(Image input, float percentage)
    	{
      		if(percentage<1)
        		throw new Exception("Thumbnail size must be at least 1% of the original size");

      		Bitmap tn=new Bitmap((Int32)(input.Width*0.01f*percentage),(Int32)(input.Height*0.01f*percentage));
      		Graphics g=Graphics.FromImage(tn);
      		g.InterpolationMode=InterpolationMode.HighQualityBilinear; 
      		g.DrawImage(input, new Rectangle(0,0,tn.Width,tn.Height),0,0,input.Width,input.Height,GraphicsUnit.Pixel);
      		g.Dispose();
      		return (Image)tn;      	
    	}
        /// <summary>
        /// Resize a Image and return it
        /// </summary>
        public static Image ResizeImage(Image input, Int32 maxWidth, Int32 maxHeight)
        {
        	float percentageW = (float)maxWidth / (float)input.Width * 100;
        	float percentageH = (float)maxHeight / (float)input.Height * 100;
        	if (percentageW > percentageH)
        		return ResizeImage(input, percentageH); 
        	else
        		return ResizeImage(input, percentageW); 
        }
        /// <summary>
        /// Resize a Image and return it
        /// </summary>
        public static Image ResizeImageW(Image input, Int32 width)
        {
        	float percentage = (float)width / (float)input.Width * 100;
        	return ResizeImage(input, percentage);
        }
        /// <summary>
        /// Resize a Image and return it
        /// </summary>
        public static Image ResizeImageH(Image input, Int32 height)
        {
        	float percentage = (float)height / (float)input.Height * 100;
        	return ResizeImage(input, percentage);
        }
        
        #endregion
        // System utils routines //
        #region System routines
        /// <summary>
        /// Run an external application, Run2 make use of external dll (for use in CE)
        /// </summary>
        public static bool Run2(string prog, string param, ref ProcessInfo pi)
        {
            //Int32 Infinite = Int32.MaxValue;
            //Int32 WAIT_OBJECT_0 = 0;
            
            Byte[] si = new Byte[128];
            if (pi == null) { pi = new ProcessInfo(); }

            Int32 returnCode = Header.CreateProcess(prog, param, IntPtr.Zero,
                IntPtr.Zero, 0, 0, IntPtr.Zero, IntPtr.Zero, si, pi);

            return (returnCode != 0);
        }       
        /// <summary>
        /// Run an external application.
        /// </summary>
        public static void Run(string prog, string param)
        {
#if CF
            ProcessInfo pi = new ProcessInfo();
            Run2(prog, param, ref pi); 
#else
            ProcessStartInfo p = new ProcessStartInfo(prog, param);
            Process.Start(p);
#endif
        }
        /// <summary>
        /// Write a file from a stream 
        /// Can be used to copy files.
        /// </summary>
        public static void Stream2File(Stream inputStream, string outputFilename)
        {
            if (inputStream == null) { return; }
            FileStream fs = new FileStream(outputFilename, FileMode.Create, FileAccess.Write, FileShare.Read);
            try
            {
                Byte[] buffer = new Byte[1024 * 1024];
                Int32 readCnt = inputStream.Read(buffer, 0, buffer.Length);
                while (readCnt > 0)
                {
                    fs.Write(buffer, 0, readCnt);
                    readCnt = inputStream.Read(buffer, 0, buffer.Length);
                }
            }
            finally
            {
                fs.Close();
            }
        }
        /// <summary>
        /// Get a list of filename
        /// </summary>        
        public static ArrayList GetFileList(string path, string mask)
        {
            ArrayList retVal = new ArrayList();
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] fInfoList = dirInfo.GetFiles(mask);
            foreach (FileInfo fi in fInfoList)
            {
                retVal.Add(AppendSlash(path) + fi.Name);
            }
            return retVal;
        }
        #endregion
        #region Filename related routines
        /// <summary>
        /// Return filename from a path
        /// </summary>
        public static string ExtractFileName(string input)
        {
            string fn = RemoveSlash(input);
            Int32 idx = fn.LastIndexOf('\\');
            if (idx == -1) 
            { return fn; }
            else
            {return fn.Substring(idx+1);}
        }
        /// <summary>
        ///  Return file path from a path (with filename)
        /// </summary>
        public static string ExtractFilePath(string input)
        {
            string retVal = RemoveSlash(input);
            Int32 ind = retVal.LastIndexOf('\\');
            if (ind > 0) { retVal = input.Substring(0, ind); }
            return retVal;
        }
        /// <summary>
        /// Return file extension of a path
        /// </summary>
        public static string ExtractFileExt(string input)
        {
            return Path.GetExtension(input);
        }

        #endregion
        // Css related routines //
        #region DecodeCssStyle() routines
        private static string getHeaderSection(string input)
        {
        	return (Utils.ExtractBefore(ref input, '{')).Trim();
        }
        private static string getDeclareSection(string input)
        {
        	return (Utils.ExtractBetween(input, '{', '}')).Trim();
        }
        private static bool readTagName(string input, ref ArrayList cssStyleGroups)
        {
        	string k = Utils.getHeaderSection(input);
        	
        	ArrayList cssTagNameList = new ArrayList();
        	string newTagName = Utils.ExtractBefore(ref k, ',');
        	while (newTagName != "")
        	{
        		cssTagNameList.Add(newTagName);
        		newTagName = Utils.ExtractBefore(ref k, ',');
        	}
        	if (k.Trim() != "") cssTagNameList.Add(k);
        	
        	cssStyleGroups = new ArrayList();
        	
        	for (Int32 i = 0; i < cssTagNameList.Count; i ++)
        	{
        		CssStyleGroupType newGroup = new CssStyleGroupType();
        		k = (string)cssTagNameList[i];
        		
        		elementType e = elementType.eClass;
        		while (k.Trim() != "") 
        			newGroup.parentTagName.Add(Utils.ExtractNextElement(ref k, ref e));
        		
        		if (newGroup.parentTagName.Count > 0)
        		{
        			newGroup.styleTagName = (string)newGroup.parentTagName[newGroup.parentTagName.Count-1];
        			newGroup.parentTagName.RemoveAt(newGroup.parentTagName.Count-1);
        		}
        		
        		cssStyleGroups.Add(newGroup);        	
        	}
        	
        	return true;
        }
        
        private static bool readTagClass(string input, out string tagName, out string tagClass)
        {
        	string k = Utils.getHeaderSection(input);
        	
        	tagName = Utils.ExtractBefore(ref k, '.');
        	tagClass = k;
        	
        	return (tagName != "") && (tagClass != "");
        		
        }
        
        private static bool readTagID(string input, out string tagName, out string tagID)
        {
        	string k = Utils.getHeaderSection(input);
        	
        	tagName = Utils.ExtractBefore(ref k, '#');
        	tagID = k;
        	
        	return (tagName != "") && (tagID != "");        		
        }
        
        private static bool readOneDeclare(string input, out string key, out string val)
        {
        	key = "";
        	val = "";
        	
        	string k;
        	
        	k = input; key = Utils.ExtractBefore(ref k, ':').Trim();
        	k = input; val = Utils.ExtractAfter(ref k, ':').Trim();
        	
        	if (key == "") key = input;
        	
        	return (key != "");
        }
        
        private static bool readTagDeclare(string input, PropertyList styles)
        {
        	string k = Utils.getDeclareSection(input);
        	
        	string nextDeclare = Utils.ExtractBefore(ref k, ';');
        	if (nextDeclare == "")
        	{
        		nextDeclare = k;
        		k = "";        	
        	}
        	while (nextDeclare != "")
        	{
        		string key, value;
        		readOneDeclare(nextDeclare, out key, out value);
        		if (key != "")
        			styles.Add(key, new PropertyItemType(key, value));
        		
        		nextDeclare = Utils.ExtractBefore(ref k, ';');
        		if (nextDeclare == "")
        		{
        			nextDeclare = k;
        			k = "";        	
        		}
        	}
        	
        	return true;        	
        }
		/// <summary>
		///Decode a full cssStyle into useable format
  		/// No checking is avaliable, crash if any error
		/// </summary>
        public static ArrayList DecodeCssStyle(string input)
        {
        	ArrayList retVal = new ArrayList();
        	ArrayList cssStyleGroups = new ArrayList();
        	
        	Utils.readTagName(input, ref cssStyleGroups);
        	for (Int32 i = 0; i < cssStyleGroups.Count; i++)
        	{
        		CssStyleGroupType curItem = (CssStyleGroupType)(cssStyleGroups[i]);
        		CssStyleType newCssStyleType = new CssStyleType();
        		newCssStyleType.styleTagName = curItem.styleTagName;
        		newCssStyleType.tagName = (new CssHeaderStyleType(curItem.styleTagName)).tagName;
        		if (newCssStyleType.tagName == "")
        			newCssStyleType.tagName = CssHeaderStyleType.UnspecifiedTagName;
        		newCssStyleType.parentTagName = curItem.parentTagName;
        		readTagDeclare(input, newCssStyleType.styles);
        	
        		retVal.Add(newCssStyleType);
        	}
        	return retVal;
        }        	                          	        
        #endregion
        #region MatchParentTag() routines
        private static bool matchHeader(HtmlTag input, CssHeaderStyleType chs)
        {        	
        	return (((chs.tagName == CssHeaderStyleType.UnspecifiedTagName) ||
        	          (input.Name == chs.tagName)) &&
        	          ((chs.tagClass == "") || (chs.tagClass == input["class"])) &&
        	          ((chs.tagID == "") || (chs.tagID == input["id"])));        	        	
        }
        private static bool haveOtherClassID(HtmlTag input, CssHeaderStyleType chs)
        {
        	bool retVal = false;
        	if ((chs.tagClass != "") && (input["class"] != ""))
        	    retVal = (chs.tagClass == input["class"]);
        	if ((chs.tagID != "") && (input["id"] != ""))
        	    retVal = (chs.tagID == input["id"]);
        	return retVal;
        }
//        /// <summary>
//        /// Check if there is parentTag match the header
//        /// </summary>        
//        /// <returns></returns>
//        internal static bool MatchParentTag(HtmlTag currentTag, string header, out HtmlTag matchTag)
//        {        	
//        	matchTag = null;
//        	CssHeaderStyleType chs = new CssHeaderStyleType(header);
//        	
//        	if (currentTag.parentTag != null)
//        		if (chs.familyTag)
//        		{
//        			HtmlTag tempTag = currentTag.parentTag;
//        			if (tempTag != null)
//        			{
//        				Int32 idx = tempTag.childTags.IndexOf(chs.tagName);
//        				if (tempTag.childTags[idx] != currentTag)
//        				{
//        					matchTag = tempTag;
//        					return true;
//        				}
//        			}
//        		}
//        		else //not familyTag
//        		{
//        			HtmlTag tempTag = currentTag.parentTag;
//        			while (tempTag != null)
//        			{
//        				if ((chs.noOtherClassID) && (Utils.haveOtherClassID(tempTag, chs)))
//        				{
//        					return false;
//        				}
//        				else
//        					if(Utils.matchHeader(tempTag, chs))
//        					{
//        						matchTag = tempTag;
//        						return true;
//        					}
//        					else
//        						tempTag = tempTag.parentTag;
//        			}
//        		}      
//        	return false;
//        }                
        #endregion
        #region Other Css routines                        
//        /// <summary>
//        /// Loop and check if currenTag matched the list ParentTagNames
//        /// </summary>
//        internal static bool MatchParentTags(HtmlTag currentTag, ArrayList parentTagNames)
//        {
//        	if (parentTagNames == null) return false;
//        	
//        	HtmlTag tempTag;
//        	for (Int32 i = parentTagNames.Count -1; i >= 0; i--)
//        		if (!(Utils.MatchParentTag(currentTag, (string)parentTagNames[i], out tempTag)))
//        			return false;
//        	
//        	return true;        	
//        }
        /// <summary>
        /// Check if currentTag matches the header
        /// </summary>
        internal static bool MatchCurrentTag(HtmlTag currentTag, string header)
        {
        	CssHeaderStyleType chs = new CssHeaderStyleType(header);
        	bool retVal = true;//((chs.tagClass == "") && (chs.tagID == ""));
        	
        	retVal = retVal && ((chs.tagClass == "") || 
        	                    ((currentTag.Contains("class")) && 
        	                     (chs.tagClass == currentTag["class"])));
        	retVal = retVal && ((chs.tagID == "") || 
        	                    ((currentTag.Contains("id")) &&
        	                                         (chs.tagID == currentTag["id"])));
        	
        	return retVal;
        }
        
        #endregion
//        /// <summary>
//        /// Show about screen of qzMiniHtml
//        /// </summary>
//        public static void AboutScreen()
//        {						
////			string ver = "";
////			#if mh_dotNet_20
////			ver = "20";
////			#elif mh_dotNet_10
////			ver = "10";
////			#endif			
////			
////			string Css = "b.title {color:Indigo;}";			
////			string Html = "<p align=\"centre\"><b class=\"title\">qzMiniHtml.Net</b><sup>ver " +
////				Assembly.GetExecutingAssembly().GetName().Version +
////				" </sup></p> <p>" +
////				"<font size=\"10\">" +
////				"qzMiniHtml.Net is a dotNet" + ver + " component that can parse html/css syntax" +
////				" and output them on graphics or other media. <br>"+				
////				"<br></font>" +
////				
////				"<p align=\"right\">"+
////				"<font size=\"8\">Copyright &copy; (2003-2006) Leung Yat Chun Joseph (lycj) <br>"+									
////				
////				"email: <a href=\"mailto://\">author2004@quickzip.org</a><br>"+
////				"www: <a href=\"http://www.quickzip.org\">http://www.quickzip.org</a><br> " +
////				
////				"</font></p>";
////			
////			mhMessageDialog.Show("About qzMiniHtml.Net",
////			                     Html, Css, MessageBoxButtons.OK, Color.Honeydew);
//        }
//        public static void DebugUnit()
//        {
//            CreatePen(Color.Lime, 9);
//            CreatePen(Brushes.AliceBlue, 9);

//            Bitmap b = new Bitmap(20, 20);
//            Graphics g = Graphics.FromImage(b);
//            Debug.Assert((FontExists("Arial", g) == true), "FontExists Failed.");

//            PrivateFontCollection pfc = new PrivateFontCollection();
//            Debug.Assert((LoadFont("AARDC___.TTF", ref pfc) == true), "loadFont Failed.");
//            Debug.Assert((UserFontExists("Aardvark Cafe", pfc) == true), "UserFontExists Failed.");
//            Console.WriteLine("TextSize of abc = " + Convert.ToString(TextSize(g, "abc", new Font(pfc.Families[0], 10F))));
//            Console.WriteLine("TextSize2 of abc = " + Convert.ToString(TextSize2(g, "abc", new Font(pfc.Families[0], 10F))));
//            Console.WriteLine("TextSize of abcdefgh = " + Convert.ToString(TextSize(g, "abcdefgh", new Font(pfc.Families[0], 12F))));
//            Console.WriteLine("TextSize2 of abcdefgh = " + Convert.ToString(TextSize2(g, "abcdefgh", new Font(pfc.Families[0], 12F))));
//            Console.WriteLine("TextSize of aToz = " + Convert.ToString(TextSize(g, "abcdefghijklmnopqrstuvwxyz", new Font(pfc.Families[0], 9F))));
//            Console.WriteLine("TextSize2 of aToz = " + Convert.ToString(TextSize2(g, "abcdefghijklmnopqrstuvwxyz", new Font(pfc.Families[0], 9F))));


//            g.Dispose();
//            b.Dispose();


//            Debug.Assert((Replace("12123", '1', '3') == "32323"), "Replace Failed.");
//            String aToz = "abcdefghijklmnopqrstuvwxyz";
//            Debug.Assert((ExtractBefore(ref aToz, 'g') == "abcdef"), "ExtractBefore Failed.");
//            Debug.Assert((aToz == "hijklmnopqrstuvwxyz"), "ExtractBefore Failed.");

//            aToz = "abcdefghijklmnopqrstuvwxyz";
//            Debug.Assert((ExtractAfter(ref aToz, 'w') == "xyz"), "ExtractAfter Failed.");
//            Debug.Assert((aToz == "abcdefghijklmnopqrstuv"), "ExtractAfter Failed.");

//            aToz = "abcdefghijklmnopqrstuvwxyz";
//            Debug.Assert((ExtractBetween(aToz, 'q', 'w') == "rstuv"), "ExtractBetween Failed.");

//            aToz = "abc;def;ghi";
//            Debug.Assert((ExtractNextItem(ref aToz, ';') == "abc"), "ExtractNextItem_1 Failed.");
//            Debug.Assert((ExtractNextItem(ref aToz, ';') == "def"), "ExtractNextItem_2 Failed.");
//            Debug.Assert((aToz == "ghi"), "ExtractNextItem Failed.");

//            aToz = "abc;def;ghi";
//            Debug.Assert((ExtractList(aToz, ';').Count == 3), "ExtractList Failed.");
//            Debug.Assert((RemoveFrontSlash("\\beta") == "beta"), "RemoveFrontSlash Failed.");
//            Debug.Assert((Capitalize("clear") == "Clear"), "Capitalize Failed.");


//            SimpleHash("abcdefg");
//            Debug.Assert((AppendSlash(@"c:\xyz") == @"c:\xyz\"), "AppendSlash Failed.");
//            Debug.Assert((RemoveSlash(@"c:\xyz\") == @"c:\xyz"), "RemoveSlash Failed.");

//            aToz = "abc;def;ghi";            
//            ArrayList outputList = ExtractList(aToz, ';');
//            aToz = "abc,def,ghi,";
//            String output = "";
//            foreach (object o in outputList)
//                output += ((string)o + ",");
//            Debug.Assert((output==aToz), "ExtractList Failed.");


//            PropertyList list = ExtravtVariables("href=\"xyz\" name=abc");
//            Debug.Assert(list["href"].value == "xyz", "ExtravtVariables Failed.");
//            Debug.Assert(list["name"].value == "abc", "ExtravtVariables Failed.");

//            Debug.Assert((ExtractFileName(@"c:\xyz\abc.txt") == @"abc.txt"), "ExtractFileName Failed.");
//            Debug.Assert((ExtractFilePath(@"c:\xyz\abc.txt") == @"c:\xyz"), "ExtractFilePath Failed.");
//            Debug.Assert((ExtractFileExt(@"c:\xyz\abc.txt") == @".txt"), "ExtractFileExt Failed.");

//            SymbolHtml();
//            Debug.Assert((LocateSymbol("&amp;") == 0038), "LocateSymbol Failed.");
//            Debug.Assert((LocateSymbol("&0062;") == 0062), "LocateSymbol Failed.");
//            Debug.Assert((DecodeSymbol("&amp;gt;") == ">"), "DecodeSymbol Failed.");

            
//            aToz = @"h1 em";
//            elementType e = elementType.eSpace;
//            Debug.Assert((ExtractNextElement(ref aToz, ref e) == "h1"), "ExtractNextElement Failed.");
//            Debug.Assert((ExtractNextElement(ref aToz, ref e) == "em"), "ExtractNextElement Failed.");

//            Debug.Assert((String2Color("Green") == Color.Green), "String2Color Failed.");
//            Debug.Assert((WebColor2Color("#FF00FF") == Color.FromArgb(255, 0, 255)), "WebColor2Color Failed.");
//            Debug.Assert((StrAlign2Align("Centre") == hAlignType.Centre), "StrAlign2Align Failed.");
//            Debug.Assert((StrSize2PixelSize("75%", 100) == 75), "StrSize2PixelSize (%) Failed.");
//            Debug.Assert((StrSize2PixelSize("20px", 100) == 20), "StrSize2PixelSize (px) Failed.");
//            Debug.Assert((StrSize2PixelSize("30em", 100) == 30), "StrSize2PixelSize (em) Failed.");
//            Debug.Assert((StrPosition2PositionType("Relative") == positionStyleType.Relative), "StrPosition2PositionType Failed.");
//            Debug.Assert((StrBorder2BorderType("Double") == borderStyleType.Double), "StrBorder2BorderType Failed.");
//            Debug.Assert((StrBullet2BulletType("UpperAlpha") == bulletStyleType.UpperAlpha), "StrBullet2BulletType Failed.");
//            Debug.Assert((StrCursor2CursorType("Pointer") == Cursors.Hand), "StrCursor2CursorType Failed.");
//            Debug.Assert((StrMethod2FormMethodType("Get") == formMethodType.Get), "StrMethod2FormMethodType Failed.");
//            Debug.Assert((StrType2VariableType("Alpha") == variableType.Alpha), "StrType2VariableType Failed.");

//        }

//        public static void DebugRun()
//        {
//            Run(@"c:\windows\notepad.exe", "");
//            ProcessInfo pi = new ProcessInfo();
//            Run2(@"c:\windows\notepad.exe", "", ref pi);
//        }
    }
}

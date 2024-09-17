using System;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

namespace QuickZip.MiniHtml2
{

    /// <summary>
    /// Enums types
    /// </summary>
    #region Enums Types       
    public enum loadType { ltString, ltFile, ltWeb, ltWebNoCache }                  //Define where to load from.
    //public enum threeSide { _default=0, _left, _top, _right }
    public enum fourSide { _default=0, _left, _top, _right, _bottom }               //Define Left, Top, Right and Bottom
    public enum hAlignType { Unknown, Left, Right, Centre }                         //Define visible object horizontal hAlign
    public enum vAlignType { Unknown, Top, Bottom }                                 //Define visible object verticial hAlign
    public enum formMethodType { Default, Get, Post }                               //Define form action
    public enum tagStatusType { Normal, Focused, Active }                           //Define state of a visile tag
    public enum selectInfoPairs { sStart, sEnd }                                    //Define Start and End of SelectInfo
    public enum parseMode { Text = 0, Html, BBCode }                          		//Parse html or bbcode
    
    public enum HTMLFlag
    {
        TextFormat, Element, Dynamic, Table, 
        Controls, Search, Xml, Region, Variable, None
    }                                                         //Define tag type in BuiltInTags
    public enum aTextStyle { isNormal, isSubScript, isSuperScript }                 //Define text style
    public enum loadStatus { Idle, Load, Update, Draw, Overlay }                    //Define what is miniHtml doing
    public enum elementType { eSpace, eText, eSymbol, eId, eClass, eStyle, eDash }  //Define type of a char
    public enum symbolType { Reserved, European, Symbol, Scientific, Shape }        //Define symbol type in BuiltInSymbols
    public enum textTransformType { None, Uppercase, Lowercase, Capitalize }        //Define how to transofm a text
    public enum positionStyleType { Static, Relative, Absolute, Fixed, Inherited }  //Define how to allocate a tag
    public enum borderStyleType                                                     //Define a list of border style 
    {
        None, Dotted, Dashed, Solid, Double, Groove, Ridge,
        Inset, Outset, Inherit
    }
    public enum bulletStyleType                                                     //Define a list of bullet style 
    {
        None, Circle, Square, Decimal, UpperAlpha, LowerAlpha,
        UpperRoman, LowerRoman
    }
    public enum variableType { Number, Alpha, String, Formated, Paragraph }         //Define variableType of variableTag (for search text)
    #endregion

    /// <summary>
    /// Records Types
    /// </summary>  
    #region Records Types
    
    public class ColorSettings
    { public Color fontColor, urlColor, activeColor, visitedColor, backGroundColor; }    
    public class KeyValuePair { public String key, value; }
    public class FontStyleSet { public bool bold, italic, regular, strikeout, underline;}
    public class HTMLTagInfo
    {
        public string Html;
        public HTMLFlag flags;
        public Int16 tagLevel;
        public HTMLTagInfo(string aHtml, HTMLFlag aFlags, Int16 aTagLevel)
        {
            this.Html = aHtml;
            this.flags = aFlags;
            this.tagLevel = aTagLevel;
        }
    }
    public class SymbolInfo
    {
        public string symbol;
        public Int32 code;
        public symbolType type;
        public SymbolInfo(string aSymbol, Int32 aCode, symbolType aType)
        {
            this.symbol = aSymbol;
            this.code = aCode;
            this.type = aType;
        }
    }
    public class loadOptionsType
    {
        public bool updateHtml, drawHtml, loadImage, alignImage;
        public Int32 maxWidth, maxHeight;
    }
    public class RomanDigits
	{
    	public UInt32 value; 
    	public string rep;
    	public RomanDigits(UInt32 aValue, string aRep) 
    	{ 
    		this.value = aValue; 
    		this.rep = aRep; 
    	}
	} 
    #endregion



    public class Defines
    {
        /// <summary>
        /// Constants
        /// </summary>
        #region Constants
        public static Int32 border = 5;
        public static string symbolList = @" !@#$%^&*()[]\,./{}:|?";
        public static string picMask = ".jpg .gif .png .bmp";
        public static Int32 defaultListIndent = 40;
        public static Int32 defaultBlockQuoteIndent = 10;
        public static Int32 defaultHRuleHeight = 10;
        public static Int32 defaultHRuleMargin = 5;
        public static Int32 defaultWidth = 200;
        public static string defaultFntName = "Courier";
        public static Int32 defaultFntSize = 12;
        public static string lineBreak = "\r\n";
        public static string formattedSpacing = "     ";
        #endregion
               
        /// <summary>
        /// Array Consts
        /// </summary>      
        #region Array Consts
       public static HTMLTagInfo[] BuiltinTags = new HTMLTagInfo[51] {   
          #region Built in tag list
           //HtmlTag Level guide
           // 50 Master
           // 40 Xml
           // 30 var(Variables tag for search)
           // 15 Html
           // 14 Title, Head, Body
           // 13 selection, hi, DIV, SPAN
           // 12 Table, centre, Form
           // 11 Tr (Table Row)
           // 10 Td, Th (Table Cell, Header)
           // 09 ul, ol (Numbering)
           // 08 li (List), Indent, blockquote
           // 07 P (Paragraph),  H1-H6
           // 06
           // 05 A HtmlTag, Input
           // 04 Text formating tags (B, strong, U, S, I, FONT, sub, sup), 
           // 03
           // 02
           // 01 Unknown Tags, script
           // 00 Text, hr, user, Img, Dynamic, BR, Meta, Binding
           new HTMLTagInfo ("master",       HTMLFlag.Region,        50), 
           new HTMLTagInfo ("xml",          HTMLFlag.Xml,           40), 
           new HTMLTagInfo ("var",          HTMLFlag.Search,        30), 
           new HTMLTagInfo ("html",         HTMLFlag.Region,        15),
           new HTMLTagInfo ("head",         HTMLFlag.Region,        14),           
           new HTMLTagInfo ("body",         HTMLFlag.Region,        14),
           new HTMLTagInfo ("title",        HTMLFlag.Region,        14),
           new HTMLTagInfo ("div",          HTMLFlag.Region,        13),
           new HTMLTagInfo ("selection",    HTMLFlag.TextFormat,    13), 
           new HTMLTagInfo ("hi",           HTMLFlag.TextFormat,    13), 
           new HTMLTagInfo ("table",        HTMLFlag.Table,         13), 
           new HTMLTagInfo ("centre",       HTMLFlag.Region,        13),
           new HTMLTagInfo ("form",         HTMLFlag.Controls,      12),
           new HTMLTagInfo ("tr",           HTMLFlag.Table,         11), 
           new HTMLTagInfo ("td",           HTMLFlag.Table,         10), 
           new HTMLTagInfo ("th",           HTMLFlag.Table,         10), 
           new HTMLTagInfo ("ul",           HTMLFlag.Region,        09),
           new HTMLTagInfo ("ol",           HTMLFlag.Region,        09),
           new HTMLTagInfo ("li",           HTMLFlag.Region,        08),
           new HTMLTagInfo ("blockquote",   HTMLFlag.TextFormat,    08), 
           new HTMLTagInfo ("indent",       HTMLFlag.Region,        08),
           new HTMLTagInfo ("p",            HTMLFlag.Region,        07),
           new HTMLTagInfo ("h1",           HTMLFlag.Region,        07),
           new HTMLTagInfo ("h2",           HTMLFlag.Region,        07),
           new HTMLTagInfo ("h3",           HTMLFlag.Region,        07),
           new HTMLTagInfo ("h4",           HTMLFlag.Region,        07),
           new HTMLTagInfo ("h5",           HTMLFlag.Region,        07),
           new HTMLTagInfo ("h6",           HTMLFlag.Region,        07),
           new HTMLTagInfo ("span",         HTMLFlag.Region,        07),
           new HTMLTagInfo ("font",         HTMLFlag.TextFormat,    04),           
           new HTMLTagInfo ("u",            HTMLFlag.TextFormat,    04),           
           new HTMLTagInfo ("b",            HTMLFlag.TextFormat,    04), 
           new HTMLTagInfo ("s",            HTMLFlag.TextFormat,    04), 
           new HTMLTagInfo ("i",            HTMLFlag.TextFormat,    04), 
           new HTMLTagInfo ("a",            HTMLFlag.TextFormat,    04), 
           new HTMLTagInfo ("sup",          HTMLFlag.TextFormat,    04), 
           new HTMLTagInfo ("sub",          HTMLFlag.TextFormat,    04), 
           new HTMLTagInfo ("strong",       HTMLFlag.TextFormat,    04), 
           new HTMLTagInfo ("color",        HTMLFlag.TextFormat,    04),
           new HTMLTagInfo ("input",        HTMLFlag.Controls,      02), 
           new HTMLTagInfo ("select",       HTMLFlag.Controls,      02), 
           new HTMLTagInfo ("option",       HTMLFlag.Controls,      02), 
           new HTMLTagInfo ("script",       HTMLFlag.None,          01),
           new HTMLTagInfo ("meta",       	HTMLFlag.Variable,      00),
           new HTMLTagInfo ("br",           HTMLFlag.Element,       00),
           new HTMLTagInfo ("hr",           HTMLFlag.Element,       00),
           new HTMLTagInfo ("img",          HTMLFlag.Element,       00),
           new HTMLTagInfo ("text",         HTMLFlag.Element,       00),
           new HTMLTagInfo ("binding",      HTMLFlag.Element,       00),  
           new HTMLTagInfo ("dynamic",      HTMLFlag.Dynamic,       00),
           new HTMLTagInfo ("user",         HTMLFlag.Dynamic,       00),                  
           #endregion
       };               

       public static SymbolInfo[] BuiltinSymbols = new SymbolInfo[252] {
          #region Built in Symbol list
            new SymbolInfo("amp"     ,0038,symbolType.Reserved), //01
            new SymbolInfo("gt"      ,0062,symbolType.Reserved), //02
            new SymbolInfo("lt"      ,0060,symbolType.Reserved), //03
            new SymbolInfo("quot"    ,0034,symbolType.Reserved), //04
            new SymbolInfo("acute"   ,0180,symbolType.European), //05
            new SymbolInfo("cedil"   ,0184,symbolType.European), //06
            new SymbolInfo("circ"    ,0710,symbolType.European), //07
            new SymbolInfo("macr"    ,0175,symbolType.European), //08
            new SymbolInfo("middot"  ,0183,symbolType.European), //09
            new SymbolInfo("tilde"   ,0732,symbolType.European), //10
            new SymbolInfo("urnl"    ,0168,symbolType.European), //11            
            new SymbolInfo("Aacute"  ,0193,symbolType.European), //12
            new SymbolInfo("aacute"  ,0225,symbolType.European), //13
            new SymbolInfo("Acirc"   ,0194,symbolType.European), //14
            new SymbolInfo("acirc"   ,0226,symbolType.European), //15
            new SymbolInfo("AElig"   ,0198,symbolType.European), //16
            new SymbolInfo("aelig"   ,0230,symbolType.European), //17
            new SymbolInfo("Agrave"  ,0192,symbolType.European), //18
            new SymbolInfo("agrave"  ,0224,symbolType.European), //19
            new SymbolInfo("Aring"   ,0197,symbolType.European), //20
            new SymbolInfo("aring"   ,0229,symbolType.European), //21
            new SymbolInfo("Atilde"  ,0195,symbolType.European), //22
            new SymbolInfo("atilde"  ,0227,symbolType.European), //23
            new SymbolInfo("Auml"    ,0196,symbolType.European), //24
            new SymbolInfo("auml"    ,0228,symbolType.European), //25
            new SymbolInfo("Ccedil"  ,0199,symbolType.European), //26
            new SymbolInfo("ccedil"  ,0231,symbolType.European), //27
            new SymbolInfo("Eacute"  ,0201,symbolType.European), //28
            new SymbolInfo("eacute"  ,0233,symbolType.European), //29
            new SymbolInfo("Ecirc"   ,0202,symbolType.European), //30
            new SymbolInfo("ecirc"   ,0234,symbolType.European), //31
            new SymbolInfo("Egrave"  ,0200,symbolType.European), //32
            new SymbolInfo("egrave"  ,0232,symbolType.European), //33
            new SymbolInfo("ETH"     ,0208,symbolType.European), //34
            new SymbolInfo("eth"     ,0240,symbolType.European), //35
            new SymbolInfo("Euml"    ,0203,symbolType.European), //36
            new SymbolInfo("euml"    ,0235,symbolType.European), //37
            new SymbolInfo("Iacute"  ,0205,symbolType.European), //38
            new SymbolInfo("iacute"  ,0237,symbolType.European), //39
            new SymbolInfo("Icirc"   ,0206,symbolType.European), //40
            new SymbolInfo("icirc"   ,0238,symbolType.European), //41
            new SymbolInfo("Igrave"  ,0204,symbolType.European), //42
            new SymbolInfo("igrave"  ,0236,symbolType.European), //43
            new SymbolInfo("Iuml"    ,0207,symbolType.European), //44
            new SymbolInfo("iuml"    ,0239,symbolType.European), //45
            new SymbolInfo("Ntide"   ,0209,symbolType.European), //46
            new SymbolInfo("Ntide"   ,0241,symbolType.European), //47
            new SymbolInfo("Oacute"  ,0211,symbolType.European), //48
            new SymbolInfo("oacute"  ,0243,symbolType.European), //49
            new SymbolInfo("Ocirc"   ,0212,symbolType.European), //50
            new SymbolInfo("Ocirc"   ,0244,symbolType.European), //51
            new SymbolInfo("OElig"   ,0338,symbolType.European), //52
            new SymbolInfo("oelig"   ,0339,symbolType.European), //53
            new SymbolInfo("Ograve"  ,0210,symbolType.European), //54
            new SymbolInfo("ograve"  ,0242,symbolType.European), //55
            new SymbolInfo("Oslash"  ,0216,symbolType.European), //56
            new SymbolInfo("oslash"  ,0248,symbolType.European), //57
            new SymbolInfo("Otilde"  ,0213,symbolType.European), //58
            new SymbolInfo("otilde"  ,0245,symbolType.European), //59
            new SymbolInfo("Ouml"    ,0214,symbolType.European), //60
            new SymbolInfo("ouml"    ,0246,symbolType.European), //61
            new SymbolInfo("Scaron"  ,0352,symbolType.European), //62
            new SymbolInfo("scaron"  ,0353,symbolType.European), //63
            new SymbolInfo("szlig"   ,0223,symbolType.European), //64
            new SymbolInfo("THORN"   ,0222,symbolType.European), //65
            new SymbolInfo("thorn"   ,0254,symbolType.European), //66
            new SymbolInfo("Uacute"  ,0218,symbolType.European), //67
            new SymbolInfo("uacute"  ,0250,symbolType.European), //68
            new SymbolInfo("Ucirc"   ,0219,symbolType.European), //69
            new SymbolInfo("ucirc"   ,0251,symbolType.European), //70
            new SymbolInfo("Ugrave"  ,0217,symbolType.European), //71
            new SymbolInfo("ugrave"  ,0249,symbolType.European), //72
            new SymbolInfo("Uuml"    ,0220,symbolType.European), //73
            new SymbolInfo("uuml"    ,0252,symbolType.European), //74
            new SymbolInfo("Yacute"  ,0221,symbolType.European), //75
            new SymbolInfo("yacute"  ,0253,symbolType.European), //76
            new SymbolInfo("Yuml"    ,0255,symbolType.European), //77
            new SymbolInfo("yuml"    ,0376,symbolType.European), //78
            new SymbolInfo("cent"    ,0162,symbolType.Symbol), //79
            new SymbolInfo("curren"  ,0164,symbolType.Symbol), //80
            new SymbolInfo("euro"    ,8364,symbolType.Symbol), //81
            new SymbolInfo("pound"   ,0163,symbolType.Symbol), //82
            new SymbolInfo("yen"     ,0165,symbolType.Symbol), //83            
            new SymbolInfo("brvbar"  ,0166,symbolType.Symbol), //84
            new SymbolInfo("bull"    ,8226,symbolType.Symbol), //85
            new SymbolInfo("copy"    ,0169,symbolType.Symbol), //86
            new SymbolInfo("dagger"  ,8224,symbolType.Symbol), //87
            new SymbolInfo("Dagger"  ,8225,symbolType.Symbol), //88
            new SymbolInfo("frasl"   ,8260,symbolType.Symbol), //89
            new SymbolInfo("hellip"  ,8230,symbolType.Symbol), //90
            new SymbolInfo("iexcl"   ,0161,symbolType.Symbol), //91
            new SymbolInfo("image"   ,8465,symbolType.Symbol), //92
            new SymbolInfo("iquest"  ,0191,symbolType.Symbol), //93
            new SymbolInfo("lrm"     ,8206,symbolType.Symbol), //94
            new SymbolInfo("mdash"   ,8212,symbolType.Symbol), //95
            new SymbolInfo("ndash"   ,8211,symbolType.Symbol), //96
            new SymbolInfo("not"     ,0172,symbolType.Symbol), //97
            new SymbolInfo("oline"   ,8254,symbolType.Symbol), //98
            new SymbolInfo("ordf"    ,0170,symbolType.Symbol), //99
            new SymbolInfo("ordm"    ,0186,symbolType.Symbol), //100
            new SymbolInfo("para"    ,0182,symbolType.Symbol), //101
            new SymbolInfo("permil"  ,8240,symbolType.Symbol), //102
            new SymbolInfo("prime"   ,8242,symbolType.Symbol), //103
            new SymbolInfo("Prime"   ,8243,symbolType.Symbol), //104
            new SymbolInfo("real"    ,8476,symbolType.Symbol), //105
            new SymbolInfo("reg"     ,0714,symbolType.Symbol), //106
            new SymbolInfo("rlm"     ,8207,symbolType.Symbol), //107
            new SymbolInfo("sect"    ,0167,symbolType.Symbol), //108
            new SymbolInfo("shy"     ,0173,symbolType.Symbol), //109
            new SymbolInfo("sup1"    ,0185,symbolType.Symbol), //110
            new SymbolInfo("trade"   ,8482,symbolType.Symbol), //111
            new SymbolInfo("weierp"  ,8472,symbolType.Symbol), //112            
            new SymbolInfo("bdquo"   ,8222,symbolType.Symbol), //113
            new SymbolInfo("laquo"   ,0171,symbolType.Symbol), //114
            new SymbolInfo("ldquo"   ,8220,symbolType.Symbol), //115
            new SymbolInfo("lsaquo"  ,8249,symbolType.Symbol), //116
            new SymbolInfo("lsquo"   ,8216,symbolType.Symbol), //117
            new SymbolInfo("raquo"   ,0187,symbolType.Symbol), //118
            new SymbolInfo("rdquo"   ,8221,symbolType.Symbol), //119
            new SymbolInfo("rsaquo"  ,8250,symbolType.Symbol), //120
            new SymbolInfo("rsquo"   ,8217,symbolType.Symbol), //121
            new SymbolInfo("sbquo"   ,8218,symbolType.Symbol), //122            
            new SymbolInfo("emsp"    ,8195,symbolType.Symbol), //123
            new SymbolInfo("ensp"    ,8194,symbolType.Symbol), //124
            new SymbolInfo("nbsp"    ,0160,symbolType.Symbol), //125
            new SymbolInfo("thinsp"  ,8201,symbolType.Symbol), //126
            new SymbolInfo("zwj"     ,8205,symbolType.Symbol), //127
            new SymbolInfo("zwnj"    ,8204,symbolType.Symbol), //128
            new SymbolInfo("deg"    ,0176,symbolType.Scientific), //129
            new SymbolInfo("divide" ,0247,symbolType.Scientific), //130
            new SymbolInfo("frac12" ,0189,symbolType.Scientific), //131
            new SymbolInfo("frac14" ,0188,symbolType.Scientific), //132
            new SymbolInfo("frac34" ,0190,symbolType.Scientific), //133
            new SymbolInfo("ge"     ,8805,symbolType.Scientific), //134
            new SymbolInfo("le"     ,8804,symbolType.Scientific), //135
            new SymbolInfo("minus"  ,8722,symbolType.Scientific), //136
            new SymbolInfo("sup2"   ,0178,symbolType.Scientific), //137
            new SymbolInfo("sup3"   ,0179,symbolType.Scientific), //138
            new SymbolInfo("times"  ,0215,symbolType.Scientific), //139
            new SymbolInfo("alefsym",8501,symbolType.Scientific), //140
            new SymbolInfo("and"    ,8743,symbolType.Scientific), //141
            new SymbolInfo("ang"    ,8736,symbolType.Scientific), //142
            new SymbolInfo("asymp"  ,8776,symbolType.Scientific), //143
            new SymbolInfo("cap"    ,8745,symbolType.Scientific), //144
            new SymbolInfo("cong"   ,8773,symbolType.Scientific), //145
            new SymbolInfo("cup"    ,8746,symbolType.Scientific), //146
            new SymbolInfo("empty"  ,8709,symbolType.Scientific), //147
            new SymbolInfo("equiv"  ,8801,symbolType.Scientific), //148
            new SymbolInfo("exist"  ,8707,symbolType.Scientific), //149
            new SymbolInfo("fnof"   ,0402,symbolType.Scientific), //150
            new SymbolInfo("forall" ,8704,symbolType.Scientific), //151
            new SymbolInfo("infin"  ,8734,symbolType.Scientific), //152
            new SymbolInfo("int"    ,8747,symbolType.Scientific), //153
            new SymbolInfo("isin"   ,8712,symbolType.Scientific), //154
            new SymbolInfo("lang"   ,9001,symbolType.Scientific), //155
            new SymbolInfo("lceil"  ,8968,symbolType.Scientific), //156
            new SymbolInfo("lfloor" ,8970,symbolType.Scientific), //157
            new SymbolInfo("lowast" ,8727,symbolType.Scientific), //158
            new SymbolInfo("micro"  ,0181,symbolType.Scientific), //159
            new SymbolInfo("nabla"  ,8711,symbolType.Scientific), //160
            new SymbolInfo("ne"     ,8800,symbolType.Scientific), //161
            new SymbolInfo("ni"     ,8715,symbolType.Scientific), //162
            new SymbolInfo("notin"  ,8713,symbolType.Scientific), //163
            new SymbolInfo("nsub"   ,8836,symbolType.Scientific), //164
            new SymbolInfo("cplus"  ,8853,symbolType.Scientific), //165
            new SymbolInfo("or"     ,8744,symbolType.Scientific), //166
            new SymbolInfo("otimes" ,8855,symbolType.Scientific), //167
            new SymbolInfo("part"   ,8706,symbolType.Scientific), //168
            new SymbolInfo("perp"   ,8869,symbolType.Scientific), //169
            new SymbolInfo("plusmn" ,0177,symbolType.Scientific), //170
            new SymbolInfo("prod"   ,8719,symbolType.Scientific), //171
            new SymbolInfo("prop"   ,8733,symbolType.Scientific), //172
            new SymbolInfo("radic"  ,8730,symbolType.Scientific), //173
            new SymbolInfo("rang"   ,9002,symbolType.Scientific), //174
            new SymbolInfo("rceil"  ,8969,symbolType.Scientific), //175
            new SymbolInfo("rfloor" ,8971,symbolType.Scientific), //176
            new SymbolInfo("sdot"   ,8901,symbolType.Scientific), //177
            new SymbolInfo("sim"    ,8764,symbolType.Scientific), //178
            new SymbolInfo("sub"    ,8834,symbolType.Scientific), //179
            new SymbolInfo("sube"   ,8838,symbolType.Scientific), //180
            new SymbolInfo("sum"    ,8721,symbolType.Scientific), //181
            new SymbolInfo("sup"    ,8835,symbolType.Scientific), //182
            new SymbolInfo("supe"   ,8839,symbolType.Scientific), //183
            new SymbolInfo("there4" ,8756,symbolType.Scientific), //184
            new SymbolInfo("Alpha"  ,0913,symbolType.Scientific), //185
            new SymbolInfo("alpha"  ,0945,symbolType.Scientific), //186
            new SymbolInfo("Beta"   ,0914,symbolType.Scientific), //187
            new SymbolInfo("beta"   ,0946,symbolType.Scientific), //188
            new SymbolInfo("Chi"    ,0935,symbolType.Scientific), //189
            new SymbolInfo("chi"    ,0967,symbolType.Scientific), //190
            new SymbolInfo("Delta"  ,0916,symbolType.Scientific), //191
            new SymbolInfo("delta"  ,0948,symbolType.Scientific), //192
            new SymbolInfo("Epsilon",0917,symbolType.Scientific), //193
            new SymbolInfo("epsilon",0949,symbolType.Scientific), //194
            new SymbolInfo("Eta"    ,0919,symbolType.Scientific), //195
            new SymbolInfo("eta"    ,0951,symbolType.Scientific), //196
            new SymbolInfo("Gamma"  ,0915,symbolType.Scientific), //197
            new SymbolInfo("gamma"  ,0947,symbolType.Scientific), //198
            new SymbolInfo("Iota"   ,0921,symbolType.Scientific), //199
            new SymbolInfo("iota"   ,0953,symbolType.Scientific), //200
            new SymbolInfo("Kappa"  ,0922,symbolType.Scientific), //201
            new SymbolInfo("kappa"  ,0954,symbolType.Scientific), //202
            new SymbolInfo("Lambda" ,0923,symbolType.Scientific), //203
            new SymbolInfo("lambda" ,0955,symbolType.Scientific), //204
            new SymbolInfo("Mu"     ,0924,symbolType.Scientific), //205
            new SymbolInfo("mu"     ,0956,symbolType.Scientific), //206
            new SymbolInfo("Nu"     ,0925,symbolType.Scientific), //207
            new SymbolInfo("nu"     ,0957,symbolType.Scientific), //208
            new SymbolInfo("Omega"  ,0937,symbolType.Scientific), //209
            new SymbolInfo("omega"  ,0969,symbolType.Scientific), //210
            new SymbolInfo("Omicron",0927,symbolType.Scientific), //211
            new SymbolInfo("omicron",0959,symbolType.Scientific), //212
            new SymbolInfo("Phi"    ,0934,symbolType.Scientific), //213
            new SymbolInfo("phi"    ,0966,symbolType.Scientific), //214
            new SymbolInfo("Pi"     ,0928,symbolType.Scientific), //215
            new SymbolInfo("pi"     ,0960,symbolType.Scientific), //216
            new SymbolInfo("piv"    ,0982,symbolType.Scientific), //217
            new SymbolInfo("Psi"    ,0936,symbolType.Scientific), //218
            new SymbolInfo("psi"    ,0968,symbolType.Scientific), //219
            new SymbolInfo("Rho"    ,0929,symbolType.Scientific), //220
            new SymbolInfo("rho"    ,0961,symbolType.Scientific), //221
            new SymbolInfo("Sigma"  ,0931,symbolType.Scientific), //222
            new SymbolInfo("sigma"  ,0963,symbolType.Scientific), //223
            new SymbolInfo("sigmaf" ,0962,symbolType.Scientific), //224
            new SymbolInfo("Tau"    ,0932,symbolType.Scientific), //225
            new SymbolInfo("tau"    ,0964,symbolType.Scientific), //226
            new SymbolInfo("Theta"  ,0920,symbolType.Scientific), //227
            new SymbolInfo("theta"  ,0952,symbolType.Scientific), //228
            new SymbolInfo("thetasym",0977,symbolType.Scientific),//229
            new SymbolInfo("upsih"  ,0978,symbolType.Scientific), //230
            new SymbolInfo("Upsilon",0933,symbolType.Scientific), //231
            new SymbolInfo("upsilon",0965,symbolType.Scientific), //232
            new SymbolInfo("Xi"     ,0926,symbolType.Scientific), //233
            new SymbolInfo("xi"     ,0958,symbolType.Scientific), //234
            new SymbolInfo("Zeta"   ,0918,symbolType.Scientific), //235
            new SymbolInfo("zeta"   ,0950,symbolType.Scientific), //236
            new SymbolInfo("crarr"  ,8629,symbolType.Shape), //237
            new SymbolInfo("darr"   ,8595,symbolType.Shape), //238
            new SymbolInfo("dArr"   ,8659,symbolType.Shape), //239
            new SymbolInfo("harr"   ,8596,symbolType.Shape), //240
            new SymbolInfo("hArr"   ,8660,symbolType.Shape), //241
            new SymbolInfo("larr"   ,8592,symbolType.Shape), //242
            new SymbolInfo("lArr"   ,8656,symbolType.Shape), //243
            new SymbolInfo("rarr"   ,8594,symbolType.Shape), //244
            new SymbolInfo("rArr"   ,8658,symbolType.Shape), //245
            new SymbolInfo("uarr"   ,8593,symbolType.Shape), //246
            new SymbolInfo("uArr"   ,8657,symbolType.Shape), //247            
            new SymbolInfo("clubs"  ,9827,symbolType.Shape), //248
            new SymbolInfo("diams"  ,9830,symbolType.Shape), //249
            new SymbolInfo("hearts" ,9829,symbolType.Shape), //250
            new SymbolInfo("spades" ,9824,symbolType.Shape), //251
            new SymbolInfo("loz"    ,9674,symbolType.Shape)  //252
            #endregion
       };
        
       public static String[] BuiltinStyles = new String[83]
       {
          #region Built in Style list        
        "background-attachment",        //00
		"background-color",             //01
		"backgroundimage",              //02
		"background-repeat",            //03
		"background-position",          //04
		"border",                       //05
		"border-color",                 //06
		"border-spacing",               //07
		"border-style",                 //08
		"border-top",                   //09
		"border-right",                 //10
		"border-bottom",                //11
		"border-left",                  //12
		"border-top-color",             //13
		"border-right-color",           //14
		"border-bottom-color",          //15
		"border-left-color",            //16
		"border-top-style",             //17
		"border-right-style",           //18
		"border-bottom-style",          //19
		"border-left-style",            //20
		"border-top-width",             //21
		"border-right-width",           //22
		"border-bottom-width",          //23
		"border-left-width",            //24
		"border-width",                 //25
		"clear",                        //26
		"bottom",                       //27
		"color",                        //28
		"cursor",                       //29
		"display",                      //30
		"float",                        //31
		"font",                         //32
		"font-family",                  //33
		"font-size",                    //34
		"font-style",                   //35
		"font-variant",                 //36
		"font-weight",                  //37
		"height",                       //38
		"left",                         //39
		"letter-spacing",               //40
		"line-height",                  //41
		"list-style",                   //42
		"list-style-image",             //43
		"list-style-position",          //44
		"list-style-type",              //45
		"margin",                       //46
		"margin-top",                   //47
		"margin-right",                 //48
		"margin-bottom",                //49
		"margin-left",                  //50
		"marks",                        //51
		"max-height",                   //52
		"max-width",                    //53
		"min-height",                   //54
		"min-width",                    //55
		"orphans",                      //56
		"overflow",                     //57
		"padding",                      //58
		"padding-top",                  //59
		"padding-right",                //60
		"padding-bottom",               //61
		"padding-left",                 //62
		"page",                         //63
		"page-break-after",             //64
		"page-break-before",            //65
		"page-break-inside",            //66
		"position",                     //67
		"right",                        //68
		"size",                         //69
		"table-display",                //70
		"text-align",                   //71
		"text-decoration",              //72
		"text-indent",                  //73
		"text-transform",               //74
		"top",                          //75
		"vertical-align",               //76
		"visibility",                   //77
		"white-space",                  //78
		"windows",                      //79
		"width",                        //80
		"word-spacing",                 //81
		"z-index"                       //82
        #endregion
       };
       
       public static RomanDigits[] BuiltinRomans = new RomanDigits[13]
       {
       	  #region Built in Romans list       	  
       	  new RomanDigits(1000, "M"),
    	  new RomanDigits(900, "CM"),
    	  new RomanDigits(500, "D"),
    	  new RomanDigits(400, "CD"),
    	  new RomanDigits(100, "C"),
	      new RomanDigits(90, "XC"),
	      new RomanDigits(50, "L"),
	      new RomanDigits(40, "XL"),
	      new RomanDigits(10, "X"),
	      new RomanDigits(9, "IX"),
	      new RomanDigits(5, "V"),
	      new RomanDigits(4, "IV"),
	      new RomanDigits(1, "I"),
	      #endregion
       };
       
       public static HTMLTagInfo[] BuiltinBBCodes = new HTMLTagInfo[14] {   
          #region Built in BBCode list           
           new HTMLTagInfo ("ul",           HTMLFlag.Region,        09),
           new HTMLTagInfo ("ol",           HTMLFlag.Region,        09),
           new HTMLTagInfo ("*",            HTMLFlag.Region,        08),
           new HTMLTagInfo ("quote",   		HTMLFlag.TextFormat,    08),
           new HTMLTagInfo ("centre",       HTMLFlag.Region,        07),
           new HTMLTagInfo ("size",         HTMLFlag.TextFormat,    04),           
           new HTMLTagInfo ("color",        HTMLFlag.TextFormat,    04),           
           new HTMLTagInfo ("u",            HTMLFlag.TextFormat,    04),           
           new HTMLTagInfo ("b",            HTMLFlag.TextFormat,    04), 
           new HTMLTagInfo ("s",            HTMLFlag.TextFormat,    04), 
           new HTMLTagInfo ("i",            HTMLFlag.TextFormat,    04), 
           new HTMLTagInfo ("url",          HTMLFlag.TextFormat,    04), 
           new HTMLTagInfo ("img",          HTMLFlag.Element,       00),
           new HTMLTagInfo ("br",           HTMLFlag.Region,        00),
           #endregion
       };  
       #endregion
        /// <summary>
        /// Event Handlers
        /// </summary>
        #region Event Handlers
       public class LinkClickEventArgs : EventArgs
       {
           private Object cTag;
           private String cURL;
           public Object currentTag { get { return cTag; } }
           public String targetURL { get { return cURL; } }
           public LinkClickEventArgs(Object aTag, String aURL)
           {
               this.cTag = aTag;
               this.cURL = aURL;
           }
       }

       public class mhWorkEventArgs : EventArgs
       {
           public enum WorkType { wUpdate, wLoad, wDraw }
           public WorkType work;
           public mhWorkEventArgs(WorkType aWorkType)
           {
               this.work = aWorkType;
           }
       }

       public class FormElementEventArgs : EventArgs
       {
           public Object ElementTag;
           public FormElementEventArgs(Object aTag)
           {
               this.ElementTag = aTag;
           }
       }

       public delegate void LinkClickEventHandler(
           Object sender,
           LinkClickEventArgs e);

       public delegate void mhWorkEventHandler(
           Object sender,
           mhWorkEventArgs e);

       public delegate void FormEventHandler(
           Object sender,
           FormElementEventArgs e);
       #endregion


        
       

   }


}

namespace Memoria.Assets
{
    public enum TextResourceFormat
    {
        /// <summary>
        /// Apple Strings (.strings) <br/>
        /// The standard strings file format consists of one or more key-value pairs along with optional comments. The key and value in a given pair are strings of text enclosed in double quotation marks, separated by an equals sign, and ending with a semicolon.
        /// </summary>
        /// <example>
        ///  /* Comment */ <br/>
        ///  "$0002_EVT_STARTUP_RYOTA_1_0041" = "{W63H2}{Widths 0,0,16,-1,0,63,16,-1}{Zidane}\n“It’s me, {Zidane}!”"; <br/>
        /// </example>
        Strings = 10,

        /// <summary>
        /// Windows JSON (.resjson)
        /// Key-value pairs are delimited with colons (:), surrounded by quotes ("), comma separated.
        /// </summary>
        /// <example>
        /// { <br/>
        ///     "$0002_EVT_STARTUP_RYOTA_1_0041"		:		"{W63H2}{Widths 0,0,16,-1,0,63,16,-1}{Zidane}\n“It’s me, {Zidane}!”", <br/>
        /// }
        /// </example>
        Resjson = 20,
    }
}

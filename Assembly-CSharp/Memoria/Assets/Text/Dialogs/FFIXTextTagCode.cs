namespace Memoria.Assets
{
    public enum FFIXTextTagCode
    {
        /// <summary>[ENDN]</summary>
        End,
        /// <summary>[IMME]</summary>
        Instantly,
        /// <summary>[FLIM]</summary>
        Flash,
        /// <summary>[NFOC]</summary>
        NoFocus,
        /// <summary>[NANI]</summary>
        NoAnimation,
        /// <summary>[PAGE]</summary>
        NewPage,
        /// <summary>[CHOO][MOVE=18,0]</summary>
        Choice,
        /// <summary>[PCHC=?,?,?...]</summary>
        PreChoose,
        /// <summary>[PCHM=?,?,?...]</summary>
        PreChooseMask,
        /// <summary>[MOVE=18,0]</summary>
        Tab,
        /// <summary>[MOVE=?,?]</summary>
        MoveCaret,

        /// <summary>[ZDNE]</summary>
        Zidane,
        /// <summary>[VIVI]</summary>
        Vivi,
        /// <summary>[DGGR]</summary>
        Dagger,
        /// <summary>[STNR]</summary>
        Steiner,
        /// <summary>[FRYA]</summary>
        Freya,
        Fraya, // For retro-compatibility
        /// <summary>[QUIN]</summary>
        Quina,
        /// <summary>[EIKO]</summary>
        Eiko,
        /// <summary>[AMRT]</summary>
        Amarant,
        /// <summary>Other character names</summary>
        CharacterName,
        /// <summary>[PTY1], [PTY2], [PTY3], [PTY4]</summary>
        Party,
        /// <summary>[TEXT=?,?,?...]</summary>
        Text,
        /// <summary>[NUMB=?]</summary>
        Variable,
        /// <summary>[ITEM=?]</summary>
        Item,

        /// <summary>Generic text color like [FFCC00FF]</summary>
        TextRGBA,
        /// <summary>[HSHD]</summary>
        ShadowToggle,
        /// <summary>[NSHD]</summary>
        ShadowOff,
        /// <summary>[C8C8C8][HSHD]</summary>
        White,
        /// <summary>[B880E0][HSHD]</summary>
        Pink,
        /// <summary>[68C0D8][HSHD]</summary>
        Cyan,
        /// <summary>[D06050][HSHD]</summary>
        Brown,
        /// <summary>[C8B040][HSHD]</summary>
        Yellow,
        /// <summary>[78C840][HSHD]</summary>
        Green,
        /// <summary>[909090][HSHD]</summary>
        Grey,

        /// <summary>[INCS][TIME=-1]</summary>
        IncreaseSignal,
        /// <summary>[INCS]</summary>
        IncreaseSignalEx,
        /// <summary>[TAIL=?]</summary>
        TailPosition,
        /// <summary>[TAIL=LOR]</summary>
        LowerRight,
        /// <summary>[TAIL=LOL]</summary>
        LowerLeft,
        /// <summary>[TAIL=UPR]</summary>
        UpperRight,
        /// <summary>[TAIL=UPL]</summary>
        UpperLeft,
        /// <summary>[TAIL=LOC]</summary>
        LowerCenter,
        /// <summary>[TAIL=UPC]</summary>
        UpperCenter,
        /// <summary>[TAIL=LORF]</summary>
        LowerRightForce,
        /// <summary>[TAIL=LOLF]</summary>
        LowerLeftForce,
        /// <summary>[TAIL=UPRF]</summary>
        UpperRightForce,
        /// <summary>[TAIL=UPLF]</summary>
        UpperLeftForce,
        /// <summary>[TAIL=DEFT]</summary>
        DialogPosition,
        /// <summary>[STRT=?,?]</summary>
        DialogSize,
        /// <summary>[YADD]</summary>
        DialogY,
        /// <summary>[XTAB]</summary>
        DialogX,
        /// <summary>[FEED]</summary>
        DialogF,
        /// <summary>[MPOS=?,?]</summary>
        Position,
        /// <summary>[SPAY=?]</summary>
        SpacingY,

        /// <summary>[SPRT=?,?,...]</summary>
        Sprite,
        /// <summary>[DBTN=?]</summary>
        DefaultButton,
        /// <summary>[CBTN=?]</summary>
        CustomButton,
        /// <summary>[KCBT=?]</summary>
        KeyboardButton,
        /// <summary>[JCBT=?]</summary>
        JoyStickButton,
        /// <summary>[DBTN=UP]</summary>
        Up,
        /// <summary>[DBTN=DOWN]</summary>
        Down,
        /// <summary>[DBTN=LEFT]</summary>
        Left,
        /// <summary>[DBTN=RIGHT]</summary>
        Right,
        /// <summary>[DBTN=CIRCLE]</summary>
        Circle,
        /// <summary>[DBTN=CROSS]</summary>
        Cross,
        /// <summary>[DBTN=TRIANGLE]</summary>
        Triangle,
        /// <summary>[DBTN=SQUARE]</summary>
        Square,
        /// <summary>[DBTN=R1]</summary>
        R1,
        /// <summary>[DBTN=R2]</summary>
        R2,
        /// <summary>[DBTN=L1]</summary>
        L1,
        /// <summary>[DBTN=L2]</summary>
        L2,
        /// <summary>[DBTN=SELECT]</summary>
        Select,
        /// <summary>[DBTN=START]</summary>
        Start,
        /// <summary>[DBTN=PAD]</summary>
        Pad,
        /// <summary>[CBTN=UP]</summary>
        UpEx,
        /// <summary>[CBTN=DOWN]</summary>
        DownEx,
        /// <summary>[CBTN=LEFT]</summary>
        LeftEx,
        /// <summary>[CBTN=RIGHT]</summary>
        RightEx,
        /// <summary>[CBTN=CIRCLE]</summary>
        CircleEx,
        /// <summary>[CBTN=CROSS]</summary>
        CrossEx,
        /// <summary>[CBTN=TRIANGLE]</summary>
        TriangleEx,
        /// <summary>[CBTN=SQUARE]</summary>
        SquareEx,
        /// <summary>[CBTN=R1]</summary>
        R1Ex,
        /// <summary>[CBTN=R2]</summary>
        R2Ex,
        /// <summary>[CBTN=L1]</summary>
        L1Ex,
        /// <summary>[CBTN=L2]</summary>
        L2Ex,
        /// <summary>[CBTN=SELECT]</summary>
        SelectEx,
        /// <summary>[CBTN=START]</summary>
        StartEx,
        /// <summary>[CBTN=PAD]</summary>
        PadEx,

        /// <summary>[SIGL=0], [SIGL=1], [SIGL=2]</summary>
        Signal,
        /// <summary>[TIME=?]</summary>
        Time,
        /// <summary>[WAIT=?]</summary>
        Wait,
        /// <summary>[NTUR]</summary>
        TurboOff,
        /// <summary>[CENT=?]</summary>
        Center,
        /// <summary>[JSTF]</summary>
        Justified,
        /// <summary>[MIRR]</summary>
        Mirrored,
        /// <summary>[sup]</summary>
        Superscript,
        /// <summary>[sub]</summary>
        Subscript,
        /// <summary>[url=?]</summary>
        Hyperlink,
        /// <summary>[b]</summary>
        Bold,
        /// <summary>[i]</summary>
        Italic,
        /// <summary>[u]</summary>
        Underline,
        /// <summary>[s]</summary>
        Strikethrough,
        /// <summary>[c]</summary>
        IgnoreColor,
        /// <summary>[ICON=?]</summary>
        Icon,
        /// <summary>[PNEW=?]</summary>
        IconEx,
        /// <summary>[MOBI=?]</summary>
        Mobile,
        /// <summary>[SPED=?]</summary>
        Speed,
        /// <summary>[WDTH=?,?,?...]</summary>
        Widths,
        /// <summary>[OFFT=?,?,?]</summary>
        Offset,
        /// <summary>[TBLE=?,?,?...]</summary>
        Table,
    }
}

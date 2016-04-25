using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Memoria
{
    public sealed class FieldTags
    {
        private readonly KeyValuePair<String, TextReplacement>[] _simpleTags;
        private readonly KeyValuePair<Regex, String>[] _complexTags;

        public FieldTags()
        {
            _simpleTags = GetSimpleTags();
            _complexTags = GetComplexTags();
        }

        public String Replace(String str)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            str = str.ReplaceAll(_simpleTags);
            foreach (KeyValuePair<Regex, String> pair in _complexTags)
                str = pair.Key.Replace(str, pair.Value);

            return str;
        }

        private static KeyValuePair<String, TextReplacement>[] GetSimpleTags()
        {
            return new Dictionary<String, TextReplacement>
            {
                {"[ENDN]", String.Empty},
                {"[ZDNE]", "{Zidane}"},
                {"[VIVI]", "{Vivi}"},
                {"[DGGR]", "{Dagger}"},
                {"[STNR]", "{Steiner}"},
                {"[FRYA]", "{Fraya}"},
                {"[QUIN]", "{Quina}"},
                {"[EIKO]", "{Eiko}"},
                {"[AMRT]", "{Amarant}"},
                {"[IMME]", "{Instantly}"},
                {"[FLIM]", "{Flash}"},
                {"[NFOC]", "{NoFocus}"},
                {"[NANI]", "{NoAnimation}"},
                {"[PAGE]", "{NewPage}"},
                {"[CHOO][MOVE=18,0]", "{Choice}"},
                {"[MOVE=18,0]", "{Tab}"},
                {"[C8C8C8][HSHD]", "{White}"},
                {"[B880E0][HSHD]", "{Pink}"},
                {"[68C0D8][HSHD]", "{Cyan}"},
                {"[D06050][HSHD]", "{Brown}"},
                {"[C8B040][HSHD]", "{Yellow}"},
                {"[78C840][HSHD]", "{Green}"},
                {"[909090][HSHD]", "{Grey}"},
                {"[INCS][TIME=-1]", "{IncreaseSignal}"},
                {"[INCS]", "{IncreaseSignal+}"},
                {"[TAIL=LOR]", "{LowerRight}"},
                {"[TAIL=LOL]", "{LowerLeft}"},
                {"[TAIL=UPR]", "{UpperRight}"},
                {"[TAIL=UPL]", "{UpperLeft}"},
                {"[TAIL=LOC]", "{LowerCenter}"},
                {"[TAIL=UPC]", "{UpperCenter}"},
                {"[TAIL=LORF]", "{LowerRightForce}"},
                {"[TAIL=LOLF]", "{LowerLeftForce}"},
                {"[TAIL=UPRF]", "{UpperRightForce}"},
                {"[TAIL=UPLF]", "{UpperLeftForce}"},
                {"[TAIL=DEFT]", "{DialogPosition}"},
                {"[DBTN=UP]", "{Up}"},
                {"[DBTN=DOWN]", "{Down}"},
                {"[DBTN=LEFT]", "{Left}"},
                {"[DBTN=RIGHT]", "{Right}"},
                {"[DBTN=CIRCLE]", "{Circle}"},
                {"[DBTN=CROSS]", "{Cross}"},
                {"[DBTN=TRIANGLE]", "{Triangle}"},
                {"[DBTN=SQUARE]", "{Square}"},
                {"[DBTN=R1]", "{R1}"},
                {"[DBTN=R2]", "{R2}"},
                {"[DBTN=L1]", "{L1}"},
                {"[DBTN=L2]", "{L2}"},
                {"[DBTN=SELECT]", "{Select}"},
                {"[DBTN=START]", "{Start}"},
                {"[DBTN=PAD]", "{Pad}"},
                {"[CBTN=UP]", "{Up+}"},
                {"[CBTN=DOWN]", "{Down+}"},
                {"[CBTN=LEFT]", "{Left+}"},
                {"[CBTN=RIGHT]", "{Right+}"},
                {"[CBTN=CIRCLE]", "{Circle+}"},
                {"[CBTN=CROSS]", "{Cross+}"},
                {"[CBTN=TRIANGLE]", "{Triangle+}"},
                {"[CBTN=SQUARE]", "{Square+}"},
                {"[CBTN=R1]", "{R1+}"},
                {"[CBTN=R2]", "{R2+}"},
                {"[CBTN=L1]", "{L1+}"},
                {"[CBTN=L2]", "{L2+}"},
                {"[CBTN=SELECT]", "{Select+}"},
                {"[CBTN=START]", "{Start+}"},
                {"[CBTN=PAD]", "{Pad+}"},
                {"[PTY1]", "{Party 1}"},
                {"[PTY2]", "{Party 2}"},
                {"[PTY3]", "{Party 3}"},
                {"[PTY4]", "{Party 4}"},
                {"[SIGL=0]", "{Signal 0}"},
                {"[SIGL=1]", "{Signal 1}"},
                {"[SIGL=2]", "{Signal 2}"}
            }.ToArray();
        }

        private KeyValuePair<Regex, String>[] GetComplexTags()
        {
            return new Dictionary<Regex, String>
            {
                {new Regex(@"\[STRT=([0-9]+),([0-9]+)\]"), "{W$1H$2}"},
                {new Regex(@"\[NUMB=([0-9]+)\]"), "{Variable $1}"},
                {new Regex(@"\[ICON=([0-9]+)\]"), "{Icon $1}"},
                {new Regex(@"\[PNEW=([0-9]+)\]"), "{Icon+ $1}"},
                {new Regex(@"\[SPED=(-?[0-9]+)\]"), "{Speed $1}"},
                {new Regex(@"\[TEXT=([^]]+)\]"), "{Text $1}"},
                {new Regex(@"\[WDTH=([^]]+)\]"), "{Widths $1}"},
                {new Regex(@"\[TIME=([^]]+)\]"), "{Time $1}"},
                {new Regex(@"\[WAIT=([^]]+)\]"), "{Wait $1}"},
                {new Regex(@"\[CENT=([^]]+)\]"), "{Center $1}"},
                {new Regex(@"\[ITEM=([^]]+)\]"), "{Item $1}"},
                {new Regex(@"\[PCHC=([^]]+)\]"), "{PreChoose $1}"},
                {new Regex(@"\[PCHM=([^]]+)\]"), "{PreChooseMask $1}"},
                {new Regex(@"\[MPOS=([^]]+)\]"), "{Position $1}"},
                {new Regex(@"\[OFFT=([^]]+)\]"), "{Offset $1}"},
                {new Regex(@"\[MOBI=([^]]+)\]"), "{Mobile $1}"},
                {new Regex(@"\[YADD=([^]]+)\]"), "{y$1}"},
                {new Regex(@"\[YSUB=([^]]+)\]"), "{y-$1}"},
                {new Regex(@"\[FEED=([^]]+)\]"), "{f$1}"},
                {new Regex(@"\[XTAB=([^]]+)\]"), "{x$1}"},
                {new Regex(@"\[TBLE=([^]]+)\]"), "{Table $1}"}
            }.ToArray();
        }
    }
}
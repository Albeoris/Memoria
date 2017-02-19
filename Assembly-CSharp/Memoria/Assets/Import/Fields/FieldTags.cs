using System;

namespace Memoria.Assets
{
    public abstract class FieldTags
    {
        public readonly TextReplacements SimpleTags;

        protected FieldTags()
        {
            SimpleTags = GetSimpleTags();
            SimpleTags.AddForward("[ENDN]", String.Empty);
            SimpleTags.AddBackward("\r\n", "\n");
            SimpleTags.AddBackward("\r", "\n");
            SimpleTags.Commit();
        }

        private static TextReplacements GetSimpleTags()
        {
            return new TextReplacements
            {
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
            };
        }
    }
}
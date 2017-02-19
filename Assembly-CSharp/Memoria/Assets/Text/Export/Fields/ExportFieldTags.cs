using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Memoria.Prime.Text;

namespace Memoria.Assets
{
    public sealed class ExportFieldTags : FieldTags
    {
        private readonly KeyValuePair<Regex, String>[] _complexTags;

        public ExportFieldTags()
        {
            _complexTags = GetComplexTags();
        }

        public String Replace(String str)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            str = str.ReplaceAll(SimpleTags.Forward);
            foreach (KeyValuePair<Regex, String> pair in _complexTags)
                str = pair.Key.Replace(str, pair.Value);

            return str;
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
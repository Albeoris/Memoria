using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Memoria.Prime;

namespace Memoria.Assets
{
    public sealed class FFIXTextTag
    {
        public const Int32 MaxTagLength = 32;

        public readonly FFIXTextTagCode Code;
        public readonly Int32[] Param;

        public FFIXTextTag(FFIXTextTagCode code)
        {
            Code = code;
            Param = null;
        }

        public FFIXTextTag(FFIXTextTagCode code, params Int32[] param)
        {
            Code = code;
            Param = param;
        }

        public static FFIXTextTag TryRead(Char[] chars, ref Int32 offset, ref Int32 left)
        {
            Int32 oldOffset = offset;
            Int32 oldleft = left;

            String tag, par;
            if (chars[offset++] != '{' || !TryGetTag(chars, ref offset, ref left, out tag, out par))
            {
                offset = oldOffset;
                left = oldleft;
                return null;
            }

            if (tag[0] == 'W' && Char.IsNumber(tag[1]))
            {
                String[] items = tag.Split('H');
                if (items.Length != 2)
                {
                    offset = oldOffset;
                    left = oldleft;
                    return null;
                }

                Int32 width, height;
                if (!Int32.TryParse(items[0].Substring(1), NumberStyles.Integer, CultureInfo.InvariantCulture, out width) ||
                    !Int32.TryParse(items[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out height))
                {
                    offset = oldOffset;
                    left = oldleft;
                    return null;
                }

                return new FFIXTextTag(FFIXTextTagCode.DialogSize, width, height);
            }

            if (tag[0] == 'y' && (tag[1] == '-' || Char.IsNumber(tag[1])))
            {
                Int32 value;
                if (!Int32.TryParse(tag.Substring(1), NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                {
                    offset = oldOffset;
                    left = oldleft;
                    return null;
                }

                return new FFIXTextTag(FFIXTextTagCode.DialogY, value);
            }

            if (tag[0] == 'x' && (tag[1] == '-' || Char.IsNumber(tag[1])))
            {
                Int32 value;
                if (!Int32.TryParse(tag.Substring(1), NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                {
                    offset = oldOffset;
                    left = oldleft;
                    return null;
                }

                return new FFIXTextTag(FFIXTextTagCode.DialogX, value);
            }

            if (tag[0] == 'f' && (tag[1] == '-' || Char.IsNumber(tag[1])))
            {
                Int32 value;
                if (!Int32.TryParse(tag.Substring(1), NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                {
                    offset = oldOffset;
                    left = oldleft;
                    return null;
                }

                return new FFIXTextTag(FFIXTextTagCode.DialogF, value);
            }

            FFIXTextTagCode? code;
            if (tag[tag.Length - 1] == '+')
                code = EnumCache<FFIXTextTagCode>.TryParse(tag.Substring(0, tag.Length - 1) + "Ex");
            else
                code = EnumCache<FFIXTextTagCode>.TryParse(tag);

            if (code == null)
            {
                offset = oldOffset;
                left = oldleft;
                return null;
            }

            if (String.IsNullOrEmpty(par))
                return new FFIXTextTag(code.Value);

            String[] values = par.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Int32[] args = new Int32[values.Length];
            for (Int32 i = 0; i < values.Length; i++)
            {
                Int32 numArg;
                if (!Int32.TryParse(values[i], NumberStyles.Integer | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out numArg))
                {
                    offset = oldOffset;
                    left = oldleft;
                    return null;
                }
                args[i] = numArg;
            }

            return new FFIXTextTag(code.Value, args);
        }

        private static Boolean TryGetTag(Char[] chars, ref Int32 offset, ref Int32 left, out String tag, out String par)
        {
            Int32 lastIndex = Array.IndexOf(chars, '}', offset);
            Int32 length = lastIndex - offset + 1;
            if (length < 2)
            {
                tag = null;
                par = null;
                return false;
            }

            left--;
            left -= length;

            Int32 spaceIndex = Array.IndexOf(chars, ' ', offset + 1, length - 2);
            if (spaceIndex < 0)
            {
                tag = new String(chars, offset, length - 1);
                par = String.Empty;
            }
            else
            {
                tag = new String(chars, offset, spaceIndex - offset);
                par = new String(chars, spaceIndex + 1, lastIndex - spaceIndex - 1);
            }

            offset = lastIndex + 1;
            return true;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder(MaxTagLength);

            switch (Code)
            {
                case FFIXTextTagCode.DialogSize:
                    return String.Format("{{W{0}H{1}}}", Param[0], Param[1]);
                case FFIXTextTagCode.DialogY:
                    return String.Format("{{y{0}}}", Param[0]);
                case FFIXTextTagCode.DialogX:
                    return String.Format("{{x{0}}}", Param[0]);
                case FFIXTextTagCode.DialogF:
                    return String.Format("{{f{0}}}", Param[0]);
            }

            sb.Append('{');
            if (EnumCache<FFIXTextTagCode>.IsDefined(Code))
            {
                if (Code == FFIXTextTagCode.Fraya)
                    sb.Append(FFIXTextTagCode.Freya);
                else
                    sb.Append(Code);
            }
            else
            {
                sb.Append("Unknown ").Append(((Byte)Code).ToString("X2"));
            }

            if (Param?.Length > 0)
            {
                sb.Append(' ');
                sb.Append(String.Join(",", Param.Select(p => p.ToString()).ToArray()));
            }

            sb.Append('}');
            return sb.ToString();
        }
    }
}

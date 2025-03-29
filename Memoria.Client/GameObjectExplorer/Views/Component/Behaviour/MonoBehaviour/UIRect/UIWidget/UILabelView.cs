using Memoria.Test;
using System;
using System.ComponentModel;
using System.Text;

namespace Memoria.Client
{
    public class UILabelView<T> : UIWidgetView<T> where T : UILabelMessage
    {
        public UILabelView(T message, RemoteGameObjects context)
            : base(message, context)
        {
        }

        [Category("UILabel")]
        [DisplayName("Text")]
        [Description("Unknown")]
        public String Text
        {
            get { return Native.Text; }
            set
            {
                if (Native.Text != value)
                {
                    Native.Text = value;
                    IReferenceMessage valueMessage = new StringMessage(value);
                    SendPropertyChanged(nameof(UILabel.rawText), valueMessage);
                    RaisePropertyChanged(nameof(Title));
                }
            }
        }

        public override String Title => FormatTitle();

        private String FormatTitle()
        {
            StringBuilder sb = new StringBuilder(40);
            String text = Native.Text;
            for (Int32 index = 0; index < Math.Min(15, text.Length); index++)
            {
                Char ch = text[index];
                switch (ch)
                {
                    case ' ':
                    case '.':
                    case ',':
                    case ';':
                    case '-':
                    case '{':
                    case '}':
                    case '(':
                    case ')':
                    case '[':
                    case ']':
                        sb.Append(ch);
                        break;
                    case '\n':
                    case '\t':
                        sb.Append(' ');
                        break;
                    default:
                        if (Char.IsLetterOrDigit(ch))
                            sb.Append(ch);
                        break;
                }
            }

            sb.Append(' ');
            sb.Append('(');
            sb.Append(Native.TypeName);
            sb.Append(')');

            return sb.ToString();
        }
    }
}

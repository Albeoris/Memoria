using System;
using System.IO;

namespace Memoria.Test
{
	public class UILabelMessage : UIWidgetMessage
	{
		public String Text;

		public UILabelMessage()
		{
		}

		public UILabelMessage(UILabel uiLabel)
			: base(uiLabel)
		{
			Text = uiLabel.text;
		}

		public override void Serialize(BinaryWriter bw)
		{
			base.Serialize(bw);

			bw.Write(Text);
		}

		public override void Deserialize(BinaryReader br)
		{
			base.Deserialize(br);

			Text = br.ReadString();
		}
	}
}

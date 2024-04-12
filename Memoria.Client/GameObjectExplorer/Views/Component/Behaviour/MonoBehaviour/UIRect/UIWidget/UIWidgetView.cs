using Memoria.Test;
using System;
using System.ComponentModel;

namespace Memoria.Client
{
	public class UIWidgetView<T> : UIRectView<T> where T : UIWidgetMessage
	{
		public UIWidgetView(T message, RemoteGameObjects context)
			: base(message, context)
		{
		}

		[Category("UIWidget")]
		[DisplayName("Width")]
		[Description("Unknown")]
		public Int32 Width
		{
			get { return Native.Width; }
			set
			{
				if (Native.Width != value)
				{
					Native.Width = value;
					SendPropertyChanged(nameof(UIWidget.width), new Int32Message(value));
				}
			}
		}

		[Category("UIWidget")]
		[DisplayName("Height")]
		[Description("Unknown")]
		public Int32 Height
		{
			get { return Native.Height; }
			set
			{
				if (Native.Height != value)
				{
					Native.Height = value;
					SendPropertyChanged(nameof(UIWidget.height), new Int32Message(value));
				}
			}
		}
	}
}

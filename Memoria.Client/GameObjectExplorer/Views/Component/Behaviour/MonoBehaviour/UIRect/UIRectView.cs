using Memoria.Client.GameObjectExplorer.Views.TypeEditors;
using Memoria.Test;
using System;
using System.ComponentModel;

namespace Memoria.Client
{
    public class UIRectView<T> : MonoBehaviourView<T> where T : UIRectMessage
    {
        public UIRectView(T message, RemoteGameObjects context)
            : base(message, context)
        {
        }

        [Category("UIRect")]
        [DisplayName("Alpha")]
        [Description("Unknown")]
        public Single Alpha
        {
            get { return Native.Alpha; }
            set
            {
                if (Native.Alpha != value)
                {
                    Native.Alpha = value;
                    IValueMessage valueMessage = new SingleMessage(value);
                    SendPropertyChanged(nameof(UIRect.alpha), valueMessage);
                }
            }
        }

        [Category("UIRect")]
        [DisplayName("UpdateAnchors")]
        [Description("Unknown")]
        public UIRect.AnchorUpdate UpdateAnchors
        {
            get { return Native.UpdateAnchors; }
            set
            {
                if (Native.UpdateAnchors != value)
                {
                    Native.UpdateAnchors = value;
                    IValueMessage valueMessage = new Int32Message((Int32)value);
                    SendPropertyChanged(nameof(UIRect.updateAnchors), valueMessage);
                }
            }
        }

        [Category("UIRect")]
        [DisplayName("LeftAnchor")]
        [Description("Unknown")]
        [Editor(typeof(UiRectPositionEditor), typeof(UiRectPositionEditor))]
        public UIRect.Position LeftAnchor
        {
            get { return Native.LeftAnchor; }
            set
            {
                if (Native.LeftAnchor != value)
                {
                    Native.LeftAnchor = value;
                    IValueMessage valueMessage = new UiRectPositionMessage(value);
                    SendPropertyChanged(nameof(UIRect.LeftAnchorPosition), valueMessage);
                }
            }
        }

        [Category("UIRect")]
        [DisplayName("RightAnchor")]
        [Description("Unknown")]
        [Editor(typeof(UiRectPositionEditor), typeof(UiRectPositionEditor))]
        public UIRect.Position RightAnchor
        {
            get { return Native.RightAnchor; }
            set
            {
                if (Native.RightAnchor != value)
                {
                    Native.RightAnchor = value;
                    IValueMessage valueMessage = new UiRectPositionMessage(value);
                    SendPropertyChanged(nameof(UIRect.RightAnchorPosition), valueMessage);
                }
            }
        }

        [Category("UIRect")]
        [DisplayName("BottomAnchor")]
        [Description("Unknown")]
        [Editor(typeof(UiRectPositionEditor), typeof(UiRectPositionEditor))]
        public UIRect.Position BottomAnchor
        {
            get { return Native.BottomAnchor; }
            set
            {
                if (Native.BottomAnchor != value)
                {
                    Native.BottomAnchor = value;
                    IValueMessage valueMessage = new UiRectPositionMessage(value);
                    SendPropertyChanged(nameof(UIRect.BottomAnchorPosition), valueMessage);
                }
            }
        }

        [Category("UIRect")]
        [DisplayName("TopAnchor")]
        [Description("Unknown")]
        [Editor(typeof(UiRectPositionEditor), typeof(UiRectPositionEditor))]
        public UIRect.Position TopAnchor
        {
            get { return Native.TopAnchor; }
            set
            {
                if (Native.TopAnchor != value)
                {
                    Native.TopAnchor = value;
                    IValueMessage valueMessage = new UiRectPositionMessage(value);
                    SendPropertyChanged(nameof(UIRect.TopAnchorPosition), valueMessage);
                }
            }
        }
    }
}

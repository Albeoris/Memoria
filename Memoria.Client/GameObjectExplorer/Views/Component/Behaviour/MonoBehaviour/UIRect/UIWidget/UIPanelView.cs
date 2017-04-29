using System;
using System.ComponentModel;
using Memoria.Client.GameObjectExplorer.Views.TypeEditors;
using Memoria.Test;
using UnityEngine;

namespace Memoria.Client
{
    public class UIPanelView<T> : UIRectView<T> where T : UIPanelMessage
    {
        public UIPanelView(T message, RemoteGameObjects context)
            : base(message, context)
        {
        }

        [Category("UIPanel")]
        [DisplayName("Width")]
        [Description("Unknown")]
        public Single Width => Native.Width;

        [Category("UIPanel")]
        [DisplayName("Height")]
        [Description("Unknown")]
        public Single Height => Native.Height;

        [Category("UIPanel")]
        [DisplayName("Depth")]
        [Description("Unknown")]
        public Int32 Depth
        {
            get { return Native.Depth; }
            set
            {
                if (Native.Depth != value)
                {
                    Native.Depth = value;
                    Int32Message valueMessage = new Int32Message(value);
                    SendPropertyChanged(nameof(UIPanel.depth), valueMessage);
                }
            }
        }

        [Category("UIPanel")]
        [DisplayName("SortingOrder")]
        [Description("Unknown")]
        public Int32 SortingOrder
        {
            get { return Native.SortingOrder; }
            set
            {
                if (Native.SortingOrder != value)
                {
                    Native.SortingOrder = value;
                    Int32Message valueMessage = new Int32Message(value);
                    SendPropertyChanged(nameof(UIPanel.sortingOrder), valueMessage);
                }
            }
        }

        [Category("UIPanel")]
        [DisplayName("Clipping")]
        [Description("Unknown")]
        public UIDrawCall.Clipping Clipping
        {
            get { return Native.Clipping; }
            set
            {
                if (Native.Clipping != value)
                {
                    Native.Clipping = value;
                    Int32Message valueMessage = new Int32Message((Int32)value);
                    SendPropertyChanged(nameof(UIPanel.clipping), valueMessage);
                }
            }
        }

        [Category("UIPanel")]
        [DisplayName("ClipOffset")]
        [Description("Unknown")]
        [Editor(typeof(Vector2Editor), typeof(Vector2Editor))]
        public Vector2 ClipOffset
        {
            get { return Native.ClipOffset; }
            set
            {
                if (Native.ClipOffset != value)
                {
                    Native.ClipOffset = value;
                    Vector2Message valueMessage = new Vector2Message(value);
                    SendPropertyChanged(nameof(UIPanel.clipping), valueMessage);
                }
            }
        }

        [Category("UIPanel")]
        [DisplayName("ClipRegion")]
        [Description("Unknown")]
        public Vector4 ClipRegion
        {
            get { return Native.ClipRegion; }
            set
            {
                if (Native.ClipRegion != value)
                {
                    Native.ClipRegion = value;
                    Vector4Message valueMessage = new Vector4Message(value);
                    SendPropertyChanged(nameof(UIPanel.baseClipRegion), valueMessage);
                }
            }
        }

        [Category("UIPanel")]
        [DisplayName("ClipSoftness")]
        [Description("Unknown")]
        [Editor(typeof(Vector2Editor), typeof(Vector2Editor))]
        public Vector2 ClipSoftness
        {
            get { return Native.ClipSoftness; }
            set
            {
                if (Native.ClipSoftness != value)
                {
                    Native.ClipSoftness = value;
                    Vector2Message valueMessage = new Vector2Message(value);
                    SendPropertyChanged(nameof(UIPanel.clipSoftness), valueMessage);
                }
            }
        }
    }
}
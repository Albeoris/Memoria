using System;
using System.ComponentModel;
using Memoria.Client.GameObjectExplorer.Views.TypeEditors;
using Memoria.Test;
using UnityEngine;

namespace Memoria.Client
{
    public class UITableView<T> : UIWidgetContainerView<T> where T : UITableMessage
    {
        public UITableView(T message, RemoteGameObjects context)
            : base(message, context)
        {
        }

        [Category("UITable")]
        [DisplayName("Columns")]
        [Description("Unknown")]
        public Int32 Columns
        {
            get { return Native.Columns; }
            set
            {
                if (Native.Columns != value)
                {
                    Native.Columns = value;
                    SendPropertyChanged(nameof(UITable.columns), new Int32Message(value));
                    SendPropertyChanged(nameof(UITable.repositionNow), new BooleanMessage(true));
                }
            }
        }

        [Category("UITable")]
        [DisplayName("Direction")]
        [Description("Unknown")]
        public UITable.Direction Direction
        {
            get { return Native.Direction; }
            set
            {
                if (Native.Direction != value)
                {
                    Native.Direction = value;
                    SendPropertyChanged(nameof(UITable.direction), new Int32Message((Int32)value));
                    SendPropertyChanged(nameof(UITable.repositionNow), new BooleanMessage(true));
                }
            }
        }

        [Category("UITable")]
        [DisplayName("Sorting")]
        [Description("Unknown")]
        public UITable.Sorting Sorting
        {
            get { return Native.Sorting; }
            set
            {
                if (Native.Sorting != value)
                {
                    Native.Sorting = value;
                    SendPropertyChanged(nameof(UITable.sorting), new Int32Message((Int32)value));
                    SendPropertyChanged(nameof(UITable.repositionNow), new BooleanMessage(true));
                }
            }
        }

        [Category("UITable")]
        [DisplayName("Pivot")]
        [Description("Unknown")]
        public UIWidget.Pivot Pivot
        {
            get { return Native.Pivot; }
            set
            {
                if (Native.Pivot != value)
                {
                    Native.Pivot = value;
                    SendPropertyChanged(nameof(UITable.pivot), new Int32Message((Int32)value));
                    SendPropertyChanged(nameof(UITable.repositionNow), new BooleanMessage(true));
                }
            }
        }

        [Category("UITable")]
        [DisplayName("CellAlignment")]
        [Description("Unknown")]
        public UIWidget.Pivot CellAlignment
        {
            get { return Native.CellAlignment; }
            set
            {
                if (Native.CellAlignment != value)
                {
                    Native.CellAlignment = value;
                    SendPropertyChanged(nameof(UITable.cellAlignment), new Int32Message((Int32)value));
                    SendPropertyChanged(nameof(UITable.repositionNow), new BooleanMessage(true));
                }
            }
        }

        [Category("UITable")]
        [DisplayName("HideInactive")]
        [Description("Unknown")]
        public Boolean HideInactive
        {
            get { return Native.HideInactive; }
            set
            {
                if (Native.HideInactive != value)
                {
                    Native.HideInactive = value;
                    SendPropertyChanged(nameof(UITable.hideInactive), new BooleanMessage(value));
                    SendPropertyChanged(nameof(UITable.repositionNow), new BooleanMessage(true));
                }
            }
        }

        [Category("UITable")]
        [DisplayName("KeepWithinPanel")]
        [Description("Unknown")]
        public Boolean KeepWithinPanel
        {
            get { return Native.KeepWithinPanel; }
            set
            {
                if (Native.KeepWithinPanel != value)
                {
                    Native.KeepWithinPanel = value;
                    SendPropertyChanged(nameof(UITable.keepWithinPanel), new BooleanMessage(value));
                    SendPropertyChanged(nameof(UITable.repositionNow), new BooleanMessage(true));
                }
            }
        }

        [Category("UITable")]
        [DisplayName("Padding")]
        [Description("Unknown")]
        [Editor(typeof(Vector2Editor), typeof(Vector2Editor))]
        public Vector2 Padding
        {
            get { return Native.Padding; }
            set
            {
                if (Native.Padding != value)
                {
                    Native.Padding = value;
                    SendPropertyChanged(nameof(UITable.padding), new Vector2Message(value));
                    SendPropertyChanged(nameof(UITable.repositionNow), new BooleanMessage(true));
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Memoria.Test;
using WpfControl = System.Windows.Controls.Control;

namespace Memoria.Client
{
    public abstract class ObjectAbstractView : INotifyPropertyChanged
    {
        public readonly ObjectMessage Message;

        protected ObjectAbstractView(ObjectMessage native)
        {
            Message = native;
        }

        public abstract String Title { get; }

        // Binding
        [Browsable(false)]
        public ListCollectionView BindableChilds => EnumerateChilds();

        [Browsable(false)]
        public ContextMenu BindableContextMenu
        {
            get
            {
                WpfControl[] menuItems = EnumerateContextMenuItems().ToArray();
                if (menuItems.Length == 0)
                    return null;

                ContextMenu menu = new ContextMenu {ItemsSource = menuItems};
                return menu;
            }
        }

        protected abstract ListCollectionView EnumerateChilds();

        protected virtual IEnumerable<WpfControl> EnumerateContextMenuItems()
        {
            yield break;
        }

        protected delegate Boolean StringToValueConverter<T>(String value, out T result);

        private String ReadArrayAsString<T>(T[] array, Func<T, String> valueToString)
        {
            if (array == null)
                return null;

            return String.Join(", ", array.Select(valueToString));
        }

        protected void WriteArrayFromString<T>(T[] array, String value, StringToValueConverter<T> stringToValue)
        {
            if (array == null)
                return;

            Int32 index = 0;
            foreach (String val in (value ?? String.Empty).Split(','))
            {
                if (index >= array.Length)
                    break;

                T number;
                if (stringToValue(val, out number))
                    array[index] = number;

                index++;
            }
        }

        protected String ReadInt32ArrayAsString(Int32[] array)
        {
            return ReadArrayAsString(array, v => v.ToString("X8"));
        }

        protected void WriteInt32ArrayFromString(Int32[] array, String value)
        {
            WriteArrayFromString(array, value, TryParseInt32);
        }

        private static Boolean TryParseInt32(String value, out Int32 result)
        {
            return Int32.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
        }

        protected internal void SendPropertyChanged(String memberName, IValueMessage value)
        {
            ChangeValueCommandMessage message = new ChangeValueCommandMessage(Message.InstanceId, memberName, value);
            NetworkClient.Execute(message);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected internal void RaisePropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public abstract class ObjectAbstractView<TNative> : ObjectAbstractView where TNative : ObjectMessage
    {
        protected internal readonly TNative Native;
        protected internal readonly RemoteGameObjects Context;

        protected ObjectAbstractView(TNative native, RemoteGameObjects context)
            :base(native)
        {
            Native = native;
            Context = context;
        }

        public override String Title => $"{Native.Name} ({Native.TypeName})";

        protected override ListCollectionView EnumerateChilds()
        {
            return Context.GetChildObjectsView(Native.InstanceId);
        }

        public override String ToString()
        {
            return Title;
        }
    }
}
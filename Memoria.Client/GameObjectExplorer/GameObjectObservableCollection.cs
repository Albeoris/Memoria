using Memoria.Test;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Threading;

namespace Memoria.Client
{
	public sealed class GameObjectObservableCollection : AbstractList, INotifyCollectionChanged
	{
		private readonly Dictionary<Int32, Int32> _keyToIndex = new Dictionary<Int32, Int32>();
		private readonly List<ObjectAbstractView> _values = new List<ObjectAbstractView>();
		private readonly Int32 _instanceId;
		private readonly RemoteGameObjects _context;
		private readonly Dispatcher _dispatcher;

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public GameObjectObservableCollection(Int32 instanceId, RemoteGameObjects context, IEnumerable<IRemotingMessage> messageCollection)
		{
			_instanceId = instanceId;
			_context = context;
			_dispatcher = Application.Current.Dispatcher;

			foreach (IRemotingMessage item in messageCollection)
			{
				ObjectMessage message = (ObjectMessage)item;
				_keyToIndex.Add(message.InstanceId, _values.Count);
				_values.Add(Wrap(message));
			}
		}

		public override Int32 Count => _values.Count;

		public GameObjectListCollectionView CreateView()
		{
			return new GameObjectListCollectionView(this);
		}

		public override Object this[Int32 index]
		{
			get
			{
				lock (_values) return _values[index];
			}
			set { throw new NotSupportedException(); }
		}

		public void Add(ObjectMessage message)
		{
			if (!TryAdd(message))
				throw new InvalidOperationException();
		}

		public Boolean TryAdd(ObjectMessage message)
		{
			lock (_values)
			{
				if (_keyToIndex.ContainsKey(message.InstanceId))
					return false;

				ObjectAbstractView item = Wrap(message);
				Int32 index = _keyToIndex.Count;
				_keyToIndex.Add(message.InstanceId, index);
				_values.Add(item);

				NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] { item }, index);
				RaiseCollectionChanged(args);
				return true;
			}
		}

		public override Int32 IndexOf(Object value)
		{
			ObjectAbstractView view = value as ObjectAbstractView;
			if (view == null)
				return -1;

			lock (_values)
			{
				Int32 index;
				if (_keyToIndex.TryGetValue(view.Message.InstanceId, out index))
					return index;

				return -1;
			}
		}

		public override IEnumerator GetEnumerator()
		{
			lock (_values)
				return _values.ToArray().GetEnumerator();
		}

		private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			if (CollectionChanged != null)
			{
				_dispatcher.BeginInvoke(new Action(() =>
				{
					CollectionChanged?.Invoke(this, args);
				}), DispatcherPriority.Send);
			}
		}

		private ObjectAbstractView Wrap(ObjectMessage message)
		{
			return ObjectViewFactory.Wrap(message, _context);
		}
	}
}

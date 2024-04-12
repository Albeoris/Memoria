using Memoria.Test;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Windows.Data;

namespace Memoria.Client
{
	public sealed class RemoteGameObjects
	{
		private readonly MessageFactory _messageFactory = new MessageFactory();

		private readonly ChildCollection _childs;
		private readonly ConcurrentDictionary<MessageCollection, GameObjectObservableCollection> _observableCollections = new ConcurrentDictionary<MessageCollection, GameObjectObservableCollection>();

		public event Action<LinkedList<ObjectMessage>> Added;

		public RemoteGameObjects()
		{
			_childs = new ChildCollection(this);
		}

		public void ProcessGameObjectMessages(BinaryReader br)
		{
			LinkedList<ObjectMessage> newObjects = new LinkedList<ObjectMessage>();

			Int32 objectsCount = br.ReadInt32();
			GameObjectMessage[] objectMessages = new GameObjectMessage[objectsCount];
			for (Int32 o = 0; o < objectsCount; o++)
			{
				GameObjectMessage objectMessage = new GameObjectMessage();
				objectMessage.Deserialize(br);

				//if (br.ReadInt32() != objectMessage.CheckField1)
				//    throw new InvalidDataException();

				objectMessages[o] = objectMessage;

				// Add as child to the parent
				MessageCollection childList = _childs.GetOrAdd(objectMessage.ParentInstanceId);
				if (childList.Replace(objectMessage) == StoreMessageResult.Added)
					newObjects.AddLast(objectMessage);

				// Get childs collection
				childList = _childs.GetOrAdd(objectMessage.InstanceId);

				Int32 componentCount = br.ReadInt32();
				ComponentMessage[] componentMessages = new ComponentMessage[componentCount];
				for (Int32 c = 0; c < componentCount; c++)
				{
					Int32 componentIndex = br.ReadInt32();
					ComponentMessage componentMessage = _messageFactory.CreateComponentMessage(componentIndex);
					componentMessage.Deserialize(br);

					//if (br.ReadInt32() != componentMessage.CheckField1)
					//    throw new InvalidDataException();

					componentMessages[c] = componentMessage;

					if (childList.Replace(componentMessage) == StoreMessageResult.Added)
						newObjects.AddLast(componentMessage);
				}
			}

			Added?.Invoke(newObjects);
		}

		public ListCollectionView GetRootObjectsView()
		{
			return GetChildObjectsView(0);
		}

		public ListCollectionView GetChildObjectsView(Int32 parentId)
		{
			return GetChildObjects(parentId).CreateView();
		}

		public GameObjectObservableCollection GetChildObjects(Int32 parentId)
		{
			MessageCollection collection = _childs.GetOrAdd(parentId);

			return _observableCollections.GetOrAdd(collection, c => c.CreateObservableView());
		}

		private enum StoreMessageResult
		{
			Added = 1,
			Updated = 2,
			Removed = 3
		}

		private sealed class ChildCollection : ConcurrentDictionary<Int32, MessageCollection>
		{
			private RemoteGameObjects _context;

			public ChildCollection(RemoteGameObjects context)
			{
				_context = context;
			}

			public MessageCollection GetOrAdd(Int32 parentInstanceId)
			{
				return GetOrAdd(parentInstanceId, CreateMessageCollection);
			}

			private MessageCollection CreateMessageCollection(Int32 parentInstanceId)
			{
				return new MessageCollection(parentInstanceId, _context);
			}
		}

		private sealed class MessageCollection : ConcurrentDictionary<Int32, IRemotingMessage>
		{
			private readonly Int32 _instanceId;
			private readonly RemoteGameObjects _context;

			private event Action<ObjectMessage> Added;

			public MessageCollection(Int32 instanceId, RemoteGameObjects context)
			{
				_instanceId = instanceId;
				_context = context;
			}

			public StoreMessageResult Replace(ObjectMessage objectMessage)
			{
				Boolean updated = false;
				this.AddOrUpdate(objectMessage.InstanceId, objectMessage, (id, old) =>
				{
					updated = true;
					return objectMessage;
				});

				if (updated)
				{
					return StoreMessageResult.Updated;
				}
				else
				{
					Added?.Invoke(objectMessage);
					return StoreMessageResult.Added;
				}
			}

			public GameObjectObservableCollection CreateObservableView()
			{
				GameObjectObservableCollection result = new GameObjectObservableCollection(_instanceId, _context, Values);
				Added += result.Add;
				return result;
			}
		}
	}
}

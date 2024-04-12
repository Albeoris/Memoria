using System;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria.Test
{
	public sealed partial class MessageFactory
	{
		public GameObjectMessage CreateGameObjectMessage(GameObject obj)
		{
			return new GameObjectMessage(obj);
		}

		public ScriptableObjectMessage CreateScriptableObjectMessage(ScriptableObject obj)
		{
			return new ScriptableObjectMessage(obj);
		}

		public ComponentMessage CreateComponentMessage(Component obj)
		{
			Type type = obj.GetType();

			Int32 index;
			ComponentMessage result;
			ComponentFactoryWrapDelegate wrapper;
			if (ComponentFactories.TryGetWrapper(type, out index, out wrapper))
			{
				result = wrapper(obj);
				result.ComponentIndex = index;
				return result;
			}

			List<Type> types = new List<Type>();
			do
			{
				types.Add(type);
				type = type.BaseType;

				if (type == null)
					throw new InvalidOperationException("Something went wrong.");
			} while (!ComponentFactories.TryGetIndex(type, out index));

			foreach (Type t in types)
				ComponentFactories.Link(t, index);

			wrapper = ComponentFactories.GetWrapper(index);
			result = wrapper(obj);
			result.ComponentIndex = index;
			return result;
		}

		public ComponentMessage CreateComponentMessage(Int32 index)
		{
			ComponentFactoryCreateDelegate creator;
			if (ComponentFactories.TryGetCreator(index, out creator))
			{
				ComponentMessage result = creator();
				result.ComponentIndex = index;
				return result;
			}

			return null;
		}

		private static readonly ComponentFactories ComponentFactories = new ComponentFactories()
		{
			{typeof(Component), () => new ComponentMessage(), obj => new ComponentMessage(obj)},
			{typeof(Transform), () => new TransformMessage(), obj => new TransformMessage((Transform)obj)},
			{typeof(RectTransform), () => new RectTransformMessage(), obj => new RectTransformMessage((RectTransform)obj)},
			{typeof(Behaviour), () => new BehaviourMessage(), obj => new BehaviourMessage((Behaviour)obj)},
			{typeof(MonoBehaviour), () => new MonoBehaviourMessage(), obj => new MonoBehaviourMessage((MonoBehaviour)obj)},
			{typeof(Camera), ()=> new CameraMessage(), obj => new CameraMessage((Camera)obj)},
			{typeof(UIRect), () => new UIRectMessage(), obj => new UIRectMessage((UIRect)obj)},
			{typeof(UIWidget), () => new UIWidgetMessage(), obj => new UIWidgetMessage((UIWidget)obj)},
			{typeof(UIPanel), () => new UIPanelMessage(), obj => new UIPanelMessage((UIPanel)obj)},
			{typeof(UILabel), () => new UILabelMessage(), obj => new UILabelMessage((UILabel)obj)},
			{typeof(UILocalize), () => new UILocalizeMessage(), obj => new UILocalizeMessage((UILocalize)obj)},
			{typeof(UIWidgetContainer), () => new UIWidgetContainerMessage(), obj => new UIWidgetContainerMessage((UIWidgetContainer)obj)},
			{typeof(UITable), () => new UITableMessage(), obj => new UITableMessage((UITable)obj)}
		};
	}
}

using Memoria.Test;
using System;
using System.Collections.Generic;

namespace Memoria.Client
{
	public static class ObjectViewFactory
	{
		public static ObjectAbstractView Wrap(ObjectMessage message, RemoteGameObjects context)
		{
			Type type = message.GetType();
			return Wrappers[type](message, context);
		}

		private delegate ObjectAbstractView WrapMessageDelegate(ObjectMessage message, RemoteGameObjects context);

		private static readonly Dictionary<Type, WrapMessageDelegate> Wrappers = new Dictionary<Type, WrapMessageDelegate>
		{
			{typeof(ObjectMessage), (message, context) => new ObjectView<ObjectMessage>(message, context)},

			{typeof(GameObjectMessage), (message, context) => new GameObjectView((GameObjectMessage)message, context)},

			{typeof(ScriptableObjectMessage), (message, context) => new ScriptableObjectView<ScriptableObjectMessage>((ScriptableObjectMessage)message, context)},

			{typeof(ComponentMessage), (message, context) => new ComponentView<ComponentMessage>((ComponentMessage)message, context)},
			{typeof(TransformMessage), (message, context) => new TransformView<TransformMessage>((TransformMessage)message, context)},
			{typeof(RectTransformMessage), (message, context) => new RectTransformView<RectTransformMessage>((RectTransformMessage)message, context)},

			{typeof(BehaviourMessage), (message, context) => new BehaviourView<BehaviourMessage>((BehaviourMessage)message, context)},
			{typeof(MonoBehaviourMessage), (message, context) => new MonoBehaviourView<MonoBehaviourMessage>((MonoBehaviourMessage)message, context)},
			{typeof(CameraMessage), (message, context) => new CameraView<CameraMessage>((CameraMessage)message, context)},
			{typeof(UIRectMessage), (message, context) => new UIRectView<UIRectMessage>((UIRectMessage)message, context)},
			{typeof(UIWidgetMessage), (message, context) => new UIWidgetView<UIWidgetMessage>((UIWidgetMessage)message, context)},
			{typeof(UIPanelMessage), (message, context) => new UIPanelView<UIPanelMessage>((UIPanelMessage)message, context)},
			{typeof(UILabelMessage), (message, context) => new UILabelView<UILabelMessage>((UILabelMessage)message, context)},

			{typeof(UIWidgetContainerMessage), (message, context) => new UIWidgetContainerView<UIWidgetContainerMessage>((UIWidgetContainerMessage)message, context)},
			{typeof(UITableMessage), (message, context) => new UITableView<UITableMessage>((UITableMessage)message, context)},

			{typeof(UILocalizeMessage), (message, context) => new UILocalizeView<UILocalizeMessage>((UILocalizeMessage)message, context)}
		};
	}
}

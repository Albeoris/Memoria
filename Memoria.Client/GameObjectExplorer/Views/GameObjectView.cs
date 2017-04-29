using System;
using System.ComponentModel;
using Memoria.Test;

namespace Memoria.Client
{
    public class GameObjectView : ObjectView<GameObjectMessage>
    {
        public GameObjectView(GameObjectMessage message, RemoteGameObjects context)
            : base(message, context)
        {
        }

        [Category("GameObject")]
        [DisplayName("Tag")]
        [Description("The tag of this game object.")]
        public String Tag
        {
            get { return Native.Tag; }
            set { Native.Tag = value; }
        }

        [Category("GameObject")]
        [DisplayName("IsActive")]
        [Description("The local active state of this GameObject.")]
        public Boolean IsActive
        {
            get { return Native.IsActive; }
            set { Native.IsActive = value; }
        }

        [Category("GameObject")]
        [DisplayName("IsActiveInHierarchy")]
        [Description("Is the GameObject active in the scene?")]
        public Boolean IsActiveInHierarchy => Native.IsActiveInHierarchy;

        [Category("GameObject")]
        [DisplayName("ParentInstanceId")]
        [Description("The parent of the transform.")]
        public Int32 ParentInstanceId => Native.ParentInstanceId;
    }
}
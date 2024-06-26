using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using Memoria.Client.Interaction;
using Memoria.Test;
using UnityEngine;
using Object = System.Object;
using WpfControl = System.Windows.Controls.Control;

namespace Memoria.Client
{
    public class ObjectView<T> : ObjectAbstractView<T> where T : ObjectMessage
    {
        public ObjectView(T message, RemoteGameObjects context)
            : base(message, context)
        {
        }

        [Category("Object")]
        [DisplayName("InstanceId")]
        [Description("The instance id of an object is always guaranteed to be unique.")]
        public Int32 InstanceId => Native.InstanceId;

        [Category("Object")]
        [DisplayName("Name")]
        [Description("The name of the object.")]
        public String Name
        {
            get { return Native.Name; }
            set { Native.Name = value; }
        }

        [Category("Object")]
        [DisplayName("HideFlags")]
        [Description("Should the object be hidden, saved with the scene or modifiable by the user?")]
        public HideFlags HideFlags
        {
            get { return Native.HideFlags; }
            set { Native.HideFlags = value; }
        }

        protected override IEnumerable<WpfControl> EnumerateContextMenuItems()
        {
            Boolean hasItems = false;
            foreach (WpfControl command in base.EnumerateContextMenuItems())
            {
                hasItems = true;
                yield return command;
            }

            if (hasItems)
            {
                yield return new Separator();
            }

            yield return new MenuItem {Header = "Branch", Command = new BranchCommand(this)};
            yield return new MenuItem {Header = "Duplicate", Command = new DuplicateCommandMessage(Native.InstanceId)};
        }

        private class BranchCommand : ICommand
        {
            private ObjectView<T> _view;

            public BranchCommand(ObjectView<T> view)
            {
                _view = view;
            }

            public Boolean CanExecute(Object parameter)
            {
                return true;
            }

            public void Execute(Object parameter)
            {
                TreeView treeView = InteractionService.GameObjectsController.CreateNewTab(_view.Title);
                treeView.Items.Add(_view);
            }

            public event EventHandler CanExecuteChanged;
        }
    }
}
using System;
using System.ComponentModel;
using Memoria.Client.GameObjectExplorer.Views.TypeEditors;
using Memoria.Test;
using UnityEngine;

namespace Memoria.Client
{
    public class TransformView<T> : ComponentView<T> where T : TransformMessage
    {
        public TransformView(T message, RemoteGameObjects context)
            : base(message, context)
        {
        }

        [Category("Transform")]
        [DisplayName("LocalPosition")]
        [Description("Position of the transform relative to the parent transform.")]
        [Editor(typeof(Vector3Editor), typeof(Vector3Editor))]
        public Vector3 LocalPosition
        {
            get { return Native.LocalPosition; }
            set
            {
                {
                    Native.LocalPosition = value;
                    SendPropertyChanged(nameof(Transform.localPosition), new Vector3Message(value));
                }
            }
        }

        [Category("Transform")]
        [DisplayName("LocalScale")]
        [Description("The scale of the transform relative to the parent.")]
        [Editor(typeof(Vector3Editor), typeof(Vector3Editor))]
        public Vector3 LocalScale
        {
            get { return Native.LocalScale; }
            set
            {
                {
                    Native.LocalScale = value;
                    SendPropertyChanged(nameof(Transform.localScale), new Vector3Message(value));
                }
            }
        }

        [Category("Transform")]
        [DisplayName("LocalEulerAngles")]
        [Description("The rotation as Euler angles in degrees relative to the parent transform's rotation.")]
        [Editor(typeof(Vector3Editor), typeof(Vector3Editor))]
        public Vector3 LocalEulerAngles
        {
            get { return Native.LocalEulerAngles; }
            set
            {
                {
                    Native.LocalEulerAngles = value;
                    SendPropertyChanged(nameof(Transform.localEulerAngles), new Vector3Message(value));
                }
            }
        }

        [Category("Transform")]
        [DisplayName("LocalRotation")]
        [Description("The rotation of the transform relative to the parent transform's rotation.")]
        [Editor(typeof(QuaternionEditor), typeof(QuaternionEditor))]
        public Quaternion LocalRotation
        {
            get { return Native.LocalRotation; }
            set
            {
                {
                    Native.LocalRotation = value;
                    SendPropertyChanged(nameof(Transform.localRotation), new QuaternionMessage(value));
                }
            }
        }
    }
}
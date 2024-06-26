using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Memoria.Test
{
    internal sealed partial class DuplicateCommandMessage : CommandMessage
    {
        public override void Execute()
        {
            Object[] objects = Object.FindObjectsOfType<Object>();
            Object obj = objects.FirstOrDefault(o => o.GetInstanceID() == InstanceId);
            if (obj == null)
                return;

            GameObject gameObject = obj as GameObject;
            if (gameObject != null)
            {
                GameObject newObject = Object.Instantiate(gameObject);
                newObject.transform.SetParent(gameObject.transform.parent.transform, false);
            }
        }
    }
}

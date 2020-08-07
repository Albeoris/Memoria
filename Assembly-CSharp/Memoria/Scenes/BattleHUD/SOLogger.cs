using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Memoria.Prime.Text;
using UnityEngine;
using Object = System.Object;

namespace Memoria.Scenes.BattleHUD
{
    public class SOLogger
    {
        private StreamWriter _output;
        private Int32 _index = 0;

        public void Do(Object obj)
        {
            if (obj == null)
                return;

            Type type = obj.GetType();
            _output = File.CreateText(type.FullName.ReplaceChars("_", Path.GetInvalidFileNameChars()) + ".txt");

            try
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (FieldInfo field in fields)
                    Do(field.Name, field.GetValue(obj));

                PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (PropertyInfo property in properties)
                {
                    if (IsAutoProperty(property))
                        Do(property.Name, property.GetValue(obj, null));
                }

                if (obj is GameObject go)
                    DoGameObject(go.name, go);
            }
            finally
            {
                _output.Dispose();
            }
        }

        public static Boolean IsAutoProperty(PropertyInfo prop)
        {
            if (!prop.CanRead)
                return false;

            Type type = prop.DeclaringType;
            if (type == null)
                return false;

            return type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Any(f => f.Name.Contains("<" + prop.Name + ">"));
        }

        private void Do(String name, Object value)
        {
            Component component = value as Component;
            if (component != null)
                DoComponent(component.name, component);

            GameObject gameObject = value as GameObject;
            if (gameObject != null)
                DoGameObject(gameObject.name, gameObject);
        }

        private void DoGameObject(String name, GameObject gameObject)
        {
            if (gameObject == null)
            {
                WriteLine("O." + name + " is null");
                return;
            }

            WriteLine($"O.{name}: 0x{gameObject.GetHashCode():X}");
            _index++;

            foreach (Component component in gameObject.GetComponents<Component>())
                DoComponent(component.name, component);

            for (Int32 i = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject child = gameObject.GetChild(i);
                DoGameObject(child.name, child);
            }
            _index--;
        }

        private void DoComponent(String name, Component component)
        {
            if (component == null)
            {
                WriteLine("C." + name + " is null");
                return;
            }

            WriteLine($"C.{name} ({component.GetType().FullName}): 0x{component.GetHashCode():X}");
        }

        private void WriteLine(String text)
        {
            for (Int32 i = 0; i < _index; i++)
                _output.Write('\t');

            _output.WriteLine(text);
        }
    }
}
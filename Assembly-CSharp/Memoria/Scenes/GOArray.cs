using System;
using UnityEngine;

namespace Memoria.Scenes
{
    internal class GOArray<T> : GOBase where T : GOBase
    {
        public readonly T[] Entries;

        public GOArray(GameObject obj)
            : base(obj)
        {
            Entries = new T[obj.transform.childCount];
            for (Int32 i = 0; i < Entries.Length; i++)
                Entries[i] = Create<T>(obj.GetChild(i));
        }
    }
}
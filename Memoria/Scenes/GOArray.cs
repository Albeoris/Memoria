using UnityEngine;

namespace Memoria
{
    internal class GOArray<T> : GOBase where T : GOBase
    {
        public readonly T[] Entries;

        public GOArray(GameObject obj)
            : base(obj)
        {
            Entries = new T[obj.transform.childCount];
            for (int i = 0; i < Entries.Length; i++)
                Entries[i] = Create<T>(obj.GetChild(i));
        }
    }
}
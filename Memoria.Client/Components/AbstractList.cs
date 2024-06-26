using System;
using System.Collections;

namespace Memoria.Client
{
    public abstract class AbstractList : IList
    {
        public virtual Int32 Count { get { throw new NotSupportedException(); } }
        public virtual Object SyncRoot { get; } = new Object();
        public virtual Boolean IsSynchronized => false;
        public virtual Boolean IsReadOnly => false;
        public virtual Boolean IsFixedSize => false;

        public virtual Object this[Int32 index]
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public virtual IEnumerator GetEnumerator()
        {
            throw new NotSupportedException();
        }

        public virtual Int32 Add(Object value)
        {
            throw new NotSupportedException();
        }

        public virtual void Insert(Int32 index, Object value)
        {
            throw new NotSupportedException();
        }

        public virtual void CopyTo(Array array, Int32 index)
        {
            throw new NotSupportedException();
        }

        public virtual Boolean Contains(Object value)
        {
            throw new NotSupportedException();
        }

        public virtual Int32 IndexOf(Object value)
        {
            throw new NotSupportedException();
        }

        public virtual void Remove(Object value)
        {
            throw new NotSupportedException();
        }

        public virtual void RemoveAt(Int32 index)
        {
            throw new NotSupportedException();
        }

        public virtual void Clear()
        {
            throw new NotSupportedException();
        }
    }
}
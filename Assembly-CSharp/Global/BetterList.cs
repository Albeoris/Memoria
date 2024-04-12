using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BetterList<T>
{
	[DebuggerStepThrough]
	public IEnumerator<T> GetEnumerator()
	{
		if (this.buffer != null)
		{
			for (Int32 i = 0; i < this.size; i++)
			{
				yield return this.buffer[i];
			}
		}
		yield break;
	}

	[DebuggerHidden]
	public T this[Int32 i]
	{
		get
		{
			return this.buffer[i];
		}
		set
		{
			this.buffer[i] = value;
		}
	}

	private void AllocateMore()
	{
		T[] array = (this.buffer == null) ? new T[32] : new T[Mathf.Max((Int32)this.buffer.Length << 1, 32)];
		if (this.buffer != null && this.size > 0)
		{
			this.buffer.CopyTo(array, 0);
		}
		this.buffer = array;
	}

	private void Trim()
	{
		if (this.size > 0)
		{
			if (this.size < (Int32)this.buffer.Length)
			{
				T[] array = new T[this.size];
				for (Int32 i = 0; i < this.size; i++)
				{
					array[i] = this.buffer[i];
				}
				this.buffer = array;
			}
		}
		else
		{
			this.buffer = null;
		}
	}

	public void Clear()
	{
		this.size = 0;
	}

	public void Release()
	{
		this.size = 0;
		this.buffer = null;
	}

	public void Add(T item)
	{
		if (this.buffer == null || this.size == (Int32)this.buffer.Length)
		{
			this.AllocateMore();
		}
		this.buffer[this.size++] = item;
	}

	public void Insert(Int32 index, T item)
	{
		if (this.buffer == null || this.size == (Int32)this.buffer.Length)
		{
			this.AllocateMore();
		}
		if (index > -1 && index < this.size)
		{
			for (Int32 i = this.size; i > index; i--)
			{
				this.buffer[i] = this.buffer[i - 1];
			}
			this.buffer[index] = item;
			this.size++;
		}
		else
		{
			this.Add(item);
		}
	}

	public Boolean Contains(T item)
	{
		if (this.buffer == null)
		{
			return false;
		}
		for (Int32 i = 0; i < this.size; i++)
		{
			if (this.buffer[i].Equals(item))
			{
				return true;
			}
		}
		return false;
	}

	public Int32 IndexOf(T item)
	{
		if (this.buffer == null)
		{
			return -1;
		}
		for (Int32 i = 0; i < this.size; i++)
		{
			if (this.buffer[i].Equals(item))
			{
				return i;
			}
		}
		return -1;
	}

	public Boolean Remove(T item)
	{
		if (this.buffer != null)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (Int32 i = 0; i < this.size; i++)
			{
				if (@default.Equals(this.buffer[i], item))
				{
					this.size--;
					this.buffer[i] = default(T);
					for (Int32 j = i; j < this.size; j++)
					{
						this.buffer[j] = this.buffer[j + 1];
					}
					this.buffer[this.size] = default(T);
					return true;
				}
			}
		}
		return false;
	}

	public void RemoveAt(Int32 index)
	{
		if (this.buffer != null && index > -1 && index < this.size)
		{
			this.size--;
			this.buffer[index] = default(T);
			for (Int32 i = index; i < this.size; i++)
			{
				this.buffer[i] = this.buffer[i + 1];
			}
			this.buffer[this.size] = default(T);
		}
	}

	public T Pop()
	{
		if (this.buffer != null && this.size != 0)
		{
			T result = this.buffer[--this.size];
			this.buffer[this.size] = default(T);
			return result;
		}
		return default(T);
	}

	public T[] ToArray()
	{
		this.Trim();
		return this.buffer;
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	public void Sort(BetterList<T>.CompareFunc comparer)
	{
		Int32 num = 0;
		Int32 num2 = this.size - 1;
		Boolean flag = true;
		while (flag)
		{
			flag = false;
			for (Int32 i = num; i < num2; i++)
			{
				if (comparer(this.buffer[i], this.buffer[i + 1]) > 0)
				{
					T t = this.buffer[i];
					this.buffer[i] = this.buffer[i + 1];
					this.buffer[i + 1] = t;
					flag = true;
				}
				else if (!flag)
				{
					num = (Int32)((i != 0) ? (i - 1) : 0);
				}
			}
		}
	}

	public T[] buffer;

	public Int32 size;

	public delegate Int32 CompareFunc(T left, T right);
}

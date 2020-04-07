using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DynamicDataDisplay.Common
{
	[DebuggerDisplay("Count = {Count}")]
	public sealed class ResourcePool<T>
	{
		private readonly Stack<T> pool = new Stack<T>();

		public T Get()
		{
			if (pool.Count < 1)
			{
				return default(T);
			}
			else
			{
				return pool.Pop();
			}
		}

		public void Put(T item)
		{
			if (item == null)
				throw new ArgumentNullException("item");

			pool.Push(item);
		}

		public int Count
		{
			get { return pool.Count; }
		}

		public void Clear()
		{
			pool.Clear();
		}
	}
}

﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace DynamicDataDisplay.Charts.Maps
{
	/// <summary>
	/// Represents an unordered set of values.
	/// </summary>
	/// <typeparam name="T">Type of values.</typeparam>
	[DebuggerDisplay("Count = {Count}")]
	public sealed class Set<T> : IEnumerable<T>
	{
		private readonly Dictionary<T, Dummy> cache = new Dictionary<T, Dummy>();

		public void Add(T item)
		{
			lock (cache)
			{
				cache.Add(item, Dummy.Instance);
			}
		}

		public bool TryAdd(T item)
		{
			lock (cache)
			{
				if (!cache.ContainsKey(item))
				{
					cache.Add(item, Dummy.Instance);
					return true;
				}
				return false;
			}
		}

		public bool Remove(T item)
		{
			lock (cache)
			{
				bool contains = cache.ContainsKey(item);
				bool result = cache.Remove(item);

				if (contains)
				{
					Debug.Assert(!cache.ContainsKey(item));
				}
				return result;
			}
		}

		public bool Contains(T item)
		{
			return cache.ContainsKey(item);
		}

		private class Dummy
		{
			public static readonly Dummy Instance = new Dummy();
		}

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return cache.Keys.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		public void Clear()
		{
			cache.Clear();
		}

		public int Count
		{
			get { return cache.Count; }
		}
	}
}

using DynamicDataDisplay.Charts.Isolines;
using System.Collections.Generic;

namespace DynamicDataDisplay.Common.Auxiliary
{
	public static class DictionaryExtensions
	{
		public static void Add<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue value, params TKey[] keys)
		{
			foreach (var key in keys)
			{
				dict.Add(key, value);
			}
		}

		public static void Add(this Dictionary<int, Edge> dict, Edge value, params CellBitmask[] keys)
		{
			foreach (var key in keys)
			{
				dict.Add((int)key, value);
			}
		}
	}
}

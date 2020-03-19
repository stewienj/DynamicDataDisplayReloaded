using Microsoft.Research.DynamicDataDisplay.Charts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public static class ArrayExtensions
	{
		public static T Last<T>(this T[] array)
		{
			return array[array.Length - 1];
		}

		public static T[] CreateArray<T>(int length, T defaultValue)
		{
			T[] res = new T[length];
			for (int i = 0; i < res.Length; i++)
			{
				res[i] = defaultValue;
			}
			return res;
		}

		public static IEnumerable<Range<T>> GetPairs<T>(this IEnumerable<T> array)
		{
			if (array == null)
				throw new ArgumentNullException("array");

			var previous = array.FirstOrDefault();
			foreach (var next in array.Skip(1))
			{
				yield return new Range<T>(previous, next);
				previous = next;
			}
		}
	}
}

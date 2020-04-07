using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;

namespace DynamicDataDisplay.Common.Auxiliary
{
	public static class Extensions
	{
		public static IEnumerable<T> GetLastestConsumingEnumerable<T>(this BlockingCollection<T> collection)
		{
			foreach (T pos in collection.GetConsumingEnumerable())
			{
				T currentPos = pos;
				T nextPos;
				while (collection.TryTake(out nextPos))
				{
					currentPos = nextPos;
				}
				yield return currentPos;
			}
		}

		public static Point? MaxX(this IEnumerable<Point> points)
		{
			Point? value = null;
			foreach (Point xy in points)
			{
				if (!value.HasValue || xy.X > value.Value.X || double.IsNaN(value.Value.X))
				{
					value = xy;
				}
			}
			return value;
		}

		public static Point? MinX(this IEnumerable<Point> points)
		{
			//if (points == null) throw Error.ArgumentNull("points");
			Point? value = null;
			foreach (Point xy in points)
			{
				if (!value.HasValue || xy.X < value.Value.X || double.IsNaN(value.Value.X))
				{
					value = xy;
				}
			}
			return value;
		}

		public static Point? MaxY(this IEnumerable<Point> points)
		{
			Point? value = null;
			foreach (Point xy in points)
			{
				if (!value.HasValue || xy.Y > value.Value.Y || double.IsNaN(value.Value.Y))
				{
					value = xy;
				}
			}
			return value;
		}

		public static Point? MinY(this IEnumerable<Point> points)
		{
			Point? value = null;
			foreach (Point xy in points)
			{
				if (!value.HasValue || xy.Y < value.Value.Y || double.IsNaN(value.Value.Y))
				{
					value = xy;
				}
			}
			return value;
		}


		public static V TryGetValueOrNew<K, V>(this Dictionary<K, V> dictionary, K key, Func<V> newValueFactory)
		{
			if (dictionary.TryGetValue(key, out V value))
			{
				return value;
			}
			else
			{
				var newValue = newValueFactory();
				dictionary.Add(key, newValue);
				return newValue;
			}
		}


	}
}

using System;
using System.Collections.Generic;

namespace DynamicDataDisplay
{
	public static class ListExtensions
	{
		/// <summary>
		/// Gets last element of list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static T GetLast<T>(this List<T> list)
		{
			if (list == null) throw new ArgumentNullException("list");
			if (list.Count == 0) throw new InvalidOperationException(Strings.Exceptions.CannotGetLastElement);

			return list[list.Count - 1];
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Windows;

namespace DynamicDataDisplay.Common.DataSearch
{
	public class SortedXSearcher1d
	{
		private readonly IList<Point> collection;
		public SortedXSearcher1d(IList<Point> collection)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			this.collection = collection;
		}

		public SearchResult1d SearchXBetween(double x)
		{
			return SearchXBetween(x, SearchResult1d.Empty);
		}

		public SearchResult1d SearchXBetween(double x, SearchResult1d result)
		{
			if (collection.Count == 0)
				return SearchResult1d.Empty;

			int lastIndex = collection.Count - 1;

			if (x < collection[0].X)
				return SearchResult1d.Empty;
			else if (collection[lastIndex].X < x)
				return SearchResult1d.Empty;

			int startIndex = !result.IsEmpty ? Math.Min(result.Index, lastIndex) : 0;

			// searching ascending
			if (collection[startIndex].X < x)
			{
				for (int i = startIndex + 1; i <= lastIndex; i++)
					if (collection[i].X >= x)
						return new SearchResult1d { Index = i - 1 };
			}
			else // searching descending
			{
				for (int i = startIndex - 1; i >= 0; i--)
					if (collection[i].X <= x)
						return new SearchResult1d { Index = i };
			}

			throw new InvalidOperationException("Should not appear here.");
		}
	}
}

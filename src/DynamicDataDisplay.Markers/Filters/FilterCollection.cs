﻿using DynamicDataDisplay.Markers.DataSources;
using DynamicDataDisplay.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;

namespace DynamicDataDisplay.Charts.NewLine.Filters
{
	public sealed class FilterCollection : D3Collection<PointsFilter2d>, IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FilterCollection"/> class.
		/// </summary>
		public FilterCollection() { }

		protected override void OnItemAdding(PointsFilter2d item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
		}

		protected override void OnItemAdded(PointsFilter2d item)
		{
			item.Changed += OnItemChanged;
		}

		private void OnItemChanged(object sender, EventArgs e)
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		protected override void OnItemRemoving(PointsFilter2d item)
		{
			item.Changed -= OnItemChanged;
		}

		internal IEnumerable<Point> Filter(IEnumerable<Point> points, IDataSourceEnvironment environment)
		{
			foreach (var filter in Items)
			{
				filter.Environment = environment;
				points = filter.Filter(points);
			}

			return points;
		}

		#region IDisposable Members

		public void Dispose()
		{
			foreach (var filter in this)
			{
				filter.Dispose();
			}
		}

		#endregion
	}
}

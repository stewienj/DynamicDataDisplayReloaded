﻿using System;
using System.Collections.Generic;
using System.Windows;

namespace DynamicDataDisplay.Charts.Filters
{
	public abstract class PointsFilterBase : IPointsFilter
	{
		#region IPointsFilter Members

		public abstract List<Point> Filter(List<Point> points);

		public virtual void SetScreenRect(Rect screenRect) { }

		protected void RaiseChanged()
		{
			Changed.Raise(this);
		}
		public event EventHandler Changed;

		#endregion
	}
}

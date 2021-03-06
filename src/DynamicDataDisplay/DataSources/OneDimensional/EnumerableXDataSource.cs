﻿using System.Collections.Generic;

namespace DynamicDataDisplay.DataSources
{
	public sealed class EnumerableXDataSource<T> : EnumerableDataSource<T>
	{
		public EnumerableXDataSource(IEnumerable<T> data) : base(data) { }
	}
}

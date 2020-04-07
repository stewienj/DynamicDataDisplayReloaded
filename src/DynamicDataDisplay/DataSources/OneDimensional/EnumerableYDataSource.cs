using System.Collections.Generic;

namespace DynamicDataDisplay.DataSources
{
	public sealed class EnumerableYDataSource<T> : EnumerableDataSource<T>
	{
		public EnumerableYDataSource(IEnumerable<T> data) : base(data) { }
	}
}

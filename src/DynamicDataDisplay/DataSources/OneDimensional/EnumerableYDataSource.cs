using System.Collections.Generic;

namespace Microsoft.Research.DynamicDataDisplay.DataSources
{
	public sealed class EnumerableYDataSource<T> : EnumerableDataSource<T>
	{
		public EnumerableYDataSource(IEnumerable<T> data) : base(data) { }
	}
}

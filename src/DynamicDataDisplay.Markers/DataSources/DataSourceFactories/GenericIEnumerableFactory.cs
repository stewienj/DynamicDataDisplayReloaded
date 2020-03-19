using System;
using System.Collections.Generic;

namespace DynamicDataDisplay.Markers.DataSources.DataSourceFactories
{
	public class GenericIEnumerableFactory : DataSourceFactory
	{
		public override bool TryBuild(object data, out PointDataSourceBase dataSource)
		{
			dataSource = null;

			var types = IEnumerableHelper.GetGenericInterfaceArgumentTypes(data, typeof(IEnumerable<>));
			if (types != null && types.Length == 1)
			{
				Type genericIEnumerableType = typeof(GenericIEnumerableDataSource<>).MakeGenericType(types);
				var result = Activator.CreateInstance(genericIEnumerableType, data);
				dataSource = (PointDataSourceBase)result;
				return true;
			}

			return false;
		}
	}
}

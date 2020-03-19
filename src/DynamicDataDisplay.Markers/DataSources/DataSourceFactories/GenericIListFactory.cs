using System;
using System.Collections.Generic;

namespace DynamicDataDisplay.Markers.DataSources.DataSourceFactories
{
	public class GenericIListFactory : DataSourceFactory
	{
		public override bool TryBuild(object data, out PointDataSourceBase dataSource)
		{
			dataSource = null;

			var types = IEnumerableHelper.GetGenericInterfaceArgumentTypes(data, typeof(IList<>));

			if (types != null && types.Length == 1)
			{
				Type genericIListType = typeof(GenericIListDataSource<>).MakeGenericType(types);
				var result = Activator.CreateInstance(genericIListType, data);
				dataSource = (PointDataSourceBase)result;
				return true;
			}

			return false;
		}
	}
}

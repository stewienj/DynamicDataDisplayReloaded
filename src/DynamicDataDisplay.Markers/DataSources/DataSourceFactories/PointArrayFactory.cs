using System.Windows;

namespace DynamicDataDisplay.Markers.DataSources.DataSourceFactories
{
	public sealed class PointArrayFactory : DataSourceFactory
	{
		public override bool TryBuild(object data, out PointDataSourceBase dataSource)
		{
			dataSource = null;

			Point[] array = data as Point[];
			if (array != null)
			{
				dataSource = new PointArrayDataSource(array);
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}

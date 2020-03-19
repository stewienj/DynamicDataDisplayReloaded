namespace DynamicDataDisplay.Markers.DataSources.DataSourceFactories
{
	public abstract class DataSourceFactory
	{
		public abstract bool TryBuild(object data, out PointDataSourceBase dataSource);
	}
}

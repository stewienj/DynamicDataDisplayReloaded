using DynamicDataDisplay.Charts;
using System.Collections.Generic;
using System.Windows;

namespace DynamicDataDisplay.DataSources
{
	public static class DataSourceHelper
	{
		public static IEnumerable<Point> GetPoints(IPointDataSource dataSource)
		{
			return GetPoints(dataSource, null);
		}

		public static IEnumerable<Point> GetPoints(IPointDataSource dataSource, DependencyObject context)
		{
			if (dataSource == null)
				yield break;

			if (context == null)
				context = new DataSource2dContext();

			using (IPointEnumerator enumerator = dataSource.GetEnumerator(context))
			{
				Point p = new Point();
				while (enumerator.MoveNext())
				{
					enumerator.GetCurrent(ref p);
					yield return p;
					p = new Point();
				}
			}
		}
	}
}

using Microsoft.Research.DynamicDataDisplay.DataSources;
using System;
using System.Linq;

namespace LegendSample
{
	public static class DataSourceHelper
	{
		public static IPointDataSource CreateSineDataSource()
		{
			var xs = Enumerable.Range(0, 1000).Select(i => i * 0.1);
			var ys = xs.Select(x => Math.Sin(x));

			var xds = xs.AsXDataSource();
			var ds = xds.Join(ys.AsYDataSource());

			return ds;
		}

		public static IPointDataSource CreateDataSource(Func<double, double> function)
		{
			var xs = Enumerable.Range(0, 1000).Select(i => i * 0.1);
			var ys = xs.Select(x => function(x));

			var xds = xs.AsXDataSource();
			var ds = xds.Join(ys.AsYDataSource());

			return ds;
		}
	}
}

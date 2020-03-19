using DynamicDataDisplay.Markers.DataSources;
using DynamicDataDisplay.Markers.DataSources.DataSourceFactories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;

namespace DynamicDataDisplay.Tests.D3.Markers
{
	[TestClass]
	public class DataSourceFactoriesTest
	{
		[TestMethod]
		public void TestCreateDataSourceFromPointArray()
		{
			Point[] pts = new Point[] { new Point(0.1, 0.2) };
			var store = DataSourceFactoryStore.Current;
			var ds = store.BuildDataSource(pts);

			Assert.IsTrue(ds is PointArrayDataSource);
		}
	}
}

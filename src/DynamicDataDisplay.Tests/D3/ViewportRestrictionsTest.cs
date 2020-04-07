using DynamicDataDisplay;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DynamicDataDisplay.Test.D3
{
	[TestClass]
	public class ViewportRestrictionsTest
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void TestAddingNull()
		{
			ChartPlotter plotter = new ChartPlotter();
			bool thrown = false;
			try
			{
				plotter.Viewport.Restrictions.Add(null);
			}
			catch (ArgumentNullException)
			{
				thrown = true;
			}

			Assert.IsTrue(thrown);
		}
	}
}

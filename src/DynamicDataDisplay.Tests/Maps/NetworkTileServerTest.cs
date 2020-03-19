using Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DynamicDataDisplay.Tests.Maps
{
	[TestClass]
	public class NetworkTileServerTest
	{
		[TestMethod]
		public void TestGCCollectsServer()
		{
			WeakReference reference = new WeakReference(new OpenStreetMapServer());

			GC.Collect(2);
			GC.WaitForPendingFinalizers();
			GC.Collect(2);

			Assert.IsTrue(!reference.IsAlive);
		}
	}
}

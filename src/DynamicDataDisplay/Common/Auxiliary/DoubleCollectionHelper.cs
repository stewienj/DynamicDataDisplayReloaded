using System.Windows.Media;

namespace DynamicDataDisplay.Common.Auxiliary
{
	public static class DoubleCollectionHelper
	{
		public static DoubleCollection Create(params double[] collection)
		{
			return new DoubleCollection(collection);
		}
	}
}

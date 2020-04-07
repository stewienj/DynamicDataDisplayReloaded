using System.Windows;

namespace DynamicDataDisplay.Common.Auxiliary
{
	public static class VectorExtensions
	{
		public static Point ToPoint(this Vector vector)
		{
			return new Point(vector.X, vector.Y);
		}
	}
}

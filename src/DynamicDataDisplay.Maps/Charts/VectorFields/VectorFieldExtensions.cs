using DynamicDataDisplay.Charts;
using DynamicDataDisplay.DataSources;
using System.Windows;

namespace DynamicDataDisplay.Maps.Charts
{
	public static class VectorFieldExtensions
	{
		public static Range<double> GetMinMaxLength(this IDataSource2D<Vector> dataSource)
		{
			double minLength = double.PositiveInfinity;
			double maxLength = double.NegativeInfinity;

			int width = dataSource.Width;
			int height = dataSource.Height;
			for (int ix = 0; ix < width; ix++)
			{
				for (int iy = 0; iy < height; iy++)
				{
					var vector = dataSource.Data[ix, iy];
					var length = vector.Length;

					if (length < minLength)
						minLength = length;
					if (length > maxLength)
						maxLength = length;
				}
			}

			if (minLength > maxLength)
			{
				minLength = maxLength = 0;
			}
			return new Range<double>(minLength, maxLength);
		}
	}
}

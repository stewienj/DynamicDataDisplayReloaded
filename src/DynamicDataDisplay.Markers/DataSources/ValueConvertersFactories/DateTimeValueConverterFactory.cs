using DynamicDataDisplay.Markers.DataSources.ValueConverters;
using DynamicDataDisplay.Charts;
using System;
using System.Windows.Data;

namespace DynamicDataDisplay.Markers.DataSources.ValueConvertersFactories
{
	public class DateTimeValueConverterFactory : ValueConverterFactory
	{
		public override IValueConverter TryBuildConverter(Type dataType, IValueConversionContext context)
		{
			var plotter = context.Plotter;

			if (dataType == typeof(DateTime))
			{
				foreach (var child in plotter.Children)
				{
					DateTimeAxis axis = child as DateTimeAxis;
					if (axis != null)
					{
						return new DateTimeValueConverter(axis);
					}
				}
			}
			return null;
		}
	}
}

using System;
using System.Windows.Data;

namespace DynamicDataDisplay.Markers.DataSources.ValueConvertersFactories
{
	public abstract class ValueConverterFactory
	{
		public abstract IValueConverter TryBuildConverter(Type dataType, IValueConversionContext context);
	}
}

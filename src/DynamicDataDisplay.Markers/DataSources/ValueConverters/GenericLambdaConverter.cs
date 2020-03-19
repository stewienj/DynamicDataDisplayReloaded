using System;
using System.Globalization;
using System.Windows.Data;

namespace DynamicDataDisplay.Markers.DataSources.ValueConverters
{
	public class GenericLambdaConverter<TIn, TOut> : IValueConverter
	{
		private readonly Func<TIn, TOut> lambda;

		public GenericLambdaConverter(Func<TIn, TOut> lambda)
		{
			if (lambda == null)
				throw new ArgumentNullException("lambda");

			this.lambda = lambda;
		}

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			TIn arg = (TIn)value;
			TOut result = lambda(arg);
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}

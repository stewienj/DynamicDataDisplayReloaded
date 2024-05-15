using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace DynamicDataDisplay.FrequencyTimeline
{
    internal class DoubleToLog10Converter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double number && number > 0)
            {
                return Math.Log10(number);
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;

    }
}

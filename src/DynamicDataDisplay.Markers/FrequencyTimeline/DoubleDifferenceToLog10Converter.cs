using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace DynamicDataDisplay.FrequencyTimeline
{
    internal class DoubleDifferenceToLog10Converter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 2)
            {
                if (values[0] is double value1 && values[1] is double value2)
                {
                    return Math.Abs(Math.Log10(value2) - Math.Log10(value1));
                }
            }
            return 0;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}

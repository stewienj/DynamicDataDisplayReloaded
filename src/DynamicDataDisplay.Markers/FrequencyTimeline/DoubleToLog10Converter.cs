using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DynamicDataDisplay.FrequencyTimeline
{
    internal class DoubleToLog10Converter : IValueConverter
    {
        private static Lazy<DoubleToLog10Converter> _converter = new Lazy<DoubleToLog10Converter>(true);

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

        public static DoubleToLog10Converter Instance => _converter.Value;
    }
}

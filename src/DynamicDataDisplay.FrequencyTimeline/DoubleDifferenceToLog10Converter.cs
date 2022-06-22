using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DynamicDataDisplay.FrequencyTimeline
{
    internal class DoubleDifferenceToLog10Converter : IMultiValueConverter
    {
        private static Lazy<DoubleDifferenceToLog10Converter> _instance = new Lazy<DoubleDifferenceToLog10Converter>(true);

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

        public static DoubleDifferenceToLog10Converter Instance => _instance.Value;
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DynamicDataDisplay.FrequencyTimeline
{
    internal class DateTimeToMinutesConverter : IValueConverter
    {
        private static Lazy<DateTimeToMinutesConverter> _instance = new Lazy<DateTimeToMinutesConverter>(true);

        private static Lazy<DateTime> _zeroTime = new Lazy<DateTime>(true);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                return (dateTime - _zeroTime.Value).TotalMinutes;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static DateTimeToMinutesConverter Instance => _instance.Value;
    }
}

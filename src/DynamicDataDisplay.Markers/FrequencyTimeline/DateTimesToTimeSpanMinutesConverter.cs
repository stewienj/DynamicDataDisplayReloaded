using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DynamicDataDisplay.FrequencyTimeline
{
    /// <summary>
    /// Converts 2 DateTimes to a range in minutew
    /// </summary>
    internal class DateTimesToTimeSpanMinutesConverter : IMultiValueConverter
    {
        private static Lazy<DateTimesToTimeSpanMinutesConverter> _instance = new Lazy<DateTimesToTimeSpanMinutesConverter>(true);

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length ==2)
            {
                if (values[0] is DateTime start && values[1] is DateTime end)
                {
                    return (end - start).TotalMinutes;
                }
            }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static DateTimesToTimeSpanMinutesConverter Instance => _instance.Value;
    }

}

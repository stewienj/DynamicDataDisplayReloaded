using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace DynamicDataDisplay.FrequencyTimeline
{
    /// <summary>
    /// Converts 2 DateTimes to a range in minutew
    /// </summary>
    internal class DateTimesToTimeSpanMinutesConverterRestricted : MarkupExtension, IMultiValueConverter
    {
        private DateTimeToMinutesConverterRestricted _pointConverter = new DateTimeToMinutesConverterRestricted();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3)
            {
                var scopedStart = (double)_pointConverter.Convert([values[0], values[2]], targetType, "StartTime", culture);
                var scopedEnd = (double)_pointConverter.Convert([values[1], values[2]], targetType, "EndTime", culture);
                return scopedEnd - scopedStart;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

}

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace DynamicDataDisplay.FrequencyTimeline
{
    internal class DateTimeToMinutesConverterRestricted : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                if (values[0] is not DateTime dateTime)
                {
                    dateTime = parameter switch
                    {
                        "StartTime" => DateTime.MinValue,
                        "EndTime" => DateTime.MaxValue,
                        _ => throw new ArgumentException()
                    };
                }

                var totalMinutes = (dateTime - DateTime.MinValue).TotalMinutes;
                if (values[1] is DataRect dataRect)
                {
                    totalMinutes = Math.Max(totalMinutes, dataRect.XMin);
                    totalMinutes = Math.Min(totalMinutes, dataRect.XMax);
                }
                return totalMinutes;
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

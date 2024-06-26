﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace DynamicDataDisplay.FrequencyTimeline
{
    /// <summary>
    /// Converts 2 DateTimes to a range in minutew
    /// </summary>
    internal class DateTimesToTimeSpanMinutesConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                if (values[0] is not DateTime start)
                {
                    start = DateTime.MinValue;
                }
                if (values[1] is not DateTime end)
                {
                    end = DateTime.MaxValue;
                }
                return (end - start).TotalMinutes;
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

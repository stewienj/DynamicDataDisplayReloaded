﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DynamicDataDisplay.Charts.Markers
{
	public class BarFromValueConverter : IValueConverter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BarFromValueConverter"/> class.
		/// </summary>
		public BarFromValueConverter() { }

		public Brush PositiveBrush { get; set; }
		public Brush NegativeBrush { get; set; }

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is double))
				return DependencyProperty.UnsetValue;

			double height = (double)value;

			if (targetType == typeof(Brush))
			{
				if (height > 0)
					return PositiveBrush;
				else
					return NegativeBrush;
			}
			else if (targetType == typeof(double))
			{
				return Math.Abs(height);
			}
			else if (targetType == typeof(VerticalAlignment))
			{
				return height > 0 ? VerticalAlignment.Bottom : VerticalAlignment.Top;
			}

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}

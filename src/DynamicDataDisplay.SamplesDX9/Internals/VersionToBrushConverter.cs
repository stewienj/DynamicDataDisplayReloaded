﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DynamicDataDisplay.SamplesDX9.Internals
{
	public sealed class VersionToBrushConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string version = value.ToString();

			switch (version)
			{
				case "0.2.0":
					return Brushes.Green;


				case "0.3.0":
					return Brushes.Orange;
				case "0.3.1":
					return Brushes.DarkMagenta;
				case "0.4.0":
					return Brushes.Blue;
				default:
					return Brushes.Gray;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}

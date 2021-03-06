﻿using DynamicDataDisplay.Charts;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace NewMarkersSample.Pages
{
	/// <summary>
	/// Interaction logic for DateTimeRectangles.xaml
	/// </summary>
	public partial class DateTimeRectanglesPage : Page
	{
		public DateTimeRectanglesPage()
		{
			InitializeComponent();
		}

		private Random rnd = new Random();
		private DateTime lastFinish = DateTime.Now;
		private readonly ObservableCollection<DateTimeRect> data = new ObservableCollection<DateTimeRect>();
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			const int count = 4;
			for (int i = 0; i < count; i++)
			{
				data.Add(CreateRandomRect());
			}

			chart.AddPropertyBinding<TimeSpan>(ViewportPanel.ViewportWidthProperty,
				duration =>
				{
					DateTime durationDT = new DateTime(duration.Ticks);
					HorizontalDateTimeAxis axis = (HorizontalDateTimeAxis)plotter.MainHorizontalAxis;
					var result = axis.ConvertToDouble(durationDT);

					return result;
				},
				"Duration");

			DataContext = data;
		}

		private DateTimeRect CreateRandomRect()
		{
			DateTimeRect rect = new DateTimeRect();
			rect.Start = lastFinish.AddMinutes(rnd.Next(2, 4));
			rect.Duration = TimeSpan.FromMinutes(rnd.Next(4, 8));
			rect.Y = rnd.NextDouble();
			rect.Height = 0.5 + rnd.NextDouble();

			lastFinish = rect.Start + rect.Duration;

			return rect;
		}
	}
}

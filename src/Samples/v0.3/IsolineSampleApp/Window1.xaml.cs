﻿using DynamicDataDisplay.Common.Auxiliary;
using DynamicDataDisplay.DataSources.MultiDimensional;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;

namespace IsolineSampleApp
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			Loaded += new RoutedEventHandler(Window1_Loaded);
		}

		private void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			LoadField();
		}

		private static string[] ParseDataString(string str)
		{
			str = str.TrimEnd(' ');
			return str.Split(' ');
		}

		private static string[] ParseGridString(string str)
		{
			return str.TrimEnd(' ').
				Substring(0, str.Length - 3).
				TrimStart('{').
				Split(new string[] { " } {" }, StringSplitOptions.None);
		}

		private void LoadField()
		{
			var assembly = Assembly.GetExecutingAssembly();

			List<string> strings = new List<string>();
			using (Stream stream = assembly.GetManifestResourceStream("IsolineSampleApp.SampleData.txt"))
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					while (!reader.EndOfStream)
					{
						string str = reader.ReadLine();
						if (str == "Data:")
						{
							// do nothing
						}
						else if (str == "Grid:")
						{
							// do nothing too
						}
						else
						{
							strings.Add(str);
						}
					}
				}
			}

			// data
			string[] nums = ParseDataString(strings[0]);
			int width = nums.Length;
			int height = strings.Count / 2;

			CultureInfo culture = new CultureInfo("ru-RU");

			double[,] data = new double[width, height];
			for (int row = 0; row < height; row++)
			{
				nums = ParseDataString(strings[row]);
				for (int column = 0; column < width; column++)
				{
					double d = double.Parse(nums[column], culture);
					data[column, row] = d;
				}
			}

			Point[,] gridData = new Point[width, height];
			for (int row = 0; row < height; row++)
			{
				string str = strings[row + height];
				nums = ParseGridString(str);
				for (int column = 0; column < width; column++)
				{
					string[] vecStrs = nums[column].Split(new string[] { "; " }, StringSplitOptions.None);
					gridData[column, row] = new Point(
						double.Parse(vecStrs[0], culture),
						double.Parse(vecStrs[1], culture));
				}
			}

			WarpedDataSource2D<double> dataSource = new WarpedDataSource2D<double>(data, gridData);
			isolineGraph.DataSource = dataSource;
			trackingGraph.DataSource = dataSource;

			Rect visible = dataSource.GetGridBounds();
			plotter.Viewport.Visible = visible;
		}
	}
}

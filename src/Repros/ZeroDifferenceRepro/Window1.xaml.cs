#define old

using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DynamicDataDisplay;
using DynamicDataDisplay.Common.Auxiliary;

#if old
using DynamicDataDisplay.DataSources;
using DynamicDataDisplay.Charts;
#else
#if !old
using DynamicDataDisplay.Charts.NewLine;
#endif
using DynamicDataDisplay.Charts;
using DynamicDataDisplay.Charts.Axes.Numeric;
#if !old
using DynamicDataDisplay.Charts.NewLine.Functional;
using DynamicDataDisplay.Charts.NewLine.Filters;
#endif
#endif

namespace ZeroDifferenceRepro
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

		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			plotter.Children.Add(new HorizontalLine { Value = 0, Stroke = Brushes.DarkGreen.MakeTransparent(0.2) });
			plotter.Children.Add(new VerticalLine { Value = 0, Stroke = Brushes.DarkGreen.MakeTransparent(0.2) });

#if old
			var xs = Enumerable.Range(0, 500).Select(i => (i - 250) * 0.02);
			var sineYDS = xs.Select(x => Math.Sin(x)).AsYDataSource();
			var atanYDS = xs.Select(x => Math.Atan(x)).AsYDataSource();

			var sineDS = new CompositeDataSource(xs.AsXDataSource(), sineYDS);
			var atanDS = new CompositeDataSource(xs.AsXDataSource(), atanYDS);

			var sineChart = plotter.AddLineGraph(sineDS);
			var atanChart = plotter.AddLineGraph(atanDS);

			//sineChart.Filters.Clear();
			//atanChart.Filters.Clear();
#else
			var xs = Enumerable.Range(0, 500).Select(i => (i - 250) * 0.02);
			var sineDS = xs.Select(x => new Point(x, Math.Sin(x))).AsDataSource();
			var atanDS = xs.Select(x => new Point(x, Math.Atan(x))).AsDataSource();
			var sincDS = Enumerable.Range(-5000, 10001).Select(i =>
			{
				double x = Math.PI * i / 1000;
				double y;
				if (i == 0)
					y = 100;
				else
					y = Math.Sin(x * 100);
				return new Point(x, y);
			}).AsDataSource();

			LineChart sincChart = new LineChart { Stroke = ColorHelper.RandomBrush, DataSource = sincDS };
			//plotter.Children.Add(sincChart);


			LineChart sineChart = new LineChart { Stroke = ColorHelper.RandomBrush, DataSource = sineDS };
			plotter.Children.Add(sineChart);
			LineChart atanChart = new LineChart { Stroke = ColorHelper.RandomBrush, DataSource = atanDS };
			plotter.Children.Add(atanChart);
#endif
		}
	}
}

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CustomLegendPosition
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			int chartsCount = 60;

			var xs = Enumerable.Range(0, 100).Select(x => x * 0.5);
			var xDS = xs.AsXDataSource();
			for (int i = 0; i < chartsCount; i++)
			{
				int itemp = i;
				var ys = xs.Select(x => itemp * Math.Sin(x));
				var yDS = ys.AsYDataSource();

				var ds = xDS.Join(yDS);
				plotter.AddLineGraph(ds, "Chart " + (i + 1));
			}

			Panel legendParent = (Panel)plotter.Legend.ContentGrid.Parent;
			legendParent.Children.Remove(plotter.Legend.ContentGrid);
			legendHolder.Content = plotter.Legend.ContentGrid;
		}
	}
}

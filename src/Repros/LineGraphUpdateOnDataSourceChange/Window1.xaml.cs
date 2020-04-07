using DynamicDataDisplay.DataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LineGraphUpdateOnDataSourceChange
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
			var xs = Enumerable.Range(0, 100).Select(i => i * 0.1);

			var xDS = xs.AsXDataSource();

			var yDS1 = xs.Select(x => Math.Sin(x)).AsYDataSource();
			var yDS2 = xs.Select(x => Math.Cos(x) + 2).AsYDataSource();

			var ds1 = xDS.Join(yDS1);
			var ds2 = xDS.Join(yDS2);

			var list = new List<IPointDataSource>();
			list.Add(ds1);
			list.Add(ds2);

			DataContext = list;
		}
	}
}

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Simplest
{
	/// <summary>Interaction logic for simplest plot application</summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			Loaded += new RoutedEventHandler(MainWindow_Loaded);
		}

		void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			// Prepare data in arrays
			const int N = 1000;
			double[] x = new double[N];
			double[] y = new double[N];

			for (int i = 0; i < N; i++)
			{
				x[i] = i * 0.1;
				y[i] = Math.Sin(x[i]);
			}

			// Create data sources:
			var xDataSource = x.AsXDataSource();
			var yDataSource = y.AsYDataSource();

			CompositeDataSource compositeDataSource = xDataSource.Join(yDataSource);
			// adding graph to plotter
			plotter.AddLineGraph(compositeDataSource,
				Colors.Goldenrod,
				3,
				"Sine");

			// Force evertyhing plotted to be visible
			plotter.FitToView();
		}
	}
}

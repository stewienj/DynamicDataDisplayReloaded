﻿using DynamicDataDisplay.DataSources;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace DynamicDataDisplay.Samples.Demos.v03
{
	/// <summary>
	/// Interaction logic for SimulationSample.xaml
	/// </summary>
	public partial class SimulationSample : Page
	{
        private volatile bool _stop = false;

		// Three observable data sources. Observable data source contains
		// inside ObservableCollection. Modification of collection instantly modify
		// visual representation of graph. 
		ObservableDataSource<Point> source1 = null;
		ObservableDataSource<Point> source2 = null;
		ObservableDataSource<Point> source3 = null;
		public SimulationSample()
		{
			InitializeComponent();
		}

		private void Simulation()
		{
			CultureInfo culture = CultureInfo.InvariantCulture;
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			// load spim-generated data from embedded resource file
			const string spimDataName = "DynamicDataDisplay.Samples.Demos.v03.Repressilator.txt";
			using (Stream spimStream = executingAssembly.GetManifestResourceStream(spimDataName))
			{
				using (StreamReader r = new StreamReader(spimStream))
				{
					string line = r.ReadLine();
					while (!r.EndOfStream && !_stop)
					{
						line = r.ReadLine();
						string[] values = line.Split(',');

						double x = double.Parse(values[0], culture);
						double y1 = double.Parse(values[1], culture);
						double y2 = double.Parse(values[2], culture);
						double y3 = double.Parse(values[3], culture);

						Point p1 = new Point(x, y1);
						Point p2 = new Point(x, y2);
						Point p3 = new Point(x, y3);

						source1.AppendAsync(Dispatcher, p1);
						source2.AppendAsync(Dispatcher, p2);
						source3.AppendAsync(Dispatcher, p3);

						Thread.Sleep(10); // Long-long time for computations...
					}
				}
			}
		}

		private Thread simThread;
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Create first source
			source1 = new ObservableDataSource<Point>();
			// Set identity mapping of point in collection to point on plot
			source1.SetXYMapping(p => p);

			// Create second source
			source2 = new ObservableDataSource<Point>();
			// Set identity mapping of point in collection to point on plot
			source2.SetXYMapping(p => p);

			// Create third source
			source3 = new ObservableDataSource<Point>();
			// Set identity mapping of point in collection to point on plot
			source3.SetXYMapping(p => p);

			// Add all three graphs. Colors are not specified and chosen random
			plotter.AddLineGraph(source1, 2, "Data row 1");
			plotter.AddLineGraph(source2, 2, "Data row 2");
			plotter.AddLineGraph(source3, 2, "Data row 3");

			// Start computation process in second thread
			simThread = new Thread(new ThreadStart(Simulation));
			simThread.IsBackground = true;
			simThread.Start();
		}

		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
            _stop = true;
#if !NET5_0_OR_GREATER
            simThread.Abort();
#endif
		}
	}
}
